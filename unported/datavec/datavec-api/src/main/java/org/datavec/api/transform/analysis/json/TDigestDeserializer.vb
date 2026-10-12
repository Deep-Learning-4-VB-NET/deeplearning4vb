Imports System
Imports System.IO
Imports TDigest = com.tdunning.math.stats.TDigest
Imports Base64 = org.apache.commons.codec.binary.Base64
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

Namespace org.datavec.api.transform.analysis.json


	Public Class TDigestDeserializer
		Inherits JsonDeserializer(Of TDigest)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public com.tdunning.math.stats.TDigest deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext d) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal d As DeserializationContext) As TDigest
			Dim node As JsonNode = CType(jp.getCodec().readTree(jp), JsonNode)
			Dim field As String = node.get("digest").asText()
			Dim b As New Base64()
			Dim bytes() As SByte = b.decode(field)
			Try
					Using ois As New ObjectInputStream(New MemoryStream(bytes))
					Return CType(ois.readObject(), TDigest)
					End Using
			Catch e As Exception
				Throw New Exception("Error deserializing TDigest object from JSON", e)
			End Try
		End Function
	End Class

End Namespace