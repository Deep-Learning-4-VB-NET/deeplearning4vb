Imports org.deeplearning4j.nn.conf.distribution
Imports JsonParseException = org.nd4j.shade.jackson.core.JsonParseException
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

Namespace org.deeplearning4j.nn.conf.distribution.serde


	Public Class LegacyDistributionDeserializer
		Inherits JsonDeserializer(Of Distribution)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Distribution deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal deserializationContext As DeserializationContext) As Distribution
			'Manually parse old format
			Dim node As JsonNode = jp.getCodec().readTree(jp)

			If node.has("normal") Then
				Dim n As JsonNode = node.get("normal")
				If Not n.has("mean") OrElse Not n.has("std") Then
					Throw New JsonParseException("Cannot deserialize Distribution: legacy format 'normal' wrapper object " & " is missing 'mean' or 'std' field", jp.getCurrentLocation())
				End If
				Dim m As Double = n.get("mean").asDouble()
				Dim s As Double = n.get("std").asDouble()
				Return New NormalDistribution(m, s)
			ElseIf node.has("gaussian") Then
				Dim n As JsonNode = node.get("gaussian")
				If Not n.has("mean") OrElse Not n.has("std") Then
					Throw New JsonParseException("Cannot deserialize Distribution: legacy format 'gaussian' wrapper object " & " is missing 'mean' or 'std' field", jp.getCurrentLocation())
				End If
				Dim m As Double = n.get("mean").asDouble()
				Dim s As Double = n.get("std").asDouble()
				Return New GaussianDistribution(m, s)

			ElseIf node.has("uniform") Then
				Dim n As JsonNode = node.get("uniform")
				If Not n.has("lower") OrElse Not n.has("upper") Then
					Throw New JsonParseException("Cannot deserialize Distribution: legacy format 'uniform' wrapper object " & " is missing 'lower' or 'upper' field", jp.getCurrentLocation())
				End If
				Dim l As Double = n.get("lower").asDouble()
				Dim u As Double = n.get("upper").asDouble()
				Return New UniformDistribution(l, u)
			ElseIf node.has("binomial") Then
				Dim n As JsonNode = node.get("binomial")
				If Not n.has("numberOfTrials") OrElse Not n.has("probabilityOfSuccess") Then
					Throw New JsonParseException("Cannot deserialize Distribution: legacy format 'binomial' wrapper object " & " is missing 'lower' or 'upper' field", jp.getCurrentLocation())
				End If
				Dim num As Integer = n.get("numberOfTrials").asInt()
				Dim p As Double = n.get("probabilityOfSuccess").asDouble()
				Return New BinomialDistribution(num, p)
			Else
				Throw New JsonParseException("Cannot deserialize Distribution: expected type field or legacy format wrapper" & " object with name being one of {normal, gaussian, uniform, binomial}", jp.getCurrentLocation())
			End If
		End Function
	End Class

End Namespace