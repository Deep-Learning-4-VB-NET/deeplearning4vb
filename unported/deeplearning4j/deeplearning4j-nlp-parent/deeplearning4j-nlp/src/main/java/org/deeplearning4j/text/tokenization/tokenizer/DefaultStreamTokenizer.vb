Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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



	''' <summary>
	''' Tokenizer based on the <seealso cref="java.io.StreamTokenizer"/>
	''' @author Adam Gibson
	''' 
	''' </summary>
	Public Class DefaultStreamTokenizer
		Implements Tokenizer

		Private streamTokenizer As StreamTokenizer
		Private tokenPreProcess As TokenPreProcess
		Private tokens As IList(Of String) = New List(Of String)()
		Private position As New AtomicInteger(0)

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DefaultStreamTokenizer))

		Public Sub New(ByVal [is] As Stream)
			Dim r As Reader = New StreamReader([is])
			streamTokenizer = New StreamTokenizer(r)

		End Sub

		''' <summary>
		''' Checks, if underlying stream has any tokens left
		''' 
		''' @return
		''' </summary>
		Private Function streamHasMoreTokens() As Boolean
			If streamTokenizer.ttype <> StreamTokenizer.TT_EOF Then
				Try
					streamTokenizer.nextToken()
				Catch e1 As IOException
					Throw New Exception(e1)
				End Try
			End If
			Return streamTokenizer.ttype <> StreamTokenizer.TT_EOF AndAlso streamTokenizer.ttype <> -1
		End Function

		''' <summary>
		''' Checks, if any prebuffered tokens left, otherswise checks underlying stream
		''' @return
		''' </summary>
		Public Overridable Function hasMoreTokens() As Boolean Implements Tokenizer.hasMoreTokens
			log.info("Tokens size: [" & tokens.Count & "], position: [" & position.get() & "]")
			If tokens.Count > 0 Then
				Return position.get() < tokens.Count
			Else
				Return streamHasMoreTokens()
			End If
		End Function

		''' <summary>
		''' Returns number of tokens
		''' PLEASE NOTE: this method effectively preloads all tokens. So use it with caution, since on large streams it will consume big amount of memory
		''' 
		''' @return
		''' </summary>
		Public Overridable Function countTokens() As Integer Implements Tokenizer.countTokens
			Return getTokens().Count
		End Function


		''' <summary>
		''' This method returns next token from prebuffered list of tokens or underlying InputStream
		''' </summary>
		''' <returns> next token as String </returns>
		Public Overridable Function nextToken() As String Implements Tokenizer.nextToken
			If tokens.Count > 0 AndAlso position.get() < tokens.Count Then
				Return tokens(position.getAndIncrement())
			End If
			Return nextTokenFromStream()
		End Function

		''' <summary>
		''' This method returns next token from underlying InputStream
		''' 
		''' @return
		''' </summary>
		Private Function nextTokenFromStream() As String
			Dim sb As New StringBuilder()


			If streamTokenizer.ttype = StreamTokenizer.TT_WORD Then
				sb.Append(streamTokenizer.sval)
			ElseIf streamTokenizer.ttype = StreamTokenizer.TT_NUMBER Then
				sb.Append(streamTokenizer.nval)
			ElseIf streamTokenizer.ttype = StreamTokenizer.TT_EOL Then
				Try
					Do While streamTokenizer.ttype = StreamTokenizer.TT_EOL
						streamTokenizer.nextToken()
					Loop
				Catch e As IOException
					Throw New Exception(e)
				End Try
			ElseIf streamHasMoreTokens() Then
				Return nextTokenFromStream()
			End If


			Dim ret As String = sb.ToString()

			If tokenPreProcess IsNot Nothing Then
				ret = tokenPreProcess.preProcess(ret)
			End If
			Return ret

		End Function

		''' <summary>
		''' Returns all tokens as list of Strings
		''' </summary>
		''' <returns> List of tokens </returns>
		Public Overridable ReadOnly Property Tokens As IList(Of String) Implements Tokenizer.getTokens
			Get
				'List<String> tokens = new ArrayList<>();
				If tokens.Count > 0 Then
					Return tokens
				End If
    
				log.info("Starting prebuffering...")
				Do While streamHasMoreTokens()
					tokens.Add(nextTokenFromStream())
				Loop
				log.info("Tokens prefetch finished. Tokens size: [" & tokens.Count & "]")
				Return tokens
			End Get
		End Property

		Public Overridable WriteOnly Property TokenPreProcessor Implements Tokenizer.setTokenPreProcessor As TokenPreProcess
			Set(ByVal tokenPreProcessor As TokenPreProcess)
				Me.tokenPreProcess = tokenPreProcessor
			End Set
		End Property

	End Class

End Namespace