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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class ChartScatter extends Chart
	Public Class ChartScatter
		Inherits Chart

		Public Const COMPONENT_TYPE As String = "ChartScatter"

		Private x As IList(Of Double())
		Private y As IList(Of Double())
		Private seriesNames As IList(Of String)

		Private Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE, builder)
			x = builder.x
			y = builder.y
			seriesNames = builder.seriesNames
		End Sub

		Public Sub New()
			MyBase.New(COMPONENT_TYPE)
			'no-arg constructor for Jackson
		End Sub



		Public Class Builder
			Inherits Chart.Builder(Of Builder)

			Friend x As IList(Of Double()) = New List(Of Double())()
			Friend y As IList(Of Double()) = New List(Of Double())()
			Friend seriesNames As IList(Of String) = New List(Of String)()

			Public Sub New(ByVal title As String, ByVal style As StyleChart)
				MyBase.New(title, style)
			End Sub

			''' 
			''' <param name="seriesName">    Name of the series </param>
			''' <param name="xValues">       Array of x values </param>
			''' <param name="yValues">       Array of y values (such that a single point i has coordinates (x[i],y[i]))
			''' @return </param>
			Public Overridable Function addSeries(ByVal seriesName As String, ByVal xValues() As Double, ByVal yValues() As Double) As Builder
				x.Add(xValues)
				y.Add(yValues)
				seriesNames.Add(seriesName)
				Return Me
			End Function

			Public Overridable Function build() As ChartScatter
				Return New ChartScatter(Me)
			End Function
		End Class

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("ChartScatter(x=[")
			Dim first As Boolean = True
			If x IsNot Nothing Then
				For Each d As Double() In x
					If Not first Then
						sb.Append(",")
					End If
					sb.Append(Arrays.toString(d))
					first = False
				Next d
			End If
			sb.Append("],y=[")
			first = True
			If y IsNot Nothing Then
				For Each d As Double() In y
					If Not first Then
						sb.Append(",")
					End If
					sb.Append(Arrays.toString(d))
					first = False
				Next d
			End If
			sb.Append("],seriesNames=")
			If seriesNames IsNot Nothing Then
				sb.Append(seriesNames)
			End If
			sb.Append(")")
			Return sb.ToString()
		End Function

	End Class

End Namespace