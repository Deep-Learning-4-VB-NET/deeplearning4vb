Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4jBase64 = org.nd4j.serde.base64.Nd4jBase64
Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
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

Namespace org.nd4j.serde.jackson.shaded


	''' <summary>
	''' @author Adam Gibson
	''' </summary>

	Public Class NDArrayDeSerializer
		Inherits JsonDeserializer(Of INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws java.io.IOException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal deserializationContext As DeserializationContext) As INDArray
			Dim node As JsonNode = jp.getCodec().readTree(jp)
			Dim field As String = node.get("array").asText()
			Return Nd4jBase64.fromBase64(field)
		End Function
	End Class

End Namespace