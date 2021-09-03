Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Convolution1DLayer = org.deeplearning4j.nn.conf.layers.Convolution1DLayer
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports org.deeplearning4j.nn.conf.layers
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitNormal = org.deeplearning4j.nn.weights.WeightInitNormal
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports RnnDataFormat = org.nd4j.enums.RnnDataFormat
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Convolution = org.nd4j.linalg.convolution.Convolution
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
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
Namespace org.deeplearning4j.nn.layers.convolution

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Convolution Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) class ConvolutionLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ConvolutionLayerTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Twd First Layer") void testTwdFirstLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testTwdFirstLayer()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).l2(2e-4).updater(New Nesterovs(0.9)).dropOut(0.5).list().layer(0, (New ConvolutionLayer.Builder(8, 8)).stride(4, 4).nOut(16).dropOut(0.5).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).nOut(32).dropOut(0.5).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(2, (New DenseLayer.Builder()).nOut(256).activation(Activation.RELU).weightInit(WeightInit.XAVIER).dropOut(0.5).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)).nOut(10).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1))
			Dim iter As DataSetIterator = New MnistDataSetIterator(10, 10)
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			For i As Integer = 0 To 4
				network.fit(ds)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN Sub Combo With Mixed HW") void testCNNSubComboWithMixedHW()
		Friend Overridable Sub testCNNSubComboWithMixedHW()
			Dim imageHeight As Integer = 20
			Dim imageWidth As Integer = 23
			Dim nChannels As Integer = 1
			Dim classes As Integer = 2
			Dim numSamples As Integer = 200
			Dim kernelHeight As Integer = 3
			Dim kernelWidth As Integer = 3
			Dim trainInput As DataSet
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New ConvolutionLayer.Builder(kernelHeight, kernelWidth)).stride(1, 1).nOut(2).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(imageHeight - kernelHeight, 1).stride(1, 1).build()).layer(2, (New OutputLayer.Builder()).nOut(classes).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(imageHeight, imageWidth, nChannels))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim emptyFeatures As INDArray = Nd4j.zeros(numSamples, imageWidth * imageHeight * nChannels)
			Dim emptyLables As INDArray = Nd4j.zeros(numSamples, classes)
			trainInput = New DataSet(emptyFeatures, emptyLables)
			model.fit(trainInput)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Causal 1 d") void testCausal1d()
		Friend Overridable Sub testCausal1d()
			Nd4j.Environment.Verbose = True
			Nd4j.Environment.Debug = True
			' See: Fixes: https://github.com/eclipse/deeplearning4j/issues/9060
			Dim learningRate As Double = 1e-3
			Dim seed As Long = 123
			Dim timeSteps As Long = 72
			Dim vectorLength As Long = 64
			Dim batchSize As Long = 1
			Dim arr As INDArray = Nd4j.randn(batchSize, vectorLength, timeSteps)
			Dim build As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).activation(Activation.RELU).weightInit(New WeightInitNormal()).updater(New Adam(learningRate)).list().layer((New Convolution1D.Builder()).kernelSize(2).rnnDataFormat(RNNFormat.NCW).stride(1).nOut(14).convolutionMode(ConvolutionMode.Causal).dilation(4).build()).layer((New RnnLossLayer.Builder()).dataFormat(RNNFormat.NCW).activation(New ActivationSoftmax()).lossFunction(New LossMCXENT()).build()).setInputType(InputType.recurrent(vectorLength, timeSteps, RNNFormat.NCW)).build()
			Dim network As New MultiLayerNetwork(build)
			network.init()
			Dim output As INDArray = network.output(arr)
			assertArrayEquals(New Long() { 1, 14, 72 }, output.shape())
			Console.WriteLine(output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN Too Large Kernel") void testCNNTooLargeKernel()
		Friend Overridable Sub testCNNTooLargeKernel()
			assertThrows(GetType(DL4JException), Sub()
			Dim imageHeight As Integer = 20
			Dim imageWidth As Integer = 23
			Dim nChannels As Integer = 1
			Dim classes As Integer = 2
			Dim numSamples As Integer = 200
			Dim kernelHeight As Integer = imageHeight
			Dim kernelWidth As Integer = imageWidth + 1
			Dim trainInput As DataSet
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New ConvolutionLayer.Builder(kernelHeight, kernelWidth)).stride(1, 1).nOut(2).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder()).nOut(classes).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(imageHeight, imageWidth, nChannels))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim emptyFeatures As INDArray = Nd4j.zeros(numSamples, imageWidth * imageHeight * nChannels)
			Dim emptyLables As INDArray = Nd4j.zeros(numSamples, classes)
			trainInput = New DataSet(emptyFeatures, emptyLables)
			model.fit(trainInput)
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN Zero Stride") void testCNNZeroStride()
		Friend Overridable Sub testCNNZeroStride()
			assertThrows(GetType(Exception), Sub()
			Dim imageHeight As Integer = 20
			Dim imageWidth As Integer = 23
			Dim nChannels As Integer = 1
			Dim classes As Integer = 2
			Dim numSamples As Integer = 200
			Dim kernelHeight As Integer = imageHeight
			Dim kernelWidth As Integer = imageWidth
			Dim trainInput As DataSet
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New ConvolutionLayer.Builder(kernelHeight, kernelWidth)).stride(1, 0).nOut(2).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder()).nOut(classes).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(imageHeight, imageWidth, nChannels))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim emptyFeatures As INDArray = Nd4j.zeros(numSamples, imageWidth * imageHeight * nChannels)
			Dim emptyLables As INDArray = Nd4j.zeros(numSamples, classes)
			trainInput = New DataSet(emptyFeatures, emptyLables)
			model.fit(trainInput)
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN Bias Init") void testCNNBiasInit()
		Friend Overridable Sub testCNNBiasInit()
			Dim cnn As ConvolutionLayer = (New ConvolutionLayer.Builder()).nIn(1).nOut(3).biasInit(1).build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer(cnn).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			assertEquals(1, layer.getParam("b").size(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN Input Setup MNIST") void testCNNInputSetupMNIST() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCNNInputSetupMNIST()
			Dim input As INDArray = MnistData
			Dim layer As Layer = MNISTConfig
			layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(input, layer.input())
			assertArrayEquals(input.shape(), layer.input().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Feature Map Shape MNIST") void testFeatureMapShapeMNIST() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testFeatureMapShapeMNIST()
			Dim inputWidth As Integer = 28
			Dim stride() As Integer = { 1, 1 }
			Dim padding() As Integer = { 0, 0 }
			Dim kernelSize() As Integer = { 9, 9 }
			Dim nChannelsIn As Integer = 1
			Dim depth As Integer = 20
			Dim featureMapWidth As Integer = (inputWidth + padding(1) * 2 - kernelSize(1)) \ stride(1) + 1
			Dim input As INDArray = MnistData
			Dim layer As Layer = getCNNConfig(nChannelsIn, depth, kernelSize, stride, padding)
			Dim convActivations As INDArray = layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(featureMapWidth, convActivations.size(2))
			assertEquals(depth, convActivations.size(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Activate Results Contained") void testActivateResultsContained()
		Friend Overridable Sub testActivateResultsContained()
			Dim layer As Layer = ContainedConfig
			Dim input As INDArray = ContainedData
			Dim expectedOutput As INDArray = Nd4j.create(New Single() { 0.98201379f, 0.98201379f, 0.98201379f, 0.98201379f, 0.99966465f, 0.99966465f, 0.99966465f, 0.99966465f, 0.98201379f, 0.98201379f, 0.98201379f, 0.98201379f, 0.99966465f, 0.99966465f, 0.99966465f, 0.99966465f, 0.98201379f, 0.98201379f, 0.98201379f, 0.98201379f, 0.99966465f, 0.99966465f, 0.99966465f, 0.99966465f, 0.98201379f, 0.98201379f, 0.98201379f, 0.98201379f, 0.99966465f, 0.99966465f, 0.99966465f, 0.99966465f }, New Integer() { 1, 2, 4, 4 })
			Dim convActivations As INDArray = layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(expectedOutput.shape(), convActivations.shape())
			assertEquals(expectedOutput, convActivations)
		End Sub

		' ////////////////////////////////////////////////////////////////////////////////
		Private Shared Function getCNNConfig(ByVal nIn As Integer, ByVal nOut As Integer, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer) As Layer
			Dim layer As ConvolutionLayer = (New ConvolutionLayer.Builder(kernelSize, stride, padding)).nIn(nIn).nOut(nOut).activation(Activation.SIGMOID).build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer(layer).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Return conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
		End Function

		Public Overridable ReadOnly Property MNISTConfig As Layer
			Get
				Dim kernelSize() As Integer = { 9, 9 }
				Dim stride() As Integer = { 1, 1 }
				Dim padding() As Integer = { 1, 1 }
				Dim nChannelsIn As Integer = 1
				Dim depth As Integer = 20
				Return getCNNConfig(nChannelsIn, depth, kernelSize, stride, padding)
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray getMnistData() throws Exception
		Public Overridable ReadOnly Property MnistData As INDArray
			Get
				Dim inputWidth As Integer = 28
				Dim inputHeight As Integer = 28
				Dim nChannelsIn As Integer = 1
				Dim nExamples As Integer = 5
				Dim data As DataSetIterator = New MnistDataSetIterator(nExamples, nExamples)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim mnist As DataSet = data.next()
				nExamples = mnist.numExamples()
				Return mnist.Features.reshape(ChrW(nExamples), nChannelsIn, inputHeight, inputWidth)
			End Get
		End Property

		Public Overridable ReadOnly Property ContainedConfig As Layer
			Get
				Dim kernelSize() As Integer = { 2, 2 }
				Dim stride() As Integer = { 2, 2 }
				Dim padding() As Integer = { 0, 0 }
				Dim nChannelsIn As Integer = 1
				Dim depth As Integer = 2
				Dim W As INDArray = Nd4j.create(New Double() { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 }, New Integer() { 2, 1, 2, 2 })
				Dim b As INDArray = Nd4j.create(New Double() { 1, 1 })
				Dim layer As Layer = getCNNConfig(nChannelsIn, depth, kernelSize, stride, padding)
				layer.setParam("W", W)
				layer.setParam("b", b)
				Return layer
			End Get
		End Property

		Public Overridable ReadOnly Property ContainedData As INDArray
			Get
				Dim ret As INDArray = Nd4j.create(New Single() { 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4 }, New Integer() { 1, 1, 8, 8 })
				Return ret
			End Get
		End Property

		Public Overridable ReadOnly Property ContainedCol As INDArray
			Get
				Return Nd4j.create(New Single() { 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4 }, New Integer() { 1, 1, 2, 2, 4, 4 })
			End Get
		End Property

		' ////////////////////////////////////////////////////////////////////////////////
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNNMLN Pretrain") void testCNNMLNPretrain() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCNNMLNPretrain()
			' Note CNN does not do pretrain
			Dim numSamples As Integer = 10
			Dim batchSize As Integer = 10
			Dim mnistIter As DataSetIterator = New MnistDataSetIterator(batchSize, numSamples, True)
			Dim model As MultiLayerNetwork = getCNNMLNConfig(False, True)
			model.fit(mnistIter)
			mnistIter.reset()
			Dim model2 As MultiLayerNetwork = getCNNMLNConfig(False, True)
			model2.fit(mnistIter)
			mnistIter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim test As DataSet = mnistIter.next()
			Dim eval As New Evaluation()
			Dim output As INDArray = model.output(test.Features)
			eval.eval(test.Labels, output)
			Dim f1Score As Double = eval.f1()
			Dim eval2 As New Evaluation()
			Dim output2 As INDArray = model2.output(test.Features)
			eval2.eval(test.Labels, output2)
			Dim f1Score2 As Double = eval2.f1()
			assertEquals(f1Score, f1Score2, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNNMLN Backprop") void testCNNMLNBackprop() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCNNMLNBackprop()
			Dim numSamples As Integer = 10
			Dim batchSize As Integer = 10
			Dim mnistIter As DataSetIterator = New MnistDataSetIterator(batchSize, numSamples, True)
			Dim model As MultiLayerNetwork = getCNNMLNConfig(True, False)
			model.fit(mnistIter)
			Dim model2 As MultiLayerNetwork = getCNNMLNConfig(True, False)
			model2.fit(mnistIter)
			mnistIter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim test As DataSet = mnistIter.next()
			Dim eval As New Evaluation()
			Dim output As INDArray = model.output(test.Features)
			eval.eval(test.Labels, output)
			Dim f1Score As Double = eval.f1()
			Dim eval2 As New Evaluation()
			Dim output2 As INDArray = model2.output(test.Features)
			eval2.eval(test.Labels, output2)
			Dim f1Score2 As Double = eval2.f1()
			assertEquals(f1Score, f1Score2, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Set Params") void testGetSetParams()
		Friend Overridable Sub testGetSetParams()
			Dim net As MultiLayerNetwork = getCNNMLNConfig(True, False)
			Dim paramsOrig As INDArray = net.params().dup()
			net.Params = paramsOrig
			Dim params2 As INDArray = net.params()
			assertEquals(paramsOrig, params2)
		End Sub

		Private Const kH As Integer = 2

		Private Const kW As Integer = 2

		Private Shared ReadOnly strides() As Integer = { 1, 1 }

		Private Shared ReadOnly pad() As Integer = { 0, 0 }

		Private Const miniBatch As Integer = 2

		Private Const inDepth As Integer = 2

		Private Const height As Integer = 3

		Private Const width As Integer = 3

		Private Const outW As Integer = 2

		Private Const outH As Integer = 2

		Private Shared ReadOnly Property Input As INDArray
			Get
		'        
		'         ----- Input images -----
		'        example 0:
		'        channels 0     channels 1
		'        [ 0  1  2      [ 9 10 11
		'          3  4  5       12 13 14
		'          6  7  8]      15 16 17]
		'        example 1:
		'        [18 19 20      [27 28 29
		'         21 22 23       30 31 32
		'         24 25 26]      33 34 35]
		'         
				Dim input As INDArray = Nd4j.create(New Integer() { miniBatch, inDepth, height, width }, "c"c)
				input.put(New INDArrayIndex() { NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all() },
				Nd4j.create(New Double()() {
					New Double() { 0, 1, 2 },
					New Double() { 3, 4, 5 },
					New Double() { 6, 7, 8 }
				}))
				input.put(New INDArrayIndex() { NDArrayIndex.point(0), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all() },
				Nd4j.create(New Double()() {
					New Double() { 9, 10, 11 },
					New Double() { 12, 13, 14 },
					New Double() { 15, 16, 17 }
				}))
				input.put(New INDArrayIndex() { NDArrayIndex.point(1), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all() },
				Nd4j.create(New Double()() {
					New Double() { 18, 19, 20 },
					New Double() { 21, 22, 23 },
					New Double() { 24, 25, 26 }
				}))
				input.put(New INDArrayIndex() { NDArrayIndex.point(1), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all() },
				Nd4j.create(New Double()() {
					New Double() { 27, 28, 29 },
					New Double() { 30, 31, 32 },
					New Double() { 33, 34, 35 }
				}))
				Return input
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn Im 2 Col Reshaping") void testCnnIm2ColReshaping()
		Friend Overridable Sub testCnnIm2ColReshaping()
			' This test: a bit unusual in that it tests the *assumptions* of the CNN implementation rather than the implementation itself
			' Specifically, it tests the row and column orders after reshaping on im2col is reshaped (both forward and backward pass)
			Dim input As INDArray = ConvolutionLayerTest.Input
			' im2col in the required order: want [outW,outH,miniBatch,depthIn,kH,kW], but need to input [miniBatch,channels,kH,kW,outH,outW]
			' given the current im2col implementation
			' To get this: create an array of the order we want, permute it to the order required by im2col implementation, and then do im2col on that
			' to get old order from required order: permute(2,3,4,5,1,2)
			Dim col As INDArray = Nd4j.create(New Integer() { miniBatch, outH, outW, inDepth, kH, kW }, "c"c)
			Dim col2 As INDArray = col.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, strides(0), strides(1), pad(0), pad(1), False, col2)
	'        
	'        Expected Output, im2col
	'        - example 0 -
	'            channels 0                        channels 1
	'        h0,w0      h0,w1               h0,w0      h0,w1
	'        0  1     1  2                 9 10      10 11
	'        3  4     4  5                12 13      13 14
	'        
	'        h1,w0      h1,w1               h1,w0      h1,w1
	'        3  4     4  5                12 13      13 14
	'        6  7     7  8                15 16      16 17
	'        
	'        - example 1 -
	'            channels 0                        channels 1
	'        h0,w0      h0,w1               h0,w0      h0,w1
	'        18 19     19 20               27 28      28 29
	'        21 22     22 23               30 31      31 32
	'        
	'        h1,w0      h1,w1               h1,w0      h1,w1
	'        21 22     22 23               30 31      31 32
	'        24 25     25 26               33 34      34 35
	'        
			' Now, after reshaping im2col to 2d, we expect:
			' Rows with order (wOut0,hOut0,mb0), (wOut1,hOut0,mb0), (wOut0,hOut1,mb0), (wOut1,hOut1,mb0), (wOut0,hOut0,mb1), ...
			' Columns with order (d0,kh0,kw0), (d0,kh0,kw1), (d0,kh1,kw0), (d0,kh1,kw1), (d1,kh0,kw0), ...
			Dim reshapedCol As INDArray = Shape.newShapeNoCopy(col, New Integer() { miniBatch * outH * outW, inDepth * kH * kW }, False)
			Dim exp2d As INDArray = Nd4j.create(outW * outH * miniBatch, inDepth * kH * kW)
			' wOut0,hOut0,mb0 -> both depths, in order (d0,kh0,kw0), (d0,kh0,kw1), (d0,kh1,kw0), (d0,kh1,kw1), (d1,kh0,kw0), (d1,kh0,kw1), (d1,kh1,kw0), (d1,kh1,kw1)
			exp2d.putRow(0, Nd4j.create(New Double() { 0, 1, 3, 4, 9, 10, 12, 13 }))
			' wOut1,hOut0,mb0
			exp2d.putRow(1, Nd4j.create(New Double() { 1, 2, 4, 5, 10, 11, 13, 14 }))
			' wOut0,hOut1,mb0
			exp2d.putRow(2, Nd4j.create(New Double() { 3, 4, 6, 7, 12, 13, 15, 16 }))
			' wOut1,hOut1,mb0
			exp2d.putRow(3, Nd4j.create(New Double() { 4, 5, 7, 8, 13, 14, 16, 17 }))
			' wOut0,hOut0,mb1
			exp2d.putRow(4, Nd4j.create(New Double() { 18, 19, 21, 22, 27, 28, 30, 31 }))
			' wOut1,hOut0,mb1
			exp2d.putRow(5, Nd4j.create(New Double() { 19, 20, 22, 23, 28, 29, 31, 32 }))
			' wOut0,hOut1,mb1
			exp2d.putRow(6, Nd4j.create(New Double() { 21, 22, 24, 25, 30, 31, 33, 34 }))
			' wOut1,hOut1,mb1
			exp2d.putRow(7, Nd4j.create(New Double() { 22, 23, 25, 26, 31, 32, 34, 35 }))
			assertEquals(exp2d, reshapedCol)
			' Check the same thing for the backprop im2col (different order)
			Dim colBackprop As INDArray = Nd4j.create(New Integer() { miniBatch, outH, outW, inDepth, kH, kW }, "c"c)
			Dim colBackprop2 As INDArray = colBackprop.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, strides(0), strides(1), pad(0), pad(1), False, colBackprop2)
			Dim reshapedColBackprop As INDArray = Shape.newShapeNoCopy(colBackprop, New Integer() { miniBatch * outH * outW, inDepth * kH * kW }, False)
			' Rows with order (mb0,h0,w0), (mb0,h0,w1), (mb0,h1,w0), (mb0,h1,w1), (mb1,h0,w0), (mb1,h0,w1), (mb1,h1,w0), (mb1,h1,w1)
			' Columns with order (d0,kh0,kw0), (d0,kh0,kw1), (d0,kh1,kw0), (d0,kh1,kw1), (d1,kh0,kw0), ...
			Dim exp2dv2 As INDArray = Nd4j.create(outW * outH * miniBatch, inDepth * kH * kW)
			' wOut0,hOut0,mb0 -> both depths, in order (d0,kh0,kw0), (d0,kh0,kw1), (d0,kh1,kw0), (d0,kh1,kw1), (d1,kh0,kw0), (d1,kh0,kw1), (d1,kh1,kw0), (d1,kh1,kw1)
			exp2dv2.putRow(0, Nd4j.create(New Double() { 0, 1, 3, 4, 9, 10, 12, 13 }))
			' wOut1,hOut0,mb0
			exp2dv2.putRow(1, Nd4j.create(New Double() { 1, 2, 4, 5, 10, 11, 13, 14 }))
			' wOut0,hOut1,mb0
			exp2dv2.putRow(2, Nd4j.create(New Double() { 3, 4, 6, 7, 12, 13, 15, 16 }))
			' wOut1,hOut1,mb0
			exp2dv2.putRow(3, Nd4j.create(New Double() { 4, 5, 7, 8, 13, 14, 16, 17 }))
			' wOut0,hOut0,mb1
			exp2dv2.putRow(4, Nd4j.create(New Double() { 18, 19, 21, 22, 27, 28, 30, 31 }))
			' wOut1,hOut0,mb1
			exp2dv2.putRow(5, Nd4j.create(New Double() { 19, 20, 22, 23, 28, 29, 31, 32 }))
			' wOut0,hOut1,mb1
			exp2dv2.putRow(6, Nd4j.create(New Double() { 21, 22, 24, 25, 30, 31, 33, 34 }))
			' wOut1,hOut1,mb1
			exp2dv2.putRow(7, Nd4j.create(New Double() { 22, 23, 25, 26, 31, 32, 34, 35 }))
			assertEquals(exp2dv2, reshapedColBackprop)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Delta Reshaping") void testDeltaReshaping()
		Friend Overridable Sub testDeltaReshaping()
			' As per above test: testing assumptions of cnn implementation...
			' Delta: initially shape [miniBatch,dOut,outH,outW]
			' permute to [dOut,miniB,outH,outW]
			' then reshape to [dOut,miniB*outH*outW]
			' Expect columns of delta2d to be like: (mb0,h0,w0), (mb0,h0,w1), (mb1,h0,w2), (mb0,h1,w0), ... (mb1,...), ..., (mb2,...)
			Dim miniBatch As Integer = 3
			Dim depth As Integer = 2
			Dim outW As Integer = 3
			Dim outH As Integer = 3
	'        
	'         ----- Input delta -----
	'        example 0:
	'        channels 0     channels 1
	'        [ 0  1  2      [ 9 10 11
	'          3  4  5       12 13 14
	'          6  7  8]      15 16 17]
	'        example 1:
	'        [18 19 20      [27 28 29
	'         21 22 23       30 31 32
	'         24 25 26]      33 34 35]
	'        example 2:
	'        [36 37 38      [45 46 47
	'         39 40 41       48 49 50
	'         42 43 44]      51 52 53]
	'         
			Dim deltaOrig As INDArray = Nd4j.create(New Integer() { miniBatch, depth, outH, outW }, "c"c)
			deltaOrig.put(New INDArrayIndex() { NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 0, 1, 2 },
				New Double() { 3, 4, 5 },
				New Double() { 6, 7, 8 }
			}))
			deltaOrig.put(New INDArrayIndex() { NDArrayIndex.point(0), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 9, 10, 11 },
				New Double() { 12, 13, 14 },
				New Double() { 15, 16, 17 }
			}))
			deltaOrig.put(New INDArrayIndex() { NDArrayIndex.point(1), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 18, 19, 20 },
				New Double() { 21, 22, 23 },
				New Double() { 24, 25, 26 }
			}))
			deltaOrig.put(New INDArrayIndex() { NDArrayIndex.point(1), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 27, 28, 29 },
				New Double() { 30, 31, 32 },
				New Double() { 33, 34, 35 }
			}))
			deltaOrig.put(New INDArrayIndex() { NDArrayIndex.point(2), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 36, 37, 38 },
				New Double() { 39, 40, 41 },
				New Double() { 42, 43, 44 }
			}))
			deltaOrig.put(New INDArrayIndex() { NDArrayIndex.point(2), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 45, 46, 47 },
				New Double() { 48, 49, 50 },
				New Double() { 51, 52, 53 }
			}))
			Dim deltaPermute As INDArray = deltaOrig.permute(1, 0, 2, 3).dup("c"c)
			Dim delta2d As INDArray = Shape.newShapeNoCopy(deltaPermute, New Integer() { depth, miniBatch * outW * outH }, False)
			Dim exp As INDArray = Nd4j.create(New Double()() {
				New Double() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 18, 19, 20, 21, 22, 23, 24, 25, 26, 36, 37, 38, 39, 40, 41, 42, 43, 44 },
				New Double() { 9, 10, 11, 12, 13, 14, 15, 16, 17, 27, 28, 29, 30, 31, 32, 33, 34, 35, 45, 46, 47, 48, 49, 50, 51, 52, 53 }
			}).castTo(delta2d.dataType())
			assertEquals(exp, delta2d)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Weight Reshaping") void testWeightReshaping()
		Friend Overridable Sub testWeightReshaping()
			' Test assumptions of weight reshaping
			' Weights: originally c order, shape [outDepth, inDepth, kH, kw]
			' permute (3,2,1,0)
			Dim depthOut As Integer = 2
			Dim depthIn As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2
	'        
	'         ----- Weights -----
	'         - dOut 0 -
	'        dIn 0      dIn 1        dIn 2
	'        [ 0  1      [ 4  5      [ 8  9
	'          2  3]       6  7]      10 11]
	'         - dOut 1 -
	'        [12 13      [16 17      [20 21
	'         14 15]      18 19]      22 23]
	'         
			Dim weightOrig As INDArray = Nd4j.create(New Integer() { depthOut, depthIn, kH, kW }, "c"c)
			weightOrig.put(New INDArrayIndex() { NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 0, 1 },
				New Double() { 2, 3 }
			}))
			weightOrig.put(New INDArrayIndex() { NDArrayIndex.point(0), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 4, 5 },
				New Double() { 6, 7 }
			}))
			weightOrig.put(New INDArrayIndex() { NDArrayIndex.point(0), NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 8, 9 },
				New Double() { 10, 11 }
			}))
			weightOrig.put(New INDArrayIndex() { NDArrayIndex.point(1), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 12, 13 },
				New Double() { 14, 15 }
			}))
			weightOrig.put(New INDArrayIndex() { NDArrayIndex.point(1), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 16, 17 },
				New Double() { 18, 19 }
			}))
			weightOrig.put(New INDArrayIndex() { NDArrayIndex.point(1), NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all() },
			Nd4j.create(New Double()() {
				New Double() { 20, 21 },
				New Double() { 22, 23 }
			}))
			Dim weightPermute As INDArray = weightOrig.permute(3, 2, 1, 0)
			Dim w2d As INDArray = Shape.newShapeNoCopy(weightPermute, New Integer() { depthIn * kH * kW, depthOut }, True)
			assertNotNull(w2d)
			' Expected order of weight rows, after reshaping: (kw0,kh0,din0), (kw1,kh0,din0), (kw0,kh1,din0), (kw1,kh1,din0), (kw0,kh0,din1), ...
			Dim wExp As INDArray = Nd4j.create(New Double()() {
				New Double() { 0, 12 },
				New Double() { 1, 13 },
				New Double() { 2, 14 },
				New Double() { 3, 15 },
				New Double() { 4, 16 },
				New Double() { 5, 17 },
				New Double() { 6, 18 },
				New Double() { 7, 19 },
				New Double() { 8, 20 },
				New Double() { 9, 21 },
				New Double() { 10, 22 },
				New Double() { 11, 23 }
			}).castTo(DataType.FLOAT)
			assertEquals(wExp, w2d)
		End Sub

		' ////////////////////////////////////////////////////////////////////////////////
		Private Shared Function getCNNMLNConfig(ByVal backprop As Boolean, ByVal pretrain As Boolean) As MultiLayerNetwork
			Dim outputNum As Integer = 10
			Dim seed As Integer = 123
			Dim conf As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 10, 10 })).nOut(6).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).stride(1, 1).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1))
			Dim model As New MultiLayerNetwork(conf.build())
			model.init()
			Return model
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test 1 d Input Type") void test1dInputType()
		Friend Overridable Sub test1dInputType()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).list().layer((New Convolution1DLayer.Builder()).nOut(3).kernelSize(2).activation(Activation.TANH).build()).layer((New Subsampling1DLayer.Builder()).kernelSize(2).stride(2).build()).layer((New Upsampling1D.Builder()).size(2).build()).layer((New RnnOutputLayer.Builder()).nOut(7).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(10)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim l As IList(Of InputType) = conf.getLayerActivationTypes(InputType.recurrent(10))
			assertEquals(InputType.recurrent(3, -1), l(0))
			assertEquals(InputType.recurrent(3, -1), l(1))
			assertEquals(InputType.recurrent(3, -1), l(2))
			assertEquals(InputType.recurrent(7, -1), l(3))
			Dim l2 As IList(Of InputType) = conf.getLayerActivationTypes(InputType.recurrent(10, 6))
			assertEquals(InputType.recurrent(3, 6), l2(0))
			assertEquals(InputType.recurrent(3, 3), l2(1))
			assertEquals(InputType.recurrent(3, 6), l2(2))
			assertEquals(InputType.recurrent(7, 6), l2(3))
			Dim [in] As INDArray = Nd4j.create(2, 10, 6)
			Dim [out] As INDArray = net.output([in])
			assertArrayEquals(New Long() { 2, 7, 6 }, [out].shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Deconv Bad Input") void testDeconvBadInput()
		Friend Overridable Sub testDeconvBadInput()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New Deconvolution2D.Builder()).nIn(5).nOut(3).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim badInput As INDArray = Nd4j.create(DataType.FLOAT, 1, 10, 5, 5)
			Try
				net.output(badInput)
			Catch e As DL4JInvalidInputException
				Dim msg As String = e.Message
				assertTrue(msg.Contains("Deconvolution2D") AndAlso msg.Contains("input") AndAlso msg.Contains("channels"),msg)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Conv 1 d Causal Allowed") void testConv1dCausalAllowed()
		Friend Overridable Sub testConv1dCausalAllowed()
			Call (New Convolution1DLayer.Builder()).convolutionMode(ConvolutionMode.Causal).kernelSize(2).build()
			Call (New Subsampling1DLayer.Builder()).convolutionMode(ConvolutionMode.Causal).kernelSize(2).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Conv 2 d No Causal Allowed") void testConv2dNoCausalAllowed()
		Friend Overridable Sub testConv2dNoCausalAllowed()
			Try
				Call (New ConvolutionLayer.Builder()).convolutionMode(ConvolutionMode.Causal).build()
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage().ToLower()
				assertTrue(m.Contains("causal") AndAlso m.Contains("1d"),m)
			End Try
			Try
				Call (New Deconvolution2D.Builder()).convolutionMode(ConvolutionMode.Causal).build()
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage().ToLower()
				assertTrue(m.Contains("causal") AndAlso m.Contains("1d"),m)
			End Try
			Try
				Call (New DepthwiseConvolution2D.Builder()).convolutionMode(ConvolutionMode.Causal).build()
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage().ToLower()
				assertTrue(m.Contains("causal") AndAlso m.Contains("1d"),m)
			End Try
			Try
				Call (New SeparableConvolution2D.Builder()).convolutionMode(ConvolutionMode.Causal).build()
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage().ToLower()
				assertTrue(m.Contains("causal") AndAlso m.Contains("1d"),m)
			End Try
			Try
				Call (New SubsamplingLayer.Builder()).convolutionMode(ConvolutionMode.Causal).build()
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage().ToLower()
				assertTrue(m.Contains("causal") AndAlso m.Contains("1d"),m)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Conv 3 d No Causal Allowed") void testConv3dNoCausalAllowed()
		Friend Overridable Sub testConv3dNoCausalAllowed()
			Try
				Call (New Convolution3D.Builder()).convolutionMode(ConvolutionMode.Causal).build()
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage().ToLower()
				assertTrue(m.Contains("causal") AndAlso m.Contains("1d"),m)
			End Try
			Try
				Call (New Subsampling3DLayer.Builder()).convolutionMode(ConvolutionMode.Causal).build()
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage().ToLower()
				assertTrue(m.Contains("causal") AndAlso m.Contains("1d"),m)
			End Try
		End Sub
	End Class

End Namespace