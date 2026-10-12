Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports RecordReaderMultiDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderMultiDataSetIterator
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports GradientCheckUtil = org.deeplearning4j.gradientcheck.GradientCheckUtil
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports GaussianNoise = org.deeplearning4j.nn.conf.dropout.GaussianNoise
Imports IDropout = org.deeplearning4j.nn.conf.dropout.IDropout
Imports org.deeplearning4j.nn.conf.graph
Imports DuplicateToTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.DuplicateToTimeSeriesVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports RepeatVector = org.deeplearning4j.nn.conf.layers.misc.RepeatVector
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports org.deeplearning4j.nn.conf.preprocessor
Imports DropConnect = org.deeplearning4j.nn.conf.weightnoise.DropConnect
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports GraphIndices = org.deeplearning4j.nn.graph.util.GraphIndices
Imports PermutePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.PermutePreprocessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports MultiLayerTest = org.deeplearning4j.nn.multilayer.MultiLayerTest
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports Resources = org.nd4j.common.resources.Resources
import static org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional.Mode.CONCAT
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

Namespace org.deeplearning4j.nn.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestComputationGraphNetwork extends org.deeplearning4j.BaseDL4JTest
	Public Class TestComputationGraphNetwork
		Inherits BaseDL4JTest

		Private Shared ReadOnly Property IrisGraphConfiguration As ComputationGraphConfiguration
			Get
				Return (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input").addLayer("firstLayer", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "input").addLayer("outputLayer", (New OutputLayer.Builder()).nIn(5).nOut(3).activation(Activation.SOFTMAX).build(), "firstLayer").setOutputs("outputLayer").build()
			End Get
		End Property

		Private Shared ReadOnly Property IrisMLNConfiguration As MultiLayerConfiguration
			Get
				Return (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nIn(5).nOut(3).activation(Activation.SOFTMAX).build()).build()
			End Get
		End Property

		Private Shared ReadOnly Property NumParams As Integer
			Get
				'Number of parameters for both iris models
				Return (4 * 5 + 5) + (5 * 3 + 3)
			End Get
		End Property

		Private Shared origMode As OpExecutioner.ProfilingMode

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void beforeClass()
	 Public Shared Sub beforeClass()
			origMode = Nd4j.Executioner.ProfilingMode
	 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterAll public static void afterClass()
		Public Shared Sub afterClass()
			Nd4j.Executioner.ProfilingMode = origMode
		End Sub

		''' <summary>
		''' Override this to set the datatype of the tests defined in the child class
		''' </summary>
		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFeedForwardToLayer()
		Public Overridable Sub testFeedForwardToLayer()

			Dim configuration As ComputationGraphConfiguration = IrisGraphConfiguration

			Dim graph As New ComputationGraph(configuration)
			graph.init()

			Dim mlc As MultiLayerConfiguration = IrisMLNConfiguration
			Dim net As New MultiLayerNetwork(mlc)
			net.init()


			Dim iris As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iris.next()

			graph.setInput(0, ds.Features)
			net.Params = graph.params()
			Dim activations As IDictionary(Of String, INDArray) = graph.feedForward(False)

			Dim feedForward As IList(Of INDArray) = net.feedForward(ds.Features)
			assertEquals(activations.Count,feedForward.Count)
			assertEquals(activations("outputLayer"),feedForward(feedForward.Count - 1))

			Dim graphForward As IDictionary(Of String, INDArray) = graph.feedForward(ds.Features,0,False)
			Dim networkForward As IList(Of INDArray) = net.feedForwardToLayer(0,ds.Features,False)
			assertEquals(graphForward("firstLayer"),networkForward(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConfigurationBasic()
		Public Overridable Sub testConfigurationBasic()

			Dim configuration As ComputationGraphConfiguration = IrisGraphConfiguration

			Dim graph As New ComputationGraph(configuration)
			graph.init()

			'Get topological sort order
			Dim order() As Integer = graph.topologicalSortOrder()
			Dim expOrder() As Integer = {0, 1, 2}
			assertArrayEquals(expOrder, order) 'Only one valid order: 0 (input) -> 1 (firstlayer) -> 2 (outputlayer)

			Dim params As INDArray = graph.params()
			assertNotNull(params)

			Dim nParams As Integer = NumParams
			assertEquals(nParams, params.length())

			Dim arr As INDArray = Nd4j.linspace(0, nParams, nParams).reshape(ChrW(1), nParams)
			assertEquals(nParams, arr.length())

			graph.Params = arr
			params = graph.params()
			assertEquals(arr, params)

			'Number of inputs and outputs:
			assertEquals(1, graph.NumInputArrays)
			assertEquals(1, graph.NumOutputArrays)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testForwardBasicIris()
		Public Overridable Sub testForwardBasicIris()

			Dim configuration As ComputationGraphConfiguration = IrisGraphConfiguration
			Dim graph As New ComputationGraph(configuration)
			graph.init()

			Dim mlc As MultiLayerConfiguration = IrisMLNConfiguration
			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			Dim iris As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iris.next()

			graph.setInput(0, ds.Features)
			Dim activations As IDictionary(Of String, INDArray) = graph.feedForward(False)
			assertEquals(3, activations.Count) '2 layers + 1 input node
			assertTrue(activations.ContainsKey("input"))
			assertTrue(activations.ContainsKey("firstLayer"))
			assertTrue(activations.ContainsKey("outputLayer"))

			'Now: set parameters of both networks to be identical. Then feedforward, and check we get the same outputs
			Nd4j.Random.setSeed(12345)
			Dim nParams As Integer = NumParams
			Dim params As INDArray = Nd4j.rand(1, nParams)
			graph.Params = params.dup()
			net.Params = params.dup()

			Dim mlnAct As IList(Of INDArray) = net.feedForward(ds.Features, False)
			activations = graph.feedForward(ds.Features, False)

			assertEquals(mlnAct(0), activations("input"))
			assertEquals(mlnAct(1), activations("firstLayer"))
			assertEquals(mlnAct(2), activations("outputLayer"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBackwardIrisBasic()
		Public Overridable Sub testBackwardIrisBasic()
			Dim configuration As ComputationGraphConfiguration = IrisGraphConfiguration
			Dim graph As New ComputationGraph(configuration)
			graph.init()

			Dim mlc As MultiLayerConfiguration = IrisMLNConfiguration
			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			Dim iris As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iris.next()

			'Now: set parameters of both networks to be identical. Then feedforward, and check we get the same outputs
			Nd4j.Random.setSeed(12345)
			Dim nParams As Integer = (4 * 5 + 5) + (5 * 3 + 3)
			Dim params As INDArray = Nd4j.rand(1, nParams)
			graph.Params = params.dup()
			net.Params = params.dup()

			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels
			graph.setInput(0, input.dup())
			graph.setLabel(0, labels.dup())

			net.Input = input.dup()
			net.Labels = labels.dup()

			'Compute gradients
			net.computeGradientAndScore()
			Dim netGradScore As Pair(Of Gradient, Double) = net.gradientAndScore()

			graph.computeGradientAndScore()
			Dim graphGradScore As Pair(Of Gradient, Double) = graph.gradientAndScore()

			assertEquals(netGradScore.Second, graphGradScore.Second, 1e-3)

			'Compare gradients
			Dim netGrad As Gradient = netGradScore.First
			Dim graphGrad As Gradient = graphGradScore.First

			assertNotNull(graphGrad)
			assertEquals(netGrad.gradientForVariable().Count, graphGrad.gradientForVariable().Count)

			assertEquals(netGrad.getGradientFor("0_W"), graphGrad.getGradientFor("firstLayer_W"))
			assertEquals(netGrad.getGradientFor("0_b"), graphGrad.getGradientFor("firstLayer_b"))
			assertEquals(netGrad.getGradientFor("1_W"), graphGrad.getGradientFor("outputLayer_W"))
			assertEquals(netGrad.getGradientFor("1_b"), graphGrad.getGradientFor("outputLayer_b"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testIrisFit()
		Public Overridable Sub testIrisFit()

			Dim configuration As ComputationGraphConfiguration = IrisGraphConfiguration
			Dim graph As New ComputationGraph(configuration)
			graph.init()

			Dim mlnConfig As MultiLayerConfiguration = IrisMLNConfiguration
			Dim net As New MultiLayerNetwork(mlnConfig)
			net.init()

			Nd4j.Random.setSeed(12345)
			Dim nParams As Integer = NumParams
			Dim params As INDArray = Nd4j.rand(1, nParams)

			graph.Params = params.dup()
			net.Params = params.dup()


			Dim iris As DataSetIterator = New IrisDataSetIterator(75, 150)

			net.fit(iris)
			iris.reset()

			graph.fit(iris)

			'Check that parameters are equal for both models after fitting:
			Dim paramsMLN As INDArray = net.params()
			Dim paramsGraph As INDArray = graph.params()

			assertNotEquals(params, paramsGraph)
			assertEquals(paramsMLN, paramsGraph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testIrisFitMultiDataSetIterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIrisFitMultiDataSetIterator()

			Dim rr As RecordReader = New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))

			Dim iter As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(10)).addReader("iris", rr).addInput("iris", 0, 3).addOutputOneHot("iris", 4, 3).build()

			Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).graphBuilder().addInputs("in").addLayer("dense", (New DenseLayer.Builder()).nIn(4).nOut(2).build(), "in").addLayer("out", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(2).nOut(3).build(), "dense").setOutputs("out").build()

			Dim cg As New ComputationGraph(config)
			cg.init()

			cg.fit(iter)


			rr.reset()
			iter = (New RecordReaderMultiDataSetIterator.Builder(10)).addReader("iris", rr).addInput("iris", 0, 3).addOutputOneHot("iris", 4, 3).build()
			Do While iter.MoveNext()
				cg.fit(iter.Current)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCloning()
		Public Overridable Sub testCloning()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = IrisGraphConfiguration
			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim g2 As ComputationGraph = graph.clone()

			Dim iris As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [in] As INDArray = iris.next().getFeatures()
			Dim activations As IDictionary(Of String, INDArray) = graph.feedForward([in], False)
			Dim activations2 As IDictionary(Of String, INDArray) = g2.feedForward([in], False)
			assertEquals(activations, activations2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScoringDataSet()
		Public Overridable Sub testScoringDataSet()
			Dim configuration As ComputationGraphConfiguration = IrisGraphConfiguration
			Dim graph As New ComputationGraph(configuration)
			graph.init()

			Dim mlc As MultiLayerConfiguration = IrisMLNConfiguration
			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			Dim iris As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iris.next()

			'Now: set parameters of both networks to be identical. Then feedforward, and check we get the same score
			Nd4j.Random.setSeed(12345)
			Dim nParams As Integer = NumParams
			Dim params As INDArray = Nd4j.rand(1, nParams)
			graph.Params = params.dup()
			net.Params = params.dup()

			Dim scoreMLN As Double = net.score(ds, False)
			Dim scoreCG As Double = graph.score(ds, False)

			assertEquals(scoreMLN, scoreCG, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreprocessorAddition()
		Public Overridable Sub testPreprocessorAddition()
			'Also check that nIns are set automatically
			'First: check FF -> RNN
			Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").setInputTypes(InputType.feedForward(5)).addLayer("rnn", (New GravesLSTM.Builder()).nOut(5).build(), "in").addLayer("out", (New RnnOutputLayer.Builder()).nOut(5).activation(Activation.SOFTMAX).build(), "rnn").setOutputs("out").build()

			assertEquals(5, CType(CType(conf1.getVertices().get("rnn"), LayerVertex).getLayerConf().getLayer(), FeedForwardLayer).getNIn())
			assertEquals(5, CType(CType(conf1.getVertices().get("out"), LayerVertex).getLayerConf().getLayer(), FeedForwardLayer).getNIn())

			Dim lv1 As LayerVertex = CType(conf1.getVertices().get("rnn"), LayerVertex)
			assertTrue(TypeOf lv1.PreProcessor Is FeedForwardToRnnPreProcessor)
			Dim lv2 As LayerVertex = CType(conf1.getVertices().get("out"), LayerVertex)
			assertNull(lv2.PreProcessor)

			'Check RNN -> FF -> RNN
			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").setInputTypes(InputType.recurrent(5)).addLayer("ff", (New DenseLayer.Builder()).nOut(5).build(), "in").addLayer("out", (New RnnOutputLayer.Builder()).nOut(5).activation(Activation.SOFTMAX).build(), "ff").setOutputs("out").build()

			assertEquals(5, CType(CType(conf2.getVertices().get("ff"), LayerVertex).getLayerConf().getLayer(), FeedForwardLayer).getNIn())
			assertEquals(5, CType(CType(conf2.getVertices().get("out"), LayerVertex).getLayerConf().getLayer(), FeedForwardLayer).getNIn())

			lv1 = CType(conf2.getVertices().get("ff"), LayerVertex)
			assertTrue(TypeOf lv1.PreProcessor Is RnnToFeedForwardPreProcessor)
			lv2 = CType(conf2.getVertices().get("out"), LayerVertex)
			assertTrue(TypeOf lv2.PreProcessor Is FeedForwardToRnnPreProcessor)

			'CNN -> Dense
			Dim conf3 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").setInputTypes(InputType.convolutional(28, 28, 1)).addLayer("cnn", (New ConvolutionLayer.Builder()).kernelSize(2, 2).padding(0, 0).stride(2, 2).nOut(3).build(), "in").addLayer("pool", (New SubsamplingLayer.Builder()).kernelSize(2, 2).padding(0, 0).stride(2, 2).build(), "cnn").addLayer("dense", (New DenseLayer.Builder()).nOut(10).build(), "pool").addLayer("out", (New OutputLayer.Builder()).nIn(10).nOut(5).activation(Activation.SOFTMAX).build(), "dense").setOutputs("out").build()
			'Check preprocessors:
			lv1 = CType(conf3.getVertices().get("cnn"), LayerVertex)
			assertNull(lv1.PreProcessor) 'Shouldn't be adding preprocessor here

			lv2 = CType(conf3.getVertices().get("pool"), LayerVertex)
			assertNull(lv2.PreProcessor)
			Dim lv3 As LayerVertex = CType(conf3.getVertices().get("dense"), LayerVertex)
			assertTrue(TypeOf lv3.PreProcessor Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(lv3.PreProcessor, CnnToFeedForwardPreProcessor)
			assertEquals(3, proc.getNumChannels())
			assertEquals(7, proc.getInputHeight())
			assertEquals(7, proc.getInputWidth())
			Dim lv4 As LayerVertex = CType(conf3.getVertices().get("out"), LayerVertex)
			assertNull(lv4.PreProcessor)
			'Check nIns:
			assertEquals(7 * 7 * 3, CType(lv3.getLayerConf().getLayer(), FeedForwardLayer).getNIn())

			'CNN->Dense, RNN->Dense, Dense->RNN
			Dim conf4 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("inCNN", "inRNN").setInputTypes(InputType.convolutional(28, 28, 1), InputType.recurrent(5)).addLayer("cnn", (New ConvolutionLayer.Builder()).kernelSize(2, 2).padding(0, 0).stride(2, 2).nOut(3).build(), "inCNN").addLayer("pool", (New SubsamplingLayer.Builder()).kernelSize(2, 2).padding(0, 0).stride(2, 2).build(), "cnn").addLayer("dense", (New DenseLayer.Builder()).nOut(10).build(), "pool").addLayer("dense2", (New DenseLayer.Builder()).nOut(10).build(), "inRNN").addVertex("merge", New MergeVertex(), "dense", "dense2").addLayer("out", (New RnnOutputLayer.Builder()).nOut(5).activation(Activation.SOFTMAX).build(), "merge").setOutputs("out").build()

			'Check preprocessors:
			lv1 = CType(conf4.getVertices().get("cnn"), LayerVertex)
			assertNull(lv1.PreProcessor) 'Expect no preprocessor: cnn data -> cnn layer

			lv2 = CType(conf4.getVertices().get("pool"), LayerVertex)
			assertNull(lv2.PreProcessor)
			lv3 = CType(conf4.getVertices().get("dense"), LayerVertex)
			assertTrue(TypeOf lv3.PreProcessor Is CnnToFeedForwardPreProcessor)
			proc = DirectCast(lv3.PreProcessor, CnnToFeedForwardPreProcessor)
			assertEquals(3, proc.getNumChannels())
			assertEquals(7, proc.getInputHeight())
			assertEquals(7, proc.getInputWidth())
			lv4 = CType(conf4.getVertices().get("dense2"), LayerVertex)
			assertTrue(TypeOf lv4.PreProcessor Is RnnToFeedForwardPreProcessor)
			Dim lv5 As LayerVertex = CType(conf4.getVertices().get("out"), LayerVertex)
			assertTrue(TypeOf lv5.PreProcessor Is FeedForwardToRnnPreProcessor)
			'Check nIns:
			assertEquals(7 * 7 * 3, CType(lv3.getLayerConf().getLayer(), FeedForwardLayer).getNIn())
			assertEquals(5, CType(lv4.getLayerConf().getLayer(), FeedForwardLayer).getNIn())
			assertEquals(20, CType(lv5.getLayerConf().getLayer(), FeedForwardLayer).getNIn()) '10+10 out of the merge vertex -> 20 in to output layer vertex


			'Input to 2 CNN layers:
			Dim conf5 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input").setInputTypes(InputType.convolutional(28, 28, 1)).addLayer("cnn_1", (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).nIn(1).nOut(3).build(), "input").addLayer("cnn_2", (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).padding(1, 1).nIn(1).nOut(3).build(), "input").addLayer("max_1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).build(), "cnn_1", "cnn_2").addLayer("output", (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build(), "max_1").setOutputs("output").build()
			lv1 = CType(conf5.getVertices().get("cnn_1"), LayerVertex)
			assertNull(lv1.PreProcessor) 'Expect no preprocessor: cnn data -> cnn layer

			lv2 = CType(conf5.getVertices().get("cnn_2"), LayerVertex)
			assertNull(lv2.PreProcessor) 'Expect no preprocessor: cnn data -> cnn layer

			assertNull(CType(conf5.getVertices().get("max_1"), LayerVertex).PreProcessor)

			lv3 = CType(conf5.getVertices().get("output"), LayerVertex)
			assertTrue(TypeOf lv3.PreProcessor Is CnnToFeedForwardPreProcessor)
			Dim cnnff As CnnToFeedForwardPreProcessor = DirectCast(lv3.PreProcessor, CnnToFeedForwardPreProcessor)
			assertEquals(6, cnnff.getNumChannels())
			assertEquals(7, cnnff.getInputHeight())
			assertEquals(7, cnnff.getInputWidth())

			Dim graph As New ComputationGraph(conf1)
			graph.init()
	'        System.out.println(graph.summary());
	'        System.out.println(graph.summary(InputType.feedForward(5)));
			graph.summary()
			graph.summary(InputType.feedForward(5))

			graph = New ComputationGraph(conf2)
			graph.init()
	'        System.out.println(graph.summary());
	'        System.out.println(graph.summary(InputType.recurrent(5)));
			graph.summary()
			graph.summary(InputType.recurrent(5))

			graph = New ComputationGraph(conf3)
			graph.init()
	'        System.out.println(graph.summary());
	'        System.out.println(graph.summary(InputType.convolutional(28, 28, 1)));
			graph.summary()
			graph.summary(InputType.convolutional(28, 28, 1))

			graph = New ComputationGraph(conf4)
			graph.init()
	'        System.out.println(graph.summary());
	'        System.out.println(graph.summary(InputType.convolutional(28, 28, 1), InputType.recurrent(5)));
			graph.summary()
			graph.summary(InputType.convolutional(28, 28, 1), InputType.recurrent(5))

			graph = New ComputationGraph(conf5)
			graph.init()
	'        System.out.println(graph.summary());
	'        System.out.println(graph.summary(InputType.convolutional(28, 28, 1)));
			graph.summary()
			graph.summary(InputType.convolutional(28, 28, 1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompGraphUnderscores()
		Public Overridable Sub testCompGraphUnderscores()
			'Problem: underscores in names could be problematic for ComputationGraphUpdater, HistogramIterationListener

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input").addLayer("first_layer", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "input").addLayer("output_layer", (New OutputLayer.Builder()).nIn(5).nOut(3).activation(Activation.SOFTMAX).build(), "first_layer").setOutputs("output_layer").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim iris As DataSetIterator = New IrisDataSetIterator(10, 150)
			Do While iris.MoveNext()
				net.fit(iris.Current)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testPreTraining()
		Public Overridable Sub testPreTraining()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(1e-6)).l2(2e-4).graphBuilder().addInputs("in").addLayer("layer0", (New VariationalAutoencoder.Builder()).nIn(4).nOut(3).dist(New UniformDistribution(0, 1)).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.KL_DIVERGENCE).build(), "in").addLayer("layer1", (New VariationalAutoencoder.Builder()).nIn(4).nOut(3).dist(New UniformDistribution(0, 1)).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.KL_DIVERGENCE).build(), "in").addLayer("layer2", (New VariationalAutoencoder.Builder()).nIn(3).nOut(3).dist(New UniformDistribution(0, 1)).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.KL_DIVERGENCE).build(), "layer1").addLayer("out", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3 + 3).nOut(3).dist(New UniformDistribution(0, 1)).activation(Activation.SOFTMAX).build(), "layer0", "layer2").setOutputs("out").build()


			Dim net As New ComputationGraph(conf)
			net.init()
			net.setListeners(New ScoreIterationListener(1))

			Dim iter As DataSetIterator = New IrisDataSetIterator(10, 150)
			net.pretrain(iter)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScoreExamples()
		Public Overridable Sub testScoreExamples()
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).l2(0.01).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(20).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(20).nOut(30).build(), "0").addLayer("2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build(), "1").setOutputs("2").build()

			Dim confNoReg As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(20).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(20).nOut(30).build(), "0").addLayer("2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build(), "1").setOutputs("2").build()


			Dim net As New ComputationGraph(conf)
			net.init()

			Dim netNoReg As New ComputationGraph(confNoReg)
			netNoReg.init()
			netNoReg.Params = net.params().dup()

			'Score single example, and compare to scoreExamples:
			Dim input As INDArray = Nd4j.rand(3, nIn)
			Dim output As INDArray = Nd4j.rand(3, nOut)
			Dim ds As New DataSet(input, output)

			Dim scoresWithRegularization As INDArray = net.scoreExamples(ds, True)
			Dim scoresNoRegularization As INDArray = net.scoreExamples(ds, False)

			assertArrayEquals(New Long(){3, 1}, scoresWithRegularization.shape())
			assertArrayEquals(New Long(){3, 1}, scoresNoRegularization.shape())

			For i As Integer = 0 To 2
				Dim singleEx As New DataSet(input.getRow(i,True), output.getRow(i,True))
				Dim score As Double = net.score(singleEx)
				Dim scoreNoReg As Double = netNoReg.score(singleEx)

				Dim scoreUsingScoreExamples As Double = scoresWithRegularization.getDouble(i)
				Dim scoreUsingScoreExamplesNoReg As Double = scoresNoRegularization.getDouble(i)
				assertEquals(score, scoreUsingScoreExamples, 1e-4)
				assertEquals(scoreNoReg, scoreUsingScoreExamplesNoReg, 1e-4)
				assertTrue(scoreUsingScoreExamples > scoreUsingScoreExamplesNoReg) 'Regularization term increases score

				'            System.out.println(score + "\t" + scoreUsingScoreExamples + "\t|\t" + scoreNoReg + "\t" + scoreUsingScoreExamplesNoReg);
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExternalErrors()
		Public Overridable Sub testExternalErrors()
			'Simple test: same network, but in one case: one less layer (the OutputLayer), where the epsilons are passed in externally
			' instead. Should get identical results

			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("Workspace mode: " & ws)

				Nd4j.Random.setSeed(12345)
				Dim inData As INDArray = Nd4j.rand(3, 10)
				Dim outData As INDArray = Nd4j.rand(3, 10)

				Nd4j.Random.setSeed(12345)
				Dim standard As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).seed(12345).graphBuilder().addInputs("in").addLayer("l0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build(), "l0").setOutputs("out").build()
				Dim s As New ComputationGraph(standard)
				s.init()


				Nd4j.Random.setSeed(12345)
				Dim external As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).seed(12345).graphBuilder().addInputs("in").addLayer("l0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").setOutputs("l0").build()

				Dim e As New ComputationGraph(external)
				e.init()

				s.Inputs = inData
				s.Labels = outData
				s.computeGradientAndScore()
				Dim sGrad As Gradient = s.gradient()

				s.feedForward(New INDArray(){inData}, True, False) 'FF without clearing inputs as we need them later

				Dim ol As org.deeplearning4j.nn.layers.OutputLayer = DirectCast(s.getLayer(1), org.deeplearning4j.nn.layers.OutputLayer)
				ol.Labels = outData
				Dim olPairStd As Pair(Of Gradient, INDArray) = ol.backpropGradient(Nothing, LayerWorkspaceMgr.noWorkspaces())

				Dim olEpsilon As INDArray = olPairStd.Second

				e.feedForward(New INDArray(){inData}, True, False) 'FF without clearing inputs as we need them later
				Dim extErrorGrad As Gradient = e.backpropGradient(olEpsilon)

				Dim nParamsDense As Integer = 10 * 10 + 10
				assertEquals(sGrad.gradient().get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nParamsDense)), extErrorGrad.gradient())

				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExternalErrors2()
		Public Overridable Sub testExternalErrors2()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
			Dim nIn As Integer = 4
			Dim nOut As Integer = 3

			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
	'            System.out.println("***** WORKSPACE: " + ws);

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.01)).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).graphBuilder().addInputs("features").addVertex("rnn2ffn", New PreprocessorVertex(New RnnToFeedForwardPreProcessor()), "features").addLayer("predict", (New DenseLayer.Builder()).nIn(nIn).nOut(nOut).activation(Activation.RELU).build(), "rnn2ffn").addVertex("ffn2rnn", New PreprocessorVertex(New FeedForwardToRnnPreProcessor()), "predict").addLayer("output", (New ActivationLayer.Builder()).activation(Activation.IDENTITY).build(), "ffn2rnn").setOutputs("output").build()

				Dim graph As New ComputationGraph(conf)
				graph.init()

				Const minibatch As Integer = 5
				Const seqLen As Integer = 6

				Dim param As INDArray = Nd4j.create(New Double(){0.54, 0.31, 0.98, -0.30, -0.66, -0.19, -0.29, -0.62, 0.13, -0.32, 0.01, -0.03, 0.00, 0.00, 0.00})
				graph.Params = param

				Dim input As INDArray = Nd4j.rand(New Integer(){minibatch, nIn, seqLen}, 12)
				Dim expected As INDArray = Nd4j.ones(minibatch, nOut, seqLen)

				Dim output As INDArray = graph.outputSingle(False, False, input)
				Dim [error] As INDArray = output.sub(expected)

				For Each l As org.deeplearning4j.nn.api.Layer In graph.Layers
					assertNotNull(l.input())
					assertFalse(l.input().Attached)
				Next l

				' Compute Gradient
				Dim gradient As Gradient = graph.backpropGradient([error])
				graph.Updater.update(gradient, 0, 0, minibatch, LayerWorkspaceMgr.noWorkspaces())

				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Next ws

			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.DISABLED
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExternalErrorsInvalid()
		Public Overridable Sub testExternalErrorsInvalid()

			Dim nIn As Integer = 2
			Dim nOut As Integer = 4
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(4).activation(Activation.RELU).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.RELU).build(), "0").addLayer("out", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nOut(nOut).build(), "1").setOutputs("out").setInputTypes(InputType.feedForward(nIn)).build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim actionInput As INDArray = Nd4j.randn(3, nIn)
			Dim input() As INDArray = {actionInput}

			cg.Inputs = input
			cg.feedForward(input, True, False)

			Dim externalError As INDArray = Nd4j.rand(3, nIn)
			Try
				cg.backpropGradient(externalError)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("output layer"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientUpdate()
		Public Overridable Sub testGradientUpdate()
			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 1)

			Dim expectedGradient As Gradient = New DefaultGradient()
			expectedGradient.setGradientFor("first_W", Nd4j.ones(4, 5).castTo(Nd4j.defaultFloatingPointType()))
			expectedGradient.setGradientFor("first_b", Nd4j.ones(1, 5).castTo(Nd4j.defaultFloatingPointType()))
			expectedGradient.setGradientFor("output_W", Nd4j.ones(5, 3).castTo(Nd4j.defaultFloatingPointType()))
			expectedGradient.setGradientFor("output_b", Nd4j.ones(1, 3).castTo(Nd4j.defaultFloatingPointType()))

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input").addLayer("first", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "input").addLayer("output", (New OutputLayer.Builder()).nIn(5).nOut(3).activation(Activation.SOFTMAX).build(), "first").setOutputs("output").build()

			Dim net As New ComputationGraph(conf)
			net.init()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			net.fit(iter.next())
			Dim actualGradient As Gradient = net.gradient_Conflict
			assertNotEquals(expectedGradient.getGradientFor("first_W"), actualGradient.getGradientFor("first_W"))

			net.update(expectedGradient)
			actualGradient = net.gradient_Conflict
			assertEquals(expectedGradient.getGradientFor("first_W"), actualGradient.getGradientFor("first_W"))

			' Update params with set
			net.setParam("first_W", Nd4j.ones(4, 5).castTo(Nd4j.defaultFloatingPointType()))
			net.setParam("first_b", Nd4j.ones(1, 5).castTo(Nd4j.defaultFloatingPointType()))
			net.setParam("output_W", Nd4j.ones(5, 3).castTo(Nd4j.defaultFloatingPointType()))
			net.setParam("output_b", Nd4j.ones(1, 3).castTo(Nd4j.defaultFloatingPointType()))
			Dim actualParams As INDArray = net.params().castTo(Nd4j.defaultFloatingPointType())

			' Confirm params
			assertEquals(Nd4j.ones(1, 43), actualParams)

			net.update(expectedGradient)
			actualParams = net.params()
			assertEquals(Nd4j.ones(1, 43).addi(1), actualParams)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnFlatInputType1()
		Public Overridable Sub testCnnFlatInputType1()

			'First: check conv input type. Expect: no preprocessor, nIn set appropriately
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").setInputTypes(InputType.convolutional(10, 8, 3)).addLayer("layer", (New ConvolutionLayer.Builder()).kernelSize(2, 2).padding(0, 0).stride(1, 1).build(), "in").addLayer("out", (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build(), "layer").setOutputs("out").build()

			Dim lv As LayerVertex = CType(conf.getVertices().get("layer"), LayerVertex)
			Dim l As FeedForwardLayer = (CType((lv).getLayerConf().getLayer(), FeedForwardLayer))
			assertEquals(3, l.getNIn())
			assertNull(lv.PreProcessor)

			'Check the equivalent config, but with flat conv data input instead
			'In this case, the only difference should be the addition of a preprocessor
			'First: check conv input type. Expect: no preprocessor, nIn set appropriately
			conf = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").setInputTypes(InputType.convolutionalFlat(10, 8, 3)).addLayer("layer", (New ConvolutionLayer.Builder()).kernelSize(2, 2).padding(0, 0).stride(1, 1).build(), "in").addLayer("out", (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build(), "layer").setOutputs("out").build()

			lv = CType(conf.getVertices().get("layer"), LayerVertex)
			l = (CType((lv).getLayerConf().getLayer(), FeedForwardLayer))
			assertEquals(3, l.getNIn())
			assertNotNull(lv.PreProcessor)
			Dim preProcessor As InputPreProcessor = lv.PreProcessor
			assertTrue(TypeOf preProcessor Is FeedForwardToCnnPreProcessor)
			Dim preproc As FeedForwardToCnnPreProcessor = DirectCast(preProcessor, FeedForwardToCnnPreProcessor)
			assertEquals(10, preproc.getInputHeight())
			assertEquals(8, preproc.getInputWidth())
			assertEquals(3, preproc.getNumChannels())


			'Finally, check configuration with a subsampling layer
			conf = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").setInputTypes(InputType.convolutionalFlat(10, 8, 3)).addLayer("l0", (New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).build(), "in").addLayer("layer", (New ConvolutionLayer.Builder()).kernelSize(2, 2).padding(0, 0).stride(1, 1).build(), "l0").addLayer("out", (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build(), "layer").setOutputs("out").build()

			'Check subsampling layer:
			lv = CType(conf.getVertices().get("l0"), LayerVertex)
			Dim sl As SubsamplingLayer = (CType((lv).getLayerConf().getLayer(), SubsamplingLayer))
			assertNotNull(lv.PreProcessor)
			preProcessor = lv.PreProcessor
			assertTrue(TypeOf preProcessor Is FeedForwardToCnnPreProcessor)
			preproc = DirectCast(preProcessor, FeedForwardToCnnPreProcessor)
			assertEquals(10, preproc.getInputHeight())
			assertEquals(8, preproc.getInputWidth())
			assertEquals(3, preproc.getNumChannels())
			'Check dense layer
			lv = CType(conf.getVertices().get("layer"), LayerVertex)
			l = (CType((lv).getLayerConf().getLayer(), FeedForwardLayer))
			assertEquals(3, l.getNIn())
			assertNull(lv.PreProcessor)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCGEvaluation()
		Public Overridable Sub testCGEvaluation()

			Nd4j.Random.setSeed(12345)
			Dim configuration As ComputationGraphConfiguration = IrisGraphConfiguration
			Dim graph As New ComputationGraph(configuration)
			graph.init()

			Nd4j.Random.setSeed(12345)
			Dim mlnConfig As MultiLayerConfiguration = IrisMLNConfiguration
			Dim net As New MultiLayerNetwork(mlnConfig)
			net.init()

			Dim iris As DataSetIterator = New IrisDataSetIterator(75, 150)

			net.fit(iris)
			iris.reset()
			graph.fit(iris)

			iris.reset()
			Dim evalExpected As Evaluation = net.evaluate(iris)
			iris.reset()
			Dim evalActual As Evaluation = graph.evaluate(iris)

			assertEquals(evalExpected.accuracy(), evalActual.accuracy(), 0e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOptimizationAlgorithmsSearchBasic()
		Public Overridable Sub testOptimizationAlgorithmsSearchBasic()
			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 1)

			Dim oas() As OptimizationAlgorithm = {OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT, OptimizationAlgorithm.LINE_GRADIENT_DESCENT, OptimizationAlgorithm.CONJUGATE_GRADIENT, OptimizationAlgorithm.LBFGS}

			For Each oa As OptimizationAlgorithm In oas
	'            System.out.println(oa);
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(oa).graphBuilder().addInputs("input").addLayer("first", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "input").addLayer("output", (New OutputLayer.Builder()).nIn(5).nOut(3).activation(Activation.SOFTMAX).build(), "first").setOutputs("output").build()

				Dim net As New ComputationGraph(conf)
				net.init()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				net.fit(iter.next())

			Next oa
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIterationCountAndPersistence() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIterationCountAndPersistence()
			Nd4j.Random.setSeed(123)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "0").setOutputs("1").build()


			Dim network As New ComputationGraph(conf)
			network.init()

			Dim iter As DataSetIterator = New IrisDataSetIterator(50, 150)

			assertEquals(0, network.Configuration.getIterationCount())
			network.fit(iter)
			assertEquals(3, network.Configuration.getIterationCount())
			iter.reset()
			network.fit(iter)
			assertEquals(6, network.Configuration.getIterationCount())
			iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			network.fit(iter.next())
			assertEquals(7, network.Configuration.getIterationCount())

			Dim baos As New MemoryStream()
			ModelSerializer.writeModel(network, baos, True)
			Dim asBytes() As SByte = baos.toByteArray()

			Dim bais As New MemoryStream(asBytes)
			Dim net As ComputationGraph = ModelSerializer.restoreComputationGraph(bais, True)
			assertEquals(7, net.Configuration.getIterationCount())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void printSummary()
		Public Overridable Sub printSummary()
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY)

			Dim conf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("inCentre", "inRight").addLayer("denseCentre0", (New DenseLayer.Builder()).nIn(10).nOut(9).build(), "inCentre").addLayer("denseCentre1", (New DenseLayer.Builder()).nIn(9).nOut(8).build(), "denseCentre0").addLayer("denseCentre2", (New DenseLayer.Builder()).nIn(8).nOut(7).build(), "denseCentre1").addLayer("denseCentre3", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("outCentre", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(7).nOut(4).build(), "denseCentre3").addVertex("subsetLeft", New SubsetVertex(0, 3), "denseCentre1").addLayer("denseLeft0", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "subsetLeft").addLayer("outLeft", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(6).build(), "denseLeft0").addLayer("denseRight", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseRight", "denseRight0").addLayer("denseRight1", (New DenseLayer.Builder()).nIn(10).nOut(5).build(), "mergeRight").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(5).build(), "denseRight1").setOutputs("outLeft", "outCentre", "outRight").build()

			Dim modelToTune As New ComputationGraph(conf)
			modelToTune.init()
	'        System.out.println(modelToTune.summary());
			modelToTune.summary()

			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToTune)).setFeatureExtractor("denseCentre2").build()
	'        System.out.println(modelNow.summary());
	'        System.out.println(modelNow.summary(InputType.feedForward(10),InputType.feedForward(2)));
			modelNow.summary()
			modelNow.summary(InputType.feedForward(10),InputType.feedForward(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFeedForwardIncludeNonLayerVertices()
		Public Overridable Sub testFeedForwardIncludeNonLayerVertices()

			Dim c As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(5).nOut(5).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(5).nOut(5).build(), "in").addVertex("merge", New MergeVertex(), "0", "1").addLayer("out", (New OutputLayer.Builder()).nIn(10).nOut(5).activation(Activation.SOFTMAX).build(), "merge").setOutputs("out").build()

			Dim cg As New ComputationGraph(c)
			cg.init()

			cg.Inputs = Nd4j.ones(1, 5)

			Dim layersOnly As IDictionary(Of String, INDArray) = cg.feedForward(True, False, False)
			Dim alsoVertices As IDictionary(Of String, INDArray) = cg.feedForward(True, False, True)

			assertEquals(4, layersOnly.Count) '3 layers + 1 input
			assertEquals(5, alsoVertices.Count) '3 layers + 1 input + merge vertex

			assertFalse(layersOnly.ContainsKey("merge"))
			assertTrue(alsoVertices.ContainsKey("merge"))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSetOutputsMultipleCalls()
		Public Overridable Sub testSetOutputsMultipleCalls()

			'Users generally shouldn't do this, but multiple setOutputs calls should *replace* not *add* outputs

			Dim c As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("out", (New OutputLayer.Builder()).nIn(10).nOut(5).activation(Activation.SOFTMAX).build(), "in").setOutputs("out").setOutputs("out").build()

			Dim l As IList(Of String) = c.getNetworkOutputs()
			assertEquals(1, l.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDropoutValidation()
		Public Overridable Sub testDropoutValidation()
			'At one point: this threw an exception due to incorrect validation
			For Each dropConnect As Boolean In New Boolean(){False, True}
				Call (New NeuralNetConfiguration.Builder()).weightNoise(New DropConnect(0.5)).graphBuilder().setInputTypes(InputType.feedForward(1)).addInputs("input1").addLayer("output", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(1).nOut(1).activation(Activation.SIGMOID).build(), "input1").setOutputs("output").backpropType(BackpropType.Standard).build()
			Next dropConnect
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNoParamLayersL1L2()
		Public Overridable Sub testNoParamLayersL1L2()

			'Don't care about this being valid
			Dim c As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).l1(0.5).l2(0.6).graphBuilder().addInputs("in").addLayer("sub1", (New SubsamplingLayer.Builder(2, 2)).build(), "in").addLayer("sub2", (New Subsampling1DLayer.Builder(2)).build(), "sub1").addLayer("act", (New ActivationLayer.Builder()).activation(Activation.TANH).build(), "sub2").addLayer("pad", (New ZeroPaddingLayer.Builder(2, 3)).build(), "act").addLayer("lrn", (New LocalResponseNormalization.Builder()).build(), "pad").addLayer("pool", (New GlobalPoolingLayer.Builder(PoolingType.AVG)).build(), "act").addLayer("drop", (New DropoutLayer.Builder(0.5)).build(), "pool").addLayer("dense", (New DenseLayer.Builder()).nIn(1).nOut(1).build(), "drop").addLayer("loss", (New LossLayer.Builder(LossFunctions.LossFunction.MCXENT)).build(), "dense").allowDisconnected(True).setOutputs("loss").build()

			Dim g As New ComputationGraph(c)
			g.init()

			g.calcRegularizationScore(True)
			g.calcRegularizationScore(False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testErrorNoOutputLayer()
		Public Overridable Sub testErrorNoOutputLayer()
			assertThrows(GetType(DL4JException),Sub()
			Dim c As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("dense", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").setOutputs("dense").build()
			Dim cg As New ComputationGraph(c)
			cg.init()
			Dim f As INDArray = Nd4j.create(1, 10)
			Dim l As INDArray = Nd4j.create(1, 10)
			cg.Inputs = f
			cg.Labels = l
			cg.computeGradientAndScore()
			End Sub)


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMergeVertexAddition()
		Public Overridable Sub testMergeVertexAddition()

			'When a vertex supports only one input, and gets multiple inputs - we should automatically add a merge
			'vertex

			Dim nnc As New NeuralNetConfiguration()
			nnc.setLayer((New DenseLayer.Builder()).build())
			Dim singleInputVertices() As GraphVertex = {
				New L2NormalizeVertex(),
				New LayerVertex(nnc, Nothing),
				New PoolHelperVertex(),
				New PreprocessorVertex(),
				New ReshapeVertex(New Integer(){1, 1}),
				New ScaleVertex(1.0),
				New ShiftVertex(1.0),
				New SubsetVertex(1, 1),
				New UnstackVertex(0, 2),
				New DuplicateToTimeSeriesVertex("in1"),
				New LastTimeStepVertex("in1")
			}

			For Each gv As GraphVertex In singleInputVertices
				Dim c As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in1", "in2").addVertex("gv", gv, "in1", "in2").setOutputs("gv").build()

				Dim foundMerge As Boolean = False
				For Each g As GraphVertex In c.getVertices().values()
					If TypeOf g Is MergeVertex Then
						foundMerge = True
						Exit For
					End If
				Next g

				If Not foundMerge Then
					fail("Network did not add merge vertex for vertex " & gv.GetType())
				End If
			Next gv
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVertexAsOutput()
		Public Overridable Sub testVertexAsOutput()
			'Simple sanity check: vertex is the last output...

			Dim minibatch As Integer = 10
			Dim height As Integer = 24
			Dim width As Integer = 24
			Dim depth As Integer = 3

			Dim img As INDArray = Nd4j.ones(minibatch, depth, height, width)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input").addLayer("L1", (New ConvolutionLayer.Builder(New Integer(){1, 1}, New Integer(){1, 1}, New Integer(){0, 0})).nIn(depth).nOut(depth).build(), "input").addVertex("L2", New ReshapeVertex(minibatch, 1, 36, 48), "L1").setOutputs("L2").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim [out]() As INDArray = net.output(img)

			assertNotNull([out])
			assertEquals(1, [out].Length)
			assertNotNull([out](0))

			assertArrayEquals(New Long(){minibatch, 1, 36, 48}, [out](0).shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testEpochCounter() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEpochCounter()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("out", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).build(), "in").setOutputs("out").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			assertEquals(0, net.Configuration.getEpochCount())


			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

			For i As Integer = 0 To 3
				assertEquals(i, net.Configuration.getEpochCount())
				net.fit(iter)
				assertEquals(i+1, net.Configuration.getEpochCount())
			Next i

			assertEquals(4, net.Configuration.getEpochCount())

			Dim baos As New MemoryStream()

			ModelSerializer.writeModel(net, baos, True)
			Dim bytes() As SByte = baos.toByteArray()

			Dim bais As New MemoryStream(bytes)

			Dim restored As ComputationGraph = ModelSerializer.restoreComputationGraph(bais, True)
			assertEquals(4, restored.Configuration.getEpochCount())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testSummary()
		Public Overridable Sub testSummary()
			Dim V_WIDTH As Integer = 130
			Dim V_HEIGHT As Integer = 130
			Dim V_NFRAMES As Integer = 150
			Dim confForArchitecture As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l2(0.001).updater(New AdaGrad(0.4)).graphBuilder().addInputs("in").addLayer("layer0", (New ConvolutionLayer.Builder(10, 10)).nIn(3).nOut(30).stride(4, 4).activation(Activation.RELU).weightInit(WeightInit.RELU).build(),"in").addLayer("layer1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3, 3).stride(2, 2).build(),"layer0").addLayer("layer2", (New ConvolutionLayer.Builder(3, 3)).nIn(30).nOut(10).stride(2, 2).activation(Activation.RELU).weightInit(WeightInit.RELU).updater(Updater.ADAGRAD).build(), "layer1").addLayer("layer3", (New DenseLayer.Builder()).activation(Activation.RELU).nIn(490).nOut(50).weightInit(WeightInit.RELU).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build(), "layer2").addLayer("layer4", (New GravesLSTM.Builder()).activation(Activation.SOFTSIGN).nIn(50).nOut(50).weightInit(WeightInit.XAVIER).updater(Updater.ADAGRAD).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build(), "layer3").addLayer("layer5", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(50).nOut(4).weightInit(WeightInit.XAVIER).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build(), "layer4").setOutputs("layer5").inputPreProcessor("layer0", New RnnToCnnPreProcessor(V_HEIGHT, V_WIDTH, 3)).inputPreProcessor("layer3", New CnnToFeedForwardPreProcessor(7, 7, 10)).inputPreProcessor("layer4", New FeedForwardToRnnPreProcessor()).backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(V_NFRAMES \ 5).tBPTTBackwardLength(V_NFRAMES \ 5).build()
			Dim modelExpectedArch As New ComputationGraph(confForArchitecture)
			modelExpectedArch.init()
			Dim modelMow As ComputationGraph = (New TransferLearning.GraphBuilder(modelExpectedArch)).setFeatureExtractor("layer2").build()
	'        System.out.println(modelExpectedArch.summary());
	'        System.out.println(modelMow.summary());
	'        System.out.println(modelExpectedArch.summary(InputType.recurrent(V_HEIGHT* V_WIDTH* 3)));
			modelExpectedArch.summary()
			modelMow.summary()
			modelExpectedArch.summary(InputType.recurrent(V_HEIGHT* V_WIDTH* 3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputClearance() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInputClearance()
			'Activations should be cleared - if not, it's possible for out of (workspace) scope arrays to be around
			' which can cause a crash
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).graphBuilder().addInputs("in").addLayer("0", (New ConvolutionLayer.Builder()).kernelSize(2,2).stride(1,1).nIn(1).nOut(1).build(), "in").addLayer("1", (New SubsamplingLayer.Builder()).kernelSize(2,2).stride(1,1).build(), "0").addLayer("2", (New DenseLayer.Builder()).nOut(10).build(), "1").addLayer("3", (New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build(), "2").setOutputs("3").setInputTypes(InputType.convolutional(28,28,1)).build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim content As INDArray = Nd4j.create(1,1,28,28)

			'Check output:
			net.output(content)
			For Each l As org.deeplearning4j.nn.api.Layer In net.Layers
				assertNull(l.input())
			Next l

			'Check feedForward:
			net.feedForward(content, False)
			For Each l As org.deeplearning4j.nn.api.Layer In net.Layers
				assertNull(l.input())
			Next l
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDisconnectedVertex()
		Public Overridable Sub testDisconnectedVertex()

			For Each allowDisconnected As Boolean In New Boolean(){False, True}
				Try
					Dim b As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).activation(Activation.SIGMOID).nOut(8).build(), "in").addLayer("1", (New DenseLayer.Builder()).activation(Activation.SIGMOID).nOut(8).build(), "in").addLayer("O", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(10).build(), "0").setOutputs("O").setInputTypes(InputType.feedForward(8))

					If allowDisconnected Then
						b.allowDisconnected(True).build() 'No exception
					Else
						b.build() 'Expect exception here
						fail("Expected exception for disconnected vertex")
					End If


				Catch e As Exception
					log.error("",e)
					If allowDisconnected Then
						fail("No exception expected")
					Else
						Dim msg As String = e.Message.ToLower()
						assertTrue(msg.Contains("disconnected"))
					End If
				End Try
			Next allowDisconnected
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLayerSize()
		Public Overridable Sub testLayerSize()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New ConvolutionLayer.Builder()).kernelSize(2,2).nOut(6).build(), "in").layer("1", (New SubsamplingLayer.Builder()).kernelSize(2,2).build(), "0").layer("2", (New DenseLayer.Builder()).nOut(30).build(), "1").layer("3", (New OutputLayer.Builder()).nOut(13).activation(Activation.SOFTMAX).build(), "2").setOutputs("3").setInputTypes(InputType.convolutional(28,28,3)).build()

			Dim net As New ComputationGraph(conf)
			net.init()

			assertEquals(6, net.layerSize(0))
			assertEquals(0, net.layerSize(1))
			assertEquals(30, net.layerSize(2))
			assertEquals(13, net.layerSize(3))

			assertEquals(3, net.layerInputSize(0))
			assertEquals(0, net.layerInputSize(1))
			assertEquals(CType(net.getLayer(2).conf().getLayer(), FeedForwardLayer).getNIn(), net.layerInputSize(2))
			assertEquals(30, net.layerInputSize(3))

			assertEquals(6, net.layerSize("0"))
			assertEquals(0, net.layerSize("1"))
			assertEquals(30, net.layerSize("2"))
			assertEquals(13, net.layerSize("3"))

			assertEquals(3, net.layerInputSize("0"))
			assertEquals(0, net.layerInputSize("1"))
			assertEquals(CType(net.getLayer(2).conf().getLayer(), FeedForwardLayer).getNIn(), net.layerInputSize("2"))
			assertEquals(30, net.layerInputSize("3"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testZeroParamNet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testZeroParamNet()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New SubsamplingLayer.Builder()).kernelSize(2,2).stride(2,2).build(), "in").layer("1", (New LossLayer.Builder()).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.MSE).build(), "0").setOutputs("1").setInputTypes(InputType.convolutionalFlat(28,28,1)).build()

			Dim net As New ComputationGraph(conf)
			net.init()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = (New MnistDataSetIterator(16, True, 12345)).next()

			Dim [out] As INDArray = net.outputSingle(ds.Features)

			Dim labelTemp As INDArray = Nd4j.create([out].shape())
			ds.Labels = labelTemp

			net.fit(ds)

			Dim net2 As ComputationGraph = TestUtils.testModelSerialization(net)
			Dim out2 As INDArray = net2.outputSingle(ds.Features)
			assertEquals([out], out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void scaleVertexGraphTest()
		Public Overridable Sub scaleVertexGraphTest()
			Const scaleFactor As Double = 2
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] inputArr = new float[]{-2, -1, 0, 1, 2};
			Dim inputArr() As Single = {-2, -1, 0, 1, 2} 'IntStream.rangeClosed(-2, 2).mapToDouble(i -> i).toArray();
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] expected = new float[inputArr.length];
			Dim expected(inputArr.Length - 1) As Single 'DoubleStream.of(inputArr).map(i -> i * scaleFactor).toArray();
			For i As Integer = 0 To expected.Length - 1
				expected(i) = CSng(inputArr(i) * scaleFactor)
			Next i

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = getInputArray4d(inputArr);
			Dim input As INDArray = getInputArray4d(inputArr) ' Replacing this line with the line below is enough to make test pass
			'final INDArray input = Nd4j.create(new float[][]{inputArr});

			Const inputName As String = "input"
			Const outputName As String = "output"
			Const scaleName As String = "scale"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ComputationGraph graph = new ComputationGraph(new NeuralNetConfiguration.Builder().graphBuilder().addInputs(inputName).setOutputs(outputName).setInputTypes(org.deeplearning4j.nn.conf.inputs.InputType.inferInputType(input)).addVertex(scaleName, new ScaleVertex(scaleFactor), inputName).addLayer(outputName, new OutputLayer.Builder().activation(new org.nd4j.linalg.activations.impl.ActivationIdentity()).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).nOut(input.length()).biasInit(0).build(), scaleName).build());
			Dim graph As New ComputationGraph((New NeuralNetConfiguration.Builder()).graphBuilder().addInputs(inputName).setOutputs(outputName).setInputTypes(InputType.inferInputType(input)).addVertex(scaleName, New ScaleVertex(scaleFactor), inputName).addLayer(outputName, (New OutputLayer.Builder()).activation(New ActivationIdentity()).lossFunction(LossFunctions.LossFunction.MSE).nOut(input.length()).biasInit(0).build(), scaleName).build())
			graph.init()

			'graph.fit(new DataSet(input, Nd4j.ones(input.length()))); // Does not help
			'graph.feedForward(new INDArray[] {input}, false); // Uncommenting this line is enough to make test pass

			'Hack output layer to be identity mapping
			graph.getOutputLayer(0).setParam("W", Nd4j.eye(input.length()))
			graph.getOutputLayer(0).setParam("b", Nd4j.zeros(input.length()))
			assertEquals(Nd4j.create(expected).reshape(ChrW(1), expected.Length), graph.outputSingle(input),"Incorrect output")
		End Sub

		Private Shared Function getInputArray4d(ByVal inputArr() As Single) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.create(org.nd4j.linalg.api.buffer.DataType.FLOAT, 1, 1, inputArr.length, 1);
			Dim input As INDArray = Nd4j.create(DataType.FLOAT, 1, 1, inputArr.Length, 1)
			Dim i As Integer = 0
			Do While i < input.length()
				input.putScalar(New Integer(){0, 0, i, 0}, inputArr(i))
				i += 1
			Loop
			Return input
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphOutputIterators()
		Public Overridable Sub testGraphOutputIterators()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim all As DataSet = (New IrisDataSetIterator(150,150)).next()
			Dim iter As DataSetIterator = New IrisDataSetIterator(5,150)

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").layer("layer", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).build(), "in").setOutputs("layer").build()
			Dim cg As New ComputationGraph(conf)
			cg.init()


			Dim outAll As INDArray = cg.outputSingle(all.Features)
			Dim outIter As INDArray = cg.outputSingle(iter)

			assertEquals(outAll, outIter)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testComputationGraphConfgurationActivationTypes()
		Public Overridable Sub testComputationGraphConfgurationActivationTypes()

			'Test for a simple net:

			Dim builder As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in1", "in2").layer("0", (New DenseLayer.Builder()).nOut(10).build(), "in1").layer("1", (New DenseLayer.Builder()).nOut(9).build(), "in1", "in2").layer("2", (New DenseLayer.Builder()).nOut(8).build(), "in2").layer("3", (New DenseLayer.Builder()).nOut(7).build(), "0").layer("4", (New DenseLayer.Builder()).nOut(6).build(), "1", "2").setInputTypes(InputType.feedForward(5), InputType.feedForward(6)).allowNoOutput(True)

			Dim conf As ComputationGraphConfiguration = builder.build()

			Dim actBuilder As IDictionary(Of String, InputType) = builder.getLayerActivationTypes()
			Dim actConf As IDictionary(Of String, InputType) = conf.getLayerActivationTypes(InputType.feedForward(5), InputType.feedForward(6))

			Dim exp As IDictionary(Of String, InputType) = New Dictionary(Of String, InputType)()
			exp("in1") = InputType.feedForward(5)
			exp("in2") = InputType.feedForward(6)
			exp("0") = InputType.feedForward(10)
			exp("1") = InputType.feedForward(9)
			exp("1-merge") = InputType.feedForward(5+6)
			exp("2") = InputType.feedForward(8)
			exp("3") = InputType.feedForward(7)
			exp("4") = InputType.feedForward(6)
			exp("4-merge") = InputType.feedForward(9+8)

			assertEquals(exp, actBuilder)
			assertEquals(exp, actConf)
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTopoSortSaving()
		Public Overridable Sub testTopoSortSaving()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in1", "in2").addLayer("l0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in1").addLayer("l1", (New DenseLayer.Builder()).nIn(20).nOut(10).build(), "in1", "in2").addLayer("l2", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in2").addLayer("l3", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "l0").addLayer("l4", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "l1").addLayer("l5", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "l2").addLayer("l6", (New OutputLayer.Builder()).nIn(20).nOut(10).activation(Activation.SOFTMAX).build(), "l3", "l5").addLayer("l7", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "l4").setOutputs("l6", "l7").build()

			Dim [in]() As INDArray = { Nd4j.rand(3, 10), Nd4j.rand(3, 10)}

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim indices As GraphIndices = cg.calculateIndices()

			Dim order() As Integer = cg.topologicalSortOrder()
			Dim strOrder As IList(Of String) = cg.Configuration.getTopologicalOrderStr()
			Dim out1() As INDArray = cg.output([in])

			'Check it's the same after loading:
			Dim cg2 As ComputationGraph = TestUtils.testModelSerialization(cg)
			Dim order2() As Integer = cg2.topologicalSortOrder()
			Dim strOrder2 As IList(Of String) = cg.Configuration.getTopologicalOrderStr()
			assertArrayEquals(order, order2)
			assertEquals(strOrder, strOrder2)

			Dim out2() As INDArray = cg2.output([in])
			assertArrayEquals(out1, out2)

			'Delete the topological order, ensure it gets recreated properly:
			Dim conf3 As ComputationGraphConfiguration = cg2.Configuration.clone()
			conf3.setTopologicalOrder(Nothing)
			conf3.setTopologicalOrderStr(Nothing)
			Dim cg3 As New ComputationGraph(conf3)
			cg3.init()
			cg3.Params = cg2.params()

			Dim order3() As Integer = cg3.topologicalSortOrder()
			Dim strOrder3 As IList(Of String) = cg.Configuration.getTopologicalOrderStr()
			Dim out3() As INDArray = cg3.output([in])
			assertArrayEquals(order, order3)
			assertEquals(strOrder, strOrder3)
			assertArrayEquals(out1, out3)


			'Now, change the order, and ensure the net is the same... note that we can do [l0, l1, l2] in any order
			Dim someValidOrders As IList(Of IList(Of String)) = New List(Of IList(Of String))()
			someValidOrders.Add(New List(Of String) From {"in1", "in2", "l0", "l1-merge", "l1", "l2", "l3", "l4", "l5", "l6-merge", "l6", "l7"})
			someValidOrders.Add(New List(Of String) From {"in1", "in2", "l1-merge", "l1", "l0", "l2", "l3", "l4", "l5", "l6-merge", "l6", "l7"})
			someValidOrders.Add(New List(Of String) From {"in1", "in2", "l2", "l1-merge", "l1", "l0", "l3", "l4", "l5", "l6-merge", "l6", "l7"})
			someValidOrders.Add(New List(Of String) From {"in1", "in2", "l2", "l5", "l0", "l1-merge", "l1", "l3", "l4", "l7", "l6-merge", "l6"})

			For Each l As IList(Of String) In someValidOrders
				assertEquals(strOrder.Count, l.Count)
			Next l

			For i As Integer = 0 To someValidOrders.Count - 1
				Dim l As IList(Of String) = someValidOrders(i)
				Dim arr(l.Count - 1) As Integer
				Dim j As Integer=0
				For Each s As String In l
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: arr[j++] = indices.getNameToIdx().get(s);
					arr(j) = indices.getNameToIdx().get(s)
						j += 1
				Next s

				Dim conf2 As ComputationGraphConfiguration = conf.clone()
				conf2.setTopologicalOrderStr(l)
				conf2.setTopologicalOrder(arr)

				Dim g As New ComputationGraph(conf2)
				g.init()
				g.ParamTable = cg.paramTable()
				Dim origOrder() As Integer = g.topologicalSortOrder()

				Dim out4() As INDArray = g.output([in])
				assertArrayEquals(out1, out4)

				Dim g2 As ComputationGraph = TestUtils.testModelSerialization(g)
				Dim loadedOrder() As Integer = g2.topologicalSortOrder()

				assertArrayEquals(origOrder, loadedOrder)

				Dim out5() As INDArray = g2.output([in])
				assertArrayEquals(out1, out5)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPretrainFitMethods()
		Public Overridable Sub testPretrainFitMethods()

			'The fit methods should *not* do layerwise pretraining:

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New VariationalAutoencoder.Builder()).nIn(10).nOut(10).encoderLayerSizes(10).decoderLayerSizes(10).build(), "in").layer("1", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim exp As ISet(Of Type) = New HashSet(Of Type)()
			exp.Add(GetType(ComputationGraph))

			Dim listener As New MultiLayerTest.CheckModelsListener()
			net.setListeners(listener)

			Dim f As INDArray = Nd4j.create(DataType.DOUBLE,1,10)
			Dim l As INDArray = Nd4j.create(DataType.DOUBLE,1,10)
			Dim ds As New DataSet(f,l)
			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(f,l)

			Dim iter As DataSetIterator = New ExistingDataSetIterator(Collections.singletonList(ds))

			net.fit(ds)
			assertEquals(exp, listener.getModelClasses())

			net.fit(iter)
			assertEquals(exp, listener.getModelClasses())

			net.fit(New INDArray(){f}, New INDArray(){l})
			assertEquals(exp, listener.getModelClasses())

			net.fit(New INDArray(){f}, New INDArray(){l}, Nothing, Nothing)
			assertEquals(exp, listener.getModelClasses())

			net.fit(mds)
			assertEquals(exp, listener.getModelClasses())

			net.fit(New SingletonMultiDataSetIterator(mds))
			assertEquals(exp, listener.getModelClasses())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAllowInputModification()
		Public Overridable Sub testAllowInputModification()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in1", "in2").layer("0", (New DenseLayer.Builder()).nOut(10).build(), "in1").layer("1", (New DenseLayer.Builder()).nOut(10).build(), "in2").layer("2", (New DenseLayer.Builder()).nOut(10).build(), "0").layer("3", (New DenseLayer.Builder()).nOut(10).build(), "1").layer("4", (New DenseLayer.Builder()).nOut(10).build(), "1").layer("5", (New DenseLayer.Builder()).nOut(10).build(), "1").layer("6", (New DenseLayer.Builder()).nOut(10).build(), "2", "3", "4", "5").setOutputs("6").setInputTypes(InputType.feedForward(10), InputType.feedForward(10)).build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim exp As IDictionary(Of String, Boolean) = New Dictionary(Of String, Boolean)()
			exp("0") = False
			exp("1") = False
			exp("2") = True
			exp("3") = False
			exp("4") = False
			exp("5") = True
			exp("6") = True


			For Each s As String In exp.Keys
				Dim allowed As Boolean = DirectCast(cg.getLayer(s), org.deeplearning4j.nn.layers.feedforward.dense.DenseLayer).isInputModificationAllowed()
	'            System.out.println(s + "\t" + allowed);
				assertEquals(exp(s), allowed,s)
			Next s
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompGraphDropoutOutputLayers()
		Public Overridable Sub testCompGraphDropoutOutputLayers()
			'https://github.com/eclipse/deeplearning4j/issues/6326
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(0.8).graphBuilder().addInputs("in1", "in2").addVertex("merge", New MergeVertex(), "in1", "in2").addLayer("lstm", New Bidirectional(Bidirectional.Mode.CONCAT, (New LSTM.Builder()).nIn(10).nOut(5).activation(Activation.TANH).dropOut(New GaussianNoise(0.05)).build()),"merge").addLayer("out1", (New RnnOutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(10).nOut(6).build(), "lstm").addLayer("out2", (New RnnOutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(10).nOut(4).build(), "lstm").setOutputs("out1", "out2").setInputTypes(InputType.recurrent(5,5,RNNFormat.NCW),InputType.recurrent(5,5,RNNFormat.NCW)).build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim features() As INDArray = {Nd4j.create(1, 5, 5), Nd4j.create(1, 5, 5)}
			Dim labels() As INDArray = {Nd4j.create(1, 6, 5), Nd4j.create(1, 4, 5)}
			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(features, labels)
			net.fit(mds)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompGraphDropoutOutputLayers2()
		Public Overridable Sub testCompGraphDropoutOutputLayers2()
			'https://github.com/eclipse/deeplearning4j/issues/6326
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(0.8).graphBuilder().addInputs("in1", "in2").addVertex("merge", New MergeVertex(), "in1", "in2").addLayer("dense", (New DenseLayer.Builder()).nIn(10).nOut(5).activation(Activation.TANH).dropOut(New GaussianNoise(0.05)).build(),"merge").addLayer("out1", (New OutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(5).nOut(6).build(), "dense").addLayer("out2", (New OutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(5).nOut(4).build(), "dense").setInputTypes(InputType.feedForward(5),InputType.feedForward(5)).setOutputs("out1", "out2").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim features() As INDArray = {Nd4j.create(1, 5), Nd4j.create(1, 5)}
			Dim labels() As INDArray = {Nd4j.create(1, 6), Nd4j.create(1, 4)}
			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(features, labels)
			net.fit(mds)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAddRemoveVertex()
		Public Overridable Sub testAddRemoveVertex()
			Call (New NeuralNetConfiguration.Builder()).graphBuilder().addVertex("toRemove", New ScaleVertex(0), "don't care").addVertex("test", New ScaleVertex(0), "toRemove").removeVertex("toRemove", True)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGetSetParamUnderscores()
		Public Overridable Sub testGetSetParamUnderscores()
			'Test get/set param with underscores in layer nome
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("layer_zero", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").layer("layer_one", (New OutputLayer.Builder()).nIn(10).nOut(10).build(), "layer_zero").setOutputs("layer_one").build()

			Dim cg As New ComputationGraph(conf)
			cg.init()
			cg.params().assign(Nd4j.linspace(1, 220, 220).reshape(ChrW(1), -11))

			Dim p0w As INDArray = cg.getParam("layer_zero_W")
			assertEquals(Nd4j.linspace(1, 100, 100).reshape("f"c, 10, 10), p0w)

			Dim p1b As INDArray = cg.getParam("layer_one_b")
			assertEquals(Nd4j.linspace(211, 220, 10).reshape(ChrW(1), 10), p1b)

			Dim newP1b As INDArray = Nd4j.valueArrayOf(New Long(){1, 10}, -1.0)
			cg.setParam("layer_one_b", newP1b)

			assertEquals(newP1b, p1b)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputSpecificLayers()
		Public Overridable Sub testOutputSpecificLayers()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").layer("0", (New DenseLayer.Builder()).nIn(10).nOut(9).build(), "in").layer("1", (New DenseLayer.Builder()).nIn(9).nOut(8).build(), "0").layer("2", (New DenseLayer.Builder()).nIn(8).nOut(7).build(), "1").layer("3", (New OutputLayer.Builder()).nIn(7).nOut(6).build(), "2").setOutputs("3").build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim [in] As INDArray = Nd4j.rand(1, 10)

			Dim outMap As IDictionary(Of String, INDArray) = cg.feedForward([in], False)

			Dim outSpecific() As INDArray = cg.output(java.util.Arrays.asList("1", "3"), False, New INDArray(){[in]}, Nothing)
			assertEquals(2, outSpecific.Length)

			assertEquals(outMap("1"), outSpecific(0))
			assertEquals(outMap("3"), outSpecific(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void singleInputElemVertex()
		Public Overridable Sub singleInputElemVertex()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.inputs.InputType inputType = org.deeplearning4j.nn.conf.inputs.InputType.convolutional(10, 10, 2);
			Dim inputType As InputType = InputType.convolutional(10, 10, 2)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ComputationGraph graph = new ComputationGraph(new NeuralNetConfiguration.Builder().graphBuilder().setInputTypes(inputType).addInputs("input").setOutputs("output").addLayer("0", new ConvolutionLayer.Builder().nOut(5).convolutionMode(ConvolutionMode.Same).build(),"input").addVertex("dummyAdd", new ElementWiseVertex(ElementWiseVertex.Op.Add), "0").addLayer("output", new CnnLossLayer(), "dummyAdd").build());
			Dim graph As New ComputationGraph((New NeuralNetConfiguration.Builder()).graphBuilder().setInputTypes(inputType).addInputs("input").setOutputs("output").addLayer("0", (New ConvolutionLayer.Builder()).nOut(5).convolutionMode(ConvolutionMode.Same).build(),"input").addVertex("dummyAdd", New ElementWiseVertex(ElementWiseVertex.Op.Add), "0").addLayer("output", New CnnLossLayer(), "dummyAdd").build())
			graph.init()
			graph.outputSingle(Nd4j.randn(1, 2, 10, 10))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCloneDropoutIndependence()
		Public Overridable Sub testCloneDropoutIndependence()

			Dim modelConf As val = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.01)).weightInit(WeightInit.XAVIER_UNIFORM).biasInit(0).graphBuilder().addInputs("input").addLayer("dense", (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.RELU).hasBias(True).dropOut(0.9).build(), "input").addLayer("output", (New OutputLayer.Builder()).nIn(10).nOut(1).lossFunction(LossFunctions.LossFunction.XENT).activation(Activation.SIGMOID).hasBias(False).build(), "dense").setOutputs("output").build()

			Dim model As New ComputationGraph(modelConf)
			model.init()

			Dim cg2 As ComputationGraph = model.clone()

			Dim d1 As IDropout = model.getLayer(0).conf().getLayer().getIDropout()
			Dim d2 As IDropout = cg2.getLayer(0).conf().getLayer().getIDropout()

			assertFalse(d1 Is d2) 'Should not be same object!
			assertEquals(d1, d2) 'But should be equal
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVerticesAndMasking7027()
		Public Overridable Sub testVerticesAndMasking7027()
			'https://github.com/eclipse/deeplearning4j/issues/7027
			Dim inputSize As Integer = 300
			Dim hiddenSize As Integer = 100
			Dim dataSize As Integer = 10
			Dim seqLen As Integer = 5
			Dim configuration As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam()).graphBuilder().addInputs("x_emb").addLayer("agg_lstm", New Bidirectional(CONCAT, (New LSTM.Builder()).nOut(hiddenSize\2).build()), "x_emb").addLayer("agg_att", (New DenseLayer.Builder()).nIn(100).nOut(1).activation(Activation.SOFTMAX).build(), "agg_lstm").addVertex("att", New PreprocessorVertex(New ComposableInputPreProcessor(New FeedForwardToRnnPreProcessor(), New PermutePreprocessor(New Integer() {0, 2, 1}), New RnnToFeedForwardPreProcessor())), "agg_att").addLayer("att_repeat", (New RepeatVector.Builder(hiddenSize)).build(),"att").addVertex("att_trans", New PreprocessorVertex(New PermutePreprocessor(New Integer() {0, 2, 1})), "att_repeat").addVertex("mult", New ElementWiseVertex(ElementWiseVertex.Op.Product), "agg_lstm", "att_trans").addLayer("sum", (New GlobalPoolingLayer.Builder()).build(), "mult").addLayer("agg_out", (New DenseLayer.Builder()).nIn(100).nOut(6).activation(Activation.TANH).build(), "sum").addLayer("output", (New OutputLayer.Builder()).nIn(6).nOut(6).lossFunction(LossFunctions.LossFunction.RECONSTRUCTION_CROSSENTROPY).build(), "agg_out").setOutputs("output").setInputTypes(InputType.recurrent(inputSize,seqLen,RNNFormat.NCW)).build()

			Dim net As New ComputationGraph(configuration)
			net.init()


			Dim features As INDArray = Nd4j.rand(New Integer() {dataSize, inputSize, seqLen})
			Dim labels As INDArray = Nd4j.rand(New Integer() {dataSize, 6})
			Dim featuresMask As INDArray = Nd4j.ones(dataSize, seqLen)
			Dim labelsMask As INDArray = Nd4j.ones(dataSize, 6)

			Dim dataSet1 As New DataSet(features, labels)
			net.fit(dataSet1)
			Dim dataSet2 As New DataSet(features, labels, featuresMask, labelsMask)
			net.fit(dataSet2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompGraphUpdaterBlocks()
		Public Overridable Sub testCompGraphUpdaterBlocks()
			'Check that setting learning rate results in correct rearrangement of updater state within updater blocks
			'https://github.com/eclipse/deeplearning4j/issues/6809#issuecomment-463892644

			Dim lr As Double = 1e-3
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).updater(New Adam(lr)).graphBuilder().backpropType(BackpropType.Standard).addInputs("in").setOutputs("out").addLayer("0",(New DenseLayer.Builder()).nIn(5).nOut(3).build(),"in").addLayer("1",(New DenseLayer.Builder()).nIn(3).nOut(2).build(),"0").addLayer("out",(New OutputLayer.Builder(LossFunctions.LossFunction.XENT)).nIn(2).nOut(1).activation(Activation.SIGMOID).build(),"1").build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim [in] As INDArray = Nd4j.rand(1, 5)
			Dim lbl As INDArray = Nd4j.rand(1,1)

			cg.fit(New DataSet([in], lbl))

			Dim viewArray As INDArray = cg.Updater.getUpdaterStateViewArray()
			Dim viewArrayCopy As INDArray = viewArray.dup()
			'Initially updater view array is set out like:
			'[m0w, m0b, m1w, m1b, m2w, m2b][v0w, v0b, v1w, v1b, v2w, v2b]
			Dim soFar As Long = 0
			Dim m0w As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+5*3)).assign(0) 'm0w
			soFar += 5*3
			Dim m0b As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+3)).assign(1) 'm0b
			soFar += 3
			Dim m1w As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+3*2)).assign(2) 'm1w
			soFar += 3*2
			Dim m1b As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+2)).assign(3) 'm1b
			soFar += 2
			Dim m2w As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+2*1)).assign(4) 'm2w
			soFar += 2*1
			Dim m2b As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+1)).assign(5) 'm2b
			soFar += 1

			Dim v0w As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+5*3)).assign(6) 'v0w
			soFar += 5*3
			Dim v0b As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+3)).assign(7) 'v0b
			soFar += 3
			Dim v1w As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+3*2)).assign(8) 'v1w
			soFar += 3*2
			Dim v1b As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+2)).assign(9) 'v1b
			soFar += 2
			Dim v2w As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+2*1)).assign(10) 'v2w
			soFar += 2*1
			Dim v2b As INDArray = viewArray.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar+1)).assign(11) 'v2b
			soFar += 1


			cg.setLearningRate("0", 0.0)

			'Expect new updater state to look like:
			'[m0w, m0b][v0w,v0b], [m1w, m1b, m2w, m2b][v1w, v1b, v2w, v2b]
			Dim exp As INDArray = Nd4j.concat(1, m0w, m0b, v0w, v0b, m1w, m1b, m2w, m2b, v1w, v1b, v2w, v2b)

			Dim act As INDArray = cg.Updater.getUpdaterStateViewArray()
	'        System.out.println(exp);
	'        System.out.println(act);

			assertEquals(exp, act)

			'And set layer 1 LR:
			cg.setLearningRate("1", 0.2)
			exp = Nd4j.concat(1, m0w, m0b, v0w, v0b, m1w, m1b, v1w, v1b, m2w, m2b, v2w, v2b)
			assertEquals(exp, cg.Updater.StateViewArray)


			'Set all back to original LR and check again:
			cg.setLearningRate("1", lr)
			cg.setLearningRate("0", lr)

			exp = Nd4j.concat(1, m0w, m0b, m1w, m1b, m2w, m2b, v0w, v0b, v1w, v1b, v2w, v2b)
			assertEquals(exp, cg.Updater.StateViewArray)


			'Finally, training sanity check (if things are wrong, we get -ve values in adam V, which causes NaNs)
			cg.Updater.StateViewArray.assign(viewArrayCopy)
			cg.setLearningRate("0", 0.0)

			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.NAN_PANIC
			cg.fit(New DataSet([in], lbl))
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) @Disabled public void testCompGraphInputReuse()
		Public Overridable Sub testCompGraphInputReuse()
			Dim inputSize As Integer = 5
			Dim outputSize As Integer = 6
			Dim layerSize As Integer = 3

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).weightInit(WeightInit.XAVIER).updater(New NoOp()).graphBuilder().addInputs("in").setOutputs("out").addLayer("0",(New DenseLayer.Builder()).nIn(inputSize).nOut(layerSize).build(),"in").addVertex("combine", New MergeVertex(), "0", "0", "0").addLayer("out",(New OutputLayer.Builder(LossFunctions.LossFunction.XENT)).nIn(3*layerSize).nOut(outputSize).activation(Activation.SIGMOID).build(),"combine").build()

			Dim net As New ComputationGraph(conf)
			net.init()


			Dim dataSize As Integer = 11
			Dim features As INDArray = Nd4j.rand(DataType.DOUBLE,New Integer() {dataSize, inputSize})
			Dim labels As INDArray = Nd4j.rand(DataType.DOUBLE,New Integer() {dataSize, outputSize})

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(net).inputs(New INDArray(){features}).labels(New INDArray(){labels}))
			assertTrue(gradOK)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConv3dMergeVertex()
		Public Overridable Sub testConv3dMergeVertex()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addLayer("l0", (New Convolution3D.Builder()).kernelSize(2,2,2).stride(1,1,1).nIn(3).nOut(3).dataFormat(Convolution3D.DataFormat.NCDHW).build(), "in").addLayer("l1", (New Convolution3D.Builder()).kernelSize(2,2,2).stride(1,1,1).nIn(3).nOut(3).dataFormat(Convolution3D.DataFormat.NCDHW).build(), "in").addVertex("out", New MergeVertex(), "l0", "l1").setInputTypes(InputType.convolutional3D(Convolution3D.DataFormat.NCDHW, 16, 16, 16, 3)).addInputs("in").setOutputs("out").build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim [in] As INDArray = Nd4j.create(DataType.FLOAT, 1, 3, 16, 16, 16)
			Dim [out] As INDArray = cg.outputSingle([in])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDualEmbedding()
		Public Overridable Sub testDualEmbedding()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("e1", (New EmbeddingLayer.Builder()).nIn(10).nOut(5).build(), "in").addLayer("e2", (New EmbeddingLayer.Builder()).nIn(10).nOut(5).build(), "in").addLayer("out", (New OutputLayer.Builder()).nIn(10).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "e1", "e2").setOutputs("out").build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim [in] As INDArray = Nd4j.createFromArray(3).reshape(ChrW(1), 1)
			Dim label As INDArray = Nd4j.createFromArray(1, 0).reshape(1, 2)
			cg.fit(New DataSet([in], label))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMergeNchw(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMergeNchw(ByVal testDir As Path)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).graphBuilder().addInputs("in").layer("l0", (New ConvolutionLayer.Builder()).nOut(16).dataFormat(CNN2DFormat.NHWC).kernelSize(2,2).stride(1,1).build(), "in").layer("l1", (New ConvolutionLayer.Builder()).nOut(8).dataFormat(CNN2DFormat.NHWC).kernelSize(2,2).stride(1,1).build(), "in").addVertex("merge", New MergeVertex(), "l0", "l1").layer("out", (New CnnLossLayer.Builder()).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "merge").setOutputs("out").setInputTypes(InputType.convolutional(32, 32, 3, CNN2DFormat.NHWC)).build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim [in]() As INDArray = {Nd4j.rand(DataType.FLOAT, 1, 32, 32, 3)}
			Dim [out] As INDArray = cg.outputSingle([in])

			Dim dir As File = testDir.toFile()
			Dim f As New File(dir, "net.zip")
			cg.save(f)

			Dim c2 As ComputationGraph = ComputationGraph.load(f, True)
			Dim out2 As INDArray = c2.outputSingle([in])

			assertEquals([out], out2)
		End Sub
	End Class

End Namespace