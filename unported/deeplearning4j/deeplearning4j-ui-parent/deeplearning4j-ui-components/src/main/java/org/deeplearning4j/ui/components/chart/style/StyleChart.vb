Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Style = org.deeplearning4j.ui.api.Style
Imports Utils = org.deeplearning4j.ui.api.Utils
Imports StyleText = org.deeplearning4j.ui.components.text.style.StyleText
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude

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

Namespace org.deeplearning4j.ui.components.chart.style


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) public class StyleChart extends org.deeplearning4j.ui.api.Style
	Public Class StyleChart
		Inherits Style

		Public Shared ReadOnly DEFAULT_CHART_MARGIN_TOP As Double? = 60.0
		Public Shared ReadOnly DEFAULT_CHART_MARGIN_BOTTOM As Double? = 20.0
		Public Shared ReadOnly DEFAULT_CHART_MARGIN_LEFT As Double? = 60.0
		Public Shared ReadOnly DEFAULT_CHART_MARGIN_RIGHT As Double? = 20.0

		Protected Friend strokeWidth As Double?
		Protected Friend pointSize As Double?
		Protected Friend seriesColors() As String
		Protected Friend axisStrokeWidth As Double?
		Protected Friend titleStyle As StyleText

		Private Sub New(ByVal b As Builder)
			MyBase.New(b)
			Me.strokeWidth = b.strokeWidth_Conflict
			Me.pointSize = b.pointSize_Conflict
			Me.seriesColors = b.seriesColors_Conflict
			Me.axisStrokeWidth = b.axisStrokeWidth_Conflict
			Me.titleStyle = b.titleStyle_Conflict
		End Sub



		Public Class Builder
			Inherits Style.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field strokeWidth was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend strokeWidth_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field pointSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend pointSize_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field seriesColors was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seriesColors_Conflict() As String
'JAVA TO VB CONVERTER NOTE: The field axisStrokeWidth was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend axisStrokeWidth_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field titleStyle was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend titleStyle_Conflict As StyleText

			Public Sub New()
				MyBase.marginTop = DEFAULT_CHART_MARGIN_TOP
				MyBase.marginBottom = DEFAULT_CHART_MARGIN_BOTTOM
				MyBase.marginLeft = DEFAULT_CHART_MARGIN_LEFT
				MyBase.marginRight = DEFAULT_CHART_MARGIN_RIGHT
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter strokeWidth was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function strokeWidth(ByVal strokeWidth_Conflict As Double) As Builder
				Me.strokeWidth_Conflict = strokeWidth_Conflict
				Return Me
			End Function

			''' <summary>
			''' Point size, for scatter plot etc </summary>
'JAVA TO VB CONVERTER NOTE: The parameter pointSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function pointSize(ByVal pointSize_Conflict As Double) As Builder
				Me.pointSize_Conflict = pointSize_Conflict
				Return Me
			End Function

			Public Overridable Function seriesColors(ParamArray ByVal colors() As Color) As Builder
				Dim str(colors.Length - 1) As String
				For i As Integer = 0 To str.Length - 1
					str(i) = Utils.colorToHex(colors(i))
				Next i
				Return seriesColors(str)
			End Function

			Public Overridable Function seriesColors(ParamArray ByVal colors() As String) As Builder
				Me.seriesColors_Conflict = colors
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter axisStrokeWidth was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function axisStrokeWidth(ByVal axisStrokeWidth_Conflict As Double) As Builder
				Me.axisStrokeWidth_Conflict = axisStrokeWidth_Conflict
				Return Me
			End Function

			Public Overridable Function titleStyle(ByVal style As StyleText) As Builder
				Me.titleStyle_Conflict = style
				Return Me
			End Function

			Public Overridable Function build() As StyleChart
				Return New StyleChart(Me)
			End Function
		End Class

	End Class

End Namespace