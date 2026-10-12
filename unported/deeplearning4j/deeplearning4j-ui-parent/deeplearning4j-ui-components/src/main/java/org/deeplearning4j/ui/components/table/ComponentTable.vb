Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Component = org.deeplearning4j.ui.api.Component
Imports StyleTable = org.deeplearning4j.ui.components.table.style.StyleTable
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

Namespace org.deeplearning4j.ui.components.table


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class ComponentTable extends org.deeplearning4j.ui.api.Component
	Public Class ComponentTable
		Inherits Component

		Public Const COMPONENT_TYPE As String = "ComponentTable"

		Private title As String
		Private header() As String
		Private content()() As String

		Public Sub New()
			MyBase.New(COMPONENT_TYPE, Nothing)
			'No arg constructor for Jackson
		End Sub

		Public Sub New(ByVal builder As Builder)
			MyBase.New(COMPONENT_TYPE, builder.style)
			Me.header = builder.header_Conflict
			Me.content = builder.content_Conflict
		End Sub

		Public Sub New(ByVal header() As String, ByVal table()() As String, ByVal style As StyleTable)
			MyBase.New(COMPONENT_TYPE, style)
			Me.header = header
			Me.content = table
		End Sub

		Public Class Builder

			Friend style As StyleTable
'JAVA TO VB CONVERTER NOTE: The field header was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend header_Conflict() As String
'JAVA TO VB CONVERTER NOTE: The field content was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend content_Conflict()() As String

			Public Sub New(ByVal style As StyleTable)
				Me.style = style
			End Sub

			''' <param name="header"> Header values for the table </param>
'JAVA TO VB CONVERTER NOTE: The parameter header was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function header(ParamArray ByVal header_Conflict() As String) As Builder
				Me.header_Conflict = header_Conflict
				Return Me
			End Function

			''' <summary>
			''' Content for the table, as 2d String[]
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter content was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function content(ByVal content_Conflict()() As String) As Builder
				Me.content_Conflict = content_Conflict
				Return Me
			End Function

			Public Overridable Function build() As ComponentTable
				Return New ComponentTable(Me)
			End Function

		End Class


	End Class

End Namespace