Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports GaussianDistribution = org.deeplearning4j.nn.conf.distribution.GaussianDistribution
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports org.deeplearning4j.nn.conf.graph
Imports DuplicateToTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.DuplicateToTimeSeriesVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports ReverseTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.ReverseTimeSeriesVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
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

Namespace org.deeplearning4j.gradientcheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) public class GradientCheckTestsComputationGraph extends org.deeplearning4j.BaseDL4JTest
	Public Class GradientCheckTestsComputationGraph
		Inherits BaseDL4JTest

		Public Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-9

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 999999999L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicIris()
		Public Overridable Sub testBasicIris()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").addLayer("firstLayer", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build(), "input").addLayer("outputLayer", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(5).nOut(3).build(), "firstLayer").setOutputs("outputLayer").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()
			Dim min As INDArray = ds.Features.min(0)
			Dim max As INDArray = ds.Features.max(0)
			ds.Features.subiRowVector(min).diviRowVector(max.sub(min))
			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels

			If PRINT_RESULTS Then
				Console.WriteLine("testBasicIris()")
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

			Dim msg As String = "testBasicIris()"
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicIrisWithMerging()
		Public Overridable Sub testBasicIrisWithMerging()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").addLayer("l1", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build(), "input").addLayer("l2", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build(), "input").addVertex("merge", New MergeVertex(), "l1", "l2").addLayer("outputLayer", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(5 + 5).nOut(3).build(), "merge").setOutputs("outputLayer").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim numParams As Integer = (4 * 5 + 5) + (4 * 5 + 5) + (10 * 3 + 3)
			assertEquals(numParams, graph.numParams())

			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()
			Dim min As INDArray = ds.Features.min(0)
			Dim max As INDArray = ds.Features.max(0)
			ds.Features.subiRowVector(min).diviRowVector(max.sub(min))
			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels

			If PRINT_RESULTS Then
				Console.WriteLine("testBasicIrisWithMerging()")
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

			Dim msg As String = "testBasicIrisWithMerging()"
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicIrisWithElementWiseNode()
		Public Overridable Sub testBasicIrisWithElementWiseNode()

			Dim ops() As ElementWiseVertex.Op = {ElementWiseVertex.Op.Add, ElementWiseVertex.Op.Subtract, ElementWiseVertex.Op.Product, ElementWiseVertex.Op.Average, ElementWiseVertex.Op.Max}

			For Each op As ElementWiseVertex.Op In ops

				Nd4j.Random.setSeed(12345)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").addLayer("l1", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build(), "input").addLayer("l2", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.SIGMOID).build(), "input").addVertex("elementwise", New ElementWiseVertex(op), "l1", "l2").addLayer("outputLayer", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(5).nOut(3).build(), "elementwise").setOutputs("outputLayer").build()

				Dim graph As New ComputationGraph(conf)
				graph.init()

				Dim numParams As Integer = (4 * 5 + 5) + (4 * 5 + 5) + (5 * 3 + 3)
				assertEquals(numParams, graph.numParams())

				Nd4j.Random.setSeed(12345)
				Dim nParams As Long = graph.numParams()
				Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
				graph.Params = newParams

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()
				Dim min As INDArray = ds.Features.min(0)
				Dim max As INDArray = ds.Features.max(0)
				ds.Features.subiRowVector(min).diviRowVector(max.sub(min))
				Dim input As INDArray = ds.Features
				Dim labels As INDArray = ds.Labels

				If PRINT_RESULTS Then
					Console.WriteLine("testBasicIrisWithElementWiseVertex(op=" & op & ")")
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

				Dim msg As String = "testBasicIrisWithElementWiseVertex(op=" & op & ")"
				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)
			Next op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicIrisWithElementWiseNodeInputSizeGreaterThanTwo()
		Public Overridable Sub testBasicIrisWithElementWiseNodeInputSizeGreaterThanTwo()

			Dim ops() As ElementWiseVertex.Op = {ElementWiseVertex.Op.Add, ElementWiseVertex.Op.Product, ElementWiseVertex.Op.Average, ElementWiseVertex.Op.Max}

			For Each op As ElementWiseVertex.Op In ops

				Nd4j.Random.setSeed(12345)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").addLayer("l1", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build(), "input").addLayer("l2", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.SIGMOID).build(), "input").addLayer("l3", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.RELU).build(), "input").addVertex("elementwise", New ElementWiseVertex(op), "l1", "l2", "l3").addLayer("outputLayer", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(5).nOut(3).build(), "elementwise").setOutputs("outputLayer").build()

				Dim graph As New ComputationGraph(conf)
				graph.init()

				Dim numParams As Integer = (4 * 5 + 5) + (4 * 5 + 5) + (4 * 5 + 5) + (5 * 3 + 3)
				assertEquals(numParams, graph.numParams())

				Nd4j.Random.setSeed(12345)
				Dim nParams As Long = graph.numParams()
				Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
				graph.Params = newParams

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()
				Dim min As INDArray = ds.Features.min(0)
				Dim max As INDArray = ds.Features.max(0)
				ds.Features.subiRowVector(min).diviRowVector(max.sub(min))
				Dim input As INDArray = ds.Features
				Dim labels As INDArray = ds.Labels

				If PRINT_RESULTS Then
					Console.WriteLine("testBasicIrisWithElementWiseVertex(op=" & op & ")")
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

				Dim msg As String = "testBasicIrisWithElementWiseVertex(op=" & op & ")"
				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)
			Next op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElementWiseVertexBroadcast()
		Public Overridable Sub testElementWiseVertexBroadcast()

			Dim ops() As ElementWiseVertex.Op = {ElementWiseVertex.Op.Add, ElementWiseVertex.Op.Average, ElementWiseVertex.Op.Subtract, ElementWiseVertex.Op.Max, ElementWiseVertex.Op.Product}

			For Each firstSmaller As Boolean In New Boolean(){False, True}
				For Each op As ElementWiseVertex.Op In ops
					Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).activation(Activation.TANH).seed(12345).graphBuilder().addInputs("in").setOutputs("out").layer("l1", (New DenseLayer.Builder()).nIn(3).nOut(If(firstSmaller, 1, 3)).build(), "in").layer("l2", (New DenseLayer.Builder()).nIn(3).nOut(If(firstSmaller, 3, 1)).build(), "in").addVertex("ew", New ElementWiseVertex(op), "l1", "l2").layer("out", (New OutputLayer.Builder()).nIn(3).nOut(2).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build(), "ew").build()

					Dim graph As New ComputationGraph(conf)
					graph.init()

					For Each mb As Integer In New Integer(){1, 5}
						Dim msg As String = (If(firstSmaller, "first smaller, ", "second smaller, ")) & "mb=" & mb & ", op=" & op

						log.info("Test: {}", msg)

						Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, mb, 3)

						Dim [out] As INDArray = graph.outputSingle([in])
						assertArrayEquals(New Long(){mb, 2}, [out].shape())

						Dim labels As INDArray = TestUtils.randomOneHot(mb, 2)

						graph.fit(New DataSet([in], labels))

						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){[in]}).labels(New INDArray(){labels}))
						assertTrue(gradOK, msg)
						TestUtils.testModelSerialization(graph)
					Next mb
				Next op
			Next firstSmaller
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnDepthMerge()
		Public Overridable Sub testCnnDepthMerge()

			For Each format As CNN2DFormat In CNN2DFormat.values()

				Dim msg As String = "testCnnDepthMerge - " & format

				Nd4j.Random.setSeed(12345)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 0.1)).updater(New NoOp()).graphBuilder().addInputs("input").addLayer("l1", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).dataFormat(format).nIn(2).nOut(2).activation(Activation.TANH).build(), "input").addLayer("l2", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).dataFormat(format).nIn(2).nOut(2).activation(Activation.TANH).build(), "input").addVertex("merge", New MergeVertex(), "l1", "l2").addLayer("outputLayer", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(5 * 5 * (2 + 2)).nOut(3).build(), "merge").setOutputs("outputLayer").setInputTypes(InputType.convolutional(6, 6, 2, format)).build()

				Dim graph As New ComputationGraph(conf)
				graph.init()

				Dim r As New Random(12345)
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE,If(format = CNN2DFormat.NCHW, New Long(){5, 2, 6, 6}, New Long()){5,6,6,2})
				Dim labels As INDArray = Nd4j.zeros(5, 3)
				For i As Integer = 0 To 4
					labels.putScalar(New Integer(){i, r.Next(3)}, 1.0)
				Next i

				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)
			Next format
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRNNWithMerging()
		Public Overridable Sub testRNNWithMerging()
			For Each format As RNNFormat In System.Enum.GetValues(GetType(RNNFormat))

				Dim msg As String = "testRNNWithMerging - " & format
				Dim timeSeriesLength As Integer = 4
				Dim batchSize As Integer = 2
				Dim inputChannels As Integer = 3
				Dim outSize As Integer = 3
				Nd4j.Random.setSeed(12345)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New UniformDistribution(0.2, 0.6)).updater(New NoOp()).graphBuilder().addInputs("input").setOutputs("out").addLayer("rnn1", (New SimpleRnn.Builder()).nOut(3).activation(Activation.TANH).build(), "input").addLayer("rnn2", (New SimpleRnn.Builder()).nOut(3).activation(Activation.TANH).build(), "rnn1").addLayer("dense1", (New DenseLayer.Builder()).nOut(3).activation(Activation.SIGMOID).build(), "rnn1").addLayer("rnn3", (New SimpleRnn.Builder()).nOut(3).activation(Activation.TANH).build(), "dense1").addVertex("merge", New MergeVertex(), "rnn2", "rnn3").addLayer("out", (New RnnOutputLayer.Builder()).nOut(outSize).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "merge").setInputTypes(InputType.recurrent(inputChannels,timeSeriesLength, format)).build()

				Dim graph As New ComputationGraph(conf)
				graph.init()
				Console.WriteLine("Configuration for " & format & " " & conf)

				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE,If(format = RNNFormat.NCW, New Long(){batchSize, inputChannels, timeSeriesLength}, New Long()){batchSize,timeSeriesLength,inputChannels})
				Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(format, batchSize, outSize, timeSeriesLength, New Random(12345))

				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)

			Next format
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMWithSubset()
		Public Overridable Sub testLSTMWithSubset()
			Nd4j.Random.setSeed(1234)
			Dim batchSize As Integer = 2
			Dim timeSeriesLength As Integer = 4
			Dim inLength As Integer = 3
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(1234).dataType(DataType.DOUBLE).weightInit(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").setOutputs("out").addLayer("lstm1", (New LSTM.Builder()).nOut(6).activation(Activation.TANH).build(), "input").addVertex("subset", New SubsetVertex(0, 2), "lstm1").addLayer("out", (New RnnOutputLayer.Builder()).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "subset").setInputTypes(InputType.recurrent(inLength,timeSeriesLength,RNNFormat.NCW)).build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim input As INDArray = Nd4j.rand(New Integer() {batchSize, inLength, timeSeriesLength})
			Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(batchSize, 2, timeSeriesLength)

			If PRINT_RESULTS Then
				Console.WriteLine("testLSTMWithSubset()")
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

			Dim msg As String = "testLSTMWithSubset()"
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMWithLastTimeStepVertex()
		Public Overridable Sub testLSTMWithLastTimeStepVertex()

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").setOutputs("out").addLayer("lstm1", (New LSTM.Builder()).nIn(3).nOut(4).activation(Activation.TANH).build(), "input").addVertex("lastTS", New LastTimeStepVertex("input"), "lstm1").addLayer("out", (New OutputLayer.Builder()).nIn(4).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "lastTS").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim r As New Random(12345)
			Dim input As INDArray = Nd4j.rand(New Integer() {2, 3, 4})
			Dim labels As INDArray = TestUtils.randomOneHot(2, 2) 'Here: labels are 2d (due to LastTimeStepVertex)

			If PRINT_RESULTS Then
				Console.WriteLine("testLSTMWithLastTimeStepVertex()")
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
			End If

			'First: test with no input mask array
			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

			Dim msg As String = "testLSTMWithLastTimeStepVertex()"
			assertTrue(gradOK, msg)

			'Second: test with input mask arrays.
			Dim inMask As INDArray = Nd4j.zeros(3, 4)
			inMask.putRow(0, Nd4j.create(New Double() {1, 1, 0, 0}))
			inMask.putRow(1, Nd4j.create(New Double() {1, 1, 1, 0}))
			inMask.putRow(2, Nd4j.create(New Double() {1, 1, 1, 1}))
			gradOK = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}).inputMask(New INDArray(){inMask}))

			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMWithDuplicateToTimeSeries()
		Public Overridable Sub testLSTMWithDuplicateToTimeSeries()

			Dim batchSize As Integer = 2
			Dim outSize As Integer = 2
			Dim timeSeriesLength As Integer = 4
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input1", "input2").setOutputs("out").addLayer("lstm1", (New LSTM.Builder()).nIn(3).nOut(3).activation(Activation.TANH).build(), "input1").addLayer("lstm2", (New LSTM.Builder()).nIn(2).nOut(4).activation(Activation.SOFTSIGN).build(), "input2").addVertex("lastTS", New LastTimeStepVertex("input2"), "lstm2").addVertex("duplicate", New DuplicateToTimeSeriesVertex("input2"), "lastTS").addLayer("out", (New RnnOutputLayer.Builder()).nIn(3+4).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "lstm1", "duplicate").setInputTypes(InputType.recurrent(3,timeSeriesLength,RNNFormat.NCW),InputType.recurrent(2,timeSeriesLength,RNNFormat.NCW)).build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim r As New Random(12345)
			Dim input1 As INDArray = Nd4j.rand(New Integer() {batchSize, 3, 4})
			Dim input2 As INDArray = Nd4j.rand(New Integer() {batchSize, 2, 4})
			Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(batchSize, outSize, timeSeriesLength)

			If PRINT_RESULTS Then
				Console.WriteLine("testLSTMWithDuplicateToTimeSeries()")
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input1, input2}).labels(New INDArray(){labels}))

			Dim msg As String = "testLSTMWithDuplicateToTimeSeries()"
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMWithReverseTimeSeriesVertex()
		Public Overridable Sub testLSTMWithReverseTimeSeriesVertex()
			Dim timeSeriesLength As Integer = 4
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").setOutputs("out").addLayer("lstm_a", (New LSTM.Builder()).nIn(2).nOut(3).activation(Activation.TANH).build(), "input").addVertex("input_rev", New ReverseTimeSeriesVertex("input"), "input").addLayer("lstm_b", (New LSTM.Builder()).nIn(2).nOut(3).activation(Activation.TANH).build(), "input_rev").addVertex("lstm_b_rev", New ReverseTimeSeriesVertex("input"), "lstm_b").addLayer("out", (New RnnOutputLayer.Builder()).nIn(3 + 3).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "lstm_a", "lstm_b_rev").setInputTypes(InputType.recurrent(2,timeSeriesLength,RNNFormat.NCW)).build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim r As New Random(12345)
			Dim input As INDArray = Nd4j.rand(New Integer() {2, 2, 4})
			Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(2, 2, 4)

			If PRINT_RESULTS Then
				Console.WriteLine("testLSTMWithReverseTimeSeriesVertex()")
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

			Dim msg As String = "testLSTMWithDuplicateToTimeSeries()"
			assertTrue(gradOK, msg)

			'Second: test with input mask arrays.
			Dim inMask As INDArray = Nd4j.zeros(3, 5)
			inMask.putRow(0, Nd4j.create(New Double() {1, 1, 1, 0, 0}))
			inMask.putRow(1, Nd4j.create(New Double() {1, 1, 0, 1, 0}))
			inMask.putRow(2, Nd4j.create(New Double() {1, 1, 1, 1, 1}))
			graph.setLayerMaskArrays(New INDArray() {inMask}, Nothing)
			gradOK = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultipleInputsLayer()
		Public Overridable Sub testMultipleInputsLayer()

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(Activation.TANH).graphBuilder().addInputs("i0", "i1", "i2").addLayer("d0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "i0").addLayer("d1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "i1").addLayer("d2", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "i2").addLayer("d3", (New DenseLayer.Builder()).nIn(6).nOut(2).build(), "d0", "d1", "d2").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(2).nOut(2).build(), "d3").setOutputs("out").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim minibatchSizes() As Integer = {1, 3}
			For Each mb As Integer In minibatchSizes
				Dim inputs(2) As INDArray
				For i As Integer = 0 To 2
					inputs(i) = Nd4j.rand(mb, 2)
				Next i
				Dim [out] As INDArray = Nd4j.rand(mb, 2)


				Dim msg As String = "testMultipleInputsLayer() - minibatchSize = " & mb
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(inputs).labels(New INDArray(){[out]}))

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)
			Next mb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultipleOutputsLayer()
		Public Overridable Sub testMultipleOutputsLayer()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(Activation.TANH).graphBuilder().addInputs("i0").addLayer("d0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "i0").addLayer("d1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "d0").addLayer("d2", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "d0").addLayer("d3", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "d0").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(6).nOut(2).build(), "d1", "d2", "d3").setOutputs("out").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim minibatchSizes() As Integer = {1, 3}
			For Each mb As Integer In minibatchSizes
				Dim input As INDArray = Nd4j.rand(mb, 2)
				Dim [out] As INDArray = Nd4j.rand(mb, 2)


				Dim msg As String = "testMultipleOutputsLayer() - minibatchSize = " & mb
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){[out]}))

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)
			Next mb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultipleOutputsMergeVertex()
		Public Overridable Sub testMultipleOutputsMergeVertex()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(Activation.TANH).graphBuilder().addInputs("i0", "i1", "i2").addLayer("d0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "i0").addLayer("d1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "i1").addLayer("d2", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "i2").addVertex("m", New MergeVertex(), "d0", "d1", "d2").addLayer("D0", (New DenseLayer.Builder()).nIn(6).nOut(2).build(), "m").addLayer("D1", (New DenseLayer.Builder()).nIn(6).nOut(2).build(), "m").addLayer("D2", (New DenseLayer.Builder()).nIn(6).nOut(2).build(), "m").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(6).nOut(2).build(), "D0", "D1", "D2").setOutputs("out").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim minibatchSizes() As Integer = {1, 3}
			For Each mb As Integer In minibatchSizes
				Dim input(2) As INDArray
				For i As Integer = 0 To 2
					input(i) = Nd4j.rand(mb, 2)
				Next i
				Dim [out] As INDArray = Nd4j.rand(mb, 2)


				Dim msg As String = "testMultipleOutputsMergeVertex() - minibatchSize = " & mb
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(input).labels(New INDArray(){[out]}))

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)
			Next mb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultipleOutputsMergeCnn()
		Public Overridable Sub testMultipleOutputsMergeCnn()
			Dim inH As Integer = 7
			Dim inW As Integer = 7

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(Activation.TANH).graphBuilder().addInputs("input").addLayer("l0", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nIn(2).nOut(2).activation(Activation.TANH).build(), "input").addLayer("l1", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nIn(2).nOut(2).activation(Activation.TANH).build(), "l0").addLayer("l2", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nIn(2).nOut(2).activation(Activation.TANH).build(), "l0").addVertex("m", New MergeVertex(), "l1", "l2").addLayer("l3", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nIn(4).nOut(2).activation(Activation.TANH).build(), "m").addLayer("l4", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nIn(4).nOut(2).activation(Activation.TANH).build(), "m").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.IDENTITY).nOut(2).build(), "l3", "l4").setOutputs("out").setInputTypes(InputType.convolutional(inH, inW, 2)).build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim minibatchSizes() As Integer = {1, 3}
			For Each mb As Integer In minibatchSizes
				Dim input As INDArray = Nd4j.rand(New Integer() {mb, 2, inH, inW}).muli(4) 'Order: examples, channels, height, width
				Dim [out] As INDArray = Nd4j.rand(mb, 2)

				Dim msg As String = "testMultipleOutputsMergeVertex() - minibatchSize = " & mb
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){input}).labels(New INDArray(){[out]}))

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(graph)
			Next mb
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicIrisTripletStackingL2Loss()
		Public Overridable Sub testBasicIrisTripletStackingL2Loss()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input1", "input2", "input3").addVertex("stack1", New StackVertex(), "input1", "input2", "input3").addLayer("l1", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build(), "stack1").addVertex("unstack0", New UnstackVertex(0, 3), "l1").addVertex("unstack1", New UnstackVertex(1, 3), "l1").addVertex("unstack2", New UnstackVertex(2, 3), "l1").addVertex("l2-1", New L2Vertex(), "unstack1", "unstack0").addVertex("l2-2", New L2Vertex(), "unstack1", "unstack2").addLayer("lossLayer", (New LossLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build(), "l2-1", "l2-2").setOutputs("lossLayer").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim numParams As Integer = (4 * 5 + 5)
			assertEquals(numParams, graph.numParams())

			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

			Dim pos As INDArray = Nd4j.rand(150, 4)
			Dim anc As INDArray = Nd4j.rand(150, 4)
			Dim neg As INDArray = Nd4j.rand(150, 4)

			Dim labels As INDArray = Nd4j.zeros(150, 2)
			Dim r As New Random(12345)
			For i As Integer = 0 To 149
				labels.putScalar(i, r.Next(2), 1.0)
			Next i


			Dim [out] As IDictionary(Of String, INDArray) = graph.feedForward(New INDArray() {pos, anc, neg}, True)

	'        for (String s : out.keySet()) {
	'            System.out.println(s + "\t" + Arrays.toString(out.get(s).shape()));
	'        }

			If PRINT_RESULTS Then
				Console.WriteLine("testBasicIrisTripletStackingL2Loss()")
	'            for (int j = 0; j < graph.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){pos, anc, neg}).labels(New INDArray(){labels}))

			Dim msg As String = "testBasicIrisTripletStackingL2Loss()"
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(graph)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicCenterLoss()
		Public Overridable Sub testBasicCenterLoss()
			Nd4j.Random.setSeed(12345)
			Dim numLabels As Integer = 2

			Dim trainFirst() As Boolean = {False, True}

			For Each train As Boolean In trainFirst
				For Each lambda As Double In New Double() {0.0, 0.5, 2.0}

					Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New GaussianDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input1").addLayer("l1", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build(), "input1").addLayer("cl", (New CenterLossOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(5).nOut(numLabels).alpha(1.0).lambda(lambda).gradientCheck(True).activation(Activation.SOFTMAX).build(), "l1").setOutputs("cl").build()

					Dim graph As New ComputationGraph(conf)
					graph.init()

					Dim example As INDArray = Nd4j.rand(150, 4)

					Dim labels As INDArray = Nd4j.zeros(150, numLabels)
					Dim r As New Random(12345)
					For i As Integer = 0 To 149
						labels.putScalar(i, r.Next(numLabels), 1.0)
					Next i

					If train Then
						For i As Integer = 0 To 9
							Dim f As INDArray = Nd4j.rand(10, 4)
							Dim l As INDArray = Nd4j.zeros(10, numLabels)
							For j As Integer = 0 To 9
								l.putScalar(j, r.Next(numLabels), 1.0)
							Next j
							graph.fit(New INDArray() {f}, New INDArray() {l})
						Next i
					End If

					Dim msg As String = "testBasicCenterLoss() - lambda = " & lambda & ", trainFirst = " & train
					If PRINT_RESULTS Then
						Console.WriteLine(msg)
	'                    for (int j = 0; j < graph.getNumLayers(); j++)
	'                        System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){example}).labels(New INDArray(){labels}))

					assertTrue(gradOK, msg)
					TestUtils.testModelSerialization(graph)
				Next lambda
			Next train
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnPoolCenterLoss()
		Public Overridable Sub testCnnPoolCenterLoss()
			Nd4j.Random.setSeed(12345)
			Dim numLabels As Integer = 2

			Dim trainFirst() As Boolean = {False, True}

			Dim inputH As Integer = 5
			Dim inputW As Integer = 4
			Dim inputDepth As Integer = 3

			For Each train As Boolean In trainFirst
				For Each lambda As Double In New Double() {0.0, 0.5, 2.0}

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1.0)).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(3).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build()).layer(2, (New CenterLossOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).nOut(numLabels).alpha(1.0).lambda(lambda).gradientCheck(True).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(inputH, inputW, inputDepth)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim example As INDArray = Nd4j.rand(New Integer() {150, inputDepth, inputH, inputW})

					Dim labels As INDArray = Nd4j.zeros(150, numLabels)
					Dim r As New Random(12345)
					For i As Integer = 0 To 149
						labels.putScalar(i, r.Next(numLabels), 1.0)
					Next i

					If train Then
						For i As Integer = 0 To 9
							Dim f As INDArray = Nd4j.rand(New Integer() {10, inputDepth, inputH, inputW})
							Dim l As INDArray = Nd4j.zeros(10, numLabels)
							For j As Integer = 0 To 9
								l.putScalar(j, r.Next(numLabels), 1.0)
							Next j
							net.fit(f, l)
						Next i
					End If

					Dim msg As String = "testBasicCenterLoss() - trainFirst = " & train
					If PRINT_RESULTS Then
						Console.WriteLine(msg)
	'                    for (int j = 0; j < net.getnLayers(); j++)
	'                        System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, example, labels)

					assertTrue(gradOK, msg)
					TestUtils.testModelSerialization(net)
				Next lambda
			Next train
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicL2()
		Public Overridable Sub testBasicL2()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in1", "in2").addLayer("d0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "in1").addLayer("d1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "in2").addVertex("l2", New L2Vertex(), "d0", "d1").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(1).nOut(1).activation(Activation.IDENTITY).build(), "l2").setOutputs("out").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()


			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

			Dim mbSizes() As Integer = {1, 3, 10}
			For Each minibatch As Integer In mbSizes

				Dim in1 As INDArray = Nd4j.rand(DataType.DOUBLE, minibatch, 2)
				Dim in2 As INDArray = Nd4j.rand(DataType.DOUBLE, minibatch, 2)

				Dim labels As INDArray = Nd4j.rand(DataType.DOUBLE, minibatch, 1)

				Dim testName As String = "testBasicL2() - minibatch = " & minibatch

				If PRINT_RESULTS Then
					Console.WriteLine(testName)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){in1, in2}).labels(New INDArray(){labels}))

				assertTrue(gradOK, testName)
				TestUtils.testModelSerialization(graph)
			Next minibatch
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicStackUnstack()
		Public Overridable Sub testBasicStackUnstack()

			Dim layerSizes As Integer = 2

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in1", "in2").addLayer("d0", (New DenseLayer.Builder()).nIn(layerSizes).nOut(layerSizes).build(), "in1").addLayer("d1", (New DenseLayer.Builder()).nIn(layerSizes).nOut(layerSizes).build(), "in2").addVertex("stack", New StackVertex(), "d0", "d1").addLayer("d2", (New DenseLayer.Builder()).nIn(layerSizes).nOut(layerSizes).build(), "stack").addVertex("u1", New UnstackVertex(0, 2), "d2").addVertex("u2", New UnstackVertex(1, 2), "d2").addLayer("out1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(layerSizes).nOut(layerSizes).activation(Activation.IDENTITY).build(), "u1").addLayer("out2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(layerSizes).nOut(2).activation(Activation.IDENTITY).build(), "u2").setOutputs("out1", "out2").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()


			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

			Dim mbSizes() As Integer = {1, 3, 10}
			For Each minibatch As Integer In mbSizes

				Dim in1 As INDArray = Nd4j.rand(minibatch, layerSizes)
				Dim in2 As INDArray = Nd4j.rand(minibatch, layerSizes)

				Dim labels1 As INDArray = Nd4j.rand(minibatch, 2)
				Dim labels2 As INDArray = Nd4j.rand(minibatch, 2)

				Dim testName As String = "testBasicStackUnstack() - minibatch = " & minibatch

				If PRINT_RESULTS Then
					Console.WriteLine(testName)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){in1, in2}).labels(New INDArray(){labels1, labels2}))

				assertTrue(gradOK, testName)
				TestUtils.testModelSerialization(graph)
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicStackUnstackDebug()
		Public Overridable Sub testBasicStackUnstackDebug()
			Nd4j.Random.setSeed(12345)

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in1", "in2").addLayer("d0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "in1").addLayer("d1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "in2").addVertex("stack", New StackVertex(), "d0", "d1").addVertex("u0", New UnstackVertex(0, 2), "stack").addVertex("u1", New UnstackVertex(1, 2), "stack").addLayer("out1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(2).nOut(2).activation(Activation.IDENTITY).build(), "u0").addLayer("out2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(2).nOut(2).activation(Activation.IDENTITY).build(), "u1").setOutputs("out1", "out2").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()


			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

			Dim mbSizes() As Integer = {1, 3, 10}
			For Each minibatch As Integer In mbSizes

				Dim in1 As INDArray = Nd4j.rand(minibatch, 2)
				Dim in2 As INDArray = Nd4j.rand(minibatch, 2)

				Dim labels1 As INDArray = Nd4j.rand(minibatch, 2)
				Dim labels2 As INDArray = Nd4j.rand(minibatch, 2)

				Dim testName As String = "testBasicStackUnstack() - minibatch = " & minibatch

				If PRINT_RESULTS Then
					Console.WriteLine(testName)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){in1, in2}).labels(New INDArray(){labels1, labels2}))

				assertTrue(gradOK, testName)
				TestUtils.testModelSerialization(graph)
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicStackUnstackVariableLengthTS()
		Public Overridable Sub testBasicStackUnstackVariableLengthTS()

			Dim layerSizes As Integer = 2

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in1", "in2").addLayer("d0", (New SimpleRnn.Builder()).nIn(layerSizes).nOut(layerSizes).build(), "in1").addLayer("d1", (New SimpleRnn.Builder()).nIn(layerSizes).nOut(layerSizes).build(), "in2").addVertex("stack", New StackVertex(), "d0", "d1").addLayer("d2", (New SimpleRnn.Builder()).nIn(layerSizes).nOut(layerSizes).build(), "stack").addVertex("u1", New UnstackVertex(0, 2), "d2").addVertex("u2", New UnstackVertex(1, 2), "d2").addLayer("p1", (New GlobalPoolingLayer.Builder(PoolingType.AVG)).build(), "u1").addLayer("p2", (New GlobalPoolingLayer.Builder(PoolingType.AVG)).build(), "u2").addLayer("out1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(layerSizes).nOut(layerSizes).activation(Activation.IDENTITY).build(), "p1").addLayer("out2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(layerSizes).nOut(2).activation(Activation.IDENTITY).build(), "p2").setOutputs("out1", "out2").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()


			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

			Dim mbSizes() As Integer = {1, 2, 3}
			For Each minibatch As Integer In mbSizes

				Dim in1 As INDArray = Nd4j.rand(New Integer() {minibatch, layerSizes, 4})
				Dim in2 As INDArray = Nd4j.rand(New Integer() {minibatch, layerSizes, 5})
				Dim inMask1 As INDArray = Nd4j.zeros(minibatch, 4)
				inMask1.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 3)).assign(1)
				Dim inMask2 As INDArray = Nd4j.zeros(minibatch, 5)
				inMask2.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 4)).assign(1)

				Dim labels1 As INDArray = Nd4j.rand(New Integer() {minibatch, 2})
				Dim labels2 As INDArray = Nd4j.rand(New Integer() {minibatch, 2})

				Dim testName As String = "testBasicStackUnstackVariableLengthTS() - minibatch = " & minibatch

				If PRINT_RESULTS Then
					Console.WriteLine(testName)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				graph.setLayerMaskArrays(New INDArray() {inMask1, inMask2}, Nothing)

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){in1, in2}).labels(New INDArray(){labels1, labels2}).inputMask(New INDArray(){inMask1, inMask2}))

				assertTrue(gradOK, testName)
				TestUtils.testModelSerialization(graph)
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicTwoOutputs()
		Public Overridable Sub testBasicTwoOutputs()
			Nd4j.Random.setSeed(12345)

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in1", "in2").addLayer("d0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "in1").addLayer("d1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "in2").addLayer("out1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(2).nOut(2).activation(Activation.IDENTITY).build(), "d0").addLayer("out2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(2).nOut(2).activation(Activation.IDENTITY).build(), "d1").setOutputs("out1", "out2").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Console.WriteLine("Num layers: " & graph.NumLayers)
			Console.WriteLine("Num params: " & graph.numParams())


			Nd4j.Random.setSeed(12345)
			Dim nParams As Long = graph.numParams()
			Dim newParams As INDArray = Nd4j.rand(New Long(){1, nParams})
			graph.Params = newParams

			Dim mbSizes() As Integer = {1, 3, 10}
			For Each minibatch As Integer In mbSizes

				Dim in1 As INDArray = Nd4j.rand(minibatch, 2)
				Dim in2 As INDArray = Nd4j.rand(minibatch, 2)
				Dim labels1 As INDArray = Nd4j.rand(minibatch, 2)
				Dim labels2 As INDArray = Nd4j.rand(minibatch, 2)

				Dim testName As String = "testBasicStackUnstack() - minibatch = " & minibatch

				If PRINT_RESULTS Then
					Console.WriteLine(testName)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){in1, in2}).labels(New INDArray(){labels1, labels2}))
				assertTrue(gradOK, testName)
				TestUtils.testModelSerialization(graph)
			Next minibatch
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testL2NormalizeVertex2d()
		Public Overridable Sub testL2NormalizeVertex2d()
			Nd4j.Random.setSeed(12345)
			Dim definitions()() As Integer = {
				Nothing, New Integer(){1}
			}
			For Each definition As Integer() In definitions
				log.info("Testing definition {}",definition)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in1").addLayer("d1", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "in1").addVertex("norm", New L2NormalizeVertex(definition,L2NormalizeVertex.DEFAULT_EPS), "d1").addLayer("out1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nIn(3).nOut(2).activation(Activation.IDENTITY).build(), "norm").setOutputs("out1").build()

				Dim graph As New ComputationGraph(conf)
				graph.init()

				Dim mbSizes() As Integer = {1, 3, 10}
				For Each minibatch As Integer In mbSizes

					Dim in1 As INDArray = Nd4j.rand(minibatch, 2)

					Dim labels1 As INDArray = Nd4j.rand(minibatch, 2)

					Dim testName As String = "testL2NormalizeVertex2d() - minibatch = " & minibatch

					If PRINT_RESULTS Then
						Console.WriteLine(testName)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){in1}).labels(New INDArray(){labels1}))

					assertTrue(gradOK, testName)
					TestUtils.testModelSerialization(graph)
				Next minibatch
			Next definition

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testL2NormalizeVertex4d()
		Public Overridable Sub testL2NormalizeVertex4d()
			Nd4j.Random.setSeed(12345)

			Dim h As Integer = 4
			Dim w As Integer = 4
			Dim dIn As Integer = 2

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in1").addLayer("d1", (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(2).build(), "in1").addVertex("norm", New L2NormalizeVertex(), "d1").addLayer("out1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.L2).nOut(2).activation(Activation.IDENTITY).build(), "norm").setOutputs("out1").setInputTypes(InputType.convolutional(h, w, dIn)).build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim mbSizes() As Integer = {1, 3, 10}
			For Each minibatch As Integer In mbSizes

				Dim in1 As INDArray = Nd4j.rand(New Integer() {minibatch, dIn, h, w})

				Dim labels1 As INDArray = Nd4j.rand(minibatch, 2)

				Dim testName As String = "testL2NormalizeVertex4d() - minibatch = " & minibatch

				If PRINT_RESULTS Then
					Console.WriteLine(testName)
	'                for (int j = 0; j < graph.getNumLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + graph.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){in1}).labels(New INDArray(){labels1}))

				assertTrue(gradOK, testName)
				TestUtils.testModelSerialization(graph)
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphEmbeddingLayerSimple()
		Public Overridable Sub testGraphEmbeddingLayerSimple()
			Dim r As New Random(12345)
			Dim nExamples As Integer = 5
			Dim input As INDArray = Nd4j.zeros(nExamples, 1)
			Dim labels As INDArray = Nd4j.zeros(nExamples, 3)
			For i As Integer = 0 To nExamples - 1
				input.putScalar(i, r.Next(4))
				labels.putScalar(New Integer() {i, r.Next(3)}, 1.0)
			Next i

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.2).l1(0.1).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345L).updater(New NoOp()).graphBuilder().addInputs("in").addLayer("0", (New EmbeddingLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			If PRINT_RESULTS Then
				Console.WriteLine("testGraphEmbeddingLayerSimple")
	'            for (int j = 0; j < cg.getNumLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + cg.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(cg).inputs(New INDArray(){input}).labels(New INDArray(){labels}))

			Dim msg As String = "testGraphEmbeddingLayerSimple"
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(cg)
		End Sub
	End Class

End Namespace