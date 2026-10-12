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
'ORIGINAL LINE: @DisplayName("Aggregable Multi Op Test") class AggregableMultiOpTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class AggregableMultiOpTest
		Inherits BaseND4JTest

		Private intList As IList(Of Integer) = New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi") void testMulti() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMulti()
			Dim af As New AggregatorImpls.AggregableFirst(Of Integer)()
			Dim [as] As New AggregatorImpls.AggregableSum(Of Integer)()
			Dim multi As New AggregableMultiOp(Of Integer)(java.util.Arrays.asList(af, [as]))
			assertTrue(multi.getOperations().size() = 2)
			For i As Integer = 0 To intList.Count - 1
				multi.accept(intList(i))
			Next i
			' mutablility
			assertTrue([as].get().toDouble() = 45R)
			assertTrue(af.get().toInt() = 1)
			Dim res As IList(Of Writable) = multi.get()
			assertTrue(res(1).toDouble() = 45R)
			assertTrue(res(0).toInt() = 1)
			Dim rf As New AggregatorImpls.AggregableFirst(Of Integer)()
			Dim rs As New AggregatorImpls.AggregableSum(Of Integer)()
			Dim reverse As New AggregableMultiOp(Of Integer)(java.util.Arrays.asList(rf, rs))
			For i As Integer = 0 To intList.Count - 1
				reverse.accept(intList(intList.Count - i - 1))
			Next i
			Dim revRes As IList(Of Writable) = reverse.get()
			assertTrue(revRes(1).toDouble() = 45R)
			assertTrue(revRes(0).toInt() = 9)
			multi.combine(reverse)
			Dim combinedRes As IList(Of Writable) = multi.get()
			assertTrue(combinedRes(1).toDouble() = 90R)
			assertTrue(combinedRes(0).toInt() = 1)
		End Sub
	End Class

End Namespace