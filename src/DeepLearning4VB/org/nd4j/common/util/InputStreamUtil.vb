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

Namespace org.nd4j.common.util

    Public Class InputStreamUtil

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Counts the lines in a stream: the number of line-feed characters, or 1 for a non-empty
        ''' stream without any line feed. The stream is closed afterwards.
        ''' </summary>
        Public Shared Function countLines(ByVal [is] As Stream) As Integer
            If [is] Is Nothing Then Throw New ArgumentNullException(NameOf([is]))
            Try
                Dim c(1023) As Byte
                Dim count As Integer = 0
                Dim empty As Boolean = True
                Dim readChars As Integer = [is].Read(c, 0, c.Length)
                Do While readChars > 0
                    empty = False
                    For i As Integer = 0 To readChars - 1
                        If c(i) = 10 Then
                            count += 1
                        End If
                    Next i
                    readChars = [is].Read(c, 0, c.Length)
                Loop
                Return If(count = 0 AndAlso Not empty, 1, count)
            Finally
                [is].Close()
            End Try
        End Function

        ''' <summary>Counts the lines in a file (see <see cref="countLines(Stream)"/>).</summary>
        Public Shared Function countLines(ByVal filename As String) As Integer
            Using fis As New FileStream(filename, FileMode.Open, FileAccess.Read)
                Return countLines(New BufferedStream(fis))
            End Using
        End Function
    End Class

End Namespace
