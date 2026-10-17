Imports System
Imports System.IO
Imports ND4VBSystemProperties = DeepLearning4VB.org.nd4vb.common.config.ND4VBSystemProperties

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

    ''' <summary>
    ''' Temporary file helpers that honour the ND4VB temporary-directory system property.
    ''' Paths are returned as strings (the Java original returned <c>java.io.File</c>).
    ''' </summary>
    Public Class ND4VBFileUtils

        Private Shared ReadOnly random As New Random()

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Creates a new, empty temporary file in the directory given by the
        ''' <see cref="ND4VBSystemProperties.ND4VB_TEMP_DIR_PROPERTY"/> system property if set,
        ''' or the default temporary directory otherwise.
        ''' </summary>
        ''' <param name="prefix">Prefix for the file name.</param>
        ''' <param name="suffix">Suffix for the file name; ".tmp" is used when Nothing.</param>
        ''' <returns>The full path of the created file.</returns>
        Public Shared Function createTempFile(ByVal prefix As String, ByVal suffix As String) As String
            Return CreateUniqueTempFile(TempDir, prefix, suffix)
        End Function

        ''' <summary>Creates a new, empty, uniquely named file in <paramref name="dir"/> (shared with DL4VBFileUtils).</summary>
        Friend Shared Function CreateUniqueTempFile(ByVal dir As String, ByVal prefix As String, ByVal suffix As String) As String
            Directory.CreateDirectory(dir)
            If suffix Is Nothing Then
                suffix = ".tmp"
            End If
            If prefix Is Nothing Then
                prefix = ""
            End If
            Try
                For attempt As Integer = 0 To 99
                    Dim n As Long
                    SyncLock random
                        n = CLng(random.NextDouble() * Long.MaxValue)
                    End SyncLock
                    Dim candidate As String = Path.Combine(dir, prefix & n.ToString() & suffix)
                    If File.Exists(candidate) Then
                        Continue For
                    End If
                    Using New FileStream(candidate, FileMode.CreateNew, FileAccess.Write)
                    End Using
                    Return candidate
                Next attempt
                Throw New IOException("Could not create a unique temporary file in " & dir)
            Catch e As IOException
                Throw New InvalidOperationException("Error creating temporary file", e)
            End Try
        End Function

        ''' <summary>
        ''' The temporary directory: the <see cref="ND4VBSystemProperties.ND4VB_TEMP_DIR_PROPERTY"/> system
        ''' property if set, otherwise the system temporary directory.
        ''' </summary>
        Public Shared ReadOnly Property TempDir As String
            Get
                Dim p As String = JavaSystem.getProperty(ND4VBSystemProperties.ND4VB_TEMP_DIR_PROPERTY)
                If p Is Nothing OrElse p.Length = 0 Then
                    Return Path.GetTempPath()
                End If
                Return p
            End Get
        End Property

        Public Shared Function getTempDir() As String
            Return TempDir
        End Function

    End Class

End Namespace
