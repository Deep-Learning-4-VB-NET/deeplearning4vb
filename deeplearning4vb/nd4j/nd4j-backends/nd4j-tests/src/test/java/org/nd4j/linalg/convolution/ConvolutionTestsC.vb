Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocUtil = org.nd4j.linalg.api.buffer.util.AllocUtil
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Pooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.convolution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class ConvolutionTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ConvolutionTestsC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConvOutWidthAndHeight(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConvOutWidthAndHeight(ByVal backend As Nd4jBackend)
			Dim outSize As Long = Convolution.outSize(2, 1, 1, 2, 1, False)
			assertEquals(6, outSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2Col(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2Col(ByVal backend As Nd4jBackend)
			Dim linspaced As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim ret As INDArray = Convolution.im2col(linspaced, 1, 1, 1, 1, 2, 2, 0, False)
			Dim im2colAssertion As INDArray = Nd4j.create(New Double() {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 2.0, 0.0, 0.0, 0.0, 0.0, 3.0, 4.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 5.0, 6.0, 0.0, 0.0, 0.0, 0.0, 7.0, 8.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 9.0, 10.0, 0.0, 0.0, 0.0, 0.0, 11.0, 12.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 13.0, 14.0, 0.0, 0.0, 0.0, 0.0, 15.0, 16.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}, New Long() {2, 2, 1, 1, 6, 6})
			assertEquals(im2colAssertion, ret)
			Dim col2ImAssertion As INDArray = Nd4j.create(New Double() {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }, New Integer() {2, 2, 2, 2})

			Dim otherConv As INDArray = Convolution.col2im(ret, 1, 1, 2, 2, 2, 2)
			assertEquals(col2ImAssertion, otherConv)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2Col2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2Col2(ByVal backend As Nd4jBackend)
			Dim kh As Integer = 2
			Dim kw As Integer = 2
			Dim ph As Integer = 0
			Dim pw As Integer = 0
			Dim sy As Integer = 2
			Dim sx As Integer = 2
			Dim depth As Integer = 2
			Dim assertion As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4}, New Long() {1, 1, 2, 2, 4, 4})
			Dim ret As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4}, New Long() {1, 1, 8, 8})

			Dim test As INDArray = Convolution.im2col(ret, kh, kw, sy, sx, ph, pw, 0, False)
			assertEquals(assertion, test)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCompareIm2ColImpl(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompareIm2ColImpl(ByVal backend As Nd4jBackend)

			Dim miniBatches() As Integer = {1, 3, 5}
			Dim depths() As Integer = {1, 3, 5}
			Dim inHeights() As Integer = {5, 21}
			Dim inWidths() As Integer = {5, 21}
			Dim strideH() As Integer = {1, 2}
			Dim strideW() As Integer = {1, 2}
			Dim sizeW() As Integer = {1, 2, 3}
			Dim sizeH() As Integer = {1, 2, 3}
			Dim padH() As Integer = {0, 1, 2}
			Dim padW() As Integer = {0, 1, 2}
			Dim coverall() As Boolean = {False, True}

			Dim types() As DataType = {DataType.FLOAT, DataType.DOUBLE, DataType.FLOAT, DataType.DOUBLE}
			Dim modes() As DataBuffer.AllocationMode = {DataBuffer.AllocationMode.HEAP, DataBuffer.AllocationMode.HEAP, DataBuffer.AllocationMode.DIRECT, DataBuffer.AllocationMode.DIRECT}

			Dim factoryClassName As String = Nd4j.factory().GetType().ToString().ToLower()
			If factoryClassName.Contains("jcublas") OrElse factoryClassName.Contains("cuda") Then
				'Only test direct for CUDA; test all for CPU
				types = New DataType() {DataType.FLOAT, DataType.DOUBLE}
				modes = New DataBuffer.AllocationMode() {DataBuffer.AllocationMode.DIRECT, DataBuffer.AllocationMode.DIRECT}
			End If

			Dim initialType As DataType = Nd4j.dataType()
			For i As Integer = 0 To types.Length - 1
				Dim type As DataType = types(i)
				Dim mode As DataBuffer.AllocationMode = modes(i)

				DataTypeUtil.setDTypeForContext(type)
				Nd4j.alloc = mode

				AllocUtil.setAllocationModeForContext(mode)

				For Each m As Integer In miniBatches
					For Each d As Integer In depths
						For Each h As Integer In inHeights
							For Each w As Integer In inWidths
								For Each sh As Integer In strideH
									For Each sw As Integer In strideW
										For Each kh As Integer In sizeH
											For Each kw As Integer In sizeW
												For Each ph As Integer In padH
													For Each pw As Integer In padW
														If (w - kw + 2 * pw) Mod sw <> 0 OrElse (h - kh + 2 * ph) Mod sh <> 0 Then
															Continue For '(w-kp+2*pW)/sw + 1 is not an integer,  i.e., number of outputs doesn't fit
														End If

														Console.WriteLine("Running " & m & " " & d & " " & h & " " & w)
														For Each [cAll] As Boolean In coverall

															Dim [in] As INDArray = Nd4j.rand(New Integer() {m, d, h, w})
															'assertEquals(in.data().allocationMode(), mode);
															'assertEquals(in.data().dataType(), opType);

															Dim outOrig As INDArray = OldConvolution.im2col([in], kh, kw, sh, sw, ph, pw, -1, [cAll]) 'Old implementation
															Dim outNew As INDArray = Convolution.im2col([in], kh, kw, sh, sw, ph, pw, [cAll]) 'Current implementation

															assertEquals(outOrig, outNew)
														Next [cAll]
													Next pw
												Next ph
											Next kw
										Next kh
									Next sw
								Next sh
							Next w
						Next h
					Next d
				Next m
			Next i

			DataTypeUtil.setDTypeForContext(initialType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling2D_Same(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling2D_Same(ByVal backend As Nd4jBackend)
			Dim miniBatches() As Integer = {1, 3, 5}
			Dim depths() As Integer = {1, 3, 5}
			Dim inHeights() As Integer = {5, 21}
			Dim inWidths() As Integer = {5, 21}
			Dim strideH() As Integer = {1, 2}
			Dim strideW() As Integer = {1, 2}
			Dim sizeW() As Integer = {1, 2, 3}
			Dim sizeH() As Integer = {1, 2, 3}
			Dim padH() As Integer = {0}
			Dim padW() As Integer = {0}
			Dim types() As Pooling2D.Pooling2DType = {Pooling2D.Pooling2DType.PNORM, Pooling2D.Pooling2DType.AVG, Pooling2D.Pooling2DType.MAX}

			Dim cnt As Integer = 0

			For Each type As Pooling2D.Pooling2DType In types
				log.info("Trying pooling type: [{}]", type)
				For Each m As Integer In miniBatches
					For Each d As Integer In depths
						For Each h As Integer In inHeights
							For Each w As Integer In inWidths
								For Each sh As Integer In strideH
									For Each sw As Integer In strideW
										For Each kh As Integer In sizeH
											For Each kw As Integer In sizeW

												Dim [in] As INDArray = Nd4j.linspace(1, (m * d * h * w), (m * d * h * w), Nd4j.defaultFloatingPointType()).reshape(New Integer(){m, d, h, w})

												Dim outSize() As Integer = getOutputSize([in], New Integer(){kh, kw}, New Integer(){sh, sw}, Nothing, True)

												'Calculate padding for same mode:
												Dim pHTotal As Integer = (outSize(0)-1)*sh + kh - h
												Dim pWTotal As Integer = (outSize(1)-1)*sw + kw - w
												Dim padTop As Integer = pHTotal \ 2
												Dim padLeft As Integer = pWTotal \ 2

												Dim col As INDArray = Nd4j.create(New Integer(){m, d, outSize(0), outSize(1), kh, kw}, "c"c)
												Dim col2 As INDArray = col.permute(0, 1, 4, 5, 2, 3)
												'INDArray col = Nd4j.createUninitialized(new int[]{m, d, kH, kW, outSize[0], outSize[1]}, 'c');
												'INDArray col2 = col;

												Convolution.im2col([in], kh, kw, sh, sw, padTop, padLeft, True, col2)

												Dim col2d As INDArray = col.reshape("c"c, m * d * outSize(0) * outSize(1), kh * kw)

												Dim output As INDArray = Nd4j.create(m, d, outSize(0), outSize(1))



												Dim reduced As INDArray = Nothing
												Select Case type
													Case Pooling2D.Pooling2DType.PNORM
														Dim pnorm As Integer = 3

														Transforms.abs(col2d, False)
														Transforms.pow(col2d, pnorm, False)
														reduced = col2d.sum(1)
														Transforms.pow(reduced, (1.0 / pnorm), False)

														Convolution.pooling2D([in], kh, kw, sh, sw, padTop, padLeft, 1, 1, True, Pooling2D.Pooling2DType.PNORM, Pooling2D.Divisor.INCLUDE_PADDING, pnorm, outSize(0), outSize(1), output)

													Case Pooling2D.Pooling2DType.MAX
														Convolution.pooling2D([in], kh, kw, sh, sw, padTop, padLeft, 1, 1, True, Pooling2D.Pooling2DType.MAX, Pooling2D.Divisor.INCLUDE_PADDING, 0.0, outSize(0), outSize(1), output)

														reduced = col2d.max(1)
													Case Pooling2D.Pooling2DType.AVG

														Convolution.pooling2D([in], kh, kw, sh, sw, padTop, padLeft, 1, 1, True, Pooling2D.Pooling2DType.AVG, Pooling2D.Divisor.INCLUDE_PADDING, 0.0, outSize(0), outSize(1), output)

														reduced = col2d.mean(1)
												End Select

												reduced = reduced.reshape("c"c,m,d, outSize(0), outSize(1)).dup("c"c)

												assertEquals(reduced, output,"Failed opType: " & type)
											Next kw
										Next kh
									Next sw
								Next sh
							Next w
						Next h
					Next d
				Next m
			Next type
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMoreIm2Col2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMoreIm2Col2(ByVal backend As Nd4jBackend)
			Dim kh As Integer = 2
			Dim kw As Integer = 2
			Dim ph As Integer = 0
			Dim pw As Integer = 0
			Dim sy As Integer = 2
			Dim sx As Integer = 2

			Dim ret As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4}, New Long() {1, 1, 8, 8})

			Dim assertion As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 1, 1, 1, 1, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 4, 4, 4, 4}, New Long() {1, 1, 2, 2, 4, 4})
			Dim im2colTest As INDArray = Convolution.im2col(ret, kh, kw, sy, sx, ph, pw, 0, False)
			assertEquals(assertion, im2colTest)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCol2Im(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCol2Im(ByVal backend As Nd4jBackend)
			Dim kh As Integer = 1
			Dim kw As Integer = 1
			Dim sy As Integer = 1
			Dim sx As Integer = 1
			Dim ph As Integer = 1
			Dim pw As Integer = 1
			Dim linspaced As INDArray = Nd4j.linspace(1, 64, 64, Nd4j.defaultFloatingPointType()).reshape(ChrW(2), 2, 2, 2, 2, 2)
			Dim newTest As INDArray = Convolution.col2im(linspaced, sy, sx, ph, pw, 2, 2)
			Dim assertion As INDArray = OldConvolution.col2im(linspaced, sy, sx, ph, pw, 2, 2)

			Console.WriteLine("Assertion dimensions: " & Arrays.toString(assertion.shape()))
			assertEquals(assertion, newTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testimcolim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testimcolim(ByVal backend As Nd4jBackend)
			Dim nEx As Integer = 2
			Dim depth As Integer = 3
			Dim width As Integer = 7
			Dim height As Integer = 7
			Dim kernel() As Integer = {3, 2}
			Dim stride() As Integer = {2, 3}
			Dim padding() As Integer = {1, 2}
			Dim prod As Integer = nEx * depth * width * height

			Dim [in] As INDArray = Nd4j.linspace(1, prod, prod, Nd4j.defaultFloatingPointType()).reshape(ChrW(nEx), depth, width, height)

			Dim assertim2col As INDArray = OldConvolution.im2col([in], kernel, stride, padding)
			Dim im2col As INDArray = Convolution.im2col([in], kernel, stride, padding)
			assertEquals(assertim2col, im2col)

			Dim assertcol2im As INDArray = OldConvolution.col2im(im2col, stride, padding, height, width)
			Dim col2im As INDArray = Convolution.col2im(im2col, stride, padding, height, width)
			assertEquals(assertcol2im, col2im)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMaxPoolBackprop()
		Public Overridable Sub testMaxPoolBackprop()
			Nd4j.Random.setSeed(12345)

			For i As Integer = 0 To 4

				Dim inputShape() As Integer = {1, 1, 4, 3}

				Dim kernel() As Integer = {2, 2}
				Dim strides() As Integer = {1, 1}
				Dim pad() As Integer = {0, 0}
				Dim dilation() As Integer = {1, 1} 'TODO non 1-1 dilation
				Dim same As Boolean = True


				Dim fn As String = "maxpool2d_bp"
				Dim nIArgs As Integer = 11

				Dim a(nIArgs - 1) As Integer
				a(0) = kernel(0)
				a(1) = kernel(1)
				a(2) = strides(0)
				a(3) = strides(1)
				a(4) = pad(0)
				a(5) = pad(1)
				a(6) = dilation(0)
				a(7) = dilation(1)
				a(8) = If(same, 1, 0)
				'a[9]: Not used with max pooling
				a(10) = 0 'For NCHW

				Dim inputs As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, inputShape, Nd4j.defaultFloatingPointType())

				For Each pIn As Pair(Of INDArray, String) In inputs
					Dim input As INDArray = pIn.First
					Dim outShapeHW() As Integer = getOutputSize(input, kernel, strides, pad, same)
					Dim eps As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, New Integer(){inputShape(0), inputShape(1), outShapeHW(0), outShapeHW(1)}, Nd4j.defaultFloatingPointType())
					For Each pEps As Pair(Of INDArray, String) In eps
						Dim epsilon As INDArray = pEps.First
						Dim epsNext As INDArray = Nd4j.create(inputShape, "c"c)

						'Runs fine with dups:
	'                    input = input.dup('c');
						epsilon = epsilon.dup("c"c)

						Dim op As DynamicCustomOp = DynamicCustomOp.builder(fn).addInputs(input, epsilon).addOutputs(epsNext).addIntegerArguments(a).build()

						Nd4j.Executioner.execAndReturn(op)

						Dim expEpsNext As INDArray = expGradMaxPoolBackPropSame(input, epsilon, kernel, strides, same)

						Dim msg As String = "input=" & pIn.Second & ", eps=" & pEps.Second
						assertEquals(expEpsNext, epsNext,msg)
					Next pEps
				Next pIn
			Next i
		End Sub

		Public Shared Function expGradMaxPoolBackPropSame(ByVal input As INDArray, ByVal gradient As INDArray, ByVal k() As Integer, ByVal s() As Integer, ByVal same As Boolean) As INDArray
			input = input.dup()
			If Not same Then
				Throw New System.NotSupportedException("non-Same mode not yet supported here")
			End If

			Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(input.size(2)/CDbl(s(0)))))
			Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(input.size(3)/CDbl(s(1)))))

			Dim totalPadH As Long = (outH-1)*s(0) + k(0) - input.size(2)
			Dim totalPadW As Long = (outW-1)*s(1) + k(1) - input.size(3)

			Dim topPad As Long = totalPadH\2
			Dim bottomPad As Long = totalPadH - topPad
			Dim leftPad As Long = totalPadW\2
			Dim rightPad As Long = totalPadW - leftPad

			Dim outGrad As INDArray = Nd4j.create(input.shape())

			Dim m As Integer=0
			Do While m<input.size(0)
				Dim d As Integer=0
				Do While d<input.size(1)
					For y As Integer = 0 To outH - 1
						For x As Integer = 0 To outW - 1

							'First: work out the *original* position for this kernel...
							Dim kTLy As Long = y*s(0) - topPad
							Dim kTLx As Long = x*s(1) - leftPad

							Dim maxPos() As Long = {kTLy, kTLx}
							Dim max As Double = -Double.MaxValue
							Dim kY As Integer=0
							Do While kY<k(0)
								Dim kX As Integer=0
								Do While kX<k(1)
									If kTLy + kY < 0 OrElse kTLy + kY >= input.size(2) OrElse kTLx + kX < 0 OrElse kTLx + kX >= input.size(3) Then
										'Is padding
										kX += 1
										Continue Do
									End If
									Dim v As Double = input.getDouble(m, d, kTLy + kY, kTLx + kX)
									If v > max Then
										max = v
										maxPos = New Long(){kTLy + kY, kTLx + kX}
									End If
									kX += 1
								Loop
								kY += 1
							Loop
							If max = -Double.MaxValue Then
								'All input values are padding, so can skip this input (should rarely happen)
								Continue For
							End If

							'Now that we know *where* the max is from: add the gradient
							Dim v As Double = outGrad.getDouble(m, d, maxPos(0), maxPos(1))
							Dim toAdd As Double = gradient.getDouble(m,d,y,x)
							outGrad.putScalar(m, d, maxPos(0), maxPos(1), v + toAdd)
						Next x
					Next y
					d += 1
				Loop
				m += 1
			Loop

			Return outGrad
		End Function



		Protected Friend Shared Function getOutputSize(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionModeSame As Boolean) As Integer()

			'FIXME: int cast
			Dim inH As Integer = CInt(inputData.size(2))
			Dim inW As Integer = CInt(inputData.size(3))

			If convolutionModeSame <> True AndAlso (kernel(0) <= 0 OrElse kernel(0) > inH + 2 * padding(0)) Then
				Throw New ND4JIllegalStateException()
			End If

			If convolutionModeSame <> True AndAlso (kernel(1) <= 0 OrElse kernel(1) > inW + 2 * padding(1)) Then
				Throw New ND4JIllegalStateException()
			End If

			If convolutionModeSame <> True Then
				If (inH - kernel(0) + 2 * padding(0)) Mod strides(0) <> 0 Then
					Dim d As Double = (inH - kernel(0) + 2 * padding(0)) / (CDbl(strides(0))) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides(0))))))
					Throw New ND4JIllegalStateException()
				End If

				If (inW - kernel(1) + 2 * padding(1)) Mod strides(1) <> 0 Then
					Dim d As Double = (inW - kernel(1) + 2 * padding(1)) / (CDbl(strides(1))) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strides(1))))))
					Throw New ND4JIllegalStateException()
				End If
			ElseIf convolutionModeSame Then
				''Same' padding mode:
				'outH = ceil(inHeight / strideH)           decimal division
				'outW = ceil(inWidth / strideW)            decimal division

				'padHeightSum = ((outH - 1) * strideH + kH - inHeight)
				'padTop = padHeightSum / 2                 integer division
				'padBottom = padHeghtSum - padTop

				'padWidthSum = ((outW - 1) * strideW + kW - inWidth)
				'padLeft = padWidthSum / 2                 integer division
				'padRight = padWidthSum - padLeft

				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides(0))))))
				Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strides(1))))))

				Return New Integer() {outH, outW}
			End If

			Dim hOut As Integer = (inH - kernel(0) + 2 * padding(0)) \ strides(0) + 1
			Dim wOut As Integer = (inW - kernel(1) + 2 * padding(1)) \ strides(1) + 1

			Return New Integer() {hOut, wOut}
		End Function

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace