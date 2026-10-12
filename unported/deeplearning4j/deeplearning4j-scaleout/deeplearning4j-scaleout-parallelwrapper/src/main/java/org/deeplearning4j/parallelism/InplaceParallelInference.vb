Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports org.deeplearning4j.nn.api
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports InferenceMode = org.deeplearning4j.parallelism.inference.InferenceMode
Imports LoadBalanceMode = org.deeplearning4j.parallelism.inference.LoadBalanceMode
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.parallelism



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class InplaceParallelInference extends ParallelInference
	Public Class InplaceParallelInference
		Inherits ParallelInference

		Protected Friend holders As IList(Of ModelHolder) = New CopyOnWriteArrayList(Of ModelHolder)()
		Protected Friend selector As New ModelSelector()

		Protected Friend ReadOnly locker As New Object()

		Protected Friend Overrides Sub init()
			Dim e As Integer = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				Dim h As val = ModelHolder.builder().sourceModel(model).workers(workers).loadBalanceMode(loadBalanceMode).targetDeviceId(e).rootDevice(e = Nd4j.AffinityManager.getDeviceForCurrentThread().Value).build()
				h.init()

				' adding for simplified access
				holders.Add(h)

				' and adding it to actual
				selector.addModelHolder(e, h)
				e += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public synchronized void updateModel(@NonNull Model model)
		Public Overrides Sub updateModel(ByVal model As Model)
			SyncLock Me
				For Each h As val In holders
					h.updateModel(model)
				Next h
			End SyncLock
		End Sub

		Protected Friend Overrides ReadOnly Property CurrentModelsFromWorkers As Model()
			Get
				SyncLock Me
					Dim models As val = New Model(holders.Count - 1){}
					Dim cnt As Integer = 0
					For Each h As val In holders
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: models[cnt++] = h.sourceModel;
						models(cnt) = h.sourceModel
							cnt += 1
					Next h
            
					Return models
				End SyncLock
			End Get
		End Property

		Public Overrides Function output(ByVal input() As INDArray, ByVal inputMasks() As INDArray) As INDArray()
			Return selector.output(input, inputMasks)
		End Function

		''' <summary>
		''' This method does forward pass and returns output provided by OutputAdapter
		''' </summary>
		''' <param name="adapter"> </param>
		''' <param name="input"> </param>
		''' <param name="inputMasks"> </param>
		''' @param <T>
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T> T output(@NonNull ModelAdapter<T> adapter, org.nd4j.linalg.api.ndarray.INDArray[] input, org.nd4j.linalg.api.ndarray.INDArray[] inputMasks, org.nd4j.linalg.api.ndarray.INDArray[] labelsMasks)
		Public Overridable Overloads Function output(Of T)(ByVal adapter As ModelAdapter(Of T), ByVal input() As INDArray, ByVal inputMasks() As INDArray, ByVal labelsMasks() As INDArray) As T
			Dim holder As val = selector.ModelForThisThread
			Dim model As Model = Nothing
			Dim acquired As Boolean = False
			Try
				model = holder.acquireModel()
				acquired = True
				Return adapter.apply(model, input, inputMasks, labelsMasks)
			Catch e As InterruptedException
				Throw New Exception(e)
			Finally
				If model IsNot Nothing AndAlso acquired Then
					holder.releaseModel(model)
				End If
			End Try
		End Function


		Protected Friend Class ModelSelector
			' this map stores collection of shared
			Protected Friend map As IDictionary(Of Integer, ModelHolder) = New Dictionary(Of Integer, ModelHolder)()

			Protected Friend ReadOnly loadBalanceMode As LoadBalanceMode

			Public Sub New()
				Me.New(LoadBalanceMode.ROUND_ROBIN)
			End Sub

			Public Sub New(ByVal loadBalanceMode As LoadBalanceMode)
				Me.loadBalanceMode = loadBalanceMode
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void addModelHolder(@NonNull Integer device, @NonNull ModelHolder holder)
			Protected Friend Overridable Sub addModelHolder(ByVal device As Integer, ByVal holder As ModelHolder)
				map(device) = holder
			End Sub

			Public Overridable Function getModelForThread(ByVal threadId As Long) As ModelHolder
				' first of all we get mapped device for this thread
				Dim device As val = Nd4j.AffinityManager.getDeviceForThread(threadId)

				' each device has it's own queue
				Dim q As val = map(device)

				' and we're returning holder right away
				Return q
			End Function

			Public Overridable Function output(ByVal input() As INDArray, ByVal inputMasks() As INDArray) As INDArray()
				Return ModelForThisThread.output(input, inputMasks)
			End Function

			Public Overridable ReadOnly Property ModelForThisThread As ModelHolder
				Get
					Return getModelForThread(Thread.CurrentThread.getId())
				End Get
			End Property
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @AllArgsConstructor @lombok.Builder protected static class ModelHolder
		Protected Friend Class ModelHolder
			Protected Friend sourceModel As Model
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default protected int workers = 4;
			Protected Friend workers As Integer = 4
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default protected List<org.deeplearning4j.nn.api.Model> replicas = new ArrayList<>();
			Protected Friend replicas As IList(Of Model) = New List(Of Model)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default protected boolean rootDevice = true;
			Protected Friend rootDevice As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default protected org.deeplearning4j.parallelism.inference.LoadBalanceMode loadBalanceMode = org.deeplearning4j.parallelism.inference.LoadBalanceMode.ROUND_ROBIN;
			Protected Friend loadBalanceMode As LoadBalanceMode = LoadBalanceMode.ROUND_ROBIN
			Protected Friend targetDeviceId As Integer

			Protected Friend ReadOnly position As New AtomicLong(0)
			Protected Friend ReadOnly modelLock As New ReentrantReadWriteLock()

			' this queue is used in FIFO mode
			Protected Friend ReadOnly queue As BlockingQueue(Of Model) = New LinkedBlockingQueue(Of Model)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default protected transient boolean isCG = false;
			<NonSerialized>
			Protected Friend isCG As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default protected transient boolean isMLN = false;
			<NonSerialized>
			Protected Friend isMLN As Boolean = False


			Protected Friend Overridable Sub init()
				SyncLock Me
					If workers < 1 Then
						Throw New ND4JIllegalStateException("Workers must be positive value")
					End If
        
					replicas.Clear()
        
					isCG = TypeOf sourceModel Is ComputationGraph
					isMLN = TypeOf sourceModel Is MultiLayerNetwork
        
					' we clone params only if we're not on the same device
					Dim params As val = If(rootDevice, sourceModel.params(), sourceModel.params().unsafeDuplication(True))
        
					' and moving it to specified device (only if NOT root
					If Not rootDevice Then
						Nd4j.AffinityManager.replicateToDevice(targetDeviceId, params)
					End If
        
					For e As Integer = 0 To workers - 1
						If TypeOf sourceModel Is ComputationGraph Then
							' building configuration with shared parameters
							Dim model As val = New ComputationGraph(ComputationGraphConfiguration.fromJson(DirectCast(sourceModel, ComputationGraph).Configuration.toJson()))
							model.init(params, False)
							Nd4j.Executioner.commit()
        
							' storing model for future reuse
							replicas.Add(model)
        
							If loadBalanceMode = LoadBalanceMode.FIFO Then
								queue.add(model)
							End If
						ElseIf TypeOf sourceModel Is MultiLayerNetwork Then
							Dim model As val = New MultiLayerNetwork(MultiLayerConfiguration.fromJson(DirectCast(sourceModel, MultiLayerNetwork).LayerWiseConfigurations.toJson()))
							model.init(params, False)
							Nd4j.Executioner.commit()
        
							replicas.Add(model)
        
							If loadBalanceMode = LoadBalanceMode.FIFO Then
								queue.add(model)
							End If
						End If
					Next e
				End SyncLock
			End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.nn.api.Model acquireModel() throws InterruptedException
			Protected Friend Overridable Function acquireModel() As Model
				Try
					modelLock.readLock().lock()

					Select Case loadBalanceMode
						Case LoadBalanceMode.FIFO
								Return queue.take()
						Case LoadBalanceMode.ROUND_ROBIN
							Return replicas(CInt(position.getAndIncrement() Mod replicas.Count))
						Case Else
							Throw New ND4JIllegalStateException("Unknown LoadBalanceMode was specified: [" & loadBalanceMode & "]")
					End Select
				Finally
					modelLock.readLock().unlock()
				End Try
			End Function

			Protected Friend Overridable Sub releaseModel(ByVal model As Model)
				Try
					modelLock.readLock().lock()

					Select Case loadBalanceMode
						Case LoadBalanceMode.FIFO
							queue.add(model)
						Case LoadBalanceMode.ROUND_ROBIN
						Case Else
							Throw New ND4JIllegalStateException("Unknown LoadBalanceMode was specified: [" & loadBalanceMode & "]")
					End Select
				Finally
					modelLock.readLock().unlock()
				End Try
			End Sub

			Protected Friend Overridable Function output(ByVal input() As INDArray, ByVal inputMasks() As INDArray) As INDArray()
				Try
					modelLock.readLock().lock()
					If isCG Then
						' acquiring model from pool
						Dim model As val = acquireModel()

						' doing inference
'JAVA TO VB CONVERTER NOTE: The local variable output was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
						Dim output_Conflict() As INDArray
						Try
							output_Conflict = CType(model, ComputationGraph).output(False, input, inputMasks)
						Finally
							' releasing model
							releaseModel(model)
						End Try
						Return output_Conflict
					ElseIf isMLN Then
						If input.Length > 1 OrElse (inputMasks IsNot Nothing AndAlso inputMasks.Length > 1) Then
							Throw New ND4JIllegalStateException("MultilayerNetwork can't have multiple inputs")
						End If

						Dim model As val = acquireModel()
						Dim result As INDArray
						Try
							result = CType(model, MultiLayerNetwork).output(input(0), False, (If(inputMasks Is Nothing, Nothing, inputMasks(0))), Nothing)
						Finally
							releaseModel(model)
						End Try
						Return New INDArray(){result}
					Else
						Throw New System.NotSupportedException()
					End If
				Catch e As InterruptedException
					Throw New Exception(e)
				Finally
					modelLock.readLock().unlock()
				End Try
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void updateModel(@NonNull Model model)
			Protected Friend Overridable Sub updateModel(ByVal model As Model)
				Try
					modelLock.writeLock().lock()

					Me.sourceModel = model

					init()
				Finally
					modelLock.writeLock().unlock()
				End Try
			End Sub

		End Class
	End Class

End Namespace