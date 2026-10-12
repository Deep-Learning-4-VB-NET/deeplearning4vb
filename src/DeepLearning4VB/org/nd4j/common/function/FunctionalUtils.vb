Imports System.Collections.Generic
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

Namespace org.nd4j.common.function


	Public Class FunctionalUtils


		''' <summary>
		''' For each key in left and right, cogroup returns the list of values
		''' as a pair for each value present in left as well as right. </summary>
		''' <param name="left"> the left list of pairs to join </param>
		''' <param name="right"> the right list of pairs to join </param>
		''' @param <K> the key type </param>
		''' @param <V> the value type </param>
		''' <returns> a map of the list of values by key for each value in the left as well as the right
		''' with each element in the pair representing the values in left and right respectively. </returns>
		Public Shared Function cogroup(Of K, V)(ByVal left As IList(Of Pair(Of K, V)), ByVal right As IList(Of Pair(Of K, V))) As IDictionary(Of K, Pair(Of IList(Of V), IList(Of V)))
			Dim ret As IDictionary(Of K, Pair(Of IList(Of V), IList(Of V))) = New Dictionary(Of K, Pair(Of IList(Of V), IList(Of V)))()

			'group by key first to consolidate values
			Dim leftMap As IDictionary(Of K, IList(Of V)) = groupByKey(left)
			Dim rightMap As IDictionary(Of K, IList(Of V)) = groupByKey(right)

			''' <summary>
			''' Iterate over each key in the list
			''' adding values to the left items
			''' as values are found in the list.
			''' </summary>
			For Each entry As KeyValuePair(Of K, IList(Of V)) In leftMap.SetOfKeyValuePairs()
				Dim key As K = entry.Key
				If Not ret.ContainsKey(key) Then
					Dim leftListPair As IList(Of V) = New List(Of V)()
					Dim rightListPair As IList(Of V) = New List(Of V)()
					Dim p As Pair(Of IList(Of V), IList(Of V)) = Pair.of(leftListPair,rightListPair)
					ret(key) = p
				End If

				Dim p As Pair(Of IList(Of V), IList(Of V)) = ret(key)
				CType(p.First, List(Of V)).AddRange(entry.Value)


			Next entry

			''' <summary>
			''' Iterate over each key in the list
			''' adding values to the right items
			''' as values are found in the list.
			''' </summary>
			For Each entry As KeyValuePair(Of K, IList(Of V)) In rightMap.SetOfKeyValuePairs()
				Dim key As K = entry.Key
				If Not ret.ContainsKey(key) Then
					Dim leftListPair As IList(Of V) = New List(Of V)()
					Dim rightListPair As IList(Of V) = New List(Of V)()
					Dim p As Pair(Of IList(Of V), IList(Of V)) = Pair.of(leftListPair,rightListPair)
					ret(key) = p
				End If

				Dim p As Pair(Of IList(Of V), IList(Of V)) = ret(key)
				CType(p.Second, List(Of V)).AddRange(entry.Value)

			Next entry

			Return ret
		End Function

		''' <summary>
		''' Group the input pairs by the key of each pair. </summary>
		''' <param name="listInput"> the list of pairs to group </param>
		''' @param <K> the key type </param>
		''' @param <V> the value type </param>
		''' <returns> a map representing a grouping of the
		''' keys by the given input key type and list of values
		''' in the grouping. </returns>
		Public Shared Function groupByKey(Of K, V)(ByVal listInput As IList(Of Pair(Of K, V))) As IDictionary(Of K, IList(Of V))
			Dim ret As IDictionary(Of K, IList(Of V)) = New Dictionary(Of K, IList(Of V))()
			For Each pair As Pair(Of K, V) In listInput
				Dim currList As IList(Of V) = ret(pair.First)
				If currList Is Nothing Then
					currList = New List(Of V)()
					ret(pair.First) = currList
				End If

				currList.Add(pair.Second)
			Next pair

			Return ret
		End Function

		''' <summary>
		''' Convert a map with a set of entries of type K for key
		''' and V for value in to a list of <seealso cref="Pair"/> </summary>
		''' <param name="map"> the map to collapse </param>
		''' @param <K> the key type </param>
		''' @param <V> the value type </param>
		''' <returns> the collapsed map as a <seealso cref="System.Collections.IList"/> </returns>
		Public Shared Function mapToPair(Of K, V)(ByVal map As IDictionary(Of K, V)) As IList(Of Pair(Of K, V))
			Dim ret As IList(Of Pair(Of K, V)) = New List(Of Pair(Of K, V))(map.Count)
			For Each entry As KeyValuePair(Of K, V) In map.SetOfKeyValuePairs()
				ret.Add(Pair.of(entry.Key,entry.Value))
			Next entry

			Return ret
		End Function

	End Class

End Namespace