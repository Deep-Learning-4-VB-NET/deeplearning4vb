Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
import static org.deeplearning4j.util.ConvolutionUtils.effectiveKernelSize

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



	Public Class Convolution3DUtils

		Private Shared ReadOnly ONES() As Integer = {1, 1}


		Private Sub New()
		End Sub

		''' <summary>
		''' Get the output size (depth/height/width) for the given input data and CNN3D configuration
		''' </summary>
		''' <param name="inputData">       Input data </param>
		''' <param name="kernel">          Kernel size (depth/height/width) </param>
		''' <param name="strides">         Strides (depth/height/width) </param>
		''' <param name="padding">         Padding (depth/height/width) </param>
		''' <param name="convolutionMode"> Convolution mode (Same, Strict, Truncate) </param>
		''' <param name="dilation">        Kernel dilation (depth/height/width) </param>
		''' <returns> Output size: int[3] with output depth/height/width </returns>
		Public Shared Function get3DOutputSize(ByVal inputData As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal isNCDHW As Boolean) As Integer()

			' NCDHW vs. NDHWC
			Dim inD As Long = (If(isNCDHW, inputData.size(2), inputData.size(1)))
			Dim inH As Long = (If(isNCDHW, inputData.size(3), inputData.size(2)))
			Dim inW As Long = (If(isNCDHW, inputData.size(4), inputData.size(3)))

			Dim eKernel() As Integer = effectiveKernelSize(kernel, dilation)
			Dim atrous As Boolean = (eKernel Is kernel)

			Dim inShape As val = New Long(){inD, inH, inW}
			validateShapes(ArrayUtil.toInts(inputData.shape()), eKernel, strides, padding, convolutionMode, dilation, inShape, atrous)

			If convolutionMode = ConvolutionMode.Same Then
				Dim outD As Integer = CInt(Math.Truncate(Math.Ceiling(inD / (CDbl(strides(0))))))
				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strides(1))))))
				Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strides(2))))))

				Return New Integer(){outD, outH, outW}
			End If

			Dim outD As Integer = (CInt(inD) - eKernel(0) + 2 * padding(0)) \ strides(0) + 1
			Dim outH As Integer = (CInt(inH) - eKernel(1) + 2 * padding(1)) \ strides(1) + 1
			Dim outW As Integer = (CInt(inW) - eKernel(2) + 2 * padding(2)) \ strides(2) + 1

			Return New Integer(){outD, outH, outW}
		End Function


		Private Shared Sub validateShapes(ByVal inputDataShape() As Integer, ByVal eKernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal inShape() As Long, ByVal atrous As Boolean)

			Dim dims() As String = {"depth", "height", "width"}

			If convolutionMode <> ConvolutionMode.Same Then
				For i As Integer = 0 To 2
					If (eKernel(i) <= 0 OrElse eKernel(i) > inShape(i) + 2 * padding(i)) Then
						Dim sb As New StringBuilder()
						sb.Append("Invalid input data or configuration: ")
						If atrous Then
							sb.Append("effective ")
						End If
						sb.Append("kernel ").Append(dims(i)).Append(" and input ").Append(dims(i)).Append(" must satisfy 0 < ")
						If atrous Then
							sb.Append("effective ")
						End If
						sb.Append("kernel ").Append(dims(i)).Append(" <= input ").Append(dims(i)).Append(" + 2 * padding ").Append(dims(i)).Append(". " & vbLf & "Got ")
						If atrous Then
							sb.Append("effective ")
						End If
						sb.Append("kernel = ").Append(eKernel(i)).Append(", input ").Append(dims(i)).Append(" = ").Append(inShape(i)).Append(" and padding ").Append(dims(i)).Append(" = ").Append(padding(i)).Append(" which do not satisfy 0 < ").Append(eKernel(i)).Append(" <= ").Append(inShape(i) + 2 * padding(i)).Append(getCommonErrorMsg(inputDataShape, eKernel, strides, padding, dilation))

						Throw New DL4JInvalidInputException(sb.ToString())
					End If
				Next i
			End If
			If convolutionMode = ConvolutionMode.Strict Then
				For j As Integer = 0 To 2
					If (inShape(j) - eKernel(0) + 2 * padding(0)) Mod strides(0) <> 0 Then
						Dim d As Double = (inShape(j) - eKernel(0) + 2 * padding(0)) / (CDbl(strides(0))) + 1.0
						Dim str As String = String.Format("{0:F2}", d)
						Dim truncated As Integer = CInt(Math.Truncate(d))
						Dim sameSize As Integer = CInt(Math.Truncate(Math.Ceiling(inShape(j) / (CDbl(strides(0))))))

						Dim sb As New StringBuilder()
						sb.Append("Invalid input data or configuration: Combination of kernel size, stride and padding ").Append("are not valid for given input height, using ConvolutionMode.Strict" & vbLf).Append("ConvolutionMode.Strict requires: output height = (input height - kernelSize + ").Append("2*padding)/stride + 1 to be an integer. Got: (").Append(inShape(j)).Append(" - ").Append(eKernel(0)).Append(" + 2*").Append(padding(0)).Append(")/").Append(strides(0)).Append(" + 1 = ").Append(str).Append(vbLf).Append("See ""Constraints on strides"" at http://cs231n.github.io/convolutional-networks/ ").Append("and ConvolutionType enumeration Javadoc." & vbLf).Append("To truncate/crop the input, such that output height = floor(").Append(str).Append(") = ").Append(truncated).Append(", use ConvolutionType.Truncate." & vbLf).Append("Alternatively use ConvolutionType.Same, which will use padding to give ").Append("an output height of ceil(").Append(inShape(j)).Append("/").Append(strides(0)).Append(")=").Append(sameSize).Append(getCommonErrorMsg(inputDataShape, eKernel, strides, padding, dilation))

						Throw New DL4JInvalidConfigException(sb.ToString())
					End If
				Next j
			End If
		End Sub


		Private Shared Function getCommonErrorMsg(ByVal inputDatashape() As Integer, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal padding() As Integer, ByVal dilation() As Integer) As String
			Dim s As String = vbLf & "Input size: [numExamples, inputDepth, inputHeight, inputWidth]=" & Arrays.toString(inputDatashape) & ", inputKernel=" & Arrays.toString(kernel)
			If dilation(0) <> 1 OrElse dilation(1) <> 1 Then
				Dim effectiveKernel() As Integer = effectiveKernelSize(kernel, dilation)
				s &= ", effectiveKernelGivenDilation=" & Arrays.toString(effectiveKernel)
			End If
			Return s & ", strides=" & Arrays.toString(strides) & ", padding=" & Arrays.toString(padding) & ", dilation=" & Arrays.toString(dilation)
		End Function

		''' <summary>
		''' Get top and left padding for same mode only for 3d convolutions
		''' </summary>
		''' <param name="outSize"> </param>
		''' <param name="inSize"> </param>
		''' <param name="kernel"> </param>
		''' <param name="strides">
		''' @return </param>
		Public Shared Function get3DSameModeTopLeftPadding(ByVal outSize() As Integer, ByVal inSize() As Integer, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal dilation() As Integer) As Integer()
			Dim eKernel() As Integer = effectiveKernelSize(kernel, dilation)
			Dim outPad(2) As Integer
			outPad(0) = ((outSize(0) - 1) * strides(0) + eKernel(0) - inSize(0)) \ 2
			outPad(1) = ((outSize(1) - 1) * strides(1) + eKernel(1) - inSize(1)) \ 2
			outPad(2) = ((outSize(2) - 1) * strides(2) + eKernel(2) - inSize(2)) \ 2
			Return outPad
		End Function

		''' <summary>
		''' Perform validation on the CNN3D layer kernel/stride/padding. Expect 3d int[], with values > 0 for kernel size and
		''' stride, and values >= 0 for padding.
		''' </summary>
		''' <param name="kernelSize"> Kernel size array to check </param>
		''' <param name="stride">     Stride array to check </param>
		''' <param name="padding">    Padding array to check </param>
		Public Shared Sub validateCnn3DKernelStridePadding(ByVal kernelSize() As Integer, ByVal stride() As Integer, ByVal padding() As Integer)
			If kernelSize Is Nothing OrElse kernelSize.Length <> 3 Then
				Throw New System.InvalidOperationException("Invalid kernel size: expected int[] of length 3, got " & (If(kernelSize Is Nothing, Nothing, Arrays.toString(kernelSize))))
			End If

			If stride Is Nothing OrElse stride.Length <> 3 Then
				Throw New System.InvalidOperationException("Invalid stride configuration: expected int[] of length 3, got " & (If(stride Is Nothing, Nothing, Arrays.toString(stride))))
			End If

			If padding Is Nothing OrElse padding.Length <> 3 Then
				Throw New System.InvalidOperationException("Invalid padding configuration: expected int[] of length 3, got " & (If(padding Is Nothing, Nothing, Arrays.toString(padding))))
			End If

			If kernelSize(0) <= 0 OrElse kernelSize(1) <= 0 OrElse kernelSize(2) <= 0 Then
				Throw New System.InvalidOperationException("Invalid kernel size: values must be positive (> 0) for all dimensions. Got: " & Arrays.toString(kernelSize))
			End If

			If stride(0) <= 0 OrElse stride(1) <= 0 OrElse stride(2) <= 0 Then
				Throw New System.InvalidOperationException("Invalid stride configuration: values must be positive (> 0) for all dimensions. Got: " & Arrays.toString(stride))
			End If

			If padding(0) < 0 OrElse padding(1) < 0 OrElse padding(2) < 0 Then
				Throw New System.InvalidOperationException("Invalid padding configuration: values must be >= 0 for all dimensions. Got: " & Arrays.toString(padding))
			End If
		End Sub

	End Class

End Namespace