Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports Platform = com.sun.jna.Platform
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Repartition = org.deeplearning4j.spark.api.Repartition
Imports CommonSparkTrainingStats = org.deeplearning4j.spark.api.stats.CommonSparkTrainingStats
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports ParameterAveragingTrainingMasterStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingMasterStats
Imports ParameterAveragingTrainingWorkerStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingWorkerStats
Imports EventStats = org.deeplearning4j.spark.stats.EventStats
Imports StatsUtils = org.deeplearning4j.spark.stats.StatsUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.spark.impl.stats


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestTrainingStatsCollection extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestTrainingStatsCollection
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStatsCollection() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStatsCollection()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim nWorkers As Integer = numExecutors()

			Dim sc As JavaSparkContext = Context

			Try

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(10).build()).build()

				Dim miniBatchSizePerWorker As Integer = 10
				Dim averagingFrequency As Integer = 5
				Dim numberOfAveragings As Integer = 3

				Dim totalExamples As Integer = nWorkers * miniBatchSizePerWorker * averagingFrequency * numberOfAveragings

				Nd4j.Random.setSeed(12345)
				Dim list As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To totalExamples - 1
					Dim f As INDArray = Nd4j.rand(1, 10)
					Dim l As INDArray = Nd4j.rand(1, 10)
					Dim ds As New DataSet(f, l)
					list.Add(ds)
				Next i

				Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)
				rdd.repartition(4)

				Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(nWorkers, 1)).averagingFrequency(averagingFrequency).batchSizePerWorker(miniBatchSizePerWorker).saveUpdater(True).workerPrefetchNumBatches(0).repartionData(Repartition.Always).build()

				Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, tm)
				sparkNet.CollectTrainingStats = True
				sparkNet.fit(rdd)


				'Collect the expected keys:
				Dim expectedStatNames As IList(Of String) = New List(Of String)()
				Dim classes() As Type = {GetType(CommonSparkTrainingStats), GetType(ParameterAveragingTrainingMasterStats), GetType(ParameterAveragingTrainingWorkerStats)}
				Dim fieldNames() As String = {"columnNames", "columnNames", "columnNames"}
				For i As Integer = 0 To classes.Length - 1
					Dim field As System.Reflection.FieldInfo = classes(i).getDeclaredField(fieldNames(i))
					field.setAccessible(True)
					Dim f As Object = field.get(Nothing)
					Dim c As ICollection(Of String) = DirectCast(f, ICollection(Of String))
					CType(expectedStatNames, List(Of String)).AddRange(c)
				Next i

	'            System.out.println(expectedStatNames);


				Dim stats As SparkTrainingStats = sparkNet.SparkTrainingStats
				Dim actualKeySet As ISet(Of String) = stats.getKeySet()
				assertEquals(expectedStatNames.Count, actualKeySet.Count)
				For Each s As String In stats.getKeySet()
					assertTrue(expectedStatNames.Contains(s))
					assertNotNull(stats.getValue(s))
				Next s

				Dim statsAsString As String = stats.statsAsString()
	'            System.out.println(statsAsString);
				assertEquals(actualKeySet.Count, statsAsString.Split(vbLf, True).length) 'One line per stat


				'Go through nested stats
				'First: master stats
				assertTrue(TypeOf stats Is ParameterAveragingTrainingMasterStats)
				Dim masterStats As ParameterAveragingTrainingMasterStats = DirectCast(stats, ParameterAveragingTrainingMasterStats)

				Dim exportTimeStats As IList(Of EventStats) = masterStats.getParameterAveragingMasterExportTimesMs()
				assertEquals(1, exportTimeStats.Count)
				assertDurationGreaterZero(exportTimeStats)
				assertNonNullFields(exportTimeStats)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(exportTimeStats, 1, 1, 1)

				Dim countRddTime As IList(Of EventStats) = masterStats.getParameterAveragingMasterCountRddSizeTimesMs()
				assertEquals(1, countRddTime.Count) 'occurs once per fit
				assertDurationGreaterEqZero(countRddTime)
				assertNonNullFields(countRddTime)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(countRddTime, 1, 1, 1) 'should occur only in master once

				Dim broadcastCreateTime As IList(Of EventStats) = masterStats.getParameterAveragingMasterBroadcastCreateTimesMs()
				assertEquals(numberOfAveragings, broadcastCreateTime.Count)
				assertDurationGreaterEqZero(broadcastCreateTime)
				assertNonNullFields(broadcastCreateTime)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(broadcastCreateTime, 1, 1, 1) 'only 1 thread for master

				Dim fitTimes As IList(Of EventStats) = masterStats.getParameterAveragingMasterFitTimesMs()
				assertEquals(1, fitTimes.Count) 'i.e., number of times fit(JavaRDD<DataSet>) was called
				assertDurationGreaterZero(fitTimes)
				assertNonNullFields(fitTimes)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(fitTimes, 1, 1, 1) 'only 1 thread for master

				Dim splitTimes As IList(Of EventStats) = masterStats.getParameterAveragingMasterSplitTimesMs()
				assertEquals(1, splitTimes.Count) 'Splitting of the data set is executed once only (i.e., one fit(JavaRDD<DataSet>) call)
				assertDurationGreaterEqZero(splitTimes)
				assertNonNullFields(splitTimes)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(splitTimes, 1, 1, 1) 'only 1 thread for master

				Dim aggregateTimesMs As IList(Of EventStats) = masterStats.getParamaterAveragingMasterAggregateTimesMs()
				assertEquals(numberOfAveragings, aggregateTimesMs.Count)
				assertDurationGreaterEqZero(aggregateTimesMs)
				assertNonNullFields(aggregateTimesMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(aggregateTimesMs, 1, 1, 1) 'only 1 thread for master

				Dim processParamsTimesMs As IList(Of EventStats) = masterStats.getParameterAveragingMasterProcessParamsUpdaterTimesMs()
				assertEquals(numberOfAveragings, processParamsTimesMs.Count)
				assertDurationGreaterEqZero(processParamsTimesMs)
				assertNonNullFields(processParamsTimesMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(processParamsTimesMs, 1, 1, 1) 'only 1 thread for master

				Dim repartitionTimesMs As IList(Of EventStats) = masterStats.getParameterAveragingMasterRepartitionTimesMs()
				assertEquals(numberOfAveragings, repartitionTimesMs.Count)
				assertDurationGreaterEqZero(repartitionTimesMs)
				assertNonNullFields(repartitionTimesMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(repartitionTimesMs, 1, 1, 1) 'only 1 thread for master

				'Second: Common spark training stats
				Dim commonStats As SparkTrainingStats = masterStats.NestedTrainingStats
				assertNotNull(commonStats)
				assertTrue(TypeOf commonStats Is CommonSparkTrainingStats)
				Dim cStats As CommonSparkTrainingStats = DirectCast(commonStats, CommonSparkTrainingStats)
				Dim workerFlatMapTotalTimeMs As IList(Of EventStats) = cStats.getWorkerFlatMapTotalTimeMs()
				assertEquals(numberOfAveragings * nWorkers, workerFlatMapTotalTimeMs.Count)
				assertDurationGreaterZero(workerFlatMapTotalTimeMs)
				assertNonNullFields(workerFlatMapTotalTimeMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(workerFlatMapTotalTimeMs, 1, 1, nWorkers)

				Dim workerFlatMapGetInitialModelTimeMs As IList(Of EventStats) = cStats.getWorkerFlatMapGetInitialModelTimeMs()
				assertEquals(numberOfAveragings * nWorkers, workerFlatMapGetInitialModelTimeMs.Count)
				assertDurationGreaterEqZero(workerFlatMapGetInitialModelTimeMs)
				assertNonNullFields(workerFlatMapGetInitialModelTimeMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(workerFlatMapGetInitialModelTimeMs, 1, 1, nWorkers)

				Dim workerFlatMapDataSetGetTimesMs As IList(Of EventStats) = cStats.getWorkerFlatMapDataSetGetTimesMs()
				Dim numMinibatchesProcessed As Integer = workerFlatMapDataSetGetTimesMs.Count
				Dim expectedNumMinibatchesProcessed As Integer = numberOfAveragings * nWorkers * averagingFrequency '1 for every time we get a data set

				'Sometimes random split is just bad - some executors might miss out on getting the expected amount of data
				assertTrue(numMinibatchesProcessed >= expectedNumMinibatchesProcessed - 5)

				Dim workerFlatMapProcessMiniBatchTimesMs As IList(Of EventStats) = cStats.getWorkerFlatMapProcessMiniBatchTimesMs()
				assertTrue(workerFlatMapProcessMiniBatchTimesMs.Count >= numberOfAveragings * nWorkers * averagingFrequency - 5)
				assertDurationGreaterEqZero(workerFlatMapProcessMiniBatchTimesMs)
				assertNonNullFields(workerFlatMapDataSetGetTimesMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(workerFlatMapDataSetGetTimesMs, 1, 1, nWorkers)

				'Third: ParameterAveragingTrainingWorker stats
				Dim paramAvgStats As SparkTrainingStats = cStats.NestedTrainingStats
				assertNotNull(paramAvgStats)
				assertTrue(TypeOf paramAvgStats Is ParameterAveragingTrainingWorkerStats)

				Dim pStats As ParameterAveragingTrainingWorkerStats = DirectCast(paramAvgStats, ParameterAveragingTrainingWorkerStats)
				Dim parameterAveragingWorkerBroadcastGetValueTimeMs As IList(Of EventStats) = pStats.getParameterAveragingWorkerBroadcastGetValueTimeMs()
				assertEquals(numberOfAveragings * nWorkers, parameterAveragingWorkerBroadcastGetValueTimeMs.Count)
				assertDurationGreaterEqZero(parameterAveragingWorkerBroadcastGetValueTimeMs)
				assertNonNullFields(parameterAveragingWorkerBroadcastGetValueTimeMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(parameterAveragingWorkerBroadcastGetValueTimeMs, 1, 1, nWorkers)

				Dim parameterAveragingWorkerInitTimeMs As IList(Of EventStats) = pStats.getParameterAveragingWorkerInitTimeMs()
				assertEquals(numberOfAveragings * nWorkers, parameterAveragingWorkerInitTimeMs.Count)
				assertDurationGreaterEqZero(parameterAveragingWorkerInitTimeMs)
				assertNonNullFields(parameterAveragingWorkerInitTimeMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(parameterAveragingWorkerInitTimeMs, 1, 1, nWorkers)

				Dim parameterAveragingWorkerFitTimesMs As IList(Of EventStats) = pStats.getParameterAveragingWorkerFitTimesMs()
				assertTrue(parameterAveragingWorkerFitTimesMs.Count >= numberOfAveragings * nWorkers * averagingFrequency - 5)
				assertDurationGreaterEqZero(parameterAveragingWorkerFitTimesMs)
				assertNonNullFields(parameterAveragingWorkerFitTimesMs)
				assertExpectedNumberMachineIdsJvmIdsThreadIds(parameterAveragingWorkerFitTimesMs, 1, 1, nWorkers)

				assertNull(pStats.NestedTrainingStats)


				'Finally: try exporting stats
				Dim tempDir As String = System.getProperty("java.io.tmpdir")
				Dim outDir As String = FilenameUtils.concat(tempDir, "dl4j_testTrainingStatsCollection")
				stats.exportStatFiles(outDir, sc.sc())

				Dim htmlPlotsPath As String = FilenameUtils.concat(outDir, "AnalysisPlots.html")
				StatsUtils.exportStatsAsHtml(stats, htmlPlotsPath, sc)

				Dim baos As New MemoryStream()
				StatsUtils.exportStatsAsHTML(stats, baos)
				baos.Close()
				Dim bytes() As SByte = baos.toByteArray()
				Dim str As String = StringHelper.NewString(bytes, "UTF-8")
				'            System.out.println(str);
			Finally
				sc.stop()
			End Try
		End Sub

		Private Shared Sub assertDurationGreaterEqZero(ByVal array As IList(Of EventStats))
			For Each e As EventStats In array
				assertTrue(e.DurationMs >= 0)
			Next e
		End Sub

		Private Shared Sub assertDurationGreaterZero(ByVal array As IList(Of EventStats))
			For Each e As EventStats In array
				assertTrue(e.DurationMs > 0)
			Next e
		End Sub

		Private Shared Sub assertNonNullFields(ByVal array As IList(Of EventStats))
			For Each e As EventStats In array
				assertNotNull(e.MachineID)
				assertNotNull(e.JvmID)
				assertNotNull(e.DurationMs)
				assertFalse(e.MachineID.Length = 0)
				assertFalse(e.JvmID.Length = 0)
				assertTrue(e.ThreadID > 0)
			Next e
		End Sub

		Private Shared Sub assertExpectedNumberMachineIdsJvmIdsThreadIds(ByVal events As IList(Of EventStats), ByVal expNMachineIDs As Integer, ByVal expNumJvmIds As Integer, ByVal expNumThreadIds As Integer)
			Dim machineIDs As ISet(Of String) = New HashSet(Of String)()
			Dim jvmIDs As ISet(Of String) = New HashSet(Of String)()
			Dim threadIDs As ISet(Of Long) = New HashSet(Of Long)()
			For Each e As EventStats In events
				machineIDs.Add(e.MachineID)
				jvmIDs.Add(e.JvmID)
				threadIDs.Add(e.ThreadID)
			Next e
			assertTrue(machineIDs.Count = expNMachineIDs)
			assertTrue(jvmIDs.Count = expNumJvmIds)
			assertTrue(threadIDs.Count = expNumThreadIds)
		End Sub
	End Class

End Namespace