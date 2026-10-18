Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DifferentialFunction = DeepLearning4VB.org.nd4vb.autodiff.functions.DifferentialFunction
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArrayStatistics
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports Aggregate = DeepLearning4VB.org.nd4vb.linalg.api.ops.aggregates.Aggregate
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.aggregates
Imports Variance = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.summarystats.Variance
Imports RandomT = DeepLearning4VB.org.nd4vb.linalg.api.rng.Random
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports TadPack = DeepLearning4VB.org.nd4vb.linalg.api.shape.TadPack
Imports TADManagerT = DeepLearning4VB.org.nd4vb.linalg.cache.TADManager
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports OpProfiler = DeepLearning4VB.org.nd4vb.linalg.profiler.OpProfiler
Imports ProfilerConfig = DeepLearning4VB.org.nd4vb.linalg.profiler.ProfilerConfig
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports Properties = DeepLearning4VB.JavaCompat.Properties

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.ops.executioner

    ''' <summary>
    ''' The base class of the op executioners (Java <c>DefaultOpExecutioner</c>): the profiling hooks (through the
    ''' process-wide <see cref="OpProfiler"/>), the workspace scope checks, the data type validation helpers, the
    ''' <c>x</c>/<c>y</c>/<c>z</c> accessors that read an <see cref="OpContext"/> when one is given, and default
    ''' implementations that throw <see cref="NotSupportedException"/> for what a backend must provide.
    ''' <para>
    ''' Differences from Java:
    ''' <list type="bullet">
    ''' <item>The fields <c>profilingMode</c>, <c>verbose</c> and <c>debug</c> are named <see cref="profilingModeField"/>,
    ''' <see cref="verboseField"/> and <see cref="debugField"/>; the deprecated profiling mode accessors stay the methods
    ''' <see cref="getProfilingMode"/> and <see cref="setProfilingMode"/>.</item>
    ''' <item>The verbose checks of the profiling hooks read this executioner's <see cref="Verbose"/> flag (Java read
    ''' <c>Nd4j.getExecutioner().isVerbose()</c>, the same object in practice). The shared
    ''' <see cref="validateDataType(DataType, Op)"/> still reads <c>Nd4vb.Executioner</c>.</item>
    ''' <item><see cref="thresholdEncode(INDArray, Double, Integer?)"/>, <see cref="thresholdDecode"/>,
    ''' <see cref="bitmapEncode(INDArray, INDArray, Double)"/>, <see cref="bitmapEncode(INDArray, Double)"/> and
    ''' <see cref="bitmapDecode"/> throw <see cref="NotSupportedException"/>: Java ran the compression custom ops
    ''' (<c>EncodeThreshold</c>, <c>DecodeThreshold</c>, <c>EncodeBitmap</c>, <c>DecodeBitmap</c>), which the managed
    ''' backend does not implement yet (later work).</item>
    ''' <item><see cref="exec(Variance)"/> and <see cref="execAndReturn(Variance)"/> are class members only (the ported
    ''' <see cref="OpExecutioner"/> interface left them out); they run the op as the <see cref="ReduceOp"/> it is.</item>
    ''' <item><see cref="opInfoString"/> formats the extra arguments itself (Java used <c>Preconditions.formatArray</c>).</item>
    ''' <item>With <c>nativeStatistics</c> profiling the z statistics are logged but not stored in <see cref="OpProfiler"/>
    ''' (the ported <c>OpProfiler</c> has no <c>setStatistics</c>).</item>
    ''' <item><see cref="EnvironmentInformation"/> stores numbers as invariant strings (<c>JavaCompat.Properties</c> holds
    ''' strings) and reports the managed heap: the memory available to the process (.NET 10: the GC's
    ''' <c>TotalAvailableMemoryBytes</c>; .NET Framework: the physical memory) for Java's <c>Runtime.maxMemory()</c>.</item>
    ''' </list>
    ''' Native-only members not ported: <c>registerGraph(long, Pointer)</c> and <c>scatterUpdate(...)</c>.
    ''' </para>
    ''' </summary>
    Public MustInherit Class DefaultOpExecutioner
        Implements OpExecutioner

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DefaultOpExecutioner))

        Private Const SCOPE_PANIC_MSG As String = "For more details, see the ND4J User Guide: https://deeplearning4j.konduit.ai/nd4j/overview#workspaces-scope-panic"

        ' the keys of Java's Nd4jEnvironment (org.nd4j.linalg.api.environment.Nd4jEnvironment, not ported yet)
        Friend Const CPU_CORES_KEY As String = "cores"
        Friend Const HOST_TOTAL_MEMORY_KEY As String = "memory.available"
        Friend Const HOST_FREE_MEMORY_KEY As String = "memory.free"
        Friend Const OS_KEY As String = "os"
        Friend Const BACKEND_KEY As String = "backend"
        Friend Const BLAS_VENDOR_KEY As String = "blas.vendor"
        Friend Const OMP_THREADS_KEY As String = "omp.threads"
        Friend Const BLAS_THREADS_KEY As String = "blas.threads"
        Friend Const MEMORY_BANDWIDTH_KEY As String = "memoryBandwidth"

        ''' <summary>The deprecated profiling mode (Java field <c>profilingMode</c>).</summary>
        Protected Friend profilingModeField As OpExecutioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC

        ''' <summary>The verbose flag (Java field <c>verbose</c>).</summary>
        Protected Friend verboseField As New AtomicBoolean(False)
        ''' <summary>The debug flag (Java field <c>debug</c>).</summary>
        Protected Friend debugField As New AtomicBoolean(False)

        Public Sub New()
        End Sub

        ''' <summary>Decompresses compressed operands of the op (after <see cref="interceptIntDataType"/>).</summary>
        Protected Friend Overridable Sub checkForCompression(ByVal op As Op)
            ' check for INT datatype arrays
            interceptIntDataType(op)

            If op.x() IsNot Nothing AndAlso op.x().Compressed Then
                Nd4vbT.Compressor.decompressi(op.x())
            End If

            If op.y() IsNot Nothing AndAlso op.y().Compressed Then
                Nd4vbT.Compressor.decompressi(op.y())
            End If

            If op.z() IsNot Nothing AndAlso op.z().Compressed Then
                Nd4vbT.Compressor.decompressi(op.z())
            End If
        End Sub

        ''' <summary>The name of the last op this executioner ran ("UNKNOWN" here).</summary>
        Public Overridable ReadOnly Property LastOp As String Implements OpExecutioner.LastOp
            Get
                Return "UNKNOWN"
            End Get
        End Property

        ''' <summary>
        ''' This method checks if any Op operand has data opType of INT, and throws exception if any (a no-op, as in
        ''' Java, which commented the check out).
        ''' </summary>
        ''' <param name="op"> </param>
        Protected Friend Overridable Sub interceptIntDataType(ByVal op As Op)
            ' FIXME: Remove this method, after we'll add support for <int> dtype operations
        End Sub

        Public MustOverride Function exec(ByVal op As Op) As INDArray Implements OpExecutioner.exec

        Public MustOverride Function exec(ByVal op As Op, ByVal opContext As OpContext) As INDArray Implements OpExecutioner.exec

        ''' <summary>Executes a transform, scalar, reduce or index-reduce op and returns it.</summary>
        ''' <exception cref="ArgumentException">The op is of another kind.</exception>
        Public Overridable Function execAndReturn(ByVal op As Op) As Op Implements OpExecutioner.execAndReturn
            If TypeOf op Is TransformOp Then
                Return execAndReturn(DirectCast(op, TransformOp))
            End If
            If TypeOf op Is ScalarOp Then
                Return execAndReturn(DirectCast(op, ScalarOp))
            End If
            If TypeOf op Is ReduceOp Then
                exec(DirectCast(op, ReduceOp))
                Return op
            End If
            If TypeOf op Is IndexAccumulation Then
                exec(DirectCast(op, IndexAccumulation))
                Return op
            End If

            Throw New ArgumentException("Illegal opType of op: " & If(op Is Nothing, "null", CObj(op).GetType().FullName))
        End Function

        Public Overridable Function execAndReturn(ByVal op As TransformOp) As TransformOp Implements OpExecutioner.execAndReturn
            exec(op)
            Return op
        End Function

        Public Overridable Function execAndReturn(ByVal op As ReduceOp) As ReduceOp Implements OpExecutioner.execAndReturn
            exec(op)
            Return op
        End Function

        ''' <summary>Executes a variance (or standard deviation) op and returns it (Java <c>execAndReturn(Variance)</c>).</summary>
        Public Overridable Function execAndReturn(ByVal op As Variance) As Variance
            exec(op)
            Return op
        End Function

        Public Overridable Function execAndReturn(ByVal op As ScalarOp) As ScalarOp Implements OpExecutioner.execAndReturn
            exec(op)
            Return op
        End Function

        Public Overridable Function execAndReturn(ByVal op As IndexAccumulation) As IndexAccumulation Implements OpExecutioner.execAndReturn
            exec(op)
            Return op
        End Function

        Public Overridable Function execAndReturn(ByVal op As BroadcastOp) As BroadcastOp Implements OpExecutioner.execAndReturn
            exec(op)
            Return op
        End Function

        ''' <summary>Executes a custom op and returns its output arrays.</summary>
        Public Overridable Function exec(ByVal op As CustomOp) As INDArray() Implements OpExecutioner.exec
            Return New List(Of INDArray)(execAndReturn(op).outputArguments()).ToArray()
        End Function

        Public MustOverride Function exec(ByVal op As ReduceOp) As INDArray Implements OpExecutioner.exec

        ''' <summary>Executes a variance (or standard deviation) op as the <see cref="ReduceOp"/> it is (Java <c>exec(Variance)</c>).</summary>
        Public Overridable Function exec(ByVal accumulation As Variance) As INDArray
            Return exec(DirectCast(accumulation, ReduceOp))
        End Function

        Public MustOverride Function exec(ByVal op As IndexAccumulation) As INDArray Implements OpExecutioner.exec

        Public MustOverride Function exec(ByVal broadcast As BroadcastOp) As INDArray Implements OpExecutioner.exec

        Public Overridable Sub exec(ByVal op As MetaOp) Implements OpExecutioner.exec
            Throw New NotSupportedException("MetaOp execution isn't supported for this OpExecutioner yet")
        End Sub

        Public Overridable Sub exec(ByVal op As GridOp) Implements OpExecutioner.exec
            Throw New NotSupportedException("GridOp execution isn't supported for this OpExecutioner yet")
        End Sub

        Public Overridable Sub exec(Of T As Aggregate)(ByVal batch As Batch(Of T)) Implements OpExecutioner.exec
            Throw New NotSupportedException()
        End Sub

        Public Overridable Sub exec(ByVal op As Aggregate) Implements OpExecutioner.exec
            Throw New NotSupportedException()
        End Sub

        Public MustOverride Function exec(ByVal op As ScalarOp) As INDArray Implements OpExecutioner.exec

        Public Overridable Sub exec(ByVal batch As IList(Of Aggregate)) Implements OpExecutioner.exec
            Throw New NotSupportedException()
        End Sub

        ''' <summary>
        ''' This method executes specified RandomOp using default RNG available via <c>Nd4vb.Random</c>
        ''' </summary>
        ''' <param name="op"> </param>
        Public Overridable Function exec(ByVal op As RandomOp) As INDArray Implements OpExecutioner.exec
            Return exec(op, Nd4vbT.Random)
        End Function

        ''' <summary>
        ''' This method executes specific RandomOp against specified RNG
        ''' </summary>
        ''' <param name="op"> </param>
        ''' <param name="rng"> </param>
        Public MustOverride Function exec(ByVal op As RandomOp, ByVal rng As RandomT) As INDArray Implements OpExecutioner.exec

        ''' <summary>
        ''' Sets the deprecated profiling mode and the matching <see cref="OpProfiler"/> configuration.
        ''' </summary>
        <Obsolete("Use ProfilingConfig")>
        Public Overridable Sub setProfilingMode(ByVal mode As OpExecutioner.ProfilingMode) Implements OpExecutioner.setProfilingMode
            profilingModeField = mode
            Dim config As ProfilerConfig
            Select Case profilingModeField
                Case OpExecutioner.ProfilingMode.ALL
                    config = ProfilerConfig.builder().checkWorkspaces(True).checkElapsedTime(True).stackTrace(True).build()
                Case OpExecutioner.ProfilingMode.METHODS
                    config = ProfilerConfig.builder().stackTrace(True).build()
                Case OpExecutioner.ProfilingMode.OPERATIONS
                    config = ProfilerConfig.builder().stackTrace(True).checkElapsedTime(True).build()
                Case OpExecutioner.ProfilingMode.SCOPE_PANIC
                    config = ProfilerConfig.builder().checkWorkspaces(True).build()
                Case OpExecutioner.ProfilingMode.ANY_PANIC
                    config = ProfilerConfig.builder().checkForINF(True).checkForNAN(True).build()
                Case OpExecutioner.ProfilingMode.INF_PANIC
                    config = ProfilerConfig.builder().checkForINF(True).build()
                Case OpExecutioner.ProfilingMode.NAN_PANIC
                    config = ProfilerConfig.builder().checkForNAN(True).build()
                Case Else
                    config = ProfilerConfig.builder().build()
            End Select
            OpProfiler.Instance.Config = config
        End Sub

        ''' <summary>The deprecated profiling mode.</summary>
        <Obsolete("Use ProfilingConfig")>
        Public Overridable Function getProfilingMode() As OpExecutioner.ProfilingMode Implements OpExecutioner.getProfilingMode
            Return profilingModeField
        End Function

        ''' <summary>Stores the profiler configuration in <see cref="OpProfiler"/>.</summary>
        Public Overridable WriteOnly Property ProfilingConfig As ProfilerConfig Implements OpExecutioner.ProfilingConfig
            Set(ByVal config As ProfilerConfig)
                OpProfiler.Instance.Config = config
            End Set
        End Property

        ''' <summary>Throws when an attached array's workspace is closed or was reopened since the array was created.</summary>
        Protected Friend Overridable Sub checkWorkspace(ByVal opName As String, ByVal array As INDArray)
            If array.Attached Then
                Dim ws As MemoryWorkspace = array.data().ParentWorkspace

                If ws.WorkspaceType <> MemoryWorkspace.Type.CIRCULAR Then

                    If Not ws.ScopeActive Then
                        Throw New ND4VBIllegalStateException("Op [" & opName & "] X argument uses leaked workspace pointer from workspace [" & ws.Id & "]: Workspace the array was defined in is no longer open." & vbLf & "All open workspaces: " & listToString(allOpenWorkspaces()) & vbLf & SCOPE_PANIC_MSG)
                    End If

                    If ws.GenerationId <> array.data().GenerationId Then
                        Throw New ND4VBIllegalStateException("Op [" & opName & "] X argument uses outdated workspace pointer from workspace [" & ws.Id & "]: Workspace array was defined in has been closed and reopened at least once since array creation. Array WS iteration: " & array.data().GenerationId & ". Workspace current iteration: " & ws.GenerationId & vbLf & "All open workspaces: " & listToString(allOpenWorkspaces()) & vbLf & SCOPE_PANIC_MSG)
                    End If
                End If
            End If
        End Sub

        ''' <summary>Checks the workspaces of the inputs and outputs of a custom op (from the context when one is given).</summary>
        Protected Friend Overridable Sub checkForWorkspaces(ByVal op As CustomOp, ByVal oc As OpContext)
            Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
            Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

            For Each input As INDArray In inArgs
                checkWorkspace(op.opName(), input)
            Next input

            For Each output As INDArray In outArgs
                checkWorkspace(op.opName(), output)
            Next output
        End Sub

        ''' <summary>Checks the workspaces of x, y and z (from the context when one is given).</summary>
        Protected Friend Overridable Sub checkForWorkspaces(ByVal op As Op, ByVal oc As OpContext)
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), op.x())
            If x IsNot Nothing Then
                checkWorkspace(op.opName(), x)
            End If

            Dim y As INDArray = If(oc IsNot Nothing AndAlso oc.getInputArrays().Count > 1, oc.getInputArray(1), op.y())
            If y IsNot Nothing Then
                checkWorkspace(op.opName(), y)
            End If

            Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), op.z())
            If z IsNot Nothing Then
                checkWorkspace(op.opName(), z)
            End If
        End Sub

        ''' <summary>The ids of the workspaces of the current thread whose scope is open.</summary>
        Public Shared Function allOpenWorkspaces() As IList(Of String)
            Dim l As IList(Of MemoryWorkspace) = Nd4vbT.WorkspaceManager.AllWorkspacesForCurrentThread
            Dim workspaces As IList(Of String) = New List(Of String)(l.Count)
            For Each ws As MemoryWorkspace In l
                If ws.ScopeActive Then
                    workspaces.Add(ws.Id)
                End If
            Next ws
            Return workspaces
        End Function

        Private Shared Function listToString(ByVal values As IList(Of String)) As String
            Return "[" & String.Join(", ", values) & "]"
        End Function

        ''' <summary>The deprecated profiling hook before a legacy op (follows <see cref="getProfilingMode"/>).</summary>
        <Obsolete("Use profilingConfigurableHookIn")>
        Public Overridable Function profilingHookIn(ByVal op As Op, ParamArray ByVal tadBuffers() As DataBuffer) As Long
            Select Case profilingModeField
                Case OpExecutioner.ProfilingMode.ALL
                    OpProfiler.Instance.processOpCall(op, tadBuffers)
                Case OpExecutioner.ProfilingMode.METHODS
                Case OpExecutioner.ProfilingMode.OPERATIONS
                    OpProfiler.Instance.processOpCall(op, tadBuffers)
                Case OpExecutioner.ProfilingMode.SCOPE_PANIC
                    checkForWorkspaces(op, Nothing)
                    Return 0L
                Case Else
                    Return 0L
            End Select

            Return JavaSystem.nanoTime()
        End Function

        ''' <summary>The deprecated profiling hook before a custom op (follows <see cref="getProfilingMode"/>).</summary>
        <Obsolete("Use profilingConfigurableHookIn")>
        Public Overridable Function profilingHookIn(ByVal op As CustomOp, ByVal oc As OpContext) As Long
            Select Case profilingModeField
                Case OpExecutioner.ProfilingMode.ALL
                    OpProfiler.Instance.processOpCall(op)
                Case OpExecutioner.ProfilingMode.METHODS
                Case OpExecutioner.ProfilingMode.OPERATIONS
                    OpProfiler.Instance.processOpCall(op)
                Case OpExecutioner.ProfilingMode.SCOPE_PANIC
                    checkForWorkspaces(op, oc)
                    Return 0L
                Case Else
                    Return 0L
            End Select

            Return JavaSystem.nanoTime()
        End Function

        ''' <summary>The deprecated profiling hook after a legacy op (follows <see cref="getProfilingMode"/>).</summary>
        <Obsolete("Use profilingConfigurableHookOut")>
        Public Overridable Sub profilingHookOut(ByVal op As Op, ByVal oc As OpContext, ByVal timeStart As Long)
            Select Case profilingModeField
                Case OpExecutioner.ProfilingMode.ALL
                    OpProfiler.Instance.processStackCall(op, timeStart)
                    OpProfiler.Instance.timeOpCall(op, timeStart)
                Case OpExecutioner.ProfilingMode.METHODS
                    OpProfiler.Instance.processStackCall(op, timeStart)
                Case OpExecutioner.ProfilingMode.OPERATIONS
                    OpProfiler.Instance.timeOpCall(op, timeStart)
                Case OpExecutioner.ProfilingMode.NAN_PANIC
                    OpExecutionerUtil.checkForNaN(op, oc)
                Case OpExecutioner.ProfilingMode.INF_PANIC
                    OpExecutionerUtil.checkForInf(op, oc)
                Case OpExecutioner.ProfilingMode.ANY_PANIC
                    OpExecutionerUtil.checkForNaN(op, oc)
                    OpExecutionerUtil.checkForInf(op, oc)
                Case Else
            End Select

            If Verbose Then
                If op.z() IsNot Nothing Then
                    log.info("Op name: {}; Z shapeInfo: {}; Z values: {}", op.opName(), ArrayToString(op.z().shapeInfoJava()), firstX(op.z(), 10))
                End If
            End If
        End Sub

        ''' <summary>The deprecated profiling hook after a custom op (follows <see cref="getProfilingMode"/>).</summary>
        <Obsolete("Use profilingConfigurableHookOut")>
        Public Overridable Sub profilingHookOut(ByVal op As CustomOp, ByVal oc As OpContext, ByVal timeStart As Long)
            Select Case profilingModeField
                Case OpExecutioner.ProfilingMode.ALL
                    OpProfiler.Instance.processStackCall(op, timeStart)
                    OpProfiler.Instance.timeOpCall(op, timeStart)
                Case OpExecutioner.ProfilingMode.METHODS
                    OpProfiler.Instance.processStackCall(op, timeStart)
                Case OpExecutioner.ProfilingMode.OPERATIONS
                    OpProfiler.Instance.timeOpCall(op, timeStart)
                Case OpExecutioner.ProfilingMode.NAN_PANIC
                    OpExecutionerUtil.checkForNaN(op, oc)
                Case OpExecutioner.ProfilingMode.INF_PANIC
                    OpExecutionerUtil.checkForInf(op, oc)
                Case OpExecutioner.ProfilingMode.ANY_PANIC
                    OpExecutionerUtil.checkForNaN(op, oc)
                    OpExecutionerUtil.checkForInf(op, oc)
                Case Else
            End Select
        End Sub

        ''' <summary>
        ''' The profiling hook before a custom op: throws when an input or output was closed, then records the call and
        ''' checks the workspaces as the <see cref="OpProfiler"/> configuration asks. Returns the start time
        ''' (nanoseconds).
        ''' </summary>
        Public Overridable Function profilingConfigurableHookIn(ByVal op As CustomOp, ByVal oc As OpContext) As Long
            Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
            Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

            For Each arr As INDArray In inArgs
                If arr.wasClosed() Then
                    Throw New InvalidOperationException("One of Input arguments was closed before call")
                End If
            Next arr

            For Each arr As INDArray In outArgs
                If arr.wasClosed() Then
                    Throw New InvalidOperationException("One of Output arguments was closed before call")
                End If
            Next arr

            Dim config As ProfilerConfig = OpProfiler.Instance.Config
            If config Is Nothing Then
                Return JavaSystem.nanoTime()
            End If

            If config.isStackTrace() OrElse config.isCheckElapsedTime() Then
                OpProfiler.Instance.processOpCall(op)
            End If

            If config.isCheckWorkspaces() Then
                checkForWorkspaces(op, oc)
            End If

            Return JavaSystem.nanoTime()
        End Function

        ''' <summary>
        ''' The profiling hook before a legacy op: throws when x, y or z was closed, then records the call (and the TAD
        ''' buffers) and checks the workspaces as the <see cref="OpProfiler"/> configuration asks. Returns the start time
        ''' (nanoseconds).
        ''' </summary>
        Public Overridable Function profilingConfigurableHookIn(ByVal op As Op, ParamArray ByVal tadBuffers() As DataBuffer) As Long
            If op.x() IsNot Nothing Then
                If op.x().wasClosed() Then
                    Throw New InvalidOperationException("Op.X argument was closed before call")
                End If
            End If

            If op.y() IsNot Nothing Then
                If op.y().wasClosed() Then
                    Throw New InvalidOperationException("Op.Y argument was closed before call")
                End If
            End If

            If op.z() IsNot Nothing Then
                If op.z().wasClosed() Then
                    Throw New InvalidOperationException("Op.Z argument was closed before call")
                End If
            End If

            Dim config As ProfilerConfig = OpProfiler.Instance.Config
            If config Is Nothing Then
                Return JavaSystem.nanoTime()
            End If

            If config.isStackTrace() OrElse config.isCheckElapsedTime() Then
                OpProfiler.Instance.processOpCall(op)
            End If

            If config.isNotOptimalTAD() Then
                OpProfiler.Instance.processOpCall(op, tadBuffers)
            End If
            If config.isCheckWorkspaces() Then
                checkForWorkspaces(op, Nothing)
            End If

            Return JavaSystem.nanoTime()
        End Function

        ''' <summary>
        ''' The profiling hook after a legacy op: stack and timing records, NaN/Inf panics and z statistics as the
        ''' <see cref="OpProfiler"/> configuration asks, and the z values in verbose mode.
        ''' </summary>
        Public Overridable Sub profilingConfigurableHookOut(ByVal op As Op, ByVal oc As OpContext, ByVal timeStart As Long)
            Dim config As ProfilerConfig = OpProfiler.Instance.Config
            If config Is Nothing Then
                Return
            End If

            If config.isStackTrace() Then
                OpProfiler.Instance.processStackCall(op, timeStart)
            End If
            If config.isCheckElapsedTime() Then
                OpProfiler.Instance.timeOpCall(op, timeStart)
            End If
            If config.isCheckForNAN() Then
                OpExecutionerUtil.checkForNaN(op, oc)
            End If
            If config.isCheckForINF() Then
                OpExecutionerUtil.checkForInf(op, oc)
            End If
            If config.isNativeStatistics() Then
                If op.z() IsNot Nothing Then
                    Dim stat As INDArrayStatistics = inspectArray(op.z())
                    ' Java also stored the statistics with OpProfiler.setStatistics, which the ported OpProfiler lacks
                    log.info("Op name: {}; Z shapeInfo: {}; Statistics: min:{} max:{} mean:{} stdev:{} pos:{}, neg:{} zero:{} inf:{} nan:{}", op.opName(), ArrayToString(op.z().shapeInfoJava()), stat.getMinValue(), stat.getMaxValue(), stat.getMeanValue(), stat.getStdDevValue(), stat.getCountPositive(), stat.getCountNegative(), stat.getCountZero(), stat.getCountInf(), stat.getCountNaN())
                End If
            End If

            If Verbose Then
                If op.z() IsNot Nothing Then
                    log.info("Op name: {}; Z shapeInfo: {}; Z values: {}", op.opName(), ArrayToString(op.z().shapeInfoJava()), firstX(op.z(), 10))
                End If
            End If
        End Sub

        ''' <summary>
        ''' The profiling hook after a custom op: stack and timing records and NaN/Inf panics as the
        ''' <see cref="OpProfiler"/> configuration asks.
        ''' </summary>
        Public Overridable Sub profilingConfigurableHookOut(ByVal op As CustomOp, ByVal oc As OpContext, ByVal timeStart As Long)
            Dim config As ProfilerConfig = OpProfiler.Instance.Config
            If config Is Nothing Then
                Return
            End If

            If config.isStackTrace() Then
                OpProfiler.Instance.processStackCall(op, timeStart)
            End If
            If config.isCheckElapsedTime() Then
                OpProfiler.Instance.timeOpCall(op, timeStart)
            End If
            If config.isCheckForNAN() Then
                OpExecutionerUtil.checkForNaN(op, oc)
            End If
            If config.isCheckForINF() Then
                OpExecutionerUtil.checkForInf(op, oc)
            End If
        End Sub

        ''' <summary>
        ''' Validate the data types
        ''' for the given operation: compressed operands are decompressed, and y must have
        ''' <paramref name="expectedType"/>. </summary>
        ''' <param name="expectedType"> </param>
        ''' <param name="op"> </param>
        ''' <exception cref="ND4VBIllegalStateException">y has another data type.</exception>
        Public Shared Sub validateDataType(ByVal expectedType As DataType, ByVal op As Op)
            If op.x() IsNot Nothing AndAlso Not ShapeT.isEmpty(op.x().shapeInfoJava()) AndAlso op.x().data().dataType() = DataType.COMPRESSED Then
                Nd4vbT.Compressor.decompressi(op.x())
            End If

            If op.y() IsNot Nothing AndAlso Not ShapeT.isEmpty(op.y().shapeInfoJava()) AndAlso op.y().data().dataType() = DataType.COMPRESSED Then
                Nd4vbT.Compressor.decompressi(op.y())
            End If

            If op.z() IsNot Nothing AndAlso Not ShapeT.isEmpty(op.z().shapeInfoJava()) AndAlso op.z().data().dataType() = DataType.COMPRESSED Then
                Nd4vbT.Compressor.decompressi(op.z())
            End If

            If op.y() IsNot Nothing AndAlso Not ShapeT.isEmpty(op.y().shapeInfoJava()) AndAlso op.y().data().dataType() <> expectedType Then
                Throw New ND4VBIllegalStateException("op.Y dataType is [" & op.y().data().dataType().ToString() & "] instead of expected [" & expectedType.ToString() & "] - x.shape = " & ArrayToString(op.x().shape()) & (If(op.y() IsNot Nothing, ", y.shape=" & ArrayToString(op.y().shape()), "")) & ", z.shape=" & ArrayToString(op.z().shape()) & " - op: " & CObj(op).GetType().FullName)
            End If

            If Nd4vbT.Executioner.Verbose Then
                log.info("Reporting [{}]", op.opName())
                If op.x() IsNot Nothing Then
                    log.info("X shapeInfo: {}; X values: {}", ArrayToString(op.x().shapeInfoJava()), firstX(op.x(), 10))
                End If

                If op.y() IsNot Nothing Then
                    log.info("Y shapeInfo: {}; Y values: {}", ArrayToString(op.y().shapeInfoJava()), firstX(op.y(), 10))
                End If
            End If
        End Sub

        ''' <summary>The first <paramref name="x"/> values of the array (row-major), formatted as "[a, b, ...]".</summary>
        Protected Friend Shared Function firstX(ByVal array As INDArray, ByVal x As Integer) As String
            Dim builder As New StringBuilder("[")
            Dim limit As Integer = CInt(Math.Min(x, array.length()))
            For e As Integer = 0 To limit - 1
                builder.Append(array.getDouble(CLng(e)).ToString("R", CultureInfo.InvariantCulture))

                If e < limit - 1 Then
                    builder.Append(", ")
                End If
            Next e
            builder.Append("]")

            Return builder.ToString()
        End Function

        ''' <summary>Throws unless every non-null operand has <paramref name="expectedType"/>.</summary>
        ''' <exception cref="ND4VBIllegalStateException">An operand has another data type.</exception>
        Public Shared Sub validateDataType(ByVal expectedType As DataType, ByVal op As Object, ParamArray ByVal operands() As INDArray)
            If operands Is Nothing OrElse operands.Length = 0 Then
                Return
            End If

            Dim cnt As Integer = 0
            For Each operand As INDArray In operands
                If operand Is Nothing Then
                    Continue For
                End If

                If operand.data().dataType() <> expectedType Then
                    Throw New ND4VBIllegalStateException("INDArray [" & cnt & "] dataType is [" & operand.data().dataType().ToString() & "] instead of expected [" & expectedType.ToString() & "]" & (If(op IsNot Nothing, " op: " & op.GetType().FullName, "")))
                End If
                cnt += 1
            Next operand
        End Sub

        ''' <summary>The TAD manager of this executioner (the base class has none).</summary>
        Public Overridable ReadOnly Property TADManager As TADManagerT Implements OpExecutioner.TADManager
            Get
                Throw New NotSupportedException()
            End Get
        End Property

        ''' <summary>
        ''' This method return set of key/value and key/key/value objects, describing current environment: the number of
        ''' cores, the memory available to the process and the operating system.
        ''' </summary>
        Public Overridable ReadOnly Property EnvironmentInformation As Properties Implements OpExecutioner.EnvironmentInformation
            Get
                Dim environment As New Properties()
                environment.put(CPU_CORES_KEY, Global.System.Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture))
                environment.put(HOST_TOTAL_MEMORY_KEY, availableMemory().ToString(CultureInfo.InvariantCulture))
                environment.put(OS_KEY, Global.System.Environment.OSVersion.ToString())
                Return environment
            End Get
        End Property

        ''' <summary>The memory available to the process in bytes (Java <c>Runtime.maxMemory()</c>).</summary>
        Friend Shared Function availableMemory() As Long
#If NETFRAMEWORK Then
            Return CLng(Math.Min(CDbl(Long.MaxValue), CDbl(New Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory)))
#Else
            Return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes
#End If
        End Function

        ''' <summary>Logs the backend, operating system, cores, memory and BLAS vendor from <see cref="EnvironmentInformation"/>.</summary>
        Public Overridable Sub printEnvironmentInformation() Implements OpExecutioner.printEnvironmentInformation
            Dim env As Properties = EnvironmentInformation
            Dim available As Long = 0
            Long.TryParse(env.getProperty(HOST_TOTAL_MEMORY_KEY), NumberStyles.Integer, CultureInfo.InvariantCulture, available)
            Dim memory As Double = available / 1024.0 / 1024.0 / 1024.0
            Dim fm As String = memory.ToString("F1", CultureInfo.InvariantCulture)
            log.info("Backend used: [{}]; OS: [{}]", env.getProperty(BACKEND_KEY), env.getProperty(OS_KEY))
            log.info("Cores: [{}]; Memory: [{}GB];", env.getProperty(CPU_CORES_KEY), fm)
            log.info("Blas vendor: [{}]", env.getProperty(BLAS_VENDOR_KEY))
        End Sub

        Public Overridable Sub push() Implements OpExecutioner.push
            ' no-op
        End Sub

        Public Overridable Sub commit() Implements OpExecutioner.commit
            ' no-op
        End Sub

        Public Overridable Function thresholdEncode(ByVal input As INDArray, ByVal threshold As Double) As INDArray Implements OpExecutioner.thresholdEncode
            Return thresholdEncode(input, threshold, Integer.MaxValue)
        End Function

        ''' <summary>Not supported yet: Java ran the <c>EncodeThreshold</c> custom op.</summary>
        Public Overridable Function thresholdEncode(ByVal input As INDArray, ByVal threshold As Double, ByVal boundary As Integer?) As INDArray Implements OpExecutioner.thresholdEncode
            Throw codecNotSupported("thresholdEncode")
        End Function

        ''' <summary>Not supported yet: Java ran the <c>DecodeThreshold</c> custom op.</summary>
        Public Overridable Function thresholdDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray Implements OpExecutioner.thresholdDecode
            Throw codecNotSupported("thresholdDecode")
        End Function

        ''' <summary>Not supported yet: Java ran the <c>EncodeBitmap</c> custom op.</summary>
        Public Overridable Function bitmapEncode(ByVal indArray As INDArray, ByVal target As INDArray, ByVal threshold As Double) As Long Implements OpExecutioner.bitmapEncode
            Throw codecNotSupported("bitmapEncode")
        End Function

        ''' <summary>Not supported yet: Java ran the <c>EncodeBitmap</c> custom op.</summary>
        Public Overridable Function bitmapEncode(ByVal indArray As INDArray, ByVal threshold As Double) As INDArray Implements OpExecutioner.bitmapEncode
            Throw codecNotSupported("bitmapEncode")
        End Function

        ''' <summary>Not supported yet: Java ran the <c>DecodeBitmap</c> custom op.</summary>
        Public Overridable Function bitmapDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray Implements OpExecutioner.bitmapDecode
            Throw codecNotSupported("bitmapDecode")
        End Function

        Private Shared Function codecNotSupported(ByVal name As String) As NotSupportedException
            Return New NotSupportedException(name & " is not implemented in the managed backend yet: it needs the threshold and bitmap compression custom ops (later work)")
        End Function

        Public Overridable ReadOnly Property CustomOperations As IDictionary(Of String, CustomOpDescriptor) Implements OpExecutioner.CustomOperations
            Get
                Throw New NotSupportedException()
            End Get
        End Property

        ''' <summary>Executes a custom op and returns it.</summary>
        Public Overridable Function execAndReturn(ByVal op As CustomOp) As CustomOp Implements OpExecutioner.execAndReturn
            exec(op)
            Return op
        End Function

        Public Overridable Function calculateOutputShape(ByVal op As CustomOp) As IList(Of LongShapeDescriptor) Implements OpExecutioner.calculateOutputShape
            Throw New NotSupportedException()
        End Function

        Public Overridable Function calculateOutputShape(ByVal op As CustomOp, ByVal opContext As OpContext) As IList(Of LongShapeDescriptor) Implements OpExecutioner.calculateOutputShape
            Throw New NotSupportedException()
        End Function

        ''' <summary>New arrays (<c>Nd4vb.create(LongShapeDescriptor)</c>) for the output shapes of the op.</summary>
        Public Overridable Function allocateOutputArrays(ByVal op As CustomOp) As INDArray() Implements OpExecutioner.allocateOutputArrays
            Dim shapes As IList(Of LongShapeDescriptor) = calculateOutputShape(op)
            Dim [out](shapes.Count - 1) As INDArray
            For i As Integer = 0 To shapes.Count - 1
                [out](i) = Nd4vbT.create(shapes(i))
            Next i
            Return [out]
        End Function

        Public Overridable Sub enableDebugMode(ByVal reallyEnable As Boolean) Implements OpExecutioner.enableDebugMode
            Throw New NotSupportedException()
        End Sub

        Public Overridable Sub enableVerboseMode(ByVal reallyEnable As Boolean) Implements OpExecutioner.enableVerboseMode
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function executeGraph(ByVal id As Long, ByVal map As IDictionary(Of String, INDArray), ByVal reverseMap As IDictionary(Of String, Integer)) As IDictionary(Of String, INDArray) Implements OpExecutioner.executeGraph
            Throw New NotSupportedException("Not yet implemented")
        End Function

        Public Overridable Sub forgetGraph(ByVal id As Long) Implements OpExecutioner.forgetGraph
            Throw New NotSupportedException("Not yet implemented")
        End Sub

        ''' <summary>
        ''' This method allows to set desired number of elements per thread, for performance optimization purposes.
        ''' I.e. if array contains 2048 elements, and threshold is set to 1024, 2 threads will be used for given op execution.
        ''' <para>
        ''' Default value: 1024
        ''' </para>
        ''' </summary>
        ''' <param name="threshold"> </param>
        Public Overridable WriteOnly Property ElementsThreshold As Integer Implements OpExecutioner.ElementsThreshold
            Set(ByVal threshold As Integer)
                ' no-op
            End Set
        End Property

        ''' <summary>
        ''' This method allows to set desired number of sub-arrays per thread, for performance optimization purposes.
        ''' I.e. if matrix has shape of 64 x 128, and threshold is set to 8, each thread will be processing 8 sub-arrays (sure, if you have 8 core cpu).
        ''' If your cpu has, say, 4, cores, only 4 threads will be spawned, and each will process 16 sub-arrays
        ''' <para>
        ''' Default value: 8
        ''' </para>
        ''' </summary>
        ''' <param name="threshold"> </param>
        Public Overridable WriteOnly Property TadThreshold As Integer Implements OpExecutioner.TadThreshold
            Set(ByVal threshold As Integer)
                ' no-op
            End Set
        End Property

        ''' <summary>True if verbose mode is enabled.</summary>
        Public Overridable ReadOnly Property Verbose As Boolean Implements OpExecutioner.Verbose
            Get
                Return verboseField.get()
            End Get
        End Property

        ''' <summary>True if debug mode is enabled.</summary>
        Public Overridable ReadOnly Property Debug As Boolean Implements OpExecutioner.Debug
            Get
                Return debugField.get()
            End Get
        End Property

        Public Overridable Function type() As OpExecutioner.ExecutionerType Implements OpExecutioner.type
            Throw New NotSupportedException()
        End Function

        Public Overridable Function getString(ByVal buffer As DataBuffer, ByVal index As Long) As String Implements OpExecutioner.getString
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Get the information about the op in a String representation, for throwing more useful exceptions (mainly for debugging) </summary>
        ''' <param name="op"> </param>
        ''' <param name="dimensions">    Use optional here for 3 states: null = "not an exec(Op, int... dim) call". empty = "exec(Op, null)".
        '''                     Otherwise present = "exec(Op,int[])" call </param>
        Public Overridable Function opInfoString(ByVal op As Op, ByVal dimensions As [Optional](Of Integer())) As String
            If op Is Nothing Then
                Return "<NULL OP>"
            End If

            Dim sb As New StringBuilder()
            sb.Append("Class: ").Append(CObj(op).GetType().FullName).Append("; opNum: ").Append(op.opNum()).Append("; opName: ").Append(op.opName())
            If TypeOf op Is DifferentialFunction Then
                sb.Append("; opType: ").Append(DirectCast(op, DifferentialFunction).opType().ToString())
            End If

            If dimensions IsNot Nothing Then
                sb.Append("; dimensions: ")
                If dimensions.Present Then
                    sb.Append(ArrayToString(dimensions.get()))
                Else
                    sb.Append("<null>")
                End If
            End If

            Dim x As INDArray = op.x()
            Dim y As INDArray = op.y()
            Dim z As INDArray = op.z()
            Dim extraArgs() As Object = op.extraArgs()

            sb.Append(vbLf)
            sb.Append("x: ").Append(arrayInfo(x)).Append("; ")
            sb.Append("y: ").Append(arrayInfo(y)).Append("; ")
            sb.Append("z: ").Append(arrayInfo(z)).Append("; ")
            If x Is y AndAlso x IsNot Nothing Then
                sb.Append("(x == y)")
            End If
            If x Is z AndAlso x IsNot Nothing Then
                sb.Append("(x == z)")
            End If
            If y Is z AndAlso y IsNot Nothing Then
                sb.Append("(y == z)")
            End If
            sb.Append(vbLf)
            sb.Append("; extraArgs: ").Append(formatArgs(extraArgs))
            Return sb.ToString()
        End Function

        Private Shared Function formatArgs(ByVal args() As Object) As String
            If args Is Nothing Then
                Return "null"
            End If
            Dim parts(args.Length - 1) As String
            For i As Integer = 0 To args.Length - 1
                parts(i) = If(args(i) Is Nothing, "null", Convert.ToString(args(i), CultureInfo.InvariantCulture))
            Next i
            Return "[" & String.Join(", ", parts) & "]"
        End Function

        ''' <summary>"[a, b, c]" for an array (Java <c>Arrays.toString</c>).</summary>
        Friend Shared Function ArrayToString(Of T)(ByVal values() As T) As String
            If values Is Nothing Then
                Return "null"
            End If
            Dim parts(values.Length - 1) As String
            For i As Integer = 0 To values.Length - 1
                parts(i) = Convert.ToString(values(i), CultureInfo.InvariantCulture)
            Next i
            Return "[" & String.Join(", ", parts) & "]"
        End Function

        ''' <summary>The shape information of the array on one line, "(empty NDArray)" or "&lt;null&gt;".</summary>
        Public Overridable Function arrayInfo(ByVal arr As INDArray) As String
            If arr Is Nothing Then
                Return "<null>"
            End If
            If arr.Empty Then
                Return "(empty NDArray)"
            End If

            Return arr.shapeInfoToString().Replace(vbLf, "")
        End Function

        Public Overridable ReadOnly Property ExperimentalMode As Boolean Implements OpExecutioner.ExperimentalMode
            Get
                Return False
            End Get
        End Property

        Public Overridable Function buildContext() As OpContext Implements OpExecutioner.buildContext
            Throw New NotSupportedException("OpContext is available only on native backends")
        End Function

        Public Overridable Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray() Implements OpExecutioner.exec
            Throw New NotSupportedException()
        End Function

        Public Overridable Function inspectArray(ByVal array As INDArray) As INDArrayStatistics Implements OpExecutioner.inspectArray
            Throw New NotSupportedException()
        End Function

        Public Overridable Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal empty As Boolean) As DataBuffer Implements OpExecutioner.createShapeInfo
            Throw New NotSupportedException()
        End Function

        Public Overridable Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal extras As Long) As DataBuffer Implements OpExecutioner.createShapeInfo
            Throw New NotSupportedException()
        End Function

        Public Overridable Function tadShapeInfoAndOffsets(ByVal array As INDArray, ByVal dimension() As Integer) As TadPack Implements OpExecutioner.tadShapeInfoAndOffsets
            Throw New NotSupportedException()
        End Function

        Public Overridable Function createConstantBuffer(ByVal values() As Long, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
            Throw New NotSupportedException()
        End Function

        Public Overridable Function createConstantBuffer(ByVal values() As Integer, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
            Return createConstantBuffer(ArrayUtil.toLongArray(values), desiredType)
        End Function

        Public Overridable Function createConstantBuffer(ByVal values() As Single, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
            Return createConstantBuffer(ArrayUtil.toDoubles(values), desiredType)
        End Function

        Public Overridable Function createConstantBuffer(ByVal values() As Double, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
            Throw New NotSupportedException()
        End Function

        ''' <summary>Sets x: input 0 of the context when one is given, otherwise the op's x.</summary>
        Public Overridable Sub setX(ByVal x As INDArray, ByVal op As Op, ByVal oc As OpContext)
            If oc IsNot Nothing Then
                oc.setInputArray(0, x)
            Else
                op.setX(x)
            End If
        End Sub

        ''' <summary>x: input 0 of the context when one is given, otherwise the op's x.</summary>
        Public Overridable Function getX(ByVal op As Op, ByVal oc As OpContext) As INDArray
            If oc IsNot Nothing Then
                Return oc.getInputArray(0)
            End If
            Return op.x()
        End Function

        ''' <summary>Sets y: input 1 of the context when one is given, otherwise the op's y.</summary>
        Public Overridable Sub setY(ByVal y As INDArray, ByVal op As Op, ByVal oc As OpContext)
            If oc IsNot Nothing Then
                oc.setInputArray(1, y)
            Else
                op.setY(y)
            End If
        End Sub

        ''' <summary>y: input 1 of the context when one is given, otherwise the op's y.</summary>
        Public Overridable Function getY(ByVal op As Op, ByVal oc As OpContext) As INDArray
            If oc IsNot Nothing Then
                Return oc.getInputArray(1)
            End If
            Return op.y()
        End Function

        ''' <summary>Sets z: output 0 of the context when one is given, otherwise the op's z.</summary>
        Public Overridable Sub setZ(ByVal z As INDArray, ByVal op As Op, ByVal oc As OpContext)
            If oc IsNot Nothing Then
                oc.setOutputArray(0, z)
            Else
                op.setZ(z)
            End If
        End Sub

        ''' <summary>z: output 0 of the context when one is given, otherwise the op's z.</summary>
        Public Overridable Function getZ(ByVal op As Op, ByVal oc As OpContext) As INDArray
            If oc IsNot Nothing Then
                Return oc.getOutputArray(0)
            End If
            Return op.z()
        End Function
    End Class

End Namespace
