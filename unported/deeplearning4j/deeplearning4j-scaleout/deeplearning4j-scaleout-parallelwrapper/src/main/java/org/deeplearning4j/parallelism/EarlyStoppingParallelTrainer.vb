Imports System
Imports System.Collections.Generic
Imports AtomicDouble = org.nd4j.shade.guava.util.concurrent.AtomicDouble
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports org.deeplearning4j.earlystopping.scorecalc
Imports EpochTerminationCondition = org.deeplearning4j.earlystopping.termination.EpochTerminationCondition
Imports IterationTerminationCondition = org.deeplearning4j.earlystopping.termination.IterationTerminationCondition
Imports org.deeplearning4j.earlystopping.trainer
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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

Namespace org.deeplearning4j.parallelism


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EarlyStoppingParallelTrainer<T extends org.deeplearning4j.nn.api.Model> implements org.deeplearning4j.earlystopping.trainer.IEarlyStoppingTrainer<T>
	Public Class EarlyStoppingParallelTrainer(Of T As org.deeplearning4j.nn.api.Model)
		Implements IEarlyStoppingTrainer(Of T)


		Protected Friend model As T

		Protected Friend ReadOnly esConfig As EarlyStoppingConfiguration(Of T)
		Private ReadOnly train As DataSetIterator
		Private ReadOnly trainMulti As MultiDataSetIterator
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private final Iterator<?> iterator;
		Private ReadOnly iterator As IEnumerator(Of Object)
'JAVA TO VB CONVERTER NOTE: The field listener was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listener_Conflict As EarlyStoppingListener(Of T)
		Private wrapper As ParallelWrapper
		Private bestModelScore As Double = Double.MaxValue
		Private bestModelEpoch As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field latestScore was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private latestScore_Conflict As New AtomicDouble(0.0)
		Private terminate As New AtomicBoolean(False)
		Private iterCount As New AtomicInteger(0)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile org.deeplearning4j.earlystopping.termination.IterationTerminationCondition terminationReason = null;
'JAVA TO VB CONVERTER NOTE: The field terminationReason was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend terminationReason_Conflict As IterationTerminationCondition = Nothing

		Public Sub New(ByVal earlyStoppingConfiguration As EarlyStoppingConfiguration(Of T), ByVal model As T, ByVal train As DataSetIterator, ByVal trainMulti As MultiDataSetIterator, ByVal workers As Integer, ByVal prefetchBuffer As Integer, ByVal averagingFrequency As Integer)
			Me.New(earlyStoppingConfiguration, model, train, trainMulti, Nothing, workers, prefetchBuffer, averagingFrequency, True, True)
		End Sub

		Public Sub New(ByVal earlyStoppingConfiguration As EarlyStoppingConfiguration(Of T), ByVal model As T, ByVal train As DataSetIterator, ByVal trainMulti As MultiDataSetIterator, ByVal listener As EarlyStoppingListener(Of T), ByVal workers As Integer, ByVal prefetchBuffer As Integer, ByVal averagingFrequency As Integer)
			Me.New(earlyStoppingConfiguration, model, train, trainMulti, listener, workers, prefetchBuffer, averagingFrequency, True, True)
		End Sub

		Public Sub New(ByVal earlyStoppingConfiguration As EarlyStoppingConfiguration(Of T), ByVal model As T, ByVal train As DataSetIterator, ByVal trainMulti As MultiDataSetIterator, ByVal listener As EarlyStoppingListener(Of T), ByVal workers As Integer, ByVal prefetchBuffer As Integer, ByVal averagingFrequency As Integer, ByVal reportScoreAfterAveraging As Boolean, ByVal useLegacyAveraging As Boolean)
			Me.esConfig = earlyStoppingConfiguration
			Me.train = train
			Me.trainMulti = trainMulti
			Me.iterator = (If(train IsNot Nothing, train, trainMulti))
			Me.listener_Conflict = listener
			Me.model = model

			' adjust UI listeners
			Dim trainerListener As New AveragingTrainingListener(Me, Me)
			If TypeOf model Is MultiLayerNetwork Then
				Dim listeners As ICollection(Of TrainingListener) = CType(model, MultiLayerNetwork).getListeners()
				Dim newListeners As ICollection(Of TrainingListener) = New LinkedList(Of TrainingListener)(listeners)
				newListeners.Add(trainerListener)
				model.setListeners(newListeners)

			ElseIf TypeOf model Is ComputationGraph Then
				Dim listeners As ICollection(Of TrainingListener) = CType(model, ComputationGraph).getListeners()
				Dim newListeners As ICollection(Of TrainingListener) = New LinkedList(Of TrainingListener)(listeners)
				newListeners.Add(trainerListener)
				model.setListeners(newListeners)
			End If

			Me.wrapper = (New ParallelWrapper.Builder(Of )(model)).workers(workers).prefetchBuffer(prefetchBuffer).averagingFrequency(averagingFrequency).reportScoreAfterAveraging(reportScoreAfterAveraging).build()
		End Sub

		Protected Friend Overridable WriteOnly Property TerminationReason As IterationTerminationCondition
			Set(ByVal terminationReason As IterationTerminationCondition)
				Me.terminationReason_Conflict = terminationReason
			End Set
		End Property

		Public Overridable Function fit() As EarlyStoppingResult(Of T) Implements IEarlyStoppingTrainer(Of T).fit
			log.info("Starting early stopping training")
			If wrapper Is Nothing Then
				Throw New System.InvalidOperationException("Trainer has already exhausted it's parallel wrapper instance. Please instantiate a new trainer.")
			End If
			If esConfig.getScoreCalculator() Is Nothing Then
				log.warn("No score calculator provided for early stopping. Score will be reported as 0.0 to epoch termination conditions")
			End If

			'Initialize termination conditions:
			If esConfig.getIterationTerminationConditions() IsNot Nothing Then
				For Each c As IterationTerminationCondition In esConfig.getIterationTerminationConditions()
					c.initialize()
				Next c
			End If
			If esConfig.getEpochTerminationConditions() IsNot Nothing Then
				For Each c As EpochTerminationCondition In esConfig.getEpochTerminationConditions()
					c.initialize()
				Next c
			End If

			If listener_Conflict IsNot Nothing Then
				listener_Conflict.onStart(esConfig, model)
			End If

			Dim scoreVsEpoch As IDictionary(Of Integer, Double) = New LinkedHashMap(Of Integer, Double)()

			' append the iteration listener
			Dim epochCount As Integer = 0

			' iterate through epochs
			Do
				' note that we don't call train.reset() because ParallelWrapper does it already
				Try
					If train IsNot Nothing Then
						wrapper.fit(train)
					Else
						wrapper.fit(trainMulti)
					End If
				Catch e As Exception
					log.warn("Early stopping training terminated due to exception at epoch {}, iteration {}", epochCount, iterCount, e)
					'Load best model to return
					Dim bestModel As T
					Try
						bestModel = esConfig.getModelSaver().getBestModel()
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try
					Return New EarlyStoppingResult(Of T)(EarlyStoppingResult.TerminationReason.Error, e.ToString(), scoreVsEpoch, bestModelEpoch, bestModelScore, epochCount, bestModel)
				End Try

				If terminate.get() Then
					'Handle termination condition:
					log.info("Hit per iteration termination condition at epoch {}, iteration {}. Reason: {}", epochCount, iterCount, terminationReason_Conflict)

					If esConfig.isSaveLastModel() Then
						'Save last model:
						Try
							esConfig.getModelSaver().saveLatestModel(model, 0.0)
						Catch e As IOException
							Throw New Exception("Error saving most recent model", e)
						End Try
					End If

					Dim bestModel As T
					Try
						bestModel = esConfig.getModelSaver().getBestModel()
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try

					If bestModel Is Nothing Then
						'Could occur with very early termination
						bestModel = model
					End If

					Dim result As New EarlyStoppingResult(Of T)(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, terminationReason_Conflict.ToString(), scoreVsEpoch, bestModelEpoch, bestModelScore, epochCount, bestModel)
					If listener_Conflict IsNot Nothing Then
						listener_Conflict.onCompletion(result)
					End If

					' clean up
					wrapper.shutdown()
					Me.wrapper = Nothing

					Return result
				End If

				log.info("Completed training epoch {}", epochCount)


				If (epochCount = 0 AndAlso esConfig.getEvaluateEveryNEpochs() = 1) OrElse epochCount Mod esConfig.getEvaluateEveryNEpochs() = 0 Then
					'Calculate score at this epoch:
					Dim sc As ScoreCalculator = esConfig.getScoreCalculator()
					Dim score As Double = (If(sc Is Nothing, 0.0, esConfig.getScoreCalculator().calculateScore(model)))
					scoreVsEpoch(epochCount - 1) = score

					If sc IsNot Nothing AndAlso score < bestModelScore Then
						'Save best model:
						If bestModelEpoch = -1 Then
							'First calculated/reported score
							log.info("Score at epoch {}: {}", epochCount, score)
						Else
							log.info("New best model: score = {}, epoch = {} (previous: score = {}, epoch = {})", score, epochCount, bestModelScore, bestModelEpoch)
						End If
						bestModelScore = score
						bestModelEpoch = epochCount

						Try
							esConfig.getModelSaver().saveBestModel(model, score)
						Catch e As IOException
							Throw New Exception("Error saving best model", e)
						End Try
					End If

					If esConfig.isSaveLastModel() Then
						'Save last model:
						Try
							esConfig.getModelSaver().saveLatestModel(model, score)
						Catch e As IOException
							Throw New Exception("Error saving most recent model", e)
						End Try
					End If

					If listener_Conflict IsNot Nothing Then
						listener_Conflict.onEpoch(epochCount, score, esConfig, model)
					End If

					'Check per-epoch termination conditions:
					Dim epochTerminate As Boolean = False
					Dim termReason As EpochTerminationCondition = Nothing
					For Each c As EpochTerminationCondition In esConfig.getEpochTerminationConditions()
						If c.terminate(epochCount, score, esConfig.getScoreCalculator().minimizeScore()) Then
							epochTerminate = True
							termReason = c
							wrapper.stopFit()
							Exit For
						End If
					Next c
					If epochTerminate Then
						log.info("Hit epoch termination condition at epoch {}. Details: {}", epochCount, termReason.ToString())
						Dim bestModel As T
						Try
							bestModel = esConfig.getModelSaver().getBestModel()
						Catch e2 As IOException
							Throw New Exception(e2)
						End Try
						Dim result As New EarlyStoppingResult(Of T)(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, termReason.ToString(), scoreVsEpoch, bestModelEpoch, bestModelScore, epochCount + 1, bestModel)
						If listener_Conflict IsNot Nothing Then
							listener_Conflict.onCompletion(result)
						End If

						' clean up
						wrapper.shutdown()
						Me.wrapper = Nothing

						Return result
					End If
				End If
				epochCount += 1
			Loop
		End Function

		Public Overridable Function pretrain() As EarlyStoppingResult(Of T) Implements IEarlyStoppingTrainer(Of T).pretrain
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overridable WriteOnly Property LatestScore As Double
			Set(ByVal latestScore As Double)
				Me.latestScore_Conflict.set(latestScore)
			End Set
		End Property

		Public Overridable Sub incrementIteration()
			Me.iterCount.incrementAndGet()
		End Sub

		Public Overridable Property Termination As Boolean
			Set(ByVal terminate As Boolean)
				Me.terminate.set(terminate)
			End Set
			Get
				Return Me.terminate.get()
			End Get
		End Property


		''' <summary>
		''' AveragingTrainingListener is attached to the primary model within ParallelWrapper. It is invoked
		''' with each averaging step, and thus averaging is considered analogous to an iteration. </summary>
		''' @param <T> </param>
		Private Class AveragingTrainingListener(Of T As org.deeplearning4j.nn.api.Model)
			Inherits BaseTrainingListener

			Private ReadOnly outerInstance As EarlyStoppingParallelTrainer(Of T)

			Friend ReadOnly log As Logger = LoggerFactory.getLogger(GetType(AveragingTrainingListener))
			Friend terminationReason As IterationTerminationCondition = Nothing
			Friend trainer As EarlyStoppingParallelTrainer(Of T)

			''' <summary>
			''' Default constructor printing every 10 iterations </summary>
			Public Sub New(ByVal outerInstance As EarlyStoppingParallelTrainer(Of T), ByVal trainer As EarlyStoppingParallelTrainer(Of T))
				Me.outerInstance = outerInstance
				Me.trainer = trainer
			End Sub

			Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
				'Check per-iteration termination conditions
				Dim latestScore As Double = model.score()
				trainer.LatestScore = latestScore
				For Each c As IterationTerminationCondition In outerInstance.esConfig.getIterationTerminationConditions()
					If c.terminate(latestScore) Then
						trainer.Termination = True
						trainer.TerminationReason = c
						Exit For
					End If
				Next c
				If trainer.Termination Then
					' use built-in kill switch to stop fit operation
					outerInstance.wrapper.stopFit()
				End If

				trainer.incrementIteration()
			End Sub
		End Class

		Public Overridable WriteOnly Property Listener(ByVal listener As EarlyStoppingListener(Of T)) Implements IEarlyStoppingTrainer.setListener As EarlyStoppingListener(Of T)
			Set(ByVal listener As EarlyStoppingListener(Of T))
				Me.listener_Conflict = listener
			End Set
		End Property

		Protected Friend Overridable Sub reset()
			If train IsNot Nothing Then
				train.reset()
			End If
			If trainMulti IsNot Nothing Then
				trainMulti.reset()
			End If
		End Sub


	End Class

End Namespace