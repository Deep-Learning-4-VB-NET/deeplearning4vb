Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports ND4JEnvironmentVars = DeepLearning4VB.org.nd4j.common.config.ND4JEnvironmentVars
Imports ND4JSystemProperties = DeepLearning4VB.org.nd4j.common.config.ND4JSystemProperties
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource
Imports Resolver = DeepLearning4VB.org.nd4j.common.resources.Resolver

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

    ''' <summary>
    ''' Resolves resources that are either real files or Strumpf reference files
    ''' (<c>&lt;name&gt;.resource_reference</c>), looking first in the directories named by the
    ''' <c>RESOURCES_LOCAL_DIRS</c> system property (comma separated) and then on the "class path"
    ''' (see <see cref="ClassPathResource"/>). Referenced files are downloaded into a local cache.
    ''' </summary>
    Public Class StrumpfResolver
        Implements Resolver

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(StrumpfResolver))
        Public Shared ReadOnly DEFAULT_CACHE_DIR As String = Path.Combine(JavaSystem.getProperty("user.home"), ".cache", "nd4j", "test_resources")
        Public Const REF As String = ".resource_reference"

        Protected Friend ReadOnly localResourceDirs As IList(Of String)
        Protected Friend ReadOnly cacheDir As String

        Public Sub New()
            Dim localDirs As String = JavaSystem.getProperty(ND4JSystemProperties.RESOURCES_LOCAL_DIRS, Nothing)
            If localDirs IsNot Nothing AndAlso localDirs.Length > 0 Then
                localResourceDirs = New List(Of String)(localDirs.Split(","c))
            Else
                localResourceDirs = Nothing
            End If

            Dim cd As String = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_RESOURCES_CACHE_DIR)
            If String.IsNullOrEmpty(cd) Then
                cd = JavaSystem.getProperty(ND4JSystemProperties.RESOURCES_CACHE_DIR, DEFAULT_CACHE_DIR)
            End If
            cacheDir = cd
            Directory.CreateDirectory(cacheDir)
        End Sub

        Public Overridable Function priority() As Integer Implements Resolver.priority
            Return 100
        End Function

        Public Overridable Function exists(ByVal resourcePath As String) As Boolean Implements Resolver.exists
            If resourcePath Is Nothing Then Throw New ArgumentNullException(NameOf(resourcePath))
            'First: check local dirs (if any exist)
            If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
                For Each s As String In localResourceDirs
                    'Check for standard file:
                    If File.Exists(Path.Combine(s, resourcePath)) Then
                        Return True
                    End If
                    'Check for reference file:
                    If File.Exists(Path.Combine(s, resourcePath & REF)) Then
                        Return True
                    End If
                Next s
            End If

            'Second: Check classpath
            Dim cpr As New ClassPathResource(resourcePath & REF)
            If cpr.exists() Then
                Return True
            End If
            cpr = New ClassPathResource(resourcePath)
            Return cpr.exists()
        End Function

        Public Overridable Function directoryExists(ByVal dirPath As String) As Boolean Implements Resolver.directoryExists
            'First: check local dirs (if any)
            If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
                For Each s As String In localResourceDirs
                    If Directory.Exists(Path.Combine(s, dirPath)) Then
                        Return True
                    End If
                Next s
            End If
            'Second: Check classpath
            Dim cpr As New ClassPathResource(dirPath)
            Return cpr.exists() AndAlso cpr.isDirectory()
        End Function

        Public Overridable Function asFile(ByVal resourcePath As String) As String Implements Resolver.asFile
            assertExists(resourcePath)

            If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
                For Each s As String In localResourceDirs
                    Dim f1 As String = Path.Combine(s, resourcePath)
                    If File.Exists(f1) Then
                        'OK - found actual file
                        Return Path.GetFullPath(f1)
                    End If
                    'Check for reference file:
                    Dim f2 As String = Path.Combine(s, resourcePath & REF)
                    If File.Exists(f2) Then
                        'OK - found resource reference. Need to download to local cache... and/or validate what we have in cache
                        ' (the Java code passed the directory instead of the reference file here, which could never work)
                        Dim rf As ResourceFile = ResourceFile.fromFile(f2)
                        Return rf.localFile(cacheDir)
                    End If
                Next s
            End If

            'Second: Check classpath for references (and actual file)
            Dim cpr As New ClassPathResource(resourcePath & REF)
            If cpr.exists() Then
                Dim rf As ResourceFile = ResourceFile.fromFile(cpr.File)
                Return rf.localFile(cacheDir)
            End If

            cpr = New ClassPathResource(resourcePath)
            If cpr.exists() Then
                Return cpr.File
            End If

            Throw New InvalidOperationException("Could not find resource file that should exist: " & resourcePath)
        End Function

        Public Overridable Function asStream(ByVal resourcePath As String) As Stream Implements Resolver.asStream
            Dim f As String = asFile(resourcePath)
            log.debug("Resolved resource {} as file at absolute path {}", resourcePath, f)
            Try
                Return New BufferedStream(New FileStream(f, FileMode.Open, FileAccess.Read, FileShare.Read))
            Catch e As FileNotFoundException
                Throw New InvalidOperationException("Error reading file for resource: """ & resourcePath & """ resolved to """ & f & """", e)
            End Try
        End Function

        Public Overridable Sub copyDirectory(ByVal dirPath As String, ByVal destinationDir As String) Implements Resolver.copyDirectory
            Directory.CreateDirectory(destinationDir)
            'First: check local resource dir
            Dim resolved As Boolean = False
            If localResourceDirs IsNot Nothing AndAlso localResourceDirs.Count > 0 Then
                For Each s As String In localResourceDirs
                    Dim f1 As String = Path.Combine(s, dirPath)
                    If Directory.Exists(f1) Then
                        ClassPathResource.CopyDirectoryRecursive(f1, destinationDir)
                        resolved = True
                        Exit For
                    End If
                Next s
            End If

            'Second: Check classpath
            If Not resolved Then
                Dim cpr As New ClassPathResource(dirPath)
                If cpr.exists() Then
                    cpr.copyDirectory(destinationDir)
                    resolved = True
                End If
            End If

            If Not resolved Then
                Throw New InvalidOperationException("Unable to find resource directory for path: " & dirPath)
            End If

            'Finally, scan directory (recursively) and replace any resource reference files with actual files...
            For Each p As String In Directory.GetFiles(destinationDir, "*" & REF, SearchOption.AllDirectories)
                Dim localFile As String = ResourceFile.fromFile(p).localFile(cacheDir)
                Dim destination As String = p.Substring(0, p.Length - REF.Length)
                File.Copy(localFile, destination, True)
                File.Delete(p)
            Next p
        End Sub

        Public Overridable Function hasLocalCache() As Boolean Implements Resolver.hasLocalCache
            Return True
        End Function

        Public Overridable Function localCacheRoot() As String Implements Resolver.localCacheRoot
            Return cacheDir
        End Function

        Public Overridable Function normalizePath(ByVal path As String) As String Implements Resolver.normalizePath
            If path Is Nothing Then Throw New ArgumentNullException(NameOf(path))
            If path.EndsWith(REF, StringComparison.Ordinal) Then
                Return path.Substring(0, path.Length - REF.Length)
            End If
            Return path
        End Function

        Protected Friend Overridable Sub assertExists(ByVal resourcePath As String)
            If Not exists(resourcePath) Then
                Throw New InvalidOperationException("Could not find resource with path """ & resourcePath & """ in local directories (" & If(localResourceDirs Is Nothing, "null", "[" & String.Join(", ", localResourceDirs) & "]") & ") or in classpath")
            End If
        End Sub

    End Class

End Namespace
