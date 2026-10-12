Imports System.Collections.Generic
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports Transform = org.datavec.api.transform.Transform
Imports Reducer = org.datavec.api.transform.reduce.Reducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports ReduceSequenceByWindowTransform = org.datavec.api.transform.sequence.window.ReduceSequenceByWindowTransform
Imports TimeWindowFunction = org.datavec.api.transform.sequence.window.TimeWindowFunction
Imports WindowFunction = org.datavec.api.transform.sequence.window.WindowFunction
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports NullWritable = org.datavec.api.writable.NullWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
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

Namespace org.datavec.api.transform.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestReduceSequenceByWindowFunction extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestReduceSequenceByWindowFunction
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReduceSequenceByWindowFunction()
		Public Overridable Sub testReduceSequenceByWindowFunction()
			'Time windowing: 1 second (1000 milliseconds) window

			'Create some data.
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'First window:
			sequence.Add(New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L), Writable), New IntWritable(0)})
			sequence.Add(New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L + 100L), Writable), New IntWritable(1)})
			sequence.Add(New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L + 200L), Writable), New IntWritable(2)})
			'Second window:
			sequence.Add(New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L + 1000L), Writable), New IntWritable(3)})
			'Third window: empty
			'Fourth window:
			sequence.Add(New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L + 3000L), Writable), New IntWritable(4)})
			sequence.Add(New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L + 3100L), Writable), New IntWritable(5)})

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()

			Dim wf As WindowFunction = New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
			wf.InputSchema = schema


			'Now: reduce by summing...
			Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).takeFirstColumns("timecolumn").build()

			Dim transform As Transform = New ReduceSequenceByWindowTransform(reducer, wf)
			transform.InputSchema = schema

			Dim postApply As IList(Of IList(Of Writable)) = transform.mapSequence(sequence)
			assertEquals(4, postApply.Count)


			Dim exp0 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L), Writable), New IntWritable(0 + 1 + 2)}
			assertEquals(exp0, postApply(0))

			Dim exp1 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L + 1000L), Writable), New IntWritable(3)}
			assertEquals(exp1, postApply(1))

			' here, takefirst of an empty window -> nullwritable makes more sense
			Dim exp2 As IList(Of Writable) = New List(Of Writable) From {DirectCast(NullWritable.INSTANCE, Writable), NullWritable.INSTANCE}
			assertEquals(exp2, postApply(2))

			Dim exp3 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New LongWritable(1451606400000L + 3000L), Writable), New IntWritable(9)}
			assertEquals(exp3, postApply(3))
		End Sub

	End Class

End Namespace