Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TestInfo = org.junit.jupiter.api.TestInfo
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports TestCase = org.nd4j.autodiff.validation.TestCase
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports AvgPooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.AvgPooling2D
Imports DepthwiseConv2D = org.nd4j.linalg.api.ops.impl.layers.convolution.DepthwiseConv2D
Imports Pooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports Pooling2DDerivative = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2DDerivative
Imports Conv1DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Conv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig
Imports DeConv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv2DConfig
Imports DeConv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig
Imports LocalResponseNormalizationConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.LocalResponseNormalizationConfig
Imports PaddingMode = org.nd4j.linalg.api.ops.impl.layers.convolution.config.PaddingMode
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
Imports Pooling3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig
Imports GRU = org.nd4j.linalg.api.ops.impl.layers.recurrent.GRU
Imports LSTMActivations = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMActivations
Imports LSTMDataFormat = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMDataFormat
Imports LSTMDirectionMode = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMDirectionMode
Imports LSTMLayerConfig = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig
Imports LSTMLayerOutputs = org.nd4j.linalg.api.ops.impl.layers.recurrent.outputs.LSTMLayerOutputs
Imports LSTMLayerWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights
Imports LayerNorm = org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm
Imports Standardize = org.nd4j.linalg.api.ops.impl.transforms.custom.Standardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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

Namespace org.nd4j.autodiff.opvalidation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) public class LayerOpValidation extends BaseOpValidation
	Public Class LayerOpValidation
		Inherits BaseOpValidation

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testXwPlusB(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testXwPlusB(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sameDiff As SameDiff = SameDiff.create()
			Dim input As INDArray = Nd4j.rand(New Long(){2, 3})
			Dim weights As INDArray = Nd4j.rand(New Long(){3, 4})
			Dim b As INDArray = Nd4j.rand(New Long(){4})

			Dim sdInput As SDVariable = sameDiff.var("input", input)
			Dim sdWeights As SDVariable = sameDiff.var("weights", weights)
			Dim sdBias As SDVariable = sameDiff.var("bias", b)

			Dim res As SDVariable = sameDiff.nn().linear(sdInput, sdWeights, sdBias)
			Dim loss As SDVariable = sameDiff.standardDeviation(res, True)

			Dim exp As INDArray = input.mmul(weights).addiRowVector(b)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(res.name(), exp)

	'        System.out.println(sameDiff.summary());
	'        System.out.println("============================");
			sameDiff.summary()
			sameDiff.createGradFunction()
	'        System.out.println(sameDiff.getFunction("grad").summary());
			sameDiff.getFunction("grad").summary()


			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReluLayer(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReluLayer(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sameDiff As SameDiff = SameDiff.create()
			Dim input As INDArray = Nd4j.rand(New Long(){2, 3})
			Dim weights As INDArray = Nd4j.rand(New Long(){3, 4})
			Dim b As INDArray = Nd4j.rand(New Long(){4})

			Dim sdInput As SDVariable = sameDiff.var("input", input)
			Dim sdWeights As SDVariable = sameDiff.var("weights", weights)
			Dim sdBias As SDVariable = sameDiff.var("bias", b)

			Dim res As SDVariable = sameDiff.nn().reluLayer(sdInput, sdWeights, sdBias)
			Dim loss As SDVariable = sameDiff.standardDeviation(res, True)

			Dim exp As INDArray = input.mmul(weights).addiRowVector(b)
			Transforms.relu(exp, False)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(res.name(), exp)


			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBiasAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBiasAdd(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim sameDiff As SameDiff = SameDiff.create()
			Dim input As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(New Long(){2, 4})
			Dim b As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).divi(4)

			Dim sdInput As SDVariable = sameDiff.var("input", input)
			Dim sdBias As SDVariable = sameDiff.var("bias", b)

			Dim res As SDVariable = sameDiff.nn().biasAdd(sdInput, sdBias, True)
			Dim loss As SDVariable = sameDiff.standardDeviation(res, True)

			Dim exp As INDArray = input.addRowVector(b)

			Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(res.name(), exp)

			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2d(ByVal backend As Nd4jBackend)
			'avg pool, batch norm, conv2d, max pool 2d, pooling2d, upsampling
			'Tested elsewhere: deconv2d, depthwise2d, LRN, sconv2d

			Nd4j.Random.setSeed(12345)

			Dim inputSizes()() As Integer = {
				New Integer() {1, 3, 8, 8}
			} ', {3, 6, 12, 12}};

			Dim failed As IList(Of String) = New List(Of String)()

			For i As Integer = 0 To 7
				For Each inSizeNCHW As Integer() In inputSizes

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = Nothing

					Dim inSize() As Integer

					Dim [out] As SDVariable
					Dim msg As String
					Select Case i
						Case 0
							'Conv2d, with bias, NCHW, same
							msg = "0 - conv2d+bias, nchw - input " & Arrays.toString(inSizeNCHW)
							inSize = inSizeNCHW
							[in] = sd.var("in", inSize)
							Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(New Integer(){3, 3, inSizeNCHW(1), 3}).muli(10)) 'kH,kW,iC,oC
							Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(New Long(){3}).muli(10))
							[out] = sd.cnn().conv2d([in], w0, b0, Conv2DConfig.builder().dataFormat(Conv2DConfig.NCHW).isSameMode(True).kH(3).kW(3).sH(1).sW(1).build())
						Case 1
							'Conv2d, with bias, NHWC, no same
							msg = "1 - conv2d+bias, nhwc - input " & Arrays.toString(inSizeNCHW)
							inSize = nchwToNhwc(inSizeNCHW)
							[in] = sd.var("in", inSize)
							Dim w1 As SDVariable = sd.var("w1", Nd4j.rand(New Integer(){2, 4, inSizeNCHW(1), 3}).muli(10)) 'kH,kW,nIn,nOut
							Dim b1 As SDVariable = sd.var("b1", Nd4j.rand(New Long(){3}).muli(10))
							[out] = sd.cnn().conv2d([in], w1, b1, Conv2DConfig.builder().dataFormat(Conv2DConfig.NHWC_Conflict).isSameMode(False).kH(2).kW(4).sH(2).sW(2).build())
						Case 2
							'Conv2d, no bias, NCHW
							msg = "2 - conv2d, no bias, nchw - input " & Arrays.toString(inSizeNCHW)
							inSize = inSizeNCHW
							[in] = sd.var("in", inSize)
							Dim w2 As SDVariable = sd.var("w0", Nd4j.rand(New Integer(){1, 3, inSizeNCHW(1), 3}).muli(10)) '//kH,kW,iC,oC
							[out] = sd.cnn().conv2d([in], w2, Conv2DConfig.builder().dataFormat(Conv2DConfig.NCHW).isSameMode(True).kH(1).kW(3).sH(1).sW(2).build())
						Case 3
							'Avg pool, NCHW
							msg = "3 - avg pool, NCHW, same - input " & Arrays.toString(inSizeNCHW)
							inSize = inSizeNCHW
							[in] = sd.var("in", inSize)
							[out] = sd.cnn().avgPooling2d([in], Pooling2DConfig.builder().isNHWC(True).isSameMode(True).kH(2).kW(2).sH(1).sW(1).build())
						Case 4
							'Avg pool, NHWC, not same
							msg = "3 - avg pool, NHWC, not same - input " & Arrays.toString(inSizeNCHW)
							inSize = nchwToNhwc(inSizeNCHW)
							[in] = sd.var("in", inSize)
							[out] = sd.cnn().avgPooling2d([in], Pooling2DConfig.builder().isNHWC(True).isSameMode(False).kH(3).kW(2).sH(2).sW(2).build())
						Case 5
							'Avg pool, NCHW
							msg = "5 - avg pool, NCHW, same - input " & Arrays.toString(inSizeNCHW)
							inSize = inSizeNCHW
							[in] = sd.var("in", inSize)
							[out] = sd.cnn().maxPooling2d([in], Pooling2DConfig.builder().isNHWC(False).isSameMode(True).kH(2).kW(2).sH(1).sW(1).build())
						Case 6
							'Max pool, NHWC, not same
							msg = "6 - avg pool, NHWC, not same - input " & Arrays.toString(inSizeNCHW)
							inSize = inSizeNCHW
							[in] = sd.var("in", inSize)
							[out] = sd.cnn().maxPooling2d([in], Pooling2DConfig.builder().isNHWC(True).isSameMode(False).kH(3).kW(2).sH(2).sW(2).build())
						Case 7
							'Upsampling
							msg = "7 - upsampling2d, NCHW, 2x2 - " & Arrays.toString(inSizeNCHW)
							inSize = inSizeNCHW
							[in] = sd.var("in", inSize)
							[out] = sd.cnn().upsampling2d([in], 2, 2, True)
						Case Else
							Throw New Exception()

					End Select

					Dim inArr As INDArray = Nd4j.rand(inSize).muli(10)
					[in].Array = inArr
					Dim loss As SDVariable = sd.standardDeviation("loss", [out], True)

					log.info("Starting test: " & msg)
					Dim tc As New TestCase(sd)
					Dim [error] As String = OpValidation.validate(tc)
					If [error] IsNot Nothing Then
						failed.Add(msg)
					End If

				Next inSizeNCHW
			Next i

			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLrn2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLrn2d(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim inputSizes()() As Integer = {
				New Integer() {1, 3, 8, 8},
				New Integer() {3, 6, 12, 12}
			}

			Dim failed As IList(Of String) = New List(Of String)()

			For Each inSizeNCHW As Integer() In inputSizes

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = Nothing

				Dim inSize() As Integer

				'LRN
				Dim msg As String = "LRN with NCHW - input" & Arrays.toString(inSizeNCHW)
				inSize = inSizeNCHW
				[in] = sd.var("in", inSize)
				Dim [out] As SDVariable = sd.cnn().localResponseNormalization([in], LocalResponseNormalizationConfig.builder().depth(3).bias(1).alpha(1).beta(0.5).build())

				Dim inArr As INDArray = Nd4j.rand(inSize).muli(10)
				[in].Array = inArr
				Dim loss As SDVariable = sd.mean("loss", [out])

				log.info("Starting test: " & msg)
				Dim tc As TestCase = (New TestCase(sd)).gradientCheck(True)
				Dim [error] As String = OpValidation.validate(tc)
				If [error] IsNot Nothing Then
					failed.Add(msg)
				End If

			Next inSizeNCHW
			assertEquals(0, failed.Count,failed.ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2Col(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2Col(ByVal backend As Nd4jBackend)
			'OpValidationSuite.ignoreFailing();      //TEMPORARY DUE TO JVM CRASH: https://github.com/eclipse/deeplearning4j/issues/6873
			Nd4j.Random.setSeed(12345)

			Dim inputSizes()() As Integer = {
				New Integer() {1, 3, 8, 8},
				New Integer() {3, 6, 12, 12}
			}

			Dim failed As IList(Of String) = New List(Of String)()

			For Each inSizeNCHW As Integer() In inputSizes

				Dim sd As SameDiff = SameDiff.create()
				Dim var As SDVariable = sd.var("in", Nd4j.rand(DataType.DOUBLE, inSizeNCHW))
				Dim im2col As SDVariable = sd.cnn().im2Col(var, Conv2DConfig.builder().kH(2).kW(2).sH(1).sW(1).isSameMode(True).build())

				Dim loss As SDVariable = sd.standardDeviation("loss", im2col, True)

				Dim msg As String = Arrays.toString(inSizeNCHW)

				Dim tc As TestCase = (New TestCase(sd)).gradientCheck(True).testName(msg)
				Dim [error] As String = OpValidation.validate(tc)
				If [error] IsNot Nothing Then
					failed.Add(msg)
				End If
			Next inSizeNCHW

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


		Private Shared Function nchwToNhwc(ByVal [in]() As Integer) As Integer()
			Return New Integer(){[in](0), [in](2), [in](3), [in](1)}
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOutputShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOutputShape(ByVal backend As Nd4jBackend)
			Dim inSize() As Long = {1, 8, 8, 3}

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", inSize)
	'        SDVariable out = sd.avgPooling2d(in, );

	'        Pooling2DConfig conf = Pooling2DConfig.builder()
	'                .isNHWC(false)
	'                .isSameMode(false)
	'                .kH(2).kW(2)
	'                .sW(1).sH(1)
	'                .build();

			Dim conf As Pooling2DConfig = Pooling2DConfig.builder().isNHWC(True).isSameMode(False).kH(3).kW(2).sH(2).sW(2).build()

			Dim input As INDArray = Nd4j.create(inSize)
			Dim avgPooling2D As New AvgPooling2D(input, Nothing, conf)

			Dim outSizes As val = Nd4j.Executioner.calculateOutputShape(avgPooling2D)

			assertEquals(1, outSizes.size())

			'NO SAME: out = (in - k + 2*p)/s + 1;
			Dim outH As Integer = (8 - 3) \ 2 + 1
			Dim outW As Integer = (8 - 2) \ 2 + 1
			Dim exp() As Long = {1, outH, outW, 3} 'NHWC

			assertEquals(1, outSizes.size())
			assertArrayEquals(exp, outSizes.get(0).getShape())

			Dim grad As INDArray = Nd4j.create(exp)


			'Test backprop:
			Dim avg2dDeriv As New Pooling2DDerivative(input, grad, Nothing, conf)

			Dim outSizesBP As val = Nd4j.Executioner.calculateOutputShape(avg2dDeriv)
			assertEquals(1, outSizesBP.size())

			assertArrayEquals(inSize, outSizesBP.get(0).getShape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAvgPool(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAvgPool(ByVal backend As Nd4jBackend)
			Dim inSize() As Long = {1, 8, 8, 3} 'NHWC

			Dim conf As Pooling2DConfig = Pooling2DConfig.builder().isNHWC(True).isSameMode(False).kH(3).kW(2).sH(2).sW(2).type(Pooling2D.Pooling2DType.AVG).build()

			Dim input As INDArray = Nd4j.create(inSize)
			Dim avgPooling2D As New AvgPooling2D(input, Nothing, conf)

			Dim outSizes As val = Nd4j.Executioner.calculateOutputShape(avgPooling2D)
			assertEquals(1, outSizes.size())

			'NO SAME: out = (in - k + 2*p)/s + 1;
			Dim outH As Integer = (8 - 3) \ 2 + 1
			Dim outW As Integer = (8 - 2) \ 2 + 1
			Dim exp() As Long = {1, outH, outW, 3} 'NHWC

			assertEquals(1, outSizes.size())
			assertArrayEquals(exp, outSizes.get(0).getShape())

			Dim grad As INDArray = Nd4j.create(exp)

			'Test backprop:
			Dim avg2dDeriv As New Pooling2DDerivative(input, grad, Nd4j.create(inSize), conf)

			Dim outSizesBP As val = Nd4j.Executioner.calculateOutputShape(avg2dDeriv)
			assertEquals(1, outSizesBP.size())
			assertArrayEquals(inSize, outSizesBP.get(0).getShape())

			Nd4j.Executioner.execAndReturn(avg2dDeriv)
		End Sub


		Private Shared Function ncdhwToNdhwc(ByVal [in]() As Integer) As Integer()
			Return New Integer(){[in](0), [in](2), [in](3), [in](4), [in](1)}
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv3d(org.nd4j.linalg.factory.Nd4jBackend backend, org.junit.jupiter.api.TestInfo testInfo)
		Public Overridable Sub testConv3d(ByVal backend As Nd4jBackend, ByVal testInfo As TestInfo)
			'Pooling3d, Conv3D, batch norm
			Nd4j.Random.setSeed(12345)

			'NCDHW format
			Dim inputSizes()() As Integer = {
				New Integer() {2, 3, 4, 5, 5}
			}

			Dim failed As IList(Of String) = New List(Of String)()

			For Each inSizeNCDHW As Integer() In inputSizes
				For Each ncdhw As Boolean In New Boolean(){True, False}
					Dim nIn As Integer = inSizeNCDHW(1)
					Dim shape() As Integer = (If(ncdhw, inSizeNCDHW, ncdhwToNdhwc(inSizeNCDHW)))

					For i As Integer = 0 To 4
						Dim sd As SameDiff = SameDiff.create()
						Dim [in] As SDVariable = sd.var("in", shape)

						Dim [out] As SDVariable
						Dim msg As String
						Select Case i
							Case 0
								'Conv3d, with bias, same
								msg = "0 - conv3d+bias+same, ncdhw=" & ncdhw & " - input " & Arrays.toString(shape)
								Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(New Integer(){2, 2, 2, nIn, 3}).muli(10)) '[kD, kH, kW, iC, oC]
								Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(New Long(){3}).muli(10))
								[out] = sd.cnn().conv3d([in], w0, b0, Conv3DConfig.builder().dataFormat(If(ncdhw, Conv3DConfig.NCDHW_Conflict, Conv3DConfig.NDHWC)).isSameMode(True).kH(2).kW(2).kD(2).sD(1).sH(1).sW(1).build())
							Case 1
								'Conv3d, no bias, no same
								msg = "1 - conv3d+no bias+no same, ncdhw=" & ncdhw & " - input " & Arrays.toString(shape)
								Dim w1 As SDVariable = sd.var("w1", Nd4j.rand(New Integer(){2, 2, 2, nIn, 3}).muli(10)) '[kD, kH, kW, iC, oC]
								[out] = sd.cnn().conv3d([in], w1, Conv3DConfig.builder().dataFormat(If(ncdhw, Conv3DConfig.NCDHW_Conflict, Conv3DConfig.NDHWC)).isSameMode(False).kH(2).kW(2).kD(2).sD(1).sH(1).sW(1).build())
							Case 2
								'pooling3d - average, no same
								msg = "2 - pooling 3d, average, same"
								[out] = sd.cnn().avgPooling3d([in], Pooling3DConfig.builder().kH(2).kW(2).kD(2).sH(1).sW(1).sD(1).isSameMode(False).isNCDHW(ncdhw).build())
							Case 3
								'pooling 3d - max, no same
								msg = "3 - pooling 3d, max, same"
								[out] = sd.cnn().maxPooling3d([in], Pooling3DConfig.builder().kH(2).kW(2).kD(2).sH(1).sW(1).sD(1).isSameMode(True).isNCDHW(ncdhw).build())
							Case 4
								'Deconv3d
								msg = "4 - deconv3d, ncdhw=" & ncdhw
								Dim wDeconv As SDVariable = sd.var(Nd4j.rand(New Integer(){2, 2, 2, 3, nIn})) '[kD, kH, kW, oC, iC]
								Dim bDeconv As SDVariable = sd.var(Nd4j.rand(New Integer(){3}))
								[out] = sd.cnn().deconv3d("Deconv3d", [in], wDeconv, bDeconv, DeConv3DConfig.builder().kD(2).kH(2).kW(2).isSameMode(True).dataFormat(If(ncdhw, DeConv3DConfig.NCDHW, DeConv3DConfig.NDHWC)).build())
							Case 5
								'Batch norm - 3d input
								Throw New Exception("Batch norm test not yet implemented")
							Case Else
								Throw New Exception()
						End Select

						Dim inArr As INDArray = Nd4j.rand(shape).muli(10)
						[in].Array = inArr
						Dim loss As SDVariable = sd.standardDeviation("loss", [out], True)

						log.info("Starting test: " & msg)
						Dim tc As TestCase = (New TestCase(sd)).gradientCheck(True)
						tc.testName(msg)
						Dim [error] As String = OpValidation.validate(tc)
						If [error] IsNot Nothing Then
							failed.Add(testInfo.getTestMethod().get().getName())
						End If
					Next i
				Next ncdhw
			Next inSizeNCDHW

			assertEquals(0, failed.Count,failed.ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDepthWiseConv2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDepthWiseConv2dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim depthWise As Integer = 4
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 28
			Dim imgW As Integer = 28


			Dim sd As SameDiff = SameDiff.create()
			Dim depthWeightArr As INDArray = Nd4j.create(kH, kW, nIn, depthWise)

			Dim bArr As INDArray = Nd4j.create(1, depthWise * nIn)
			Dim inArr As INDArray = Nd4j.create(mb, nIn, imgH, imgW)

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim dW As SDVariable = sd.var("dW", depthWeightArr)
			Dim b As SDVariable = sd.var("b", bArr)

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(False).build()

			Dim [out] As SDVariable = sd.cnn().separableConv2d([in], dW, Nothing, b, c)
			[out] = sd.nn().tanh("out", [out])

			Dim outArr As INDArray = [out].eval()
			'Expected output size: out = (in - k + 2*p)/s + 1 = (28-2+0)/1+1 = 27
			Dim outShape As val = outArr.shape()
			assertArrayEquals(New Long(){mb, depthWise * nIn, 27, 27}, outShape)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSeparableConv2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSeparableConv2dBasic(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 2
			Dim nOut As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 2
			Dim imgH As Integer = 8
			Dim imgW As Integer = 8

			Dim depthWise As Integer = 3

			Dim sd As SameDiff = SameDiff.create()
			Dim depthWeightArr As INDArray = Nd4j.rand(New Integer(){kH, kW, nIn, depthWise})
			Dim pointWeightArr As INDArray = Nd4j.rand(New Integer(){1, 1, nIn * depthWise, nOut})

			Dim bArr As INDArray = Nd4j.rand(New Integer(){nOut})
			Dim inArr As INDArray = Nd4j.rand(New Integer(){mb, nIn, imgH, imgW})

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim dW As SDVariable = sd.var("dW", depthWeightArr)
			Dim pW As SDVariable = sd.var("pW", pointWeightArr)
			Dim b As SDVariable = sd.var("b", bArr)

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(False).dataFormat(Conv2DConfig.NCHW).build()

			Dim [out] As SDVariable = sd.cnn().separableConv2d([in], dW, pW, b, c)
			[out] = sd.nn().tanh("out", [out])

			Dim outArr As INDArray = [out].eval()
			'Expected output size: out = (in - k + 2*p)/s + 1 = (8-2+0)/1+1 = 7
			Dim outShape As val = outArr.shape()
			assertArrayEquals(New Long(){mb, nOut, 7, 7}, outShape)

			Dim loss As SDVariable = [out].std(True)

	'        System.out.println(sd.summary());
	'        System.out.println("--------------------------");
	'        sd.createGradFunction();
	'        System.out.println(sd.getFunction("grad").summary());

			'Gradient check:
			Dim tc As TestCase = (New TestCase(sd)).gradientCheck(True)
			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeconv2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDeconv2dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 2
			Dim nOut As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 2
			Dim imgH As Integer = 8
			Dim imgW As Integer = 8

			Dim sd As SameDiff = SameDiff.create()
			Dim wArr As INDArray = Nd4j.rand(New Integer(){kH, kW, nOut, nIn}) 'Libnd4j expected weights format: [kH, kW, cOut, cIn]
			Dim bArr As INDArray = Nd4j.rand(New Long(){nOut})
			Dim inArr As INDArray = Nd4j.rand(New Long(){mb, nIn, imgH, imgW})

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim w As SDVariable = sd.var("W", wArr)
			Dim b As SDVariable = sd.var("b", bArr)

			Dim deconv As DeConv2DConfig = DeConv2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(False).build()

			Dim [out] As SDVariable = sd.cnn().deconv2d([in], w, b, deconv)
			[out] = sd.nn().tanh("out", [out])

			Dim outArr As INDArray = [out].eval()
			'Expected output size: out = (in + k + 2*p)/ s - 1 = (8 + 2+0)/1 - 1 = 9
			Dim outShape As val = outArr.shape()
			assertArrayEquals(New Long(){mb, nOut, 9, 9}, outShape)

			Dim loss As SDVariable = [out].std(True)
			'Gradient check:
			Dim tc As TestCase = (New TestCase(sd)).gradientCheck(True)
			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 28
			Dim imgW As Integer = 28

			Dim sd As SameDiff = SameDiff.create()
			Dim wArr As INDArray = Nd4j.create(kH, kW, nIn, nOut)
			Dim bArr As INDArray = Nd4j.create(1, nOut)
			Dim inArr As INDArray = Nd4j.create(mb, nIn, imgH, imgW)

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim w As SDVariable = sd.var("W", wArr)
			Dim b As SDVariable = sd.var("b", bArr)

			'Order: https://github.com/deeplearning4j/libnd4j/blob/6c41ea5528bb1f454e92a9da971de87b93ff521f/include/ops/declarable/generic/convo/conv2d.cpp#L20-L22
			'in, w, b - bias is optional

			Dim c As Conv2DConfig = Conv2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(False).build()

			Dim [out] As SDVariable = sd.cnn().conv2d("conv", [in], w, b, c)
			[out] = sd.nn().tanh("out", [out])

			Dim outArr As INDArray = [out].eval()
			'Expected output size: out = (in - k + 2*p)/s + 1 = (28-2+0)/1+1 = 27
			Dim outShape As val = outArr.shape()
			assertArrayEquals(New Long(){mb, nOut, 27, 27}, outShape)
			' sd.execBackwards(); // TODO: test failing here
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxPoolingArgMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxPoolingArgMax(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 8
			Dim imgW As Integer = 8

			Dim sd As SameDiff = SameDiff.create()
			Dim inArr As INDArray = Nd4j.rand(New Integer(){mb, nIn, imgH, imgW})

			Dim [in] As SDVariable = sd.var("in", inArr)

			Dim pooling2DConfig As Pooling2DConfig = Pooling2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(True).build()

			Dim results() As SDVariable = sd.cnn().maxPoolWithArgmax(New String(){"out", "idx"}, [in], pooling2DConfig)
			assertArrayEquals(inArr.shape(), results(0).eval().shape())
			assertArrayEquals(inArr.shape(), results(1).eval().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxPooling2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxPooling2dBasic(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 8
			Dim imgW As Integer = 8

			Dim sd As SameDiff = SameDiff.create()
			Dim inArr As INDArray = Nd4j.rand(New Integer(){mb, nIn, imgH, imgW})

			Dim [in] As SDVariable = sd.var("in", inArr)

			Dim pooling2DConfig As Pooling2DConfig = Pooling2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(False).build()

			Dim outPool As SDVariable = sd.cnn().maxPooling2d([in], pooling2DConfig)
			Dim [out] As SDVariable = sd.nn().tanh("out", outPool)

			Dim outArr As INDArray = [out].eval()
			Dim outShape As val = outArr.shape()
			' oH = (iH - (kH + (kH-1)*(dH-1)) + 2*pH)/sH + 1;
			assertArrayEquals(New Long(){mb, nIn, 7, 7}, outShape)

			Dim loss As SDVariable = [out].std(True)

			Dim exp As INDArray = Nd4j.create(mb, nIn, 7, 7)
			Dim iter As New NdIndexIterator(mb, nIn, 7, 7)
			Do While iter.MoveNext()
				Dim [next]() As Long = iter.Current
				Dim max As Double = Me.max(inArr.getDouble([next]), inArr.getDouble([next](0), [next](1), [next](2) + 1, [next](3)), inArr.getDouble([next](0), [next](1), [next](2), [next](3) + 1), inArr.getDouble([next](0), [next](1), [next](2) + 1, [next](3) + 1))
				exp.putScalar([next], max)
			Loop

			assertNull(OpValidation.validate((New TestCase(sd)).gradientCheck(True).expected(outPool, exp)))
		End Sub

		Private Function max(ParamArray ByVal [in]() As Double) As Double
'JAVA TO VB CONVERTER NOTE: The local variable max was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim max_Conflict As Double = -Double.MaxValue
			For Each d As Double In [in]
				If d > max_Conflict Then
					max_Conflict = d
				End If
			Next d
			Return max_Conflict
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAvgPooling2dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAvgPooling2dBasic(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 8
			Dim imgW As Integer = 8

			Dim sd As SameDiff = SameDiff.create()
			Dim inArr As INDArray = Nd4j.rand(New Integer(){mb, nIn, imgH, imgW})

			Dim [in] As SDVariable = sd.var("in", inArr)

			Dim pooling2DConfig As Pooling2DConfig = Pooling2DConfig.builder().kH(kH).kW(kW).pH(0).pW(0).sH(1).sW(1).dH(1).dW(1).isSameMode(False).build()

			Dim outPool As SDVariable = sd.cnn().avgPooling2d([in], pooling2DConfig)
			Dim [out] As SDVariable = sd.nn().tanh("out", outPool)

			Dim outArr As INDArray = [out].eval()
			Dim outShape As val = outArr.shape()
			' oH = (iH - (kH + (kH-1)*(dH-1)) + 2*pH)/sH + 1;
			assertArrayEquals(New Long(){mb, nIn, 7, 7}, outShape)

			Dim loss As SDVariable = [out].std(True)

			Dim exp As INDArray = Nd4j.create(mb, nIn, 7, 7)
			Dim iter As New NdIndexIterator(mb, nIn, 7, 7)
			Do While iter.MoveNext()
				Dim [next]() As Long = iter.Current
				Dim avg As Double = (inArr.getDouble([next]) + inArr.getDouble([next](0), [next](1), [next](2) + 1, [next](3)) + inArr.getDouble([next](0), [next](1), [next](2), [next](3) + 1) + inArr.getDouble([next](0), [next](1), [next](2) + 1, [next](3) + 1)) / 4.0
				exp.putScalar([next], avg)
			Loop

			assertNull(OpValidation.validate((New TestCase(sd)).expected(outPool, exp).gradientCheck(True)))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAvgPooling3dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAvgPooling3dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim kD As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 5
			Dim imgW As Integer = 5
			Dim imgD As Integer = 5

			Dim sd As SameDiff = SameDiff.create()
			Dim inArr As INDArray = Nd4j.rand(New Long(){mb, nIn, imgD, imgH, imgW})

			Dim [in] As SDVariable = sd.var("in", inArr)

			Dim pooling3DConfig As Pooling3DConfig = Pooling3DConfig.builder().kH(kH).kW(kW).kD(kD).pH(0).pW(0).pD(0).sH(1).sW(1).sD(1).dH(1).dW(1).dD(1).isSameMode(False).isNCDHW(True).build()

			Dim [out] As SDVariable = sd.cnn().avgPooling3d([in], pooling3DConfig)
			[out] = sd.nn().tanh("loss", [out]).shape().rename("out")

			' oH = (iH - (kH + (kH-1)*(dH-1)) + 2*pH)/sH + 1;
			Dim outArr As INDArray = Nd4j.createFromArray(mb, nIn, 4, 4, 4L)

			Dim tc As TestCase = (New TestCase(sd)).expectedOutput("out", outArr).gradientCheck(False)
			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxPooling3dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaxPooling3dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim kD As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 5
			Dim imgW As Integer = 5
			Dim imgD As Integer = 5

			Dim sd As SameDiff = SameDiff.create()
			Dim inArr As INDArray = Nd4j.create(mb, nIn, imgD, imgH, imgW)

			Dim [in] As SDVariable = sd.var("in", inArr)

			Dim pooling3DConfig As Pooling3DConfig = Pooling3DConfig.builder().kH(kH).kW(kW).kD(kD).pH(0).pW(0).pD(0).sH(1).sW(1).sD(1).dH(1).dW(1).dD(1).isSameMode(False).build()

			Dim [out] As SDVariable = sd.cnn().maxPooling3d([in], pooling3DConfig)
			[out] = sd.nn().tanh("loss", [out]).shape().rename("out")

			sd.setLossVariables("loss")

			' oH = (iH - (kH + (kH-1)*(dH-1)) + 2*pH)/sH + 1;
			Dim outArr As INDArray = Nd4j.createFromArray(mb, nIn, 4, 4, 4L)

			Dim tc As TestCase = (New TestCase(sd)).expectedOutput("out", outArr).gradientCheck(False)
			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv1dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv1dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim k As Integer = 2
			Dim mb As Integer = 3
			Dim img As Integer = 28

			Dim sd As SameDiff = SameDiff.create()
			Dim wArr As INDArray = Nd4j.create(k, nIn, nOut)
			Dim inArr As INDArray = Nd4j.create(mb, nIn, img)

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim w As SDVariable = sd.var("W", wArr)

			Dim vars() As SDVariable = {[in], w}

			Dim conv1DConfig As Conv1DConfig = Conv1DConfig.builder().k(k).p(0).s(1).paddingMode(PaddingMode.VALID).build()

			Dim [out] As SDVariable = sd.cnn().conv1d([in], w, conv1DConfig)
			[out] = sd.nn().tanh("loss", [out]).shape().rename("out")

			sd.setLossVariables("loss")

			'Expected output size: out = (in - k + 2*p)/s + 1 = (28-2+0)/1+1 = 27
			Dim outArr As INDArray = Nd4j.createFromArray(mb, nOut, 27L)
			Dim tc As TestCase = (New TestCase(sd)).expectedOutput("out", outArr).gradientCheck(False)
			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv1dCausal(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv1dCausal(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim mb As Integer = 2

			For Each k As Integer In New Integer(){2, 3}
				For Each sz As Integer In New Integer(){3, 4, 5}
					For Each s As Integer In New Integer(){1, 2}
						For Each d As Integer In New Integer(){1, 2}
							For Each ncw As Boolean In New Boolean(){True, False}

								Dim sd As SameDiff = SameDiff.create()
								Dim wArr As INDArray = Nd4j.rand(DataType.DOUBLE, k, nIn, nOut)
								Dim inArr As INDArray = Nd4j.rand(DataType.DOUBLE, (If(ncw, New Long(){mb, nIn, sz}, New Long()){mb, sz, nIn}))
								Dim bArr As INDArray = Nd4j.rand(DataType.DOUBLE, nOut)

								Dim [in] As SDVariable = sd.var("in", inArr)
								Dim w As SDVariable = sd.var("W", wArr)
								Dim b As SDVariable = sd.var("b", bArr)

								Dim conv1DConfig As Conv1DConfig = Conv1DConfig.builder().dataFormat(If(ncw, Conv1DConfig.NCW, Conv1DConfig.NWC_Conflict)).k(k).p(0).s(s).d(d).paddingMode(PaddingMode.CAUSAL).build()

								Dim [out] As SDVariable = sd.cnn().conv1d([in], w, b, conv1DConfig)
								Dim loss As SDVariable = sd.nn().tanh([out]).std(True).rename("loss")

								sd.setLossVariables("loss")

								Dim name As String = "k=" & k & ", sz=" & sz & ", ncw=" & ncw

								Console.WriteLine(name)

								Dim tc As TestCase = (New TestCase(sd)).testName(name).gradientCheck(True)
								Dim err As String = OpValidation.validate(tc)
								assertNull(err)
							Next ncw
						Next d
					Next s
				Next sz
			Next k
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv1dForward(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv1dForward(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 2
			Dim nOut As Integer = 1
			Dim kernel As Integer = 3
			Dim batchSize As Integer = 10
			Dim sequenceSize As Integer = 5

			Dim sd As SameDiff = SameDiff.create()

			Dim inArr As INDArray = Nd4j.linspace(0, nIn * batchSize * sequenceSize, nIn * batchSize * sequenceSize).reshape(ChrW(batchSize), nIn, sequenceSize)

			Dim wArr As INDArray = Nd4j.linspace(0, kernel * nIn * nOut, kernel * nIn * nOut).reshape(ChrW(kernel), nIn, nOut)

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim w As SDVariable = sd.var("w", wArr)

			Dim res As SDVariable = sd.cnn_Conflict.conv1d([in], w, Conv1DConfig.builder().k(kernel).paddingMode(PaddingMode.VALID).build())

			Dim expected As INDArray = Nd4j.createFromArray(New Double()()(){
				New Double()() {
					New Double() {82.42424f, 100.60606f, 118.78788f}
				},
				New Double()() {
					New Double() {264.2424f, 282.4242f, 300.6061f}
				},
				New Double()() {
					New Double() {446.0606f, 464.2424f, 482.424f}
				},
				New Double()() {
					New Double() {627.8788f, 646.0606f, 664.2424f}
				},
				New Double()() {
					New Double() {809.6970f, 827.8788f, 846.0606f}
				},
				New Double()() {
					New Double() {991.5152f, 1009.69696f, 1027.8788f}
				},
				New Double()() {
					New Double() {1173.3333f, 1191.5152f, 1209.6970f}
				},
				New Double()() {
					New Double() {1355.1515f, 1373.3333f, 1391.5153f}
				},
				New Double()() {
					New Double() {1536.9697f, 1555.1515f, 1573.3333f}
				},
				New Double()() {
					New Double() {1718.7878f, 1736.9697f, 1755.1515f}
				}
			})

			Dim tc As TestCase = (New TestCase(sd)).gradientCheck(False).expectedOutput(res.name(), expected)
			Dim err As String = OpValidation.validate(tc)

			assertNull(err)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv3dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv3dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim kD As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 5
			Dim imgW As Integer = 5
			Dim imgT As Integer = 5

			Dim sd As SameDiff = SameDiff.create()
			Dim wArr As INDArray = Nd4j.rand(New Integer(){kD, kH, kW, nIn, nOut})
			Dim bArr As INDArray = Nd4j.rand(1, nOut)
			Dim inArr As INDArray = Nd4j.rand(New Integer(){mb, nIn, imgT, imgH, imgW})

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim w As SDVariable = sd.var("W", wArr)
			Dim b As SDVariable = sd.var("b", bArr)

			Dim conv3DConfig As Conv3DConfig = Conv3DConfig.builder().kH(kH).kW(kW).kD(kD).sD(1).sH(1).sW(1).dH(1).dW(1).dD(1).isSameMode(True).biasUsed(False).dataFormat(Conv3DConfig.NCDHW_Conflict).build()

			Dim [out] As SDVariable = sd.cnn().conv3d([in], w, b, conv3DConfig)
			[out] = sd.nn().tanh("loss", [out]).shape().rename("out")

			sd.setLossVariables("loss")

			'Expected output size, NOT same mode: out = (in - k)/d + 1 = (28-2+0)/1+1 = 27
			'Expected output size, WITH same mode: out = in/stride
			Dim outArr As INDArray = Nd4j.createFromArray(mb, nOut, 5, 5, 5L)

			Dim tc As TestCase = (New TestCase(sd)).expectedOutput("out", outArr).gradientCheck(True)
			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeConv3dBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDeConv3dBasic(ByVal backend As Nd4jBackend)
			Dim nIn As Integer = 4
			Dim nOut As Integer = 3
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim kD As Integer = 2

			Dim mb As Integer = 3
			Dim imgH As Integer = 5
			Dim imgW As Integer = 5
			Dim imgT As Integer = 5

			Dim sd As SameDiff = SameDiff.create()
			Dim inArr As INDArray = Nd4j.rand(New Long(){mb, nIn, 5, 5, 5})
			Dim wArr As INDArray = Nd4j.rand(kD, kH, kW, nOut, nIn)

			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim w As SDVariable = sd.var("W", wArr)

			Dim conv3DConfig As DeConv3DConfig = DeConv3DConfig.builder().kH(kH).kW(kW).kD(kD).sD(1).sH(1).sW(1).dH(1).dW(1).dD(1).isSameMode(True).dataFormat(DeConv3DConfig.NCDHW).build()

			Dim [out] As SDVariable = sd.cnn().deconv3d([in], w, conv3DConfig)
			[out] = sd.nn().tanh("loss", [out]).shape().rename("out")

			sd.setLossVariables("loss")

			'Expected conv3d size, NOT same mode: out = (in - k)/d + 1 = (28-2+0)/1+1 = 27
			'Expected conv3d size, WITH same mode: out = in/stride
			' reversed this for deconv3d
			Dim outArr As INDArray = Nd4j.createFromArray(New Long(){mb, nOut, imgT, imgH, imgW})

			Dim tc As TestCase = (New TestCase(sd)).expectedOutput("out", outArr).gradientCheck(True)
			Dim err As String = OpValidation.validate(tc)
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayerNorm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayerNorm(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 10, 4);
			Dim random As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray standardized = random.ulike();
			Dim standardized As INDArray = random.ulike()
			Nd4j.Executioner.exec(New Standardize(random, standardized, 1))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray gain = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim gain As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray bias = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim bias As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = standardized.mulRowVector(gain).addRowVector(bias);
			Dim res As INDArray = standardized.mulRowVector(gain).addRowVector(bias)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expOut = res.norm1();
			Dim expOut As INDArray = res.norm1()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int[] axis = new int[]{1};
			Dim axis() As Integer = {1}
			Dim sd As SameDiff = SameDiff.create()
			Dim sdInput As SDVariable = sd.var("input", standardized)
			Dim sdGain As SDVariable = sd.var("gain", gain)
			Dim sdBias As SDVariable = sd.var("bias", bias)
			Dim [out] As SDVariable = sd.nn_Conflict.layerNorm(sdInput, sdGain, sdBias, True, axis)
			[out].norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", expOut).gradientCheck(True))
			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayerNorm4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayerNorm4d(ByVal backend As Nd4jBackend)
			Dim mb As Integer = 3
			Dim ch As Integer = 4
			For Each nchw As Boolean In New Boolean(){True, False}
				Dim eps As Double = 0.0
				Dim x As INDArray = Nd4j.rand(DataType.DOUBLE,If(nchw, New Long(){mb, ch, 8, 8}, New Long()){mb, 8, 8, ch})
				Dim gain4d As INDArray = Nd4j.rand(DataType.DOUBLE,If(nchw, New Long(){1, ch, 1, 1}, New Long()){1, 1, 1, ch})
				Dim bias4d As INDArray = Nd4j.rand(DataType.DOUBLE,If(nchw, New Long(){1, ch, 1, 1}, New Long()){1, 1, 1, ch})
				Dim mean As INDArray = x.mean(True, 1, 2, 3)
				Dim std As INDArray = Transforms.sqrt(x.var(False, 1, 2, 3).addi(eps)).reshape(ChrW(mb), 1, 1, 1)

				Dim standardized As INDArray = x.sub(mean).div(std)
				Dim exp As INDArray = standardized.mul(gain4d).add(bias4d)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int[] axis = new int[]{1, 2, 3};
				Dim axis() As Integer = {1, 2, 3}
				Dim sd As SameDiff = SameDiff.create()
				Dim sdInput As SDVariable = sd.var("input", x)
				Dim sdGain As SDVariable = sd.var("gain", gain4d.reshape(ChrW(ch)))
				Dim sdBias As SDVariable = sd.var("bias", bias4d.reshape(ChrW(ch)))
				Dim [out] As SDVariable = sd.nn_Conflict.layerNorm("layernorm", sdInput, sdGain, sdBias, nchw, axis)

				Dim loss As SDVariable = sd.loss_Conflict.l2Loss([out])

				Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("layernorm", exp).gradientCheck(True))
				assertNull(err)
			Next nchw
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayerNormOP(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayerNormOP(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 10, 4);
			Dim random As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray standardized = random.ulike();
			Dim standardized As INDArray = random.ulike()
			Nd4j.Executioner.exec(New Standardize(random, standardized, 1))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray gain = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim gain As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray bias = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim bias As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = standardized.mulRowVector(gain).addRowVector(bias);
			Dim res As INDArray = standardized.mulRowVector(gain).addRowVector(bias)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray output = org.nd4j.linalg.factory.Nd4j.zerosLike(res);
			Dim output As INDArray = Nd4j.zerosLike(res)
			Nd4j.Executioner.exec(New LayerNorm(standardized, gain, bias, output, True, 1))

			assertEquals(res, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayerNormNoBias(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayerNormNoBias(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 10, 4);
			Dim random As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray standardized = random.ulike();
			Dim standardized As INDArray = random.ulike()
			Nd4j.Executioner.exec(New Standardize(random, standardized, 1))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray gain = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim gain As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = standardized.mulRowVector(gain);
			Dim res As INDArray = standardized.mulRowVector(gain)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expOut = res.norm1();
			Dim expOut As INDArray = res.norm1()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int[] axis = new int[]{1};
			Dim axis() As Integer = {1}
			Dim sd As SameDiff = SameDiff.create()
			Dim sdInput As SDVariable = sd.var("input", standardized)
			Dim sdGain As SDVariable = sd.var("gain", gain)
			Dim [out] As SDVariable = sd.nn_Conflict.layerNorm(sdInput, sdGain, True, axis)
			[out].norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", expOut).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayerNormOPNoBias(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayerNormOPNoBias(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 10, 4);
			Dim random As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray standardized = random.ulike();
			Dim standardized As INDArray = random.ulike()
			Nd4j.Executioner.exec(New Standardize(random, standardized, 1))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray gain = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim gain As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = standardized.mulRowVector(gain);
			Dim res As INDArray = standardized.mulRowVector(gain)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray output = org.nd4j.linalg.factory.Nd4j.zerosLike(res);
			Dim output As INDArray = Nd4j.zerosLike(res)
			Nd4j.Executioner.exec(New LayerNorm(standardized, gain, output, True, 1))

			assertEquals(res, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayerNormNoDeviation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayerNormNoDeviation(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray random = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 10, 4);
			Dim random As INDArray = Nd4j.rand(DataType.DOUBLE, 10, 4)
			For i As Integer = 0 To 3
				random.putScalar(1, i, 7)
			Next i

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray standardized = random.ulike();
			Dim standardized As INDArray = random.ulike()
			Nd4j.Executioner.exec(New Standardize(random, standardized, 1))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray gain = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim gain As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray bias = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.@DOUBLE, 4);
			Dim bias As INDArray = Nd4j.rand(DataType.DOUBLE, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray res = standardized.mulRowVector(gain).addRowVector(bias);
			Dim res As INDArray = standardized.mulRowVector(gain).addRowVector(bias)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray expOut = res.norm1();
			Dim expOut As INDArray = res.norm1()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int[] axis = new int[]{1};
			Dim axis() As Integer = {1}
			Dim sd As SameDiff = SameDiff.create()
			Dim sdInput As SDVariable = sd.var("input", standardized)
			Dim sdGain As SDVariable = sd.var("gain", gain)
			Dim sdBias As SDVariable = sd.var("bias", bias)
			Dim [out] As SDVariable = sd.nn_Conflict.layerNorm(sdInput, sdGain, sdBias, True, axis)
			[out].norm1("out")

			Dim err As String = OpValidation.validate((New TestCase(sd)).expectedOutput("out", expOut).gradCheckMask(Collections.singletonMap("input", random.neq(7))).gradientCheck(True))
			assertNull(err, err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void exceptionThrown_WhenConv1DConfigInvalid(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub exceptionThrown_WhenConv1DConfigInvalid(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4
			Dim k As Integer = 2
			Dim mb As Integer = 3
			Dim img As Integer = 28
			Dim sd As SameDiff = SameDiff.create()
			Dim wArr As INDArray = Nd4j.create(k, nIn, nOut)
			Dim inArr As INDArray = Nd4j.create(mb, nIn, img)
			Dim [in] As SDVariable = sd.var("in", inArr)
			Dim w As SDVariable = sd.var("W", wArr)
			Dim vars() As SDVariable = {[in], w}
			Dim conv1DConfig As Conv1DConfig = Conv1DConfig.builder().k(k).p(-1).s(0).paddingMode(PaddingMode.VALID).build()
			Dim [out] As SDVariable = sd.cnn().conv1d([in], w, conv1DConfig)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void exceptionThrown_WhenConv2DConfigInvalid(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub exceptionThrown_WhenConv2DConfigInvalid(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = Nothing
			Dim inSizeNCHW() As Integer = {1, 3, 8, 8}
			Dim msg As String = "0 - conv2d+bias, nchw - input " & Arrays.toString(inSizeNCHW)
			Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(New Integer(){3, 3, inSizeNCHW(1), 3}).muli(10))
			Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(New Long(){3}).muli(10))
			Dim [out] As SDVariable = sd.cnn().conv2d([in], w0, b0, Conv2DConfig.builder().dataFormat(Conv2DConfig.NCHW).isSameMode(True).kH(3).kW(-3).sH(1).sW(0).build())
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void exceptionThrown_WhenConf3DInvalid(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub exceptionThrown_WhenConf3DInvalid(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Nd4j.Random.setSeed(12345)
			Dim inSizeNCDHW() As Integer = {2, 3, 4, 5, 5}
			Dim failed As IList(Of String) = New List(Of String)()
			For Each ncdhw As Boolean In New Boolean(){True, False}
				Dim nIn As Integer = inSizeNCDHW(1)
				Dim shape() As Integer = (If(ncdhw, inSizeNCDHW, ncdhwToNdhwc(inSizeNCDHW)))
				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.var("in", shape)
				Dim [out] As SDVariable
				Dim msg As String = "0 - conv3d+bias+same, ncdhw=" & ncdhw & " - input " & Arrays.toString(shape)
				Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(New Integer(){2, 2, 2, nIn, 3}).muli(10))
				Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(New Long(){3}).muli(10))
				[out] = sd.cnn().conv3d([in], w0, b0, Conv3DConfig.builder().dataFormat(If(ncdhw, Conv3DConfig.NCDHW_Conflict, Conv3DConfig.NDHWC)).isSameMode(True).kH(2).kW(2).kD(2).sD(1).sH(1).sW(-1).dW(-1).build())
			Next ncdhw
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLayerNormMixedOrders(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLayerNormMixedOrders(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, 3, 8).dup("f"c)
			Dim gain As INDArray = Nd4j.rand(DataType.DOUBLE, 8).dup("f"c)
			Dim bias As INDArray = Nd4j.rand(DataType.DOUBLE, 8).dup("f"c)

			Dim outFF As INDArray = Nd4j.create(DataType.DOUBLE, New Long(){3, 8}, "f"c)
			Dim outCC As INDArray = Nd4j.create(DataType.DOUBLE, New Long(){3, 8}, "c"c)
			Dim outFC As INDArray = Nd4j.create(DataType.DOUBLE, New Long(){3, 8}, "c"c)
			Dim outCF As INDArray = Nd4j.create(DataType.DOUBLE, New Long(){3, 8}, "f"c)

			'F in, F out case
			Nd4j.exec(DynamicCustomOp.builder("layer_norm").addInputs(input, gain, bias).addOutputs(outFF).addIntegerArguments(1).build())

			'C in, C out case
			Nd4j.exec(DynamicCustomOp.builder("layer_norm").addInputs(input.dup("c"c), gain.dup("c"c), bias.dup("c"c)).addOutputs(outCC).addIntegerArguments(1).build())

			assertEquals(outFF, outCC) 'OK

			'C in, F out case
			outFF.assign(0)
			Nd4j.exec(DynamicCustomOp.builder("layer_norm").addInputs(input.dup("c"c), gain.dup("c"c), bias.dup("c"c)).addOutputs(outCF).addIntegerArguments(1).build())
			assertEquals(outCC, outCF) 'Fails here

			'F in, C out case
			outFF.assign(0)
			Nd4j.exec(DynamicCustomOp.builder("layer_norm").addInputs(input, gain, bias).addOutputs(outFC).addIntegerArguments(1).build())
			assertEquals(outCC, outFC) 'Fails here
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBiasAdd_nchw_nhwc(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBiasAdd_nchw_nhwc(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			For Each nchw As Boolean In New Boolean(){True, False}
				log.info("Starting test: {}",If(nchw, "nchw", "nhwc"))
				Dim sameDiff As SameDiff = SameDiff.create()

				Dim [in] As SDVariable = sameDiff.var("input", Nd4j.rand(DataType.DOUBLE,If(nchw, New Long(){2, 4, 3, 3}, New Long()){2, 3, 3, 4}))
				Dim b As SDVariable = sameDiff.var("bias", Nd4j.rand(DataType.DOUBLE, New Long(){4}))

				Dim bAdd As SDVariable = sameDiff.nn_Conflict.biasAdd([in], b, nchw)
				Dim loss As SDVariable = bAdd.std(True)


				Dim exp As INDArray = [in].Arr.dup()
				If nchw Then
					exp.addi(b.Arr.reshape(ChrW(1), 4, 1, 1))
				Else
					exp.addi(b.Arr.reshape(ChrW(1), 1, 1, 4))
				End If

				Dim tc As TestCase = (New TestCase(sameDiff)).gradientCheck(True).expectedOutput(bAdd.name(), exp)

				Dim err As String = OpValidation.validate(tc)
				assertNull(err)
			Next nchw
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDepthwiseConv2D()
		Public Overridable Sub testDepthwiseConv2D()

			Dim bS As Integer = 10

			Dim kernelHeight As Integer = 2
			Dim kernelWidth As Integer = 2
			Dim strideHeight As Integer = 2
			Dim strideWidth As Integer = 2
			Dim inChannels As Integer = 2
			Dim outChannels As Integer = 3
			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.rand(bS, inChannels, 5,5))
			Dim weights As SDVariable = sd.var("weights", Nd4j.rand(DataType.DOUBLE, kernelHeight, kernelWidth, inChannels, outChannels))
			Dim bias As SDVariable = sd.var("bias", Nd4j.rand(DataType.DOUBLE, inChannels*outChannels))
			Dim config As Conv2DConfig = Conv2DConfig.builder().kH(kernelHeight).kW(kernelWidth).sH(strideHeight).sW(strideWidth).dataFormat("NCHW").build()

			Dim [out] As SDVariable = sd.cnn_Conflict.depthWiseConv2d([in], weights, bias, config)
			Dim loss As SDVariable = sd.standardDeviation("loss", [out], True)
			loss.markAsLoss()

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))
			assertNull(err)



		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void LSTMLayerTestCase1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub LSTMLayerTestCase1(ByVal backend As Nd4jBackend)

			Dim bS As Integer = 5
			Dim nIn As Integer = 3
			Dim numUnits As Integer = 7
			Dim sL As Integer = 3 'small just for test


			' notations:
			' bS - batch size, numExamples
			' sL - sequence length, number of time steps, timeLength
			' nIn - input size, inOutSize

			'  TNS: shape [timeLength, numExamples, inOutSize] - sometimes referred to as "time major"<br>
			'  NST: shape [numExamples, inOutSize, timeLength]<br>
			'  NTS: shape [numExamples, timeLength, inOutSize]<br>
			'  for bidirectional:
			'  T2NS: 3 = [timeLength, 2, numExamples, inOutSize] (for ONNX)


			For Each useCLast As Boolean In New Boolean(){False, True}
				For Each useYLast As Boolean In New Boolean(){False, True}

					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.var("in", Nd4j.randn(DataType.DOUBLE, bS, nIn, sL))


					Dim cLast As SDVariable = If(useCLast, sd.var("cLast", Nd4j.zeros(DataType.DOUBLE, bS, numUnits)), Nothing)
					Dim yLast As SDVariable = If(useYLast, sd.var("yLast", Nd4j.zeros(DataType.DOUBLE, bS, numUnits)), Nothing)


					Dim c As LSTMLayerConfig = LSTMLayerConfig.builder().lstmdataformat(LSTMDataFormat.NST).directionMode(LSTMDirectionMode.FWD).gateAct(LSTMActivations.SIGMOID).cellAct(LSTMActivations.TANH).outAct(LSTMActivations.TANH).retFullSequence(True).retLastC(True).retLastH(True).build()

					Dim outputs As New LSTMLayerOutputs(sd.rnn_Conflict.lstmLayer([in], cLast, yLast, Nothing, LSTMLayerWeights.builder().weights(sd.var("weights", Nd4j.randn(DataType.DOUBLE, nIn, 4 * numUnits))).rWeights(sd.var("rWeights", Nd4j.randn(DataType.DOUBLE, numUnits, 4 * numUnits))).peepholeWeights(sd.var("inputPeepholeWeights", Nd4j.randn(DataType.DOUBLE, 3 * numUnits))).bias(sd.var("bias", Nd4j.rand(DataType.DOUBLE, 4 * numUnits))).build(), c), c)

					Dim [out]() As Long = {bS, numUnits, sL}
					Dim hL() As Long = {bS, numUnits}
					Dim cL() As Long = {bS, numUnits}

					assertArrayEquals([out], outputs.Output.eval().shape())
					assertArrayEquals(hL, outputs.LastOutput.eval().shape())
					assertArrayEquals(cL, outputs.LastState.eval().shape())

					sd.setLossVariables(outputs.Output, outputs.getLastTimeStepOutput(), outputs.getTimeSeriesOutput())

					Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True).testName("cLast=" & cLast & ", yLast=" & yLast))

					assertNull(err)
				Next useYLast
			Next useCLast


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void LSTMLayerTestCase2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub LSTMLayerTestCase2(ByVal backend As Nd4jBackend)
			Dim bS As Integer = 5
			Dim nIn As Integer = 3
			Dim numUnits As Integer = 7
			Dim sL As Integer = 3 'small just for test

			Dim sd As SameDiff = SameDiff.create()

			' notations:
			' bS - batch size, numExamples
			' sL - sequence length, number of time steps, timeLength
			' nIn - input size, inOutSize

			'  TNS: shape [timeLength, numExamples, inOutSize] - sometimes referred to as "time major"<br>
			'  NST: shape [numExamples, inOutSize, timeLength]<br>
			'  NTS: shape [numExamples, timeLength, inOutSize]<br>
			'  for bidirectional:
			'  T2NS: 3 = [timeLength, 2, numExamples, inOutSize] (for ONNX)
			Dim [in] As SDVariable = sd.var("in", Nd4j.rand(DataType.DOUBLE, sL, bS, nIn))


			Dim cLast As SDVariable = sd.var("cLast", Nd4j.zeros(DataType.DOUBLE, bS, numUnits))
			Dim yLast As SDVariable = sd.var("yLast", Nd4j.zeros(DataType.DOUBLE, bS, numUnits))

			Dim c As LSTMLayerConfig = LSTMLayerConfig.builder().lstmdataformat(LSTMDataFormat.TNS).directionMode(LSTMDirectionMode.FWD).gateAct(LSTMActivations.SIGMOID).cellAct(LSTMActivations.TANH).outAct(LSTMActivations.TANH).retFullSequence(True).retLastC(False).retLastH(False).build()

			Dim outputs As New LSTMLayerOutputs(sd.rnn_Conflict.lstmLayer([in], cLast, yLast, Nothing, LSTMLayerWeights.builder().weights(sd.var("weights", Nd4j.rand(DataType.DOUBLE, nIn, 4 * numUnits))).rWeights(sd.var("rWeights", Nd4j.rand(DataType.DOUBLE, numUnits, 4 * numUnits))).build(), c), c)


			Dim [out]() As Long = {sL, bS, numUnits}
			assertArrayEquals([out], outputs.Output.eval().shape())

			sd.setLossVariables(outputs.Output)

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))

			assertNull(err)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void LSTMLayerTestCase3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub LSTMLayerTestCase3(ByVal backend As Nd4jBackend)
			Dim bS As Integer = 5
			Dim nIn As Integer = 3
			Dim numUnits As Integer = 7
			Dim sL As Integer = 3 'small just for test

			Dim sd As SameDiff = SameDiff.create()

			' notations:
			' bS - batch size, numExamples
			' sL - sequence length, number of time steps, timeLength
			' nIn - input size, inOutSize

			'  TNS: shape [timeLength, numExamples, inOutSize] - sometimes referred to as "time major"<br>
			'  NST: shape [numExamples, inOutSize, timeLength]<br>
			'  NTS: shape [numExamples, timeLength, inOutSize]<br>
			'  for bidirectional:
			'  T2NS: 3 = [timeLength, 2, numExamples, inOutSize] (for ONNX)
			Dim [in] As SDVariable = sd.var("in", Nd4j.rand(DataType.DOUBLE, bS, sL, nIn))


			' when directionMode >= 2 (BIDIR_CONCAT=3)
			' Wx, Wr [2, nIn, 4*nOut]
			' hI, cI [2, bS, nOut]
			Dim cLast As SDVariable = sd.var("cLast", Nd4j.zeros(DataType.DOUBLE, 2, bS, numUnits))
			Dim yLast As SDVariable = sd.var("yLast", Nd4j.zeros(DataType.DOUBLE, 2, bS, numUnits))

			Dim c As LSTMLayerConfig = LSTMLayerConfig.builder().lstmdataformat(LSTMDataFormat.NTS).directionMode(LSTMDirectionMode.BIDIR_CONCAT).gateAct(LSTMActivations.SIGMOID).cellAct(LSTMActivations.SOFTPLUS).outAct(LSTMActivations.SOFTPLUS).retFullSequence(True).retLastC(False).retLastH(False).build()

			Dim outputs As New LSTMLayerOutputs(sd.rnn_Conflict.lstmLayer(New String(){"out"}, [in], cLast, yLast, Nothing, LSTMLayerWeights.builder().weights(sd.var("weights", Nd4j.rand(DataType.DOUBLE, 2, nIn, 4 * numUnits))).rWeights(sd.var("rWeights", Nd4j.rand(DataType.DOUBLE, 2, numUnits, 4 * numUnits))).build(), c), c)


			Dim [out]() As Long = {bS, sL, 2 * numUnits}

			assertArrayEquals([out], outputs.Output.eval().shape())

			sd.setLossVariables(outputs.Output)

			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))

			assertNull(err)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void GRUTestCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub GRUTestCase(ByVal backend As Nd4jBackend)
			Dim bS As Integer = 5
			Dim nIn As Integer = 4
			Dim nOut As Integer = 6
			Dim time As Integer = 2

			Dim sd As SameDiff = SameDiff.create()

			Dim [in] As SDVariable = sd.var("in", Nd4j.randn(DataType.DOUBLE, time, bS, nIn).muli(10))
			Dim hLast As SDVariable = sd.var("cLast", Nd4j.zeros(DataType.DOUBLE, bS, nOut))
			Dim Wx As SDVariable = sd.var("Wx", Nd4j.randn(DataType.DOUBLE, nIn, 3*nOut))
			Dim Wh As SDVariable = sd.var("Wh", Nd4j.randn(DataType.DOUBLE, nOut, 3*nOut))
			Dim biases As SDVariable = sd.var("bias", Nd4j.randn(DataType.DOUBLE, 3*nOut))

			Dim [out] As SDVariable = (New GRU(sd, [in], hLast, Wx, Wh,biases)).outputVariable()

			Dim outShapes() As Long = {time, bS, nOut}
			assertArrayEquals(New Long(){time, bS, nOut}, [out].eval().shape())

			sd.setLossVariables([out].std(True))
			Dim err As String = OpValidation.validate((New TestCase(sd)).gradientCheck(True))

			assertNull(err)

		End Sub




	End Class
End Namespace