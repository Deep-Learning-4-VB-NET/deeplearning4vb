Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
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

Namespace org.deeplearning4j.ui.components.text.style



	''' <summary>
	''' Style for text
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) public class StyleText extends org.deeplearning4j.ui.api.Style
	Public Class StyleText
		Inherits Style

		Private font As String
		Private fontSize As Double?
		Private underline As Boolean?
		Private color As String
		Private whitespacePre As Boolean?

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.font = builder.font_Conflict
			Me.fontSize = builder.fontSize_Conflict
			Me.underline = builder.underline_Conflict
			Me.color = builder.color_Conflict
			Me.whitespacePre = builder.whitespacePre_Conflict
		End Sub


		Public Class Builder
			Inherits Style.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field font was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend font_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field fontSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend fontSize_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field underline was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend underline_Conflict As Boolean?
'JAVA TO VB CONVERTER NOTE: The field color was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend color_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field whitespacePre was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend whitespacePre_Conflict As Boolean?

			''' <summary>
			''' Specify the font to be used for the text </summary>
'JAVA TO VB CONVERTER NOTE: The parameter font was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function font(ByVal font_Conflict As String) As Builder
				Me.font_Conflict = font_Conflict
				Return Me
			End Function

			''' <summary>
			''' Size of the font (pt) </summary>
			Public Overridable Function fontSize(ByVal size As Double) As Builder
				Me.fontSize_Conflict = size
				Return Me
			End Function

			''' <summary>
			''' If true: text should be underlined (default: not) </summary>
'JAVA TO VB CONVERTER NOTE: The parameter underline was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function underline(ByVal underline_Conflict As Boolean) As Builder
				Me.underline_Conflict = underline_Conflict
				Return Me
			End Function

			''' <summary>
			''' Color for the text </summary>
'JAVA TO VB CONVERTER NOTE: The parameter color was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function color(ByVal color_Conflict As Color) As Builder
				Return Me.color(Utils.colorToHex(color_Conflict))
			End Function

			''' <summary>
			''' Color for the text </summary>
'JAVA TO VB CONVERTER NOTE: The parameter color was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function color(ByVal color_Conflict As String) As Builder
				Me.color_Conflict = color_Conflict
				Return Me
			End Function

			''' <summary>
			''' If set to true: add a "white-space: pre" to the style.
			''' In effect, this stops the representation from compressing the whitespace characters, and messing up/removing
			''' text that contains newlines, tabs, etc.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter whitespacePre was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function whitespacePre(ByVal whitespacePre_Conflict As Boolean) As Builder
				Me.whitespacePre_Conflict = whitespacePre_Conflict
				Return Me
			End Function

			Public Overridable Function build() As StyleText
				Return New StyleText(Me)
			End Function

		End Class

	End Class

End Namespace