Imports System.Collections.Generic
Imports Condition = org.datavec.api.transform.condition.Condition
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports IntegerColumnCondition = org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports Schema = org.datavec.api.transform.schema.Schema
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertFalse
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

Namespace org.datavec.api.transform.filter


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestFilters extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestFilters
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFilterNumColumns()
		Public Overridable Sub testFilterNumColumns()
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			list.Add(Collections.singletonList(DirectCast(New IntWritable(-1), Writable)))
			list.Add(Collections.singletonList(DirectCast(New IntWritable(0), Writable)))
			list.Add(Collections.singletonList(DirectCast(New IntWritable(2), Writable)))

			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("intCol", 0, 10).addColumnDouble("doubleCol", -100.0, 100.0).build()
			Dim numColumns As Filter = New InvalidNumColumns(schema)
			For i As Integer = 0 To list.Count - 1
				assertTrue(numColumns.removeExample(list(i)))
			Next i

			Dim correct As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			assertFalse(numColumns.removeExample(correct))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFilterInvalidValues()
		Public Overridable Sub testFilterInvalidValues()

			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			list.Add(Collections.singletonList(DirectCast(New IntWritable(-1), Writable)))
			list.Add(Collections.singletonList(DirectCast(New IntWritable(0), Writable)))
			list.Add(Collections.singletonList(DirectCast(New IntWritable(2), Writable)))

			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("intCol", 0, 10).addColumnDouble("doubleCol", -100.0, 100.0).build()

			Dim filter As Filter = New FilterInvalidValues("intCol", "doubleCol")
			filter.InputSchema = schema

			'Test valid examples:
			assertFalse(filter.removeExample(asList(DirectCast(New IntWritable(0), Writable), New DoubleWritable(0))))
			assertFalse(filter.removeExample(asList(DirectCast(New IntWritable(10), Writable), New DoubleWritable(0))))
			assertFalse(filter.removeExample(asList(DirectCast(New IntWritable(0), Writable), New DoubleWritable(-100))))
			assertFalse(filter.removeExample(asList(DirectCast(New IntWritable(0), Writable), New DoubleWritable(100))))

			'Test invalid:
			assertTrue(filter.removeExample(asList(DirectCast(New IntWritable(-1), Writable), New DoubleWritable(0))))
			assertTrue(filter.removeExample(asList(DirectCast(New IntWritable(11), Writable), New DoubleWritable(0))))
			assertTrue(filter.removeExample(asList(DirectCast(New IntWritable(0), Writable), New DoubleWritable(-101))))
			assertTrue(filter.removeExample(asList(DirectCast(New IntWritable(0), Writable), New DoubleWritable(101))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConditionFilter()
		Public Overridable Sub testConditionFilter()
			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("column").build()

			Dim condition As Condition = New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
			condition.InputSchema = schema

			Dim filter As Filter = New ConditionFilter(condition)

			assertFalse(filter.removeExample(Collections.singletonList(DirectCast(New IntWritable(10), Writable))))
			assertFalse(filter.removeExample(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
			assertFalse(filter.removeExample(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertTrue(filter.removeExample(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertTrue(filter.removeExample(Collections.singletonList(DirectCast(New IntWritable(-10), Writable))))
		End Sub

	End Class

End Namespace