Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesBuilder = org.deeplearning4j.rl4j.agent.learning.update.FeaturesBuilder
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports org.deeplearning4j.rl4j.experience
Imports CommonLabelNames = org.deeplearning4j.rl4j.network.CommonLabelNames
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.dqn


	Public MustInherit Class BaseTransitionTDAlgorithm
		Implements IUpdateAlgorithm(Of FeaturesLabels, StateActionRewardState(Of Integer))

		Public MustOverride Function compute(ByVal trainingBatch As IList(Of EXPERIENCE_TYPE)) As RESULT_TYPE

		Protected Friend ReadOnly qNetwork As IOutputNeuralNet
		Protected Friend ReadOnly gamma As Double

		Private ReadOnly errorClamp As Double
		Private ReadOnly isClamped As Boolean

		Private ReadOnly featuresBuilder As FeaturesBuilder
		''' 
		''' <param name="qNetwork"> The Q-Network </param>
		''' <param name="configuration"> The <seealso cref="Configuration"/> to use </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseTransitionTDAlgorithm(@NonNull IOutputNeuralNet qNetwork, @NonNull Configuration configuration)
		Protected Friend Sub New(ByVal qNetwork As IOutputNeuralNet, ByVal configuration As Configuration)
			Me.qNetwork = qNetwork
			Me.gamma = configuration.getGamma()

			Me.errorClamp = configuration.getErrorClamp()
			isClamped = Not Double.IsNaN(errorClamp)

			featuresBuilder = New FeaturesBuilder(qNetwork.isRecurrent())
		End Sub

		''' <summary>
		''' Called just before the calculation starts </summary>
		''' <param name="features"> A <seealso cref="Features"/> instance of all observations in the batch </param>
		''' <param name="nextFeatures"> A <seealso cref="Features"/> instance of all next observations in the batch </param>
		Protected Friend Overridable Sub initComputation(ByVal features As Features, ByVal nextFeatures As Features)
			' Do nothing
		End Sub

		''' <summary>
		''' Compute the new estimated Q-Value for every transition in the batch
		''' </summary>
		''' <param name="batchIdx"> The index in the batch of the current transition </param>
		''' <param name="reward"> The reward of the current transition </param>
		''' <param name="isTerminal"> True if it's the last transition of the "game" </param>
		''' <returns> The estimated Q-Value </returns>
		Protected Friend MustOverride Function computeTarget(ByVal batchIdx As Integer, ByVal reward As Double, ByVal isTerminal As Boolean) As Double

		Public Overridable Function compute(ByVal stateActionRewardStates As IList(Of StateActionRewardState(Of Integer))) As FeaturesLabels

			Dim size As Integer = stateActionRewardStates.Count

			Dim features As Features = featuresBuilder.build(stateActionRewardStates)
			Dim nextFeatures As Features = featuresBuilder.build(stateActionRewardStates.Select(Function(e) e.getNextObservation()), stateActionRewardStates.Count)

			initComputation(features, nextFeatures)

			Dim updatedQValues As INDArray = qNetwork.output(features).get(CommonOutputNames.QValues)
			For i As Integer = 0 To size - 1
				Dim stateActionRewardState As StateActionRewardState(Of Integer) = stateActionRewardStates(i)
				Dim yTarget As Double = computeTarget(i, stateActionRewardState.getReward(), stateActionRewardState.isTerminal())

				If isClamped Then
					Dim previousQValue As Double = updatedQValues.getDouble(i, stateActionRewardState.getAction())
					Dim lowBound As Double = previousQValue - errorClamp
					Dim highBound As Double = previousQValue + errorClamp
					yTarget = Math.Min(highBound, Math.Max(yTarget, lowBound))
				End If
				updatedQValues.putScalar(i, stateActionRewardState.getAction(), yTarget)
			Next i

			Dim featuresLabels As New FeaturesLabels(features)
			featuresLabels.putLabels(CommonLabelNames.QValues, updatedQValues)

			Return featuresLabels
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration
		Public Class Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default double gamma = 0.99;
			Friend gamma As Double = 0.99

			''' <summary>
			''' Will prevent the new Q-Value from being farther than <i>errorClamp</i> away from the previous value. Double.NaN will disable the clamping (default).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default double errorClamp = Double.NaN;
			Friend errorClamp As Double = Double.NaN
		End Class
	End Class

End Namespace