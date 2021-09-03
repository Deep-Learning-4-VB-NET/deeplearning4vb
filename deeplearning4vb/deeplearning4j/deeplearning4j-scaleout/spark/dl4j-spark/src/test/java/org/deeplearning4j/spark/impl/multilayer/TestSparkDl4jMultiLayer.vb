Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports org.deeplearning4j.spark.api
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
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

Namespace org.deeplearning4j.spark.impl.multilayer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestSparkDl4jMultiLayer extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestSparkDl4jMultiLayer
		Inherits BaseSparkTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000L
			End Get
		End Property

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

		Public Overrides ReadOnly Property DefaultFPDataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEvaluationSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEvaluationSimple()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Nd4j.Random.setSeed(12345)

			For Each evalWorkers As Integer In New Integer(){1, 4, 8}
				'Simple test to validate DL4J issue 4099 is fixed...

				Dim numEpochs As Integer = 1
				Dim batchSizePerWorker As Integer = 8

				'Load the data into memory then parallelize
				'This isn't a good approach in general - but is simple to use for this example
				Dim iterTrain As DataSetIterator = New MnistDataSetIterator(batchSizePerWorker, True, 12345)
				Dim iterTest As DataSetIterator = New MnistDataSetIterator(batchSizePerWorker, False, 12345)
				Dim trainDataList As IList(Of DataSet) = New List(Of DataSet)()
				Dim testDataList As IList(Of DataSet) = New List(Of DataSet)()
				Dim count As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (iterTrain.hasNext() && count++ < 30)
				Do While iterTrain.MoveNext() AndAlso count++ < 30
					trainDataList.Add(iterTrain.Current)
				Loop
				Do While iterTest.MoveNext()
					testDataList.Add(iterTest.Current)
				Loop

				Dim trainData As JavaRDD(Of DataSet) = sc.parallelize(trainDataList)
				Dim testData As JavaRDD(Of DataSet) = sc.parallelize(testDataList)


				'----------------------------------
				'Create network configuration and conduct network training
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).activation(Activation.LEAKYRELU).weightInit(WeightInit.XAVIER).updater(New Adam(1e-3)).l2(1e-5).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28).nOut(500).build()).layer(1, (New DenseLayer.Builder()).nIn(500).nOut(100).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).activation(Activation.SOFTMAX).nIn(100).nOut(10).build()).build()

				'Configuration for Spark training: see https://deeplearning4j.konduit.ai/distributed-deep-learning/howto for explanation of these configuration options

				Dim tm As TrainingMaster = (New ParameterAveragingTrainingMaster.Builder(batchSizePerWorker)).averagingFrequency(2).build()

				'Create the Spark network
				Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, tm)
				sparkNet.DefaultEvaluationWorkers = evalWorkers

				'Execute training:
				For i As Integer = 0 To numEpochs - 1
					sparkNet.fit(trainData)
				Next i

				'Perform evaluation (distributed)
				Dim evaluation As Evaluation = sparkNet.evaluate(testData)
				log.info("***** Evaluation *****")
				log.info(evaluation.stats())

				'Delete the temp training files, now that we are done with them
				tm.deleteTempFiles(sc)

				assertEquals(10000, evaluation.NumRowCounter) '10k test set
				assertTrue(Not Double.IsNaN(evaluation.accuracy()))
				assertTrue(evaluation.accuracy() >= 0.10)
				assertTrue(evaluation.precision() >= 0.10)
				assertTrue(evaluation.recall() >= 0.10)
				assertTrue(evaluation.f1() >= 0.10)
			Next evalWorkers
		End Sub


	End Class

End Namespace