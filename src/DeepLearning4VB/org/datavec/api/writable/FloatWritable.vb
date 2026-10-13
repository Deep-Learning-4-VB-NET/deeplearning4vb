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

    ''' <summary>Writable for float values.</summary>
    <Serializable>
    Public Class FloatWritable
        Implements WritableComparable

        Private valueField As Single

        Public Sub New()
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal value As Single)
            [set](value)
        End Sub

        ''' <summary>
        ''' Set the value of this FloatWritable. </summary>
        Public Overridable Sub [set](ByVal value As Single)
            Me.valueField = value
        End Sub

        ''' <summary>
        ''' Return the value of this FloatWritable. </summary>
        Public Overridable Function [get]() As Single
            Return valueField
        End Function

        <JsonPropertyName("value")>
        Public Property Value As Single
            Get
                Return valueField
            End Get
            Set(ByVal value As Single)
                valueField = value
            End Set
        End Property

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
            valueField = [in].readFloat()
        End Sub

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
            [out].writeShort(WritableType.Float.typeIdx())
        End Sub

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
            [out].writeFloat(valueField)
        End Sub

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
        ''' Returns true iff <code>o</code> is a FloatWritable with the same value. </summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If TypeOf o Is FloatWritable Then
                Dim other As FloatWritable = DirectCast(o, FloatWritable)
                Return Me.valueField = other.valueField
            End If
            If TypeOf o Is DoubleWritable Then
                Dim other As DoubleWritable = DirectCast(o, DoubleWritable)
                Dim otherFloat As Single = CSng(other.get())
                Return (other.get() = otherFloat AndAlso Me.valueField = otherFloat)
            Else
                Return False
            End If
        End Function

        Public Overrides Function GetHashCode() As Integer
            ' defer to Float.hashCode, which does what we mean for it to do
            Return BitConverter.ToInt32(BitConverter.GetBytes(valueField), 0)
        End Function

        ''' <summary>
        ''' Compares two FloatWritables. </summary>
        Public Overridable Function CompareTo(ByVal o As Object) As Integer Implements IComparable.CompareTo
            Dim thisValue As Single = Me.valueField
            Dim thatValue As Single = DirectCast(o, FloatWritable).valueField
            Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
        End Function

        ''' <summary>Java <c>Float.toString</c> formatting (e.g. "1.0").</summary>
        Public Overrides Function ToString() As String
            Return Arrays.JavaFloatToString(valueField)
        End Function

        ''' <summary>
        ''' A Comparator optimized for FloatWritable. </summary>
        Public Class Comparator
            Inherits WritableComparator

            Public Sub New()
                MyBase.New(GetType(FloatWritable))
            End Sub

            Public Overrides Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
                Dim thisValue As Single = readFloat(b1, s1)
                Dim thatValue As Single = readFloat(b2, s2)
                Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
            End Function
        End Class

        Shared Sub New() ' register this comparator
            WritableComparator.define(GetType(FloatWritable), New Comparator())
        End Sub

        Public Overridable Function toDouble() As Double Implements Writable.toDouble
            Return valueField
        End Function

        Public Overridable Function toFloat() As Single Implements Writable.toFloat
            Return valueField
        End Function

        Public Overridable Function toInt() As Integer Implements Writable.toInt
            Return DoubleWritable.JavaCastToInt(valueField)
        End Function

        Public Overridable Function toLong() As Long Implements Writable.toLong
            Return DoubleWritable.JavaCastToLong(valueField)
        End Function

        Public Overridable Shadows Function [getType]() As WritableType Implements Writable.getType
            Return WritableType.Float
        End Function
    End Class

End Namespace
