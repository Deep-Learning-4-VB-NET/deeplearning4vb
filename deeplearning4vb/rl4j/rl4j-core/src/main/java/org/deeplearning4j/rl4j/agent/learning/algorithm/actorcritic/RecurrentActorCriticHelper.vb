Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.actorcritic

	Public Class RecurrentActorCriticHelper
		Inherits ActorCriticHelper

		Private ReadOnly actionSpaceSize As Integer

		Public Sub New(ByVal actionSpaceSize As Integer)
			Me.actionSpaceSize = actionSpaceSize
		End Sub

		Public Overrides Function createValueLabels(ByVal trainingBatchSize As Integer) As INDArray
			Return Nd4j.create(1, 1, trainingBatchSize)
		End Function

		Public Overrides Function createPolicyLabels(ByVal trainingBatchSize As Integer) As INDArray
			Return Nd4j.zeros(1, actionSpaceSize, trainingBatchSize)
		End Function

		Public Overrides Sub setPolicy(ByVal policy As INDArray, ByVal idx As Long, ByVal action As Integer, ByVal advantage As Double)
			policy.putScalar(0, action, idx, advantage)
		End Sub
	End Class

End Namespace