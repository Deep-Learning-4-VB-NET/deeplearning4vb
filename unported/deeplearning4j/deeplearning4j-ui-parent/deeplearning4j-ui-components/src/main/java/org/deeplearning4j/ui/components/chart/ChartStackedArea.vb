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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class ChartStackedArea extends Chart
	Public Class ChartStackedArea
		Inherits Chart

		Public Const COMPONENT_TYPE As String = "ChartStackedArea"

		Private x(-1) As Double
		Private y As IList(Of Double()) = New List(Of Double())()
		Private labels As IList(Of String) = New List(Of String)()

		Public Sub New()
			MyBase.New(COMPONENT_TYPE)
		End Sub

		Public Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE, builder)
			Me.x = builder.x
			Me.y = builder.y
			Me.labels = builder.seriesNames
		End Sub

		Public Class Builder
			Inherits Chart.Builder(Of Builder)

			Friend x() As Double
			Friend y As IList(Of Double()) = New List(Of Double())()
			Friend seriesNames As IList(Of String) = New List(Of String)()


			Public Sub New(ByVal title As String, ByVal style As StyleChart)
				MyBase.New(title, style)
			End Sub

			''' <summary>
			''' Set the x-axis values
			''' </summary>
			Public Overridable Function setXValues(ByVal x() As Double) As Builder
				Me.x = x
				Return Me
			End Function

			''' <summary>
			''' Add a single series.
			''' </summary>
			''' <param name="seriesName"> Name of the series </param>
			''' <param name="yValues">    length of the yValues array must be same as the x-values array </param>
			Public Overridable Function addSeries(ByVal seriesName As String, ByVal yValues() As Double) As Builder
				y.Add(yValues)
				seriesNames.Add(seriesName)
				Return Me
			End Function

			Public Overridable Function build() As ChartStackedArea
				Return New ChartStackedArea(Me)
			End Function
		End Class

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("ChartStackedArea(x=")
			If x IsNot Nothing Then
				sb.Append(Arrays.toString(x))
			Else
				sb.Append("[]")
			End If
			sb.Append(",y=[")
			Dim first As Boolean = True
			If y IsNot Nothing Then
				For Each d As Double() In y
					If Not first Then
						sb.Append(",")
					End If
					sb.Append(Arrays.toString(d))
					first = False
				Next d
			End If
			sb.Append("],labels=")
			If labels IsNot Nothing Then
				sb.Append(labels)
			End If
			sb.Append(")")
			Return sb.ToString()
		End Function
	End Class

End Namespace