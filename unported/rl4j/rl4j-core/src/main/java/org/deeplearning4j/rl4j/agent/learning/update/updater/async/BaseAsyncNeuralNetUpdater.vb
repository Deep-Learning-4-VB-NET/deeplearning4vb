Imports NonNull = lombok.NonNull
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

	Public MustInherit Class BaseAsyncNeuralNetUpdater(Of DATA_TYPE)
		Implements INeuralNetUpdater(Of DATA_TYPE)

		Protected Friend ReadOnly threadCurrent As ITrainableNeuralNet
		Private ReadOnly sharedNetworksUpdateHandler As AsyncSharedNetworksUpdateHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseAsyncNeuralNetUpdater(@NonNull ITrainableNeuralNet threadCurrent, @NonNull AsyncSharedNetworksUpdateHandler sharedNetworksUpdateHandler)
		Protected Friend Sub New(ByVal threadCurrent As ITrainableNeuralNet, ByVal sharedNetworksUpdateHandler As AsyncSharedNetworksUpdateHandler)
			Me.threadCurrent = threadCurrent
			Me.sharedNetworksUpdateHandler = sharedNetworksUpdateHandler
		End Sub

		Public MustOverride Overrides Sub update(ByVal dataType As DATA_TYPE) Implements INeuralNetUpdater(Of DATA_TYPE).update

		Protected Friend Overridable Sub updateAndSync(ByVal gradients As Gradients)
			sharedNetworksUpdateHandler.handleGradients(gradients)
		End Sub

		Public Overridable Sub synchronizeCurrent() Implements INeuralNetUpdater(Of DATA_TYPE).synchronizeCurrent
			threadCurrent.copyFrom(sharedNetworksUpdateHandler.getGlobalCurrent())
		End Sub

	End Class

End Namespace