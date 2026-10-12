Imports System
Imports System.Collections.Generic
Imports StringUtils = org.apache.commons.lang3.StringUtils

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
	''' @author sonali
	''' </summary>
	Public Class NGramTokenizer
		Implements Tokenizer

		Private tokens As IList(Of String)
		Private originalTokens As IList(Of String)
		Private index As Integer
		Private preProcess As TokenPreProcess
		Private tokenizer As Tokenizer

		Public Sub New(ByVal tokenizer As Tokenizer, ByVal minN As Integer?, ByVal maxN As Integer?)
			Me.tokens = New List(Of String)()
			Do While tokenizer.hasMoreTokens()
				Dim nextToken As String = tokenizer.nextToken()
				Me.tokens.Add(nextToken)
			Loop
			If maxN <> 1 Then
				Me.originalTokens = Me.tokens
				Me.tokens = New List(Of String)()
				Dim nOriginalTokens As Integer? = Me.originalTokens.Count
				Dim min As Integer? = Math.Min(maxN.Value + 1, nOriginalTokens.Value + 1)
				For i As Integer = minN To min.Value - 1
					Dim j As Integer = 0
					Do While j < nOriginalTokens.Value - i + 1
						Dim originalTokensSlice As IList(Of String) = Me.originalTokens.subList(j, j + i)
						Me.tokens.Add(StringUtils.join(originalTokensSlice, " "))
						j += 1
					Loop
				Next i
			End If
		End Sub

		Public Overridable Function hasMoreTokens() As Boolean Implements Tokenizer.hasMoreTokens
			Return index < tokens.Count
		End Function

		Public Overridable Function countTokens() As Integer Implements Tokenizer.countTokens
			Return tokens.Count
		End Function

		Public Overridable Function nextToken() As String Implements Tokenizer.nextToken
			Dim ret As String = tokens(index)
			index += 1
			Return ret
		End Function

		Public Overridable ReadOnly Property Tokens As IList(Of String) Implements Tokenizer.getTokens
			Get
				Dim tokens As IList(Of String) = New List(Of String)()
				Do While hasMoreTokens()
					tokens.Add(nextToken())
				Loop
				Return tokens
			End Get
		End Property

		Public Overridable WriteOnly Property TokenPreProcessor Implements Tokenizer.setTokenPreProcessor As TokenPreProcess
			Set(ByVal tokenPreProcessor As TokenPreProcess)
				Me.preProcess = tokenPreProcessor
			End Set
		End Property
	End Class

End Namespace