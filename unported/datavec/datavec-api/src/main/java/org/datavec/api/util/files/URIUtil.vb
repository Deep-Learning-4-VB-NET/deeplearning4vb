Imports System
Imports System.IO

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

Namespace org.datavec.api.util.files


	Public Class URIUtil

		Public Shared Function fileToURI(ByVal f As File) As URI
			Try
				' manually construct URI (this is faster)
				Dim sp As String = slashify(f.getAbsoluteFile().getPath(), False)
				If Not sp.StartsWith("//", StringComparison.Ordinal) Then
					sp = "//" & sp
				End If
				Return New URI("file", Nothing, sp, Nothing)

			Catch x As URISyntaxException
				Throw New Exception(x) ' Can't happen
			End Try
		End Function

		Private Shared Function slashify(ByVal path As String, ByVal isDirectory As Boolean) As String
			Dim p As String = path
			If Path.DirectorySeparatorChar <> "/"c Then
				p = p.Replace(Path.DirectorySeparatorChar, "/"c)
			End If
			If Not p.StartsWith("/", StringComparison.Ordinal) Then
				p = "/" & p
			End If
			If Not p.EndsWith("/", StringComparison.Ordinal) AndAlso isDirectory Then
				p = p & "/"
			End If
			Return p
		End Function
	End Class

End Namespace