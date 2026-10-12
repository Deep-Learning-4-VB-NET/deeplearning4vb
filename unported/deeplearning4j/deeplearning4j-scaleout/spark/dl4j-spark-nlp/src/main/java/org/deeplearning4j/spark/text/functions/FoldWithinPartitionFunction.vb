Imports System.Collections.Generic
Imports Accumulator = org.apache.spark.Accumulator
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.spark.text.functions


	''' <summary>
	''' @author jeffreytang
	''' </summary>
	Public Class FoldWithinPartitionFunction
		Implements Function2(Of Integer, IEnumerator(Of AtomicLong), IEnumerator(Of AtomicLong))

		Public Sub New(ByVal maxPartitionAcc As Accumulator(Of Counter(Of Integer)))
			Me.maxPerPartitionAcc = maxPartitionAcc
		End Sub

		Private maxPerPartitionAcc As Accumulator(Of Counter(Of Integer))


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<java.util.concurrent.atomic.AtomicLong> call(System.Nullable<Integer> ind, java.util.Iterator<java.util.concurrent.atomic.AtomicLong> partition) throws Exception
		Public Overrides Function [call](ByVal ind As Integer?, ByVal partition As IEnumerator(Of AtomicLong)) As IEnumerator(Of AtomicLong)

			Dim foldedItemList As IList(Of AtomicLong) = New ArrayListAnonymousInnerClass(Me)

			' Recurrent state implementation of cum sum
			Dim foldedItemListSize As Integer = 1
			Do While partition.MoveNext()
				Dim curPartitionItem As Long = partition.Current.get()
				Dim lastFoldedIndex As Integer = foldedItemListSize - 1
				Dim lastFoldedItem As Long = foldedItemList(lastFoldedIndex).get()
				Dim sumLastCurrent As New AtomicLong(curPartitionItem + lastFoldedItem)

				foldedItemList(lastFoldedIndex) = sumLastCurrent
				foldedItemList.Add(sumLastCurrent)
				foldedItemListSize += 1
			Loop

			' Update Accumulator
			Dim maxFoldedItem As Long = foldedItemList.RemoveAt(foldedItemListSize - 1).get()
			Dim partitionIndex2maxItemCounter As New Counter(Of Integer)()
			partitionIndex2maxItemCounter.incrementCount(ind, maxFoldedItem)
			maxPerPartitionAcc.add(partitionIndex2maxItemCounter)

			Return foldedItemList.GetEnumerator()
		End Function

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of AtomicLong)

			Private ReadOnly outerInstance As FoldWithinPartitionFunction

			Public Sub New(ByVal outerInstance As FoldWithinPartitionFunction)
				Me.outerInstance = outerInstance

				Me.add(New AtomicLong(0L))
			End Sub

		End Class
	End Class

End Namespace