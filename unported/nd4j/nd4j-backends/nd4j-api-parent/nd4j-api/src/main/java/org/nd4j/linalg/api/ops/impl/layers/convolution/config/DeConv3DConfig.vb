Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Preconditions = org.nd4j.common.base.Preconditions
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
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor public class DeConv3DConfig extends BaseConvolutionConfig
	Public Class DeConv3DConfig
		Inherits BaseConvolutionConfig

		Public Const NCDHW As String = "NCDHW"
		Public Const NDHWC As String = "NDHWC"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kD = -1L;
		Private kD As Long = -1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kH = -1L;
		Private kH As Long = -1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kW = -1L;
		Private kW As Long = -1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sD = 1L;
		Private sD As Long = 1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sH = 1L;
		Private sH As Long = 1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sW = 1L;
		Private sW As Long = 1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pD = 0;
		Private pD As Long = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pH = 0;
		Private pH As Long = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pW = 0;
		Private pW As Long = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dD = 1L;
		Private dD As Long = 1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dH = 1L;
		Private dH As Long = 1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long dW = 1L;
		Private dW As Long = 1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean isSameMode = false;
		Private isSameMode As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private String dataFormat = NCDHW;
		Private dataFormat As String = NCDHW

		Public Sub New(ByVal kD As Long, ByVal kH As Long, ByVal kW As Long, ByVal sD As Long, ByVal sH As Long, ByVal sW As Long, ByVal pD As Long, ByVal pH As Long, ByVal pW As Long, ByVal dD As Long, ByVal dH As Long, ByVal dW As Long, ByVal isSameMode As Boolean, ByVal dataFormat As String)
			Me.kD = kD
			Me.kH = kH
			Me.kW = kW
			Me.sD = sD
			Me.sH = sH
			Me.sW = sW
			Me.pD = pD
			Me.pH = pH
			Me.pW = pW
			Me.dD = dD
			Me.dH = dH
			Me.dW = dW
			Me.isSameMode = isSameMode
			Me.dataFormat = dataFormat

			validate()
		End Sub


		Public Overrides Function toProperties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("kD") = kD
			ret("kH") = kH
			ret("kW") = kW
			ret("sD") = sD
			ret("sH") = sH
			ret("sW") = sW
			ret("pD") = pD
			ret("pH") = pH
			ret("pW") = pW
			ret("dD") = dD
			ret("dH") = dH
			ret("dW") = dW
			ret("isSameMode") = isSameMode
			ret("dataFormat") = dataFormat
			Return ret
		End Function

		Protected Friend Overrides Sub validate()
			ConvConfigUtil.validate3D(kH, kW, kD, sH, sW, sD, pH, pW, pD, dH, dW, dD)
			Preconditions.checkArgument(dataFormat IsNot Nothing, "Data format can't be null")
		End Sub
	End Class

End Namespace