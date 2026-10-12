Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
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

	Public Interface IOutputNeuralNet
		''' <summary>
		''' Compute the output for the supplied observation. Multiple calls to output() with the same observation will
		''' give the same output, even if the internal state has changed, until the network is reset or an operation
		''' that modifies it is performed (See <seealso cref="ITrainableNeuralNet.fit"/>, <seealso cref="ITrainableNeuralNet.applyGradients"/>,
		''' and <seealso cref="ITrainableNeuralNet.copyFrom"/>). </summary>
		''' <param name="observation"> An <seealso cref="Observation"/> </param>
		''' <returns> The ouptut of the network </returns>
		Function output(ByVal observation As Observation) As NeuralNetOutput

		''' <summary>
		''' Compute the output for the supplied batch. </summary>
		''' <param name="batch"> </param>
		''' <returns> The ouptut of the network </returns>
		Function output(ByVal batch As INDArray) As NeuralNetOutput ' FIXME: Remove once legacy classes are gone

		''' <summary>
		''' Compute the output for the supplied batch. </summary>
		''' <param name="features"> A <seealso cref="Features"/> instance </param>
		''' <returns> The ouptut of the network </returns>
		Function output(ByVal features As Features) As NeuralNetOutput

		''' <summary>
		''' Clear the neural net of any previous state
		''' </summary>
		Sub reset()

		''' <returns> True if the neural net is a RNN </returns>
		ReadOnly Property Recurrent As Boolean
	End Interface
End Namespace