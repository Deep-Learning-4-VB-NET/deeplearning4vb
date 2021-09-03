Imports Log = lombok.extern.java.Log
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping3D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping3D
Imports Cnn3DToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.Cnn3DToFeedForwardPreProcessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
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
Namespace org.deeplearning4j.gradientcheck

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Log @DisplayName("Cnn 3 D Gradient Check Test") @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Disabled("Fails on gpu, to be revisited") class CNN3DGradientCheckTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CNN3DGradientCheckTest
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True

		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False

		Private Const DEFAULT_EPS As Double = 1e-6

		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3

		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 3 D Plain") void testCnn3DPlain()
		Friend Overridable Sub testCnn3DPlain()
			Nd4j.Random.setSeed(1337)
			' Note: we checked this with a variety of parameters, but it takes a lot of time.
			Dim depths() As Integer = { 6 }
			Dim heights() As Integer = { 6 }
			Dim widths() As Integer = { 6 }
			Dim minibatchSizes() As Integer = { 3 }
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim denseNOut As Integer = 5
			Dim finalNOut As Integer = 42
			Dim kernels()() As Integer = {
				New Integer() { 2, 2, 2 }
			}
			Dim strides()() As Integer = {
				New Integer() { 1, 1, 1 }
			}
			Dim activations() As Activation = { Activation.SIGMOID }
			Dim modes() As ConvolutionMode = { ConvolutionMode.Truncate, ConvolutionMode.Same }
			For Each afn As Activation In activations
				For Each miniBatchSize As Integer In minibatchSizes
					For Each depth As Integer In depths
						For Each height As Integer In heights
							For Each width As Integer In widths
								For Each mode As ConvolutionMode In modes
									For Each kernel As Integer() In kernels
										For Each stride As Integer() In strides
											For Each df As Convolution3D.DataFormat In System.Enum.GetValues(GetType(Convolution3D.DataFormat))
												Dim outDepth As Integer = If(mode = ConvolutionMode.Same, depth \ stride(0), (depth - kernel(0)) \ stride(0) + 1)
												Dim outHeight As Integer = If(mode = ConvolutionMode.Same, height \ stride(1), (height - kernel(1)) \ stride(1) + 1)
												Dim outWidth As Integer = If(mode = ConvolutionMode.Same, width \ stride(2), (width - kernel(2)) \ stride(2) + 1)
												Dim input As INDArray
												If df = Convolution3D.DataFormat.NDHWC Then
													input = Nd4j.rand(New Integer() { miniBatchSize, depth, height, width, convNIn })
												Else
													input = Nd4j.rand(New Integer() { miniBatchSize, convNIn, depth, height, width })
												End If
												Dim labels As INDArray = Nd4j.zeros(miniBatchSize, finalNOut)
												For i As Integer = 0 To miniBatchSize - 1
													labels.putScalar(New Integer() { i, i Mod finalNOut }, 1.0)
												Next i
												Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(WeightInit.LECUN_NORMAL).dist(New NormalDistribution(0, 1)).list().layer(0, (New Convolution3D.Builder()).activation(afn).kernelSize(kernel).stride(stride).nIn(convNIn).nOut(convNOut1).hasBias(False).convolutionMode(mode).dataFormat(df).build()).layer(1, (New Convolution3D.Builder()).activation(afn).kernelSize(1, 1, 1).nIn(convNOut1).nOut(convNOut2).hasBias(False).convolutionMode(mode).dataFormat(df).build()).layer(2, (New DenseLayer.Builder()).nOut(denseNOut).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).inputPreProcessor(2, New Cnn3DToFeedForwardPreProcessor(outDepth, outHeight, outWidth, convNOut2, df = Convolution3D.DataFormat.NCDHW)).setInputType(InputType.convolutional3D(df, depth, height, width, convNIn)).build()
												Dim json As String = conf.toJson()
												Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
												assertEquals(conf, c2)
												Dim net As New MultiLayerNetwork(conf)
												net.init()
												Dim msg As String = "DataFormat = " & df & ", minibatch size = " & miniBatchSize & ", activationFn=" & afn & ", kernel = " & Arrays.toString(kernel) & ", stride = " & Arrays.toString(stride) & ", mode = " & mode.ToString() & ", input depth " & depth & ", input height " & height & ", input width " & width
												If PRINT_RESULTS Then
													log.info(msg)
													' for (int j = 0; j < net.getnLayers(); j++) {
													' log.info("Layer " + j + " # params: " + net.getLayer(j).numParams());
													' }
												End If
												Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(128))
												assertTrue(gradOK,msg)
												TestUtils.testModelSerialization(net)
											Next df
										Next stride
									Next kernel
								Next mode
							Next width
						Next height
					Next depth
				Next miniBatchSize
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 3 D Zero Padding") void testCnn3DZeroPadding()
		Friend Overridable Sub testCnn3DZeroPadding()
			Nd4j.Random.setSeed(42)
			Dim depth As Integer = 4
			Dim height As Integer = 4
			Dim width As Integer = 4
			Dim minibatchSizes() As Integer = { 3 }
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim denseNOut As Integer = 5
			Dim finalNOut As Integer = 42
			Dim kernel() As Integer = { 2, 2, 2 }
			Dim zeroPadding() As Integer = { 1, 1, 2, 2, 3, 3 }
			Dim activations() As Activation = { Activation.SIGMOID }
			Dim modes() As ConvolutionMode = { ConvolutionMode.Truncate, ConvolutionMode.Same }
			For Each afn As Activation In activations
				For Each miniBatchSize As Integer In minibatchSizes
					For Each mode As ConvolutionMode In modes
						Dim outDepth As Integer = If(mode = ConvolutionMode.Same, depth, (depth - kernel(0)) + 1)
						Dim outHeight As Integer = If(mode = ConvolutionMode.Same, height, (height - kernel(1)) + 1)
						Dim outWidth As Integer = If(mode = ConvolutionMode.Same, width, (width - kernel(2)) + 1)
						outDepth += zeroPadding(0) + zeroPadding(1)
						outHeight += zeroPadding(2) + zeroPadding(3)
						outWidth += zeroPadding(4) + zeroPadding(5)
						Dim input As INDArray = Nd4j.rand(New Integer() { miniBatchSize, convNIn, depth, height, width })
						Dim labels As INDArray = Nd4j.zeros(miniBatchSize, finalNOut)
						For i As Integer = 0 To miniBatchSize - 1
							labels.putScalar(New Integer() { i, i Mod finalNOut }, 1.0)
						Next i
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(WeightInit.LECUN_NORMAL).dist(New NormalDistribution(0, 1)).list().layer(0, (New Convolution3D.Builder()).activation(afn).kernelSize(kernel).nIn(convNIn).nOut(convNOut1).hasBias(False).convolutionMode(mode).dataFormat(Convolution3D.DataFormat.NCDHW).build()).layer(1, (New Convolution3D.Builder()).activation(afn).kernelSize(1, 1, 1).nIn(convNOut1).nOut(convNOut2).hasBias(False).convolutionMode(mode).dataFormat(Convolution3D.DataFormat.NCDHW).build()).layer(2, (New ZeroPadding3DLayer.Builder(zeroPadding)).build()).layer(3, (New DenseLayer.Builder()).nOut(denseNOut).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).inputPreProcessor(3, New Cnn3DToFeedForwardPreProcessor(outDepth, outHeight, outWidth, convNOut2, True)).setInputType(InputType.convolutional3D(depth, height, width, convNIn)).build()
						Dim json As String = conf.toJson()
						Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
						assertEquals(conf, c2)
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim msg As String = "Minibatch size = " & miniBatchSize & ", activationFn=" & afn & ", kernel = " & Arrays.toString(kernel) & ", mode = " & mode.ToString() & ", input depth " & depth & ", input height " & height & ", input width " & width
						If PRINT_RESULTS Then
							log.info(msg)
							' for (int j = 0; j < net.getnLayers(); j++) {
							' log.info("Layer " + j + " # params: " + net.getLayer(j).numParams());
							' }
						End If
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(512))
						assertTrue(gradOK,msg)
						TestUtils.testModelSerialization(net)
					Next mode
				Next miniBatchSize
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 3 D Pooling") void testCnn3DPooling()
		Friend Overridable Sub testCnn3DPooling()
			Nd4j.Random.setSeed(42)
			Dim depth As Integer = 4
			Dim height As Integer = 4
			Dim width As Integer = 4
			Dim minibatchSizes() As Integer = { 3 }
			Dim convNIn As Integer = 2
			Dim convNOut As Integer = 4
			Dim denseNOut As Integer = 5
			Dim finalNOut As Integer = 42
			Dim kernel() As Integer = { 2, 2, 2 }
			Dim activations() As Activation = { Activation.SIGMOID }
			Dim poolModes() As Subsampling3DLayer.PoolingType = { Subsampling3DLayer.PoolingType.AVG }
			Dim modes() As ConvolutionMode = { ConvolutionMode.Truncate }
			For Each afn As Activation In activations
				For Each miniBatchSize As Integer In minibatchSizes
					For Each pool As Subsampling3DLayer.PoolingType In poolModes
						For Each mode As ConvolutionMode In modes
							For Each df As Convolution3D.DataFormat In System.Enum.GetValues(GetType(Convolution3D.DataFormat))
								Dim outDepth As Integer = depth \ kernel(0)
								Dim outHeight As Integer = height \ kernel(1)
								Dim outWidth As Integer = width \ kernel(2)
								Dim input As INDArray = Nd4j.rand(If(df = Convolution3D.DataFormat.NCDHW, New Integer() { miniBatchSize, convNIn, depth, height, width }, New Integer()){ miniBatchSize, depth, height, width, convNIn })
								Dim labels As INDArray = Nd4j.zeros(miniBatchSize, finalNOut)
								For i As Integer = 0 To miniBatchSize - 1
									labels.putScalar(New Integer() { i, i Mod finalNOut }, 1.0)
								Next i
								Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(WeightInit.XAVIER).dist(New NormalDistribution(0, 1)).list().layer(0, (New Convolution3D.Builder()).activation(afn).kernelSize(1, 1, 1).nIn(convNIn).nOut(convNOut).hasBias(False).convolutionMode(mode).dataFormat(df).build()).layer(1, (New Subsampling3DLayer.Builder(kernel)).poolingType(pool).convolutionMode(mode).dataFormat(df).build()).layer(2, (New DenseLayer.Builder()).nOut(denseNOut).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).inputPreProcessor(2, New Cnn3DToFeedForwardPreProcessor(outDepth, outHeight, outWidth, convNOut, df)).setInputType(InputType.convolutional3D(df, depth, height, width, convNIn)).build()
								Dim json As String = conf.toJson()
								Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
								assertEquals(conf, c2)
								Dim net As New MultiLayerNetwork(conf)
								net.init()
								Dim msg As String = "Minibatch size = " & miniBatchSize & ", activationFn=" & afn & ", kernel = " & Arrays.toString(kernel) & ", mode = " & mode.ToString() & ", input depth " & depth & ", input height " & height & ", input width " & width & ", dataFormat=" & df
								If PRINT_RESULTS Then
									log.info(msg)
								End If
								Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
								assertTrue(gradOK,msg)
								TestUtils.testModelSerialization(net)
							Next df
						Next mode
					Next pool
				Next miniBatchSize
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 3 D Upsampling") void testCnn3DUpsampling()
		Friend Overridable Sub testCnn3DUpsampling()
			Nd4j.Random.setSeed(42)
			Dim depth As Integer = 2
			Dim height As Integer = 2
			Dim width As Integer = 2
			Dim minibatchSizes() As Integer = { 3 }
			Dim convNIn As Integer = 2
			Dim convNOut As Integer = 4
			Dim denseNOut As Integer = 5
			Dim finalNOut As Integer = 42
			Dim upsamplingSize() As Integer = { 2, 2, 2 }
			Dim activations() As Activation = { Activation.SIGMOID }
			Dim modes() As ConvolutionMode = { ConvolutionMode.Truncate }
			For Each afn As Activation In activations
				For Each miniBatchSize As Integer In minibatchSizes
					For Each mode As ConvolutionMode In modes
						For Each df As Convolution3D.DataFormat In System.Enum.GetValues(GetType(Convolution3D.DataFormat))
							Dim outDepth As Integer = depth * upsamplingSize(0)
							Dim outHeight As Integer = height * upsamplingSize(1)
							Dim outWidth As Integer = width * upsamplingSize(2)
							Dim input As INDArray = If(df = Convolution3D.DataFormat.NCDHW, Nd4j.rand(miniBatchSize, convNIn, depth, height, width), Nd4j.rand(miniBatchSize, depth, height, width, convNIn))
							Dim labels As INDArray = Nd4j.zeros(miniBatchSize, finalNOut)
							For i As Integer = 0 To miniBatchSize - 1
								labels.putScalar(New Integer() { i, i Mod finalNOut }, 1.0)
							Next i
							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(WeightInit.LECUN_NORMAL).dist(New NormalDistribution(0, 1)).seed(12345).list().layer(0, (New Convolution3D.Builder()).activation(afn).kernelSize(1, 1, 1).nIn(convNIn).nOut(convNOut).hasBias(False).convolutionMode(mode).dataFormat(df).build()).layer(1, (New Upsampling3D.Builder(upsamplingSize(0))).dataFormat(df).build()).layer(2, (New DenseLayer.Builder()).nOut(denseNOut).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).inputPreProcessor(2, New Cnn3DToFeedForwardPreProcessor(outDepth, outHeight, outWidth, convNOut, True)).setInputType(InputType.convolutional3D(df, depth, height, width, convNIn)).build()
							Dim json As String = conf.toJson()
							Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
							assertEquals(conf, c2)
							Dim net As New MultiLayerNetwork(conf)
							net.init()
							Dim msg As String = "Minibatch size = " & miniBatchSize & ", activationFn=" & afn & ", kernel = " & Arrays.toString(upsamplingSize) & ", mode = " & mode.ToString() & ", input depth " & depth & ", input height " & height & ", input width " & width
							If PRINT_RESULTS Then
								log.info(msg)
								' for (int j = 0; j < net.getnLayers(); j++) {
								' log.info("Layer " + j + " # params: " + net.getLayer(j).numParams());
								' }
							End If
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
							assertTrue(gradOK,msg)
							TestUtils.testModelSerialization(net)
						Next df
					Next mode
				Next miniBatchSize
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 3 D Cropping") void testCnn3DCropping()
		Friend Overridable Sub testCnn3DCropping()
			Nd4j.Random.setSeed(42)
			Dim depth As Integer = 6
			Dim height As Integer = 6
			Dim width As Integer = 6
			Dim minibatchSizes() As Integer = { 3 }
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim denseNOut As Integer = 5
			Dim finalNOut As Integer = 8
			Dim kernel() As Integer = { 1, 1, 1 }
			Dim cropping() As Integer = { 0, 0, 1, 1, 2, 2 }
			Dim activations() As Activation = { Activation.SIGMOID }
			Dim modes() As ConvolutionMode = { ConvolutionMode.Same }
			For Each afn As Activation In activations
				For Each miniBatchSize As Integer In minibatchSizes
					For Each mode As ConvolutionMode In modes
						Dim outDepth As Integer = If(mode = ConvolutionMode.Same, depth, (depth - kernel(0)) + 1)
						Dim outHeight As Integer = If(mode = ConvolutionMode.Same, height, (height - kernel(1)) + 1)
						Dim outWidth As Integer = If(mode = ConvolutionMode.Same, width, (width - kernel(2)) + 1)
						outDepth -= cropping(0) + cropping(1)
						outHeight -= cropping(2) + cropping(3)
						outWidth -= cropping(4) + cropping(5)
						Dim input As INDArray = Nd4j.rand(New Integer() { miniBatchSize, convNIn, depth, height, width })
						Dim labels As INDArray = Nd4j.zeros(miniBatchSize, finalNOut)
						For i As Integer = 0 To miniBatchSize - 1
							labels.putScalar(New Integer() { i, i Mod finalNOut }, 1.0)
						Next i
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(WeightInit.LECUN_NORMAL).dist(New NormalDistribution(0, 1)).list().layer(0, (New Convolution3D.Builder()).activation(afn).kernelSize(kernel).nIn(convNIn).nOut(convNOut1).hasBias(False).convolutionMode(mode).dataFormat(Convolution3D.DataFormat.NCDHW).build()).layer(1, (New Convolution3D.Builder()).activation(afn).kernelSize(1, 1, 1).nIn(convNOut1).nOut(convNOut2).hasBias(False).convolutionMode(mode).dataFormat(Convolution3D.DataFormat.NCDHW).build()).layer(2, (New Cropping3D.Builder(cropping)).build()).layer(3, (New DenseLayer.Builder()).nOut(denseNOut).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).inputPreProcessor(3, New Cnn3DToFeedForwardPreProcessor(outDepth, outHeight, outWidth, convNOut2, True)).setInputType(InputType.convolutional3D(depth, height, width, convNIn)).build()
						Dim json As String = conf.toJson()
						Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
						assertEquals(conf, c2)
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim msg As String = "Minibatch size = " & miniBatchSize & ", activationFn=" & afn & ", kernel = " & Arrays.toString(kernel) & ", mode = " & mode.ToString() & ", input depth " & depth & ", input height " & height & ", input width " & width
						If PRINT_RESULTS Then
							log.info(msg)
							' for (int j = 0; j < net.getnLayers(); j++) {
							' log.info("Layer " + j + " # params: " + net.getLayer(j).numParams());
							' }
						End If
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						assertTrue(gradOK,msg)
						TestUtils.testModelSerialization(net)
					Next mode
				Next miniBatchSize
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Deconv 3 d") void testDeconv3d()
		Friend Overridable Sub testDeconv3d()
			Nd4j.Random.setSeed(12345)
			' Note: we checked this with a variety of parameters, but it takes a lot of time.
			Dim depths() As Integer = { 8, 8, 9 }
			Dim heights() As Integer = { 8, 9, 9 }
			Dim widths() As Integer = { 8, 8, 9 }
			Dim kernels()() As Integer = {
				New Integer() { 2, 2, 2 },
				New Integer() { 3, 3, 3 },
				New Integer() { 2, 3, 2 }
			}
			Dim strides()() As Integer = {
				New Integer() { 1, 1, 1 },
				New Integer() { 1, 1, 1 },
				New Integer() { 2, 2, 2 }
			}
			Dim activations() As Activation = { Activation.SIGMOID, Activation.TANH, Activation.IDENTITY }
			Dim modes() As ConvolutionMode = { ConvolutionMode.Truncate, ConvolutionMode.Same, ConvolutionMode.Same }
			Dim mbs() As Integer = { 1, 3, 2 }
			Dim dataFormats() As Convolution3D.DataFormat = { Convolution3D.DataFormat.NCDHW, Convolution3D.DataFormat.NDHWC, Convolution3D.DataFormat.NCDHW }
			Dim convNIn As Integer = 2
			Dim finalNOut As Integer = 2
			Dim deconvOut() As Integer = { 2, 3, 4 }
			For i As Integer = 0 To activations.Length - 1
				Dim afn As Activation = activations(i)
				Dim miniBatchSize As Integer = mbs(i)
				Dim depth As Integer = depths(i)
				Dim height As Integer = heights(i)
				Dim width As Integer = widths(i)
				Dim mode As ConvolutionMode = modes(i)
				Dim kernel() As Integer = kernels(i)
				Dim stride() As Integer = strides(i)
				Dim df As Convolution3D.DataFormat = dataFormats(i)
				Dim dOut As Integer = deconvOut(i)
				Dim input As INDArray
				If df = Convolution3D.DataFormat.NDHWC Then
					input = Nd4j.rand(New Integer() { miniBatchSize, depth, height, width, convNIn })
				Else
					input = Nd4j.rand(New Integer() { miniBatchSize, convNIn, depth, height, width })
				End If
				Dim labels As INDArray = Nd4j.zeros(miniBatchSize, finalNOut)
				For j As Integer = 0 To miniBatchSize - 1
					labels.putScalar(New Integer() { j, j Mod finalNOut }, 1.0)
				Next j
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(New NormalDistribution(0, 0.1)).list().layer(0, (New Convolution3D.Builder()).activation(afn).kernelSize(kernel).stride(stride).nIn(convNIn).nOut(dOut).hasBias(False).convolutionMode(mode).dataFormat(df).build()).layer(1, (New Deconvolution3D.Builder()).activation(afn).kernelSize(kernel).stride(stride).nOut(dOut).hasBias(False).convolutionMode(mode).dataFormat(df).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).setInputType(InputType.convolutional3D(df, depth, height, width, convNIn)).build()
				Dim json As String = conf.toJson()
				Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
				assertEquals(conf, c2)
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim msg As String = "DataFormat = " & df & ", minibatch size = " & miniBatchSize & ", activationFn=" & afn & ", kernel = " & Arrays.toString(kernel) & ", stride = " & Arrays.toString(stride) & ", mode = " & mode.ToString() & ", input depth " & depth & ", input height " & height & ", input width " & width
				If PRINT_RESULTS Then
					log.info(msg)
				End If
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(64))
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub
	End Class

End Namespace