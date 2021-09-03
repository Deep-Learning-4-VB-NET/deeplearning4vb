Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
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

Namespace org.deeplearning4j.rl4j.learning.async

	Public Interface IAsyncGlobal(Of NN As org.deeplearning4j.rl4j.network.ITrainableNeuralNet)

		ReadOnly Property TrainingComplete As Boolean

		''' <summary>
		''' The number of updates that have been applied by worker threads.
		''' </summary>
		ReadOnly Property WorkerUpdateCount As Integer

		''' <summary>
		''' The total number of environment steps that have been processed.
		''' </summary>
		ReadOnly Property StepCount As Integer

		''' <summary>
		''' A copy of the global network that is updated after a certain number of worker episodes.
		''' </summary>
		ReadOnly Property Target As NN

		''' <summary>
		''' Apply gradients to the global network </summary>
		''' <param name="gradient"> </param>
		''' <param name="batchSize"> </param>
		Sub applyGradient(ByVal gradient() As Gradient, ByVal batchSize As Integer)

	End Interface

End Namespace