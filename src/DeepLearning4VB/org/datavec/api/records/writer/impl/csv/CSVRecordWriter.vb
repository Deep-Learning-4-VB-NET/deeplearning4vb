Imports System.Collections.Generic
Imports FileRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.FileRecordWriter
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData

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

Namespace org.datavec.api.records.writer.impl.csv

    ''' <summary>Writes records as comma separated lines (no new line after the last line).</summary>
    Public Class CSVRecordWriter
        Inherits FileRecordWriter

        Public Const DEFAULT_DELIMITER As String = ","

        Private ReadOnly delimBytes() As Byte
        Private firstLine As Boolean = True

        Public Sub New()
            delimBytes = encoding.GetBytes(DEFAULT_DELIMITER)
        End Sub

        Public Overrides Function supportsBatch() As Boolean
            Return True
        End Function

        Public Overrides Function writeBatch(ByVal batch As IList(Of IList(Of writable.Writable))) As PartitionMetaData
            For Each record As IList(Of writable.Writable) In batch
                writeRecord(record)
            Next record

            Return PartitionMetaData.builder().numRecordsUpdated(batch.Count).build()
        End Function

        Public Overrides Function write(ByVal record As IList(Of writable.Writable)) As PartitionMetaData
            writeRecord(record)

            Return PartitionMetaData.builder().numRecordsUpdated(1).build()
        End Function

        Private Sub writeRecord(ByVal record As IList(Of writable.Writable))
            If record.Count > 0 Then
                'Add new line before appending lines rather than after (avoids newline after last line)
                If Not firstLine Then
                    [out].write(encoding.GetBytes(NEW_LINE))
                Else
                    firstLine = False
                End If

                Dim count As Integer = 0
                Dim last As Integer = record.Count - 1
                For Each w As writable.Writable In record
                    [out].write(encoding.GetBytes(w.ToString()))
                    If count <> last Then
                        [out].write(delimBytes)
                    End If
                    count += 1
                Next w

                [out].flush()
            End If
        End Sub

    End Class

End Namespace
