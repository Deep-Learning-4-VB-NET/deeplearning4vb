Imports System
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature

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

Namespace org.deeplearning4j.models.word2vec.wordstore


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class VocabularyWord implements java.io.Serializable
	<Serializable>
	Public Class VocabularyWord
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private String word;
		Private word As String
		Private count As Integer = 1

		' these fields are used for vocab serialization/deserialization only. Usual runtime value is null.
		Private syn0() As Double
		Private syn1() As Double
		Private syn1Neg() As Double
		Private historicalGradient() As Double

'JAVA TO VB CONVERTER NOTE: The field mapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared mapper_Conflict As ObjectMapper
		Private Shared ReadOnly lock As New Object()

		' There's no reasons to save HuffmanNode data, it will be recalculated after deserialization
		<NonSerialized>
		Private huffmanNode As HuffmanNode

		' empty constructor is required for proper deserialization
		Public Sub New()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocabularyWord(@NonNull String word)
		Public Sub New(ByVal word As String)
			Me.word = word
		End Sub

	'    
	'        since scavenging mechanics are targeting low-freq words, byte values is definitely enough.
	'        please note, array is initialized outside of this scope, since it's only holder, no logic involved inside this class
	'      
		<NonSerialized>
		Private frequencyShift() As SByte
		<NonSerialized>
		Private retentionStep As SByte

		' special mark means that this word should NOT be affected by minWordFrequency setting
		Private special As Boolean = False

		Public Overridable Sub incrementCount()
			Me.count += 1
		End Sub

		Public Overridable Sub incrementRetentionStep()
			Me.retentionStep += 1
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim word1 As VocabularyWord = DirectCast(o, VocabularyWord)

			Return If(word IsNot Nothing, word.Equals(word1.word), word1.word Is Nothing)

		End Function

		Public Overrides Function GetHashCode() As Integer
			Return If(word IsNot Nothing, word.GetHashCode(), 0)
		End Function

		Private Shared Function mapper() As ObjectMapper
			If mapper_Conflict Is Nothing Then
				SyncLock lock
					If mapper_Conflict Is Nothing Then
						mapper_Conflict = New ObjectMapper()
						mapper_Conflict.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
						mapper_Conflict.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
						mapper_Conflict.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
						Return mapper_Conflict
					End If
				End SyncLock
			End If
			Return mapper_Conflict
		End Function

		Public Overridable Function toJson() As String
			Dim mapper As ObjectMapper = VocabularyWord.mapper()
			Try
	'            
	'                we need JSON as single line to save it at first line of the CSV model file
	'            
				Return mapper.writeValueAsString(Me)
			Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function fromJson(ByVal json As String) As VocabularyWord
			Dim mapper As ObjectMapper = VocabularyWord.mapper()
			Try
				Dim ret As VocabularyWord = mapper.readValue(json, GetType(VocabularyWord))
				Return ret
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace