﻿Imports DateTimeFieldType = org.joda.time.DateTimeFieldType
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

Namespace org.datavec.api.util.jackson


	Public Class DateTimeFieldTypeSerializer
		Inherits JsonSerializer(Of DateTimeFieldType)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(org.joda.time.DateTimeFieldType dateTimeFieldType, org.nd4j.shade.jackson.core.JsonGenerator jsonGenerator, org.nd4j.shade.jackson.databind.SerializerProvider serializerProvider) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Sub serialize(ByVal dateTimeFieldType As DateTimeFieldType, ByVal jsonGenerator As JsonGenerator, ByVal serializerProvider As SerializerProvider)
			jsonGenerator.writeStartObject()
			jsonGenerator.writeStringField("fieldType", dateTimeFieldType.getName())
			jsonGenerator.writeEndObject()
		End Sub
	End Class

End Namespace