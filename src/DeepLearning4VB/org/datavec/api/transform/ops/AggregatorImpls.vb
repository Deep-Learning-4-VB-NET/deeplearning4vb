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
Imports DeepLearning4VB.org.nd4j.common.function
Imports CardinalityMergeException = DeepLearning4VB.com.clearspring.analytics.stream.cardinality.CardinalityMergeException
Imports HyperLogLogPlus = DeepLearning4VB.com.clearspring.analytics.stream.cardinality.HyperLogLogPlus
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports UnsafeWritableInjector = DeepLearning4VB.org.datavec.api.writable.UnsafeWritableInjector

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' The aggregations a reducer can apply to one column, each as a combinable accumulator.
    ''' The Java classes bound their element type to <c>Number</c>; here the numeric accumulators take any
    ''' <c>T</c> and read the elements as boxed Java numbers (<c>Number.doubleValue()</c> etc.), while the
    ''' comparison based ones require <see cref="IComparable(Of T)"/>. "No value yet" (Java: a null field) is
    ''' tracked with a flag so that value types work.
    ''' </summary>
    Public Class AggregatorImpls

        Private Shared Function incompatible(ByVal accu As Object, ByVal expected As Object) As NotSupportedException
            Return New NotSupportedException("Tried to combine() incompatible " & accu.GetType().FullName & " operator where " & expected.GetType().FullName & " expected")
        End Function

        ''' <summary>Keeps the first (non-null) element seen; left-favouring when combined.</summary>
        <Serializable>
        Public Class AggregableFirst(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private elem As T
            Private hasElem As Boolean

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                If Not hasElem Then
                    elem = element
                    hasElem = element IsNot Nothing
                End If
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                ' left-favoring for first
                If accu Is Nothing Then
                    Throw New NotSupportedException("Tried to combine() incompatible null operator where " & Me.GetType().FullName & " expected")
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                If Not hasElem Then Return NullWritable.INSTANCE
                Return UnsafeWritableInjector.inject(elem)
            End Function
        End Class

        ''' <summary>Keeps the last (non-null) element seen; right-favouring when combined.</summary>
        <Serializable>
        Public Class AggregableLast(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private elem As T
            Private hasElem As Boolean
            Private override As writable.Writable = Nothing

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                If element IsNot Nothing Then
                    elem = element
                    hasElem = True
                End If
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                If TypeOf accu Is AggregableLast(Of T) Then
                    override = accu.get() ' right-favoring for last
                Else
                    Throw incompatible(accu, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                If override Is Nothing Then
                    If Not hasElem Then Return NullWritable.INSTANCE
                    Return UnsafeWritableInjector.inject(elem)
                Else
                    Return override
                End If
            End Function
        End Class

        ''' <summary>Sums the (numeric) elements in the widest type seen (int, long, float or double).</summary>
        <Serializable>
        Public Class AggregableSum(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private sum As Object ' a boxed Java Number
            Private initialElement As Object ' this value is ignored and just serves as a subtype indicator

            ''' <summary>Java <c>Number + Number</c> in the wider of the two boxed types.</summary>
            Friend Shared Function addNumbers(ByVal a As Object, ByVal b As Object) As Object
                If TypeOf a Is Double OrElse TypeOf b Is Double Then
                    Return JavaNumbers.doubleValue(a) + JavaNumbers.doubleValue(b)
                ElseIf TypeOf a Is Single OrElse TypeOf b Is Single Then
                    Return JavaNumbers.floatValue(a) + JavaNumbers.floatValue(b)
                ElseIf TypeOf a Is Long OrElse TypeOf b Is Long Then
                    Return JavaNumbers.longValue(a) + JavaNumbers.longValue(b)
                Else
                    Return JavaNumbers.intValue(a) + JavaNumbers.intValue(b)
                End If
            End Function

            ''' <summary>The running sum as a boxed number (Java <c>getSum()</c>).</summary>
            Public Function getSum() As Object
                Return sum
            End Function

            ''' <summary>The first element seen, as a boxed number (Java <c>getInitialElement()</c>).</summary>
            Public Function getInitialElement() As Object
                Return initialElement
            End Function

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                If sum Is Nothing Then
                    sum = element
                    initialElement = element
                Else
                    If initialElement.GetType().IsAssignableFrom(element.GetType()) Then
                        sum = addNumbers(sum, element)
                    End If
                End If
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                Dim accumulator = TryCast(accu, AggregableSum(Of T))
                If accumulator IsNot Nothing Then
                    ' the type of this now becomes that of the union of initialelement
                    If accumulator.initialElement IsNot Nothing AndAlso (initialElement Is Nothing OrElse accumulator.initialElement.GetType().IsAssignableFrom(initialElement.GetType())) Then
                        initialElement = accumulator.initialElement
                    End If
                    If sum Is Nothing Then
                        sum = accumulator.sum
                    ElseIf accumulator.sum IsNot Nothing Then
                        sum = addNumbers(sum, accumulator.sum)
                    End If
                Else
                    Throw incompatible(accu, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                Return UnsafeWritableInjector.inject(sum)
            End Function
        End Class

        ''' <summary>Multiplies the (numeric) elements in the widest type seen (int, long, float or double).</summary>
        <Serializable>
        Public Class AggregableProd(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private prod As Object ' a boxed Java Number
            Private initialElement As Object ' this value is ignored and just serves as a subtype indicator

            ''' <summary>Java <c>Number * Number</c> in the wider of the two boxed types.</summary>
            Friend Shared Function multiplyNumbers(ByVal a As Object, ByVal b As Object) As Object
                If TypeOf a Is Double OrElse TypeOf b Is Double Then
                    Return JavaNumbers.doubleValue(a) * JavaNumbers.doubleValue(b)
                ElseIf TypeOf a Is Single OrElse TypeOf b Is Single Then
                    Return JavaNumbers.floatValue(a) * JavaNumbers.floatValue(b)
                ElseIf TypeOf a Is Long OrElse TypeOf b Is Long Then
                    Return JavaNumbers.longValue(a) * JavaNumbers.longValue(b)
                Else
                    Return JavaNumbers.intValue(a) * JavaNumbers.intValue(b)
                End If
            End Function

            ''' <summary>The running product as a boxed number (Java <c>getProd()</c>).</summary>
            Public Function getProd() As Object
                Return prod
            End Function

            ''' <summary>The first element seen, as a boxed number (Java <c>getInitialElement()</c>).</summary>
            Public Function getInitialElement() As Object
                Return initialElement
            End Function

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                If prod Is Nothing Then
                    prod = element
                    initialElement = element
                Else
                    If initialElement.GetType().IsAssignableFrom(element.GetType()) Then
                        prod = multiplyNumbers(prod, element)
                    End If
                End If
            End Sub

            ''' <summary>
            ''' Combines with another product. The Java original tested for (and read the sum of) an
            ''' <c>AggregableSum</c> here, so two products could never be combined; this port combines products.
            ''' </summary>
            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                Dim accumulator = TryCast(accu, AggregableProd(Of T))
                If accumulator IsNot Nothing Then
                    ' the type of this now becomes that of the union of initialelement
                    If accumulator.initialElement IsNot Nothing AndAlso (initialElement Is Nothing OrElse accumulator.initialElement.GetType().IsAssignableFrom(initialElement.GetType())) Then
                        initialElement = accumulator.initialElement
                    End If
                    If prod Is Nothing Then
                        prod = accumulator.prod
                    ElseIf accumulator.prod IsNot Nothing Then
                        prod = multiplyNumbers(prod, accumulator.prod)
                    End If
                Else
                    Throw incompatible(accu, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                Return UnsafeWritableInjector.inject(prod)
            End Function
        End Class

        ''' <summary>The maximum of the elements.</summary>
        <Serializable>
        Public Class AggregableMax(Of T As IComparable(Of T))
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private max As T
            Private hasMax As Boolean

            ''' <summary>The maximum so far (the default value of <typeparamref name="T"/> if nothing was accepted).</summary>
            Public Function getMax() As T
                Return max
            End Function

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                If Not hasMax OrElse max.CompareTo(element) < 0 Then
                    max = element
                    hasMax = True
                End If
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                Dim other = TryCast(accu, AggregableMax(Of T))
                If other Is Nothing Then
                    Throw incompatible(accu, Me)
                End If
                If other.hasMax AndAlso (Not hasMax OrElse max.CompareTo(other.max) < 0) Then
                    max = other.max
                    hasMax = True
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                If Not hasMax Then Return NullWritable.INSTANCE
                Return UnsafeWritableInjector.inject(max)
            End Function
        End Class

        ''' <summary>The minimum of the elements.</summary>
        <Serializable>
        Public Class AggregableMin(Of T As IComparable(Of T))
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private min As T
            Private hasMin As Boolean

            ''' <summary>The minimum so far (the default value of <typeparamref name="T"/> if nothing was accepted).</summary>
            Public Function getMin() As T
                Return min
            End Function

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                If Not hasMin OrElse min.CompareTo(element) > 0 Then
                    min = element
                    hasMin = True
                End If
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                Dim other = TryCast(accu, AggregableMin(Of T))
                If other Is Nothing Then
                    Throw incompatible(accu, Me)
                End If
                If other.hasMin AndAlso (Not hasMin OrElse min.CompareTo(other.min) > 0) Then
                    min = other.min
                    hasMin = True
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                If Not hasMin Then Return NullWritable.INSTANCE
                Return UnsafeWritableInjector.inject(min)
            End Function
        End Class

        ''' <summary>The range (max - min) of the (numeric) elements, in the element type.</summary>
        <Serializable>
        Public Class AggregableRange(Of T As IComparable(Of T))
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private min As T
            Private max As T
            Private hasValue As Boolean

            Public Function getMin() As T
                Return min
            End Function

            Public Function getMax() As T
                Return max
            End Function

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                If Not hasValue OrElse min.CompareTo(element) > 0 Then
                    min = element
                End If
                If Not hasValue OrElse max.CompareTo(element) < 0 Then
                    max = element
                End If
                hasValue = True
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                Dim other = TryCast(accu, AggregableRange(Of T))
                If other Is Nothing Then
                    Throw incompatible(accu, Me)
                End If
                If other.hasValue Then
                    If Not hasValue OrElse max.CompareTo(other.max) < 0 Then
                        max = other.max
                    End If
                    If Not hasValue OrElse min.CompareTo(other.min) > 0 Then
                        min = other.min
                    End If
                    hasValue = True
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                If Not hasValue Then
                    Throw New NullReferenceException("No value: AggregableRange has not accepted any element")
                End If
                Dim lo As Object = min
                Dim hi As Object = max
                If TypeOf lo Is Long Then
                    Return UnsafeWritableInjector.inject(JavaNumbers.longValue(hi) - JavaNumbers.longValue(lo))
                ElseIf TypeOf lo Is Integer Then
                    Return UnsafeWritableInjector.inject(JavaNumbers.intValue(hi) - JavaNumbers.intValue(lo))
                ElseIf TypeOf lo Is Single Then
                    Return UnsafeWritableInjector.inject(JavaNumbers.floatValue(hi) - JavaNumbers.floatValue(lo))
                ElseIf TypeOf lo Is Double Then
                    Return UnsafeWritableInjector.inject(JavaNumbers.doubleValue(hi) - JavaNumbers.doubleValue(lo))
                ElseIf TypeOf lo Is SByte OrElse TypeOf lo Is Byte OrElse TypeOf lo Is Short Then
                    Return UnsafeWritableInjector.inject(JavaNumbers.intValue(hi) - JavaNumbers.intValue(lo))
                Else
                    Throw New ArgumentException("Wrong type for Aggregable Range operation " & lo.GetType().FullName)
                End If
            End Function
        End Class

        ''' <summary>Counts the elements.</summary>
        <Serializable>
        Public Class AggregableCount(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private count As Long = 0L

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                count += 1L
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                If TypeOf accu Is AggregableCount(Of T) Then
                    count = count + accu.get().toLong()
                Else
                    Throw incompatible(accu, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                Return New LongWritable(count)
            End Function
        End Class

        ''' <summary>The arithmetic mean of the (numeric) elements (Knuth TAOCP vol 2, 3rd edition, page 232).</summary>
        <Serializable>
        Public Class AggregableMean(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private count As Long = 0L
            Private mean As Double = 0R

            Public Function getCount() As Long
                Return count
            End Function

            Public Function getMean() As Double
                Return mean
            End Function

            Public Overridable Sub accept(ByVal n As T) Implements Consumer(Of T).accept
                ' See Knuth TAOCP vol 2, 3rd edition, page 232
                If count = 0 Then
                    count = 1L
                    mean = JavaNumbers.doubleValue(n)
                Else
                    count += 1
                    mean = mean + (JavaNumbers.doubleValue(n) - mean) / count
                End If
            End Sub

            Public Overridable Sub combine(ByVal acc As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                Dim other = TryCast(acc, AggregableMean(Of T))
                If other IsNot Nothing Then
                    Dim cnt As Long = other.getCount()
                    Dim newCount As Long = count + cnt
                    mean = (mean * count + (acc.get().toDouble() * cnt)) / newCount
                    count = newCount
                Else
                    Throw incompatible(acc, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                Return New DoubleWritable(mean)
            End Function
        End Class

        ''' <summary>
        ''' This class offers an aggregable reduce operation for the unbiased standard deviation, i.e. the estimator
        ''' of the square root of the arithmetic mean of squared differences to the mean, corrected with Bessel's correction.
        '''
        ''' See <a href="https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation">https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation</a>
        ''' This is computed with Welford's method for increased numerical stability and aggregability.
        ''' <para>Like the Java original, <see cref="combine"/> merges the counts and means but keeps this accumulator's
        ''' sum of squared deviations (the merged value is computed and then discarded upstream); the DataVec tests
        ''' assert that behaviour.</para>
        ''' </summary>
        <Serializable>
        Public Class AggregableStdDev(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private count As Long = 0L
            Private mean As Double = 0R
            Private variation As Double = 0R

            Public Function getCount() As Long
                Return count
            End Function

            Public Function getMean() As Double
                Return mean
            End Function

            Public Function getVariation() As Double
                Return variation
            End Function

            Public Overridable Sub accept(ByVal n As T) Implements Consumer(Of T).accept
                Dim d As Double = JavaNumbers.doubleValue(n)
                If count = 0 Then
                    count = 1L
                    mean = d
                    variation = 0R
                Else
                    Dim newCount As Long = count + 1
                    Dim newMean As Double = mean + (d - mean) / newCount
                    Dim newvariation As Double = variation + (d - mean) * (d - newMean)
                    count = newCount
                    mean = newMean
                    variation = newvariation
                End If
            End Sub

            Public Overridable Sub combine(ByVal acc As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                If Me.GetType().IsAssignableFrom(acc.GetType()) Then
                    Dim accu As AggregableStdDev(Of T) = DirectCast(acc, AggregableStdDev(Of T))

                    Dim totalCount As Long = count + accu.getCount()
                    Dim totalMean As Double = (accu.getMean() * accu.getCount() + mean * count) / totalCount
                    ' the variance of the union is the sum of variances
                    Dim variance As Double = variation / (count - 1)
                    Dim otherVariance As Double = accu.getVariation() / (accu.getCount() - 1)
                    Dim totalVariation As Double = (variance + otherVariance) * (totalCount - 1)
                    count = totalCount
                    mean = totalMean
                    variation = variation ' sic (Java): totalVariation is not applied
                Else
                    Throw incompatible(acc, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                Return New DoubleWritable(Math.Sqrt(variation / (count - 1)))
            End Function
        End Class

        ''' <summary>
        ''' This class offers an aggregable reduce operation for the biased standard deviation, i.e. the estimator
        ''' of the square root of the arithmetic mean of squared differences to the mean.
        '''
        ''' See <a href="https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation">https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation</a>
        ''' This is computed with Welford's method for increased numerical stability and aggregability.
        ''' </summary>
        <Serializable>
        Public Class AggregableUncorrectedStdDev(Of T)
            Inherits AggregableStdDev(Of T)

            Public Overrides Function [get]() As writable.Writable
                Return New DoubleWritable(Math.Sqrt(Me.getVariation() / Me.getCount()))
            End Function
        End Class

        ''' <summary>
        ''' This class offers an aggregable reduce operation for the unbiased variance, i.e. the estimator
        ''' of the arithmetic mean of squared differences to the mean, corrected with Bessel's correction.
        '''
        ''' See <a href="https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation">https://en.wikipedia.org/wiki/Unbiased_estimation_of_standard_deviation</a>
        ''' This is computed with Welford's method for increased numerical stability and aggregability.
        ''' <para><see cref="combine"/> keeps this accumulator's sum of squared deviations, as in Java (see
        ''' <see cref="AggregableStdDev(Of T)"/>).</para>
        ''' </summary>
        <Serializable>
        Public Class AggregableVariance(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private count As Long = 0L
            Private mean As Double = 0R
            Private variation As Double = 0R

            Public Function getCount() As Long
                Return count
            End Function

            Public Function getMean() As Double
                Return mean
            End Function

            Public Function getVariation() As Double
                Return variation
            End Function

            Public Overridable Sub accept(ByVal n As T) Implements Consumer(Of T).accept
                Dim d As Double = JavaNumbers.doubleValue(n)
                If count = 0 Then
                    count = 1L
                    mean = d
                    variation = 0R
                Else
                    Dim newCount As Long = count + 1
                    Dim newMean As Double = mean + (d - mean) / newCount
                    Dim newvariation As Double = variation + (d - mean) * (d - newMean)
                    count = newCount
                    mean = newMean
                    variation = newvariation
                End If
            End Sub

            Public Overridable Sub combine(ByVal acc As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                If Me.GetType().IsAssignableFrom(acc.GetType()) Then
                    Dim accu As AggregableVariance(Of T) = DirectCast(acc, AggregableVariance(Of T))

                    Dim totalCount As Long = count + accu.getCount()
                    Dim totalMean As Double = (accu.getMean() * accu.getCount() + mean * count) / totalCount
                    ' the variance of the union is the sum of variances
                    Dim variance As Double = variation / (count - 1)
                    Dim otherVariance As Double = accu.getVariation() / (accu.getCount() - 1)
                    Dim totalVariation As Double = (variance + otherVariance) * (totalCount - 1)
                    count = totalCount
                    mean = totalMean
                    variation = variation ' sic (Java): totalVariation is not applied
                Else
                    Throw incompatible(acc, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                Return New DoubleWritable(variation / (count - 1))
            End Function
        End Class

        ''' <summary>
        ''' This class offers an aggregable reduce operation for the population variance, i.e. the uncorrected estimator
        ''' of the arithmetic mean of squared differences to the mean.
        '''
        ''' See <a href="https://en.wikipedia.org/wiki/Variance#Population_variance_and_sample_variance">https://en.wikipedia.org/wiki/Variance#Population_variance_and_sample_variance</a>
        ''' This is computed with Welford's method for increased numerical stability and aggregability.
        ''' </summary>
        <Serializable>
        Public Class AggregablePopulationVariance(Of T)
            Inherits AggregableVariance(Of T)

            Public Overrides Function [get]() As writable.Writable
                Return New DoubleWritable(Me.getVariation() / Me.getCount())
            End Function
        End Class

        ''' <summary>
        ''' This distinct count is based on streamlib's implementation of "HyperLogLog in Practice:
        ''' Algorithmic Engineering of a State of The Art Cardinality Estimation Algorithm", available
        ''' <a href="http://dx.doi.org/10.1145/2452376.2452456">here</a>.
        '''
        ''' The relative accuracy is approximately `1.054 / sqrt(2^p)`. Setting
        ''' a nonzero `sp &gt; p` in HyperLogLogPlus(p, sp) would trigger sparse
        ''' representation of registers, which may reduce the memory consumption
        ''' and increase accuracy when the cardinality is small.
        ''' <para>As in Java, the estimator is created by the field initializer from the default precision
        ''' (0.05), so the precision passed to the constructor only changes the stored <c>p</c>.</para>
        ''' </summary>
        <Serializable>
        Public Class AggregableCountUnique(Of T)
            Implements IAggregableReduceOp(Of T, writable.Writable)

            Private p As Single = 0.05F
            Private hll As New HyperLogLogPlus(CInt(Math.Truncate(Math.Ceiling(2.0 * Math.Log(1.054 / p) / Math.Log(2)))), 0)

            Public Sub New()
            End Sub

            Public Sub New(ByVal precision As Single)
                Me.p = precision
            End Sub

            Public Function getHll() As HyperLogLogPlus
                Return hll
            End Function

            Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                hll.offer(element)
            End Sub

            Public Overridable Sub combine(ByVal acc As IAggregableReduceOp(Of T, writable.Writable)) Implements IAggregableReduceOp(Of T, writable.Writable).combine
                Dim other = TryCast(acc, AggregableCountUnique(Of T))
                If other IsNot Nothing Then
                    Try
                        hll.addAll(other.getHll())
                    Catch e As CardinalityMergeException
                        Throw New Exception(e.Message, e)
                    End Try
                Else
                    Throw incompatible(acc, Me)
                End If
            End Sub

            Public Overridable Function [get]() As writable.Writable Implements Supplier(Of writable.Writable).[get]
                Return New LongWritable(hll.cardinality())
            End Function
        End Class
    End Class

End Namespace
