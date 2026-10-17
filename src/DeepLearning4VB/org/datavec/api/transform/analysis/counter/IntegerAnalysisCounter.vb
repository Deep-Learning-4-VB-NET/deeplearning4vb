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
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.counter

    ''' <summary>
    ''' Analysis counter of an integer (or boolean) column: running statistics, counts of zero, positive, negative,
    ''' minimum and maximum values and a t-digest for the quantiles. As in Java, zero counts as positive, and the
    ''' minimum/maximum of an empty counter are <c>(int) Double.MAX_VALUE</c> = <see cref="Integer.MaxValue"/> and
    ''' <c>(int) -Double.MAX_VALUE</c> = <see cref="Integer.MinValue"/> (Java's saturating casts).
    ''' </summary>
    ' Java: @AllArgsConstructor @Data
    <Serializable>
    Public Class IntegerAnalysisCounter
        Implements AnalysisCounter(Of IntegerAnalysisCounter)

        Private counter As New StatCounter()
        Private countZero As Long = 0
        Private countMinValue As Long = 0
        Private countMaxValue As Long = 0
        Private countPositive As Long = 0
        Private countNegative As Long = 0
        ''' <summary>
        ''' A histogram structure that will record a sketch of a distribution.
        '''
        ''' The compression argument regulates how accuracy should be traded for size? A value of N here
        ''' will give quantile errors almost always less than 3/N with considerably smaller errors expected
        ''' for extreme quantiles. Conversely, you should expect to track about 5 N centroids for this
        ''' accuracy.
        ''' </summary>
        Private digest As TDigest = TDigest.createDigest(100)

        Public Sub New()
        End Sub

        Public Sub New(ByVal counter As StatCounter, ByVal countZero As Long, ByVal countMinValue As Long, ByVal countMaxValue As Long, ByVal countPositive As Long, ByVal countNegative As Long, ByVal digest As TDigest)
            Me.counter = counter
            Me.countZero = countZero
            Me.countMinValue = countMinValue
            Me.countMaxValue = countMaxValue
            Me.countPositive = countPositive
            Me.countNegative = countNegative
            Me.digest = digest
        End Sub

        ''' <summary>Java's <c>(int) d</c>: truncates towards zero, saturates at the Integer range, NaN gives 0.</summary>
        Private Shared Function javaDoubleToInt(ByVal d As Double) As Integer
            If Double.IsNaN(d) Then Return 0
            If d >= Integer.MaxValue Then Return Integer.MaxValue
            If d <= Integer.MinValue Then Return Integer.MinValue
            Return CInt(Math.Truncate(d))
        End Function

        ''' <summary>Java's <c>(long) d</c>: truncates towards zero, saturates at the Long range, NaN gives 0.</summary>
        Private Shared Function javaDoubleToLong(ByVal d As Double) As Long
            If Double.IsNaN(d) Then Return 0L
            If d >= 9.2233720368547758E+18 Then Return Long.MaxValue
            If d <= -9.2233720368547758E+18 Then Return Long.MinValue
            Return CLng(Math.Truncate(d))
        End Function

        Public Overridable ReadOnly Property MinValueSeen As Integer
            Get
                Return javaDoubleToInt(counter.Min)
            End Get
        End Property

        Public Overridable ReadOnly Property MaxValueSeen As Integer
            Get
                Return javaDoubleToInt(counter.Max)
            End Get
        End Property

        Public Overridable ReadOnly Property Sum As Long
            Get
                Return javaDoubleToLong(counter.Sum)
            End Get
        End Property

        Public Overridable ReadOnly Property CountTotal As Long
            Get
                Return counter.Count
            End Get
        End Property

        Public Overridable ReadOnly Property SampleStdev As Double
            Get
                Return counter.getStddev(False)
            End Get
        End Property

        Public Overridable ReadOnly Property Mean As Double
            Get
                Return counter.Mean
            End Get
        End Property

        Public Overridable ReadOnly Property SampleVariance As Double
            Get
                Return counter.getVariance(False)
            End Get
        End Property

        Public Overridable Function add(ByVal writable As WritableT) As IntegerAnalysisCounter Implements AnalysisCounter(Of IntegerAnalysisCounter).add
            Dim value As Integer = writable.toInt()

            If value = 0 Then
                countZero += 1
            End If

            If value = MinValueSeen Then
                countMinValue += 1
            ElseIf value < MinValueSeen Then
                countMinValue = 1
            End If

            If value = MaxValueSeen Then
                countMaxValue += 1
            ElseIf value > MaxValueSeen Then
                countMaxValue = 1
            End If

            If value >= 0 Then
                countPositive += 1
            Else
                countNegative += 1
            End If

            digest.add(CDbl(value))
            counter.add(CDbl(value))

            Return Me
        End Function

        ''' <summary>
        ''' Merges the other counter into this one. As in Java, this counter's statistics and digest are updated in
        ''' place and a new counter sharing them is returned.
        ''' </summary>
        Public Overridable Function merge(ByVal other As IntegerAnalysisCounter) As IntegerAnalysisCounter Implements AnalysisCounter(Of IntegerAnalysisCounter).merge
            Dim otherMin As Integer = other.MinValueSeen
            Dim newCountMinValue As Long
            If MinValueSeen = otherMin Then
                newCountMinValue = countMinValue + other.getCountMinValue()
            ElseIf MinValueSeen > otherMin Then
                'Keep other, take count from other
                newCountMinValue = other.getCountMinValue()
            Else
                'Keep this min, no change to count
                newCountMinValue = countMinValue
            End If

            Dim otherMax As Integer = other.MaxValueSeen
            Dim newCountMaxValue As Long
            If MaxValueSeen = otherMax Then
                newCountMaxValue = countMaxValue + other.getCountMaxValue()
            ElseIf MaxValueSeen < otherMax Then
                'Keep other, take count from other
                newCountMaxValue = other.getCountMaxValue()
            Else
                'Keep this max, no change to count
                newCountMaxValue = countMaxValue
            End If

            digest.add(other.getDigest())

            Return New IntegerAnalysisCounter(counter.merge(other.getCounter()), countZero + other.getCountZero(), newCountMinValue, newCountMaxValue, countPositive + other.getCountPositive(), countNegative + other.getCountNegative(), digest)
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As AnalysisCounter Implements AnalysisCounter.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As AnalysisCounter) As AnalysisCounter Implements AnalysisCounter.merge
            Return merge(DirectCast(other, IntegerAnalysisCounter))
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getCounter() As StatCounter
            Return counter
        End Function

        Public Sub setCounter(ByVal counter As StatCounter)
            Me.counter = counter
        End Sub

        Public Function getCountZero() As Long
            Return countZero
        End Function

        Public Sub setCountZero(ByVal countZero As Long)
            Me.countZero = countZero
        End Sub

        Public Function getCountMinValue() As Long
            Return countMinValue
        End Function

        Public Sub setCountMinValue(ByVal countMinValue As Long)
            Me.countMinValue = countMinValue
        End Sub

        Public Function getCountMaxValue() As Long
            Return countMaxValue
        End Function

        Public Sub setCountMaxValue(ByVal countMaxValue As Long)
            Me.countMaxValue = countMaxValue
        End Sub

        Public Function getCountPositive() As Long
            Return countPositive
        End Function

        Public Sub setCountPositive(ByVal countPositive As Long)
            Me.countPositive = countPositive
        End Sub

        Public Function getCountNegative() As Long
            Return countNegative
        End Function

        Public Sub setCountNegative(ByVal countNegative As Long)
            Me.countNegative = countNegative
        End Sub

        Public Function getDigest() As TDigest
            Return digest
        End Function

        Public Sub setDigest(ByVal digest As TDigest)
            Me.digest = digest
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As IntegerAnalysisCounter = TryCast(obj, IntegerAnalysisCounter)
            If other Is Nothing Then Return False
            Return Object.Equals(counter, other.counter) AndAlso countZero = other.countZero AndAlso countMinValue = other.countMinValue AndAlso
                   countMaxValue = other.countMaxValue AndAlso countPositive = other.countPositive AndAlso countNegative = other.countNegative AndAlso
                   Object.Equals(digest, other.digest)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(counter, countZero, countMinValue, countMaxValue, countPositive, countNegative, digest)
        End Function

        Public Overrides Function ToString() As String
            Return "IntegerAnalysisCounter(counter=" & Objects.toString(counter) & ", countZero=" & countZero & ", countMinValue=" & countMinValue & ", countMaxValue=" & countMaxValue &
                   ", countPositive=" & countPositive & ", countNegative=" & countNegative & ", digest=" & Objects.toString(digest) & ")"
        End Function
    End Class

End Namespace
