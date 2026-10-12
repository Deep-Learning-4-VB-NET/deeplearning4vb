Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports DeserializationContext = org.nd4j.shade.jackson.databind.DeserializationContext
Imports JsonDeserializer = org.nd4j.shade.jackson.databind.JsonDeserializer
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
Imports ArrayNode = org.nd4j.shade.jackson.databind.node.ArrayNode

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

Namespace org.deeplearning4j.nn.conf.serde.legacy


	Public Class LegacyIntArrayDeserializer
		Inherits JsonDeserializer(Of Integer())

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public int[] deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal deserializationContext As DeserializationContext) As Integer()
			Dim n As JsonNode = jp.getCodec().readTree(jp)
			If n.isArray() Then
				Dim an As ArrayNode = CType(n, ArrayNode)
				Dim size As Integer = an.size()
				Dim [out](size - 1) As Integer
				For i As Integer = 0 To size - 1
					[out](i) = an.get(i).asInt()
				Next i
				Return [out]
			ElseIf n.isNumber() Then
				Dim v As Integer = n.asInt()
				Return New Integer(){v, v}
			Else
				Throw New System.InvalidOperationException("Could not deserialize value: " & n)
			End If
		End Function
	End Class

End Namespace