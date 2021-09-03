Imports System
Imports System.Collections.Concurrent
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

Namespace org.nd4j.common.primitives



	<Serializable>
	Public Class Counter(Of T)
		Private Const serialVersionUID As Long = 119L

		Protected Friend map As New ConcurrentDictionary(Of T, AtomicDouble)()
'JAVA TO VB CONVERTER NOTE: The field totalCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend totalCount_Conflict As New AtomicDouble(0)
		Protected Friend dirty As New AtomicBoolean(False)

		Public Sub New()

		End Sub

		Public Overridable Function getCount(ByVal element As T) As Double
			Dim t As AtomicDouble = map(element)
			If t Is Nothing Then
				Return 0.0
			End If

			Return t.get()
		End Function

		Public Overridable Sub incrementCount(ByVal element As T, ByVal inc As Double)
			Dim t As AtomicDouble = map(element)
			If t IsNot Nothing Then
				t.addAndGet(inc)
			Else
				map(element) = New AtomicDouble(inc)
			End If

			totalCount_Conflict.addAndGet(inc)
		End Sub

		''' <summary>
		''' This method will increment all elements in collection
		''' </summary>
		''' <param name="elements"> </param>
		''' <param name="inc"> </param>
		Public Overridable Sub incrementAll(ByVal elements As ICollection(Of T), ByVal inc As Double)
			For Each element As T In elements
				incrementCount(element, inc)
			Next element
		End Sub

		''' <summary>
		''' This method will increment counts of this counter by counts from other counter </summary>
		''' <param name="other"> </param>
		Public Overridable Sub incrementAll(Of T2 As T)(ByVal other As Counter(Of T2))
			For Each element As T2 In other.keySet()
				Dim cnt As Double = other.getCount(element)
				incrementCount(element, cnt)
			Next element
		End Sub

		''' <summary>
		''' This method returns probability of given element
		''' </summary>
		''' <param name="element">
		''' @return </param>
		Public Overridable Function getProbability(ByVal element As T) As Double
			If totalCount() <= 0.0 Then
				Throw New System.InvalidOperationException("Can't calculate probability with empty counter")
			End If

			Return getCount(element) / totalCount()
		End Function

		''' <summary>
		''' This method sets new counter value for given element
		''' </summary>
		''' <param name="element"> element to be updated </param>
		''' <param name="count"> new counter value </param>
		''' <returns> previous value </returns>
		Public Overridable Function setCount(ByVal element As T, ByVal count As Double) As Double
			Dim t As AtomicDouble = map(element)
			If t IsNot Nothing Then
				Dim val As Double = t.getAndSet(count)
				dirty.set(True)
				Return val
			Else
				map(element) = New AtomicDouble(count)
				totalCount_Conflict.addAndGet(count)
				Return 0
			End If

		End Function

		''' <summary>
		''' This method returns Set of elements used in this counter
		''' 
		''' @return
		''' </summary>
		Public Overridable Function keySet() As ISet(Of T)
			Return map.Keys
		End Function

		''' <summary>
		''' This method returns TRUE if counter has no elements, FALSE otherwise
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return map.Count = 0
			End Get
		End Property

		''' <summary>
		''' This method returns Set<Entry> of this counter
		''' @return
		''' </summary>
		Public Overridable Function entrySet() As ISet(Of KeyValuePair(Of T, AtomicDouble))
			Return map.SetOfKeyValuePairs()
		End Function

		''' <summary>
		''' This method returns List of elements, sorted by their counts
		''' @return
		''' </summary>
		Public Overridable Function keySetSorted() As IList(Of T)
			Dim result As IList(Of T) = New List(Of T)()

			Dim pq As PriorityQueue(Of Pair(Of T, Double)) = asPriorityQueue()
			Do While Not pq.isEmpty()
				result.Add(pq.poll().getFirst())
			Loop

			Return result
		End Function

		''' <summary>
		''' This method will apply normalization to counter values and totals.
		''' </summary>
		Public Overridable Sub normalize()
			For Each key As T In keySet()
				setCount(key, getCount(key) / totalCount_Conflict.get())
			Next key

			rebuildTotals()
		End Sub

		Protected Friend Overridable Sub rebuildTotals()
			totalCount_Conflict.set(0)
			For Each key As T In keySet()
				totalCount_Conflict.addAndGet(getCount(key))
			Next key

			dirty.set(False)
		End Sub

		''' <summary>
		''' This method returns total sum of counter values
		''' @return
		''' </summary>
		Public Overridable Function totalCount() As Double
			If dirty.get() Then
				rebuildTotals()
			End If

			Return totalCount_Conflict.get()
		End Function

		''' <summary>
		''' This method removes given key from counter
		''' </summary>
		''' <param name="element"> </param>
		''' <returns> counter value </returns>
		Public Overridable Function removeKey(ByVal element As T) As Double
			Dim v As AtomicDouble = map.Remove(element)
			dirty.set(True)

			If v IsNot Nothing Then
				Return v.get()
			Else
				Return 0.0
			End If
		End Function

		''' <summary>
		''' This method returns element with highest counter value
		''' 
		''' @return
		''' </summary>
		Public Overridable Function argMax() As T
			Dim maxCount As Double = -Double.MaxValue
			Dim maxKey As T = Nothing
			For Each entry As KeyValuePair(Of T, AtomicDouble) In map.SetOfKeyValuePairs()
				If entry.Value.get() > maxCount OrElse maxKey Is Nothing Then
					maxKey = entry.Key
					maxCount = entry.Value.get()
				End If
			Next entry
			Return maxKey
		End Function

		''' <summary>
		''' This method will remove all elements with counts below given threshold from counter </summary>
		''' <param name="threshold"> </param>
		Public Overridable Sub dropElementsBelowThreshold(ByVal threshold As Double)
			Dim iterator As IEnumerator(Of T) = keySet().GetEnumerator()
			Do While iterator.MoveNext()
				Dim element As T = iterator.Current
				Dim val As Double = map(element).get()
				If val < threshold Then
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
					iterator.remove()
					dirty.set(True)
				End If
			Loop

		End Sub

		''' <summary>
		''' This method checks, if element exist in this counter
		''' </summary>
		''' <param name="element">
		''' @return </param>
		Public Overridable Function containsElement(ByVal element As T) As Boolean
			Return map.ContainsKey(element)
		End Function

		''' <summary>
		''' This method effectively resets counter to empty state
		''' </summary>
		Public Overridable Sub clear()
			map.Clear()
			totalCount_Conflict.set(0.0)
			dirty.set(False)
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is Counter) Then
				Return False
			End If
			Dim c2 As Counter = DirectCast(o, Counter)
			Return map.Equals(c2.map)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return map.GetHashCode()
		End Function

		''' <summary>
		''' Returns total number of tracked elements
		''' 
		''' @return
		''' </summary>
		Public Overridable Function size() As Integer
			Return map.Count
		End Function

		''' <summary>
		''' This method removes all elements except of top N by counter values </summary>
		''' <param name="N"> </param>
		Public Overridable Sub keepTopNElements(ByVal N As Integer)
			Dim queue As PriorityQueue(Of Pair(Of T, Double)) = asPriorityQueue()
			clear()
			For e As Integer = 0 To N - 1
				Dim pair As Pair(Of T, Double) = queue.poll()
				If pair IsNot Nothing Then
					incrementCount(pair.First, pair.Second)
				End If
			Next e
		End Sub


		Public Overridable Function asPriorityQueue() As PriorityQueue(Of Pair(Of T, Double))
			Dim pq As New PriorityQueue(Of Pair(Of T, Double))(Math.Max(1,map.Count), New PairComparator(Me))
			For Each entry As KeyValuePair(Of T, AtomicDouble) In map.SetOfKeyValuePairs()
				pq.add(Pair.create(entry.Key, entry.Value.get()))
			Next entry

			Return pq
		End Function


		Public Overridable Function asReversedPriorityQueue() As PriorityQueue(Of Pair(Of T, Double))
			Dim pq As New PriorityQueue(Of Pair(Of T, Double))(Math.Max(1,map.Count), New ReversedPairComparator(Me))
			For Each entry As KeyValuePair(Of T, AtomicDouble) In map.SetOfKeyValuePairs()
				pq.add(Pair.create(entry.Key, entry.Value.get()))
			Next entry

			Return pq
		End Function

		Public Class PairComparator
			Implements IComparer(Of Pair(Of T, Double))

			Private ReadOnly outerInstance As Counter(Of T)

			Public Sub New(ByVal outerInstance As Counter(Of T))
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Function Compare(ByVal o1 As Pair(Of T, Double), ByVal o2 As Pair(Of T, Double)) As Integer Implements IComparer(Of Pair(Of T, Double)).Compare
				Return o2.value.CompareTo(o1.value)
			End Function
		End Class

		Public Class ReversedPairComparator
			Implements IComparer(Of Pair(Of T, Double))

			Private ReadOnly outerInstance As Counter(Of T)

			Public Sub New(ByVal outerInstance As Counter(Of T))
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Function Compare(ByVal o1 As Pair(Of T, Double), ByVal o2 As Pair(Of T, Double)) As Integer Implements IComparer(Of Pair(Of T, Double)).Compare
				Return o1.value.CompareTo(o2.value)
			End Function
		End Class
	End Class

End Namespace