Imports System.Collections.Generic
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports InMemorySequenceRecordReader = org.datavec.api.records.reader.impl.inmemory.InMemorySequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.datavec.api.records.reader.impl.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TransformProcessRecordReaderTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class TransformProcessRecordReaderTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void simpleTransformTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub simpleTransformTest()
			Dim schema As Schema = (New Schema.Builder()).addColumnsDouble("%d", 0, 4).build()
			Dim transformProcess As TransformProcess = (New TransformProcess.Builder(schema)).removeColumns("0").build()
			Dim csvRecordReader As New CSVRecordReader()
			csvRecordReader.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim rr As New TransformProcessRecordReader(csvRecordReader, transformProcess)
			Dim count As Integer = 0
			Dim all As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				Dim [next] As IList(Of Writable) = rr.next()
				assertEquals(4, [next].Count)
				count += 1
				all.Add([next])
			Loop
			assertEquals(150, count)

			'Test batch:
			assertTrue(rr.resetSupported())
			rr.reset()
			Dim batch As IList(Of IList(Of Writable)) = rr.next(150)
			assertEquals(all, batch)
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
			Dim transformProcessSequenceRecordReader As New TransformProcessSequenceRecordReader(inMemorySequenceRecordReader, transformProcess)
			Dim [next] As IList(Of IList(Of Writable)) = transformProcessSequenceRecordReader.sequenceRecord()
			assertEquals(2, [next](0).Count)

		End Sub
	End Class

End Namespace