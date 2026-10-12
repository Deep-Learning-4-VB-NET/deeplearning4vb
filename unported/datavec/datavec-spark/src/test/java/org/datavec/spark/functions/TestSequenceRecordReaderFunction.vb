Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports Configuration = org.datavec.api.conf.Configuration
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports ArrayWritable = org.datavec.api.writable.ArrayWritable
Imports Writable = org.datavec.api.writable.Writable
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestSequenceRecordReaderFunction extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class TestSequenceRecordReaderFunction
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceRecordReaderFunctionCSV(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSequenceRecordReaderFunctionCSV(ByVal testDir As Path)
			Dim sc As JavaSparkContext = Context

			Dim f As File = testDir.toFile()
			Call (New ClassPathResource("datavec-spark/csvsequence/")).copyDirectory(f)

			Dim path As String = f.getAbsolutePath() & "/*"

			Dim origData As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(path)
			assertEquals(3, origData.count()) '3 CSV files

			Dim srrf As New SequenceRecordReaderFunction(New CSVSequenceRecordReader(1, ",")) 'CSV, skip 1 line
			Dim rdd As JavaRDD(Of IList(Of IList(Of Writable))) = origData.map(srrf)
			Dim listSpark As IList(Of IList(Of IList(Of Writable))) = rdd.collect()

			assertEquals(3, listSpark.Count)
			For i As Integer = 0 To 2
				Dim thisSequence As IList(Of IList(Of Writable)) = listSpark(i)
				assertEquals(4, thisSequence.Count) 'Expect exactly 4 time steps in sequence
				For Each c As IList(Of Writable) In thisSequence
					assertEquals(3, c.Count) '3 values per time step
				Next c
			Next i

			'Load normally, and check that we get the same results (order not withstanding)
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(f, New String() {"txt"}, True)
			'        System.out.println("Locations:");
			'        System.out.println(Arrays.toString(is.locations()));

			Dim srr As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			srr.initialize([is])

			Dim list As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(3)
			Do While srr.hasNext()
				list.Add(srr.sequenceRecord())
			Loop
			assertEquals(3, list.Count)

			'        System.out.println("Spark list:");
			'        for(List<List<Writable>> c : listSpark ) System.out.println(c);
			'        System.out.println("Local list:");
			'        for(List<List<Writable>> c : list ) System.out.println(c);

			'Check that each of the values from Spark equals exactly one of the values doing it normally
			Dim found(2) As Boolean
			For i As Integer = 0 To 2
				Dim foundIndex As Integer = -1
				Dim collection As IList(Of IList(Of Writable)) = listSpark(i)
				For j As Integer = 0 To 2
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
			assertEquals(3, count) 'Expect all 3 and exactly 3 pairwise matches between spark and local versions
		End Sub




	End Class

End Namespace