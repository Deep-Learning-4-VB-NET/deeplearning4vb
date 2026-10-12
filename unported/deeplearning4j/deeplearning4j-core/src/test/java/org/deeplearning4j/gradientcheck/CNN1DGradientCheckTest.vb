Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping1D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping1D
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Convolution1DUtils = org.deeplearning4j.util.Convolution1DUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports DisplayName = org.junit.jupiter.api.DisplayName
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
Namespace org.deeplearning4j.gradientcheck

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Cnn 1 D Gradient Check Test") @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Disabled("To be looked in to") class CNN1DGradientCheckTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CNN1DGradientCheckTest
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
				Return 180000
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 1 D With Locally Connected 1 D") void testCnn1DWithLocallyConnected1D()
		Friend Overridable Sub testCnn1DWithLocallyConnected1D()
			Nd4j.Random.setSeed(1337)
			Dim minibatchSizes() As Integer = { 2, 3 }
			Dim length As Integer = 7
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim finalNOut As Integer = 4
			Dim kernels() As Integer = { 1 }
			Dim stride As Integer = 1
			Dim padding As Integer = 0
			Dim activations() As Activation = { Activation.SIGMOID }
			For Each afn As Activation In activations
				For Each minibatchSize As Integer In minibatchSizes
					For Each kernel As Integer In kernels
						Dim input As INDArray = Nd4j.rand(New Integer() { minibatchSize, convNIn, length })
						Dim labels As INDArray = Nd4j.zeros(minibatchSize, finalNOut, length)
						For i As Integer = 0 To minibatchSize - 1
							For j As Integer = 0 To length - 1
								labels.putScalar(New Integer() { i, i Mod finalNOut, j }, 1.0)
							Next j
						Next i
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1)).convolutionMode(ConvolutionMode.Same).list().layer((New Convolution1DLayer.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nIn(convNIn).nOut(convNOut1).rnnDataFormat(RNNFormat.NCW).build()).layer((New LocallyConnected1D.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nIn(convNOut1).nOut(convNOut2).hasBias(False).build()).layer((New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).setInputType(InputType.recurrent(convNIn, length)).build()
						Dim json As String = conf.toJson()
						Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
						assertEquals(conf, c2)
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim msg As String = "Minibatch=" & minibatchSize & ", activationFn=" & afn & ", kernel = " & kernel
						If PRINT_RESULTS Then
							Console.WriteLine(msg)
							' for (int j = 0; j < net.getnLayers(); j++)
							' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
						End If
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						assertTrue(gradOK,msg)
						TestUtils.testModelSerialization(net)
					Next kernel
				Next minibatchSize
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 1 D With Cropping 1 D") void testCnn1DWithCropping1D()
		Friend Overridable Sub testCnn1DWithCropping1D()
			Nd4j.Random.setSeed(1337)
			Dim minibatchSizes() As Integer = { 1, 3 }
			Dim length As Integer = 7
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim finalNOut As Integer = 4
			Dim kernels() As Integer = { 1, 2, 4 }
			Dim stride As Integer = 1
			Dim padding As Integer = 0
			Dim cropping As Integer = 1
			Dim croppedLength As Integer = length - 2 * cropping
			Dim activations() As Activation = { Activation.SIGMOID }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG, SubsamplingLayer.PoolingType.PNORM }
			For Each afn As Activation In activations
				For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
					For Each minibatchSize As Integer In minibatchSizes
						For Each kernel As Integer In kernels
							Dim input As INDArray = Nd4j.rand(New Integer() { minibatchSize, convNIn, length })
							Dim labels As INDArray = Nd4j.zeros(minibatchSize, finalNOut, croppedLength)
							For i As Integer = 0 To minibatchSize - 1
								For j As Integer = 0 To croppedLength - 1
									labels.putScalar(New Integer() { i, i Mod finalNOut, j }, 1.0)
								Next j
							Next i
							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1)).convolutionMode(ConvolutionMode.Same).list().layer((New Convolution1DLayer.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nOut(convNOut1).build()).layer((New Cropping1D.Builder(cropping)).build()).layer((New Convolution1DLayer.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nOut(convNOut2).build()).layer((New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).setInputType(InputType.recurrent(convNIn, length, RNNFormat.NCW)).build()
							Dim json As String = conf.toJson()
							Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
							assertEquals(conf, c2)
							Dim net As New MultiLayerNetwork(conf)
							net.init()
							Dim msg As String = "PoolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn & ", kernel = " & kernel
							If PRINT_RESULTS Then
								Console.WriteLine(msg)
								' for (int j = 0; j < net.getnLayers(); j++)
								' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
							End If
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
											   assertTrue(gradOK,msg)

							TestUtils.testModelSerialization(net)
						Next kernel
					Next minibatchSize
				Next poolingType
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 1 D With Zero Padding 1 D") void testCnn1DWithZeroPadding1D()
		Friend Overridable Sub testCnn1DWithZeroPadding1D()
			Nd4j.Random.setSeed(1337)
			Dim minibatchSizes() As Integer = { 1, 3 }
			Dim length As Integer = 7
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim finalNOut As Integer = 4
			Dim kernels() As Integer = { 1, 2, 4 }
			Dim stride As Integer = 1
			Dim pnorm As Integer = 2
			Dim padding As Integer = 0
			Dim zeroPadding As Integer = 2
			Dim paddedLength As Integer = length + 2 * zeroPadding
			Dim activations() As Activation = { Activation.SIGMOID }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG, SubsamplingLayer.PoolingType.PNORM }
			For Each afn As Activation In activations
				For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
					For Each minibatchSize As Integer In minibatchSizes
						For Each kernel As Integer In kernels
							Dim input As INDArray = Nd4j.rand(New Integer() { minibatchSize, convNIn, length })
							Dim labels As INDArray = Nd4j.zeros(minibatchSize, finalNOut, paddedLength)
							For i As Integer = 0 To minibatchSize - 1
								For j As Integer = 0 To paddedLength - 1
									labels.putScalar(New Integer() { i, i Mod finalNOut, j }, 1.0)
								Next j
							Next i
							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1)).convolutionMode(ConvolutionMode.Same).list().layer((New Convolution1DLayer.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nOut(convNOut1).build()).layer((New ZeroPadding1DLayer.Builder(zeroPadding)).build()).layer((New Convolution1DLayer.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nOut(convNOut2).build()).layer((New ZeroPadding1DLayer.Builder(0)).build()).layer((New Subsampling1DLayer.Builder(poolingType)).kernelSize(kernel).stride(stride).padding(padding).pnorm(pnorm).build()).layer((New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).setInputType(InputType.recurrent(convNIn, length, RNNFormat.NCW)).build()
							Dim json As String = conf.toJson()
							Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
							assertEquals(conf, c2)
							Dim net As New MultiLayerNetwork(conf)
							net.init()
							Dim msg As String = "PoolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn & ", kernel = " & kernel
							If PRINT_RESULTS Then
								Console.WriteLine(msg)
								' for (int j = 0; j < net.getnLayers(); j++)
								' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
							End If
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
											   assertTrue(gradOK,msg)

							TestUtils.testModelSerialization(net)
						Next kernel
					Next minibatchSize
				Next poolingType
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 1 D With Subsampling 1 D") void testCnn1DWithSubsampling1D()
		Friend Overridable Sub testCnn1DWithSubsampling1D()
			Nd4j.Random.setSeed(12345)
			Dim minibatchSizes() As Integer = { 1, 3 }
			Dim length As Integer = 7
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim finalNOut As Integer = 4
			Dim kernels() As Integer = { 1, 2, 4 }
			Dim stride As Integer = 1
			Dim padding As Integer = 0
			Dim pnorm As Integer = 2
			Dim activations() As Activation = { Activation.SIGMOID, Activation.TANH }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG, SubsamplingLayer.PoolingType.PNORM }
			For Each afn As Activation In activations
				For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
					For Each minibatchSize As Integer In minibatchSizes
						For Each kernel As Integer In kernels
							Dim input As INDArray = Nd4j.rand(New Integer() { minibatchSize, convNIn, length })
							Dim labels As INDArray = Nd4j.zeros(minibatchSize, finalNOut, length)
							For i As Integer = 0 To minibatchSize - 1
								For j As Integer = 0 To length - 1
									labels.putScalar(New Integer() { i, i Mod finalNOut, j }, 1.0)
								Next j
							Next i
							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1)).convolutionMode(ConvolutionMode.Same).list().layer(0, (New Convolution1DLayer.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nOut(convNOut1).build()).layer(1, (New Convolution1DLayer.Builder()).activation(afn).kernelSize(kernel).stride(stride).padding(padding).nOut(convNOut2).build()).layer(2, (New Subsampling1DLayer.Builder(poolingType)).kernelSize(kernel).stride(stride).padding(padding).pnorm(pnorm).build()).layer(3, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).setInputType(InputType.recurrent(convNIn, length, RNNFormat.NCW)).build()
							Dim json As String = conf.toJson()
							Dim c2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
							assertEquals(conf, c2)
							Dim net As New MultiLayerNetwork(conf)
							net.init()
							Dim msg As String = "PoolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn & ", kernel = " & kernel
							If PRINT_RESULTS Then
								Console.WriteLine(msg)
								' for (int j = 0; j < net.getnLayers(); j++)
								' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
							End If
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
											   assertTrue(gradOK,msg)

							TestUtils.testModelSerialization(net)
						Next kernel
					Next minibatchSize
				Next poolingType
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 1 d With Masking") void testCnn1dWithMasking()
		Friend Overridable Sub testCnn1dWithMasking()
			Dim length As Integer = 12
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim finalNOut As Integer = 3
			Dim pnorm As Integer = 2
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG }
			For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
				For Each cm As ConvolutionMode In New ConvolutionMode() { ConvolutionMode.Same, ConvolutionMode.Truncate }
					For Each stride As Integer In New Integer() { 1, 2 }
						Dim s As String = cm & ", stride=" & stride & ", pooling=" & poolingType
						log.info("Starting test: " & s)
						Nd4j.Random.setSeed(12345)
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.TANH).dist(New NormalDistribution(0, 1)).convolutionMode(cm).seed(12345).list().layer((New Convolution1DLayer.Builder()).kernelSize(2).rnnDataFormat(RNNFormat.NCW).stride(stride).nIn(convNIn).nOut(convNOut1).build()).layer((New Subsampling1DLayer.Builder(poolingType)).kernelSize(2).stride(stride).pnorm(pnorm).build()).layer((New Convolution1DLayer.Builder()).kernelSize(2).rnnDataFormat(RNNFormat.NCW).stride(stride).nIn(convNOut1).nOut(convNOut2).build()).layer(New GlobalPoolingLayer(PoolingType.AVG)).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).setInputType(InputType.recurrent(convNIn, length)).build()
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim f As INDArray = Nd4j.rand(New Integer() { 2, convNIn, length })
						Dim fm As INDArray = Nd4j.create(2, length)
						fm.get(NDArrayIndex.point(0), NDArrayIndex.all()).assign(1)
						fm.get(NDArrayIndex.point(1), NDArrayIndex.interval(0, 6)).assign(1)
						Dim label As INDArray = TestUtils.randomOneHot(2, finalNOut)
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(f).labels(label).inputMask(fm))
						assertTrue(gradOK,s)
						TestUtils.testModelSerialization(net)
						' TODO also check that masked step values don't impact forward pass, score or gradients
						Dim ds As New DataSet(f, label, fm, Nothing)
						Dim scoreBefore As Double = net.score(ds)
						net.Input = f
						net.Labels = label
						net.setLayerMaskArrays(fm, Nothing)
						net.computeGradientAndScore()
						Dim gradBefore As INDArray = net.getFlattenedGradients().dup()
						f.putScalar(1, 0, 10, 10.0)
						f.putScalar(1, 1, 11, 20.0)
						Dim scoreAfter As Double = net.score(ds)
						net.Input = f
						net.Labels = label
						net.setLayerMaskArrays(fm, Nothing)
						net.computeGradientAndScore()
						Dim gradAfter As INDArray = net.getFlattenedGradients().dup()
						assertEquals(scoreBefore, scoreAfter, 1e-6)
						assertEquals(gradBefore, gradAfter)
					Next stride
				Next cm
			Next poolingType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn 1 Causal") void testCnn1Causal() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCnn1Causal()
			Dim convNIn As Integer = 2
			Dim convNOut1 As Integer = 3
			Dim convNOut2 As Integer = 4
			Dim finalNOut As Integer = 3
			Dim lengths() As Integer = { 11, 12, 13, 9, 10, 11 }
			Dim kernels() As Integer = { 2, 3, 2, 4, 2, 3 }
			Dim dilations() As Integer = { 1, 1, 2, 1, 2, 1 }
			Dim strides() As Integer = { 1, 2, 1, 2, 1, 1 }
			Dim masks() As Boolean = { False, True, False, True, False, True }
			Dim hasB() As Boolean = { True, False, True, False, True, True }
			For i As Integer = 0 To lengths.Length - 1
				Dim length As Integer = lengths(i)
				Dim k As Integer = kernels(i)
				Dim d As Integer = dilations(i)
				Dim st As Integer = strides(i)
				Dim mask As Boolean = masks(i)
				Dim hasBias As Boolean = hasB(i)
				' TODO has bias
				Dim s As String = "k=" & k & ", s=" & st & " d=" & d & ", seqLen=" & length
				log.info("Starting test: " & s)
				Nd4j.Random.setSeed(12345)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.TANH).weightInit(New NormalDistribution(0, 1)).seed(12345).list().layer((New Convolution1DLayer.Builder()).kernelSize(k).dilation(d).hasBias(hasBias).convolutionMode(ConvolutionMode.Causal).stride(st).nOut(convNOut1).build()).layer((New Convolution1DLayer.Builder()).kernelSize(k).dilation(d).convolutionMode(ConvolutionMode.Causal).stride(st).nOut(convNOut2).build()).layer((New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(finalNOut).build()).setInputType(InputType.recurrent(convNIn, length, RNNFormat.NCW)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim f As INDArray = Nd4j.rand(DataType.DOUBLE, 2, convNIn, length)
				Dim fm As INDArray = Nothing
				If mask Then
					fm = Nd4j.create(2, length)
					fm.get(NDArrayIndex.point(0), NDArrayIndex.all()).assign(1)
					fm.get(NDArrayIndex.point(1), NDArrayIndex.interval(0, length - 2)).assign(1)
				End If
				Dim outSize1 As Long = Convolution1DUtils.getOutputSize(length, k, st, 0, ConvolutionMode.Causal, d)
				Dim outSize2 As Long = Convolution1DUtils.getOutputSize(outSize1, k, st, 0, ConvolutionMode.Causal, d)
				Dim label As INDArray = TestUtils.randomOneHotTimeSeries(2, finalNOut, CInt(outSize2))
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(f).labels(label).inputMask(fm))
				assertTrue(gradOK,s)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub
	End Class

End Namespace