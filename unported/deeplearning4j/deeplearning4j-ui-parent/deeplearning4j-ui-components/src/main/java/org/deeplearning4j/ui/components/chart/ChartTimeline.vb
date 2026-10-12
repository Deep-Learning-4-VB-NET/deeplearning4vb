Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Utils = org.deeplearning4j.ui.api.Utils
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) public class ChartTimeline extends Chart
	Public Class ChartTimeline
		Inherits Chart

		Public Const COMPONENT_TYPE As String = "ChartTimeline"

		Private laneNames As IList(Of String) = New List(Of String)()
		Private laneData As IList(Of IList(Of TimelineEntry)) = New List(Of IList(Of TimelineEntry))()

		Public Sub New()
			MyBase.New(COMPONENT_TYPE)
			'no-arg constructor for Jackson
		End Sub

		Private Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE, builder)
			Me.laneNames = builder.laneNames
			Me.laneData = builder.laneData
		End Sub



		Public Class Builder
			Inherits Chart.Builder(Of Builder)

			Friend laneNames As IList(Of String) = New List(Of String)()
			Friend laneData As IList(Of IList(Of TimelineEntry)) = New List(Of IList(Of TimelineEntry))()


			Public Sub New(ByVal title As String, ByVal style As StyleChart)
				MyBase.New(title, style)
			End Sub


			Public Overridable Function addLane(ByVal name As String, ByVal data As IList(Of TimelineEntry)) As Builder
				laneNames.Add(name)
				laneData.Add(data)
				Return Me
			End Function

			Public Overridable Function build() As ChartTimeline
				Return New ChartTimeline(Me)
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor @JsonInclude(JsonInclude.Include.NON_NULL) public static class TimelineEntry
		Public Class TimelineEntry
			Friend entryLabel As String
			Friend startTimeMs As Long
			Friend endTimeMs As Long
			Friend color As String

			Public Sub New(ByVal entryLabel As String, ByVal startTimeMs As Long, ByVal endTimeMs As Long)
				Me.entryLabel = entryLabel
				Me.startTimeMs = startTimeMs
				Me.endTimeMs = endTimeMs
			End Sub

			Public Sub New(ByVal entryLabel As String, ByVal startTimeMs As Long, ByVal endTimeMs As Long, ByVal color As Color)
				Me.entryLabel = entryLabel
				Me.startTimeMs = startTimeMs
				Me.endTimeMs = endTimeMs
				Me.color = Utils.colorToHex(color)
			End Sub

		End Class
	End Class

End Namespace