﻿Imports System
Imports DoubleMath = org.nd4j.shade.guava.math.DoubleMath
Imports org.datavec.api.io
Imports WritableComparator = org.datavec.api.io.WritableComparator
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.writable



	Public Class ByteWritable
		Implements WritableComparable

		Private value As SByte

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ByteWritable(@JsonProperty("value") byte value)
		Public Sub New(ByVal value As SByte)
			set(value)
		End Sub

		''' <summary>
		''' Set the value of this ByteWritable. </summary>
		Public Overridable Sub set(ByVal value As SByte)
			Me.value = value
		End Sub

		''' <summary>
		''' Return the value of this ByteWritable. </summary>
		Public Overridable Function get() As SByte
			Return value
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput)
			value = [in].readByte()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput)
			[out].writeShort(WritableType.Byte.typeIdx())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub write(ByVal [out] As DataOutput)
			[out].writeByte(value)
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
			Return DoubleMath.fuzzyEquals(Me.value, other, tolerance)
		End Function

		''' <summary>
		''' Returns true iff <code>o</code> is a ByteWritable with the same value. </summary>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If TypeOf o Is IntWritable OrElse TypeOf o Is LongWritable Then
				Return (New IntWritable(Me.value)).Equals(o)
			End If
			If TypeOf o Is ByteWritable Then
				Dim other As ByteWritable = DirectCast(o, ByteWritable)
				Return Me.value = other.value
			Else
				Return False
			End If
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return CInt(value)
		End Function

		''' <summary>
		''' Compares two ByteWritables. </summary>
		Public Overridable Function compareTo(ByVal o As Object) As Integer
			Dim thisValue As Integer = Me.value
			Dim thatValue As Integer = DirectCast(o, ByteWritable).value
			Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
		End Function

		Public Overrides Function ToString() As String
			Return Convert.ToString(value)
		End Function

		''' <summary>
		''' A Comparator optimized for ByteWritable. </summary>
		Public Class Comparator
			Inherits WritableComparator

			Public Sub New()
				MyBase.New(GetType(ByteWritable))
			End Sub

			Public Overrides Function compare(ByVal b1() As SByte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As SByte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
				Dim thisValue As SByte = b1(s1)
				Dim thatValue As SByte = b2(s2)
				Return (If(thisValue < thatValue, -1, (If(thisValue = thatValue, 0, 1))))
			End Function
		End Class

		Shared Sub New() ' register this comparator
			WritableComparator.define(GetType(ByteWritable), New Comparator())
		End Sub

		Public Overridable Function toDouble() As Double
			Return value
		End Function

		Public Overridable Function toFloat() As Single
			Return value
		End Function

		Public Overridable Function toInt() As Integer
			Return value
		End Function

		Public Overridable Function toLong() As Long
			Return value
		End Function

		Public Overridable Function [getType]() As WritableType
			Return WritableType.Byte
		End Function
	End Class

End Namespace