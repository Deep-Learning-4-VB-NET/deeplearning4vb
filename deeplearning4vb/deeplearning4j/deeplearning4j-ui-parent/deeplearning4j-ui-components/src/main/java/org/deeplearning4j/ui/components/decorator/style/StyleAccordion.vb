Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
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

Namespace org.deeplearning4j.ui.components.decorator.style


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) public class StyleAccordion extends org.deeplearning4j.ui.api.Style
	Public Class StyleAccordion
		Inherits Style

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
		End Sub


		Public Class Builder
			Inherits Style.Builder(Of Builder)


			Public Overridable Function build() As StyleAccordion
				Return New StyleAccordion(Me)
			End Function

		End Class

	End Class

End Namespace