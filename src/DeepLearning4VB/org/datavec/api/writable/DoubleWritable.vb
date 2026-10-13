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
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.io
Imports WritableComparator = DeepLearning4VB.org.datavec.api.io.WritableComparator

Namespace org.datavec.api.writable

    ''' <summary>
    ''' Writable for Double values.
    ''' </summary>
    <Serializable>
    Public Class DoubleWritable
        Implements WritableComparable

        Private valueField As Double = 0.0

        Public Sub New()

        End Sub

        <JsonConstructor>
        Public Sub New(ByVal value As Double)
            [set](value)
        End Sub

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
            valueField = [in].readDouble()
        End Sub

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
            [out].writeShort(WritableType.Double.typeIdx())
        End Sub

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
            [out].writeDouble(valueField)
        End Sub

        Public Overridable Sub [set](ByVal value As Double)
            Me.valueField = value
        End Sub

        Public Overridable Function [get]() As Double
            Return valueField
        End Function

        <JsonPropertyName("value")>
        Public Property Value As Double
            Get
                Return valueField
            End Get
            Set(ByVal value As Double)
                valueField = value
            End Set
        End Property

        Public Overridable Function fuzzyEquals(ByVal o As Writable, ByVal tolerance As Double) As Boolean
            Dim other As Double
            If TypeOf o Is IntWritable Then
                other = DirectCast(o, IntWritable).toDouble()
            ElseIf TypeOf o Is LongWritable Then
                other = DirectCast(o, LongWritable).toDouble()
            ElseIf TypeOf o Is ByteWritable Then
                other = DirectCast(o, ByteWritable).toDouble()
            ElseIf TypeOf o Is DoubleWritable Then
                other = DirectCast(o, DoubleWritable).toDouble()
            ElseIf TypeOf o Is FloatWritable Then
                other = DirectCast(o, FloatWritable).toDouble()
            Else
                Return False
            End If
            Return WritableHelpers.fuzzyEquals(Me.valueField, other, tolerance)
        End Function

        ''' <summary>
        ''' Returns true iff <code>o</code> is a DoubleWritable with the same value.
        ''' </summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If TypeOf o Is DoubleWritable Then
                Dim other As DoubleWritable = DirectCast(o, DoubleWritable)
                Return Me.valueField = other.valueField
            End If
            If TypeOf o Is FloatWritable Then
                Dim other As FloatWritable = DirectCast(o, FloatWritable)
                Dim thisFloat As Single = CSng(Me.valueField)
                Return (Me.valueField = thisFloat AndAlso other.get() = thisFloat)
            Else
                Return False
            End If
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim bits As Long = BitConverter.DoubleToInt64Bits(valueField)
            Return CInt(bits Xor CLng(CULng(bits) >> 32))
        End Function

        Public Overridable Function CompareTo(ByVal o As Object) As Integer Implements IComparable.CompareTo
            Dim other As DoubleWritable = DirectCast(o, DoubleWritable)
            Return (If(valueField < other.valueField, -1, (If(valueField = other.valueField, 0, 1))))
        End Function

        ''' <summary>Java <c>Double.toString</c> formatting (e.g. "1.0", "1.0E10").</summary>
        Public Overrides Function ToString() As String
            Return Arrays.JavaDoubleToString(valueField)
        End Function

        ''' <summary>
        ''' A Comparator optimized for DoubleWritable. </summary>
        Public Class Comparator
            Inherits WritableComparator

            Public Sub New()
                MyBase.New(GetType(DoubleWritable))
            End Sub

            Public Overrides Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
                Dim thisValue As Double = readDouble(b1, s1)
                Dim thatValue As Double = readDouble(b2, s2)
                Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
            End Function
        End Class

        Shared Sub New() ' register this comparator
            WritableComparator.define(GetType(DoubleWritable), New Comparator())
        End Sub

        Public Overridable Function toDouble() As Double Implements Writable.toDouble
            Return valueField
        End Function

        Public Overridable Function toFloat() As Single Implements Writable.toFloat
            Return CSng(valueField)
        End Function

        Public Overridable Function toInt() As Integer Implements Writable.toInt
            Return JavaCastToInt(valueField)
        End Function

        Public Overridable Function toLong() As Long Implements Writable.toLong
            Return JavaCastToLong(valueField)
        End Function

        Public Overridable Shadows Function [getType]() As WritableType Implements Writable.getType
            Return WritableType.Double
        End Function

        ''' <summary>Java <c>(int) d</c>: truncates, saturates at the int range and maps NaN to 0.</summary>
        Friend Shared Function JavaCastToInt(ByVal d As Double) As Integer
            If Double.IsNaN(d) Then Return 0
            If d >= Integer.MaxValue Then Return Integer.MaxValue
            If d <= Integer.MinValue Then Return Integer.MinValue
            Return CInt(Math.Truncate(d))
        End Function

        ''' <summary>Java <c>(long) d</c>: truncates, saturates at the long range and maps NaN to 0.</summary>
        Friend Shared Function JavaCastToLong(ByVal d As Double) As Long
            If Double.IsNaN(d) Then Return 0
            If d >= Long.MaxValue Then Return Long.MaxValue
            If d <= Long.MinValue Then Return Long.MinValue
            Return CLng(Math.Truncate(d))
        End Function
    End Class

End Namespace
