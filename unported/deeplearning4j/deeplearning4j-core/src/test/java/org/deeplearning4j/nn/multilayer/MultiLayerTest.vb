Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Yolo2OutputLayer = org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToCnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports org.deeplearning4j.nn.layers
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports org.junit.jupiter.api
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports SplitTestAndTrain = org.nd4j.linalg.dataset.SplitTestAndTrain
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Heartbeat = org.nd4j.linalg.heartbeat.Heartbeat
Imports Environment = org.nd4j.linalg.heartbeat.reports.Environment
Imports [Event] = org.nd4j.linalg.heartbeat.reports.Event
Imports Task = org.nd4j.linalg.heartbeat.reports.Task
Imports EnvironmentUtils = org.nd4j.linalg.heartbeat.utils.EnvironmentUtils
Imports TaskUtils = org.nd4j.linalg.heartbeat.utils.TaskUtils
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
import static org.junit.jupiter.api.Assertions.assertThrows

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
Namespace org.deeplearning4j.nn.multilayer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Multi Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class MultiLayerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class MultiLayerTest
		Inherits BaseDL4JTest

		Private Shared origMode As OpExecutioner.ProfilingMode

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll static void beforeClass()
		Friend Shared Sub beforeClass()
			origMode = Nd4j.Executioner.ProfilingMode
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void before()
		Friend Overridable Sub before()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterAll static void afterClass()
		Friend Shared Sub afterClass()
			Nd4j.Executioner.ProfilingMode = origMode
		End Sub

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Params") void testSetParams()
		Friend Overridable Sub testSetParams()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.TANH).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).build()).build()
			Dim network3 As New MultiLayerNetwork(conf)
			network3.init()
			Dim params As INDArray = network3.params()
			Dim weights As INDArray = network3.getLayer(0).getParam(DefaultParamInitializer.WEIGHT_KEY).dup()
			Dim bias As INDArray = network3.getLayer(0).getParam(DefaultParamInitializer.BIAS_KEY).dup()
			network3.Parameters = params
			assertEquals(weights, network3.getLayer(0).getParam(DefaultParamInitializer.WEIGHT_KEY))
			assertEquals(bias, network3.getLayer(0).getParam(DefaultParamInitializer.BIAS_KEY))
			Dim params4 As INDArray = network3.params()
			assertEquals(params, params4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batch Norm") void testBatchNorm()
		Friend Overridable Sub testBatchNorm()
			Nd4j.Random.setSeed(123)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(2, (New BatchNormalization.Builder()).nOut(2).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(2).nOut(3).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.setListeners(New ScoreIterationListener(1))
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			[next].normalizeZeroMeanZeroUnitVariance()
			Dim trainTest As SplitTestAndTrain = [next].splitTestAndTrain(110)
			network.Labels = trainTest.Train.Labels
			network.init()
			For i As Integer = 0 To 4
				network.fit(trainTest.Train)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Back Prop") void testBackProp()
		Friend Overridable Sub testBackProp()
			Nd4j.Random.setSeed(123)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(2).nOut(3).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.setListeners(New ScoreIterationListener(1))
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			[next].normalizeZeroMeanZeroUnitVariance()
			Dim trainTest As SplitTestAndTrain = [next].splitTestAndTrain(110)
			network.Input = trainTest.Train.Features
			network.Labels = trainTest.Train.Labels
			network.init()
			For i As Integer = 0 To 4
				network.fit(trainTest.Train)
			Next i
			Dim test As DataSet = trainTest.Test
			Dim eval As New Evaluation()
			Dim output As INDArray = network.output(test.Features)
			eval.eval(test.Labels, output)
			log.info("Score " & eval.stats())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient With As List") void testGradientWithAsList()
		Friend Overridable Sub testGradientWithAsList()
			Dim net1 As New MultiLayerNetwork(Conf)
			Dim net2 As New MultiLayerNetwork(Conf)
			net1.init()
			net2.init()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim x1 As DataSet = (New IrisDataSetIterator(1, 150)).next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim all As DataSet = (New IrisDataSetIterator(150, 150)).next()
			Dim x2 As DataSet = all.asList()(0)
			' x1 and x2 contain identical data
			assertArrayEquals(asFloat(x1.Features), asFloat(x2.Features), 0.0f)
			assertArrayEquals(asFloat(x1.Labels), asFloat(x2.Labels), 0.0f)
			assertEquals(x1, x2)
			' Set inputs/outputs so gradient can be calculated:
			net1.feedForward(x1.Features)
			net2.feedForward(x2.Features)
			DirectCast(net1.getLayer(1), BaseOutputLayer).setLabels(x1.Labels)
			DirectCast(net2.getLayer(1), BaseOutputLayer).setLabels(x2.Labels)
			net1.gradient()
			net2.gradient()
		End Sub

		''' <summary>
		'''  This test intended only to test activateSelectedLayers method, it does not involves fully-working AutoEncoder.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Selected Activations") void testSelectedActivations()
		Friend Overridable Sub testSelectedActivations()
			' Train DeepAutoEncoder on very limited trainset
			Const numRows As Integer = 28
			Const numColumns As Integer = 28
			Dim seed As Integer = 123
			Dim numSamples As Integer = 3
			Dim iterations As Integer = 1
			Dim listenerFreq As Integer = iterations \ 5
			log.info("Load data....")
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim trainingData[][] As Single = new Single[numSamples][numColumns * numRows]
			Dim trainingData()() As Single = RectangularArrays.RectangularSingleArray(numSamples, numColumns * numRows)
			Arrays.Fill(trainingData(0), 0.95f)
			Arrays.Fill(trainingData(1), 0.5f)
			Arrays.Fill(trainingData(2), 0.05f)
			log.info("Build model....")
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(numRows * numColumns).nOut(1000).build()).layer(1, (New DenseLayer.Builder()).nIn(1000).nOut(500).build()).layer(2, (New DenseLayer.Builder()).nIn(500).nOut(250).build()).layer(3, (New DenseLayer.Builder()).nIn(250).nOut(100).build()).layer(4, (New DenseLayer.Builder()).nIn(100).nOut(30).build()).layer(5, (New DenseLayer.Builder()).nIn(30).nOut(100).build()).layer(6, (New DenseLayer.Builder()).nIn(100).nOut(250).build()).layer(7, (New DenseLayer.Builder()).nIn(250).nOut(500).build()).layer(8, (New DenseLayer.Builder()).nIn(500).nOut(1000).build()).layer(9, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nIn(1000).nOut(numRows * numColumns).activation(Activation.SOFTMAX).build()).build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			model.addListeners(New ScoreIterationListener(listenerFreq))
			log.info("Train model....")
			Dim cnt As Integer = 0
			Do While cnt < numSamples
				Dim input As INDArray = Nd4j.create(trainingData(cnt)).reshape(1, -1)
				model.fit(New DataSet(input, input))
				cnt += 1
			Loop
			' Make two separate selective calls
			log.info("Testing full cycle...")
			Dim comparableResult As IList(Of INDArray) = model.feedForward(Nd4j.create(trainingData(0), New Long() { 1, trainingData(0).Length }))
			Dim encodeResult As INDArray = model.activateSelectedLayers(0, 4, Nd4j.create(trainingData(0), New Long() { 1, trainingData(0).Length }))
			log.info("Compare feedForward results with selectedActivation")
			assertEquals(comparableResult(5), encodeResult)
			Dim decodeResults As INDArray = model.activateSelectedLayers(5, 9, encodeResult)
			log.info("Decode results: " & decodeResults.columns() & " " & decodeResults)
			log.info("Comparable  results: " & comparableResult(10).columns() & " " & comparableResult(10))
			assertEquals(comparableResult(10), decodeResults)
		End Sub

		Private Shared ReadOnly Property Conf As MultiLayerConfiguration
			Get
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).dist(New NormalDistribution(0, 1)).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).dist(New NormalDistribution(0, 1)).build()).build()
				Return conf
			End Get
		End Property

		Public Shared Function asFloat(ByVal arr As INDArray) As Single()
			Dim len As Long = arr.length()
			Dim f(CInt(len) - 1) As Single
			For i As Integer = 0 To len - 1
				f(i) = arr.getFloat(i)
			Next i
			Return f
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Feed Forward To Layer") void testFeedForwardToLayer()
		Friend Overridable Sub testFeedForwardToLayer()
			Dim nIn As Integer = 30
			Dim nOut As Integer = 25
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).updater(New Sgd(1e-3)).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(600).dist(New NormalDistribution(0, 1e-5)).build()).layer(1, (New DenseLayer.Builder()).nIn(600).nOut(250).dist(New NormalDistribution(0, 1e-5)).build()).layer(2, (New DenseLayer.Builder()).nIn(250).nOut(100).dist(New NormalDistribution(0, 1e-5)).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(100).nOut(25).activation(Activation.SOFTMAX).weightInit(New NormalDistribution(0, 1e-5)).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Dim input As INDArray = Nd4j.rand(5, nIn)
			Dim activations As IList(Of INDArray) = network.feedForward(input)
			' 4 layers + input
			assertEquals(5, activations.Count)
			Dim activationsAll As IList(Of INDArray) = network.feedForwardToLayer(3, input)
			assertEquals(activations, activationsAll)
			For i As Integer = 3 To 0 Step -1
				Dim activationsPartial As IList(Of INDArray) = network.feedForwardToLayer(i, input)
				' i+2: for layer 3: input + activations of {0,1,2,3} -> 5 total = 3+2
				assertEquals(i + 2, activationsPartial.Count)
				For j As Integer = 0 To i
					Dim exp As INDArray = activationsAll(j)
					Dim act As INDArray = activationsPartial(j)
					assertEquals(exp, act)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Backprop Gradient") void testBackpropGradient()
		Friend Overridable Sub testBackpropGradient()
			' Testing: MultiLayerNetwork.backpropGradient()
			' i.e., specifically without an output layer
			Dim nIn As Integer = 10
			Dim nOut As Integer = 40
			Dim miniBatch As Integer = 5
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(20).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(30).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(2, (New DenseLayer.Builder()).nIn(30).nOut(nOut).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Nd4j.Random.setSeed(12345)
			Dim eps As INDArray = Nd4j.rand(miniBatch, nOut)
			Dim input As INDArray = Nd4j.rand(miniBatch, nIn)
			net.Input = input
			' Need to feed forward before backprop
			net.feedForward(True, False)
			Dim pair As Pair(Of Gradient, INDArray) = net.backpropGradient(eps, LayerWorkspaceMgr.noWorkspaces())
			Dim epsOut As INDArray = pair.Second
			assertNotNull(epsOut)
			assertArrayEquals(New Long() { miniBatch, nIn }, epsOut.shape())
			Dim g As Gradient = pair.First
			Dim gradMap As IDictionary(Of String, INDArray) = g.gradientForVariable()
			' 3 layers, weight + bias gradients for each
			assertEquals(6, gradMap.Count)
			Dim expKeys() As String = { "0_" & DefaultParamInitializer.WEIGHT_KEY, "0_" & DefaultParamInitializer.BIAS_KEY, "1_" & DefaultParamInitializer.WEIGHT_KEY, "2_" & DefaultParamInitializer.BIAS_KEY, "2_" & DefaultParamInitializer.WEIGHT_KEY, "2_" & DefaultParamInitializer.BIAS_KEY }
			Dim keys As ISet(Of String) = gradMap.Keys
			For Each s As String In expKeys
				assertTrue(keys.Contains(s))
			Next s
	'        
	'        System.out.println(pair);
	'        
	'        //Use updater to go from raw gradients -> updates
	'        //Apply learning rate, gradient clipping, adagrad/momentum/rmsprop etc
	'        Updater updater = UpdaterCreator.getUpdater(net);
	'        updater.update(net, g, 0, miniBatch);
	'        
	'        StepFunction stepFunction = new NegativeGradientStepFunction();
	'        INDArray params = net.params();
	'        System.out.println(Arrays.toString(params.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 10)).dup().data().asFloat()));
	'        stepFunction.step(params, g.gradient());
	'        net.setParams(params);    //params() may not be in-place
	'        System.out.println(Arrays.toString(params.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 10)).dup().data().asFloat()));
	'        
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer Names") void testLayerNames()
		Friend Overridable Sub testLayerNames()
			Dim nIn As Integer = 10
			Dim nOut As Integer = 40
			Dim layerNameList As IList(Of String) = New List(Of String)()
			layerNameList.Add("dnn1")
			layerNameList.Add("dnn2")
			layerNameList.Add("dnn3")
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).list().layer(0, (New DenseLayer.Builder()).name("dnn1").nIn(nIn).nOut(20).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New DenseLayer.Builder()).name("dnn2").nIn(20).nOut(30).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(2, (New DenseLayer.Builder()).name("dnn3").nIn(30).nOut(nOut).activation(Activation.SOFTMAX).weightInit(WeightInit.XAVIER).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(layerNameList(0), net.getLayer(0).conf().getLayer().getLayerName())
			assertEquals(layerNameList, net.getLayerNames())
			Dim b As BaseLayer = CType(net.getLayer(layerNameList(2)).conf().getLayer(), BaseLayer)
			assertEquals(b.getActivationFn().ToString(), "softmax")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Score Examples") void testScoreExamples()
		Friend Overridable Sub testScoreExamples()
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).l2(0.01).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(20).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(30).build()).layer(2, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build()).build()
			Dim confNoReg As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(20).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(30).build()).layer(2, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim netNoReg As New MultiLayerNetwork(confNoReg)
			netNoReg.init()
			netNoReg.Parameters = net.params().dup()
			' Score single example, and compare to scoreExamples:
			Dim input As INDArray = Nd4j.rand(3, nIn)
			Dim output As INDArray = Nd4j.rand(3, nOut)
			Dim ds As New DataSet(input, output)
			Dim scoresWithRegularization As INDArray = net.scoreExamples(ds, True)
			Dim scoresNoRegularization As INDArray = net.scoreExamples(ds, False)
			assertArrayEquals(New Long() { 3, 1 }, scoresWithRegularization.shape())
			assertArrayEquals(New Long() { 3, 1 }, scoresNoRegularization.shape())
			For i As Integer = 0 To 2
				Dim singleEx As New DataSet(input.getRow(i, True), output.getRow(i, True))
				Dim score As Double = net.score(singleEx)
				Dim scoreNoReg As Double = netNoReg.score(singleEx)
				Dim scoreUsingScoreExamples As Double = scoresWithRegularization.getDouble(i)
				Dim scoreUsingScoreExamplesNoReg As Double = scoresNoRegularization.getDouble(i)
				assertEquals(score, scoreUsingScoreExamples, 1e-4)
				assertEquals(scoreNoReg, scoreUsingScoreExamplesNoReg, 1e-4)
				' Regularization term increases score
				assertTrue(scoreUsingScoreExamples > scoreUsingScoreExamplesNoReg)
				' System.out.println(score + "\t" + scoreUsingScoreExamples + "\t|\t" + scoreNoReg + "\t" + scoreUsingScoreExamplesNoReg);
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Data Set Score") void testDataSetScore()
		Friend Overridable Sub testDataSetScore()
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).activation(Activation.SIGMOID).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim [in] As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0, 4.0 }, New Long() { 1, 4 })
			Dim [out] As INDArray = Nd4j.create(New Double() { 1, 0, 0 }, New Long() { 1, 3 })
			Dim score As Double = net.score(New DataSet([in], [out]))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Data Set Score CNN") void testDataSetScoreCNN()
		Friend Overridable Sub testDataSetScoreCNN()
			Dim miniBatch As Integer = 3
			Dim depth As Integer = 2
			Dim width As Integer = 3
			Dim height As Integer = 3
			Dim nOut As Integer = 2
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder(2, 2)).nOut(1).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(2).build()).setInputType(InputType.convolutionalFlat(height, width, depth)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Nd4j.Random.setSeed(12345)
			Dim r As New Random(12345)
			Dim input As INDArray = Nd4j.rand(miniBatch, depth * width * height)
			Dim labels As INDArray = Nd4j.create(miniBatch, nOut)
			For i As Integer = 0 To miniBatch - 1
				labels.putScalar(New Integer() { i, r.Next(nOut) }, 1.0)
			Next i
			Dim score As Double = net.score(New DataSet(input, labels))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Predict") void testPredict() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPredict()
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(784).nOut(50).activation(Activation.RELU).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(50).nOut(10).build()).setInputType(InputType.convolutional(28, 28, 1)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim ds As DataSetIterator = New MnistDataSetIterator(10, 10)
			net.fit(ds)
			Dim testDs As DataSetIterator = New MnistDataSetIterator(1, 1)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim testData As DataSet = testDs.next()
			testData.LabelNames = New List(Of String) From {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}
			Dim actualLables As String = testData.getLabelName(0)
			Dim prediction As IList(Of String) = net.predict(testData)
			assertTrue(actualLables IsNot Nothing)
			assertTrue(prediction(0) IsNot Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @DisplayName("Test Cid") void testCid() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCid()
			Console.WriteLine(EnvironmentUtils.buildCId())
			Dim environment As Environment = EnvironmentUtils.buildEnvironment()
			environment.setSerialVersionID(EnvironmentUtils.buildCId())
			Dim task As Task = TaskUtils.buildTask(Nd4j.create(New Double() { 1, 2, 3, 4, 5, 6 }, New Long() { 1, 6 }))
			Heartbeat.Instance.reportEvent([Event].STANDALONE, environment, task)
			Thread.Sleep(25000)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Output") void testOutput() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testOutput()
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(784).nOut(50).activation(Activation.RELU).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(50).nOut(10).build()).setInputType(InputType.convolutional(28, 28, 1)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim fullData As DataSetIterator = New MnistDataSetIterator(1, 2)
			net.fit(fullData)
			fullData.reset()
			Dim expectedSet As DataSet = fullData.next(2)
			Dim expectedOut As INDArray = net.output(expectedSet.Features, False)
			fullData.reset()
			Dim actualOut As INDArray = net.output(fullData)
			assertEquals(expectedOut, actualOut)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient Update") void testGradientUpdate() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGradientUpdate()
			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 1)
			Dim expectedGradient As Gradient = New DefaultGradient()
			expectedGradient.setGradientFor("0_W", Nd4j.ones(4, 5).castTo(DataType.DOUBLE))
			expectedGradient.setGradientFor("0_b", Nd4j.ones(1, 5).castTo(DataType.DOUBLE))
			expectedGradient.setGradientFor("1_W", Nd4j.ones(5, 3).castTo(DataType.DOUBLE))
			expectedGradient.setGradientFor("1_b", Nd4j.ones(1, 3).castTo(DataType.DOUBLE))
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(1.0)).activation(Activation.RELU).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).name("dnn1").nIn(4).nOut(5).build()).layer(1, (New OutputLayer.Builder()).name("output").nIn(5).nOut(3).activation(Activation.SOFTMAX).weightInit(WeightInit.XAVIER).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			net.fit(iter.next())
			' TODO validate actual layer gradientView - issue getting var out of BaseLayer w/o adding MLN getter that gets confused with local gradient vars
			Dim actualGradient As Gradient = net.gradient_Conflict
			assertNotEquals(expectedGradient.getGradientFor("0_W"), actualGradient.getGradientFor("0_W"))
			net.update(expectedGradient)
			actualGradient = net.gradient_Conflict
			assertEquals(expectedGradient.getGradientFor("0_W"), actualGradient.getGradientFor("0_W"))
			' Update params with set
			net.setParam("0_W", Nd4j.ones(4, 5).castTo(DataType.DOUBLE))
			net.setParam("0_b", Nd4j.ones(1, 5).castTo(DataType.DOUBLE))
			net.setParam("1_W", Nd4j.ones(5, 3).castTo(DataType.DOUBLE))
			net.setParam("1_b", Nd4j.ones(1, 3).castTo(DataType.DOUBLE))
			Dim actualParams As INDArray = net.params().castTo(DataType.DOUBLE)
			' Confirm params
			assertEquals(expectedGradient.gradient(), actualParams)
			net.update(expectedGradient)
			actualParams = net.params().castTo(DataType.DOUBLE)
			assertEquals(Nd4j.ones(1, 43).addi(1).castTo(DataType.DOUBLE), actualParams)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn Invalid Data") void testCnnInvalidData()
		Friend Overridable Sub testCnnInvalidData()
			assertThrows(GetType(DL4JException), Sub()
			Dim miniBatch As Integer = 3
			Dim depth As Integer = 2
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).padding(0, 0).nIn(2).nOut(2).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(2).build()).setInputType(InputType.convolutional(height, width, depth)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim inputWrongDepth As INDArray = Nd4j.rand(New Integer() { miniBatch, 5, height, width })
			net.feedForward(inputWrongDepth)
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Applying Pre Train Config And Params") void testApplyingPreTrainConfigAndParams()
		Friend Overridable Sub testApplyingPreTrainConfigAndParams()
			Dim nIn As Integer = 10
			Dim nOut As Integer = 10
			' Test pretrain true
			Dim aePre As MultiLayerNetwork = getAeModel(True, nIn, nOut)
			Dim actualNP As Integer = CInt(aePre.numParams())
			assertEquals(2 * (nIn * nOut + nOut) + nIn, actualNP)
			Dim params As INDArray = aePre.params()
			' check num params
			assertEquals(params.length(), actualNP)
			Dim paramTable As IDictionary(Of String, INDArray) = aePre.paramTable()
			' check vb exists for pretrain layer
			assertTrue(paramTable.ContainsKey("0_vb"))
			aePre.setParam("0_vb", Nd4j.ones(10))
			params = aePre.getParam("0_vb")
			' check set params for vb
			assertEquals(Nd4j.ones(1, 10), params)
			' Test pretrain false, expect same for true because its not changed when applying update
			Dim aeNoPre As MultiLayerNetwork = getAeModel(False, nIn, nOut)
			actualNP = CInt(aeNoPre.numParams())
			assertEquals(2 * (nIn * nOut + nOut) + nIn, actualNP)
			params = aeNoPre.params()
			assertEquals(params.length(), actualNP)
			paramTable = aePre.paramTable()
			assertTrue(paramTable.ContainsKey("0_vb"))
		End Sub

		Public Overridable Function getAeModel(ByVal preTrain As Boolean, ByVal nIn As Integer, ByVal nOut As Integer) As MultiLayerNetwork
			Dim vae As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(42).updater(New NoOp()).weightInit(WeightInit.UNIFORM).list((New AutoEncoder.Builder()).activation(Activation.IDENTITY).nOut(nIn).build(), (New OutputLayer.Builder(LossFunctions.LossFunction.COSINE_PROXIMITY)).activation(Activation.IDENTITY).nOut(nOut).build()).setInputType(InputType.feedForward(nOut)).build()
			Dim network As New MultiLayerNetwork(vae)
			network.init()
			Return network
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Iteration Count And Persistence") void testIterationCountAndPersistence() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testIterationCountAndPersistence()
			Nd4j.Random.setSeed(123)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Dim iter As DataSetIterator = New IrisDataSetIterator(50, 150)
			assertEquals(0, network.LayerWiseConfigurations.getIterationCount())
			network.fit(iter)
			assertEquals(3, network.LayerWiseConfigurations.getIterationCount())
			iter.reset()
			network.fit(iter)
			assertEquals(6, network.LayerWiseConfigurations.getIterationCount())
			iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			network.fit(iter.next())
			assertEquals(7, network.LayerWiseConfigurations.getIterationCount())
			Dim baos As New MemoryStream()
			ModelSerializer.writeModel(network, baos, True)
			Dim asBytes() As SByte = baos.toByteArray()
			Dim bais As New MemoryStream(asBytes)
			Dim net As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(bais, True)
			assertEquals(7, net.LayerWiseConfigurations.getIterationCount())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Bias L 1 L 2") void testBiasL1L2()
		Friend Overridable Sub testBiasL1L2()
			Nd4j.Random.setSeed(123)
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).activation(Activation.TANH).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(10).nOut(10).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).l1Bias(0.1).l2Bias(0.2).weightInit(WeightInit.XAVIER).activation(Activation.TANH).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(10).nOut(10).build()).build()
			Dim net1 As New MultiLayerNetwork(conf1)
			net1.init()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			Dim bl0 As BaseLayer = CType(net2.getLayer(0).conf().getLayer(), BaseLayer)
			assertEquals(0.1, TestUtils.getL1(bl0.getRegularizationBias()), 1e-6)
			assertEquals(0.2, TestUtils.getL2(bl0.getRegularizationBias()), 1e-6)
			Dim features As INDArray = Nd4j.rand(10, 10)
			Dim labels As INDArray = Nd4j.rand(10, 10)
			net2.Params = net1.params().dup()
			net1.Input = features
			net1.Labels = labels
			net2.Input = features
			net2.Labels = labels
			net1.computeGradientAndScore()
			net2.computeGradientAndScore()
			Dim r As Double = net1.calcRegularizationScore(True)
			assertEquals(0.0, r, 0.0)
			r = net2.calcRegularizationScore(True)
			assertEquals(0.0, r, 0.0)
			Dim s1 As Double = net1.score()
			Dim s2 As Double = net2.score()
			' Biases initialized to 0 -> should initially have same score
			assertEquals(s1, s2, 1e-6)
			For i As Integer = 0 To 9
				net1.fit(features, labels)
			Next i
			net2.Params = net1.params().dup()
			net1.computeGradientAndScore()
			net2.computeGradientAndScore()
			r = net1.calcRegularizationScore(True)
			assertEquals(0.0, r, 0.0)
			r = net2.calcRegularizationScore(True)
			assertTrue(r > 0.0)
			s1 = net1.score()
			s2 = net2.score()
			' Scores should differ due to bias l1/l2
			assertNotEquals(s1, s2, 1e-6)
			For i As Integer = 0 To 1
				assertEquals(0.0, net1.getLayer(i).calcRegularizationScore(True), 0.0)
				assertTrue(net2.getLayer(i).calcRegularizationScore(True) > 0.0)
			Next i
		End Sub

	'    
	'        Summary should pick up preprocessors set manually on inputs as well
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Summary") void testSummary()
		Friend Overridable Sub testSummary()
			Dim V_WIDTH As Integer = 130
			Dim V_HEIGHT As Integer = 130
			Dim V_NFRAMES As Integer = 150
			Dim confForArchitecture As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l2(0.001).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New ConvolutionLayer.Builder(10, 10)).nIn(3).nOut(30).stride(4, 4).activation(Activation.RELU).weightInit(WeightInit.RELU).updater(Updater.ADAGRAD).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3, 3).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(3, 3)).nIn(30).nOut(10).stride(2, 2).activation(Activation.RELU).weightInit(WeightInit.RELU).updater(Updater.ADAGRAD).build()).layer(3, (New DenseLayer.Builder()).activation(Activation.RELU).nIn(490).nOut(50).weightInit(WeightInit.RELU).updater(Updater.ADAGRAD).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).layer(4, (New GravesLSTM.Builder()).activation(Activation.SOFTSIGN).nIn(50).nOut(50).weightInit(WeightInit.XAVIER).updater(Updater.ADAGRAD).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).layer(5, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(50).nOut(4).updater(Updater.ADAGRAD).weightInit(WeightInit.XAVIER).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).inputPreProcessor(0, New RnnToCnnPreProcessor(V_HEIGHT, V_WIDTH, 3)).inputPreProcessor(3, New CnnToFeedForwardPreProcessor(7, 7, 10)).inputPreProcessor(4, New FeedForwardToRnnPreProcessor()).backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(V_NFRAMES \ 5).tBPTTBackwardLength(V_NFRAMES \ 5).build()
			Dim modelExpectedArch As New MultiLayerNetwork(confForArchitecture)
			modelExpectedArch.init()
			Dim modelMow As MultiLayerNetwork = (New TransferLearning.Builder(modelExpectedArch)).setFeatureExtractor(2).build()
			' System.out.println(modelExpectedArch.summary());
			' System.out.println(modelMow.summary());
			' System.out.println(modelMow.summary(InputType.recurrent(V_HEIGHT*V_WIDTH*3)));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Error No Output Layer") void testErrorNoOutputLayer()
		Friend Overridable Sub testErrorNoOutputLayer()
			assertThrows(GetType(DL4JException), Sub()
			Dim c As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).build()
			Dim net As New MultiLayerNetwork(c)
			net.init()
			Dim f As INDArray = Nd4j.create(1, 10)
			Dim l As INDArray = Nd4j.create(1, 10)
			net.Input = f
			net.Labels = l
			net.computeGradientAndScore()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Param Table") void testSetParamTable()
		Friend Overridable Sub testSetParamTable()
			Nd4j.Random.setSeed(123)
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(2, (New LSTM.Builder()).nIn(2).nOut(2).build()).layer(3, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(2).nOut(3).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(987).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(2, (New LSTM.Builder()).nIn(2).nOut(2).build()).layer(3, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(2).nOut(3).build()).build()
			Dim net1 As New MultiLayerNetwork(conf1)
			net1.init()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			assertNotEquals(net1.params(), net2.params())
			assertNotEquals(net1.paramTable(), net2.paramTable())
			net1.ParamTable = net2.paramTable()
			assertEquals(net1.params(), net2.params())
			assertEquals(net1.paramTable(), net2.paramTable())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Compare Layer Methods") void testCompareLayerMethods()
		Friend Overridable Sub testCompareLayerMethods()
			' Simple test: compare .layer(int, Layer) and .layer(Layer) are identical
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(2, (New LSTM.Builder()).nIn(2).nOut(2).build()).layer(3, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(2).nOut(3).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer((New DenseLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer((New DenseLayer.Builder()).nIn(3).nOut(2).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer((New LSTM.Builder()).nIn(2).nOut(2).build()).layer((New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(2).nOut(3).build()).build()
			assertEquals(conf1, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Epoch Counter") void testEpochCounter() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEpochCounter()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(0, net.LayerWiseConfigurations.EpochCount)
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			For i As Integer = 0 To 3
				assertEquals(i, net.LayerWiseConfigurations.EpochCount)
				net.fit(iter)
				assertEquals(i + 1, net.LayerWiseConfigurations.EpochCount)
			Next i
			assertEquals(4, net.LayerWiseConfigurations.EpochCount)
			Dim restored As MultiLayerNetwork = TestUtils.testModelSerialization(net)
			assertEquals(4, restored.LayerWiseConfigurations.EpochCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Clearance") void testInputClearance() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInputClearance()
			' Activations should be cleared - if not, it's possible for out of (workspace) scope arrays to be around
			' which can cause a crash
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).list().layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nIn(1).nOut(1).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(1, 1).build()).layer((New DenseLayer.Builder()).nOut(10).build()).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim content As INDArray = Nd4j.create(1, 1, 28, 28)
			' Check output:
			net.output(content)
			For Each l As org.deeplearning4j.nn.api.Layer In net.Layers
				assertNull(l.input())
			Next l
			' Check feedForward:
			net.feedForward(content, False)
			For Each l As org.deeplearning4j.nn.api.Layer In net.Layers
				assertNull(l.input())
			Next l
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test External Errors") void testExternalErrors()
		Friend Overridable Sub testExternalErrors()
			' Simple test: same network, but in one case: one less layer (the OutputLayer), where the epsilons are passed in externally
			' instead. Should get identical results
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("Workspace mode: " & ws)
				Nd4j.Random.setSeed(12345)
				Dim inData As INDArray = Nd4j.rand(3, 10)
				Dim outData As INDArray = Nd4j.rand(3, 10)
				Nd4j.Random.setSeed(12345)
				Dim standard As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).seed(12345).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build()).build()
				Dim s As New MultiLayerNetwork(standard)
				s.init()
				Nd4j.Random.setSeed(12345)
				Dim external As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).seed(12345).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).build()
				Dim e As New MultiLayerNetwork(external)
				e.init()
				s.Input = inData
				s.Labels = outData
				s.computeGradientAndScore()
				Dim sGrad As Gradient = s.gradient()
				s.Input = inData
				' FF without clearing inputs as we need them later
				s.feedForward(True, False)
				e.Input = inData
				' FF without clearing inputs as we need them later
				e.feedForward(True, False)
				Dim ol As org.deeplearning4j.nn.layers.OutputLayer = DirectCast(s.getLayer(1), org.deeplearning4j.nn.layers.OutputLayer)
				Dim olPairStd As Pair(Of Gradient, INDArray) = ol.backpropGradient(Nothing, LayerWorkspaceMgr.noWorkspaces())
				Dim olEpsilon As INDArray = olPairStd.Second.detach()
				e.Input = inData
				e.feedForward(True, False)
				Dim extErrorGrad As Pair(Of Gradient, INDArray) = e.backpropGradient(olEpsilon, LayerWorkspaceMgr.noWorkspaces())
				Dim nParamsDense As Integer = 10 * 10 + 10
				assertEquals(sGrad.gradient().get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(0, nParamsDense)), extErrorGrad.First.gradient())
				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test External Errors 2") void testExternalErrors2()
		Friend Overridable Sub testExternalErrors2()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
			Dim nIn As Integer = 4
			Dim nOut As Integer = 3
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				' System.out.println("***** WORKSPACE: " + ws);
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.01)).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).list().layer((New DenseLayer.Builder()).nIn(nIn).nOut(nOut).activation(Activation.RELU).build()).layer((New ActivationLayer.Builder()).activation(Activation.IDENTITY).build()).inputPreProcessor(0, New RnnToFeedForwardPreProcessor()).inputPreProcessor(1, New FeedForwardToRnnPreProcessor()).build()
				Dim graph As New MultiLayerNetwork(conf)
				graph.init()
				Const minibatch As Integer = 5
				Const seqLen As Integer = 6
				Dim param As INDArray = Nd4j.create(New Double() { 0.54, 0.31, 0.98, -0.30, -0.66, -0.19, -0.29, -0.62, 0.13, -0.32, 0.01, -0.03, 0.00, 0.00, 0.00 }).reshape(ChrW(1), -1)
				graph.Params = param
				Dim input As INDArray = Nd4j.rand(New Integer() { minibatch, nIn, seqLen }, 12)
				Dim expected As INDArray = Nd4j.ones(minibatch, nOut, seqLen)
				graph.Input = input
				Dim output As INDArray = graph.feedForward(False, False)(2)
				Dim [error] As INDArray = output.sub(expected)
				For Each l As org.deeplearning4j.nn.api.Layer In graph.Layers
					assertNotNull(l.input())
					assertFalse(l.input().Attached)
				Next l
				' Compute Gradient
				Dim gradient As Pair(Of Gradient, INDArray) = graph.backpropGradient([error], LayerWorkspaceMgr.noWorkspaces())
				graph.Updater.update(graph, gradient.First, 0, 0, minibatch, LayerWorkspaceMgr.noWorkspaces())
				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Next ws
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.DISABLED
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer Size") void testLayerSize()
		Friend Overridable Sub testLayerSize()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).nOut(6).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).build()).layer((New DenseLayer.Builder()).nOut(30).build()).layer((New OutputLayer.Builder()).nOut(13).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 3)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(6, net.layerSize(0))
			assertEquals(0, net.layerSize(1))
			assertEquals(30, net.layerSize(2))
			assertEquals(13, net.layerSize(3))
			assertEquals(3, net.layerInputSize(0))
			assertEquals(0, net.layerInputSize(1))
			assertEquals(CType(net.getLayer(2).conf().getLayer(), FeedForwardLayer).getNIn(), net.layerInputSize(2))
			assertEquals(30, net.layerInputSize(3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Zero Param Net") void testZeroParamNet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testZeroParamNet()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(2, 2).build()).layer((New LossLayer.Builder()).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.MSE).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = (New MnistDataSetIterator(16, True, 12345)).next()
			Dim [out] As INDArray = net.output(ds.Features)
			Dim labelTemp As INDArray = Nd4j.create([out].shape())
			ds.Labels = labelTemp
			net.fit(ds)
			Dim net2 As MultiLayerNetwork = TestUtils.testModelSerialization(net)
			Dim out2 As INDArray = net2.output(ds.Features)
			assertEquals([out], out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Activation Gradient") void testInputActivationGradient()
		Friend Overridable Sub testInputActivationGradient()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).activation(Activation.TANH).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim [in] As INDArray = Nd4j.rand(1, 10).castTo(DataType.DOUBLE)
			Dim label As INDArray = Nd4j.rand(1, 10).castTo(DataType.DOUBLE)
			Dim p As Pair(Of Gradient, INDArray) = net.calculateGradients([in], label, Nothing, Nothing)
			' Quick gradient check:
			Dim eps As Double = 1e-6
			Dim maxRelError As Double = 1e-5
			For i As Integer = 0 To 9
				Dim orig As Double = [in].getDouble(i)
				[in].putScalar(i, orig + eps)
				Dim scorePlus As Double = net.score(New DataSet([in], label))
				[in].putScalar(i, orig - eps)
				Dim scoreMinus As Double = net.score(New DataSet([in], label))
				[in].putScalar(i, orig)
				Dim expGrad As Double = (scorePlus - scoreMinus) / (2.0 * eps)
				Dim actGrad As Double = p.Second.getDouble(i)
				Dim relError As Double = (Math.Abs(expGrad - actGrad)) / (Math.Abs(expGrad) + Math.Abs(actGrad))
				Dim str As String = i & " - " & relError & " - exp=" & expGrad & ", act=" & actGrad
				assertTrue(relError < maxRelError,str)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Layer Configuration Activation Types") void testMultiLayerConfigurationActivationTypes()
		Friend Overridable Sub testMultiLayerConfigurationActivationTypes()
			Dim builder As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).list().layer((New LSTM.Builder()).nOut(6).build()).layer((New LSTM.Builder()).nOut(7).build()).layer(New GlobalPoolingLayer()).layer((New OutputLayer.Builder()).nOut(8).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(10))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim outBuilder As IList(Of InputType) = builder.getLayerActivationTypes()
			Dim outConf As IList(Of InputType) = conf.getLayerActivationTypes(InputType.recurrent(10))
			Dim exp As IList(Of InputType) = New List(Of InputType) From {InputType.recurrent(6), InputType.recurrent(7), InputType.recurrent(7), InputType.feedForward(8,RNNFormat.NCW)}
			assertEquals(exp, outBuilder)
			assertEquals(exp, outConf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multiple Epochs Simple") void testMultipleEpochsSimple()
		Friend Overridable Sub testMultipleEpochsSimple()
			' Mainly a simple sanity check on the preconditions in the method...
			Dim iter As DataSetIterator = New IrisDataSetIterator(10, 150)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.fit(iter, 3)
			Dim g As ComputationGraph = net.toComputationGraph()
			g.fit(iter, 3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Pretrain Fit Methods") void testPretrainFitMethods()
		Friend Overridable Sub testPretrainFitMethods()
			' The fit methods should *not* do layerwise pretraining:
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New VariationalAutoencoder.Builder()).nIn(10).nOut(10).encoderLayerSizes(10).decoderLayerSizes(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim exp As ISet(Of Type) = New HashSet(Of Type)()
			exp.Add(GetType(MultiLayerNetwork))
			Dim listener As New CheckModelsListener()
			net.setListeners(listener)
			Dim f As INDArray = Nd4j.create(1, 10)
			Dim l As INDArray = Nd4j.create(1, 10)
			Dim ds As New DataSet(f, l)
			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(f, l)
			Dim iter As DataSetIterator = New ExistingDataSetIterator(Collections.singletonList(ds))
			net.fit(iter)
			assertEquals(exp, listener.getModelClasses())
			net.fit(ds)
			assertEquals(exp, listener.getModelClasses())
			net.fit(f, l)
			assertEquals(exp, listener.getModelClasses())
			net.fit(f, l, Nothing, Nothing)
			assertEquals(exp, listener.getModelClasses())
			net.fit(mds)
			assertEquals(exp, listener.getModelClasses())
			net.fit(New SingletonMultiDataSetIterator(mds))
			assertEquals(exp, listener.getModelClasses())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test IND Array Config Cloning") void testINDArrayConfigCloning()
		Friend Overridable Sub testINDArrayConfigCloning()
			' INDArrays in config should be cloned to avoid threading issues
			Dim mb As Integer = 3
			Dim b As Integer = 4
			Dim c As Integer = 3
			Dim depth As Integer = b * (5 + c)
			Dim w As Integer = 6
			Dim h As Integer = 6
			Dim bbPrior As INDArray = Nd4j.rand(b, 2).muliRowVector(Nd4j.create(New Double() { w, h }).castTo(Nd4j.defaultFloatingPointType()))
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.01).list().layer((New ConvolutionLayer.Builder()).nIn(depth).nOut(depth).kernelSize(1, 1).build()).layer((New Yolo2OutputLayer.Builder()).boundingBoxPriors(bbPrior).build()).build()
			Dim conf2 As MultiLayerConfiguration = conf.clone()
			Dim bb1 As INDArray = CType(conf.getConf(1).getLayer(), Yolo2OutputLayer).getBoundingBoxes().castTo(Nd4j.defaultFloatingPointType())
			Dim bb2 As INDArray = CType(conf2.getConf(1).getLayer(), Yolo2OutputLayer).getBoundingBoxes().castTo(Nd4j.defaultFloatingPointType())
			assertFalse(bb1 Is bb2)
			assertEquals(bb1, bb2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @DisplayName("Check Models Listener") public static class CheckModelsListener extends org.deeplearning4j.optimize.api.BaseTrainingListener
		Public Class CheckModelsListener
			Inherits BaseTrainingListener

			Friend modelClasses As ISet(Of Type) = New HashSet(Of Type)()

			Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
				modelClasses.Add(model.GetType())
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLN Updater Blocks") void testMLNUpdaterBlocks()
		Friend Overridable Sub testMLNUpdaterBlocks()
			' Check that setting learning rate results in correct rearrangement of updater state within updater blocks
			' https://github.com/eclipse/deeplearning4j/issues/6809#issuecomment-463892644
			Dim lr As Double = 1e-3
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).updater(New Adam(lr)).list().layer((New DenseLayer.Builder()).nIn(5).nOut(3).build()).layer((New DenseLayer.Builder()).nIn(3).nOut(2).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.XENT)).nIn(2).nOut(1).activation(Activation.SIGMOID).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim [in] As INDArray = Nd4j.rand(1, 5)
			Dim lbl As INDArray = Nd4j.rand(1, 1)
			net.fit(New DataSet([in], lbl))
			Dim viewArray As INDArray = net.Updater.StateViewArray
			Dim viewArrayCopy As INDArray = viewArray.dup()
			' Initially updater view array is set out like:
			' [m0w, m0b, m1w, m1b, m2w, m2b][v0w, v0b, v1w, v1b, v2w, v2b]
			Dim soFar As Long = 0
			' m0w
			Dim m0w As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 5 * 3)).assign(0)
			soFar += 5 * 3
			' m0b
			Dim m0b As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 3)).assign(1)
			soFar += 3
			' m1w
			Dim m1w As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 3 * 2)).assign(2)
			soFar += 3 * 2
			' m1b
			Dim m1b As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 2)).assign(3)
			soFar += 2
			' m2w
			Dim m2w As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 2 * 1)).assign(4)
			soFar += 2 * 1
			' m2b
			Dim m2b As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 1)).assign(5)
			soFar += 1
			' v0w
			Dim v0w As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 5 * 3)).assign(6)
			soFar += 5 * 3
			' v0b
			Dim v0b As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 3)).assign(7)
			soFar += 3
			' v1w
			Dim v1w As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 3 * 2)).assign(8)
			soFar += 3 * 2
			' v1b
			Dim v1b As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 2)).assign(9)
			soFar += 2
			' v2w
			Dim v2w As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 2 * 1)).assign(10)
			soFar += 2 * 1
			' v2b
			Dim v2b As INDArray = viewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + 1)).assign(11)
			soFar += 1
			net.setLearningRate(0, 0.0)
			' Expect new updater state to look like:
			' [m0w, m0b][v0w,v0b], [m1w, m1b, m2w, m2b][v1w, v1b, v2w, v2b]
			Dim exp As INDArray = Nd4j.concat(1, m0w, m0b, v0w, v0b, m1w, m1b, m2w, m2b, v1w, v1b, v2w, v2b)
			Dim act As INDArray = net.Updater.StateViewArray
			' System.out.println(exp);
			' System.out.println(act);
			assertEquals(exp, act)
			' And set layer 1 LR:
			net.setLearningRate(1, 0.2)
			exp = Nd4j.concat(1, m0w, m0b, v0w, v0b, m1w, m1b, v1w, v1b, m2w, m2b, v2w, v2b)
			assertEquals(exp, net.Updater.StateViewArray)
			' Set all back to original LR and check again:
			net.setLearningRate(1, lr)
			net.setLearningRate(0, lr)
			exp = Nd4j.concat(1, m0w, m0b, m1w, m1b, m2w, m2b, v0w, v0b, v1w, v1b, v2w, v2b)
			assertEquals(exp, net.Updater.StateViewArray)
			' Finally, training sanity check (if things are wrong, we get -ve values in adam V, which causes NaNs)
			net.Updater.StateViewArray.assign(viewArrayCopy)
			net.setLearningRate(0, 0.0)
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.NAN_PANIC
			net.fit(New DataSet([in], lbl))
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub
	End Class

End Namespace