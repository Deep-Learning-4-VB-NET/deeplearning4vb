Imports System.Collections.Generic

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

Namespace org.deeplearning4j.rl4j.agent.learning.algorithm


	Public Interface IUpdateAlgorithm(Of RESULT_TYPE, EXPERIENCE_TYPE)
		''' <summary>
		''' Compute the labels required to update the network from the training batch </summary>
		''' <param name="trainingBatch"> The transitions from the experience replay </param>
		''' <returns> A DataSet where every element is the observation and the estimated Q-Values for all actions </returns>
		Function compute(ByVal trainingBatch As IList(Of EXPERIENCE_TYPE)) As RESULT_TYPE
	End Interface

End Namespace