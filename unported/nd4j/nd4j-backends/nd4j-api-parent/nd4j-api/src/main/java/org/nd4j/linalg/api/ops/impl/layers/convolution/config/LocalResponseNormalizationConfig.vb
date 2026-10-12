Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ConvConfigUtil = org.nd4j.linalg.util.ConvConfigUtil

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

Namespace org.nd4j.linalg.api.ops.impl.layers.convolution.config

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor public class LocalResponseNormalizationConfig extends BaseConvolutionConfig
	Public Class LocalResponseNormalizationConfig
		Inherits BaseConvolutionConfig

		Private alpha, beta, bias As Double
		Private depth As Integer

		Public Sub New(ByVal alpha As Double, ByVal beta As Double, ByVal bias As Double, ByVal depth As Integer)
			Me.alpha = alpha
			Me.beta = beta
			Me.bias = bias
			Me.depth = depth

			validate()
		End Sub

		Public Overrides Function toProperties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("alpha") = alpha
			ret("beta") = beta
			ret("bias") = bias
			ret("depth") = depth
			Return ret
		End Function

		Protected Friend Overrides Sub validate()
			ConvConfigUtil.validateLRN(alpha, beta, bias, depth)
		End Sub

	End Class

End Namespace