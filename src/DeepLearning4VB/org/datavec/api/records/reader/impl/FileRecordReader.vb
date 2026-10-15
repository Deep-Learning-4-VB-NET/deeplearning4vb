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
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataURI
Imports BaseRecordReader = DeepLearning4VB.org.datavec.api.records.reader.BaseRecordReader
Imports JavaIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of System.Uri)
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.records.reader.impl

    ''' <summary>
    ''' File reader/writer: each file of the input split is one record holding the whole file as a
    ''' <see cref="Text"/>, optionally followed by the index of the parent directory in the label list.
    '''
    ''' @author Adam Gibson
    ''' </summary>
    <Serializable>
    Public Class FileRecordReader
        Inherits BaseRecordReader

        <NonSerialized>
        Protected Friend locationsIterator As JavaIterator
        ''' <summary>The configuration (the field is called <c>conf</c> in Java; renamed because of the <c>Conf</c> property).</summary>
        <NonSerialized>
        Protected Friend confValue As Configuration
        Protected Friend currentUri As Uri
        ''' <summary>The labels (the field is called <c>labels</c> in Java; renamed because of the <c>Labels</c> property).</summary>
        Protected Friend labelList As IList(Of String)
        Protected Friend appendLabel As Boolean = False
        ''' <summary>Charset name (Java kept a String because <c>StandardCharsets.UTF_8</c> is not serializable).</summary>
        Protected Friend charset As String = "UTF-8"

        Public Sub New()
        End Sub

        Public Overrides Sub initialize(ByVal split As InputSplit)
            MyBase.initialize(split)
            doInitialize(split)
        End Sub

        Protected Friend Overridable Sub doInitialize(ByVal split As InputSplit)

            If labelList Is Nothing AndAlso appendLabel Then
                Dim locations() As Uri = split.locations()
                If locations.Length > 0 Then
                    Dim labels As ISet(Of String) = New HashSet(Of String)()
                    For Each u As Uri In locations
                        Dim pathSplit() As String = Regex.Split(URIUtil.toUriString(u), "[/\\]")
                        labels.Add(pathSplit(pathSplit.Length - 2))
                    Next u
                    Dim sorted As New List(Of String)(labels)
                    sorted.Sort(StringComparer.Ordinal)
                    Me.labelList = sorted
                End If
            End If
            locationsIterator = New JavaIterator(split.locationsIterator())
        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            appendLabel = conf.getBoolean(APPEND_LABEL, True)
            ' Java set the inputSplit field directly; going through the base class also picks up the
            ' stream creator function of a StreamInputSplit, as initialize(InputSplit) does.
            MyBase.initialize(split)
            doInitialize(split)
            Me.confValue = conf
        End Sub

        Public Overrides Function [next]() As IList(Of writable.Writable)
            Return nextRecord().getRecord()
        End Function

        Private Function loadFromStream(ByVal uri As Uri, ByVal [next] As Stream, ByVal encoding As Encoding) As IList(Of writable.Writable)
            Dim ret As IList(Of writable.Writable) = New List(Of writable.Writable)()
            Try
                Dim s As String
                Using reader As New StreamReader([next], encoding, True, 4096, True)
                    s = reader.ReadToEnd()
                End Using
                ret.Add(New Text(s))
                If appendLabel Then
                    Dim idx As Integer = getLabel(uri)
                    ret.Add(New IntWritable(idx))
                End If
            Catch e As IOException
                Throw New InvalidOperationException("Error reading from input stream: " & URIUtil.toUriString(uri), e)
            End Try
            Return ret
        End Function

        ''' <summary>The <see cref="Encoding"/> for the <see cref="charset"/> name.</summary>
        Protected Friend Function charsetEncoding() As Encoding
            Return If(charset Is Nothing, Encoding.UTF8, Encoding.GetEncoding(charset))
        End Function

        ''' <summary>
        ''' Return the current label.
        ''' The index of the current file's parent directory
        ''' in the label list </summary>
        ''' <returns> The index of the current file's parent directory </returns>
        Public Overridable ReadOnly Property CurrentLabel As Integer
            Get
                Return getLabel(currentUri)
            End Get
        End Property

        ''' <summary>Java-style accessor of <see cref="CurrentLabel"/>.</summary>
        Public Function getCurrentLabel() As Integer
            Return CurrentLabel
        End Function

        Public Overridable Function getLabel(ByVal uri As Uri) As Integer
            Dim s As String = URIUtil.toUriString(uri)
            Dim lastIdx As Integer = Math.Max(s.LastIndexOf("/"c), s.LastIndexOf("\"c)) 'Note: if neither are found, -1 is fine here
            Dim [sub] As String = s.Substring(0, lastIdx)
            Dim secondLastIdx As Integer = Math.Max([sub].LastIndexOf("/"c), [sub].LastIndexOf("\"c))
            Dim name As String = s.Substring(secondLastIdx + 1, lastIdx - (secondLastIdx + 1))
            Return labelList.IndexOf(name)
        End Function

        Public Overrides ReadOnly Property Labels As IList(Of String)
            Get
                Return labelList
            End Get
        End Property

        ''' <summary>Sets the labels (Java <c>setLabels</c>; the <c>Labels</c> property is read-only on the base class).</summary>
        Public Overridable Sub setLabels(ByVal labels As IList(Of String))
            Me.labelList = labels
        End Sub

        Public Overrides Function hasNext() As Boolean
            Return locationsIterator.hasNext()
        End Function

        Public Overrides Sub Dispose()

        End Sub

        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)
                Me.confValue = value
            End Set
            Get
                Return confValue
            End Get
        End Property

        Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of writable.Writable))
            Dim ret As IList(Of IList(Of writable.Writable)) = New List(Of IList(Of writable.Writable))(num)
            Dim numBatches As Integer = 0
            Do While hasNext() AndAlso numBatches < num
                ret.Add([next]())
                numBatches += 1 ' the Java loop never incremented the counter and read everything
            Loop

            Return ret
        End Function

        Public Overrides Sub reset()
            If inputSplit Is Nothing Then
                Throw New NotSupportedException("Cannot reset without first initializing")
            End If
            Try
                doInitialize(inputSplit)
            Catch e As Exception
                Throw New Exception("Error during LineRecordReader reset", e)
            End Try
        End Sub

        Public Overrides Function resetSupported() As Boolean
            If inputSplit IsNot Nothing Then
                Return inputSplit.resetSupported()
            End If
            Return False 'reset() throws exception on reset() if inputSplit is null
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            invokeListeners(uri)
            'Here: reading the entire file to a Text writable
            Dim br As New StreamReader(dataInputStream.BaseStream, charsetEncoding(), True, 4096, True)
            Dim sb As New StringBuilder()
            Dim line As String
            line = br.ReadLine()
            Do While line IsNot Nothing
                sb.Append(line).Append(ControlChars.Lf)
                line = br.ReadLine()
            Loop
            Return JavaCollections.singletonList(Of writable.Writable)(New Text(sb.ToString()))
        End Function

        Public Overrides Function nextRecord() As Record
            Dim [next] As Uri = locationsIterator.next()
            currentUri = [next]
            invokeListeners([next])

            Dim ret As IList(Of writable.Writable)
            Try
                Using s As Stream = streamCreatorFn.apply([next])
                    ret = loadFromStream([next], s, charsetEncoding())
                End Using
            Catch e As IOException
                Throw New Exception("Error reading from stream for URI: " & URIUtil.toUriString([next]), e)
            End Try

            Return New RecordImpl(ret, New RecordMetaDataURI([next], GetType(FileRecordReader)))
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Return loadFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
            Dim [out] As IList(Of Record) = New List(Of Record)()

            For Each meta As RecordMetaData In recordMetaDatas
                Dim uri As Uri = meta.URI

                Dim list As IList(Of writable.Writable)
                Try
                    Using s As Stream = streamCreatorFn.apply(uri)
                        list = loadFromStream(uri, s, charsetEncoding())
                    End Using
                Catch e As IOException
                    Throw New Exception("Error reading from stream for URI: " & URIUtil.toUriString(uri), e)
                End Try

                [out].Add(New RecordImpl(list, meta))
            Next meta

            Return [out]
        End Function
    End Class

End Namespace
