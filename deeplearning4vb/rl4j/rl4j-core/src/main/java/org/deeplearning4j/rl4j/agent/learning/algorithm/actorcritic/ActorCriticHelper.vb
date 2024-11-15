﻿Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

	Public MustInherit Class ActorCriticHelper
		''' <summary>
		''' Create an empty INDArray to be used as the value array </summary>
		''' <param name="trainingBatchSize"> the size of the training batch </param>
		''' <returns> An empty value array </returns>
		Public MustOverride Function createValueLabels(ByVal trainingBatchSize As Integer) As INDArray

		''' <summary>
		''' Create an empty INDArray to be used as the policy array </summary>
		''' <param name="trainingBatchSize"> the size of the training batch </param>
		''' <returns> An empty policy array </returns>
		Public MustOverride Function createPolicyLabels(ByVal trainingBatchSize As Integer) As INDArray

		''' <summary>
		''' Set the advantage for a given action and training batch index in the policy array </summary>
		''' <param name="policy"> The policy array </param>
		''' <param name="idx"> The training batch index </param>
		''' <param name="action"> The action </param>
		''' <param name="advantage"> The advantage value </param>
		Public MustOverride Sub setPolicy(ByVal policy As INDArray, ByVal idx As Long, ByVal action As Integer, ByVal advantage As Double)
	End Class

End Namespace