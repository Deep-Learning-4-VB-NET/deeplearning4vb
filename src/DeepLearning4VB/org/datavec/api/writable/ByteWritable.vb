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
Imports System.Globalization
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.io
Imports WritableComparator = DeepLearning4VB.org.datavec.api.io.WritableComparator

Namespace org.datavec.api.writable

    ''' <summary>Writable for a single (signed, like Java's <c>byte</c>) byte value.</summary>
    <Serializable>
    Public Class ByteWritable
        Implements WritableComparable

        Private valueField As SByte

        Public Sub New()
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal value As SByte)
            [set](value)
        End Sub

        ''' <summary>
        ''' Set the value of this ByteWritable. </summary>
        Public Overridable Sub [set](ByVal value As SByte)
            Me.valueField = value
        End Sub

        ''' <summary>
        ''' Return the value of this ByteWritable. </summary>
        Public Overridable Function [get]() As SByte
            Return valueField
        End Function

        <JsonPropertyName("value")>
        Public Property Value As SByte
            Get
                Return valueField
            End Get
            Set(ByVal value As SByte)
                valueField = value
            End Set
        End Property

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
            valueField = [in].readByte()
        End Sub

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
            [out].writeShort(WritableType.Byte.typeIdx())
        End Sub

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
            [out].writeByte(valueField)
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
        ''' Returns true iff <code>o</code> is a ByteWritable with the same value. </summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If TypeOf o Is IntWritable OrElse TypeOf o Is LongWritable Then
                Return (New IntWritable(Me.valueField)).Equals(o)
            End If
            If TypeOf o Is ByteWritable Then
                Dim other As ByteWritable = DirectCast(o, ByteWritable)
                Return Me.valueField = other.valueField
            Else
                Return False
            End If
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return CInt(valueField)
        End Function

        ''' <summary>
        ''' Compares two ByteWritables. </summary>
        Public Overridable Function CompareTo(ByVal o As Object) As Integer Implements IComparable.CompareTo
            Dim thisValue As Integer = Me.valueField
            Dim thatValue As Integer = DirectCast(o, ByteWritable).valueField
            Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
        End Function

        Public Overrides Function ToString() As String
            Return valueField.ToString(CultureInfo.InvariantCulture)
        End Function

        ''' <summary>
        ''' A Comparator optimized for ByteWritable. </summary>
        Public Class Comparator
            Inherits WritableComparator

            Public Sub New()
                MyBase.New(GetType(ByteWritable))
            End Sub

            Public Overrides Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
                Dim thisValue As Integer = WritableHelpers.signExtend(b1(s1))
                Dim thatValue As Integer = WritableHelpers.signExtend(b2(s2))
                Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
            End Function
        End Class

        Shared Sub New() ' register this comparator
            WritableComparator.define(GetType(ByteWritable), New Comparator())
        End Sub

        Public Overridable Function toDouble() As Double Implements Writable.toDouble
            Return valueField
        End Function

        Public Overridable Function toFloat() As Single Implements Writable.toFloat
            Return valueField
        End Function

        Public Overridable Function toInt() As Integer Implements Writable.toInt
            Return valueField
        End Function

        Public Overridable Function toLong() As Long Implements Writable.toLong
            Return valueField
        End Function

        Public Overridable Shadows Function [getType]() As WritableType Implements Writable.getType
            Return WritableType.Byte
        End Function
    End Class

End Namespace
