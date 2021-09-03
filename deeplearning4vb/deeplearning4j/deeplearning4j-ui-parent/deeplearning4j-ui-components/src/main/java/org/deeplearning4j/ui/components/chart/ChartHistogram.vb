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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class ChartHistogram extends Chart
	Public Class ChartHistogram
		Inherits Chart

		Public Const COMPONENT_TYPE As String = "ChartHistogram"

		Private lowerBounds As IList(Of Double) = New List(Of Double)()
		Private upperBounds As IList(Of Double) = New List(Of Double)()
		Private yValues As IList(Of Double) = New List(Of Double)()

		Public Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE, builder)
			Me.lowerBounds = builder.lowerBounds
			Me.upperBounds = builder.upperBounds
			Me.yValues = builder.yValues
		End Sub

		'No arg constructor for Jackson
		Public Sub New()
			MyBase.New(COMPONENT_TYPE)
		End Sub


		Public Class Builder
			Inherits Chart.Builder(Of Builder)

			Friend lowerBounds As IList(Of Double) = New List(Of Double)()
			Friend upperBounds As IList(Of Double) = New List(Of Double)()
			Friend yValues As IList(Of Double) = New List(Of Double)()

			Public Sub New(ByVal title As String, ByVal style As StyleChart)
				MyBase.New(title, style)
			End Sub

			''' <summary>
			''' Add a single bin
			''' </summary>
			''' <param name="lower">  Lower (minimum/left) value for the bin (x axis) </param>
			''' <param name="upper">  Upper (maximum/right) value for the bin (x axis) </param>
			''' <param name="yValue"> The height of the bin </param>
			Public Overridable Function addBin(ByVal lower As Double, ByVal upper As Double, ByVal yValue As Double) As Builder
				lowerBounds.Add(lower)
				upperBounds.Add(upper)
				yValues.Add(yValue)
				Return Me
			End Function

			Public Overridable Function build() As ChartHistogram
				Return New ChartHistogram(Me)
			End Function
		End Class

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("ChartHistogram(lowerBounds=")
			If lowerBounds IsNot Nothing Then
				sb.Append(lowerBounds)
			Else
				sb.Append("[]")
			End If
			sb.Append(",upperBounds=")
			If upperBounds IsNot Nothing Then
				sb.Append(upperBounds)
			Else
				sb.Append("[]")
			End If
			sb.Append(",yValues=")
			If yValues IsNot Nothing Then
				sb.Append(yValues)
			Else
				sb.Append("[]")
			End If

			sb.Append(")")
			Return sb.ToString()
		End Function
	End Class

End Namespace