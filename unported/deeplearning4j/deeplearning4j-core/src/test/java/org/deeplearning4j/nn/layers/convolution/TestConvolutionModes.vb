Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.deeplearning4j.nn.layers.convolution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestConvolutionModes extends org.deeplearning4j.BaseDL4JTest
	Public Class TestConvolutionModes
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStrictTruncateConvolutionModeOutput()
		Public Overridable Sub testStrictTruncateConvolutionModeOutput()

			'Idea: with convolution mode == Truncate, input size shouldn't matter (within the bounds of truncated edge),
			' and edge data shouldn't affect the output

			'Use: 9x9, kernel 3, stride 3, padding 0
			' Should get same output for 10x10 and 11x11...

			Nd4j.Random.setSeed(12345)
			Dim minibatches() As Integer = {1, 3}
			Dim inDepths() As Integer = {1, 3}
			Dim inSizes() As Integer = {9, 10, 11}

			For Each isSubsampling As Boolean In New Boolean() {False, True}
				For Each minibatch As Integer In minibatches
					For Each inDepth As Integer In inDepths

						Dim origData As INDArray = Nd4j.rand(New Integer() {minibatch, inDepth, 9, 9})

						For Each inSize As Integer In inSizes

							For Each cm As ConvolutionMode In New ConvolutionMode() {ConvolutionMode.Strict, ConvolutionMode.Truncate}

								Dim inputData As INDArray = Nd4j.rand(New Integer() {minibatch, inDepth, inSize, inSize})
								inputData.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 9), NDArrayIndex.interval(0, 9)).assign(origData)

								Dim layer As Layer
								If isSubsampling Then
									layer = (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).build()
								Else
									layer = (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).nOut(3).build()
								End If

								Dim net As MultiLayerNetwork = Nothing
								Try
									Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(cm).list().layer(0, layer).layer(1, (New OutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nOut(3).build()).setInputType(InputType.convolutional(inSize, inSize, inDepth)).build()

									net = New MultiLayerNetwork(conf)
									net.init()
									If inSize > 9 AndAlso cm = ConvolutionMode.Strict Then
										fail("Expected exception")
									End If
								Catch e As DL4JException
									If inSize = 9 OrElse cm <> ConvolutionMode.Strict Then
										log.error("",e)
										fail("Unexpected exception")
									End If
									Continue For 'Expected exception
								Catch e As Exception
									log.error("",e)
									fail("Unexpected exception")
								End Try

								Dim [out] As INDArray = net.output(origData)
								Dim out2 As INDArray = net.output(inputData)

								assertEquals([out], out2)
							Next cm
						Next inSize
					Next inDepth
				Next minibatch
			Next isSubsampling
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStrictTruncateConvolutionModeCompGraph()
		Public Overridable Sub testStrictTruncateConvolutionModeCompGraph()

			'Idea: with convolution mode == Truncate, input size shouldn't matter (within the bounds of truncated edge),
			' and edge data shouldn't affect the output

			'Use: 9x9, kernel 3, stride 3, padding 0
			' Should get same output for 10x10 and 11x11...

			Nd4j.Random.setSeed(12345)
			Dim minibatches() As Integer = {1, 3}
			Dim inDepths() As Integer = {1, 3}
			Dim inSizes() As Integer = {9, 10, 11}

			For Each isSubsampling As Boolean In New Boolean() {False, True}
				For Each minibatch As Integer In minibatches
					For Each inDepth As Integer In inDepths

						Dim origData As INDArray = Nd4j.rand(New Integer() {minibatch, inDepth, 9, 9})

						For Each inSize As Integer In inSizes

							For Each cm As ConvolutionMode In New ConvolutionMode() {ConvolutionMode.Strict, ConvolutionMode.Truncate}

								Dim inputData As INDArray = Nd4j.rand(New Integer() {minibatch, inDepth, inSize, inSize})
								inputData.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 9), NDArrayIndex.interval(0, 9)).assign(origData)

								Dim layer As Layer
								If isSubsampling Then
									layer = (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).build()
								Else
									layer = (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).nOut(3).build()
								End If

								Dim net As ComputationGraph = Nothing
								Try
									Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(cm).graphBuilder().addInputs("in").addLayer("0", layer, "in").addLayer("1", (New OutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nOut(3).build(), "0").setOutputs("1").setInputTypes(InputType.convolutional(inSize, inSize, inDepth)).build()

									net = New ComputationGraph(conf)
									net.init()
									If inSize > 9 AndAlso cm = ConvolutionMode.Strict Then
										fail("Expected exception")
									End If
								Catch e As DL4JException
									If inSize = 9 OrElse cm <> ConvolutionMode.Strict Then
										log.error("",e)
										fail("Unexpected exception")
									End If
									Continue For 'Expected exception
								Catch e As Exception
									log.error("",e)
									fail("Unexpected exception")
								End Try

								Dim [out] As INDArray = net.outputSingle(origData)
								Dim out2 As INDArray = net.outputSingle(inputData)

								assertEquals([out], out2)
							Next cm
						Next inSize
					Next inDepth
				Next minibatch
			Next isSubsampling
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGlobalLocalConfig()
		Public Overridable Sub testGlobalLocalConfig()
			For Each cm As ConvolutionMode In New ConvolutionMode() {ConvolutionMode.Strict, ConvolutionMode.Truncate}
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(cm).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build()).layer(1, (New ConvolutionLayer.Builder()).convolutionMode(ConvolutionMode.Strict).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build()).layer(2, (New ConvolutionLayer.Builder()).convolutionMode(ConvolutionMode.Truncate).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build()).layer(3, (New ConvolutionLayer.Builder()).convolutionMode(ConvolutionMode.Same).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build()).layer(4, (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).build()).layer(5, (New SubsamplingLayer.Builder()).convolutionMode(ConvolutionMode.Strict).kernelSize(3, 3).stride(3, 3).padding(0, 0).build()).layer(6, (New SubsamplingLayer.Builder()).convolutionMode(ConvolutionMode.Truncate).kernelSize(3, 3).stride(3, 3).padding(0, 0).build()).layer(7, (New SubsamplingLayer.Builder()).convolutionMode(ConvolutionMode.Same).kernelSize(3, 3).stride(3, 3).padding(0, 0).build()).layer(8, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).nOut(3).activation(Activation.SOFTMAX).build()).build()

				assertEquals(cm, CType(conf.getConf(0).getLayer(), ConvolutionLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Strict, CType(conf.getConf(1).getLayer(), ConvolutionLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Truncate, CType(conf.getConf(2).getLayer(), ConvolutionLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Same, CType(conf.getConf(3).getLayer(), ConvolutionLayer).getConvolutionMode())

				assertEquals(cm, CType(conf.getConf(4).getLayer(), SubsamplingLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Strict, CType(conf.getConf(5).getLayer(), SubsamplingLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Truncate, CType(conf.getConf(6).getLayer(), SubsamplingLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Same, CType(conf.getConf(7).getLayer(), SubsamplingLayer).getConvolutionMode())
			Next cm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGlobalLocalConfigCompGraph()
		Public Overridable Sub testGlobalLocalConfigCompGraph()
			For Each cm As ConvolutionMode In New ConvolutionMode() {ConvolutionMode.Strict, ConvolutionMode.Truncate, ConvolutionMode.Same}
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).convolutionMode(cm).graphBuilder().addInputs("in").addLayer("0", (New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build(), "in").addLayer("1", (New ConvolutionLayer.Builder()).convolutionMode(ConvolutionMode.Strict).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build(), "0").addLayer("2", (New ConvolutionLayer.Builder()).convolutionMode(ConvolutionMode.Truncate).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build(), "1").addLayer("3", (New ConvolutionLayer.Builder()).convolutionMode(ConvolutionMode.Same).kernelSize(3, 3).stride(3, 3).padding(0, 0).nIn(3).nOut(3).build(), "2").addLayer("4", (New SubsamplingLayer.Builder()).kernelSize(3, 3).stride(3, 3).padding(0, 0).build(), "3").addLayer("5", (New SubsamplingLayer.Builder()).convolutionMode(ConvolutionMode.Strict).kernelSize(3, 3).stride(3, 3).padding(0, 0).build(), "4").addLayer("6", (New SubsamplingLayer.Builder()).convolutionMode(ConvolutionMode.Truncate).kernelSize(3, 3).stride(3, 3).padding(0, 0).build(), "5").addLayer("7", (New SubsamplingLayer.Builder()).convolutionMode(ConvolutionMode.Same).kernelSize(3, 3).stride(3, 3).padding(0, 0).build(), "6").addLayer("8", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nOut(3).build(), "7").setOutputs("8").build()

				assertEquals(cm, CType(CType(conf.getVertices().get("0"), LayerVertex).getLayerConf().getLayer(), ConvolutionLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Strict, CType(CType(conf.getVertices().get("1"), LayerVertex).getLayerConf().getLayer(), ConvolutionLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Truncate, CType(CType(conf.getVertices().get("2"), LayerVertex).getLayerConf().getLayer(), ConvolutionLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Same, CType(CType(conf.getVertices().get("3"), LayerVertex).getLayerConf().getLayer(), ConvolutionLayer).getConvolutionMode())

				assertEquals(cm, CType(CType(conf.getVertices().get("4"), LayerVertex).getLayerConf().getLayer(), SubsamplingLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Strict, CType(CType(conf.getVertices().get("5"), LayerVertex).getLayerConf().getLayer(), SubsamplingLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Truncate, CType(CType(conf.getVertices().get("6"), LayerVertex).getLayerConf().getLayer(), SubsamplingLayer).getConvolutionMode())
				assertEquals(ConvolutionMode.Same, CType(CType(conf.getVertices().get("7"), LayerVertex).getLayerConf().getLayer(), SubsamplingLayer).getConvolutionMode())
			Next cm
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvolutionModeInputTypes()
		Public Overridable Sub testConvolutionModeInputTypes()
			'Test 1: input 3x3, stride 1, kernel 2

			Dim inH As Integer = 3
			Dim inW As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim sH As Integer = 1
			Dim sW As Integer = 1
			Dim pH As Integer = 0
			Dim pW As Integer = 0

			Dim minibatch As Integer = 3
			Dim dIn As Integer = 5
			Dim dOut As Integer = 7
			Dim kernel() As Integer = {kH, kW}
			Dim stride() As Integer = {sH, sW}
			Dim padding() As Integer = {pH, pW}
			Dim dilation() As Integer = {1, 1}

			Dim inData As INDArray = Nd4j.create(minibatch, dIn, inH, inW)
			Dim inputType As InputType = InputType.convolutional(inH, inW, dIn)

			'Strict mode: expect 2x2 out -> (inH - kernel + 2*padding)/stride + 1 = (3-2+0)/1+1 = 2
			Dim it As InputType.InputTypeConvolutional = DirectCast(InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, padding, dilation, ConvolutionMode.Strict, dOut, -1, "layerName", GetType(ConvolutionLayer)), InputType.InputTypeConvolutional)
			assertEquals(2, it.getHeight())
			assertEquals(2, it.getWidth())
			assertEquals(dOut, it.getChannels())
			Dim outSize() As Integer = ConvolutionUtils.getOutputSize(inData, kernel, stride, padding, ConvolutionMode.Strict)
			assertEquals(2, outSize(0))
			assertEquals(2, outSize(1))


			'Truncate: same as strict here
			it = DirectCast(InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, padding, dilation, ConvolutionMode.Truncate, dOut, -1, "layerName", GetType(ConvolutionLayer)), InputType.InputTypeConvolutional)
			assertEquals(2, it.getHeight())
			assertEquals(2, it.getWidth())
			assertEquals(dOut, it.getChannels())
			outSize = ConvolutionUtils.getOutputSize(inData, kernel, stride, padding, ConvolutionMode.Truncate)
			assertEquals(2, outSize(0))
			assertEquals(2, outSize(1))

			'Same mode: ceil(in / stride) = 3
			it = DirectCast(InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, Nothing, dilation, ConvolutionMode.Same, dOut, -1, "layerName", GetType(ConvolutionLayer)), InputType.InputTypeConvolutional)
			assertEquals(3, it.getHeight())
			assertEquals(3, it.getWidth())
			assertEquals(dOut, it.getChannels())
			outSize = ConvolutionUtils.getOutputSize(inData, kernel, stride, Nothing, ConvolutionMode.Same)
			assertEquals(3, outSize(0))
			assertEquals(3, outSize(1))



			'Test 2: input 3x4, stride 2, kernel 3
			inH = 3
			inW = 4
			kH = 3
			kW = 3
			sH = 2
			sW = 2

			kernel = New Integer() {kH, kW}
			stride = New Integer() {sH, sW}
			padding = New Integer() {pH, pW}

			inData = Nd4j.create(minibatch, dIn, inH, inW)
			inputType = InputType.convolutional(inH, inW, dIn)

			'Strict mode: (4-3+0)/2+1 is not an integer -> exception
			Try
				InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, padding, dilation, ConvolutionMode.Strict, dOut, -1, "layerName", GetType(ConvolutionLayer))
				fail("Expected exception")
			Catch e As DL4JException
				Console.WriteLine(e.Message)
			End Try
			Try
				outSize = ConvolutionUtils.getOutputSize(inData, kernel, stride, padding, ConvolutionMode.Strict)
				fail("Exception expected")
			Catch e As DL4JException
				Console.WriteLine(e.Message)
			End Try

			'Truncate: (3-3+0)/2+1 = 1 in height dim; (4-3+0)/2+1 = 1 in width dim
			it = DirectCast(InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, padding, dilation, ConvolutionMode.Truncate, dOut, -1, "layerName", GetType(ConvolutionLayer)), InputType.InputTypeConvolutional)
			assertEquals(1, it.getHeight())
			assertEquals(1, it.getWidth())
			assertEquals(dOut, it.getChannels())
			outSize = ConvolutionUtils.getOutputSize(inData, kernel, stride, padding, ConvolutionMode.Truncate)
			assertEquals(1, outSize(0))
			assertEquals(1, outSize(1))

			'Same mode: ceil(3/2) = 2 in height dim; ceil(4/2) = 2 in width dimension
			it = DirectCast(InputTypeUtil.getOutputTypeCnnLayers(inputType, kernel, stride, Nothing, dilation, ConvolutionMode.Same, dOut, -1, "layerName", GetType(ConvolutionLayer)), InputType.InputTypeConvolutional)
			assertEquals(2, it.getHeight())
			assertEquals(2, it.getWidth())
			assertEquals(dOut, it.getChannels())
			outSize = ConvolutionUtils.getOutputSize(inData, kernel, stride, Nothing, ConvolutionMode.Same)
			assertEquals(2, outSize(0))
			assertEquals(2, outSize(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameModeActivationSizes()
		Public Overridable Sub testSameModeActivationSizes()
			Dim inH As Integer = 3
			Dim inW As Integer = 4
			Dim inDepth As Integer = 3
			Dim minibatch As Integer = 5

			Dim sH As Integer = 2
			Dim sW As Integer = 2
			Dim kH As Integer = 3
			Dim kW As Integer = 3

			Dim l(1) As Layer
			l(0) = (New ConvolutionLayer.Builder()).nOut(4).kernelSize(kH, kW).stride(sH, sW).build()
			l(1) = (New SubsamplingLayer.Builder()).kernelSize(kH, kW).stride(sH, sW).build()

			For i As Integer = 0 To l.Length - 1

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).list().layer(0, l(i)).layer(1, (New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(inH, inW, inDepth)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inData As INDArray = Nd4j.create(minibatch, inDepth, inH, inW)
				Dim activations As IList(Of INDArray) = net.feedForward(inData)
				Dim actL0 As INDArray = activations(1)

				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(sH)))))
				Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(sW)))))

				Console.WriteLine(Arrays.toString(actL0.shape()))
				assertArrayEquals(New Long() {minibatch, (If(i = 0, 4, inDepth)), outH, outW}, actL0.shape())
			Next i
		End Sub
	End Class

End Namespace