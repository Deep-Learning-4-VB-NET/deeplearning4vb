Imports System.Collections.Generic
Imports DateTimeFieldType = org.joda.time.DateTimeFieldType
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

Namespace org.datavec.api.util.jackson


	Public Class DateTimeFieldTypeDeserializer
		Inherits JsonDeserializer(Of DateTimeFieldType)

		'Yes this is ugly - couldn't find a better way :/
		Private Shared ReadOnly map As IDictionary(Of String, DateTimeFieldType) = getMap()

		Private Shared ReadOnly Property Map As IDictionary(Of String, DateTimeFieldType)
			Get
				Dim ret As IDictionary(Of String, DateTimeFieldType) = New Dictionary(Of String, DateTimeFieldType)()
				ret(DateTimeFieldType.centuryOfEra().getName()) = DateTimeFieldType.centuryOfEra()
				ret(DateTimeFieldType.clockhourOfDay().getName()) = DateTimeFieldType.clockhourOfDay()
				ret(DateTimeFieldType.clockhourOfHalfday().getName()) = DateTimeFieldType.clockhourOfHalfday()
				ret(DateTimeFieldType.dayOfMonth().getName()) = DateTimeFieldType.dayOfMonth()
				ret(DateTimeFieldType.dayOfWeek().getName()) = DateTimeFieldType.dayOfWeek()
				ret(DateTimeFieldType.dayOfYear().getName()) = DateTimeFieldType.dayOfYear()
				ret(DateTimeFieldType.era().getName()) = DateTimeFieldType.era()
				ret(DateTimeFieldType.halfdayOfDay().getName()) = DateTimeFieldType.halfdayOfDay()
				ret(DateTimeFieldType.hourOfDay().getName()) = DateTimeFieldType.hourOfDay()
				ret(DateTimeFieldType.hourOfHalfday().getName()) = DateTimeFieldType.hourOfHalfday()
				ret(DateTimeFieldType.millisOfDay().getName()) = DateTimeFieldType.millisOfDay()
				ret(DateTimeFieldType.millisOfSecond().getName()) = DateTimeFieldType.millisOfSecond()
				ret(DateTimeFieldType.minuteOfDay().getName()) = DateTimeFieldType.minuteOfDay()
				ret(DateTimeFieldType.minuteOfHour().getName()) = DateTimeFieldType.minuteOfHour()
				ret(DateTimeFieldType.secondOfDay().getName()) = DateTimeFieldType.secondOfDay()
				ret(DateTimeFieldType.secondOfMinute().getName()) = DateTimeFieldType.secondOfMinute()
				ret(DateTimeFieldType.weekOfWeekyear().getName()) = DateTimeFieldType.weekOfWeekyear()
				ret(DateTimeFieldType.weekyear().getName()) = DateTimeFieldType.weekyear()
				ret(DateTimeFieldType.weekyearOfCentury().getName()) = DateTimeFieldType.weekyearOfCentury()
				ret(DateTimeFieldType.year().getName()) = DateTimeFieldType.year()
				ret(DateTimeFieldType.yearOfCentury().getName()) = DateTimeFieldType.yearOfCentury()
				ret(DateTimeFieldType.yearOfEra().getName()) = DateTimeFieldType.yearOfEra()
    
				Return ret
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.joda.time.DateTimeFieldType deserialize(org.nd4j.shade.jackson.core.JsonParser jsonParser, org.nd4j.shade.jackson.databind.DeserializationContext deserializationContext) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Function deserialize(ByVal jsonParser As JsonParser, ByVal deserializationContext As DeserializationContext) As DateTimeFieldType
			Dim node As JsonNode = jsonParser.getCodec().readTree(jsonParser)
			Dim value As String = node.get("fieldType").textValue()
			Return map(value)
		End Function
	End Class

End Namespace