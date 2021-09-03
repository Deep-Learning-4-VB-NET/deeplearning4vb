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
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports org.deeplearning4j.rl4j.network
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.nstepqlearning


	Public Class NStepQLearning
		Implements IUpdateAlgorithm(Of Gradients, StateActionReward(Of Integer))

		Private ReadOnly threadCurrent As ITrainableNeuralNet
		Private ReadOnly target As IOutputNeuralNet
		Private ReadOnly gamma As Double
		Private ReadOnly algorithmHelper As NStepQLearningHelper
		Private ReadOnly featuresBuilder As FeaturesBuilder

		''' <param name="threadCurrent"> The &theta;' parameters (the thread-specific network) </param>
		''' <param name="target"> The &theta;<sup>&ndash;</sup> parameters (the global target network) </param>
		''' <param name="actionSpaceSize"> The numbers of possible actions that can be taken on the environment </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NStepQLearning(@NonNull ITrainableNeuralNet threadCurrent, @NonNull IOutputNeuralNet target, int actionSpaceSize, @NonNull Configuration configuration)
		Public Sub New(ByVal threadCurrent As ITrainableNeuralNet, ByVal target As IOutputNeuralNet, ByVal actionSpaceSize As Integer, ByVal configuration As Configuration)
			Me.threadCurrent = threadCurrent
			Me.target = target
			Me.gamma = configuration.getGamma()

			algorithmHelper = If(threadCurrent.isRecurrent(), New RecurrentNStepQLearningHelper(actionSpaceSize), New NonRecurrentNStepQLearningHelper(actionSpaceSize))

			featuresBuilder = New FeaturesBuilder(threadCurrent.isRecurrent())
		End Sub

		Public Overridable Function compute(ByVal trainingBatch As IList(Of StateActionReward(Of Integer))) As Gradients
			Dim size As Integer = trainingBatch.Count

			Dim stateActionReward As StateActionReward(Of Integer) = trainingBatch(size - 1)

			Dim features As Features = featuresBuilder.build(trainingBatch)

			Dim labels As INDArray = algorithmHelper.createLabels(size)

			Dim r As Double
			If stateActionReward.isTerminal() Then
				r = 0
			Else
				Dim expectedValuesOfLast As INDArray = algorithmHelper.getTargetExpectedQValuesOfLast(target, trainingBatch, features)
				r = Nd4j.max(expectedValuesOfLast).getDouble(0)
			End If

			For i As Integer = size - 1 To 0 Step -1
				stateActionReward = trainingBatch(i)

				r = stateActionReward.getReward() + gamma * r
				Dim expectedQValues As INDArray = threadCurrent.output(stateActionReward.getObservation()).get(CommonOutputNames.QValues)
				expectedQValues = expectedQValues.putScalar(stateActionReward.getAction(), r)

				algorithmHelper.setLabels(labels, i, expectedQValues)
			Next i

			Dim featuresLabels As New FeaturesLabels(features)
			featuresLabels.putLabels(CommonLabelNames.QValues, labels)
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