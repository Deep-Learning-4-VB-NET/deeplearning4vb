Imports System.Collections.Generic

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
	''' Default tokenizer
	''' @author Adam Gibson
	''' </summary>
	Public Class DefaultTokenizer
		Implements Tokenizer

		Public Sub New(ByVal tokens As String)
			tokenizer = New StringTokenizer(tokens)
		End Sub

		Private tokenizer As StringTokenizer
		Private tokenPreProcess As TokenPreProcess

		Public Overridable Function hasMoreTokens() As Boolean Implements Tokenizer.hasMoreTokens
			Return tokenizer.hasMoreTokens()
		End Function

		Public Overridable Function countTokens() As Integer Implements Tokenizer.countTokens
			Return tokenizer.countTokens()
		End Function

		Public Overridable Function nextToken() As String Implements Tokenizer.nextToken
			Dim base As String = tokenizer.nextToken()
			If tokenPreProcess IsNot Nothing Then
				base = tokenPreProcess.preProcess(base)
			End If
			Return base
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
				Me.tokenPreProcess = tokenPreProcessor
    
			End Set
		End Property



	End Class

End Namespace