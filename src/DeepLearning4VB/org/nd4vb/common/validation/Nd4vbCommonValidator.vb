Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Text.Json

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

Namespace org.nd4vb.common.validation

    ''' <summary>Validation helpers for files, JSON documents and zip archives. Files are path strings.</summary>
    Public NotInheritable Class Nd4vbCommonValidator

        Private Sub New()
        End Sub

        Public Shared Function isValidFile(ByVal f As String) As ValidationResult
            Dim vr As ValidationResult = isValidFile(f, "File", False)
            If vr IsNot Nothing Then
                Return vr
            End If
            Return ValidationResult.builder().valid(True).formatType("File").path(getPath(f)).build()
        End Function

        ''' <summary>Returns a failed result if the path is missing, a directory, or (unless allowed) empty; Nothing if it is fine.</summary>
        Public Shared Function isValidFile(ByVal f As String, ByVal formatType As String, ByVal allowEmpty As Boolean) As ValidationResult
            If f Is Nothing Then Throw New ArgumentNullException(NameOf(f))
            Dim path As String = getPath(f)

            If Directory.Exists(f) Then
                Return ValidationResult.builder().valid(False).formatType(formatType).path(path).issues("Specified path is a directory").build()
            End If

            If Not File.Exists(f) Then
                Return ValidationResult.builder().valid(False).formatType(formatType).path(path).issues("File does not exist").build()
            End If

            If Not allowEmpty AndAlso New FileInfo(f).Length <= 0 Then
                Return ValidationResult.builder().valid(False).formatType(formatType).path(path).issues("File is empty (length 0)").build()
            End If

            Return Nothing 'OK
        End Function

        Public Shared Function isValidJsonUTF8(ByVal f As String) As ValidationResult
            Return isValidJson(f, Encoding.UTF8)
        End Function

        Public Shared Function isValidJson(ByVal f As String, ByVal charset As Encoding) As ValidationResult
            Dim vr As ValidationResult = isValidFile(f, "JSON", False)
            If vr IsNot Nothing Then
                Return vr
            End If

            Dim content As String
            Try
                content = File.ReadAllText(f, If(charset, Encoding.UTF8))
            Catch e As IOException
                Return ValidationResult.builder().valid(False).formatType("JSON").path(getPath(f)).issues("Unable to read file (IOException)").exception(e).build()
            End Try

            Return isValidJson(content, f)
        End Function

        Public Shared Function isValidJSON(ByVal s As String) As ValidationResult
            Return isValidJson(s, DirectCast(Nothing, String))
        End Function

        ''' <summary>Valid when the content parses as a JSON object (the Java code deserialised it as a Map).</summary>
        Protected Friend Shared Function isValidJson(ByVal content As String, ByVal f As String) As ValidationResult
            Try
                Using doc As JsonDocument = JsonDocument.Parse(content)
                    If doc.RootElement.ValueKind <> JsonValueKind.Object Then
                        Throw New JsonException("Root element is not a JSON object but " & doc.RootElement.ValueKind.ToString())
                    End If
                End Using
            Catch t As Exception
                Return ValidationResult.builder().valid(False).formatType("JSON").path(getPath(f)).issues("File does not appear to be valid JSON").exception(t).build()
            End Try
            Return ValidationResult.builder().valid(True).formatType("JSON").path(getPath(f)).build()
        End Function

        Public Shared Function isValidZipFile(ByVal f As String, ByVal allowEmpty As Boolean) As ValidationResult
            Return isValidZipFile(f, allowEmpty, DirectCast(Nothing, IList(Of String)))
        End Function

        Public Shared Function isValidZipFile(ByVal f As String, ByVal allowEmpty As Boolean, ByVal ParamArray requiredEntries() As String) As ValidationResult
            Return isValidZipFile(f, allowEmpty, If(requiredEntries Is Nothing, Nothing, DirectCast(New List(Of String)(requiredEntries), IList(Of String))))
        End Function

        Public Shared Function isValidZipFile(ByVal f As String, ByVal allowEmpty As Boolean, ByVal requiredEntries As IList(Of String)) As ValidationResult
            Dim vr As ValidationResult = isValidFile(f, "Zip File", False)
            If vr IsNot Nothing Then
                Return vr
            End If

            Dim zf As ZipArchive
            Try
                zf = ZipFile.OpenRead(f)
            Catch e As Exception
                Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues("File does not appear to be valid zip file (not a zip file or content is corrupt)").exception(e).build()
            End Try

            Try
                Dim numEntries As Integer = zf.Entries.Count
                If Not allowEmpty AndAlso numEntries <= 0 Then
                    Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues("Zip file is empty").build()
                End If

                If requiredEntries IsNot Nothing AndAlso requiredEntries.Count > 0 Then
                    Dim missing As IList(Of String) = Nothing
                    For Each s As String In requiredEntries
                        If zf.GetEntry(s) Is Nothing Then
                            If missing Is Nothing Then
                                missing = New List(Of String)()
                            End If
                            missing.Add(s)
                        End If
                    Next s

                    If missing IsNot Nothing Then
                        Dim msg As String = "Zip file is missing " & missing.Count & " of " & requiredEntries.Count & " required entries: [" & String.Join(", ", missing) & "]"
                        Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues(msg).build()
                    End If
                End If
            Catch t As Exception
                Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues("Error reading zip file").exception(t).build()
            Finally
                zf.Dispose()
            End Try

            Return ValidationResult.builder().valid(True).formatType("Zip File").path(getPath(f)).build()
        End Function

        ''' <summary>The absolute path, or the path as given if it cannot be resolved.</summary>
        Public Shared Function getPath(ByVal f As String) As String
            If f Is Nothing Then
                Return Nothing
            End If
            Try
                Return Path.GetFullPath(f)
            Catch t As Exception
                Return f
            End Try
        End Function

    End Class

End Namespace
