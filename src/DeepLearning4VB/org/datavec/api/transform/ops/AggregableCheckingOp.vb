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
Imports DeepLearning4VB.org.nd4j.common.function
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' Wraps an aggregation so that only values that are valid for the column (according to its
    ''' <see cref="ColumnMetaData"/>) are passed on. Combining passes the wrapped accumulator of the other
    ''' checking op to this one's accumulator (the Java original passed the checking op itself, which the
    ''' wrapped accumulators reject as incompatible).
    ''' </summary>
    <Serializable>
    Public Class AggregableCheckingOp(Of T)
        Implements IAggregableReduceOp(Of writable.Writable, T)

        Private ReadOnly operation As IAggregableReduceOp(Of writable.Writable, T)
        Private ReadOnly metaData As ColumnMetaData

        Public Sub New(ByVal operation As IAggregableReduceOp(Of writable.Writable, T), ByVal metaData As ColumnMetaData)
            Me.operation = operation
            Me.metaData = metaData
        End Sub

        Public Function getOperation() As IAggregableReduceOp(Of writable.Writable, T)
            Return operation
        End Function

        Public Function getMetaData() As ColumnMetaData
            Return metaData
        End Function

        Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of writable.Writable, T)) Implements IAggregableReduceOp(Of writable.Writable, T).combine
            Dim accumulator = TryCast(accu, AggregableCheckingOp(Of T))
            If accumulator IsNot Nothing Then
                If metaData.ColumnType <> accumulator.getMetaData().getColumnType() Then
                    Throw New ArgumentException("Invalid merge with operation on " & accumulator.getMetaData().getName() & " of type " & accumulator.getMetaData().getColumnType().ToString() & " expected " & metaData.Name & " of type " & metaData.ColumnType.ToString())
                Else
                    operation.combine(accumulator.getOperation())
                End If
            Else
                Throw New NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
            End If
        End Sub

        Public Overridable Sub accept(ByVal writable As writable.Writable) Implements Consumer(Of writable.Writable).accept
            If metaData.isValid(writable) Then
                operation.accept(writable)
            End If
        End Sub

        Public Overridable Function [get]() As T Implements Supplier(Of T).[get]
            Return operation.get()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other = TryCast(obj, AggregableCheckingOp(Of T))
            If other Is Nothing Then Return False
            Return Objects.equals(operation, other.operation) AndAlso Objects.equals(metaData, other.metaData)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(operation, metaData)
        End Function

        Public Overrides Function ToString() As String
            Return "AggregableCheckingOp(operation=" & Objects.toString(operation) & ", metaData=" & Objects.toString(metaData) & ")"
        End Function
    End Class

End Namespace
