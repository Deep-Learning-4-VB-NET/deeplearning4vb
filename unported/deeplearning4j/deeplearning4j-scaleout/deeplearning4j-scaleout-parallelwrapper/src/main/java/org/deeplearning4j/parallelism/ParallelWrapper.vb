Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports EncodingHandler = org.deeplearning4j.optimize.solvers.accumulation.EncodingHandler
Imports AdaptiveThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.AdaptiveThresholdAlgorithm
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports AsyncMultiDataSetIterator = org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
Imports DummyBlockDataSetIterator = org.deeplearning4j.datasets.iterator.DummyBlockDataSetIterator
Imports DummyBlockMultiDataSetIterator = org.deeplearning4j.datasets.iterator.DummyBlockMultiDataSetIterator
Imports InterleavedDataSetCallback = org.deeplearning4j.datasets.iterator.callbacks.InterleavedDataSetCallback
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports Model = org.deeplearning4j.nn.api.Model
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports SharedGradient = org.deeplearning4j.optimize.listeners.SharedGradient
Imports EncodedGradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.EncodedGradientsAccumulator
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports Registerable = org.deeplearning4j.optimize.solvers.accumulation.Registerable
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports DefaultTrainerContext = org.deeplearning4j.parallelism.factory.DefaultTrainerContext
Imports SymmetricTrainerContext = org.deeplearning4j.parallelism.factory.SymmetricTrainerContext
Imports TrainerContext = org.deeplearning4j.parallelism.factory.TrainerContext
Imports Trainer = org.deeplearning4j.parallelism.trainer.Trainer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.function

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


	' TODO: We want this thing to be NUMA-aware in foreseeable future
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public class ParallelWrapper implements AutoCloseable
	Public Class ParallelWrapper
		Implements AutoCloseable

		Public Enum TrainingMode
			''' <summary>
			''' Averaging every X epochs will be applied
			''' </summary>
			AVERAGING

			''' <summary>
			''' Models within ParallelWrapper instance will share gradients updates
			''' </summary>
			SHARED_GRADIENTS

			''' <summary>
			''' This option assumes use of GradientsAccumulator with any MessageHandler
			''' </summary>
			CUSTOM
		End Enum

		Protected Friend modelParamsSupplier As Supplier(Of INDArray)
		Protected Friend updaterParamsSupplier As Supplier(Of INDArray)

		Protected Friend exceptionEncountered As AtomicBoolean
		Protected Friend exception As Exception
		Protected Friend ReadOnly uuid As String = System.Guid.randomUUID().ToString()
		Protected Friend model As Model
		Protected Friend workers As Integer = 2
		Protected Friend prefetchSize As Integer = 2
		Protected Friend averagingFrequency As Integer = 1
		Protected Friend zoo() As Trainer
		Protected Friend trainerContext As TrainerContext
		Protected Friend iterationsCounter As New AtomicLong(0)
		Protected Friend reportScore As Boolean = False
		Protected Friend averageUpdaters As Boolean = True
		Protected Friend legacyAveraging As Boolean = False
		Protected Friend wasAveraged As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field stopFit was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend stopFit_Conflict As New AtomicBoolean(False)
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend listeners_Conflict As IList(Of TrainingListener) = New List(Of TrainingListener)()
		Protected Friend storageRouter As StatsStorageRouter
		Protected Friend isMQ As Boolean
		Protected Friend workspaceMode As WorkspaceMode
		Protected Friend trainerContextArgs() As Object
		Protected Friend debug As Boolean = False

		Protected Friend executorService As ThreadPoolExecutor

		Protected Friend ReadOnly workerCounter As New AtomicInteger(0)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator gradientsAccumulator;
		Protected Friend gradientsAccumulator As GradientsAccumulator

		' log uncaught exceptions
		Friend handler As Thread.UncaughtExceptionHandler = New UncaughtExceptionHandlerAnonymousInnerClass()

		Private Class UncaughtExceptionHandlerAnonymousInnerClass
			Inherits Thread.UncaughtExceptionHandler

			Public Sub uncaughtException(ByVal th As Thread, ByVal ex As Exception)
				log.error("Uncaught exception: " & ex)
				Console.WriteLine(ex.ToString())
				Console.Write(ex.StackTrace)
				If outerInstance.exceptionEncountered IsNot Nothing Then
					outerInstance.exceptionEncountered.set(True)
					outerInstance.exception = ex
				End If
			End Sub
		End Class

		Protected Friend Sub New(ByVal model As Model, ByVal workers As Integer, ByVal prefetchSize As Integer)
			Me.model = model
			Me.workers = workers
			Me.prefetchSize = prefetchSize

			If TypeOf Me.model Is MultiLayerNetwork Then
				DirectCast(Me.model, MultiLayerNetwork).Updater
			ElseIf TypeOf Me.model Is ComputationGraph Then
				DirectCast(Me.model, ComputationGraph).Updater
			End If
		End Sub

		Protected Friend Overridable Sub init()
			workerCounter.set(0)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: this.executorService = (java.util.concurrent.ThreadPoolExecutor) java.util.concurrent.Executors.newFixedThreadPool(workers, new java.util.concurrent.ThreadFactory()
			Me.executorService = CType(Executors.newFixedThreadPool(workers, New ThreadFactoryAnonymousInnerClass(Me)), ThreadPoolExecutor)
		End Sub

		Private Class ThreadFactoryAnonymousInnerClass
			Inherits ThreadFactory

			Private ReadOnly outerInstance As ParallelWrapper

			Public Sub New(ByVal outerInstance As ParallelWrapper)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Thread newThread(@NonNull final Runnable r)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
			Public Overrides Function newThread(ByVal r As ThreadStart) As Thread
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int cThread = workerCounter.getAndIncrement();
				Dim cThread As Integer = outerInstance.workerCounter.getAndIncrement()

				Dim t As New Thread(Sub()
				Nd4j.AffinityManager.unsafeSetDevice(cThread Mod Nd4j.AffinityManager.NumberOfDevices)
				r.run()
				End Sub)

				t.setName("ParallelWrapper training thread " & cThread)
				t.setDaemon(True)
				t.setUncaughtExceptionHandler(outerInstance.handler)

				Return t
			End Function
		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()
			If zoo IsNot Nothing Then
				For i As Integer = 0 To zoo.Length - 1
					If zoo(i) IsNot Nothing Then
						zoo(i).shutdown()
					End If
				Next i
				zoo = Nothing
			End If

			If executorService IsNot Nothing Then
				executorService.shutdown()
				executorService = Nothing
			End If

			If gradientsAccumulator IsNot Nothing Then
				gradientsAccumulator.reset()
			End If
		End Sub

		''' <summary>
		''' This method causes all threads used for parallel training to stop
		''' </summary>
		Public Overridable Sub shutdown()
			SyncLock Me
				Try
					close()
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End SyncLock
		End Sub

		''' <summary>
		''' Will stop a fit operation from continuing to iterate.
		''' </summary>
		Public Overridable Sub stopFit()
			stopFit_Conflict.set(True)
		End Sub

		''' 
		''' <param name="source"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void fit(@NonNull MultiDataSetIterator source)
		Public Overridable Sub fit(ByVal source As MultiDataSetIterator)
			SyncLock Me
				stopFit_Conflict.set(False)
				createZooIfNeccessary(True)
        
				If Not source.hasNext() AndAlso source.resetSupported() Then
					source.reset()
				End If
        
				Dim iterator As MultiDataSetIterator = source
				If prefetchSize > 0 AndAlso source.asyncSupported() Then
					If isMQ Then
						If workers Mod Nd4j.AffinityManager.NumberOfDevices <> 0 Then
							log.warn("Number of workers [{}] isn't optimal for available devices [{}]", workers, Nd4j.AffinityManager.NumberOfDevices)
						End If
        
						iterator = New AsyncMultiDataSetIterator(source, prefetchSize, New LinkedBlockingQueue(Of org.nd4j.linalg.dataset.api.MultiDataSet)(prefetchSize * workers), True, New InterleavedDataSetCallback(prefetchSize * 2))
					Else
						iterator = New AsyncMultiDataSetIterator(source, prefetchSize)
					End If
				End If
        
				Dim locker As val = New AtomicInteger(0)
        
				Dim blockWrapper As val = New DummyBlockMultiDataSetIterator(iterator)
        
				Dim time1 = DateTimeHelper.CurrentUnixTimeMillis()
				Do While blockWrapper.hasAnything() AndAlso Not stopFit_Conflict.get()
					If modelParamsSupplier IsNot Nothing Then
						Dim params As val = modelParamsSupplier.get()
						If params IsNot Nothing Then
							If zoo IsNot Nothing Then
								For Each z As val In zoo
									z.updateModelParams(params)
								Next z
							End If
						End If
					End If
        
					If updaterParamsSupplier IsNot Nothing Then
						Dim params As val = updaterParamsSupplier.get()
						If params IsNot Nothing Then
							If zoo IsNot Nothing Then
								For Each z As val In zoo
									z.updateUpdaterParams(params)
								Next z
							End If
						End If
					End If
        
					Dim dataSets As val = blockWrapper.next(workers)
					Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
        
					If dataSets Is Nothing Then
						Throw New ND4JIllegalStateException("You can't have NULL as MultiDataSet")
					End If
        
					locker.set(dataSets.length)
        
		'            
		'             * if we're using registerable accumulator (i.e. we're on spark or cuda with gradients sharing),
		'             * update it & notify about number of threads in this training round then
		'             
					If gradientsAccumulator IsNot Nothing AndAlso TypeOf gradientsAccumulator Is Registerable Then
						DirectCast(gradientsAccumulator, Registerable).registerConsumers(dataSets.length)
					End If
        
		'            
		'             now dataSet should be dispatched to next free workers, until all workers are busy. And then we should block till all finished.
		'            
        
					For pos As Integer = 0 To dataSets.length - 1
						zoo(pos).feedMultiDataSet(dataSets(pos), time2 - time1)
					Next pos
        
					iterationsCounter.incrementAndGet()
        
		'            
		'             * if all workers are dispatched now, join till all are finished
		'             
					For pos As Integer = 0 To dataSets.length - 1
						zoo(pos).waitTillRunning()
					Next pos
        
					'Nd4j.getMemoryManager().invokeGcOccasionally();
        
					' optional averaging
					If zoo(0).averagingRequired() AndAlso iterationsCounter.get() Mod averagingFrequency = 0 Then
		'                
		'                 * average model, and propagate it to all workers
		'                 
        
						Dim score As Double = getScore(locker)
        
						' averaging updaters state
						averageUpdatersState(locker, score)
					End If
        
					locker.set(0)
        
					time1 = DateTimeHelper.CurrentUnixTimeMillis()
				Loop
        
				If debug Then
					log.info("Stopping everyone...")
				End If
        
				If debug Then
					log.info("Shutting down iterator...")
				End If
        
				If prefetchSize > 0 AndAlso source.asyncSupported() Then
					DirectCast(iterator, AsyncMultiDataSetIterator).shutdown()
				End If
        
		'        
		'        // TODO: get rid of this code, 0 model is not replicated anyway
		'        // now we transfer models back from workers
		'        List<Model> models = new ArrayList<>();
		'        for (int i = 0; i < zoo.length; i++) {
		'            models.add(zoo[0].getModel());
		'        }
		'        
		'        // actual transfer code depends on trainer
		'        trainerContext.finalizeTraining(model, models.toArray(new Model[0]));
		'        
				Try
					close()
				Catch e As Exception
					Throw New Exception(e)
				End Try
        
				' sanity checks, or the dataset may never average
				If Not wasAveraged Then
					log.warn("Parameters were never averaged on current fit(). Ratios of batch size, num workers, and averaging frequency may be responsible.")
				End If
				'            throw new IllegalStateException("Parameters were never averaged. Please check batch size ratios, number of workers, and your averaging frequency.");
        
				log.debug("Iterations passed: {}", iterationsCounter.get())
				'        iterationsCounter.set(0);
			End SyncLock
		End Sub

		Private Function getScore(ByVal locker As AtomicInteger) As Double
			wasAveraged = True
			Dim score As Double = 0.0

			Dim params As IList(Of INDArray) = New List(Of INDArray)()
			Dim cnt As Integer = 0
			Do While cnt < workers AndAlso cnt < locker.get()
				params.Add(zoo(cnt).Model.params())
				score += zoo(cnt).Model.score()
				cnt += 1
			Loop

			Nd4j.averageAndPropagate(Nothing, params)


			score /= Math.Min(workers, locker.get())

			' TODO: improve this
			If reportScore Then
				log.info("Averaged score: " & score)
			End If

			Return score
		End Function

		Private Sub averageUpdatersState(ByVal locker As AtomicInteger, ByVal score As Double)
			' averaging updaters state
			If TypeOf model Is MultiLayerNetwork Then
				If averageUpdaters Then
					Dim updater As Updater = DirectCast(model, MultiLayerNetwork).Updater
					Dim batchSize As Integer = 0

					If updater IsNot Nothing AndAlso updater.StateViewArray IsNot Nothing Then
						Dim updaters As IList(Of INDArray) = New List(Of INDArray)()
						Dim cnt As Integer = 0
						Do While cnt < workers AndAlso cnt < locker.get()
							Dim workerModel As MultiLayerNetwork = DirectCast(zoo(cnt).Model, MultiLayerNetwork)
							updaters.Add(workerModel.Updater.StateViewArray)
							batchSize += workerModel.batchSize()
							cnt += 1
						Loop

						Nd4j.averageAndPropagate(Nothing, updaters)
					End If
				End If

				DirectCast(model, MultiLayerNetwork).Score = score
			ElseIf TypeOf model Is ComputationGraph Then
				If averageUpdaters Then
					Dim updater As ComputationGraphUpdater = DirectCast(model, ComputationGraph).Updater
					Dim batchSize As Integer = 0

					If updater IsNot Nothing AndAlso updater.StateViewArray IsNot Nothing Then
						Dim updaters As IList(Of INDArray) = New List(Of INDArray)()
						Dim cnt As Integer = 0
						Do While cnt < workers AndAlso cnt < locker.get()
							Dim workerModel As ComputationGraph = DirectCast(zoo(cnt).Model, ComputationGraph)
							updaters.Add(workerModel.Updater.StateViewArray)
							batchSize += workerModel.batchSize()
							cnt += 1
						Loop
						Nd4j.averageAndPropagate(Nothing, updaters)
					End If
				End If

				DirectCast(model, ComputationGraph).Score = score
			End If
		End Sub


		''' <summary>
		''' This method allows you to specify trainingListeners for this model.
		''' Note that for listeners like StatsListener (that have state that will be sent somewhere), consider instead
		''' using <seealso cref="setListeners(StatsStorageRouter, Collection)"/>
		''' </summary>
		''' <param name="listeners">    Listeners to set </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setListeners(@NonNull Collection<org.deeplearning4j.optimize.api.TrainingListener> listeners)
		Public Overridable WriteOnly Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				setListeners(Nothing, listeners)
			End Set
		End Property

		''' <summary>
		''' This method allows you to specify trainingListeners for this model.
		''' Note that for listeners like StatsListener (that have state that will be sent somewhere), consider instead
		''' using <seealso cref="setListeners(StatsStorageRouter, Collection)"/>
		''' </summary>
		''' <param name="listeners">    Listeners to set </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setListeners(@NonNull TrainingListener... listeners)
		Public Overridable WriteOnly Property Listeners As TrainingListener()
			Set(ByVal listeners() As TrainingListener)
				setListeners(java.util.Arrays.asList(listeners))
			End Set
		End Property

		''' <summary>
		''' Set the listeners, along with a StatsStorageRouter that the results will be shuffled to (in the case of any listeners
		''' that implement the <seealso cref="RoutingIterationListener"/> interface)
		''' </summary>
		''' <param name="statsStorage"> Stats storage router to place the results into </param>
		''' <param name="listeners">    Listeners to set </param>
		Public Overridable Sub setListeners(ByVal statsStorage As StatsStorageRouter, ParamArray ByVal listeners() As TrainingListener)
			setListeners(statsStorage, java.util.Arrays.asList(listeners))
		End Sub

		''' <summary>
		''' Set the listeners, along with a StatsStorageRouter that the results will be shuffled to (in the case of any listeners
		''' that implement the <seealso cref="RoutingIterationListener"/> interface)
		''' </summary>
		''' <param name="statsStorage"> Stats storage router to place the results into </param>
		''' <param name="listeners">    Listeners to set </param>
		Public Overridable Sub setListeners(Of T1 As TrainingListener)(ByVal statsStorage As StatsStorageRouter, ByVal listeners As ICollection(Of T1))
			'Check if we have any RoutingIterationListener instances that need a StatsStorage implementation...
			If listeners IsNot Nothing Then
				For Each l As TrainingListener In listeners
					If TypeOf l Is RoutingIterationListener Then
						Dim rl As RoutingIterationListener = DirectCast(l, RoutingIterationListener)
						If statsStorage Is Nothing AndAlso rl.StorageRouter Is Nothing Then
							log.warn("RoutingIterationListener provided without providing any StatsStorage instance. Iterator may not function without one. Listener: {}", l)
						End If
					End If
				Next l

				CType(Me.listeners_Conflict, List(Of TrainingListener)).AddRange(listeners)
			Else
				Me.listeners_Conflict.Clear()
			End If

			Me.storageRouter = statsStorage
		End Sub

		''' <summary>
		''' This method will propagate gradients across all workers
		''' </summary>
		''' <param name="gradients"> </param>
		Public Overridable Sub broadcastGradients(ByVal gradients As SharedGradient)
			' TODO: add implementation
	'        
	'            Basically all we want here is:
	'            1) Ensure length matches parameters length
	'            2) Ensure data is acessible from all devices somehow (i.e. it's in HOST-only mode
	'         
	'        
	'        if (zoo[0] instanceof CommunicativeTrainer) {
	'            for (int i = 0; i < zoo.length; i++) {
	'                ((CommunicativeTrainer) zoo[i]).enqueueGradient(gradients);
	'            }
	'        }
	'        
		End Sub


		''' <summary>
		''' This method takes DataSetIterator, and starts training over it by scheduling DataSets to different executors
		''' </summary>
		''' <param name="source"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void fit(@NonNull DataSetIterator source)
		Public Overridable Sub fit(ByVal source As DataSetIterator)
			SyncLock Me
				log.info("Using workspaceMode {} for training", workspaceMode.ToString())
				stopFit_Conflict.set(False)
				createZooIfNeccessary(False)
        
				If Not source.hasNext() AndAlso source.resetSupported() Then
					source.reset()
				End If
        
				Dim iterator As DataSetIterator = source
        
				If prefetchSize > 0 AndAlso source.asyncSupported() Then
					log.info("Creating asynchronous prefetcher...")
					If isMQ Then
						If workers Mod Nd4j.AffinityManager.NumberOfDevices <> 0 Then
							log.warn("Number of workers [{}] isn't optimal for available devices [{}]", workers, Nd4j.AffinityManager.NumberOfDevices)
						End If
        
						iterator = New AsyncDataSetIterator(source, prefetchSize, New LinkedBlockingQueue(Of org.nd4j.linalg.dataset.DataSet)(prefetchSize * workers), True, New InterleavedDataSetCallback(prefetchSize * 2))
        
					Else
						iterator = New AsyncDataSetIterator(source, prefetchSize)
					End If
				End If
        
        
				Dim nanos As val = New List(Of Long)()
				Dim locker As val = New AtomicInteger(0)
				Dim time1 = DateTimeHelper.CurrentUnixTimeMillis()
				log.info("Starting ParallelWrapper training round...")
				Dim intcnt As Long = 0
        
				Dim blockWrapper As val = New DummyBlockDataSetIterator(iterator)
        
				Do While blockWrapper.hasAnything() AndAlso Not stopFit_Conflict.get()
					If modelParamsSupplier IsNot Nothing Then
						Dim params As val = modelParamsSupplier.get()
						If params IsNot Nothing Then
							If zoo IsNot Nothing Then
								log.info("Updating model parameters...")
								For Each z As val In zoo
									z.updateModelParams(params)
								Next z
							End If
						End If
					End If
        
					If updaterParamsSupplier IsNot Nothing Then
						Dim params As val = updaterParamsSupplier.get()
						If params IsNot Nothing Then
							If zoo IsNot Nothing Then
								log.info("Updating updater parameters...")
								For Each z As val In zoo
									z.updateUpdaterParams(params)
								Next z
							End If
						End If
					End If
        
					intcnt += 1
					Dim dataSets As val = blockWrapper.next(workers)
					Dim time2 = DateTimeHelper.CurrentUnixTimeMillis()
					Dim lastEtlTime = time2 - time1
        
					If dataSets Is Nothing Then
						Throw New ND4JIllegalStateException("You can't have NULL as DataSet")
					End If
        
					If zoo Is Nothing Then
						Throw New System.InvalidOperationException("ParallelWrapper.shutdown() has been called too early and will fail from this point forward.")
					End If
        
					locker.set(dataSets.length)
		'            
		'             * if we're using registerable accumulator (i.e. we're on spark or cuda with gradients sharing),
		'             * update it & notify about number of threads in this training round then
		'             
					If gradientsAccumulator IsNot Nothing AndAlso TypeOf gradientsAccumulator Is Registerable Then
						DirectCast(gradientsAccumulator, Registerable).registerConsumers(dataSets.length)
					End If
        
        
					' feeding datasets
					For pos As Integer = 0 To dataSets.length - 1
						If debug Then
							log.info("Feeding dataset {} to worker {}", intcnt, pos)
						End If
        
						zoo(pos).feedDataSet(dataSets(pos), lastEtlTime)
					Next pos
        
					iterationsCounter.incrementAndGet()
        
        
					' waiting till all threads are done
					For pos As Integer = 0 To dataSets.length - 1
						Try
							zoo(pos).waitTillRunning()
						Catch e As Exception
							Throw New Exception(e)
						End Try
					Next pos
        
        
					' optional averaging
					If iterationsCounter.get() Mod averagingFrequency = 0 AndAlso zoo(0).averagingRequired() Then
						Dim timeA1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
        
						' model averaging happens within
						Dim score As Double = getScore(locker)
        
						' updaters averging happens within (if any)
						averageUpdatersState(locker, score)
        
						Dim timeA2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
						If reportScore Then
							log.info("Averaging time: {} ms", timeA2 - timeA1)
						End If
					End If
        
					time1 = DateTimeHelper.CurrentUnixTimeMillis()
					locker.set(0)
				Loop
        
				If debug Then
					log.info("Stopping everyone...")
				End If
        
				' ensure all threads stopped processing
				For cnt As Integer = 0 To workers - 1
					Try
						zoo(cnt).waitTillRunning()
					Catch e As Exception
						Throw New Exception(e)
					End Try
				Next cnt
        
				If debug Then
					log.info("Shutting down iterator...")
				End If
        
				If prefetchSize > 0 AndAlso source.asyncSupported() Then
					DirectCast(iterator, AsyncDataSetIterator).shutdown()
				End If
        
				Try
					close()
				Catch e As Exception
					Throw New Exception(e)
				End Try
        
				If debug Then
					log.info("Iterations passed: {}", iterationsCounter.get())
				End If
			End SyncLock
		End Sub


		Private Sub createZooIfNeccessary(ByVal useMDS As Boolean)
			If zoo Is Nothing Then
				trainerContext.init(model, trainerContextArgs)

				zoo = New Trainer(workers - 1){}
				Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices
				For cnt As Integer = 0 To workers - 1
					' we pass true here, to tell Trainer to use MultiDataSet queue for training
					zoo(cnt) = trainerContext.create(Me.uuid, cnt, model, Nd4j.AffinityManager.getDeviceForCurrentThread(), useMDS, Me, workspaceMode, averagingFrequency)

	'                
	'                zoo[cnt].setUncaughtExceptionHandler(handler);
	'                
	'                if (zoo[cnt] instanceof Thread) {
	'                    Nd4j.getAffinityManager().attachThreadToDevice((Thread) zoo[cnt], cnt % numDevices);
	'                }
	'                zoo[cnt].start();
	'                

					If executorService Is Nothing Then
						init()
					End If

					executorService.execute(zoo(cnt))
				Next cnt
			End If
		End Sub

		Public Class Builder(Of T As org.deeplearning4j.nn.api.Model)
'JAVA TO VB CONVERTER NOTE: The field trainingMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend trainingMode_Conflict As TrainingMode = TrainingMode.AVERAGING
			Protected Friend model As T
'JAVA TO VB CONVERTER NOTE: The field workers was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend workers_Conflict As Integer = Nd4j.AffinityManager.NumberOfDevices
			Protected Friend prefetchSize As Integer = 16
'JAVA TO VB CONVERTER NOTE: The field averagingFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend averagingFrequency_Conflict As Integer = 1
			Protected Friend reportScore As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field averageUpdaters was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend averageUpdaters_Conflict As Boolean = True
			Protected Friend legacyAveraging As Boolean = True
			Protected Friend isMQ As Boolean = Nd4j.AffinityManager.NumberOfDevices > 1
			Protected Friend trainerContext As TrainerContext = New DefaultTrainerContext()
'JAVA TO VB CONVERTER NOTE: The field trainerContextArgs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend trainerContextArgs_Conflict() As Object
'JAVA TO VB CONVERTER NOTE: The field workspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend workspaceMode_Conflict As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER NOTE: The field modelParamsSupplier was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend modelParamsSupplier_Conflict As Supplier(Of INDArray)
'JAVA TO VB CONVERTER NOTE: The field updaterParamsSupplier was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend updaterParamsSupplier_Conflict As Supplier(Of INDArray)
'JAVA TO VB CONVERTER NOTE: The field thresholdAlgorithm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend thresholdAlgorithm_Conflict As ThresholdAlgorithm
'JAVA TO VB CONVERTER NOTE: The field residualPostProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend residualPostProcessor_Conflict As ResidualPostProcessor
			Protected Friend encoderMemory As Long? = -1L

			Protected Friend accumulator As GradientsAccumulator

			''' <summary>
			''' Transer context args are for calling a
			''' <seealso cref="TrainerContext"/> init method
			''' when <seealso cref="ParallelWrapper"/> starts training </summary>
			''' <param name="trainerContextArgs"> the args to use (maybe null)
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter trainerContextArgs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function trainerContextArgs(ParamArray ByVal trainerContextArgs_Conflict() As Object) As Builder
				Me.trainerContextArgs_Conflict = trainerContextArgs_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify a <seealso cref="TrainerContext"/>
			''' for the given <seealso cref="ParallelWrapper"/>
			''' instance.
			''' Defaults to <seealso cref="DefaultTrainerContext"/>
			''' otherwise </summary>
			''' <param name="trainerContext"> the trainer factory to use </param>
			''' <returns> builder pattern </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainerFactory(@NonNull TrainerContext trainerContext)
			Public Overridable Function trainerFactory(ByVal trainerContext As TrainerContext) As Builder
				Me.trainerContext = trainerContext
				Return Me
			End Function

			''' <summary>
			''' This method allows to override model's WorkspaceMode configuration option </summary>
			''' <param name="mode">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder workspaceMode(@NonNull WorkspaceMode mode)
			Public Overridable Function workspaceMode(ByVal mode As WorkspaceMode) As Builder
				Me.workspaceMode_Conflict = mode
				Return Me
			End Function

			''' <summary>
			''' This method attaches supplier that'll probably provide model params update
			''' 
			''' PLEASE NOTE: This method is mostly used in Spark environment as part of fault tolerance logic </summary>
			''' <param name="supplier">
			''' @return </param>
			Public Overridable Function modelParamsSupplier(ByVal supplier As Supplier(Of INDArray)) As Builder
				Me.modelParamsSupplier_Conflict = supplier
				Return Me
			End Function

			''' <summary>
			''' This method attaches supplier that'll probably provide updater params update
			''' 
			''' PLEASE NOTE: This method is mostly used in Spark environment as part of fault tolerance logic </summary>
			''' <param name="supplier">
			''' @return </param>
			Public Overridable Function updaterParamsSupplier(ByVal supplier As Supplier(Of INDArray)) As Builder
				Me.updaterParamsSupplier_Conflict = supplier
				Return Me
			End Function

			''' <summary>
			''' Build ParallelWrapper for MultiLayerNetwork
			''' </summary>
			''' <param name="model"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull T model)
			Public Sub New(ByVal model As T)
				Me.model = model
			End Sub

			''' <summary>
			''' This method allows to configure number of workers that'll be used for parallel training
			''' </summary>
			''' <param name="num">
			''' @return </param>
			Public Overridable Function workers(ByVal num As Integer) As Builder
				If num < 2 Then
					Throw New Exception("Number of workers can't be lower then 2!")
				End If

				Me.workers_Conflict = num
				Return Me
			End Function

			''' <summary>
			''' Model averaging frequency.
			''' </summary>
			''' <param name="freq"> number of iterations between averaging
			''' @return </param>
			Public Overridable Function averagingFrequency(ByVal freq As Integer) As Builder
				If freq < 0 Then
					freq = 0
				End If

				Me.averagingFrequency_Conflict = freq
				Return Me
			End Function

			''' <summary>
			''' This method enables/disables updaters averaging.
			''' 
			''' Default value: TRUE
			''' 
			''' PLEASE NOTE: This method is suitable for debugging purposes mostly. So don't change default value, unless you're sure why you need it.
			''' PLEASE NOTE: This method is suitable for parameters averaging training only. For gradients sharing mechanism it'll be ignored
			''' </summary>
			''' <param name="reallyAverage">
			''' @return </param>
			Public Overridable Function averageUpdaters(ByVal reallyAverage As Boolean) As Builder
				Me.averageUpdaters_Conflict = reallyAverage
				Return Me
			End Function


			''' <summary>
			''' Size of prefetch buffer that will be used for background data prefetching.
			''' Usually it's better to keep this value equal to the number of workers.
			''' 
			''' Default value: 2
			''' </summary>
			''' <param name="size"> 0 to disable prefetching, any positive number
			''' @return </param>
			Public Overridable Function prefetchBuffer(ByVal size As Integer) As Builder
				If size < 0 Then
					size = 0
				End If

				Me.prefetchSize = size

				Return Me
			End Function

			''' <summary>
			'''  This method allows you to specify training mode for this instance of PW.<br>
			'''  1) AVERAGING - stands for parameters averaging. Each X epochs weights and updaters state will be averaged across all models<br>
			'''  2) SHARED_GRADIENTS - stands for gradients sharing - more details available here: <a href="https://deeplearning4j.konduit.ai/distributed-deep-learning/intro">https://deeplearning4j.konduit.ai/distributed-deep-learning/intro</a><br>
			'''  3) CUSTOM - this method allows you to specify custom gradients accumulator, this giving you better control of configuration params for training.<br>
			''' </summary>
			''' <param name="mode">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainingMode(@NonNull TrainingMode mode)
			Public Overridable Function trainingMode(ByVal mode As TrainingMode) As Builder
				Me.trainingMode_Conflict = mode
				Return Me
			End Function

			''' <summary>
			''' This method allows you to specify GradientsAccumulator instance to be used in this ParallelWrapper instance
			''' 
			''' PLEASE NOTE: This method is applicable only to gradients sharing mechanics. If parameters averaging is used, accumulator will be ignored
			''' </summary>
			''' <param name="accumulator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder gradientsAccumulator(@NonNull GradientsAccumulator accumulator)
			Public Overridable Function gradientsAccumulator(ByVal accumulator As GradientsAccumulator) As Builder
				Me.accumulator = accumulator
				Return Me
			End Function


			''' <summary>
			''' This method enables/disables averaged model score reporting
			''' </summary>
			''' <param name="reallyReport">
			''' @return </param>
			Public Overridable Function reportScoreAfterAveraging(ByVal reallyReport As Boolean) As Builder
				Me.reportScore = reallyReport
				Return Me
			End Function

			''' <summary>
			''' Set the threshold algorithm. Not used for single machine training (only for PW used in a distributed setting),
			''' and should not be set by users in most cases. </summary>
			''' <param name="thresholdAlgorithm"> Threshold algorithm to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter thresholdAlgorithm was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function thresholdAlgorithm(ByVal thresholdAlgorithm_Conflict As ThresholdAlgorithm) As Builder
				Me.thresholdAlgorithm_Conflict = thresholdAlgorithm_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method allows to define amount of temporary memory that will be used for gradients sharing.
			''' Typically it's safe to keep default value.
			''' 
			''' Default value: -1, amount of temporary memory will be calculated automatically </summary>
			''' <param name="numBytes"> number of bytes to be used
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder temporaryMemory(@NonNull Long numBytes)
			Public Overridable Function temporaryMemory(ByVal numBytes As Long) As Builder
				Me.encoderMemory = numBytes
				Return Me
			End Function

			''' <summary>
			''' Set the residual post processor algorithm. Not used for single machine training (only for PW used in a
			''' distributed setting), and should not be set by users in most cases. </summary>
			''' <param name="residualPostProcessor"> Residual post processor to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter residualPostProcessor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function residualPostProcessor(ByVal residualPostProcessor_Conflict As ResidualPostProcessor) As Builder
				Me.residualPostProcessor_Conflict = residualPostProcessor_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method returns ParallelWrapper instance
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As ParallelWrapper
				Dim wrapper As New ParallelWrapper(model, workers_Conflict, prefetchSize)
				wrapper.averagingFrequency = Me.averagingFrequency_Conflict
				wrapper.reportScore = Me.reportScore
				wrapper.averageUpdaters = Me.averageUpdaters_Conflict
				wrapper.legacyAveraging = Me.legacyAveraging
				wrapper.isMQ = Me.isMQ
				wrapper.workspaceMode = Me.workspaceMode_Conflict
				wrapper.modelParamsSupplier = Me.modelParamsSupplier_Conflict
				wrapper.updaterParamsSupplier = Me.updaterParamsSupplier_Conflict


				Select Case trainingMode_Conflict
					Case org.deeplearning4j.parallelism.ParallelWrapper.TrainingMode.AVERAGING
						Me.trainerContext = New DefaultTrainerContext()
						Me.accumulator = Nothing
						log.info("Creating new AveragingTraining instance")
					Case org.deeplearning4j.parallelism.ParallelWrapper.TrainingMode.SHARED_GRADIENTS
						If thresholdAlgorithm_Conflict Is Nothing Then
							thresholdAlgorithm_Conflict = New AdaptiveThresholdAlgorithm()
						End If

						Me.trainerContext = New SymmetricTrainerContext()
						If Me.accumulator Is Nothing Then
							log.info("Creating new GradientsAccumulator instance with default threshold of [5e-4]")
							Dim numParams As val = model.numParams()

							' we're limiting max size of updates for Sparse encoding to the size of bitmap encoded message
							Dim maxUpdate As val = CInt(numParams / 16 + 5)

							' memory sie in number of bytes
							Dim memorySize As Long = If(encoderMemory Is Nothing OrElse encoderMemory < 0, maxUpdate * 4 * (workers_Conflict + 3), encoderMemory)

							Me.accumulator = New EncodedGradientsAccumulator(workers_Conflict, New EncodingHandler(thresholdAlgorithm_Conflict, residualPostProcessor_Conflict, maxUpdate, False), memorySize, workers_Conflict + 2, Integer.MaxValue, False)
						End If
					Case org.deeplearning4j.parallelism.ParallelWrapper.TrainingMode.CUSTOM
						Me.trainerContext = New SymmetricTrainerContext()
						If Me.accumulator Is Nothing Then
							Throw New DL4JInvalidConfigException("Please specify GradientsAccumulator fo encoded gradients mode")
						End If
					Case Else
						Throw New System.NotSupportedException("Unknown trainingMode: [" & trainingMode_Conflict & "]")
				End Select

				wrapper.trainerContext = Me.trainerContext
				wrapper.gradientsAccumulator = Me.accumulator

				wrapper.init()

				Dim modelListeners As IList(Of TrainingListener) = Nothing
				If TypeOf model Is MultiLayerNetwork Then
					modelListeners = New List(Of TrainingListener)(DirectCast(model, MultiLayerNetwork).getListeners())
					model.setListeners(java.util.Collections.emptyList())
				ElseIf TypeOf model Is ComputationGraph Then
					modelListeners = New List(Of TrainingListener)(DirectCast(model, ComputationGraph).getListeners())
					model.setListeners(java.util.Collections.emptyList())
				End If

				If modelListeners IsNot Nothing AndAlso modelListeners.Count > 0 Then
					wrapper.setListeners(modelListeners)
				End If

				Return wrapper
			End Function
		End Class

		Private Shared Function cloneListener(ByVal original As TrainingListener) As TrainingListener
			If TypeOf original Is RoutingIterationListener Then
				Return DirectCast(original, RoutingIterationListener).clone()
			End If
			Return original
		End Function

		Private Sub configureListeners(ByVal workerUUID As String, ByVal oldListeners As ICollection(Of TrainingListener), ByVal replicatedListeners As ICollection(Of TrainingListener))
			For Each listener As TrainingListener In oldListeners
				Dim l As TrainingListener = cloneListener(listener)

				If TypeOf l Is RoutingIterationListener Then
					Dim rl As RoutingIterationListener = DirectCast(l, RoutingIterationListener)
					'We're assuming session ID is set by the original RoutingIterationListener constructor, which means
					' it will be synced across all cloned instances
					rl.SessionID = DirectCast(listener, RoutingIterationListener).SessionID
					rl.WorkerID = workerUUID

					Dim currentRouter As StatsStorageRouter = DirectCast(listener, RoutingIterationListener).StorageRouter
					If currentRouter IsNot Nothing Then
						'User has set router on the listener/model, instead of via the
						' setListeners(StatsStorageRouter, ...) method
						rl.StorageRouter = currentRouter
					Else
						rl.StorageRouter = ParallelWrapper.this.storageRouter
					End If

				End If
				replicatedListeners.Add(l)
			Next listener
		End Sub
	End Class


End Namespace