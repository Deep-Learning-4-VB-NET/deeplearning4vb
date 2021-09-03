Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY, getterVisibility = JsonAutoDetect.Visibility.NONE, setterVisibility = JsonAutoDetect.Visibility.NONE) @Data public class ExtVocabWord extends org.deeplearning4j.models.word2vec.VocabWord
	<Serializable>
	Public Class ExtVocabWord
		Inherits VocabWord

		Protected Friend extString As String
		Protected Friend counter As Long

		Protected Friend Sub New()
			MyBase.New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExtVocabWord(String extString, long counter, double wordFrequency, @NonNull String word)
		Public Sub New(ByVal extString As String, ByVal counter As Long, ByVal wordFrequency As Double, ByVal word As String)
			MyBase.New(wordFrequency, word)
			Me.extString = extString
			Me.counter = counter
		End Sub
	End Class

End Namespace