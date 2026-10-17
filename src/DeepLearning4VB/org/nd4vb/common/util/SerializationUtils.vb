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

    ''' <summary>
    ''' Object serialization helpers: the .NET counterpart of Java object serialization
    ''' (<c>ObjectOutputStream</c>/<c>ObjectInputStream</c>) for <c>[Serializable]</c> types.
    ''' The format is DeepLearning4VB's own binary object-graph format (see <see cref="ObjectGraphSerializer"/>),
    ''' which works the same on .NET Framework 4.8.1 and .NET 10 and replaces the BinaryFormatter that
    ''' earlier versions used. Byte arrays are <c>Byte()</c>. Only deserialize data from trusted sources.
    ''' </summary>
    Public Class SerializationUtils

        Protected Sub New()
        End Sub

        ''' <summary>Reads a serialized object from a file.</summary>
        Public Shared Function readObject(Of T)(ByVal file As String) As T
            Try
                Using fs As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Return CType(ObjectGraphSerializer.Deserialize(fs), T)
                End Using
            Catch e As Exception
                Throw New InvalidOperationException("Error reading object from file " & file, e)
            End Try
        End Function

        ''' <summary>Reads a serialized object from a stream (the stream is left open).</summary>
        Public Shared Function readObject(Of T)(ByVal [is] As Stream) As T
            Try
                Return CType(ObjectGraphSerializer.Deserialize([is]), T)
            Catch e As Exception
                Throw New InvalidOperationException("Error reading object from stream", e)
            End Try
        End Function

        ''' <summary>Serializes the object graph to a byte array.</summary>
        Public Shared Function toByteArray(ByVal toSave As Object) As Byte()
            Try
                Using bos As New MemoryStream()
                    ObjectGraphSerializer.Serialize(bos, toSave)
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

        ''' <summary>Serializes the object graph to the stream (flushed, left open).</summary>
        Public Shared Sub writeObject(ByVal toSave As Object, ByVal writeTo As Stream)
            Try
                ObjectGraphSerializer.Serialize(writeTo, toSave)
                writeTo.Flush()
            Catch e As Exception
                Throw New InvalidOperationException("Error serializing object", e)
            End Try
        End Sub

        Public Shared Sub serialize(ByVal [object] As Object, ByVal os As Stream)
            writeObject([object], os)
        End Sub

        ''' <summary>Serializes the object graph to a file, creating its directory if needed.</summary>
        Public Shared Sub saveObject(ByVal toSave As Object, ByVal saveTo As String)
            Try
                Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(saveTo))
                If Not String.IsNullOrEmpty(dir) Then Directory.CreateDirectory(dir)
                Using fs As New FileStream(saveTo, FileMode.Create, FileAccess.Write)
                    ObjectGraphSerializer.Serialize(fs, toSave)
                End Using
            Catch e As Exception
                Throw New InvalidOperationException("Error saving object to file " & saveTo, e)
            End Try
        End Sub

        ''' <summary>A deep copy of a serializable object graph (serialize, then deserialize).</summary>
        Public Shared Function clone(Of T)(ByVal obj As T) As T
            Return fromByteArray(Of T)(toByteArray(obj))
        End Function
    End Class

End Namespace
