Imports System
Imports System.Collections.Generic
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.layers
Imports GaussianReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.GaussianReconstructionDistribution
Imports LossFunctionWrapper = org.deeplearning4j.nn.conf.layers.variational.LossFunctionWrapper
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports VariationalAutoencoder = org.deeplearning4j.nn.layers.variational.VariationalAutoencoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports org.deeplearning4j.spark.impl.multilayer.scoring
Imports org.deeplearning4j.spark.impl.multilayer.scoring
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
Imports Tuple2 = scala.Tuple2
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestMiscFunctions extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestMiscFunctions
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFeedForwardWithKey()
		Public Overridable Sub testFeedForwardWithKey()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).activation(Activation.SOFTMAX).build()).build()


			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()


			Dim expected As IList(Of INDArray) = New List(Of INDArray)()
			Dim mapFeatures As IList(Of Tuple2(Of Integer, INDArray)) = New List(Of Tuple2(Of Integer, INDArray))()
			Dim count As Integer = 0
			Dim arrayCount As Integer = 0
			Dim r As New Random(12345)
			Do While count < 150
				Dim exampleCount As Integer = r.Next(5) + 1 '1 to 5 inclusive examples
				If count + exampleCount > 150 Then
					exampleCount = 150 - count
				End If

				Dim subset As INDArray = ds.Features.get(NDArrayIndex.interval(count, count + exampleCount), NDArrayIndex.all())

				expected.Add(net.output(subset, False))
				mapFeatures.Add(New Tuple2(Of Integer, INDArray)(arrayCount, subset))
				arrayCount += 1
				count += exampleCount
			Loop

	'        JavaPairRDD<Integer, INDArray> rdd = sc.parallelizePairs(mapFeatures);
			Dim rdd As JavaPairRDD(Of Integer, INDArray) = sc.parallelizePairs(mapFeatures)

			Dim multiLayer As New SparkDl4jMultiLayer(sc, net, Nothing)
			Dim map As IDictionary(Of Integer, INDArray) = multiLayer.feedForwardWithKey(rdd, 16).collectAsMap()

			For i As Integer = 0 To expected.Count - 1
				Dim exp As INDArray = expected(i)
				Dim act As INDArray = map(i)

				assertEquals(exp, act)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFeedForwardWithKeyInputMask()
		Public Overridable Sub testFeedForwardWithKeyInputMask()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nIn(4).nOut(3).build()).layer(New GlobalPoolingLayer(PoolingType.AVG)).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).activation(Activation.SOFTMAX).build()).build()


			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim ds As IList(Of org.nd4j.linalg.dataset.DataSet) = New List(Of org.nd4j.linalg.dataset.DataSet) From {
				New org.nd4j.linalg.dataset.DataSet(Nd4j.rand(New Integer(){1, 4, 5}), Nd4j.create(New Double(){1, 1, 1, 0, 0})),
				New org.nd4j.linalg.dataset.DataSet(Nd4j.rand(New Integer(){1, 4, 5}), Nd4j.create(New Double(){1, 1, 1, 1, 0})),
				New org.nd4j.linalg.dataset.DataSet(Nd4j.rand(New Integer(){1, 4, 5}), Nd4j.create(New Double(){1, 1, 1, 1, 1}))
			}


			Dim expected As IDictionary(Of Integer, INDArray) = New Dictionary(Of Integer, INDArray)()
			Dim mapFeatures As IList(Of Tuple2(Of Integer, Tuple2(Of INDArray, INDArray))) = New List(Of Tuple2(Of Integer, Tuple2(Of INDArray, INDArray)))()
			Dim count As Integer = 0
			Dim arrayCount As Integer = 0
			Dim r As New Random(12345)


			Dim i As Integer=0
			For Each d As org.nd4j.linalg.dataset.DataSet In ds

				Dim f As INDArray = d.Features
				Dim fm As INDArray = d.FeaturesMaskArray

				mapFeatures.Add(New Tuple2(Of Integer, Tuple2(Of INDArray, INDArray))(i, New Tuple2(Of Integer, Tuple2(Of INDArray, INDArray))(f, fm)))

				Dim [out] As INDArray = net.output(f, False, fm, Nothing)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: expected.put(i++, out);
				expected(i) = [out]
					i += 1
			Next d

			Dim rdd As JavaPairRDD(Of Integer, Tuple2(Of INDArray, INDArray)) = sc.parallelizePairs(mapFeatures)

			Dim multiLayer As New SparkDl4jMultiLayer(sc, net, Nothing)
			Dim map As IDictionary(Of Integer, INDArray) = multiLayer.feedForwardWithMaskAndKey(rdd, 16).collectAsMap()

			For i = 0 To expected.Count - 1
				Dim exp As INDArray = expected(i)
				Dim act As INDArray = map(i)

				assertEquals(exp, act)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFeedForwardWithKeyGraph()
		Public Overridable Sub testFeedForwardWithKeyGraph()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in1", "in2").addLayer("0", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "in1").addLayer("1", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "in2").addLayer("2", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(6).nOut(3).activation(Activation.SOFTMAX).build(), "0", "1").setOutputs("2").build()


			Dim net As New ComputationGraph(conf)
			net.init()

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()


			Dim expected As IList(Of INDArray) = New List(Of INDArray)()
			Dim mapFeatures As IList(Of Tuple2(Of Integer, INDArray())) = New List(Of Tuple2(Of Integer, INDArray()))()
			Dim count As Integer = 0
			Dim arrayCount As Integer = 0
			Dim r As New Random(12345)
			Do While count < 150
				Dim exampleCount As Integer = r.Next(5) + 1 '1 to 5 inclusive examples
				If count + exampleCount > 150 Then
					exampleCount = 150 - count
				End If

				Dim subset As INDArray = ds.Features.get(NDArrayIndex.interval(count, count + exampleCount), NDArrayIndex.all())

				expected.Add(net.outputSingle(False, subset, subset))
				mapFeatures.Add(New Tuple2(Of Integer, INDArray())(arrayCount, New INDArray() {subset, subset}))
				arrayCount += 1
				count += exampleCount
			Loop

			Dim rdd As JavaPairRDD(Of Integer, INDArray()) = sc.parallelizePairs(mapFeatures)

			Dim graph As New SparkComputationGraph(sc, net, Nothing)
			Dim map As IDictionary(Of Integer, INDArray()) = graph.feedForwardWithKey(rdd, 16).collectAsMap()

			For i As Integer = 0 To expected.Count - 1
				Dim exp As INDArray = expected(i)
				Dim act As INDArray = map(i)(0)

				assertEquals(exp, act)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVaeReconstructionProbabilityWithKey()
		Public Overridable Sub testVaeReconstructionProbabilityWithKey()

			'Simple test. We can't do a direct comparison, as the reconstruction probabilities are stochastic
			' due to sampling

			Dim nIn As Integer = 10

			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder.Builder()).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.IDENTITY)).nIn(nIn).nOut(5).encoderLayerSizes(12).decoderLayerSizes(13).build()).build()

			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			Dim toScore As IList(Of Tuple2(Of Integer, INDArray)) = New List(Of Tuple2(Of Integer, INDArray))()
			For i As Integer = 0 To 99
				Dim arr As INDArray = Nd4j.rand(1, nIn)
				toScore.Add(New Tuple2(Of Integer, INDArray)(i, arr))
			Next i

			Dim rdd As JavaPairRDD(Of Integer, INDArray) = sc.parallelizePairs(toScore)

			Dim reconstr As JavaPairRDD(Of Integer, Double) = rdd.mapPartitionsToPair(New VaeReconstructionProbWithKeyFunction(Of Integer)(sc.broadcast(net.params()), sc.broadcast(mlc.toJson()), True, 16, 128))

			Dim l As IDictionary(Of Integer, Double) = reconstr.collectAsMap()

			assertEquals(100, l.Count)

			For i As Integer = 0 To 99
				assertTrue(l.ContainsKey(i))
				assertTrue(l(i) < 0.0) 'log probability: should be negative
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVaeReconstructionErrorWithKey()
		Public Overridable Sub testVaeReconstructionErrorWithKey()
			'Simple test. We CAN do a direct comparison here vs. local, as reconstruction error is deterministic

			Dim nIn As Integer = 10

			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder.Builder()).reconstructionDistribution(New LossFunctionWrapper(Activation.IDENTITY, New LossMSE())).nIn(nIn).nOut(5).encoderLayerSizes(12).decoderLayerSizes(13).build()).build()

			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			Dim vae As VariationalAutoencoder = DirectCast(net.getLayer(0), VariationalAutoencoder)

			Dim toScore As IList(Of Tuple2(Of Integer, INDArray)) = New List(Of Tuple2(Of Integer, INDArray))()
			For i As Integer = 0 To 99
				Dim arr As INDArray = Nd4j.rand(1, nIn)
				toScore.Add(New Tuple2(Of Integer, INDArray)(i, arr))
			Next i

			Dim rdd As JavaPairRDD(Of Integer, INDArray) = sc.parallelizePairs(toScore)

			Dim reconstrErrors As JavaPairRDD(Of Integer, Double) = rdd.mapPartitionsToPair(New VaeReconstructionErrorWithKeyFunction(Of Integer)(sc.broadcast(net.params()), sc.broadcast(mlc.toJson()), 16))

			Dim l As IDictionary(Of Integer, Double) = reconstrErrors.collectAsMap()

			assertEquals(100, l.Count)

			For i As Integer = 0 To 99
				assertTrue(l.ContainsKey(i))

				Dim localToScore As INDArray = toScore(i)._2()
				Dim localScore As Double = vae.reconstructionError(localToScore).data().asDouble()(0)

				assertEquals(localScore, l(i), 1e-6)
			Next i
		End Sub

	End Class

End Namespace