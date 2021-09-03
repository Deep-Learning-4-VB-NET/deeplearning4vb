Imports CircularFifoQueue = org.apache.commons.collections4.queue.CircularFifoQueue
Imports HistoryMergeTransform = org.deeplearning4j.rl4j.observation.transform.operation.HistoryMergeTransform
Imports Preconditions = org.nd4j.common.base.Preconditions
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
Namespace org.deeplearning4j.rl4j.observation.transform.operation.historymerge

	Public Class CircularFifoStore
		Implements HistoryMergeElementStore

		Private ReadOnly queue As CircularFifoQueue(Of INDArray)

		Public Sub New(ByVal size As Integer)
			Preconditions.checkArgument(size > 0, "The size must be at least 1, got %s", size)
			queue = New CircularFifoQueue(Of INDArray)(size)
		End Sub

		''' <summary>
		''' Add an element to the store, if this addition would make the store to overflow, the new element replaces the oldest. </summary>
		''' <param name="elem"> </param>
		Public Overridable Sub add(ByVal elem As INDArray) Implements HistoryMergeElementStore.add
			queue.add(elem)
		End Sub

		''' <returns> The content of the store, returned in order from oldest to newest. </returns>
		Public Overridable Function get() As INDArray() Implements HistoryMergeElementStore.get
			Dim size As Integer = queue.size()
			Dim array(size - 1) As INDArray
			For i As Integer = 0 To size - 1
				array(i) = queue.get(i).castTo(Nd4j.dataType())
			Next i
			Return array
		End Function

		''' <summary>
		''' The CircularFifoStore needs to be completely filled before being ready. </summary>
		''' <returns> false when the number of elements in the store is less than the store capacity (default is 4) </returns>
		Public Overridable ReadOnly Property Ready As Boolean Implements HistoryMergeElementStore.isReady
			Get
				Return queue.isAtFullCapacity()
			End Get
		End Property

		''' <summary>
		''' Clears the store.
		''' </summary>
		Public Overridable Sub reset() Implements HistoryMergeElementStore.reset
			queue.clear()
		End Sub
	End Class

End Namespace