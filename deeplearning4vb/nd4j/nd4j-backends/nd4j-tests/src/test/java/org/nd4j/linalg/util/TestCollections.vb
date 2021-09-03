Imports System
Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports CompactHeapStringList = org.nd4j.common.collection.CompactHeapStringList
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.linalg.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class TestCollections extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestCollections
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testCompactHeapStringList(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompactHeapStringList(ByVal backend As Nd4jBackend)

			Dim reallocSizeBytes() As Integer = {1024, 1048576}
			Dim intReallocSizeBytes() As Integer = {1024, 1048576}

			Dim numElementsToTest As Integer = 10000
			Dim minLength As Integer = 1
			Dim maxLength As Integer = 1048

			Dim r As New Random(12345)

			Dim compare As IList(Of String) = New List(Of String)(numElementsToTest)
			For i As Integer = 0 To numElementsToTest - 1
				Dim thisLength As Integer = minLength + r.Next(maxLength)
				Dim c(thisLength - 1) As Char
				For j As Integer = 0 To c.Length - 1
					c(j) = CChar(r.Next(65536))
				Next j
				Dim s As New String(c)
				compare.Add(s)
			Next i


			For Each rb As Integer In reallocSizeBytes
				For Each irb As Integer In intReallocSizeBytes
					'                System.out.println(rb + "\t" + irb);
					Dim list As IList(Of String) = New CompactHeapStringList(rb, irb)

					assertTrue(list.Count = 0)
					assertEquals(0, list.Count)


					For i As Integer = 0 To numElementsToTest - 1
						Dim s As String = compare(i)
						list.Add(s)

						assertEquals(i + 1, list.Count)
						Dim s2 As String = list(i)
						assertEquals(s, s2)
					Next i

					assertEquals(numElementsToTest, list.Count)

					assertEquals(list, compare)
					assertEquals(compare, list)
					assertEquals(compare, java.util.Arrays.asList(list.ToArray()))

					For i As Integer = 0 To numElementsToTest - 1
						assertEquals(i, list.IndexOf(compare(i)))
					Next i

					Dim iter As IEnumerator(Of String) = list.GetEnumerator()
					Dim count As Integer = 0
					Do While iter.MoveNext()
						Dim s As String = iter.Current
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(s, compare.get(count++));
						assertEquals(s, compare(count))
							count += 1
					Loop
					assertEquals(numElementsToTest, count)
				Next irb
			Next rb
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace