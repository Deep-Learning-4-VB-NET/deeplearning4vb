Imports Data = lombok.Data
Imports org.deeplearning4j.ui.components.chart
Imports ComponentDiv = org.deeplearning4j.ui.components.component.ComponentDiv
Imports DecoratorAccordion = org.deeplearning4j.ui.components.decorator.DecoratorAccordion
Imports ComponentTable = org.deeplearning4j.ui.components.table.ComponentTable
Imports ComponentText = org.deeplearning4j.ui.components.text.ComponentText
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
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = {@JsonSubTypes.Type(value = ChartHistogram.class, name = "ChartHistogram"), @JsonSubTypes.Type(value = ChartHorizontalBar.class, name = "ChartHorizontalBar"), @JsonSubTypes.Type(value = ChartLine.class, name = "ChartLine"), @JsonSubTypes.Type(value = ChartScatter.class, name = "ChartScatter"), @JsonSubTypes.Type(value = ChartStackedArea.class, name = "ChartStackedArea"), @JsonSubTypes.Type(value = ChartTimeline.class, name = "ChartTimeline"), @JsonSubTypes.Type(value = ComponentDiv.class, name = "ComponentDiv"), @JsonSubTypes.Type(value = DecoratorAccordion.class, name = "DecoratorAccordion"), @JsonSubTypes.Type(value = ComponentTable.class, name = "ComponentTable"), @JsonSubTypes.Type(value = ComponentText.class, name = "ComponentText")}) @Data public abstract class Component
	Public MustInherit Class Component

		''' <summary>
		''' Component type: used by the Arbiter UI to determine how to decode and render the object which is
		''' represented by the JSON representation of this object
		''' </summary>
		Protected Friend ReadOnly componentType As String
		Protected Friend ReadOnly style As Style

		Public Sub New(ByVal componentType As String, ByVal style As Style)
			Me.componentType = componentType
			Me.style = style
		End Sub

	End Class

End Namespace