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

    ''' <summary>Writable for long values.</summary>
    <Serializable>
    Public Class LongWritable
        Implements WritableComparable

        Private valueField As Long

        Public Sub New()
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal value As Long)
            [set](value)
        End Sub

        ''' <summary>
        ''' Set the value of this LongWritable. </summary>
        Public Overridable Sub [set](ByVal value As Long)
            Me.valueField = value
        End Sub

        ''' <summary>
        ''' Return the value of this LongWritable. </summary>
        Public Overridable Function [get]() As Long
            Return valueField
        End Function

        <JsonPropertyName("value")>
        Public Property Value As Long
            Get
                Return valueField
            End Get
            Set(ByVal value As Long)
                valueField = value
            End Set
        End Property

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
            valueField = [in].readLong()
        End Sub

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
            [out].writeShort(WritableType.Long.typeIdx())
        End Sub

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
            [out].writeLong(valueField)
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
        ''' Returns true iff <code>o</code> is a LongWritable with the same value. </summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If TypeOf o Is ByteWritable Then
                Dim other As ByteWritable = DirectCast(o, ByteWritable)
                Return Me.valueField = other.get()
            End If
            If TypeOf o Is LongWritable Then
                Dim other As LongWritable = DirectCast(o, LongWritable)
                Return Me.valueField = other.get()
            End If
            If TypeOf o Is IntWritable Then
                Dim other As IntWritable = DirectCast(o, IntWritable)
                Return Me.valueField = other.get()
            Else
                Return False
            End If
        End Function

        Public Overrides Function GetHashCode() As Integer
            ' copy the long hashCode
            Return CInt(valueField Xor CLng(CULng(valueField) >> 32))
        End Function

        ''' <summary>
        ''' Compares two LongWritables. </summary>
        Public Overridable Function CompareTo(ByVal o As Object) As Integer Implements IComparable.CompareTo
            Dim thisValue As Long = Me.valueField
            Dim thatValue As Long = DirectCast(o, LongWritable).valueField
            Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
        End Function

        Public Overrides Function ToString() As String
            Return valueField.ToString(CultureInfo.InvariantCulture)
        End Function

        ''' <summary>
        ''' A Comparator optimized for LongWritable. </summary>
        Public Class Comparator
            Inherits WritableComparator

            Public Sub New()
                MyBase.New(GetType(LongWritable))
            End Sub

            Public Overrides Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
                Dim thisValue As Long = readLong(b1, s1)
                Dim thatValue As Long = readLong(b2, s2)
                Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
            End Function
        End Class

        ''' <summary>
        ''' A decreasing Comparator optimized for LongWritable. </summary>
        Public Class DecreasingComparator
            Inherits Comparator

            Public Overrides Function compare(ByVal a As WritableComparable, ByVal b As WritableComparable) As Integer
                Return -MyBase.compare(a, b)
            End Function

            Public Overrides Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
                Return -MyBase.compare(b1, s1, l1, b2, s2, l2)
            End Function
        End Class

        Shared Sub New() ' register default comparator
            WritableComparator.define(GetType(LongWritable), New Comparator())
        End Sub

        Public Overridable Function toDouble() As Double Implements Writable.toDouble
            Return valueField
        End Function

        Public Overridable Function toFloat() As Single Implements Writable.toFloat
            Return valueField
        End Function

        Public Overridable Function toInt() As Integer Implements Writable.toInt
            Return CInt(valueField)
        End Function

        Public Overridable Function toLong() As Long Implements Writable.toLong
            Return valueField
        End Function

        Public Overridable Shadows Function [getType]() As WritableType Implements Writable.getType
            Return WritableType.Long
        End Function

    End Class

End Namespace
