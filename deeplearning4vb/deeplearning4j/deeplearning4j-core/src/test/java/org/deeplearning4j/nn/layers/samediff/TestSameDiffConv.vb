Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports GradientCheckUtil = org.deeplearning4j.gradientcheck.GradientCheckUtil
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports SameDiffConv = org.deeplearning4j.nn.layers.samediff.testlayers.SameDiffConv
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions
import static org.junit.Assume.assumeTrue

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

Namespace org.deeplearning4j.nn.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class TestSameDiffConv extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSameDiffConv
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffConvBasic()
		Public Overridable Sub testSameDiffConvBasic()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim kH As Integer = 2
			Dim kW As Integer = 3

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New SameDiffConv.Builder()).nIn(nIn).nOut(nOut).kernelSize(kH, kW).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim pt1 As IDictionary(Of String, INDArray) = net.getLayer(0).paramTable()
			assertNotNull(pt1)
			assertEquals(2, pt1.Count)
			assertNotNull(pt1(ConvolutionParamInitializer.WEIGHT_KEY))
			assertNotNull(pt1(ConvolutionParamInitializer.BIAS_KEY))

			assertArrayEquals(New Long(){kH, kW, nIn, nOut}, pt1(ConvolutionParamInitializer.WEIGHT_KEY).shape())
			assertArrayEquals(New Long(){1, nOut}, pt1(ConvolutionParamInitializer.BIAS_KEY).shape())

			TestUtils.testModelSerialization(net)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Failure on gpu") public void testSameDiffConvForward()
		Public Overridable Sub testSameDiffConvForward()

			Dim imgH As Integer = 16
			Dim imgW As Integer = 20

			Dim count As Integer = 0

			'Note: to avoid the exponential number of tests here, we'll randomly run every Nth test only.
			'With n=1, m=3 this is 1 out of every 3 tests (on average)
			Dim r As New Random(12345)
			For Each minibatch As Integer In New Integer(){5, 1}

				Dim afns() As Activation = { Activation.TANH, Activation.SIGMOID, Activation.ELU, Activation.IDENTITY, Activation.SOFTPLUS, Activation.SOFTSIGN, Activation.CUBE, Activation.HARDTANH, Activation.RELU }

				For Each hasBias As Boolean In New Boolean(){True, False}
					For Each nIn As Integer In New Integer(){3, 4}
						For Each nOut As Integer In New Integer(){4, 5}
							For Each kernel As Integer() In New Integer()(){
								New Integer() {2, 2},
								New Integer() {2, 1},
								New Integer() {3, 2}
							}
								For Each strides As Integer() In New Integer()(){
									New Integer() {1, 1},
									New Integer() {2, 2},
									New Integer() {2, 1}
								}
									For Each dilation As Integer() In New Integer()(){
										New Integer() {1, 1},
										New Integer() {2, 2},
										New Integer() {1, 2}
									}
										For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
											For Each a As Activation In afns
												If r.Next(80) <> 0 Then
													Continue For '1 of 80 on average - of 3888 possible combinations here -> ~49 tests
												End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String msg = "Test " + (count++) + " - minibatch=" + minibatch + ", nIn=" + nIn + ", nOut=" + nOut + ", kernel=" + java.util.Arrays.toString(kernel) + ", stride=" + java.util.Arrays.toString(strides) + ", dilation=" + java.util.Arrays.toString(dilation) + ", ConvolutionMode=" + cm + ", ActFn=" + a + ", hasBias=" + hasBias;
												Dim msg As String = "Test " & (count) & " - minibatch=" & minibatch & ", nIn=" & nIn & ", nOut=" & nOut & ", kernel=" & Arrays.toString(kernel) & ", stride=" & Arrays.toString(strides) & ", dilation=" & Arrays.toString(dilation) & ", ConvolutionMode=" & cm & ", ActFn=" & a & ", hasBias=" & hasBias
													count += 1
												log.info("Starting test: " & msg)

												Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).list().layer((New SameDiffConv.Builder()).weightInit(WeightInit.XAVIER).nIn(nIn).nOut(nOut).kernelSize(kernel).stride(strides).dilation(dilation).convolutionMode(cm).activation(a).hasBias(hasBias).build()).layer((New SameDiffConv.Builder()).weightInit(WeightInit.XAVIER).nIn(nOut).nOut(nOut).kernelSize(kernel).stride(strides).dilation(dilation).convolutionMode(cm).activation(a).hasBias(hasBias).build()).build()

												Dim net As New MultiLayerNetwork(conf)
												net.init()

												assertNotNull(net.paramTable())

												Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(WeightInit.XAVIER).seed(12345).list().layer((New ConvolutionLayer.Builder()).nIn(nIn).nOut(nOut).kernelSize(kernel).stride(strides).dilation(dilation).convolutionMode(cm).activation(a).hasBias(hasBias).build()).layer((New ConvolutionLayer.Builder()).nIn(nOut).nOut(nOut).kernelSize(kernel).stride(strides).dilation(dilation).convolutionMode(cm).activation(a).hasBias(hasBias).build()).build()

												Dim net2 As New MultiLayerNetwork(conf2)
												net2.init()

												'Check params: note that samediff/libnd4j conv params are [kH, kW, iC, oC]
												'DL4J are [nOut, nIn, kH, kW]
												Dim params1 As IDictionary(Of String, INDArray) = net.paramTable()
												Dim params2 As IDictionary(Of String, INDArray) = net2.paramTable()
												For Each e As KeyValuePair(Of String, INDArray) In params1.SetOfKeyValuePairs()
													If e.Key.EndsWith("_W") Then
														Dim p1 As INDArray = e.Value
														Dim p2 As INDArray = params2(e.Key)
														p2 = p2.permute(2, 3, 1, 0)
														p1.assign(p2)
													Else
														assertEquals(params2(e.Key), e.Value)
													End If
												Next e

												Dim [in] As INDArray = Nd4j.rand(New Integer(){minibatch, nIn, imgH, imgW})
												Dim [out] As INDArray = net.output([in])
												Dim outExp As INDArray = net2.output([in])

												assertEquals(outExp, [out], msg)

												'Also check serialization:
												Dim netLoaded As MultiLayerNetwork = TestUtils.testModelSerialization(net)
												Dim outLoaded As INDArray = netLoaded.output([in])

												assertEquals(outExp, outLoaded, msg)

												'Sanity check on different minibatch sizes:
												Dim newIn As INDArray = Nd4j.vstack([in], [in])
												Dim outMbsd As INDArray = net.output(newIn)
												Dim outMb As INDArray = net2.output(newIn)
												assertEquals(outMb, outMbsd)
											Next a
										Next cm
									Next dilation
								Next strides
							Next kernel
						Next nOut
					Next nIn
				Next hasBias
			Next minibatch
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffConvGradient()
		Public Overridable Sub testSameDiffConvGradient()
			Dim imgH As Integer = 8
			Dim imgW As Integer = 8
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim kernel() As Integer = {2, 2}
			Dim strides() As Integer = {1, 1}
			Dim dilation() As Integer = {1, 1}

			Dim count As Integer = 0

			'Note: to avoid the exporential number of tests here, we'll randomly run every Nth test only.
			'With n=1, m=3 this is 1 out of every 3 tests (on average)
			Dim r As New Random(12345)
			Dim n As Integer = 1
			Dim m As Integer = 5
			For Each workspaces As Boolean In New Boolean(){False, True}
				For Each minibatch As Integer In New Integer(){5, 1}
					For Each hasBias As Boolean In New Boolean(){True, False}
						For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
							Dim i As Integer = r.Next(m)
							If i >= n Then
								'Example: n=2, m=3... skip on i=2, run test on i=0, i=1
								Continue For
							End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String msg = "Test " + (count++) + " - minibatch=" + minibatch + ", ConvolutionMode=" + cm + ", hasBias=" + hasBias;
							Dim msg As String = "Test " & (count) & " - minibatch=" & minibatch & ", ConvolutionMode=" & cm & ", hasBias=" & hasBias
								count += 1

							Dim outH As Integer = If(cm = ConvolutionMode.Same, imgH, (imgH-2))
							Dim outW As Integer = If(cm = ConvolutionMode.Same, imgW, (imgW-2))

							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).updater(New NoOp()).trainingWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).inferenceWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).list().layer((New SameDiffConv.Builder()).weightInit(WeightInit.XAVIER).nIn(nIn).nOut(nOut).kernelSize(kernel).stride(strides).dilation(dilation).convolutionMode(cm).activation(Activation.TANH).hasBias(hasBias).build()).layer((New SameDiffConv.Builder()).weightInit(WeightInit.XAVIER).nIn(nOut).nOut(nOut).kernelSize(kernel).stride(strides).dilation(dilation).convolutionMode(cm).activation(Activation.SIGMOID).hasBias(hasBias).build()).layer((New OutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(nOut * outH * outW).nOut(nOut).build()).inputPreProcessor(2, New CnnToFeedForwardPreProcessor(outH, outW, nOut)).build()

							Dim net As New MultiLayerNetwork(conf)
							net.init()

							Dim f As INDArray = Nd4j.rand(New Integer(){minibatch, nIn, imgH, imgW})
							Dim l As INDArray = TestUtils.randomOneHot(minibatch, nOut)

							log.info("Starting: " & msg)
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(f).labels(l).subset(True).maxPerParam(50))

							assertTrue(gradOK, msg)

							TestUtils.testModelSerialization(net)

							'Sanity check on different minibatch sizes:
							Dim newIn As INDArray = Nd4j.vstack(f, f)
							net.output(newIn)
						Next cm
					Next hasBias
				Next minibatch
			Next workspaces
		End Sub
	End Class

End Namespace