Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Platform = com.sun.jna.Platform
Imports val = lombok.val
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports Text = org.apache.hadoop.io.Text
Imports SequenceFileOutputFormat = org.apache.hadoop.mapreduce.lib.output.SequenceFileOutputFormat
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports SequenceRecordReaderFunction = org.datavec.spark.functions.SequenceRecordReaderFunction
Imports org.datavec.spark.functions.pairdata
Imports StringToWritablesFunction = org.datavec.spark.transform.misc.StringToWritablesFunction
Imports DataVecSparkUtil = org.datavec.spark.util.DataVecSparkUtil
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Tuple2 = scala.Tuple2
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

Namespace org.deeplearning4j.spark.datavec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestDataVecDataSetFunctions extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestDataVecDataSetFunctions
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataVecDataSetFunction(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataVecDataSetFunction(ByVal testDir As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim sc As JavaSparkContext = Context

			Dim f As File = testDir.toFile()
			Dim cpr As New ClassPathResource("dl4j-spark/imagetest/")
			cpr.copyDirectory(f)

			'Test Spark record reader functionality vs. local
			Dim labelsList As IList(Of String) = New List(Of String) From {"0", "1"} 'Need this for Spark: can't infer without init call

			Dim path As String = f.getPath() & "/*"

			Dim origData As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(path)
			assertEquals(4, origData.count()) '4 images

			Dim rr As New ImageRecordReader(28, 28, 1, New ParentPathLabelGenerator())
			rr.Labels = labelsList
			Dim rrf As New org.datavec.spark.functions.RecordReaderFunction(rr)
			Dim rdd As JavaRDD(Of IList(Of Writable)) = origData.map(rrf)
			Dim data As JavaRDD(Of DataSet) = rdd.map(New DataVecDataSetFunction(1, 2, False))
			Dim collected As IList(Of DataSet) = data.collect()

			'Load normally (i.e., not via Spark), and check that we get the same results (order not withstanding)
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(f, New String() {"bmp"}, True)
			Dim irr As New ImageRecordReader(28, 28, 1, New ParentPathLabelGenerator())
			irr.initialize([is])

			Dim iter As New RecordReaderDataSetIterator(irr, 1, 1, 2)
			Dim listLocal As IList(Of DataSet) = New List(Of DataSet)(4)
			Do While iter.MoveNext()
				listLocal.Add(iter.Current)
			Loop


			'Compare:
			assertEquals(4, collected.Count)
			assertEquals(4, listLocal.Count)

			'Check that results are the same (order not withstanding)
			Dim found(3) As Boolean
			For i As Integer = 0 To 3
				Dim foundIndex As Integer = -1
				Dim ds As DataSet = collected(i)
				For j As Integer = 0 To 3
					If ds.Equals(listLocal(j)) Then
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataVecDataSetFunctionMultiLabelRegression() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataVecDataSetFunctionMultiLabelRegression()
			Dim sc As JavaSparkContext = Context

			Dim stringData As IList(Of String) = New List(Of String)()
			Dim n As Integer = 6
			For i As Integer = 0 To 9
				Dim sb As New StringBuilder()
				Dim first As Boolean = True
				For j As Integer = 0 To n - 1
					If Not first Then
						sb.Append(",")
					End If
					sb.Append(10 * i + j)
					first = False
				Next j
				stringData.Add(sb.ToString())
			Next i

			Dim stringList As JavaRDD(Of String) = sc.parallelize(stringData)
			Dim writables As JavaRDD(Of IList(Of Writable)) = stringList.map(New StringToWritablesFunction(New CSVRecordReader()))
			Dim dataSets As JavaRDD(Of DataSet) = writables.map(New DataVecDataSetFunction(3, 5, -1, True, Nothing, Nothing))

			Dim ds As IList(Of DataSet) = dataSets.collect()
			assertEquals(10, ds.Count)

			Dim seen(9) As Boolean
			For Each d As DataSet In ds
				Dim f As INDArray = d.Features
				Dim l As INDArray = d.Labels
				assertEquals(3, f.length())
				assertEquals(3, l.length())

				Dim exampleIdx As Integer = (CInt(Math.Truncate(f.getDouble(0)))) \ 10
				seen(exampleIdx) = True

				For j As Integer = 0 To 2
					assertEquals(10 * exampleIdx + j, CInt(Math.Truncate(f.getDouble(j))))
					assertEquals(10 * exampleIdx + j + 3, CInt(Math.Truncate(l.getDouble(j))))
				Next j
			Next d

			Dim seenCount As Integer = 0
			For Each b As Boolean In seen
				If b Then
					seenCount += 1
				End If
			Next b
			assertEquals(10, seenCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDataVecSequenceDataSetFunction(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataVecSequenceDataSetFunction(ByVal testDir As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim sc As JavaSparkContext = Context
			'Test Spark record reader functionality vs. local
			Dim dir As File = testDir.toFile()
			Dim cpr As New ClassPathResource("dl4j-spark/csvsequence/")
			cpr.copyDirectory(dir)

			Dim origData As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(dir.getAbsolutePath())
			assertEquals(3, origData.count()) '3 CSV sequences



			Dim seqRR As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim rrf As New SequenceRecordReaderFunction(seqRR)
			Dim rdd As JavaRDD(Of IList(Of IList(Of Writable))) = origData.map(rrf)
			Dim data As JavaRDD(Of DataSet) = rdd.map(New DataVecSequenceDataSetFunction(2, -1, True, Nothing, Nothing))
			Dim collected As IList(Of DataSet) = data.collect()

			'Load normally (i.e., not via Spark), and check that we get the same results (order not withstanding)
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(dir, New String() {"txt"}, True)
			Dim seqRR2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			seqRR2.initialize([is])

			Dim iter As New SequenceRecordReaderDataSetIterator(seqRR2, 1, -1, 2, True)
			Dim listLocal As IList(Of DataSet) = New List(Of DataSet)(3)
			Do While iter.MoveNext()
				listLocal.Add(iter.Current)
			Loop


			'Compare:
			assertEquals(3, collected.Count)
			assertEquals(3, listLocal.Count)

			'Check that results are the same (order not withstanding)
			Dim found(2) As Boolean
			For i As Integer = 0 To 2
				Dim foundIndex As Integer = -1
				Dim ds As DataSet = collected(i)
				For j As Integer = 0 To 2
					If ds.Equals(listLocal(j)) Then
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testDataVecSequencePairDataSetFunction(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataVecSequencePairDataSetFunction(ByVal testDir As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim sc As JavaSparkContext = Context

			Dim f As New File(testDir.toFile(),"f")
			Dim cpr As New ClassPathResource("dl4j-spark/csvsequence/")
			cpr.copyDirectory(f)
			Dim path As String = f.getAbsolutePath() & "/*"

			Dim pathConverter As PathToKeyConverter = New PathToKeyConverterFilename()
			Dim toWrite As JavaPairRDD(Of Text, BytesPairWritable) = DataVecSparkUtil.combineFilesForSequenceFile(sc, path, path, pathConverter)

			Dim p As Path = (New File(testDir.toFile(),"dl4j_testSeqPairFn")).toPath()
			p.toFile().deleteOnExit()
			Dim outPath As String = p.ToString() & "/out"
			Call (New File(outPath)).deleteOnExit()
			toWrite.saveAsNewAPIHadoopFile(outPath, GetType(Text), GetType(BytesPairWritable), GetType(SequenceFileOutputFormat))

			'Load from sequence file:
			Dim fromSeq As JavaPairRDD(Of Text, BytesPairWritable) = sc.sequenceFile(outPath, GetType(Text), GetType(BytesPairWritable))

			Dim srr1 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim srr2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim psrbf As New PairSequenceRecordReaderBytesFunction(srr1, srr2)
			Dim writables As JavaRDD(Of Tuple2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))) = fromSeq.map(psrbf)

			'Map to DataSet:
			Dim pairFn As New DataVecSequencePairDataSetFunction()
			Dim data As JavaRDD(Of DataSet) = writables.map(pairFn)
			Dim sparkData As IList(Of DataSet) = data.collect()


			'Now: do the same thing locally (SequenceRecordReaderDataSetIterator) and compare
			Dim featuresPath As String = FilenameUtils.concat(f.getAbsolutePath(), "csvsequence_%d.txt")

			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))

			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, -1, True)

			Dim localData As IList(Of DataSet) = New List(Of DataSet)(3)
			Do While iter.MoveNext()
				localData.Add(iter.Current)
			Loop

			assertEquals(3, sparkData.Count)
			assertEquals(3, localData.Count)

			For i As Integer = 0 To 2
				'Check shapes etc. data sets order may differ for spark vs. local
				Dim dsSpark As DataSet = sparkData(i)
				Dim dsLocal As DataSet = localData(i)

				assertNull(dsSpark.FeaturesMaskArray)
				assertNull(dsSpark.LabelsMaskArray)

				Dim fSpark As INDArray = dsSpark.Features
				Dim fLocal As INDArray = dsLocal.Features
				Dim lSpark As INDArray = dsSpark.Labels
				Dim lLocal As INDArray = dsLocal.Labels

				Dim s As val = New Long() {1, 3, 4} '1 example, 3 values, 3 time steps
				assertArrayEquals(s, fSpark.shape())
				assertArrayEquals(s, fLocal.shape())
				assertArrayEquals(s, lSpark.shape())
				assertArrayEquals(s, lLocal.shape())
			Next i


			'Check that results are the same (order not withstanding)
			Dim found(2) As Boolean
			For i As Integer = 0 To 2
				Dim foundIndex As Integer = -1
				Dim ds As DataSet = sparkData(i)
				For j As Integer = 0 To 2
					If ds.Equals(localData(j)) Then
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Permissions issues") public void testDataVecSequencePairDataSetFunctionVariableLength(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataVecSequencePairDataSetFunctionVariableLength(ByVal testDir As Path)
			'Same sort of test as testDataVecSequencePairDataSetFunction() but with variable length time series (labels shorter, align end)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim dirFeatures As New File(testDir.toFile(),"dirFeatures")
			Dim cpr As New ClassPathResource("dl4j-spark/csvsequence/")
			cpr.copyDirectory(dirFeatures)

			Dim dirLabels As New File(testDir.toFile(),"dirLables")
			Dim cpr2 As New ClassPathResource("dl4j-spark/csvsequencelabels/")
			cpr2.copyDirectory(dirLabels)


			Dim pathConverter As PathToKeyConverter = New PathToKeyConverterNumber() 'Extract a number from the file name
			Dim toWrite As JavaPairRDD(Of Text, BytesPairWritable) = DataVecSparkUtil.combineFilesForSequenceFile(sc, dirFeatures.getAbsolutePath(), dirLabels.getAbsolutePath(), pathConverter)

			Dim p As Path = (New File(testDir.toFile(),"dl4j_testSeqPairFnVarLength")).toPath()
			p.toFile().deleteOnExit()
			Dim outPath As String = p.toFile().getAbsolutePath() & "/out"
			Call (New File(outPath)).deleteOnExit()
			toWrite.saveAsNewAPIHadoopFile(outPath, GetType(Text), GetType(BytesPairWritable), GetType(SequenceFileOutputFormat))

			'Load from sequence file:
			Dim fromSeq As JavaPairRDD(Of Text, BytesPairWritable) = sc.sequenceFile(outPath, GetType(Text), GetType(BytesPairWritable))

			Dim srr1 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim srr2 As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim psrbf As New PairSequenceRecordReaderBytesFunction(srr1, srr2)
			Dim writables As JavaRDD(Of Tuple2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))) = fromSeq.map(psrbf)

			'Map to DataSet:
			Dim pairFn As New DataVecSequencePairDataSetFunction(4, False, DataVecSequencePairDataSetFunction.AlignmentMode.ALIGN_END)
			Dim data As JavaRDD(Of DataSet) = writables.map(pairFn)
			Dim sparkData As IList(Of DataSet) = data.collect()


			'Now: do the same thing locally (SequenceRecordReaderDataSetIterator) and compare
			Dim featuresPath As String = FilenameUtils.concat(dirFeatures.getAbsolutePath(), "csvsequence_%d.txt")
			Dim labelsPath As String = FilenameUtils.concat(dirLabels.getAbsolutePath(), "csvsequencelabelsShort_%d.txt")

			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			Dim labelReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))

			Dim iter As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END)

			Dim localData As IList(Of DataSet) = New List(Of DataSet)(3)
			Do While iter.MoveNext()
				localData.Add(iter.Current)
			Loop

			assertEquals(3, sparkData.Count)
			assertEquals(3, localData.Count)

			Dim fShapeExp As val = New Long() {1, 3, 4} '1 example, 3 values, 4 time steps
			Dim lShapeExp As val = New Long() {1, 4, 4} '1 example, 4 values/classes, 4 time steps (after padding)
			For i As Integer = 0 To 2
				'Check shapes etc. data sets order may differ for spark vs. local
				Dim dsSpark As DataSet = sparkData(i)
				Dim dsLocal As DataSet = localData(i)

				assertNotNull(dsSpark.LabelsMaskArray) 'Expect mask array for labels

				Dim fSpark As INDArray = dsSpark.Features
				Dim fLocal As INDArray = dsLocal.Features
				Dim lSpark As INDArray = dsSpark.Labels
				Dim lLocal As INDArray = dsLocal.Labels


				assertArrayEquals(fShapeExp, fSpark.shape())
				assertArrayEquals(fShapeExp, fLocal.shape())
				assertArrayEquals(lShapeExp, lSpark.shape())
				assertArrayEquals(lShapeExp, lLocal.shape())
			Next i


			'Check that results are the same (order not withstanding)
			Dim found(2) As Boolean
			For i As Integer = 0 To 2
				Dim foundIndex As Integer = -1
				Dim ds As DataSet = sparkData(i)
				For j As Integer = 0 To 2
					If dataSetsEqual(ds, localData(j)) Then
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


			'-------------------------------------------------
			'NOW: test same thing, but for align start...
			Dim pairFnAlignStart As New DataVecSequencePairDataSetFunction(4, False, DataVecSequencePairDataSetFunction.AlignmentMode.ALIGN_START)
			Dim rddDataAlignStart As JavaRDD(Of DataSet) = writables.map(pairFnAlignStart)
			Dim sparkDataAlignStart As IList(Of DataSet) = rddDataAlignStart.collect()

			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2)) 're-initialize to reset
			labelReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsPath, 0, 2))
			Dim iterAlignStart As New SequenceRecordReaderDataSetIterator(featureReader, labelReader, 1, 4, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_START)

			Dim localDataAlignStart As IList(Of DataSet) = New List(Of DataSet)(3)
			Do While iterAlignStart.MoveNext()
				localDataAlignStart.Add(iterAlignStart.Current)
			Loop

			assertEquals(3, sparkDataAlignStart.Count)
			assertEquals(3, localDataAlignStart.Count)

			For i As Integer = 0 To 2
				'Check shapes etc. data sets order may differ for spark vs. local
				Dim dsSpark As DataSet = sparkDataAlignStart(i)
				Dim dsLocal As DataSet = localDataAlignStart(i)

				assertNotNull(dsSpark.LabelsMaskArray) 'Expect mask array for labels

				Dim fSpark As INDArray = dsSpark.Features
				Dim fLocal As INDArray = dsLocal.Features
				Dim lSpark As INDArray = dsSpark.Labels
				Dim lLocal As INDArray = dsLocal.Labels


				assertArrayEquals(fShapeExp, fSpark.shape())
				assertArrayEquals(fShapeExp, fLocal.shape())
				assertArrayEquals(lShapeExp, lSpark.shape())
				assertArrayEquals(lShapeExp, lLocal.shape())
			Next i


			'Check that results are the same (order not withstanding)
			found = New Boolean(2){}
			For i As Integer = 0 To 2
				Dim foundIndex As Integer = -1
				Dim ds As DataSet = sparkData(i)
				For j As Integer = 0 To 2
					If dataSetsEqual(ds, localData(j)) Then
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
			count = 0
			For Each b As Boolean In found
				If b Then
					count += 1
				End If
			Next b
			assertEquals(3, count) 'Expect all 3 and exactly 3 pairwise matches between spark and local versions
		End Sub


		Private Shared Function dataSetsEqual(ByVal d1 As DataSet, ByVal d2 As DataSet) As Boolean

			If Not d1.Features.Equals(d2.Features) Then
				Return False
			End If
			If d1.Labels Is Nothing AndAlso d2.Labels IsNot Nothing OrElse d1.Labels IsNot Nothing AndAlso d2.Labels Is Nothing Then
				Return False
			End If
			If d1.Labels IsNot Nothing AndAlso Not d1.Labels.Equals(d2.Labels) Then
				Return False
			End If

			Return masksEqual(d1.Features, d2.Features) AndAlso masksEqual(d1.LabelsMaskArray, d2.LabelsMaskArray)
		End Function

		Private Shared Function masksEqual(ByVal m1 As INDArray, ByVal m2 As INDArray) As Boolean
			If m1 Is Nothing AndAlso m2 Is Nothing Then
				Return True
			End If
			If m1 IsNot Nothing AndAlso m2 IsNot Nothing Then
				Return m1.Equals(m2)
			End If
			'One is null, other is not. Null and ones mask arrays are equal though
			If m1 IsNot Nothing AndAlso Not m1.Equals(Nd4j.ones(m1.shape())) Then
				Return False
			End If
			If m2 IsNot Nothing AndAlso Not m2.Equals(Nd4j.ones(m2.shape())) Then
				Return False
			End If

			Return True
		End Function

	End Class

End Namespace