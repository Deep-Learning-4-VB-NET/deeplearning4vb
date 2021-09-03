Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class CounterMap<F, S> implements java.io.Serializable
	<Serializable>
	Public Class CounterMap(Of F, S)
		Private Const serialVersionUID As Long = 119L

		Protected Friend maps As IDictionary(Of F, Counter(Of S)) = New ConcurrentDictionary(Of F, Counter(Of S))()

		Public Sub New()

		End Sub

		''' <summary>
		''' This method checks if this CounterMap has any values stored
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return maps.Count = 0
			End Get
		End Property

		''' <summary>
		''' This method checks if this CounterMap has any values stored for a given first element
		''' </summary>
		''' <param name="element">
		''' @return </param>
		Public Overridable Function isEmpty(ByVal element As F) As Boolean
			If Empty Then
				Return True
			End If

			Dim m As Counter(Of S) = maps(element)
			If m Is Nothing Then
				Return True
			Else
				Return m.Empty
			End If
		End Function

		''' <summary>
		''' This method will increment values of this counter, by counts of other counter
		''' </summary>
		''' <param name="other"> </param>
		Public Overridable Sub incrementAll(ByVal other As CounterMap(Of F, S))
			For Each entry As KeyValuePair(Of F, Counter(Of S)) In other.maps.SetOfKeyValuePairs()
				Dim key As F = entry.Key
				Dim innerCounter As Counter(Of S) = entry.Value
				For Each innerEntry As KeyValuePair(Of S, AtomicDouble) In innerCounter.entrySet()
					Dim value As S = innerEntry.Key
					incrementCount(key, value, innerEntry.Value.get())
				Next innerEntry
			Next entry
		End Sub

		''' <summary>
		''' This method will increment counts for a given first/second pair
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="second"> </param>
		''' <param name="inc"> </param>
		Public Overridable Sub incrementCount(ByVal first As F, ByVal second As S, ByVal inc As Double)
			Dim counter As Counter(Of S) = maps(first)
			If counter Is Nothing Then
				counter = New Counter(Of S)()
				maps(first) = counter
			End If

			counter.incrementCount(second, inc)
		End Sub

		''' <summary>
		''' This method returns counts for a given first/second pair
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="second">
		''' @return </param>
		Public Overridable Function getCount(ByVal first As F, ByVal second As S) As Double
			Dim counter As Counter(Of S) = maps(first)
			If counter Is Nothing Then
				Return 0.0
			End If

			Return counter.getCount(second)
		End Function

		''' <summary>
		''' This method allows you to set counter value for a given first/second pair
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="second"> </param>
		''' <param name="value">
		''' @return </param>
		Public Overridable Function setCount(ByVal first As F, ByVal second As S, ByVal value As Double) As Double
			Dim counter As Counter(Of S) = maps(first)
			If counter Is Nothing Then
				counter = New Counter(Of S)()
				maps(first) = counter
			End If

			Return counter.setCount(second, value)
		End Function

		''' <summary>
		''' This method returns pair of elements with a max value
		''' 
		''' @return
		''' </summary>
		Public Overridable Function argMax() As Pair(Of F, S)
			Dim maxCount As Double? = -Double.MaxValue
			Dim maxKey As Pair(Of F, S) = Nothing
			For Each entry As KeyValuePair(Of F, Counter(Of S)) In maps.SetOfKeyValuePairs()
				Dim counter As Counter(Of S) = entry.Value
				Dim localMax As S = counter.argMax()
				If counter.getCount(localMax) > maxCount OrElse maxKey Is Nothing Then
					maxKey = New Pair(Of F, S)(entry.Key, localMax)
					maxCount = counter.getCount(localMax)
				End If
			Next entry
			Return maxKey
		End Function

		''' <summary>
		''' This method purges all counters
		''' </summary>
		Public Overridable Sub clear()
			maps.Clear()
		End Sub

		''' <summary>
		''' This method purges counter for a given first element </summary>
		''' <param name="element"> </param>
		Public Overridable Sub clear(ByVal element As F)
			Dim s As Counter(Of S) = maps(element)
			If s IsNot Nothing Then
				s.clear()
			End If
		End Sub

		''' <summary>
		''' This method returns Set of all first elements
		''' @return
		''' </summary>
		Public Overridable Function keySet() As ISet(Of F)
			Return maps.Keys
		End Function

		''' <summary>
		''' This method returns counter for a given first element
		''' </summary>
		''' <param name="first">
		''' @return </param>
		Public Overridable Function getCounter(ByVal first As F) As Counter(Of S)
			Return maps(first)
		End Function

		''' <summary>
		''' This method returns Iterator of all first/second pairs stored in this counter
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Iterator As IEnumerator(Of Pair(Of F, S))
			Get
				Return New IteratorAnonymousInnerClass(Me)
			End Get
		End Property

		Private Class IteratorAnonymousInnerClass
			Implements IEnumerator(Of Pair(Of F, S))

			Private ReadOnly outerInstance As CounterMap(Of F, S)

			Public Sub New(ByVal outerInstance As CounterMap(Of F, S))
				Me.outerInstance = outerInstance

				outerIt = outerInstance.keySet().GetEnumerator()
			End Sub


			Friend outerIt As IEnumerator(Of F)
			Friend innerIt As IEnumerator(Of S)
			Friend curKey As F

			Private Function hasInside() As Boolean
				If innerIt Is Nothing OrElse Not innerIt.hasNext() Then
					If Not outerIt.hasNext() Then
						Return False
					End If
					curKey = outerIt.next()
					innerIt = outerInstance.getCounter(curKey).keySet().GetEnumerator()
				End If
				Return True
			End Function

			Public Function hasNext() As Boolean
				Return hasInside()
			End Function

			Public Function [next]() As Pair(Of F, S)
				hasInside()
				If curKey Is Nothing Then
					Throw New Exception("Outer element can't be null")
				End If

				Return Pair.makePair(curKey, innerIt.next())
			End Function

			Public Sub remove()
				'
			End Sub
		End Class

		''' <summary>
		''' This method returns number of First elements in this CounterMap
		''' @return
		''' </summary>
		Public Overridable Function size() As Integer
			Return maps.Count
		End Function

		''' <summary>
		''' This method returns total number of elements in this CounterMap
		''' @return
		''' </summary>
		Public Overridable Function totalSize() As Integer
			Dim size As Integer = 0
			For Each first As F In keySet()
				size += getCounter(first).size()
			Next first

			Return size
		End Function
	End Class

End Namespace