Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.network

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
Namespace org.deeplearning4j.rl4j.builder

	Public Class AsyncNetworkHandler
		Implements INetworksHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter final org.deeplearning4j.rl4j.network.ITrainableNeuralNet targetNetwork;
		Friend ReadOnly targetNetwork As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter ITrainableNeuralNet threadCurrentNetwork;
		Friend threadCurrentNetwork As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter final org.deeplearning4j.rl4j.network.ITrainableNeuralNet globalCurrentNetwork;
		Friend ReadOnly globalCurrentNetwork As ITrainableNeuralNet

		Public Sub New(ByVal network As ITrainableNeuralNet)
			globalCurrentNetwork = network
			targetNetwork = network.clone()
		End Sub

		Public Overridable Sub resetForNewBuild() Implements INetworksHandler.resetForNewBuild
			threadCurrentNetwork = globalCurrentNetwork.clone()
		End Sub
	End Class

End Namespace