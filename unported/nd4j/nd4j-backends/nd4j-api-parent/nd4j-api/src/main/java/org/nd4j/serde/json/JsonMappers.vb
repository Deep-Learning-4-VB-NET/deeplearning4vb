Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports AtomicDouble = org.nd4j.common.primitives.AtomicDouble
Imports JsonDeserializerAtomicBoolean = org.nd4j.common.primitives.serde.JsonDeserializerAtomicBoolean
Imports JsonDeserializerAtomicDouble = org.nd4j.common.primitives.serde.JsonDeserializerAtomicDouble
Imports JsonSerializerAtomicBoolean = org.nd4j.common.primitives.serde.JsonSerializerAtomicBoolean
Imports JsonSerializerAtomicDouble = org.nd4j.common.primitives.serde.JsonSerializerAtomicDouble
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
Imports SimpleModule = org.nd4j.shade.jackson.databind.module.SimpleModule
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory

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

Namespace org.nd4j.serde.json

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JsonMappers
	Public Class JsonMappers

		Private Shared jsonMapper As ObjectMapper = configureMapper(New ObjectMapper())
'JAVA TO VB CONVERTER NOTE: The field yamlMapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared yamlMapper_Conflict As ObjectMapper = configureMapper(New ObjectMapper(New YAMLFactory()))

		''' <returns> The default/primary ObjectMapper for deserializing JSON objects </returns>
		Public Shared ReadOnly Property Mapper As ObjectMapper
			Get
				Return jsonMapper
			End Get
		End Property

		''' <returns> The default/primary ObjectMapper for deserializing JSON objects </returns>
		Public Shared ReadOnly Property YamlMapper As ObjectMapper
			Get
				Return jsonMapper
			End Get
		End Property

		Private Shared Function configureMapper(ByVal ret As ObjectMapper) As ObjectMapper
			ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, False)
			ret.enable(SerializationFeature.INDENT_OUTPUT)
			Dim atomicModule As New SimpleModule()
			atomicModule.addSerializer(GetType(AtomicDouble), New JsonSerializerAtomicDouble())
			atomicModule.addSerializer(GetType(AtomicBoolean), New JsonSerializerAtomicBoolean())
			atomicModule.addDeserializer(GetType(AtomicDouble), New JsonDeserializerAtomicDouble())
			atomicModule.addDeserializer(GetType(AtomicBoolean), New JsonDeserializerAtomicBoolean())
			ret.registerModule(atomicModule)
			'Serialize fields only, not using getters
			ret.setVisibilityChecker(ret.getSerializationConfig().getDefaultVisibilityChecker().withFieldVisibility(JsonAutoDetect.Visibility.ANY).withGetterVisibility(JsonAutoDetect.Visibility.NONE).withSetterVisibility(JsonAutoDetect.Visibility.NONE).withCreatorVisibility(JsonAutoDetect.Visibility.ANY))
			Return ret
		End Function
	End Class

End Namespace