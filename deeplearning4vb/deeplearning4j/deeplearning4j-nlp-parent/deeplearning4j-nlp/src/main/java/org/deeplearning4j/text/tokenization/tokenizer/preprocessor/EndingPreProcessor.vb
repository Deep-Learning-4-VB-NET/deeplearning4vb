Imports System
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

	''' <summary>
	''' Gets rid of endings:
	''' 
	'''    ed,ing, ly, s, .
	''' @author Adam Gibson
	''' </summary>
	Public Class EndingPreProcessor
		Implements TokenPreProcess

		Public Overridable Function preProcess(ByVal token As String) As String Implements TokenPreProcess.preProcess
			If token.EndsWith("s", StringComparison.Ordinal) AndAlso Not token.EndsWith("ss", StringComparison.Ordinal) Then
				token = token.Substring(0, token.Length - 1)
			End If
			If token.EndsWith(".", StringComparison.Ordinal) Then
				token = token.Substring(0, token.Length - 1)
			End If
			If token.EndsWith("ed", StringComparison.Ordinal) Then
				token = token.Substring(0, token.Length - 2)
			End If
			If token.EndsWith("ing", StringComparison.Ordinal) Then
				token = token.Substring(0, token.Length - 3)
			End If
			If token.EndsWith("ly", StringComparison.Ordinal) Then
				token = token.Substring(0, token.Length - 2)
			End If
			Return token
		End Function
	End Class

End Namespace