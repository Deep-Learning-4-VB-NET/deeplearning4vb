Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports DataFormat = org.deeplearning4j.nn.conf.DataFormat
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
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
Namespace org.deeplearning4j.nn.conf.serde.format


	Public Class DataFormatDeserializer
		Inherits JsonDeserializer(Of DataFormat)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.DataFormat deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal deserializationContext As DeserializationContext) As DataFormat
			Dim node As JsonNode = jp.getCodec().readTree(jp)
			Dim text As String = node.textValue()
			Select Case text
				Case "NCHW"
					Return CNN2DFormat.NCHW
				Case "NHWC"
					Return CNN2DFormat.NHWC
				Case "NCW"
					Return RNNFormat.NCW
				Case "NWC"
					Return RNNFormat.NWC
				Case Else
					Return Nothing
			End Select
		End Function
	End Class

End Namespace