Imports System
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



	Public Class BooleanWritable
		Implements WritableComparable

		Private value As Boolean

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BooleanWritable(@JsonProperty("value") boolean value)
		Public Sub New(ByVal value As Boolean)
			set(value)
		End Sub

		''' <summary>
		''' Set the value of the BooleanWritable
		''' </summary>
		Public Overridable Sub set(ByVal value As Boolean)
			Me.value = value
		End Sub

		''' <summary>
		''' Returns the value of the BooleanWritable
		''' </summary>
		Public Overridable Function get() As Boolean
			Return value
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput)
			value = [in].readBoolean()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput)
			[out].writeShort(WritableType.Boolean.typeIdx())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub write(ByVal [out] As DataOutput)
			[out].writeBoolean(value)
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is BooleanWritable) Then
				Return False
			End If
			Dim other As BooleanWritable = DirectCast(o, BooleanWritable)
			Return Me.value = other.value
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return If(value, 0, 1)
		End Function



		Public Overridable Function compareTo(ByVal o As Object) As Integer
			Dim a As Boolean = Me.value
			Dim b As Boolean = DirectCast(o, BooleanWritable).value
			Return (If(a = b, 0, If(a = False, -1, 1)))
		End Function

		Public Overrides Function ToString() As String
			Return Convert.ToString(get())
		End Function

		''' <summary>
		''' A Comparator optimized for BooleanWritable.
		''' </summary>
		Public Class Comparator
			Inherits WritableComparator

			Public Sub New()
				MyBase.New(GetType(BooleanWritable))
			End Sub

			Public Overrides Function compare(ByVal b1() As SByte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As SByte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
				Return compareBytes(b1, s1, l1, b2, s2, l2)
			End Function
		End Class


		Shared Sub New()
			WritableComparator.define(GetType(BooleanWritable), New Comparator())
		End Sub

		Public Overridable Function toDouble() As Double
			Return (If(value, 1.0, 0.0))
		End Function

		Public Overridable Function toFloat() As Single
			Return (If(value, 1.0f, 0.0f))
		End Function

		Public Overridable Function toInt() As Integer
			Return (If(value, 1, 0))
		End Function

		Public Overridable Function toLong() As Long
			Return (If(value, 1L, 0L))
		End Function

		Public Overridable Function [getType]() As WritableType
			Return WritableType.Boolean
		End Function
	End Class

End Namespace