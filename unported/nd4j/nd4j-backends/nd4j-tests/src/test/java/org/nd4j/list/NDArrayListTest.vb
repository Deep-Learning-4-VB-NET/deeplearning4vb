Imports System.Collections.Generic
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.list


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class NDArrayListTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NDArrayListTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testList(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testList(ByVal backend As Nd4jBackend)
			Dim ndArrayList As New NDArrayList()
			Dim arrayAssertion As IList(Of Double) = New List(Of Double)()
			For i As Integer = 0 To 10
				ndArrayList.add(CDbl(i))
				arrayAssertion.Add(CDbl(i))
			Next i

			assertEquals(arrayAssertion.Count,arrayAssertion.Count)
			assertEquals(arrayAssertion,ndArrayList)


			arrayAssertion.RemoveAt(1)
			ndArrayList.remove(1)
			assertEquals(ndArrayList,arrayAssertion)

			arrayAssertion.RemoveAt(2)
			ndArrayList.remove(2)
			assertEquals(ndArrayList,arrayAssertion)


			arrayAssertion.Insert(5,8.0)
			ndArrayList.add(5,8.0)
			assertEquals(arrayAssertion,ndArrayList)

			assertEquals(arrayAssertion.Contains(8.0),ndArrayList.contains(8.0))
			assertEquals(arrayAssertion.IndexOf(8.0),ndArrayList.indexOf(8.0))
			assertEquals(arrayAssertion.LastIndexOf(8.0),ndArrayList.lastIndexOf(8.0))
			assertEquals(ndArrayList.size(),ndArrayList.array().length())

		End Sub
	End Class

End Namespace