Imports System
Imports System.Collections.Generic
Imports System.Text
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resources = org.nd4j.common.resources.Resources
Imports Resource = org.springframework.core.io.Resource
Imports PathMatchingResourcePatternResolver = org.springframework.core.io.support.PathMatchingResourcePatternResolver
Imports ResourcePatternResolver = org.springframework.core.io.support.ResourcePatternResolver

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
Namespace org.nd4j.common.tests


	Public Class ResourceUtils

		Private Sub New()
		End Sub

		''' <summary>
		''' List all classpath resource files, optionally recursively, inside the specified path/directory
		''' The path argument should be a directory.
		''' Returns the path of the resources, normalized by <seealso cref="Resources.normalize(String)"/>
		''' </summary>
		''' <param name="path">               Path in which to list all files </param>
		''' <param name="recursive">          If true: list all files in subdirectories also. If false: only include files in the specified
		'''                           directory, but not any files in subdirectories </param>
		''' <param name="includeDirectories"> If true: include any subdirectories in the returned list of files. False: Only return
		'''                           files, not directories </param>
		''' <param name="extensions">         Optional - may be null (or length 0). If null/length 0: files with any extension are returned
		'''                           If non-null: only files matching one of the specified extensions are included.
		'''                           Extensions can we specified with or without "." - i.e., "csv" and ".csv" are the same </param>
		''' <returns> List of files (and optionally directories) in the specified path </returns>
		Public Shared Function listClassPathFiles(ByVal path As String, ByVal recursive As Boolean, ByVal includeDirectories As Boolean, ParamArray ByVal extensions() As String) As IList(Of String)
			Try
				Return listClassPathFilesHelper(path, recursive, includeDirectories, extensions)
			Catch e As IOException
				Throw New Exception("Error listing class path files", e)
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static java.util.List<String> listClassPathFilesHelper(String path, boolean recursive, boolean includeDirectories, String... extensions) throws java.io.IOException
		Private Shared Function listClassPathFilesHelper(ByVal path As String, ByVal recursive As Boolean, ByVal includeDirectories As Boolean, ParamArray ByVal extensions() As String) As IList(Of String)
			Dim resolver As ResourcePatternResolver = New PathMatchingResourcePatternResolver((New ClassPathResource(path)).ClassLoader)

			Dim sbPattern As New StringBuilder("classpath*:" & path)
			If recursive Then
				sbPattern.Append("/**/*")
			Else
				sbPattern.Append("/*")
			End If

			'Normalize extensions so they are all like ".csv" etc - with leading "."
			Dim normExt() As String = Nothing
			If extensions IsNot Nothing AndAlso extensions.Length > 0 Then
				normExt = New String(extensions.Length - 1){}
				For i As Integer = 0 To extensions.Length - 1
					If Not extensions(i).StartsWith(".", StringComparison.Ordinal) Then
						normExt(i) = "." & extensions(i)
					Else
						normExt(i) = extensions(i)
					End If
				Next i
			End If

			Dim pattern As String = sbPattern.ToString()
			Dim resources() As Resource = resolver.getResources(pattern)
			Dim [out] As IList(Of String) = New List(Of String)(resources.Length)
			For Each r As Resource In resources
				Dim origPath As String = r.getURL().ToString()
				Dim idx As Integer = origPath.IndexOf(path, StringComparison.Ordinal)
				Dim relativePath As String = origPath.Substring(idx)
				Dim resourcePath As String = Resources.normalizePath(relativePath)


				If resourcePath.EndsWith("/", StringComparison.Ordinal) Then
					If includeDirectories Then
						[out].Add(resourcePath)
					Else
						Continue For 'Skip directory
					End If
				End If

				If normExt IsNot Nothing Then
					'Check if it matches any of the specified extensions
					Dim matches As Boolean = False
					For Each ext As String In normExt
						If resourcePath.EndsWith(ext, StringComparison.Ordinal) Then
							matches = True
							Exit For
						End If
					Next ext
					If matches Then
						[out].Add(resourcePath)
					End If
				Else
					'Include all files
					[out].Add(resourcePath)
				End If

			Next r
			Return [out]
		End Function
	End Class

End Namespace