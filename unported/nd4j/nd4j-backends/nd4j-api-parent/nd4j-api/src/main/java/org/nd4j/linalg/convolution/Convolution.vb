Imports System
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Col2Im = org.nd4j.linalg.api.ops.impl.layers.convolution.Col2Im
Imports Im2col = org.nd4j.linalg.api.ops.impl.layers.convolution.Im2col
Imports Pooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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




	Public Class Convolution


		Public Enum Type
			FULL
			VALID
			SAME
		End Enum


		''' <summary>
		''' Default no-arg constructor.
		''' </summary>
		Private Sub New()
		End Sub

		''' <param name="col"> </param>
		''' <param name="stride"> </param>
		''' <param name="padding"> </param>
		''' <param name="height"> </param>
		''' <param name="width">
		''' @return </param>
		Public Shared Function col2im(ByVal col As INDArray, ByVal stride() As Integer, ByVal padding() As Integer, ByVal height As Integer, ByVal width As Integer) As INDArray
			Return col2im(col, stride(0), stride(1), padding(0), padding(1), height, width)
		End Function

		''' <summary>
		''' Rearrange matrix
		''' columns into blocks
		''' </summary>
		''' <param name="col"> the column
		'''            transposed image to convert </param>
		''' <param name="sH">  stride height </param>
		''' <param name="sW">  stride width </param>
		''' <param name="ph">  padding height </param>
		''' <param name="pW">  padding width </param>
		''' <param name="kH">  height </param>
		''' <param name="kW">  width
		''' @return </param>
		Public Shared Function col2im(ByVal col As INDArray, ByVal sH As Integer, ByVal sW As Integer, ByVal ph As Integer, ByVal pW As Integer, ByVal kH As Integer, ByVal kW As Integer) As INDArray
			If col.rank() <> 6 Then
				Throw New System.ArgumentException("col2im input array must be rank 6")
			End If

			Dim output As INDArray = Nd4j.create(col.dataType(), New Long(){col.size(0), col.size(1), kH, kW})

			Dim cfg As val = Conv2DConfig.builder().sH(sH).sW(sW).dH(1).dW(1).kH(kH).kW(kW).pH(ph).pW(pW).build()

'JAVA TO VB CONVERTER NOTE: The local variable col2Im was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim col2Im_Conflict As Col2Im = Col2Im.builder().inputArrays(New INDArray(){col}).outputs(New INDArray(){output}).conv2DConfig(cfg).build()

			Nd4j.Executioner.execAndReturn(col2Im_Conflict)
			Return col2Im_Conflict.outputArguments()(0)
		End Function

		Public Shared Function col2im(ByVal col As INDArray, ByVal z As INDArray, ByVal sH As Integer, ByVal sW As Integer, ByVal pH As Integer, ByVal pW As Integer, ByVal kH As Integer, ByVal kW As Integer, ByVal dH As Integer, ByVal dW As Integer) As INDArray
			If col.rank() <> 6 Then
				Throw New System.ArgumentException("col2im input array must be rank 6")
			End If
			If z.rank() <> 4 Then
				Throw New System.ArgumentException("col2im output array must be rank 4")
			End If
'JAVA TO VB CONVERTER NOTE: The local variable col2Im was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim col2Im_Conflict As Col2Im = Col2Im.builder().inputArrays(New INDArray(){col}).outputs(New INDArray(){z}).conv2DConfig(Conv2DConfig.builder().sH(sH).sW(sW).dH(dH).dW(dW).kH(kH).kW(kW).pH(pH).pW(pW).build()).build()

			Nd4j.Executioner.execAndReturn(col2Im_Conflict)

			Return z
		End Function

		''' <param name="img"> </param>
		''' <param name="kernel"> </param>
		''' <param name="stride"> </param>
		''' <param name="padding">
		''' @return </param>
		Public Shared Function im2col(ByVal img As INDArray, ByVal kernel() As Integer, ByVal stride() As Integer, ByVal padding() As Integer) As INDArray
			Nd4j.Compressor.autoDecompress(img)
			Return im2col(img, kernel(0), kernel(1), stride(0), stride(1), padding(0), padding(1), 0, False)
		End Function

		''' <summary>
		''' Implement column formatted images
		''' </summary>
		''' <param name="img">        the image to process </param>
		''' <param name="kh">         the kernel height </param>
		''' <param name="kw">         the kernel width </param>
		''' <param name="sy">         the stride along y </param>
		''' <param name="sx">         the stride along x </param>
		''' <param name="ph">         the padding width </param>
		''' <param name="pw">         the padding height </param>
		''' <param name="isSameMode"> whether to cover the whole image or not </param>
		''' <returns> the column formatted image </returns>
		Public Shared Function im2col(ByVal img As INDArray, ByVal kh As Integer, ByVal kw As Integer, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal isSameMode As Boolean) As INDArray
			Return im2col(img, kh, kw, sy, sx, ph, pw, 1, 1, isSameMode)
		End Function

		Public Shared Function im2col(ByVal img As INDArray, ByVal kh As Integer, ByVal kw As Integer, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal dh As Integer, ByVal dw As Integer, ByVal isSameMode As Boolean) As INDArray
			Nd4j.Compressor.autoDecompress(img)
			'Input: NCHW format
			Dim outH As Long = outputSize(img.size(2), kh, sy, ph, dh, isSameMode)
			Dim outW As Long = outputSize(img.size(3), kw, sx, pw, dw, isSameMode)

			'[miniBatch,depth,kH,kW,outH,outW]
			Dim [out] As INDArray = Nd4j.create(New Long(){img.size(0), img.size(1), kh, kw, outH, outW}, "c"c)

			Return im2col(img, kh, kw, sy, sx, ph, pw, dh, dw, isSameMode, [out])
		End Function

		Public Shared Function im2col(ByVal img As INDArray, ByVal kh As Integer, ByVal kw As Integer, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal isSameMode As Boolean, ByVal [out] As INDArray) As INDArray
'JAVA TO VB CONVERTER NOTE: The local variable im2col was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim im2col_Conflict As Im2col = Im2col.builder().outputs(New INDArray(){[out]}).inputArrays(New INDArray(){img}).conv2DConfig(Conv2DConfig.builder().kH(kh).pW(pw).pH(ph).sH(sy).sW(sx).kH(kh).kW(kw).dH(1).dW(1).isSameMode(isSameMode).build()).build()

			Nd4j.Executioner.execAndReturn(im2col_Conflict)
			Return im2col_Conflict.outputArguments()(0)
		End Function

		''' <summary>
		''' Execute im2col. Note the input must be NCHW. </summary>
		''' <param name="img"> the input image in NCHW </param>
		''' <param name="kh"> </param>
		''' <param name="kw"> </param>
		''' <param name="sy"> </param>
		''' <param name="sx"> </param>
		''' <param name="ph"> </param>
		''' <param name="pw"> </param>
		''' <param name="dH"> </param>
		''' <param name="dW"> </param>
		''' <param name="isSameMode"> </param>
		''' <param name="out">
		''' @return </param>
		Public Shared Function im2col(ByVal img As INDArray, ByVal kh As Integer, ByVal kw As Integer, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal dH As Integer, ByVal dW As Integer, ByVal isSameMode As Boolean, ByVal [out] As INDArray) As INDArray

'JAVA TO VB CONVERTER NOTE: The local variable im2col was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim im2col_Conflict As Im2col = Im2col.builder().outputs(New INDArray(){[out]}).inputArrays(New INDArray(){img}).conv2DConfig(Conv2DConfig.builder().pW(pw).pH(ph).sH(sy).sW(sx).kW(kw).kH(kh).dW(dW).dH(dH).isSameMode(isSameMode).build()).build()

			Nd4j.Executioner.execAndReturn(im2col_Conflict)
			Return im2col_Conflict.outputArguments()(0)
		End Function

		''' <summary>
		''' Pooling 2d implementation
		''' </summary>
		''' <param name="img"> </param>
		''' <param name="kh"> </param>
		''' <param name="kw"> </param>
		''' <param name="sy"> </param>
		''' <param name="sx"> </param>
		''' <param name="ph"> </param>
		''' <param name="pw"> </param>
		''' <param name="dh"> </param>
		''' <param name="dw"> </param>
		''' <param name="isSameMode"> </param>
		''' <param name="type"> </param>
		''' <param name="extra">         optional argument. I.e. used in pnorm pooling. </param>
		''' <param name="virtualHeight"> </param>
		''' <param name="virtualWidth"> </param>
		''' <param name="out">
		''' @return </param>
		Public Shared Function pooling2D(ByVal img As INDArray, ByVal kh As Integer, ByVal kw As Integer, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal dh As Integer, ByVal dw As Integer, ByVal isSameMode As Boolean, ByVal type As Pooling2D.Pooling2DType, ByVal divisor As Pooling2D.Divisor, ByVal extra As Double, ByVal virtualHeight As Integer, ByVal virtualWidth As Integer, ByVal [out] As INDArray) As INDArray
			Dim pooling As New Pooling2D(img, [out], Pooling2DConfig.builder().dH(dh).dW(dw).extra(extra).kH(kh).kW(kw).pH(ph).pW(pw).isSameMode(isSameMode).sH(sy).sW(sx).type(type).divisor(divisor).build())
			Nd4j.Executioner.execAndReturn(pooling)
			Return [out]
		End Function

		''' <summary>
		''' Implement column formatted images
		''' </summary>
		''' <param name="img">        the image to process </param>
		''' <param name="kh">         the kernel height </param>
		''' <param name="kw">         the kernel width </param>
		''' <param name="sy">         the stride along y </param>
		''' <param name="sx">         the stride along x </param>
		''' <param name="ph">         the padding width </param>
		''' <param name="pw">         the padding height </param>
		''' <param name="pval">       the padding value (not used) </param>
		''' <param name="isSameMode"> whether padding mode is 'same' </param>
		''' <returns> the column formatted image </returns>
		Public Shared Function im2col(ByVal img As INDArray, ByVal kh As Integer, ByVal kw As Integer, ByVal sy As Integer, ByVal sx As Integer, ByVal ph As Integer, ByVal pw As Integer, ByVal pval As Integer, ByVal isSameMode As Boolean) As INDArray
			Dim output As INDArray = Nothing

			If isSameMode Then
				Dim oH As Integer = CInt(Math.Truncate(Math.Ceiling(img.size(2) * 1.0f / sy)))
				Dim oW As Integer = CInt(Math.Truncate(Math.Ceiling(img.size(3) * 1.0f / sx)))

				output = Nd4j.createUninitialized(img.dataType(), New Long(){img.size(0), img.size(1), kh, kw, oH, oW}, "c"c)
			Else
				Dim oH As Long = (img.size(2) - (kh + (kh - 1) * (1 - 1)) + 2 * ph) \ sy + 1
				Dim oW As Long = (img.size(3) - (kw + (kw - 1) * (1 - 1)) + 2 * pw) \ sx + 1

				output = Nd4j.createUninitialized(img.dataType(), New Long(){img.size(0), img.size(1), kh, kw, oH, oW}, "c"c)
			End If

'JAVA TO VB CONVERTER NOTE: The local variable im2col was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim im2col_Conflict As Im2col = Im2col.builder().inputArrays(New INDArray(){img}).outputs(New INDArray(){output}).conv2DConfig(Conv2DConfig.builder().pW(pw).pH(ph).sH(sy).sW(sx).kW(kw).kH(kh).dW(1).dH(1).isSameMode(isSameMode).build()).build()

			Nd4j.Executioner.execAndReturn(im2col_Conflict)
			Return im2col_Conflict.outputArguments()(0)
		End Function

		''' <summary>
		''' The out size for a convolution
		''' </summary>
		''' <param name="size"> </param>
		''' <param name="k"> </param>
		''' <param name="s"> </param>
		''' <param name="p"> </param>
		''' <param name="coverAll">
		''' @return </param>
		<Obsolete>
		Public Shared Function outSize(ByVal size As Long, ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal dilation As Integer, ByVal coverAll As Boolean) As Long
			k = effectiveKernelSize(k, dilation)

			If coverAll Then
				Return (size + p * 2 - k + s - 1) \ s + 1
			Else
				Return (size + p * 2 - k) \ s + 1
			End If
		End Function

		Public Shared Function outputSize(ByVal size As Long, ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal dilation As Integer, ByVal isSameMode As Boolean) As Long
			k = effectiveKernelSize(k, dilation)

			If isSameMode Then
				Return CInt(Math.Truncate(Math.Ceiling(size * 1.0f / s)))
			Else
				Return (size - k + 2 * p) \ s + 1
			End If
		End Function

		Public Shared Function effectiveKernelSize(ByVal kernel As Long, ByVal dilation As Integer) As Long
			Return kernel + (kernel - 1) * (dilation - 1)
		End Function


		''' <summary>
		''' 2d convolution (aka the last 2 dimensions
		''' </summary>
		''' <param name="input">  the input to op </param>
		''' <param name="kernel"> the kernel to convolve with </param>
		''' <param name="type">
		''' @return </param>
		Public Shared Function conv2d(ByVal input As INDArray, ByVal kernel As INDArray, ByVal type As Type) As INDArray
			Return Nd4j.Convolution.conv2d(input, kernel, type)
		End Function

		''' <summary>
		''' ND Convolution
		''' </summary>
		''' <param name="input">  the input to op </param>
		''' <param name="kernel"> the kerrnel to op with </param>
		''' <param name="type">   the opType of convolution </param>
		''' <param name="axes">   the axes to do the convolution along </param>
		''' <returns> the convolution of the given input and kernel </returns>
		Public Shared Function convn(ByVal input As INDArray, ByVal kernel As INDArray, ByVal type As Type, ByVal axes() As Integer) As INDArray
			Return Nd4j.Convolution.convn(input, kernel, type, axes)
		End Function

		''' <summary>
		''' ND Convolution
		''' </summary>
		''' <param name="input">  the input to op </param>
		''' <param name="kernel"> the kernel to op with </param>
		''' <param name="type">   the opType of convolution </param>
		''' <returns> the convolution of the given input and kernel </returns>
		Public Shared Function convn(ByVal input As INDArray, ByVal kernel As INDArray, ByVal type As Type) As INDArray
			Return Nd4j.Convolution.convn(input, kernel, type)
		End Function
	End Class

End Namespace