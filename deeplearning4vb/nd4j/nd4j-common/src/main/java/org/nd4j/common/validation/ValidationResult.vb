Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils

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

Namespace org.nd4j.common.validation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Builder @Data public class ValidationResult implements java.io.Serializable
	<Serializable>
	Public Class ValidationResult

		Private formatType As String 'Human readable format/model type
		Private formatClass As Type 'Actual class the format/model is (or should be)
		Private path As String 'Path of file (if applicable)
		Private valid As Boolean 'Whether the file/model is valid
		Private issues As IList(Of String) 'List of issues (generally only present if not valid)
		Private exception As Exception 'Exception, if applicable



		Public Overrides Function ToString() As String
			Dim lines As IList(Of String) = New List(Of String)()
			If formatType IsNot Nothing Then
				lines.Add("Format type: " & formatType)
			End If
			If formatClass IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				lines.Add("Format class: " & formatClass.FullName)
			End If
			If path IsNot Nothing Then
				lines.Add("Path: " & path)
			End If
			lines.Add("Format valid: " & valid)
			If issues IsNot Nothing AndAlso issues.Count > 0 Then
				If issues.Count = 1 Then
					addWithIndent(issues(0), lines, "Issue: ", "       ")
				Else
					lines.Add("Issues:")
					For Each s As String In issues
						addWithIndent(s, lines, "- ", "  ")
					Next s
				End If
			End If
			If exception IsNot Nothing Then
				Dim ex As String = ExceptionUtils.getStackTrace(exception)
				lines.Add("Stack Trace:")
				addWithIndent(ex, lines, "  ", "  ")
			End If
			'Would use String.join but that's Java 8...
			Dim sb As New StringBuilder()
			Dim first As Boolean = True
			For Each s As String In lines
				If Not first Then
					sb.Append(vbLf)
				End If
				sb.Append(s)
				first = False
			Next s
			Return sb.ToString()
		End Function

		Protected Friend Shared Sub addWithIndent(ByVal toAdd As String, ByVal list As IList(Of String), ByVal firstLineIndent As String, ByVal laterLineIndent As String)
			Dim split() As String = toAdd.Split(vbLf, True)
			Dim first As Boolean = True
			For Each issueLine As String In split
				list.Add((If(first, firstLineIndent, laterLineIndent)) & issueLine)
				first = False
			Next issueLine
		End Sub

	End Class

End Namespace