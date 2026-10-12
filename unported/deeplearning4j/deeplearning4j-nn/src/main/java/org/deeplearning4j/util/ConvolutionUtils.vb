Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping2D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping2D
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports BroadcastCopyOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastCopyOp
Imports MaxPooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPooling2D
Imports PaddingMode = org.nd4j.linalg.api.ops.impl.layers.convolution.config.PaddingMode
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
Imports Assign = org.nd4j.linalg.api.ops.impl.transforms.custom.Assign
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
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

Namespace org.deeplearning4j.util



	Public Class ConvolutionUtils

		Public Shared ReadOnly NCHW_NHWC_ERROR_MSG As String = "Note: Convolution layers can be configured for either NCHW (channels first)" & " or NHWC (channels last) format for input images and activations." & vbLf & "Layers can be configured using .dataFormat(CNN2DFormat.NCHW/NHWC) when constructing the layer, or for the entire net using" & " .setInputType(InputType.convolutional(height, width, depth, CNN2DForman.NCHW/NHWC))." & vbLf & "ImageRecordReader and NativeImageLoader can also be configured to load image data in either NCHW or NHWC format which must match the network"


		Private Shared ReadOnly ONES() As Integer = {1, 1}


		Private Sub New()
		End Sub

		''' <summary>
		''' Use <seealso cref="getOutputSize(INDArray, Integer[], Integer[], Integer[], ConvolutionMode, Integer[], CNN2DFormat)"/>
		''' </summary>
		<Obsolete>
		Public Shared Function getOutputSize(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionMode As ConvolutionMode) As Integer()
			Return getOutputSize(inputData, kernel, strides, padding, convolutionMode, ONES)
		End Function

		''' <summary>
		''' Get the output size of a deconvolution operation for given input data. In deconvolution, we compute the inverse
		''' of the shape computation of a convolution.
		''' </summary>
		''' <param name="inputData">       Input data </param>
		''' <param name="kernel">          Kernel size (height/width) </param>
		''' <param name="strides">         Strides (height/width) </param>
		''' <param name="padding">         Padding (height/width) </param>
		''' <param name="convolutionMode"> Convolution mode (Same, Strict, Truncate) </param>
		''' <param name="dilation">        Kernel dilation (height/width) </param>
		''' <returns> Output size: int[2] with output height/width </returns>
		Public Shared Function getDeconvolutionOutputSize(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat) As Integer()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim hDim As Integer = If(nchw, 2, 1)
			Dim wDim As Integer = If(nchw, 3, 2)

			If inputData.size(hDim) > Integer.MaxValue OrElse inputData.size(wDim) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim hIn As Integer = CInt(inputData.size(hDim))
			Dim wIn As Integer = CInt(inputData.size(wDim))
			Dim eKernel() As Integer = effectiveKernelSize(kernel, dilation)

			If convolutionMode = ConvolutionMode.Same Then
				Dim hOut As Integer = strides(0) * hIn
				Dim wOut As Integer = strides(1) * wIn
				Return New Integer(){hOut, wOut}
			End If

			Dim hOut As Integer = strides(0) * (hIn - 1) + eKernel(0) - 2 * padding(0)
			Dim wOut As Integer = strides(1) * (wIn - 1) + eKernel(1) - 2 * padding(1)

			Return New Integer(){hOut, wOut}
		End Function

		''' <summary>
		''' Get the output size of a deconvolution operation for given input data. In deconvolution, we compute the inverse
		''' of the shape computation of a convolution.
		''' </summary>
		''' <param name="inputData">       Input data </param>
		''' <param name="kernel">          Kernel size (height/width) </param>
		''' <param name="strides">         Strides (height/width) </param>
		''' <param name="padding">         Padding (height/width) </param>
		''' <param name="convolutionMode"> Convolution mode (Same, Strict, Truncate) </param>
		''' <param name="dilation">        Kernel dilation (height/width) </param>
		''' <returns> Output size: int[2] with output height/width </returns>
		Public Shared Function getDeconvolution3DOutputSize(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dataFormat As Convolution3D.DataFormat) As Long()

			Dim hIn, wIn, dIn As Long
			If dataFormat = Convolution3D.DataFormat.NCDHW Then
				hIn = inputData.size(2)
				wIn = inputData.size(3)
				dIn = inputData.size(4)
			Else
				hIn = inputData.size(1)
				wIn = inputData.size(2)
				dIn = inputData.size(3)
			End If


			Dim eKernel() As Integer = effectiveKernelSize(kernel, dilation)

			If convolutionMode = ConvolutionMode.Same Then
				Dim hOut As Long = strides(0) * hIn
				Dim wOut As Long = strides(1) * wIn
				Dim dOut As Long = strides(2) * dIn
				Return New Long(){hOut, wOut, dOut}
			End If

			Dim hOut As Long = strides(0) * (hIn - 1) + eKernel(0) - 2 * padding(0)
			Dim wOut As Long = strides(1) * (wIn - 1) + eKernel(1) - 2 * padding(1)
			Dim dOut As Long = strides(2) * (dIn - 1) + eKernel(2) - 2 * padding(2)

			Return New Long(){hOut, wOut, dOut}
		End Function


		''' @deprecated Use <seealso cref="getOutputSize(INDArray, Integer[], Integer[], Integer[], ConvolutionMode, Integer[], CNN2DFormat)"/> 
		<Obsolete("Use <seealso cref=""getOutputSize(INDArray, Integer[], Integer[], Integer[], ConvolutionMode, Integer[], CNN2DFormat)""/>")>
		Public Shared Function getOutputSize(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer) As Integer()
			Return getOutputSize(inputData, kernel, strides, padding, convolutionMode, dilation, CNN2DFormat.NCHW)
		End Function

		''' <summary>
		''' Returns true if a layer has a
		''' <seealso cref="CNN2DFormat"/> property.
		''' This is currently in use for:
		''' <seealso cref="ConvolutionLayer"/>,
		''' <seealso cref="SubsamplingLayer"/>,
		''' <seealso cref="Upsampling2D"/>,
		''' <seealso cref="SpaceToBatchLayer"/>,
		''' <seealso cref="SpaceToDepthLayer"/>,
		''' <seealso cref="ZeroPaddingLayer"/>,
		''' <seealso cref="SeparableConvolution2D"/>,
		''' <seealso cref="Cropping2D"/>,
		''' <seealso cref="DepthwiseConvolution2D"/> </summary>
		''' <param name="layer"> the layer to check </param>
		''' <returns> true if the layer is one of the above types, false otherwise </returns>
		Public Shared Function layerHasConvolutionLayout(ByVal layer As Layer) As Boolean
			Return TypeOf layer Is ConvolutionLayer OrElse TypeOf layer Is SubsamplingLayer OrElse TypeOf layer Is SpaceToBatchLayer OrElse TypeOf layer Is Upsampling2D OrElse TypeOf layer Is SpaceToDepthLayer OrElse TypeOf layer Is ZeroPaddingLayer OrElse TypeOf layer Is SeparableConvolution2D OrElse TypeOf layer Is Deconvolution2D OrElse TypeOf layer Is Cropping2D OrElse TypeOf layer Is DepthwiseConvolution2D
		End Function

		''' <summary>
		''' Get the format for a given layer.
		''' <seealso cref="layerHasConvolutionLayout(Layer)"/>
		''' should return true on the given <seealso cref="Layer"/>
		''' type or an <seealso cref="System.ArgumentException"/>
		''' will be thrown </summary>
		''' <param name="layer"> the input layer </param>
		''' <returns> the <seealso cref="CNN2DFormat"/> for the given
		''' layer </returns>
		Public Shared Function getFormatForLayer(ByVal layer As Layer) As CNN2DFormat
		   If TypeOf layer Is Convolution1DLayer Then
			   Dim convolution1DLayer As Convolution1DLayer = DirectCast(layer, Convolution1DLayer)
			   Return convolution1DLayer.getCnn2dDataFormat()
		   ElseIf TypeOf layer Is ConvolutionLayer Then
				Dim convolutionLayer As ConvolutionLayer = DirectCast(layer, ConvolutionLayer)
				Return convolutionLayer.getCnn2dDataFormat()
			ElseIf TypeOf layer Is SubsamplingLayer Then
'JAVA TO VB CONVERTER NOTE: The variable subsamplingLayer was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim subsamplingLayer_Conflict As SubsamplingLayer = DirectCast(layer, SubsamplingLayer)
				Return subsamplingLayer_Conflict.getCnn2dDataFormat()
			ElseIf TypeOf layer Is SpaceToBatchLayer Then
				Dim spaceToBatchLayer As SpaceToBatchLayer = DirectCast(layer, SpaceToBatchLayer)
				Return spaceToBatchLayer.getFormat()
			ElseIf TypeOf layer Is Upsampling2D Then
				Dim upsampling2D As Upsampling2D = DirectCast(layer, Upsampling2D)
				Return upsampling2D.getFormat()
			ElseIf TypeOf layer Is SpaceToDepthLayer Then
				Dim spaceToDepthLayer As SpaceToDepthLayer = DirectCast(layer, SpaceToDepthLayer)
				Return spaceToDepthLayer.getDataFormat()
			ElseIf TypeOf layer Is ZeroPaddingLayer Then
				Dim zeroPaddingLayer As ZeroPaddingLayer = DirectCast(layer, ZeroPaddingLayer)
				Return zeroPaddingLayer.getDataFormat()
			ElseIf TypeOf layer Is SeparableConvolution2D Then
			   Dim separableConvolution2D As SeparableConvolution2D = DirectCast(layer, SeparableConvolution2D)
			   Return separableConvolution2D.getCnn2dDataFormat()
		   ElseIf TypeOf layer Is Deconvolution2D Then
			   Dim deconvolution2D As Deconvolution2D = DirectCast(layer, Deconvolution2D)
			   Return deconvolution2D.getCnn2dDataFormat()
		   ElseIf TypeOf layer Is DepthwiseConvolution2D Then
			   Dim depthwiseConvolution2D As DepthwiseConvolution2D = DirectCast(layer, DepthwiseConvolution2D)
			   Return depthwiseConvolution2D.getCnn2dDataFormat()
		   ElseIf TypeOf layer Is Cropping2D Then
			   Dim cropping2D As Cropping2D = DirectCast(layer, Cropping2D)
			   Return cropping2D.getDataFormat()
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("Illegal type given " & layer.GetType().FullName)
			End If
		End Function


		''' <summary>
		''' Convert <seealso cref="ConvolutionMode"/>
		''' to <seealso cref="PaddingMode"/>
		''' <seealso cref="ConvolutionMode.Same"/> : <seealso cref="PaddingMode.SAME"/>
		''' <seealso cref="ConvolutionMode.Strict"/>, <seealso cref="ConvolutionMode.Truncate"/> : <seealso cref="PaddingMode.VALID"/>
		''' <seealso cref="ConvolutionMode.Causal"/> : <seealso cref="PaddingMode.VALID"/> </summary>
		''' <param name="convolutionMode"> the input <seealso cref="ConvolutionMode"/> </param>
		''' <returns> the equivalent <seealso cref="PaddingMode"/> </returns>
		Public Shared Function paddingModeForConvolutionMode(ByVal convolutionMode As ConvolutionMode) As PaddingMode
			Select Case convolutionMode
				Case ConvolutionMode.Same
					Return PaddingMode.SAME
				Case ConvolutionMode.Causal
				   Return PaddingMode.CAUSAL
				Case ConvolutionMode.Strict, Truncate
					Return PaddingMode.VALID
				Case Else
					Throw New System.ArgumentException("Invalid input convolution mode: " & convolutionMode)
			End Select
		End Function

		''' <summary>
		''' Get the output size (height/width) for the given input data and CNN configuration
		''' </summary>
		''' <param name="inputData">       Input data </param>
		''' <param name="kernel">          Kernel size (height/width) </param>
		''' <param name="strides">         Strides (height/width) </param>
		''' <param name="padding">         Padding (height/width) </param>
		''' <param name="convolutionMode"> Convolution mode (Same, Strict, Truncate) </param>
		''' <param name="dilation">        Kernel dilation (height/width) </param>
		''' <param name="format">          Format for input activations </param>
		''' <returns> Output size: int[2] with output height/width </returns>
		Public Shared Function getOutputSize(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat) As Integer()
			Dim hDim As Integer = 2
			Dim wDim As Integer = 3

			If format = CNN2DFormat.NHWC Then
				hDim = 1
				wDim = 2
			End If

			If inputData.size(hDim) > Integer.MaxValue OrElse inputData.size(wDim) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim inH As Integer = CInt(inputData.size(hDim))
			Dim inW As Integer = CInt(inputData.size(wDim))

			'Determine the effective kernel size, accounting for dilation
			'http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions
			Dim eKernel() As Integer = effectiveKernelSize(kernel, dilation)
			Dim atrous As Boolean = (eKernel Is kernel)

			Dim inShape() As Integer = {inH, inW}
			validateShapes(inputData, eKernel, strides, padding, convolutionMode, dilation, inShape, atrous)

			If convolutionMode = ConvolutionMode.Same OrElse convolutionMode = ConvolutionMode.Causal Then

				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides(0))))))
				Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strides(1))))))

				Return New Integer(){outH, outW}
			End If

			Dim hOut As Integer = (inH - eKernel(0) + 2 * padding(0)) \ strides(0) + 1
			Dim wOut As Integer = (inW - eKernel(1) + 2 * padding(1)) \ strides(1) + 1

			Return New Integer(){hOut, wOut}
		End Function

		Public Shared Sub validateShapes(ByVal inputData As INDArray, ByVal eKernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal inShape() As Integer, ByVal atrous As Boolean)

			Dim inH As Integer = inShape(0)
			Dim inW As Integer = inShape(1)

			Dim t As Boolean = (convolutionMode = ConvolutionMode.Truncate)

			If t AndAlso (eKernel(0) <= 0 OrElse eKernel(0) > inH + 2 * padding(0)) Then
				Dim sb As New StringBuilder()
				sb.Append("Invalid input data or configuration: ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel height and input height must satisfy 0 < ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel height <= input height + 2 * padding height. " & vbLf & "Got ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel height = ").Append(eKernel(0)).Append(", input height = ").Append(inH).Append(" and padding height = ").Append(padding(0)).Append(" which do not satisfy 0 < ").Append(eKernel(0)).Append(" <= ").Append(inH + 2 * padding(0)).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))

				Throw New DL4JInvalidInputException(sb.ToString())
			End If

			If t AndAlso (eKernel(1) <= 0 OrElse eKernel(1) > inW + 2 * padding(1)) Then
				Dim sb As New StringBuilder()
				sb.Append("Invalid input data or configuration: ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel width and input width must satisfy  0 < kernel width <= input width + 2 * padding width. ")
				sb.Append(vbLf & "Got ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel width = ").Append(eKernel(1)).Append(", input width = ").Append(inW).Append(" and padding width = ").Append(padding(1)).Append(" which do not satisfy 0 < ").Append(eKernel(1)).Append(" <= ").Append(inW + 2 * padding(1)).Append(vbLf & "Input size: [numExamples,inputDepth,inputHeight,inputWidth]=").Append(Arrays.toString(inputData.shape())).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))

				Throw New DL4JInvalidInputException(sb.ToString())
			End If

			If eKernel.Length = 3 AndAlso t AndAlso (eKernel(2) <= 0 OrElse eKernel(2) > inShape(2) + 2 * padding(2)) Then
				Dim inD As Integer = inShape(2)
				Dim sb As New StringBuilder()
				sb.Append("Invalid input data or configuration: ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel channels and input channels must satisfy 0 < ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel channels <= input channels + 2 * padding channels. " & vbLf & "Got ")
				If atrous Then
					sb.Append("effective ")
				End If
				sb.Append("kernel channels = ").Append(eKernel(2)).Append(", input channels = ").Append(inD).Append(" and padding height = ").Append(padding(2)).Append(" which do not satisfy 0 < ").Append(eKernel(2)).Append(" <= ").Append(inD + 2 * padding(2)).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))

				Throw New DL4JInvalidInputException(sb.ToString())
			End If

			If convolutionMode = ConvolutionMode.Strict Then
				If (inH - eKernel(0) + 2 * padding(0)) Mod strides(0) <> 0 Then
					Dim d As Double = (inH - eKernel(0) + 2 * padding(0)) / (CDbl(strides(0))) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides(0))))))

					Dim sb As New StringBuilder()
					sb.Append("Invalid input data or configuration: Combination of kernel size, stride and padding are not valid for given input height, using ConvolutionMode.Strict" & vbLf).Append("ConvolutionMode.Strict requires: output height = (input height - kernelSize + 2*padding)/stride + 1 to be an integer. Got: (").Append(inH).Append(" - ").Append(eKernel(0)).Append(" + 2*").Append(padding(0)).Append(")/").Append(strides(0)).Append(" + 1 = ").Append(str).Append(vbLf).Append("See ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/ and ConvolutionType enumeration Javadoc." & vbLf).Append("To truncate/crop the input, such that output height = floor(").Append(str).Append(") = ").Append(truncated).Append(", use ConvolutionType.Truncate." & vbLf).Append("Alternatively use ConvolutionType.Same, which will use padding to give an output height of ceil(").Append(inH).Append("/").Append(strides(0)).Append(")=").Append(sameSize).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))

					Throw New DL4JInvalidConfigException(sb.ToString())
				End If

				If (inW - eKernel(1) + 2 * padding(1)) Mod strides(1) <> 0 Then
					Dim d As Double = (inW - eKernel(1) + 2 * padding(1)) / (CDbl(strides(1))) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strides(1))))))
					Dim sb As New StringBuilder()
					sb.Append("Invalid input data or configuration: Combination of kernel size, stride and padding are not valid for given input width, using ConvolutionMode.Strict" & vbLf).Append("ConvolutionMode.Strict requires: output width = (input - kernelSize + 2*padding)/stride + 1 to be an integer. Got: (").Append(inW).Append(" - ").Append(eKernel(1)).Append(" + 2*").Append(padding(1)).Append(")/").Append(strides(1)).Append(" + 1 = ").Append(str).Append(vbLf).Append("See ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/ and ConvolutionType enumeration Javadoc." & vbLf).Append("To truncate/crop the input, such that output width = floor(").Append(str).Append(") = ").Append(truncated).Append(", use ConvolutionType.Truncate." & vbLf).Append("Alternatively use ConvolutionType.Same, which will use padding to give an output width of ceil(").Append(inW).Append("/").Append(strides(1)).Append(")=").Append(sameSize).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))
					Throw New DL4JInvalidConfigException(sb.ToString())
				End If

				If eKernel.Length = 3 AndAlso (inShape(2) - eKernel(2) + 2 * padding(2)) Mod strides(2) <> 0 Then
					Dim inD As Integer = inShape(2)
					Dim d As Double = (inD - eKernel(2) + 2 * padding(2)) / (CDbl(strides(2))) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inD / (CDbl(strides(2))))))
					Dim sb As New StringBuilder()
					sb.Append("Invalid input data or configuration: Combination of kernel size, stride and padding are not valid for given input width, using ConvolutionMode.Strict" & vbLf).Append("ConvolutionMode.Strict requires: output channels = (input - kernelSize + 2*padding)/stride + 1 to be an integer. Got: (").Append(inD).Append(" - ").Append(eKernel(2)).Append(" + 2*").Append(padding(2)).Append(")/").Append(strides(1)).Append(" + 1 = ").Append(str).Append(vbLf).Append("See ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/ and ConvolutionType enumeration Javadoc." & vbLf).Append("To truncate/crop the input, such that output width = floor(").Append(str).Append(") = ").Append(truncated).Append(", use ConvolutionType.Truncate." & vbLf).Append("Alternatively use ConvolutionType.Same, which will use padding to give an output width of ceil(").Append(inW).Append("/").Append(strides(2)).Append(")=").Append(sameSize).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))
					Throw New DL4JInvalidConfigException(sb.ToString())
				End If
			End If

		End Sub

		Public Shared Function effectiveKernelSize(ByVal kernel() As Integer, ByVal dilation() As Integer) As Integer()
			'Determine the effective kernel size, accounting for dilation
			'http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions
			If kernel.Length = 2 Then
				If dilation(0) = 1 AndAlso dilation(1) = 1 Then
					Return kernel
				Else
					Return New Integer(){ kernel(0) + (kernel(0) - 1) * (dilation(0) - 1), kernel(1) + (kernel(1) - 1) * (dilation(1) - 1)}
				End If
			ElseIf kernel.Length = 3 Then
				If dilation(0) = 1 AndAlso dilation(1) = 1 AndAlso dilation(2) = 1 Then
					Return kernel
				Else
					Return New Integer(){ kernel(0) + (kernel(0) - 1) * (dilation(0) - 1), kernel(1) + (kernel(1) - 1) * (dilation(1) - 1), kernel(2) + (kernel(2) - 1) * (dilation(2) - 1) }
				End If
			Else
				Throw New System.ArgumentException("Kernel size has to be either two or three, got: " & kernel.Length)
			End If
		End Function

		Private Shared Function getCommonErrorMsg(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer) As String
			Dim s As String = vbLf & "Input size: [numExamples,inputDepth,inputHeight,inputWidth]=" & Arrays.toString(inputData.shape()) & ", inputKernel=" & Arrays.toString(kernel)
			If dilation(0) <> 1 OrElse dilation(1) <> 1 Then
				Dim effectiveKernel() As Integer = effectiveKernelSize(kernel, dilation)
				s &= ", effectiveKernelGivenDilation=" & Arrays.toString(effectiveKernel)
			End If
			Return s & ", strides=" & Arrays.toString(strides) & ", padding=" & Arrays.toString(padding) & ", dilation=" & Arrays.toString(dilation)
		End Function

		''' <summary>
		''' Get top and left padding for same mode only.
		''' </summary>
		''' <param name="outSize">  Output size (length 2 array, height dimension first) </param>
		''' <param name="inSize">   Input size (length 2 array, height dimension first) </param>
		''' <param name="kernel">   Kernel size (length 2 array, height dimension first) </param>
		''' <param name="strides">  Strides  (length 2 array, height dimension first) </param>
		''' <param name="dilation"> Dilation (length 2 array, height dimension first) </param>
		''' <returns> Top left padding (length 2 array, height dimension first) </returns>
		Public Shared Function getSameModeTopLeftPadding(ByVal outSize() As Integer, ByVal inSize() As Integer, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal dilation() As Integer) As Integer()
			Dim eKernel() As Integer = effectiveKernelSize(kernel, dilation)
			Dim outPad(kernel.Length - 1) As Integer
			Dim allGt0 As Boolean = True

			For i As Integer = 0 To kernel.Length - 1
				outPad(i) = ((outSize(i) - 1) * strides(i) + eKernel(i) - inSize(i)) \ 2 'Note that padBottom is 1 bigger than this if bracketed term is not divisible by 2
				allGt0 = allGt0 And outPad(i) >= 0
			Next i

			Preconditions.checkState(allGt0, "Invalid padding values calculated: %s - layer configuration is invalid? Input size %s, output size %s, kernel %s, strides %s, dilation %s", outPad, inSize, outSize, kernel, strides, dilation)

			Return outPad
		End Function

		''' <summary>
		''' Get bottom and right padding for same mode only.
		''' </summary>
		''' <param name="outSize">  Output size (length 2 array, height dimension first) </param>
		''' <param name="inSize">   Input size (length 2 array, height dimension first) </param>
		''' <param name="kernel">   Kernel size (length 2 array, height dimension first) </param>
		''' <param name="strides">  Strides  (length 2 array, height dimension first) </param>
		''' <param name="dilation"> Dilation (length 2 array, height dimension first) </param>
		''' <returns> Bottom right padding (length 2 array, height dimension first) </returns>
		Public Shared Function getSameModeBottomRightPadding(ByVal outSize() As Integer, ByVal inSize() As Integer, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal dilation() As Integer) As Integer()
			Dim eKernel() As Integer = effectiveKernelSize(kernel, dilation)
			Dim outPad(1) As Integer
			outPad(0) = ((outSize(0) - 1) * strides(0) + eKernel(0) - inSize(0) + 1) \ 2 'Note that padTop is 1 smaller than this if bracketed term is not divisible by 2
			outPad(1) = ((outSize(1) - 1) * strides(1) + eKernel(1) - inSize(1) + 1) \ 2 'As above
			Preconditions.checkState(outPad(0) >= 0 AndAlso outPad(1) >= 0, "Invalid padding values calculated: %s - layer configuration is invalid? Input size %s, output size %s, kernel %s, strides %s, dilation %s", outPad, inSize, outSize, kernel, strides, dilation)
			Return outPad
		End Function

		''' <summary>
		''' Get the height and width
		''' from the configuration
		''' </summary>
		''' <param name="conf"> the configuration to get height and width from </param>
		''' <returns> the configuration to get height and width from </returns>
		Public Shared Function getHeightAndWidth(ByVal conf As NeuralNetConfiguration) As Integer()
			Return getHeightAndWidth(CType(conf.getLayer(), ConvolutionLayer).getKernelSize())
		End Function


		''' <param name="conf"> the configuration to get
		'''             the number of kernels from </param>
		''' <returns> the number of kernels/filters to apply </returns>
		Public Shared Function numFeatureMap(ByVal conf As NeuralNetConfiguration) As Long
			Return CType(conf.getLayer(), ConvolutionLayer).getNOut()
		End Function

		''' <summary>
		''' Get the height and width
		''' for an image
		''' </summary>
		''' <param name="shape"> the shape of the image </param>
		''' <returns> the height and width for the image </returns>
		Public Shared Function getHeightAndWidth(ByVal shape() As Integer) As Integer()
			If shape.Length < 2 Then
				Throw New System.ArgumentException("No width and height able to be found: array must be at least length 2")
			End If
			Return New Integer(){shape(shape.Length - 1), shape(shape.Length - 2)}
		End Function

		''' <summary>
		''' Returns the number of
		''' feature maps for a given shape (must be at least 3 dimensions
		''' </summary>
		''' <param name="shape"> the shape to get the
		'''              number of feature maps for </param>
		''' <returns> the number of feature maps
		''' for a particular shape </returns>
		Public Shared Function numChannels(ByVal shape() As Integer) As Integer
			If shape.Length < 4 Then
				Return 1
			End If
			Return shape(1)
		End Function


		''' <summary>
		''' Check that the convolution mode is consistent with the padding specification
		''' </summary>
		Public Shared Sub validateConvolutionModePadding(ByVal mode As ConvolutionMode, ByVal padding() As Integer)
			If mode = ConvolutionMode.Same Then
				Dim nullPadding As Boolean = True
				For Each i As Integer In padding
					If i <> 0 Then
						nullPadding = False
					End If
				Next i
				If Not nullPadding Then
					Throw New System.ArgumentException("Padding cannot be used when using the `same' convolution mode")
				End If
			End If
		End Sub

		''' <summary>
		''' Perform validation on the CNN layer kernel/stride/padding. Expect 2d int[], with values > 0 for kernel size and
		''' stride, and values >= 0 for padding.
		''' </summary>
		''' <param name="kernelSize"> Kernel size array to check </param>
		''' <param name="stride">     Stride array to check </param>
		''' <param name="padding">    Padding array to check </param>
		Public Shared Sub validateCnnKernelStridePadding(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
			If kernelSize Is Nothing OrElse kernelSize.Length <> 2 Then
				Throw New System.InvalidOperationException("Invalid kernel size: expected int[] of length 2, got " & (If(kernelSize Is Nothing, Nothing, Arrays.toString(kernelSize))))
			End If

			If stride Is Nothing OrElse stride.Length <> 2 Then
				Throw New System.InvalidOperationException("Invalid stride configuration: expected int[] of length 2, got " & (If(stride Is Nothing, Nothing, Arrays.toString(stride))))
			End If

			If padding Is Nothing OrElse padding.Length <> 2 Then
				Throw New System.InvalidOperationException("Invalid padding configuration: expected int[] of length 2, got " & (If(padding Is Nothing, Nothing, Arrays.toString(padding))))
			End If

			If kernelSize(0) <= 0 OrElse kernelSize(1) <= 0 Then
				Throw New System.InvalidOperationException("Invalid kernel size: values must be positive (> 0) for all dimensions. Got: " & Arrays.toString(kernelSize))
			End If

			If stride(0) <= 0 OrElse stride(1) <= 0 Then
				Throw New System.InvalidOperationException("Invalid stride configuration: values must be positive (> 0) for all dimensions. Got: " & Arrays.toString(stride))
			End If

			If padding(0) < 0 OrElse padding(1) < 0 Then
				Throw New System.InvalidOperationException("Invalid padding configuration: values must be >= 0 for all dimensions. Got: " & Arrays.toString(padding))
			End If
		End Sub


		Public Shared Function reshape4dTo2d(ByVal [in] As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			Return reshape4dTo2d([in], CNN2DFormat.NCHW, workspaceMgr, type)
		End Function

		Public Shared Function reshape4dTo2d(ByVal [in] As INDArray, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			If [in].rank() <> 4 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 4, got rank " & [in].rank() & " with shape " & Arrays.toString([in].shape()))
			End If
			Dim shape As val = [in].shape()

			If format = CNN2DFormat.NCHW Then
				'Reshape: from [n,c,h,w] to [n*h*w,c]
				Dim [out] As INDArray = [in].permute(0, 2, 3, 1)
				If [out].ordering() <> "c"c OrElse Not Shape.strideDescendingCAscendingF([out]) Then
					[out] = workspaceMgr.dup(type, [out], "c"c)
				End If
				Return workspaceMgr.leverageTo(type, [out].reshape("c"c, shape(0) * shape(2) * shape(3), shape(1)))
			Else
				'Reshape: from [n,h,w,c] to [n*h*w,c]
				If [in].ordering() <> "c"c OrElse Not Shape.strideDescendingCAscendingF([in]) Then
					[in] = workspaceMgr.dup(type, [in], "c"c)
				End If
				Return workspaceMgr.leverageTo(type, [in].reshape("c"c, shape(0) * shape(1) * shape(2), shape(3)))
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray reshape5dTo2d(@NonNull Convolution3D.DataFormat format, org.nd4j.linalg.api.ndarray.INDArray in, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr, org.deeplearning4j.nn.workspace.ArrayType type)
		Public Shared Function reshape5dTo2d(ByVal format As Convolution3D.DataFormat, ByVal [in] As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			Preconditions.checkState([in].rank() = 5, "Invalid input: expect NDArray with rank 5, got rank %ndRank with shape %ndShape", [in], [in])
			'Reshape: from either [n,c,d,h,w] to [n*d*h*w,c] (NCDHW format)
			' or reshape from [n,d,h,w,c] to [n*d*h*w,c] (NDHWC format)
			If format <> Convolution3D.DataFormat.NDHWC Then
				[in] = [in].permute(0, 2, 3, 4, 1)
			End If

			If [in].ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape([in]) Then
				[in] = workspaceMgr.dup(type, [in], "c"c)
			End If
			Return workspaceMgr.leverageTo(type, [in].reshape("c"c, [in].size(0)*[in].size(1)*[in].size(2)*[in].size(3), [in].size(4)))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray reshapeCnn3dMask(@NonNull Convolution3D.DataFormat format, org.nd4j.linalg.api.ndarray.INDArray mask, org.nd4j.linalg.api.ndarray.INDArray label, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr, org.deeplearning4j.nn.workspace.ArrayType type)
		Public Shared Function reshapeCnn3dMask(ByVal format As Convolution3D.DataFormat, ByVal mask As INDArray, ByVal label As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			If mask Is Nothing Then
				Return Nothing
			End If
			Preconditions.checkState(mask.rank() = 5, "Expected rank 5 mask for Cnn3DLossLayer in a shape broadcastable to labels shape:" & " got mask shape %ndShape with label shape %ndShape", mask, label)

			If mask.equalShapes(label) OrElse (format = Convolution3D.DataFormat.NDHWC AndAlso mask.size(0) = label.size(0) AndAlso mask.size(1) = label.size(1) AndAlso mask.size(2) = label.size(2) AndAlso mask.size(3) = label.size(3)) OrElse (format = Convolution3D.DataFormat.NDHWC AndAlso mask.size(0) = label.size(0) AndAlso mask.size(2) = label.size(2) AndAlso mask.size(3) = label.size(3) AndAlso mask.size(4) = label.size(4)) Then
				'Already OK shape for reshaping
				Return reshape5dTo2d(format, mask, workspaceMgr, type)
			Else
				'Need to broadcast first
				Dim lShape() As Long = CType(label.shape().Clone(), Long())
				Dim channelIdx As Integer = If(format = Convolution3D.DataFormat.NCDHW, 1, 4)
				lShape(channelIdx) = mask.size(channelIdx) 'Keep existing channel size

				Dim bMask As INDArray = workspaceMgr.createUninitialized(type, mask.dataType(), lShape, "c"c)
				Nd4j.exec(New Assign(New INDArray(){bMask, mask}, New INDArray(){bMask}))
				Return reshape5dTo2d(format, bMask, workspaceMgr, type)
			End If
		End Function

		Public Shared Function reshape2dTo4d(ByVal in2d As INDArray, ByVal toShape() As Long, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			If in2d.rank() <> 2 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 2")
			End If
			If toShape.Length <> 4 Then
				Throw New System.ArgumentException("Invalid input: expect toShape with 4 elements: got " & Arrays.toString(toShape))
			End If

			If in2d.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(in2d) Then
				in2d = workspaceMgr.dup(type, in2d, "c"c)
			End If

			If format = CNN2DFormat.NCHW Then
				'Reshape: from [n*h*w,c] to [n,h,w,c] to [n,c,h,w]
				Dim [out] As INDArray = in2d.reshape("c"c, toShape(0), toShape(2), toShape(3), toShape(1))
				Return workspaceMgr.leverageTo(type, [out].permute(0, 3, 1, 2))
			Else
				'Reshape: from [n*h*w,c] to [n,h,w,c]
				Return workspaceMgr.leverageTo(type, in2d.reshape("c"c, toShape))
			End If
		End Function

		Public Shared Function reshape2dTo5d(ByVal format As Convolution3D.DataFormat, ByVal in2d As INDArray, ByVal n As Long, ByVal d As Long, ByVal h As Long, ByVal w As Long, ByVal ch As Long, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			If in2d.rank() <> 2 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 2")
			End If

			'Reshape: from [n*d*h*w,c] to [n,d,h,w,c]; if NCDHW format permute to [n,c,d,h,w]
			If in2d.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(in2d) Then
				in2d = workspaceMgr.dup(type, in2d, "c"c)
			End If

			Dim ndhwc As INDArray = in2d.reshape("c"c, n, d, h, w, ch)
			If format = Convolution3D.DataFormat.NDHWC Then
				Return workspaceMgr.leverageTo(type, ndhwc)
			Else
				Return workspaceMgr.leverageTo(type, ndhwc.permute(0, 4, 1, 2, 3))
			End If
		End Function

		''' @deprecated Use <seealso cref="reshapeMaskIfRequired(INDArray, INDArray, CNN2DFormat, LayerWorkspaceMgr, ArrayType)"/> 
		<Obsolete("Use <seealso cref=""reshapeMaskIfRequired(INDArray, INDArray, CNN2DFormat, LayerWorkspaceMgr, ArrayType)""/>")>
		Public Shared Function reshapeMaskIfRequired(ByVal mask As INDArray, ByVal output As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			Return reshapeMaskIfRequired(mask, output, Nothing, workspaceMgr, type)
		End Function

		Public Shared Function reshapeMaskIfRequired(ByVal mask As INDArray, ByVal output As INDArray, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			If mask Is Nothing Then
				Return Nothing
			End If
			If mask.rank() = 2 Then
				Return adapt2dMask(mask, output, format, workspaceMgr, type)
			ElseIf mask.rank() = 3 Then
				Return reshape3dMask(mask, workspaceMgr, type)
			Else
				Return reshape4dTo2d(mask, workspaceMgr, type)
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray adapt2dMask(org.nd4j.linalg.api.ndarray.INDArray mask, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull CNN2DFormat format, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr, org.deeplearning4j.nn.workspace.ArrayType type)
		Public Shared Function adapt2dMask(ByVal mask As INDArray, ByVal output As INDArray, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray

			If format = CNN2DFormat.NCHW Then
				'Input in [n,c,h,w] which is reshaped to [n*h*w,c], mask is [n,1]
				'So: We'll broadcast to [n,1,h,w] then reshape to [n*h*w,1] required for the current DL4J loss functions...

				'Use workaround for: https://github.com/deeplearning4j/nd4j/issues/2066

				Dim s As val = output.shape()
				Dim bMask As INDArray = workspaceMgr.create(type, mask.dataType(), New Long(){s(0), 1, s(2), s(3)}, "c"c)
				Nd4j.Executioner.exec(New BroadcastCopyOp(bMask, mask, bMask, 0, 1))

				Dim bMaskPermute As INDArray = bMask.permute(0, 2, 3, 1).dup("c"c) 'Not sure if dup is strictly necessary...

				Return workspaceMgr.leverageTo(type, bMaskPermute.reshape("c"c, s(0) * s(2) * s(3), 1))
			Else
				'Input in [n,h,w,c] which is reshaped to [n*h*w,c], mask is [n,1]
				'So: We'll broadcast to [n,h,w,1] then reshape to [n*h*w,1] required for the current DL4J loss functions...
				Dim s As val = output.shape()
				Dim bMask As INDArray = workspaceMgr.create(type, mask.dataType(), New Long(){s(0), s(2), s(3), 1}, "c"c)
				Nd4j.Executioner.exec(New BroadcastCopyOp(bMask, mask, bMask, 0, 3))

				Return workspaceMgr.leverageTo(type, bMask.reshape("c"c, s(0) * s(2) * s(3), 1))
			End If
		End Function

		Public Shared Function reshape3dMask(ByVal mask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			'Assume mask has shape [n,h,w] and will be broadcast along dimension
			If mask.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(mask) Then
				mask = workspaceMgr.dup(type, mask, "c"c)
			End If

			Return mask.reshape("c"c, mask.length(), 1)
		End Function

		Public Shared Function reshape4dMask(ByVal mask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			Return reshape4dTo2d(mask, workspaceMgr, arrayType)
		End Function

		''' <summary>
		''' Get heigh/width/channels as length 3 int[] from the InputType
		''' </summary>
		''' <param name="inputType"> Input type to get </param>
		''' <returns> Length </returns>
		Public Shared Function getHWDFromInputType(ByVal inputType As InputType) As Integer()
			Dim inH As Integer
			Dim inW As Integer
			Dim inDepth As Integer

			If TypeOf inputType Is InputType.InputTypeConvolutional Then
				Dim conv As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
				If conv.getHeight() > Integer.MaxValue OrElse conv.getWidth() > Integer.MaxValue OrElse conv.getChannels() > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				inH = CInt(Math.Truncate(conv.getHeight()))
				inW = CInt(Math.Truncate(conv.getWidth()))
				inDepth = CInt(Math.Truncate(conv.getChannels()))
			ElseIf TypeOf inputType Is InputType.InputTypeConvolutionalFlat Then
				Dim conv As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
				If conv.getHeight() > Integer.MaxValue OrElse conv.getWidth() > Integer.MaxValue OrElse conv.getDepth() > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				inH = CInt(Math.Truncate(conv.getHeight()))
				inW = CInt(Math.Truncate(conv.getWidth()))
				inDepth = CInt(Math.Truncate(conv.getDepth()))
			Else
				Throw New System.InvalidOperationException("Invalid input type: expected InputTypeConvolutional or InputTypeConvolutionalFlat." & " Got: " & inputType)
			End If
			Return New Integer(){inH, inW, inDepth}
		End Function

		''' <summary>
		''' Given a mask array for a 1D CNN layer of shape [minibatch, sequenceLength], reduce the mask according to the 1D CNN layer configuration.
		''' Unlike RNN layers, 1D CNN layers may down-sample the data; consequently, we need to down-sample the mask array
		''' in the same way, to maintain the correspondence between the masks and the output activations
		''' </summary>
		''' <param name="in">       Input size </param>
		''' <param name="kernel">   Kernel size </param>
		''' <param name="stride">   Stride </param>
		''' <param name="padding">  Padding </param>
		''' <param name="dilation"> Dilation </param>
		''' <param name="cm">       Convolution mode </param>
		''' <returns> Reduced mask </returns>
		Public Shared Function cnn1dMaskReduction(ByVal [in] As INDArray, ByVal kernel As Integer, ByVal stride As Integer, ByVal padding As Integer, ByVal dilation As Integer, ByVal cm As ConvolutionMode) As INDArray
			Preconditions.checkState([in].rank()=2, "Rank must be 2 for cnn1d mask array - shape ", [in].shape())
			If (cm = ConvolutionMode.Same OrElse cm = ConvolutionMode.Causal) AndAlso stride = 1 Then
				Return [in]
			End If

			If Not Shape.hasDefaultStridesForShape([in]) Then
				[in] = [in].dup()
			End If

			Dim reshaped4d As INDArray = [in].reshape(ChrW([in].size(0)), 1, [in].size(1), 1)

			Dim outSize() As Integer
			Dim pad() As Integer = Nothing
			Dim k() As Integer = {kernel, 1}
			Dim s() As Integer = {stride, 1}
			Dim d() As Integer = {dilation, 1}
			If cm = ConvolutionMode.Same OrElse cm = ConvolutionMode.Causal Then
				outSize = ConvolutionUtils.getOutputSize(reshaped4d, k, s, Nothing, cm, d, CNN2DFormat.NCHW) 'Also performs validation
			Else
				pad = New Integer(){padding, 0}
				outSize = ConvolutionUtils.getOutputSize(reshaped4d, k, s, pad, cm, d, CNN2DFormat.NCHW) 'Also performs validation
			End If
			Dim outH As Integer = outSize(0)

			Dim output As INDArray = Nd4j.createUninitialized(New Integer(){CInt([in].size(0)), 1, outH, 1}, "c"c)

			Dim op As DynamicCustomOp = New MaxPooling2D(reshaped4d, output, Pooling2DConfig.builder().kH(k(0)).kW(k(1)).sH(s(0)).sW(s(1)).pH(If(pad Is Nothing, 0, pad(0))).pW(If(pad Is Nothing, 0, pad(1))).dH(d(0)).dW(d(1)).isSameMode(cm = ConvolutionMode.Same OrElse cm = ConvolutionMode.Causal).isNHWC(False).build())

			Nd4j.Executioner.exec(op)
			Return output.reshape("c"c, [in].size(0), outH)
		End Function

		''' <summary>
		''' Reduce a 2d CNN layer mask array (of 0s and 1s) according to the layer configuration. Note that when a CNN layer
		''' changes the shape of the activations (for example, stride > 1) the corresponding mask array needs to change shape
		''' also (as there is a correspondence between the two). This method performs the forward pass for the mask. </summary>
		''' <param name="inMask">          Input mask array - rank 4, shape [mb,c,h,1] or [mb,c,w,1] or [mb,c,h,w] </param>
		''' <param name="kernel">          Kernel configuration for the layer </param>
		''' <param name="stride">          Stride </param>
		''' <param name="padding">         Padding </param>
		''' <param name="dilation">        Dilation </param>
		''' <param name="convolutionMode"> Convolution mode </param>
		''' <returns> The mask array corresponding to the network output </returns>
		Public Shared Function cnn2dMaskReduction(ByVal inMask As INDArray, ByVal kernel() As Integer, ByVal stride() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer, ByVal convolutionMode As ConvolutionMode) As INDArray
			'Mask array should be broadcastable with CNN activations. Thus should have shape [mb,x,y,z]
			'where:
			' x == 1 OR channels
			' y == 1 OR height
			' z == 1 OR width

			If inMask.rank() <> 4 Then
				Throw New System.InvalidOperationException("Expected rank 4 mask array for 2D CNN layers. Mask arrays for 2D CNN layers " & "must have shape [batchSize,channels,X,Y] where X = (1 or activationsHeight) and Y = (1 or activationsWidth): " & "Got rank " & inMask.rank() & " array with shape " & Arrays.toString(inMask.shape()))
			End If

			If convolutionMode = ConvolutionMode.Same AndAlso stride(0) = 1 AndAlso stride(1) = 1 Then
				'Output activations size same as input activations size
				Return inMask
			End If

			If inMask.size(2) = 1 AndAlso inMask.size(3) = 1 Then
				'per-example mask - broadcast along all channels/x/y
				Return inMask
			End If

			Dim k() As Integer
			Dim s() As Integer
			Dim p() As Integer
			Dim d() As Integer
			If inMask.size(3) = 1 Then
				'[mb,x,y,1] case -> pool mask along height
				k = New Integer(){kernel(0), 1}
				s = New Integer(){stride(0), 1}
				p = New Integer(){padding(0), 0}
				d = New Integer(){dilation(0), 1}
			ElseIf inMask.size(2) = 1 Then
				'[mb,x,1,z] case -> pool mask along width
				k = New Integer(){1, kernel(1)}
				s = New Integer(){1, stride(1)}
				p = New Integer(){0, padding(1)}
				d = New Integer(){1, dilation(1)}
			Else
				'[mb,x,y,z] -> pool mask along height and width
				k = kernel
				s = stride
				p = padding
				d = dilation
			End If

			Dim outSize() As Integer = ConvolutionUtils.getOutputSize(inMask, k, s, p, convolutionMode, d) 'Also performs validation
			Dim allEq As Boolean = True
			For i As Integer = 0 To outSize.Length - 1
				If outSize(i) <> inMask.size(i) Then
					allEq = False
					Exit For
				End If
			Next i
			If allEq Then
				'Same output size -> same mask size
				Return inMask
			End If

			Dim outArraySize() As Long = {inMask.size(0), inMask.size(1), outSize(0), outSize(1)}
			Dim outMask As INDArray = Nd4j.createUninitialized(inMask.dataType(), outArraySize)

			Dim op As DynamicCustomOp = New MaxPooling2D(inMask, outMask, Pooling2DConfig.builder().kH(k(0)).kW(k(1)).sH(s(0)).sW(s(1)).pH(p(0)).pW(p(1)).dH(d(0)).dW(d(1)).isSameMode(convolutionMode = ConvolutionMode.Same).isNHWC(False).build())

			Nd4j.exec(op)
			Return outMask
		End Function
	End Class

End Namespace