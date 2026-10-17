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
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4vb.common.function

Namespace org.datavec.api.transform.ops

    ''' <summary>Adapts an aggregation over longs to writables (<c>writable.Writable.toLong()</c>).</summary>
    <Serializable>
    Public Class LongWritableOp(Of T)
        Implements IAggregableReduceOp(Of writable.Writable, T)

        Private ReadOnly operation As IAggregableReduceOp(Of Long, T)

        Public Sub New(ByVal operation As IAggregableReduceOp(Of Long, T))
            Me.operation = operation
        End Sub

        Public Function getOperation() As IAggregableReduceOp(Of Long, T)
            Return operation
        End Function

        Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of writable.Writable, T)) Implements IAggregableReduceOp(Of writable.Writable, T).combine
            If TypeOf accu Is LongWritableOp(Of T) Then
                operation.combine(DirectCast(accu, LongWritableOp(Of T)).getOperation())
            Else
                Throw New NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
            End If
        End Sub

        Public Overridable Sub accept(ByVal writable As writable.Writable) Implements Consumer(Of writable.Writable).accept
            operation.accept(writable.toLong())
        End Sub

        Public Overridable Function [get]() As T Implements Supplier(Of T).[get]
            Return operation.get()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other = TryCast(obj, LongWritableOp(Of T))
            Return other IsNot Nothing AndAlso Objects.equals(operation, other.operation)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hashCode(operation)
        End Function

        Public Overrides Function ToString() As String
            Return "LongWritableOp(operation=" & Objects.toString(operation) & ")"
        End Function
    End Class

End Namespace
