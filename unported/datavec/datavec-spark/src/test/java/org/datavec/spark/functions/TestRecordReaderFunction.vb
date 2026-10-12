Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Platform = com.sun.jna.Platform
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports ArrayWritable = org.datavec.api.writable.ArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestRecordReaderFunction extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class TestRecordReaderFunction
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRecordReaderFunction(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRecordReaderFunction(ByVal testDir As Path)
			If Platform.isWindows() Then
				Return
			End If
			Dim f As File = testDir.toFile()
			Call (New ClassPathResource("datavec-spark/imagetest/")).copyDirectory(f)
			Dim labelsList As IList(Of String) = New List(Of String) From {"0", "1"} 'Need this for Spark: can't infer without init call

			Dim path As String = f.getAbsolutePath() & "/*"

			Dim origData As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(path)
			assertEquals(4, origData.count()) '4 images

			Dim irr As New ImageRecordReader(28, 28, 1, New ParentPathLabelGenerator())
			irr.Labels = labelsList
			Dim rrf As New RecordReaderFunction(irr)
			Dim rdd As JavaRDD(Of IList(Of Writable)) = origData.map(rrf)
			Dim listSpark As IList(Of IList(Of Writable)) = rdd.collect()

			assertEquals(4, listSpark.Count)
			For i As Integer = 0 To 3
				assertEquals(1 + 1, listSpark(i).Count)
				assertEquals(28 * 28, CType(listSpark(i).GetEnumerator().next(), ArrayWritable).length())
			Next i

			'Load normally (i.e., not via Spark), and check that we get the same results (order not withstanding)
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(f, New String() {"bmp"}, True)
			'        System.out.println("Locations: " + Arrays.toString(is.locations()));
			irr = New ImageRecordReader(28, 28, 1, New ParentPathLabelGenerator())
			irr.initialize([is])

			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(4)
			Do While irr.hasNext()
				list.Add(irr.next())
			Loop
			assertEquals(4, list.Count)

			'        System.out.println("Spark list:");
			'        for(List<Writable> c : listSpark ) System.out.println(c);
			'        System.out.println("Local list:");
			'        for(List<Writable> c : list ) System.out.println(c);

			'Check that each of the values from Spark equals exactly one of the values doing it locally
			Dim found(3) As Boolean
			For i As Integer = 0 To 3
				Dim foundIndex As Integer = -1
				Dim collection As IList(Of Writable) = listSpark(i)
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
		End Sub

	End Class

End Namespace