Imports System
Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Configuration = org.datavec.api.conf.Configuration
Imports LibSvmRecordReader = org.datavec.api.records.reader.impl.misc.LibSvmRecordReader
Imports LibSvmRecordWriter = org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter
Imports FileSplit = org.datavec.api.split.FileSplit
Imports NumberOfRecordsPartitioner = org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
import static org.junit.jupiter.api.Assertions.assertEquals
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
Namespace org.datavec.api.records.writer.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Lib Svm Record Writer Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class LibSvmRecordWriterTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class LibSvmRecordWriterTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Basic") void testBasic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBasic()
			Dim configWriter As New Configuration()
			Dim configReader As New Configuration()
			configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
			configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)
			Dim inputFile As File = (New ClassPathResource("datavec-api/svmlight/basic.txt")).File
			executeTest(configWriter, configReader, inputFile)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test No Label") void testNoLabel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNoLabel()
			Dim configWriter As New Configuration()
			configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
			configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 9)
			Dim configReader As New Configuration()
			configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
			configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)
			Dim inputFile As File = (New ClassPathResource("datavec-api/svmlight/basic.txt")).File
			executeTest(configWriter, configReader, inputFile)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multioutput Record") void testMultioutputRecord() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultioutputRecord()
			Dim configWriter As New Configuration()
			configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
			configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 9)
			Dim configReader As New Configuration()
			configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
			configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)
			Dim inputFile As File = (New ClassPathResource("datavec-api/svmlight/multioutput.txt")).File
			executeTest(configWriter, configReader, inputFile)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multilabel Record") void testMultilabelRecord() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultilabelRecord()
			Dim configWriter As New Configuration()
			configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
			configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 9)
			configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
			Dim configReader As New Configuration()
			configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
			configReader.setBoolean(LibSvmRecordReader.MULTILABEL, True)
			configReader.setInt(LibSvmRecordReader.NUM_LABELS, 4)
			configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)
			Dim inputFile As File = (New ClassPathResource("datavec-api/svmlight/multilabel.txt")).File
			executeTest(configWriter, configReader, inputFile)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Zero Based Indexing") void testZeroBasedIndexing() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testZeroBasedIndexing()
			Dim configWriter As New Configuration()
			configWriter.setBoolean(LibSvmRecordWriter.ZERO_BASED_INDEXING, True)
			configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
			configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 10)
			configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
			Dim configReader As New Configuration()
			configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 11)
			configReader.setBoolean(LibSvmRecordReader.MULTILABEL, True)
			configReader.setInt(LibSvmRecordReader.NUM_LABELS, 5)
			Dim inputFile As File = (New ClassPathResource("datavec-api/svmlight/multilabel.txt")).File
			executeTest(configWriter, configReader, inputFile)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void executeTest(org.datavec.api.conf.Configuration configWriter, org.datavec.api.conf.Configuration configReader, java.io.File inputFile) throws Exception
		Public Shared Sub executeTest(ByVal configWriter As Configuration, ByVal configReader As Configuration, ByVal inputFile As File)
			Dim tempFile As File = File.createTempFile("LibSvmRecordWriter", ".txt")
			tempFile.setWritable(True)
			tempFile.deleteOnExit()
			If tempFile.exists() Then
				tempFile.delete()
			End If
			Using writer As New org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter()
				Dim outputSplit As New org.datavec.api.Split.FileSplit(tempFile)
				writer.initialize(configWriter, outputSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
				Dim rr As New LibSvmRecordReader()
				rr.initialize(configReader, New org.datavec.api.Split.FileSplit(inputFile))
				Do While rr.hasNext()
					Dim record As IList(Of Writable) = rr.next()
					writer.write(record)
				Loop
			End Using
			Dim p As Pattern = Pattern.compile(String.Format("{0}:\d+ ", LibSvmRecordReader.QID_PREFIX))
			Dim linesOriginal As IList(Of String) = New List(Of String)()
			For Each line As String In FileUtils.readLines(inputFile)
				If Not line.StartsWith(LibSvmRecordReader.COMMENT_CHAR, StringComparison.Ordinal) Then
					Dim lineClean As String = line.Split(LibSvmRecordReader.COMMENT_CHAR, 2)(0)
					If lineClean.StartsWith(" ", StringComparison.Ordinal) Then
						lineClean = " " & lineClean.Trim()
					Else
						lineClean = lineClean.Trim()
					End If
					Dim m As Matcher = p.matcher(lineClean)
					lineClean = m.replaceAll("")
					linesOriginal.Add(lineClean)
				End If
			Next line
			Dim linesNew As IList(Of String) = FileUtils.readLines(tempFile)
			assertEquals(linesOriginal, linesNew)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test ND Array Writables") void testNDArrayWritables() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNDArrayWritables()
			Dim arr2 As INDArray = Nd4j.zeros(2)
			arr2.putScalar(0, 11)
			arr2.putScalar(1, 12)
			Dim arr3 As INDArray = Nd4j.zeros(3)
			arr3.putScalar(0, 13)
			arr3.putScalar(1, 14)
			arr3.putScalar(2, 15)
			Dim record As IList(Of Writable) = New List(Of Writable) From {DirectCast(New DoubleWritable(1), Writable), New NDArrayWritable(arr2), New IntWritable(2), New DoubleWritable(3), New NDArrayWritable(arr3), New IntWritable(4)}
			Dim tempFile As File = File.createTempFile("LibSvmRecordWriter", ".txt")
			tempFile.setWritable(True)
			tempFile.deleteOnExit()
			If tempFile.exists() Then
				tempFile.delete()
			End If
			Dim lineOriginal As String = "13.0,14.0,15.0,4 1:1.0 2:11.0 3:12.0 4:2.0 5:3.0"
			Using writer As New org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter()
				Dim configWriter As New Configuration()
				configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 3)
				Dim outputSplit As New org.datavec.api.Split.FileSplit(tempFile)
				writer.initialize(configWriter, outputSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
				writer.write(record)
			End Using
			Dim lineNew As String = FileUtils.readFileToString(tempFile).Trim()
			assertEquals(lineOriginal, lineNew)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test ND Array Writables Multilabel") void testNDArrayWritablesMultilabel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNDArrayWritablesMultilabel()
			Dim arr2 As INDArray = Nd4j.zeros(2)
			arr2.putScalar(0, 11)
			arr2.putScalar(1, 12)
			Dim arr3 As INDArray = Nd4j.zeros(3)
			arr3.putScalar(0, 0)
			arr3.putScalar(1, 1)
			arr3.putScalar(2, 0)
			Dim record As IList(Of Writable) = New List(Of Writable) From {DirectCast(New DoubleWritable(1), Writable), New NDArrayWritable(arr2), New IntWritable(2), New DoubleWritable(3), New NDArrayWritable(arr3), New DoubleWritable(1)}
			Dim tempFile As File = File.createTempFile("LibSvmRecordWriter", ".txt")
			tempFile.setWritable(True)
			tempFile.deleteOnExit()
			If tempFile.exists() Then
				tempFile.delete()
			End If
			Dim lineOriginal As String = "2,4 1:1.0 2:11.0 3:12.0 4:2.0 5:3.0"
			Using writer As New org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter()
				Dim configWriter As New Configuration()
				configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 3)
				Dim outputSplit As New org.datavec.api.Split.FileSplit(tempFile)
				writer.initialize(configWriter, outputSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
				writer.write(record)
			End Using
			Dim lineNew As String = FileUtils.readFileToString(tempFile).Trim()
			assertEquals(lineOriginal, lineNew)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test ND Array Writables Zero Index") void testNDArrayWritablesZeroIndex() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNDArrayWritablesZeroIndex()
			Dim arr2 As INDArray = Nd4j.zeros(2)
			arr2.putScalar(0, 11)
			arr2.putScalar(1, 12)
			Dim arr3 As INDArray = Nd4j.zeros(3)
			arr3.putScalar(0, 0)
			arr3.putScalar(1, 1)
			arr3.putScalar(2, 0)
			Dim record As IList(Of Writable) = New List(Of Writable) From {DirectCast(New DoubleWritable(1), Writable), New NDArrayWritable(arr2), New IntWritable(2), New DoubleWritable(3), New NDArrayWritable(arr3), New DoubleWritable(1)}
			Dim tempFile As File = File.createTempFile("LibSvmRecordWriter", ".txt")
			tempFile.setWritable(True)
			tempFile.deleteOnExit()
			If tempFile.exists() Then
				tempFile.delete()
			End If
			Dim lineOriginal As String = "1,3 0:1.0 1:11.0 2:12.0 3:2.0 4:3.0"
			Using writer As New org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter()
				Dim configWriter As New Configuration()
				' NOT STANDARD!
				configWriter.setBoolean(LibSvmRecordWriter.ZERO_BASED_INDEXING, True)
				' NOT STANDARD!
				configWriter.setBoolean(LibSvmRecordWriter.ZERO_BASED_LABEL_INDEXING, True)
				configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 3)
				Dim outputSplit As New org.datavec.api.Split.FileSplit(tempFile)
				writer.initialize(configWriter, outputSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
				writer.write(record)
			End Using
			Dim lineNew As String = FileUtils.readFileToString(tempFile).Trim()
			assertEquals(lineOriginal, lineNew)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Non Integer But Valid Multilabel") void testNonIntegerButValidMultilabel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNonIntegerButValidMultilabel()
			Dim record As IList(Of Writable) = New List(Of Writable) From {DirectCast(New IntWritable(3), Writable), New IntWritable(2), New DoubleWritable(1.0)}
			Dim tempFile As File = File.createTempFile("LibSvmRecordWriter", ".txt")
			tempFile.setWritable(True)
			tempFile.deleteOnExit()
			If tempFile.exists() Then
				tempFile.delete()
			End If
			Using writer As New org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter()
				Dim configWriter As New Configuration()
				configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 1)
				configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
				Dim outputSplit As New org.datavec.api.Split.FileSplit(tempFile)
				writer.initialize(configWriter, outputSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
				writer.write(record)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Non Integer Multilabel") void nonIntegerMultilabel()
		Friend Overridable Sub nonIntegerMultilabel()
			assertThrows(GetType(System.FormatException), Sub()
			Dim record As IList(Of Writable) = New List(Of Writable) From {DirectCast(New IntWritable(3), Writable), New IntWritable(2), New DoubleWritable(1.2)}
			Dim tempFile As File = File.createTempFile("LibSvmRecordWriter", ".txt")
			tempFile.setWritable(True)
			tempFile.deleteOnExit()
			If tempFile.exists() Then
				tempFile.delete()
			End If
			Using writer As New org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter()
				Dim configWriter As New Configuration()
				configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 1)
				configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
				Dim outputSplit As New org.datavec.api.Split.FileSplit(tempFile)
				writer.initialize(configWriter, outputSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
				writer.write(record)
			End Using
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Non Binary Multilabel") void nonBinaryMultilabel()
		Friend Overridable Sub nonBinaryMultilabel()
			assertThrows(GetType(System.FormatException), Sub()
			Dim record As IList(Of Writable) = New List(Of Writable) From {DirectCast(New IntWritable(0), Writable), New IntWritable(1), New IntWritable(2)}
			Dim tempFile As File = File.createTempFile("LibSvmRecordWriter", ".txt")
			tempFile.setWritable(True)
			tempFile.deleteOnExit()
			If tempFile.exists() Then
				tempFile.delete()
			End If
			Using writer As New org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter()
				Dim configWriter As New Configuration()
				configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
				configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 1)
				configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
				Dim outputSplit As New org.datavec.api.Split.FileSplit(tempFile)
				writer.initialize(configWriter, outputSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
				writer.write(record)
			End Using
			End Sub)
		End Sub
	End Class

End Namespace