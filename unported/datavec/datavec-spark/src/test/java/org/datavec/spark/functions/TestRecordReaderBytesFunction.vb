Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Platform = com.sun.jna.Platform
Imports BytesWritable = org.apache.hadoop.io.BytesWritable
Imports Text = org.apache.hadoop.io.Text
Imports SequenceFileOutputFormat = org.apache.hadoop.mapreduce.lib.output.SequenceFileOutputFormat
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports FilesAsBytesFunction = org.datavec.spark.functions.data.FilesAsBytesFunction
Imports RecordReaderBytesFunction = org.datavec.spark.functions.data.RecordReaderBytesFunction
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.fail

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

Namespace org.datavec.spark.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestRecordReaderBytesFunction extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class TestRecordReaderBytesFunction
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRecordReaderBytesFunction(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRecordReaderBytesFunction(ByVal testDir As Path)
			If Platform.isWindows() Then
				Return
			End If
			Dim sc As JavaSparkContext = Context

			'Local file path
			Dim f As File = testDir.toFile()
			Call (New ClassPathResource("datavec-spark/imagetest/")).copyDirectory(f)
			Dim labelsList As IList(Of String) = New List(Of String) From {"0", "1"} 'Need this for Spark: can't infer without init call
			Dim path As String = f.getAbsolutePath() & "/*"

			'Load binary data from local file system, convert to a sequence file:
			'Load and convert
			Dim origData As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(path)
			Dim filesAsBytes As JavaPairRDD(Of Text, BytesWritable) = origData.mapToPair(New FilesAsBytesFunction())
			'Write the sequence file:
			Dim p As Path = Files.createTempDirectory("dl4j_rrbytesTest")
			p.toFile().deleteOnExit()
			Dim outPath As String = p.ToString() & "/out"
			filesAsBytes.saveAsNewAPIHadoopFile(outPath, GetType(Text), GetType(BytesWritable), GetType(SequenceFileOutputFormat))

			'Load data from sequence file, parse via RecordReader:
			Dim fromSeqFile As JavaPairRDD(Of Text, BytesWritable) = sc.sequenceFile(outPath, GetType(Text), GetType(BytesWritable))
			Dim irr As New ImageRecordReader(28, 28, 1, New ParentPathLabelGenerator())
			irr.Labels = labelsList
			Dim dataVecData As JavaRDD(Of IList(Of Writable)) = fromSeqFile.map(New RecordReaderBytesFunction(irr))


			'Next: do the same thing locally, and compare the results
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(f, New String() {"bmp"}, True)
			irr = New ImageRecordReader(28, 28, 1, New ParentPathLabelGenerator())
			irr.initialize([is])

			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(4)
			Do While irr.hasNext()
				list.Add(irr.next())
			Loop

			Dim fromSequenceFile As IList(Of IList(Of Writable)) = dataVecData.collect()

			assertEquals(4, list.Count)
			assertEquals(4, fromSequenceFile.Count)

			'Check that each of the values from Spark equals exactly one of the values doing it locally
			Dim found(3) As Boolean
			For i As Integer = 0 To 3
				Dim foundIndex As Integer = -1
				Dim collection As IList(Of Writable) = fromSequenceFile(i)
				For j As Integer = 0 To 3
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (collection.equals(list.get(j)))
					If collection.SequenceEqual(list(j)) Then
						If foundIndex <> -1 Then
							fail() 'Already found this value -> suggests this spark value equals two or more of local version? (Shouldn't happen)
						End If
						foundIndex = j
						If found(foundIndex) Then
							fail() 'One of the other spark values was equal to this one -> suggests duplicates in Spark list
						End If
						found(foundIndex) = True 'mark this one as seen before
					End If
				Next j
			Next i



			Dim count As Integer = 0
			For Each b As Boolean In found
				If b Then
					count += 1
				End If
			Next b
			assertEquals(4, count) 'Expect all 4 and exactly 4 pairwise matches between spark and local versions
			'        System.out.println("COUNT: " + count);
		End Sub

	End Class

End Namespace