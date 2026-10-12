Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LegacyJsonFormat = org.datavec.api.transform.serde.legacy.LegacyJsonFormat
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports PropertyAccessor = org.nd4j.shade.jackson.annotation.PropertyAccessor
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory
Imports JodaModule = org.nd4j.shade.jackson.datatype.joda.JodaModule

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

Namespace org.datavec.api.transform.serde

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JsonMappers
	Public Class JsonMappers

		Private Shared jsonMapper As ObjectMapper
		Private Shared yamlMapper As ObjectMapper
'JAVA TO VB CONVERTER NOTE: The field legacyMapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared legacyMapper_Conflict As ObjectMapper 'For 1.0.0-alpha and earlier TransformProcess etc

		Shared Sub New()
			jsonMapper = New ObjectMapper()
			yamlMapper = New ObjectMapper(New YAMLFactory())
			configureMapper(jsonMapper)
			configureMapper(yamlMapper)
		End Sub

		Public Shared ReadOnly Property LegacyMapper As ObjectMapper
			Get
				SyncLock GetType(JsonMappers)
					If legacyMapper_Conflict Is Nothing Then
						legacyMapper_Conflict = LegacyJsonFormat.legacyMapper()
						configureMapper(legacyMapper_Conflict)
					End If
					Return legacyMapper_Conflict
				End SyncLock
			End Get
		End Property

		''' <returns> The default/primary ObjectMapper for deserializing JSON network configurations in DL4J </returns>
		Public Shared ReadOnly Property Mapper As ObjectMapper
			Get
				Return jsonMapper
			End Get
		End Property

		''' <returns> The default/primary ObjectMapper for deserializing network configurations in DL4J (YAML format) </returns>
		Public Shared ReadOnly Property MapperYaml As ObjectMapper
			Get
				Return yamlMapper
			End Get
		End Property

		Private Shared Sub configureMapper(ByVal ret As ObjectMapper)
			ret.registerModule(New JodaModule())
			ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			ret.enable(SerializationFeature.INDENT_OUTPUT)
			ret.setVisibility(PropertyAccessor.ALL, JsonAutoDetect.Visibility.NONE)
			ret.setVisibility(PropertyAccessor.FIELD, JsonAutoDetect.Visibility.ANY)
			ret.setVisibility(PropertyAccessor.CREATOR, JsonAutoDetect.Visibility.ANY) 'Need this otherwise JsonProperty annotations on constructors won't be seen
		End Sub

	End Class

End Namespace