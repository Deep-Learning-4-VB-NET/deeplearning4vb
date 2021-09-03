Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping1D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping1D
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException

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



	Public Class Convolution1DUtils

		Private Const ONE As Integer = 1


		Private Sub New()
		End Sub


		Public Shared Function getOutputSize(ByVal inputData As INDArray, ByVal kernel As Integer, ByVal strides As Integer, ByVal padding As Integer, ByVal convolutionMode As ConvolutionMode) As Integer
			Return getOutputSize(inputData, kernel, strides, padding, convolutionMode, ONE)
		End Function

		''' <summary>
		''' Returns true if the given layer has an
		''' <seealso cref="RNNFormat"/>.
		''' This is true for:
		''' <seealso cref="Convolution1DLayer"/>,
		''' <seealso cref="Subsampling1DLayer"/>
		''' <seealso cref="SimpleRnn"/>
		''' <seealso cref="LSTM"/>
		''' <seealso cref="EmbeddingSequenceLayer"/> </summary>
		''' <param name="layer"> the layer to test </param>
		''' <returns> true if the input layer has an rnn format
		''' false otherwise </returns>
		Public Shared Function hasRnnDataFormat(ByVal layer As Layer) As Boolean
			Return TypeOf layer Is Convolution1D OrElse TypeOf layer Is Convolution1DLayer OrElse TypeOf layer Is Subsampling1DLayer OrElse TypeOf layer Is SimpleRnn OrElse TypeOf layer Is LSTM OrElse TypeOf layer Is EmbeddingSequenceLayer
		End Function

		''' <summary>
		''' Get the <seealso cref="RNNFormat"/> for the given layer.
		''' Throws an <seealso cref="System.ArgumentException"/>
		''' if a layer doesn't have an rnn format </summary>
		''' <param name="layer"> the layer to get the format for </param>
		''' <returns> the format for the layer </returns>
		Public Shared Function getRnnFormatFromLayer(ByVal layer As Layer) As RNNFormat
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Preconditions.checkState(hasRnnDataFormat(layer),"Layer of type " & layer.GetType().FullName & " and name " & layer.LayerName & " does not have an RNNFormat")
			If TypeOf layer Is SimpleRnn Then
				Dim simpleRnn As SimpleRnn = DirectCast(layer, SimpleRnn)
				Return simpleRnn.getRnnDataFormat()
			ElseIf TypeOf layer Is Convolution1D Then
				Dim convolution1D As Convolution1D = DirectCast(layer, Convolution1D)
				Return convolution1D.getRnnDataFormat()
			ElseIf TypeOf layer Is Convolution1DLayer Then
				Dim convolution1DLayer As Convolution1DLayer = DirectCast(layer, Convolution1DLayer)
				Return convolution1DLayer.getRnnDataFormat()
			ElseIf TypeOf layer Is Subsampling1DLayer Then
				Dim subsampling1DLayer As Subsampling1DLayer = DirectCast(layer, Subsampling1DLayer)
				Return If(subsampling1DLayer.getCnn2dDataFormat() = CNN2DFormat.NCHW, RNNFormat.NCW, RNNFormat.NWC)
			ElseIf TypeOf layer Is LSTM Then
				Dim lstm As LSTM = DirectCast(layer, LSTM)
				Return lstm.getRnnDataFormat()
			ElseIf TypeOf layer Is EmbeddingSequenceLayer Then
				Dim embeddingSequenceLayer As EmbeddingSequenceLayer = DirectCast(layer, EmbeddingSequenceLayer)
				Return embeddingSequenceLayer.getOutputFormat()
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("Illegal layer type " & layer.GetType().FullName & " and name " & layer.LayerName)
			End If
		End Function

		''' <summary>
		''' Reshapes the given weight
		''' array or weight gradient
		''' to work with the specified
		''' <seealso cref="RNNFormat"/> </summary>
		''' <param name="w"> the weight array or gradient </param>
		''' <param name="rnnFormat"> the <seealso cref="RNNFormat"/> to use </param>
		''' <returns> the reshaped array. </returns>
		Public Shared Function reshapeWeightArrayOrGradientForFormat(ByVal w As INDArray, ByVal rnnFormat As RNNFormat) As INDArray
			If rnnFormat = RNNFormat.NWC Then
				w = w.reshape(w.ordering(), w.size(0), w.size(1), w.size(2)).permute(2, 1, 0) '[oC, iC, k, 1] to [k, iC, oC]
			Else
				w = w.reshape(w.ordering(),w.size(2),w.size(1),w.size(0))
			End If

			Return w
		End Function


		''' <summary>
		''' Get the output size (height) for the given input data and CNN1D configuration
		''' </summary>
		''' <param name="inH">             Input size (height, or channels). </param>
		''' <param name="kernel">          Kernel size </param>
		''' <param name="strides">         Stride </param>
		''' <param name="padding">         Padding </param>
		''' <param name="convolutionMode"> Convolution mode (Same, Strict, Truncate) </param>
		''' <param name="dilation">        Kernel dilation </param>
		''' <returns> Output size (width) </returns>
		Public Shared Function getOutputSize(ByVal inH As Long, ByVal kernel As Integer, ByVal strides As Integer, ByVal padding As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation As Integer) As Long
			Dim eKernel As Long = effectiveKernelSize(kernel, dilation)
			If convolutionMode = ConvolutionMode.Same OrElse convolutionMode = ConvolutionMode.Causal Then
				Return CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides)))))
			End If
			Return (inH - eKernel + 2 * padding) \ strides + 1
		End Function

		''' <summary>
		''' Get the output size (height) for the given input data and CNN1D configuration
		''' </summary>
		''' <param name="inputData">       Input data </param>
		''' <param name="kernel">          Kernel size </param>
		''' <param name="strides">         Stride </param>
		''' <param name="padding">         Padding </param>
		''' <param name="convolutionMode"> Convolution mode (Same, Strict, Truncate) </param>
		''' <param name="dilation">        Kernel dilation </param>
		''' <returns> Output size (width) </returns>
		Public Shared Function getOutputSize(ByVal inputData As INDArray, ByVal kernel As Integer, ByVal strides As Integer, ByVal padding As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation As Integer) As Integer
			If inputData.size(2) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim inH As Integer = CInt(inputData.size(2))
			Dim eKernel As Integer = effectiveKernelSize(kernel, dilation)
			Dim atrous As Boolean = (eKernel = kernel)
			validateShapes(inputData, eKernel, strides, padding, convolutionMode, dilation, inH, atrous)

			If convolutionMode = ConvolutionMode.Same OrElse convolutionMode = ConvolutionMode.Causal Then
				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides)))))
				Return outH
			End If

			Dim outH As Integer = (inH - eKernel + 2 * padding) \ strides + 1
			Return outH
		End Function

		Public Shared Sub validateShapes(ByVal inputData As INDArray, ByVal eKernel As Integer, ByVal strides As Integer, ByVal padding As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation As Integer, ByVal inShape As Integer, ByVal atrous As Boolean)

			Dim inH As Integer = inShape
			Dim t As Boolean = convolutionMode = ConvolutionMode.Truncate

			If t AndAlso (eKernel <= 0 OrElse eKernel > inH + 2 * padding) Then
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
				sb.Append("kernel height = ").Append(eKernel).Append(", input height = ").Append(inH).Append(" and padding height = ").Append(padding).Append(" which do not satisfy 0 < ").Append(eKernel).Append(" <= ").Append(inH + 2 * padding).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))

				Throw New DL4JInvalidInputException(sb.ToString())
			End If


			If convolutionMode = ConvolutionMode.Strict Then
				If (inH - eKernel + 2 * padding) Mod strides <> 0 Then
					Dim d As Double = (inH - eKernel + 2 * padding) / (CDbl(strides)) + 1.0
					Dim str As String = String.Format("{0:F2}", d)
					Dim truncated As Integer = CInt(Math.Truncate(d))
					Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides)))))

					Dim sb As New StringBuilder()
					sb.Append("Invalid input data or configuration: Combination of kernel size, " & "stride and padding are not " & "valid for given input height, using ConvolutionMode.Strict" & vbLf).Append("ConvolutionMode.Strict requires: output height = (input height - kernelSize + " & "2*padding)/stride + 1 to be an integer. Got: (").Append(inH).Append(" - ").Append(eKernel).Append(" + 2*").Append(padding).Append(")/").Append(strides).Append(" + 1 = ").Append(str).Append(vbLf).Append("See ""Constraints on strides"" at http://cs231n.github." & "io/convolutional-networks/ and ConvolutionType enumeration Javadoc." & vbLf).Append("To truncate/crop the input, such that output height = floor(").Append(str).Append(") = ").Append(truncated).Append(", use ConvolutionType.Truncate." & vbLf).Append("Alternatively use ConvolutionType.Same, which will use padding to give an " & "output height of ceil(").Append(inH).Append("/").Append(strides).Append(")=").Append(sameSize).Append(getCommonErrorMsg(inputData, eKernel, strides, padding, dilation))

					Throw New DL4JInvalidConfigException(sb.ToString())
				End If
			End If

		End Sub

		Public Shared Function effectiveKernelSize(ByVal kernel As Integer, ByVal dilation As Integer) As Integer
			'Determine the effective kernel size, accounting for dilation
			'http://deeplearning.net/software/theano/tutorial/conv_arithmetic.html#dilated-convolutions
			If dilation = 1 Then
				Return kernel
			Else
				Return kernel + (kernel - 1) * (dilation - 1)
			End If
		End Function

		Private Shared Function getCommonErrorMsg(ByVal inputData As INDArray, ByVal kernel As Integer, ByVal strides As Integer, ByVal padding As Integer, ByVal dilation As Integer) As String
			Dim s As String = vbLf & "Input size: [numExamples,inputDepth,inputHeight,inputWidth]=" & Arrays.toString(inputData.shape()) & ", inputKernel=" & kernel
			If dilation <> 1 Then
				Dim effectiveKernel As Integer = effectiveKernelSize(kernel, dilation)
				s &= ", effectiveKernelGivenDilation=" & effectiveKernel
			End If
			Return s & ", stride=" & strides & ", padding=" & padding & ", dilation=" & dilation
		End Function


		''' <summary>
		''' Check that the convolution mode is consistent with the padding specification
		''' </summary>
		Public Shared Sub validateConvolutionModePadding(ByVal mode As ConvolutionMode, ByVal padding As Integer)
			If mode = ConvolutionMode.Same Then
				Dim nullPadding As Boolean = True
				If padding <> 0 Then
					nullPadding = False
				End If
				If Not nullPadding Then
					Throw New System.ArgumentException("Padding cannot be used when using the `same' convolution mode")
				End If

			End If
		End Sub

		''' <summary>
		''' Get top padding for same mode only.
		''' </summary>
		''' <param name="outSize">  Output size (length 2 array, height dimension first) </param>
		''' <param name="inSize">   Input size (length 2 array, height dimension first) </param>
		''' <param name="kernel">   Kernel size (length 2 array, height dimension first) </param>
		''' <param name="strides">  Strides  (length 2 array, height dimension first) </param>
		''' <param name="dilation"> Dilation (length 2 array, height dimension first) </param>
		''' <returns> Top left padding (length 2 array, height dimension first) </returns>
		Public Shared Function getSameModeTopLeftPadding(ByVal outSize As Integer, ByVal inSize As Integer, ByVal kernel As Integer, ByVal strides As Integer, ByVal dilation As Integer) As Integer
			Dim eKernel As Integer = effectiveKernelSize(kernel, dilation)
			'Note that padBottom is 1 bigger than this if bracketed term is not divisible by 2
			Dim outPad As Integer = ((outSize - 1) * strides + eKernel - inSize) \ 2
			Preconditions.checkState(outPad >= 0, "Invalid padding values calculated: %s - " & "layer configuration is invalid? Input size %s, output size %s, kernel %s, " & "strides %s, dilation %s", outPad, inSize, outSize, kernel, strides, dilation)
			Return outPad
		End Function

		Public Shared Function getSameModeBottomRightPadding(ByVal outSize As Integer, ByVal inSize As Integer, ByVal kernel As Integer, ByVal strides As Integer, ByVal dilation As Integer) As Integer
			Dim eKernel As Integer = effectiveKernelSize(kernel, dilation)
			Dim totalPad As Integer = ((outSize - 1) * strides + eKernel - inSize)
			Dim tlPad As Integer = totalPad \ 2
			Dim brPad As Integer = totalPad - tlPad
			Preconditions.checkState(brPad >= 0, "Invalid padding values (right) calculated: %s - " & "layer configuration is invalid? Input size %s, output size %s, kernel %s, " & "strides %s, dilation %s", brPad, inSize, outSize, kernel, strides, dilation)
			Return brPad
		End Function

		''' <summary>
		''' Perform validation on the CNN layer kernel/stride/padding. Expect int, with values > 0 for kernel size and
		''' stride, and values >= 0 for padding.
		''' </summary>
		''' <param name="kernel">  Kernel size  to check </param>
		''' <param name="stride">  Stride to check </param>
		''' <param name="padding"> Padding to check </param>
		Public Shared Sub validateCnn1DKernelStridePadding(ByVal kernel As Integer, ByVal stride As Integer, ByVal padding As Integer)

			If kernel <= 0 Then
				Throw New System.InvalidOperationException("Invalid kernel size: value must be positive (> 0). Got: " & kernel)
			End If
			If stride <= 0 Then
				Throw New System.InvalidOperationException("Invalid kernel size: value must be positive (> 0). Got: " & stride)

			End If
			If padding < 0 Then
				Throw New System.InvalidOperationException("Invalid kernel size: value must be positive (> 0). Got: " & padding)
			End If
		End Sub


	End Class

End Namespace