Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports BaseTokenizerFunction = org.deeplearning4j.spark.models.sequencevectors.functions.BaseTokenizerFunction
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument

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

Namespace org.deeplearning4j.spark.models.paragraphvectors.functions


	<Serializable>
	Public Class DocumentSequenceConvertFunction
		Inherits BaseTokenizerFunction
		Implements [Function](Of LabelledDocument, Sequence(Of VocabWord))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DocumentSequenceConvertFunction(@NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> configurationBroadcast)
		Public Sub New(ByVal configurationBroadcast As Broadcast(Of VectorsConfiguration))
			MyBase.New(configurationBroadcast)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.word2vec.VocabWord> call(org.deeplearning4j.text.documentiterator.LabelledDocument document) throws Exception
		Public Overrides Function [call](ByVal document As LabelledDocument) As Sequence(Of VocabWord)
			Dim sequence As New Sequence(Of VocabWord)()

			' get elements
			If document.getReferencedContent() IsNot Nothing AndAlso Not document.getReferencedContent().isEmpty() Then
				sequence.addElements(document.getReferencedContent())
			Else
				If tokenizerFactory Is Nothing Then
					instantiateTokenizerFactory()
				End If

				Dim tokens As IList(Of String) = tokenizerFactory.create(document.getContent()).getTokens()

				For Each token As String In tokens
					If token Is Nothing OrElse token.Length = 0 Then
						Continue For
					End If

					Dim word As New VocabWord(1.0, token)
					sequence.addElement(word)
				Next token
			End If

			' get labels
			For Each label As String In document.getLabels()
				If label Is Nothing OrElse label.Length = 0 Then
					Continue For
				End If

				Dim labelElement As New VocabWord(1.0, label)
				labelElement.markAsLabel(True)

				sequence.addSequenceLabel(labelElement)
			Next label

			Return sequence
		End Function
	End Class

End Namespace