Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Component = org.deeplearning4j.ui.api.Component
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) public abstract class Chart extends org.deeplearning4j.ui.api.Component
	Public MustInherit Class Chart
		Inherits Component

		Private title As String
		Private suppressAxisHorizontal As Boolean?
		Private suppressAxisVertical As Boolean?
		Private showLegend As Boolean

		Private setXMin As Double?
		Private setXMax As Double?
		Private setYMin As Double?
		Private setYMax As Double?

		Private gridVerticalStrokeWidth As Double?
		Private gridHorizontalStrokeWidth As Double?

		Public Sub New(ByVal componentType As String)
			MyBase.New(componentType, Nothing)
		End Sub

		Public Sub New(ByVal componentType As String, ByVal builder As Builder)
			MyBase.New(componentType, builder.getStyle())
			Me.title = builder.title
			Me.suppressAxisHorizontal = builder.suppressAxisHorizontal
			Me.suppressAxisVertical = builder.suppressAxisVertical
			Me.showLegend = builder.showLegend

			Me.setXMin = builder.setXMin
			Me.setXMax = builder.setXMax
			Me.setYMin = builder.setYMin
			Me.setYMax = builder.setYMax

			Me.gridVerticalStrokeWidth = builder.gridVerticalStrokeWidth
			Me.gridHorizontalStrokeWidth = builder.gridHorizontalStrokeWidth
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @SuppressWarnings("unchecked") public static abstract class Builder<T extends Builder<T>>
		Public MustInherit Class Builder(Of T As Builder(Of T))

			Friend title As String
			Friend style As StyleChart
'JAVA TO VB CONVERTER NOTE: The field suppressAxisHorizontal was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend suppressAxisHorizontal_Conflict As Boolean?
'JAVA TO VB CONVERTER NOTE: The field suppressAxisVertical was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend suppressAxisVertical_Conflict As Boolean?
'JAVA TO VB CONVERTER NOTE: The field showLegend was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend showLegend_Conflict As Boolean

			Friend setXMin As Double?
			Friend setXMax As Double?
			Friend setYMin As Double?
			Friend setYMax As Double?

			Friend gridVerticalStrokeWidth As Double?
			Friend gridHorizontalStrokeWidth As Double?

			''' <param name="title"> Title for the chart (may be null) </param>
			''' <param name="style"> Style for the chart (may be null) </param>
			Public Sub New(ByVal title As String, ByVal style As StyleChart)
				Me.title = title
				Me.style = style
			End Sub

			''' <param name="suppressAxisHorizontal"> if true: don't show the horizontal axis (default: show) </param>
'JAVA TO VB CONVERTER NOTE: The parameter suppressAxisHorizontal was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function suppressAxisHorizontal(ByVal suppressAxisHorizontal_Conflict As Boolean) As T
				Me.suppressAxisHorizontal_Conflict = suppressAxisHorizontal_Conflict
				Return CType(Me, T)
			End Function

			''' <param name="suppressAxisVertical"> if true: don't show the vertical axis (default: show) </param>
'JAVA TO VB CONVERTER NOTE: The parameter suppressAxisVertical was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function suppressAxisVertical(ByVal suppressAxisVertical_Conflict As Boolean) As T
				Me.suppressAxisVertical_Conflict = suppressAxisVertical_Conflict
				Return CType(Me, T)
			End Function

			''' <param name="showLegend"> if true: show the legend. (default: no legend) </param>
'JAVA TO VB CONVERTER NOTE: The parameter showLegend was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function showLegend(ByVal showLegend_Conflict As Boolean) As T
				Me.showLegend_Conflict = showLegend_Conflict
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Used to override/set the minimum value for the x axis. If this is not set, x axis minimum is set based on the data </summary>
			''' <param name="xMin"> Minimum value to use for the x axis </param>
			Public Overridable Function setXMin(ByVal xMin As Double?) As T
				Me.setXMin = xMin
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Used to override/set the maximum value for the x axis. If this is not set, x axis maximum is set based on the data </summary>
			''' <param name="xMax"> Maximum value to use for the x axis </param>
			Public Overridable Function setXMax(ByVal xMax As Double?) As T
				Me.setXMax = xMax
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Used to override/set the minimum value for the y axis. If this is not set, y axis minimum is set based on the data </summary>
			''' <param name="yMin"> Minimum value to use for the y axis </param>
			Public Overridable Function setYMin(ByVal yMin As Double?) As T
				Me.setYMin = yMin
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Used to override/set the maximum value for the y axis. If this is not set, y axis minimum is set based on the data </summary>
			''' <param name="yMax"> Minimum value to use for the y axis </param>
			Public Overridable Function setYMax(ByVal yMax As Double?) As T
				Me.setYMax = yMax
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the grid lines to be enabled, and if enabled: set the grid. </summary>
			''' <param name="gridVerticalStrokeWidth">      If null (or 0): show no vertical grid. Otherwise: width in px </param>
			''' <param name="gridHorizontalStrokeWidth">    If null (or 0): show no horizontal grid. Otherwise: width in px </param>
			Public Overridable Function setGridWidth(ByVal gridVerticalStrokeWidth As Double?, ByVal gridHorizontalStrokeWidth As Double?) As T
				Me.gridVerticalStrokeWidth = gridVerticalStrokeWidth
				Me.gridHorizontalStrokeWidth = gridHorizontalStrokeWidth
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the grid lines to be enabled, and if enabled: set the grid. </summary>
			''' <param name="gridVerticalStrokeWidth">      If null (or 0): show no vertical grid. Otherwise: width in px </param>
			''' <param name="gridHorizontalStrokeWidth">    If null (or 0): show no horizontal grid. Otherwise: width in px </param>
			Public Overridable Function setGridWidth(ByVal gridVerticalStrokeWidth As Integer?, ByVal gridHorizontalStrokeWidth As Integer?) As T
				Return setGridWidth((If(gridVerticalStrokeWidth IsNot Nothing, gridVerticalStrokeWidth.Value, Nothing)), (If(gridHorizontalStrokeWidth IsNot Nothing, gridHorizontalStrokeWidth.Value, Nothing)))
			End Function

		End Class

	End Class

End Namespace