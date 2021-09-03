Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Pooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports Divisor = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D.Divisor
Imports Pooling2DType = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D.Pooling2DType
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
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor public class Pooling2DConfig extends BaseConvolutionConfig
	Public Class Pooling2DConfig
		Inherits BaseConvolutionConfig

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kH = -1, kW = -1;
		Private kH As Long = -1, kW As Long = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sH = 1, sW = 1;
		Private sH As Long = 1, sW As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pH = 0, pW = 0;
		Private pH As Long = 0, pW As Long = 0
		''' <summary>
		''' Extra is an optional parameter mainly for use with pnorm right now.
		''' All pooling implementations take 9 parameters save pnorm.
		''' Pnorm takes 10 and is cast to an int.
		''' </summary>
		Private extra As Double
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D.Pooling2DType type = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D.Pooling2DType.MAX;
		Private type As Pooling2D.Pooling2DType = Pooling2D.Pooling2DType.MAX
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D.Divisor divisor = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D.Divisor.EXCLUDE_PADDING;
		Private divisor As Pooling2D.Divisor = Pooling2D.Divisor.EXCLUDE_PADDING
		Private isSameMode As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dH = 1;
		Private dH As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dW = 1;
		Private dW As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean isNHWC = false;
		Private isNHWC As Boolean = False

		Public Sub New(ByVal kH As Long, ByVal kW As Long, ByVal sH As Long, ByVal sW As Long, ByVal pH As Long, ByVal pW As Long, ByVal extra As Double, ByVal type As Pooling2D.Pooling2DType, ByVal divisor As Pooling2D.Divisor, ByVal isSameMode As Boolean, ByVal dH As Long, ByVal dW As Long, ByVal isNHWC As Boolean)
			Me.kH = kH
			Me.kW = kW
			Me.sH = sH
			Me.sW = sW
			Me.pH = pH
			Me.pW = pW
			Me.extra = extra
			Me.type = type
			Me.divisor = divisor
			Me.isSameMode = isSameMode
			Me.dH = dH
			Me.dW = dW
			Me.isNHWC = isNHWC

			validate()
		End Sub

		Public Overrides Function toProperties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("kH") = kH
			ret("kW") = kW
			ret("sH") = sH
			ret("sW") = sW
			ret("pH") = pH
			ret("pW") = pW
			ret("extra") = extra
			ret("type") = type.ToString()
			ret("isSameMode") = isSameMode
			ret("dH") = dH
			ret("dW") = dW
			ret("isNHWC") = isNHWC
			Return ret
		End Function

		Protected Friend Overrides Sub validate()
			ConvConfigUtil.validate2D(kH, kW, sH, sW, pH, pW, dH, dW)

			'TODO check other args?
		End Sub

	End Class

End Namespace