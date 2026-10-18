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
Imports DeepLearning4VB.org.datavec.local.transforms.functions
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports JoinT = DeepLearning4VB.org.datavec.api.transform.join.Join
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.join

    ''' <summary>
    ''' Executes a join for one key: given (key, (left records, right records)) from a co-group, returns the joined
    ''' records for the join type of the <see cref="JoinT"/>: the cross product of the left and right records, plus,
    ''' for the outer joins, the records of one side joined with nulls when the other side has no record for the key.
    ''' Records come out in the order of the left records (outer loop) and then of the right records, as in Java.
    ''' </summary>
    <Serializable>
    Public Class ExecuteJoinFromCoGroupFlatMapFunctionAdapter
        Implements FlatMapFunctionAdapter(Of Pair(Of IList(Of WritableT), Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT)))), IList(Of WritableT))

        Private ReadOnly join As JoinT

        Public Sub New(ByVal join As JoinT)
            Me.join = join
        End Sub

        Public Overridable Function [call](ByVal t2 As Pair(Of IList(Of WritableT), Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT))))) As IList(Of IList(Of WritableT)) Implements FlatMapFunctionAdapter(Of Pair(Of IList(Of WritableT), Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT)))), IList(Of WritableT)).call

            Dim leftList As IList(Of IList(Of WritableT)) = t2.Second.First
            Dim rightList As IList(Of IList(Of WritableT)) = t2.Second.Second

            Dim ret As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim jt As JoinT.JoinType = join.getJoinType()
            Select Case jt
                Case JoinT.JoinType.Inner
                    'Return records where key columns appear in BOTH
                    'So if no values from left OR right: no return values
                    For Each jvl As IList(Of WritableT) In leftList
                        For Each jvr As IList(Of WritableT) In rightList
                            ret.Add(join.joinExamples(jvl, jvr))
                        Next jvr
                    Next jvl
                Case JoinT.JoinType.LeftOuter
                    'Return all records from left, even if no corresponding right value (NullWritable in that case)
                    For Each jvl As IList(Of WritableT) In leftList
                        If rightList.Count = 0 Then
                            ret.Add(join.joinExamples(jvl, Nothing))
                        Else
                            For Each jvr As IList(Of WritableT) In rightList
                                ret.Add(join.joinExamples(jvl, jvr))
                            Next jvr
                        End If
                    Next jvl
                Case JoinT.JoinType.RightOuter
                    'Return all records from right, even if no corresponding left value (NullWritable in that case)
                    For Each jvr As IList(Of WritableT) In rightList
                        If leftList.Count = 0 Then
                            ret.Add(join.joinExamples(Nothing, jvr))
                        Else
                            For Each jvl As IList(Of WritableT) In leftList
                                ret.Add(join.joinExamples(jvl, jvr))
                            Next jvl
                        End If
                    Next jvr
                Case JoinT.JoinType.FullOuter
                    'Return all records, even if no corresponding left/right value (NullWritable in that case)
                    If leftList.Count = 0 Then
                        'Only right values
                        For Each jvr As IList(Of WritableT) In rightList
                            ret.Add(join.joinExamples(Nothing, jvr))
                        Next jvr
                    ElseIf rightList.Count = 0 Then
                        'Only left values
                        For Each jvl As IList(Of WritableT) In leftList
                            ret.Add(join.joinExamples(jvl, Nothing))
                        Next jvl
                    Else
                        'Records from both left and right
                        For Each jvl As IList(Of WritableT) In leftList
                            For Each jvr As IList(Of WritableT) In rightList
                                ret.Add(join.joinExamples(jvl, jvr))
                            Next jvr
                        Next jvl
                    End If
            End Select

            Return ret
        End Function

    End Class

End Namespace
