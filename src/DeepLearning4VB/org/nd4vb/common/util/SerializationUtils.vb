Imports System
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

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
    ''' Object serialization helpers. Java object serialization is replaced by the
    ''' <see cref="BinaryFormatter"/> for <c>[Serializable]</c> types; byte arrays are <c>Byte()</c>.
    ''' Only deserialize data from trusted sources.
    ''' </summary>
    Public Class SerializationUtils

        Protected Sub New()
        End Sub

        Private Shared Function NewFormatter() As BinaryFormatter
            Return New BinaryFormatter()
        End Function

        ''' <summary>Reads a serialized object from a file.</summary>
        Public Shared Function readObject(Of T)(ByVal file As String) As T
            Try
                Using fs As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Return CType(NewFormatter().Deserialize(fs), T)
                End Using
            Catch e As Exception
                Throw New InvalidOperationException("Error reading object from file " & file, e)
            End Try
        End Function

        ''' <summary>Reads a serialized object from a stream (the stream is left open).</summary>
        Public Shared Function readObject(Of T)(ByVal [is] As Stream) As T
            Try
                Return CType(NewFormatter().Deserialize([is]), T)
            Catch e As Exception
                Throw New InvalidOperationException("Error reading object from stream", e)
            End Try
        End Function

        Public Shared Function toByteArray(ByVal toSave As Object) As Byte()
            Try
                Using bos As New MemoryStream()
                    NewFormatter().Serialize(bos, toSave)
                    Return bos.ToArray()
                End Using
            Catch e As Exception
                Throw New InvalidOperationException("Error serializing object", e)
            End Try
        End Function

        Public Shared Function fromByteArray(Of T)(ByVal bytes() As Byte) As T
            Using ms As New MemoryStream(bytes)
                Return readObject(Of T)(ms)
            End Using
        End Function

        Public Shared Function deserialize(Of T)(ByVal bytes() As Byte) As T
            Return fromByteArray(Of T)(bytes)
        End Function

        Public Shared Function deserialize(Of T)(ByVal [is] As Stream) As T
            Return readObject(Of T)([is])
        End Function

        Public Shared Sub writeObject(ByVal toSave As Object, ByVal writeTo As Stream)
            Try
                NewFormatter().Serialize(writeTo, toSave)
                writeTo.Flush()
            Catch e As Exception
                Throw New InvalidOperationException("Error serializing object", e)
            End Try
        End Sub

        Public Shared Sub serialize(ByVal [object] As Object, ByVal os As Stream)
            writeObject([object], os)
        End Sub

        Public Shared Sub saveObject(ByVal toSave As Object, ByVal saveTo As String)
            Try
                Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(saveTo))
                If Not String.IsNullOrEmpty(dir) Then Directory.CreateDirectory(dir)
                Using fs As New FileStream(saveTo, FileMode.Create, FileAccess.Write)
                    NewFormatter().Serialize(fs, toSave)
                End Using
            Catch e As Exception
                Throw New InvalidOperationException("Error saving object to file " & saveTo, e)
            End Try
        End Sub
    End Class

End Namespace
