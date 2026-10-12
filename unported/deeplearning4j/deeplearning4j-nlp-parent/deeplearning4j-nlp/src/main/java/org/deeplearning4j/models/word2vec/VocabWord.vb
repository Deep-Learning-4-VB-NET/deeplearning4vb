Imports System
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.models.word2vec



	''' <summary>
	''' Intermediate layers of the neural network
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class", defaultImpl = VocabWord.class) @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY, getterVisibility = JsonAutoDetect.Visibility.NONE, setterVisibility = JsonAutoDetect.Visibility.NONE) public class VocabWord extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement implements java.io.Serializable
	<Serializable>
	Public Class VocabWord
		Inherits SequenceElement

		Private Const serialVersionUID As Long = 2223750736522624256L

		'for my sanity
'JAVA TO VB CONVERTER NOTE: The field word was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private word_Conflict As String

	'    
	'        Used for Joint/Distributed vocabs mechanics
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected System.Nullable<Long> vocabId;
		Protected Friend vocabId As Long?
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected System.Nullable<Long> affinityId;
		Protected Friend affinityId As Long?

		Public Shared Function none() As VocabWord
			Return New VocabWord(0, "none")
		End Function

		''' 
		''' <param name="wordFrequency"> count of the word
		'''  </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocabWord(double wordFrequency, @NonNull String word)
		Public Sub New(ByVal wordFrequency As Double, ByVal word As String)
			If word.Length = 0 Then
				Throw New System.ArgumentException("Word must not be null or empty")
			End If
			Me.word_Conflict = word
			Me.elementFrequency_Conflict.set(wordFrequency)
			Me.storageId = SequenceElement.getLongHash(word)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocabWord(double wordFrequency, @NonNull String word, long storageId)
		Public Sub New(ByVal wordFrequency As Double, ByVal word As String, ByVal storageId As Long)
			Me.New(wordFrequency, word)
			Me.storageId = storageId
		End Sub


		Public Sub New()
		End Sub


		Public Overrides ReadOnly Property Label As String
			Get
				Return Me.word_Conflict
			End Get
		End Property
	'    
	'    	public void write(DataOutputStream dos) throws IOException {
	'    		dos.writeDouble(this.elementFrequency.get());
	'    	}
	'    
	'    	public VocabWord read(DataInputStream dos) throws IOException {
	'    		this.elementFrequency.set(dos.readDouble());
	'    		return this;
	'    	}
	'    
	'    

		Public Overridable Property Word As String
			Get
				Return word_Conflict
			End Get
			Set(ByVal word As String)
				Me.word_Conflict = word
			End Set
		End Property



		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If Not (TypeOf o Is VocabWord) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final VocabWord vocabWord = (VocabWord) o;
'JAVA TO VB CONVERTER NOTE: The variable vocabWord was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim vocabWord_Conflict As VocabWord = DirectCast(o, VocabWord)
			If Me.word_Conflict Is Nothing Then
				Return vocabWord_Conflict.word_Conflict Is Nothing
			End If

			Return Me.word_Conflict.Equals(vocabWord_Conflict.Word)
	'        
	'        if (codeLength != vocabWord.codeLength) return false;
	'        if (index != vocabWord.index) return false;
	'        if (!codes.equals(vocabWord.codes)) return false;
	'        if (historicalGradient != null ? !historicalGradient.equals(vocabWord.historicalGradient) : vocabWord.historicalGradient != null)
	'            return false;
	'        if (!points.equals(vocabWord.points)) return false;
	'        if (!word.equals(vocabWord.word)) return false;
	'        return this.elementFrequency.get() == vocabWord.elementFrequency.get();
	'        
		End Function


		Public Overrides Function GetHashCode() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int result = this.word == null ? 0 : this.word.hashCode();
			Dim result As Integer = If(Me.word_Conflict Is Nothing, 0, Me.word_Conflict.GetHashCode()) 'this.elementFrequency.hashCode();
	'        result = 31 * result + index;
	'        result = 31 * result + codes.hashCode();
	'        result = 31 * result + word.hashCode();
	'        result = 31 * result + (historicalGradient != null ? historicalGradient.hashCode() : 0);
	'        result = 31 * result + points.hashCode();
	'        result = 31 * result + codeLength;
			Return result
		End Function

		Public Overrides Function ToString() As String
			Return "VocabWord{" & "wordFrequency=" & Me.elementFrequency_Conflict & ", index=" & index_Conflict & ", word='" & word_Conflict & "'"c & ", codeLength=" & codeLength_Conflict & "}"c
		End Function

		Public Overrides Function toJSON() As String
			Dim mapper As ObjectMapper = VocabWord.mapper()
			Try
	'            
	'                we need JSON as single line to save it at first line of the CSV model file
	'            
				Return mapper.writeValueAsString(Me)
			Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function


	End Class


End Namespace