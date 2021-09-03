Imports System.Collections.Generic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports ImageRecordReader = org.datavec.image.recordreader.ImageRecordReader
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports FeedForwardToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
'ORIGINAL LINE: @DisplayName("Convolution Layer Setup Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LARGE_RESOURCES) class ConvolutionLayerSetupTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ConvolutionLayerSetupTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convolution Layer Setup") void testConvolutionLayerSetup()
		Friend Overridable Sub testConvolutionLayerSetup()
			Dim builder As MultiLayerConfiguration.Builder = inComplete()
			builder.InputType = InputType.convolutionalFlat(28, 28, 1)
			Dim completed As MultiLayerConfiguration = complete().build()
			Dim test As MultiLayerConfiguration = builder.build()
			assertEquals(completed, test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dense To Output Layer") void testDenseToOutputLayer()
		Friend Overridable Sub testDenseToOutputLayer()
			Nd4j.Random.setSeed(12345)
			Const numRows As Integer = 76
			Const numColumns As Integer = 76
			Dim nChannels As Integer = 3
			Dim outputNum As Integer = 6
			Dim seed As Integer = 123
			' setup the network
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).l1(1e-1).l2(2e-4).dropOut(0.5).miniBatch(True).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nOut(5).dropOut(0.5).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).build()).layer(2, (New ConvolutionLayer.Builder(3, 3)).nOut(10).dropOut(0.5).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(3, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).build()).layer(4, (New DenseLayer.Builder()).nOut(100).activation(Activation.RELU).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(numRows, numColumns, nChannels))
			Dim d As New DataSet(Nd4j.rand(New Integer() { 10, nChannels, numRows, numColumns }), FeatureUtil.toOutcomeMatrix(New Integer() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 6))
			Dim network As New MultiLayerNetwork(builder.build())
			network.init()
			network.fit(d)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Mnist Lenet") void testMnistLenet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMnistLenet()
			Dim incomplete As MultiLayerConfiguration.Builder = incompleteMnistLenet()
			incomplete.InputType = InputType.convolutionalFlat(28, 28, 1)
			Dim testConf As MultiLayerConfiguration = incomplete.build()
			assertEquals(800, CType(testConf.getConf(4).getLayer(), FeedForwardLayer).getNIn())
			assertEquals(500, CType(testConf.getConf(5).getLayer(), FeedForwardLayer).getNIn())
			' test instantiation
			Dim iter As DataSetIterator = New MnistDataSetIterator(10, 10)
			Dim network As New MultiLayerNetwork(testConf)
			network.init()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			network.fit(iter.next())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Channel") void testMultiChannel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultiChannel()
			Dim [in] As INDArray = Nd4j.rand(New Integer() { 10, 3, 28, 28 })
			Dim labels As INDArray = Nd4j.rand(10, 2)
			Dim [next] As New DataSet([in], labels)
			Dim builder As NeuralNetConfiguration.ListBuilder = DirectCast(incompleteLFW(), NeuralNetConfiguration.ListBuilder)
			builder.InputType = InputType.convolutional(28, 28, 3)
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim layer2 As ConvolutionLayer = CType(conf.getConf(2).getLayer(), ConvolutionLayer)
			assertEquals(6, layer2.getNIn())
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.fit([next])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test LRN") void testLRN(@TempDir Path testFolder) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLRN(ByVal testFolder As Path)
			Dim labels As IList(Of String) = New List(Of String) From {"Zico", "Ziwang_Xu"}
			Dim dir As File = testFolder.toFile()
			Call (New ClassPathResource("lfwtest/")).copyDirectory(dir)
			Dim rootDir As String = dir.getAbsolutePath()
			Dim reader As RecordReader = New ImageRecordReader(28, 28, 3)
			reader.initialize(New org.datavec.api.Split.FileSplit(New File(rootDir)))
			Dim recordReader As DataSetIterator = New RecordReaderDataSetIterator(reader, 10, 1, labels.Count)
			labels.Remove("lfwtest")
			Dim builder As NeuralNetConfiguration.ListBuilder = DirectCast(incompleteLRN(), NeuralNetConfiguration.ListBuilder)
			builder.InputType = InputType.convolutional(28, 28, 3)
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim layer2 As ConvolutionLayer = CType(conf.getConf(3).getLayer(), ConvolutionLayer)
			assertEquals(6, layer2.getNIn())
		End Sub

		Public Overridable Function incompleteLRN() As MultiLayerConfiguration.Builder
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(3).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nOut(6).build()).layer(1, (New SubsamplingLayer.Builder(New Integer() { 2, 2 })).build()).layer(2, (New LocalResponseNormalization.Builder()).build()).layer(3, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nOut(6).build()).layer(4, (New SubsamplingLayer.Builder(New Integer() { 2, 2 })).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(2).activation(Activation.SOFTMAX).build())
			Return builder
		End Function

		Public Overridable Function incompleteLFW() As MultiLayerConfiguration.Builder
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(3).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nOut(6).build()).layer(1, (New SubsamplingLayer.Builder(New Integer() { 2, 2 })).build()).layer(2, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nOut(6).build()).layer(3, (New SubsamplingLayer.Builder(New Integer() { 2, 2 })).build()).layer(4, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).activation(Activation.SOFTMAX).nOut(2).build())
			Return builder
		End Function

		Public Overridable Function incompleteMnistLenet() As MultiLayerConfiguration.Builder
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(3).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nIn(1).nOut(20).build()).layer(1, (New SubsamplingLayer.Builder(New Integer() { 2, 2 }, New Integer() { 2, 2 })).build()).layer(2, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nIn(20).nOut(50).build()).layer(3, (New SubsamplingLayer.Builder(New Integer() { 2, 2 }, New Integer() { 2, 2 })).build()).layer(4, (New DenseLayer.Builder()).nOut(500).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).activation(Activation.SOFTMAX).nOut(10).build())
			Return builder
		End Function

		Public Overridable Function mnistLenet() As MultiLayerConfiguration
			Dim builder As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(3).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nIn(1).nOut(6).build()).layer(1, (New SubsamplingLayer.Builder(New Integer() { 5, 5 }, New Integer() { 2, 2 })).build()).layer(2, (New ConvolutionLayer.Builder(New Integer() { 5, 5 })).nIn(1).nOut(6).build()).layer(3, (New SubsamplingLayer.Builder(New Integer() { 5, 5 }, New Integer() { 2, 2 })).build()).layer(4, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nIn(150).nOut(10).build()).build()
			Return builder
		End Function

		Public Overridable Function inComplete() As MultiLayerConfiguration.Builder
			Dim nChannels As Integer = 1
			Dim outputNum As Integer = 10
			Dim seed As Integer = 123
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 10, 10 }, New Integer() { 2, 2 })).nIn(nChannels).nOut(6).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build())
			Return builder
		End Function

		Public Overridable Function complete() As MultiLayerConfiguration.Builder
			Const numRows As Integer = 28
			Const numColumns As Integer = 28
			Dim nChannels As Integer = 1
			Dim outputNum As Integer = 10
			Dim seed As Integer = 123
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 10, 10 }, New Integer() { 2, 2 })).nIn(nChannels).nOut(6).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nIn(5 * 5 * 1 * 6).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).inputPreProcessor(0, New FeedForwardToCnnPreProcessor(numRows, numColumns, nChannels)).inputPreProcessor(2, New CnnToFeedForwardPreProcessor(5, 5, 6))
			Return builder
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Deconvolution") void testDeconvolution()
		Friend Overridable Sub testDeconvolution()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer(0, (New Deconvolution2D.Builder(2, 2)).padding(0, 0).stride(2, 2).nIn(1).nOut(3).build()).layer(1, (New SubsamplingLayer.Builder()).kernelSize(2, 2).padding(1, 1).stride(2, 2).build()).layer(2, (New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1))
			Dim conf As MultiLayerConfiguration = builder.build()
			assertNotNull(conf.getInputPreProcess(2))
			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(2), CnnToFeedForwardPreProcessor)
			assertEquals(29, proc.getInputHeight())
			assertEquals(29, proc.getInputWidth())
			assertEquals(3, proc.getNumChannels())
			assertEquals(29 * 29 * 3, CType(conf.getConf(2).getLayer(), FeedForwardLayer).getNIn())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sub Sampling With Padding") void testSubSamplingWithPadding()
		Friend Overridable Sub testSubSamplingWithPadding()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer(0, (New ConvolutionLayer.Builder(2, 2)).padding(0, 0).stride(2, 2).nIn(1).nOut(3).build()).layer(1, (New SubsamplingLayer.Builder()).kernelSize(2, 2).padding(1, 1).stride(2, 2).build()).layer(2, (New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1))
			Dim conf As MultiLayerConfiguration = builder.build()
			assertNotNull(conf.getInputPreProcess(2))
			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(2), CnnToFeedForwardPreProcessor)
			assertEquals(8, proc.getInputHeight())
			assertEquals(8, proc.getInputWidth())
			assertEquals(3, proc.getNumChannels())
			assertEquals(8 * 8 * 3, CType(conf.getConf(2).getLayer(), FeedForwardLayer).getNIn())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Upsampling") void testUpsampling()
		Friend Overridable Sub testUpsampling()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer((New ConvolutionLayer.Builder(2, 2)).padding(0, 0).stride(2, 2).nIn(1).nOut(3).build()).layer((New Upsampling2D.Builder()).size(3).build()).layer((New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1))
			Dim conf As MultiLayerConfiguration = builder.build()
			assertNotNull(conf.getInputPreProcess(2))
			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(2), CnnToFeedForwardPreProcessor)
			assertEquals(42, proc.getInputHeight())
			assertEquals(42, proc.getInputWidth())
			assertEquals(3, proc.getNumChannels())
			assertEquals(42 * 42 * 3, CType(conf.getConf(2).getLayer(), FeedForwardLayer).getNIn())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Space To Batch") void testSpaceToBatch()
		Friend Overridable Sub testSpaceToBatch()
			Dim blocks() As Integer = { 2, 2 }
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer((New ConvolutionLayer.Builder(2, 2)).padding(0, 0).stride(2, 2).nIn(1).nOut(3).build()).layer((New SpaceToBatchLayer.Builder(blocks)).build()).layer((New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1))
			Dim conf As MultiLayerConfiguration = builder.build()
			assertNotNull(conf.getInputPreProcess(2))
			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(2), CnnToFeedForwardPreProcessor)
			assertEquals(7, proc.getInputHeight())
			assertEquals(7, proc.getInputWidth())
			assertEquals(3, proc.getNumChannels())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Space To Depth") void testSpaceToDepth()
		Friend Overridable Sub testSpaceToDepth()
			Dim blocks As Integer = 2
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer((New ConvolutionLayer.Builder(2, 2)).padding(0, 0).stride(2, 2).nIn(1).nOut(3).build()).layer((New SpaceToDepthLayer.Builder(blocks, SpaceToDepthLayer.DataFormat.NCHW)).build()).layer((New OutputLayer.Builder()).nIn(3 * 2 * 2).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1))
			Dim conf As MultiLayerConfiguration = builder.build()
			assertNotNull(conf.getInputPreProcess(2))
			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(2), CnnToFeedForwardPreProcessor)
			assertEquals(7, proc.getInputHeight())
			assertEquals(7, proc.getInputWidth())
			assertEquals(12, proc.getNumChannels())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNNDBN Multi Layer") void testCNNDBNMultiLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCNNDBNMultiLayer()
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			' Run with separate activation layer
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).weightInit(WeightInit.XAVIER).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 1, 1 }, New Integer() { 1, 1 })).nIn(1).nOut(6).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(3, (New DenseLayer.Builder()).nIn(28 * 28 * 6).nOut(10).activation(Activation.IDENTITY).build()).layer(4, (New BatchNormalization.Builder()).nOut(10).build()).layer(5, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(6, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.Input = [next].Features
			Dim activationsActual As INDArray = network.output([next].Features)
			assertEquals(10, activationsActual.shape()(1), 1e-2)
			network.fit([next])
			Dim actualGammaParam As INDArray = network.getLayer(1).getParam(BatchNormalizationParamInitializer.GAMMA)
			Dim actualBetaParam As INDArray = network.getLayer(1).getParam(BatchNormalizationParamInitializer.BETA)
			assertTrue(actualGammaParam IsNot Nothing)
			assertTrue(actualBetaParam IsNot Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Separable Conv 2 D") void testSeparableConv2D()
		Friend Overridable Sub testSeparableConv2D()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer((New SeparableConvolution2D.Builder(2, 2)).depthMultiplier(2).padding(0, 0).stride(2, 2).nIn(1).nOut(3).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).padding(1, 1).stride(2, 2).build()).layer(2, (New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1))
			Dim conf As MultiLayerConfiguration = builder.build()
			assertNotNull(conf.getInputPreProcess(2))
			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(2), CnnToFeedForwardPreProcessor)
			assertEquals(8, proc.getInputHeight())
			assertEquals(8, proc.getInputWidth())
			assertEquals(3, proc.getNumChannels())
			assertEquals(8 * 8 * 3, CType(conf.getConf(2).getLayer(), FeedForwardLayer).getNIn())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Deconv 2 D") void testDeconv2D()
		Friend Overridable Sub testDeconv2D()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer((New Deconvolution2D.Builder(2, 2)).padding(0, 0).stride(2, 2).nIn(1).nOut(3).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).padding(1, 1).stride(2, 2).build()).layer(2, (New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1))
			Dim conf As MultiLayerConfiguration = builder.build()
			assertNotNull(conf.getInputPreProcess(2))
			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)
			Dim proc As CnnToFeedForwardPreProcessor = DirectCast(conf.getInputPreProcess(2), CnnToFeedForwardPreProcessor)
			assertEquals(29, proc.getInputHeight())
			assertEquals(29, proc.getInputWidth())
			assertEquals(3, proc.getNumChannels())
			assertEquals(29 * 29 * 3, CType(conf.getConf(2).getLayer(), FeedForwardLayer).getNIn())
		End Sub
	End Class

End Namespace