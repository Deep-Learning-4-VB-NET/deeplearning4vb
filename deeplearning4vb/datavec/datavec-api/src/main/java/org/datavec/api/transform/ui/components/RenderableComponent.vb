Imports Data = lombok.Data
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

Namespace org.datavec.api.transform.ui.components

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.@As.WRAPPER_OBJECT) @JsonSubTypes(value = { @JsonSubTypes.Type(value = RenderableComponentLineChart.class, name = "RenderableComponentLineChart"), @JsonSubTypes.Type(value = RenderableComponentTable.class, name = "RenderableComponentTable"), @JsonSubTypes.Type(value = RenderableComponentHistogram.class, name = "RenderableComponentHistogram")}) @Data public abstract class RenderableComponent
	Public MustInherit Class RenderableComponent

		''' <summary>
		''' Component type: used by the Arbiter UI to determine how to decode and render the object which is
		''' represented by the JSON representation of this object
		''' </summary>
		Protected Friend ReadOnly componentType As String

		Public Sub New(ByVal componentType As String)
			Me.componentType = componentType
		End Sub

	End Class

End Namespace