Imports System
Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
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
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor public class Conv1DConfig extends BaseConvolutionConfig
	Public Class Conv1DConfig
		Inherits BaseConvolutionConfig

		Public Const NCW As String = "NCW"
'JAVA TO VB CONVERTER NOTE: The field NWC was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Const NWC_Conflict As String = "NWC"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long k = -1L;
		Private k As Long = -1L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long s = 1;
		Private s As Long = 1 ' strides
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long p = 0;
		Private p As Long = 0 ' padding
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long d = 1;
		Private d As Long = 1 ' dilation
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private String dataFormat = NCW;
		Private dataFormat As String = NCW
		Private paddingMode As PaddingMode

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Conv1DConfig(long k, long s, long p, long d, String dataFormat, @NonNull PaddingMode paddingMode)
		Public Sub New(ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal d As Long, ByVal dataFormat As String, ByVal paddingMode As PaddingMode)
			Me.k = k
			Me.s = s
			Me.p = p
			Me.d = d
			Me.dataFormat = dataFormat
			Me.paddingMode = paddingMode

			validate()
		End Sub

		Public Sub New(ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal dataFormat As String, ByVal isSameMode As Boolean)
			Me.k = k
			Me.s = s
			Me.p = p
			Me.dataFormat = dataFormat
			Me.paddingMode = If(isSameMode, PaddingMode.SAME, PaddingMode.VALID)

			validate()
		End Sub

		Public Overridable ReadOnly Property NWC As Boolean
			Get
				Preconditions.checkState(dataFormat.Equals(NCW, StringComparison.OrdinalIgnoreCase) OrElse dataFormat.Equals(NWC_Conflict, StringComparison.OrdinalIgnoreCase), "Data format must be one of %s or %s, got %s", NCW, NWC_Conflict, dataFormat)
				Return dataFormat.Equals(NWC_Conflict, StringComparison.OrdinalIgnoreCase)
			End Get
		End Property

		Public Overridable Sub isNWC(ByVal isNWC As Boolean)
			If isNWC Then
				dataFormat = NWC_Conflict
			Else
				dataFormat = NCW
			End If
		End Sub

		Public Overrides Function toProperties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("k") = k
			ret("s") = s
			ret("p") = p
			ret("d") = d
			ret("paddingMode") = paddingMode
			ret("dataFormat") = dataFormat
			Return ret
		End Function

		Protected Friend Overrides Sub validate()
			ConvConfigUtil.validate1D(k, s, p, d)
			Preconditions.checkArgument(dataFormat IsNot Nothing, "Data format can't be null")
		End Sub


	End Class

End Namespace