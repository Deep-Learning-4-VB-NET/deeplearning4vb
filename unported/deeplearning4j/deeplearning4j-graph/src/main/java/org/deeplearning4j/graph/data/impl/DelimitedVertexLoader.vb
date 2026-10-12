Imports System
Imports System.Collections.Generic
Imports System.IO
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.data
Imports ParseException = org.deeplearning4j.graph.exception.ParseException

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

Namespace org.deeplearning4j.graph.data.impl


	''' <summary>
	'''Load vertex information, one per line of form "0<delim>Some text attribute/label"
	''' </summary>
	Public Class DelimitedVertexLoader
		Implements VertexLoader(Of String)

		Private ReadOnly delimiter As String
		Private ReadOnly ignoreLinesPrefix() As String

		Public Sub New(ByVal delimiter As String)
			Me.New(delimiter, Nothing)
		End Sub

		Public Sub New(ByVal delimiter As String, ParamArray ByVal ignoreLinesPrefix() As String)
			Me.delimiter = delimiter
			Me.ignoreLinesPrefix = ignoreLinesPrefix
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.deeplearning4j.graph.api.Vertex<String>> loadVertices(String path) throws java.io.IOException
		Public Overridable Function loadVertices(ByVal path As String) As IList(Of Vertex(Of String)) Implements VertexLoader(Of String).loadVertices
			Dim vertices As IList(Of Vertex(Of String)) = New List(Of Vertex(Of String))()

			Dim lineCount As Integer = 0
			Using br As New StreamReader(New java.io.File(path))
				Dim line As String
				line = br.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = br.readLine()) != null)
				Do While line IsNot Nothing
					lineCount += 1
					If ignoreLinesPrefix IsNot Nothing Then
						Dim skipLine As Boolean = False
						For Each s As String In ignoreLinesPrefix
							If line.StartsWith(s, StringComparison.Ordinal) Then
								skipLine = True
								Exit For
							End If
						Next s
						If skipLine Then
							Continue Do
						End If
					End If

					Dim idx As Integer = line.IndexOf(delimiter, StringComparison.Ordinal)
					If idx = -1 Then
						Throw New ParseException("Error parsing line (could not find delimiter): " & line)
					End If

					Dim first As String = line.Substring(0, idx)
					Dim second As String = line.Substring(idx + 1)

					vertices.Add(New Vertex(Of String)(Integer.Parse(first), second))
						line = br.ReadLine()
				Loop
			End Using

			Return vertices
		End Function
	End Class

End Namespace