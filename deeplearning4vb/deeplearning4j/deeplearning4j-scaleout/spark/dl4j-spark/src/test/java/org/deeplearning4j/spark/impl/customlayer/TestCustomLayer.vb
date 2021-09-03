Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports CustomLayer = org.deeplearning4j.spark.impl.customlayer.layer.CustomLayer
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j.spark.impl.customlayer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) public class TestCustomLayer extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestCustomLayer
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSparkWithCustomLayer()
		Public Overridable Sub testSparkWithCustomLayer()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			'Basic test - checks whether exceptions etc are thrown with custom layers + spark
			'Custom layers are tested more extensively in dl4j core
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, New CustomLayer(3.14159)).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(10).nOut(10).build()).build()

			Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(1)).averagingFrequency(2).batchSizePerWorker(5).saveUpdater(True).workerPrefetchNumBatches(0).build()

			Dim net As New SparkDl4jMultiLayer(sc, conf, tm)

			Dim testData As IList(Of DataSet) = New List(Of DataSet)()
			Dim r As New Random(12345)
			For i As Integer = 0 To 199
				Dim f As INDArray = Nd4j.rand(1, 10)
				Dim l As INDArray = Nd4j.zeros(1, 10)
				l.putScalar(0, r.Next(10), 1.0)
				testData.Add(New DataSet(f, l))
			Next i

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(testData)
			net.fit(rdd)
		End Sub

	End Class

End Namespace