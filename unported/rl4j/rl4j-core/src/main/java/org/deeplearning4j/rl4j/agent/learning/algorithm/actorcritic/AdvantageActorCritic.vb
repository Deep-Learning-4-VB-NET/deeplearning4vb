Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesBuilder = org.deeplearning4j.rl4j.agent.learning.update.FeaturesBuilder
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports org.deeplearning4j.rl4j.experience
Imports CommonLabelNames = org.deeplearning4j.rl4j.network.CommonLabelNames
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports org.deeplearning4j.rl4j.network
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
Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.actorcritic


	Public Class AdvantageActorCritic
		Implements IUpdateAlgorithm(Of Gradients, StateActionReward(Of Integer))

		Private ReadOnly threadCurrent As ITrainableNeuralNet

		Private ReadOnly gamma As Double

		Private ReadOnly algorithmHelper As ActorCriticHelper

		Private ReadOnly featuresBuilder As FeaturesBuilder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdvantageActorCritic(@NonNull ITrainableNeuralNet threadCurrent, int actionSpaceSize, @NonNull Configuration configuration)
		Public Sub New(ByVal threadCurrent As ITrainableNeuralNet, ByVal actionSpaceSize As Integer, ByVal configuration As Configuration)
			Me.threadCurrent = threadCurrent
			gamma = configuration.getGamma()

			algorithmHelper = If(threadCurrent.isRecurrent(), New RecurrentActorCriticHelper(actionSpaceSize), New NonRecurrentActorCriticHelper(actionSpaceSize))

			featuresBuilder = New FeaturesBuilder(threadCurrent.isRecurrent())
		End Sub

		Public Overridable Function compute(ByVal trainingBatch As IList(Of StateActionReward(Of Integer))) As Gradients
			Dim size As Integer = trainingBatch.Count

			Dim features As Features = featuresBuilder.build(trainingBatch)
			Dim values As INDArray = algorithmHelper.createValueLabels(size)
			Dim policy As INDArray = algorithmHelper.createPolicyLabels(size)

			Dim stateActionReward As StateActionReward(Of Integer) = trainingBatch(size - 1)
			Dim value As Double
			If stateActionReward.isTerminal() Then
				value = 0
			Else
				value = threadCurrent.output(trainingBatch(size - 1).getObservation()).get(CommonOutputNames.ActorCritic.Value).getDouble(0)
			End If

			For i As Integer = size - 1 To 0 Step -1
				stateActionReward = trainingBatch(i)

				value = stateActionReward.getReward() + gamma * value

				'the critic
				values.putScalar(i, value)

				'the actor
				Dim expectedV As Double = threadCurrent.output(trainingBatch(i).getObservation()).get(CommonOutputNames.ActorCritic.Value).getDouble(0)
				Dim advantage As Double = value - expectedV
				algorithmHelper.setPolicy(policy, i, stateActionReward.getAction(), advantage)
			Next i

			Dim featuresLabels As New FeaturesLabels(features)
			featuresLabels.putLabels(CommonLabelNames.ActorCritic.Value, values)
			featuresLabels.putLabels(CommonLabelNames.ActorCritic.Policy, policy)

			Return threadCurrent.computeGradients(featuresLabels)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration
		Public Class Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default double gamma = 0.99;
			Friend gamma As Double = 0.99
		End Class
	End Class

End Namespace