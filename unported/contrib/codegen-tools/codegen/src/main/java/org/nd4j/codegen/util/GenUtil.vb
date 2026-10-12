Imports System.Text
Imports Microsoft.VisualBasic
Imports Op = org.nd4j.codegen.api.Op

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.codegen.util

	Public Class GenUtil

		Private Sub New()
		End Sub

		Public Shared Function ensureFirstIsCap(ByVal [in] As String) As String
			If Char.IsUpper([in].Chars(0)) Then
			   Return [in]
			End If

			Return Char.ToUpper([in].Chars(0)) & [in].Substring(1)
		End Function

		Public Shared Function ensureFirstIsNotCap(ByVal [in] As String) As String
			If Char.IsLower([in].Chars(0)) Then
				Return [in]
			End If

			Return Char.ToLower([in].Chars(0)) & [in].Substring(1)
		End Function

		Public Shared Function repeat(ByVal [in] As String, ByVal count As Integer) As String
			Dim sb As New StringBuilder()
			For i As Integer = 0 To count - 1
				sb.Append([in])
			Next i
			Return sb.ToString()
		End Function

		Public Shared Function addIndent(ByVal [in] As String, ByVal count As Integer) As String
			If [in] Is Nothing Then
				Return Nothing
			End If
			Dim lines() As String = [in].Split(vbLf, True)
			Dim [out] As New StringBuilder()
			Dim indent As String = repeat(" ", count)
			For Each s As String In lines
				[out].Append(indent).Append(s).Append(vbLf)
			Next s
			Return [out].ToString()
		End Function
	End Class

End Namespace