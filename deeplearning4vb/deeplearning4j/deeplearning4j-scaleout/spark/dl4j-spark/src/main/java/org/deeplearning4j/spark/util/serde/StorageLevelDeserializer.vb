Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports DeserializationContext = org.nd4j.shade.jackson.databind.DeserializationContext
Imports JsonDeserializer = org.nd4j.shade.jackson.databind.JsonDeserializer
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode

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

Namespace org.deeplearning4j.spark.util.serde


	Public Class StorageLevelDeserializer
		Inherits JsonDeserializer(Of StorageLevel)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.apache.spark.storage.StorageLevel deserialize(org.nd4j.shade.jackson.core.JsonParser jsonParser, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jsonParser As JsonParser, ByVal deserializationContext As DeserializationContext) As StorageLevel
			Dim node As JsonNode = jsonParser.getCodec().readTree(jsonParser)
			Dim value As String = node.textValue()
			If value Is Nothing OrElse "null".Equals(value) Then
				Return Nothing
			End If
			Return StorageLevel.fromString(value)
		End Function
	End Class

End Namespace