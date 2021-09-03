Imports System.Collections.Generic
Imports org.nd4j.evaluation.classification
Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports DeserializationContext = org.nd4j.shade.jackson.databind.DeserializationContext
Imports JsonDeserializer = org.nd4j.shade.jackson.databind.JsonDeserializer
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
Imports ArrayNode = org.nd4j.shade.jackson.databind.node.ArrayNode
Imports ObjectNode = org.nd4j.shade.jackson.databind.node.ObjectNode

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

Namespace org.nd4j.evaluation.serde


	Public Class ConfusionMatrixDeserializer
		Inherits JsonDeserializer(Of ConfusionMatrix(Of Integer))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.evaluation.classification.ConfusionMatrix<Integer> deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext ctxt) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal ctxt As DeserializationContext) As ConfusionMatrix(Of Integer)
			Dim n As JsonNode = jp.getCodec().readTree(jp)

			'Get class names/labels
			Dim classesNode As ArrayNode = CType(n.get("classes"), ArrayNode)
			Dim classes As IList(Of Integer) = New List(Of Integer)()
			For Each cn As JsonNode In classesNode
				classes.Add(cn.asInt())
			Next cn

			Dim cm As New ConfusionMatrix(Of Integer)(classes)

			Dim matrix As ObjectNode = CType(n.get("matrix"), ObjectNode)
			Dim matrixIter As IEnumerator(Of KeyValuePair(Of String, JsonNode)) = matrix.fields()
			Do While matrixIter.MoveNext()
				Dim e As KeyValuePair(Of String, JsonNode) = matrixIter.Current

				Dim actualClass As Integer = Integer.Parse(e.Key)
				Dim an As ArrayNode = CType(e.Value, ArrayNode)

				Dim innerMultiSetKey As ArrayNode = CType(an.get(0), ArrayNode)
				Dim innerMultiSetCount As ArrayNode = CType(an.get(1), ArrayNode)

				Dim iterKey As IEnumerator(Of JsonNode) = innerMultiSetKey.GetEnumerator()
				Dim iterCnt As IEnumerator(Of JsonNode) = innerMultiSetCount.GetEnumerator()
				Do While iterKey.MoveNext()
					Dim predictedClass As Integer = iterKey.Current.asInt()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim count As Integer = iterCnt.next().asInt()

					cm.add(actualClass, predictedClass, count)
				Loop
			Loop

			Return cm
		End Function
	End Class

End Namespace