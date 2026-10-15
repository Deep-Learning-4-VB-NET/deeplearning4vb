Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports DeepLearning4VB.JavaCompat
Imports FileRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.FileRecordReader
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable

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

Namespace org.datavec.api.records.reader.impl.misc

    ''' <summary>
    ''' Reads Matlab ASCII files: one record per line, values separated by spaces or tabs, <c>%</c> starts a
    ''' comment. Each file of the split is parsed on the first <c>next()</c> call and its records are then
    ''' returned one at a time. (The Java version threw "Strange state detected" after parsing a file, only
    ''' looked at the file iterator in <c>hasNext()</c>, and returned an extra empty record for a file ending
    ''' in a new line; all three are fixed here.)
    ''' </summary>
    <Serializable>
    Public Class MatlabRecordReader
        Inherits FileRecordReader

        Private records As IList(Of IList(Of writable.Writable)) = New List(Of IList(Of writable.Writable))()
        <NonSerialized>
        Private currIter As JavaIterator(Of IList(Of writable.Writable))

        Public Overrides Function hasNext() As Boolean
            Return (currIter IsNot Nothing AndAlso currIter.hasNext()) OrElse MyBase.hasNext()
        End Function

        Public Overrides Function [next]() As IList(Of writable.Writable)
            'use the current iterator
            If currIter IsNot Nothing AndAlso currIter.hasNext() Then
                Return New List(Of writable.Writable)(currIter.next())
            End If
            records.Clear()
            'next file
            Dim nextFile As IList(Of writable.Writable) = MyBase.next()
            Dim val As String = nextFile(0).ToString()
            Dim reader As New StringReader(val)
            Dim c As Integer
            Dim chr As Char
            Dim fileContent As StringBuilder
            Dim isComment As Boolean
            Dim currRecord As IList(Of writable.Writable) = New List(Of writable.Writable)()

            fileContent = New StringBuilder()
            isComment = False
            records.Add(currRecord)
            Try
                ' determine number of attributes
                c = reader.Read()
                Do While c <> -1
                    chr = ChrW(c)

                    ' comment found?
                    If chr = "%"c Then
                        isComment = True
                    End If

                    ' end of line reached
                    If (chr = ControlChars.Lf) OrElse (chr = ControlChars.Cr) Then
                        isComment = False
                        If fileContent.Length > 0 Then
                            currRecord.Add(New DoubleWritable(parseDouble(fileContent.ToString())))
                        End If
                        If currRecord.Count > 0 Then
                            currRecord = New List(Of writable.Writable)()
                            records.Add(currRecord)
                        End If
                        fileContent = New StringBuilder()
                        c = reader.Read()
                        Continue Do
                    End If

                    ' skip till end of comment line
                    If isComment Then
                        c = reader.Read()
                        Continue Do
                    End If

                    ' separator found?
                    If (chr = ControlChars.Tab) OrElse (chr = " "c) Then
                        If fileContent.Length > 0 Then
                            currRecord.Add(New DoubleWritable(parseDouble(fileContent.ToString())))
                            fileContent = New StringBuilder()
                        End If
                    Else
                        fileContent.Append(chr)
                    End If
                    c = reader.Read()
                Loop

                ' last number?
                If fileContent.Length > 0 Then
                    currRecord.Add(New DoubleWritable(parseDouble(fileContent.ToString())))
                End If

                ' A file ending in a new line leaves an empty record behind (the Java version returned it as
                ' a record); only an empty file still yields a single empty record.
                If records.Count > 1 AndAlso records(records.Count - 1).Count = 0 Then
                    records.RemoveAt(records.Count - 1)
                End If

                currIter = New JavaIterator(Of IList(Of writable.Writable))(records)
            Catch ex As Exception
                Throw New InvalidOperationException("Unable to determine structure as Matlab ASCII file: " & ex.ToString(), ex)
            End Try

            If currIter.hasNext() Then
                Return New List(Of writable.Writable)(currIter.next())
            End If
            Throw New InvalidOperationException("Strange state detected")
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            Throw New NotSupportedException("Reading Matlab data from DataInputStream: not yet implemented")
        End Function

        ''' <summary>Java <c>Double.parseDouble</c>: culture invariant.</summary>
        Private Shared Function parseDouble(ByVal s As String) As Double
            Return Double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture)
        End Function

    End Class

End Namespace
