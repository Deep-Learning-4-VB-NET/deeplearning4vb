Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Model = org.deeplearning4j.nn.api.Model
Imports org.deeplearning4j.nn.api
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports InferenceMode = org.deeplearning4j.parallelism.inference.InferenceMode
Imports InferenceObservable = org.deeplearning4j.parallelism.inference.InferenceObservable
Imports LoadBalanceMode = org.deeplearning4j.parallelism.inference.LoadBalanceMode
Imports BasicInferenceObservable = org.deeplearning4j.parallelism.inference.observers.BasicInferenceObservable
Imports BasicInferenceObserver = org.deeplearning4j.parallelism.inference.observers.BasicInferenceObserver
Imports BatchedInferenceObservable = org.deeplearning4j.parallelism.inference.observers.BatchedInferenceObservable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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
'ORIGINAL LINE: @Slf4j public class ParallelInference
	Public Class ParallelInference
		Protected Friend model As Model
		Protected Friend nanos As Long
		Protected Friend workers As Integer
		Protected Friend batchLimit As Integer
		Protected Friend inferenceMode As InferenceMode
		Protected Friend queueLimit As Integer
		Protected Friend loadBalanceMode As LoadBalanceMode = LoadBalanceMode.FIFO

		' this queue holds data for inference
		Private observables As BlockingQueue(Of InferenceObservable)

		Private ReadOnly locker As New Object()

		Private zoo() As InferenceWorker
		Private provider As ObservablesProvider



		Public Shared ReadOnly DEFAULT_NUM_WORKERS As Integer = Nd4j.AffinityManager.NumberOfDevices
		Public Const DEFAULT_BATCH_LIMIT As Integer = 32
		Public Const DEFAULT_INFERENCE_MODE As InferenceMode = InferenceMode.BATCHED
		Public Const DEFAULT_QUEUE_LIMIT As Integer = 64



		Protected Friend Sub New()
			'
		End Sub

		''' <summary>
		''' This method allows to update Model used for inference in runtime, without queue reset
		''' </summary>
		''' <param name="model"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void updateModel(@NonNull Model model)
		Public Overridable Sub updateModel(ByVal model As Model)
			If zoo IsNot Nothing Then
				For Each w As val In zoo
					w.updateModel(model)
				Next w
			Else
				' if zoo wasn't initalized yet - just replace model
				Me.model = model
			End If
		End Sub

		''' <summary>
		''' This method returns Models used in workers at this moment
		''' PLEASE NOTE: This method is NOT thread safe, and should NOT be used anywhere but tests
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable ReadOnly Property CurrentModelsFromWorkers As Model()
			Get
				If zoo Is Nothing Then
					Return New Model(){}
				End If
    
				Dim models As val = New Model(zoo.Length - 1){}
				Dim cnt As Integer = 0
				For Each w As val In zoo
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: models[cnt++] = w.replicatedModel;
					models(cnt) = w.replicatedModel
						cnt += 1
				Next w
    
				Return models
			End Get
		End Property

		Protected Friend Overridable Sub init()
			observables = New LinkedBlockingQueue(Of InferenceObservable)(queueLimit)

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices
			Dim currentDevice As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Dim assignedRoot As New AtomicBoolean(False)

			zoo = New InferenceWorker(workers - 1){}
			For i As Integer = 0 To workers - 1
				Dim cDevice As Integer = i Mod numDevices
				Dim cRoot As Boolean = Not assignedRoot.get() AndAlso cDevice = currentDevice
				assignedRoot.compareAndSet(False, cRoot)

				zoo(i) = New InferenceWorker(Me, i, model, observables, cRoot, cDevice)

				zoo(i).setDaemon(True)
				zoo(i).Start()
			Next i


			If inferenceMode = InferenceMode.BATCHED Then
				log.info("Initializing ObservablesProvider...")
				provider = New ObservablesProvider(nanos, batchLimit, observables)
			End If
		End Sub

		Protected Friend Overridable Function getWorkerCounter(ByVal workerIdx As Integer) As Long
			Return zoo(workerIdx).CounterValue
		End Function

		''' <summary>
		''' This method gracefully shuts down ParallelInference instance
		''' </summary>
		Public Overridable Sub shutdown()
			SyncLock Me
				If zoo Is Nothing Then
					Return
				End If
        
				For e As Integer = 0 To zoo.Length - 1
					If zoo(e) Is Nothing Then
						Continue For
					End If
        
					zoo(e).Interrupt()
					zoo(e).shutdown()
					zoo(e) = Nothing
				Next e
				zoo = Nothing
        
				System.GC.Collect()
			End SyncLock
		End Sub

		''' 
		''' <param name="input">
		''' @return </param>
		Public Overridable Function output(ByVal input() As Double) As INDArray
			Return output(Nd4j.create(input))
		End Function

		''' 
		''' <param name="input">
		''' @return </param>
		Public Overridable Function output(ByVal input() As Single) As INDArray
			Return output(Nd4j.create(input))
		End Function

		Public Overridable Function output(ByVal input As INDArray) As INDArray
			Return output(input, Nothing)
		End Function

		Public Overridable Function output(ByVal input As INDArray, ByVal inputMask As INDArray) As INDArray
			Dim [out]() As INDArray = output(New INDArray(){input}, (If(inputMask Is Nothing, Nothing, New INDArray()){inputMask}))
			' basically, depending on model type we either
			' throw stuff to specific model, or wait for batch
			If [out].Length <> 1 Then
				Throw New System.ArgumentException("Network has multiple (" & [out].Length & ") output arrays, but only a" & " single output can be returned using this method. Use for output(INDArray[] input, INDArray[] " & "inputMasks) for multi-output nets")
			End If
			Return [out](0)
		End Function

		''' 
		''' <param name="dataSet">
		''' @return </param>
		Public Overridable Function output(ByVal dataSet As DataSet) As INDArray
			Return output(dataSet.Features, dataSet.FeaturesMaskArray)
		End Function

		''' <summary>
		''' Generate predictions/output from the netwonk
		''' </summary>
		''' <param name="input"> Input to the network </param>
		''' <returns> Output from the network </returns>
		Public Overridable Function output(ParamArray ByVal input() As INDArray) As INDArray()
			Return output(input, Nothing)
		End Function

		''' <summary>
		''' Generate predictions/outputs from the network, optionally using input masks for predictions
		''' </summary>
		''' <param name="input">      Input to the network </param>
		''' <param name="inputMasks"> Input masks for the network. May be null. </param>
		''' <returns> Output from the network </returns>
		Public Overridable Function output(ByVal input() As INDArray, ByVal inputMasks() As INDArray) As INDArray()
			Nd4j.Executioner.commit() 'Commit before passing input to other thread

			' basically, depending on model type we either throw stuff to specific model, or wait for batch
			Dim observer As New BasicInferenceObserver()
			Dim observable As InferenceObservable

			If inferenceMode = InferenceMode.SEQUENTIAL Then
				observable = New BasicInferenceObservable(input, inputMasks)
				observable.addObserver(observer)
				Try
					observables.put(observable)
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					Throw New Exception(e)
				End Try
			Else
				observable = provider.setInput(observer, input, inputMasks)
			End If

			Try
				' submit query to processing
				' and block until Observable returns
				'observer.wait();

				observer.waitTillDone()
			Catch e As Exception
				Throw New Exception(e)
			End Try

			Return observable.Output
		End Function

		''' <summary>
		''' This method does forward pass and returns output provided by OutputAdapter
		''' </summary>
		''' <param name="adapter"> </param>
		''' <param name="inputs">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T> T output(@NonNull ModelAdapter<T> adapter, org.nd4j.linalg.api.ndarray.INDArray... inputs)
		Public Overridable Function output(Of T)(ByVal adapter As ModelAdapter(Of T), ParamArray ByVal inputs() As INDArray) As T
			Return output(adapter, inputs, Nothing)
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
'ORIGINAL LINE: public <T> T output(@NonNull ModelAdapter<T> adapter,org.nd4j.linalg.api.ndarray.INDArray[] input, org.nd4j.linalg.api.ndarray.INDArray[] inputMasks)
		Public Overridable Function output(Of T)(ByVal adapter As ModelAdapter(Of T), ByVal input() As INDArray, ByVal inputMasks() As INDArray) As T
			Throw New ND4JIllegalStateException("Adapted mode requires Inplace inference mode")
		End Function


		Public Class Builder
			Friend model As Model
'JAVA TO VB CONVERTER NOTE: The field workers was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend workers_Conflict As Integer = DEFAULT_NUM_WORKERS
'JAVA TO VB CONVERTER NOTE: The field batchLimit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend batchLimit_Conflict As Integer = DEFAULT_BATCH_LIMIT
'JAVA TO VB CONVERTER NOTE: The field inferenceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inferenceMode_Conflict As InferenceMode = DEFAULT_INFERENCE_MODE
'JAVA TO VB CONVERTER NOTE: The field queueLimit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend queueLimit_Conflict As Integer = DEFAULT_QUEUE_LIMIT
'JAVA TO VB CONVERTER NOTE: The field loadBalanceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend loadBalanceMode_Conflict As LoadBalanceMode = LoadBalanceMode.FIFO

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull Model model)
			Public Sub New(ByVal model As Model)
				Me.model = model
			End Sub


			''' <summary>
			''' This method allows you to define mode that'll be used during inference. Options are:
			''' 
			''' SEQUENTIAL: Input will be sent to last-used worker unmodified.
			''' BATCHED: Multiple inputs will be packed into single batch, and
			''' sent to last-used device.
			''' </summary>
			''' <param name="inferenceMode">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder inferenceMode(@NonNull InferenceMode inferenceMode)
'JAVA TO VB CONVERTER NOTE: The parameter inferenceMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inferenceMode(ByVal inferenceMode_Conflict As InferenceMode) As Builder
				Me.inferenceMode_Conflict = inferenceMode_Conflict
				Return Me
			End Function


			''' <summary>
			''' This method allows you to specify load balance mode
			''' </summary>
			''' <param name="loadBalanceMode">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder loadBalanceMode(@NonNull LoadBalanceMode loadBalanceMode)
'JAVA TO VB CONVERTER NOTE: The parameter loadBalanceMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function loadBalanceMode(ByVal loadBalanceMode_Conflict As LoadBalanceMode) As Builder
				Me.loadBalanceMode_Conflict = loadBalanceMode_Conflict
				Return Me
			End Function


			''' <summary>
			''' This method defines, how many model copies will be used for inference.
			''' 
			''' PLEASE NOTE: This method primarily suited for multi-GPU systems
			''' PLEASE NOTE: For INPLACE inference mode this value will mean number of models per DEVICE
			''' </summary>
			''' <param name="workers">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter workers was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workers(ByVal workers_Conflict As Integer) As Builder
				If workers_Conflict < 1 Then
					Throw New System.InvalidOperationException("Workers should be positive value")
				End If

				Me.workers_Conflict = workers_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines, how many input samples can
			''' be batched within given time frame.
			''' 
			''' PLEASE NOTE: This value has no effect in
			''' SEQUENTIAL inference mode
			''' </summary>
			''' <param name="limit">
			''' @return </param>
			Public Overridable Function batchLimit(ByVal limit As Integer) As Builder
				If limit < 1 Then
					Throw New System.InvalidOperationException("Batch limit should be positive value")
				End If

				Me.batchLimit_Conflict = limit
				Return Me
			End Function

			''' <summary>
			''' This method defines buffer queue size.
			''' 
			''' Default value: 64
			''' </summary>
			''' <param name="limit">
			''' @return </param>
			Public Overridable Function queueLimit(ByVal limit As Integer) As Builder
				If limit < 1 Then
					Throw New System.InvalidOperationException("Queue limit should be positive value")
				End If

				Me.queueLimit_Conflict = limit
				Return Me
			End Function

			''' <summary>
			''' This method builds new ParallelInference instance
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As ParallelInference
				If Me.inferenceMode_Conflict = InferenceMode.INPLACE Then
					Dim inf As val = New InplaceParallelInference()
					inf.inferenceMode = Me.inferenceMode_Conflict
					inf.model = Me.model
					inf.workers = Me.workers_Conflict
					inf.loadBalanceMode = Me.loadBalanceMode_Conflict

					inf.init()

					Return inf
				Else
					Dim inference As New ParallelInference()
					inference.batchLimit = Me.batchLimit_Conflict
					inference.queueLimit = Me.queueLimit_Conflict
					inference.inferenceMode = Me.inferenceMode_Conflict
					inference.model = Me.model
					inference.workers = Me.workers_Conflict
					inference.loadBalanceMode = Me.loadBalanceMode_Conflict

					inference.init()

					Return inference
				End If
			End Function
		End Class


		''' <summary>
		''' This class actually does inference with respect to device affinity
		''' 
		''' </summary>
		Private Class InferenceWorker
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As ParallelInference

			Friend inputQueue As BlockingQueue(Of InferenceObservable)
			Friend shouldWork As New AtomicBoolean(True)
			Friend isStopped As New AtomicBoolean(False)
			Friend protoModel As Model
			Friend replicatedModel As Model
			Friend counter As New AtomicLong(0)
			Friend rootDevice As Boolean
			Friend deviceId As Integer

			Friend modelLock As New ReentrantReadWriteLock()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private InferenceWorker(int id, @NonNull Model model, @NonNull BlockingQueue inputQueue, boolean rootDevice, int deviceId)
			Friend Sub New(ByVal outerInstance As ParallelInference, ByVal id As Integer, ByVal model As Model, ByVal inputQueue As BlockingQueue, ByVal rootDevice As Boolean, ByVal deviceId As Integer)
				Me.outerInstance = outerInstance
				Me.inputQueue = inputQueue
				Me.protoModel = model
				Me.rootDevice = rootDevice
				Me.deviceId = deviceId

				Me.setDaemon(True)
				Me.setName("InferenceThread-" & id)

			End Sub

			Protected Friend Overridable ReadOnly Property CounterValue As Long
				Get
					Return counter.get()
				End Get
			End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void updateModel(@NonNull Model model)
			Protected Friend Overridable Sub updateModel(ByVal model As Model)
				Try
					modelLock.writeLock().lock()
					Me.protoModel = model

					' now re-init model
					initializeReplicaModel()
				Finally
					modelLock.writeLock().unlock()
				End Try
			End Sub

			''' <summary>
			''' This method duplicates model for future use during inference
			''' </summary>
			Protected Friend Overridable Sub initializeReplicaModel()
				If TypeOf protoModel Is ComputationGraph Then
					If Not rootDevice Then
						Me.replicatedModel = New ComputationGraph(ComputationGraphConfiguration.fromJson(DirectCast(protoModel, ComputationGraph).Configuration.toJson()))
						Me.replicatedModel.init()

						SyncLock outerInstance.locker
							Me.replicatedModel.Params = protoModel.params().unsafeDuplication(True)

							Nd4j.Executioner.commit()
						End SyncLock
					Else
						Me.replicatedModel = protoModel
					End If
				ElseIf TypeOf protoModel Is MultiLayerNetwork Then
					If Not rootDevice Then
						Me.replicatedModel = New MultiLayerNetwork(MultiLayerConfiguration.fromJson(DirectCast(protoModel, MultiLayerNetwork).LayerWiseConfigurations.toJson()))
						Me.replicatedModel.init()

						SyncLock outerInstance.locker
							Me.replicatedModel.Params = protoModel.params().unsafeDuplication(True)

							Nd4j.Executioner.commit()
						End SyncLock
					Else
						Me.replicatedModel = protoModel
					End If
				End If
			End Sub

			Public Overrides Sub run()
				Nd4j.AffinityManager.unsafeSetDevice(deviceId)
				Try
					' model should be replicated & initialized here
					initializeReplicaModel()

					Dim isCG As Boolean = TypeOf replicatedModel Is ComputationGraph
					Dim isMLN As Boolean = TypeOf replicatedModel Is MultiLayerNetwork

					Do While shouldWork.get()
						Dim request As InferenceObservable = inputQueue.take()

						If request IsNot Nothing Then
							counter.incrementAndGet()

							' FIXME: get rid of instanceof here, model won't change during runtime anyway
							If isCG Then
								Dim batches As IList(Of Pair(Of INDArray(), INDArray())) = request.getInputBatches()
								Dim [out] As IList(Of INDArray()) = New List(Of INDArray())(batches.Count)
								Try
									For Each inBatch As Pair(Of INDArray(), INDArray()) In batches
										Try
											modelLock.readLock().lock()

											Dim output() As INDArray = DirectCast(replicatedModel, ComputationGraph).output(False, inBatch.First, inBatch.Second)
											[out].Add(output)
										Finally
											Nd4j.Executioner.commit()
											modelLock.readLock().unlock()
										End Try

									Next inBatch
									request.OutputBatches = [out]
								Catch e As Exception
									request.OutputException = e
								End Try
							ElseIf isMLN Then
								Dim batches As IList(Of Pair(Of INDArray(), INDArray())) = request.getInputBatches()
								Dim [out] As IList(Of INDArray()) = New List(Of INDArray())(batches.Count)
								Try
									For Each inBatch As Pair(Of INDArray(), INDArray()) In batches
										Dim f As INDArray = inBatch.First(0)
										Dim fm As INDArray = (If(inBatch.Second Is Nothing, Nothing, inBatch.Second(0)))
										Try
											modelLock.readLock().lock()

											Dim output As INDArray = DirectCast(replicatedModel, MultiLayerNetwork).output(f, False, fm, Nothing)
											[out].Add(New INDArray(){output})
										Finally
											Nd4j.Executioner.commit()
											modelLock.readLock().unlock()
										End Try
									Next inBatch
									request.OutputBatches = [out]
								Catch e As Exception
									request.OutputException = e
								End Try
							End If


						Else
							' just do nothing, i guess and hope for next round?
						End If
					Loop
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					' do nothing
				Catch e As Exception
					Throw New Exception(e)
				Finally
					isStopped.set(True)
				End Try
			End Sub

			Protected Friend Overridable Sub shutdown()
				shouldWork.set(False)
				Do While Not isStopped.get()
					' block until main loop is finished
				Loop
			End Sub
		End Class


		Protected Friend Class ObservablesProvider
			Friend targetQueue As BlockingQueue(Of InferenceObservable)
			Friend nanos As Long
			Friend batchLimit As Integer

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile org.deeplearning4j.parallelism.inference.observers.BatchedInferenceObservable currentObservable;
			Friend currentObservable As BatchedInferenceObservable
			Friend ReadOnly locker As New Object()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected ObservablesProvider(long nanos, int batchLimit, @NonNull BlockingQueue<org.deeplearning4j.parallelism.inference.InferenceObservable> queue)
			Protected Friend Sub New(ByVal nanos As Long, ByVal batchLimit As Integer, ByVal queue As BlockingQueue(Of InferenceObservable))
				Me.targetQueue = queue
				Me.nanos = nanos
				Me.batchLimit = batchLimit
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected org.deeplearning4j.parallelism.inference.InferenceObservable setInput(@NonNull Observer observer, org.nd4j.linalg.api.ndarray.INDArray input)
			Protected Friend Overridable Function setInput(ByVal observer As Observer, ByVal input As INDArray) As InferenceObservable
				Return setInput(observer, New INDArray(){input}, Nothing)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected org.deeplearning4j.parallelism.inference.InferenceObservable setInput(@NonNull Observer observer, org.nd4j.linalg.api.ndarray.INDArray... input)
			Protected Friend Overridable Function setInput(ByVal observer As Observer, ParamArray ByVal input() As INDArray) As InferenceObservable
				Return setInput(observer, input, Nothing)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected org.deeplearning4j.parallelism.inference.InferenceObservable setInput(@NonNull Observer observer, org.nd4j.linalg.api.ndarray.INDArray[] input, org.nd4j.linalg.api.ndarray.INDArray[] inputMask)
			Protected Friend Overridable Function setInput(ByVal observer As Observer, ByVal input() As INDArray, ByVal inputMask() As INDArray) As InferenceObservable
				SyncLock locker
					Dim isNew As Boolean = False
					If currentObservable Is Nothing OrElse currentObservable.Counter >= batchLimit OrElse currentObservable.Locked Then
						isNew = True
						currentObservable = New BatchedInferenceObservable()
					End If

					currentObservable.addInput(input, inputMask)
					currentObservable.addObserver(observer)

					Try
						If isNew Then
							targetQueue.put(currentObservable)
						End If
					Catch e As InterruptedException
						Thread.CurrentThread.Interrupt()
						Throw New Exception(e)
					End Try

					Return currentObservable
				End SyncLock
			End Function
		End Class
	End Class

End Namespace