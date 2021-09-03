Imports System.Collections.Generic
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports SequenceSplitTimeSeparation = org.datavec.api.transform.sequence.split.SequenceSplitTimeSeparation
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Text = org.datavec.api.writable.Text
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
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestSequenceSplit extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestSequenceSplit
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceSplitTimeSeparation()
		Public Overridable Sub testSequenceSplitTimeSeparation()

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnTime("time", DateTimeZone.UTC).addColumnString("text").build()

			Dim inputSequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputSequence.Add(New List(Of Writable) From {
				New LongWritable(0),
				New Text("t0")
			})
			inputSequence.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New Text("t1")
			})
			'Second split: 74 seconds later
			inputSequence.Add(New List(Of Writable) From {
				New LongWritable(75000),
				New Text("t2")
			})
			inputSequence.Add(New List(Of Writable) From {
				New LongWritable(100000),
				New Text("t3")
			})
			'Third split: 1 minute and 1 milliseconds later
			inputSequence.Add(New List(Of Writable) From {
				New LongWritable(160001),
				New Text("t4")
			})

			Dim seqSplit As SequenceSplit = New org.datavec.api.transform.sequence.Split.SequenceSplitTimeSeparation("time", 1, TimeUnit.MINUTES)
			seqSplit.InputSchema = schema

			Dim splits As IList(Of IList(Of IList(Of Writable))) = seqSplit.split(inputSequence)
			assertEquals(3, splits.Count)

			Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp0.Add(New List(Of Writable) From {
				New LongWritable(0),
				New Text("t0")
			})
			exp0.Add(New List(Of Writable) From {
				New LongWritable(1000),
				New Text("t1")
			})
			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp1.Add(New List(Of Writable) From {
				New LongWritable(75000),
				New Text("t2")
			})
			exp1.Add(New List(Of Writable) From {
				New LongWritable(100000),
				New Text("t3")
			})
			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp2.Add(New List(Of Writable) From {
				New LongWritable(160001),
				New Text("t4")
			})

			assertEquals(exp0, splits(0))
			assertEquals(exp1, splits(1))
			assertEquals(exp2, splits(2))
		End Sub

	End Class

End Namespace