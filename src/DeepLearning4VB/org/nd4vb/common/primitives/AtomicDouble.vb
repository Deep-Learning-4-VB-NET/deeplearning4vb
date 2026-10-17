Imports System
Imports System.Threading

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

Namespace org.nd4vb.common.primitives

    ''' <summary>
    ''' A double that supports atomic get/set/add/compare-and-set, equivalent to the Guava
    ''' <c>AtomicDouble</c> the Java class extended. Compare-and-set compares the raw bit
    ''' pattern of the doubles, as Guava does, so NaN compares equal to NaN and 0.0 differs from -0.0.
    ''' </summary>
    <Serializable>
    Public Class AtomicDouble
        Private current As Double

        Public Sub New()
            Me.New(0.0)
        End Sub

        Public Sub New(ByVal value As Double)
            current = value
        End Sub

        Public Sub New(ByVal value As Single)
            Me.New(CDbl(value))
        End Sub

        Public Function [get]() As Double
            Return Volatile.Read(current)
        End Function

        Public Sub [set](ByVal newValue As Double)
            Volatile.Write(current, newValue)
        End Sub

        Public Sub lazySet(ByVal newValue As Double)
            [set](newValue)
        End Sub

        Public Property Value As Double
            Get
                Return [get]()
            End Get
            Set(ByVal newValue As Double)
                [set](newValue)
            End Set
        End Property

        ''' <summary>Atomically sets the value and returns the previous value.</summary>
        Public Function getAndSet(ByVal newValue As Double) As Double
            Return Interlocked.Exchange(current, newValue)
        End Function

        ''' <summary>Atomically sets the value to <paramref name="update"/> if the current value is bitwise equal to <paramref name="expect"/>.</summary>
        Public Function compareAndSet(ByVal expect As Double, ByVal update As Double) As Boolean
            Dim previous As Double = Interlocked.CompareExchange(current, update, expect)
            Return BitConverter.DoubleToInt64Bits(previous) = BitConverter.DoubleToInt64Bits(expect)
        End Function

        Public Function weakCompareAndSet(ByVal expect As Double, ByVal update As Double) As Boolean
            Return compareAndSet(expect, update)
        End Function

        ''' <summary>Atomically adds <paramref name="delta"/> and returns the updated value.</summary>
        Public Function addAndGet(ByVal delta As Double) As Double
            Do
                Dim previous As Double = [get]()
                Dim nextValue As Double = previous + delta
                If compareAndSet(previous, nextValue) Then
                    Return nextValue
                End If
            Loop
        End Function

        ''' <summary>Atomically adds <paramref name="delta"/> and returns the previous value.</summary>
        Public Function getAndAdd(ByVal delta As Double) As Double
            Do
                Dim previous As Double = [get]()
                Dim nextValue As Double = previous + delta
                If compareAndSet(previous, nextValue) Then
                    Return previous
                End If
            Loop
        End Function

        Public Function doubleValue() As Double
            Return [get]()
        End Function

        Public Function floatValue() As Single
            Return CSng([get]())
        End Function

        Public Function intValue() As Integer
            Return CInt(Math.Truncate([get]()))
        End Function

        Public Function longValue() As Long
            Return CLng(Math.Truncate([get]()))
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            ' The Guava AtomicDouble extends Number, so the Java class compared against any Number.
            If TypeOf o Is AtomicDouble Then
                Return [get]() = DirectCast(o, AtomicDouble).[get]()
            End If
            If TypeOf o Is Double OrElse TypeOf o Is Single OrElse TypeOf o Is Integer OrElse TypeOf o Is Long OrElse TypeOf o Is Short OrElse TypeOf o Is Byte OrElse TypeOf o Is SByte OrElse TypeOf o Is Decimal Then
                Return [get]() = Convert.ToDouble(o)
            End If
            Return False
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return [get]().GetHashCode()
        End Function

        Public Overrides Function ToString() As String
            Return Arrays.FormatElement([get]())
        End Function
    End Class

End Namespace
