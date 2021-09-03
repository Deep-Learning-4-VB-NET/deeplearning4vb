Imports System.Collections.Generic
Imports System.Linq
Imports Test = org.junit.jupiter.api.Test
Imports FunctionalUtils = org.nd4j.common.function.FunctionalUtils
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals

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


	Public Class FunctionalUtilsTest


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCoGroup()
		Public Overridable Sub testCoGroup()
			Dim leftMap As IList(Of Pair(Of String, String)) = New List(Of Pair(Of String, String))()
			Dim rightMap As IList(Of Pair(Of String, String)) = New List(Of Pair(Of String, String))()

			leftMap.Add(Pair.of("cat","adam"))
			leftMap.Add(Pair.of("dog","adam"))

			rightMap.Add(Pair.of("fish","alex"))
			rightMap.Add(Pair.of("cat","alice"))
			rightMap.Add(Pair.of("dog","steve"))

			'[(fish,([],[alex])), (dog,([adam],[steve])), (cat,([adam],[alice]))]
			Dim assertion As IDictionary(Of String, Pair(Of IList(Of String), IList(Of String))) = New Dictionary(Of String, Pair(Of IList(Of String), IList(Of String)))()
			assertion("cat") = Pair.of(java.util.Arrays.asList("adam"),java.util.Arrays.asList("alice"))
			assertion("dog") = Pair.of(java.util.Arrays.asList("adam"),java.util.Arrays.asList("steve"))
			assertion("fish") = Pair.of(Enumerable.Empty(Of String)(),java.util.Arrays.asList("alex"))

			Dim cogroup As IDictionary(Of String, Pair(Of IList(Of String), IList(Of String))) = FunctionalUtils.cogroup(leftMap, rightMap)
			assertEquals(assertion,cogroup)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGroupBy()
		Public Overridable Sub testGroupBy()
			Dim list As IList(Of Pair(Of Integer, Integer)) = New List(Of Pair(Of Integer, Integer))()
			For i As Integer = 0 To 9
				For j As Integer = 0 To 4
					list.Add(Pair.of(i, j))
				Next j
			Next i

			Dim integerIterableMap As IDictionary(Of Integer, IList(Of Integer)) = FunctionalUtils.groupByKey(list)
			assertEquals(10,integerIterableMap.Keys.Count)
			assertEquals(5,integerIterableMap(0).Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMapToPair()
		Public Overridable Sub testMapToPair()
			Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			For i As Integer = 0 To 4
				map(i.ToString()) = i.ToString()
			Next i

			Dim pairs As IList(Of Pair(Of String, String)) = FunctionalUtils.mapToPair(map)
			assertEquals(map.Count,pairs.Count)
		End Sub

	End Class

End Namespace