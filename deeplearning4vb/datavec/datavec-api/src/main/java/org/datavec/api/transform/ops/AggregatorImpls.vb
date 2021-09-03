Imports System
Imports CardinalityMergeException = com.clearspring.analytics.stream.cardinality.CardinalityMergeException
Imports HyperLogLogPlus = com.clearspring.analytics.stream.cardinality.HyperLogLogPlus
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports UnsafeWritableInjector = org.datavec.api.writable.UnsafeWritableInjector
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

Namespace org.datavec.api.transform.ops

	Public Class AggregatorImpls

		<Serializable>
		Public Class AggregableFirst(Of T)
			Implements IAggregableReduceOp(Of T, Writable)

			Friend elem As T = Nothing

			Public Overridable Sub accept(ByVal element As T)
				If elem Is Nothing Then
					elem = element
				End If
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				' left-favoring for first
				If Not (TypeOf accu Is IAggregableReduceOp) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return UnsafeWritableInjector.inject(elem)
			End Function
		End Class

		<Serializable>
		Public Class AggregableLast(Of T)
			Implements IAggregableReduceOp(Of T, Writable)

			Friend elem As T = Nothing
			Friend override As Writable = Nothing

			Public Overridable Sub accept(ByVal element As T)
				If element IsNot Nothing Then
					elem = element
				End If
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				If TypeOf accu Is AggregableLast Then
					override = accu.get() ' right-favoring for last
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				If override Is Nothing Then
					Return UnsafeWritableInjector.inject(elem)
				Else
					Return override
				End If
			End Function
		End Class

		<Serializable>
		Public Class AggregableSum(Of T As Number)
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private Number sum;
			Friend sum As Number
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T initialElement;
			Friend initialElement As T ' this value is ignored and jut serves as a subtype indicator

			Friend Shared Function addNumbers(Of U As Number)(ByVal a As U, ByVal b As U) As Number
				If TypeOf a Is Double? OrElse TypeOf b Is Double? Then
					Return New Double?(a.doubleValue() + b.doubleValue())
				ElseIf TypeOf a Is Single? OrElse TypeOf b Is Single? Then
					Return New Single?(a.floatValue() + b.floatValue())
				ElseIf TypeOf a Is Long? OrElse TypeOf b Is Long? Then
					Return New Long?(a.longValue() + b.longValue())
				Else
					Return New Integer?(a.intValue() + b.intValue())
				End If
			End Function

			Public Overridable Sub accept(ByVal element As T)
				If sum Is Nothing Then
					sum = element
					initialElement = element
				Else
					If initialElement.GetType().IsAssignableFrom(element.GetType()) Then
						sum = addNumbers(sum, element)
					End If
				End If
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				If TypeOf accu Is AggregableSum Then
					Dim accumulator As AggregableSum(Of T) = CType(accu, AggregableSum(Of T))
					' the type of this now becomes that of the union of initialelement
					If accumulator.getInitialElement().GetType().IsAssignableFrom(initialElement.GetType()) Then
						initialElement = accumulator.initialElement
					End If
					sum = addNumbers(sum, accumulator.getSum())
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return UnsafeWritableInjector.inject(sum)
			End Function
		End Class

		<Serializable>
		Public Class AggregableProd(Of T As Number)
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private Number prod;
			Friend prod As Number
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T initialElement;
			Friend initialElement As T ' this value is ignored and jut serves as a subtype indicator

			Friend Shared Function multiplyNumbers(Of U As Number)(ByVal a As U, ByVal b As U) As Number
				If TypeOf a Is Double? OrElse TypeOf b Is Double? Then
					Return New Double?(a.doubleValue() * b.doubleValue())
				ElseIf TypeOf a Is Single? OrElse TypeOf b Is Single? Then
					Return New Single?(a.floatValue() * b.floatValue())
				ElseIf TypeOf a Is Long? OrElse TypeOf b Is Long? Then
					Return New Long?(a.longValue() * b.longValue())
				Else
					Return New Integer?(a.intValue() * b.intValue())
				End If
			End Function

			Public Overridable Sub accept(ByVal element As T)
				If prod Is Nothing Then
					prod = element
					initialElement = element
				Else
					If initialElement.GetType().IsAssignableFrom(element.GetType()) Then
						prod = multiplyNumbers(prod, element)
					End If
				End If
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				If TypeOf accu Is AggregableSum Then
					Dim accumulator As AggregableSum(Of T) = CType(accu, AggregableSum(Of T))
					' the type of this now becomes that of the union of initialelement
					If accumulator.getInitialElement().GetType().IsAssignableFrom(initialElement.GetType()) Then
						initialElement = accumulator.initialElement
					End If
					prod = multiplyNumbers(prod, accumulator.getSum())
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return UnsafeWritableInjector.inject(prod)
			End Function
		End Class

		<Serializable>
		Public Class AggregableMax(Of T As {Number, IComparable(Of T)})
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T max = null;
			Friend max As T = Nothing

			Public Overridable Sub accept(ByVal element As T)
				If max Is Nothing OrElse max.compareTo(element) < 0 Then
					max = element
				End If
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				If max Is Nothing OrElse (TypeOf accu Is AggregableMax AndAlso max.compareTo(CType(accu, AggregableMax(Of T)).getMax()) < 0) Then
					max = CType(accu, AggregableMax(Of T)).getMax()
				ElseIf Not (TypeOf accu Is AggregableMax) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return UnsafeWritableInjector.inject(max)
			End Function
		End Class


		<Serializable>
		Public Class AggregableMin(Of T As {Number, IComparable(Of T)})
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T min = null;
			Friend min As T = Nothing

			Public Overridable Sub accept(ByVal element As T)
				If min Is Nothing OrElse min.compareTo(element) > 0 Then
					min = element
				End If
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				If min Is Nothing OrElse (TypeOf accu Is AggregableMin AndAlso min.compareTo(CType(accu, AggregableMin(Of T)).getMin()) > 0) Then
					min = CType(accu, AggregableMin(Of T)).getMin()
				ElseIf Not (TypeOf accu Is AggregableMin) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return UnsafeWritableInjector.inject(min)
			End Function
		End Class

		<Serializable>
		Public Class AggregableRange(Of T As {Number, IComparable(Of T)})
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T min = null;
			Friend min As T = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T max = null;
			Friend max As T = Nothing

			Public Overridable Sub accept(ByVal element As T)
				If min Is Nothing OrElse min.compareTo(element) > 0 Then
					min = element
				End If
				If max Is Nothing OrElse max.compareTo(element) < 0 Then
					max = element
				End If
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				If max Is Nothing OrElse (TypeOf accu Is AggregableRange AndAlso max.compareTo(CType(accu, AggregableRange(Of T)).getMax()) < 0) Then
					max = CType(accu, AggregableRange(Of T)).getMax()
				End If
				If min Is Nothing OrElse (TypeOf accu Is AggregableRange AndAlso min.compareTo(CType(accu, AggregableRange(Of T)).getMin()) > 0) Then
					min = CType(accu, AggregableRange(Of T)).getMin()
				End If
				If Not (TypeOf accu Is AggregableRange) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub


			Public Overridable Function get() As Writable
				If min.GetType() = GetType(Long) Then
					Return UnsafeWritableInjector.inject(max.longValue() - min.longValue())
				ElseIf min.GetType() = GetType(Integer) Then
					Return UnsafeWritableInjector.inject(max.intValue() - min.intValue())
				ElseIf min.GetType() = GetType(Float) Then
					Return UnsafeWritableInjector.inject(max.floatValue() - min.floatValue())
				ElseIf min.GetType() = GetType(Double) Then
					Return UnsafeWritableInjector.inject(max.doubleValue() - min.doubleValue())
				ElseIf min.GetType() = GetType(Byte) Then
					Return UnsafeWritableInjector.inject(max - min)
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.ArgumentException("Wrong type for Aggregable Range operation " & min.GetType().FullName)
				End If
			End Function
		End Class


		<Serializable>
		Public Class AggregableCount(Of T)
			Implements IAggregableReduceOp(Of T, Writable)

			Friend count As Long? = 0L

			Public Overridable Sub accept(ByVal element As T)
				count += 1L
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
				If TypeOf accu Is AggregableCount Then
					count = count.Value + accu.get().toLong()
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return New LongWritable(count)
			End Function
		End Class

		<Serializable>
		Public Class AggregableMean(Of T As Number)
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private System.Nullable<Long> count = 0L;
			Friend count As Long? = 0L
			Friend mean As Double? = 0R


			Public Overridable Sub accept(ByVal n As T)

				' See Knuth TAOCP vol 2, 3rd edition, page 232
				If count = 0 Then
					count = 1L
					mean = n.doubleValue()
				Else
					count = count.Value + 1
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					mean = mean.Value + (n.doubleValue() - mean.Value) / count.Value
				End If
			End Sub

			Public Overridable Sub combine(Of U As IAggregableReduceOp(Of T, Writable))(ByVal acc As U)
				If TypeOf acc Is AggregableMean Then
					Dim cnt As Long? = CType(acc, AggregableMean(Of T)).getCount()
					Dim newCount As Long? = count.Value + cnt.Value
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					mean = (mean.Value * count.Value + (acc.get().toDouble() * cnt.Value)) / newCount.Value
					count = newCount
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & acc.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return New DoubleWritable(mean)
			End Function
		End Class

		''' <summary>
		''' This class offers an aggregable reduce operation for the unbiased standard deviation, i.e. the estimator
		''' of the square root of the arithmetic mean of squared differences to the mean, corrected with Bessel's correction.
		''' 
		''' See <a href="https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation">https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation</a>
		''' This is computed with Welford's method for increased numerical stability & aggregability.
		''' </summary>
		<Serializable>
		Public Class AggregableStdDev(Of T As Number)
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private System.Nullable<Long> count = 0L;
			Friend count As Long? = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private System.Nullable<Double> mean = 0D;
			Friend mean As Double? = 0R
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private System.Nullable<Double> variation = 0D;
			Friend variation As Double? = 0R


			Public Overridable Sub accept(ByVal n As T)
				If count = 0 Then
					count = 1L
					mean = n.doubleValue()
					variation = 0R
				Else
					Dim newCount As Long? = count.Value + 1
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim newMean As Double? = mean.Value + (n.doubleValue() - mean.Value) / newCount.Value
					Dim newvariation As Double? = variation.Value + (n.doubleValue() - mean.Value) * (n.doubleValue() - newMean.Value)
					count = newCount
					mean = newMean
					variation = newvariation
				End If
			End Sub

			Public Overridable Sub combine(Of U As IAggregableReduceOp(Of T, Writable))(ByVal acc As U)
				If Me.GetType().IsAssignableFrom(acc.GetType()) Then
					Dim accu As AggregableStdDev(Of T) = CType(acc, AggregableStdDev(Of T))

					Dim totalCount As Long? = count.Value + accu.getCount()
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim totalMean As Double? = (accu.getMean() * accu.getCount() + mean.Value * count.Value) / totalCount.Value
					' the variance of the union is the sum of variances
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim variance As Double? = variation.Value / (count.Value - 1)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim otherVariance As Double? = accu.getVariation() / (accu.getCount() - 1)
					Dim totalVariation As Double? = (variance.Value + otherVariance.Value) * (totalCount.Value - 1)
					count = totalCount
					mean = totalMean
					variation = variation
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & acc.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return New DoubleWritable(Math.Sqrt(variation.Value / (count.Value - 1)))
			End Function
		End Class

		''' <summary>
		''' This class offers an aggregable reduce operation for the biased standard deviation, i.e. the estimator
		''' of the square root of the arithmetic mean of squared differences to the mean.
		''' 
		''' See <a href="https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation">https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation</a>
		''' This is computed with Welford's method for increased numerical stability & aggregability.
		''' </summary>
		<Serializable>
		Public Class AggregableUncorrectedStdDev(Of T As Number)
			Inherits AggregableStdDev(Of T)

			Public Overrides Function get() As Writable
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return New DoubleWritable(Math.Sqrt(Me.getVariation() / Me.getCount()))
			End Function
		End Class


		''' <summary>
		''' This class offers an aggregable reduce operation for the unbiased variance, i.e. the estimator
		''' of the arithmetic mean of squared differences to the mean, corrected with Bessel's correction.
		''' 
		''' See <a href="https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation">https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation</a>
		''' This is computed with Welford's method for increased numerical stability & aggregability.
		''' </summary>
		<Serializable>
		Public Class AggregableVariance(Of T As Number)
			Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private System.Nullable<Long> count = 0L;
			Friend count As Long? = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private System.Nullable<Double> mean = 0D;
			Friend mean As Double? = 0R
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private System.Nullable<Double> variation = 0D;
			Friend variation As Double? = 0R


			Public Overridable Sub accept(ByVal n As T)
				If count = 0 Then
					count = 1L
					mean = n.doubleValue()
					variation = 0R
				Else
					Dim newCount As Long? = count.Value + 1
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim newMean As Double? = mean.Value + (n.doubleValue() - mean.Value) / newCount.Value
					Dim newvariation As Double? = variation.Value + (n.doubleValue() - mean.Value) * (n.doubleValue() - newMean.Value)
					count = newCount
					mean = newMean
					variation = newvariation
				End If
			End Sub

			Public Overridable Sub combine(Of U As IAggregableReduceOp(Of T, Writable))(ByVal acc As U)
				If Me.GetType().IsAssignableFrom(acc.GetType()) Then
					Dim accu As AggregableVariance(Of T) = CType(acc, AggregableVariance(Of T))

					Dim totalCount As Long? = count.Value + accu.getCount()
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim totalMean As Double? = (accu.getMean() * accu.getCount() + mean.Value * count.Value) / totalCount.Value
					' the variance of the union is the sum of variances
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim variance As Double? = variation.Value / (count.Value - 1)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim otherVariance As Double? = accu.getVariation() / (accu.getCount() - 1)
					Dim totalVariation As Double? = (variance.Value + otherVariance.Value) * (totalCount.Value - 1)
					count = totalCount
					mean = totalMean
					variation = variation
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & acc.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return New DoubleWritable(variation.Value / (count.Value - 1))
			End Function
		End Class


		''' <summary>
		''' This class offers an aggregable reduce operation for the population variance, i.e. the uncorrected estimator
		''' of the arithmetic mean of squared differences to the mean.
		''' 
		''' See <a href="https://en.wikipedia.org/wiki/Variance#Population_variance_and_sample_variance">https://en.wikipedia.org/wiki/Variance#Population_variance_and_sample_variance</a>
		''' This is computed with Welford's method for increased numerical stability & aggregability.
		''' </summary>
		<Serializable>
		Public Class AggregablePopulationVariance(Of T As Number)
			Inherits AggregableVariance(Of T)

			Public Overrides Function get() As Writable
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return New DoubleWritable(Me.getVariation() / Me.getCount())
			End Function
		End Class

		''' 
		''' <summary>
		''' This distinct count is based on streamlib's implementation of "HyperLogLog in Practice:
		''' Algorithmic Engineering of a State of The Art Cardinality Estimation Algorithm", available
		''' <a href="http://dx.doi.org/10.1145/2452376.2452456">here</a>.
		''' 
		''' The relative accuracy is approximately `1.054 / sqrt(2^p)`. Setting
		''' a nonzero `sp > p` in HyperLogLogPlus(p, sp) would trigger sparse
		''' representation of registers, which may reduce the memory consumption
		''' and increase accuracy when the cardinality is small. </summary>
		''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public static class AggregableCountUnique<T> implements IAggregableReduceOp<T, org.datavec.api.writable.Writable>
		<Serializable>
		Public Class AggregableCountUnique(Of T)
			Implements IAggregableReduceOp(Of T, Writable)

			Friend p As Single = 0.05f
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private com.clearspring.analytics.stream.cardinality.HyperLogLogPlus hll = new com.clearspring.analytics.stream.cardinality.HyperLogLogPlus((int) Math.ceil(2.0 * Math.log(1.054 / p) / Math.log(2)), 0);
			Friend hll As New HyperLogLogPlus(CInt(Math.Truncate(Math.Ceiling(2.0 * Math.Log(1.054 / p) / Math.Log(2)))), 0)

			Public Sub New(ByVal precision As Single)
				Me.p = precision
			End Sub

			Public Overridable Sub accept(ByVal element As T)
				hll.offer(element)
			End Sub

			Public Overridable Sub combine(Of U As IAggregableReduceOp(Of T, Writable))(ByVal acc As U)
				If TypeOf acc Is AggregableCountUnique Then
					Try
						hll.addAll(CType(acc, AggregableCountUnique(Of T)).getHll())
					Catch e As CardinalityMergeException
						Throw New Exception(e)
					End Try
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.NotSupportedException("Tried to combine() incompatible " & acc.GetType().FullName & " operator where " & Me.GetType().FullName & " expected")
				End If
			End Sub

			Public Overridable Function get() As Writable
				Return New LongWritable(hll.cardinality())
			End Function
		End Class
	End Class

End Namespace