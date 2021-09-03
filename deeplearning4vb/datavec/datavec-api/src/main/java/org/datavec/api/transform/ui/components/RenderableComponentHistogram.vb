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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class RenderableComponentHistogram extends RenderableComponent
	Public Class RenderableComponentHistogram
		Inherits RenderableComponent

		Public Const COMPONENT_TYPE As String = "histogram"

		Private title As String
		Private lowerBounds As IList(Of Double) = New List(Of Double)()
		Private upperBounds As IList(Of Double) = New List(Of Double)()
		Private yValues As IList(Of Double) = New List(Of Double)()
		Private marginTop As Integer
		Private marginBottom As Integer
		Private marginLeft As Integer
		Private marginRight As Integer

		Public Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE)
			Me.title = builder.title_Conflict
			Me.lowerBounds = builder.lowerBounds
			Me.upperBounds = builder.upperBounds
			Me.yValues = builder.yValues
			Me.marginTop = builder.marginTop
			Me.marginBottom = builder.marginBottom
			Me.marginLeft = builder.marginLeft
			Me.marginRight = builder.marginRight
		End Sub


		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field title was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend title_Conflict As String
			Friend lowerBounds As IList(Of Double) = New List(Of Double)()
			Friend upperBounds As IList(Of Double) = New List(Of Double)()
			Friend yValues As IList(Of Double) = New List(Of Double)()
			Friend marginTop As Integer = 60
			Friend marginBottom As Integer = 60
			Friend marginLeft As Integer = 60
			Friend marginRight As Integer = 20

'JAVA TO VB CONVERTER NOTE: The parameter title was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function title(ByVal title_Conflict As String) As Builder
				Me.title_Conflict = title_Conflict
				Return Me
			End Function

			Public Overridable Function addBin(ByVal lower As Double, ByVal upper As Double, ByVal yValue As Double) As Builder
				lowerBounds.Add(lower)
				upperBounds.Add(upper)
				yValues.Add(yValue)
				Return Me
			End Function

			Public Overridable Function margins(ByVal top As Integer, ByVal bottom As Integer, ByVal left As Integer, ByVal right As Integer) As Builder
				Me.marginTop = top
				Me.marginBottom = bottom
				Me.marginLeft = left
				Me.marginRight = right
				Return Me
			End Function

			Public Overridable Function build() As RenderableComponentHistogram
				Return New RenderableComponentHistogram(Me)
			End Function
		End Class
	End Class

End Namespace