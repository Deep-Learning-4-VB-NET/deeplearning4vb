Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resolver = org.nd4j.common.resources.Resolver

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

Namespace org.nd4j.common.resources.strumpf


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StrumpfResolver implements org.nd4j.common.resources.Resolver
	Public Class StrumpfResolver
		Implements Resolver

		Public Shared ReadOnly DEFAULT_CACHE_DIR As String = (New File(System.getProperty("user.home"), ".cache/nd4j/test_resources")).getAbsolutePath()
		Public Const REF As String = ".resource_reference"

		Protected Friend ReadOnly localResourceDirs As IList(Of String)
		Protected Friend ReadOnly cacheDir As File

		Public Sub New()

			Dim localDirs As String = System.getProperty(ND4JSystemProperties.RESOURCES_LOCAL_DIRS, Nothing)

			If localDirs IsNot Nothing AndAlso localDirs.Length > 0 Then
				Dim split() As String = localDirs.Split(",", True)
				localResourceDirs = New List(Of String) From {split}
			Else
				localResourceDirs = Nothing
			End If

			Dim cd As String = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_RESOURCES_CACHE_DIR)
			If cd Is Nothing OrElse cd.Length = 0 Then
				cd = System.getProperty(ND4JSystemProperties.RESOURCES_CACHE_DIR, DEFAULT_CACHE_DIR)
			End If
			cacheDir = New File(cd)
			cacheDir.mkdirs()
		End Sub

		Public Overridable Function priority() As Integer Implements Resolver.priority
			Return 100
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean exists(@NonNull String resourcePath)
		Public Overridable Function exists(ByVal resourcePath As String) As Boolean Implements Resolver.exists
			'First: check local dirs (if any exist)
			If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
				For Each s As String In localResourceDirs
					'Check for standard file:
					Dim f1 As New File(s, resourcePath)
					If f1.exists() AndAlso f1.isFile() Then
						'OK - found actual file
						Return True
					End If

					'Check for reference file:
					Dim f2 As New File(s, resourcePath & REF)
					If f2.exists() AndAlso f2.isFile() Then
						'OK - found resource reference
						Return False
					End If
				Next s
			End If

			'Second: Check classpath
			Dim cpr As New ClassPathResource(resourcePath & REF)
			If cpr.exists() Then
				Return True
			End If

			cpr = New ClassPathResource(resourcePath)
			If cpr.exists() Then
				Return True
			End If

			Return False
		End Function

		Public Overridable Function directoryExists(ByVal dirPath As String) As Boolean Implements Resolver.directoryExists
			'First: check local dirs (if any)
			If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
				For Each s As String In localResourceDirs
					Dim f1 As New File(s, dirPath)
					If f1.exists() AndAlso f1.isDirectory() Then
						'OK - found directory
						Return True
					End If
				Next s
			End If

			'Second: Check classpath
			Dim cpr As New ClassPathResource(dirPath)
			If cpr.exists() Then
				Return True
			End If

			Return False
		End Function

		Public Overridable Function asFile(ByVal resourcePath As String) As File
			assertExists(resourcePath)

			If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
				For Each s As String In localResourceDirs
					Dim f1 As New File(s, resourcePath)
					If f1.exists() AndAlso f1.isFile() Then
						'OK - found actual file
						Return f1
					End If

					'Check for reference file:
					Dim f2 As New File(s, resourcePath & REF)
					If f2.exists() AndAlso f2.isFile() Then
						'OK - found resource reference. Need to download to local cache... and/or validate what we have in cache
						Dim rf As ResourceFile = ResourceFile.fromFile(s)
						Return rf.localFile(cacheDir)
					End If
				Next s
			End If


			'Second: Check classpath for references (and actual file)
			Dim cpr As New ClassPathResource(resourcePath & REF)
			If cpr.exists() Then
				Dim rf As ResourceFile
				Try
					rf = ResourceFile.fromFile(cpr.File)
				Catch e As IOException
					Throw New Exception(e)
				End Try
				Return rf.localFile(cacheDir)
			End If

			cpr = New ClassPathResource(resourcePath)
			If cpr.exists() Then
				Try
					Return cpr.File
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End If

			Throw New Exception("Could not find resource file that should exist: " & resourcePath)
		End Function

		Public Overridable Function asStream(ByVal resourcePath As String) As Stream
			Dim f As File = asFile(resourcePath)
			log.debug("Resolved resource " & resourcePath & " as file at absolute path " & f.getAbsolutePath())
			Try
				Return New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
			Catch e As FileNotFoundException
				Throw New Exception("Error reading file for resource: """ & resourcePath & """ resolved to """ & f & """")
			End Try
		End Function

		Public Overridable Sub copyDirectory(ByVal dirPath As String, ByVal destinationDir As File)
			'First: check local resource dir
			Dim resolved As Boolean = False
			If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
				For Each s As String In localResourceDirs
					Dim f1 As New File(s, dirPath)
					Try
						FileUtils.copyDirectory(f1, destinationDir)
						resolved = True
						Exit For
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Next s
			End If

			'Second: Check classpath
			If Not resolved Then
				Dim cpr As New ClassPathResource(dirPath)
				If cpr.exists() Then
					Try
						cpr.copyDirectory(destinationDir)
						resolved = True
					Catch e As IOException
						Throw New Exception(e)
					End Try
				End If
			End If

			If Not resolved Then
				Throw New Exception("Unable to find resource directory for path: " & dirPath)
			End If

			'Finally, scan directory (recursively) and replace any resource files with actual files...
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Path> toResolve = new java.util.ArrayList<>();
			Dim toResolve As IList(Of Path) = New List(Of Path)()
			Try
				Files.walkFileTree(destinationDir.toPath(), New SimpleFileVisitorAnonymousInnerClass(Me, toResolve))
			Catch e As IOException
				Throw New Exception(e)
			End Try

			If toResolve.Count > 0 Then
				For Each p As Path In toResolve
					Dim localFile As File = ResourceFile.fromFile(p.toFile()).localFile(cacheDir)
					Dim newPath As String = p.toFile().getAbsolutePath()
					newPath = newPath.Substring(0, newPath.Length - REF.Length)
					Dim destination As New File(newPath)
					Try
						FileUtils.copyFile(localFile, destination)
					Catch e As IOException
						Throw New Exception(e)
					End Try
					Try
						FileUtils.forceDelete(p.toFile())
					Catch e As IOException
						Throw New Exception("Error deleting temporary reference file", e)
					End Try
				Next p
			End If
		End Sub

		Private Class SimpleFileVisitorAnonymousInnerClass
			Inherits SimpleFileVisitor(Of Path)

			Private ReadOnly outerInstance As StrumpfResolver

			Private toResolve As IList(Of Path)

			Public Sub New(ByVal outerInstance As StrumpfResolver, ByVal toResolve As IList(Of Path))
				Me.outerInstance = outerInstance
				Me.toResolve = toResolve
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public FileVisitResult visitFile(Path file, java.nio.file.attribute.BasicFileAttributes attrs) throws IOException
			Public Overrides Function visitFile(ByVal file As Path, ByVal attrs As BasicFileAttributes) As FileVisitResult
				If file.ToString().EndsWith(REF, StringComparison.Ordinal) Then
					toResolve.Add(file)
				End If
				Return FileVisitResult.CONTINUE
			End Function
		End Class

		Public Overridable Function hasLocalCache() As Boolean Implements Resolver.hasLocalCache
			Return True
		End Function

		Public Overridable Function localCacheRoot() As File
			Return cacheDir
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public String normalizePath(@NonNull String path)
		Public Overridable Function normalizePath(ByVal path As String) As String Implements Resolver.normalizePath
			If path.EndsWith(REF, StringComparison.Ordinal) Then
				Return path.Substring(0, path.Length-REF.Length)
			End If
			Return path
		End Function


		Protected Friend Overridable Sub assertExists(ByVal resourcePath As String)
			If Not exists(resourcePath) Then
				Throw New System.InvalidOperationException("Could not find resource with path """ & resourcePath & """ in local directories (" & localResourceDirs & ") or in classpath")
			End If
		End Sub


	End Class

End Namespace