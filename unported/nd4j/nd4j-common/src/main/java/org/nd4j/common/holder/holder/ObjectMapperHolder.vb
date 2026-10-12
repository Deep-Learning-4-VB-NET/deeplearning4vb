Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.nd4j.common.holder

	Public Class ObjectMapperHolder

		Private Shared objectMapper As ObjectMapper = Mapper

		Private Sub New()
		End Sub


		''' <summary>
		''' Get a single object mapper for use
		''' with reading and writing json
		''' @return
		''' </summary>
		Public Shared ReadOnly Property JsonMapper As ObjectMapper
			Get
				Return objectMapper
			End Get
		End Property

		Private Shared ReadOnly Property Mapper As ObjectMapper
			Get
				Dim om As New ObjectMapper()
				'Serialize fields only, not using getters
				'Not all getters are supported - for example, UserEntity
				om.setVisibilityChecker(om.getSerializationConfig().getDefaultVisibilityChecker().withFieldVisibility(JsonAutoDetect.Visibility.ANY).withGetterVisibility(JsonAutoDetect.Visibility.NONE).withSetterVisibility(JsonAutoDetect.Visibility.NONE).withCreatorVisibility(JsonAutoDetect.Visibility.NONE))
				om.setSerializationInclusion(JsonInclude.Include.NON_NULL)
				Return om
			End Get
		End Property



	End Class

End Namespace