Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BufferAllocator = org.apache.arrow.memory.BufferAllocator
Imports RootAllocator = org.apache.arrow.memory.RootAllocator
Imports FieldVector = org.apache.arrow.vector.FieldVector
Imports TimeStampMilliVector = org.apache.arrow.vector.TimeStampMilliVector
Imports VectorSchemaRoot = org.apache.arrow.vector.VectorSchemaRoot
Imports VectorUnloader = org.apache.arrow.vector.VectorUnloader
Imports ArrowFileWriter = org.apache.arrow.vector.ipc.ArrowFileWriter
Imports FloatingPointPrecision = org.apache.arrow.vector.types.FloatingPointPrecision
Imports ArrowType = org.apache.arrow.vector.types.pojo.ArrowType
Imports Field = org.apache.arrow.vector.types.pojo.Field
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = org.datavec.api.records.metadata.RecordMetaDataIndex
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Schema = org.datavec.api.transform.schema.Schema
Imports org.datavec.api.writable
Imports ArrowRecordReader = org.datavec.arrow.recordreader.ArrowRecordReader
Imports ArrowWritableRecordBatch = org.datavec.arrow.recordreader.ArrowWritableRecordBatch
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.datavec.arrow

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Arrow Converter Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class ArrowConverterTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class ArrowConverterTest
		Inherits BaseND4JTest

		Private Shared bufferAllocator As BufferAllocator = New RootAllocator(Long.MaxValue)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test To Array From IND Array") void testToArrayFromINDArray()
		Friend Overridable Sub testToArrayFromINDArray()
			Dim schemaBuilder As New Schema.Builder()
			schemaBuilder.addColumnNDArray("outputArray", New Long() { 1, 4 })
			Dim schema As Schema = schemaBuilder.build()
			Dim numRows As Integer = 4
			Dim ret As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(numRows)
			For i As Integer = 0 To numRows - 1
				ret.Add(New List(Of Writable) From {Of Writable})
			Next i
			Dim fieldVectors As IList(Of FieldVector) = ArrowConverter.toArrowColumns(bufferAllocator, schema, ret)
			Dim arrowWritableRecordBatch As New ArrowWritableRecordBatch(fieldVectors, schema)
			Dim array As INDArray = ArrowConverter.toArray(arrowWritableRecordBatch)
			assertArrayEquals(New Long() { 4, 4 }, array.shape())
			Dim assertion As INDArray = Nd4j.repeat(Nd4j.linspace(1, 4, 4), 4).reshape(ChrW(4), 4)
			assertEquals(assertion, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Arrow Column IND Array") void testArrowColumnINDArray()
		Friend Overridable Sub testArrowColumnINDArray()
			Dim schema As New Schema.Builder()
			Dim [single] As IList(Of String) = New List(Of String)()
			Dim numCols As Integer = 2
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4)
			For i As Integer = 0 To numCols - 1
				schema.addColumnNDArray(i.ToString(), New Long() { 1, 4 })
				[single].Add(i.ToString())
			Next i
			Dim buildSchema As Schema = schema.build()
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim firstRow As IList(Of Writable) = New List(Of Writable)()
			For i As Integer = 0 To numCols - 1
				firstRow.Add(New NDArrayWritable(arr))
			Next i
			list.Add(firstRow)
			Dim fieldVectors As IList(Of FieldVector) = ArrowConverter.toArrowColumns(bufferAllocator, buildSchema, list)
			assertEquals(numCols, fieldVectors.Count)
			assertEquals(1, fieldVectors(0).getValueCount())
			assertFalse(fieldVectors(0).isNull(0))
			Dim arrowWritableRecordBatch As ArrowWritableRecordBatch = ArrowConverter.toArrowWritables(fieldVectors, buildSchema)
			assertEquals(1, arrowWritableRecordBatch.Count)
			Dim writable As Writable = arrowWritableRecordBatch(0).get(0)
			assertTrue(TypeOf writable Is NDArrayWritable)
			Dim ndArrayWritable As NDArrayWritable = DirectCast(writable, NDArrayWritable)
			assertEquals(arr, ndArrayWritable.get())
			Dim writable1 As Writable = ArrowConverter.fromEntry(0, fieldVectors(0), ColumnType.NDArray)
			Dim ndArrayWritablewritable1 As NDArrayWritable = DirectCast(writable1, NDArrayWritable)
			Console.WriteLine(ndArrayWritablewritable1.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Arrow Column String") void testArrowColumnString()
		Friend Overridable Sub testArrowColumnString()
			Dim schema As New Schema.Builder()
			Dim [single] As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 1
				schema.addColumnInteger(i.ToString())
				[single].Add(i.ToString())
			Next i
			Dim fieldVectors As IList(Of FieldVector) = ArrowConverter.toArrowColumnsStringSingle(bufferAllocator, schema.build(), [single])
			Dim records As IList(Of IList(Of Writable)) = ArrowConverter.toArrowWritables(fieldVectors, schema.build())
			Dim assertion As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			assertion.Add(New List(Of Writable) From {Of Writable})
			assertEquals(assertion, records)
			Dim batch As IList(Of IList(Of String)) = New List(Of IList(Of String))()
			For i As Integer = 0 To 1
				batch.Add(New List(Of String) From {i.ToString(), i.ToString()})
			Next i
			Dim fieldVectorsBatch As IList(Of FieldVector) = ArrowConverter.toArrowColumnsString(bufferAllocator, schema.build(), batch)
			Dim batchRecords As IList(Of IList(Of Writable)) = ArrowConverter.toArrowWritables(fieldVectorsBatch, schema.build())
			Dim assertionBatch As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			assertionBatch.Add(New List(Of Writable) From {
				New IntWritable(0),
				New IntWritable(0)
			})
			assertionBatch.Add(New List(Of Writable) From {
				New IntWritable(1),
				New IntWritable(1)
			})
			assertEquals(assertionBatch, batchRecords)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Arrow Batch Set Time") void testArrowBatchSetTime()
		Friend Overridable Sub testArrowBatchSetTime()
			Dim schema As New Schema.Builder()
			Dim [single] As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 1
				schema.addColumnTime(i.ToString(), TimeZone.getDefault())
				[single].Add(i.ToString())
			Next i
			Dim input As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New LongWritable(0), New LongWritable(1)), Arrays.asList(Of Writable)(New LongWritable(2), New LongWritable(3))}
			Dim fieldVector As IList(Of FieldVector) = ArrowConverter.toArrowColumns(bufferAllocator, schema.build(), input)
			Dim writableRecordBatch As New ArrowWritableRecordBatch(fieldVector, schema.build())
			Dim assertion As IList(Of Writable) = New List(Of Writable) From {
				New LongWritable(4),
				New LongWritable(5)
			}
			writableRecordBatch(1) = java.util.Arrays.asList(New LongWritable(4), New LongWritable(5))
			Dim recordTest As IList(Of Writable) = writableRecordBatch(1)
			assertEquals(assertion, recordTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Arrow Batch Set") void testArrowBatchSet()
		Friend Overridable Sub testArrowBatchSet()
			Dim schema As New Schema.Builder()
			Dim [single] As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To 1
				schema.addColumnInteger(i.ToString())
				[single].Add(i.ToString())
			Next i
			Dim input As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New IntWritable(0), New IntWritable(1)), Arrays.asList(Of Writable)(New IntWritable(2), New IntWritable(3))}
			Dim fieldVector As IList(Of FieldVector) = ArrowConverter.toArrowColumns(bufferAllocator, schema.build(), input)
			Dim writableRecordBatch As New ArrowWritableRecordBatch(fieldVector, schema.build())
			Dim assertion As IList(Of Writable) = New List(Of Writable) From {
				New IntWritable(4),
				New IntWritable(5)
			}
			writableRecordBatch(1) = java.util.Arrays.asList(New IntWritable(4), New IntWritable(5))
			Dim recordTest As IList(Of Writable) = writableRecordBatch(1)
			assertEquals(assertion, recordTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Arrow Columns String Time Series") void testArrowColumnsStringTimeSeries()
		Friend Overridable Sub testArrowColumnsStringTimeSeries()
			Dim schema As New Schema.Builder()
			Dim entries As IList(Of IList(Of IList(Of String))) = New List(Of IList(Of IList(Of String)))()
			For i As Integer = 0 To 2
				schema.addColumnInteger(i.ToString())
			Next i
			For i As Integer = 0 To 4
				Dim arr As IList(Of IList(Of String)) = New List(Of IList(Of String)) From {Arrays.asList(i.ToString(), i.ToString(), i.ToString())}
				entries.Add(arr)
			Next i
			Dim fieldVectors As IList(Of FieldVector) = ArrowConverter.toArrowColumnsStringTimeSeries(bufferAllocator, schema.build(), entries)
			assertEquals(3, fieldVectors.Count)
			assertEquals(5, fieldVectors(0).getValueCount())
			Dim exp As INDArray = Nd4j.create(5, 3)
			For i As Integer = 0 To 4
				exp.getRow(i).assign(i)
			Next i
			' Convert to ArrowWritableRecordBatch - note we can't do this in general with time series...
			Dim wri As ArrowWritableRecordBatch = ArrowConverter.toArrowWritables(fieldVectors, schema.build())
			Dim arr As INDArray = ArrowConverter.toArray(wri)
			assertArrayEquals(New Long() { 5, 3 }, arr.shape())
			assertEquals(exp, arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convert Vector") void testConvertVector()
		Friend Overridable Sub testConvertVector()
			Dim schema As New Schema.Builder()
			Dim entries As IList(Of IList(Of IList(Of String))) = New List(Of IList(Of IList(Of String)))()
			For i As Integer = 0 To 2
				schema.addColumnInteger(i.ToString())
			Next i
			For i As Integer = 0 To 4
				Dim arr As IList(Of IList(Of String)) = New List(Of IList(Of String)) From {Arrays.asList(i.ToString(), i.ToString(), i.ToString())}
				entries.Add(arr)
			Next i
			Dim fieldVectors As IList(Of FieldVector) = ArrowConverter.toArrowColumnsStringTimeSeries(bufferAllocator, schema.build(), entries)
			Dim arr As INDArray = ArrowConverter.convertArrowVector(fieldVectors(0), schema.build().getType(0))
			assertEquals(5, arr.length())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Create ND Array") void testCreateNDArray() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCreateNDArray()
			Dim recordsToWrite As val = recordToWrite()
			Dim byteArrayOutputStream As New MemoryStream()
			ArrowConverter.writeRecordBatchTo(recordsToWrite.getRight(), recordsToWrite.getFirst(), byteArrayOutputStream)
			Dim f As File = testDir.toFile()
			Dim tmpFile As New File(f, "tmp-arrow-file-" & System.Guid.randomUUID().ToString() & ".arrorw")
			Dim outputStream As New FileStream(tmpFile, FileMode.Create, FileAccess.Write)
			tmpFile.deleteOnExit()
			ArrowConverter.writeRecordBatchTo(recordsToWrite.getRight(), recordsToWrite.getFirst(), outputStream)
			outputStream.Flush()
			outputStream.Close()
			Dim schemaArrowWritableRecordBatchPair As Pair(Of Schema, ArrowWritableRecordBatch) = ArrowConverter.readFromFile(tmpFile)
			assertEquals(recordsToWrite.getFirst(), schemaArrowWritableRecordBatchPair.First)
			assertEquals(recordsToWrite.getRight(), schemaArrowWritableRecordBatchPair.Right.toArrayList())
			Dim arr() As SByte = byteArrayOutputStream.toByteArray()
			Dim read As val = ArrowConverter.readFromBytes(arr)
			assertEquals(recordsToWrite, read)
			' send file
			Dim tmp As File = tmpDataFile(recordsToWrite)
			Dim recordReader As New ArrowRecordReader()
			recordReader.initialize(New org.datavec.api.Split.FileSplit(tmp))
			recordReader.next()
			Dim currentBatch As ArrowWritableRecordBatch = recordReader.getCurrentBatch()
			Dim arr2 As INDArray = ArrowConverter.toArray(currentBatch)
			assertEquals(2, arr2.rows())
			assertEquals(2, arr2.columns())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convert To Arrow Vectors") void testConvertToArrowVectors()
		Friend Overridable Sub testConvertToArrowVectors()
			Dim matrix As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim vectors As val = ArrowConverter.convertToArrowVector(matrix, New List(Of String) From {"test", "test2"}, ColumnType.Double, bufferAllocator)
			assertEquals(matrix.rows(), vectors.size())
			Dim vector As INDArray = Nd4j.linspace(1, 4, 4)
			Dim vectors2 As val = ArrowConverter.convertToArrowVector(vector, New List(Of String) From {"test"}, ColumnType.Double, bufferAllocator)
			assertEquals(1, vectors2.size())
			assertEquals(matrix.length(), vectors2.get(0).getValueCount())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Schema Conversion Basic") void testSchemaConversionBasic()
		Friend Overridable Sub testSchemaConversionBasic()
			Dim schemaBuilder As New Schema.Builder()
			For i As Integer = 0 To 1
				schemaBuilder.addColumnDouble("test-" & i)
				schemaBuilder.addColumnInteger("testi-" & i)
				schemaBuilder.addColumnLong("testl-" & i)
				schemaBuilder.addColumnFloat("testf-" & i)
			Next i
			Dim schema As Schema = schemaBuilder.build()
			Dim schema2 As val = ArrowConverter.toArrowSchema(schema)
			assertEquals(8, schema2.getFields().size())
			Dim convertedSchema As val = ArrowConverter.toDatavecSchema(schema2)
			assertEquals(schema, convertedSchema)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Read Schema And Records From Byte Array") void testReadSchemaAndRecordsFromByteArray() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReadSchemaAndRecordsFromByteArray()
			Dim allocator As BufferAllocator = New RootAllocator(Long.MaxValue)
			Dim valueCount As Integer = 3
			Dim fields As IList(Of Field) = New List(Of Field)()
			fields.Add(ArrowConverter.field("field1", New ArrowType.FloatingPoint(FloatingPointPrecision.SINGLE)))
			fields.Add(ArrowConverter.intField("field2"))
			Dim fieldVectors As IList(Of FieldVector) = New List(Of FieldVector)()
			fieldVectors.Add(ArrowConverter.vectorFor(allocator, "field1", New Single() { 1, 2, 3 }))
			fieldVectors.Add(ArrowConverter.vectorFor(allocator, "field2", New Integer() { 1, 2, 3 }))
			Dim schema As New org.apache.arrow.vector.types.pojo.Schema(fields)
			Dim schemaRoot1 As New VectorSchemaRoot(schema, fieldVectors, valueCount)
			Dim vectorUnloader As New VectorUnloader(schemaRoot1)
			vectorUnloader.getRecordBatch()
			Dim byteArrayOutputStream As New MemoryStream()
			Try
					Using arrowFileWriter As New ArrowFileWriter(schemaRoot1, Nothing, newChannel(byteArrayOutputStream))
					arrowFileWriter.writeBatch()
					End Using
			Catch e As IOException
				log.error("", e)
			End Try
			Dim arr() As SByte = byteArrayOutputStream.toByteArray()
			Dim arr2 As val = ArrowConverter.readFromBytes(arr)
			assertEquals(2, arr2.getFirst().numColumns())
			assertEquals(3, arr2.getRight().size())
			Dim arrowCols As val = ArrowConverter.toArrowColumns(allocator, arr2.getFirst(), arr2.getRight())
			assertEquals(2, arrowCols.size())
			assertEquals(valueCount, arrowCols.get(0).getValueCount())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Vector For Edge Cases") void testVectorForEdgeCases()
		Friend Overridable Sub testVectorForEdgeCases()
			Dim allocator As BufferAllocator = New RootAllocator(Long.MaxValue)
			Dim vector As val = ArrowConverter.vectorFor(allocator, "field1", New Single() { Single.Epsilon, Single.MaxValue })
			assertEquals(Single.Epsilon, vector.get(0), 1e-2)
			assertEquals(Single.MaxValue, vector.get(1), 1e-2)
			Dim vectorInt As val = ArrowConverter.vectorFor(allocator, "field1", New Integer() { Integer.MinValue, Integer.MaxValue })
			assertEquals(Integer.MinValue, vectorInt.get(0), 1e-2)
			assertEquals(Integer.MaxValue, vectorInt.get(1), 1e-2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Vector For") void testVectorFor()
		Friend Overridable Sub testVectorFor()
			Dim allocator As BufferAllocator = New RootAllocator(Long.MaxValue)
			Dim vector As val = ArrowConverter.vectorFor(allocator, "field1", New Single() { 1, 2, 3 })
			assertEquals(3, vector.getValueCount())
			assertEquals(1, vector.get(0), 1e-2)
			assertEquals(2, vector.get(1), 1e-2)
			assertEquals(3, vector.get(2), 1e-2)
			Dim vectorLong As val = ArrowConverter.vectorFor(allocator, "field1", New Long() { 1, 2, 3 })
			assertEquals(3, vectorLong.getValueCount())
			assertEquals(1, vectorLong.get(0), 1e-2)
			assertEquals(2, vectorLong.get(1), 1e-2)
			assertEquals(3, vectorLong.get(2), 1e-2)
			Dim vectorInt As val = ArrowConverter.vectorFor(allocator, "field1", New Integer() { 1, 2, 3 })
			assertEquals(3, vectorInt.getValueCount())
			assertEquals(1, vectorInt.get(0), 1e-2)
			assertEquals(2, vectorInt.get(1), 1e-2)
			assertEquals(3, vectorInt.get(2), 1e-2)
			Dim vectorDouble As val = ArrowConverter.vectorFor(allocator, "field1", New Double() { 1, 2, 3 })
			assertEquals(3, vectorDouble.getValueCount())
			assertEquals(1, vectorDouble.get(0), 1e-2)
			assertEquals(2, vectorDouble.get(1), 1e-2)
			assertEquals(3, vectorDouble.get(2), 1e-2)
			Dim vectorBool As val = ArrowConverter.vectorFor(allocator, "field1", New Boolean() { True, True, False })
			assertEquals(3, vectorBool.getValueCount())
			assertEquals(1, vectorBool.get(0), 1e-2)
			assertEquals(1, vectorBool.get(1), 1e-2)
			assertEquals(0, vectorBool.get(2), 1e-2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Record Reader And Write File") void testRecordReaderAndWriteFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRecordReaderAndWriteFile()
			Dim recordsToWrite As val = recordToWrite()
			Dim byteArrayOutputStream As New MemoryStream()
			ArrowConverter.writeRecordBatchTo(recordsToWrite.getRight(), recordsToWrite.getFirst(), byteArrayOutputStream)
			Dim arr() As SByte = byteArrayOutputStream.toByteArray()
			Dim read As val = ArrowConverter.readFromBytes(arr)
			assertEquals(recordsToWrite, read)
			' send file
			Dim tmp As File = tmpDataFile(recordsToWrite)
			Dim recordReader As RecordReader = New ArrowRecordReader()
			recordReader.initialize(New org.datavec.api.Split.FileSplit(tmp))
			Dim record As IList(Of Writable) = recordReader.next()
			assertEquals(2, record.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Record Reader Meta Data List") void testRecordReaderMetaDataList() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRecordReaderMetaDataList()
			Dim recordsToWrite As val = recordToWrite()
			' send file
			Dim tmp As File = tmpDataFile(recordsToWrite)
			Dim recordReader As RecordReader = New ArrowRecordReader()
			Dim recordMetaDataIndex As New RecordMetaDataIndex(0, tmp.toURI(), GetType(ArrowRecordReader))
			recordReader.loadFromMetaData(java.util.Arrays.asList(Of RecordMetaData)(recordMetaDataIndex))
			Dim record As Record = recordReader.nextRecord()
			assertEquals(2, record.getRecord().Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dates") void testDates()
		Friend Overridable Sub testDates()
			Dim now As DateTime = DateTime.Now
			Dim bufferAllocator As BufferAllocator = New RootAllocator(Long.MaxValue)
			Dim timeStampMilliVector As TimeStampMilliVector = ArrowConverter.vectorFor(bufferAllocator, "col1", New DateTime() { now })
			assertEquals(now.Ticks, timeStampMilliVector.get(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Record Reader Meta Data") void testRecordReaderMetaData() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRecordReaderMetaData()
			Dim recordsToWrite As val = recordToWrite()
			' send file
			Dim tmp As File = tmpDataFile(recordsToWrite)
			Dim recordReader As RecordReader = New ArrowRecordReader()
			Dim recordMetaDataIndex As New RecordMetaDataIndex(0, tmp.toURI(), GetType(ArrowRecordReader))
			recordReader.loadFromMetaData(recordMetaDataIndex)
			Dim record As Record = recordReader.nextRecord()
			assertEquals(2, record.getRecord().Count)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.io.File tmpDataFile(org.nd4j.common.primitives.Pair<org.datavec.api.transform.schema.Schema, List<List<Writable>>> recordsToWrite) throws java.io.IOException
		Private Function tmpDataFile(ByVal recordsToWrite As Pair(Of Schema, IList(Of IList(Of Writable)))) As File
			Dim f As File = testDir.toFile()
			' send file
			Dim tmp As New File(f, "tmp-file-" & System.Guid.randomUUID().ToString())
			tmp.mkdirs()
			Dim tmpFile As New File(tmp, "data.arrow")
			tmpFile.deleteOnExit()
			Dim bufferedOutputStream As New FileStream(tmpFile, FileMode.Create, FileAccess.Write)
			ArrowConverter.writeRecordBatchTo(recordsToWrite.Right, recordsToWrite.First, bufferedOutputStream)
			bufferedOutputStream.Flush()
			bufferedOutputStream.Close()
			Return tmp
		End Function

		Private Function recordToWrite() As Pair(Of Schema, IList(Of IList(Of Writable)))
			Dim records As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			records.Add(New List(Of Writable) From {Of Writable})
			records.Add(New List(Of Writable) From {Of Writable})
			Dim schemaBuilder As New Schema.Builder()
			For i As Integer = 0 To 1
				schemaBuilder.addColumnFloat("col-" & i)
			Next i
			Return Pair.of(schemaBuilder.build(), records)
		End Function
	End Class

End Namespace