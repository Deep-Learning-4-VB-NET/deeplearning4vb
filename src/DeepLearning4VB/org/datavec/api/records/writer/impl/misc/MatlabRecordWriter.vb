Imports System
Imports System.Collections.Generic
Imports System.Text
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

Namespace org.datavec.api.records.writer.impl.misc

    ''' <summary>Writes records as space separated lines (Matlab ASCII format).</summary>
    Public Class MatlabRecordWriter
        Inherits FileRecordWriter

        Public Sub New()
        End Sub

        Public Overrides Function supportsBatch() As Boolean
            Return False
        End Function

        Public Overrides Function write(ByVal record As IList(Of writable.Writable)) As PartitionMetaData
            Dim result As New StringBuilder()

            Dim count As Integer = 0
            For Each w As writable.Writable In record
                ' attributes
                If count > 0 Then
                    Dim tabs As Boolean = False
                    result.Append(If(tabs, vbTab, " "))
                End If
                result.Append(w.ToString())
                count += 1
            Next w

            [out].write(encoding.GetBytes(result.ToString()))
            [out].write(encoding.GetBytes(NEW_LINE))

            Return PartitionMetaData.builder().numRecordsUpdated(1).build()
        End Function

        Public Overrides Function writeBatch(ByVal batch As IList(Of IList(Of writable.Writable))) As PartitionMetaData
            Throw New NotImplementedException("writeBatch is not supported on " & Me.GetType().Name)
        End Function

    End Class

End Namespace
