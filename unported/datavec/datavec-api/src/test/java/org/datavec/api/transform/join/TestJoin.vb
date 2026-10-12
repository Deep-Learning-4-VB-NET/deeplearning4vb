Imports System.Collections.Generic
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Schema = org.datavec.api.transform.schema.Schema
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NullWritable = org.datavec.api.writable.NullWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertThrows

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

Namespace org.datavec.api.transform.join


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestJoin extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestJoin
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJoin(@TempDir Path testDir)
		Public Overridable Sub testJoin(ByVal testDir As Path)

			Dim firstSchema As Schema = (New Schema.Builder()).addColumnString("keyColumn").addColumnsInteger("first0", "first1").build()

			Dim secondSchema As Schema = (New Schema.Builder()).addColumnString("keyColumn").addColumnsInteger("second0").build()

			Dim first As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			first.Add(New List(Of Writable) From {
				New Text("key0"),
				New IntWritable(0),
				New IntWritable(1)
			})
			first.Add(New List(Of Writable) From {
				New Text("key1"),
				New IntWritable(10),
				New IntWritable(11)
			})

			Dim second As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			second.Add(New List(Of Writable) From {
				New Text("key0"),
				New IntWritable(100)
			})
			second.Add(New List(Of Writable) From {
				New Text("key1"),
				New IntWritable(110)
			})

			Dim join As Join = (New Join.Builder(Join.JoinType.Inner)).setJoinColumns("keyColumn").setSchemas(firstSchema, secondSchema).build()

			Dim expected As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {
				New Text("key0"),
				New IntWritable(0),
				New IntWritable(1),
				New IntWritable(100)
			})
			expected.Add(New List(Of Writable) From {
				New Text("key1"),
				New IntWritable(10),
				New IntWritable(11),
				New IntWritable(110)
			})


			'Check schema:
			Dim joinedSchema As Schema = join.OutputSchema
			assertEquals(4, joinedSchema.numColumns())
			assertEquals(Arrays.asList("keyColumn", "first0", "first1", "second0"), joinedSchema.getColumnNames())
			assertEquals(Arrays.asList(ColumnType.String, ColumnType.Integer, ColumnType.Integer, ColumnType.Integer), joinedSchema.getColumnTypes())


			'Check joining with null values:
			expected = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {DirectCast(New Text("key0"), Writable), New IntWritable(0), New IntWritable(1), NullWritable.INSTANCE})
			expected.Add(New List(Of Writable) From {DirectCast(New Text("key1"), Writable), New IntWritable(10), New IntWritable(11), NullWritable.INSTANCE})
			For i As Integer = 0 To first.Count - 1
				Dim [out] As IList(Of Writable) = join.joinExamples(first(i), Nothing)
				assertEquals(expected(i), [out])
			Next i

			expected = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {DirectCast(New Text("key0"), Writable), NullWritable.INSTANCE, NullWritable.INSTANCE, New IntWritable(100)})
			expected.Add(New List(Of Writable) From {DirectCast(New Text("key1"), Writable), NullWritable.INSTANCE, NullWritable.INSTANCE, New IntWritable(110)})
			For i As Integer = 0 To first.Count - 1
				Dim [out] As IList(Of Writable) = join.joinExamples(Nothing, second(i))
				assertEquals(expected(i), [out])
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testJoinValidation()
		Public Overridable Sub testJoinValidation()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim firstSchema As Schema = (New Schema.Builder()).addColumnString("keyColumn1").addColumnsInteger("first0", "first1").build()
			Dim secondSchema As Schema = (New Schema.Builder()).addColumnString("keyColumn2").addColumnsInteger("second0").build()
			Call (New Join.Builder(Join.JoinType.Inner)).setJoinColumns("keyColumn1", "thisDoesntExist").setSchemas(firstSchema, secondSchema).build()
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testJoinValidation2()
		Public Overridable Sub testJoinValidation2()
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim firstSchema As Schema = (New Schema.Builder()).addColumnString("keyColumn1").addColumnsInteger("first0", "first1").build()
		   Dim secondSchema As Schema = (New Schema.Builder()).addColumnString("keyColumn2").addColumnsInteger("second0").build()
		   Call (New Join.Builder(Join.JoinType.Inner)).setJoinColumns("keyColumn1").setSchemas(firstSchema, secondSchema).build()
		   End Sub)

		End Sub
	End Class

End Namespace