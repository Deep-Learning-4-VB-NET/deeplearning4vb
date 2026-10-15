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
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4j.common.function

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' Applies several aggregations to the same column at once; the result is the list of their results.
    ''' </summary>
    <Serializable>
    Public Class AggregableMultiOp(Of T)
        Implements IAggregableReduceOp(Of T, IList(Of writable.Writable))

        Private ReadOnly operations As IList(Of IAggregableReduceOp(Of T, writable.Writable))

        Public Sub New(ByVal operations As IList(Of IAggregableReduceOp(Of T, writable.Writable)))
            If operations Is Nothing Then
                Throw New ArgumentNullException("operations", "operations is marked non-null but is null")
            End If
            Me.operations = operations
        End Sub

        Public Function getOperations() As IList(Of IAggregableReduceOp(Of T, writable.Writable))
            Return operations
        End Function

        Public Overridable Sub accept(ByVal t As T) Implements Consumer(Of T).accept
            For i As Integer = 0 To operations.Count - 1
                operations(i).accept(t)
            Next i
        End Sub

        Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, IList(Of writable.Writable))) Implements IAggregableReduceOp(Of T, IList(Of writable.Writable)).combine
            Dim accumulator = TryCast(accu, AggregableMultiOp(Of T))
            If accumulator IsNot Nothing Then
                Dim otherAccumulators As IList(Of IAggregableReduceOp(Of T, writable.Writable)) = accumulator.getOperations()
                If operations.Count <> otherAccumulators.Count Then
                    Throw New ArgumentException("Tried to combine() incompatible " & Me.GetType().FullName & " operators: received " & otherAccumulators.Count & " operations, expected " & operations.Count)
                End If
                For i As Integer = 0 To operations.Count - 1
                    operations(i).combine(otherAccumulators(i))
                Next i
            Else
                Throw New NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
            End If
        End Sub

        Public Overridable Function [get]() As IList(Of writable.Writable) Implements Supplier(Of IList(Of writable.Writable)).[get]
            Dim res As IList(Of writable.Writable) = New List(Of writable.Writable)(operations.Count)
            For i As Integer = 0 To operations.Count - 1
                res.Add(operations(i).get())
            Next i
            Return res
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other = TryCast(obj, AggregableMultiOp(Of T))
            If other Is Nothing Then Return False
            Return Arrays.equals(New List(Of IAggregableReduceOp(Of T, writable.Writable))(operations).ToArray(), New List(Of IAggregableReduceOp(Of T, writable.Writable))(other.operations).ToArray())
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Arrays.hashCode(New List(Of IAggregableReduceOp(Of T, writable.Writable))(operations).ToArray())
        End Function

        Public Overrides Function ToString() As String
            Return "AggregableMultiOp(operations=" & Arrays.toString(New List(Of IAggregableReduceOp(Of T, writable.Writable))(operations).ToArray()) & ")"
        End Function

    End Class

End Namespace
