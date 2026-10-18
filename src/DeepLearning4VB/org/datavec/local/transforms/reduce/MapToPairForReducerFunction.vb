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

Imports System.Collections.Generic
Imports System.Text
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports IAssociativeReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.reduce

    ''' <summary>
    ''' Maps a record to a (reduction key, record) pair. The key is the string value of the reducer's key column, or
    ''' the string values of its key columns joined with "_"; a reducer without key columns (a global reduction) uses
    ''' <see cref="GLOBAL_KEY"/> for every record.
    ''' </summary>
    Public Class MapToPairForReducerFunction
        Implements [Function](Of IList(Of WritableT), Pair(Of String, IList(Of WritableT)))

        Public Const GLOBAL_KEY As String = ""

        Private ReadOnly reducer As IAssociativeReducerT

        Public Sub New(ByVal reducer As IAssociativeReducerT)
            Me.reducer = reducer
        End Sub

        Public Overridable Function apply(ByVal writables As IList(Of WritableT)) As Pair(Of String, IList(Of WritableT)) Implements [Function](Of IList(Of WritableT), Pair(Of String, IList(Of WritableT))).apply
            Dim keyColumns As IList(Of String) = reducer.KeyColumns
            If keyColumns Is Nothing Then
                'Global reduction
                Return Pair.of(GLOBAL_KEY, writables)
            Else
                Dim schema As SchemaT = reducer.InputSchema
                Dim key As String
                If keyColumns.Count = 1 Then
                    key = writables(schema.getIndexOfColumn(keyColumns(0))).ToString()
                Else
                    Dim sb As New StringBuilder()
                    For i As Integer = 0 To keyColumns.Count - 1
                        If i > 0 Then
                            sb.Append("_")
                        End If
                        sb.Append(writables(schema.getIndexOfColumn(keyColumns(i))).ToString())
                    Next i
                    key = sb.ToString()
                End If
                Return Pair.of(key, writables)
            End If
        End Function

    End Class

End Namespace
