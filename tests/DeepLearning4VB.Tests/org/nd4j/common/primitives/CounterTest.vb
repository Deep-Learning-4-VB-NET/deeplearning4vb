Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Test = org.junit.jupiter.api.Test
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @Slf4j public class CounterTest
	Public Class CounterTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCounterIncrementAll1()
		Public Overridable Sub testCounterIncrementAll1()
			Dim counterA As New Counter(Of String)()

			counterA.incrementCount("A", 1)
			counterA.incrementCount("A", 1)
			counterA.incrementCount("A", 1)



			Dim counterB As New Counter(Of String)()
			counterB.incrementCount("B", 2)
			counterB.incrementCount("B", 2)

			assertEquals(3.0, counterA.getCount("A"), 1e-5)
			assertEquals(4.0, counterB.getCount("B"), 1e-5)

			counterA.incrementAll(counterB)

			assertEquals(3.0, counterA.getCount("A"), 1e-5)
			assertEquals(4.0, counterA.getCount("B"), 1e-5)

			counterA.setCount("B", 234)

			assertEquals(234.0, counterA.getCount("B"), 1e-5)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCounterTopN1()
		Public Overridable Sub testCounterTopN1()
			Dim counterA As New Counter(Of String)()

			counterA.incrementCount("A", 1)
			counterA.incrementCount("B", 2)
			counterA.incrementCount("C", 3)
			counterA.incrementCount("D", 4)
			counterA.incrementCount("E", 5)

			counterA.keepTopNElements(4)

			assertEquals(4,counterA.size())

			' we expect element A to be gone
			assertEquals(0.0, counterA.getCount("A"), 1e-5)
			assertEquals(2.0, counterA.getCount("B"), 1e-5)
			assertEquals(3.0, counterA.getCount("C"), 1e-5)
			assertEquals(4.0, counterA.getCount("D"), 1e-5)
			assertEquals(5.0, counterA.getCount("E"), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testKeysSorted1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testKeysSorted1()
			Dim counterA As New Counter(Of String)()

			counterA.incrementCount("A", 1)
			counterA.incrementCount("B", 2)
			counterA.incrementCount("C", 3)
			counterA.incrementCount("D", 4)
			counterA.incrementCount("E", 5)

			assertEquals("E", counterA.argMax())

			Dim list As IList(Of String) = counterA.keySetSorted()

			assertEquals(5, list.Count)

			assertEquals("E", list(0))
			assertEquals("D", list(1))
			assertEquals("C", list(2))
			assertEquals("B", list(3))
			assertEquals("A", list(4))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCounterTotal()
		Public Overridable Sub testCounterTotal()
			Dim counter As New Counter(Of String)()

			counter.incrementCount("A", 1)
			counter.incrementCount("B", 1)
			counter.incrementCount("C", 1)

			assertEquals(3.0, counter.totalCount(), 1e-5)

			counter.setCount("B", 234)

			assertEquals(236.0, counter.totalCount(), 1e-5)

			counter.setCount("D", 1)

			assertEquals(237.0, counter.totalCount(), 1e-5)

			counter.removeKey("B")

			assertEquals(3.0, counter.totalCount(), 1e-5)

		End Sub

	End Class
End Namespace