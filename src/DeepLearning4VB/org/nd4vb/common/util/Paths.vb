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

Namespace org.nd4vb.common.util

    ''' <summary>Path utilities.</summary>
    Public Class Paths

        Public Const PATH_ENV_VARIABLE As String = "PATH"

        Private Sub New()
        End Sub

        ''' <summary>Checks whether a file with the given name exists in one of the PATH directories.</summary>
        ''' <param name="name">The file name (with extension) to look for.</param>
        Public Shared Function nameExistsInPath(ByVal name As String) As Boolean
            If String.IsNullOrEmpty(name) Then
                Return False
            End If
            Dim pathVariable As String = Environment.GetEnvironmentVariable(PATH_ENV_VARIABLE)
            If String.IsNullOrEmpty(pathVariable) Then
                Return False
            End If
            For Each dir As String In pathVariable.Split(Path.PathSeparator)
                If dir.Length = 0 Then
                    Continue For
                End If
                Try
                    If File.Exists(dir) Then
                        If String.Equals(Path.GetFileName(dir), name, StringComparison.OrdinalIgnoreCase) Then
                            Return True
                        End If
                    ElseIf Directory.Exists(dir) Then
                        If File.Exists(Path.Combine(dir, name)) Then
                            Return True
                        End If
                    End If
                Catch ex As Exception When TypeOf ex Is ArgumentException OrElse TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
                    ' Malformed or inaccessible PATH entries are skipped, as in the Java original.
                End Try
            Next dir
            Return False
        End Function

    End Class

End Namespace
