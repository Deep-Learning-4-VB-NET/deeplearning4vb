Imports System
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.data

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

	Public Class DelimitedEdgeLineProcessor
		Implements EdgeLineProcessor(Of String)

		Private ReadOnly delimiter As String
		Private ReadOnly skipLinesStartingWith() As String
		Private ReadOnly directed As Boolean

		Public Sub New(ByVal delimiter As String, ByVal directed As Boolean)
			Me.New(delimiter, directed, Nothing)
		End Sub

		Public Sub New(ByVal delimiter As String, ByVal directed As Boolean, ParamArray ByVal skipLinesStartingWith() As String)
			Me.delimiter = delimiter
			Me.skipLinesStartingWith = skipLinesStartingWith
			Me.directed = directed
		End Sub

		Public Overridable Function processLine(ByVal line As String) As Edge(Of String) Implements EdgeLineProcessor(Of String).processLine
			If skipLinesStartingWith IsNot Nothing Then
				For Each s As String In skipLinesStartingWith
					If line.StartsWith(s, StringComparison.Ordinal) Then
						Return Nothing
					End If
				Next s
			End If

			Dim split() As String = line.Split(delimiter, True)
			If split.Length <> 2 Then
				Throw New System.ArgumentException("Invalid line: expected format """ & 0 & delimiter & 1 & """; received """ & line & """")
			End If

			Dim from As Integer = Integer.Parse(split(0))
			Dim [to] As Integer = Integer.Parse(split(1))
			Dim edgeName As String = from + (If(directed, "->", "--")) + [to]
			Return New Edge(Of String)(from, [to], edgeName, directed)
		End Function
	End Class

End Namespace