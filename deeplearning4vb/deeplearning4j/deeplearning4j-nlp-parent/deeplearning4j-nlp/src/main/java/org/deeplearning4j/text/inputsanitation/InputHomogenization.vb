Imports System.Collections.Generic
Imports System.Text

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

Namespace org.deeplearning4j.text.inputsanitation


	Public Class InputHomogenization
		Private input As String
		Private ignoreCharactersContaining As IList(Of String)
		Private preserveCase As Boolean

		''' <summary>
		''' Input text to applyTransformToOrigin </summary>
		''' <param name="input"> the input text to applyTransformToOrigin,
		''' equivalent to calling this(input,false)
		''' wrt preserving case </param>
		Public Sub New(ByVal input As String)
			Me.New(input, False)
		End Sub

		''' 
		''' <param name="input"> the input to applyTransformToOrigin </param>
		''' <param name="preserveCase"> whether to preserve case </param>
		Public Sub New(ByVal input As String, ByVal preserveCase As Boolean)
			Me.input = input
			Me.preserveCase = preserveCase
		End Sub

		''' 
		''' <param name="input"> the input to applyTransformToOrigin </param>
		''' <param name="ignoreCharactersContaining"> ignore transformation of words
		''' containigng specified strings </param>
		Public Sub New(ByVal input As String, ByVal ignoreCharactersContaining As IList(Of String))
			Me.input = input
			Me.ignoreCharactersContaining = ignoreCharactersContaining
		End Sub

		''' <summary>
		''' Returns the normalized text passed in via constructor </summary>
		''' <returns> the normalized text passed in via constructor </returns>
		Public Overridable Function transform() As String
			Dim sb As New StringBuilder()
			For i As Integer = 0 To input.Length - 1
				If ignoreCharactersContaining IsNot Nothing AndAlso ignoreCharactersContaining.Contains(input.Chars(i).ToString()) Then
					sb.Append(input.Chars(i))
				ElseIf Char.IsDigit(input.Chars(i)) Then
					sb.Append("d")
				ElseIf Char.IsUpper(input.Chars(i)) AndAlso Not preserveCase Then
					sb.Append(Char.ToLower(input.Chars(i)))
				Else
					sb.Append(input.Chars(i))
				End If

			Next i

			Dim normalized As String = Normalizer.normalize(sb.ToString(), Normalizer.Form.NFD)
			normalized = normalized.Replace(".", "")
			normalized = normalized.Replace(",", "")
			normalized = normalized.replaceAll("""", "")
			normalized = normalized.Replace("'", "")
			normalized = normalized.Replace("(", "")
			normalized = normalized.Replace(")", "")
			normalized = normalized.Replace("“", "")
			normalized = normalized.Replace("”", "")
			normalized = normalized.Replace("…", "")
			normalized = normalized.Replace("|", "")
			normalized = normalized.Replace("/", "")
			normalized = normalized.Replace("\", "")
			normalized = normalized.Replace("[", "")
			normalized = normalized.Replace("]", "")
			normalized = normalized.Replace("‘", "")
			normalized = normalized.Replace("’", "")
			normalized = normalized.replaceAll("[!]+", "!")
			Return normalized
		End Function

	End Class

End Namespace