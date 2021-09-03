Imports System
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.models.sequencevectors.serialization


	Public Class AbstractElementFactory(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements SequenceElementFactory(Of T)

		Private ReadOnly targetClass As Type

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(AbstractElementFactory))

		''' <summary>
		''' This is the only constructor available for AbstractElementFactory </summary>
		''' <param name="cls"> class that going to be serialized/deserialized using this instance. I.e.: VocabWord.class </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AbstractElementFactory(@NonNull @Class cls)
		Public Sub New(ByVal cls As Type)
			targetClass = cls
		End Sub

		''' <summary>
		''' This method builds object from provided JSON
		''' </summary>
		''' <param name="json"> JSON for restored object </param>
		''' <returns> restored object </returns>
		Public Overridable Function deserialize(ByVal json As String) As T Implements SequenceElementFactory(Of T).deserialize
			Dim mapper As ObjectMapper = SequenceElement.mapper()
			Try
				Dim ret As T = CType(mapper.readValue(json, targetClass), T)
				Return ret
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' This method serializaes object  into JSON string
		''' </summary>
		''' <param name="element">
		''' @return </param>
		Public Overridable Function serialize(ByVal element As T) As String Implements SequenceElementFactory(Of T).serialize
			Dim json As String = Nothing
			Try
				json = element.toJSON()
			Catch e As Exception
				log.error("Direct serialization failed, falling back to jackson")
			End Try

			If json Is Nothing OrElse json.Length = 0 Then
				Dim mapper As ObjectMapper = SequenceElement.mapper()
				Try
					json = mapper.writeValueAsString(element)
				Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
					Throw New Exception(e)
				End Try
			End If

			Return json
		End Function
	End Class

End Namespace