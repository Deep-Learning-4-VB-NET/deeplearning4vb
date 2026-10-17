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
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' A <see cref="DispatchOp(Of T, U)"/> over writables where the i-th aggregation only sees the records that
    ''' satisfy the i-th <see cref="condition.Condition"/>.
    ''' </summary>
    <Serializable>
    Public Class DispatchWithConditionOp(Of U)
        Inherits DispatchOp(Of writable.Writable, U)

        Private ReadOnly conditions As IList(Of condition.Condition)

        Public Sub New(ByVal ops As IList(Of IAggregableReduceOp(Of writable.Writable, IList(Of U))), ByVal conds As IList(Of condition.Condition))
            MyBase.New(ops)
            Preconditions.checkNotNull(conds, "Empty condtions for a DispatchWitConditionsOp, use DispatchOp instead")
            Preconditions.checkArgument(ops.Count = conds.Count, "Found conditions size " & conds.Count & " expected " & ops.Count)
            conditions = conds
        End Sub

        Public Function getConditions() As IList(Of condition.Condition)
            Return conditions
        End Function

        Public Overrides Sub accept(ByVal ts As IList(Of writable.Writable))
            For i As Integer = 0 To Math.Min(MyBase.getOperations().Count, ts.Count) - 1
                Dim cond As condition.Condition = conditions(i)
                If cond.condition(ts) Then
                    MyBase.getOperations()(i).accept(ts(i))
                End If
            Next i
        End Sub

    End Class

End Namespace
