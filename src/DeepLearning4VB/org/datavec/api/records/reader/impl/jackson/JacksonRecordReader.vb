Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports PathLabelGenerator = DeepLearning4VB.org.datavec.api.io.labels.PathLabelGenerator
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataURI
Imports BaseRecordReader = DeepLearning4VB.org.datavec.api.records.reader.BaseRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil

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
    ''' Reads each file (URI) of the split as one JSON document and extracts the fields of a
    ''' <see cref="FieldSelection"/>, optionally appending a label from a <see cref="PathLabelGenerator"/>.
    ''' The Java constructors also took an <c>ObjectMapper</c> (JSON, YAML or XML); this port reads JSON
    ''' only with System.Text.Json (see <see cref="JacksonReaderUtils"/>), so that parameter is gone.
    ''' <c>initialize(InputSplit)</c> passes the split to the base class (the Java code passed the still
    ''' unset field, so the base class never saw the split).
    ''' </summary>
    <Serializable>
    Public Class JacksonRecordReader
        Inherits BaseRecordReader

        Private selection As FieldSelection
        Private shuffle As Boolean
        Private rngSeed As Long
        Private labelGenerator As PathLabelGenerator
        Private labelPosition As Integer
        Private r As Random
        Private charset As String = "UTF-8" 'Using String as StandardCharsets.UTF_8 is not serializable

        Private uris() As Uri
        Private cursor As Integer = 0

        Public Sub New(ByVal selection As FieldSelection)
            Me.New(selection, False)
        End Sub

        Public Sub New(ByVal selection As FieldSelection, ByVal shuffle As Boolean)
            Me.New(selection, shuffle, JavaSystem.currentTimeMillis(), Nothing)
        End Sub

        Public Sub New(ByVal selection As FieldSelection, ByVal shuffle As Boolean, ByVal rngSeed As Long, ByVal labelGenerator As PathLabelGenerator)
            Me.New(selection, shuffle, rngSeed, labelGenerator, -1)
        End Sub

        Public Sub New(ByVal selection As FieldSelection, ByVal shuffle As Boolean, ByVal rngSeed As Long, ByVal labelGenerator As PathLabelGenerator, ByVal labelPosition As Integer)
            Me.selection = selection
            Me.shuffle = shuffle
            Me.rngSeed = rngSeed
            If shuffle Then
                r = New Random(CInt(rngSeed And &H7FFFFFFFL))
            End If
            Me.labelGenerator = labelGenerator
            Me.labelPosition = labelPosition
        End Sub

        Public Overrides Sub initialize(ByVal split As InputSplit)
            If TypeOf split Is FileSplit Then
                Throw New NotSupportedException("Cannot use JacksonRecordReader with FileSplit")
            End If
            MyBase.initialize(split)
            Me.uris = split.locations()
            If shuffle Then
                Dim list As IList(Of Uri) = New List(Of Uri)(uris)
                JavaCollections.shuffle(list, r)
                uris = CType(list, List(Of Uri)).ToArray()
            End If
        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            initialize(split)
        End Sub

        Public Overrides Function [next]() As IList(Of writable.Writable)
            If uris Is Nothing Then
                Throw New InvalidOperationException("URIs are null. Not initialized?")
            End If
            If Not hasNext() Then
                Throw New InvalidOperationException("No next element")
            End If

            Dim uri As Uri = uris(cursor)
            cursor += 1
            invokeListeners(uri)
            Dim fileAsString As String
            Try
                Using s As Stream = streamCreatorFn.apply(uri)
                    Using reader As New StreamReader(s, charsetEncoding())
                        fileAsString = reader.ReadToEnd()
                    End Using
                End Using
            Catch e As IOException
                Throw New Exception("Error reading URI file", e)
            End Try

            Return readValues(uri, fileAsString)
        End Function

        Public Overrides Function hasNext() As Boolean
            Return cursor < uris.Length
        End Function

        Public Overrides ReadOnly Property Labels As IList(Of String)
            Get
                Throw New NotSupportedException()
            End Get
        End Property

        Public Overrides Sub reset()
            cursor = 0
            If shuffle Then
                Dim list As IList(Of Uri) = New List(Of Uri)(uris)
                JavaCollections.shuffle(list, r)
                uris = CType(list, List(Of Uri)).ToArray()
            End If
        End Sub

        Public Overrides Function resetSupported() As Boolean
            Return True
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            Dim br As New StreamReader(dataInputStream.BaseStream, charsetEncoding(), True, 4096, True)
            Dim sb As New StringBuilder()
            Dim line As String
            line = br.ReadLine()
            Do While line IsNot Nothing
                sb.Append(line).Append(ControlChars.Lf)
                line = br.ReadLine()
            Loop

            Return readValues(uri, sb.ToString())
        End Function

        Public Overrides Sub Dispose()

        End Sub

        ''' <summary>Java's <c>setConf</c> ignored the configuration and <c>getConf</c> returned null.</summary>
        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)

            End Set
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The <see cref="Encoding"/> for the <see cref="charset"/> name.</summary>
        Private Function charsetEncoding() As Encoding
            Return If(charset Is Nothing, Encoding.UTF8, Encoding.GetEncoding(charset))
        End Function

        Private Function readValues(ByVal uri As Uri, ByVal fileContents As String) As IList(Of writable.Writable)
            Dim [out] As IList(Of writable.Writable) = JacksonReaderUtils.parseRecord(fileContents, selection)

            'Add label - if required
            If labelGenerator IsNot Nothing Then
                Dim label As writable.Writable = labelGenerator.getLabelForPath(uri)
                Dim paths As IList(Of String()) = selection.getFieldPaths()
                If (labelPosition >= paths.Count OrElse labelPosition = -1) Then
                    'Edge case: might want label as the last value
                    [out].Add(label)
                Else
                    [out].Insert(labelPosition, label) 'Add and shift existing to right
                End If
            End If

            Return [out]
        End Function

        Public Overrides Function nextRecord() As Record
            Dim currentURI As Uri = uris(cursor)
            Dim writables As IList(Of writable.Writable) = [next]()
            Dim meta As RecordMetaData = New RecordMetaDataURI(currentURI, GetType(JacksonRecordReader))
            Return New RecordImpl(writables, meta)
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Return loadFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
            Dim [out] As IList(Of Record) = New List(Of Record)()
            For Each metaData As RecordMetaData In recordMetaDatas
                Dim uri As Uri = metaData.URI
                Dim fileAsString As String
                Try
                    fileAsString = File.ReadAllText(URIUtil.toFilePath(uri), charsetEncoding())
                Catch e As IOException
                    Throw New Exception("Error reading URI file", e)
                End Try

                Dim writables As IList(Of writable.Writable) = readValues(uri, fileAsString)
                [out].Add(New RecordImpl(writables, metaData))
            Next metaData

            Return [out]
        End Function

    End Class

End Namespace
