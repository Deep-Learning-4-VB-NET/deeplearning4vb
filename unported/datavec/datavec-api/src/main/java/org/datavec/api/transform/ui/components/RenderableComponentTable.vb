Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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

Namespace org.datavec.api.transform.ui.components

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class RenderableComponentTable extends RenderableComponent
	Public Class RenderableComponentTable
		Inherits RenderableComponent

		Public Const COMPONENT_TYPE As String = "simpletable"

		Private title As String
		Private header() As String
		Private table()() As String
		Private padLeftPx As Integer = 0
		Private padRightPx As Integer = 0
		Private padTopPx As Integer = 0
		Private padBottomPx As Integer = 0
		Private border As Integer = 0
		Private colWidthsPercent() As Double = Nothing
		Private backgroundColor As String
		Private headerColor As String

		Public Sub New()
			MyBase.New(COMPONENT_TYPE)
			'No arg constructor for Jackson
		End Sub

		Public Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE)
			Me.title = builder.title_Conflict
			Me.header = builder.header_Conflict
			Me.table = builder.table_Conflict
			Me.padLeftPx = builder.padLeftPx_Conflict
			Me.padRightPx = builder.padRightPx_Conflict
			Me.padTopPx = builder.padTopPx_Conflict
			Me.padBottomPx = builder.padBottomPx_Conflict
			Me.border = builder.border_Conflict
			Me.colWidthsPercent = builder.colWidthsPercent_Conflict
			Me.backgroundColor = builder.backgroundColor_Conflict
			Me.headerColor = builder.headerColor_Conflict
		End Sub

		Public Sub New(ByVal header() As String, ByVal table()() As String)
			Me.New(Nothing, header, table)
		End Sub

		Public Sub New(ByVal title As String, ByVal header() As String, ByVal table()() As String)
			MyBase.New(COMPONENT_TYPE)
			Me.title = title
			Me.header = header
			Me.table = table
		End Sub

		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field title was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend title_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field header was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend header_Conflict() As String
'JAVA TO VB CONVERTER NOTE: The field table was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend table_Conflict()() As String
'JAVA TO VB CONVERTER NOTE: The field padLeftPx was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padLeftPx_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field padRightPx was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padRightPx_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field padTopPx was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padTopPx_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field padBottomPx was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend padBottomPx_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field border was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend border_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field colWidthsPercent was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend colWidthsPercent_Conflict() As Double
'JAVA TO VB CONVERTER NOTE: The field backgroundColor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend backgroundColor_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field headerColor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend headerColor_Conflict As String

'JAVA TO VB CONVERTER NOTE: The parameter title was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function title(ByVal title_Conflict As String) As Builder
				Me.title_Conflict = title_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter header was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function header(ParamArray ByVal header_Conflict() As String) As Builder
				Me.header_Conflict = header_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter table was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function table(ByVal table_Conflict()() As String) As Builder
				Me.table_Conflict = table_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter border was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function border(ByVal border_Conflict As Integer) As Builder
				Me.border_Conflict = border_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter padLeftPx was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padLeftPx(ByVal padLeftPx_Conflict As Integer) As Builder
				Me.padLeftPx_Conflict = padLeftPx_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter padRightPx was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padRightPx(ByVal padRightPx_Conflict As Integer) As Builder
				Me.padRightPx_Conflict = padRightPx_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter padTopPx was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padTopPx(ByVal padTopPx_Conflict As Integer) As Builder
				Me.padTopPx_Conflict = padTopPx_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter padBottomPx was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padBottomPx(ByVal padBottomPx_Conflict As Integer) As Builder
				Me.padBottomPx_Conflict = padBottomPx_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter paddingPx was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function paddingPx(ByVal paddingPx_Conflict As Integer) As Builder
				padLeftPx(paddingPx_Conflict)
				padRightPx(paddingPx_Conflict)
				padTopPx(paddingPx_Conflict)
				padBottomPx(paddingPx_Conflict)
				Return Me
			End Function

			Public Overridable Function paddingPx(ByVal left As Integer, ByVal right As Integer, ByVal top As Integer, ByVal bottom As Integer) As Builder
				padLeftPx(left)
				padRightPx(right)
				padTopPx(top)
				padBottomPx(bottom)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter colWidthsPercent was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function colWidthsPercent(ParamArray ByVal colWidthsPercent_Conflict() As Double) As Builder
				Me.colWidthsPercent_Conflict = colWidthsPercent_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter backgroundColor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function backgroundColor(ByVal backgroundColor_Conflict As String) As Builder
				Me.backgroundColor_Conflict = backgroundColor_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter headerColor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function headerColor(ByVal headerColor_Conflict As String) As Builder
				Me.headerColor_Conflict = headerColor_Conflict
				Return Me
			End Function

			Public Overridable Function build() As RenderableComponentTable
				Return New RenderableComponentTable(Me)
			End Function

		End Class



	End Class

End Namespace