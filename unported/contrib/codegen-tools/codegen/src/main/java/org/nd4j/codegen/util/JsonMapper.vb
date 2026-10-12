Imports JsonAutoDetect = com.fasterxml.jackson.annotation.JsonAutoDetect
Imports JsonInclude = com.fasterxml.jackson.annotation.JsonInclude
Imports PropertyAccessor = com.fasterxml.jackson.annotation.PropertyAccessor
Imports ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper
Imports SerializationFeature = com.fasterxml.jackson.databind.SerializationFeature

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.codegen.util


	Public Class JsonMapper

		Public Shared ReadOnly Property Mapper As ObjectMapper
			Get
				Dim om As New ObjectMapper()
		'        om.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
				om.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
				om.enable(SerializationFeature.INDENT_OUTPUT)
				om.setSerializationInclusion(JsonInclude.Include.NON_NULL)
				om.setVisibility(PropertyAccessor.ALL, JsonAutoDetect.Visibility.NONE)
				om.setVisibility(PropertyAccessor.FIELD, JsonAutoDetect.Visibility.ANY)
				om.setVisibility(PropertyAccessor.CREATOR, JsonAutoDetect.Visibility.ANY)
				om.setVisibility(PropertyAccessor.SETTER, JsonAutoDetect.Visibility.ANY)
    
				Return om
			End Get
		End Property

	End Class

End Namespace