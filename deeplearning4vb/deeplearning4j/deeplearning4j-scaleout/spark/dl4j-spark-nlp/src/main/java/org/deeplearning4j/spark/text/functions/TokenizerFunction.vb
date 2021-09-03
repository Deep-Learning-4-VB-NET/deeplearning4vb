Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports [Function] = org.apache.spark.api.java.function.Function
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess
Imports NGramTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.NGramTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory

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

Namespace org.deeplearning4j.spark.text.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") @Slf4j public class TokenizerFunction implements org.apache.spark.api.java.function.@Function<String, java.util.List<String>>
	Public Class TokenizerFunction
		Implements [Function](Of String, IList(Of String))

		Private tokenizerFactoryClazz As String
		Private tokenizerPreprocessorClazz As String
'JAVA TO VB CONVERTER NOTE: The field tokenizerFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private tokenizerFactory_Conflict As TokenizerFactory
		Private nGrams As Integer = 1

		Public Sub New(ByVal tokenizer As String, ByVal tokenizerPreprocessor As String, ByVal nGrams As Integer)
			Me.tokenizerFactoryClazz = tokenizer
			Me.tokenizerPreprocessorClazz = tokenizerPreprocessor
			Me.nGrams = nGrams
		End Sub

		Public Overrides Function [call](ByVal str As String) As IList(Of String)
			If tokenizerFactory_Conflict Is Nothing Then
				tokenizerFactory_Conflict = TokenizerFactory
			End If

			If str.Length = 0 Then
				Return Collections.singletonList("")
			End If

			Return tokenizerFactory_Conflict.create(str).getTokens()
		End Function

		Private ReadOnly Property TokenizerFactory As TokenizerFactory
			Get
				Dim tokenPreProcessInst As TokenPreProcess = Nothing
    
				If StringUtils.isNotEmpty(tokenizerPreprocessorClazz) Then
					tokenPreProcessInst = DL4JClassLoading.createNewInstance(tokenizerPreprocessorClazz)
				End If
    
				tokenizerFactory_Conflict = DL4JClassLoading.createNewInstance(tokenizerFactoryClazz)
    
				If tokenPreProcessInst IsNot Nothing Then
					tokenizerFactory_Conflict.TokenPreProcessor = tokenPreProcessInst
				End If
				If nGrams > 1 Then
					tokenizerFactory_Conflict = New NGramTokenizerFactory(tokenizerFactory_Conflict, nGrams, nGrams)
				End If
    
				Return tokenizerFactory_Conflict
			End Get
		End Property

	End Class

End Namespace