Imports System.Collections.Generic
Imports Record = org.datavec.api.records.Record
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CollectionRecordReader = org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports InMemorySequenceRecordReader = org.datavec.api.records.reader.impl.inmemory.InMemorySequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports CategoricalColumnCondition = org.datavec.api.transform.condition.column.CategoricalColumnCondition
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.datavec.local.transforms


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class LocalTransformProcessRecordReaderTests
	Public Class LocalTransformProcessRecordReaderTests
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void simpleTransformTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub simpleTransformTest()
			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("0").addColumnDouble("1").addColumnDouble("2").addColumnDouble("3").addColumnDouble("4").build()
			Dim transformProcess As TransformProcess = (New TransformProcess.Builder(schema)).removeColumns("0").build()
			Dim csvRecordReader As New CSVRecordReader()
			csvRecordReader.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("iris.dat")).File))
			Dim transformProcessRecordReader As New LocalTransformProcessRecordReader(csvRecordReader, transformProcess)
			assertEquals(4, transformProcessRecordReader.next().Count)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void simpleTransformTestSequence()
		Public Overridable Sub simpleTransformTestSequence()
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			'First window:
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L),
				New IntWritable(0),
				New IntWritable(0)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 100L),
				New IntWritable(1),
				New IntWritable(0)
			})
			sequence.Add(New List(Of Writable) From {
				New LongWritable(1451606400000L + 200L),
				New IntWritable(2),
				New IntWritable(0)
			})

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").addColumnInteger("intcolumn2").build()
			Dim transformProcess As TransformProcess = (New TransformProcess.Builder(schema)).removeColumns("intcolumn2").build()
			Dim inMemorySequenceRecordReader As New InMemorySequenceRecordReader(New List(Of IList(Of IList(Of Writable))) From {sequence})
			Dim transformProcessSequenceRecordReader As New LocalTransformProcessSequenceRecordReader(inMemorySequenceRecordReader, transformProcess)
			Dim [next] As IList(Of IList(Of Writable)) = transformProcessSequenceRecordReader.sequenceRecord()
			assertEquals(2, [next](0).Count)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLocalFilter()
		Public Overridable Sub testLocalFilter()

			Dim [in] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			[in].Add(New List(Of Writable) From {
				New Text("Keep"),
				New IntWritable(0)
			})
			[in].Add(New List(Of Writable) From {
				New Text("Remove"),
				New IntWritable(1)
			})
			[in].Add(New List(Of Writable) From {
				New Text("Keep"),
				New IntWritable(2)
			})
			[in].Add(New List(Of Writable) From {
				New Text("Remove"),
				New IntWritable(3)
			})

			Dim s As Schema = (New Schema.Builder()).addColumnCategorical("cat", "Keep", "Remove").addColumnInteger("int").build()

			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).filter(New CategoricalColumnCondition("cat", ConditionOp.Equal, "Remove")).build()

			Dim rr As RecordReader = New CollectionRecordReader([in])
			Dim ltprr As New LocalTransformProcessRecordReader(rr, tp)

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While ltprr.hasNext()
				[out].Add(ltprr.next())
			Loop

			Dim exp As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {[in](0), [in](2)}

			assertEquals(exp, [out])

			'Check reset:
			ltprr.reset()
			[out].Clear()
			Do While ltprr.hasNext()
				[out].Add(ltprr.next())
			Loop
			assertEquals(exp, [out])


			'Also test Record method:
			Dim rl As IList(Of Record) = New List(Of Record)()
			rr.reset()
			Do While rr.hasNext()
				rl.Add(rr.nextRecord())
			Loop
			Dim exp2 As IList(Of Record) = New List(Of Record) From {rl(0), rl(2)}

			Dim act As IList(Of Record) = New List(Of Record)()
			ltprr.reset()
			Do While ltprr.hasNext()
				act.Add(ltprr.nextRecord())
			Loop
		End Sub

	End Class

End Namespace