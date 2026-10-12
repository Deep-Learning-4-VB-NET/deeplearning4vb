Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports org.deeplearning4j.earlystopping.scorecalc
Imports EpochTerminationCondition = org.deeplearning4j.earlystopping.termination.EpochTerminationCondition
Imports IterationTerminationCondition = org.deeplearning4j.earlystopping.termination.IterationTerminationCondition
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports AsyncMultiDataSetIterator = org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.earlystopping.trainer




	Public MustInherit Class BaseEarlyStoppingTrainer(Of T As org.deeplearning4j.nn.api.Model)
		Implements IEarlyStoppingTrainer(Of T)

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(BaseEarlyStoppingTrainer))

		Protected Friend model As T

		Protected Friend ReadOnly esConfig As EarlyStoppingConfiguration(Of T)
		Private ReadOnly train As DataSetIterator
		Private ReadOnly trainMulti As MultiDataSetIterator
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private final java.util.Iterator<?> iterator;
		Private ReadOnly iterator As IEnumerator(Of Object)
'JAVA TO VB CONVERTER NOTE: The field listener was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listener_Conflict As EarlyStoppingListener(Of T)

		Private bestModelScore As Double = Double.MaxValue
		Private bestModelEpoch As Integer = -1

		Protected Friend Sub New(ByVal earlyStoppingConfiguration As EarlyStoppingConfiguration(Of T), ByVal model As T, ByVal train As DataSetIterator, ByVal trainMulti As MultiDataSetIterator, ByVal listener As EarlyStoppingListener(Of T))
			If train IsNot Nothing AndAlso train.asyncSupported() Then
				train = New AsyncDataSetIterator(train)
			End If
			If trainMulti IsNot Nothing AndAlso trainMulti.asyncSupported() Then
				trainMulti = New AsyncMultiDataSetIterator(trainMulti)
			End If

			Me.esConfig = earlyStoppingConfiguration
			Me.model = model
			Me.train = train
			Me.trainMulti = trainMulti
			Me.iterator = (If(train IsNot Nothing, train, trainMulti))
			Me.listener_Conflict = listener
		End Sub

		Protected Friend MustOverride Sub fit(ByVal ds As DataSet)

		Protected Friend MustOverride Sub fit(ByVal mds As MultiDataSet)

		Protected Friend MustOverride Sub pretrain(ByVal ds As DataSet)

		Protected Friend MustOverride Sub pretrain(ByVal mds As MultiDataSet)

		Public Overridable Function fit() As EarlyStoppingResult(Of T) Implements IEarlyStoppingTrainer(Of T).fit
			Return fit(False)
		End Function

		Public Overridable Function pretrain() As EarlyStoppingResult(Of T) Implements IEarlyStoppingTrainer(Of T).pretrain
			Return fit(True)
		End Function

		Protected Friend Overridable Function fit(ByVal pretrain As Boolean) As EarlyStoppingResult(Of T)
			esConfig.validate()
			log.info("Starting early stopping training")
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

			Preconditions.checkNotNull(esConfig.getScoreCalculator(), "Score calculator cannot be null")
			If esConfig.getScoreCalculator().minimizeScore() Then
				bestModelScore = Double.MaxValue
			Else
				bestModelScore = -Double.MaxValue
			End If

			Dim epochCount As Integer = 0
			Do
				reset()
				Dim lastScore As Double
				Dim terminate As Boolean = False
				Dim terminationReason As IterationTerminationCondition = Nothing
				Dim iterCount As Integer = 0
				triggerEpochListeners(True, model, epochCount)
				Do While iterator.MoveNext()
					Try
						If pretrain Then
							If train IsNot Nothing Then
								Me.pretrain(CType(iterator.Current, DataSet))
							Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								pretrain(trainMulti.next())
							End If
						Else
							If train IsNot Nothing Then
								fit(CType(iterator.Current, DataSet))
							Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								fit(trainMulti.next())
							End If
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

					'Check per-iteration termination conditions
					If pretrain Then
						'TODO support for non-first-layer pretraining
						If TypeOf model Is MultiLayerNetwork Then
							lastScore = (DirectCast(model, MultiLayerNetwork).getLayer(0)).score()
						Else
							lastScore = (DirectCast(model, ComputationGraph).getLayer(0)).score()
						End If
					Else
						lastScore = model.score()
					End If
					For Each c As IterationTerminationCondition In esConfig.getIterationTerminationConditions()
						If c.terminate(lastScore) Then
							terminate = True
							terminationReason = c
							Exit For
						End If
					Next c
					If terminate Then
						Exit Do
					End If

					iterCount += 1
				Loop

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not iterator.hasNext() Then
					'End of epoch (if iterator does have next - means terminated)
					triggerEpochListeners(False, model, epochCount)
				End If

				If terminate Then
					'Handle termination condition:
					log.info("Hit per iteration epoch termination condition at epoch {}, iteration {}. Reason: {}", epochCount, iterCount, terminationReason)

					If esConfig.isSaveLastModel() Then
						'Save last model:
						Try
							esConfig.getModelSaver().saveLatestModel(model, 0.0)
						Catch e As IOException
							'best model not saved, let's just use default
							If TypeOf e Is FileNotFoundException Then

							Else
								Throw New Exception("Error saving most recent model", e)
							End If
						End Try
					End If

					Dim bestModel As T
					Try
						bestModel = esConfig.getModelSaver().getBestModel()
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try


					Dim result As New EarlyStoppingResult(Of T)(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, terminationReason.ToString(), scoreVsEpoch, bestModelEpoch, bestModelScore, epochCount, bestModel)
					If listener_Conflict IsNot Nothing Then
						listener_Conflict.onCompletion(result)
					End If
					Return result
				End If

				log.info("Completed training epoch {}", epochCount)


				If (epochCount = 0 AndAlso esConfig.getEvaluateEveryNEpochs() = 1) OrElse epochCount Mod esConfig.getEvaluateEveryNEpochs() = 0 Then
					'Calculate score at this epoch:
					Dim sc As ScoreCalculator = esConfig.getScoreCalculator()
					Dim score As Double = esConfig.getScoreCalculator().calculateScore(model)
					scoreVsEpoch(epochCount) = score

					Dim invalidScore As Boolean = Double.IsNaN(score) OrElse Double.IsInfinity(score)
					If invalidScore Then
						log.warn("Score is not finite for epoch {}: score = {}", epochCount, score)
					End If

					If (sc.minimizeScore() AndAlso score < bestModelScore) OrElse (Not sc.minimizeScore() AndAlso score > bestModelScore) OrElse (bestModelEpoch = -1 AndAlso invalidScore) Then
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
					Else
						log.info("Score at epoch {}: {}", epochCount, score)
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
							Exit For
						End If
					Next c
					If epochTerminate Then
						log.info("Hit epoch termination condition at epoch {}. Details: {}", epochCount, termReason.ToString())
						Dim bestModel As T
						Try
							bestModel = esConfig.getModelSaver().getBestModel()
						Catch e2 As IOException
							'Best model does not exist. Just save the current model
							If esConfig.isSaveLastModel() Then
								Try
									esConfig.getModelSaver().saveBestModel(model,0.0)
									bestModel = model
								Catch e As IOException
									log.error("Unable to save model.",e)
									Throw New Exception(e)
								End Try
							Else
								log.error("Error with earlystopping",e2)
								Throw New Exception(e2)
							End If

						End Try


						Dim result As New EarlyStoppingResult(Of T)(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, termReason.ToString(), scoreVsEpoch, bestModelEpoch, bestModelScore, epochCount + 1, bestModel)
						If listener_Conflict IsNot Nothing Then
							listener_Conflict.onCompletion(result)
						End If

						Return result
					End If
				End If
				epochCount += 1

			Loop
		End Function

		Public Overridable WriteOnly Property Listener(ByVal listener As EarlyStoppingListener(Of T)) Implements IEarlyStoppingTrainer.setListener As EarlyStoppingListener(Of T)
			Set(ByVal listener As EarlyStoppingListener(Of T))
				Me.listener_Conflict = listener
			End Set
		End Property

		'Trigger epoch listener methods manually - these won't be triggered due to not calling fit(DataSetIterator) etc
		Protected Friend Overridable Sub triggerEpochListeners(ByVal epochStart As Boolean, ByVal model As Model, ByVal epochNum As Integer)
			Dim listeners As ICollection(Of TrainingListener)
			If TypeOf model Is MultiLayerNetwork Then
				Dim n As MultiLayerNetwork = (DirectCast(model, MultiLayerNetwork))
				listeners = n.getListeners()
				n.EpochCount = epochNum
			ElseIf TypeOf model Is ComputationGraph Then
				Dim cg As ComputationGraph = (DirectCast(model, ComputationGraph))
				listeners = cg.getListeners()
				cg.Configuration.setEpochCount(epochNum)
			Else
				Return
			End If

			If listeners IsNot Nothing AndAlso listeners.Count > 0 Then
				For Each l As TrainingListener In listeners
					If epochStart Then
						l.onEpochStart(model)
					Else
						l.onEpochEnd(model)
					End If
				Next l
			End If
		End Sub

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