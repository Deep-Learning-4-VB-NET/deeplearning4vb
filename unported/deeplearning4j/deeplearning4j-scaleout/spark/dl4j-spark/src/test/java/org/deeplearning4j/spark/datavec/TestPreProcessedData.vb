Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Platform = com.sun.jna.Platform
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Repartition = org.deeplearning4j.spark.api.Repartition
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports StringToDataSetExportFunction = org.deeplearning4j.spark.datavec.export.StringToDataSetExportFunction
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports PortableDataStreamDataSetIterator = org.deeplearning4j.spark.iterator.PortableDataStreamDataSetIterator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestPreProcessedData extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestPreProcessedData
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreprocessedData()
		Public Overridable Sub testPreprocessedData()
			'Test _loading_ of preprocessed data
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim dataSetObjSize As Integer = 5
			Dim batchSizePerExecutor As Integer = 10

			Dim path As String = FilenameUtils.concat(System.getProperty("java.io.tmpdir"), "dl4j_testpreprocdata")
			Dim f As New File(path)
			If f.exists() Then
				f.delete()
			End If
			f.mkdir()

			Dim iter As DataSetIterator = New IrisDataSetIterator(5, 150)
			Dim i As Integer = 0
			Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: java.io.File f2 = new java.io.File(org.apache.commons.io.FilenameUtils.concat(path, "data" + (i++) + ".bin"));
				Dim f2 As New File(FilenameUtils.concat(path, "data" & (i) & ".bin"))
					i += 1
				iter.Current.save(f2)
			Loop

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(Updater.RMSPROP).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.TANH).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).activation(Activation.SOFTMAX).build()).build()

			Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(1).repartionData(Repartition.Always).build())
			sparkNet.CollectTrainingStats = True

			sparkNet.fit("file:///" & path.replaceAll("\\", "/"))

			Dim sts As SparkTrainingStats = sparkNet.SparkTrainingStats
			Dim expNumFits As Integer = 12 '4 'fits' per averaging (4 executors, 1 averaging freq); 10 examples each -> 40 examples per fit. 150/40 = 3 averagings (round down); 3*4 = 12

			'Unfortunately: perfect partitioning isn't guaranteed by SparkUtils.balancedRandomSplit (esp. if original partitions are all size 1
			' which appears to be occurring at least some of the time), but we should get close to what we expect...
			assertTrue(Math.Abs(expNumFits - sts.getValue("ParameterAveragingWorkerFitTimesMs").Count) < 3)

			assertEquals(3, sts.getValue("ParameterAveragingMasterMapPartitionsTimesMs").Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreprocessedDataCompGraphDataSet()
		Public Overridable Sub testPreprocessedDataCompGraphDataSet()
			'Test _loading_ of preprocessed DataSet data
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim dataSetObjSize As Integer = 5
			Dim batchSizePerExecutor As Integer = 10

			Dim path As String = FilenameUtils.concat(System.getProperty("java.io.tmpdir"), "dl4j_testpreprocdata2")
			Dim f As New File(path)
			If f.exists() Then
				f.delete()
			End If
			f.mkdir()

			Dim iter As DataSetIterator = New IrisDataSetIterator(5, 150)
			Dim i As Integer = 0
			Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: java.io.File f2 = new java.io.File(org.apache.commons.io.FilenameUtils.concat(path, "data" + (i++) + ".bin"));
				Dim f2 As New File(FilenameUtils.concat(path, "data" & (i) & ".bin"))
					i += 1
				iter.Current.save(f2)
			Loop

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(Updater.RMSPROP).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.TANH).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Dim sparkNet As New SparkComputationGraph(sc, conf, (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(1).repartionData(Repartition.Always).build())
			sparkNet.CollectTrainingStats = True

			sparkNet.fit("file:///" & path.replaceAll("\\", "/"))

			Dim sts As SparkTrainingStats = sparkNet.SparkTrainingStats
			Dim expNumFits As Integer = 12 '4 'fits' per averaging (4 executors, 1 averaging freq); 10 examples each -> 40 examples per fit. 150/40 = 3 averagings (round down); 3*4 = 12

			'Unfortunately: perfect partitioning isn't guaranteed by SparkUtils.balancedRandomSplit (esp. if original partitions are all size 1
			' which appears to be occurring at least some of the time), but we should get close to what we expect...
			assertTrue(Math.Abs(expNumFits - sts.getValue("ParameterAveragingWorkerFitTimesMs").Count) < 3)

			assertEquals(3, sts.getValue("ParameterAveragingMasterMapPartitionsTimesMs").Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreprocessedDataCompGraphMultiDataSet() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPreprocessedDataCompGraphMultiDataSet()
			'Test _loading_ of preprocessed MultiDataSet data
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim dataSetObjSize As Integer = 5
			Dim batchSizePerExecutor As Integer = 10

			Dim path As String = FilenameUtils.concat(System.getProperty("java.io.tmpdir"), "dl4j_testpreprocdata3")
			Dim f As New File(path)
			If f.exists() Then
				f.delete()
			End If
			f.mkdir()

			Dim iter As DataSetIterator = New IrisDataSetIterator(5, 150)
			Dim i As Integer = 0
			Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: java.io.File f2 = new java.io.File(org.apache.commons.io.FilenameUtils.concat(path, "data" + (i++) + ".bin"));
				Dim f2 As New File(FilenameUtils.concat(path, "data" & (i) & ".bin"))
					i += 1
				Dim ds As DataSet = iter.Current
				Dim mds As New MultiDataSet(ds.Features, ds.Labels)
				mds.save(f2)
			Loop

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(Updater.RMSPROP).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.TANH).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Dim sparkNet As New SparkComputationGraph(sc, conf, (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(1).repartionData(Repartition.Always).build())
			sparkNet.CollectTrainingStats = True

			sparkNet.fitMultiDataSet("file:///" & path.replaceAll("\\", "/"))

			Dim sts As SparkTrainingStats = sparkNet.SparkTrainingStats
			Dim expNumFits As Integer = 12 '4 'fits' per averaging (4 executors, 1 averaging freq); 10 examples each -> 40 examples per fit. 150/40 = 3 averagings (round down); 3*4 = 12

			'Unfortunately: perfect partitioning isn't guaranteed by SparkUtils.balancedRandomSplit (esp. if original partitions are all size 1
			' which appears to be occurring at least some of the time), but we should get close to what we expect...
			assertTrue(Math.Abs(expNumFits - sts.getValue("ParameterAveragingWorkerFitTimesMs").Count) < 3)

			assertEquals(3, sts.getValue("ParameterAveragingMasterMapPartitionsTimesMs").Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCsvPreprocessedDataGeneration() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCsvPreprocessedDataGeneration()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim list As IList(Of String) = New List(Of String)()
			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 150)
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				list.Add(toString(ds.Features, Nd4j.argMax(ds.Labels, 1).getInt(0)))
			Loop

			Dim rdd As JavaRDD(Of String) = sc.parallelize(list)
			Dim partitions As Integer = rdd.partitions().size()

			Dim tempDir As URI = (New File(System.getProperty("java.io.tmpdir"))).toURI()
			Dim outputDir As New URI(tempDir.getPath() & "/dl4j_testPreprocessedData2")
			Dim temp As New File(outputDir.getPath())
			If temp.exists() Then
				FileUtils.deleteDirectory(temp)
			End If

			Dim numBinFiles As Integer = 0
			Try
				Dim batchSize As Integer = 5
				Dim labelIdx As Integer = 4
				Dim numPossibleLabels As Integer = 3

				rdd.foreachPartition(New StringToDataSetExportFunction(outputDir, New CSVRecordReader(0), batchSize, False, labelIdx, numPossibleLabels))

				Dim fileList() As File = (New File(outputDir.getPath())).listFiles()

				Dim totalExamples As Integer = 0
				For Each f2 As File In fileList
					If Not f2.getPath().EndsWith(".bin") Then
						Continue For
					End If
					'                System.out.println(f2.getPath());
					numBinFiles += 1

					Dim ds As New DataSet()
					ds.load(f2)

					assertEquals(4, ds.numInputs())
					assertEquals(3, ds.numOutcomes())

					totalExamples += ds.numExamples()
				Next f2

				assertEquals(150, totalExamples)
				assertTrue(Math.Abs(150 \ batchSize - numBinFiles) <= partitions) 'Expect 30, give or take due to partitioning randomness



				'Test the PortableDataStreamDataSetIterator:
				Dim pds As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(outputDir.getPath())
				Dim pdsList As IList(Of PortableDataStream) = pds.values().collect()

				Dim pdsIter As DataSetIterator = New PortableDataStreamDataSetIterator(pdsList)
				Dim pdsCount As Integer = 0
				Dim totalExamples2 As Integer = 0
				Do While pdsIter.MoveNext()
					Dim ds As DataSet = pdsIter.Current
					pdsCount += 1
					totalExamples2 += ds.numExamples()

					assertEquals(4, ds.numInputs())
					assertEquals(3, ds.numOutcomes())
				Loop

				assertEquals(150, totalExamples2)
				assertEquals(numBinFiles, pdsCount)
			Finally
				FileUtils.deleteDirectory(temp)
			End Try
		End Sub

		Private Shared Function toString(ByVal rowVector As INDArray, ByVal labelIdx As Integer) As String
			Dim sb As New StringBuilder()
			Dim length As Long = rowVector.length()
			For i As Integer = 0 To length - 1
				sb.Append(rowVector.getDouble(i))
				sb.Append(",")
			Next i
			sb.Append(labelIdx)
			Return sb.ToString()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCsvPreprocessedDataGenerationNoLabel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCsvPreprocessedDataGenerationNoLabel()
			'Same as above test, but without any labels (in which case: input and output arrays are the same)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim list As IList(Of String) = New List(Of String)()
			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 150)
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				list.Add(toString(ds.Features, Nd4j.argMax(ds.Labels, 1).getInt(0)))
			Loop

			Dim rdd As JavaRDD(Of String) = sc.parallelize(list)
			Dim partitions As Integer = rdd.partitions().size()

			Dim tempDir As URI = (New File(System.getProperty("java.io.tmpdir"))).toURI()
			Dim outputDir As New URI(tempDir.getPath() & "/dl4j_testPreprocessedData3")
			Dim temp As New File(outputDir.getPath())
			If temp.exists() Then
				FileUtils.deleteDirectory(temp)
			End If

			Dim numBinFiles As Integer = 0
			Try
				Dim batchSize As Integer = 5
				Dim labelIdx As Integer = -1
				Dim numPossibleLabels As Integer = -1

				rdd.foreachPartition(New StringToDataSetExportFunction(outputDir, New CSVRecordReader(0), batchSize, False, labelIdx, numPossibleLabels))

				Dim fileList() As File = (New File(outputDir.getPath())).listFiles()

				Dim totalExamples As Integer = 0
				For Each f2 As File In fileList
					If Not f2.getPath().EndsWith(".bin") Then
						Continue For
					End If
					'                System.out.println(f2.getPath());
					numBinFiles += 1

					Dim ds As New DataSet()
					ds.load(f2)

					assertEquals(5, ds.numInputs())
					assertEquals(5, ds.numOutcomes())

					totalExamples += ds.numExamples()
				Next f2

				assertEquals(150, totalExamples)
				assertTrue(Math.Abs(150 \ batchSize - numBinFiles) <= partitions) 'Expect 30, give or take due to partitioning randomness



				'Test the PortableDataStreamDataSetIterator:
				Dim pds As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(outputDir.getPath())
				Dim pdsList As IList(Of PortableDataStream) = pds.values().collect()

				Dim pdsIter As DataSetIterator = New PortableDataStreamDataSetIterator(pdsList)
				Dim pdsCount As Integer = 0
				Dim totalExamples2 As Integer = 0
				Do While pdsIter.MoveNext()
					Dim ds As DataSet = pdsIter.Current
					pdsCount += 1
					totalExamples2 += ds.numExamples()

					assertEquals(5, ds.numInputs())
					assertEquals(5, ds.numOutcomes())
				Loop

				assertEquals(150, totalExamples2)
				assertEquals(numBinFiles, pdsCount)
			Finally
				FileUtils.deleteDirectory(temp)
			End Try
		End Sub


	End Class

End Namespace