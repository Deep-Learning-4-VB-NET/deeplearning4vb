Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports BatchAndExportDataSetsFunction = org.deeplearning4j.spark.data.BatchAndExportDataSetsFunction
Imports BatchAndExportMultiDataSetsFunction = org.deeplearning4j.spark.data.BatchAndExportMultiDataSetsFunction
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull

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

Namespace org.deeplearning4j.spark.datavec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestExport extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestExport
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBatchAndExportDataSetsFunction() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBatchAndExportDataSetsFunction()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim baseDir As String = System.getProperty("java.io.tmpdir")
			baseDir = FilenameUtils.concat(baseDir, "dl4j_spark_testBatchAndExport/")
			baseDir = baseDir.replaceAll("\\", "/")
			Dim f As New File(baseDir)
			If f.exists() Then
				FileUtils.deleteDirectory(f)
			End If
			f.mkdir()
			f.deleteOnExit()
			Dim minibatchSize As Integer = 5
			Dim nIn As Integer = 4
			Dim nOut As Integer = 3

			Dim dataSets As IList(Of DataSet) = New List(Of DataSet)()
			dataSets.Add(New DataSet(Nd4j.create(10, nIn), Nd4j.create(10, nOut))) 'Larger than minibatch size -> tests splitting
			For i As Integer = 0 To 97
				If i Mod 2 = 0 Then
					dataSets.Add(New DataSet(Nd4j.create(5, nIn), Nd4j.create(5, nOut)))
				Else
					dataSets.Add(New DataSet(Nd4j.create(1, nIn), Nd4j.create(1, nOut)))
					dataSets.Add(New DataSet(Nd4j.create(1, nIn), Nd4j.create(1, nOut)))
					dataSets.Add(New DataSet(Nd4j.create(3, nIn), Nd4j.create(3, nOut)))
				End If
			Next i

			Collections.shuffle(dataSets, New Random(12345))

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(dataSets)
			rdd = rdd.repartition(1) 'For testing purposes (should get exactly 100 out, but maybe more with more partitions)


			Dim pathsRdd As JavaRDD(Of String) = rdd.mapPartitionsWithIndex(New BatchAndExportDataSetsFunction(minibatchSize, "file:///" & baseDir), True)

			Dim paths As IList(Of String) = pathsRdd.collect()
			assertEquals(100, paths.Count)

			Dim files() As File = f.listFiles()
			assertNotNull(files)

			Dim count As Integer = 0
			For Each file As File In files
				If Not file.getPath().EndsWith(".bin") Then
					Continue For
				End If
	'            System.out.println(file);
				Dim ds As New DataSet()
				ds.load(file)
				assertEquals(minibatchSize, ds.numExamples())

				count += 1
			Next file

			assertEquals(100, count)

			FileUtils.deleteDirectory(f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBatchAndExportMultiDataSetsFunction() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBatchAndExportMultiDataSetsFunction()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim baseDir As String = System.getProperty("java.io.tmpdir")
			baseDir = FilenameUtils.concat(baseDir, "dl4j_spark_testBatchAndExportMDS/")
			baseDir = baseDir.replaceAll("\\", "/")
			Dim f As New File(baseDir)
			If f.exists() Then
				FileUtils.deleteDirectory(f)
			End If
			f.mkdir()
			f.deleteOnExit()
			Dim minibatchSize As Integer = 5
			Dim nIn As Integer = 4
			Dim nOut As Integer = 3

			Dim dataSets As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
			dataSets.Add(New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.create(10, nIn), Nd4j.create(10, nOut))) 'Larger than minibatch size -> tests splitting
			For i As Integer = 0 To 97
				If i Mod 2 = 0 Then
					dataSets.Add(New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.create(5, nIn), Nd4j.create(5, nOut)))
				Else
					dataSets.Add(New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.create(1, nIn), Nd4j.create(1, nOut)))
					dataSets.Add(New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.create(1, nIn), Nd4j.create(1, nOut)))
					dataSets.Add(New org.nd4j.linalg.dataset.MultiDataSet(Nd4j.create(3, nIn), Nd4j.create(3, nOut)))
				End If
			Next i

			Collections.shuffle(dataSets, New Random(12345))

			Dim rdd As JavaRDD(Of MultiDataSet) = sc.parallelize(dataSets)
			rdd = rdd.repartition(1) 'For testing purposes (should get exactly 100 out, but maybe more with more partitions)


			Dim pathsRdd As JavaRDD(Of String) = rdd.mapPartitionsWithIndex(New BatchAndExportMultiDataSetsFunction(minibatchSize, "file:///" & baseDir), True)

			Dim paths As IList(Of String) = pathsRdd.collect()
			assertEquals(100, paths.Count)

			Dim files() As File = f.listFiles()
			assertNotNull(files)

			Dim count As Integer = 0
			For Each file As File In files
				If Not file.getPath().EndsWith(".bin") Then
					Continue For
				End If
	'            System.out.println(file);
				Dim ds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet()
				ds.load(file)
				assertEquals(minibatchSize, ds.getFeatures(0).size(0))
				assertEquals(minibatchSize, ds.getLabels(0).size(0))

				count += 1
			Next file

			assertEquals(100, count)

			FileUtils.deleteDirectory(f)
		End Sub
	End Class

End Namespace