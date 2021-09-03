Imports HistoryMergeTransform = org.deeplearning4j.rl4j.observation.transform.operation.HistoryMergeTransform
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
Namespace org.deeplearning4j.rl4j.observation.transform.operation.historymerge

	''' <summary>
	''' HistoryMergeElementStore is used with the <seealso cref="HistoryMergeTransform HistoryMergeTransform"/>. Used to supervise how data from the
	''' HistoryMergeTransform is stored.
	''' 
	''' @author Alexandre Boulanger
	''' </summary>
	Public Interface HistoryMergeElementStore
		''' <summary>
		''' Add an element into the store </summary>
		''' <param name="observation"> </param>
		Sub add(ByVal observation As INDArray)

		''' <summary>
		''' Get the content of the store </summary>
		''' <returns> the content of the store </returns>
		Function get() As INDArray()

		''' <summary>
		''' Used to tell the HistoryMergeTransform that the store is ready. The HistoryMergeTransform will tell the <seealso cref="org.deeplearning4j.rl4j.observation.transform.TransformProcess TransformProcess"/>
		''' to skip the observation is the store is not ready. </summary>
		''' <returns> true if the store is ready </returns>
		ReadOnly Property Ready As Boolean

		''' <summary>
		''' Resets the store to an initial state.
		''' </summary>
		Sub reset()
	End Interface

End Namespace