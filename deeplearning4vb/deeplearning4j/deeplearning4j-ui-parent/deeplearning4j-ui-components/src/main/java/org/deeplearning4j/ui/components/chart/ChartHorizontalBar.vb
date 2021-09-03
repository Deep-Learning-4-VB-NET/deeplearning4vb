Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports StyleChart = org.deeplearning4j.ui.components.chart.style.StyleChart
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

Namespace org.deeplearning4j.ui.components.chart


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class ChartHorizontalBar extends Chart
	Public Class ChartHorizontalBar
		Inherits Chart

		Public Const COMPONENT_TYPE As String = "ChartHorizontalBar"

		Private labels As IList(Of String) = New List(Of String)()
		Private values As IList(Of Double) = New List(Of Double)()
		Private xmin As Double?
		Private xmax As Double?

		Private Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE, builder)
			labels = builder.labels
			values = builder.values
			Me.xmin = builder.xMin_Conflict
			Me.xmax = builder.xMax_Conflict
		End Sub

		Public Sub New()
			MyBase.New(COMPONENT_TYPE, Nothing)
			'no-arg constructor for Jackson
		End Sub


		Public Class Builder
			Inherits Chart.Builder(Of Builder)

			Friend labels As IList(Of String) = New List(Of String)()
			Friend values As IList(Of Double) = New List(Of Double)()
'JAVA TO VB CONVERTER NOTE: The field xMin was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend xMin_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field xMax was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend xMax_Conflict As Double?

			Public Sub New(ByVal title As String, ByVal style As StyleChart)
				MyBase.New(title, style)
			End Sub

			Public Overridable Function addValue(ByVal name As String, ByVal value As Double) As Builder
				labels.Add(name)
				values.Add(value)
				Return Me
			End Function

			Public Overridable Function addValues(ByVal names As IList(Of String), ByVal values() As Double) As Builder
				For i As Integer = 0 To names.Count - 1
					addValue(names(i), values(i))
				Next i
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter xMin was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function xMin(ByVal xMin_Conflict As Double) As Builder
				Me.xMin_Conflict = xMin_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter xMax was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function xMax(ByVal xMax_Conflict As Double) As Builder
				Me.xMax_Conflict = xMax_Conflict
				Return Me
			End Function

			Public Overridable Function addValues(ByVal names As IList(Of String), ByVal values() As Single) As Builder
				For i As Integer = 0 To names.Count - 1
					addValue(names(i), values(i))
				Next i
				Return Me
			End Function

			Public Overridable Function build() As ChartHorizontalBar
				Return New ChartHorizontalBar(Me)
			End Function
		End Class

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("ChartHorizontalBar(labels=")
			If labels IsNot Nothing Then
				sb.Append(labels)
			Else
				sb.Append("[]")
			End If
			sb.Append(",values=")
			If values IsNot Nothing Then
				sb.Append(values)
			Else
				sb.Append("[]")
			End If
			If xmin IsNot Nothing Then
				sb.Append(",xMin=").Append(xmin)
			End If
			If xmax IsNot Nothing Then
				sb.Append(",xMax=").Append(xmax)
			End If

			sb.Append(")")
			Return sb.ToString()
		End Function

	End Class

End Namespace