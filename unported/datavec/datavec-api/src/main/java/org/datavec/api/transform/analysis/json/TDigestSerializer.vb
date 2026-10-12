Imports System.IO
Imports TDigest = com.tdunning.math.stats.TDigest
Imports Base64 = org.apache.commons.codec.binary.Base64
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
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

Namespace org.datavec.api.transform.analysis.json


	Public Class TDigestSerializer
		Inherits JsonSerializer(Of TDigest)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(com.tdunning.math.stats.TDigest td, org.nd4j.shade.jackson.core.JsonGenerator j, org.nd4j.shade.jackson.databind.SerializerProvider sp) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Sub serialize(ByVal td As TDigest, ByVal j As JsonGenerator, ByVal sp As SerializerProvider)
			Using baos As New MemoryStream(), oos As New java.io.ObjectOutputStream(baos)
				oos.writeObject(td)
				oos.close()
				Dim bytes() As SByte = baos.toByteArray()
				Dim b As New Base64()
				Dim str As String = b.encodeAsString(bytes)
				j.writeStartObject()
				j.writeStringField("digest", str)
				j.writeEndObject()
			End Using
		End Sub
	End Class

End Namespace