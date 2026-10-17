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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.counter

    ''' <summary>
    ''' Analysis counter of a string column: statistics of the string lengths (UTF-16 code units, like Java's
    ''' <c>String.length()</c>) and counts of empty, shortest and longest values.
    ''' </summary>
    ' Java: @AllArgsConstructor @Data
    <Serializable>
    Public Class StringAnalysisCounter
        Implements AnalysisCounter(Of StringAnalysisCounter)

        Private counter As New StatCounter()
        Private countZeroLength As Long = 0
        Private countMinLength As Long = 0
        Private countMaxLength As Long = 0

        Public Sub New()
        End Sub

        Public Sub New(ByVal counter As StatCounter, ByVal countZeroLength As Long, ByVal countMinLength As Long, ByVal countMaxLength As Long)
            Me.counter = counter
            Me.countZeroLength = countZeroLength
            Me.countMinLength = countMinLength
            Me.countMaxLength = countMaxLength
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

        ''' <summary>Shortest length seen; <see cref="Integer.MaxValue"/> while empty (Java's <c>(int) Double.MAX_VALUE</c>).</summary>
        Public Overridable ReadOnly Property MinLengthSeen As Integer
            Get
                Return javaDoubleToInt(counter.Min)
            End Get
        End Property

        ''' <summary>Longest length seen; <see cref="Integer.MinValue"/> while empty (Java's <c>(int) -Double.MAX_VALUE</c>).</summary>
        Public Overridable ReadOnly Property MaxLengthSeen As Integer
            Get
                Return javaDoubleToInt(counter.Max)
            End Get
        End Property

        Public Overridable ReadOnly Property SumLength As Long
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

        Public Overridable Function add(ByVal writable As WritableT) As StringAnalysisCounter Implements AnalysisCounter(Of StringAnalysisCounter).add
            Dim length As Integer = writable.ToString().Length

            If length = 0 Then
                countZeroLength += 1
            End If

            If length = MinLengthSeen Then
                countMinLength += 1
            ElseIf length < MinLengthSeen Then
                countMinLength = 1
            End If

            If length = MaxLengthSeen Then
                countMaxLength += 1
            ElseIf length > MaxLengthSeen Then
                countMaxLength = 1
            End If

            counter.add(CDbl(length))

            Return Me
        End Function

        ''' <summary>
        ''' Merges the other counter into this one. As in Java, this counter's statistics are updated in place and a
        ''' new counter sharing them is returned.
        ''' </summary>
        Public Overridable Function merge(ByVal other As StringAnalysisCounter) As StringAnalysisCounter Implements AnalysisCounter(Of StringAnalysisCounter).merge
            Dim otherMin As Integer = other.MinLengthSeen
            Dim newCountMinLength As Long
            If MinLengthSeen = otherMin Then
                newCountMinLength = countMinLength + other.getCountMinLength()
            ElseIf MinLengthSeen > otherMin Then
                'Keep other, take count from other
                newCountMinLength = other.getCountMinLength()
            Else
                'Keep this min, no change to count
                newCountMinLength = countMinLength
            End If

            Dim otherMax As Integer = other.MaxLengthSeen
            Dim newCountMaxLength As Long
            If MaxLengthSeen = otherMax Then
                newCountMaxLength = countMaxLength + other.getCountMaxLength()
            ElseIf MaxLengthSeen < otherMax Then
                'Keep other, take count from other
                newCountMaxLength = other.getCountMaxLength()
            Else
                'Keep this max, no change to count
                newCountMaxLength = countMaxLength
            End If

            Return New StringAnalysisCounter(counter.merge(other.getCounter()), countZeroLength + other.getCountZeroLength(), newCountMinLength, newCountMaxLength)
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As AnalysisCounter Implements AnalysisCounter.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As AnalysisCounter) As AnalysisCounter Implements AnalysisCounter.merge
            Return merge(DirectCast(other, StringAnalysisCounter))
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getCounter() As StatCounter
            Return counter
        End Function

        Public Sub setCounter(ByVal counter As StatCounter)
            Me.counter = counter
        End Sub

        Public Function getCountZeroLength() As Long
            Return countZeroLength
        End Function

        Public Sub setCountZeroLength(ByVal countZeroLength As Long)
            Me.countZeroLength = countZeroLength
        End Sub

        Public Function getCountMinLength() As Long
            Return countMinLength
        End Function

        Public Sub setCountMinLength(ByVal countMinLength As Long)
            Me.countMinLength = countMinLength
        End Sub

        Public Function getCountMaxLength() As Long
            Return countMaxLength
        End Function

        Public Sub setCountMaxLength(ByVal countMaxLength As Long)
            Me.countMaxLength = countMaxLength
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As StringAnalysisCounter = TryCast(obj, StringAnalysisCounter)
            If other Is Nothing Then Return False
            Return Object.Equals(counter, other.counter) AndAlso countZeroLength = other.countZeroLength AndAlso
                   countMinLength = other.countMinLength AndAlso countMaxLength = other.countMaxLength
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(counter, countZeroLength, countMinLength, countMaxLength)
        End Function

        Public Overrides Function ToString() As String
            Return "StringAnalysisCounter(counter=" & Objects.toString(counter) & ", countZeroLength=" & countZeroLength & ", countMinLength=" & countMinLength & ", countMaxLength=" & countMaxLength & ")"
        End Function
    End Class

End Namespace
