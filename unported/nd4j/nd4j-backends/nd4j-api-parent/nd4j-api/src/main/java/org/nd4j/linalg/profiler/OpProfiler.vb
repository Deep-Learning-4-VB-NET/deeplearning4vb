Imports System
Imports System.Collections.Generic
Imports System.Text
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = org.nd4j.linalg.api.ndarray.INDArrayStatistics
Imports org.nd4j.linalg.api.ops
Imports StackAggregator = org.nd4j.linalg.profiler.data.StackAggregator
Imports StringAggregator = org.nd4j.linalg.profiler.data.StringAggregator
Imports StringCounter = org.nd4j.linalg.profiler.data.StringCounter
import static org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.NONE

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

Namespace org.nd4j.linalg.profiler


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public class OpProfiler
	Public Class OpProfiler

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

		Private listeners As IList(Of OpProfilerListener) = New List(Of OpProfilerListener)()

'JAVA TO VB CONVERTER NOTE: The field invocationsCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private invocationsCount_Conflict As New AtomicLong(0)
		Private Shared ourInstance As New OpProfiler()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ndarray.INDArrayStatistics statistics = new org.nd4j.linalg.api.ndarray.INDArrayStatistics();
		Private statistics As New INDArrayStatistics()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringAggregator classAggergator = new org.nd4j.linalg.profiler.data.StringAggregator();
		Private classAggergator As New StringAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringAggregator longAggergator = new org.nd4j.linalg.profiler.data.StringAggregator();
		Private longAggergator As New StringAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter classCounter = new org.nd4j.linalg.profiler.data.StringCounter();
		Private classCounter As New StringCounter()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter opCounter = new org.nd4j.linalg.profiler.data.StringCounter();
		Private opCounter As New StringCounter()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter classPairsCounter = new org.nd4j.linalg.profiler.data.StringCounter();
		Private classPairsCounter As New StringCounter()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter opPairsCounter = new org.nd4j.linalg.profiler.data.StringCounter();
		Private opPairsCounter As New StringCounter()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter matchingCounter = new org.nd4j.linalg.profiler.data.StringCounter();
		Private matchingCounter As New StringCounter()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter matchingCounterDetailed = new org.nd4j.linalg.profiler.data.StringCounter();
		Private matchingCounterDetailed As New StringCounter()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter matchingCounterInverted = new org.nd4j.linalg.profiler.data.StringCounter();
		Private matchingCounterInverted As New StringCounter()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter orderCounter = new org.nd4j.linalg.profiler.data.StringCounter();
		Private orderCounter As New StringCounter()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator methodsAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
		Private methodsAggregator As New StackAggregator()

		' this aggregator holds getScalar/putScalar entries
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator scalarAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
'JAVA TO VB CONVERTER NOTE: The field scalarAggregator was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private scalarAggregator_Conflict As New StackAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator mixedOrderAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
'JAVA TO VB CONVERTER NOTE: The field mixedOrderAggregator was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private mixedOrderAggregator_Conflict As New StackAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator nonEwsAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
		Private nonEwsAggregator As New StackAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator stridedAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
		Private stridedAggregator As New StackAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator tadStridedAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
		Private tadStridedAggregator As New StackAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator tadNonEwsAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
		Private tadNonEwsAggregator As New StackAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StackAggregator blasAggregator = new org.nd4j.linalg.profiler.data.StackAggregator();
		Private blasAggregator As New StackAggregator()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.profiler.data.StringCounter blasOrderCounter = new org.nd4j.linalg.profiler.data.StringCounter();
		Private blasOrderCounter As New StringCounter()


		Private ReadOnly THRESHOLD As Long = 100000

		Private prevOpClass As String = ""
		Private prevOpName As String = ""

		Private prevOpMatching As String = ""
		Private prevOpMatchingDetailed As String = ""
		Private prevOpMatchingInverted As String = ""
		Private lastZ As Long = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private ProfilerConfig config = ProfilerConfig.builder().build();
		Private config As ProfilerConfig = ProfilerConfig.builder().build()


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
			invocationsCount_Conflict.set(0)

			classAggergator.reset()
			longAggergator.reset()
			classCounter.reset()
			opCounter.reset()
			classPairsCounter.reset()
			opPairsCounter.reset()
			matchingCounter.reset()
			matchingCounterDetailed.reset()
			matchingCounterInverted.reset()
			methodsAggregator.reset()

			scalarAggregator_Conflict.reset()
			nonEwsAggregator.reset()
			stridedAggregator.reset()
			tadNonEwsAggregator.reset()
			tadStridedAggregator.reset()
			mixedOrderAggregator_Conflict.reset()

			blasAggregator.reset()
			blasOrderCounter.reset()

			orderCounter.reset()
			listeners.Clear()
			statistics = INDArrayStatistics.builder().build()
		End Sub


		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public Shared ReadOnly Property Instance As OpProfiler
			Get
				Return ourInstance
			End Get
		End Property

		Private Sub New()

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
			invocationsCount_Conflict.incrementAndGet()
			scalarAggregator_Conflict.incrementCount()
		End Sub

		''' <summary>
		''' This method tracks op calls
		''' </summary>
		''' <param name="op"> </param>
		Public Overridable Sub processOpCall(ByVal op As Op)
			' total number of invocations
			invocationsCount_Conflict.incrementAndGet()

			' number of invocations for this specific op
			opCounter.incrementCount(op.opName())

			' number of invocations for specific class
			Dim opClass As String = getOpClass(op)
			classCounter.incrementCount(opClass)

			If op.x() Is Nothing OrElse (op.x() IsNot Nothing AndAlso op.x().data().platformAddress() = lastZ AndAlso op.z() Is op.x() AndAlso op.y() Is Nothing) Then
				' we have possible shift here
				matchingCounter.incrementCount(prevOpMatching & " -> " & opClass)
				matchingCounterDetailed.incrementCount(prevOpMatchingDetailed & " -> " & opClass & " " & op.opName())
			Else
				matchingCounter.totalsIncrement()
				matchingCounterDetailed.totalsIncrement()
				If op.y() IsNot Nothing AndAlso op.y().data().address() = lastZ Then
					matchingCounterInverted.incrementCount(prevOpMatchingInverted & " -> " & opClass & " " & op.opName())
				Else
					matchingCounterInverted.totalsIncrement()
				End If

			End If
			lastZ = If(op.z() IsNot Nothing, op.z().data().platformAddress(), 0L)
			prevOpMatching = opClass
			prevOpMatchingDetailed = opClass & " " & op.opName()
			prevOpMatchingInverted = opClass & " " & op.opName()

			updatePairs(op.opName(), opClass)

			If config.isNotOptimalArguments() Then
				Dim causes() As PenaltyCause = processOperands(op.x(), op.y(), op.z())
				For Each cause As PenaltyCause In causes
					Select Case cause
						Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.NON_EWS_ACCESS
							nonEwsAggregator.incrementCount()
						Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.STRIDED_ACCESS
							stridedAggregator.incrementCount()
						Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.MIXED_ORDER
							mixedOrderAggregator_Conflict.incrementCount()
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
			invocationsCount_Conflict.incrementAndGet()

			' number of invocations for this specific op
			opCounter.incrementCount(op.opName())

			' number of invocations for specific class
			Dim opClass As String = getOpClass(op)
			classCounter.incrementCount(opClass)


			lastZ = 0
			prevOpMatching = opClass
			prevOpMatchingDetailed = opClass & " " & op.opName()
			prevOpMatchingInverted = opClass & " " & op.opName()

			updatePairs(op.opName(), opClass)

			' TODO: to be implemented
			'for (OpProfilerListener listener : listeners) {
			'  listener.invoke(op);
			'}
		End Sub

		''' 
		''' <param name="op"> </param>
		''' <param name="tadBuffers"> </param>
		Public Overridable Sub processOpCall(ByVal op As Op, ParamArray ByVal tadBuffers() As DataBuffer)
			processOpCall(op)

			Dim causes() As PenaltyCause = processTADOperands(tadBuffers)
			For Each cause As PenaltyCause In causes
				Select Case cause
				   Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.TAD_NON_EWS_ACCESS
					  tadNonEwsAggregator.incrementCount()
				   Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.TAD_STRIDED_ACCESS
					  tadStridedAggregator.incrementCount()
				   Case Else
				End Select
			Next cause
		End Sub

		''' <summary>
		''' Dev-time method.
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable ReadOnly Property MixedOrderAggregator As StackAggregator
			Get
				' FIXME: remove this method, or make it protected
				Return mixedOrderAggregator_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property ScalarAggregator As StackAggregator
			Get
				Return scalarAggregator_Conflict
			End Get
		End Property

		Protected Friend Overridable Sub updatePairs(ByVal opName As String, ByVal opClass As String)
			' now we save pairs of ops/classes
			Dim cOpNameKey As String = prevOpName & " -> " & opName
			Dim cOpClassKey As String = prevOpClass & " -> " & opClass

			classPairsCounter.incrementCount(cOpClassKey)
			opPairsCounter.incrementCount(cOpNameKey)

			prevOpName = opName
			prevOpClass = opClass
		End Sub

		Public Overridable Sub timeOpCall(ByVal op As Op, ByVal startTime As Long)
			Dim currentTime As Long = System.nanoTime() - startTime
			classAggergator.putTime(getOpClass(op), op, currentTime)

			If currentTime > THRESHOLD Then
				Dim keyExt As String = getOpClass(op) & " " & op.opName() & " (" & op.opNum() & ")"
				longAggergator.putTime(keyExt, currentTime)
			End If
		End Sub

		Public Overridable Sub timeOpCall(ByVal op As CustomOp, ByVal startTime As Long)
			Dim currentTime As Long = System.nanoTime() - startTime
			classAggergator.putTime(getOpClass(op), op, currentTime)

			If currentTime > THRESHOLD Then
				Dim keyExt As String = getOpClass(op) & " " & op.opName() & " (" & op.opHash() & ")"
				longAggergator.putTime(keyExt, currentTime)
			End If
		End Sub

		''' <summary>
		''' This method tracks blasCalls
		''' </summary>
		<Obsolete>
		Public Overridable Sub processBlasCall(ByVal blasOpName As String)
			Dim key As String = "BLAS"
			invocationsCount_Conflict.incrementAndGet()

			' using blas function opName as key
			opCounter.incrementCount(blasOpName)

			' all blas calls share the same key
			classCounter.incrementCount(key)

			updatePairs(blasOpName, key)

			prevOpMatching = ""
			lastZ = 0
		End Sub

		Public Overridable Sub timeBlasCall()

		End Sub

		''' <summary>
		''' This method prints out dashboard state
		''' </summary>
		Public Overridable Sub printOutDashboard()
			log.info("---Total Op Calls: {}", invocationsCount_Conflict.get())
			Console.WriteLine()
			log.info("--- OpClass calls statistics: ---")
			Console.WriteLine(classCounter.asString())
			Console.WriteLine()
			log.info("--- OpClass pairs statistics: ---")
			Console.WriteLine(classPairsCounter.asString())
			Console.WriteLine()
			log.info("--- Individual Op calls statistics: ---")
			Console.WriteLine(opCounter.asString())
			Console.WriteLine()
			log.info("--- Matching Op calls statistics: ---")
			Console.WriteLine(matchingCounter.asString())
			Console.WriteLine()
			log.info("--- Matching detailed Op calls statistics: ---")
			Console.WriteLine(matchingCounterDetailed.asString())
			Console.WriteLine()
			log.info("--- Matching inverts Op calls statistics: ---")
			Console.WriteLine(matchingCounterInverted.asString())
			Console.WriteLine()
			log.info("--- Time for OpClass calls statistics: ---")
			Console.WriteLine(classAggergator.asString())
			Console.WriteLine()
			log.info("--- Time for long Op calls statistics: ---")
			Console.WriteLine(longAggergator.asString())
			Console.WriteLine()
			log.info("--- Time spent for Op calls statistics: ---")
			Console.WriteLine(classAggergator.asPercentageString())
			Console.WriteLine()
			log.info("--- Time spent for long Op calls statistics: ---")
			Console.WriteLine(longAggergator.asPercentageString())
			Console.WriteLine()
			log.info("--- Time spent within methods: ---")
			methodsAggregator.renderTree(True)
			Console.WriteLine()
			log.info("--- Bad strides stack tree: ---")
			Console.WriteLine("Unique entries: " & stridedAggregator.UniqueBranchesNumber)
			stridedAggregator.renderTree()
			Console.WriteLine()
			log.info("--- non-EWS access stack tree: ---")
			Console.WriteLine("Unique entries: " & nonEwsAggregator.UniqueBranchesNumber)
			nonEwsAggregator.renderTree()
			Console.WriteLine()
			log.info("--- Mixed orders access stack tree: ---")
			Console.WriteLine("Unique entries: " & mixedOrderAggregator_Conflict.UniqueBranchesNumber)
			mixedOrderAggregator_Conflict.renderTree()
			Console.WriteLine()
			log.info("--- TAD bad strides stack tree: ---")
			Console.WriteLine("Unique entries: " & tadStridedAggregator.UniqueBranchesNumber)
			tadStridedAggregator.renderTree()
			Console.WriteLine()
			log.info("--- TAD non-EWS access stack tree: ---")
			Console.WriteLine("Unique entries: " & tadNonEwsAggregator.UniqueBranchesNumber)
			tadNonEwsAggregator.renderTree()
			Console.WriteLine()
			log.info("--- Scalar access stack tree: ---")
			Console.WriteLine("Unique entries: " & scalarAggregator_Conflict.UniqueBranchesNumber)
			scalarAggregator_Conflict.renderTree(False)
			Console.WriteLine()
			log.info("--- Blas GEMM odrders count: ---")
			Console.WriteLine(blasOrderCounter.asString())
			Console.WriteLine()
			log.info("--- BLAS access stack trace: ---")
			Console.WriteLine("Unique entries: " & blasAggregator.UniqueBranchesNumber)
			blasAggregator.renderTree(False)
			Console.WriteLine()

		End Sub



		Public Overridable ReadOnly Property InvocationsCount As Long
			Get
				Return invocationsCount_Conflict.get()
			End Get
		End Property



		''' <summary>
		''' This method builds </summary>
		''' <param name="op"> </param>
		Public Overridable Sub processStackCall(ByVal op As Op, ByVal timeStart As Long)
			'StackTraceElement stack[] = Thread.currentThread().getStackTrace();

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim timeSpent As Long = (System.nanoTime() - timeStart) / 1000

	'        
	'           basically we want to unroll stack trace for few levels ABOVE nd4j classes
	'           and update invocations list for last few levels, to keep that stat on few levels
	'         

			methodsAggregator.incrementCount(timeSpent)
		End Sub

		Public Overridable Sub processStackCall(ByVal op As CustomOp, ByVal timeStart As Long)
			'StackTraceElement stack[] = Thread.currentThread().getStackTrace();

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim timeSpent As Long = (System.nanoTime() - timeStart) / 1000

	'        
	'           basically we want to unroll stack trace for few levels ABOVE nd4j classes
	'           and update invocations list for last few levels, to keep that stat on few levels
	'         

			methodsAggregator.incrementCount(timeSpent)
		End Sub


		Public Overridable Function processOrders(ParamArray ByVal operands() As INDArray) As String
			Dim buffer As New StringBuilder()

			For e As Integer = 0 To operands.Length - 1

				If operands(e) Is Nothing Then
					buffer.Append("null")
				Else
					buffer.Append(("" & operands(e).ordering()).ToUpper())
				End If

				If e < operands.Length - 1 Then
					buffer.Append(" x ")
				End If
			Next e

			orderCounter.incrementCount(buffer.ToString())

			Return buffer.ToString()
		End Function

		Public Overridable Sub processBlasCall(ByVal isGemm As Boolean, ParamArray ByVal operands() As INDArray)

			If isGemm Then
				''' <summary>
				''' but for gemm we also care about equal orders case: FF, CC
				''' </summary>
				Dim key As String = processOrders(operands)
				blasOrderCounter.incrementCount(key)

				Dim causes() As PenaltyCause = processOperands(operands)
				For Each cause As PenaltyCause In causes
					Select Case cause
						Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.NON_EWS_ACCESS, STRIDED_ACCESS, NONE
							blasAggregator.incrementCount()
						Case Else
					End Select
				Next cause

			Else
				''' 
				''' <summary>
				''' by default we only care about strides.
				''' 
				''' </summary>

				Dim causes() As PenaltyCause = processOperands(operands)
				For Each cause As PenaltyCause In causes
					Select Case cause
						Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.NON_EWS_ACCESS
							nonEwsAggregator.incrementCount()
						Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.STRIDED_ACCESS
							stridedAggregator.incrementCount()
						Case org.nd4j.linalg.profiler.OpProfiler.PenaltyCause.MIXED_ORDER
							mixedOrderAggregator_Conflict.incrementCount()
						Case Else
					End Select
				Next cause
			End If
		End Sub

		Public Overridable Function processOperands(ByVal x As INDArray, ByVal y As INDArray) As PenaltyCause()
			Dim penalties As IList(Of PenaltyCause) = New List(Of PenaltyCause)()

			If x IsNot Nothing AndAlso x.ordering() <> y.ordering() Then
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
				penalties.Add(NONE)
			End If

			Return CType(penalties, List(Of PenaltyCause)).ToArray()
		End Function

		Public Overridable Function processTADOperands(ParamArray ByVal tadBuffers() As DataBuffer) As PenaltyCause()

			Dim causes As IList(Of PenaltyCause) = New List(Of PenaltyCause)()
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
				causes.Add(NONE)
			End If

			Return CType(causes, List(Of PenaltyCause)).ToArray()
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

				If (causeXY.Length = 1 AndAlso causeXY(0) = NONE) AndAlso (causeXZ.Length = 1 AndAlso causeXZ(0) = NONE) Then
					Return causeXY
				ElseIf causeXY.Length = 1 AndAlso causeXY(0) = NONE Then
					Return causeXZ
				ElseIf causeXZ.Length = 1 AndAlso causeXZ(0) = NONE Then
					Return causeXY
				Else
					Return joinDistinct(causeXY, causeXZ)
				End If
			End If
		End Function

		Protected Friend Overridable Function joinDistinct(ByVal a() As PenaltyCause, ByVal b() As PenaltyCause) As PenaltyCause()
			Dim causes As IList(Of PenaltyCause) = New List(Of PenaltyCause)()

			For Each cause As PenaltyCause In a
				If cause <> Nothing AndAlso Not causes.Contains(cause) Then
					causes.Add(cause)
				End If
			Next cause

			For Each cause As PenaltyCause In b
				If cause <> Nothing AndAlso Not causes.Contains(cause) Then
					causes.Add(cause)
				End If
			Next cause

			Return CType(causes, List(Of PenaltyCause)).ToArray()
		End Function

		''' <summary>
		''' This method checks for something somewhere
		''' </summary>
		''' <param name="operands"> </param>
		Public Overridable Function processOperands(ParamArray ByVal operands() As INDArray) As PenaltyCause()
			If operands Is Nothing Then
				Return New PenaltyCause() {NONE}
			End If

			Dim causes As IList(Of PenaltyCause) = New List(Of PenaltyCause)()
			For e As Integer = 0 To operands.Length - 2
				If operands(e) Is Nothing AndAlso operands(e + 1) Is Nothing Then
					Continue For
				End If

				Dim lc() As PenaltyCause = processOperands(operands(e), operands(e + 1))

				For Each cause As PenaltyCause In lc
					If cause <> NONE AndAlso Not causes.Contains(cause) Then
						causes.Add(cause)
					End If
				Next cause
			Next e
			If causes.Count = 0 Then
				causes.Add(NONE)
			End If

			Return CType(causes, List(Of PenaltyCause)).ToArray()
		End Function

		Public Overridable Sub processMemoryAccess()

		End Sub
	End Class

End Namespace