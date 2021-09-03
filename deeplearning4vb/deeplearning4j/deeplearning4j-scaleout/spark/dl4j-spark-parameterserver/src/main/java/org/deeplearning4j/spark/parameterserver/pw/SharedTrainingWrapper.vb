Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Loader = org.bytedeco.javacpp.Loader
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports DL4JEnvironmentVars = org.deeplearning4j.common.config.DL4JEnvironmentVars
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports Model = org.deeplearning4j.nn.api.Model
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.deeplearning4j.nn.updater
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports SleepyTrainingListener = org.deeplearning4j.optimize.listeners.SleepyTrainingListener
Imports EncodedGradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.EncodedGradientsAccumulator
Imports EncodingHandler = org.deeplearning4j.optimize.solvers.accumulation.EncodingHandler
Imports ParallelWrapper = org.deeplearning4j.parallelism.ParallelWrapper
Imports SharedTrainingConfiguration = org.deeplearning4j.spark.parameterserver.conf.SharedTrainingConfiguration
Imports VirtualDataSetIterator = org.deeplearning4j.spark.parameterserver.iterators.VirtualDataSetIterator
Imports org.deeplearning4j.spark.parameterserver.iterators
Imports VirtualMultiDataSetIterator = org.deeplearning4j.spark.parameterserver.iterators.VirtualMultiDataSetIterator
Imports ModelParamsConsumer = org.deeplearning4j.spark.parameterserver.networking.v2.ModelParamsConsumer
Imports UpdaterParamsConsumer = org.deeplearning4j.spark.parameterserver.networking.v2.UpdaterParamsConsumer
Imports UpdatesConsumer = org.deeplearning4j.spark.parameterserver.networking.v2.UpdatesConsumer
Imports WiredEncodingHandler = org.deeplearning4j.spark.parameterserver.networking.v2.WiredEncodingHandler
Imports SharedTrainingResult = org.deeplearning4j.spark.parameterserver.training.SharedTrainingResult
Imports SharedTrainingWorker = org.deeplearning4j.spark.parameterserver.training.SharedTrainingWorker
Imports BlockingObserver = org.deeplearning4j.spark.parameterserver.util.BlockingObserver
Imports org.deeplearning4j.spark.parameterserver.util
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports TransportType = org.nd4j.parameterserver.distributed.enums.TransportType
Imports NetworkOrganizer = org.nd4j.parameterserver.distributed.util.NetworkOrganizer
Imports ModelParameterServer = org.nd4j.parameterserver.distributed.v2.ModelParameterServer
Imports UpdaterParametersProvider = org.nd4j.parameterserver.distributed.v2.transport.UpdaterParametersProvider
Imports AeronUdpTransport = org.nd4j.parameterserver.distributed.v2.transport.impl.AeronUdpTransport

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

Namespace org.deeplearning4j.spark.parameterserver.pw


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SharedTrainingWrapper
	Public Class SharedTrainingWrapper
		Private Shared INSTANCE As New SharedTrainingWrapper()
		Private Shared LAST_INSTANCE_ID As New AtomicLong(Long.MinValue)
		Protected Friend wrapper As ParallelWrapper
		Protected Friend iteratorDS As VirtualDataSetIterator
		Protected Friend iteratorMDS As VirtualMultiDataSetIterator

		Protected Friend iteratorsDS As IList(Of IEnumerator(Of DataSet))
		Protected Friend iteratorsMDS As IList(Of IEnumerator(Of MultiDataSet))


		Protected Friend isFirst As New AtomicBoolean(False)
		Protected Friend exceptionEncountered As New AtomicBoolean(False)
		Protected Friend exception As Exception

		Protected Friend iteratorDataSetCount As New ThreadLocal(Of AtomicInteger)() 'Using AtomicInteger because it's mutable, not because it's atomic
		Protected Friend observer As New ThreadLocal(Of BlockingObserver)()
		Protected Friend accumulator As EncodedGradientsAccumulator
		Protected Friend originalModel As Model

		Protected Friend consumer As UpdatesConsumer

		Protected Friend Sub New()
			init()
		End Sub

		Protected Friend Overridable Sub init()
			' instantiate some stuff here
			iteratorsDS = New CopyOnWriteArrayList(Of IEnumerator(Of DataSet))()
			iteratorsMDS = New CopyOnWriteArrayList(Of IEnumerator(Of MultiDataSet))()

			' now we're creating DataSetIterators, to feed ParallelWrapper
			iteratorDS = New VirtualDataSetIterator(iteratorsDS)
			iteratorMDS = New VirtualMultiDataSetIterator(iteratorsMDS)
		End Sub

		Public Shared Function getInstance(ByVal id As Long) As SharedTrainingWrapper
			SyncLock GetType(SharedTrainingWrapper)
				If LAST_INSTANCE_ID.get() <> Long.MinValue AndAlso LAST_INSTANCE_ID.get() <> id Then
					log.debug("Shutting down existing SharedTrainingWrapper instances; resetting state - previous instance ID {}," & " new instance ID {}", LAST_INSTANCE_ID.get(), id)
					If INSTANCE.wrapper IsNot Nothing Then
						INSTANCE.wrapper.shutdown()
						INSTANCE.wrapper = Nothing
					End If
					INSTANCE.iteratorsDS.Clear()
					INSTANCE.iteratorsMDS.Clear()
					INSTANCE.exceptionEncountered.set(False)
					INSTANCE.iteratorDataSetCount = New ThreadLocal(Of AtomicInteger)()
					INSTANCE.accumulator = Nothing
					INSTANCE.originalModel = Nothing
					INSTANCE.consumer = Nothing
					LAST_INSTANCE_ID.set(id)
				End If
        
				If LAST_INSTANCE_ID.get() = Long.MinValue Then
					LAST_INSTANCE_ID.set(id)
				End If
        
				Return INSTANCE
			End SyncLock
		End Function

		''' <summary>
		''' This method registers given Iterable<DataSet> in VirtualDataSetIterator
		''' </summary>
		''' <param name="iterator"> </param>
		Public Overridable Sub attachDS(ByVal iterator As IEnumerator(Of DataSet))
			log.debug("Attaching thread...")

			'Count the number of minibatches - used for reporting/debugging purposes
			If iteratorDataSetCount.get() Is Nothing Then
				iteratorDataSetCount.set(New AtomicInteger(0))
			End If
			Dim count As AtomicInteger = iteratorDataSetCount.get()
			count.set(0)

			' we're creating our Observable wrapper
			Dim wrapped As New VirtualIterator(Of DataSet)(New CountingIterator(Of )(iterator, count))

			' and creating Observer which will be used to monitor progress within iterator
			Dim obs As New BlockingObserver(exceptionEncountered)
			wrapped.addObserver(obs)

			' putting that "somewhere"
			iteratorsDS.Add(wrapped)

			' storing observer into ThreadLocal, since we're going to use that later
			observer.set(obs)
		End Sub

		''' <summary>
		''' This method registers given Iterable<MultiDataSet> in VirtualMultiDataSetIterator
		''' </summary>
		''' <param name="iterator"> </param>
		Public Overridable Sub attachMDS(ByVal iterator As IEnumerator(Of MultiDataSet))
			log.debug("Attaching thread...")

			'Count the number of minibatches - used for reporting/debugging purposes
			If iteratorDataSetCount.get() Is Nothing Then
				iteratorDataSetCount.set(New AtomicInteger(0))
			End If
			Dim count As AtomicInteger = iteratorDataSetCount.get()
			count.set(0)

			' we're creating our Observable wrapper
			Dim wrapped As New VirtualIterator(Of MultiDataSet)(New CountingIterator(Of )(iterator, count))

			' and creating Observer which will be used to monitor progress within iterator
			Dim obs As New BlockingObserver(exceptionEncountered)
			wrapped.addObserver(obs)

			' putting that "somewhere"
			iteratorsMDS.Add(wrapped)

			' storing observer into ThreadLocal, since we're going to use that later
			observer.set(obs)
		End Sub

		Public Overridable Function run(ByVal worker As SharedTrainingWorker) As SharedTrainingResult
	'        
	'            first call instantiates pw, messenger etc, and gets in charge here.
	'         
			If isFirst.compareAndSet(False, True) Then
				'Reset past exception encountered in case we're doing correct fit after incorrect...
				exceptionEncountered.set(False)
				exception = Nothing

				Dim trainingConfiguration As SharedTrainingConfiguration = worker.getBroadcastConfiguration().getValue()
				Dim voidConfiguration As VoidConfiguration = worker.getBroadcastConfiguration().getValue().getVoidConfiguration()

				Dim model As Model = Nothing

	'            
	'                    Plan is simple here: if there's defined field in SharedTrainingConfiguration - use that.
	'                    If no - try to guess something
	'                 
				Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices

				Dim numCores As Integer = Loader.totalCores()

				''' <summary>
				''' Logic here is simple:
				''' 1) If user had specified number of workers per node - use that value
				''' 2) If not, and there's > 1 devices in system (as in Multi-GPU system) - use numberOfDevices as number of workers
				''' 3) otherwise, let's assume that's regular multi-core node, so we'll use 1..6 workers, depending on number of cores/4
				''' </summary>
				Dim numWorkers As Integer = If(trainingConfiguration.getNumberOfWorkersPerNode() > 0, trainingConfiguration.getNumberOfWorkersPerNode(), If(numDevices > 1, numDevices, Math.Min(6, Math.Max(1, numCores \ 4))))

				If numDevices > 1 AndAlso numWorkers > numDevices Then
					log.warn("WARNING! Using more workers then number of available computational devices!")
				End If



				' now we're attaching VoidParameterServer to GradientsAccumulator, but doing that only once
				If wrapper Is Nothing Then
					log.debug("Starting ParallelWrapper at thread {}", Thread.CurrentThread.getId())

					model = worker.InitialModel
					If model Is Nothing Then
						model = worker.InitialModelGraph
					End If

					If model Is Nothing Then
						Throw New DL4JInvalidConfigException("No model was defined for training")
					End If

					Dim listeners As IList(Of TrainingListener) = worker.getListeners()
					If listeners IsNot Nothing Then
						model.setListeners(listeners)
						Dim r As StatsStorageRouter = worker.getRouter()
						If r IsNot Nothing Then
							For Each l As TrainingListener In listeners
								If TypeOf l Is RoutingIterationListener Then
									DirectCast(l, RoutingIterationListener).StorageRouter = r
								End If
							Next l
						End If
					End If

					Dim handler As val = New WiredEncodingHandler(trainingConfiguration.getThresholdAlgorithm(), trainingConfiguration.getResidualPostProcessor(), Nothing, trainingConfiguration.isEncodingDebugMode())

					' TODO: if there will be no code difference - use the same class instead of 2 different classes
					Dim modelParamsSupplier As val = New ModelParamsConsumer()
					Dim updateParamsSupplier As val = New UpdaterParamsConsumer()

					' this accumulator will provide sharing gradients over network, via WiredEncodedHandler. But we create it only once
					If accumulator Is Nothing Then
						''' <summary>
						'''  We know, that updates are guaranteed to have MAX size of params / 16. So, here we go.
						'''  I.e. for model with 100m params, that's 400m of floats (or 800m of doubles)
						'''  The worst case for us is bitmap encoding, that takes 2 bits to encode each gradient value
						''' 
						'''  so, for float in worst case we'll have (100m / 16) int elements. So, our buffer size will be 6.25m * queueSize * 4 bytes per int
						''' </summary>

						Dim queueSize As Integer = numWorkers * 2

						Dim bufferSize As val = If(trainingConfiguration.getBufferSize() > 0, trainingConfiguration.getBufferSize(), EncodedGradientsAccumulator.getOptimalBufferSize(model, numWorkers, 2))

						accumulator = (New EncodedGradientsAccumulator.Builder(numWorkers)).messageHandler(handler).thresholdAlgorithm(trainingConfiguration.getThresholdAlgorithm()).residualPostProcessor(trainingConfiguration.getResidualPostProcessor()).memoryParameters(bufferSize, queueSize).encodingDebugMode(trainingConfiguration.isEncodingDebugMode()).build()

						' we should introduce ourselves to controller
						' FIXME: if localIP is null - use original ip discovery available in VoidParameterServer
						Dim localIP As String = Nothing

						' picking IP address based on network mask
						If localIP Is Nothing AndAlso voidConfiguration.NetworkMask IsNot Nothing Then
							Dim organizer As New NetworkOrganizer(voidConfiguration.NetworkMask)
							localIP = organizer.MatchingAddress
						End If

						' last resort here...
						If localIP Is Nothing Then
							localIP = Environment.GetEnvironmentVariable(DL4JEnvironmentVars.DL4J_VOID_IP)
						End If

						' set it to localhost, and hope for BroadcastTransport used
						If localIP Is Nothing Then
							localIP = "127.0.0.1"
							log.warn("Can't get IP address to start VoidParameterServer client. Using localhost instead")
						End If

						log.debug("Checking for ModelParameterServer existence")

						' we're saving reference to original model
						originalModel = model

						' if we're running in spark localhost mode - we don't want double initialization
						If Not ModelParameterServer.Instance.Initialized Then
							log.info("Initializing transport [{}:{}] with root as [{}:{}]...", localIP, voidConfiguration.getPortSupplier().getPort(), voidConfiguration.getControllerAddress(), voidConfiguration.getUnicastControllerPort())
							' FIXME: implement support for Custom transport implementation

							Dim transport As val = If(voidConfiguration.getTransportType() = TransportType.ROUTED_UDP, New AeronUdpTransport(localIP, voidConfiguration.getPortSupplier().getPort(), voidConfiguration.getControllerAddress(), voidConfiguration.getUnicastControllerPort(), voidConfiguration), Nothing)

							If transport Is Nothing Then
								Throw New DL4JInvalidConfigException("No Transport implementation was defined for this training session!")
							End If

							consumer = UpdatesConsumer.builder().numWorkers(numWorkers).accumulator(accumulator).params(model.params()).build()

							accumulator.ExternalSource = consumer.UpdatesQueue

							log.debug("Configuring transport...")
							'  pass values right away
							ModelParameterServer.Instance.configure(voidConfiguration, transport, New UpdaterParametersProviderAnonymousInnerClass(Me))

							ModelParameterServer.Instance.addUpdatesSubscriber(consumer)
							ModelParameterServer.Instance.addModelParamsSubscriber(modelParamsSupplier)
							ModelParameterServer.Instance.addUpdaterParamsSubscriber(updateParamsSupplier)
						End If

						log.debug("Starting ModelParameterServer...")
						' after initialization finished, we're ok to actually start training
						ModelParameterServer.Instance.launch()

						' waiting for introduction. probably no-op in 99.9999% cases
						Do While Not ModelParameterServer.Instance.getTransport().isIntroduced()
							Try
								Thread.Sleep(100)
							Catch e As InterruptedException
								Throw New Exception(e)
							End Try
						Loop
					End If

					' propagate iteration/epoch numbers
					If TypeOf originalModel Is MultiLayerNetwork Then
						DirectCast(model, MultiLayerNetwork).IterationCount = ModelParameterServer.Instance.getStartPosition().First
						DirectCast(model, MultiLayerNetwork).EpochCount = ModelParameterServer.Instance.getStartPosition().Second
					ElseIf TypeOf originalModel Is ComputationGraph Then
						DirectCast(model, ComputationGraph).Configuration.setIterationCount(ModelParameterServer.Instance.getStartPosition().First)
						DirectCast(model, ComputationGraph).Configuration.setEpochCount(ModelParameterServer.Instance.getStartPosition().Second)
					End If

					' if we're going to extend iteratation for debugging purposes - let's do that here
					If trainingConfiguration.getDebugLongerIterations() > 0 Then
						log.warn("Adding SleepyListener: {} ms", trainingConfiguration.getDebugLongerIterations())
						model.addListeners(SleepyTrainingListener.builder().timerIteration(trainingConfiguration.getDebugLongerIterations()).build())
					End If

					' :)
					accumulator.markExternalUpdates(True)

					' we're launching PW only if number of workers is more then 1
					If numWorkers > 1 Then
						'log.info("Params at PW:  {mean: [{}]; stdev: [{}]}", originalModel.params().meanNumber().doubleValue(), originalModel.params().stdNumber().doubleValue());

						wrapper = (New ParallelWrapper.Builder(Of )(originalModel)).workers(numWorkers).workspaceMode(trainingConfiguration.getWorkspaceMode()).trainingMode(ParallelWrapper.TrainingMode.CUSTOM).gradientsAccumulator(accumulator).prefetchBuffer(trainingConfiguration.getPrefetchSize()).modelParamsSupplier(modelParamsSupplier).updaterParamsSupplier(updateParamsSupplier).thresholdAlgorithm(trainingConfiguration.getThresholdAlgorithm()).residualPostProcessor(trainingConfiguration.getResidualPostProcessor()).build()
						wrapper.setExceptionEncountered(exceptionEncountered)
					Else
						log.debug("Using standalone model instead...")

						' since there'll be only one consumer, we don't need complex sync logic anymore
						accumulator.fallbackToSingleConsumerMode(True)
						accumulator.touch()

						' checking if there were updated params received (i.e. if that's failover routine
						Dim mParams As val = modelParamsSupplier.get()
						If mParams IsNot Nothing Then
							log.info("Updating model params to the most recent ones...")
							originalModel.params().assign(mParams)
						End If

						' ok. attaching accumulator to model
						If TypeOf model Is ComputationGraph Then
							DirectCast(originalModel, ComputationGraph).Configuration.setTrainingWorkspaceMode(trainingConfiguration.getWorkspaceMode())
							DirectCast(originalModel, ComputationGraph).GradientsAccumulator = accumulator
						ElseIf TypeOf model Is MultiLayerNetwork Then
							DirectCast(originalModel, MultiLayerNetwork).LayerWiseConfigurations.setTrainingWorkspaceMode(trainingConfiguration.getWorkspaceMode())
							DirectCast(originalModel, MultiLayerNetwork).GradientsAccumulator = accumulator
						End If
					End If
				End If

				' TODO: optionally we might be waiting until we have >1 splits delivered


				If consumer IsNot Nothing Then
					consumer.bypassMode(False)
				End If

				' now we're just calling for fit
				If iteratorDS Is Nothing AndAlso iteratorMDS Is Nothing Then
					Throw New DL4JInvalidConfigException("No iterators were defined for training")
				End If

				Try
					Dim dsNext As Boolean
					Dim mdsNext As Boolean
					mdsNext = iteratorMDS IsNot Nothing && iteratorMDS.MoveNext()
					dsNext = iteratorDS IsNot Nothing && iteratorDS.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((dsNext = iteratorDS != null && iteratorDS.hasNext()) || (mdsNext = iteratorMDS != null && iteratorMDS.hasNext()))
					Do While dsNext OrElse mdsNext
						'Loop as a guard against concurrent modifications and RCs

						If wrapper IsNot Nothing Then
							If dsNext Then
								wrapper.fit(iteratorDS)
							Else
								wrapper.fit(iteratorMDS)
							End If
						Else
							' if wrapper is null, we're fitting standalone model then
							If dsNext Then
								If TypeOf model Is ComputationGraph Then
									DirectCast(originalModel, ComputationGraph).fit(iteratorDS)
								ElseIf TypeOf model Is MultiLayerNetwork Then
									DirectCast(originalModel, MultiLayerNetwork).fit(iteratorDS)
								End If
							Else
								If TypeOf model Is ComputationGraph Then
									DirectCast(originalModel, ComputationGraph).fit(iteratorMDS)
								ElseIf TypeOf model Is MultiLayerNetwork Then
									DirectCast(originalModel, MultiLayerNetwork).fit(iteratorMDS)
								End If
							End If
						End If

						If consumer IsNot Nothing Then
							consumer.UpdatesQueue.purge()
						End If
							mdsNext = iteratorMDS IsNot Nothing AndAlso iteratorMDS.MoveNext()
							dsNext = iteratorDS IsNot Nothing AndAlso iteratorDS.MoveNext()
					Loop
				Catch t As Exception
					log.warn("Exception encountered during fit operation", t)
					exceptionEncountered.set(True)
					exception = t
				End Try


				' conditionally shutdown & reset ParallelWrapper
				Dim accum As EncodedGradientsAccumulator
				If wrapper IsNot Nothing Then
					accum = CType(wrapper.getGradientsAccumulator(), EncodedGradientsAccumulator) 'Store before possible shutdown for below
				Else
					accum = accumulator
				End If
				If trainingConfiguration.isEpochReset() Then
					wrapper.shutdown()
					wrapper = Nothing
				End If

				' reset iterators too
				init()

				' and accumulator, to reset its states
				accumulator.reset()

				' current TrainingDriver won't be receiving any updates beyond this point
				If consumer IsNot Nothing Then
					consumer.bypassMode(True)
				End If


				isFirst.set(False)

				log.info("Master thread done...")

				Dim updaterState As INDArray = Nothing
				If TypeOf model Is ComputationGraph Then
					updaterState = DirectCast(originalModel, ComputationGraph).Updater.getUpdaterStateViewArray()
				ElseIf TypeOf model Is MultiLayerNetwork Then
					updaterState = DirectCast(originalModel, MultiLayerNetwork).Updater.StateViewArray
				End If

				'Get threshold algorithm instances from each thread, and average them - they may have state that needs
				' to be averaged and persisted, to avoid starting threshold adaption from scratch
				Dim mh As val = CType(accum.getHandler(), EncodingHandler)
				Dim taAveraged As val = mh.getAverageThresholdAlgorithm()

				' FIXME: fill stats here
				Dim result As val = SharedTrainingResult.builder().aggregationsCount(1).scoreSum(originalModel.score()).updaterStateArray(updaterState).listenerMetaData(New List(Of )()).listenerStaticInfo(New List(Of )()).listenerUpdates(New List(Of )()).minibatchesPerExecutor(Collections.singletonMap(SparkUtils.SparkExecutorId, iteratorDataSetCount.get().get())).thresholdAlgorithm(taAveraged).build()

				' releasing Context here
	'            Nd4j.getMemoryManager().releaseCurrentContext();

				Return result
			Else
				' blocking call right here, all non-master threads will be blocked here
				Try
					observer.get().waitTillDone()
					'observer.get().wait();

					log.info("Feeder [{}] thread done...", Thread.CurrentThread.getName())

					If exceptionEncountered.get() Then
						'Propagate exception
						Dim t As Exception
						If wrapper Is Nothing OrElse exception IsNot Nothing Then
							t = exception
						Else
							t = wrapper.getException()
						End If

						Throw New Exception("Training failed due to exception in ParallelWrapper fit operation", t)
					End If

					'  nothing to do here, just give away empty result (other than iterator count)
					Return SharedTrainingResult.builder().minibatchesPerExecutor(Collections.singletonMap(SparkUtils.SparkExecutorId, iteratorDataSetCount.get().get())).build()
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					' FIXME: we don't really need to throw it again, it's here only for debugging purposes
					Throw New Exception(e)
				End Try
			End If
		End Function

		Private Class UpdaterParametersProviderAnonymousInnerClass
			Implements UpdaterParametersProvider

			Private ReadOnly outerInstance As SharedTrainingWrapper

			Public Sub New(ByVal outerInstance As SharedTrainingWrapper)
				Me.outerInstance = outerInstance
			End Sub

			Public ReadOnly Property UpdaterParameters As INDArray Implements UpdaterParametersProvider.getUpdaterParameters
				Get
					log.info("Serving updater parameters...")
					Dim updater As Updater = Nothing
					If TypeOf outerInstance.originalModel Is MultiLayerNetwork Then
						updater = DirectCast(outerInstance.originalModel, MultiLayerNetwork).Updater
					ElseIf TypeOf outerInstance.originalModel Is ComputationGraph Then
						updater = DirectCast(outerInstance.originalModel, ComputationGraph).Updater
					End If
    
					If updater IsNot Nothing Then
						If TypeOf updater Is BaseMultiLayerUpdater Then
							Return DirectCast(updater, BaseMultiLayerUpdater).getStateViewArrayCopy()
						Else
							log.error("Updater doesn't implement getStateViewArrayCopy()")
							Return Nothing
						End If
					Else
						log.warn("No Updater in the model")
						Return Nothing
					End If
				End Get
			End Property
		End Class

		Public Overridable Sub passDataSet(ByVal dataSet As DataSet)
			' we're going to save this dataset into VirtualDataSetIterator
		End Sub

		Public Overridable Sub passDataSet(ByVal dataSet As MultiDataSet)
			' we're going to save this dataset into VirtualMultiDataSetIterator
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void blockUntilFinished() throws InterruptedException
		Public Overridable Sub blockUntilFinished()
			If observer.get() IsNot Nothing Then
				Monitor.Wait(observer.get())
			Else
				Throw New System.InvalidOperationException("This method can't be called before iterators initialization")
			End If
		End Sub
	End Class

End Namespace