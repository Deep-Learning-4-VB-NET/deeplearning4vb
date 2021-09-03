Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports LengthUnit = org.deeplearning4j.ui.api.LengthUnit
Imports Style = org.deeplearning4j.ui.api.Style
Imports Utils = org.deeplearning4j.ui.api.Utils
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

Namespace org.deeplearning4j.ui.components.table.style



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) public class StyleTable extends org.deeplearning4j.ui.api.Style
	Public Class StyleTable
		Inherits Style

		Private columnWidths() As Double
		Private columnWidthUnit As LengthUnit
		Private borderWidthPx As Integer?
		Private headerColor As String
		Private Shadows backgroundColor As String
		Private whitespaceMode As String


		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.columnWidths = builder.columnWidths_Conflict
			Me.columnWidthUnit = builder.columnWidthUnit
			Me.borderWidthPx = builder.borderWidthPx
			Me.headerColor = builder.headerColor_Conflict
			Me.backgroundColor = builder.backgroundColor_Conflict
			Me.whitespaceMode = builder.whitespaceMode_Conflict
		End Sub

		'No arg constructor for Jackson
		Private Sub New()

		End Sub


		Public Class Builder
			Inherits Style.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field columnWidths was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend columnWidths_Conflict() As Double
			Friend columnWidthUnit As LengthUnit
			Friend borderWidthPx As Integer?
'JAVA TO VB CONVERTER NOTE: The field headerColor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend headerColor_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field backgroundColor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend Shadows backgroundColor_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field whitespaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend whitespaceMode_Conflict As String

			''' <summary>
			''' Specify the widths for the columns
			''' </summary>
			''' <param name="unit">   Unit that the widths are specified in </param>
			''' <param name="widths"> Width values for the columns </param>
			Public Overridable Function columnWidths(ByVal unit As LengthUnit, ParamArray ByVal widths() As Double) As Builder
				Me.columnWidthUnit = unit
				Me.columnWidths_Conflict = widths
				Return Me
			End Function

			''' <param name="borderWidthPx">    Width of the border, in px </param>
			Public Overridable Function borderWidth(ByVal borderWidthPx As Integer) As Builder
				Me.borderWidthPx = borderWidthPx
				Return Me
			End Function

			''' <param name="color">    Background color for the header row </param>
			Public Overridable Function headerColor(ByVal color As Color) As Builder
				Dim hex As String = Utils.colorToHex(color)
				Return headerColor(hex)
			End Function

			''' <param name="color">    Background color for the header row </param>
			Public Overridable Function headerColor(ByVal color As String) As Builder
				If Not color.ToLower().matches("#[a-f0-9]{6}") Then
					Throw New System.ArgumentException("Invalid color: must be hex format. Got: " & color)
				End If
				Me.headerColor_Conflict = color
				Return Me
			End Function

			''' <param name="color">    Background color for the table cells (ex. header row) </param>
			Public Overrides Function backgroundColor(ByVal color As Color) As Builder
				Dim hex As String = Utils.colorToHex(color)
				Return backgroundColor(hex)
			End Function

			''' <param name="color">    Background color for the table cells (ex. header row) </param>
			Public Overrides Function backgroundColor(ByVal color As String) As Builder
				If Not color.ToLower().matches("#[a-f0-9]{6}") Then
					Throw New System.ArgumentException("Invalid color: must be hex format. Got: " & color)
				End If
				Me.backgroundColor_Conflict = color
				Return Me
			End Function

			''' <summary>
			''' Set the whitespace mode (CSS style tag). For example, "pre" to maintain current formatting with no wrapping,
			''' "pre-wrap" to wrap (but otherwise take into account new line characters in text, etc)
			''' </summary>
			''' <param name="whitespaceMode">    CSS whitespace mode </param>
'JAVA TO VB CONVERTER NOTE: The parameter whitespaceMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function whitespaceMode(ByVal whitespaceMode_Conflict As String) As Builder
				Me.whitespaceMode_Conflict = whitespaceMode_Conflict
				Return Me
			End Function

			Public Overridable Function build() As StyleTable
				Return New StyleTable(Me)
			End Function
		End Class

	End Class

End Namespace