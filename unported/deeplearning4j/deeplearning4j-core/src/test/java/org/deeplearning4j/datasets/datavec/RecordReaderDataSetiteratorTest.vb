Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Files = org.nd4j.shade.guava.io.Files
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CollectionRecordReader = org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports CollectionSequenceRecordReader = org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputStreamInputSplit = org.datavec.api.split.InputStreamInputSplit
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ZeroLengthSequenceException = org.deeplearning4j.datasets.datavec.exception.ZeroLengthSequenceException
Imports SpecialImageRecordReader = org.deeplearning4j.datasets.datavec.tools.SpecialImageRecordReader
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.nd4j.common.primitives
Imports Resources = org.nd4j.common.resources.Resources
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
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
Namespace org.deeplearning4j.datasets.datavec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Record Reader Data Setiterator Test") @Disabled @NativeTag class RecordReaderDataSetiteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class RecordReaderDataSetiteratorTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path temporaryFolder;
		Public temporaryFolder As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader") void testRecordReader(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRecordReader(ByVal nd4jBackend As Nd4jBackend)
			Dim recordReader As RecordReader = New CSVRecordReader()
			Dim csv As New org.datavec.api.Split.FileSplit(Resources.asFile("csv-example.csv"))
			recordReader.initialize(csv)
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(recordReader, 34)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			assertEquals(34, [next].numExamples())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader Max Batch Limit") void testRecordReaderMaxBatchLimit(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRecordReaderMaxBatchLimit(ByVal backend As Nd4jBackend)
			Dim recordReader As RecordReader = New CSVRecordReader()
			Dim csv As New org.datavec.api.Split.FileSplit(Resources.asFile("csv-example.csv"))
			recordReader.initialize(csv)
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(recordReader, 10, -1, -1, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			assertFalse(ds Is Nothing)
			assertEquals(10, ds.numExamples())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			iter.hasNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			iter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertEquals(False, iter.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader Multi Regression") void testRecordReaderMultiRegression(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRecordReaderMultiRegression(ByVal backend As Nd4jBackend)
			For Each builder As Boolean In New Boolean() { False, True }
				Dim csv As RecordReader = New CSVRecordReader()
				csv.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
				Dim batchSize As Integer = 3
				Dim labelIdxFrom As Integer = 3
				Dim labelIdxTo As Integer = 4
				Dim iter As DataSetIterator
				If builder Then
					iter = (New RecordReaderDataSetIterator.Builder(csv, batchSize)).regression(labelIdxFrom, labelIdxTo).build()
				Else
					iter = New RecordReaderDataSetIterator(csv, batchSize, labelIdxFrom, labelIdxTo, True)
				End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = iter.next()
				Dim f As INDArray = ds.Features
				Dim l As INDArray = ds.Labels
				assertArrayEquals(New Long() { 3, 3 }, f.shape())
				assertArrayEquals(New Long() { 3, 2 }, l.shape())
				' Check values:
				Dim fExpD()() As Double = {
					New Double() { 5.1, 3.5, 1.4 },
					New Double() { 4.9, 3.0, 1.4 },
					New Double() { 4.7, 3.2, 1.3 }
				}
				Dim lExpD()() As Double = {
					New Double() { 0.2, 0 },
					New Double() { 0.2, 0 },
					New Double() { 0.2, 0 }
				}
				Dim fExp As INDArray = Nd4j.create(fExpD).castTo(DataType.FLOAT)
				Dim lExp As INDArray = Nd4j.create(lExpD).castTo(DataType.FLOAT)
				assertEquals(fExp, f)
				assertEquals(lExp, l)
			Next builder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader") @Tag(org.nd4j.common.tests.tags.TagNames.NDARRAY_INDEXING) void testSequenceRecordReader(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequenceRecordReader(ByVal backend As Nd4jBackend)
			Dim rootDir As File = temporaryFolder.toFile()
			' need to manually extract
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequence_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequence_{0:D}.txt", i)))
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequencelabels_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequencelabels_{0:D}.txt", i)))
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabels_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False)
			assertEquals(3, iter.inputColumns())
			assertEquals(4, iter.totalOutcomes())
			Dim dsList As IList(Of DataSet) = New List(Of DataSet)()
			Do While iter.MoveNext()
				dsList.Add(iter.Current)
			Loop
			' 3 files
			assertEquals(3, dsList.Count)
			For i As Integer = 0 To 2
				Dim ds As DataSet = dsList(i)
				Dim features As INDArray = ds.Features
				Dim labels As INDArray = ds.Labels
				' 1 example in mini-batch
				assertEquals(1, features.size(0))
				assertEquals(1, labels.size(0))
				' 3 values per line/time step
				assertEquals(3, features.size(1))
				' 1 value per line, but 4 possible values -> one-hot vector
				assertEquals(4, labels.size(1))
				' sequence length = 4
				assertEquals(4, features.size(2))
				assertEquals(4, labels.size(2))
			Next i
			' Check features vs. expected:
			Dim expF0 As INDArray = Nd4j.create(1, 3, 4)
			expF0.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 1, 2 }))
			expF0.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 10, 11, 12 }))
			expF0.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 20, 21, 22 }))
			expF0.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 30, 31, 32 }))
			assertEquals(dsList(0).getFeatures(), expF0)
			Dim expF1 As INDArray = Nd4j.create(1, 3, 4)
			expF1.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 100, 101, 102 }))
			expF1.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 110, 111, 112 }))
			expF1.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 120, 121, 122 }))
			expF1.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 130, 131, 132 }))
			assertEquals(dsList(1).getFeatures(), expF1)
			Dim expF2 As INDArray = Nd4j.create(1, 3, 4)
			expF2.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 200, 201, 202 }))
			expF2.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 210, 211, 212 }))
			expF2.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 220, 221, 222 }))
			expF2.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 230, 231, 232 }))
			assertEquals(dsList(2).getFeatures(), expF2)
			' Check labels vs. expected:
			Dim expL0 As INDArray = Nd4j.create(1, 4, 4)
			expL0.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 1, 0, 0, 0 }))
			expL0.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			expL0.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0, 0, 1, 0 }))
			expL0.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0, 0, 0, 1 }))
			assertEquals(dsList(0).getLabels(), expL0)
			Dim expL1 As INDArray = Nd4j.create(1, 4, 4)
			expL1.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 0, 0, 1 }))
			expL1.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0, 0, 1, 0 }))
			expL1.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			expL1.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 1, 0, 0, 0 }))
			assertEquals(dsList(1).getLabels(), expL1)
			Dim expL2 As INDArray = Nd4j.create(1, 4, 4)
			expL2.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			expL2.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 1, 0, 0, 0 }))
			expL2.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0, 0, 0, 1 }))
			expL2.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0, 0, 1, 0 }))
			assertEquals(dsList(2).getLabels(), expL2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Meta") void testSequenceRecordReaderMeta(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequenceRecordReaderMeta(ByVal backend As Nd4jBackend)
			Dim rootDir As File = temporaryFolder.toFile()
			' need to manually extract
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequence_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequence_{0:D}.txt", i)))
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequencelabels_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequencelabels_{0:D}.txt", i)))
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabels_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False)
			iter.setCollectMetaData(True)
			assertEquals(3, iter.inputColumns())
			assertEquals(4, iter.totalOutcomes())
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				Dim meta As IList(Of RecordMetaData) = ds.getExampleMetaData(GetType(RecordMetaData))
				Dim fromMeta As DataSet = iter.loadFromMetaData(meta)
				assertEquals(ds, fromMeta)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Regression") void testSequenceRecordReaderRegression(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequenceRecordReaderRegression(ByVal backend As Nd4jBackend)
			' need to manually extract
			Dim rootDir As File = temporaryFolder.toFile()
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequence_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequence_{0:D}.txt", i)))
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 0, True)
			assertEquals(3, iter.inputColumns())
			assertEquals(3, iter.totalOutcomes())
			Dim dsList As IList(Of DataSet) = New List(Of DataSet)()
			Do While iter.MoveNext()
				dsList.Add(iter.Current)
			Loop
			' 3 files
			assertEquals(3, dsList.Count)
			For i As Integer = 0 To 2
				Dim ds As DataSet = dsList(i)
				Dim features As INDArray = ds.Features
				Dim labels As INDArray = ds.Labels
				' 1 examples, 3 values, 4 time steps
				assertArrayEquals(New Long() { 1, 3, 4 }, features.shape())
				assertArrayEquals(New Long() { 1, 3, 4 }, labels.shape())
				assertEquals(features, labels)
			Next i
			' Also test regression + reset from a single reader:
			featureReader.reset()
			iter = New SequenceRecordReaderDataSetIterator(featureReader, 1, 0, 2, True)
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				assertEquals(2, ds.Features.size(1))
				assertEquals(1, ds.Labels.size(1))
				count += 1
			Loop
			assertEquals(3, count)
			iter.reset()
			count = 0
			Do While iter.MoveNext()
				iter.Current
				count += 1
			Loop
			assertEquals(3, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Multi Regression") void testSequenceRecordReaderMultiRegression(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequenceRecordReaderMultiRegression(ByVal backend As Nd4jBackend)
			Dim rootDir As File = temporaryFolder.toFile()
			' need to manually extract
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequence_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequence_{0:D}.txt", i)))
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim reader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			reader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			Dim iter As New SequenceRecordReaderDataSetIterator(reader, 1, 2, 1, True)
			assertEquals(1, iter.inputColumns())
			assertEquals(2, iter.totalOutcomes())
			Dim dsList As IList(Of DataSet) = New List(Of DataSet)()
			Do While iter.MoveNext()
				dsList.Add(iter.Current)
			Loop
			' 3 files
			assertEquals(3, dsList.Count)
			For i As Integer = 0 To 2
				Dim ds As DataSet = dsList(i)
				Dim features As INDArray = ds.Features
				Dim labels As INDArray = ds.Labels
				' 1 examples, 1 values, 4 time steps
				assertArrayEquals(New Long() { 1, 1, 4 }, features.shape())
				assertArrayEquals(New Long() { 1, 2, 4 }, labels.shape())
				Dim f2d As INDArray = features.get(point(0), all(), all()).transpose()
				Dim l2d As INDArray = labels.get(point(0), all(), all()).transpose()
				Select Case i
					Case 0
						assertEquals(Nd4j.create(New Double() { 0, 10, 20, 30 }, New Integer() { 4, 1 }).castTo(DataType.FLOAT), f2d)
						assertEquals(Nd4j.create(New Double()() {
							New Double() { 1, 2 },
							New Double() { 11, 12 },
							New Double() { 21, 22 },
							New Double() { 31, 32 }
						}).castTo(DataType.FLOAT), l2d)
					Case 1
						assertEquals(Nd4j.create(New Double() { 100, 110, 120, 130 }, New Integer() { 4, 1 }).castTo(DataType.FLOAT), f2d)
						assertEquals(Nd4j.create(New Double()() {
							New Double() { 101, 102 },
							New Double() { 111, 112 },
							New Double() { 121, 122 },
							New Double() { 131, 132 }
						}).castTo(DataType.FLOAT), l2d)
					Case 2
						assertEquals(Nd4j.create(New Double() { 200, 210, 220, 230 }, New Integer() { 4, 1 }).castTo(DataType.FLOAT), f2d)
						assertEquals(Nd4j.create(New Double()() {
							New Double() { 201, 202 },
							New Double() { 211, 212 },
							New Double() { 221, 222 },
							New Double() { 231, 232 }
						}).castTo(DataType.FLOAT), l2d)
					Case Else
						Throw New Exception()
				End Select
			Next i
			iter.reset()
			Dim count As Integer = 0
			Do While iter.MoveNext()
				iter.Current
				count += 1
			Loop
			assertEquals(3, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Reset") void testSequenceRecordReaderReset(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequenceRecordReaderReset(ByVal backend As Nd4jBackend)
			Dim rootDir As File = temporaryFolder.toFile()
			' need to manually extract
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequence_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequence_{0:D}.txt", i)))
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequencelabels_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequencelabels_{0:D}.txt", i)))
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabels_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False)
			assertEquals(3, iter.inputColumns())
			assertEquals(4, iter.totalOutcomes())
			Dim nResets As Integer = 5
			For i As Integer = 0 To nResets - 1
				iter.reset()
				Dim count As Integer = 0
				Do While iter.MoveNext()
					Dim ds As DataSet = iter.Current
					Dim features As INDArray = ds.Features
					Dim labels As INDArray = ds.Labels
					assertArrayEquals(New Long() { 1, 3, 4 }, features.shape())
					assertArrayEquals(New Long() { 1, 4, 4 }, labels.shape())
					count += 1
				Loop
				assertEquals(3, count)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test CSV Loading Regression") void testCSVLoadingRegression(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCSVLoadingRegression(ByVal backend As Nd4jBackend)
			Dim nLines As Integer = 30
			Dim nFeatures As Integer = 5
			Dim miniBatchSize As Integer = 10
			Dim labelIdx As Integer = 0
			Dim path As String = "rr_csv_test_rand.csv"
			Dim p As Pair(Of Double()(), File) = makeRandomCSV(path, nLines, nFeatures)
			Dim data()() As Double = p.First
			Dim testReader As RecordReader = New CSVRecordReader()
			testReader.initialize(New org.datavec.api.Split.FileSplit(p.Second))
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(testReader, miniBatchSize, labelIdx, labelIdx, True)
			Dim miniBatch As Integer = 0
			Do While iter.MoveNext()
				Dim test As DataSet = iter.Current
				Dim features As INDArray = test.Features
				Dim labels As INDArray = test.Labels
				assertArrayEquals(New Long() { miniBatchSize, nFeatures }, features.shape())
				assertArrayEquals(New Long() { miniBatchSize, 1 }, labels.shape())
				Dim startRow As Integer = miniBatch * miniBatchSize
				For i As Integer = 0 To miniBatchSize - 1
					Dim labelExp As Double = data(startRow + i)(labelIdx)
					Dim labelAct As Double = labels.getDouble(i)
					assertEquals(labelExp, labelAct, 1e-5f)
					Dim featureCount As Integer = 0
					Dim j As Integer = 0
					Do While j < nFeatures + 1
						If j = labelIdx Then
							j += 1
							Continue Do
						End If
						Dim featureExp As Double = data(startRow + i)(j)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: double featureAct = features.getDouble(i, featureCount++);
						Dim featureAct As Double = features.getDouble(i, featureCount)
							featureCount += 1
						assertEquals(featureExp, featureAct, 1e-5f)
						j += 1
					Loop
				Next i
				miniBatch += 1
			Loop
			assertEquals(nLines \ miniBatchSize, miniBatch)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.common.primitives.Pair<double[][], File> makeRandomCSV(String tempFile, int nLines, int nFeatures) throws IOException
		Public Overridable Function makeRandomCSV(ByVal tempFile As String, ByVal nLines As Integer, ByVal nFeatures As Integer) As Pair(Of Double()(), File)
			Dim temp As File = temporaryFolder.resolve(tempFile).toFile()
			temp.mkdirs()
			temp.deleteOnExit()
			Dim rand As New Random(12345)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim dArr[][] As Double = new Double[nLines][nFeatures + 1]
			Dim dArr()() As Double = RectangularArrays.RectangularDoubleArray(nLines, nFeatures + 1)
			Try
					Using [out] As New PrintWriter(New StreamWriter(temp))
					For i As Integer = 0 To nLines - 1
						' First column: label
						dArr(i)(0) = rand.NextDouble()
						[out].print(dArr(i)(0))
						For j As Integer = 0 To nFeatures - 1
							dArr(i)(j + 1) = rand.NextDouble()
							[out].print("," & dArr(i)(j + 1))
						Next j
						[out].println()
					Next i
					End Using
			Catch e As IOException
				log.error("", e)
			End Try
			Return New Pair(Of Double()(), File)(dArr, temp)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Variable Length Sequence") void testVariableLengthSequence(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testVariableLengthSequence(ByVal backend As Nd4jBackend)
			Dim rootDir As File = temporaryFolder.toFile()
			' need to manually extract
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequence_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequence_{0:D}.txt", i)))
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequencelabelsShort_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequencelabelsShort_{0:D}.txt", i)))
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabelsShort_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim featureReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iterAlignStart As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_START)
			Dim iterAlignEnd As New SequenceRecordReaderDataSetIterator(featureReader2, labelReader2, 1, 4, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END)
			assertEquals(3, iterAlignStart.inputColumns())
			assertEquals(4, iterAlignStart.totalOutcomes())
			assertEquals(3, iterAlignEnd.inputColumns())
			assertEquals(4, iterAlignEnd.totalOutcomes())
			Dim dsListAlignStart As IList(Of DataSet) = New List(Of DataSet)()
			Do While iterAlignStart.MoveNext()
				dsListAlignStart.Add(iterAlignStart.Current)
			Loop
			Dim dsListAlignEnd As IList(Of DataSet) = New List(Of DataSet)()
			Do While iterAlignEnd.MoveNext()
				dsListAlignEnd.Add(iterAlignEnd.Current)
			Loop
			' 3 files
			assertEquals(3, dsListAlignStart.Count)
			' 3 files
			assertEquals(3, dsListAlignEnd.Count)
			For i As Integer = 0 To 2
				Dim ds As DataSet = dsListAlignStart(i)
				Dim features As INDArray = ds.Features
				Dim labels As INDArray = ds.Labels
				' 1 example in mini-batch
				assertEquals(1, features.size(0))
				assertEquals(1, labels.size(0))
				' 3 values per line/time step
				assertEquals(3, features.size(1))
				' 1 value per line, but 4 possible values -> one-hot vector
				assertEquals(4, labels.size(1))
				' sequence length = 4
				assertEquals(4, features.size(2))
				assertEquals(4, labels.size(2))
				Dim ds2 As DataSet = dsListAlignEnd(i)
				features = ds2.Features
				labels = ds2.Labels
				' 1 example in mini-batch
				assertEquals(1, features.size(0))
				assertEquals(1, labels.size(0))
				' 3 values per line/time step
				assertEquals(3, features.size(1))
				' 1 value per line, but 4 possible values -> one-hot vector
				assertEquals(4, labels.size(1))
				' sequence length = 4
				assertEquals(4, features.size(2))
				assertEquals(4, labels.size(2))
			Next i
			' Check features vs. expected:
			' Here: labels always longer than features -> same features for align start and align end
			Dim expF0 As INDArray = Nd4j.create(1, 3, 4)
			expF0.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 1, 2 }))
			expF0.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 10, 11, 12 }))
			expF0.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 20, 21, 22 }))
			expF0.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 30, 31, 32 }))
			assertEquals(expF0, dsListAlignStart(0).getFeatures())
			assertEquals(expF0, dsListAlignEnd(0).getFeatures())
			Dim expF1 As INDArray = Nd4j.create(1, 3, 4)
			expF1.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 100, 101, 102 }))
			expF1.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 110, 111, 112 }))
			expF1.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 120, 121, 122 }))
			expF1.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 130, 131, 132 }))
			assertEquals(expF1, dsListAlignStart(1).getFeatures())
			assertEquals(expF1, dsListAlignEnd(1).getFeatures())
			Dim expF2 As INDArray = Nd4j.create(1, 3, 4)
			expF2.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 200, 201, 202 }))
			expF2.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 210, 211, 212 }))
			expF2.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 220, 221, 222 }))
			expF2.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 230, 231, 232 }))
			assertEquals(expF2, dsListAlignStart(2).getFeatures())
			assertEquals(expF2, dsListAlignEnd(2).getFeatures())
			' Check features mask array:
			' null: equivalent to all 1s (i.e., present for all time steps)
			Dim featuresMaskExpected As INDArray = Nothing
			For i As Integer = 0 To 2
				Dim featuresMaskStart As INDArray = dsListAlignStart(i).getFeaturesMaskArray()
				Dim featuresMaskEnd As INDArray = dsListAlignEnd(i).getFeaturesMaskArray()
				assertEquals(featuresMaskExpected, featuresMaskStart)
				assertEquals(featuresMaskExpected, featuresMaskEnd)
			Next i
			' Check labels vs. expected:
			' First: aligning start
			Dim expL0 As INDArray = Nd4j.create(1, 4, 4)
			expL0.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 1, 0, 0, 0 }))
			expL0.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			assertEquals(expL0, dsListAlignStart(0).getLabels())
			Dim expL1 As INDArray = Nd4j.create(1, 4, 4)
			expL1.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			assertEquals(expL1, dsListAlignStart(1).getLabels())
			Dim expL2 As INDArray = Nd4j.create(1, 4, 4)
			expL2.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 0, 0, 1 }))
			expL2.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0, 0, 1, 0 }))
			expL2.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			assertEquals(expL2, dsListAlignStart(2).getLabels())
			' Second: align end
			Dim expL0end As INDArray = Nd4j.create(1, 4, 4)
			expL0end.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 1, 0, 0, 0 }))
			expL0end.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			assertEquals(expL0end, dsListAlignEnd(0).getLabels())
			Dim expL1end As INDArray = Nd4j.create(1, 4, 4)
			expL1end.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			assertEquals(expL1end, dsListAlignEnd(1).getLabels())
			Dim expL2end As INDArray = Nd4j.create(1, 4, 4)
			expL2end.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0, 0, 0, 1 }))
			expL2end.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0, 0, 1, 0 }))
			expL2end.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0, 1, 0, 0 }))
			assertEquals(expL2end, dsListAlignEnd(2).getLabels())
			' Check labels mask array
			Dim labelsMaskExpectedStart() As INDArray = { Nd4j.create(New Single() { 1, 1, 0, 0 }, New Integer() { 1, 4 }), Nd4j.create(New Single() { 1, 0, 0, 0 }, New Integer() { 1, 4 }), Nd4j.create(New Single() { 1, 1, 1, 0 }, New Integer() { 1, 4 }) }
			Dim labelsMaskExpectedEnd() As INDArray = { Nd4j.create(New Single() { 0, 0, 1, 1 }, New Integer() { 1, 4 }), Nd4j.create(New Single() { 0, 0, 0, 1 }, New Integer() { 1, 4 }), Nd4j.create(New Single() { 0, 1, 1, 1 }, New Integer() { 1, 4 }) }
			For i As Integer = 0 To 2
				Dim labelsMaskStart As INDArray = dsListAlignStart(i).getLabelsMaskArray()
				Dim labelsMaskEnd As INDArray = dsListAlignEnd(i).getLabelsMaskArray()
				assertEquals(labelsMaskExpectedStart(i), labelsMaskStart)
				assertEquals(labelsMaskExpectedEnd(i), labelsMaskEnd)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Single Reader") void testSequenceRecordReaderSingleReader(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequenceRecordReaderSingleReader(ByVal backend As Nd4jBackend)
			Dim rootDir As File = temporaryFolder.toFile()
			' need to manually extract
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequenceSingle_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequenceSingle_{0:D}.txt", i)))
			Next i
			Dim path As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequenceSingle_%d.txt")
			Dim reader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			reader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2))
			Dim iteratorClassification As New SequenceRecordReaderDataSetIterator(reader, 1, 3, 0, False)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iteratorClassification.hasNext())
			Dim reader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			reader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2))
			Dim iteratorRegression As New SequenceRecordReaderDataSetIterator(reader2, 1, 1, 0, True)
			Dim expF0 As INDArray = Nd4j.create(1, 2, 4)
			expF0.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 1, 2 }))
			expF0.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 11, 12 }))
			expF0.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 21, 22 }))
			expF0.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 31, 32 }))
			Dim expF1 As INDArray = Nd4j.create(1, 2, 4)
			expF1.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 101, 102 }))
			expF1.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 111, 112 }))
			expF1.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 121, 122 }))
			expF1.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 131, 132 }))
			Dim expF2 As INDArray = Nd4j.create(1, 2, 4)
			expF2.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 201, 202 }))
			expF2.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 211, 212 }))
			expF2.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 221, 222 }))
			expF2.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 231, 232 }))
			Dim expF() As INDArray = { expF0, expF1, expF2 }
			' Expected out for classification:
			Dim expOut0 As INDArray = Nd4j.create(1, 3, 4)
			expOut0.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 1, 0, 0 }))
			expOut0.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0, 1, 0 }))
			expOut0.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0, 0, 1 }))
			expOut0.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 1, 0, 0 }))
			Dim expOut1 As INDArray = Nd4j.create(1, 3, 4)
			expOut1.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 1, 0 }))
			expOut1.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0, 0, 1 }))
			expOut1.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 1, 0, 0 }))
			expOut1.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0, 0, 1 }))
			Dim expOut2 As INDArray = Nd4j.create(1, 3, 4)
			expOut2.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0, 1, 0 }))
			expOut2.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 1, 0, 0 }))
			expOut2.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0, 1, 0 }))
			expOut2.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0, 0, 1 }))
			Dim expOutClassification() As INDArray = { expOut0, expOut1, expOut2 }
			' Expected out for regression:
			Dim expOutR0 As INDArray = Nd4j.create(1, 1, 4)
			expOutR0.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 0 }))
			expOutR0.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 1 }))
			expOutR0.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 2 }))
			expOutR0.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 0 }))
			Dim expOutR1 As INDArray = Nd4j.create(1, 1, 4)
			expOutR1.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 1 }))
			expOutR1.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 2 }))
			expOutR1.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 0 }))
			expOutR1.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 2 }))
			Dim expOutR2 As INDArray = Nd4j.create(1, 1, 4)
			expOutR2.tensorAlongDimension(0, 1).assign(Nd4j.create(New Double() { 1 }))
			expOutR2.tensorAlongDimension(1, 1).assign(Nd4j.create(New Double() { 0 }))
			expOutR2.tensorAlongDimension(2, 1).assign(Nd4j.create(New Double() { 1 }))
			expOutR2.tensorAlongDimension(3, 1).assign(Nd4j.create(New Double() { 2 }))
			Dim expOutRegression() As INDArray = { expOutR0, expOutR1, expOutR2 }
			Dim countC As Integer = 0
			Do While iteratorClassification.MoveNext()
				Dim ds As DataSet = iteratorClassification.Current
				Dim f As INDArray = ds.Features
				Dim l As INDArray = ds.Labels
				assertNull(ds.FeaturesMaskArray)
				assertNull(ds.LabelsMaskArray)
				assertArrayEquals(New Long() { 1, 2, 4 }, f.shape())
				' One-hot representation
				assertArrayEquals(New Long() { 1, 3, 4 }, l.shape())
				assertEquals(expF(countC), f)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(expOutClassification[countC++], l);
				assertEquals(expOutClassification(countC), l)
					countC += 1
			Loop
			assertEquals(3, countC)
			assertEquals(3, iteratorClassification.totalOutcomes())
			Dim countF As Integer = 0
			Do While iteratorRegression.MoveNext()
				Dim ds As DataSet = iteratorRegression.Current
				Dim f As INDArray = ds.Features
				Dim l As INDArray = ds.Labels
				assertNull(ds.FeaturesMaskArray)
				assertNull(ds.LabelsMaskArray)
				assertArrayEquals(New Long() { 1, 2, 4 }, f.shape())
				' Regression (single output)
				assertArrayEquals(New Long() { 1, 1, 4 }, l.shape())
				assertEquals(expF(countF), f)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(expOutRegression[countF++], l);
				assertEquals(expOutRegression(countF), l)
					countF += 1
			Loop
			assertEquals(3, countF)
			assertEquals(1, iteratorRegression.totalOutcomes())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Single Reader With Empty Sequence Throws") void testSequenceRecordReaderSingleReaderWithEmptySequenceThrows(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSequenceRecordReaderSingleReaderWithEmptySequenceThrows(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ZeroLengthSequenceException), Sub()
			Dim reader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			reader.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("empty.txt")))
			Call (New SequenceRecordReaderDataSetIterator(reader, 1, -1, 1, True)).next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Two Readers With Empty Feature Sequence Throws") void testSequenceRecordReaderTwoReadersWithEmptyFeatureSequenceThrows(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSequenceRecordReaderTwoReadersWithEmptyFeatureSequenceThrows(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ZeroLengthSequenceException), Sub()
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("empty.txt")))
			labelReader.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("csvsequencelabels_0.txt")))
			Call (New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, -1, True)).next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Two Readers With Empty Label Sequence Throws") void testSequenceRecordReaderTwoReadersWithEmptyLabelSequenceThrows(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSequenceRecordReaderTwoReadersWithEmptyLabelSequenceThrows(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ZeroLengthSequenceException), Sub()
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim f As File = Resources.asFile("csvsequence_0.txt")
			featureReader.initialize(New org.datavec.api.Split.FileSplit(f))
			labelReader.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("empty.txt")))
			Call (New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, -1, True)).next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Sequence Record Reader Single Reader Meta Data") void testSequenceRecordReaderSingleReaderMetaData(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequenceRecordReaderSingleReaderMetaData(ByVal backend As Nd4jBackend)
			Dim rootDir As File = temporaryFolder.toFile()
			' need to manually extract
			For i As Integer = 0 To 2
				FileUtils.copyFile(Resources.asFile(String.Format("csvsequenceSingle_{0:D}.txt", i)), New File(rootDir, String.Format("csvsequenceSingle_{0:D}.txt", i)))
			Next i
			Dim path As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequenceSingle_%d.txt")
			Dim reader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			reader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2))
			Dim iteratorClassification As New SequenceRecordReaderDataSetIterator(reader, 1, 3, 0, False)
			Dim reader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			reader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 2))
			Dim iteratorRegression As New SequenceRecordReaderDataSetIterator(reader2, 1, 1, 0, True)
			iteratorClassification.setCollectMetaData(True)
			iteratorRegression.setCollectMetaData(True)
			Do While iteratorClassification.MoveNext()
				Dim ds As DataSet = iteratorClassification.Current
				Dim fromMeta As DataSet = iteratorClassification.loadFromMetaData(ds.getExampleMetaData(GetType(RecordMetaData)))
				assertEquals(ds, fromMeta)
			Loop
			Do While iteratorRegression.MoveNext()
				Dim ds As DataSet = iteratorRegression.Current
				Dim fromMeta As DataSet = iteratorRegression.loadFromMetaData(ds.getExampleMetaData(GetType(RecordMetaData)))
				assertEquals(ds, fromMeta)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Seq RRDSI Array Writable One Reader") void testSeqRRDSIArrayWritableOneReader(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSeqRRDSIArrayWritableOneReader(ByVal backend As Nd4jBackend)
			Dim sequence1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence1.Add(New List(Of Writable) From {DirectCast(New NDArrayWritable(Nd4j.create(New Double() { 1, 2, 3 }, New Long() { 1, 3 })), Writable), New IntWritable(0)})
			sequence1.Add(New List(Of Writable) From {DirectCast(New NDArrayWritable(Nd4j.create(New Double() { 4, 5, 6 }, New Long() { 1, 3 })), Writable), New IntWritable(1)})
			Dim sequence2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence2.Add(New List(Of Writable) From {DirectCast(New NDArrayWritable(Nd4j.create(New Double() { 7, 8, 9 }, New Long() { 1, 3 })), Writable), New IntWritable(2)})
			sequence2.Add(New List(Of Writable) From {DirectCast(New NDArrayWritable(Nd4j.create(New Double() { 10, 11, 12 }, New Long() { 1, 3 })), Writable), New IntWritable(3)})
			Dim rr As SequenceRecordReader = New CollectionSequenceRecordReader(java.util.Arrays.asList(sequence1, sequence2))
			Dim iter As New SequenceRecordReaderDataSetIterator(rr, 2, 4, 1, False)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			' 2 examples, 3 values per time step, 2 time steps
			Dim expFeatures As INDArray = Nd4j.create(2, 3, 2)
			expFeatures.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 1, 4 },
				New Double() { 2, 5 },
				New Double() { 3, 6 }
			}))
			expFeatures.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 7, 10 },
				New Double() { 8, 11 },
				New Double() { 9, 12 }
			}))
			Dim expLabels As INDArray = Nd4j.create(2, 4, 2)
			expLabels.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 1, 0 },
				New Double() { 0, 1 },
				New Double() { 0, 0 },
				New Double() { 0, 0 }
			}))
			expLabels.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 0, 0 },
				New Double() { 0, 0 },
				New Double() { 1, 0 },
				New Double() { 0, 1 }
			}))
			assertEquals(expFeatures, ds.Features)
			assertEquals(expLabels, ds.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Seq RRDSI Array Writable One Reader Regression") void testSeqRRDSIArrayWritableOneReaderRegression(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSeqRRDSIArrayWritableOneReaderRegression(ByVal backend As Nd4jBackend)
			' Regression, where the output is an array writable
			Dim sequence1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence1.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 1, 2, 3 }, New Long() { 1, 3 })),
				New NDArrayWritable(Nd4j.create(New Double() { 100, 200, 300 }, New Long() { 1, 3 }))
			})
			sequence1.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 4, 5, 6 }, New Long() { 1, 3 })),
				New NDArrayWritable(Nd4j.create(New Double() { 400, 500, 600 }, New Long() { 1, 3 }))
			})
			Dim sequence2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence2.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 7, 8, 9 }, New Long() { 1, 3 })),
				New NDArrayWritable(Nd4j.create(New Double() { 700, 800, 900 }, New Long() { 1, 3 }))
			})
			sequence2.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 10, 11, 12 }, New Long() { 1, 3 })),
				New NDArrayWritable(Nd4j.create(New Double() { 1000, 1100, 1200 }, New Long() { 1, 3 }))
			})
			Dim rr As SequenceRecordReader = New CollectionSequenceRecordReader(java.util.Arrays.asList(sequence1, sequence2))
			Dim iter As New SequenceRecordReaderDataSetIterator(rr, 2, -1, 1, True)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			' 2 examples, 3 values per time step, 2 time steps
			Dim expFeatures As INDArray = Nd4j.create(2, 3, 2)
			expFeatures.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 1, 4 },
				New Double() { 2, 5 },
				New Double() { 3, 6 }
			}))
			expFeatures.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 7, 10 },
				New Double() { 8, 11 },
				New Double() { 9, 12 }
			}))
			Dim expLabels As INDArray = Nd4j.create(2, 3, 2)
			expLabels.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 100, 400 },
				New Double() { 200, 500 },
				New Double() { 300, 600 }
			}))
			expLabels.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 700, 1000 },
				New Double() { 800, 1100 },
				New Double() { 900, 1200 }
			}))
			assertEquals(expFeatures, ds.Features)
			assertEquals(expLabels, ds.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Seq RRDSI Multiple Array Writables One Reader") void testSeqRRDSIMultipleArrayWritablesOneReader(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSeqRRDSIMultipleArrayWritablesOneReader(ByVal backend As Nd4jBackend)
			' Input with multiple array writables:
			Dim sequence1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence1.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 1, 2, 3 }, New Long() { 1, 3 })),
				New NDArrayWritable(Nd4j.create(New Double() { 100, 200, 300 }, New Long() { 1, 3 })),
				New IntWritable(0)
			})
			sequence1.Add(New List(Of Writable) From {DirectCast(New NDArrayWritable(Nd4j.create(New Double() { 4, 5, 6 }, New Long() { 1, 3 })), Writable), New NDArrayWritable(Nd4j.create(New Double() { 400, 500, 600 }, New Long() { 1, 3 })), New IntWritable(1)})
			Dim sequence2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence2.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 7, 8, 9 }, New Long() { 1, 3 })),
				New NDArrayWritable(Nd4j.create(New Double() { 700, 800, 900 }, New Long() { 1, 3 })),
				New IntWritable(2)
			})
			sequence2.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 10, 11, 12 }, New Long() { 1, 3 })),
				New NDArrayWritable(Nd4j.create(New Double() { 1000, 1100, 1200 }, New Long() { 1, 3 })),
				New IntWritable(3)
			})
			Dim rr As SequenceRecordReader = New CollectionSequenceRecordReader(java.util.Arrays.asList(sequence1, sequence2))
			Dim iter As New SequenceRecordReaderDataSetIterator(rr, 2, 4, 2, False)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			' 2 examples, 6 values per time step, 2 time steps
			Dim expFeatures As INDArray = Nd4j.create(2, 6, 2)
			expFeatures.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 1, 4 },
				New Double() { 2, 5 },
				New Double() { 3, 6 },
				New Double() { 100, 400 },
				New Double() { 200, 500 },
				New Double() { 300, 600 }
			}))
			expFeatures.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 7, 10 },
				New Double() { 8, 11 },
				New Double() { 9, 12 },
				New Double() { 700, 1000 },
				New Double() { 800, 1100 },
				New Double() { 900, 1200 }
			}))
			Dim expLabels As INDArray = Nd4j.create(2, 4, 2)
			expLabels.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 1, 0 },
				New Double() { 0, 1 },
				New Double() { 0, 0 },
				New Double() { 0, 0 }
			}))
			expLabels.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 0, 0 },
				New Double() { 0, 0 },
				New Double() { 1, 0 },
				New Double() { 0, 1 }
			}))
			assertEquals(expFeatures, ds.Features)
			assertEquals(expLabels, ds.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Seq RRDSI Array Writable Two Readers") void testSeqRRDSIArrayWritableTwoReaders(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSeqRRDSIArrayWritableTwoReaders(ByVal backend As Nd4jBackend)
			Dim sequence1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence1.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 1, 2, 3 }, New Long() { 1, 3 })),
				New IntWritable(100)
			})
			sequence1.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 4, 5, 6 }, New Long() { 1, 3 })),
				New IntWritable(200)
			})
			Dim sequence2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence2.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 7, 8, 9 }, New Long() { 1, 3 })),
				New IntWritable(300)
			})
			sequence2.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 10, 11, 12 }, New Long() { 1, 3 })),
				New IntWritable(400)
			})
			Dim rrFeatures As SequenceRecordReader = New CollectionSequenceRecordReader(java.util.Arrays.asList(sequence1, sequence2))
			Dim sequence1L As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence1L.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 100, 200, 300 }, New Long() { 1, 3 })),
				New IntWritable(101)
			})
			sequence1L.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 400, 500, 600 }, New Long() { 1, 3 })),
				New IntWritable(201)
			})
			Dim sequence2L As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence2L.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 700, 800, 900 }, New Long() { 1, 3 })),
				New IntWritable(301)
			})
			sequence2L.Add(New List(Of Writable) From {
				New NDArrayWritable(Nd4j.create(New Double() { 1000, 1100, 1200 }, New Long() { 1, 3 })),
				New IntWritable(401)
			})
			Dim rrLabels As SequenceRecordReader = New CollectionSequenceRecordReader(java.util.Arrays.asList(sequence1L, sequence2L))
			Dim iter As New SequenceRecordReaderDataSetIterator(rrFeatures, rrLabels, 2, -1, True)
			' 2 examples, 4 values per time step, 2 time steps
			Dim expFeatures As INDArray = Nd4j.create(2, 4, 2)
			expFeatures.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 1, 4 },
				New Double() { 2, 5 },
				New Double() { 3, 6 },
				New Double() { 100, 200 }
			}))
			expFeatures.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 7, 10 },
				New Double() { 8, 11 },
				New Double() { 9, 12 },
				New Double() { 300, 400 }
			}))
			Dim expLabels As INDArray = Nd4j.create(2, 4, 2)
			expLabels.tensorAlongDimension(0, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 100, 400 },
				New Double() { 200, 500 },
				New Double() { 300, 600 },
				New Double() { 101, 201 }
			}))
			expLabels.tensorAlongDimension(1, 1, 2).assign(Nd4j.create(New Double()() {
				New Double() { 700, 1000 },
				New Double() { 800, 1100 },
				New Double() { 900, 1200 },
				New Double() { 301, 401 }
			}))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			assertEquals(expFeatures, ds.Features)
			assertEquals(expLabels, ds.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader Meta Data") void testRecordReaderMetaData() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRecordReaderMetaData()
			Dim csv As RecordReader = New CSVRecordReader()
			csv.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim batchSize As Integer = 10
			Dim labelIdx As Integer = 4
			Dim numClasses As Integer = 3
			Dim rrdsi As New RecordReaderDataSetIterator(csv, batchSize, labelIdx, numClasses)
			rrdsi.CollectMetaData = True
			Do While rrdsi.MoveNext()
				Dim ds As DataSet = rrdsi.Current
				Dim meta As IList(Of RecordMetaData) = ds.getExampleMetaData(GetType(RecordMetaData))
				Dim i As Integer = 0
				For Each m As RecordMetaData In meta
					Dim r As Record = csv.loadFromMetaData(m)
					Dim row As INDArray = ds.Features.getRow(i)
					' if(i <= 3) {
					' System.out.println(m.getLocation() + "\t" + r.getRecord() + "\t" + row);
					' }
					For j As Integer = 0 To 3
						Dim exp As Double = r.getRecord()(j).toDouble()
						Dim act As Double = row.getDouble(j)
						assertEquals(exp, act, 1e-6,"Failed on idx: " & j)
					Next j
					i += 1
				Next m
				' System.out.println();
				Dim fromMeta As DataSet = rrdsi.loadFromMetaData(meta)
				assertEquals(ds, fromMeta)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test RRDS Iwith Async") void testRRDSIwithAsync(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRRDSIwithAsync(ByVal backend As Nd4jBackend)
			Dim csv As RecordReader = New CSVRecordReader()
			csv.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim batchSize As Integer = 10
			Dim labelIdx As Integer = 4
			Dim numClasses As Integer = 3
			Dim rrdsi As New RecordReaderDataSetIterator(csv, batchSize, labelIdx, numClasses)
			Dim adsi As New AsyncDataSetIterator(rrdsi, 8, True)
			Do While adsi.MoveNext()
				Dim ds As DataSet = adsi.Current
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader Data Set Iterator ND Array Writable Labels") void testRecordReaderDataSetIteratorNDArrayWritableLabels(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testRecordReaderDataSetIteratorNDArrayWritableLabels(ByVal backend As Nd4jBackend)
			Dim data As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			data.Add(java.util.Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New NDArrayWritable(Nd4j.create(New Double() { 1.1, 2.1, 3.1 }, New Long() { 1, 3 }))))
			data.Add(java.util.Arrays.asList(Of Writable)(New DoubleWritable(2), New DoubleWritable(3), New NDArrayWritable(Nd4j.create(New Double() { 4.1, 5.1, 6.1 }, New Long() { 1, 3 }))))
			data.Add(java.util.Arrays.asList(Of Writable)(New DoubleWritable(4), New DoubleWritable(5), New NDArrayWritable(Nd4j.create(New Double() { 7.1, 8.1, 9.1 }, New Long() { 1, 3 }))))
			Dim rr As RecordReader = New CollectionRecordReader(data)
			Dim batchSize As Integer = 3
			Dim labelIndexFrom As Integer = 2
			Dim labelIndexTo As Integer = 2
			Dim regression As Boolean = True
			Dim rrdsi As DataSetIterator = New RecordReaderDataSetIterator(rr, batchSize, labelIndexFrom, labelIndexTo, regression)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = rrdsi.next()
			Dim expFeatures As INDArray = Nd4j.create(New Single()() {
				New Single() { 0, 1 },
				New Single() { 2, 3 },
				New Single() { 4, 5 }
			})
			Dim expLabels As INDArray = Nd4j.create(New Single()() {
				New Single() { 1.1f, 2.1f, 3.1f },
				New Single() { 4.1f, 5.1f, 6.1f },
				New Single() { 7.1f, 8.1f, 9.1f }
			})
			assertEquals(expFeatures, ds.Features)
			assertEquals(expLabels, ds.Labels)
			' ALSO: test if we have NDArrayWritables for BOTH the features and the labels
			data = New List(Of ICollection(Of Writable))()
			data.Add(java.util.Arrays.asList(Of Writable)(New NDArrayWritable(Nd4j.create(New Double() { 0, 1 }, New Long() { 1, 2 })), New NDArrayWritable(Nd4j.create(New Double() { 1.1, 2.1, 3.1 }, New Long() { 1, 3 }))))
			data.Add(java.util.Arrays.asList(Of Writable)(New NDArrayWritable(Nd4j.create(New Double() { 2, 3 }, New Long() { 1, 2 })), New NDArrayWritable(Nd4j.create(New Double() { 4.1, 5.1, 6.1 }, New Long() { 1, 3 }))))
			data.Add(java.util.Arrays.asList(Of Writable)(New NDArrayWritable(Nd4j.create(New Double() { 4, 5 }, New Long() { 1, 2 })), New NDArrayWritable(Nd4j.create(New Double() { 7.1, 8.1, 9.1 }, New Long() { 1, 3 }))))
			labelIndexFrom = 1
			labelIndexTo = 1
			rr = New CollectionRecordReader(data)
			rrdsi = New RecordReaderDataSetIterator(rr, batchSize, labelIndexFrom, labelIndexTo, regression)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds2 As DataSet = rrdsi.next()
			assertEquals(expFeatures, ds2.Features)
			assertEquals(expLabels, ds2.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled @DisplayName("Special RR Test 4") void specialRRTest4(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub specialRRTest4(ByVal backend As Nd4jBackend)
			Dim rr As RecordReader = New SpecialImageRecordReader(25000, 10, 3, 224, 224)
			Dim rrdsi As New RecordReaderDataSetIterator(rr, 128)
			Dim cnt As Integer = 0
			Dim examples As Integer = 0
			Do While rrdsi.MoveNext()
				Dim ds As DataSet = rrdsi.Current
				assertEquals(128, ds.numExamples())
				Dim i As Integer = 0
				Do While i < ds.numExamples()
					Dim example As INDArray = ds.Features.tensorAlongDimension(i, 1, 2, 3).dup()
					' assertEquals("Failed on DataSet [" + cnt + "], example [" + i + "]", (double) examples, example.meanNumber().doubleValue(), 0.01);
					' assertEquals("Failed on DataSet [" + cnt + "], example [" + i + "]", (double) examples, ds.getLabels().getRow(i).meanNumber().doubleValue(), 0.01);
					examples += 1
					i += 1
				Loop
				cnt += 1
			Loop
		End Sub

	'    
	'    @Test
	'    public void specialRRTest1() throws Exception {
	'        RecordReader rr = new SpecialImageRecordReader(250, 10,3, 224, 224);
	'        DataSetIterator rrdsi = new ParallelRecordReaderDataSetIterator.Builder(rr)
	'                .setBatchSize(10)
	'                .numberOfWorkers(1)
	'                .build();
	'    
	'        int cnt = 0;
	'        int examples = 0;
	'        while (rrdsi.hasNext()) {
	'            DataSet ds = rrdsi.next();
	'            for (int i = 0; i < ds.numExamples(); i++) {
	'                INDArray example = ds.getFeatures().tensorAlongDimension(i, 1, 2, 3).dup();
	'                assertEquals("Failed on DataSet ["+ cnt + "], example ["+ i +"]",(double) examples, example.meanNumber().doubleValue(), 0.01);
	'                examples++;
	'            }
	'            cnt++;
	'            log.info("DataSet {} passed...", cnt);
	'        }
	'    
	'        assertEquals(25, cnt);
	'    }
	'    
	'    
	'    @Test
	'    public void specialRRTest2() throws Exception {
	'        RecordReader rr = new SpecialImageRecordReader(250, 10,3, 224, 224);
	'        DataSetIterator rrdsi = new ParallelRecordReaderDataSetIterator.Builder(rr)
	'                .setBatchSize(10)
	'                .numberOfWorkers(1)
	'                .prefetchBufferSize(4)
	'                .build();
	'    
	'        rrdsi = new AsyncDataSetIterator(rrdsi);
	'    
	'        int cnt = 0;
	'        int examples = 0;
	'        while (rrdsi.hasNext()) {
	'            DataSet ds = rrdsi.next();
	'            for (int i = 0; i < ds.numExamples(); i++) {
	'                INDArray example = ds.getFeatures().tensorAlongDimension(i, 1, 2, 3).dup();
	'                assertEquals("Failed on DataSet ["+ cnt + "], example ["+ i +"]",(double) examples, example.meanNumber().doubleValue(), 0.01);
	'                examples++;
	'            }
	'            cnt++;
	'        }
	'    
	'        assertEquals(25, cnt);
	'    }
	'    
	'    
	'    @Test
	'    public void specialRRTest3() throws Exception {
	'        RecordReader rr = new SpecialImageRecordReader(400, 10,3, 224, 224);
	'        DataSetIterator rrdsi = new ParallelRecordReaderDataSetIterator.Builder(rr)
	'                .setBatchSize(128)
	'                .numberOfWorkers(2)
	'                .prefetchBufferSize(2)
	'                .build();
	'    
	'        log.info("DataType: {}", Nd4j.dataType() );
	'    
	'       // rrdsi = new AsyncDataSetIterator(rrdsi);
	'    
	'        int cnt = 0;
	'        int examples = 0;
	'        while (rrdsi.hasNext()) {
	'            DataSet ds = rrdsi.next();
	'            for (int i = 0; i < ds.numExamples(); i++) {
	'                INDArray example = ds.getFeatures().tensorAlongDimension(i, 1, 2, 3).dup();
	'                assertEquals("Failed on DataSet ["+ cnt + "], example ["+ i +"]",(double) examples, example.meanNumber().doubleValue(), 0.01);
	'                examples++;
	'            }
	'            cnt++;
	'        }
	'    
	'    }
	'    
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader Data Set Iterator Concat") void testRecordReaderDataSetIteratorConcat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testRecordReaderDataSetIteratorConcat(ByVal backend As Nd4jBackend)
			' [DoubleWritable, DoubleWritable, NDArrayWritable([1,10]), IntWritable] -> concatenate to a [1,13] feature vector automatically.
			Dim l As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim rr As RecordReader = New CollectionRecordReader(Collections.singletonList(l))
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 1, 5, 3)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim expF As INDArray = Nd4j.create(New Single() { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, New Integer() { 1, 9 })
			Dim expL As INDArray = Nd4j.create(New Single() { 0, 1, 0 }, New Integer() { 1, 3 })
			assertEquals(expF, ds.Features)
			assertEquals(expL, ds.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader Data Set Iterator Concat 2") void testRecordReaderDataSetIteratorConcat2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testRecordReaderDataSetIteratorConcat2(ByVal backend As Nd4jBackend)
			Dim l As IList(Of Writable) = New List(Of Writable)()
			l.Add(New IntWritable(0))
			l.Add(New NDArrayWritable(Nd4j.arange(1, 9)))
			l.Add(New IntWritable(9))
			Dim rr As RecordReader = New CollectionRecordReader(Collections.singletonList(l))
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 1)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim expF As INDArray = Nd4j.create(New Single() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, New Integer() { 1, 10 })
			assertEquals(expF, ds.Features)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Record Reader Data Set Iterator Disjoint Features") void testRecordReaderDataSetIteratorDisjointFeatures(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testRecordReaderDataSetIteratorDisjointFeatures(ByVal backend As Nd4jBackend)
			' Idea: input vector is like [f,f,f,f,l,l,f,f] or similar - i.e., label writables aren't start/end
			Dim l As IList(Of Writable) = New List(Of Writable) From {
				New DoubleWritable(1),
				New NDArrayWritable(Nd4j.create(New Single() { 2, 3, 4 }, New Long() { 1, 3 })),
				New DoubleWritable(5),
				New NDArrayWritable(Nd4j.create(New Single() { 6, 7, 8 }, New Long() { 1, 3 }))
			}
			Dim expF As INDArray = Nd4j.create(New Single() { 1, 6, 7, 8 }, New Long() { 1, 4 })
			Dim expL As INDArray = Nd4j.create(New Single() { 2, 3, 4, 5 }, New Long() { 1, 4 })
			Dim rr As RecordReader = New CollectionRecordReader(Collections.singletonList(l))
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 1, 1, 2, True)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			assertEquals(expF, ds.Features)
			assertEquals(expL, ds.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Normalizer Prefetch Reset") void testNormalizerPrefetchReset(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNormalizerPrefetchReset(ByVal backend As Nd4jBackend)
			' Check NPE fix for: https://github.com/eclipse/deeplearning4j/issues/4214
			Dim csv As RecordReader = New CSVRecordReader()
			csv.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim batchSize As Integer = 3
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(csv, batchSize, 4, 4, True)
			Dim normalizer As DataNormalization = New NormalizerMinMaxScaler(0, 1)
			normalizer.fit(iter)
			iter.PreProcessor = normalizer
			' Prefetch
			iter.inputColumns()
			iter.totalOutcomes()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			iter.hasNext()
			iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			iter.next()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Reading From Stream") void testReadingFromStream(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReadingFromStream(ByVal backend As Nd4jBackend)
			For Each b As Boolean In New Boolean() { False, True }
				Dim batchSize As Integer = 1
				Dim labelIndex As Integer = 4
				Dim numClasses As Integer = 3
				Dim dataFile As Stream = Resources.asStream("iris.txt")
				Dim recordReader As RecordReader = New CSVRecordReader(0, ","c)
				recordReader.initialize(New org.datavec.api.Split.InputStreamInputSplit(dataFile))
				assertTrue(recordReader.hasNext())
				assertFalse(recordReader.resetSupported())
				Dim iterator As DataSetIterator
				If b Then
					iterator = (New RecordReaderDataSetIterator.Builder(recordReader, batchSize)).classification(labelIndex, numClasses).build()
				Else
					iterator = New RecordReaderDataSetIterator(recordReader, batchSize, labelIndex, numClasses)
				End If
				assertFalse(iterator.resetSupported())
				Dim count As Integer = 0
				Do While iterator.MoveNext()
					assertNotNull(iterator.Current)
					count += 1
				Loop
				assertEquals(150, count)
				Try
					iterator.reset()
					fail("Expected exception")
				Catch e As Exception
					' expected
				End Try
			Next b
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Images RRDSI") void testImagesRRDSI(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testImagesRRDSI(ByVal backend As Nd4jBackend)
			Dim parentDir As File = temporaryFolder.toFile()
			parentDir.deleteOnExit()
			Dim str1 As String = FilenameUtils.concat(parentDir.getAbsolutePath(), "Zico/")
			Dim str2 As String = FilenameUtils.concat(parentDir.getAbsolutePath(), "Ziwang_Xu/")
			Dim f2 As New File(str2)
			Dim f1 As New File(str1)
			f1.mkdirs()
			f2.mkdirs()
			TestUtils.writeStreamToFile(New File(FilenameUtils.concat(f1.getPath(), "Zico_0001.jpg")), (New ClassPathResource("lfwtest/Zico/Zico_0001.jpg")).InputStream)
			TestUtils.writeStreamToFile(New File(FilenameUtils.concat(f2.getPath(), "Ziwang_Xu_0001.jpg")), (New ClassPathResource("lfwtest/Ziwang_Xu/Ziwang_Xu_0001.jpg")).InputStream)
			Dim r As New Random(12345)
			Dim labelMaker As New ParentPathLabelGenerator()
			Dim rr1 As New ImageRecordReader(28, 28, 3, labelMaker)
			rr1.initialize(New org.datavec.api.Split.FileSplit(parentDir))
			Dim rrdsi As New RecordReaderDataSetIterator(rr1, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = rrdsi.next()
			assertArrayEquals(New Long() { 2, 3, 28, 28 }, ds.Features.shape())
			assertArrayEquals(New Long() { 2, 2 }, ds.Labels.shape())
			' Check the same thing via the builder:
			rr1.reset()
			rrdsi = (New RecordReaderDataSetIterator.Builder(rr1, 2)).classification(1, 2).build()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			ds = rrdsi.next()
			assertArrayEquals(New Long() { 2, 3, 28, 28 }, ds.Features.shape())
			assertArrayEquals(New Long() { 2, 2 }, ds.Labels.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Seq RRDSI No Labels") void testSeqRRDSINoLabels(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSeqRRDSINoLabels(ByVal backend As Nd4jBackend)
			Dim sequence1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence1.Add(New List(Of Writable) From {
				New DoubleWritable(1),
				New DoubleWritable(2)
			})
			sequence1.Add(New List(Of Writable) From {
				New DoubleWritable(3),
				New DoubleWritable(4)
			})
			sequence1.Add(New List(Of Writable) From {
				New DoubleWritable(5),
				New DoubleWritable(6)
			})
			Dim sequence2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence2.Add(New List(Of Writable) From {
				New DoubleWritable(10),
				New DoubleWritable(20)
			})
			sequence2.Add(New List(Of Writable) From {
				New DoubleWritable(30),
				New DoubleWritable(40)
			})
			Dim rrFeatures As SequenceRecordReader = New CollectionSequenceRecordReader(java.util.Arrays.asList(sequence1, sequence2))
			Dim iter As New SequenceRecordReaderDataSetIterator(rrFeatures, 2, -1, -1)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			assertNotNull(ds.Features)
			assertNull(ds.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Collect Meta Data") void testCollectMetaData(org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCollectMetaData(ByVal backend As Nd4jBackend)
			Dim trainIter As RecordReaderDataSetIterator = (New RecordReaderDataSetIterator.Builder(New CollectionRecordReader(Enumerable.Empty(Of IList(Of Writable))()), 1)).collectMetaData(True).build()
			assertTrue(trainIter.isCollectMetaData())
			trainIter.CollectMetaData = False
			assertFalse(trainIter.isCollectMetaData())
			trainIter.CollectMetaData = True
			assertTrue(trainIter.isCollectMetaData())
		End Sub
	End Class

End Namespace