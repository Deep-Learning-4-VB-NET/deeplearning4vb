Imports System.Collections.Generic
Imports System.Text
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function

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

Namespace org.datavec.local.transforms.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class WritablesToStringFunction implements org.nd4j.common.function.@Function<java.util.List<org.datavec.api.writable.Writable>, String>
	Public Class WritablesToStringFunction
		Implements [Function](Of IList(Of Writable), String)

		Private ReadOnly delim As String
		Private ReadOnly quote As String

		Public Sub New(ByVal delim As String)
			Me.New(delim, Nothing)
		End Sub

		Public Overridable Function apply(ByVal c As IList(Of Writable)) As String

			Dim sb As New StringBuilder()
			append(c, sb, delim, quote)

			Return sb.ToString()
		End Function

		Public Shared Sub append(ByVal c As IList(Of Writable), ByVal sb As StringBuilder, ByVal delim As String, ByVal quote As String)
			Dim first As Boolean = True
			For Each w As Writable In c
				If Not first Then
					sb.Append(delim)
				End If
				Dim s As String = w.ToString()
				Dim needQuotes As Boolean = s.Contains(delim)
				If needQuotes AndAlso quote IsNot Nothing Then
					sb.Append(quote)
					s = s.Replace(quote, quote & quote)
				End If
				sb.Append(s)
				If needQuotes AndAlso quote IsNot Nothing Then
					sb.Append(quote)
				End If
				first = False
			Next w
		End Sub
	End Class

End Namespace