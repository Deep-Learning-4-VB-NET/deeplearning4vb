Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

	Public Class DefaultConvolutionInstance
		Inherits BaseConvolution

		''' <summary>
		''' ND Convolution
		''' </summary>
		''' <param name="input">  the input to op </param>
		''' <param name="kernel"> the kernel to op with </param>
		''' <param name="type">   the opType of convolution </param>
		''' <param name="axes">   the axes to do the convolution along </param>
		''' <returns> the convolution of the given input and kernel </returns>
		Public Overrides Function convn(ByVal input As INDArray, ByVal kernel As INDArray, ByVal type As Convolution.Type, ByVal axes() As Integer) As INDArray
			Throw New System.NotSupportedException()
		End Function

	End Class

End Namespace