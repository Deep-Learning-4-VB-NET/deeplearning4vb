Imports System.Collections.Generic
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class RenderableComponentLineChart extends RenderableComponent
	Public Class RenderableComponentLineChart
		Inherits RenderableComponent

		Public Const COMPONENT_TYPE As String = "linechart"

		Private title As String
		Private x As IList(Of Double())
		Private y As IList(Of Double())
		Private seriesNames As IList(Of String)
		Private removeAxisHorizontal As Boolean
		Private marginTop As Integer
		Private marginBottom As Integer
		Private marginLeft As Integer
		Private marginRight As Integer
		Private legend As Boolean

		Private Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE)
			title = builder.title_Conflict
			x = builder.x
			y = builder.y
			seriesNames = builder.seriesNames
			Me.removeAxisHorizontal = builder.removeAxisHorizontal_Conflict
			Me.marginTop = builder.marginTop
			Me.marginBottom = builder.marginBottom
			Me.marginLeft = builder.marginLeft
			Me.marginRight = builder.marginRight
		End Sub

		Public Sub New()
			MyBase.New(COMPONENT_TYPE)
			'no-arg constructor for Jackson
		End Sub



		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field title was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend title_Conflict As String
			Friend x As IList(Of Double()) = New List(Of Double())()
			Friend y As IList(Of Double()) = New List(Of Double())()
			Friend seriesNames As IList(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field removeAxisHorizontal was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend removeAxisHorizontal_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field legend was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend legend_Conflict As Boolean = True

			Friend marginTop As Integer = 60
			Friend marginBottom As Integer = 60
			Friend marginLeft As Integer = 60
			Friend marginRight As Integer = 20

'JAVA TO VB CONVERTER NOTE: The parameter title was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function title(ByVal title_Conflict As String) As Builder
				Me.title_Conflict = title_Conflict
				Return Me
			End Function

			Public Overridable Function addSeries(ByVal seriesName As String, ByVal xValues() As Double, ByVal yValues() As Double) As Builder
				x.Add(xValues)
				y.Add(yValues)
				seriesNames.Add(seriesName)
				Return Me
			End Function

			Public Overridable Function setRemoveAxisHorizontal(ByVal removeAxisHorizontal As Boolean) As Builder
				Me.removeAxisHorizontal_Conflict = removeAxisHorizontal
				Return Me
			End Function

			Public Overridable Function margins(ByVal top As Integer, ByVal bottom As Integer, ByVal left As Integer, ByVal right As Integer) As Builder
				Me.marginTop = top
				Me.marginBottom = bottom
				Me.marginLeft = left
				Me.marginRight = right
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter legend was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function legend(ByVal legend_Conflict As Boolean) As Builder
				Me.legend_Conflict = legend_Conflict
				Return Me
			End Function

			Public Overridable Function build() As RenderableComponentLineChart
				Return New RenderableComponentLineChart(Me)
			End Function

		End Class

	End Class

End Namespace