Imports System
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports CnnToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToRnnPreProcessor
Imports FeedForwardToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class InputTypeUtil
	Public Class InputTypeUtil

		Private Sub New()
		End Sub

		Public Shared Function getOutputTypeDeconvLayer(ByVal inputType As InputType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal outputDepth As Long, ByVal layerIdx As Long, ByVal layerName As String, ByVal layerClass As Type) As InputType
			Dim i As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)

			Dim hIn As val = i.getHeight()
			Dim wIn As val = i.getWidth()

			Dim padH As Integer = (If(padding Is Nothing, 0, padding(0))) 'May be null for ConvolutionMode.Same
			Dim padW As Integer = (If(padding Is Nothing, 0, padding(1)))
			Dim kH As Integer = kernelSize(0)
			Dim kW As Integer = kernelSize(1)
			If dilation(0) <> 1 Then
				kH = kH + (kH - 1) * (dilation(0) - 1)
			End If
			If dilation(1) <> 1 Then
				kW = kW + (kW - 1) * (dilation(1) - 1)
			End If

			Dim sH As Integer = stride(0)
			Dim sW As Integer = stride(1)

			If sH <= 0 OrElse sW <= 0 Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, sH <= 0) & " Invalid strides: strides must be > 0 (strideH = " & sH & ", strideW = " & sW & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputDepth, convolutionMode))
			End If

			If convolutionMode = ConvolutionMode.Same Then
				Dim hOut As Long = stride(0) * hIn
				Dim wOut As Long = stride(1) * wIn
				Return InputType.convolutional(hOut, wOut, outputDepth, i.getFormat())
			End If

			Dim hOut As Long = sH * (hIn - 1) + kH - 2 * padH
			Dim wOut As Long = sW * (wIn - 1) + kW - 2 * padW

			Return InputType.convolutional(hOut, wOut, outputDepth, i.getFormat())
		End Function

		Public Shared Function getOutputTypeDeconv3dLayer(ByVal inputType As InputType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dataFormat As Convolution3D.DataFormat, ByVal outputDepth As Long, ByVal layerIdx As Long, ByVal layerName As String, ByVal layerClass As Type) As InputType
			Dim i As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)

			Dim hIn As Long = i.getHeight()
			Dim wIn As Long = i.getWidth()
			Dim dIn As Long = i.getDepth()


			Dim padH As Integer = (If(padding Is Nothing, 0, padding(0))) 'May be null for ConvolutionMode.Same
			Dim padW As Integer = (If(padding Is Nothing, 0, padding(1)))
			Dim padD As Integer = (If(padding Is Nothing, 0, padding(2)))
			Dim kH As Integer = kernelSize(0)
			Dim kW As Integer = kernelSize(1)
			Dim kD As Integer = kernelSize(2)
			If dilation(0) <> 1 Then
				kH = kH + (kH - 1) * (dilation(0) - 1)
			End If
			If dilation(1) <> 1 Then
				kW = kW + (kW - 1) * (dilation(1) - 1)
			End If
			If dilation(2) <> 1 Then
				kD = kD + (kD - 1) * (dilation(2) - 1)
			End If

			Dim sH As Integer = stride(0)
			Dim sW As Integer = stride(1)
			Dim sD As Integer = stride(2)

			If sH <= 0 OrElse sW <= 0 OrElse sD <= 0 Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, sH <= 0) & " Invalid strides: strides must be > 0 (strideH = " & sH & ", strideW = " & sW & ", stride = " & sD & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputDepth, convolutionMode))
			End If

			If convolutionMode = ConvolutionMode.Same Then
				Dim hOut As Long = stride(0) * hIn
				Dim wOut As Long = stride(1) * wIn
				Dim dOut As Long = stride(2) * dIn
				Return InputType.convolutional3D(dataFormat, dOut, hOut, wOut, outputDepth)
			End If

			Dim hOut As Long = sH * (hIn - 1) + kH - 2 * padH
			Dim wOut As Long = sW * (wIn - 1) + kW - 2 * padW
			Dim dOut As Long = sD * (dIn - 1) + kD - 2 * padD

			Return InputType.convolutional3D(dataFormat, dOut, hOut, wOut, outputDepth)
		End Function

		Public Shared Function getOutputTypeCnn3DLayers(ByVal inputType As InputType, ByVal dataFormat As Convolution3D.DataFormat, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal outputChannels As Long, ByVal layerIdx As Long, ByVal layerName As String, ByVal layerClass As Type) As InputType
			If convolutionMode = Nothing Then
				Dim name As String = If(layerName Is Nothing, "(not named)", layerName)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New DL4JInvalidConfigException("Invalid configuration: convolution mode is null for layer (idx=" & layerIdx & ", name=" & name & ", type=" & layerClass.FullName & ")")
			End If

			Dim i As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)

			Dim inDepth As Long = i.getDepth()
			Dim inHeight As Long = i.getHeight()
			Dim inWidth As Long = i.getWidth()

			Dim padD As Integer = (If(padding Is Nothing, 0, padding(0)))
			Dim padH As Integer = (If(padding Is Nothing, 0, padding(1)))
			Dim padW As Integer = (If(padding Is Nothing, 0, padding(2)))

			Dim kD As Integer = kernelSize(0)
			Dim kH As Integer = kernelSize(1)
			Dim kW As Integer = kernelSize(2)


			If dilation(0) <> 1 Then
				'Use *effective* kernel size, accounting for dilation
				kD = kD + (kD - 1) * (dilation(0) - 1)
			End If
			If dilation(1) <> 1 Then
				kH = kH + (kH - 1) * (dilation(1) - 1)
			End If
			If dilation(2) <> 1 Then
				kW = kW + (kW - 1) * (dilation(2) - 1)
			End If

			Dim sD As Integer = stride(0)
			Dim sH As Integer = stride(1)
			Dim sW As Integer = stride(1)

			If sH <= 0 OrElse sW <= 0 OrElse sD <= 0 Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, sH <= 0) & " Invalid strides: strides must be > 0 (strideH = " & sH & ", strideW = " & sW & ", strideD = " & sD & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputChannels, convolutionMode))
			End If

			If kH <= 0 OrElse (padH > 0 AndAlso kH > inHeight + 2 * padH) Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, True) & " Invalid input configuration for kernel height. Require 0 < kH <= inHeight + 2*padH; got (kH=" & kH & ", inHeight=" & inHeight & ", padH=" & padH & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputChannels, convolutionMode))
			End If

			If kW <= 0 OrElse (padW > 0 AndAlso kW > inWidth + 2 * padW) Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, False) & " Invalid input configuration for kernel width. Require 0 < kW <= inWidth + 2*padW; got (kW=" & kW & ", inWidth=" & inWidth & ", padW=" & padW & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputChannels, convolutionMode))
			End If
			If kD <= 0 OrElse (padD > 0 AndAlso kD > inDepth + 2 * padD) Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, False) & " Invalid input configuration for kernel channels. Require 0 < kD <= inDepth + 2*padD; got (kD=" & kD & ", inDepth=" & inDepth & ", padD=" & padD & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputChannels, convolutionMode))
			End If
			'Strict mode: require exactly the right size...
			If convolutionMode = ConvolutionMode.Strict Then
				If (inHeight - kH + 2 * padH) Mod sH <> 0 Then
					Dim d As Double = (inHeight - kH + 2 * padH) / (CDbl(sH)) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inHeight / (CDbl(stride(0))))))
					Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, True) & vbLf & "Combination of kernel size, stride and padding are not valid for given input height, using ConvolutionMode.Strict" & vbLf & "ConvolutionMode.Strict requires: output height = (input height - kernelSize + 2*padding)/stride + 1 in height dimension to be an integer. Got: (" & inHeight & " - " & kH & " + 2*" & padH & ")/" & sH & " + 1 = " & str & vbLf & "See ConvolutionType enumeration Javadoc and ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/" & vbLf & "To truncate/crop the input, such that output height = floor(" & str & ") = " & truncated & ", use ConvolutionType.Truncate." & vbLf & "Alternatively use ConvolutionType.Same, which will use padding to give an output height of ceil(" & inHeight & "/" & stride(0) & ")=" & sameSize & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputChannels, convolutionMode))
				End If

				If (inWidth - kW + 2 * padW) Mod sW <> 0 Then
					Dim d As Double = (inWidth - kW + 2 * padW) / (CDbl(sW)) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inWidth / (CDbl(stride(1))))))
					Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, False) & vbLf & "Combination of kernel size, stride and padding are not valid for given input width, using ConvolutionMode.Strict" & vbLf & "ConvolutionMode.Strict requires: output width = (input width - kernelSize + 2*padding)/stride + 1 in width dimension to be an integer. Got: (" & inWidth & " - " & kW & " + 2*" & padW & ")/" & sW & " + 1 = " & str & vbLf & "See ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/ and ConvolutionType enumeration Javadoc." & vbLf & "To truncate/crop the input, such that output width = floor(" & str & ") = " & truncated & ", use ConvolutionType.Truncate." & vbLf & "Alternatively use ConvolutionType.Same, which will use padding to give an output width of ceil(" & inWidth & "/" & stride(1) & ")=" & sameSize & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputChannels, convolutionMode))
				End If

				If (inDepth - kD + 2 * padD) Mod sD <> 0 Then
					Dim d As Double = (inDepth - kD + 2 * padD) / (CDbl(sD)) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inDepth / (CDbl(stride(2))))))
					Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, False) & vbLf & "Combination of kernel size, stride and padding are not valid for given input width, using ConvolutionMode.Strict" & vbLf & "ConvolutionMode.Strict requires: output channels = (input channels - kernelSize + 2*padding)/stride + 1 in width dimension to be an integer. Got: (" & inDepth & " - " & kD & " + 2*" & padD & ")/" & sD & " + 1 = " & str & vbLf & "See ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/ and ConvolutionType enumeration Javadoc." & vbLf & "To truncate/crop the input, such that output width = floor(" & str & ") = " & truncated & ", use ConvolutionType.Truncate." & vbLf & "Alternatively use ConvolutionType.Same, which will use padding to give an output width of ceil(" & inDepth & "/" & stride(2) & ")=" & sameSize & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputChannels, convolutionMode))
				End If
			ElseIf convolutionMode = ConvolutionMode.Same Then

				Dim outD As Integer = CInt(Math.Truncate(Math.Ceiling(inDepth / (CDbl(sD)))))
				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inHeight / (CDbl(sH)))))
				Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inWidth / (CDbl(sW)))))

				Return InputType.convolutional3D(dataFormat, outD, outH, outW, outputChannels)
			End If

			Dim dOut As Long = (inDepth - kD + 2 * padD) \ sD + 1
			Dim hOut As Long = (inHeight - kH + 2 * padH) \ sH + 1
			Dim wOut As Long = (inWidth - kW + 2 * padW) \ sW + 1
			Return InputType.convolutional3D(dOut, hOut, wOut, outputChannels)
		End Function


		Public Shared Function getOutputTypeCnn1DLayers(ByVal inputType As InputType, ByVal kH As Integer, ByVal sH As Integer, ByVal padH As Integer, ByVal dilation As Integer, ByVal convolutionMode As ConvolutionMode, ByVal outputDepth As Long, ByVal layerIdx As Long, ByVal layerName As String, ByVal layerClass As Type) As InputType

			If convolutionMode = Nothing Then
				Dim name As String = If(layerName Is Nothing, "(not named)", layerName)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New DL4JInvalidConfigException("Invalid configuration: convolution mode is null for layer (idx=" & layerIdx & ", name=" & name & ", type=" & layerClass.FullName & ")")
			End If

			Dim i As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)

			Dim inHeight As val = CInt(Math.Truncate(i.getTimeSeriesLength()))
			If dilation <> 1 Then
				kH = kH + (kH - 1) * (dilation - 1)
			End If

			If sH <= 0 Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, sH <= 0) & " Invalid strides: strides must be > 0 (strideH = " & sH & ")" & vbLf & getConfigErrorCommonLastLine1D(inputType, kH, sH, padH, outputDepth, convolutionMode))
			End If

			If kH <= 0 OrElse (padH > 0 AndAlso kH > inHeight + 2 * padH) Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, True) & " Invalid input configuration for kernel height. Require 0 < kH <= inHeight + 2*padH; got (kH=" & kH & ", inHeight=" & inHeight & ", padH=" & padH & ")" & vbLf & getConfigErrorCommonLastLine1D(inputType, kH, sH, padH, outputDepth, convolutionMode))
			End If


			'Strict mode: require exactly the right size...
			If convolutionMode = ConvolutionMode.Strict Then
				If (inHeight - kH + 2 * padH) Mod sH <> 0 Then
					Dim d As Double = (inHeight - kH + 2 * padH) / (CDbl(sH)) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inHeight / (CDbl(sH)))))
					Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, True) & vbLf & "Combination of kernel size, stride and padding are not valid for given input height, " & "using ConvolutionMode.Strict" & vbLf & "ConvolutionMode.Strict requires: output height = (input height - kernelSize + " & "2*padding)/stride + 1 in height dimension to be an integer. Got: (" & inHeight & " - " & kH & " + 2*" & padH & ")/" & sH & " + 1 = " & str & vbLf & "See ConvolutionType enumeration Javadoc and ""Constraints on strides"" at " & "http://cs231n.github.io/convolutional-networks/" & vbLf & "To truncate/crop the input, such that output height = floor(" & str & ") = " & truncated & ", use ConvolutionType.Truncate." & vbLf & "Alternatively use ConvolutionType.Same, which will use padding to give an " & "output height of ceil(" & inHeight & "/" & sH & ")=" & sameSize & vbLf & getConfigErrorCommonLastLine1D(inputType, kH, sH, padH, outputDepth, convolutionMode))
				End If

			ElseIf convolutionMode = ConvolutionMode.Same Then

				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inHeight / (CDbl(sH)))))

				Return InputType.recurrent(outputDepth, outH)
			End If

			Dim outH As Integer = (inHeight - kH + 2 * padH) / sH + 1
			Return InputType.recurrent(outputDepth, outH)
		End Function

		''' @deprecated Use <seealso cref="getOutputTypeCnnLayers(InputType, Integer[], Integer[], Integer[], Integer[], ConvolutionMode, Long, Long, String, CNN2DFormat, Class)"/> 
		<Obsolete("Use <seealso cref=""getOutputTypeCnnLayers(InputType, Integer[], Integer[], Integer[], Integer[], ConvolutionMode, Long, Long, String, CNN2DFormat, Class)""/>")>
		Public Shared Function getOutputTypeCnnLayers(ByVal inputType As InputType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal outputDepth As Long, ByVal layerIdx As Long, ByVal layerName As String, ByVal layerClass As Type) As InputType
			Return getOutputTypeCnnLayers(inputType, kernelSize, stride, padding, dilation, convolutionMode, outputDepth, layerIdx, layerName, CNN2DFormat.NCHW, layerClass)
		End Function

		Public Shared Function getOutputTypeCnnLayers(ByVal inputType As InputType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal outputDepth As Long, ByVal layerIdx As Long, ByVal layerName As String, ByVal format As CNN2DFormat, ByVal layerClass As Type) As InputType

			If convolutionMode = Nothing Then
				Dim name As String = If(layerName Is Nothing, "(not named)", layerName)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New DL4JInvalidConfigException("Invalid configuration: convolution mode is null for layer (idx=" & layerIdx & ", name=" & name & ", type=" & layerClass.FullName & ")")
			End If


			Dim i As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)

			Dim inHeight As Long = i.getHeight()
			Dim inWidth As Long = i.getWidth()
			'rearrange height/width for input calculations for new output type
			If format <> i.getFormat() Then
				'NCHW
				'convert NWHC to NCHW
				If format = CNN2DFormat.NCHW Then
					inWidth = i.getChannels()
					outputDepth = i.getWidth()
				'NHWC
				'convert NWHC to NCHW
				ElseIf format = CNN2DFormat.NHWC Then
					inWidth = i.getChannels()
					outputDepth = i.getWidth()
				End If
			End If
			Dim padH As Integer = (If(padding Is Nothing, 0, padding(0))) 'May be null for ConvolutionMode.Same
			Dim padW As Integer = (If(padding Is Nothing, 0, padding(1)))
			Dim kH As Integer = kernelSize(0)
			Dim kW As Integer = kernelSize(1)
			If dilation(0) <> 1 Then
				'Use *effective* kernel size, accounting for dilation
				kH = kH + (kH - 1) * (dilation(0) - 1)
			End If
			If dilation(1) <> 1 Then
				kW = kW + (kW - 1) * (dilation(1) - 1)
			End If

			Dim sH As Integer = stride(0)
			Dim sW As Integer = stride(1)
			If sH <= 0 OrElse sW <= 0 Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, sH <= 0) & " Invalid strides: strides must be > 0 (strideH = " & sH & ", strideW = " & sW & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputDepth, convolutionMode))
			End If
			'note the padding check > 0 here. This validation fails for padding == 0. Verified on resnet50
			If kH <= 0 OrElse padH > 0 AndAlso (padH > 0 AndAlso kH > inHeight + 2 * padH) Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, True) & " Invalid input configuration for kernel height. Require 0 < kH <= inHeight + 2*padH; got (kH=" & kH & ", inHeight=" & inHeight & ", padH=" & padH & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputDepth, convolutionMode))
			End If

			'note the padding check > 0 here. This validation fails for padding == 0. Verified on resnet50
			If kW <= 0 OrElse padW > 0 AndAlso (padW > 0 AndAlso kW > inWidth + 2 * padW) Then
				Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, False) & " Invalid input configuration for kernel width. Require 0 < kW <= inWidth + 2*padW; got (kW=" & kW & ", inWidth=" & inWidth & ", padW=" & padW & ")" & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputDepth, convolutionMode))
			End If

			'Strict mode: require exactly the right size...
			If convolutionMode = ConvolutionMode.Strict Then
				If (inHeight - kH + 2 * padH) Mod sH <> 0 Then
					Dim d As Double = (inHeight - kH + 2 * padH) / (CDbl(sH)) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inHeight / (CDbl(stride(0))))))
					Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, True) & vbLf & "Combination of kernel size, stride and padding are not valid for given input height, using ConvolutionMode.Strict" & vbLf & "ConvolutionMode.Strict requires: output height = (input height - kernelSize + 2*padding)/stride + 1 in height dimension to be an integer. Got: (" & inHeight & " - " & kH & " + 2*" & padH & ")/" & sH & " + 1 = " & str & vbLf & "See ConvolutionType enumeration Javadoc and ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/" & vbLf & "To truncate/crop the input, such that output height = floor(" & str & ") = " & truncated & ", use ConvolutionType.Truncate." & vbLf & "Alternatively use ConvolutionType.Same, which will use padding to give an output height of ceil(" & inHeight & "/" & stride(0) & ")=" & sameSize & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputDepth, convolutionMode))
				End If


				If (inWidth - kW + 2 * padW) Mod sW <> 0 Then
					Dim d As Double = (inWidth - kW + 2 * padW) / (CDbl(sW)) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inWidth / (CDbl(stride(1))))))
					Throw New DL4JInvalidConfigException(getConfigErrorCommonLine(layerIdx, layerName, layerClass, False) & vbLf & "Combination of kernel size, stride and padding are not valid for given input width, using ConvolutionMode.Strict" & vbLf & "ConvolutionMode.Strict requires: output width = (input width - kernelSize + 2*padding)/stride + 1 in width dimension to be an integer. Got: (" & inWidth & " - " & kW & " + 2*" & padW & ")/" & sW & " + 1 = " & str & vbLf & "See ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/ and ConvolutionType enumeration Javadoc." & vbLf & "To truncate/crop the input, such that output width = floor(" & str & ") = " & truncated & ", use ConvolutionType.Truncate." & vbLf & "Alternatively use ConvolutionType.Same, which will use padding to give an output width of ceil(" & inWidth & "/" & stride(1) & ")=" & sameSize & vbLf & getConfigErrorCommonLastLine(inputType, kernelSize, stride, padding, outputDepth, convolutionMode))
				End If
			ElseIf convolutionMode = ConvolutionMode.Same Then
				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inHeight / (CDbl(stride(0))))))
				Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inWidth / (CDbl(stride(1))))))
				Return InputType.convolutional(outH, outW, outputDepth, format)
			End If



			Dim hOut As Long = (inHeight - kH + 2 * padH) \ sH + 1
			Dim wOut As Long = (inWidth - kW + 2 * padW) \ sW + 1
			Return InputType.convolutional(hOut, wOut, outputDepth, format)
		End Function

		Private Shared Function getConfigErrorCommonLine(ByVal layerIdx As Long, ByVal layerName As String, ByVal layerClass As Type, ByVal isHeight As Boolean) As String
			Dim name As String = If(layerName Is Nothing, "(not named)", layerName)
			Dim layerType As String = layerClass.Name

			Return "Invalid configuration for layer (idx=" & layerIdx & ", name=" & name & ", type=" & layerType & ") for " & (If(isHeight, "height", "width")) & " dimension: "
		End Function

		Private Shared Function getConfigErrorCommonLastLine1D(ByVal inputType As InputType, ByVal kernelSize As Integer, ByVal stride As Integer, ByVal padding As Integer, ByVal outputDepth As Long, ByVal convolutionMode As ConvolutionMode) As String
			Return "Input type = " & inputType & ", kernel = " & kernelSize & ", strides = " & stride & ", padding = " & padding & ", layer size (output channels) = " & outputDepth & ", convolution mode = " & convolutionMode
		End Function

		Private Shared Function getConfigErrorCommonLastLine(ByVal inputType As InputType, ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal outputDepth As Long, ByVal convolutionMode As ConvolutionMode) As String
			Return "Input type = " & inputType & ", kernel = " & Arrays.toString(kernelSize) & ", strides = " & Arrays.toString(stride) & ", padding = " & Arrays.toString(padding) & ", layer size (output channels) = " & outputDepth & ", convolution mode = " & convolutionMode
		End Function

		''' <summary>
		''' Utility method for determining the appropriate preprocessor for CNN layers, such as <seealso cref="ConvolutionLayer"/> and
		''' <seealso cref="SubsamplingLayer"/>
		''' </summary>
		''' <param name="inputType">     Input type to get the preprocessor for </param>
		''' <returns>              Null if no preprocessor is required; otherwise the appropriate preprocessor for the given input type </returns>
		Public Shared Function getPreProcessorForInputTypeCnn3DLayers(ByVal inputType As InputType, ByVal layerName As String) As InputPreProcessor
			Select Case inputType.getType()
				Case FF
					log.info("Automatic addition of FF -> CNN3D preprocessors: not yet implemented (layer name: """ & layerName & """)")
					Return Nothing
				Case RNN
					log.warn("Automatic addition of RNN -> CNN3D preprocessors: not yet implemented (layer name: """ & layerName & """)")
					Return Nothing
				' TODO: handle CNN to CNN3D
				Case CNN3D
					Return Nothing
				Case Else
					Throw New Exception("Unknown input type: " & inputType)
			End Select
		End Function

		''' <summary>
		''' Utility method for determining the appropriate preprocessor for CNN layers, such as <seealso cref="ConvolutionLayer"/> and
		''' <seealso cref="SubsamplingLayer"/>
		''' </summary>
		''' <param name="inputType">     Input type to get the preprocessor for </param>
		''' <returns>              Null if no preprocessor is required; otherwise the appropriate preprocessor for the given input type </returns>
		Public Shared Function getPreProcessorForInputTypeCnnLayers(ByVal inputType As InputType, ByVal layerName As String) As InputPreProcessor

			'To add x-to-CNN preprocessor: need to know image channels/width/height after reshaping
			'But this can't be inferred from the FF/RNN activations directly (could be anything)

			Select Case inputType.getType()
				Case FF
					'FF -> CNN
					'                return new FeedForwardToCnnPreProcessor(inputSize[0], inputSize[1], inputDepth);
					log.info("Automatic addition of FF -> CNN preprocessors: not yet implemented (layer name: """ & layerName & """)")
					Return Nothing
				Case RNN
					'RNN -> CNN
					'                return new RnnToCnnPreProcessor(inputSize[0], inputSize[1], inputDepth);
					log.warn("Automatic addition of RNN -> CNN preprocessors: not yet implemented (layer name: """ & layerName & """)")
					Return Nothing
				Case CNN
					'CNN -> CNN: no preprocessor required
					Return Nothing
				Case CNNFlat
					'CNN (flat) -> CNN
					Dim f As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
					Return New FeedForwardToCnnPreProcessor(f.getHeight(), f.getWidth(), f.getDepth())
				Case Else
					Throw New Exception("Unknown input type: " & inputType)
			End Select
		End Function

		Public Shared Function getPreprocessorForInputTypeRnnLayers(ByVal inputType As InputType, ByVal rnnDataFormat As RNNFormat, ByVal layerName As String) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for RNN layer (layer name = """ & layerName & """): input type is null")
			End If

			Select Case inputType.getType()
				Case CNNFlat
					'FF -> RNN or CNNFlat -> RNN
					'In either case, input data format is a row vector per example
					Return New FeedForwardToRnnPreProcessor(rnnDataFormat)
				Case FF
					'If time distributed format is defined, use that. Otherwise use the layer-defined rnnDataFormat, which may be default
					Dim ff As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
					If ff.getTimeDistributedFormat() IsNot Nothing AndAlso TypeOf ff.getTimeDistributedFormat() Is RNNFormat Then
						Return New FeedForwardToRnnPreProcessor(CType(ff.getTimeDistributedFormat(), RNNFormat))
					End If
					Return New FeedForwardToRnnPreProcessor(rnnDataFormat)
				Case RNN
					'RNN -> RNN: No preprocessor necessary
					Return Nothing
				Case CNN
					'CNN -> RNN
					Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
					Return New CnnToRnnPreProcessor(c.getHeight(), c.getWidth(), c.getChannels(), rnnDataFormat)
				Case Else
					Throw New Exception("Unknown input type: " & inputType)
			End Select
		End Function

		''' <summary>
		''' Convert multiple types when multiple are found.
		''' Only handles simple obvious cases, otherwise errs on throwing an exception.
		''' Useful for multiple input vertices such as <seealso cref="org.deeplearning4j.nn.conf.graph.MergeVertex"/>
		'''  and <seealso cref="org.deeplearning4j.nn.conf.graph.ElementWiseVertex"/> </summary>
		''' <param name="vertexInputs"> the input types to convert </param>
		Public Shared Sub convertMultipleTypes(ByVal vertexInputs() As InputType)
			Dim counter As New Counter(Of InputType.Type)()
			For i As Integer = 0 To vertexInputs.Length - 1
				counter.incrementCount(vertexInputs(i).getType(),1.0)
			Next i

			Dim maxType As InputType.Type = counter.argMax()
			'more than one type
			'convert feed forward to rnn and back
			If counter.size() > 1 Then
				Select Case maxType
					Case InputType.Type.FF
						For i As Integer = 0 To vertexInputs.Length - 1
							If vertexInputs(i).getType() <> maxType Then
								Select Case vertexInputs(i).getType()
									Case RNN
										Dim recurrent As InputType.InputTypeRecurrent = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent)
										If recurrent.getTimeSeriesLength() = 1 Then
											vertexInputs(i) = InputType.feedForward(recurrent.getSize())
										End If
									Case Else
										Throw New System.ArgumentException("Attempted conversion of types and was unable to")
								End Select
							End If
						Next i
					Case InputType.Type.RNN
						Dim rnnFormat As RNNFormat = Nothing
						For i As Integer = 0 To vertexInputs.Length - 1
							If vertexInputs(i).getType() = InputType.Type.RNN Then
								Dim firstRecurrent As InputType.InputTypeRecurrent = DirectCast(vertexInputs(i), InputType.InputTypeRecurrent)
								rnnFormat = firstRecurrent.getFormat()
								Exit For

							End If
						Next i
						For i As Integer = 0 To vertexInputs.Length - 1
							If vertexInputs(i).getType() <> maxType Then
								Select Case vertexInputs(i).getType()
									Case FF
										Dim ff As InputType.InputTypeFeedForward = DirectCast(vertexInputs(i), InputType.InputTypeFeedForward)
										vertexInputs(i) = InputType.recurrent(ff.getSize(),rnnFormat)
									Case Else
										Throw New System.ArgumentException("Attempted conversion of types and was unable to")

								End Select
							End If
						Next i
				End Select
			End If
		End Sub
	End Class

End Namespace