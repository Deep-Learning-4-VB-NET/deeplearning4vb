Imports System.Collections.Generic
Imports Writable = org.datavec.api.writable.Writable
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.datavec.api.transform.ops

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Dispatch Op Test") class DispatchOpTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class DispatchOpTest
		Inherits BaseND4JTest

		Private intList As IList(Of Integer) = New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9}

		Private stringList As IList(Of String) = New List(Of String) From {"arakoa", "abracadabra", "blast", "acceptance"}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dispatch Simple") void testDispatchSimple()
		Friend Overridable Sub testDispatchSimple()
			Dim af As New AggregatorImpls.AggregableFirst(Of Integer)()
			Dim [as] As New AggregatorImpls.AggregableSum(Of Integer)()
			Dim multiaf As New AggregableMultiOp(Of Integer)(Collections.singletonList(Of IAggregableReduceOp(Of Integer, Writable))(af))
			Dim multias As New AggregableMultiOp(Of Integer)(Collections.singletonList(Of IAggregableReduceOp(Of Integer, Writable))([as]))
			Dim parallel As New DispatchOp(Of Integer, Writable)(Arrays.asList(Of IAggregableReduceOp(Of Integer, IList(Of Writable)))(multiaf, multias))
			assertTrue(multiaf.getOperations().size() = 1)
			assertTrue(multias.getOperations().size() = 1)
			assertTrue(parallel.getOperations().size() = 2)
			For i As Integer = 0 To intList.Count - 1
				parallel.accept(New List(Of Integer) From {intList(i), intList(i)})
			Next i
			Dim res As IList(Of Writable) = parallel.get()
			assertTrue(res(1).toDouble() = 45R)
			assertTrue(res(0).toInt() = 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dispatch Flat Map") void testDispatchFlatMap()
		Friend Overridable Sub testDispatchFlatMap()
			Dim af As New AggregatorImpls.AggregableFirst(Of Integer)()
			Dim [as] As New AggregatorImpls.AggregableSum(Of Integer)()
			Dim multi As New AggregableMultiOp(Of Integer)(Arrays.asList(af, [as]))
			Dim al As New AggregatorImpls.AggregableLast(Of Integer)()
			Dim amax As New AggregatorImpls.AggregableMax(Of Integer)()
			Dim otherMulti As New AggregableMultiOp(Of Integer)(Arrays.asList(al, amax))
			Dim parallel As New DispatchOp(Of Integer, Writable)(Arrays.asList(Of IAggregableReduceOp(Of Integer, IList(Of Writable)))(multi, otherMulti))
			assertTrue(multi.getOperations().size() = 2)
			assertTrue(otherMulti.getOperations().size() = 2)
			assertTrue(parallel.getOperations().size() = 2)
			For i As Integer = 0 To intList.Count - 1
				parallel.accept(New List(Of Integer) From {intList(i), intList(i)})
			Next i
			Dim res As IList(Of Writable) = parallel.get()
			assertTrue(res(1).toDouble() = 45R)
			assertTrue(res(0).toInt() = 1)
			assertTrue(res(3).toDouble() = 9)
			assertTrue(res(2).toInt() = 9)
		End Sub
	End Class

End Namespace