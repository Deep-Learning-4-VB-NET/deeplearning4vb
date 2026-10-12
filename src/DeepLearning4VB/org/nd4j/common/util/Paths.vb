Imports System
Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils

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

Namespace org.nd4j.common.util


	''' <summary>
	''' Path Utilities
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class Paths

		Public Const PATH_ENV_VARIABLE As String = "PATH"

		Private Sub New()
		End Sub

		''' <summary>
		''' Check if a file exists in the path </summary>
		''' <param name="name"> the name of the file </param>
		''' <returns> true if the name exists
		''' false otherwise </returns>
		Public Shared Function nameExistsInPath(ByVal name As String) As Boolean
			Dim path As String = Environment.GetEnvironmentVariable(PATH_ENV_VARIABLE)
			Dim dirs() As String = path.Split(File.pathSeparator, True)
			For Each dir As String In dirs
				Dim dirFile As New File(dir)
				If Not dirFile.exists() Then
					Continue For
				End If

				If dirFile.isFile() AndAlso dirFile.getName().Equals(name) Then
					Return True
				Else
					Dim files As IEnumerator(Of File) = FileUtils.iterateFiles(dirFile, Nothing, False)
					Do While files.MoveNext()
						Dim curr As File = files.Current
						If curr.getName().Equals(name) Then
							Return True
						End If
					Loop

				End If
			Next dir

			Return False
		End Function


	End Class

End Namespace