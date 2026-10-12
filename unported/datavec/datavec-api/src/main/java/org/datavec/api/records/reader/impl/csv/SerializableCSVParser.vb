Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic

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

Namespace org.datavec.api.records.reader.impl.csv


	<Serializable>
	Public Class SerializableCSVParser

		Private ReadOnly separator As Char

		Private ReadOnly quotechar As Char

		Private ReadOnly escape As Char

		Private ReadOnly strictQuotes As Boolean

'JAVA TO VB CONVERTER NOTE: The field pending was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private pending_Conflict As String
		Private inField As Boolean = False

		Private ReadOnly ignoreLeadingWhiteSpace As Boolean

		''' <summary>
		''' The default separator to use if none is supplied to the constructor.
		''' </summary>
		Public Const DEFAULT_SEPARATOR As Char = ","c

		Public Const INITIAL_READ_SIZE As Integer = 128

		''' <summary>
		''' The default quote character to use if none is supplied to the
		''' constructor.
		''' </summary>
		Public Shared ReadOnly DEFAULT_QUOTE_CHARACTER As Char = """"c


		''' <summary>
		''' The default escape character to use if none is supplied to the
		''' constructor.
		''' </summary>
		Public Const DEFAULT_ESCAPE_CHARACTER As Char = "\"c

		''' <summary>
		''' The default strict quote behavior to use if none is supplied to the
		''' constructor
		''' </summary>
		Public Const DEFAULT_STRICT_QUOTES As Boolean = False

		''' <summary>
		''' The default leading whitespace behavior to use if none is supplied to the
		''' constructor
		''' </summary>
		Public Const DEFAULT_IGNORE_LEADING_WHITESPACE As Boolean = True

		''' <summary>
		''' This is the "null" character - if a value is set to this then it is ignored.
		''' I.E. if the quote character is set to null then there is no quote character.
		''' </summary>
		Public Shared ReadOnly NULL_CHARACTER As Char = ControlChars.NullChar

		''' <summary>
		''' Constructs CSVParser using a comma for the separator.
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_SEPARATOR, DEFAULT_QUOTE_CHARACTER, DEFAULT_ESCAPE_CHARACTER)
		End Sub

		''' <summary>
		''' Constructs CSVParser with supplied separator.
		''' </summary>
		''' <param name="separator"> the delimiter to use for separating entries. </param>
		Public Sub New(ByVal separator As Char)
			Me.New(separator, DEFAULT_QUOTE_CHARACTER, DEFAULT_ESCAPE_CHARACTER)
		End Sub


		''' <summary>
		''' Constructs CSVParser with supplied separator and quote char.
		''' </summary>
		''' <param name="separator"> the delimiter to use for separating entries </param>
		''' <param name="quotechar"> the character to use for quoted elements </param>
		Public Sub New(ByVal separator As Char, ByVal quotechar As Char)
			Me.New(separator, quotechar, DEFAULT_ESCAPE_CHARACTER)
		End Sub

		''' <summary>
		''' Constructs CSVReader with supplied separator and quote char.
		''' </summary>
		''' <param name="separator"> the delimiter to use for separating entries </param>
		''' <param name="quotechar"> the character to use for quoted elements </param>
		''' <param name="escape">    the character to use for escaping a separator or quote </param>
		Public Sub New(ByVal separator As Char, ByVal quotechar As Char, ByVal escape As Char)
			Me.New(separator, quotechar, escape, DEFAULT_STRICT_QUOTES)
		End Sub

		''' <summary>
		''' Constructs CSVReader with supplied separator and quote char.
		''' Allows setting the "strict quotes" flag
		''' </summary>
		''' <param name="separator">    the delimiter to use for separating entries </param>
		''' <param name="quotechar">    the character to use for quoted elements </param>
		''' <param name="escape">       the character to use for escaping a separator or quote </param>
		''' <param name="strictQuotes"> if true, characters outside the quotes are ignored </param>
		Public Sub New(ByVal separator As Char, ByVal quotechar As Char, ByVal escape As Char, ByVal strictQuotes As Boolean)
			Me.New(separator, quotechar, escape, strictQuotes, DEFAULT_IGNORE_LEADING_WHITESPACE)
		End Sub

		''' <summary>
		''' Constructs CSVReader with supplied separator and quote char.
		''' Allows setting the "strict quotes" and "ignore leading whitespace" flags
		''' </summary>
		''' <param name="separator">               the delimiter to use for separating entries </param>
		''' <param name="quotechar">               the character to use for quoted elements </param>
		''' <param name="escape">                  the character to use for escaping a separator or quote </param>
		''' <param name="strictQuotes">            if true, characters outside the quotes are ignored </param>
		''' <param name="ignoreLeadingWhiteSpace"> if true, white space in front of a quote in a field is ignored </param>
		Public Sub New(ByVal separator As Char, ByVal quotechar As Char, ByVal escape As Char, ByVal strictQuotes As Boolean, ByVal ignoreLeadingWhiteSpace As Boolean)
			If anyCharactersAreTheSame(separator, quotechar, escape) Then
				Throw New System.NotSupportedException("The separator, quote, and escape characters must be different!")
			End If
			If separator = NULL_CHARACTER Then
				Throw New System.NotSupportedException("The separator character must be defined!")
			End If
			Me.separator = separator
			Me.quotechar = quotechar
			Me.escape = escape
			Me.strictQuotes = strictQuotes
			Me.ignoreLeadingWhiteSpace = ignoreLeadingWhiteSpace
		End Sub

		Private Function anyCharactersAreTheSame(ByVal separator As Char, ByVal quotechar As Char, ByVal escape As Char) As Boolean
			Return isSameCharacter(separator, quotechar) OrElse isSameCharacter(separator, escape) OrElse isSameCharacter(quotechar, escape)
		End Function

		Private Function isSameCharacter(ByVal c1 As Char, ByVal c2 As Char) As Boolean
			Return c1 <> NULL_CHARACTER AndAlso c1 = c2
		End Function

		''' <returns> true if something was left over from last call(s) </returns>
		Public Overridable ReadOnly Property Pending As Boolean
			Get
				Return pending_Conflict IsNot Nothing
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String[] parseLineMulti(String nextLine) throws java.io.IOException
		Public Overridable Function parseLineMulti(ByVal nextLine As String) As String()
			Return parseLine(nextLine, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String[] parseLine(String nextLine) throws java.io.IOException
		Public Overridable Function parseLine(ByVal nextLine As String) As String()
			Return parseLine(nextLine, False)
		End Function

		''' <summary>
		''' Parses an incoming String and returns an array of elements.
		''' </summary>
		''' <param name="nextLine"> the string to parse </param>
		''' <param name="multi"> </param>
		''' <returns> the comma-tokenized list of elements, or null if nextLine is null </returns>
		''' <exception cref="IOException"> if bad things happen during the read </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private String[] parseLine(String nextLine, boolean multi) throws java.io.IOException
		Private Function parseLine(ByVal nextLine As String, ByVal multi As Boolean) As String()

			If Not multi AndAlso pending_Conflict IsNot Nothing Then
				pending_Conflict = Nothing
			End If

			If nextLine Is Nothing Then
				If pending_Conflict IsNot Nothing Then
					Dim s As String = pending_Conflict
					pending_Conflict = Nothing
					Return New String(){s}
				Else
					Return Nothing
				End If
			End If

			Dim tokensOnThisLine As IList(Of String) = New List(Of String)()
			Dim sb As New StringBuilder(INITIAL_READ_SIZE)
			Dim inQuotes As Boolean = False
			If pending_Conflict IsNot Nothing Then
				sb.Append(pending_Conflict)
				pending_Conflict = Nothing
				inQuotes = True
			End If
			Dim i As Integer = 0
			Do While i < nextLine.Length

				Dim c As Char = nextLine.Chars(i)
				If c = Me.escape Then
					If isNextCharacterEscapable(nextLine, inQuotes OrElse inField, i) Then
						sb.Append(nextLine.Chars(i + 1))
						i += 1
					End If
				ElseIf c = quotechar Then
					If isNextCharacterEscapedQuote(nextLine, inQuotes OrElse inField, i) Then
						sb.Append(nextLine.Chars(i + 1))
						i += 1
					Else
						'inQuotes = !inQuotes;

						' the tricky case of an embedded quote in the middle: a,bc"d"ef,g
						If Not strictQuotes Then
							If i > 2 AndAlso nextLine.Chars(i - 1) <> Me.separator AndAlso nextLine.Length > (i + 1) AndAlso nextLine.Chars(i + 1) <> Me.separator Then

								If ignoreLeadingWhiteSpace AndAlso sb.Length > 0 AndAlso isAllWhiteSpace(sb) Then
									sb.Length = 0 'discard white space leading up to quote
								Else
									sb.Append(c)
									'continue;
								End If

							End If
						End If

						inQuotes = Not inQuotes
					End If
					inField = Not inField
				ElseIf c = separator AndAlso Not inQuotes Then
					tokensOnThisLine.Add(sb.ToString())
					sb.Length = 0 ' start work on next token
					inField = False
				Else
					If Not strictQuotes OrElse inQuotes Then
						sb.Append(c)
						inField = True
					End If
				End If
				i += 1
			Loop
			' line is done - check status
			If inQuotes Then
				If multi Then
					' continuing a quoted section, re-append newline
					sb.Append(vbLf)
					pending_Conflict = sb.ToString()
					sb = Nothing ' this partial content is not to be added to field list yet
				Else
					Throw New IOException("Un-terminated quoted field at end of CSV line")
				End If
			End If
			If sb IsNot Nothing Then
				tokensOnThisLine.Add(sb.ToString())
			End If
			Return CType(tokensOnThisLine, List(Of String)).ToArray()

		End Function

		''' <summary>
		''' precondition: the current character is a quote or an escape
		''' </summary>
		''' <param name="nextLine"> the current line </param>
		''' <param name="inQuotes"> true if the current context is quoted </param>
		''' <param name="i">        current index in line </param>
		''' <returns> true if the following character is a quote </returns>
		Private Function isNextCharacterEscapedQuote(ByVal nextLine As String, ByVal inQuotes As Boolean, ByVal i As Integer) As Boolean
			Return inQuotes AndAlso nextLine.Length > (i + 1) AndAlso nextLine.Chars(i + 1) = quotechar
		End Function

		''' <summary>
		''' precondition: the current character is an escape
		''' </summary>
		''' <param name="nextLine"> the current line </param>
		''' <param name="inQuotes"> true if the current context is quoted </param>
		''' <param name="i">        current index in line </param>
		''' <returns> true if the following character is a quote </returns>
		Protected Friend Overridable Function isNextCharacterEscapable(ByVal nextLine As String, ByVal inQuotes As Boolean, ByVal i As Integer) As Boolean
			Return inQuotes AndAlso nextLine.Length > (i + 1) AndAlso (nextLine.Chars(i + 1) = quotechar OrElse nextLine.Chars(i + 1) = Me.escape)
		End Function

		''' <summary>
		''' precondition: sb.length() > 0
		''' </summary>
		''' <param name="sb"> A sequence of characters to examine </param>
		''' <returns> true if every character in the sequence is whitespace </returns>
		Protected Friend Overridable Function isAllWhiteSpace(ByVal sb As CharSequence) As Boolean
			Dim result As Boolean = True
			For i As Integer = 0 To sb.length() - 1
				Dim c As Char = sb.charAt(i)

				If Not Char.IsWhiteSpace(c) Then
					Return False
				End If
			Next i
			Return result
		End Function
	End Class

End Namespace