Imports System
Imports System.Collections.Generic
Imports lombok
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports CnnLossLayer = org.deeplearning4j.nn.conf.layers.CnnLossLayer
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports ZeroPaddingLayer = org.deeplearning4j.nn.conf.layers.ZeroPaddingLayer
Imports Cropping2D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping2D
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports ComposableInputPreProcessor = org.deeplearning4j.nn.conf.preprocessor.ComposableInputPreProcessor
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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
Namespace org.deeplearning4j.nn.layers.convolution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Disabled("Fails on gpu, to be revisited") public class ConvDataFormatTests extends org.deeplearning4j.BaseDL4JTest
	Public Class ConvDataFormatTests
		Inherits BaseDL4JTest


		Public Shared Function params() As Stream(Of Arguments)
			Dim args As IList(Of Arguments) = New List(Of Arguments)()
			For Each nd4jBackend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
				For Each dataType As DataType In Arrays.asList(New DataType(){DataType.FLOAT, DataType.DOUBLE})
					args.Add(Arguments.of(dataType,nd4jBackend))
				Next dataType
			Next nd4jBackend
			Return args.stream()
		End Function


		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 999999999L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testConv2d(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2d(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & cm & ")", "No helpers (" & cm & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getConv2dNet(dataType,CNN2DFormat.NCHW, True, cm)).net2(getConv2dNet(dataType,CNN2DFormat.NCHW, False, cm)).net3(getConv2dNet(dataType,CNN2DFormat.NHWC, True, cm)).net4(getConv2dNet(dataType,CNN2DFormat.NHWC, False, cm)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next cm
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testSubsampling2d(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSubsampling2d(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & cm & ")", "No helpers (" & cm & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getSubsampling2dNet(dataType,CNN2DFormat.NCHW, True, cm)).net2(getSubsampling2dNet(dataType,CNN2DFormat.NCHW, False, cm)).net3(getSubsampling2dNet(dataType,CNN2DFormat.NHWC, True, cm)).net4(getSubsampling2dNet(dataType,CNN2DFormat.NHWC, False, cm)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next cm
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testDepthwiseConv2d(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDepthwiseConv2d(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & cm & ")", "No helpers (" & cm & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getDepthwiseConv2dNet(dataType,CNN2DFormat.NCHW, True, cm)).net2(getDepthwiseConv2dNet(dataType,CNN2DFormat.NCHW, False, cm)).net3(getDepthwiseConv2dNet(dataType,CNN2DFormat.NHWC, True, cm)).net4(getDepthwiseConv2dNet(dataType,CNN2DFormat.NHWC, False, cm)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next cm
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testSeparableConv2d(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSeparableConv2d(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & cm & ")", "No helpers (" & cm & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getSeparableConv2dNet(dataType,CNN2DFormat.NCHW, True, cm)).net2(getSeparableConv2dNet(dataType,CNN2DFormat.NCHW, False, cm)).net3(getSeparableConv2dNet(dataType,CNN2DFormat.NHWC, True, cm)).net4(getSeparableConv2dNet(dataType,CNN2DFormat.NHWC, False, cm)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next cm
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testDeconv2d(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDeconv2d(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & cm & ")", "No helpers (" & cm & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getDeconv2DNet2dNet(dataType,CNN2DFormat.NCHW, True, cm)).net2(getDeconv2DNet2dNet(dataType,CNN2DFormat.NCHW, False, cm)).net3(getDeconv2DNet2dNet(dataType,CNN2DFormat.NHWC, True, cm)).net4(getDeconv2DNet2dNet(dataType,CNN2DFormat.NHWC, False, cm)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next cm
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testLRN(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLRN(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & cm & ")", "No helpers (" & cm & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getLrnLayer(dataType,CNN2DFormat.NCHW, True, cm)).net2(getLrnLayer(dataType,CNN2DFormat.NCHW, False, cm)).net3(getLrnLayer(dataType,CNN2DFormat.NHWC, True, cm)).net4(getLrnLayer(dataType,CNN2DFormat.NHWC, False, cm)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next cm
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testZeroPaddingLayer(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZeroPaddingLayer(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					Nd4j.Random.setSeed(12345)
					Nd4j.Environment.allowHelpers(helpers)
					Dim msg As String = If(helpers, "With helpers", "No helpers")
					Console.WriteLine(" --- " & msg & " ---")

					Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
					Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

					Dim tc As TestCase = TestCase.builder().msg(msg).net1(getZeroPaddingNet(dataType,CNN2DFormat.NCHW, True)).net2(getZeroPaddingNet(dataType,CNN2DFormat.NCHW, False)).net3(getZeroPaddingNet(dataType,CNN2DFormat.NHWC, True)).net4(getZeroPaddingNet(dataType,CNN2DFormat.NHWC, False)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

					testHelper(tc)
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testCropping2DLayer(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCropping2DLayer(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					Nd4j.Random.setSeed(12345)
					Nd4j.Environment.allowHelpers(helpers)
					Dim msg As String = If(helpers, "With helpers", "No helpers")
					Console.WriteLine(" --- " & msg & " ---")

					Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
					Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

					Dim tc As TestCase = TestCase.builder().msg(msg).net1(getCropping2dNet(dataType,CNN2DFormat.NCHW, True)).net2(getCropping2dNet(dataType,CNN2DFormat.NCHW, False)).net3(getCropping2dNet(dataType,CNN2DFormat.NHWC, True)).net4(getCropping2dNet(dataType,CNN2DFormat.NHWC, False)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

					testHelper(tc)
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testUpsampling2d(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUpsampling2d(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					Nd4j.Random.setSeed(12345)
					Nd4j.Environment.allowHelpers(helpers)
					Dim msg As String = If(helpers, "With helpers", "No helpers")
					Console.WriteLine(" --- " & msg & " ---")

					Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
					Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

					Dim tc As TestCase = TestCase.builder().msg(msg).net1(getUpsamplingNet(dataType,CNN2DFormat.NCHW, True)).net2(getUpsamplingNet(dataType,CNN2DFormat.NCHW, False)).net3(getUpsamplingNet(dataType,CNN2DFormat.NHWC, True)).net4(getUpsamplingNet(dataType,CNN2DFormat.NHWC, False)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

					testHelper(tc)
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testBatchNormNet(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBatchNormNet(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each useLogStd As Boolean In New Boolean(){True, False}
					For Each helpers As Boolean In New Boolean(){False, True}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = (If(helpers, "With helpers", "No helpers")) & " - " & (If(useLogStd, "logstd", "std"))
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getBatchNormNet(dataType,useLogStd, CNN2DFormat.NCHW, True)).net2(getBatchNormNet(dataType,useLogStd, CNN2DFormat.NCHW, False)).net3(getBatchNormNet(dataType,useLogStd, CNN2DFormat.NHWC, True)).net4(getBatchNormNet(dataType,useLogStd, CNN2DFormat.NHWC, False)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next helpers
				Next useLogStd
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testCnnLossLayer(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCnnLossLayer(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					Nd4j.Random.setSeed(12345)
					Nd4j.Environment.allowHelpers(helpers)
					Dim msg As String = If(helpers, "With helpers", "No helpers")
					Console.WriteLine(" --- " & msg & " ---")

					Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
					Dim labelsNHWC As INDArray = TestUtils.randomOneHot(dataType,2*6*6, 3)
					labelsNHWC = labelsNHWC.reshape(ChrW(2), 6, 6, 3)
					Dim labelsNCHW As INDArray = labelsNHWC.permute(0,3,1,2).dup()



					Dim tc As TestCase = TestCase.builder().msg(msg).net1(getCnnLossNet(CNN2DFormat.NCHW, True, ConvolutionMode.Same)).net2(getCnnLossNet(CNN2DFormat.NCHW, False, ConvolutionMode.Same)).net3(getCnnLossNet(CNN2DFormat.NHWC, True, ConvolutionMode.Same)).net4(getCnnLossNet(CNN2DFormat.NHWC, False, ConvolutionMode.Same)).inNCHW(inNCHW).labelsNCHW(labelsNCHW).labelsNHWC(labelsNHWC).testLayerIdx(1).nhwcOutput(True).build()

					testHelper(tc)
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testSpaceToDepthNet(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpaceToDepthNet(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					Nd4j.Random.setSeed(12345)
					Nd4j.Environment.allowHelpers(helpers)
					Dim msg As String = If(helpers, "With helpers", "No helpers")
					Console.WriteLine(" --- " & msg & " ---")

					Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
					Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

					Dim tc As TestCase = TestCase.builder().msg(msg).net1(getSpaceToDepthNet(dataType,CNN2DFormat.NCHW, True)).net2(getSpaceToDepthNet(dataType,CNN2DFormat.NCHW, False)).net3(getSpaceToDepthNet(dataType,CNN2DFormat.NHWC, True)).net4(getSpaceToDepthNet(dataType,CNN2DFormat.NHWC, False)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

					testHelper(tc)
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testSpaceToBatchNet(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpaceToBatchNet(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					Nd4j.Random.setSeed(12345)
					Nd4j.Environment.allowHelpers(helpers)
					Dim msg As String = If(helpers, "With helpers", "No helpers")
					Console.WriteLine(" --- " & msg & " ---")

					Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 16, 16)
					Dim labels As INDArray = TestUtils.randomOneHot(8, 10)

					Dim tc As TestCase = TestCase.builder().msg(msg).net1(getSpaceToBatchNet(dataType,CNN2DFormat.NCHW, True)).net2(getSpaceToBatchNet(dataType,CNN2DFormat.NCHW, False)).net3(getSpaceToBatchNet(dataType,CNN2DFormat.NHWC, True)).net4(getSpaceToBatchNet(dataType,CNN2DFormat.NHWC, False)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

					testHelper(tc)
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testLocallyConnected(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLocallyConnected(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Truncate, ConvolutionMode.Same}
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & cm & ")", "No helpers (" & cm & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getLocallyConnectedNet(dataType,CNN2DFormat.NCHW, True, cm)).net2(getLocallyConnectedNet(dataType,CNN2DFormat.NCHW, False, cm)).net3(getLocallyConnectedNet(dataType,CNN2DFormat.NHWC, True, cm)).net4(getLocallyConnectedNet(dataType,CNN2DFormat.NHWC, False, cm)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next cm
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.convolution.ConvDataFormatTests#params") @ParameterizedTest public void testGlobalPooling(org.nd4j.linalg.api.buffer.DataType dataType,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGlobalPooling(ByVal dataType As DataType, ByVal backend As Nd4jBackend)
			Try
				For Each helpers As Boolean In New Boolean(){False, True}
					For Each pt As PoolingType In System.Enum.GetValues(GetType(PoolingType))
						Nd4j.Random.setSeed(12345)
						Nd4j.Environment.allowHelpers(helpers)
						Dim msg As String = If(helpers, "With helpers (" & pt & ")", "No helpers (" & pt & ")")
						Console.WriteLine(" --- " & msg & " ---")

						Dim inNCHW As INDArray = Nd4j.rand(dataType, 2, 3, 12, 12)
						Dim labels As INDArray = TestUtils.randomOneHot(2, 10)

						Dim tc As TestCase = TestCase.builder().msg(msg).net1(getGlobalPoolingNet(dataType,CNN2DFormat.NCHW, pt, True)).net2(getGlobalPoolingNet(dataType,CNN2DFormat.NCHW, pt, False)).net3(getGlobalPoolingNet(dataType,CNN2DFormat.NHWC, pt, True)).net4(getGlobalPoolingNet(dataType,CNN2DFormat.NHWC, pt, False)).inNCHW(inNCHW).labelsNCHW(labels).labelsNHWC(labels).testLayerIdx(1).build()

						testHelper(tc)
					Next pt
				Next helpers
			Finally
				Nd4j.Environment.allowHelpers(True)
			End Try
		End Sub

		Private Function getConv2dNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).dataFormat(format).nOut(3).helperAllowFallback(False).build(), format, cm, Nothing)
			Else
				Return getNetWithLayer(dataType,(New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).nOut(3).helperAllowFallback(False).build(), format, cm, Nothing)
			End If
		End Function

		Private Function getSubsampling2dNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(1, 1).dataFormat(format).helperAllowFallback(False).build(), format, cm, Nothing)
			Else
				Return getNetWithLayer(dataType,(New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(1, 1).helperAllowFallback(False).build(), format, cm, Nothing)
			End If
		End Function

		Private Function getSeparableConv2dNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New SeparableConvolution2D.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).dataFormat(format).nOut(3).helperAllowFallback(False).build(), format, cm, Nothing)
			Else
				Return getNetWithLayer(dataType,(New SeparableConvolution2D.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).nOut(3).helperAllowFallback(False).build(), format, cm, Nothing)
			End If
		End Function

		Private Function getDepthwiseConv2dNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New DepthwiseConvolution2D.Builder()).depthMultiplier(2).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).dataFormat(format).nOut(3).helperAllowFallback(False).build(), format, cm, Nothing)
			Else
				Return getNetWithLayer(dataType,(New DepthwiseConvolution2D.Builder()).depthMultiplier(2).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).nOut(3).helperAllowFallback(False).build(), format, cm, Nothing)
			End If
		End Function

		Private Function getLrnLayer(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New LocalResponseNormalization.Builder()).dataFormat(format).helperAllowFallback(False).build(), format, cm, Nothing)
			Else
				Return getNetWithLayer(dataType,(New LocalResponseNormalization.Builder()).helperAllowFallback(False).build(), format, cm, Nothing)
			End If
		End Function

		Private Function getZeroPaddingNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New ZeroPaddingLayer.Builder(2,2)).dataFormat(format).build(), format, ConvolutionMode.Same, Nothing)
			Else
				Return getNetWithLayer(dataType,(New ZeroPaddingLayer.Builder(2,2)).build(), format, ConvolutionMode.Same, Nothing)
			End If
		End Function

		Private Function getCropping2dNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
			   Return getNetWithLayer(dataType,(New Cropping2D.Builder(2,2)).dataFormat(format).build(), format, ConvolutionMode.Same, Nothing)
			Else
				Return getNetWithLayer(dataType,(New Cropping2D.Builder(2,2)).build(), format, ConvolutionMode.Same, Nothing)
			End If
		End Function

		Private Function getUpsamplingNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New Upsampling2D.Builder(2)).dataFormat(format).build(), format, ConvolutionMode.Same, Nothing)
			Else
				Return getNetWithLayer(dataType,(New Upsampling2D.Builder(2)).build(), format, ConvolutionMode.Same, Nothing)
			End If
		End Function

		Private Function getDeconv2DNet2dNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New Deconvolution2D.Builder()).nOut(2).activation(Activation.TANH).kernelSize(2,2).dataFormat(format).stride(2,2).build(), format, cm, Nothing)
			Else
				Return getNetWithLayer(dataType,(New Deconvolution2D.Builder()).nOut(2).activation(Activation.TANH).kernelSize(2,2).dataFormat(format).stride(2,2).build(), format, cm, Nothing)
			End If
		End Function

		Private Function getBatchNormNet(ByVal dataType As DataType, ByVal logStdev As Boolean, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New BatchNormalization.Builder()).useLogStd(logStdev).dataFormat(format).helperAllowFallback(False).nOut(3).build(), format, ConvolutionMode.Same, Nothing)
			Else
				Return getNetWithLayer(dataType,(New BatchNormalization.Builder()).useLogStd(logStdev).helperAllowFallback(False).nOut(3).build(), format, ConvolutionMode.Same, Nothing)
			End If
		End Function

		Private Function getSpaceToDepthNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New SpaceToDepthLayer.Builder()).blocks(2).dataFormat(format).build(), format, ConvolutionMode.Same, Nothing)
			Else
				Return getNetWithLayer(dataType,(New SpaceToDepthLayer.Builder()).blocks(2).build(), format, ConvolutionMode.Same, Nothing)
			End If
		End Function

		Private Function getSpaceToBatchNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New SpaceToBatchLayer.Builder()).blocks(2, 2).dataFormat(format).build(), format, ConvolutionMode.Same, InputType.convolutional(16, 16, 3, format))
			Else
				Return getNetWithLayer(dataType,(New SpaceToBatchLayer.Builder()).blocks(2, 2).build(), format, ConvolutionMode.Same, InputType.convolutional(16, 16, 3, format))
			End If
		End Function

		Private Function getLocallyConnectedNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New LocallyConnected2D.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).dataFormat(format).nOut(3).build(), format, cm, Nothing)
			Else
				Return getNetWithLayer(dataType,(New LocallyConnected2D.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).nOut(3).build(), format, cm, Nothing)
			End If
		End Function

		Private Function getNetWithLayer(ByVal dataType As DataType, ByVal layer As Layer, ByVal format As CNN2DFormat, ByVal cm As ConvolutionMode, ByVal inputType As InputType) As MultiLayerNetwork
			Dim builder As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).dataType(dataType).seed(12345).convolutionMode(cm).list().layer((New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).nOut(3).helperAllowFallback(False).build()).layer(layer).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(If(inputType IsNot Nothing, inputType, InputType.convolutional(12, 12, 3, format)))

			If format = CNN2DFormat.NHWC AndAlso Not (TypeOf layer Is GlobalPoolingLayer) Then
				'Add a preprocessor due to the differences in how NHWC and NCHW activations are flattened
				'DL4J's flattening behaviour matches Keras (hence TF) for import compatibility
				builder.inputPreProcessor(2, New ComposableInputPreProcessor(New NHWCToNCHWPreprocessor(), New CnnToFeedForwardPreProcessor()))
			End If

			Dim net As New MultiLayerNetwork(builder.build())
			net.init()
			Return net
		End Function

		Private Function getGlobalPoolingNet(ByVal dataType As DataType, ByVal format As CNN2DFormat, ByVal pt As PoolingType, ByVal setOnLayerAlso As Boolean) As MultiLayerNetwork
			If setOnLayerAlso Then
				Return getNetWithLayer(dataType,(New GlobalPoolingLayer.Builder(pt)).poolingDimensions(If(format = CNN2DFormat.NCHW, New Integer(){2, 3}, New Integer()){1,2}).build(), format, ConvolutionMode.Same, Nothing)
			Else
				Return getNetWithLayer(dataType,(New GlobalPoolingLayer.Builder(pt)).build(), format, ConvolutionMode.Same, Nothing)
			End If
		End Function

		Private Function getCnnLossNet(ByVal format As CNN2DFormat, ByVal setOnLayerAlso As Boolean, ByVal cm As ConvolutionMode) As MultiLayerNetwork
			Dim builder As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).convolutionMode(cm).list().layer((New ConvolutionLayer.Builder()).kernelSize(3, 3).stride(2, 2).activation(Activation.TANH).dataFormat(format).nOut(3).helperAllowFallback(False).build())
			If setOnLayerAlso Then
				builder.layer((New CnnLossLayer.Builder()).format(format).activation(Activation.SOFTMAX).build())
			Else
				builder.layer((New CnnLossLayer.Builder()).activation(Activation.SOFTMAX).build())
			End If

			builder.InputType = InputType.convolutional(12, 12, 3, format)

			Dim net As New MultiLayerNetwork(builder.build())
			net.init()
			Return net
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @NoArgsConstructor @Builder private static class TestCase
		Private Class TestCase
			Friend msg As String
			Friend net1 As MultiLayerNetwork
			Friend net2 As MultiLayerNetwork
			Friend net3 As MultiLayerNetwork
			Friend net4 As MultiLayerNetwork
			Friend inNCHW As INDArray
			Friend labelsNCHW As INDArray
			Friend labelsNHWC As INDArray
			Friend testLayerIdx As Integer
			Friend nhwcOutput As Boolean
		End Class

		Public Shared Sub testHelper(ByVal tc As TestCase)

			tc.net2.params().assign(tc.net1.params())
			tc.net3.params().assign(tc.net1.params())
			tc.net4.params().assign(tc.net1.params())

			'Test forward pass:
			Dim inNCHW As INDArray = tc.inNCHW
			Dim inNHWC As INDArray = tc.inNCHW.permute(0, 2, 3, 1).dup()

			Dim l0_1 As INDArray = tc.net1.feedForward(inNCHW)(tc.testLayerIdx + 1)
			Dim l0_2 As INDArray = tc.net2.feedForward(inNCHW)(tc.testLayerIdx + 1)
			Dim l0_3 As INDArray = tc.net3.feedForward(inNHWC)(tc.testLayerIdx + 1)
			Dim l0_4 As INDArray = tc.net4.feedForward(inNHWC)(tc.testLayerIdx + 1)

			assertEquals(l0_1, l0_2,tc.msg)
			If l0_1.rank() = 4 Then
				assertEquals(l0_1, l0_3.permute(0, 3, 1, 2),tc.msg)
				assertEquals(l0_1, l0_4.permute(0, 3, 1, 2),tc.msg)
			Else
				assertEquals(l0_1, l0_3,tc.msg)
				assertEquals(l0_1, l0_4,tc.msg)
			End If


			Dim out1 As INDArray = tc.net1.output(inNCHW)
			Dim out2 As INDArray = tc.net2.output(inNCHW)
			Dim out3 As INDArray = tc.net3.output(inNHWC)
			Dim out4 As INDArray = tc.net4.output(inNHWC)

			assertEquals(out1, out2,tc.msg)
			If Not tc.nhwcOutput Then
				assertEquals(out1, out3,tc.msg)
				assertEquals(out1, out4,tc.msg)
			Else
				assertEquals(out1, out3.permute(0,3,1,2),tc.msg) 'NHWC to NCHW
				assertEquals(out1, out4.permute(0,3,1,2),tc.msg)
			End If

			'Test backprop
			Dim p1 As Pair(Of Gradient, INDArray) = tc.net1.calculateGradients(inNCHW, tc.labelsNCHW, Nothing, Nothing)
			Dim p2 As Pair(Of Gradient, INDArray) = tc.net2.calculateGradients(inNCHW, tc.labelsNCHW, Nothing, Nothing)
			Dim p3 As Pair(Of Gradient, INDArray) = tc.net3.calculateGradients(inNHWC, tc.labelsNHWC, Nothing, Nothing)
			Dim p4 As Pair(Of Gradient, INDArray) = tc.net4.calculateGradients(inNHWC, tc.labelsNHWC, Nothing, Nothing)

				'Inpput gradients
			assertEquals(p1.Second, p2.Second,tc.msg)
			assertEquals(p1.Second, p3.Second.permute(0,3,1,2),tc.msg) 'Input gradients for NHWC input are also in NHWC format
			assertEquals(p1.Second, p4.Second.permute(0,3,1,2),tc.msg)

			Dim diff12 As IList(Of String) = differentGrads(p1.First, p2.First)
			Dim diff13 As IList(Of String) = differentGrads(p1.First, p3.First)
			Dim diff14 As IList(Of String) = differentGrads(p1.First, p4.First)
			assertEquals(0, diff12.Count,tc.msg & " " & diff12)
			assertEquals(0, diff13.Count,tc.msg & " " & diff13)
			assertEquals(0, diff14.Count,tc.msg & " " & diff14)

			assertEquals(p1.First.gradientForVariable(), p2.First.gradientForVariable(),tc.msg)
			assertEquals(p1.First.gradientForVariable(), p3.First.gradientForVariable(),tc.msg)
			assertEquals(p1.First.gradientForVariable(), p4.First.gradientForVariable(),tc.msg)

			tc.net1.fit(inNCHW, tc.labelsNCHW)
			tc.net2.fit(inNCHW, tc.labelsNCHW)
			tc.net3.fit(inNHWC, tc.labelsNHWC)
			tc.net4.fit(inNHWC, tc.labelsNHWC)

			assertEquals(tc.net1.params(), tc.net2.params(),tc.msg)
			assertEquals(tc.net1.params(), tc.net3.params(),tc.msg)
			assertEquals(tc.net1.params(), tc.net4.params(),tc.msg)

			'Test serialization
			Dim net1a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net1)
			Dim net2a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net2)
			Dim net3a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net3)
			Dim net4a As MultiLayerNetwork = TestUtils.testModelSerialization(tc.net4)

			out1 = tc.net1.output(inNCHW)
			assertEquals(out1, net1a.output(inNCHW),tc.msg)
			assertEquals(out1, net2a.output(inNCHW),tc.msg)
			If Not tc.nhwcOutput Then
				assertEquals(out1, net3a.output(inNHWC),tc.msg)
				assertEquals(out1, net4a.output(inNHWC),tc.msg)
			Else
				assertEquals(out1, net3a.output(inNHWC).permute(0,3,1,2),tc.msg) 'NHWC to NCHW
				assertEquals(out1, net4a.output(inNHWC).permute(0,3,1,2),tc.msg)
			End If

		End Sub

		Private Shared Function differentGrads(ByVal g1 As Gradient, ByVal g2 As Gradient) As IList(Of String)
			Dim differs As IList(Of String) = New List(Of String)()
			Dim m1 As IDictionary(Of String, INDArray) = g1.gradientForVariable()
			Dim m2 As IDictionary(Of String, INDArray) = g2.gradientForVariable()
			For Each s As String In m1.Keys
				Dim a1 As INDArray = m1(s)
				Dim a2 As INDArray = m2(s)
				If Not a1.Equals(a2) Then
					differs.Add(s)
				End If
			Next s
			Return differs
		End Function


		'Converts NHWC to NCHW activations
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode private static class NHWCToNCHWPreprocessor implements InputPreProcessor
		<Serializable>
		Private Class NHWCToNCHWPreprocessor
			Implements InputPreProcessor

			Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input.permute(0,3,1,2))
			End Function

			Public Overridable Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, output.permute(0,2,3,1))
			End Function

			Public Overridable Function clone() As InputPreProcessor Implements InputPreProcessor.clone
				Return Me
			End Function

			Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
				Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
				Return InputType.convolutional(c.getHeight(), c.getWidth(), c.getChannels(), CNN2DFormat.NCHW)
			End Function

			Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
				Return Nothing
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWrongFormatIn()
		Public Overridable Sub testWrongFormatIn()

			For Each df As CNN2DFormat In CNN2DFormat.values()
				For i As Integer = 0 To 3
					Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).list()
					Select Case i
						Case 0
							b.layer((New ConvolutionLayer.Builder()).kernelSize(2,2).nIn(3).nOut(3).dataFormat(df).build())
							b.InputType = InputType.convolutional(12,12,3,df)
						Case 1
							b.layer((New DepthwiseConvolution2D.Builder()).kernelSize(2,2).nIn(3).nOut(3).dataFormat(df).build())
							b.InputType = InputType.convolutional(12,12,3,df)
						Case 2
							b.layer((New Deconvolution2D.Builder()).dataFormat(df).kernelSize(2,2).nIn(3).nOut(3).build())
							b.InputType = InputType.convolutional(12,12,3,df)
						Case 3
							b.layer((New SeparableConvolution2D.Builder()).dataFormat(df).kernelSize(2,2).nIn(3).nOut(3).build())
							b.InputType = InputType.convolutional(12,12,3,df)
					End Select


					Dim net As New MultiLayerNetwork(b.build())
					net.init()

					Dim [in] As INDArray
					Dim wrongFormatIn As INDArray
					If df = CNN2DFormat.NCHW Then
						[in] = Nd4j.create(DataType.FLOAT, 5, 3, 12, 12)
						wrongFormatIn = Nd4j.create(DataType.FLOAT, 5, 12, 12, 3)
					Else
						[in] = Nd4j.create(DataType.FLOAT, 5, 12, 12, 3)
						wrongFormatIn = Nd4j.create(DataType.FLOAT, 5, 3, 12, 12)
					End If

					net.output([in])

					Try
						net.output(wrongFormatIn)
					Catch e As DL4JInvalidInputException
	'                    e.printStackTrace();
						Dim msg As String = e.Message
						assertTrue(msg.Contains(ConvolutionUtils.NCHW_NHWC_ERROR_MSG) OrElse msg.Contains("input array channels does not match CNN layer configuration"),msg)
					End Try
				Next i
			Next df


		End Sub
	End Class

End Namespace