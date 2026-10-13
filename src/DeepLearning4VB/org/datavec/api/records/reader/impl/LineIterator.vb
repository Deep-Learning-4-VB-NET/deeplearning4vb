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

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports JavaIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of String)

Namespace org.datavec.api.records.reader.impl

    ''' <summary>
    ''' The commons-io <c>LineIterator</c>: a Java-style iterator over the lines of a <see cref="TextReader"/>
    ''' (or of a stream, decoded with the given encoding). <see cref="close"/>/<see cref="Dispose"/> close the
    ''' underlying reader; like commons-io, the iterator is exhausted afterwards.
    ''' </summary>
    Public Class LineIterator
        Inherits JavaIterator

        Private ReadOnly reader As TextReader

        Public Sub New(ByVal reader As TextReader)
            MyBase.New(readLines(reader))
            Me.reader = reader
        End Sub

        ''' <summary>Iterates the lines of <paramref name="stream"/> decoded with <paramref name="encoding"/> (commons-io <c>IOUtils.lineIterator(InputStream, charset)</c>).</summary>
        Public Sub New(ByVal stream As Stream, ByVal encoding As Encoding)
            Me.New(New StreamReader(stream, If(encoding, Encoding.UTF8)))
        End Sub

        ''' <summary>Iterates the lines of <paramref name="stream"/> decoded with the named charset.</summary>
        Public Sub New(ByVal stream As Stream, ByVal charsetName As String)
            Me.New(stream, If(charsetName Is Nothing, Encoding.UTF8, Encoding.GetEncoding(charsetName)))
        End Sub

        Private Shared Iterator Function readLines(ByVal reader As TextReader) As IEnumerable(Of String)
            If reader Is Nothing Then Throw New ArgumentNullException(NameOf(reader))
            Do
                Dim line As String = reader.ReadLine()
                If line Is Nothing Then Exit Do
                Yield line
            Loop
        End Function

        ''' <summary>The next line (commons-io <c>nextLine()</c>).</summary>
        Public Function nextLine() As String
            Return [next]()
        End Function

        ''' <summary>Closes the underlying reader (commons-io <c>close()</c>).</summary>
        Public Sub close()
            Dispose()
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()
            reader.Dispose()
        End Sub

    End Class

End Namespace
