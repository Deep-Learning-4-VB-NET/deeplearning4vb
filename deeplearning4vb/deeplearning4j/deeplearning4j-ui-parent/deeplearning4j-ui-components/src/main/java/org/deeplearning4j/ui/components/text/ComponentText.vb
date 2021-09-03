Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Component = org.deeplearning4j.ui.api.Component
Imports StyleText = org.deeplearning4j.ui.components.text.style.StyleText
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

Namespace org.deeplearning4j.ui.components.text

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @JsonInclude(JsonInclude.Include.NON_NULL) public class ComponentText extends org.deeplearning4j.ui.api.Component
	Public Class ComponentText
		Inherits Component

		Public Const COMPONENT_TYPE As String = "ComponentText"
		Private text As String

		Public Sub New()
			MyBase.New(COMPONENT_TYPE, Nothing)
			'No arg constructor for Jackson deserialization
			text = Nothing
		End Sub

		Public Sub New(ByVal text As String, ByVal style As StyleText)
			MyBase.New(COMPONENT_TYPE, style)
			Me.text = text
		End Sub

		Private Sub New(ByVal builder As Builder)
			Me.New(builder.text, builder.style)
		End Sub


		Public Overrides Function ToString() As String
			Return "ComponentText(" & text & ")"
		End Function

		Public Class Builder

			Friend style As StyleText
			Friend text As String

			Public Sub New(ByVal text As String, ByVal style As StyleText)
				Me.text = text
				Me.style = style
			End Sub

			Public Overridable Function build() As ComponentText
				Return New ComponentText(Me)
			End Function
		End Class

	End Class

End Namespace