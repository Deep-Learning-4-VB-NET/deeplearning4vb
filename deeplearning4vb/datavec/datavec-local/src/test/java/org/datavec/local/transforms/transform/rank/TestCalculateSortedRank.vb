Imports System.Collections.Generic
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Schema = org.datavec.api.transform.schema.Schema
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports DoubleWritableComparator = org.datavec.api.writable.comparator.DoubleWritableComparator
Imports LocalTransformExecutor = org.datavec.local.transforms.LocalTransformExecutor
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
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

Namespace org.datavec.local.transforms.transform.rank




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class TestCalculateSortedRank
	Public Class TestCalculateSortedRank
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCalculateSortedRank()
		Public Overridable Sub testCalculateSortedRank()

			Dim data As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			data.Add(New List(Of Writable) From {DirectCast(New Text("0"), Writable), New DoubleWritable(0.0)})
			data.Add(New List(Of Writable) From {DirectCast(New Text("3"), Writable), New DoubleWritable(0.3)})
			data.Add(New List(Of Writable) From {DirectCast(New Text("2"), Writable), New DoubleWritable(0.2)})
			data.Add(New List(Of Writable) From {DirectCast(New Text("1"), Writable), New DoubleWritable(0.1)})

			Dim rdd As IList(Of IList(Of Writable)) = (data)

			Dim schema As Schema = (New Schema.Builder()).addColumnsString("TextCol").addColumnDouble("DoubleCol").build()

			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).calculateSortedRank("rank", "DoubleCol", New DoubleWritableComparator()).build()

			Dim outSchema As Schema = tp.FinalSchema
			assertEquals(3, outSchema.numColumns())
			assertEquals(Arrays.asList("TextCol", "DoubleCol", "rank"), outSchema.getColumnNames())
			assertEquals(Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Long), outSchema.getColumnTypes())

			Dim [out] As IList(Of IList(Of Writable)) = LocalTransformExecutor.execute(rdd, tp)

			Dim collected As IList(Of IList(Of Writable)) = [out]
			assertEquals(4, collected.Count)
			For i As Integer = 0 To 3
				assertEquals(3, collected(i).Count)
			Next i

			For Each example As IList(Of Writable) In collected
				Dim exampleNum As Integer = example(0).toInt()
				Dim rank As Integer = example(2).toInt()
				assertEquals(exampleNum, rank)
			Next example
		End Sub

	End Class

End Namespace