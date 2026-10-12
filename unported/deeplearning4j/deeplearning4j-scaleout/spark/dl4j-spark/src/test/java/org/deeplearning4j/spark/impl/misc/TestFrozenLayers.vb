Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports FineTuneConfiguration = org.deeplearning4j.nn.transferlearning.FineTuneConfiguration
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports RDDTrainingApproach = org.deeplearning4j.spark.api.RDDTrainingApproach
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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

Namespace org.deeplearning4j.spark.impl.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestFrozenLayers extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestFrozenLayers
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSparkFrozenLayers()
		Public Overridable Sub testSparkFrozenLayers()

			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.TANH)

			Dim finetune As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).build()

			Dim nIn As Integer = 6
			Dim nOut As Integer = 3

			Dim origModel As New MultiLayerNetwork(overallConf.clone().list().layer(0, (New DenseLayer.Builder()).nIn(6).nOut(5).build()).layer(1, (New DenseLayer.Builder()).nIn(5).nOut(4).build()).layer(2, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(3, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			origModel.init()

			Dim withFrozen As MultiLayerNetwork = (New TransferLearning.Builder(origModel)).fineTuneConfiguration(finetune).setFeatureExtractor(1).build()

			Dim m As IDictionary(Of String, INDArray) = withFrozen.paramTable()
			Dim pCopy As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each entry As KeyValuePair(Of String, INDArray) In m.SetOfKeyValuePairs()
				pCopy(entry.Key) = entry.Value.dup()
			Next entry


			Dim avgFreq As Integer = 2
			Dim batchSize As Integer = 8
			Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(batchSize)).averagingFrequency(avgFreq).batchSizePerWorker(batchSize).rddTrainingApproach(RDDTrainingApproach.Direct).workerPrefetchNumBatches(0).build()

			Dim sNet As New SparkDl4jMultiLayer(sc, withFrozen.clone(), tm)

			assertTrue(TypeOf withFrozen.getLayer(0) Is FrozenLayer)
			assertTrue(TypeOf withFrozen.getLayer(1) Is FrozenLayer)

			Dim numMinibatches As Integer = 4 * sc.defaultParallelism()

			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To numMinibatches - 1
				Dim f As INDArray = Nd4j.rand(batchSize, nIn)
				Dim l As INDArray = Nd4j.zeros(batchSize, nOut)
				For j As Integer = 0 To batchSize - 1
					l.putScalar(j, j Mod nOut, 1.0)
				Next j
				list.Add(New DataSet(f, l))
			Next i

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

			sNet.fit(rdd)

			Dim fitted As MultiLayerNetwork = sNet.Network

			Dim fittedParams As IDictionary(Of String, INDArray) = fitted.paramTable()

			For Each entry As KeyValuePair(Of String, INDArray) In fittedParams.SetOfKeyValuePairs()
				Dim orig As INDArray = pCopy(entry.Key)
				Dim now As INDArray = entry.Value
				Dim isFrozen As Boolean = entry.Key.StartsWith("0_") OrElse entry.Key.StartsWith("1_")

				If isFrozen Then
					'Layer should be frozen -> no change
					assertEquals(orig, now, entry.Key)
				Else
					'Not frozen -> should be different
					assertNotEquals(orig, now, entry.Key)
				End If
			Next entry
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSparkFrozenLayersCompGraph()
		Public Overridable Sub testSparkFrozenLayersCompGraph()

			Dim finetune As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).build()

			Dim nIn As Integer = 6
			Dim nOut As Integer = 3

			Dim origModel As New ComputationGraph((New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.TANH).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(6).nOut(5).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(5).nOut(4).build(), "0").addLayer("2", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "1").addLayer("3", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "2").setOutputs("3").build())
			origModel.init()

			Dim withFrozen As ComputationGraph = (New TransferLearning.GraphBuilder(origModel)).fineTuneConfiguration(finetune).setFeatureExtractor("1").build()

			Dim m As IDictionary(Of String, INDArray) = withFrozen.paramTable()
			Dim pCopy As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each entry As KeyValuePair(Of String, INDArray) In m.SetOfKeyValuePairs()
				pCopy(entry.Key) = entry.Value.dup()
			Next entry


			Dim avgFreq As Integer = 2
			Dim batchSize As Integer = 8
			Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(batchSize)).averagingFrequency(avgFreq).batchSizePerWorker(batchSize).rddTrainingApproach(RDDTrainingApproach.Direct).workerPrefetchNumBatches(0).build()

			Dim sNet As New SparkComputationGraph(sc, withFrozen.clone(), tm)

			assertTrue(TypeOf withFrozen.getLayer(0) Is FrozenLayer)
			assertTrue(TypeOf withFrozen.getLayer(1) Is FrozenLayer)

			Dim numMinibatches As Integer = 4 * sc.defaultParallelism()

			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To numMinibatches - 1
				Dim f As INDArray = Nd4j.rand(batchSize, nIn)
				Dim l As INDArray = Nd4j.zeros(batchSize, nOut)
				For j As Integer = 0 To batchSize - 1
					l.putScalar(j, j Mod nOut, 1.0)
				Next j
				list.Add(New DataSet(f, l))
			Next i

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

			sNet.fit(rdd)

			Dim fitted As ComputationGraph = sNet.Network

			Dim fittedParams As IDictionary(Of String, INDArray) = fitted.paramTable()

			For Each entry As KeyValuePair(Of String, INDArray) In fittedParams.SetOfKeyValuePairs()
				Dim orig As INDArray = pCopy(entry.Key)
				Dim now As INDArray = entry.Value
				Dim isFrozen As Boolean = entry.Key.StartsWith("0_") OrElse entry.Key.StartsWith("1_")

				If isFrozen Then
					'Layer should be frozen -> no change
					assertEquals(orig, now, entry.Key)
				Else
					'Not frozen -> should be different
					assertNotEquals(orig, now, entry.Key)
				End If
			Next entry
		End Sub

	End Class

End Namespace