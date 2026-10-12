Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports org.deeplearning4j.earlystopping.scorecalc
Imports EpochTerminationCondition = org.deeplearning4j.earlystopping.termination.EpochTerminationCondition
Imports IterationTerminationCondition = org.deeplearning4j.earlystopping.termination.IterationTerminationCondition
Imports org.deeplearning4j.earlystopping.trainer
Imports Model = org.deeplearning4j.nn.api.Model
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.spark.earlystopping


	Public MustInherit Class BaseSparkEarlyStoppingTrainer(Of T As org.deeplearning4j.nn.api.Model)
		Implements IEarlyStoppingTrainer(Of T)

		Public MustOverride Function pretrain() As EarlyStoppingResult(Of T) Implements IEarlyStoppingTrainer(Of T).pretrain

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(BaseSparkEarlyStoppingTrainer))

		Private sc As JavaSparkContext
		Private ReadOnly esConfig As EarlyStoppingConfiguration(Of T)
		Private net As T
		Private ReadOnly train As JavaRDD(Of DataSet)
		Private ReadOnly trainMulti As JavaRDD(Of MultiDataSet)
'JAVA TO VB CONVERTER NOTE: The field listener was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listener_Conflict As EarlyStoppingListener(Of T)

		Private bestModelScore As Double = Double.MaxValue
		Private bestModelEpoch As Integer = -1

		Protected Friend Sub New(ByVal sc As JavaSparkContext, ByVal esConfig As EarlyStoppingConfiguration(Of T), ByVal net As T, ByVal train As JavaRDD(Of DataSet), ByVal trainMulti As JavaRDD(Of MultiDataSet), ByVal listener As EarlyStoppingListener(Of T))
			If (esConfig.getEpochTerminationConditions() Is Nothing OrElse esConfig.getEpochTerminationConditions().isEmpty()) AndAlso (esConfig.getIterationTerminationConditions() Is Nothing OrElse esConfig.getIterationTerminationConditions().isEmpty()) Then
				Throw New System.ArgumentException("Cannot conduct early stopping without a termination condition (both Iteration " & "and Epoch termination conditions are null/empty)")
			End If

			Me.sc = sc
			Me.esConfig = esConfig
			Me.net = net
			Me.train = train
			Me.trainMulti = trainMulti
			Me.listener_Conflict = listener
		End Sub

		Protected Friend MustOverride Sub fit(ByVal data As JavaRDD(Of DataSet))

		Protected Friend MustOverride Sub fitMulti(ByVal data As JavaRDD(Of MultiDataSet))

		Protected Friend MustOverride ReadOnly Property Score As Double

		Public Overridable Function fit() As EarlyStoppingResult(Of T) Implements IEarlyStoppingTrainer(Of T).fit
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
				listener_Conflict.onStart(esConfig, net)
			End If

			Dim scoreVsEpoch As IDictionary(Of Integer, Double) = New LinkedHashMap(Of Integer, Double)()

			Dim epochCount As Integer = 0
			Do 'Iterate (do epochs) until termination condition hit
				Dim lastScore As Double
				Dim terminate As Boolean = False
				Dim terminationReason As IterationTerminationCondition = Nothing

				If train IsNot Nothing Then
					fit(train)
				Else
					fitMulti(trainMulti)
				End If

				'TODO revisit per iteration termination conditions, ensuring they are evaluated *per averaging* not per epoch
				'Check per-iteration termination conditions
				lastScore = Score
				For Each c As IterationTerminationCondition In esConfig.getIterationTerminationConditions()
					If c.terminate(lastScore) Then
						terminate = True
						terminationReason = c
						Exit For
					End If
				Next c

				If terminate Then
					'Handle termination condition:
					log.info("Hit per iteration epoch termination condition at epoch {}, iteration {}. Reason: {}", epochCount, epochCount, terminationReason)

					If esConfig.isSaveLastModel() Then
						'Save last model:
						Try
							esConfig.getModelSaver().saveLatestModel(net, 0.0)
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
					Dim score As Double = (If(sc Is Nothing, 0.0, esConfig.getScoreCalculator().calculateScore(net)))
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
							esConfig.getModelSaver().saveBestModel(net, score)
						Catch e As IOException
							Throw New Exception("Error saving best model", e)
						End Try
					End If

					If esConfig.isSaveLastModel() Then
						'Save last model:
						Try
							esConfig.getModelSaver().saveLatestModel(net, score)
						Catch e As IOException
							Throw New Exception("Error saving most recent model", e)
						End Try
					End If

					If listener_Conflict IsNot Nothing Then
						listener_Conflict.onEpoch(epochCount, score, esConfig, net)
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
							Throw New Exception(e2)
						End Try
						Dim result As New EarlyStoppingResult(Of T)(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, termReason.ToString(), scoreVsEpoch, bestModelEpoch, bestModelScore, epochCount + 1, bestModel)
						If listener_Conflict IsNot Nothing Then
							listener_Conflict.onCompletion(result)
						End If
						Return result
					End If

					epochCount += 1
				End If
			Loop
		End Function

		Public Overridable WriteOnly Property Listener(ByVal listener As EarlyStoppingListener(Of T)) Implements IEarlyStoppingTrainer.setListener As EarlyStoppingListener(Of T)
			Set(ByVal listener As EarlyStoppingListener(Of T))
				Me.listener_Conflict = listener
			End Set
		End Property
	End Class

End Namespace