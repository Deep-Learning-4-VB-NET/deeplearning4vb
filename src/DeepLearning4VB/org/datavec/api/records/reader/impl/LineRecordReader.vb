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
Imports System.Linq
Imports System.Text
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DeepLearning4VB.org.slf4j
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports BaseRecordReader = DeepLearning4VB.org.datavec.api.records.reader.BaseRecordReader
Imports StringIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of String)
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports InputStreamInputSplit = DeepLearning4VB.org.datavec.api.split.InputStreamInputSplit
Imports StringSplit = DeepLearning4VB.org.datavec.api.split.StringSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions

Namespace org.datavec.api.records.reader.impl

    ''' <summary>
    ''' Reads files line by line. Line iterators are Java-style (<c>hasNext()</c>/<c>next()</c>)
    ''' <see cref="JavaIterator(Of T)"/>s; file lines come from a <see cref="LineIterator"/>.
    '''
    ''' @author Adam Gibson
    ''' </summary>
    <Serializable>
    Public Class LineRecordReader
        Inherits BaseRecordReader

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(LineRecordReader))

        <NonSerialized>
        Private iter As StringIterator
        Protected Friend locations() As Uri
        Protected Friend splitIndex As Integer = 0
        Protected Friend lineIndex As Integer = 0 'Line index within the current split
        ''' <summary>The configuration (the field is called <c>conf</c> in Java; renamed because of the <c>Conf</c> property).</summary>
        <NonSerialized>
        Protected Friend confValue As Configuration
        Protected Friend initialized As Boolean
        ''' <summary>Charset name (Java kept a String because <c>StandardCharsets.UTF_8</c> is not serializable).</summary>
        Protected Friend charset As String = "UTF-8"

        Public Overrides Sub initialize(ByVal split As InputSplit)
            MyBase.initialize(split)
            If Not (TypeOf inputSplit Is StringSplit OrElse TypeOf inputSplit Is InputStreamInputSplit) Then
                Dim uris As New List(Of Uri)()
                Dim uriIterator As IEnumerator(Of Uri) = inputSplit.locationsIterator()
                Do While uriIterator.MoveNext()
                    uris.Add(uriIterator.Current)
                Loop

                Me.locations = uris.ToArray()
            End If
            Me.iter = getIterator(0)
            Me.initialized = True
        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            Me.confValue = conf
            initialize(split)
        End Sub

        Public Overrides Function [next]() As IList(Of writable.Writable)
            Preconditions.checkState(initialized, "Record reader has not been initialized")
            Dim ret As IList(Of writable.Writable) = New List(Of writable.Writable)()

            If iter.hasNext() Then
                Dim record As String = iter.next()
                invokeListeners(record)
                ret.Add(New Text(record))
                lineIndex += 1
                Return ret
            Else
                If Not (TypeOf inputSplit Is StringSplit) AndAlso splitIndex < locations.Length - 1 Then
                    splitIndex += 1
                    lineIndex = 0 'New split opened -> reset line index
                    Try
                        Dispose()
                        iter = getIterator(splitIndex)
                        onLocationOpen(locations(splitIndex))
                    Catch e As IOException
                        log.error("", e)
                    End Try

                    If iter.hasNext() Then
                        Dim record As String = iter.next()
                        invokeListeners(record)
                        ret.Add(New Text(record))
                        lineIndex += 1
                        Return ret
                    End If
                End If

                Throw New InvalidOperationException("No more elements found!")
            End If
        End Function

        Public Overrides Function hasNext() As Boolean
            Preconditions.checkState(initialized, "Record reader has not been initialized")

            If iter IsNot Nothing AndAlso iter.hasNext() Then
                Return True
            Else
                If locations IsNot Nothing AndAlso Not (TypeOf inputSplit Is StringSplit) AndAlso splitIndex < locations.Length - 1 Then
                    splitIndex += 1
                    lineIndex = 0 'New split -> reset line count
                    Try
                        Dispose()
                        iter = getIterator(splitIndex)
                        onLocationOpen(locations(splitIndex))
                    Catch e As IOException
                        log.error("", e)
                    End Try

                    Return iter.hasNext()
                End If

                Return False
            End If
        End Function

        Protected Friend Overridable Sub onLocationOpen(ByVal location As Uri)

        End Sub

        Public Overrides Sub Dispose()
            If iter IsNot Nothing Then
                If TypeOf iter Is LineIterator Then
                    Dim iter2 As LineIterator = CType(iter, LineIterator)
                    iter2.close()
                End If
            End If
        End Sub

        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)
                Me.confValue = value
            End Set
            Get
                Return confValue
            End Get
        End Property

        Public Overrides ReadOnly Property Labels As IList(Of String)
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides Sub reset()
            If inputSplit Is Nothing Then
                Throw New NotSupportedException("Cannot reset without first initializing")
            End If
            Try
                inputSplit.reset()
                Dispose()
                initialize(inputSplit)
                splitIndex = 0
            Catch e As Exception
                Throw New Exception("Error during LineRecordReader reset", e)
            End Try
            lineIndex = 0
        End Sub

        Public Overrides Function resetSupported() As Boolean
            If inputSplit IsNot Nothing Then
                Return inputSplit.resetSupported()
            End If
            Return True
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            invokeListeners(uri)
            'Here: we are reading a single line from the DataInputStream
            Dim br As New StreamReader(dataInputStream.BaseStream, charsetEncoding(), True, 4096, True)
            Dim line As String = br.ReadLine()
            Return JavaCollections.singletonList(Of writable.Writable)(New Text(line))
        End Function

        ''' <summary>The <see cref="Encoding"/> for the <see cref="charset"/> name.</summary>
        Protected Friend Function charsetEncoding() As Encoding
            Return If(charset Is Nothing, Encoding.UTF8, Encoding.GetEncoding(charset))
        End Function

        Protected Friend Overridable Function getIterator(ByVal location As Integer) As StringIterator
            Dim iterator As StringIterator = Nothing
            If TypeOf inputSplit Is StringSplit Then
                Dim stringSplit As StringSplit = DirectCast(inputSplit, StringSplit)
                iterator = StringIterator.singleton(stringSplit.Data)
            ElseIf TypeOf inputSplit Is InputStreamInputSplit Then
                Dim [is] As Stream = DirectCast(inputSplit, InputStreamInputSplit).getIs()
                If [is] IsNot Nothing Then
                    iterator = New LineIterator([is], charsetEncoding())
                End If
            Else
                If locations.Length > 0 Then
                    Dim inputStream As Stream = streamCreatorFn.apply(locations(location))
                    iterator = New LineIterator(inputStream, charsetEncoding())
                End If
            End If
            If iterator Is Nothing Then
                Throw New NotSupportedException("Unknown input split: " & Objects.toString(inputSplit))
            End If
            Return iterator
        End Function

        Protected Friend Overridable Sub closeIfRequired(ByVal iterator As StringIterator)
            If TypeOf iterator Is LineIterator Then
                Dim iter As LineIterator = CType(iterator, LineIterator)
                iter.close()
            End If
        End Sub

        Public Overrides Function nextRecord() As Record
            Dim [next] As IList(Of writable.Writable) = Me.next()
            Dim uri As Uri = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
            Dim meta As RecordMetaData = New RecordMetaDataLine(Me.lineIndex - 1, uri, GetType(LineRecordReader)) '-1 as line number has been incremented already...
            Return New RecordImpl([next], meta)
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Return Nothing
        End Function

        ''' <summary>Java <c>URI.compareTo</c>: ordinal comparison of the URI strings.</summary>
        Private Shared Function compareUris(ByVal a As Uri, ByVal b As Uri) As Integer
            Return String.CompareOrdinal(URIUtil.toUriString(a), URIUtil.toUriString(b))
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
            'First: create a sorted list of the RecordMetaData
            Dim list As List(Of Triple(Of Integer, RecordMetaDataLine, IList(Of writable.Writable))) = New List(Of Triple(Of Integer, RecordMetaDataLine, IList(Of writable.Writable)))()
            Dim uris As ISet(Of Uri) = New HashSet(Of Uri)()
            Dim count As Integer = 0
            For Each rmd As RecordMetaData In recordMetaDatas
                If Not (TypeOf rmd Is RecordMetaDataLine) Then
                    Throw New ArgumentException("Invalid metadata; expected RecordMetaDataLine instance; got: " & Objects.toString(rmd))
                End If
                list.Add(New Triple(Of Integer, RecordMetaDataLine, IList(Of writable.Writable))(count, DirectCast(rmd, RecordMetaDataLine), Nothing))
                count += 1
                If rmd.URI IsNot Nothing Then
                    uris.Add(rmd.URI)
                End If
            Next rmd
            Dim sortedURIs As List(Of Uri) = Nothing
            If uris.Count > 0 Then
                sortedURIs = New List(Of Uri)(uris)
                sortedURIs.Sort(AddressOf compareUris)
            End If

            'Sort by URI first (if possible - don't always have URIs though, for String split etc), then sort by line number:
            ' (a stable sort, like Java's Collections.sort)
            list = list.OrderBy(Function(t) t, Comparer(Of Triple(Of Integer, RecordMetaDataLine, IList(Of writable.Writable))).Create(
                Function(o1, o2)
                    If o1.getSecond().getURI() IsNot Nothing Then
                        If Not o1.getSecond().getURI().Equals(o2.getSecond().getURI()) Then
                            Return compareUris(o1.getSecond().getURI(), o2.getSecond().getURI())
                        End If
                    End If
                    Return Ints.compare(o1.getSecond().getLineNumber(), o2.getSecond().getLineNumber())
                End Function)).ToList()

            If uris.Count > 0 AndAlso sortedURIs IsNot Nothing Then
                'URIs case - possibly with multiple URIs
                Dim currentURI As Uri = sortedURIs(0)
                Dim currentUriIter As StringIterator = New LineIterator(streamCreatorFn.apply(currentURI), charsetEncoding())

                Dim currentURIIdx As Integer = 0 'Index of URI
                Dim currentLineIdx As Integer = 0 'Index of the line for the current URI
                Dim line As String = currentUriIter.next()
                For Each t As Triple(Of Integer, RecordMetaDataLine, IList(Of writable.Writable)) In list
                    Dim thisURI As Uri = t.getSecond().getURI()
                    Dim nextLineIdx As Integer = t.getSecond().getLineNumber()

                    'First: find the right URI for this record...
                    Do While Not currentURI.Equals(thisURI)
                        'Iterate to the next URI
                        currentURIIdx += 1
                        If currentURIIdx >= sortedURIs.Count Then
                            'Should never happen
                            Throw New InvalidOperationException("Count not find URI " & URIUtil.toUriString(thisURI) & " in URIs list: " & Arrays.toString(sortedURIs.ToArray()))
                        End If
                        currentURI = sortedURIs(currentURIIdx)
                        currentLineIdx = 0
                        If currentURI.Equals(thisURI) Then
                            'Found the correct URI for this MetaData instance
                            closeIfRequired(currentUriIter)
                            currentUriIter = New LineIterator(streamCreatorFn.apply(currentURI), charsetEncoding())
                            line = currentUriIter.next()
                        End If
                    Loop

                    'Have the correct URI/iter open -> scan to the required line
                    Do While currentLineIdx < nextLineIdx AndAlso currentUriIter.hasNext()
                        line = currentUriIter.next()
                        currentLineIdx += 1
                    Loop
                    If currentLineIdx < nextLineIdx AndAlso Not currentUriIter.hasNext() Then
                        Throw New InvalidOperationException("Could not get line " & nextLineIdx & " from URI " & URIUtil.toUriString(currentURI) & ": has only " & currentLineIdx & " lines")
                    End If
                    t.setThird(JavaCollections.singletonList(Of writable.Writable)(New Text(line)))
                Next t
                closeIfRequired(currentUriIter)
            Else
                'Not URI based: String split, etc
                Dim iterator As StringIterator = getIterator(0)
                Dim currentLineIdx As Integer = 0
                Dim line As String = iterator.next()
                For Each t As Triple(Of Integer, RecordMetaDataLine, IList(Of writable.Writable)) In list
                    Dim nextLineIdx As Integer = t.getSecond().getLineNumber()
                    Do While currentLineIdx < nextLineIdx AndAlso iterator.hasNext()
                        line = iterator.next()
                        currentLineIdx += 1
                    Loop
                    t.setThird(JavaCollections.singletonList(Of writable.Writable)(New Text(line)))
                Next t
                closeIfRequired(iterator)
            End If

            'Now, sort by the original (request) order:
            list.Sort(Function(o1, o2) Ints.compare(o1.getFirst(), o2.getFirst()))

            'And return...
            Dim [out] As IList(Of Record) = New List(Of Record)()
            For Each t As Triple(Of Integer, RecordMetaDataLine, IList(Of writable.Writable)) In list
                [out].Add(New RecordImpl(t.getThird(), t.getSecond()))
            Next t
            Return [out]
        End Function

    End Class

End Namespace
