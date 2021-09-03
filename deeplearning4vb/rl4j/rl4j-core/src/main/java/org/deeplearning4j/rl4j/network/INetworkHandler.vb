Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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
Namespace org.deeplearning4j.rl4j.network

	Public Interface INetworkHandler
		''' <returns> true if the network is recurrent </returns>
		ReadOnly Property Recurrent As Boolean

		''' <summary>
		''' Will notify the network that a gradient calculation has been performed.
		''' </summary>
		Sub notifyGradientCalculation()

		''' <summary>
		''' Will notify the network that a gradient has been applied
		''' </summary>
		Sub notifyIterationDone()

		''' <summary>
		''' Perform a fit on the network. </summary>
		''' <param name="featuresLabels"> The features-labels </param>
		Sub performFit(ByVal featuresLabels As FeaturesLabels)

		''' <summary>
		''' Compute the gradients from the features-labels </summary>
		''' <param name="featuresLabels"> The features-labels </param>
		Sub performGradientsComputation(ByVal featuresLabels As FeaturesLabels)

		''' <summary>
		''' Fill the supplied gradients with the results of the last gradients computation </summary>
		''' <param name="gradients"> The <seealso cref="Gradients"/> to fill </param>
		Sub fillGradientsResponse(ByVal gradients As Gradients)

		''' <summary>
		''' Will apply the gradients to the network </summary>
		''' <param name="gradients"> The <seealso cref="Gradients"/> to apply </param>
		''' <param name="batchSize"> The batch size </param>
		Sub applyGradient(ByVal gradients As Gradients, ByVal batchSize As Long)

		''' <param name="observation"> An <seealso cref="Observation"/> </param>
		''' <returns> The output of the observation computed with the current network state. (i.e. not cached) </returns>
		Function recurrentStepOutput(ByVal observation As Observation) As INDArray()

		''' <param name="observation"> An <seealso cref="Observation"/> </param>
		''' <returns> The output of the observation computed without using or updating the network state. </returns>
		Function stepOutput(ByVal observation As Observation) As INDArray()

		''' <summary>
		''' Compute the output of a batch </summary>
		''' <param name="features"> A <seealso cref="Features"/> instance </param>
		''' <returns> The output of the batch. The current state of the network is not used or changed. </returns>
		Function batchOutput(ByVal features As Features) As INDArray()

		''' <summary>
		''' Clear all network state.
		''' </summary>
		Sub resetState()

		''' <returns> An identical copy of the current instance. </returns>
		Function clone() As INetworkHandler

		''' <summary>
		''' Copies the parameter of another network to the instance. </summary>
		''' <param name="from"> </param>
		Sub copyFrom(ByVal from As INetworkHandler)
	End Interface

End Namespace