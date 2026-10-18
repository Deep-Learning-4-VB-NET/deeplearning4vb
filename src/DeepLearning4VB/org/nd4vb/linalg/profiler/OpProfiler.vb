Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArrayStatistics
Imports BroadcastOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BroadcastOp
Imports CustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.CustomOp
Imports GridOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.GridOp
Imports IndexAccumulation = DeepLearning4VB.org.nd4vb.linalg.api.ops.IndexAccumulation
Imports MetaOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.MetaOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
Imports RandomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.RandomOp
Imports ReduceOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.ReduceOp
Imports ScalarOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.ScalarOp
Imports TransformOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.TransformOp
Imports StackAggregator = DeepLearning4VB.org.nd4vb.linalg.profiler.data.StackAggregator
Imports StringAggregator = DeepLearning4VB.org.nd4vb.linalg.profiler.data.StringAggregator
Imports StringCounter = DeepLearning4VB.org.nd4vb.linalg.profiler.data.StringCounter
Imports ComparableAtomicLong = DeepLearning4VB.org.nd4vb.linalg.profiler.data.primitives.ComparableAtomicLong

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

Namespace org.nd4vb.linalg.profiler

    ''' <summary>
    ''' The process-wide op profiler (Java <c>OpProfiler</c>): counts op invocations per op and op class, op pairs,
    ''' op timings, BLAS orders and the call stacks of operands with bad strides.
    ''' <para>
    ''' Differences from Java: Lombok's <c>@Getter</c> fields are read-only properties (<see cref="Statistics"/>,
    ''' <see cref="ClassAggergator"/>, ...; Java's spelling is kept) and <c>@Setter config</c> is <see cref="Config"/>;
    ''' the <c>@Data</c> <c>equals</c>/<c>hashCode</c> of the singleton are not written (reference equality). Java
    ''' detected that an op works on the output of the previous op by comparing native buffer addresses
    ''' (<c>platformAddress()</c>, deleted as native-only); here the data buffers are compared by reference. A
    ''' <c>Nothing</c> second operand of the two-operand <see cref="processOperands(INDArray, INDArray)"/> is
    ''' skipped (Java threw a <c>NullPointerException</c>).
    ''' </para>
    ''' </summary>
    Public Class OpProfiler

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(OpProfiler))

        Public Enum PenaltyCause
            NONE
            NON_EWS_ACCESS
            STRIDED_ACCESS
            MIXED_ORDER
            TAD_NON_EWS_ACCESS
            TAD_STRIDED_ACCESS
        End Enum

        Public Interface OpProfilerListener
            Sub invoke(ByVal op As Op)
        End Interface

        Private ReadOnly listeners As IList(Of OpProfilerListener) = New List(Of OpProfilerListener)()

        Private ReadOnly invocationsCountValue As New ComparableAtomicLong(0)
        Private Shared ReadOnly ourInstance As New OpProfiler()

        Private statisticsValue As New INDArrayStatistics()

        Private ReadOnly classAggergatorValue As New StringAggregator()
        Private ReadOnly longAggergatorValue As New StringAggregator()
        Private ReadOnly classCounterValue As New StringCounter()
        Private ReadOnly opCounterValue As New StringCounter()
        Private ReadOnly classPairsCounterValue As New StringCounter()
        Private ReadOnly opPairsCounterValue As New StringCounter()

        Private ReadOnly matchingCounterValue As New StringCounter()
        Private ReadOnly matchingCounterDetailedValue As New StringCounter()
        Private ReadOnly matchingCounterInvertedValue As New StringCounter()
        Private ReadOnly orderCounterValue As New StringCounter()
        Private ReadOnly methodsAggregatorValue As New StackAggregator()

        ' this aggregator holds getScalar/putScalar entries
        Private ReadOnly scalarAggregatorValue As New StackAggregator()
        Private ReadOnly mixedOrderAggregatorValue As New StackAggregator()
        Private ReadOnly nonEwsAggregatorValue As New StackAggregator()
        Private ReadOnly stridedAggregatorValue As New StackAggregator()
        Private ReadOnly tadStridedAggregatorValue As New StackAggregator()
        Private ReadOnly tadNonEwsAggregatorValue As New StackAggregator()
        Private ReadOnly blasAggregatorValue As New StackAggregator()
        Private ReadOnly blasOrderCounterValue As New StringCounter()

        Private ReadOnly THRESHOLD As Long = 100000

        Private prevOpClass As String = ""
        Private prevOpName As String = ""

        Private prevOpMatching As String = ""
        Private prevOpMatchingDetailed As String = ""
        Private prevOpMatchingInverted As String = ""
        ' Java: the native address of the last z buffer (0 = none)
        Private lastZ As WeakReference(Of DataBuffer)

        Private configValue As ProfilerConfig = ProfilerConfig.builder().build()

        Private Sub New()
        End Sub

        ''' <summary>The profiler instance.</summary>
        Public Shared ReadOnly Property Instance As OpProfiler
            Get
                Return ourInstance
            End Get
        End Property

        ''' <summary>Java <c>getInstance()</c>.</summary>
        Public Shared Function getInstance() As OpProfiler
            Return ourInstance
        End Function

        ''' <summary>The array statistics (Java <c>getStatistics()</c>).</summary>
        Public Overridable ReadOnly Property Statistics As INDArrayStatistics
            Get
                Return statisticsValue
            End Get
        End Property

        Public Overridable ReadOnly Property ClassAggergator As StringAggregator
            Get
                Return classAggergatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property LongAggergator As StringAggregator
            Get
                Return longAggergatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property ClassCounter As StringCounter
            Get
                Return classCounterValue
            End Get
        End Property

        Public Overridable ReadOnly Property OpCounter As StringCounter
            Get
                Return opCounterValue
            End Get
        End Property

        Public Overridable ReadOnly Property ClassPairsCounter As StringCounter
            Get
                Return classPairsCounterValue
            End Get
        End Property

        Public Overridable ReadOnly Property OpPairsCounter As StringCounter
            Get
                Return opPairsCounterValue
            End Get
        End Property

        Public Overridable ReadOnly Property MatchingCounter As StringCounter
            Get
                Return matchingCounterValue
            End Get
        End Property

        Public Overridable ReadOnly Property MatchingCounterDetailed As StringCounter
            Get
                Return matchingCounterDetailedValue
            End Get
        End Property

        Public Overridable ReadOnly Property MatchingCounterInverted As StringCounter
            Get
                Return matchingCounterInvertedValue
            End Get
        End Property

        Public Overridable ReadOnly Property OrderCounter As StringCounter
            Get
                Return orderCounterValue
            End Get
        End Property

        Public Overridable ReadOnly Property MethodsAggregator As StackAggregator
            Get
                Return methodsAggregatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property ScalarAggregator As StackAggregator
            Get
                Return scalarAggregatorValue
            End Get
        End Property

        ''' <summary>Dev-time member.</summary>
        Public Overridable ReadOnly Property MixedOrderAggregator As StackAggregator
            Get
                Return mixedOrderAggregatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property NonEwsAggregator As StackAggregator
            Get
                Return nonEwsAggregatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property StridedAggregator As StackAggregator
            Get
                Return stridedAggregatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property TadStridedAggregator As StackAggregator
            Get
                Return tadStridedAggregatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property TadNonEwsAggregator As StackAggregator
            Get
                Return tadNonEwsAggregatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property BlasAggregator As StackAggregator
            Get
                Return blasAggregatorValue
            End Get
        End Property

        Public Overridable ReadOnly Property BlasOrderCounter As StringCounter
            Get
                Return blasOrderCounterValue
            End Get
        End Property

        ''' <summary>The profiler configuration (Java <c>getConfig()</c>/<c>setConfig(...)</c>).</summary>
        Public Overridable Property Config As ProfilerConfig
            Get
                Return configValue
            End Get
            Set(ByVal value As ProfilerConfig)
                configValue = value
            End Set
        End Property

        ''' <summary>The total number of tracked calls.</summary>
        Public Overridable ReadOnly Property InvocationsCount As Long
            Get
                Return invocationsCountValue.get()
            End Get
        End Property

        ''' <summary>
        ''' Clear the listener from the profiler </summary>
        ''' <param name="listener"> the listener to clear </param>
        Public Overridable Sub clearListener(ByVal listener As OpProfilerListener)
            listeners.Remove(listener)
        End Sub

        ''' <summary>
        ''' dd the listener to the profiler </summary>
        ''' <param name="listener"> the listener to add </param>
        Public Overridable Sub addListener(ByVal listener As OpProfilerListener)
            listeners.Add(listener)
        End Sub

        ''' <summary>
        ''' This method resets all counters
        ''' </summary>
        Public Overridable Sub reset()
            invocationsCountValue.set(0)

            classAggergatorValue.reset()
            longAggergatorValue.reset()
            classCounterValue.reset()
            opCounterValue.reset()
            classPairsCounterValue.reset()
            opPairsCounterValue.reset()
            matchingCounterValue.reset()
            matchingCounterDetailedValue.reset()
            matchingCounterInvertedValue.reset()
            methodsAggregatorValue.reset()

            scalarAggregatorValue.reset()
            nonEwsAggregatorValue.reset()
            stridedAggregatorValue.reset()
            tadNonEwsAggregatorValue.reset()
            tadStridedAggregatorValue.reset()
            mixedOrderAggregatorValue.reset()

            blasAggregatorValue.reset()
            blasOrderCounterValue.reset()

            orderCounterValue.reset()
            listeners.Clear()
            statisticsValue = INDArrayStatistics.builder().build()
        End Sub

        ''' <summary>
        ''' This method returns op class opName
        ''' </summary>
        ''' <param name="op">
        ''' @return </param>
        Protected Friend Overridable Function getOpClass(ByVal op As Op) As String
            If TypeOf op Is ScalarOp Then
                Return "ScalarOp"
            ElseIf TypeOf op Is MetaOp Then
                Return "MetaOp"
            ElseIf TypeOf op Is GridOp Then
                Return "GridOp"
            ElseIf TypeOf op Is BroadcastOp Then
                Return "BroadcastOp"
            ElseIf TypeOf op Is RandomOp Then
                Return "RandomOp"
            ElseIf TypeOf op Is ReduceOp Then
                Return "AccumulationOp"
            ElseIf TypeOf op Is TransformOp Then
                If op.y() Is Nothing Then
                    Return "TransformOp"
                Else
                    Return "PairWiseTransformOp"
                End If
            ElseIf TypeOf op Is IndexAccumulation Then
                Return "IndexAccumulationOp"
            ElseIf TypeOf op Is CustomOp Then
                Return "CustomOp"
            Else
                Return "Unknown Op calls"
            End If
        End Function

        Protected Friend Overridable Function getOpClass(ByVal op As CustomOp) As String
            Return "CustomOp"
        End Function

        ''' <summary>
        ''' This method tracks INDArray.putScalar()/getScalar() calls
        ''' </summary>
        Public Overridable Sub processScalarCall()
            invocationsCountValue.incrementAndGet()
            scalarAggregatorValue.incrementCount()
        End Sub

        Private Function isLastZ(ByVal buffer As DataBuffer) As Boolean
            Dim last As DataBuffer = Nothing
            Return buffer IsNot Nothing AndAlso lastZ IsNot Nothing AndAlso lastZ.TryGetTarget(last) AndAlso last Is buffer
        End Function

        ''' <summary>
        ''' This method tracks op calls
        ''' </summary>
        ''' <param name="op"> </param>
        Public Overridable Sub processOpCall(ByVal op As Op)
            ' total number of invocations
            invocationsCountValue.incrementAndGet()

            ' number of invocations for this specific op
            opCounterValue.incrementCount(op.opName())

            ' number of invocations for specific class
            Dim opClass As String = getOpClass(op)
            classCounterValue.incrementCount(opClass)

            If op.x() Is Nothing OrElse (op.x() IsNot Nothing AndAlso isLastZ(op.x().data()) AndAlso op.z() Is op.x() AndAlso op.y() Is Nothing) Then
                ' we have possible shift here
                matchingCounterValue.incrementCount(prevOpMatching & " -> " & opClass)
                matchingCounterDetailedValue.incrementCount(prevOpMatchingDetailed & " -> " & opClass & " " & op.opName())
            Else
                matchingCounterValue.totalsIncrement()
                matchingCounterDetailedValue.totalsIncrement()
                If op.y() IsNot Nothing AndAlso isLastZ(op.y().data()) Then
                    matchingCounterInvertedValue.incrementCount(prevOpMatchingInverted & " -> " & opClass & " " & op.opName())
                Else
                    matchingCounterInvertedValue.totalsIncrement()
                End If

            End If
            lastZ = If(op.z() IsNot Nothing, New WeakReference(Of DataBuffer)(op.z().data()), Nothing)
            prevOpMatching = opClass
            prevOpMatchingDetailed = opClass & " " & op.opName()
            prevOpMatchingInverted = opClass & " " & op.opName()

            updatePairs(op.opName(), opClass)

            If configValue.isNotOptimalArguments() Then
                Dim causes() As PenaltyCause = processOperands(op.x(), op.y(), op.z())
                For Each cause As PenaltyCause In causes
                    Select Case cause
                        Case PenaltyCause.NON_EWS_ACCESS
                            nonEwsAggregatorValue.incrementCount()
                        Case PenaltyCause.STRIDED_ACCESS
                            stridedAggregatorValue.incrementCount()
                        Case PenaltyCause.MIXED_ORDER
                            mixedOrderAggregatorValue.incrementCount()
                        Case Else
                    End Select
                Next cause
            End If

            For Each listener As OpProfilerListener In listeners
                listener.invoke(op)
            Next listener
        End Sub

        ''' <summary>
        ''' This method tracks op calls
        ''' </summary>
        ''' <param name="op"> </param>
        Public Overridable Sub processOpCall(ByVal op As CustomOp)
            ' total number of invocations
            invocationsCountValue.incrementAndGet()

            ' number of invocations for this specific op
            opCounterValue.incrementCount(op.opName())

            ' number of invocations for specific class
            Dim opClass As String = getOpClass(op)
            classCounterValue.incrementCount(opClass)

            lastZ = Nothing
            prevOpMatching = opClass
            prevOpMatchingDetailed = opClass & " " & op.opName()
            prevOpMatchingInverted = opClass & " " & op.opName()

            updatePairs(op.opName(), opClass)

            ' TODO: to be implemented
            'for (OpProfilerListener listener : listeners) {
            '  listener.invoke(op);
            '}
        End Sub

        ''' <param name="op"> </param>
        ''' <param name="tadBuffers"> </param>
        Public Overridable Sub processOpCall(ByVal op As Op, ParamArray ByVal tadBuffers() As DataBuffer)
            processOpCall(op)

            Dim causes() As PenaltyCause = processTADOperands(tadBuffers)
            For Each cause As PenaltyCause In causes
                Select Case cause
                    Case PenaltyCause.TAD_NON_EWS_ACCESS
                        tadNonEwsAggregatorValue.incrementCount()
                    Case PenaltyCause.TAD_STRIDED_ACCESS
                        tadStridedAggregatorValue.incrementCount()
                    Case Else
                End Select
            Next cause
        End Sub

        Protected Friend Overridable Sub updatePairs(ByVal opName As String, ByVal opClass As String)
            ' now we save pairs of ops/classes
            Dim cOpNameKey As String = prevOpName & " -> " & opName
            Dim cOpClassKey As String = prevOpClass & " -> " & opClass

            classPairsCounterValue.incrementCount(cOpClassKey)
            opPairsCounterValue.incrementCount(cOpNameKey)

            prevOpName = opName
            prevOpClass = opClass
        End Sub

        Public Overridable Sub timeOpCall(ByVal op As Op, ByVal startTime As Long)
            Dim currentTime As Long = JavaSystem.nanoTime() - startTime
            classAggergatorValue.putTime(getOpClass(op), op, currentTime)

            If currentTime > THRESHOLD Then
                Dim keyExt As String = getOpClass(op) & " " & op.opName() & " (" & op.opNum() & ")"
                longAggergatorValue.putTime(keyExt, currentTime)
            End If
        End Sub

        Public Overridable Sub timeOpCall(ByVal op As CustomOp, ByVal startTime As Long)
            Dim currentTime As Long = JavaSystem.nanoTime() - startTime
            classAggergatorValue.putTime(getOpClass(op), op, currentTime)

            If currentTime > THRESHOLD Then
                Dim keyExt As String = getOpClass(op) & " " & op.opName() & " (" & op.opHash() & ")"
                longAggergatorValue.putTime(keyExt, currentTime)
            End If
        End Sub

        ''' <summary>
        ''' This method tracks blasCalls
        ''' </summary>
        <Obsolete("Deprecated in Java")>
        Public Overridable Sub processBlasCall(ByVal blasOpName As String)
            Dim key As String = "BLAS"
            invocationsCountValue.incrementAndGet()

            ' using blas function opName as key
            opCounterValue.incrementCount(blasOpName)

            ' all blas calls share the same key
            classCounterValue.incrementCount(key)

            updatePairs(blasOpName, key)

            prevOpMatching = ""
            lastZ = Nothing
        End Sub

        Public Overridable Sub timeBlasCall()

        End Sub

        ''' <summary>
        ''' This method prints out dashboard state
        ''' </summary>
        Public Overridable Sub printOutDashboard()
            log.info("---Total Op Calls: {}", invocationsCountValue.get())
            Console.WriteLine()
            log.info("--- OpClass calls statistics: ---")
            Console.WriteLine(classCounterValue.asString())
            Console.WriteLine()
            log.info("--- OpClass pairs statistics: ---")
            Console.WriteLine(classPairsCounterValue.asString())
            Console.WriteLine()
            log.info("--- Individual Op calls statistics: ---")
            Console.WriteLine(opCounterValue.asString())
            Console.WriteLine()
            log.info("--- Matching Op calls statistics: ---")
            Console.WriteLine(matchingCounterValue.asString())
            Console.WriteLine()
            log.info("--- Matching detailed Op calls statistics: ---")
            Console.WriteLine(matchingCounterDetailedValue.asString())
            Console.WriteLine()
            log.info("--- Matching inverts Op calls statistics: ---")
            Console.WriteLine(matchingCounterInvertedValue.asString())
            Console.WriteLine()
            log.info("--- Time for OpClass calls statistics: ---")
            Console.WriteLine(classAggergatorValue.asString())
            Console.WriteLine()
            log.info("--- Time for long Op calls statistics: ---")
            Console.WriteLine(longAggergatorValue.asString())
            Console.WriteLine()
            log.info("--- Time spent for Op calls statistics: ---")
            Console.WriteLine(classAggergatorValue.asPercentageString())
            Console.WriteLine()
            log.info("--- Time spent for long Op calls statistics: ---")
            Console.WriteLine(longAggergatorValue.asPercentageString())
            Console.WriteLine()
            log.info("--- Time spent within methods: ---")
            methodsAggregatorValue.renderTree(True)
            Console.WriteLine()
            log.info("--- Bad strides stack tree: ---")
            Console.WriteLine("Unique entries: " & stridedAggregatorValue.UniqueBranchesNumber)
            stridedAggregatorValue.renderTree()
            Console.WriteLine()
            log.info("--- non-EWS access stack tree: ---")
            Console.WriteLine("Unique entries: " & nonEwsAggregatorValue.UniqueBranchesNumber)
            nonEwsAggregatorValue.renderTree()
            Console.WriteLine()
            log.info("--- Mixed orders access stack tree: ---")
            Console.WriteLine("Unique entries: " & mixedOrderAggregatorValue.UniqueBranchesNumber)
            mixedOrderAggregatorValue.renderTree()
            Console.WriteLine()
            log.info("--- TAD bad strides stack tree: ---")
            Console.WriteLine("Unique entries: " & tadStridedAggregatorValue.UniqueBranchesNumber)
            tadStridedAggregatorValue.renderTree()
            Console.WriteLine()
            log.info("--- TAD non-EWS access stack tree: ---")
            Console.WriteLine("Unique entries: " & tadNonEwsAggregatorValue.UniqueBranchesNumber)
            tadNonEwsAggregatorValue.renderTree()
            Console.WriteLine()
            log.info("--- Scalar access stack tree: ---")
            Console.WriteLine("Unique entries: " & scalarAggregatorValue.UniqueBranchesNumber)
            scalarAggregatorValue.renderTree(False)
            Console.WriteLine()
            log.info("--- Blas GEMM odrders count: ---")
            Console.WriteLine(blasOrderCounterValue.asString())
            Console.WriteLine()
            log.info("--- BLAS access stack trace: ---")
            Console.WriteLine("Unique entries: " & blasAggregatorValue.UniqueBranchesNumber)
            blasAggregatorValue.renderTree(False)
            Console.WriteLine()
        End Sub

        ''' <summary>
        ''' This method builds </summary>
        ''' <param name="op"> </param>
        Public Overridable Sub processStackCall(ByVal op As Op, ByVal timeStart As Long)
            Dim timeSpent As Long = (JavaSystem.nanoTime() - timeStart) \ 1000

            '
            '           basically we want to unroll stack trace for few levels ABOVE nd4j classes
            '           and update invocations list for last few levels, to keep that stat on few levels
            '
            methodsAggregatorValue.incrementCount(timeSpent)
        End Sub

        Public Overridable Sub processStackCall(ByVal op As CustomOp, ByVal timeStart As Long)
            Dim timeSpent As Long = (JavaSystem.nanoTime() - timeStart) \ 1000

            methodsAggregatorValue.incrementCount(timeSpent)
        End Sub

        Public Overridable Function processOrders(ParamArray ByVal operands() As INDArray) As String
            Dim buffer As New StringBuilder()

            For e As Integer = 0 To operands.Length - 1

                If operands(e) Is Nothing Then
                    buffer.Append("null")
                Else
                    buffer.Append(Char.ToUpperInvariant(operands(e).ordering()))
                End If

                If e < operands.Length - 1 Then
                    buffer.Append(" x ")
                End If
            Next e

            orderCounterValue.incrementCount(buffer.ToString())

            Return buffer.ToString()
        End Function

        Public Overridable Sub processBlasCall(ByVal isGemm As Boolean, ParamArray ByVal operands() As INDArray)

            If isGemm Then
                ' but for gemm we also care about equal orders case: FF, CC
                Dim key As String = processOrders(operands)
                blasOrderCounterValue.incrementCount(key)

                Dim causes() As PenaltyCause = processOperands(operands)
                For Each cause As PenaltyCause In causes
                    Select Case cause
                        Case PenaltyCause.NON_EWS_ACCESS, PenaltyCause.STRIDED_ACCESS, PenaltyCause.NONE
                            blasAggregatorValue.incrementCount()
                        Case Else
                    End Select
                Next cause

            Else
                ' by default we only care about strides.
                Dim causes() As PenaltyCause = processOperands(operands)
                For Each cause As PenaltyCause In causes
                    Select Case cause
                        Case PenaltyCause.NON_EWS_ACCESS
                            nonEwsAggregatorValue.incrementCount()
                        Case PenaltyCause.STRIDED_ACCESS
                            stridedAggregatorValue.incrementCount()
                        Case PenaltyCause.MIXED_ORDER
                            mixedOrderAggregatorValue.incrementCount()
                        Case Else
                    End Select
                Next cause
            End If
        End Sub

        Public Overridable Function processOperands(ByVal x As INDArray, ByVal y As INDArray) As PenaltyCause()
            Dim penalties As New List(Of PenaltyCause)()

            If x IsNot Nothing AndAlso y IsNot Nothing AndAlso x.ordering() <> y.ordering() Then
                penalties.Add(PenaltyCause.MIXED_ORDER)
            End If

            If x IsNot Nothing AndAlso x.elementWiseStride() < 1 Then
                penalties.Add(PenaltyCause.NON_EWS_ACCESS)
            ElseIf y IsNot Nothing AndAlso y.elementWiseStride() < 1 Then
                penalties.Add(PenaltyCause.NON_EWS_ACCESS)
            End If

            If x IsNot Nothing AndAlso x.elementWiseStride() > 1 Then
                penalties.Add(PenaltyCause.STRIDED_ACCESS)
            ElseIf y IsNot Nothing AndAlso y.elementWiseStride() > 1 Then
                penalties.Add(PenaltyCause.STRIDED_ACCESS)
            End If

            If penalties.Count = 0 Then
                penalties.Add(PenaltyCause.NONE)
            End If

            Return penalties.ToArray()
        End Function

        Public Overridable Function processTADOperands(ParamArray ByVal tadBuffers() As DataBuffer) As PenaltyCause()

            Dim causes As New List(Of PenaltyCause)()
            For Each tadBuffer As DataBuffer In tadBuffers
                If tadBuffer Is Nothing Then
                    Continue For
                End If

                Dim rank As Integer = tadBuffer.getInt(0)
                Dim length As Integer = rank * 2 + 4
                Dim ews As Integer = tadBuffer.getInt(length - 2)

                If (ews < 1 OrElse rank > 2 OrElse (rank = 2 AndAlso tadBuffer.getInt(1) > 1 AndAlso tadBuffer.getInt(2) > 1)) AndAlso Not causes.Contains(PenaltyCause.TAD_NON_EWS_ACCESS) Then
                    causes.Add(PenaltyCause.TAD_NON_EWS_ACCESS)
                ElseIf ews > 1 AndAlso Not causes.Contains(PenaltyCause.TAD_STRIDED_ACCESS) Then
                    causes.Add(PenaltyCause.TAD_STRIDED_ACCESS)
                End If
            Next tadBuffer

            If causes.Count = 0 Then
                causes.Add(PenaltyCause.NONE)
            End If

            Return causes.ToArray()
        End Function

        Public Overridable Function processOperands(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray) As PenaltyCause()
            If y Is Nothing Then
                Return processOperands(x, z)
            End If

            If x Is z OrElse y Is z Then
                Return processOperands(x, y)
            Else
                Dim causeXY() As PenaltyCause = processOperands(x, y)
                Dim causeXZ() As PenaltyCause = processOperands(x, z)

                If (causeXY.Length = 1 AndAlso causeXY(0) = PenaltyCause.NONE) AndAlso (causeXZ.Length = 1 AndAlso causeXZ(0) = PenaltyCause.NONE) Then
                    Return causeXY
                ElseIf causeXY.Length = 1 AndAlso causeXY(0) = PenaltyCause.NONE Then
                    Return causeXZ
                ElseIf causeXZ.Length = 1 AndAlso causeXZ(0) = PenaltyCause.NONE Then
                    Return causeXY
                Else
                    Return joinDistinct(causeXY, causeXZ)
                End If
            End If
        End Function

        Protected Friend Overridable Function joinDistinct(ByVal a() As PenaltyCause, ByVal b() As PenaltyCause) As PenaltyCause()
            Dim causes As New List(Of PenaltyCause)()

            For Each cause As PenaltyCause In a
                If Not causes.Contains(cause) Then
                    causes.Add(cause)
                End If
            Next cause

            For Each cause As PenaltyCause In b
                If Not causes.Contains(cause) Then
                    causes.Add(cause)
                End If
            Next cause

            Return causes.ToArray()
        End Function

        ''' <summary>
        ''' This method checks for something somewhere
        ''' </summary>
        ''' <param name="operands"> </param>
        Public Overridable Function processOperands(ParamArray ByVal operands() As INDArray) As PenaltyCause()
            If operands Is Nothing Then
                Return New PenaltyCause() {PenaltyCause.NONE}
            End If

            Dim causes As New List(Of PenaltyCause)()
            For e As Integer = 0 To operands.Length - 2
                If operands(e) Is Nothing AndAlso operands(e + 1) Is Nothing Then
                    Continue For
                End If

                Dim lc() As PenaltyCause = processOperands(operands(e), operands(e + 1))

                For Each cause As PenaltyCause In lc
                    If cause <> PenaltyCause.NONE AndAlso Not causes.Contains(cause) Then
                        causes.Add(cause)
                    End If
                Next cause
            Next e
            If causes.Count = 0 Then
                causes.Add(PenaltyCause.NONE)
            End If

            Return causes.ToArray()
        End Function

        Public Overridable Sub processMemoryAccess()

        End Sub

    End Class

End Namespace
