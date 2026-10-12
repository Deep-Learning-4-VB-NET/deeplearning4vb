Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord

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

Namespace org.deeplearning4j.spark.models.sequencevectors.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TokenizerFunction extends BaseTokenizerFunction implements org.apache.spark.api.java.function.@Function<String, org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.word2vec.VocabWord>>
	<Serializable>
	Public Class TokenizerFunction
		Inherits BaseTokenizerFunction
		Implements [Function](Of String, Sequence(Of VocabWord))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TokenizerFunction(@NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> configurationBroadcast)
		Public Sub New(ByVal configurationBroadcast As Broadcast(Of VectorsConfiguration))
			MyBase.New(configurationBroadcast)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.word2vec.VocabWord> call(String s) throws Exception
		Public Overrides Function [call](ByVal s As String) As Sequence(Of VocabWord)
			If tokenizerFactory Is Nothing Then
				instantiateTokenizerFactory()
			End If

			Dim tokens As IList(Of String) = tokenizerFactory.create(s).getTokens()
			Dim seq As New Sequence(Of VocabWord)()
			For Each token As String In tokens
				If token Is Nothing OrElse token.Length = 0 Then
					Continue For
				End If

				seq.addElement(New VocabWord(1.0, token))
			Next token

			Return seq
		End Function
	End Class

End Namespace