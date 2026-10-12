Imports System.Collections.Generic
Imports Test = org.junit.jupiter.api.Test
Imports org.nd4j.common.primitives
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


	Public Class CounterMapTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIterator()
		Public Overridable Sub testIterator()
			Dim counterMap As New CounterMap(Of Integer, Integer)()

			counterMap.incrementCount(0, 0, 1)
			counterMap.incrementCount(0, 1, 1)
			counterMap.incrementCount(0, 2, 1)
			counterMap.incrementCount(1, 0, 1)
			counterMap.incrementCount(1, 1, 1)
			counterMap.incrementCount(1, 2, 1)

			Dim iterator As IEnumerator(Of Pair(Of Integer, Integer)) = counterMap.getIterator()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim pair As Pair(Of Integer, Integer) = iterator.next()

			assertEquals(0, pair.First)
			assertEquals(0, pair.Second)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			pair = iterator.next()

			assertEquals(0, pair.First)
			assertEquals(1, pair.Second)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			pair = iterator.next()

			assertEquals(0, pair.First)
			assertEquals(2, pair.Second)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			pair = iterator.next()

			assertEquals(1, pair.First)
			assertEquals(0, pair.Second)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			pair = iterator.next()

			assertEquals(1, pair.First)
			assertEquals(1, pair.Second)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			pair = iterator.next()

			assertEquals(1, pair.First)
			assertEquals(2, pair.Second)


'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(iterator.hasNext())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIncrementAll()
		Public Overridable Sub testIncrementAll()
			Dim counterMapA As New CounterMap(Of Integer, Integer)()

			counterMapA.incrementCount(0, 0, 1)
			counterMapA.incrementCount(0, 1, 1)
			counterMapA.incrementCount(0, 2, 1)
			counterMapA.incrementCount(1, 0, 1)
			counterMapA.incrementCount(1, 1, 1)
			counterMapA.incrementCount(1, 2, 1)

			Dim counterMapB As New CounterMap(Of Integer, Integer)()

			counterMapB.incrementCount(1, 1, 1)
			counterMapB.incrementCount(2, 1, 1)

			counterMapA.incrementAll(counterMapB)

			assertEquals(2.0, counterMapA.getCount(1,1), 1e-5)
			assertEquals(1.0, counterMapA.getCount(2,1), 1e-5)
			assertEquals(1.0, counterMapA.getCount(0,0), 1e-5)


			assertEquals(7, counterMapA.totalSize())


			counterMapA.setCount(2, 1, 17)

			assertEquals(17.0, counterMapA.getCount(2, 1), 1e-5)
		End Sub
	End Class
End Namespace