Imports System
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

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

Namespace org.nd4vb.linalg.indexing

    ''' <summary>
    ''' An index representing an interval <c>[begin, end)</c> (or <c>[begin, end]</c> when inclusive) with a stride.
    ''' The length is <c>(endInclusive - begin) \ stride + 1</c>.
    ''' <para>
    ''' Java's fields <c>end</c>, <c>stride</c> and <c>length</c> are <see cref="endValue"/>, <see cref="strideValue"/> and
    ''' <see cref="lengthValue"/> (VB does not allow a field and a method with the same name); Lombok's
    ''' <c>@Getter</c> on <c>inclusive</c> is <see cref="isInclusive"/>.
    ''' </para>
    ''' </summary>
    ''' @author Adam Gibson
    Public Class IntervalIndex
        Implements INDArrayIndex

        ''' <summary>The first position.</summary>
        Protected Friend begin As Long
        ''' <summary>The end position (exclusive unless <see cref="inclusive"/>), Java's <c>end</c>.</summary>
        Protected Friend endValue As Long
        ''' <summary>Whether <see cref="endValue"/> is included.</summary>
        Protected Friend inclusive As Boolean
        ''' <summary>The step between positions, Java's <c>stride</c>.</summary>
        Protected Friend strideValue As Long = 1
        ''' <summary>The current position (set to <see cref="begin"/> by <c>init</c>).</summary>
        Protected Friend index As Long = 0
        ''' <summary>The number of positions, Java's <c>length</c>.</summary>
        Protected Friend lengthValue As Long = 0

        ''' <param name="inclusive"> whether to include the last number </param>
        ''' <param name="stride"> the stride for the interval </param>
        Public Sub New(ByVal inclusive As Boolean, ByVal stride As Long)
            Me.inclusive = inclusive
            Me.strideValue = stride
        End Sub

        ''' <summary>Whether the end is included (Lombok <c>@Getter</c>).</summary>
        Public Overridable Function isInclusive() As Boolean
            Return inclusive
        End Function

        Public Overridable Function [end]() As Long Implements INDArrayIndex.end
            Return endValue
        End Function

        Public Overridable Function offset() As Long Implements INDArrayIndex.offset
            Return begin
        End Function

        Public Overridable Function length() As Long Implements INDArrayIndex.length
            Return lengthValue
        End Function

        Public Overridable Function stride() As Long Implements INDArrayIndex.stride
            Return strideValue
        End Function

        ''' <summary>Swaps the begin and the end.</summary>
        Public Overridable Sub reverse() Implements INDArrayIndex.reverse
            Dim oldEnd As Long = endValue
            Dim oldBegin As Long = begin
            Me.endValue = oldBegin
            Me.begin = oldEnd
        End Sub

        ''' <summary>True.</summary>
        Public Overridable ReadOnly Property Interval As Boolean Implements INDArrayIndex.Interval
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Initializes the interval to run from <paramref name="begin"/> (negative values count from the end) to the end
        ''' of dimension <paramref name="dimension"/> of <paramref name="arr"/>.
        ''' </summary>
        Public Overridable Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer) Implements INDArrayIndex.init
            If begin < 0 Then
                begin += arr.size(dimension)
            End If

            Me.begin = begin
            Me.index = begin
            Me.endValue = If(inclusive, arr.size(dimension) + 1, arr.size(dimension))

            'Calculation of length: (endInclusive - begin)/stride + 1
            Dim endInc As Long = arr.size(dimension) - (If(inclusive, 0, 1))
            Me.lengthValue = (endInc - begin) \ strideValue + 1
            Preconditions.checkState(endInc < arr.size(dimension), "Invalid interval: %s on array with shape %ndShape", Me, arr)
        End Sub

        ''' <summary>Initializes the interval over the whole of dimension <paramref name="dimension"/>.</summary>
        Public Overridable Sub init(ByVal arr As INDArray, ByVal dimension As Integer) Implements INDArrayIndex.init
            init(arr, 0, dimension)
        End Sub

        ''' <summary>
        ''' Initializes the interval from <paramref name="begin"/> to <paramref name="end"/>; negative values count back
        ''' from <paramref name="max"/>.
        ''' </summary>
        Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long) Implements INDArrayIndex.init
            If begin < 0 Then
                begin += max
            End If

            If [end] < 0 Then
                [end] += max
            End If
            Me.begin = begin
            Me.index = begin
            Me.endValue = [end]

            Dim endInc As Long = [end] - (If(inclusive, 0, 1))
            Me.lengthValue = (endInc - begin) \ strideValue + 1
        End Sub

        ''' <summary>Initializes the interval from <paramref name="begin"/> to <paramref name="end"/>.</summary>
        ''' <exception cref="ArgumentException">A bound is negative (use an overload that knows the dimension size).</exception>
        Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long) Implements INDArrayIndex.init
            If begin < 0 OrElse [end] < 0 Then
                Throw New ArgumentException("Please pass in an array for negative indices. Unable to determine size for dimension otherwise")
            End If
            Me.begin = begin
            Me.index = begin
            Me.endValue = [end]

            Dim endInc As Long = [end] - (If(inclusive, 0, 1))
            Me.lengthValue = (endInc - begin) \ strideValue + 1
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If Not (TypeOf o Is IntervalIndex) Then
                Return False
            End If

            Dim that As IntervalIndex = DirectCast(o, IntervalIndex)

            If begin <> that.begin Then
                Return False
            End If
            If endValue <> that.endValue Then
                Return False
            End If
            If inclusive <> that.inclusive Then
                Return False
            End If
            If strideValue <> that.strideValue Then
                Return False
            End If
            Return index = that.index
        End Function

        ''' <summary>Java's hash code over begin, end, inclusive, stride and index (Guava's <c>Longs.hashCode</c>).</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = longHash(begin)
            result = 31 * result + longHash(endValue)
            result = 31 * result + (If(inclusive, 1, 0))
            result = 31 * result + longHash(strideValue)
            result = 31 * result + longHash(index)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "Interval(b=" & begin & ",e=" & endValue & ",s=" & strideValue & (If(inclusive, ",inclusive", "")) & ")"
        End Function

        ''' <summary>Guava's <c>Longs.hashCode</c>: <c>(int)(v ^ (v &gt;&gt;&gt; 32))</c>.</summary>
        Private Shared Function longHash(ByVal v As Long) As Integer
            Return Arrays.WrapToInt32(v Xor (v >> 32))
        End Function

    End Class

End Namespace
