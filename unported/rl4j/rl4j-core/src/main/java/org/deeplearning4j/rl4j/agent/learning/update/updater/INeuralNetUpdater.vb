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
Namespace org.deeplearning4j.rl4j.agent.learning.update.updater

	''' <summary>
	''' The role of INeuralNetUpdater implementations is to update a <seealso cref="org.deeplearning4j.rl4j.network.NeuralNet NeuralNet"/>.<p /> </summary>
	''' @param <DATA_TYPE> The type of the data needed to to update the netwok. See <seealso cref="org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels FeaturesLabels"/>
	'''                   and <seealso cref="org.deeplearning4j.rl4j.agent.learning.update.Gradients Gradients"/>. </param>
	Public Interface INeuralNetUpdater(Of DATA_TYPE)
		''' <summary>
		''' Update a <seealso cref="org.deeplearning4j.rl4j.network.NeuralNet NeuralNet"/>. </summary>
		''' <param name="dataType"> </param>
		Sub update(ByVal dataType As DATA_TYPE)

		''' <summary>
		''' Make sure the thread local current netwrok is synchronized with the global current (in the async case)
		''' </summary>
		Sub synchronizeCurrent()
	End Interface

End Namespace