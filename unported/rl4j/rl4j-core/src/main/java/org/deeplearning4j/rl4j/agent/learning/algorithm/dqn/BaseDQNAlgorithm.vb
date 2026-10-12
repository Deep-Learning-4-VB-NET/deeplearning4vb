Imports NonNull = lombok.NonNull
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
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

	''' <summary>
	''' The base of all DQN based algorithms
	''' 
	''' @author Alexandre Boulanger
	''' 
	''' </summary>
	Public MustInherit Class BaseDQNAlgorithm
		Inherits BaseTransitionTDAlgorithm

		Private ReadOnly targetQNetwork As IOutputNeuralNet

		''' <summary>
		''' In literature, this corresponds to Q<sub>net</sub>(s(t+1), a)
		''' </summary>
		Protected Friend qNetworkNextFeatures As INDArray

		''' <summary>
		''' In literature, this corresponds to Q<sub>tnet</sub>(s(t+1), a)
		''' </summary>
		Protected Friend targetQNetworkNextFeatures As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseDQNAlgorithm(org.deeplearning4j.rl4j.network.IOutputNeuralNet qNetwork, @NonNull IOutputNeuralNet targetQNetwork, BaseTransitionTDAlgorithm.Configuration configuration)
		Protected Friend Sub New(ByVal qNetwork As IOutputNeuralNet, ByVal targetQNetwork As IOutputNeuralNet, ByVal configuration As BaseTransitionTDAlgorithm.Configuration)
			MyBase.New(qNetwork, configuration)
			Me.targetQNetwork = targetQNetwork
		End Sub

		Protected Friend Overrides Sub initComputation(ByVal features As Features, ByVal nextFeatures As Features)
			MyBase.initComputation(features, nextFeatures)

			qNetworkNextFeatures = qNetwork.output(nextFeatures).get(CommonOutputNames.QValues)
			targetQNetworkNextFeatures = targetQNetwork.output(nextFeatures).get(CommonOutputNames.QValues)
		End Sub
	End Class

End Namespace