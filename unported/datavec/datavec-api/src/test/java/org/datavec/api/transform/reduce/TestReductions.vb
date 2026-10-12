Imports System.Collections.Generic
Imports org.datavec.api.transform.ops
Imports GeographicMidpointReduction = org.datavec.api.transform.reduce.impl.GeographicMidpointReduction
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.datavec.api.transform.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestReductions extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestReductions
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGeographicMidPointReduction()
		Public Overridable Sub testGeographicMidPointReduction()

			'http://www.geomidpoint.com/example.html
			'That particular example is weighted - have 3x weight for t1, 2x weight for t2, 1x weight for t1
			Dim t1 As New Text("40.7143528,-74.0059731")
			Dim t2 As New Text("41.8781136,-87.6297982")
			Dim t3 As New Text("33.7489954,-84.3879824")

			Dim list As IList(Of Writable) = New List(Of Writable) From {Of Writable}

			Dim reduction As New GeographicMidpointReduction(",")

			Dim reduceOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = reduction.reduceOp()
			For Each w As Writable In list
				reduceOp.accept(w)
			Next w

			Dim listOut As IList(Of Writable) = reduceOp.get()
			assertEquals(1, listOut.Count)
			Dim wOut As Writable = listOut(0)

			Dim split() As String = wOut.ToString().Split(",", True)
			Dim lat As Double = Double.Parse(split(0))
			Dim lng As Double = Double.Parse(split(1))

			Dim expLat As Double = 40.11568861
			Dim expLong As Double = -80.29960280

			assertEquals(expLat, lat, 1e-6)
			assertEquals(expLong, lng, 1e-6)


			'Test multiple reductions
			list = New List(Of Writable) From {Of Writable}
			Dim list2 As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			reduceOp = reduction.reduceOp()
			For Each w As Writable In list
				reduceOp.accept(w)
			Next w

			Dim reduceOp2 As IAggregableReduceOp(Of Writable, IList(Of Writable)) = reduction.reduceOp()
			For Each w As Writable In list2
				reduceOp2.accept(w)
			Next w

			reduceOp.combine(reduceOp2)

			listOut = reduceOp.get()
			assertEquals(1, listOut.Count)
			wOut = listOut(0)

			split = wOut.ToString().Split(",", True)
			lat = Double.Parse(split(0))
			lng = Double.Parse(split(1))

			assertEquals(expLat, lat, 1e-6)
			assertEquals(expLong, lng, 1e-6)
		End Sub

	End Class

End Namespace