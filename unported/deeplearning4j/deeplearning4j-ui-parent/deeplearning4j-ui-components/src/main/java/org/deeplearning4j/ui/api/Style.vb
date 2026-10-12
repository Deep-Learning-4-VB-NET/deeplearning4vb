Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports StyleChart = org.deeplearning4j.ui.components.chart.style.StyleChart
Imports StyleDiv = org.deeplearning4j.ui.components.component.style.StyleDiv
Imports StyleAccordion = org.deeplearning4j.ui.components.decorator.style.StyleAccordion
Imports StyleTable = org.deeplearning4j.ui.components.table.style.StyleTable
Imports StyleText = org.deeplearning4j.ui.components.text.style.StyleText
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.ui.api


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = StyleChart.class, name = "StyleChart"), @JsonSubTypes.Type(value = StyleTable.class, name = "StyleTable"), @JsonSubTypes.Type(value = StyleText.class, name = "StyleText"), @JsonSubTypes.Type(value = StyleAccordion.class, name = "StyleAccordion"), @JsonSubTypes.Type(value = StyleDiv.class, name = "StyleDiv")}) @Data @AllArgsConstructor @NoArgsConstructor public abstract class Style
	Public MustInherit Class Style

		Private width As Double?
		Private height As Double?
		Private widthUnit As LengthUnit
		Private heightUnit As LengthUnit

		Protected Friend marginUnit As LengthUnit
		Protected Friend marginTop As Double?
		Protected Friend marginBottom As Double?
		Protected Friend marginLeft As Double?
		Protected Friend marginRight As Double?

		Protected Friend backgroundColor As String

		Public Sub New(ByVal b As Builder)
			Me.width = b.width
			Me.height = b.height
			Me.widthUnit = b.widthUnit
			Me.heightUnit = b.heightUnit

			Me.marginUnit = b.marginUnit
			Me.marginTop = b.marginTop
			Me.marginBottom = b.marginBottom
			Me.marginLeft = b.marginLeft
			Me.marginRight = b.marginRight

			Me.backgroundColor = b.backgroundColor
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static abstract class Builder<T extends Builder<T>>
		Public MustInherit Class Builder(Of T As Builder(Of T))
'JAVA TO VB CONVERTER NOTE: The field width was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend width_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field height was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend height_Conflict As Double?
			Protected Friend widthUnit As LengthUnit
			Protected Friend heightUnit As LengthUnit

			Protected Friend marginUnit As LengthUnit
			Protected Friend marginTop As Double?
			Protected Friend marginBottom As Double?
			Protected Friend marginLeft As Double?
			Protected Friend marginRight As Double?

'JAVA TO VB CONVERTER NOTE: The field backgroundColor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend backgroundColor_Conflict As String

'JAVA TO VB CONVERTER NOTE: The parameter width was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function width(ByVal width_Conflict As Double, ByVal widthUnit As LengthUnit) As T
				Me.width_Conflict = width_Conflict
				Me.widthUnit = widthUnit
				Return CType(Me, T)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter height was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function height(ByVal height_Conflict As Double, ByVal heightUnit As LengthUnit) As T
				Me.height_Conflict = height_Conflict
				Me.heightUnit = heightUnit
				Return CType(Me, T)
			End Function

			Public Overridable Function margin(ByVal unit As LengthUnit, ByVal marginTop As Integer?, ByVal marginBottom As Integer?, ByVal marginLeft As Integer?, ByVal marginRight As Integer?) As T
				Return margin(unit, (If(marginTop IsNot Nothing, marginTop.Value, Nothing)), (If(marginBottom IsNot Nothing, marginBottom.Value, Nothing)), (If(marginLeft IsNot Nothing, marginLeft.Value, Nothing)), (If(marginRight IsNot Nothing, marginRight.Value, Nothing)))
			End Function

			Public Overridable Function margin(ByVal unit As LengthUnit, ByVal marginTop As Double?, ByVal marginBottom As Double?, ByVal marginLeft As Double?, ByVal marginRight As Double?) As T
				Me.marginUnit = unit
				Me.marginTop = marginTop
				Me.marginBottom = marginBottom
				Me.marginLeft = marginLeft
				Me.marginRight = marginRight
				Return CType(Me, T)
			End Function

			Public Overridable Function backgroundColor(ByVal color As Color) As T
				Return backgroundColor(Utils.colorToHex(color))
			End Function

			Public Overridable Function backgroundColor(ByVal color As String) As T
				Me.backgroundColor_Conflict = color
				Return CType(Me, T)
			End Function
		End Class
	End Class

End Namespace