Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Component = org.deeplearning4j.ui.api.Component
Imports Style = org.deeplearning4j.ui.api.Style
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

Namespace org.deeplearning4j.ui.components.component


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) public class ComponentDiv extends org.deeplearning4j.ui.api.Component
	Public Class ComponentDiv
		Inherits Component

		Public Const COMPONENT_TYPE As String = "ComponentDiv"

		Private components() As Component

		Public Sub New()
			MyBase.New(COMPONENT_TYPE, Nothing)
		End Sub


		Public Sub New(ByVal style As Style, ParamArray ByVal components() As Component)
			MyBase.New(COMPONENT_TYPE, style)
			Me.components = components
		End Sub

		Public Sub New(ByVal style As Style, ByVal componentCollection As ICollection(Of Component))
			Me.New(style, (If(componentCollection Is Nothing, Nothing, componentCollection.ToArray())))
		End Sub
	End Class

End Namespace