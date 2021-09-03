Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports org.deeplearning4j.spark.api
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports MapDBStatsStorage = org.deeplearning4j.ui.model.storage.mapdb.MapDBStatsStorage
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.deeplearning4j.spark.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Tag(TagNames.UI) public class TestListeners extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestListeners
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStatsCollection()
		Public Overridable Sub testStatsCollection()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim sc As JavaSparkContext = Context
			Dim nExecutors As Integer = numExecutors()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(100).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(100).nOut(3).activation(Activation.SOFTMAX).weightInit(WeightInit.XAVIER).build()).build()



			Dim network As New MultiLayerNetwork(conf)
			network.init()


			Dim tm As TrainingMaster = (New ParameterAveragingTrainingMaster.Builder(1)).batchSizePerWorker(5).averagingFrequency(6).build()

			Dim net As New SparkDl4jMultiLayer(sc, conf, tm)
			Dim ss As StatsStorage = New MapDBStatsStorage() 'In-memory

			net.setListeners(ss, Collections.singletonList(New StatsListener(Nothing)))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim list As IList(Of DataSet) = (New IrisDataSetIterator(120, 150)).next().asList()
			'120 examples, 4 executors, 30 examples per executor -> 6 updates of size 5 per executor

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

			net.fit(rdd)

			Dim sessions As IList(Of String) = ss.listSessionIDs()
	'        System.out.println("Sessions: " + sessions);
			assertEquals(1, sessions.Count)

			Dim sid As String = sessions(0)

			Dim typeIDs As IList(Of String) = ss.listTypeIDsForSession(sid)
			Dim workers As IList(Of String) = ss.listWorkerIDsForSession(sid)

	'        System.out.println(sid + "\t" + typeIDs + "\t" + workers);

			Dim lastUpdates As IList(Of Persistable) = ss.getLatestUpdateAllWorkers(sid, StatsListener.TYPE_ID)
	'        System.out.println(lastUpdates);

	'        System.out.println("Static info:");
			For Each wid As String In workers
				Dim staticInfo As Persistable = ss.getStaticInfo(sid, StatsListener.TYPE_ID, wid)
	'            System.out.println(sid + "\t" + wid);
			Next wid

			assertEquals(1, typeIDs.Count)
			assertEquals(numExecutors(), workers.Count)
			Dim firstWorker As String = workers(0)
			Dim firstWorkerSubstring As String = workers(0).Substring(0, firstWorker.Length - 1)
			For Each wid As String In workers
				Dim widSubstring As String = wid.Substring(0, wid.Length - 1)
				assertEquals(firstWorkerSubstring, widSubstring)

								Dim tempVar = wid.length() - 1
				Dim counterVal As String = wid.Substring(tempVar, wid.Length - (tempVar))
				Dim cv As Integer = Integer.Parse(counterVal)
				assertTrue(0 <= cv AndAlso cv < numExecutors())
			Next wid
		End Sub
	End Class

End Namespace