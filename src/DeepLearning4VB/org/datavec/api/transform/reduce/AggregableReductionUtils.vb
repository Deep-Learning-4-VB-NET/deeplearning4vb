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
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce

    ''' <summary>
    ''' Builds the accumulators that apply a list of <see cref="ReduceOp"/>s to one column of a given
    ''' <see cref="ColumnType"/>. The result of each accumulator is the list of the op results, in order.
    ''' With <c>ignoreInvalid</c> the values that are not valid for the column metadata are skipped
    ''' (<see cref="AggregableCheckingOp(Of T)"/>); otherwise an unparsable value makes the accumulator throw
    ''' (<see cref="JavaCompat.NumberFormatException"/> for text in a numeric column).
    ''' </summary>
    Public Class AggregableReductionUtils

        Private Sub New()
        End Sub

        ''' <summary>The accumulator for the ops on a column of the given type.</summary>
        ''' <exception cref="NotSupportedException">for column types without reductions (Boolean, NDArray)</exception>
        Public Shared Function reduceColumn(ByVal op As IList(Of ReduceOp), ByVal type As ColumnType, ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Select Case type
                Case ColumnType.Integer
                    Return reduceIntColumn(op, ignoreInvalid, metaData)
                Case ColumnType.Long
                    Return reduceLongColumn(op, ignoreInvalid, metaData)
                Case ColumnType.Float
                    Return reduceFloatColumn(op, ignoreInvalid, metaData)
                Case ColumnType.Double
                    Return reduceDoubleColumn(op, ignoreInvalid, metaData)
                Case ColumnType.String, ColumnType.Categorical
                    Return reduceStringOrCategoricalColumn(op, ignoreInvalid, metaData)
                Case ColumnType.Time
                    Return reduceTimeColumn(op, ignoreInvalid, metaData)
                Case ColumnType.Bytes
                    Return reduceBytesColumn(op, ignoreInvalid, metaData)
                Case Else
                    Throw New NotSupportedException("Unknown or not implemented column type: " & type.ToString())
            End Select
        End Function

        Public Shared Function reduceIntColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Dim res As IList(Of IAggregableReduceOp(Of Integer, WritableT)) = New List(Of IAggregableReduceOp(Of Integer, WritableT))(lop.Count)
            For i As Integer = 0 To lop.Count - 1
                Select Case lop(i)
                    Case ReduceOp.Prod
                        res.Add(New AggregatorImpls.AggregableProd(Of Integer)())
                    Case ReduceOp.Min
                        res.Add(New AggregatorImpls.AggregableMin(Of Integer)())
                    Case ReduceOp.Max
                        res.Add(New AggregatorImpls.AggregableMax(Of Integer)())
                    Case ReduceOp.Range
                        res.Add(New AggregatorImpls.AggregableRange(Of Integer)())
                    Case ReduceOp.Sum
                        res.Add(New AggregatorImpls.AggregableSum(Of Integer)())
                    Case ReduceOp.Mean
                        res.Add(New AggregatorImpls.AggregableMean(Of Integer)())
                    Case ReduceOp.Stdev
                        res.Add(New AggregatorImpls.AggregableStdDev(Of Integer)())
                    Case ReduceOp.UncorrectedStdDev
                        res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Integer)())
                    Case ReduceOp.Variance
                        res.Add(New AggregatorImpls.AggregableVariance(Of Integer)())
                    Case ReduceOp.PopulationVariance
                        res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Integer)())
                    Case ReduceOp.Count
                        res.Add(New AggregatorImpls.AggregableCount(Of Integer)())
                    Case ReduceOp.CountUnique
                        res.Add(New AggregatorImpls.AggregableCountUnique(Of Integer)())
                    Case ReduceOp.TakeFirst
                        res.Add(New AggregatorImpls.AggregableFirst(Of Integer)())
                    Case ReduceOp.TakeLast
                        res.Add(New AggregatorImpls.AggregableLast(Of Integer)())
                    Case Else
                        Throw New NotSupportedException("Unknown or not implemented op: " & lop(i).ToString())
                End Select
            Next i
            Dim thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New IntWritableOp(Of IList(Of WritableT))(New AggregableMultiOp(Of Integer)(res))
            Return withChecking(thisOp, ignoreInvalid, metaData)
        End Function

        Public Shared Function reduceLongColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Dim res As IList(Of IAggregableReduceOp(Of Long, WritableT)) = New List(Of IAggregableReduceOp(Of Long, WritableT))(lop.Count)
            For i As Integer = 0 To lop.Count - 1
                Select Case lop(i)
                    Case ReduceOp.Prod
                        res.Add(New AggregatorImpls.AggregableProd(Of Long)())
                    Case ReduceOp.Min
                        res.Add(New AggregatorImpls.AggregableMin(Of Long)())
                    Case ReduceOp.Max
                        res.Add(New AggregatorImpls.AggregableMax(Of Long)())
                    Case ReduceOp.Range
                        res.Add(New AggregatorImpls.AggregableRange(Of Long)())
                    Case ReduceOp.Sum
                        res.Add(New AggregatorImpls.AggregableSum(Of Long)())
                    Case ReduceOp.Stdev
                        res.Add(New AggregatorImpls.AggregableStdDev(Of Long)())
                    Case ReduceOp.UncorrectedStdDev
                        res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Long)())
                    Case ReduceOp.Variance
                        res.Add(New AggregatorImpls.AggregableVariance(Of Long)())
                    Case ReduceOp.PopulationVariance
                        res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Long)())
                    Case ReduceOp.Mean
                        res.Add(New AggregatorImpls.AggregableMean(Of Long)())
                    Case ReduceOp.Count
                        res.Add(New AggregatorImpls.AggregableCount(Of Long)())
                    Case ReduceOp.CountUnique
                        res.Add(New AggregatorImpls.AggregableCountUnique(Of Long)())
                    Case ReduceOp.TakeFirst
                        res.Add(New AggregatorImpls.AggregableFirst(Of Long)())
                    Case ReduceOp.TakeLast
                        res.Add(New AggregatorImpls.AggregableLast(Of Long)())
                    Case Else
                        Throw New NotSupportedException("Unknown or not implemented op: " & lop(i).ToString())
                End Select
            Next i
            Dim thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New LongWritableOp(Of IList(Of WritableT))(New AggregableMultiOp(Of Long)(res))
            Return withChecking(thisOp, ignoreInvalid, metaData)
        End Function

        Public Shared Function reduceFloatColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Dim res As IList(Of IAggregableReduceOp(Of Single, WritableT)) = New List(Of IAggregableReduceOp(Of Single, WritableT))(lop.Count)
            For i As Integer = 0 To lop.Count - 1
                Select Case lop(i)
                    Case ReduceOp.Prod
                        res.Add(New AggregatorImpls.AggregableProd(Of Single)())
                    Case ReduceOp.Min
                        res.Add(New AggregatorImpls.AggregableMin(Of Single)())
                    Case ReduceOp.Max
                        res.Add(New AggregatorImpls.AggregableMax(Of Single)())
                    Case ReduceOp.Range
                        res.Add(New AggregatorImpls.AggregableRange(Of Single)())
                    Case ReduceOp.Sum
                        res.Add(New AggregatorImpls.AggregableSum(Of Single)())
                    Case ReduceOp.Mean
                        res.Add(New AggregatorImpls.AggregableMean(Of Single)())
                    Case ReduceOp.Stdev
                        res.Add(New AggregatorImpls.AggregableStdDev(Of Single)())
                    Case ReduceOp.UncorrectedStdDev
                        res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Single)())
                    Case ReduceOp.Variance
                        res.Add(New AggregatorImpls.AggregableVariance(Of Single)())
                    Case ReduceOp.PopulationVariance
                        res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Single)())
                    Case ReduceOp.Count
                        res.Add(New AggregatorImpls.AggregableCount(Of Single)())
                    Case ReduceOp.CountUnique
                        res.Add(New AggregatorImpls.AggregableCountUnique(Of Single)())
                    Case ReduceOp.TakeFirst
                        res.Add(New AggregatorImpls.AggregableFirst(Of Single)())
                    Case ReduceOp.TakeLast
                        res.Add(New AggregatorImpls.AggregableLast(Of Single)())
                    Case Else
                        Throw New NotSupportedException("Unknown or not implemented op: " & lop(i).ToString())
                End Select
            Next i
            Dim thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New FloatWritableOp(Of IList(Of WritableT))(New AggregableMultiOp(Of Single)(res))
            Return withChecking(thisOp, ignoreInvalid, metaData)
        End Function

        Public Shared Function reduceDoubleColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Dim res As IList(Of IAggregableReduceOp(Of Double, WritableT)) = New List(Of IAggregableReduceOp(Of Double, WritableT))(lop.Count)
            For i As Integer = 0 To lop.Count - 1
                Select Case lop(i)
                    Case ReduceOp.Prod
                        res.Add(New AggregatorImpls.AggregableProd(Of Double)())
                    Case ReduceOp.Min
                        res.Add(New AggregatorImpls.AggregableMin(Of Double)())
                    Case ReduceOp.Max
                        res.Add(New AggregatorImpls.AggregableMax(Of Double)())
                    Case ReduceOp.Range
                        res.Add(New AggregatorImpls.AggregableRange(Of Double)())
                    Case ReduceOp.Sum
                        res.Add(New AggregatorImpls.AggregableSum(Of Double)())
                    Case ReduceOp.Mean
                        res.Add(New AggregatorImpls.AggregableMean(Of Double)())
                    Case ReduceOp.Stdev
                        res.Add(New AggregatorImpls.AggregableStdDev(Of Double)())
                    Case ReduceOp.UncorrectedStdDev
                        res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Double)())
                    Case ReduceOp.Variance
                        res.Add(New AggregatorImpls.AggregableVariance(Of Double)())
                    Case ReduceOp.PopulationVariance
                        res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Double)())
                    Case ReduceOp.Count
                        res.Add(New AggregatorImpls.AggregableCount(Of Double)())
                    Case ReduceOp.CountUnique
                        res.Add(New AggregatorImpls.AggregableCountUnique(Of Double)())
                    Case ReduceOp.TakeFirst
                        res.Add(New AggregatorImpls.AggregableFirst(Of Double)())
                    Case ReduceOp.TakeLast
                        res.Add(New AggregatorImpls.AggregableLast(Of Double)())
                    Case Else
                        Throw New NotSupportedException("Unknown or not implemented op: " & lop(i).ToString())
                End Select
            Next i
            Dim thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New DoubleWritableOp(Of IList(Of WritableT))(New AggregableMultiOp(Of Double)(res))
            Return withChecking(thisOp, ignoreInvalid, metaData)
        End Function

        Public Shared Function reduceStringOrCategoricalColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Dim res As IList(Of IAggregableReduceOp(Of String, WritableT)) = New List(Of IAggregableReduceOp(Of String, WritableT))(lop.Count)
            For i As Integer = 0 To lop.Count - 1
                Select Case lop(i)
                    Case ReduceOp.Count
                        res.Add(New AggregatorImpls.AggregableCount(Of String)())
                    Case ReduceOp.CountUnique
                        res.Add(New AggregatorImpls.AggregableCountUnique(Of String)())
                    Case ReduceOp.TakeFirst
                        res.Add(New AggregatorImpls.AggregableFirst(Of String)())
                    Case ReduceOp.TakeLast
                        res.Add(New AggregatorImpls.AggregableLast(Of String)())
                    Case ReduceOp.Append
                        res.Add(New StringAggregatorImpls.AggregableStringAppend())
                    Case ReduceOp.Prepend
                        res.Add(New StringAggregatorImpls.AggregableStringPrepend())
                    Case Else
                        Throw New NotSupportedException("Cannot execute op """ & lop(i).ToString() & """ on String/Categorical column " & "(can only perform Append, Prepend, Count, CountUnique, TakeFirst and TakeLast ops on categorical columns)")
                End Select
            Next i
            Dim thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New StringWritableOp(Of IList(Of WritableT))(New AggregableMultiOp(Of String)(res))
            Return withChecking(thisOp, ignoreInvalid, metaData)
        End Function

        Public Shared Function reduceTimeColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Dim res As IList(Of IAggregableReduceOp(Of Long, WritableT)) = New List(Of IAggregableReduceOp(Of Long, WritableT))(lop.Count)
            For i As Integer = 0 To lop.Count - 1
                Select Case lop(i)
                    Case ReduceOp.Min
                        res.Add(New AggregatorImpls.AggregableMin(Of Long)())
                    Case ReduceOp.Max
                        res.Add(New AggregatorImpls.AggregableMax(Of Long)())
                    Case ReduceOp.Range
                        res.Add(New AggregatorImpls.AggregableRange(Of Long)())
                    Case ReduceOp.Mean
                        res.Add(New AggregatorImpls.AggregableMean(Of Long)())
                    Case ReduceOp.Stdev
                        res.Add(New AggregatorImpls.AggregableStdDev(Of Long)())
                    Case ReduceOp.Count
                        res.Add(New AggregatorImpls.AggregableCount(Of Long)())
                    Case ReduceOp.CountUnique
                        res.Add(New AggregatorImpls.AggregableCountUnique(Of Long)())
                    Case ReduceOp.TakeFirst
                        res.Add(New AggregatorImpls.AggregableFirst(Of Long)())
                    Case ReduceOp.TakeLast
                        res.Add(New AggregatorImpls.AggregableLast(Of Long)())
                    Case Else
                        Throw New NotSupportedException("Reduction op """ & lop(i).ToString() & """ not supported on time columns")
                End Select
            Next i
            Dim thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New LongWritableOp(Of IList(Of WritableT))(New AggregableMultiOp(Of Long)(res))
            Return withChecking(thisOp, ignoreInvalid, metaData)
        End Function

        Public Shared Function reduceBytesColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            Dim res As IList(Of IAggregableReduceOp(Of SByte, WritableT)) = New List(Of IAggregableReduceOp(Of SByte, WritableT))(lop.Count)
            For i As Integer = 0 To lop.Count - 1
                Select Case lop(i)
                    Case ReduceOp.TakeFirst
                        res.Add(New AggregatorImpls.AggregableFirst(Of SByte)())
                    Case ReduceOp.TakeLast
                        res.Add(New AggregatorImpls.AggregableLast(Of SByte)())
                    Case Else
                        Throw New NotSupportedException("Cannot execute op """ & lop(i).ToString() & """ on Bytes column " & "(can only perform TakeFirst and TakeLast ops on bytes columns)")
                End Select
            Next i
            Dim thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New ByteWritableOp(Of IList(Of WritableT))(New AggregableMultiOp(Of SByte)(res))
            Return withChecking(thisOp, ignoreInvalid, metaData)
        End Function

        ''' <summary>Wraps the op in an <see cref="AggregableCheckingOp(Of T)"/> when invalid values are to be ignored.</summary>
        Private Shared Function withChecking(ByVal thisOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of WritableT, IList(Of WritableT))
            If ignoreInvalid Then
                Return New AggregableCheckingOp(Of IList(Of WritableT))(thisOp, metaData)
            Else
                Return thisOp
            End If
        End Function

    End Class

End Namespace
