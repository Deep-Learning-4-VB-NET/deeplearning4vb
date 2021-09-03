Imports System.Collections.Generic
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Broadcast = org.apache.spark.broadcast.Broadcast
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
	Public Class FoldBetweenPartitionFunction
		Implements Function2(Of Integer, IEnumerator(Of AtomicLong), IEnumerator(Of Long))

		Private broadcastedMaxPerPartitionCounter As Broadcast(Of Counter(Of Integer))

		Public Sub New(ByVal broadcastedMaxPerPartitionCounter As Broadcast(Of Counter(Of Integer)))
			Me.broadcastedMaxPerPartitionCounter = broadcastedMaxPerPartitionCounter
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<Long> call(System.Nullable<Integer> ind, java.util.Iterator<java.util.concurrent.atomic.AtomicLong> partition) throws Exception
		Public Overrides Function [call](ByVal ind As Integer?, ByVal partition As IEnumerator(Of AtomicLong)) As IEnumerator(Of Long)
			Dim sumToAdd As Integer = 0
			Dim maxPerPartitionCounterInScope As Counter(Of Integer) = broadcastedMaxPerPartitionCounter.value()

			' Add the sum of counts of all the partition with an index lower than the current one
			If ind <> 0 Then
				For i As Integer = 0 To ind.Value - 1
					sumToAdd += maxPerPartitionCounterInScope.getCount(i)
				Next i
			End If

			' Add the sum of counts to each element of the partition
			Dim itemsAddedToList As IList(Of Long) = New List(Of Long)()
			Do While partition.MoveNext()
				itemsAddedToList.Add(partition.Current.get() + sumToAdd)
			Loop

			Return itemsAddedToList.GetEnumerator()
		End Function
	End Class

End Namespace