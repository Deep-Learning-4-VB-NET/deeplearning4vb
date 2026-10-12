Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayTextDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer
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
Namespace org.deeplearning4j.nn.conf.layers.objdetect


	Public Class BoundingBoxesDeserializer
		Inherits JsonDeserializer(Of INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal deserializationContext As DeserializationContext) As INDArray
			Dim node As JsonNode = jp.getCodec().readTree(jp)
			If node.has("dataBuffer") Then
				'Must be legacy format serialization
				Dim arr As JsonNode = node.get("dataBuffer")
				Dim rank As Integer = node.get("rankField").asInt()
				Dim numElements As Integer = node.get("numElements").asInt()
				Dim offset As Integer = node.get("offsetField").asInt()
				Dim shape As JsonNode = node.get("shapeField")
				Dim stride As JsonNode = node.get("strideField")
				Dim shapeArr(rank - 1) As Integer
				Dim strideArr(rank - 1) As Integer
				Dim buff As DataBuffer = Nd4j.createBuffer(numElements)
				For i As Integer = 0 To numElements - 1
					buff.put(i, arr.get(i).asDouble())
				Next i

				Dim ordering As String = node.get("orderingField").asText()
				For i As Integer = 0 To rank - 1
					shapeArr(i) = shape.get(i).asInt()
					strideArr(i) = stride.get(i).asInt()
				Next i

				Return Nd4j.create(buff, shapeArr, strideArr, offset, ordering.Chars(0))
			End If
			'Standard/new format
			Return (New NDArrayTextDeSerializer()).deserialize(node)
		End Function
	End Class

End Namespace