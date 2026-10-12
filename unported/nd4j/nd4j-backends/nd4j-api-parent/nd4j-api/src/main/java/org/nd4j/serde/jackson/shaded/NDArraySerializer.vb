Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4jBase64 = org.nd4j.serde.base64.Nd4jBase64
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports JsonSerializer = org.nd4j.shade.jackson.databind.JsonSerializer
Imports SerializerProvider = org.nd4j.shade.jackson.databind.SerializerProvider

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
	Public Class NDArraySerializer
		Inherits JsonSerializer(Of INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(org.nd4j.linalg.api.ndarray.INDArray indArray, org.nd4j.shade.jackson.core.JsonGenerator jsonGenerator, org.nd4j.shade.jackson.databind.SerializerProvider serializerProvider) throws java.io.IOException
		Public Overrides Sub serialize(ByVal indArray As INDArray, ByVal jsonGenerator As JsonGenerator, ByVal serializerProvider As SerializerProvider)
			Dim toBase64 As String = Nd4jBase64.base64String(indArray)
			jsonGenerator.writeStartObject()
			jsonGenerator.writeStringField("array", toBase64)
			jsonGenerator.writeEndObject()
		End Sub
	End Class

End Namespace