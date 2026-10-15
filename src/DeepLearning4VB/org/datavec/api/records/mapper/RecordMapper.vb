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
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports NumberOfRecordsPartitioner = DeepLearning4VB.org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports Partitioner = DeepLearning4VB.org.datavec.api.split.partition.Partitioner

Namespace org.datavec.api.records.mapper

    ''' <summary>
    ''' Copies the records of a <see cref="RecordReader"/> (or of several readers, concatenated) to a
    ''' <see cref="RecordWriter"/>, partitioning the output with a <see cref="Partitioner"/>. Lombok's
    ''' <c>@Builder</c> is written out as the nested <see cref="Builder"/> class; the static <c>builder()</c>
    ''' method is <see cref="newBuilder"/> (VB is case-insensitive, so it cannot share the name of the class).
    ''' </summary>
    Public Class RecordMapper

        Private recordReader As RecordReader
        Private recordWriter As RecordWriter
        Private inputUrl As InputSplit
        Private splitPerReader() As InputSplit
        Private readersToConcat() As RecordReader

        Private outputUrl As InputSplit
        Private callInitRecordReader As Boolean = True
        Private callInitRecordWriter As Boolean = True
        Private callInitPartitioner As Boolean = True
        Private configuration As New Configuration()

        Private configurationsPerReader() As Configuration

        Private partitionerValue As Partitioner = New NumberOfRecordsPartitioner()
        Private batchSize As Integer

        Private Sub New(ByVal builder As Builder)
            Me.recordReader = builder.recordReaderValue
            Me.recordWriter = builder.recordWriterValue
            Me.inputUrl = builder.inputUrlValue
            Me.splitPerReader = builder.splitPerReaderValue
            Me.readersToConcat = builder.readersToConcatValue
            Me.outputUrl = builder.outputUrlValue
            Me.callInitRecordReader = builder.callInitRecordReaderValue
            Me.callInitRecordWriter = builder.callInitRecordWriterValue
            Me.callInitPartitioner = builder.callInitPartitionerValue
            Me.configuration = builder.configurationValue
            Me.configurationsPerReader = builder.configurationsPerReaderValue
            Me.partitionerValue = builder.partitionerValue
            Me.batchSize = builder.batchSizeValue
        End Sub

        ''' <summary>Creates a builder (Lombok's static <c>builder()</c>).</summary>
        Public Shared Function newBuilder() As Builder
            Return New Builder()
        End Function

        ''' <summary>The partitioner (Lombok <c>@Getter</c>).</summary>
        Public Function getPartitioner() As Partitioner
            Return partitionerValue
        End Function

        Public ReadOnly Property Partitioner As Partitioner
            Get
                Return partitionerValue
            End Get
        End Property

        ''' <summary>
        ''' Copy the <see cref="RecordReader"/>
        ''' data using the <see cref="RecordWriter"/>.
        ''' Note that unless batch is supported by
        ''' both the <see cref="RecordReader"/> and <see cref="RecordWriter"/>
        ''' then writes will happen one at a time.
        ''' You can see if batch is enabled via <see cref="RecordReader.batchesSupported()"/>
        ''' and <see cref="RecordWriter.supportsBatch()"/> respectively. </summary>
        ''' <exception cref="Exception"> </exception>
        Public Overridable Sub copy()
            If callInitRecordReader Then
                If recordReader IsNot Nothing Then
                    recordReader.initialize(configuration, inputUrl)
                Else
                    If readersToConcat Is Nothing OrElse splitPerReader Is Nothing Then
                        Throw New ArgumentException("No readers or input  splits found.")
                    End If

                    If readersToConcat.Length <> splitPerReader.Length Then
                        Throw New ArgumentException("One input split must be specified per record reader")
                    End If

                    For i As Integer = 0 To readersToConcat.Length - 1
                        If readersToConcat(i) Is Nothing Then
                            Throw New InvalidOperationException("Reader at record " & i & " was null!")
                        End If
                        If splitPerReader(i) Is Nothing Then
                            Throw New InvalidOperationException("Split at " & i & " is null!")
                        End If
                        'allow for, but do not enforce configurations per reader.
                        If configurationsPerReader IsNot Nothing Then
                            readersToConcat(i).initialize(configurationsPerReader(i), splitPerReader(i))
                        Else
                            readersToConcat(i).initialize(configuration, splitPerReader(i))
                        End If
                    Next i
                End If
            End If

            If callInitPartitioner Then
                partitionerValue.init(configuration, outputUrl)
            End If

            If callInitRecordWriter Then
                recordWriter.initialize(configuration, outputUrl, partitionerValue)
            End If

            If recordReader IsNot Nothing Then
                write(recordReader, True)
            ElseIf readersToConcat IsNot Nothing Then
                For Each reader As RecordReader In readersToConcat
                    write(reader, False)
                Next reader

                'close since we can't do it within the method
                recordWriter.close()
            End If

        End Sub

        Private Sub write(ByVal recordReader As RecordReader, ByVal closeWriter As Boolean)
            If batchSize > 0 AndAlso recordReader.batchesSupported() AndAlso recordWriter.supportsBatch() Then
                Do While recordReader.hasNext()
                    Dim [next] As IList(Of IList(Of writable.Writable)) = recordReader.next(batchSize)
                    'ensure we can write a file for either the current or next iterations
                    If partitionerValue.needsNewPartition() Then
                        partitionerValue.currentOutputStream().Flush()
                        partitionerValue.currentOutputStream().Close()
                        partitionerValue.openNewStream()
                    End If
                    'update records written
                    partitionerValue.updatePartitionInfo(recordWriter.writeBatch([next]))

                Loop

                partitionerValue.currentOutputStream().Flush()
                recordReader.Dispose()
                If closeWriter Then
                    partitionerValue.currentOutputStream().Close()
                    recordWriter.close()
                End If

            Else
                Do While recordReader.hasNext()
                    Dim [next] As IList(Of writable.Writable) = recordReader.next()
                    'update records written
                    partitionerValue.updatePartitionInfo(recordWriter.write([next]))
                    If partitionerValue.needsNewPartition() Then
                        partitionerValue.openNewStream()
                    End If
                Loop
            End If
        End Sub

        ''' <summary>Builder for <see cref="RecordMapper"/> (Lombok <c>@Builder</c>).</summary>
        Public Class Builder
            Friend recordReaderValue As RecordReader
            Friend recordWriterValue As RecordWriter
            Friend inputUrlValue As InputSplit
            Friend splitPerReaderValue() As InputSplit
            Friend readersToConcatValue() As RecordReader
            Friend outputUrlValue As InputSplit
            Friend callInitRecordReaderValue As Boolean = True
            Friend callInitRecordWriterValue As Boolean = True
            Friend callInitPartitionerValue As Boolean = True
            Friend configurationValue As Configuration = New Configuration()
            Friend configurationsPerReaderValue() As Configuration
            Friend partitionerValue As Partitioner = New NumberOfRecordsPartitioner()
            Friend batchSizeValue As Integer

            Public Function recordReader(ByVal value As RecordReader) As Builder
                Me.recordReaderValue = value
                Return Me
            End Function

            Public Function recordWriter(ByVal value As RecordWriter) As Builder
                Me.recordWriterValue = value
                Return Me
            End Function

            Public Function inputUrl(ByVal value As InputSplit) As Builder
                Me.inputUrlValue = value
                Return Me
            End Function

            Public Function splitPerReader(ByVal value() As InputSplit) As Builder
                Me.splitPerReaderValue = value
                Return Me
            End Function

            Public Function readersToConcat(ByVal value() As RecordReader) As Builder
                Me.readersToConcatValue = value
                Return Me
            End Function

            Public Function outputUrl(ByVal value As InputSplit) As Builder
                Me.outputUrlValue = value
                Return Me
            End Function

            Public Function callInitRecordReader(ByVal value As Boolean) As Builder
                Me.callInitRecordReaderValue = value
                Return Me
            End Function

            Public Function callInitRecordWriter(ByVal value As Boolean) As Builder
                Me.callInitRecordWriterValue = value
                Return Me
            End Function

            Public Function callInitPartitioner(ByVal value As Boolean) As Builder
                Me.callInitPartitionerValue = value
                Return Me
            End Function

            Public Function configuration(ByVal value As Configuration) As Builder
                Me.configurationValue = value
                Return Me
            End Function

            Public Function configurationsPerReader(ByVal value() As Configuration) As Builder
                Me.configurationsPerReaderValue = value
                Return Me
            End Function

            Public Function partitioner(ByVal value As Partitioner) As Builder
                Me.partitionerValue = value
                Return Me
            End Function

            Public Function batchSize(ByVal value As Integer) As Builder
                Me.batchSizeValue = value
                Return Me
            End Function

            Public Function build() As RecordMapper
                Return New RecordMapper(Me)
            End Function

            Public Overrides Function ToString() As String
                Return "RecordMapper.RecordMapperBuilder(recordReader=" & Objects.toString(recordReaderValue) &
                    ", recordWriter=" & Objects.toString(recordWriterValue) &
                    ", inputUrl=" & Objects.toString(inputUrlValue) &
                    ", outputUrl=" & Objects.toString(outputUrlValue) &
                    ", callInitRecordReader=" & callInitRecordReaderValue &
                    ", callInitRecordWriter=" & callInitRecordWriterValue &
                    ", callInitPartitioner=" & callInitPartitionerValue &
                    ", batchSize=" & batchSizeValue & ")"
            End Function
        End Class
    End Class

End Namespace
