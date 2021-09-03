Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
Imports DeserializationContext = org.nd4j.shade.jackson.databind.DeserializationContext
Imports JsonDeserializer = org.nd4j.shade.jackson.databind.JsonDeserializer
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
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

Namespace org.nd4j.serde.json


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseLegacyDeserializer<T> extends org.nd4j.shade.jackson.databind.JsonDeserializer<T>
	Public MustInherit Class BaseLegacyDeserializer(Of T)
		Inherits JsonDeserializer(Of T)

		Public MustOverride ReadOnly Property LegacyNamesMap As IDictionary(Of String, String)

		Public MustOverride ReadOnly Property LegacyJsonMapper As ObjectMapper

		Public MustOverride ReadOnly Property DeserializedType As Type

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public T deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws java.io.IOException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal deserializationContext As DeserializationContext) As T
			'Manually parse old format
			Dim node As JsonNode = jp.getCodec().readTree(jp)

			Dim nodes As IEnumerator(Of KeyValuePair(Of String, JsonNode)) = node.fields()

			Dim list As IList(Of KeyValuePair(Of String, JsonNode)) = New List(Of KeyValuePair(Of String, JsonNode))()
			Do While nodes.MoveNext()
				list.Add(nodes.Current)
			Loop

			If list.Count <> 1 Then
				'Should only occur if field is null?
				Return Nothing
			End If

			Dim name As String = list(0).Key
			Dim value As JsonNode = list(0).Value

			Dim legacyNamesMap As IDictionary(Of String, String) = getLegacyNamesMap()
			Dim layerClass As String = legacyNamesMap(name)
			If layerClass Is Nothing Then
				Throw New System.InvalidOperationException("Cannot deserialize " & DeserializedType & " with name """ & name & """: legacy class mapping with this name is unknown")
			End If

			Dim lClass As Type = ND4JClassLoading.loadClassByName(layerClass)
			Objects.requireNonNull(lClass, "Could not find class for deserialization of """ & name & """ of type " & DeserializedType & ": class " & layerClass & " is not on the classpath?")

			Dim m As ObjectMapper = LegacyJsonMapper

			If m Is Nothing Then
				'Should never happen, unless the user is doing something unusual
				Throw New System.InvalidOperationException("Cannot deserialize unknown subclass of type " & DeserializedType & ": no legacy JSON mapper has been set")
			End If

			Dim nodeAsString As String = value.ToString()
			Try
				Dim t As T = m.readValue(nodeAsString, lClass)
				Return t
			Catch e As Exception
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.InvalidOperationException("Cannot deserialize legacy JSON format of object with name """ & name & """ of type " & DeserializedType.FullName, e)
			End Try
		End Function



	End Class

End Namespace