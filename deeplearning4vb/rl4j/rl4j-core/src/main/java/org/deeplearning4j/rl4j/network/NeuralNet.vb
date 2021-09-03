Imports System.IO
Imports NeuralNetwork = org.deeplearning4j.nn.api.NeuralNetwork
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
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


	Public Interface NeuralNet(Of NN As NeuralNet)
		Inherits ITrainableNeuralNet(Of NN)

		''' <summary>
		''' Returns the underlying MultiLayerNetwork or ComputationGraph objects.
		''' </summary>
		ReadOnly Property NeuralNetworks As NeuralNetwork()

		''' <summary>
		''' returns true if this is a recurrent network
		''' </summary>
		ReadOnly Property Recurrent As Boolean

		''' <summary>
		''' required for recurrent networks during init
		''' </summary>
		Sub reset()

		''' <param name="batch"> batch to evaluate </param>
		''' <returns> evaluation by the model of the input by all outputs </returns>
		Function outputAll(ByVal batch As INDArray) As INDArray()

		''' <summary>
		''' Calculate the gradients from input and label (target) of all outputs </summary>
		''' <param name="input"> input batch </param>
		''' <param name="labels"> target batch </param>
		''' <returns> the gradients </returns>
		Function gradient(ByVal input As INDArray, ByVal labels() As INDArray) As Gradient()

		''' <summary>
		''' fit from input and labels </summary>
		''' <param name="input"> input batch </param>
		''' <param name="labels"> target batch </param>
		Sub fit(ByVal input As INDArray, ByVal labels() As INDArray)

		''' <summary>
		''' update the params from the gradients and the batchSize </summary>
		''' <param name="gradients"> gradients to apply the gradient from </param>
		''' <param name="batchSize"> batchSize from which the gradient was calculated on (similar to nstep) </param>
		Sub applyGradient(ByVal gradients() As Gradient, ByVal batchSize As Integer)


		''' <summary>
		''' latest score from lastest fit </summary>
		''' <returns> latest score </returns>
		ReadOnly Property LatestScore As Double

		''' <summary>
		''' save the neural net into an OutputStream </summary>
		''' <param name="os"> OutputStream to save in </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void save(java.io.OutputStream os) throws java.io.IOException;
		Sub save(ByVal os As Stream)

		''' <summary>
		''' save the neural net into a filename </summary>
		''' <param name="filename"> filename to save in </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void save(String filename) throws java.io.IOException;
		Sub save(ByVal filename As String)

	End Interface

End Namespace