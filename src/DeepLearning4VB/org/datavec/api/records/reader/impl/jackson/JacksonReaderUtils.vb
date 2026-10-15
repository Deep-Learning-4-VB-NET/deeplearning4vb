Imports System
Imports System.Collections.Generic
Imports System.Text.Json
Imports DeepLearning4VB.JavaCompat
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

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

Namespace org.datavec.api.records.reader.impl.jackson

    ''' <summary>
    ''' Extracts the fields of a <see cref="FieldSelection"/> from a JSON document. The Java version parsed the
    ''' document with a Jackson <c>ObjectMapper</c> into a <c>Map</c>, which also allowed YAML and XML
    ''' factories; this port reads JSON only, with System.Text.Json (<see cref="JsonDocument"/>), so the
    ''' <c>ObjectMapper</c> parameter is gone. Numbers are returned as written in the document (Jackson
    ''' returned <c>Number.toString()</c>, which only differs for exponent forms such as <c>1e3</c>), a JSON
    ''' <c>null</c> counts as a missing value, and any other value kind (boolean, array, object) at the end
    ''' of a path is an error, as it was in Java.
    ''' </summary>
    Public Class JacksonReaderUtils

        Private Sub New()
        End Sub

        Public Shared Function parseRecord(ByVal line As String, ByVal selection As FieldSelection) As IList(Of writable.Writable)
            Dim [out] As IList(Of writable.Writable) = New List(Of writable.Writable)()
            Dim paths As IList(Of String()) = selection.getFieldPaths()
            Dim valueIfMissing As IList(Of writable.Writable) = selection.getValueIfMissing()

            Dim doc As JsonDocument
            Try
                doc = JsonDocument.Parse(line)
            Catch e As JsonException
                Throw New Exception("Error parsing file", e)
            End Try

            Using doc
                Dim root As JsonElement = doc.RootElement
                If root.ValueKind <> JsonValueKind.Object Then
                    Throw New Exception("Error parsing file: expected a JSON object, found " & root.ValueKind.ToString())
                End If

                'Now, extract out values...
                For i As Integer = 0 To paths.Count - 1
                    Dim currPath() As String = paths(i)
                    Dim value As String = Nothing
                    Dim currElement As JsonElement = root
                    For j As Integer = 0 To currPath.Length - 1
                        Dim o As JsonElement
                        If currElement.ValueKind = JsonValueKind.Object AndAlso currElement.TryGetProperty(currPath(j), o) Then
                            If j = currPath.Length - 1 Then
                                'Expect to get the final value
                                Select Case o.ValueKind
                                    Case JsonValueKind.String
                                        value = o.GetString()
                                    Case JsonValueKind.Number
                                        value = o.GetRawText()
                                    Case JsonValueKind.Null
                                        value = Nothing
                                    Case Else
                                        Throw New InvalidOperationException("Expected to find String on path " & Arrays.toString(currPath) & ", found " & o.ValueKind.ToString() & " with value " & o.GetRawText())
                                End Select
                            Else
                                'Expect to get a map...
                                If o.ValueKind = JsonValueKind.Object Then
                                    currElement = o
                                End If
                            End If
                        Else
                            'Not found
                            value = Nothing
                            Exit For
                        End If
                    Next j

                    Dim outputWritable As writable.Writable
                    If value Is Nothing Then
                        outputWritable = valueIfMissing(i)
                    Else
                        outputWritable = New Text(value)
                    End If
                    [out].Add(outputWritable)
                Next i
            End Using

            Return [out]
        End Function

    End Class

End Namespace
