Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
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
Namespace org.deeplearning4j.rl4j.agent.learning.update.updater.async

	Public Class AsyncGradientsNeuralNetUpdater
		Inherits BaseAsyncNeuralNetUpdater(Of Gradients)

		''' <param name="threadCurrent"> The thread-current network </param>
		''' <param name="sharedNetworksUpdateHandler"> An instance shared among all threads that updates the shared networks </param>
		Public Sub New(ByVal threadCurrent As ITrainableNeuralNet, ByVal sharedNetworksUpdateHandler As AsyncSharedNetworksUpdateHandler)
			MyBase.New(threadCurrent, sharedNetworksUpdateHandler)
		End Sub

		''' <summary>
		''' Perform the necessary updates to the networks. </summary>
		''' <param name="gradients"> A <seealso cref="Gradients"/> that will be used to update the network. </param>
		Public Overridable Overloads Sub update(ByVal gradients As Gradients)
			updateAndSync(gradients)
		End Sub
	End Class

End Namespace