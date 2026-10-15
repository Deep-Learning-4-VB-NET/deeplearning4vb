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
Imports DeepLearning4VB.org.nd4j.common.function

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' Dispatches the i-th element of each record to the i-th aggregation; the result is the concatenation of
    ''' the aggregations' (list) results.
    ''' </summary>
    <Serializable>
    Public Class DispatchOp(Of T, U)
        Implements IAggregableReduceOp(Of IList(Of T), IList(Of U))

        Private ReadOnly operations As IList(Of IAggregableReduceOp(Of T, IList(Of U)))

        Public Sub New(ByVal operations As IList(Of IAggregableReduceOp(Of T, IList(Of U))))
            If operations Is Nothing Then
                Throw New ArgumentNullException("operations", "operations is marked non-null but is null")
            End If
            Me.operations = operations
        End Sub

        Public Function getOperations() As IList(Of IAggregableReduceOp(Of T, IList(Of U)))
            Return operations
        End Function

        Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of IList(Of T), IList(Of U))) Implements IAggregableReduceOp(Of IList(Of T), IList(Of U)).combine
            Dim other = TryCast(accu, DispatchOp(Of T, U))
            If other IsNot Nothing Then
                Dim otherOps As IList(Of IAggregableReduceOp(Of T, IList(Of U))) = other.getOperations()
                If operations.Count <> otherOps.Count Then
                    Throw New ArgumentException("Tried to combine() incompatible " & Me.GetType().FullName & " operators: received " & otherOps.Count & " operations, expected " & operations.Count)
                End If
                For i As Integer = 0 To Math.Min(operations.Count, otherOps.Count) - 1
                    operations(i).combine(otherOps(i))
                Next i
            Else
                Throw New NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
            End If
        End Sub

        Public Overridable Sub accept(ByVal ts As IList(Of T)) Implements Consumer(Of IList(Of T)).accept
            For i As Integer = 0 To Math.Min(operations.Count, ts.Count) - 1
                operations(i).accept(ts(i))
            Next i
        End Sub

        Public Overridable Function [get]() As IList(Of U) Implements Supplier(Of IList(Of U)).[get]
            Dim res As New List(Of U)()
            For i As Integer = 0 To operations.Count - 1
                res.AddRange(operations(i).get())
            Next i
            Return res
        End Function
    End Class

End Namespace
