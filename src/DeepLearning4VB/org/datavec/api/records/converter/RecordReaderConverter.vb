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
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter
Imports SequenceRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.SequenceRecordWriter

Namespace org.datavec.api.records.converter

    ''' <summary>Copies every record of a reader to a writer.</summary>
    Public NotInheritable Class RecordReaderConverter

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Write all values from the specified record reader to the specified record writer.
        ''' Closes the record writer on completion
        ''' </summary>
        ''' <param name="reader"> Record reader (source of data) </param>
        ''' <param name="writer"> Record writer (location to write data) </param>
        ''' <exception cref="System.IO.IOException"> If underlying reader/writer throws an exception </exception>
        Public Shared Sub convert(ByVal reader As RecordReader, ByVal writer As RecordWriter)
            convert(reader, writer, True)
        End Sub

        ''' <summary>
        ''' Write all values from the specified record reader to the specified record writer.
        ''' Optionally, close the record writer on completion
        ''' </summary>
        ''' <param name="reader"> Record reader (source of data) </param>
        ''' <param name="writer"> Record writer (location to write data) </param>
        ''' <param name="closeOnCompletion"> if true: close the record writer once complete, via <see cref="RecordWriter.close()"/> </param>
        ''' <exception cref="System.IO.IOException"> If underlying reader/writer throws an exception </exception>
        Public Shared Sub convert(ByVal reader As RecordReader, ByVal writer As RecordWriter, ByVal closeOnCompletion As Boolean)

            If Not reader.hasNext() Then
                Throw New NotSupportedException("Cannot convert RecordReader: reader has no next element")
            End If

            Do While reader.hasNext()
                writer.write(reader.next())
            Loop

            If closeOnCompletion Then
                writer.close()
            End If
        End Sub

        ''' <summary>
        ''' Write all sequences from the specified sequence record reader to the specified sequence record writer.
        ''' Closes the sequence record writer on completion.
        ''' </summary>
        ''' <param name="reader"> Sequence record reader (source of data) </param>
        ''' <param name="writer"> Sequence record writer (location to write data) </param>
        ''' <exception cref="System.IO.IOException"> If underlying reader/writer throws an exception </exception>
        Public Shared Sub convert(ByVal reader As SequenceRecordReader, ByVal writer As SequenceRecordWriter)
            convert(reader, writer, True)
        End Sub

        ''' <summary>
        ''' Write all sequences from the specified sequence record reader to the specified sequence record writer.
        ''' Closes the sequence record writer on completion.
        ''' </summary>
        ''' <param name="reader"> Sequence record reader (source of data) </param>
        ''' <param name="writer"> Sequence record writer (location to write data) </param>
        ''' <param name="closeOnCompletion"> if true: close the record writer once complete, via <see cref="SequenceRecordWriter.close()"/> </param>
        ''' <exception cref="System.IO.IOException"> If underlying reader/writer throws an exception </exception>
        Public Shared Sub convert(ByVal reader As SequenceRecordReader, ByVal writer As SequenceRecordWriter, ByVal closeOnCompletion As Boolean)

            If Not reader.hasNext() Then
                Throw New NotSupportedException("Cannot convert SequenceRecordReader: reader has no next element")
            End If

            Do While reader.hasNext()
                writer.write(reader.sequenceRecord())
            Loop

            If closeOnCompletion Then
                writer.close()
            End If
        End Sub

    End Class

End Namespace
