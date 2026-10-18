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

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource
Imports CollectionRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports ConditionOpT = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports CSVRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DoubleColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports FileSplitT = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InMemorySequenceRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.inmemory.InMemorySequenceRecordReader
Imports RecordMetaDataT = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordT = DeepLearning4VB.org.datavec.api.records.Record
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceRecordT = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TransformProcessRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.transform.TransformProcessRecordReader
Imports TransformProcessSequenceRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.transform.TransformProcessSequenceRecordReader
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl.transform

    ''' <summary>
    ''' Port of the DataVec <c>TransformProcessRecordReaderTests</c> JUnit class, plus tests of filtering, metadata and
    ''' the sequence reader's other members.
    ''' </summary>
    <TestClass>
    Public Class TransformProcessRecordReaderTests

        <TestMethod>
        Public Sub simpleTransformTest()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsDouble("%d", 0, 4).build()
            Dim transformProcess As TransformProcessT = (New TransformProcessT.Builder(schema)).removeColumns("0").build()
            Dim csvRecordReader As New CSVRecordReaderT()
            csvRecordReader.initialize(New FileSplitT((New ClassPathResource("datavec-api/iris.dat")).File))
            Dim rr As New TransformProcessRecordReaderT(csvRecordReader, transformProcess)
            Dim count As Integer = 0
            Dim all As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Do While rr.hasNext()
                Dim [next] As IList(Of WritableT) = rr.next()
                assertEquals(4, [next].Count)
                count += 1
                all.Add([next])
            Loop
            assertEquals(150, count)

            'Test batch:
            assertTrue(rr.resetSupported())
            rr.reset()
            Dim batch As IList(Of IList(Of WritableT)) = rr.next(150)
            assertEquals(all, batch)
        End Sub

        <TestMethod>
        Public Sub simpleTransformTestSequence()
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            'First window:
            sequence.Add(New List(Of WritableT) From {New LongWritable(1451606400000L), New IntWritable(0), New IntWritable(0)})
            sequence.Add(New List(Of WritableT) From {New LongWritable(1451606400000L + 100L), New IntWritable(1), New IntWritable(0)})
            sequence.Add(New List(Of WritableT) From {New LongWritable(1451606400000L + 200L), New IntWritable(2), New IntWritable(0)})

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").addColumnInteger("intcolumn2").build()
            Dim transformProcess As TransformProcessT = (New TransformProcessT.Builder(schema)).removeColumns("intcolumn2").build()
            Dim inMemorySequenceRecordReader As New InMemorySequenceRecordReaderT(New List(Of IList(Of IList(Of WritableT))) From {sequence})
            Dim transformProcessSequenceRecordReader As New TransformProcessSequenceRecordReaderT(inMemorySequenceRecordReader, transformProcess)
            Dim [next] As IList(Of IList(Of WritableT)) = transformProcessSequenceRecordReader.sequenceRecord()
            assertEquals(2, [next](0).Count)
        End Sub

        <TestMethod>
        Public Sub filteredRecordsAreSkipped()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("x").addColumnDouble("y").build()
            ' remove the examples with x < 0
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).filter(New DoubleColumnConditionT("x", ConditionOpT.LessThan, 0.0)).removeColumns("y").build()
            Dim data As New List(Of IList(Of WritableT)) From {
                New List(Of WritableT) From {New DoubleWritable(-1), New DoubleWritable(10)},
                New List(Of WritableT) From {New DoubleWritable(1), New DoubleWritable(11)},
                New List(Of WritableT) From {New DoubleWritable(-2), New DoubleWritable(12)},
                New List(Of WritableT) From {New DoubleWritable(-3), New DoubleWritable(13)},
                New List(Of WritableT) From {New DoubleWritable(2), New DoubleWritable(14)}
            }
            Dim rr As New TransformProcessRecordReaderT(New CollectionRecordReaderT(data), tp)
            assertTrue(rr.batchesSupported())
            Dim first As RecordT = rr.nextRecord()
            assertEquals(New List(Of WritableT) From {New DoubleWritable(1)}, first.getRecord())
            assertTrue(rr.hasNext())
            assertEquals(New List(Of WritableT) From {New DoubleWritable(2)}, rr.next())
            assertFalse(rr.hasNext())
            assertThrows(Of InvalidOperationException)(Sub() rr.next())

            rr.reset()
            assertEquals(New List(Of IList(Of WritableT)) From {New List(Of WritableT) From {New DoubleWritable(1)}, New List(Of WritableT) From {New DoubleWritable(2)}}, rr.next(10))
            rr.Dispose()
        End Sub

        <TestMethod>
        Public Sub sequenceReaderMembers()
            Dim seq1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                New List(Of WritableT) From {New IntWritable(1), New IntWritable(10)},
                New List(Of WritableT) From {New IntWritable(2), New IntWritable(20)}
            }
            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                New List(Of WritableT) From {New IntWritable(3), New IntWritable(30)}
            }
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("a").addColumnInteger("b").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).removeColumns("b").build()
            Dim rr As New TransformProcessSequenceRecordReaderT(New InMemorySequenceRecordReaderT(New List(Of IList(Of IList(Of WritableT))) From {seq1, seq2}), tp)

            assertFalse(rr.batchesSupported())
            assertTrue(rr.hasNext())
            Dim r As SequenceRecordT = rr.nextSequence()
            assertEquals(New List(Of IList(Of WritableT)) From {New List(Of WritableT) From {New IntWritable(1)}, New List(Of WritableT) From {New IntWritable(2)}}, r.getSequenceRecord())
            assertEquals(New List(Of IList(Of WritableT)) From {New List(Of WritableT) From {New IntWritable(3)}}, rr.sequenceRecord())
            assertFalse(rr.hasNext())
            assertTrue(rr.resetSupported())
            rr.reset()
            assertTrue(rr.hasNext())
            Assert.IsNull(rr.loadSequenceFromMetaData(New List(Of RecordMetaDataT)()))
            assertThrows(Of NotSupportedException)(Sub() rr.next(2))
        End Sub

    End Class

End Namespace
