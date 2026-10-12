Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports DefaultStreamTokenizer = org.deeplearning4j.text.tokenization.tokenizer.DefaultStreamTokenizer
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
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

Namespace org.deeplearning4j.text.movingwindow


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Windows
	Public Class Windows


		Private Sub New()
		End Sub

		''' <summary>
		''' Constructs a list of window of size windowSize.
		''' Note that padding for each window is created as well. </summary>
		''' <param name="words"> the words to tokenize and construct windows from </param>
		''' <param name="windowSize"> the window size to generate </param>
		''' <returns> the list of windows for the tokenized string </returns>
		Public Shared Function windows(ByVal words As Stream, ByVal windowSize As Integer) As IList(Of Window)
			Dim tokenizer As Tokenizer = New DefaultStreamTokenizer(words)
			Dim list As IList(Of String) = New List(Of String)()
			Do While tokenizer.hasMoreTokens()
				list.Add(tokenizer.nextToken())
			Loop
			Return windows(list, windowSize)
		End Function

		''' <summary>
		''' Constructs a list of window of size windowSize.
		''' Note that padding for each window is created as well. </summary>
		''' <param name="words"> the words to tokenize and construct windows from </param>
		''' <param name="tokenizerFactory"> tokenizer factory to use </param>
		''' <param name="windowSize"> the window size to generate </param>
		''' <returns> the list of windows for the tokenized string </returns>
		Public Shared Function windows(ByVal words As Stream, ByVal tokenizerFactory As TokenizerFactory, ByVal windowSize As Integer) As IList(Of Window)
			Dim tokenizer As Tokenizer = tokenizerFactory.create(words)
			Dim list As IList(Of String) = New List(Of String)()
			Do While tokenizer.hasMoreTokens()
				list.Add(tokenizer.nextToken())
			Loop

			If list.Count = 0 Then
				Throw New System.InvalidOperationException("No tokens found for windows")
			End If

			Return windows(list, windowSize)
		End Function


		''' <summary>
		''' Constructs a list of window of size windowSize.
		''' Note that padding for each window is created as well. </summary>
		''' <param name="words"> the words to tokenize and construct windows from </param>
		''' <param name="windowSize"> the window size to generate </param>
		''' <returns> the list of windows for the tokenized string </returns>
		Public Shared Function windows(ByVal words As String, ByVal windowSize As Integer) As IList(Of Window)
			Dim tokenizer As New StringTokenizer(words)
			Dim list As IList(Of String) = New List(Of String)()
			Do While tokenizer.hasMoreTokens()
				list.Add(tokenizer.nextToken())
			Loop
			Return windows(list, windowSize)
		End Function

		''' <summary>
		''' Constructs a list of window of size windowSize.
		''' Note that padding for each window is created as well. </summary>
		''' <param name="words"> the words to tokenize and construct windows from </param>
		''' <param name="tokenizerFactory"> tokenizer factory to use </param>
		''' <param name="windowSize"> the window size to generate </param>
		''' <returns> the list of windows for the tokenized string </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static java.util.List<Window> windows(String words, @NonNull TokenizerFactory tokenizerFactory, int windowSize, org.deeplearning4j.models.embeddings.wordvectors.WordVectors vectors)
		Public Shared Function windows(ByVal words As String, ByVal tokenizerFactory As TokenizerFactory, ByVal windowSize As Integer, ByVal vectors As WordVectors) As IList(Of Window)
			Dim tokenizer As Tokenizer = tokenizerFactory.create(words)
			Dim list As IList(Of String) = New List(Of String)()
			Do While tokenizer.hasMoreTokens()
				Dim token As String = tokenizer.nextToken()

				' if we don't have UNK word defined - we have to skip this word
				If vectors.getWordVectorMatrix(token) IsNot Nothing Then
					list.Add(token)
				End If
			Loop

			If list.Count = 0 Then
				Throw New System.InvalidOperationException("No tokens found for windows")
			End If

			Return windows(list, windowSize)
		End Function


		''' <summary>
		''' Constructs a list of window of size windowSize.
		''' Note that padding for each window is created as well. </summary>
		''' <param name="words"> the words to tokenize and construct windows from </param>
		''' <returns> the list of windows for the tokenized string </returns>
		Public Shared Function windows(ByVal words As String) As IList(Of Window)
			Dim tokenizer As New StringTokenizer(words)
			Dim list As IList(Of String) = New List(Of String)()
			Do While tokenizer.hasMoreTokens()
				list.Add(tokenizer.nextToken())
			Loop
			Return windows(list, 5)
		End Function

		''' <summary>
		''' Constructs a list of window of size windowSize.
		''' Note that padding for each window is created as well. </summary>
		''' <param name="words"> the words to tokenize and construct windows from </param>
		''' <param name="tokenizerFactory"> tokenizer factory to use </param>
		''' <returns> the list of windows for the tokenized string </returns>
		Public Shared Function windows(ByVal words As String, ByVal tokenizerFactory As TokenizerFactory) As IList(Of Window)
			Dim tokenizer As Tokenizer = tokenizerFactory.create(words)
			Dim list As IList(Of String) = New List(Of String)()
			Do While tokenizer.hasMoreTokens()
				list.Add(tokenizer.nextToken())
			Loop
			Return windows(list, 5)
		End Function


		''' <summary>
		''' Creates a sliding window from text </summary>
		''' <param name="windowSize"> the window size to use </param>
		''' <param name="wordPos"> the position of the word to center </param>
		''' <param name="sentence"> the sentence to createComplex a window for </param>
		''' <returns> a window based on the given sentence </returns>
		Public Shared Function windowForWordInPosition(ByVal windowSize As Integer, ByVal wordPos As Integer, ByVal sentence As IList(Of String)) As Window
			Dim window As IList(Of String) = New List(Of String)()
			Dim onlyTokens As IList(Of String) = New List(Of String)()
			Dim contextSize As Integer = CInt(Math.Floor((windowSize - 1) \ 2))

			Dim i As Integer = wordPos - contextSize
			Do While i <= wordPos + contextSize
				If i < 0 Then
					window.Add("<s>")
				ElseIf i >= sentence.Count Then
					window.Add("</s>")
				Else
					onlyTokens.Add(sentence(i))
					window.Add(sentence(i))

				End If
				i += 1
			Loop

			Dim wholeSentence As String = StringUtils.join(sentence, " ")
			Dim window2 As String = StringUtils.join(onlyTokens, " ")
			Dim begin As Integer = wholeSentence.IndexOf(window2, StringComparison.Ordinal)
			Dim [end] As Integer = begin + window2.Length
			Return New Window(window, windowSize, begin, [end])

		End Function


		''' <summary>
		''' Constructs a list of window of size windowSize </summary>
		''' <param name="words"> the words to  construct windows from </param>
		''' <returns> the list of windows for the tokenized string </returns>
		Public Shared Function windows(ByVal words As IList(Of String), ByVal windowSize As Integer) As IList(Of Window)

			Dim ret As IList(Of Window) = New List(Of Window)()

			Dim i As Integer = 0
			Do While i < words.Count
				ret.Add(windowForWordInPosition(windowSize, i, words))
				i += 1
			Loop


			Return ret
		End Function

	End Class

End Namespace