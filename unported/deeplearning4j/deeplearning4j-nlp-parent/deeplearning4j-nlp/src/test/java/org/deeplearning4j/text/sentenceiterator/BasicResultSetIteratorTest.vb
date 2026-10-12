Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports Mockito = org.mockito.Mockito
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

Namespace org.deeplearning4j.text.sentenceiterator


	Public Class BasicResultSetIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHasMoreLines() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasMoreLines()

			' Setup a mock ResultSet object
			Dim resultSetMock As ResultSet = Mockito.mock(GetType(ResultSet))

			' when .next() is called, first time true, then false
			Mockito.when(resultSetMock.next()).thenReturn(True).thenReturn(False)
			Mockito.when(resultSetMock.getString("line")).thenReturn("The quick brown fox")

			Dim iterator As New BasicResultSetIterator(resultSetMock, "line")

			Dim cnt As Integer = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(1, cnt)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHasMoreLinesAndReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasMoreLinesAndReset()

			' Setup a mock ResultSet object
			Dim resultSetMock As ResultSet = Mockito.mock(GetType(ResultSet))

			' when .next() is called, first time true, then false, then after we reset we want the same behaviour
			Mockito.when(resultSetMock.next()).thenReturn(True).thenReturn(False).thenReturn(True).thenReturn(False)
			Mockito.when(resultSetMock.getString("line")).thenReturn("The quick brown fox")

			Dim iterator As New BasicResultSetIterator(resultSetMock, "line")

			Dim cnt As Integer = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(1, cnt)

			iterator.reset()

			cnt = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(1, cnt)
		End Sub
	End Class

End Namespace