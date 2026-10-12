Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.optimize.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class PerformanceListener extends org.deeplearning4j.optimize.api.BaseTrainingListener implements java.io.Serializable
	<Serializable>
	Public Class PerformanceListener
		Inherits BaseTrainingListener

		Private ReadOnly frequency As Integer
		<NonSerialized>
		Private samplesPerSec As New ThreadLocal(Of Double)()
		<NonSerialized>
		Private batchesPerSec As New ThreadLocal(Of Double)()
		<NonSerialized>
		Private lastTime As New ThreadLocal(Of Long)()
		<NonSerialized>
		Private lastGcCount As New ThreadLocal(Of IDictionary(Of String, Long))()
		<NonSerialized>
		Private lastGcMs As New ThreadLocal(Of IDictionary(Of String, Long))()
		<NonSerialized>
		Private gcBeans As IList(Of GarbageCollectorMXBean) = Nothing

		Private reportScore As Boolean
		Private reportGC As Boolean
		Private reportSample As Boolean = True
		Private reportBatch As Boolean = True
		Private reportIteration As Boolean = True
		Private reportEtl As Boolean = True
		Private reportTime As Boolean = True



		Public Sub New(ByVal frequency As Integer)
			Me.New(frequency, False)
		End Sub

		Public Sub New(ByVal frequency As Integer, ByVal reportScore As Boolean)
			Me.New(frequency, reportScore, False)
		End Sub

		Public Sub New(ByVal frequency As Integer, ByVal reportScore As Boolean, ByVal reportGC As Boolean)
			Preconditions.checkArgument(frequency > 0, "Invalid frequency, must be > 0: Got " & frequency)
			Me.frequency = frequency
			Me.reportScore = reportScore
			Me.reportGC = reportGC

			lastTime.set(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			' we update lastTime on every iteration
			' just to simplify things
			If lastTime.get() Is Nothing Then
				lastTime.set(DateTimeHelper.CurrentUnixTimeMillis())
			End If

			If samplesPerSec.get() Is Nothing Then
				samplesPerSec.set(0.0)
			End If

			If batchesPerSec.get() Is Nothing Then
				batchesPerSec.set(0.0)
			End If

			If iteration Mod frequency = 0 Then
				Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()

				Dim timeSpent As Long = currentTime - lastTime.get()
				Dim timeSec As Single = timeSpent / 1000f

				Dim input As INDArray
				If TypeOf model Is ComputationGraph Then
					' for comp graph (with multidataset
					Dim cg As ComputationGraph = DirectCast(model, ComputationGraph)
					Dim inputs() As INDArray = cg.Inputs

					If inputs IsNot Nothing AndAlso inputs.Length > 0 Then
						input = inputs(0)
					Else
						input = model.input()
					End If
				Else
					input = model.input()
				End If

				'            long tadLength = Shape.getTADLength(input.shape(), ArrayUtil.range(1, input.rank()));

				Dim numSamples As Long = input.size(0)

				samplesPerSec.set(CDbl(numSamples / timeSec))
				batchesPerSec.set(CDbl(1 / timeSec))


				Dim builder As New StringBuilder()

				If Nd4j.AffinityManager.NumberOfDevices > 1 Then
					builder.Append("Device: [").Append(Nd4j.AffinityManager.getDeviceForCurrentThread()).Append("]; ")
				End If

				If reportEtl Then
					Dim time As Long = If(TypeOf model Is MultiLayerNetwork, DirectCast(model, MultiLayerNetwork).LastEtlTime, DirectCast(model, ComputationGraph).LastEtlTime)
					builder.Append("ETL: ").Append(time).Append(" ms; ")
				End If

				If reportIteration Then
					builder.Append("iteration ").Append(iteration).Append("; ")
				End If

				If reportTime Then
					builder.Append("iteration time: ").Append(timeSpent).Append(" ms; ")
				End If

				If reportSample Then
					builder.Append("samples/sec: ").Append(String.Format("{0:F3}", samplesPerSec.get())).Append("; ")
				End If

				If reportBatch Then
					builder.Append("batches/sec: ").Append(String.Format("{0:F3}", batchesPerSec.get())).Append("; ")
				End If

				If reportScore Then
					builder.Append("score: ").Append(model.score()).Append(";")
				End If

				If reportGC Then
					If gcBeans Is Nothing Then
						Try
							gcBeans = ManagementFactory.getGarbageCollectorMXBeans()
						Catch t As Exception
							log.warn("Error getting garbage collector MX beans. PerformanceListener will not report garbage collection information")
							reportGC = False
						End Try
					End If

					If reportGC Then
						Dim reportAny As Boolean = False
						For Each g As GarbageCollectorMXBean In gcBeans
							Dim count As Long = g.getCollectionCount()
							Dim time As Long = g.getCollectionTime()
							If lastGcCount.get() IsNot Nothing AndAlso lastGcCount.get().containsKey(g.getName()) Then
								Dim countDelta As Long = count - lastGcCount.get().get(g.getName())
								Dim timeDelta As Long = time - lastGcMs.get().get(g.getName())
								If Not reportAny Then
									builder.Append(" GC: ")
									reportAny = True
								Else
									builder.Append(", ")
								End If
								builder.Append("[").Append(g.getName()).Append(": ").Append(countDelta).Append(" (").Append(timeDelta).Append("ms)").Append("]")
							End If
							If lastGcCount.get() Is Nothing Then
								lastGcCount.set(New LinkedHashMap(Of String, Long)())
								lastGcMs.set(New LinkedHashMap(Of String, Long)())
							End If
							lastGcCount.get().put(g.getName(), count)
							lastGcMs.get().put(g.getName(), time)
						Next g
						If reportAny Then
							builder.Append(";")
						End If
					End If
				End If

				log.info(builder.ToString())
			End If

			lastTime.set(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws IOException, ClassNotFoundException
		Private Sub readObject(ByVal [in] As ObjectInputStream)
			'Custom deserializer, as transient ThreadLocal fields won't be initialized...
			[in].defaultReadObject()
			samplesPerSec = New ThreadLocal(Of Double)()
			batchesPerSec = New ThreadLocal(Of Double)()
			lastTime = New ThreadLocal(Of Long)()
			lastGcCount = New ThreadLocal(Of IDictionary(Of String, Long))()
			lastGcMs = New ThreadLocal(Of IDictionary(Of String, Long))()
		End Sub

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field frequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend frequency_Conflict As Integer = 1

'JAVA TO VB CONVERTER NOTE: The field reportScore was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend reportScore_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field reportSample was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend reportSample_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field reportBatch was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend reportBatch_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field reportIteration was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend reportIteration_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field reportTime was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend reportTime_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field reportEtl was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend reportEtl_Conflict As Boolean = True

			Public Sub New()

			End Sub

			''' <summary>
			''' This method defines, if iteration number should be reported together with other data
			''' </summary>
			''' <param name="reportIteration">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter reportIteration was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function reportIteration(ByVal reportIteration_Conflict As Boolean) As Builder
				Me.reportIteration_Conflict = reportIteration_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines, if time per iteration should be reported together with other data
			''' </summary>
			''' <param name="reportTime">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter reportTime was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function reportTime(ByVal reportTime_Conflict As Boolean) As Builder
				Me.reportTime_Conflict = reportTime_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines, if ETL time per iteration should be reported together with other data
			''' </summary>
			''' <param name="reportEtl">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter reportEtl was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function reportETL(ByVal reportEtl_Conflict As Boolean) As Builder
				Me.reportEtl_Conflict = reportEtl_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines, if samples/sec should be reported together with other data
			''' </summary>
			''' <param name="reportSample">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter reportSample was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function reportSample(ByVal reportSample_Conflict As Boolean) As Builder
				Me.reportSample_Conflict = reportSample_Conflict
				Return Me
			End Function


			''' <summary>
			''' This method defines, if batches/sec should be reported together with other data
			''' </summary>
			''' <param name="reportBatch">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter reportBatch was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function reportBatch(ByVal reportBatch_Conflict As Boolean) As Builder
				Me.reportBatch_Conflict = reportBatch_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines, if score should be reported together with other data
			''' </summary>
			''' <param name="reportScore">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter reportScore was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function reportScore(ByVal reportScore_Conflict As Boolean) As Builder
				Me.reportScore_Conflict = reportScore_Conflict
				Return Me
			End Function

			''' <summary>
			''' Desired TrainingListener activation frequency
			''' </summary>
			''' <param name="frequency">
			''' @return </param>
			Public Overridable Function setFrequency(ByVal frequency As Integer) As Builder
				Me.frequency_Conflict = frequency
				Return Me
			End Function

			''' <summary>
			''' This method returns configured PerformanceListener instance
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As PerformanceListener
				Dim listener As New PerformanceListener(frequency_Conflict, reportScore_Conflict)
				listener.reportIteration = Me.reportIteration_Conflict
				listener.reportTime = Me.reportTime_Conflict
				listener.reportBatch = Me.reportBatch_Conflict
				listener.reportSample = Me.reportSample_Conflict
				listener.reportEtl = Me.reportEtl_Conflict

				Return listener
			End Function
		End Class
	End Class

End Namespace