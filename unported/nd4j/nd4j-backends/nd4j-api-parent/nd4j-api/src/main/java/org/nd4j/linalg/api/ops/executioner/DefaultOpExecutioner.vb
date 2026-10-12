Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4jEnvironment = org.nd4j.linalg.api.environment.Nd4jEnvironment
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = org.nd4j.linalg.api.ndarray.INDArrayStatistics
Imports org.nd4j.linalg.api.ops
Imports Aggregate = org.nd4j.linalg.api.ops.aggregates.Aggregate
Imports org.nd4j.linalg.api.ops.aggregates
Imports DecodeBitmap = org.nd4j.linalg.api.ops.compression.DecodeBitmap
Imports DecodeThreshold = org.nd4j.linalg.api.ops.compression.DecodeThreshold
Imports EncodeBitmap = org.nd4j.linalg.api.ops.compression.EncodeBitmap
Imports EncodeThreshold = org.nd4j.linalg.api.ops.compression.EncodeThreshold
Imports ScatterUpdate = org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports Random = org.nd4j.linalg.api.rng.Random
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports TadPack = org.nd4j.linalg.api.shape.TadPack
Imports TADManager = org.nd4j.linalg.cache.TADManager
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports org.nd4j.common.primitives
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.ops.executioner


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class DefaultOpExecutioner implements OpExecutioner
	Public MustInherit Class DefaultOpExecutioner
		Implements OpExecutioner

		Private Const SCOPE_PANIC_MSG As String = "For more details, see the ND4J User Guide: https://deeplearning4j.konduit.ai/nd4j/overview#workspaces-scope-panic"

'JAVA TO VB CONVERTER NOTE: The field profilingMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend profilingMode_Conflict As ProfilingMode = ProfilingMode.SCOPE_PANIC

'JAVA TO VB CONVERTER NOTE: The field verbose was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend verbose_Conflict As New AtomicBoolean(False)
'JAVA TO VB CONVERTER NOTE: The field debug was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend debug_Conflict As New AtomicBoolean(False)

		Public Sub New()
		End Sub

		Protected Friend Overridable Sub checkForCompression(ByVal op As Op)
			' check for INT datatype arrays
			interceptIntDataType(op)

			If op.x() IsNot Nothing AndAlso op.x().Compressed Then
				Nd4j.Compressor.decompressi(op.x())
			End If

			If op.y() IsNot Nothing AndAlso op.y().Compressed Then
				Nd4j.Compressor.decompressi(op.y())
			End If

			If op.z() IsNot Nothing AndAlso op.z().Compressed Then
				Nd4j.Compressor.decompressi(op.z())
			End If
		End Sub

		Public Overridable ReadOnly Property LastOp As String Implements OpExecutioner.getLastOp
			Get
				Return "UNKNOWN"
			End Get
		End Property

		''' <summary>
		''' This method checks if any Op operand has data opType of INT, and throws exception if any.
		''' </summary>
		''' <param name="op"> </param>
		Protected Friend Overridable Sub interceptIntDataType(ByVal op As Op)
			' FIXME: Remove this method, after we'll add support for <int> dtype operations
	'
	'        if (op.x() != null && op.x().data().dataType() == DataType.INT)
	'            throw new ND4JIllegalStateException(
	'                            "Op.X contains INT data. Operations on INT dataType are not supported yet");
	'
	'        if (op.z() != null && op.z().data().dataType() == DataType.INT)
	'            throw new ND4JIllegalStateException(
	'                            "Op.Z contains INT data. Operations on INT dataType are not supported yet");
	'
	'        if (op.y() != null && op.y().data().dataType() == DataType.INT)
	'            throw new ND4JIllegalStateException(
	'                            "Op.Y contains INT data. Operations on INT dataType are not supported yet.");
	'        
		End Sub

		Public MustOverride Overrides Function exec(ByVal op As Op) As INDArray Implements OpExecutioner.exec

		Public MustOverride Overrides Function exec(ByVal op As Op, ByVal opContext As OpContext) As INDArray Implements OpExecutioner.exec

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

			Throw New System.ArgumentException("Illegal opType of op: " & op.GetType())
		End Function

		Public Overridable Function execAndReturn(ByVal op As TransformOp) As TransformOp Implements OpExecutioner.execAndReturn
			exec(op)
			Return op
		End Function


		Public Overridable Function execAndReturn(ByVal op As ReduceOp) As ReduceOp Implements OpExecutioner.execAndReturn
			exec(op)
			Return op
		End Function

		Public Overridable Function execAndReturn(ByVal op As Variance) As Variance Implements OpExecutioner.execAndReturn
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

		Public Overridable Function exec(ByVal op As CustomOp) As INDArray() Implements OpExecutioner.exec
			Return CType(execAndReturn(op).outputArguments(), List(Of INDArray)).ToArray()
		End Function

		Public MustOverride Overrides Function exec(ByVal op As ReduceOp) As INDArray Implements OpExecutioner.exec

		Public MustOverride Overrides Function exec(ByVal accumulation As Variance) As INDArray Implements OpExecutioner.exec

		Public MustOverride Overrides Function exec(ByVal op As IndexAccumulation) As INDArray Implements OpExecutioner.exec

		Public MustOverride Overrides Function exec(ByVal broadcast As BroadcastOp) As INDArray Implements OpExecutioner.exec

		Public Overridable Sub exec(ByVal op As MetaOp) Implements OpExecutioner.exec
			Throw New System.NotSupportedException("MetaOp execution isn't supported for this OpExecutioner yet")
		End Sub

		Public Overridable Sub exec(ByVal op As GridOp) Implements OpExecutioner.exec
			Throw New System.NotSupportedException("GridOp execution isn't supported for this OpExecutioner yet")
		End Sub

		Public Overridable Sub exec(Of T As Aggregate)(ByVal batch As Batch(Of T)) Implements OpExecutioner.exec
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Sub exec(ByVal op As Aggregate) Implements OpExecutioner.exec
			Throw New System.NotSupportedException()
		End Sub

		Public MustOverride Overrides Function exec(ByVal op As ScalarOp) As INDArray Implements OpExecutioner.exec

		Public Overridable Sub exec(ByVal batch As IList(Of Aggregate))
			Throw New System.NotSupportedException()
		End Sub

		''' <summary>
		''' This method executes specified RandomOp using default RNG available via Nd4j.getRandom()
		''' </summary>
		''' <param name="op"> </param>
		Public Overridable Function exec(ByVal op As RandomOp) As INDArray Implements OpExecutioner.exec
			Return exec(op, Nd4j.Random)
		End Function

		''' <summary>
		''' This method executes specific RandomOp against specified RNG
		''' </summary>
		''' <param name="op"> </param>
		''' <param name="rng"> </param>
		Public MustOverride Overrides Function exec(ByVal op As RandomOp, ByVal rng As Random) As INDArray Implements OpExecutioner.exec


		<Obsolete>
		Public Overridable Property ProfilingMode Implements OpExecutioner.setProfilingMode As ProfilingMode
			Set(ByVal mode As ProfilingMode)
    
				profilingMode_Conflict = mode
				Dim config As ProfilerConfig = Nothing
				Select Case profilingMode_Conflict
					Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ALL
						config = ProfilerConfig.builder().checkWorkspaces(True).checkElapsedTime(True).stackTrace(True).build()
					Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.METHODS
						config = ProfilerConfig.builder().stackTrace(True).build()
					Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.OPERATIONS
						config = ProfilerConfig.builder().stackTrace(True).checkElapsedTime(True).build()
					Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.SCOPE_PANIC
						config = ProfilerConfig.builder().checkWorkspaces(True).build()
					Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ANY_PANIC
						config = ProfilerConfig.builder().checkForINF(True).checkForNAN(True).build()
					Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.INF_PANIC
						config = ProfilerConfig.builder().checkForINF(True).build()
					Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.NAN_PANIC
						config = ProfilerConfig.builder().checkForNAN(True).build()
					Case Else
						config = ProfilerConfig.builder().build()
				End Select
				OpProfiler.Instance.setConfig(config)
			End Set
			Get
				Return profilingMode_Conflict
			End Get
		End Property

		Public Overridable WriteOnly Property ProfilingConfig Implements OpExecutioner.setProfilingConfig As ProfilerConfig
			Set(ByVal config As ProfilerConfig)
				OpProfiler.Instance.setConfig(config)
			End Set
		End Property


		Protected Friend Overridable Sub checkWorkspace(ByVal opName As String, ByVal array As INDArray)
			If array.Attached Then
				Dim ws As val = array.data().ParentWorkspace

				If ws.getWorkspaceType() <> MemoryWorkspace.Type.CIRCULAR Then

					If Not ws.isScopeActive() Then
						Throw New ND4JIllegalStateException("Op [" & opName & "] X argument uses leaked workspace pointer from workspace [" & ws.getId() & "]: Workspace the array was defined in is no longer open." & vbLf & "All open workspaces: " & allOpenWorkspaces() & vbLf & SCOPE_PANIC_MSG)
					End If

					If ws.getGenerationId() <> array.data().GenerationId Then
						Throw New ND4JIllegalStateException("Op [" & opName & "] X argument uses outdated workspace pointer from workspace [" & ws.getId() & "]: Workspace array was defined in has been closed and reopened at least once since array creation. Array WS iteration: " & array.data().GenerationId & ". Workspace current iteration: " & ws.getGenerationId() & vbLf & "All open workspaces: " & allOpenWorkspaces() & vbLf & SCOPE_PANIC_MSG)
					End If
				End If
			End If
		End Sub

		Protected Friend Overridable Sub checkForWorkspaces(ByVal op As CustomOp, ByVal oc As OpContext)
			Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
			Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

			For Each input As val In inArgs
				checkWorkspace(op.opName(), input)
			Next input

			For Each output As val In outArgs
				checkWorkspace(op.opName(), output)
			Next output
		End Sub

		Protected Friend Overridable Sub checkForWorkspaces(ByVal op As Op, ByVal oc As OpContext)
			Dim x As val = If(oc IsNot Nothing, oc.getInputArray(0), op.x())
			If x IsNot Nothing Then
				checkWorkspace(op.opName(), x)
			End If

			Dim y As val = If(oc IsNot Nothing AndAlso oc.getInputArrays().Count > 1, oc.getInputArray(1), op.y())
			If y IsNot Nothing Then
				checkWorkspace(op.opName(), y)
			End If

			Dim z As val = If(oc IsNot Nothing, oc.getOutputArray(0), op.z())
			If z IsNot Nothing Then
				checkWorkspace(op.opName(), z)
			End If
		End Sub

		Public Shared Function allOpenWorkspaces() As IList(Of String)
			Dim l As IList(Of MemoryWorkspace) = Nd4j.WorkspaceManager.getAllWorkspacesForCurrentThread()
			Dim workspaces As IList(Of String) = New List(Of String)(l.Count)
			For Each ws As MemoryWorkspace In l
				If ws.ScopeActive Then
					workspaces.Add(ws.Id)
				End If
			Next ws
			Return workspaces
		End Function

		<Obsolete>
		Public Overridable Function profilingHookIn(ByVal op As Op, ParamArray ByVal tadBuffers() As DataBuffer) As Long
			Select Case profilingMode_Conflict
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ALL
					OpProfiler.Instance.processOpCall(op, tadBuffers)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.METHODS
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.OPERATIONS
					OpProfiler.Instance.processOpCall(op, tadBuffers)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.SCOPE_PANIC
					checkForWorkspaces(op, Nothing)
					Return 0L
				Case Else
					Return 0L
			End Select

			Return System.nanoTime()
		End Function

		<Obsolete>
		Public Overridable Function profilingHookIn(ByVal op As CustomOp, ByVal oc As OpContext) As Long
			Select Case profilingMode_Conflict
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ALL
					OpProfiler.Instance.processOpCall(op)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.METHODS
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.OPERATIONS
					OpProfiler.Instance.processOpCall(op)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.SCOPE_PANIC
					checkForWorkspaces(op, oc)
					Return 0L
				Case Else
					Return 0L
			End Select

			Return System.nanoTime()
		End Function

		<Obsolete>
		Public Overridable Sub profilingHookOut(ByVal op As Op, ByVal oc As OpContext, ByVal timeStart As Long)
			Select Case profilingMode_Conflict
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ALL
					OpProfiler.Instance.processStackCall(op, timeStart)
					OpProfiler.Instance.timeOpCall(op, timeStart)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.METHODS
					OpProfiler.Instance.processStackCall(op, timeStart)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.OPERATIONS
					OpProfiler.Instance.timeOpCall(op, timeStart)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.NAN_PANIC
					OpExecutionerUtil.checkForNaN(op, oc)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.INF_PANIC
					OpExecutionerUtil.checkForInf(op, oc)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ANY_PANIC
					OpExecutionerUtil.checkForNaN(op, oc)
					OpExecutionerUtil.checkForInf(op, oc)
				Case Else
			End Select

			If Nd4j.Executioner.Verbose Then
				If op.z() IsNot Nothing Then
					log.info("Op name: {}; Z shapeInfo: {}; Z values: {}", op.opName(), op.z().shapeInfoJava(), firstX(op.z(), 10))
				End If
			End If
		End Sub

		<Obsolete>
		Public Overridable Sub profilingHookOut(ByVal op As CustomOp, ByVal oc As OpContext, ByVal timeStart As Long)
			Select Case profilingMode_Conflict
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ALL
					OpProfiler.Instance.processStackCall(op, timeStart)
					OpProfiler.Instance.timeOpCall(op, timeStart)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.METHODS
					OpProfiler.Instance.processStackCall(op, timeStart)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.OPERATIONS
					OpProfiler.Instance.timeOpCall(op, timeStart)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.NAN_PANIC
					OpExecutionerUtil.checkForNaN(op, oc)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.INF_PANIC
					OpExecutionerUtil.checkForInf(op, oc)
				Case org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.ANY_PANIC
					OpExecutionerUtil.checkForNaN(op, oc)
					OpExecutionerUtil.checkForInf(op, oc)
				Case Else
			End Select
		End Sub


		Public Overridable Function profilingConfigurableHookIn(ByVal op As CustomOp, ByVal oc As OpContext) As Long
			Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
			Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

			For Each arr As val In inArgs
				If arr.wasClosed() Then
					Throw New System.InvalidOperationException("One of Input arguments was closed before call")
				End If
			Next arr

			For Each arr As val In outArgs
				If arr.wasClosed() Then
					Throw New System.InvalidOperationException("One of Output arguments was closed before call")
				End If
			Next arr

			If OpProfiler.Instance.getConfig() Is Nothing Then
				Return System.nanoTime()
			End If

			If OpProfiler.Instance.getConfig().isStackTrace() OrElse OpProfiler.Instance.getConfig().isCheckElapsedTime() Then
				OpProfiler.Instance.processOpCall(op)
			End If

			If OpProfiler.Instance.getConfig().isCheckWorkspaces() Then
				checkForWorkspaces(op, oc)
			End If

			Return System.nanoTime()
		End Function

		Public Overridable Function profilingConfigurableHookIn(ByVal op As Op, ParamArray ByVal tadBuffers() As DataBuffer) As Long
			If op.x() IsNot Nothing Then
				If op.x().wasClosed() Then
					Throw New System.InvalidOperationException("Op.X argument was closed before call")
				End If
			End If

			If op.y() IsNot Nothing Then
				If op.y().wasClosed() Then
					Throw New System.InvalidOperationException("Op.Y argument was closed before call")
				End If
			End If

			If op.z() IsNot Nothing Then
				If op.z().wasClosed() Then
					Throw New System.InvalidOperationException("Op.Z argument was closed before call")
				End If
			End If

			If OpProfiler.Instance.getConfig() Is Nothing Then
				Return System.nanoTime()
			End If

			If OpProfiler.Instance.getConfig().isStackTrace() OrElse OpProfiler.Instance.getConfig().isCheckElapsedTime() Then
				OpProfiler.Instance.processOpCall(op)
			End If

			If OpProfiler.Instance.getConfig().isNotOptimalTAD() Then
				OpProfiler.Instance.processOpCall(op, tadBuffers)
			End If
			If OpProfiler.Instance.getConfig().isCheckWorkspaces() Then
				checkForWorkspaces(op, Nothing)
			End If

			Return System.nanoTime()
		End Function


		Public Overridable Sub profilingConfigurableHookOut(ByVal op As Op, ByVal oc As OpContext, ByVal timeStart As Long)
			If OpProfiler.Instance.getConfig() Is Nothing Then
				Return
			End If

			If OpProfiler.Instance.getConfig().isStackTrace() Then
				OpProfiler.Instance.processStackCall(op, timeStart)
			End If
			If OpProfiler.Instance.getConfig().isCheckElapsedTime() Then
				OpProfiler.Instance.timeOpCall(op, timeStart)
			End If
			If OpProfiler.Instance.getConfig().isCheckForNAN() Then
				OpExecutionerUtil.checkForNaN(op, oc)
			End If
			If OpProfiler.Instance.getConfig().isCheckForINF() Then
				OpExecutionerUtil.checkForInf(op, oc)
			End If
			If OpProfiler.Instance.getConfig().isNativeStatistics() Then
				If op.z() IsNot Nothing Then
					Dim stat As INDArrayStatistics = inspectArray(op.z())
					OpProfiler.Instance.setStatistics(stat)
					log.info("Op name: {}; Z shapeInfo: {}; Statistics: min:{} max:{} mean:{} stdev:{} pos:{}, neg:{} zero:{} inf:{} nan:{}", op.opName(), op.z().shapeInfoJava(), stat.getMinValue(), stat.getMaxValue(), stat.getMeanValue(), stat.getStdDevValue(), stat.getCountPositive(), stat.getCountNegative(), stat.getCountZero(), stat.getCountInf(), stat.getCountNaN())
				End If
			End If

			If Nd4j.Executioner.Verbose Then
				If op.z() IsNot Nothing Then
					log.info("Op name: {}; Z shapeInfo: {}; Z values: {}", op.opName(), op.z().shapeInfoJava(), firstX(op.z(), 10))
				End If
			End If
		End Sub

		Public Overridable Sub profilingConfigurableHookOut(ByVal op As CustomOp, ByVal oc As OpContext, ByVal timeStart As Long)
			If OpProfiler.Instance.getConfig() Is Nothing Then
				Return
			End If

			If OpProfiler.Instance.getConfig().isStackTrace() Then
				OpProfiler.Instance.processStackCall(op, timeStart)
			End If
			If OpProfiler.Instance.getConfig().isCheckElapsedTime() Then
				OpProfiler.Instance.timeOpCall(op, timeStart)
			End If
			If OpProfiler.Instance.getConfig().isCheckForNAN() Then
				OpExecutionerUtil.checkForNaN(op, oc)
			End If
			If OpProfiler.Instance.getConfig().isCheckForINF() Then
				OpExecutionerUtil.checkForInf(op, oc)
			End If
		End Sub

		''' <summary>
		''' Validate the data types
		''' for the given operation </summary>
		''' <param name="expectedType"> </param>
		''' <param name="op"> </param>
		Public Shared Sub validateDataType(ByVal expectedType As DataType, ByVal op As Op)
			If op.x() IsNot Nothing AndAlso Not Shape.isEmpty(op.x().shapeInfoJava()) AndAlso op.x().data().dataType() = DataType.COMPRESSED Then
				Nd4j.Compressor.decompressi(op.x())
			End If

			If op.y() IsNot Nothing AndAlso Not Shape.isEmpty(op.y().shapeInfoJava()) AndAlso op.y().data().dataType() = DataType.COMPRESSED Then
				Nd4j.Compressor.decompressi(op.y())
			End If

			If op.z() IsNot Nothing AndAlso Not Shape.isEmpty(op.z().shapeInfoJava()) AndAlso op.z().data().dataType() = DataType.COMPRESSED Then
				Nd4j.Compressor.decompressi(op.z())
			End If

	'        
	'        if (op.x() != null && !Shape.isEmpty(op.x().shapeInfoJava())
	'                && op.x().data().dataType() != expectedType
	'                && op.x().data().dataType() != DataType.COMPRESSED) {
	'            throw new ND4JIllegalStateException("op.X dataType is [" + op.x().data().dataType()
	'                    + "] instead of expected [" + expectedType + "] - x.shape = " + Arrays.toString(op.x().shape())
	'                    + (op.y() != null ? ", y.shape=" + Arrays.toString(op.y().shape()) : "")
	'                    + ", z.shape=" + Arrays.toString(op.z().shape()) + " - op: " + op.getClass().getName());
	'        }
	'        
	'
	'        if (op.z() != null && !Shape.isEmpty(op.z().shapeInfoJava())
	'                        && op.z().data().dataType() != expectedType
	'                        && op.z().data().dataType() != DataType.COMPRESSED)
	'            throw new ND4JIllegalStateException("op.Z dataType is [" + op.z().data().dataType()
	'                            + "] instead of expected [" + expectedType + "]");
	'        

			If op.y() IsNot Nothing AndAlso Not Shape.isEmpty(op.y().shapeInfoJava()) AndAlso op.y().data().dataType() <> expectedType Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New ND4JIllegalStateException("op.Y dataType is [" & op.y().data().dataType() & "] instead of expected [" & expectedType & "] - x.shape = " & java.util.Arrays.toString(op.x().shape()) & (If(op.y() IsNot Nothing, ", y.shape=" & java.util.Arrays.toString(op.y().shape()), "")) & ", z.shape=" & java.util.Arrays.toString(op.z().shape()) & " - op: " & op.GetType().FullName)

			End If


			If Nd4j.Executioner.Verbose Then
				log.info("Reporting [{}]", op.opName())
				If op.x() IsNot Nothing Then
					log.info("X shapeInfo: {}; X values: {}", op.x().shapeInfoJava(), firstX(op.x(), 10))
				End If

				If op.y() IsNot Nothing Then
					log.info("Y shapeInfo: {}; Y values: {}", op.y().shapeInfoJava(), firstX(op.y(), 10))
				End If
			End If
		End Sub

		Protected Friend Shared Function firstX(ByVal array As INDArray, ByVal x As Integer) As String
			Dim builder As val = New StringBuilder("[")
			Dim limit As val = CInt(Math.Min(x, array.length()))
			For e As Integer = 0 To limit - 1
				builder.append(array.getDouble(e))

				If e < limit - 1 Then
					builder.append(", ")
				End If
			Next e
			builder.append("]")

			Return builder.ToString()
		End Function

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
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New ND4JIllegalStateException("INDArray [" & cnt & "] dataType is [" & operand.data().dataType() & "] instead of expected [" & expectedType & "]" & (If(op IsNot Nothing, " op: " & op.GetType().FullName, "")))
				End If
				cnt += 1
			Next operand
		End Sub

		Public Overridable ReadOnly Property TADManager As TADManager Implements OpExecutioner.getTADManager
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		''' <summary>
		''' This method return set of key/value and key/key/value objects, describing current environment
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property EnvironmentInformation As Properties
			Get
				Dim environment As New Properties()
				environment.put(Nd4jEnvironment.CPU_CORES_KEY, Runtime.getRuntime().availableProcessors())
				environment.put(Nd4jEnvironment.HOST_TOTAL_MEMORY_KEY, Runtime.getRuntime().maxMemory())
				environment.put(Nd4jEnvironment.OS_KEY, System.getProperty("os.name"))
				Return environment
			End Get
		End Property

		Public Overridable Sub printEnvironmentInformation() Implements OpExecutioner.printEnvironmentInformation
			Dim env As Properties = EnvironmentInformation
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim memory As Double = (CType(env.get("memory.available"), Long?)).Value / CDbl(1024) / 1024 / 1024
			Dim fm As String = String.Format("{0:F1}", memory)
			log.info("Backend used: [{}]; OS: [{}]", env.get("backend"), env.get("os"))
			log.info("Cores: [{}]; Memory: [{}GB];", env.get("cores"), fm)
			log.info("Blas vendor: [{}]", env.get("blas.vendor"))
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

		Private Function _length(ByVal shape() As Long) As Long
			' scalar case
			If shape.Length = 0 Then
				Return 1
			ElseIf shape.Length = 1 Then
				Return shape(0)
			Else
				Dim length As Long = 1
				For e As Integer = 0 To shape.Length - 1
					length *= shape(e)
				Next e

				Return length
			End If
		End Function

		Public Overridable Function thresholdEncode(ByVal input As INDArray, ByVal threshold As Double, ByVal boundary As Integer?) As INDArray Implements OpExecutioner.thresholdEncode
			Dim op_shape As val = New EncodeThreshold(input, CSng(threshold), boundary)
			Dim shapes As val = Nd4j.Executioner.calculateOutputShape(op_shape)

			If _length(shapes.get(1).getShape()) < 2 Then
				Return Nothing
			End If

			Dim result As val = Nd4j.create(DataType.INT32, shapes.get(1).getShape())

			op_shape.addOutputArgument(input, result)
			Nd4j.exec(op_shape)

			Return If(result.getInt(0) > 0, result, Nothing)
		End Function

		Public Overridable Function thresholdDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray Implements OpExecutioner.thresholdDecode
			Nd4j.exec(New DecodeThreshold(encoded, target))
			Return target
		End Function

		Public Overridable Function bitmapEncode(ByVal indArray As INDArray, ByVal target As INDArray, ByVal threshold As Double) As Long Implements OpExecutioner.bitmapEncode
			Dim results As val = Nd4j.exec(New EncodeBitmap(indArray, target, Nd4j.scalar(0), CSng(threshold)))

			' return number of elements taht were compressed
			Return results(2).getInt(0)
		End Function

		Public Overridable Function bitmapEncode(ByVal indArray As INDArray, ByVal threshold As Double) As INDArray Implements OpExecutioner.bitmapEncode
			Dim array As val = Nd4j.create(DataType.INT32, indArray.length() \ 16 + 5)
			bitmapEncode(indArray, array, threshold)
			Return array
		End Function

		Public Overridable Function bitmapDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray Implements OpExecutioner.bitmapDecode
			Nd4j.exec(New DecodeBitmap(encoded, target))
			Return target
		End Function


		Public Overridable ReadOnly Property CustomOperations As IDictionary(Of String, CustomOpDescriptor)
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		Public Overridable Function execAndReturn(ByVal op As CustomOp) As CustomOp Implements OpExecutioner.execAndReturn
			exec(op)
			Return op
		End Function

		Public Overridable Function calculateOutputShape(ByVal op As CustomOp) As IList(Of LongShapeDescriptor)
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function calculateOutputShape(ByVal op As CustomOp, ByVal opContext As OpContext) As IList(Of LongShapeDescriptor)
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function allocateOutputArrays(ByVal op As CustomOp) As INDArray() Implements OpExecutioner.allocateOutputArrays
			Dim shapes As IList(Of LongShapeDescriptor) = calculateOutputShape(op)
			Dim [out](shapes.Count - 1) As INDArray
			For i As Integer = 0 To shapes.Count - 1
				[out](i) = Nd4j.create(shapes(i))
			Next i
			Return [out]
		End Function


		Public Overridable Sub enableDebugMode(ByVal reallyEnable As Boolean) Implements OpExecutioner.enableDebugMode
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Sub enableVerboseMode(ByVal reallyEnable As Boolean) Implements OpExecutioner.enableVerboseMode
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Sub registerGraph(ByVal id As Long, ByVal graph As Pointer) Implements OpExecutioner.registerGraph
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub

		Public Overridable Function executeGraph(ByVal id As Long, ByVal map As IDictionary(Of String, INDArray), ByVal reverseMap As IDictionary(Of String, Integer)) As IDictionary(Of String, INDArray)
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overridable Sub forgetGraph(ByVal id As Long) Implements OpExecutioner.forgetGraph
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub


		''' <summary>
		''' This method allows to set desired number of elements per thread, for performance optimization purposes.
		''' I.e. if array contains 2048 elements, and threshold is set to 1024, 2 threads will be used for given op execution.
		''' <para>
		''' Default value: 1024
		''' 
		''' </para>
		''' </summary>
		''' <param name="threshold"> </param>
		Public Overridable WriteOnly Property ElementsThreshold Implements OpExecutioner.setElementsThreshold As Integer
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
		''' 
		''' </para>
		''' </summary>
		''' <param name="threshold"> </param>
		Public Overridable WriteOnly Property TadThreshold Implements OpExecutioner.setTadThreshold As Integer
			Set(ByVal threshold As Integer)
				' no-op
			End Set
		End Property

		Public Overridable ReadOnly Property Verbose As Boolean Implements OpExecutioner.isVerbose
			Get
				Return verbose_Conflict.get()
			End Get
		End Property

		Public Overridable ReadOnly Property Debug As Boolean Implements OpExecutioner.isDebug
			Get
				Return debug_Conflict.get()
			End Get
		End Property

		Public Overridable Function type() As ExecutionerType Implements OpExecutioner.type
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function getString(ByVal buffer As DataBuffer, ByVal index As Long) As String Implements OpExecutioner.getString
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub scatterUpdate(ByVal op As ScatterUpdate.UpdateOp, ByVal array As INDArray, ByVal indices As INDArray, ByVal updates As INDArray, ByVal axis() As Integer) Implements OpExecutioner.scatterUpdate
			Throw New System.NotSupportedException()
		End Sub


		''' <summary>
		''' Get the information about the op in a String representation, for throwing more useful exceptions (mainly for debugging) </summary>
		''' <param name="op"> </param>
		''' <param name="dimensions">    Use optional here for 3 states: null = "not an exec(Op, int... dim) call". empty = "exec(Op, null)".
		'''                     Otherwise present = "exec(Op,int[])" call
		''' @return </param>
		Public Overridable Function opInfoString(ByVal op As Op, ByVal dimensions As [Optional](Of Integer())) As String
			If op Is Nothing Then
				Return "<NULL OP>"
			End If

			Dim sb As New StringBuilder()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			sb.Append("Class: ").Append(op.GetType().FullName).Append("; opNum: ").Append(op.opNum()).Append("; opName: ").Append(op.opName())
			If TypeOf op Is DifferentialFunction Then
				sb.Append("; opType: ").Append(DirectCast(op, DifferentialFunction).opType())
			End If

			If dimensions IsNot Nothing Then
				sb.Append("; dimensions: ")
				If dimensions.Present Then
					sb.Append(java.util.Arrays.toString(dimensions.get()))
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
			sb.Append("; extraArgs: ").Append(Preconditions.formatArray(extraArgs))
			Return sb.ToString()
		End Function

		Public Overridable Function arrayInfo(ByVal arr As INDArray) As String
			If arr Is Nothing Then
				Return "<null>"
			End If
			If arr.Empty Then
				Return "(empty NDArray)"
			End If

			Return arr.shapeInfoToString().replaceAll(vbLf,"")
		End Function

		Public Overridable ReadOnly Property ExperimentalMode As Boolean Implements OpExecutioner.isExperimentalMode
			Get
				Return False
			End Get
		End Property

		Public Overridable Function buildContext() As OpContext Implements OpExecutioner.buildContext
			Throw New System.NotSupportedException("OpContext is available only on native backends")
		End Function

		Public Overridable Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray() Implements OpExecutioner.exec
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inspectArray(ByVal array As INDArray) As INDArrayStatistics Implements OpExecutioner.inspectArray
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal empty As Boolean) As DataBuffer Implements OpExecutioner.createShapeInfo
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal extras As Long) As DataBuffer Implements OpExecutioner.createShapeInfo
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function tadShapeInfoAndOffsets(ByVal array As INDArray, ByVal dimension() As Integer) As TadPack Implements OpExecutioner.tadShapeInfoAndOffsets
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function createConstantBuffer(ByVal values() As Long, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function createConstantBuffer(ByVal values() As Integer, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
			Return createConstantBuffer(ArrayUtil.toLongArray(values), desiredType)
		End Function

		Public Overridable Function createConstantBuffer(ByVal values() As Single, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
			Return createConstantBuffer(ArrayUtil.toDoubles(values), desiredType)
		End Function

		Public Overridable Function createConstantBuffer(ByVal values() As Double, ByVal desiredType As DataType) As DataBuffer Implements OpExecutioner.createConstantBuffer
			Throw New System.NotSupportedException()
		End Function



		Public Overridable Sub setX(ByVal x As INDArray, ByVal op As Op, ByVal oc As OpContext)
			If oc IsNot Nothing Then
				oc.setInputArray(0, x)
			Else
				op.X = x
			End If
		End Sub

		Public Overridable Function getX(ByVal op As Op, ByVal oc As OpContext) As INDArray
			If oc IsNot Nothing Then
				Return oc.getInputArray(0)
			End If
			Return op.x()
		End Function

		Public Overridable Sub setY(ByVal y As INDArray, ByVal op As Op, ByVal oc As OpContext)
			If oc IsNot Nothing Then
				oc.setInputArray(1, y)
			Else
				op.Y = y
			End If
		End Sub

		Public Overridable Function getY(ByVal op As Op, ByVal oc As OpContext) As INDArray
			If oc IsNot Nothing Then
				Return oc.getInputArray(1)
			End If
			Return op.y()
		End Function

		Public Overridable Sub setZ(ByVal z As INDArray, ByVal op As Op, ByVal oc As OpContext)
			If oc IsNot Nothing Then
				oc.setOutputArray(0, z)
			Else
				op.Z = z
			End If
		End Sub

		Public Overridable Function getZ(ByVal op As Op, ByVal oc As OpContext) As INDArray
			If oc IsNot Nothing Then
				Return oc.getOutputArray(0)
			End If
			Return op.z()
		End Function
	End Class

End Namespace