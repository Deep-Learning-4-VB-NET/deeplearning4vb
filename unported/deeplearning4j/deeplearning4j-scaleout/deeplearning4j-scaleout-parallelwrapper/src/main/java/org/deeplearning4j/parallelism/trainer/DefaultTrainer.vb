Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports Model = org.deeplearning4j.nn.api.Model
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ParallelWrapper = org.deeplearning4j.parallelism.ParallelWrapper
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.parallelism.trainer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Slf4j @NoArgsConstructor @AllArgsConstructor public class DefaultTrainer extends Thread implements Trainer
	Public Class DefaultTrainer
		Inherits Thread
		Implements Trainer

		Protected Friend replicatedModel As Model

		' TODO: make queue size configurable
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected java.util.concurrent.LinkedBlockingQueue<org.nd4j.linalg.dataset.api.DataSet> queue = new java.util.concurrent.LinkedBlockingQueue<>(1);
		Protected Friend queue As New LinkedBlockingQueue(Of DataSet)(1)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected java.util.concurrent.LinkedBlockingQueue<org.nd4j.linalg.dataset.api.MultiDataSet> queueMDS = new java.util.concurrent.LinkedBlockingQueue<>(1);
		Protected Friend queueMDS As New LinkedBlockingQueue(Of MultiDataSet)(1)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected java.util.concurrent.atomic.AtomicInteger running = new java.util.concurrent.atomic.AtomicInteger(0);
'JAVA TO VB CONVERTER NOTE: The field running was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend running_Conflict As New AtomicInteger(0)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected java.util.concurrent.atomic.AtomicBoolean shouldUpdate = new java.util.concurrent.atomic.AtomicBoolean(false);
		Protected Friend shouldUpdate As New AtomicBoolean(False)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected java.util.concurrent.atomic.AtomicBoolean shouldStop = new java.util.concurrent.atomic.AtomicBoolean(false);
		Protected Friend shouldStop As New AtomicBoolean(False)
		Protected Friend thrownException As Exception
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected volatile boolean useMDS = false;
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Protected Friend useMDS As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected String uuid;
		Protected Friend uuid As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected boolean onRootModel = false;
		Protected Friend onRootModel As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected volatile java.util.concurrent.atomic.AtomicLong lastEtlTime = new java.util.concurrent.atomic.AtomicLong(0);
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Protected Friend lastEtlTime As New AtomicLong(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected java.util.concurrent.atomic.AtomicBoolean nullMode = new java.util.concurrent.atomic.AtomicBoolean(false);
		Protected Friend nullMode As New AtomicBoolean(False)
		Protected Friend nullDataSet As DataSet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected java.util.concurrent.atomic.AtomicBoolean isStopped = new java.util.concurrent.atomic.AtomicBoolean(false);
		Protected Friend isStopped As New AtomicBoolean(False)

		Protected Friend parallelWrapper As ParallelWrapper
		Protected Friend workspaceMode As WorkspaceMode
		Protected Friend averagingFrequency As Integer
		Protected Friend threadId As Integer
		Protected Friend originalModel As Model

		Protected Friend ReadOnly modelLock As New ReentrantReadWriteLock()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void feedMultiDataSet(@NonNull MultiDataSet dataSet, long etlTime)
		Public Overridable Sub feedMultiDataSet(ByVal dataSet As MultiDataSet, ByVal etlTime As Long) Implements Trainer.feedMultiDataSet
			setupIfNeccessary()
			Try
				queueMDS.put(dataSet)
				running_Conflict.incrementAndGet()
			Catch e As InterruptedException
				Thread.CurrentThread.Interrupt()
				' do nothing
			End Try

			If lastEtlTime Is Nothing Then
				lastEtlTime = New AtomicLong(0)
			End If

			lastEtlTime.set(etlTime)
		End Sub

		Public Overridable Sub feedDataSet(ByVal dataSet As DataSet, ByVal etlTime As Long)
			setupIfNeccessary()
			If dataSet IsNot Nothing Then
				Try
					queue.put(dataSet)
					running_Conflict.incrementAndGet()
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					' do nothing
				End Try
			Else
				If nullMode Is Nothing Then
					nullMode = New AtomicBoolean(False)
				End If

				nullMode.set(True)
			End If

			If lastEtlTime Is Nothing Then
				lastEtlTime = New AtomicLong(0)
			End If

			lastEtlTime.set(etlTime)
		End Sub

		Public Overridable ReadOnly Property Model As Model Implements Trainer.getModel
			Get
				Return replicatedModel
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void updateModel(@NonNull Model model)
		Public Overridable Sub updateModel(ByVal model As Model) Implements Trainer.updateModel
			Me.shouldUpdate.set(True)
			Try
				modelLock.writeLock().lock()
				If TypeOf replicatedModel Is MultiLayerNetwork Then


					replicatedModel.Params = model.params().unsafeDuplication(True)

					Dim updater As Updater = CType(model, MultiLayerNetwork).Updater
					Dim view As INDArray = updater.StateViewArray

					If view IsNot Nothing Then
						updater = DirectCast(replicatedModel, MultiLayerNetwork).Updater
						Dim viewD As INDArray = view.dup()

						Nd4j.Executioner.commit()

						updater.setStateViewArray(DirectCast(replicatedModel, MultiLayerNetwork), viewD, False)
					End If
				ElseIf TypeOf replicatedModel Is ComputationGraph Then
					replicatedModel.Params = model.params().unsafeDuplication(True)

					Dim updater As ComputationGraphUpdater = CType(model, ComputationGraph).Updater
					Dim view As INDArray = updater.StateViewArray

					If view IsNot Nothing Then
						Dim viewD As INDArray = view.dup()

						Nd4j.Executioner.commit()

						updater = DirectCast(replicatedModel, ComputationGraph).Updater
						updater.StateViewArray = viewD
					End If
				End If

				Nd4j.Executioner.commit()
			Finally
				modelLock.writeLock().unlock()
			End Try
		End Sub



		Protected Friend Overridable Sub setupIfNeccessary()
			If queue Is Nothing Then
				queue = New LinkedBlockingQueue(Of DataSet)(1)
			End If
			If queueMDS Is Nothing Then
				queueMDS = New LinkedBlockingQueue(Of MultiDataSet)(1)
			End If
			If running_Conflict Is Nothing Then
				running_Conflict = New AtomicInteger(0)
			End If
			If shouldStop Is Nothing Then
				shouldStop = New AtomicBoolean(False)
			End If
			If shouldUpdate Is Nothing Then
				shouldUpdate = New AtomicBoolean(False)
			End If
			If isStopped Is Nothing Then
				isStopped = New AtomicBoolean(False)
			End If
			If lastEtlTime Is Nothing Then
				lastEtlTime = New AtomicLong(0)
			End If
		End Sub

		Public Overridable ReadOnly Property Running As Boolean Implements Trainer.isRunning
			Get
				' if Trainer thread got exception during training - rethrow it here
				If thrownException IsNot Nothing Then
					Throw New Exception(thrownException)
				End If
    
				Return running_Conflict.get() = 0
			End Get
		End Property

		Public Overridable Sub shutdown() Implements Trainer.shutdown
			shouldStop.set(True)
			Do While Not isStopped.get()
				LockSupport.parkNanos(1000L)
			Loop

			shouldStop.set(False)
			isStopped.set(False)
		End Sub

		Protected Friend Overridable Sub fit(ByVal dataSet As DataSet)
			If TypeOf replicatedModel Is MultiLayerNetwork Then
				If lastEtlTime Is Nothing Then
					lastEtlTime = New AtomicLong(0)
				End If

				DirectCast(replicatedModel, MultiLayerNetwork).LastEtlTime = lastEtlTime.get()

				' we want this model locked out for possible updates
				Try
					modelLock.readLock().lock()
					DirectCast(replicatedModel, MultiLayerNetwork).fit(dataSet)
				Finally
					modelLock.readLock().unlock()
				End Try
			ElseIf TypeOf replicatedModel Is ComputationGraph Then
				If lastEtlTime Is Nothing Then
					lastEtlTime = New AtomicLong(0)
				End If

				DirectCast(replicatedModel, ComputationGraph).LastEtlTime = lastEtlTime.get()

				' we want this model locked out for possible updates
				Try
					modelLock.readLock().lock()
					DirectCast(replicatedModel, ComputationGraph).fit(dataSet)
				Finally
					modelLock.readLock().unlock()
				End Try
			End If
		End Sub

		Protected Friend Overridable Sub fit(ByVal dataSet As MultiDataSet)
			If lastEtlTime Is Nothing Then
				lastEtlTime = New AtomicLong(0)
			End If

			DirectCast(replicatedModel, ComputationGraph).LastEtlTime = lastEtlTime.get()

			' we want this model locked out for possible updates
			Try
				modelLock.readLock().lock()
				DirectCast(replicatedModel, ComputationGraph).fit(dataSet)
			Finally
				modelLock.readLock().unlock()
			End Try
		End Sub

		''' <summary>
		''' This method does post-initialization configuration of Model.
		''' Good place to configure listeners and all such a things
		''' </summary>
		Protected Friend Overridable Sub postInit()
			Dim oldListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()
			Dim replicatedListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()

			If parallelWrapper.getListeners() IsNot Nothing Then
				oldListeners.addAll(parallelWrapper.getListeners())
			End If
			configureListeners(uuid, oldListeners, replicatedListeners)

			Me.replicatedModel.setListeners(replicatedListeners)
		End Sub

		Public Overrides Sub run()
			setupIfNeccessary()
			Dim iterationsCounter As New AtomicInteger(0)

			' FIXME: make this thing CUDA-compatible, and avoid RC at originalModel relocation
			If threadId = 0 Then
				onRootModel = True
			End If

			Try
				' we create fresh network, with the same configuration, as initially created by user
				' however, we don't need clone or anything here
				If TypeOf originalModel Is MultiLayerNetwork Then
					If Not onRootModel Then
						Dim conf As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(DirectCast(originalModel, MultiLayerNetwork).LayerWiseConfigurations.toJson())
						conf.setTrainingWorkspaceMode(workspaceMode)
						Me.replicatedModel = New MultiLayerNetwork(conf)

						replicatedModel.init()

						' we replicate original model params & updater state, just in case it's pre-trained model
						Try
							modelLock.writeLock().lock()
							replicatedModel.Params = originalModel.params().unsafeDuplication(True)

							Dim updaterReplica As Updater = DirectCast(replicatedModel, MultiLayerNetwork).Updater
							Dim updaterOrigina As Updater = DirectCast(originalModel, MultiLayerNetwork).Updater

							If updaterOrigina IsNot Nothing AndAlso updaterOrigina.StateViewArray IsNot Nothing Then
								updaterReplica.setStateViewArray(DirectCast(replicatedModel, MultiLayerNetwork), updaterOrigina.StateViewArray.unsafeDuplication(True), False)
							End If

							Nd4j.Executioner.commit()
						Finally
							modelLock.writeLock().unlock()
						End Try
					Else
						Me.replicatedModel = originalModel
						If Not DirectCast(replicatedModel, MultiLayerNetwork).InitCalled Then
							Me.replicatedModel.init()
						End If

						DirectCast(replicatedModel, MultiLayerNetwork).LayerWiseConfigurations.setTrainingWorkspaceMode(workspaceMode)
					End If
				ElseIf TypeOf originalModel Is ComputationGraph Then
					If Not onRootModel Then
						Dim conf As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(DirectCast(originalModel, ComputationGraph).Configuration.toJson())
						conf.setTrainingWorkspaceMode(workspaceMode)

						Me.replicatedModel = New ComputationGraph(conf)
						Me.replicatedModel.init()

						' we replicate original model params & updater state, just in case it's pre-trained model
						Try
							modelLock.writeLock().lock()
							replicatedModel.Params = originalModel.params().unsafeDuplication(True)

							Dim updaterReplica As ComputationGraphUpdater = DirectCast(replicatedModel, ComputationGraph).Updater
							Dim updaterOrigina As ComputationGraphUpdater = DirectCast(originalModel, ComputationGraph).Updater

							If updaterOrigina IsNot Nothing AndAlso updaterOrigina.StateViewArray IsNot Nothing Then
								updaterReplica.StateViewArray = updaterOrigina.StateViewArray.unsafeDuplication(True)
							End If

							Nd4j.Executioner.commit()
						Finally
							modelLock.writeLock().unlock()
						End Try
					Else
						Me.replicatedModel = originalModel
						Me.replicatedModel.init()
						DirectCast(replicatedModel, ComputationGraph).Configuration.setTrainingWorkspaceMode(workspaceMode)
					End If
				End If

				If replicatedModel Is Nothing Then
					log.error("replicatedModel is NULL at worker_{}", threadId)
				End If

				' classes that extend DefaultTrainer might hook something there
				postInit()

				If Not useMDS Then
					Do While Not shouldStop.get()
						Dim dataSet As DataSet = Nothing
						If nullMode Is Nothing OrElse Not nullMode.get() Then
							dataSet = queue.poll(10, TimeUnit.MILLISECONDS)
						Else
							' this code branch is for debugging only, please ignore :)
							If nullDataSet Is Nothing Then
								nullDataSet = New org.nd4j.linalg.dataset.DataSet(Nd4j.create(64, 28 * 28), Nd4j.create(64, 10))
							End If

							dataSet = nullDataSet
						End If
						If dataSet IsNot Nothing Then

							fit(dataSet)

							' if we don't support cross-device stuff (like multi-gpu on windows) - sync back to host
							If Not Nd4j.AffinityManager.CrossDeviceAccessSupported AndAlso (averagingFrequency = 0 OrElse iterationsCounter.incrementAndGet() Mod averagingFrequency = 0) AndAlso averagingRequired() Then
								' we ensure all operations are finished in this training round
								Nd4j.Executioner.commit()

								' we ensure memory is updated on host side
								Nd4j.AffinityManager.ensureLocation(replicatedModel.params(), AffinityManager.Location.HOST)

								If TypeOf replicatedModel Is MultiLayerNetwork Then
									Dim updaterReplica As Updater = DirectCast(replicatedModel, MultiLayerNetwork).Updater
									If updaterReplica.StateViewArray IsNot Nothing Then
										Nd4j.AffinityManager.ensureLocation(updaterReplica.StateViewArray, AffinityManager.Location.HOST)
									End If
								Else
									Dim updaterReplica As ComputationGraphUpdater = DirectCast(replicatedModel, ComputationGraph).Updater

									If updaterReplica.StateViewArray IsNot Nothing Then
										Nd4j.AffinityManager.ensureLocation(updaterReplica.StateViewArray, AffinityManager.Location.HOST)
									End If
								End If
							End If

							running_Conflict.decrementAndGet()
						End If
					Loop
				Else
					' loop for MultiDataSet
					Do While Not shouldStop.get()
						Dim dataSet As MultiDataSet = queueMDS.poll(10, TimeUnit.MILLISECONDS)
						If dataSet IsNot Nothing Then

							' just fitting
							fit(dataSet)

							' if we don't support cross-device stuff (like multi-gpu on windows) - sync back to host
							If Not Nd4j.AffinityManager.CrossDeviceAccessSupported AndAlso (averagingFrequency = 0 OrElse iterationsCounter.incrementAndGet() Mod averagingFrequency = 0) AndAlso averagingRequired() Then
								' we ensure all operations are finished in this training round
								Nd4j.Executioner.commit()

								' we ensure memory is updated on host side
								Nd4j.AffinityManager.ensureLocation(replicatedModel.params(), AffinityManager.Location.HOST)

								Dim updaterReplica As ComputationGraphUpdater = DirectCast(replicatedModel, ComputationGraph).Updater

								If updaterReplica.StateViewArray IsNot Nothing Then
									Nd4j.AffinityManager.ensureLocation(updaterReplica.StateViewArray, AffinityManager.Location.HOST)
								End If
							End If

							running_Conflict.decrementAndGet()
						End If
					Loop
				End If
			Catch e As Exception
				Me.thrownException = e
				Throw New Exception(e)
			Finally
				log.debug("Terminating all workspaces for trainer_{}", threadId)
				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()

				If Not onRootModel Then
					replicatedModel.close()
				End If

				' let's try to enforce GC to actually clean all references now
				replicatedModel.clear()
				System.GC.Collect()
				isStopped.set(True)
			End Try
		End Sub

		Public Overridable Sub waitTillRunning() Implements Trainer.waitTillRunning
			Do While running_Conflict.get() <> 0
				' if Trainer thread got exception during training - rethrow it here
				'log.info("Thread {} running {}", Thread.currentThread().getId(), running.get());
				If thrownException IsNot Nothing Then
					Throw New Exception(thrownException)
				End If

				LockSupport.parkNanos(1000L)
			Loop
		End Sub


		Public Overridable Sub updateModelParams(ByVal params As INDArray) Implements Trainer.updateModelParams
			Try
				modelLock.writeLock().lock()

				' just set it right away
				replicatedModel.Params = params.unsafeDuplication(True)
				Nd4j.Executioner.commit()
			Finally
				modelLock.writeLock().unlock()
			End Try
		End Sub

		Public Overridable Sub updateUpdaterParams(ByVal params As INDArray) Implements Trainer.updateUpdaterParams
			Try
				modelLock.writeLock().lock()

				If TypeOf replicatedModel Is ComputationGraph Then
					DirectCast(replicatedModel, ComputationGraph).Updater.StateViewArray.assign(params.unsafeDuplication(True))
				ElseIf TypeOf replicatedModel Is MultiLayerNetwork Then
					DirectCast(replicatedModel, MultiLayerNetwork).Updater.StateViewArray.assign(params.unsafeDuplication(True))
				End If

				Nd4j.Executioner.commit()
			Finally
				modelLock.writeLock().unlock()
			End Try
		End Sub

		Public Overridable Function averagingRequired() As Boolean Implements Trainer.averagingRequired
			Return True
		End Function

		Protected Friend Shared Function cloneListener(ByVal original As TrainingListener) As TrainingListener
			If TypeOf original Is RoutingIterationListener Then
				Return DirectCast(original, RoutingIterationListener).clone()
			End If
			Return original
		End Function


		Protected Friend Overridable Sub configureListeners(ByVal workerUUID As String, ByVal oldListeners As ICollection(Of TrainingListener), ByVal replicatedListeners As ICollection(Of TrainingListener))
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
						rl.StorageRouter = parallelWrapper.getStorageRouter()
					End If

				End If
				If Not replicatedListeners.Contains((l)) Then
					replicatedListeners.Add(l)
				End If
			Next listener
		End Sub


		Public Class DefaultTrainerBuilder
			Public Sub New()
			End Sub
		End Class

	End Class

End Namespace