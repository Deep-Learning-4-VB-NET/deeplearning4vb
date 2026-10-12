Imports System.Collections.Generic
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports org.datavec.api.transform.ops
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.reduce


	Public Class AggregableReductionUtils

		Private Sub New()
		End Sub


		Public Shared Function reduceColumn(ByVal op As IList(Of ReduceOp), ByVal type As ColumnType, ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))
			Select Case type.innerEnumValue
				Case Integer?
					Return reduceIntColumn(op, ignoreInvalid, metaData)
				Case Long?
					Return reduceLongColumn(op, ignoreInvalid, metaData)
				Case Single?
					Return reduceFloatColumn(op, ignoreInvalid, metaData)
				Case Double?
					Return reduceDoubleColumn(op, ignoreInvalid, metaData)
				Case ColumnType.InnerEnum.String, Categorical
					Return reduceStringOrCategoricalColumn(op, ignoreInvalid, metaData)
				Case ColumnType.InnerEnum.Time
					Return reduceTimeColumn(op, ignoreInvalid, metaData)
				Case ColumnType.InnerEnum.Bytes
					Return reduceBytesColumn(op, ignoreInvalid, metaData)
				Case Else
					Throw New System.NotSupportedException("Unknown or not implemented column type: " & type)
			End Select
		End Function

		Public Shared Function reduceIntColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))

			Dim res As IList(Of IAggregableReduceOp(Of Integer, Writable)) = New List(Of IAggregableReduceOp(Of Integer, Writable))(lop.Count)
			For i As Integer = 0 To lop.Count - 1
				Select Case lop(i)
					Case Prod
						res.Add(New AggregatorImpls.AggregableProd(Of Integer)())
					Case Min
						res.Add(New AggregatorImpls.AggregableMin(Of Integer)())
					Case Max
						res.Add(New AggregatorImpls.AggregableMax(Of Integer)())
					Case Range
						res.Add(New AggregatorImpls.AggregableRange(Of Integer)())
					Case Sum
						res.Add(New AggregatorImpls.AggregableSum(Of Integer)())
					Case Mean
						res.Add(New AggregatorImpls.AggregableMean(Of Integer)())
					Case Stdev
						res.Add(New AggregatorImpls.AggregableStdDev(Of Integer)())
					Case UncorrectedStdDev
						res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Integer)())
					Case Variance
						res.Add(New AggregatorImpls.AggregableVariance(Of Integer)())
					Case PopulationVariance
						res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Integer)())
					Case Count
						res.Add(New AggregatorImpls.AggregableCount(Of Integer)())
					Case CountUnique
						res.Add(New AggregatorImpls.AggregableCountUnique(Of Integer)())
					Case TakeFirst
						res.Add(New AggregatorImpls.AggregableFirst(Of Integer)())
					Case TakeLast
						res.Add(New AggregatorImpls.AggregableLast(Of Integer)())
					Case Else
						Throw New System.NotSupportedException("Unknown or not implemented op: " & lop(i))
				End Select
			Next i
			Dim thisOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = New IntWritableOp(Of Writable, IList(Of Writable))(New AggregableMultiOp(Of )(res))
			If ignoreInvalid Then
				Return New AggregableCheckingOp(Of Writable, IList(Of Writable))(thisOp, metaData)
			Else
				Return thisOp
			End If
		End Function

		Public Shared Function reduceLongColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))

			Dim res As IList(Of IAggregableReduceOp(Of Long, Writable)) = New List(Of IAggregableReduceOp(Of Long, Writable))(lop.Count)
			For i As Integer = 0 To lop.Count - 1
				Select Case lop(i)
					Case Prod
						res.Add(New AggregatorImpls.AggregableProd(Of Long)())
					Case Min
						res.Add(New AggregatorImpls.AggregableMin(Of Long)())
					Case Max
						res.Add(New AggregatorImpls.AggregableMax(Of Long)())
					Case Range
						res.Add(New AggregatorImpls.AggregableRange(Of Long)())
					Case Sum
						res.Add(New AggregatorImpls.AggregableSum(Of Long)())
					Case Stdev
						res.Add(New AggregatorImpls.AggregableStdDev(Of Long)())
					Case UncorrectedStdDev
						res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Long)())
					Case Variance
						res.Add(New AggregatorImpls.AggregableVariance(Of Long)())
					Case PopulationVariance
						res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Long)())
					Case Mean
						res.Add(New AggregatorImpls.AggregableMean(Of Long)())
					Case Count
						res.Add(New AggregatorImpls.AggregableCount(Of Long)())
					Case CountUnique
						res.Add(New AggregatorImpls.AggregableCountUnique(Of Long)())
					Case TakeFirst
						res.Add(New AggregatorImpls.AggregableFirst(Of Long)())
					Case TakeLast
						res.Add(New AggregatorImpls.AggregableLast(Of Long)())
					Case Else
						Throw New System.NotSupportedException("Unknown or not implemented op: " & lop(i))
				End Select
			Next i
			Dim thisOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = New LongWritableOp(Of Writable, IList(Of Writable))(New AggregableMultiOp(Of )(res))
			If ignoreInvalid Then
				Return New AggregableCheckingOp(Of Writable, IList(Of Writable))(thisOp, metaData)
			Else
				Return thisOp
			End If
		End Function

		Public Shared Function reduceFloatColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))

			Dim res As IList(Of IAggregableReduceOp(Of Single, Writable)) = New List(Of IAggregableReduceOp(Of Single, Writable))(lop.Count)
			For i As Integer = 0 To lop.Count - 1
				Select Case lop(i)
					Case Prod
						res.Add(New AggregatorImpls.AggregableProd(Of Single)())
					Case Min
						res.Add(New AggregatorImpls.AggregableMin(Of Single)())
					Case Max
						res.Add(New AggregatorImpls.AggregableMax(Of Single)())
					Case Range
						res.Add(New AggregatorImpls.AggregableRange(Of Single)())
					Case Sum
						res.Add(New AggregatorImpls.AggregableSum(Of Single)())
					Case Mean
						res.Add(New AggregatorImpls.AggregableMean(Of Single)())
					Case Stdev
						res.Add(New AggregatorImpls.AggregableStdDev(Of Single)())
					Case UncorrectedStdDev
						res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Single)())
					Case Variance
						res.Add(New AggregatorImpls.AggregableVariance(Of Single)())
					Case PopulationVariance
						res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Single)())
					Case Count
						res.Add(New AggregatorImpls.AggregableCount(Of Single)())
					Case CountUnique
						res.Add(New AggregatorImpls.AggregableCountUnique(Of Single)())
					Case TakeFirst
						res.Add(New AggregatorImpls.AggregableFirst(Of Single)())
					Case TakeLast
						res.Add(New AggregatorImpls.AggregableLast(Of Single)())
					Case Else
						Throw New System.NotSupportedException("Unknown or not implemented op: " & lop(i))
				End Select
			Next i
			Dim thisOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = New FloatWritableOp(Of Writable, IList(Of Writable))(New AggregableMultiOp(Of )(res))
			If ignoreInvalid Then
				Return New AggregableCheckingOp(Of Writable, IList(Of Writable))(thisOp, metaData)
			Else
				Return thisOp
			End If
		End Function

		Public Shared Function reduceDoubleColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))

			Dim res As IList(Of IAggregableReduceOp(Of Double, Writable)) = New List(Of IAggregableReduceOp(Of Double, Writable))(lop.Count)
			For i As Integer = 0 To lop.Count - 1
				Select Case lop(i)
					Case Prod
						res.Add(New AggregatorImpls.AggregableProd(Of Double)())
					Case Min
						res.Add(New AggregatorImpls.AggregableMin(Of Double)())
					Case Max
						res.Add(New AggregatorImpls.AggregableMax(Of Double)())
					Case Range
						res.Add(New AggregatorImpls.AggregableRange(Of Double)())
					Case Sum
						res.Add(New AggregatorImpls.AggregableSum(Of Double)())
					Case Mean
						res.Add(New AggregatorImpls.AggregableMean(Of Double)())
					Case Stdev
						res.Add(New AggregatorImpls.AggregableStdDev(Of Double)())
					Case UncorrectedStdDev
						res.Add(New AggregatorImpls.AggregableUncorrectedStdDev(Of Double)())
					Case Variance
						res.Add(New AggregatorImpls.AggregableVariance(Of Double)())
					Case PopulationVariance
						res.Add(New AggregatorImpls.AggregablePopulationVariance(Of Double)())
					Case Count
						res.Add(New AggregatorImpls.AggregableCount(Of Double)())
					Case CountUnique
						res.Add(New AggregatorImpls.AggregableCountUnique(Of Double)())
					Case TakeFirst
						res.Add(New AggregatorImpls.AggregableFirst(Of Double)())
					Case TakeLast
						res.Add(New AggregatorImpls.AggregableLast(Of Double)())
					Case Else
						Throw New System.NotSupportedException("Unknown or not implemented op: " & lop(i))
				End Select
			Next i
			Dim thisOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = New DoubleWritableOp(Of Writable, IList(Of Writable))(New AggregableMultiOp(Of )(res))
			If ignoreInvalid Then
				Return New AggregableCheckingOp(Of Writable, IList(Of Writable))(thisOp, metaData)
			Else
				Return thisOp
			End If
		End Function

		Public Shared Function reduceStringOrCategoricalColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))

			Dim res As IList(Of IAggregableReduceOp(Of String, Writable)) = New List(Of IAggregableReduceOp(Of String, Writable))(lop.Count)
			For i As Integer = 0 To lop.Count - 1
				Select Case lop(i)
					Case Count
						res.Add(New AggregatorImpls.AggregableCount(Of String)())
					Case CountUnique
						res.Add(New AggregatorImpls.AggregableCountUnique(Of String)())
					Case TakeFirst
						res.Add(New AggregatorImpls.AggregableFirst(Of String)())
					Case TakeLast
						res.Add(New AggregatorImpls.AggregableLast(Of String)())
					Case Append
						res.Add(New StringAggregatorImpls.AggregableStringAppend())
					Case Prepend
						res.Add(New StringAggregatorImpls.AggregableStringPrepend())
					Case Else
						Throw New System.NotSupportedException("Cannot execute op """ & lop(i) & """ on String/Categorical column " & "(can only perform Append, Prepend, Count, CountUnique, TakeFirst and TakeLast ops on categorical columns)")
				End Select
			Next i

			Dim thisOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = New StringWritableOp(Of Writable, IList(Of Writable))(New AggregableMultiOp(Of )(res))
			If ignoreInvalid Then
				Return New AggregableCheckingOp(Of Writable, IList(Of Writable))(thisOp, metaData)
			Else
				Return thisOp
			End If
		End Function

		Public Shared Function reduceTimeColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))

			Dim res As IList(Of IAggregableReduceOp(Of Long, Writable)) = New List(Of IAggregableReduceOp(Of Long, Writable))(lop.Count)
			For i As Integer = 0 To lop.Count - 1
				Select Case lop(i)
					Case Min
						res.Add(New AggregatorImpls.AggregableMin(Of Long)())
					Case Max
						res.Add(New AggregatorImpls.AggregableMax(Of Long)())
					Case Range
						res.Add(New AggregatorImpls.AggregableRange(Of Long)())
					Case Mean
						res.Add(New AggregatorImpls.AggregableMean(Of Long)())
					Case Stdev
						res.Add(New AggregatorImpls.AggregableStdDev(Of Long)())
					Case Count
						res.Add(New AggregatorImpls.AggregableCount(Of Long)())
					Case CountUnique
						res.Add(New AggregatorImpls.AggregableCountUnique(Of Long)())
					Case TakeFirst
						res.Add(New AggregatorImpls.AggregableFirst(Of Long)())
					Case TakeLast
						res.Add(New AggregatorImpls.AggregableLast(Of Long)())
					Case Else
						Throw New System.NotSupportedException("Reduction op """ & lop(i) & """ not supported on time columns")
				End Select
			Next i
			Dim thisOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = New LongWritableOp(Of Writable, IList(Of Writable))(New AggregableMultiOp(Of )(res))
			If ignoreInvalid Then
				Return New AggregableCheckingOp(Of Writable, IList(Of Writable))(thisOp, metaData)
			Else
				Return thisOp
			End If
		End Function

		Public Shared Function reduceBytesColumn(ByVal lop As IList(Of ReduceOp), ByVal ignoreInvalid As Boolean, ByVal metaData As ColumnMetaData) As IAggregableReduceOp(Of Writable, IList(Of Writable))

			Dim res As IList(Of IAggregableReduceOp(Of SByte, Writable)) = New List(Of IAggregableReduceOp(Of SByte, Writable))(lop.Count)
			For i As Integer = 0 To lop.Count - 1
				Select Case lop(i)
					Case TakeFirst
						res.Add(New AggregatorImpls.AggregableFirst(Of SByte)())
					Case TakeLast
						res.Add(New AggregatorImpls.AggregableLast(Of SByte)())
					Case Else
						Throw New System.NotSupportedException("Cannot execute op """ & lop(i) & """ on Bytes column " & "(can only perform TakeFirst and TakeLast ops on bytes columns)")
				End Select
			Next i
			Dim thisOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = New ByteWritableOp(Of Writable, IList(Of Writable))(New AggregableMultiOp(Of )(res))
			If ignoreInvalid Then
				Return New AggregableCheckingOp(Of Writable, IList(Of Writable))(thisOp, metaData)
			Else
				Return thisOp
			End If
		End Function


	End Class

End Namespace