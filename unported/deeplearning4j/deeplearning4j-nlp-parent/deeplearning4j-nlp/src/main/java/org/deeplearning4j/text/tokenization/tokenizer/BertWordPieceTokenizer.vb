Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BertWordPiecePreProcessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.BertWordPiecePreProcessor

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

Namespace org.deeplearning4j.text.tokenization.tokenizer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BertWordPieceTokenizer implements Tokenizer
	Public Class BertWordPieceTokenizer
		Implements Tokenizer

		Public Shared ReadOnly splitPattern As Pattern = Pattern.compile("\p{javaWhitespace}+|((?<=\p{Punct})+|(?=\p{Punct}+))")

		Private ReadOnly tokens As IList(Of String)
		Private ReadOnly preTokenizePreProcessor As TokenPreProcess
		Private tokenPreProcess As TokenPreProcess
		Private ReadOnly cursor As New AtomicInteger(0)

		Public Sub New(ByVal tokens As String, ByVal vocab As NavigableMap(Of String, Integer), ByVal preTokenizePreProcessor As TokenPreProcess, ByVal tokenPreProcess As TokenPreProcess)
			If vocab.comparator() Is Nothing OrElse vocab.comparator().compare("a", "b") < 0 Then
				Throw New System.ArgumentException("Vocab must use reverse sort order!")
			End If
			Me.preTokenizePreProcessor = preTokenizePreProcessor
			Me.tokenPreProcess = tokenPreProcess

			Me.tokens = tokenize(vocab, tokens)
		End Sub


		Public Overridable Function hasMoreTokens() As Boolean Implements Tokenizer.hasMoreTokens
			Return cursor.get() < tokens.Count
		End Function

		Public Overridable Function countTokens() As Integer Implements Tokenizer.countTokens
			Return tokens.Count
		End Function

		Public Overridable Function nextToken() As String Implements Tokenizer.nextToken
			Dim base As String = tokens(cursor.getAndIncrement())
			If tokenPreProcess IsNot Nothing Then
				base = tokenPreProcess.preProcess(base)
			End If
			Return base
		End Function

		Public Overridable ReadOnly Property Tokens As IList(Of String)
			Get
				If tokenPreProcess IsNot Nothing Then
	'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
	'ORIGINAL LINE: final List<String> result = new ArrayList<>(tokens.size());
					Dim result As IList(Of String) = New List(Of String)(tokens.Count)
					For Each token As String In tokens
						result.Add(tokenPreProcess.preProcess(token))
					Next token
					Return result
				Else
					Return tokens
				End If
			End Get
		End Property

		Public Overridable WriteOnly Property TokenPreProcessor Implements Tokenizer.setTokenPreProcessor As TokenPreProcess
			Set(ByVal tokenPreProcessor As TokenPreProcess)
				Me.tokenPreProcess = tokenPreProcessor
    
			End Set
		End Property

		Private Function tokenize(ByVal vocab As NavigableMap(Of String, Integer), ByVal toTokenize As String) As IList(Of String)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<String> output = new ArrayList<>();
			Dim output As IList(Of String) = New List(Of String)()

			Dim fullString As String = toTokenize
			If preTokenizePreProcessor IsNot Nothing Then
				fullString = preTokenizePreProcessor.preProcess(toTokenize)
			End If

			For Each basicToken As String In splitPattern.split(fullString)
				Dim candidate As String = basicToken
				Dim count As Integer = 0
				Do While candidate.Length > 0 AndAlso Not "##".Equals(candidate)
					Dim longestSubstring As String = findLongestSubstring(vocab, candidate)
					output.Add(longestSubstring)
					candidate = "##" & candidate.Substring(longestSubstring.Length)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if(count++ > basicToken.length())
					If count > basicToken.Length Then
							count += 1
						'Can't take more steps to tokenize than the length of the token
						Throw New System.InvalidOperationException("Invalid token encountered: """ & basicToken & """ likely contains characters that are not " & "present in the vocabulary. Invalid tokens may be cleaned in a preprocessing step using a TokenPreProcessor." & " preTokenizePreProcessor=" & preTokenizePreProcessor & ", tokenPreProcess=" & tokenPreProcess)
						Else
							count += 1
						End If
				Loop
			Next basicToken

			Return output
		End Function

		Protected Friend Overridable Function findLongestSubstring(ByVal vocab As NavigableMap(Of String, Integer), ByVal candidate As String) As String
			Dim tailMap As NavigableMap(Of String, Integer) = vocab.tailMap(candidate, True)
			checkIfEmpty(tailMap, candidate)

			Dim longestSubstring As String = tailMap.firstKey()
			Dim subStringLength As Integer = Math.Min(candidate.Length, longestSubstring.Length)
			Do While Not candidate.StartsWith(longestSubstring, StringComparison.Ordinal)
				subStringLength -= 1
				tailMap = tailMap.tailMap(candidate.Substring(0, subStringLength), True)
				checkIfEmpty(tailMap, candidate)
				longestSubstring = tailMap.firstKey()
			Loop
			Return longestSubstring
		End Function

		Protected Friend Overridable Sub checkIfEmpty(ByVal m As IDictionary(Of String, Integer), ByVal candidate As String)
			If m.Count = 0 Then
				Throw New System.InvalidOperationException("Invalid token/character encountered: """ & candidate & """ likely contains characters that are not " & "present in the vocabulary. Invalid tokens may be cleaned in a preprocessing step using a TokenPreProcessor." & " preTokenizePreProcessor=" & preTokenizePreProcessor & ", tokenPreProcess=" & tokenPreProcess)
			End If
		End Sub

	End Class

End Namespace