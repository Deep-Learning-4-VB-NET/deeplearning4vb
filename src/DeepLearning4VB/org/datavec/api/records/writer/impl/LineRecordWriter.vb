Imports System
Imports System.Collections.Generic
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData
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

Namespace org.datavec.api.records.writer.impl

    ''' <summary>
    ''' Writes the first <see cref="Text"/> of each record as a line. Like the Java version, the text is written
    ''' with <see cref="Text.write"/> (length prefixed), followed by a new line.
    ''' </summary>
    Public Class LineRecordWriter
        Inherits FileRecordWriter

        Public Sub New()
        End Sub

        Public Overrides Function supportsBatch() As Boolean
            Return False
        End Function

        Public Overrides Function write(ByVal record As IList(Of writable.Writable)) As PartitionMetaData
            If record.Count > 0 Then
                Dim t As Text = CType(record(0), Text)
                t.write([out])
                [out].write(encoding.GetBytes(NEW_LINE))
            End If

            Return PartitionMetaData.builder().numRecordsUpdated(1).build()
        End Function

        Public Overrides Function writeBatch(ByVal batch As IList(Of IList(Of writable.Writable))) As PartitionMetaData
            Throw New NotImplementedException("writeBatch is not supported on " & Me.GetType().Name)
        End Function

    End Class

End Namespace
