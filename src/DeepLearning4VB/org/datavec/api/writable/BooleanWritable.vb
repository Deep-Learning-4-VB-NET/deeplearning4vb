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

    ''' <summary>Writable for boolean values.</summary>
    <Serializable>
    Public Class BooleanWritable
        Implements WritableComparable

        Private valueField As Boolean

        Public Sub New()
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal value As Boolean)
            [set](value)
        End Sub

        ''' <summary>
        ''' Set the value of the BooleanWritable
        ''' </summary>
        Public Overridable Sub [set](ByVal value As Boolean)
            Me.valueField = value
        End Sub

        ''' <summary>
        ''' Returns the value of the BooleanWritable
        ''' </summary>
        Public Overridable Function [get]() As Boolean
            Return valueField
        End Function

        <JsonPropertyName("value")>
        Public Property Value As Boolean
            Get
                Return valueField
            End Get
            Set(ByVal value As Boolean)
                valueField = value
            End Set
        End Property

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
            valueField = [in].readBoolean()
        End Sub

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
            [out].writeShort(WritableType.Boolean.typeIdx())
        End Sub

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
            [out].writeBoolean(valueField)
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Not (TypeOf o Is BooleanWritable) Then
                Return False
            End If
            Dim other As BooleanWritable = DirectCast(o, BooleanWritable)
            Return Me.valueField = other.valueField
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return If(valueField, 0, 1)
        End Function

        Public Overridable Function CompareTo(ByVal o As Object) As Integer Implements IComparable.CompareTo
            Dim a As Boolean = Me.valueField
            Dim b As Boolean = DirectCast(o, BooleanWritable).valueField
            Return (If(a = b, 0, If(a = False, -1, 1)))
        End Function

        ''' <summary>Java <c>Boolean.toString</c>: "true" or "false".</summary>
        Public Overrides Function ToString() As String
            Return If(valueField, "true", "false")
        End Function

        ''' <summary>
        ''' A Comparator optimized for BooleanWritable.
        ''' </summary>
        Public Class Comparator
            Inherits WritableComparator

            Public Sub New()
                MyBase.New(GetType(BooleanWritable))
            End Sub

            Public Overrides Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
                Return compareBytes(b1, s1, l1, b2, s2, l2)
            End Function
        End Class

        Shared Sub New()
            WritableComparator.define(GetType(BooleanWritable), New Comparator())
        End Sub

        Public Overridable Function toDouble() As Double Implements Writable.toDouble
            Return (If(valueField, 1.0, 0.0))
        End Function

        Public Overridable Function toFloat() As Single Implements Writable.toFloat
            Return (If(valueField, 1.0F, 0.0F))
        End Function

        Public Overridable Function toInt() As Integer Implements Writable.toInt
            Return (If(valueField, 1, 0))
        End Function

        Public Overridable Function toLong() As Long Implements Writable.toLong
            Return (If(valueField, 1L, 0L))
        End Function

        Public Overridable Shadows Function [getType]() As WritableType Implements Writable.getType
            Return WritableType.Boolean
        End Function
    End Class

End Namespace
