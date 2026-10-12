Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Files = org.nd4j.shade.guava.io.Files
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports Configuration = org.datavec.api.conf.Configuration
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CollectionSequenceRecordReader = org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports CollectionInputSplit = org.datavec.api.split.CollectionInputSplit
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resources = org.nd4j.common.resources.Resources
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval
import static org.nd4j.linalg.indexing.NDArrayIndex.point
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
Namespace org.deeplearning4j.datasets.datavec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Record Reader Multi Data Set Iterator Test") @Disabled @Tag(TagNames.FILE_IO) class RecordReaderMultiDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class RecordReaderMultiDataSetIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path temporaryFolder;
		Public temporaryFolder As Path



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Tests Basic") void testsBasic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testsBasic()
			' Load details from CSV files; single input/output -> compare to RecordReaderDataSetIterator
			Dim rr As RecordReader = New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim rrdsi As New RecordReaderDataSetIterator(rr, 10, 4, 3)
			Dim rr2 As RecordReader = New CSVRecordReader(0, ","c)
			rr2.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim rrmdsi As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(10)).addReader("reader", rr2).addInput("reader", 0, 3).addOutputOneHot("reader", 4, 3).build()
			Do While rrdsi.MoveNext()
				Dim ds As DataSet = rrdsi.Current
				Dim fds As INDArray = ds.Features
				Dim lds As INDArray = ds.Labels
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mds As MultiDataSet = rrmdsi.next()
				assertEquals(1, mds.Features.Length)
				assertEquals(1, mds.Labels.Length)
				assertNull(mds.FeaturesMaskArrays)
				assertNull(mds.LabelsMaskArrays)
				Dim fmds As INDArray = mds.getFeatures(0)
				Dim lmds As INDArray = mds.getLabels(0)
				assertNotNull(fmds)
				assertNotNull(lmds)
				assertEquals(fds, fmds)
				assertEquals(lds, lmds)
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(rrmdsi.hasNext())
			' need to manually extract
			Dim rootDir As File = temporaryFolder.toFile()
			For i As Integer = 0 To 2
				Call (New ClassPathResource(String.Format("csvsequence_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabels_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabelsShort_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
			Next i
			' Load time series from CSV sequence files; compare to SequenceRecordReaderDataSetIterator
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabels_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False)
			Dim featureReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim srrmdsi As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addSequenceReader("in", featureReader2).addSequenceReader("out", labelReader2).addInput("in").addOutputOneHot("out", 0, 4).build()
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				Dim fds As INDArray = ds.Features
				Dim lds As INDArray = ds.Labels
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mds As MultiDataSet = srrmdsi.next()
				assertEquals(1, mds.Features.Length)
				assertEquals(1, mds.Labels.Length)
				assertNull(mds.FeaturesMaskArrays)
				assertNull(mds.LabelsMaskArrays)
				Dim fmds As INDArray = mds.getFeatures(0)
				Dim lmds As INDArray = mds.getLabels(0)
				assertNotNull(fmds)
				assertNotNull(lmds)
				assertEquals(fds, fmds)
				assertEquals(lds, lmds)
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(srrmdsi.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Tests Basic Meta") void testsBasicMeta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testsBasicMeta()
			' As per testBasic - but also loading metadata
			Dim rr2 As RecordReader = New CSVRecordReader(0, ","c)
			rr2.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim rrmdsi As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(10)).addReader("reader", rr2).addInput("reader", 0, 3).addOutputOneHot("reader", 4, 3).build()
			rrmdsi.setCollectMetaData(True)
			Dim count As Integer = 0
			Do While rrmdsi.MoveNext()
				Dim mds As MultiDataSet = rrmdsi.Current
				Dim fromMeta As MultiDataSet = rrmdsi.loadFromMetaData(mds.getExampleMetaData(GetType(RecordMetaData)))
				assertEquals(mds, fromMeta)
				count += 1
			Loop
			assertEquals(150 \ 10, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Splitting CSV") void testSplittingCSV() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSplittingCSV()
			' Here's the idea: take Iris, and split it up into 2 inputs and 2 output arrays
			' Inputs: columns 0 and 1-2
			' Outputs: columns 3, and 4->OneHot
			' need to manually extract
			Dim rr As RecordReader = New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim rrdsi As New RecordReaderDataSetIterator(rr, 10, 4, 3)
			Dim rr2 As RecordReader = New CSVRecordReader(0, ","c)
			rr2.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim rrmdsi As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(10)).addReader("reader", rr2).addInput("reader", 0, 0).addInput("reader", 1, 2).addOutput("reader", 3, 3).addOutputOneHot("reader", 4, 3).build()
			Do While rrdsi.MoveNext()
				Dim ds As DataSet = rrdsi.Current
				Dim fds As INDArray = ds.Features
				Dim lds As INDArray = ds.Labels
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mds As MultiDataSet = rrmdsi.next()
				assertEquals(2, mds.Features.Length)
				assertEquals(2, mds.Labels.Length)
				assertNull(mds.FeaturesMaskArrays)
				assertNull(mds.LabelsMaskArrays)
				Dim fmds() As INDArray = mds.Features
				Dim lmds() As INDArray = mds.Labels
				assertNotNull(fmds)
				assertNotNull(lmds)
				For i As Integer = 0 To fmds.Length - 1
					assertNotNull(fmds(i))
				Next i
				For i As Integer = 0 To lmds.Length - 1
					assertNotNull(lmds(i))
				Next i
				' Get the subsets of the original iris data
				Dim expIn1 As INDArray = fds.get(all(), interval(0, 0, True))
				Dim expIn2 As INDArray = fds.get(all(), interval(1, 2, True))
				Dim expOut1 As INDArray = fds.get(all(), interval(3, 3, True))
				Dim expOut2 As INDArray = lds
				assertEquals(expIn1, fmds(0))
				assertEquals(expIn2, fmds(1))
				assertEquals(expOut1, lmds(0))
				assertEquals(expOut2, lmds(1))
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(rrmdsi.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Splitting CSV Meta") void testSplittingCSVMeta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSplittingCSVMeta()
			' Here's the idea: take Iris, and split it up into 2 inputs and 2 output arrays
			' Inputs: columns 0 and 1-2
			' Outputs: columns 3, and 4->OneHot
			Dim rr2 As RecordReader = New CSVRecordReader(0, ","c)
			rr2.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim rrmdsi As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(10)).addReader("reader", rr2).addInput("reader", 0, 0).addInput("reader", 1, 2).addOutput("reader", 3, 3).addOutputOneHot("reader", 4, 3).build()
			rrmdsi.setCollectMetaData(True)
			Dim count As Integer = 0
			Do While rrmdsi.MoveNext()
				Dim mds As MultiDataSet = rrmdsi.Current
				Dim fromMeta As MultiDataSet = rrmdsi.loadFromMetaData(mds.getExampleMetaData(GetType(RecordMetaData)))
				assertEquals(mds, fromMeta)
				count += 1
			Loop
			assertEquals(150 \ 10, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Splitting CSV Sequence") void testSplittingCSVSequence() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSplittingCSVSequence()
			' Idea: take CSV sequences, and split "csvsequence_i.txt" into two separate inputs; keep "csvSequencelables_i.txt"
			' as standard one-hot output
			' need to manually extract
			Dim rootDir As File = temporaryFolder.toFile()
			For i As Integer = 0 To 2
				Call (New ClassPathResource(String.Format("csvsequence_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabels_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabelsShort_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabels_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False)
			Dim featureReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim srrmdsi As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addSequenceReader("seq1", featureReader2).addSequenceReader("seq2", labelReader2).addInput("seq1", 0, 1).addInput("seq1", 2, 2).addOutputOneHot("seq2", 0, 4).build()
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				Dim fds As INDArray = ds.Features
				Dim lds As INDArray = ds.Labels
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mds As MultiDataSet = srrmdsi.next()
				assertEquals(2, mds.Features.Length)
				assertEquals(1, mds.Labels.Length)
				assertNull(mds.FeaturesMaskArrays)
				assertNull(mds.LabelsMaskArrays)
				Dim fmds() As INDArray = mds.Features
				Dim lmds() As INDArray = mds.Labels
				assertNotNull(fmds)
				assertNotNull(lmds)
				For i As Integer = 0 To fmds.Length - 1
					assertNotNull(fmds(i))
				Next i
				For i As Integer = 0 To lmds.Length - 1
					assertNotNull(lmds(i))
				Next i
				Dim expIn1 As INDArray = fds.get(all(), NDArrayIndex.interval(0, 1, True), all())
				Dim expIn2 As INDArray = fds.get(all(), NDArrayIndex.interval(2, 2, True), all())
				assertEquals(expIn1, fmds(0))
				assertEquals(expIn2, fmds(1))
				assertEquals(lds, lmds(0))
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(srrmdsi.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Splitting CSV Sequence Meta") void testSplittingCSVSequenceMeta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSplittingCSVSequenceMeta()
			' Idea: take CSV sequences, and split "csvsequence_i.txt" into two separate inputs; keep "csvSequencelables_i.txt"
			' as standard one-hot output
			' need to manually extract
			Dim rootDir As File = temporaryFolder.toFile()
			For i As Integer = 0 To 2
				Call (New ClassPathResource(String.Format("csvsequence_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabels_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabelsShort_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabels_%d.txt")
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim featureReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader2.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim srrmdsi As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addSequenceReader("seq1", featureReader2).addSequenceReader("seq2", labelReader2).addInput("seq1", 0, 1).addInput("seq1", 2, 2).addOutputOneHot("seq2", 0, 4).build()
			srrmdsi.setCollectMetaData(True)
			Dim count As Integer = 0
			Do While srrmdsi.MoveNext()
				Dim mds As MultiDataSet = srrmdsi.Current
				Dim fromMeta As MultiDataSet = srrmdsi.loadFromMetaData(mds.getExampleMetaData(GetType(RecordMetaData)))
				assertEquals(mds, fromMeta)
				count += 1
			Loop
			assertEquals(3, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Validation") void testInputValidation()
		Friend Overridable Sub testInputValidation()
			' Test: no readers
			Try
				Dim r As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addInput("something").addOutput("something").build()
				fail("Should have thrown exception")
			Catch e As Exception
			End Try
			' Test: reference to reader that doesn't exist
			Try
				Dim rr As RecordReader = New CSVRecordReader(0, ","c)
				rr.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
				Dim r As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("iris", rr).addInput("thisDoesntExist", 0, 3).addOutputOneHot("iris", 4, 3).build()
				fail("Should have thrown exception")
			Catch e As Exception
			End Try
			' Test: no inputs or outputs
			Try
				Dim rr As RecordReader = New CSVRecordReader(0, ","c)
				rr.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
				Dim r As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("iris", rr).build()
				fail("Should have thrown exception")
			Catch e As Exception
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Variable Length TS") void testVariableLengthTS() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testVariableLengthTS()
			' need to manually extract
			Dim rootDir As File = temporaryFolder.toFile()
			For i As Integer = 0 To 2
				Call (New ClassPathResource(String.Format("csvsequence_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabels_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabelsShort_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
			Next i
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabelsShort_%d.txt")
			' Set up SequenceRecordReaderDataSetIterators for comparison
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
			' Set up
			Dim featureReader3 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader3 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader3.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader3.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim featureReader4 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader4 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader4.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader4.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim rrmdsiStart As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addSequenceReader("in", featureReader3).addSequenceReader("out", labelReader3).addInput("in").addOutputOneHot("out", 0, 4).sequenceAlignmentMode(RecordReaderMultiDataSetIterator.AlignmentMode.ALIGN_START).build()
			Dim rrmdsiEnd As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addSequenceReader("in", featureReader4).addSequenceReader("out", labelReader4).addInput("in").addOutputOneHot("out", 0, 4).sequenceAlignmentMode(RecordReaderMultiDataSetIterator.AlignmentMode.ALIGN_END).build()
			Do While iterAlignStart.MoveNext()
				Dim dsStart As DataSet = iterAlignStart.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim dsEnd As DataSet = iterAlignEnd.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mdsStart As MultiDataSet = rrmdsiStart.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mdsEnd As MultiDataSet = rrmdsiEnd.next()
				assertEquals(1, mdsStart.Features.Length)
				assertEquals(1, mdsStart.Labels.Length)
				' assertEquals(1, mdsStart.getFeaturesMaskArrays().length); //Features data is always longer -> don't need mask arrays for it
				assertEquals(1, mdsStart.LabelsMaskArrays.Length)
				assertEquals(1, mdsEnd.Features.Length)
				assertEquals(1, mdsEnd.Labels.Length)
				' assertEquals(1, mdsEnd.getFeaturesMaskArrays().length);
				assertEquals(1, mdsEnd.LabelsMaskArrays.Length)
				assertEquals(dsStart.Features, mdsStart.getFeatures(0))
				assertEquals(dsStart.Labels, mdsStart.getLabels(0))
				assertEquals(dsStart.LabelsMaskArray, mdsStart.getLabelsMaskArray(0))
				assertEquals(dsEnd.Features, mdsEnd.getFeatures(0))
				assertEquals(dsEnd.Labels, mdsEnd.getLabels(0))
				assertEquals(dsEnd.LabelsMaskArray, mdsEnd.getLabelsMaskArray(0))
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(rrmdsiStart.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(rrmdsiEnd.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Variable Length TS Meta") void testVariableLengthTSMeta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testVariableLengthTSMeta()
			' need to manually extract
			Dim rootDir As File = temporaryFolder.toFile()
			For i As Integer = 0 To 2
				Call (New ClassPathResource(String.Format("csvsequence_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabels_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
				Call (New ClassPathResource(String.Format("csvsequencelabelsShort_{0:D}.txt", i))).getTempFileFromArchive(rootDir)
			Next i
			' Set up SequenceRecordReaderDataSetIterators for comparison
			Dim featuresPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(rootDir.getAbsolutePath(), "csvsequencelabelsShort_%d.txt")
			' Set up
			Dim featureReader3 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader3 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader3.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader3.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim featureReader4 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader4 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader4.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader4.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim rrmdsiStart As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addSequenceReader("in", featureReader3).addSequenceReader("out", labelReader3).addInput("in").addOutputOneHot("out", 0, 4).sequenceAlignmentMode(RecordReaderMultiDataSetIterator.AlignmentMode.ALIGN_START).build()
			Dim rrmdsiEnd As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addSequenceReader("in", featureReader4).addSequenceReader("out", labelReader4).addInput("in").addOutputOneHot("out", 0, 4).sequenceAlignmentMode(RecordReaderMultiDataSetIterator.AlignmentMode.ALIGN_END).build()
			rrmdsiStart.setCollectMetaData(True)
			rrmdsiEnd.setCollectMetaData(True)
			Dim count As Integer = 0
			Do While rrmdsiStart.MoveNext()
				Dim mdsStart As MultiDataSet = rrmdsiStart.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mdsEnd As MultiDataSet = rrmdsiEnd.next()
				Dim mdsStartFromMeta As MultiDataSet = rrmdsiStart.loadFromMetaData(mdsStart.getExampleMetaData(GetType(RecordMetaData)))
				Dim mdsEndFromMeta As MultiDataSet = rrmdsiEnd.loadFromMetaData(mdsEnd.getExampleMetaData(GetType(RecordMetaData)))
				assertEquals(mdsStart, mdsStartFromMeta)
				assertEquals(mdsEnd, mdsEndFromMeta)
				count += 1
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(rrmdsiStart.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(rrmdsiEnd.hasNext())
			assertEquals(3, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Images RRDMSI") void testImagesRRDMSI() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testImagesRRDMSI()
			Dim parentDir As File = temporaryFolder.toFile()
			parentDir.deleteOnExit()
			Dim str1 As String = FilenameUtils.concat(parentDir.getAbsolutePath(), "Zico/")
			Dim str2 As String = FilenameUtils.concat(parentDir.getAbsolutePath(), "Ziwang_Xu/")
			Dim f1 As New File(str1)
			Dim f2 As New File(str2)
			f1.mkdirs()
			f2.mkdirs()
			TestUtils.writeStreamToFile(New File(FilenameUtils.concat(f1.getPath(), "Zico_0001.jpg")), (New ClassPathResource("lfwtest/Zico/Zico_0001.jpg")).InputStream)
			TestUtils.writeStreamToFile(New File(FilenameUtils.concat(f2.getPath(), "Ziwang_Xu_0001.jpg")), (New ClassPathResource("lfwtest/Ziwang_Xu/Ziwang_Xu_0001.jpg")).InputStream)
			Dim outputNum As Integer = 2
			Dim r As New Random(12345)
			Dim labelMaker As New ParentPathLabelGenerator()
			Dim rr1 As New ImageRecordReader(10, 10, 1, labelMaker)
			Dim rr1s As New ImageRecordReader(5, 5, 1, labelMaker)
			rr1.initialize(New org.datavec.api.Split.FileSplit(parentDir))
			rr1s.initialize(New org.datavec.api.Split.FileSplit(parentDir))
			Dim trainDataIterator As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("rr1", rr1).addReader("rr1s", rr1s).addInput("rr1", 0, 0).addInput("rr1s", 0, 0).addOutputOneHot("rr1s", 1, outputNum).build()
			' Now, do the same thing with ImageRecordReader, and check we get the same results:
			Dim rr1_b As New ImageRecordReader(10, 10, 1, labelMaker)
			Dim rr1s_b As New ImageRecordReader(5, 5, 1, labelMaker)
			rr1_b.initialize(New org.datavec.api.Split.FileSplit(parentDir))
			rr1s_b.initialize(New org.datavec.api.Split.FileSplit(parentDir))
			Dim dsi1 As DataSetIterator = New RecordReaderDataSetIterator(rr1_b, 1, 1, 2)
			Dim dsi2 As DataSetIterator = New RecordReaderDataSetIterator(rr1s_b, 1, 1, 2)
			For i As Integer = 0 To 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mds As MultiDataSet = trainDataIterator.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim d1 As DataSet = dsi1.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim d2 As DataSet = dsi2.next()
				assertEquals(d1.Features, mds.getFeatures(0))
				assertEquals(d2.Features, mds.getFeatures(1))
				assertEquals(d1.Labels, mds.getLabels(0))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Images RRDMSI _ Batched") void testImagesRRDMSI_Batched() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testImagesRRDMSI_Batched()
			Dim parentDir As File = temporaryFolder.toFile()
			parentDir.deleteOnExit()
			Dim str1 As String = FilenameUtils.concat(parentDir.getAbsolutePath(), "Zico/")
			Dim str2 As String = FilenameUtils.concat(parentDir.getAbsolutePath(), "Ziwang_Xu/")
			Dim f1 As New File(str1)
			Dim f2 As New File(str2)
			f1.mkdirs()
			f2.mkdirs()
			TestUtils.writeStreamToFile(New File(FilenameUtils.concat(f1.getPath(), "Zico_0001.jpg")), (New ClassPathResource("lfwtest/Zico/Zico_0001.jpg")).InputStream)
			TestUtils.writeStreamToFile(New File(FilenameUtils.concat(f2.getPath(), "Ziwang_Xu_0001.jpg")), (New ClassPathResource("lfwtest/Ziwang_Xu/Ziwang_Xu_0001.jpg")).InputStream)
			Dim outputNum As Integer = 2
			Dim labelMaker As New ParentPathLabelGenerator()
			Dim rr1 As New ImageRecordReader(10, 10, 1, labelMaker)
			Dim rr1s As New ImageRecordReader(5, 5, 1, labelMaker)
			Dim uris() As URI = (New org.datavec.api.Split.FileSplit(parentDir)).locations()
			rr1.initialize(New org.datavec.api.Split.CollectionInputSplit(uris))
			rr1s.initialize(New org.datavec.api.Split.CollectionInputSplit(uris))
			Dim trainDataIterator As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(2)).addReader("rr1", rr1).addReader("rr1s", rr1s).addInput("rr1", 0, 0).addInput("rr1s", 0, 0).addOutputOneHot("rr1s", 1, outputNum).build()
			' Now, do the same thing with ImageRecordReader, and check we get the same results:
			Dim rr1_b As New ImageRecordReader(10, 10, 1, labelMaker)
			Dim rr1s_b As New ImageRecordReader(5, 5, 1, labelMaker)
			rr1_b.initialize(New org.datavec.api.Split.FileSplit(parentDir))
			rr1s_b.initialize(New org.datavec.api.Split.FileSplit(parentDir))
			Dim dsi1 As DataSetIterator = New RecordReaderDataSetIterator(rr1_b, 2, 1, 2)
			Dim dsi2 As DataSetIterator = New RecordReaderDataSetIterator(rr1s_b, 2, 1, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = trainDataIterator.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim d1 As DataSet = dsi1.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim d2 As DataSet = dsi2.next()
			assertEquals(d1.Features, mds.getFeatures(0))
			assertEquals(d2.Features, mds.getFeatures(1))
			assertEquals(d1.Labels, mds.getLabels(0))
			' Check label assignment:
			Dim currentFile As File = rr1_b.CurrentFile
			Dim expLabels As INDArray
			If currentFile.getAbsolutePath().contains("Zico") Then
				expLabels = Nd4j.create(New Double()() {
					New Double() { 0, 1 },
					New Double() { 1, 0 }
				})
			Else
				expLabels = Nd4j.create(New Double()() {
					New Double() { 1, 0 },
					New Double() { 0, 1 }
				})
			End If
			assertEquals(expLabels, d1.Labels)
			assertEquals(expLabels, d2.Labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Time Series Random Offset") void testTimeSeriesRandomOffset()
		Friend Overridable Sub testTimeSeriesRandomOffset()
			' 2 in, 2 out, 3 total sequences of length [1,3,5]
			Dim seq1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(1.0), New DoubleWritable(2.0))}
			Dim seq2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(10.0), New DoubleWritable(11.0)), Arrays.asList(Of Writable)(New DoubleWritable(20.0), New DoubleWritable(21.0)), Arrays.asList(Of Writable)(New DoubleWritable(30.0), New DoubleWritable(31.0))}
			Dim seq3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(100.0), New DoubleWritable(101.0)), Arrays.asList(Of Writable)(New DoubleWritable(200.0), New DoubleWritable(201.0)), Arrays.asList(Of Writable)(New DoubleWritable(300.0), New DoubleWritable(301.0)), Arrays.asList(Of Writable)(New DoubleWritable(400.0), New DoubleWritable(401.0)), Arrays.asList(Of Writable)(New DoubleWritable(500.0), New DoubleWritable(501.0))}
			Dim seqs As ICollection(Of IList(Of IList(Of Writable))) = java.util.Arrays.asList(seq1, seq2, seq3)
			Dim rr As SequenceRecordReader = New CollectionSequenceRecordReader(seqs)
			Dim rrmdsi As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(3)).addSequenceReader("rr", rr).addInput("rr", 0, 0).addOutput("rr", 1, 1).timeSeriesRandomOffset(True, 1234L).build()
			' Provides seed for each minibatch
			Dim r As New Random(1234)
			Dim seed As Long = r.nextLong()
			' Use same RNG seed in new RNG for each minibatch
			Dim r2 As New Random(CInt(seed))
			' 0 to 4 inclusive
			Dim expOffsetSeq1 As Integer = r2.Next(5 - 1 + 1)
			Dim expOffsetSeq2 As Integer = r2.Next(5 - 3 + 1)
			' Longest TS, always 0
			Dim expOffsetSeq3 As Integer = 0
			' With current seed: 3, 1, 0
			' System.out.println(expOffsetSeq1 + "\t" + expOffsetSeq2 + "\t" + expOffsetSeq3);
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = rrmdsi.next()
			Dim expMask As INDArray = Nd4j.create(New Double()() {
				New Double() { 0, 0, 0, 1, 0 },
				New Double() { 0, 1, 1, 1, 0 },
				New Double() { 1, 1, 1, 1, 1 }
			})
			assertEquals(expMask, mds.getFeaturesMaskArray(0))
			assertEquals(expMask, mds.getLabelsMaskArray(0))
			Dim f As INDArray = mds.getFeatures(0)
			Dim l As INDArray = mds.getLabels(0)
			Dim expF1 As INDArray = Nd4j.create(New Double() { 1.0 }, New Integer() { 1, 1 })
			Dim expL1 As INDArray = Nd4j.create(New Double() { 2.0 }, New Integer() { 1, 1 })
			Dim expF2 As INDArray = Nd4j.create(New Double() { 10, 20, 30 }, New Integer() { 1, 3 })
			Dim expL2 As INDArray = Nd4j.create(New Double() { 11, 21, 31 }, New Integer() { 1, 3 })
			Dim expF3 As INDArray = Nd4j.create(New Double() { 100, 200, 300, 400, 500 }, New Integer() { 1, 5 })
			Dim expL3 As INDArray = Nd4j.create(New Double() { 101, 201, 301, 401, 501 }, New Integer() { 1, 5 })
			assertEquals(expF1, f.get(point(0), all(), NDArrayIndex.interval(expOffsetSeq1, expOffsetSeq1 + 1)))
			assertEquals(expL1, l.get(point(0), all(), NDArrayIndex.interval(expOffsetSeq1, expOffsetSeq1 + 1)))
			assertEquals(expF2, f.get(point(1), all(), NDArrayIndex.interval(expOffsetSeq2, expOffsetSeq2 + 3)))
			assertEquals(expL2, l.get(point(1), all(), NDArrayIndex.interval(expOffsetSeq2, expOffsetSeq2 + 3)))
			assertEquals(expF3, f.get(point(2), all(), NDArrayIndex.interval(expOffsetSeq3, expOffsetSeq3 + 5)))
			assertEquals(expL3, l.get(point(2), all(), NDArrayIndex.interval(expOffsetSeq3, expOffsetSeq3 + 5)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Seq RRDSI Masking") void testSeqRRDSIMasking()
		Friend Overridable Sub testSeqRRDSIMasking()
			' This also tests RecordReaderMultiDataSetIterator, by virtue of
			Dim features As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim labels As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			features.Add(New List(Of IList(Of Writable)) From {l(New DoubleWritable(1)), l(New DoubleWritable(2)), l(New DoubleWritable(3))})
			features.Add(New List(Of IList(Of Writable)) From {l(New DoubleWritable(4)), l(New DoubleWritable(5))})
			labels.Add(New List(Of IList(Of Writable)) From {l(New IntWritable(0))})
			labels.Add(New List(Of IList(Of Writable)) From {l(New IntWritable(1))})
			Dim fR As New CollectionSequenceRecordReader(features)
			Dim lR As New CollectionSequenceRecordReader(labels)
			Dim seqRRDSI As New SequenceRecordReaderDataSetIterator(fR, lR, 2, 2, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = seqRRDSI.next()
			Dim fMask As INDArray = Nd4j.create(New Double()() {
				New Double() { 1, 1, 1 },
				New Double() { 1, 1, 0 }
			})
			Dim lMask As INDArray = Nd4j.create(New Double()() {
				New Double() { 0, 0, 1 },
				New Double() { 0, 1, 0 }
			})
			assertEquals(fMask, ds.FeaturesMaskArray)
			assertEquals(lMask, ds.LabelsMaskArray)
			Dim f As INDArray = Nd4j.create(New Double()() {
				New Double() { 1, 2, 3 },
				New Double() { 4, 5, 0 }
			})
			Dim l As INDArray = Nd4j.create(2, 2, 3)
			l.putScalar(0, 0, 2, 1.0)
			l.putScalar(1, 1, 1, 1.0)
			assertEquals(f, ds.Features.get(all(), point(0), all()))
			assertEquals(l, ds.Labels)
		End Sub

		Private Shared Function l(ParamArray ByVal [in]() As Writable) As IList(Of Writable)
			Return New List(Of Writable) From {[in]}
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Exclude String Col CSV") void testExcludeStringColCSV() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testExcludeStringColCSV()
			Dim csvFile As File = temporaryFolder.toFile()
			Dim sb As New StringBuilder()
			For i As Integer = 1 To 10
				If i > 1 Then
					sb.Append(vbLf)
				End If
				sb.Append("skip_").Append(i).Append(",").Append(i).Append(",").Append(i + 0.5)
			Next i
			FileUtils.writeStringToFile(csvFile, sb.ToString())
			Dim rr As RecordReader = New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit(csvFile))
			Dim rrmdsi As RecordReaderMultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(10)).addReader("rr", rr).addInput("rr", 1, 1).addOutput("rr", 2, 2).build()
			Dim expFeatures As INDArray = Nd4j.linspace(1, 10, 10).reshape(ChrW(1), 10).transpose()
			Dim expLabels As INDArray = Nd4j.linspace(1, 10, 10).addi(0.5).reshape(1, 10).transpose()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = rrmdsi.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(rrmdsi.hasNext())
			assertEquals(expFeatures, mds.getFeatures(0).castTo(expFeatures.dataType()))
			assertEquals(expLabels, mds.getLabels(0).castTo(expLabels.dataType()))
		End Sub

		Private Const nX As Integer = 32

		Private Const nY As Integer = 32

		Private Const nZ As Integer = 28

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test RRMDSI 5 D") void testRRMDSI5D()
		Friend Overridable Sub testRRMDSI5D()
			Dim batchSize As Integer = 5
			Dim recordReader As New CustomRecordReader()
			Dim dataIter As DataSetIterator = New RecordReaderDataSetIterator(recordReader, batchSize, 1, 2)
			Dim count As Integer = 0
			Do While dataIter.MoveNext()
				Dim ds As DataSet = dataIter.Current
				Dim offset As Integer = 5 * count
				For i As Integer = 0 To 4
					Dim act As INDArray = ds.Features.get(interval(i, i, True), all(), all(), all(), all())
					Dim exp As INDArray = Nd4j.valueArrayOf(New Integer() { 1, 1, nZ, nX, nY }, i + offset)
					assertEquals(exp, act)
				Next i
				count += 1
			Loop
			assertEquals(2, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Custom Record Reader") static class CustomRecordReader extends org.datavec.api.records.reader.BaseRecordReader
		<Serializable>
		Friend Class CustomRecordReader
			Inherits BaseRecordReader

			Friend n As Integer = 0

			Friend Sub New()
			End Sub

			Public Overrides Function batchesSupported() As Boolean
				Return False
			End Function

			Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
				Throw New Exception("Not implemented")
			End Function

			Public Overrides Function [next]() As IList(Of Writable)
				Dim nd As INDArray = Nd4j.create(New Single((nZ * nY * nX) - 1){}, New Integer() { 1, 1, nZ, nY, nX }, "c"c).assign(n)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<org.datavec.api.writable.Writable> res = org.datavec.api.util.ndarray.RecordConverter.toRecord(nd);
				Dim res As IList(Of Writable) = RecordConverter.toRecord(nd)
				res.Add(New IntWritable(0))
				n += 1
				Return res
			End Function

			Public Overrides Function hasNext() As Boolean
				Return n < 10
			End Function

			Friend Shared ReadOnly labels As New List(Of String)(2)

			Shared Sub New()
				labels.Add("lbl0")
				labels.Add("lbl1")
			End Sub

			Public Overrides ReadOnly Property Labels As IList(Of String)
				Get
					Return labels
				End Get
			End Property

			Public Overrides Sub reset()
				n = 0
			End Sub

			Public Overrides Function resetSupported() As Boolean
				Return True
			End Function

			Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
				Return [next]()
			End Function

			Public Overrides Function nextRecord() As Record
				Dim r As IList(Of Writable) = [next]()
				Return New org.datavec.api.records.impl.Record(r, Nothing)
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws IOException
			Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
				Throw New Exception("Not implemented")
			End Function

			Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
				Throw New Exception("Not implemented")
			End Function

			Public Overridable Sub Dispose()
			End Sub

			Public Overrides Property Conf As Configuration
				Set(ByVal conf As Configuration)
				End Set
				Get
					Return Nothing
				End Get
			End Property


			Public Overrides Sub initialize(ByVal split As InputSplit)
				n = 0
			End Sub

			Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
				n = 0
			End Sub
		End Class
	End Class

End Namespace