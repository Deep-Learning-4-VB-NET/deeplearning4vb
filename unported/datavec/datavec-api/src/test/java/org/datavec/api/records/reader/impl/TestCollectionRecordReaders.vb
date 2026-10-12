Imports System.Collections.Generic
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CollectionSequenceRecordReader = org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.junit.jupiter.api.Assertions

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

Namespace org.datavec.api.records.reader.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestCollectionRecordReaders extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestCollectionRecordReaders
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCollectionSequenceRecordReader() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCollectionSequenceRecordReader()

			Dim listOfSequences As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()

			Dim sequence1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence1.Add(New List(Of Writable) From {DirectCast(New IntWritable(0), Writable), New IntWritable(1)})
			sequence1.Add(New List(Of Writable) From {DirectCast(New IntWritable(2), Writable), New IntWritable(3)})
			listOfSequences.Add(sequence1)

			Dim sequence2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence2.Add(New List(Of Writable) From {DirectCast(New IntWritable(4), Writable), New IntWritable(5)})
			sequence2.Add(New List(Of Writable) From {DirectCast(New IntWritable(6), Writable), New IntWritable(7)})
			listOfSequences.Add(sequence2)

			Dim seqRR As SequenceRecordReader = New CollectionSequenceRecordReader(listOfSequences)
			assertTrue(seqRR.hasNext())

			assertEquals(sequence1, seqRR.sequenceRecord())
			assertEquals(sequence2, seqRR.sequenceRecord())
			assertFalse(seqRR.hasNext())

			seqRR.reset()
			assertEquals(sequence1, seqRR.sequenceRecord())
			assertEquals(sequence2, seqRR.sequenceRecord())
			assertFalse(seqRR.hasNext())

			'Test metadata:
			seqRR.reset()
			Dim out2 As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim seq As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()

			Do While seqRR.hasNext()
				Dim r As SequenceRecord = seqRR.nextSequence()
				out2.Add(r.getSequenceRecord())
				seq.Add(r)
				meta.Add(r.MetaData)
			Loop

			assertEquals(listOfSequences, out2)

			Dim fromMeta As IList(Of SequenceRecord) = seqRR.loadSequenceFromMetaData(meta)
			assertEquals(seq, fromMeta)
		End Sub

	End Class

End Namespace