Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
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
Namespace org.deeplearning4j.rl4j.agent.learning.update.updater.sync

	Public Class SyncLabelsNeuralNetUpdater
		Inherits BaseSyncNeuralNetUpdater(Of FeaturesLabels)

		Public Sub New(ByVal current As ITrainableNeuralNet, ByVal target As ITrainableNeuralNet, ByVal configuration As NeuralNetUpdaterConfiguration)
			MyBase.New(current, target, configuration)
		End Sub

		''' <summary>
		''' Update the current network
		''' </summary>
		''' <param name="featuresLabels"> A <seealso cref="FeaturesLabels"/> that will be used to update the network. </param>
		Public Overridable Overloads Sub update(ByVal featuresLabels As FeaturesLabels)
			current.fit(featuresLabels)
			syncTargetNetwork()
		End Sub
	End Class
End Namespace