Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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


	Public MustInherit Class BaseConvolution
		Implements ConvolutionInstance

		Public MustOverride Function convn(ByVal input As INDArray, ByVal kernel As INDArray, ByVal type As Convolution.Type, ByVal axes() As Integer) As INDArray
		''' <summary>
		''' 2d convolution (aka the last 2 dimensions
		''' </summary>
		''' <param name="input">  the input to op </param>
		''' <param name="kernel"> the kernel to convolve with </param>
		''' <param name="type">
		''' @return </param>
		Public Overridable Function conv2d(ByVal input As INDArray, ByVal kernel As INDArray, ByVal type As Convolution.Type) As INDArray Implements ConvolutionInstance.conv2d
			Dim axes() As Integer = If(input.shape().Length < 2, ArrayUtil.range(0, 1), ArrayUtil.range(input.shape().Length - 2, input.shape().Length))
			Return convn(input, kernel, type, axes)
		End Function


		''' <summary>
		''' ND Convolution
		''' </summary>
		''' <param name="input">  the input to transform </param>
		''' <param name="kernel"> the kernel to transform with </param>
		''' <param name="type">   the opType of convolution </param>
		''' <returns> the convolution of the given input and kernel </returns>
		Public Overridable Function convn(ByVal input As INDArray, ByVal kernel As INDArray, ByVal type As Convolution.Type) As INDArray Implements ConvolutionInstance.convn
			Return convn(input, kernel, type, ArrayUtil.range(0, input.shape().Length))
		End Function
	End Class

End Namespace