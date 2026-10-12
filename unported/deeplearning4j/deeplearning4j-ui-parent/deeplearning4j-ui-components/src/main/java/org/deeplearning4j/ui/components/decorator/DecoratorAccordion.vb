Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Component = org.deeplearning4j.ui.api.Component
Imports StyleAccordion = org.deeplearning4j.ui.components.decorator.style.StyleAccordion
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

Namespace org.deeplearning4j.ui.components.decorator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class DecoratorAccordion extends org.deeplearning4j.ui.api.Component
	Public Class DecoratorAccordion
		Inherits Component

		Public Const COMPONENT_TYPE As String = "DecoratorAccordion"

		Private title As String
		Private defaultCollapsed As Boolean
		Private innerComponents() As Component

		Public Sub New()
			MyBase.New(COMPONENT_TYPE, Nothing)
			'No arg constructor for Jackson
		End Sub

		Private Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE, builder.style)
			Me.title = builder.title_Conflict
			Me.defaultCollapsed = builder.defaultCollapsed_Conflict
			Me.innerComponents = CType(builder.innerComponents, List(Of Component)).ToArray()
		End Sub

		Public Class Builder

			Friend style As StyleAccordion
'JAVA TO VB CONVERTER NOTE: The field title was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend title_Conflict As String
			Friend innerComponents As IList(Of Component) = New List(Of Component)()
'JAVA TO VB CONVERTER NOTE: The field defaultCollapsed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend defaultCollapsed_Conflict As Boolean

			Public Sub New(ByVal style As StyleAccordion)
				Me.New(Nothing, style)
			End Sub

			Public Sub New(ByVal title As String, ByVal style As StyleAccordion)
				Me.title_Conflict = title
				Me.style = style
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter title was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function title(ByVal title_Conflict As String) As Builder
				Me.title_Conflict = title_Conflict
				Return Me
			End Function

			''' <summary>
			''' Components to show internally in the accordion element
			''' </summary>
			Public Overridable Function addComponents(ParamArray ByVal innerComponents() As Component) As Builder
				Collections.addAll(Me.innerComponents, innerComponents)
				Return Me
			End Function

			''' <summary>
			''' Set the default collapsed/expanded state
			''' </summary>
			''' <param name="defaultCollapsed"> If true: default to collapsed </param>
			Public Overridable Function setDefaultCollapsed(ByVal defaultCollapsed As Boolean) As Builder
				Me.defaultCollapsed_Conflict = defaultCollapsed
				Return Me
			End Function

			Public Overridable Function build() As DecoratorAccordion
				Return New DecoratorAccordion(Me)
			End Function
		End Class

	End Class

End Namespace