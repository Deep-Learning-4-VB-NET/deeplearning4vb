Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients

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

	Public Interface ITrainableNeuralNet(Of NET_TYPE As ITrainableNeuralNet)
		Inherits IOutputNeuralNet

		''' <summary>
		''' Train the neural net using the supplied <i>feature-labels</i> </summary>
		''' <param name="featuresLabels"> The feature-labels </param>
		Sub fit(ByVal featuresLabels As FeaturesLabels)

		''' <summary>
		''' Use the supplied <i>feature-labels</i> to compute the <seealso cref="Gradients"/> on the neural network. </summary>
		''' <param name="featuresLabels"> The feature-labels </param>
		''' <returns> The computed <seealso cref="Gradients"/> </returns>
		Function computeGradients(ByVal featuresLabels As FeaturesLabels) As Gradients

		''' <summary>
		''' Applies a <seealso cref="Gradients"/> to the network </summary>
		''' <param name="gradients"> </param>
		Sub applyGradients(ByVal gradients As Gradients)

		''' <summary>
		''' Changes this instance to be a copy of the <i>from</i> network. </summary>
		''' <param name="from"> The network that will be the source of the copy. </param>
		Sub copyFrom(ByVal from As NET_TYPE)

		''' <summary>
		''' Creates a clone of the network instance.
		''' </summary>
		Function clone() As NET_TYPE
	End Interface

End Namespace