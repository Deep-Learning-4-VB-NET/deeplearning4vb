Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports LegacyJsonFormat = org.deeplearning4j.nn.conf.serde.legacy.LegacyJsonFormat
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports org.nd4j.shade.jackson.databind
Imports MapperConfig = org.nd4j.shade.jackson.databind.cfg.MapperConfig
Imports BeanDeserializerModifier = org.nd4j.shade.jackson.databind.deser.BeanDeserializerModifier
Imports Annotated = org.nd4j.shade.jackson.databind.introspect.Annotated
Imports AnnotatedClass = org.nd4j.shade.jackson.databind.introspect.AnnotatedClass
Imports AnnotationMap = org.nd4j.shade.jackson.databind.introspect.AnnotationMap
Imports JacksonAnnotationIntrospector = org.nd4j.shade.jackson.databind.introspect.JacksonAnnotationIntrospector
Imports TypeResolverBuilder = org.nd4j.shade.jackson.databind.jsontype.TypeResolverBuilder
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

Namespace org.deeplearning4j.nn.conf.serde

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JsonMappers
	Public Class JsonMappers

		Private Shared jsonMapper As New ObjectMapper()
		Private Shared yamlMapper As New ObjectMapper(New YAMLFactory())

'JAVA TO VB CONVERTER NOTE: The field legacyMapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared legacyMapper_Conflict As ObjectMapper

		Shared Sub New()
			configureMapper(jsonMapper)
			configureMapper(yamlMapper)
		End Sub

		''' <returns> The default/primary ObjectMapper for deserializing JSON network configurations in DL4J </returns>
		Public Shared ReadOnly Property Mapper As ObjectMapper
			Get
				Return jsonMapper
			End Get
		End Property

		Public Shared ReadOnly Property LegacyMapper As ObjectMapper
			Get
				SyncLock GetType(JsonMappers)
					If legacyMapper_Conflict Is Nothing Then
						legacyMapper_Conflict = LegacyJsonFormat.Mapper100alpha
						configureMapper(legacyMapper_Conflict)
					End If
					Return legacyMapper_Conflict
				End SyncLock
			End Get
		End Property

		''' <returns> The default/primary ObjectMapper for deserializing network configurations in DL4J (YAML format) </returns>
		Public Shared ReadOnly Property MapperYaml As ObjectMapper
			Get
				Return yamlMapper
			End Get
		End Property

		Private Shared Sub configureMapper(ByVal ret As ObjectMapper)
			ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			ret.enable(SerializationFeature.INDENT_OUTPUT)

			Dim customDeserializerModule As New SimpleModule()
			customDeserializerModule.setDeserializerModifier(New BeanDeserializerModifierAnonymousInnerClass())

			ret.registerModule(customDeserializerModule)
		End Sub

		Private Class BeanDeserializerModifierAnonymousInnerClass
			Inherits BeanDeserializerModifier

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: @Override public JsonDeserializer<?> modifyDeserializer(DeserializationConfig config, BeanDescription beanDesc, JsonDeserializer<?> deserializer)
			Public Overrides Function modifyDeserializer(Of T1)(ByVal config As DeserializationConfig, ByVal beanDesc As BeanDescription, ByVal deserializer As JsonDeserializer(Of T1)) As JsonDeserializer(Of Object)
				'Use our custom deserializers to handle backward compatibility for updaters -> IUpdater
				If beanDesc.getBeanClass() = GetType(MultiLayerConfiguration) Then
					Return New MultiLayerConfigurationDeserializer(deserializer)
				ElseIf beanDesc.getBeanClass() = GetType(ComputationGraphConfiguration) Then
					Return New ComputationGraphConfigurationDeserializer(deserializer)
				End If
				Return deserializer
			End Function
		End Class
	End Class

End Namespace