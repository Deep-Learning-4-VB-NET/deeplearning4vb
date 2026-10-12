Imports System
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
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor public class Conv3DConfig extends BaseConvolutionConfig
	Public Class Conv3DConfig
		Inherits BaseConvolutionConfig

		Public Const NDHWC As String = "NDHWC"
'JAVA TO VB CONVERTER NOTE: The field NCDHW was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Const NCDHW_Conflict As String = "NCDHW"

		'kernel
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kD = -1;
		Private kD As Long = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kW = -1;
		Private kW As Long = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long kH = -1;
		Private kH As Long = -1

		'strides
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sD = 1;
		Private sD As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sW = 1;
		Private sW As Long = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long sH = 1;
		Private sH As Long = 1

		'padding
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pD = 0;
		Private pD As Long = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pW = 0;
		Private pW As Long = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long pH = 0;
		Private pH As Long = 0

		'dilations
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
'ORIGINAL LINE: @Builder.@Default private boolean biasUsed = false;
		Private biasUsed As Boolean = False
		Private isSameMode As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private String dataFormat = NDHWC;
		Private dataFormat As String = NDHWC

		Public Sub New(ByVal kD As Long, ByVal kW As Long, ByVal kH As Long, ByVal sD As Long, ByVal sW As Long, ByVal sH As Long, ByVal pD As Long, ByVal pW As Long, ByVal pH As Long, ByVal dD As Long, ByVal dW As Long, ByVal dH As Long, ByVal biasUsed As Boolean, ByVal isSameMode As Boolean, ByVal dataFormat As String)
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
			Me.biasUsed = biasUsed
			Me.isSameMode = isSameMode
			Me.dataFormat = dataFormat

			validate()
		End Sub

		Public Overridable ReadOnly Property NCDHW As Boolean
			Get
				Preconditions.checkState(dataFormat.Equals(NCDHW_Conflict, StringComparison.OrdinalIgnoreCase) OrElse dataFormat.Equals(NDHWC, StringComparison.OrdinalIgnoreCase), "Data format must be one of %s or %s, got %s", NCDHW_Conflict, NDHWC, dataFormat)
				Return dataFormat.Equals(NCDHW_Conflict, StringComparison.OrdinalIgnoreCase)
			End Get
		End Property

		Public Overridable Sub isNCDHW(ByVal isNCDHW As Boolean)
			If isNCDHW Then
				dataFormat = NCDHW_Conflict
			Else
				dataFormat = NDHWC
			End If
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
			ret("biasUsed") = biasUsed
			ret("dataFormat") = dataFormat
			ret("isSameMode") = isSameMode

			Return ret
		End Function

		Protected Friend Overrides Sub validate()
			ConvConfigUtil.validate3D(kH, kW, kD, sH, sW, sD, pH, pW, pD, dH, dW, dD)
			Preconditions.checkArgument(dataFormat IsNot Nothing, "Data format can't be null")
		End Sub


	End Class

End Namespace