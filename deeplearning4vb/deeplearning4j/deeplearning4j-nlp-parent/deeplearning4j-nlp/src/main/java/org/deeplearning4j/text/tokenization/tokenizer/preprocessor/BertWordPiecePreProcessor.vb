Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text
Imports Microsoft.VisualBasic
Imports IntOpenHashSet = it.unimi.dsi.fastutil.ints.IntOpenHashSet
Imports IntSet = it.unimi.dsi.fastutil.ints.IntSet
Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess

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

Namespace org.deeplearning4j.text.tokenization.tokenizer.preprocessor


	Public Class BertWordPiecePreProcessor
		Implements TokenPreProcess

		Public Const REPLACEMENT_CHAR As Char = ChrW(&Hfffd)

		Protected Friend ReadOnly lowerCase As Boolean
		Protected Friend ReadOnly stripAccents As Boolean
		Protected Friend ReadOnly charSet As IntSet

		Public Sub New()
			Me.New(False, False, Nothing)
		End Sub

		''' 
		''' <param name="lowerCase"> If true: tokenization should convert all characters to lower case </param>
		''' <param name="stripAccents">  If true: strip accents off characters. Usually same as lower case. Should be true when using "uncased" official BERT TensorFlow models </param>
		Public Sub New(ByVal lowerCase As Boolean, ByVal stripAccents As Boolean, ByVal vocab As IDictionary(Of String, Integer))
			Me.lowerCase = lowerCase
			Me.stripAccents = stripAccents
			If vocab IsNot Nothing Then
				charSet = New IntOpenHashSet()
				For Each s As String In vocab.Keys
					Dim cpNum As Integer = 0
					Dim n As Integer = s.codePointCount(0, s.Length)
					Dim charOffset As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while (cpNum++ < n)
					Do While cpNum < n
							cpNum += 1
						Dim cp As Integer = Char.ConvertToUtf32(s, charOffset)
						charOffset += Character.charCount(cp)
						charSet.add(cp)
					Loop
						cpNum += 1
				Next s
			Else
				charSet = Nothing
			End If
		End Sub

		Public Overridable Function preProcess(ByVal token As String) As String Implements TokenPreProcess.preProcess
			If stripAccents Then
				token = Normalizer.normalize(token, Normalizer.Form.NFD)
			End If

			Dim n As Integer = token.codePointCount(0, token.Length)
			Dim sb As New StringBuilder()
			Dim charOffset As Integer = 0
			Dim cps As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while(cps++ < n)
			Do While cps < n
					cps += 1
				Dim cp As Integer = Char.ConvertToUtf32(token, charOffset)
				charOffset += Character.charCount(cp)

				'Remove control characters and accents
				If cp = 0 OrElse cp = AscW(REPLACEMENT_CHAR) OrElse isControlCharacter(cp) OrElse (stripAccents AndAlso Character.getType(cp) = UnicodeCategory.NonSpacingMark) Then
					Continue Do
				End If

				'Convert to lower case if necessary
				If lowerCase Then
					cp = AscW(Char.ToLower(cp))
				End If

				'Replace whitespace chars with space
				If isWhiteSpace(cp) Then
					sb.Append(" "c)
					Continue Do
				End If

				If charSet IsNot Nothing AndAlso Not charSet.contains(cp) Then
					'Skip unknown character (out-of-vocab - though this should rarely happen)
					Continue Do
				End If

				'Handle Chinese and other characters
				If isChineseCharacter(cp) Then
					sb.Append(" "c)
					sb.appendCodePoint(cp)
					sb.Append(" "c)
					Continue Do
				End If

				'All other characters - keep
				sb.appendCodePoint(cp)
			Loop
				cps += 1

			Return sb.ToString()
		End Function

		Public Shared Function isControlCharacter(ByVal cp As Integer) As Boolean
			'Treat newline/tab as whitespace
			If cp = ControlChars.Tab OrElse cp = ControlChars.Lf OrElse cp = ControlChars.Cr Then
				Return False
			End If
			Dim type As Integer = Character.getType(cp)
			Return type = UnicodeCategory.Control OrElse type = UnicodeCategory.Format
		End Function

		Public Shared Function isWhiteSpace(ByVal cp As Integer) As Boolean
			'Treat newline/tab as whitespace
			If cp = ControlChars.Tab OrElse cp = ControlChars.Lf OrElse cp = ControlChars.Cr Then
				Return True
			End If
			Dim type As Integer = Character.getType(cp)
			Return type = UnicodeCategory.SpaceSeparator
		End Function

		Public Shared Function isChineseCharacter(ByVal cp As Integer) As Boolean
			'Remove any CJK Unicode code block characters
			' https://en.wikipedia.org/wiki/List_of_CJK_Unified_Ideographs,_part_1_of_4
			Return (cp >= &H4E00 AndAlso cp <= &H9FFF) OrElse (cp >= &H3400 AndAlso cp <= &H4DBF) OrElse (cp >= &H20000 AndAlso cp <= &H2A6DF) OrElse (cp >= &H2A700 AndAlso cp <= &H2B73F) OrElse (cp >= &H2B740 AndAlso cp <= &H2B81F) OrElse (cp >= &H2B820 AndAlso cp <= &H2CEAF) OrElse (cp >= &HF900 AndAlso cp <= &HFAFF) OrElse (cp >= &H2F800 AndAlso cp <= &H2FA1F)
		End Function


		''' <summary>
		''' Reconstruct the String from tokens </summary>
		''' <param name="tokens">
		''' @return </param>
		Public Shared Function reconstructFromTokens(ByVal tokens As IList(Of String)) As String
			Dim sb As New StringBuilder()
			Dim first As Boolean = True
			For Each s As String In tokens
				If s.StartsWith("##", StringComparison.Ordinal) Then
					sb.Append(s.Substring(2))
				Else
					If Not first AndAlso Not ".".Equals(s) Then
						sb.Append(" ")
					End If
					sb.Append(s)
					first = False
	'            }
				End If
			Next s
			Return sb.ToString()
		End Function
	End Class

End Namespace