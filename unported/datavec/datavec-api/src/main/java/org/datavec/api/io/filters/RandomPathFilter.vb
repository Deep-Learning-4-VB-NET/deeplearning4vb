Imports System
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

Namespace org.datavec.api.io.filters


	''' <summary>
	''' Randomizes the order of paths in an array.
	''' 
	''' @author saudet
	''' </summary>
	Public Class RandomPathFilter
		Implements PathFilter

		Protected Friend random As Random
		Protected Friend extensions() As String
		Protected Friend maxPaths As Long = 0

		''' <summary>
		''' Calls {@code this(random, extensions, 0)}. </summary>
		Public Sub New(ByVal random As Random, ParamArray ByVal extensions() As String)
			Me.New(random, extensions, 0)
		End Sub

		''' <summary>
		''' Constructs an instance of the PathFilter.
		''' </summary>
		''' <param name="random">     object to use </param>
		''' <param name="extensions"> of files to keep </param>
		''' <param name="maxPaths">   max number of paths to return (0 == unlimited) </param>
		Public Sub New(ByVal random As Random, ByVal extensions() As String, ByVal maxPaths As Long)
			Me.random = random
			Me.extensions = extensions
			Me.maxPaths = maxPaths
		End Sub

		Protected Friend Overridable Function accept(ByVal name As String) As Boolean
			If extensions Is Nothing OrElse extensions.Length = 0 Then
				Return True
			End If
			For Each extension As String In extensions
				If name.EndsWith("." & extension, StringComparison.Ordinal) Then
					Return True
				End If
			Next extension
			Return False
		End Function

		Public Overridable Function filter(ByVal paths() As URI) As URI() Implements PathFilter.filter
			' shuffle before to avoid sampling bias
			Dim paths2 As New List(Of URI) From {paths}
			Collections.shuffle(paths2, random)

			Dim newpaths As New List(Of URI)()
			For Each path As URI In paths2
				If accept(path.ToString()) Then
					newpaths.Add(path)
				End If
				If maxPaths > 0 AndAlso newpaths.Count >= maxPaths Then
					Exit For
				End If
			Next path
			Return newpaths.ToArray()
		End Function
	End Class

End Namespace