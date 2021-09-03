Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.experience

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

Namespace org.deeplearning4j.rl4j.learning.sync


	Public Interface IExpReplay(Of A)

		''' <returns> The size of the batch that will be returned by getBatch() </returns>
		ReadOnly Property BatchSize As Integer

		''' <returns> a batch of uniformly sampled transitions </returns>
		ReadOnly Property Batch As List(Of StateActionRewardState(Of A))

		''' 
		''' <param name="stateActionRewardState"> a new transition to store </param>
		Sub store(ByVal stateActionRewardState As StateActionRewardState(Of A))

		''' <returns> The desired size of batches </returns>
		ReadOnly Property DesignatedBatchSize As Integer
	End Interface

End Namespace