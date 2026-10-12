Imports System.Collections.Generic
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports org.deeplearning4j.rl4j.experience
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
Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.nstepqlearning


	Public MustInherit Class NStepQLearningHelper

		''' <summary>
		''' Get the expected Q value given a training batch index from the pre-computed Q values </summary>
		''' <param name="allExpectedQValues"> A INDArray containg all pre-computed Q values </param>
		''' <param name="idx"> The training batch index </param>
		''' <returns> The expected Q value </returns>
		Public MustOverride Function getExpectedQValues(ByVal allExpectedQValues As INDArray, ByVal idx As Integer) As INDArray

		''' <summary>
		''' Create an empty INDArray to be used as the Q values array </summary>
		''' <param name="trainingBatchSize"> the size of the training batch </param>
		''' <returns> An empty Q values array </returns>
		Public MustOverride Function createLabels(ByVal trainingBatchSize As Integer) As INDArray

		''' <summary>
		''' Set the label in the Q values array for a given training batch index </summary>
		''' <param name="labels"> The Q values array </param>
		''' <param name="idx"> The training batch index </param>
		''' <param name="data"> The updated Q values to set </param>
		Public MustOverride Sub setLabels(ByVal labels As INDArray, ByVal idx As Long, ByVal data As INDArray)

		''' <summary>
		''' Get the expected Q values for the last element of the training batch, estimated using the target network. </summary>
		''' <param name="target"> The target network </param>
		''' <param name="trainingBatch"> An experience training batch </param>
		''' <returns> A INDArray filled with the observations from the trainingBatch </returns>
		''' <returns> The expected Q values for the last element of the training batch </returns>
		Public MustOverride Function getTargetExpectedQValuesOfLast(ByVal target As IOutputNeuralNet, ByVal trainingBatch As IList(Of StateActionReward(Of Integer)), ByVal features As Features) As INDArray
	End Class
End Namespace