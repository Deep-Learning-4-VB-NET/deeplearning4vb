Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Pooling3D = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling3D
Imports Pooling3DType = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling3D.Pooling3DType
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
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor public class Pooling3DConfig extends BaseConvolutionConfig
	Public Class Pooling3DConfig
		Inherits BaseConvolutionConfig

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kD = -1, kW = -1, kH = -1;
		Private kD As Long = -1, kW As Long = -1, kH As Long = -1 ' kernel
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sD = 1, sW = 1, sH = 1;
		Private sD As Long = 1, sW As Long = 1, sH As Long = 1 ' strides
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pD = 0, pW = 0, pH = 0;
		Private pD As Long = 0, pW As Long = 0, pH As Long = 0 ' padding
		' dilation
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dD = 1;
		Private dD As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dW = 1;
		Private dW As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dH = 1;
		Private dH As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling3D.Pooling3DType type = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling3D.Pooling3DType.MAX;
		Private type As Pooling3D.Pooling3DType = Pooling3D.Pooling3DType.MAX
		Private isSameMode As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean isNCDHW = true;
		Private isNCDHW As Boolean = True

		Public Sub New(ByVal kD As Long, ByVal kW As Long, ByVal kH As Long, ByVal sD As Long, ByVal sW As Long, ByVal sH As Long, ByVal pD As Long, ByVal pW As Long, ByVal pH As Long, ByVal dD As Long, ByVal dW As Long, ByVal dH As Long, ByVal type As Pooling3D.Pooling3DType, ByVal isSameMode As Boolean, ByVal isNCDHW As Boolean)
			Me.kD = kD
			Me.kW = kW
			Me.kH = kH
			Me.sD = sD
			Me.sW = sW
			Me.sH = sH
			Me.pD = pD
			Me.pW = pW
			Me.pH = pH
			Me.dD = dD
			Me.dW = dW
			Me.dH = dH
			Me.type = type
			Me.isSameMode = isSameMode
			Me.isNCDHW = isNCDHW

			validate()
		End Sub

		Public Overrides Function toProperties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("kD") = kD
			ret("kW") = kW
			ret("kH") = kH
			ret("sD") = sD
			ret("sW") = sW
			ret("sH") = sH
			ret("pD") = pD
			ret("pW") = pW
			ret("pH") = pH
			ret("dD") = dD
			ret("dW") = dW
			ret("dH") = dH
			ret("type") = type.ToString()
			ret("isSameMode") = isSameMode
			Return ret

		End Function

		Protected Friend Overrides Sub validate()
			ConvConfigUtil.validate3D(kH, kW, kD, sH, sW, sD, pH, pW, pD, dH, dW, dD)

			'TODO check other args
		End Sub
	End Class

End Namespace