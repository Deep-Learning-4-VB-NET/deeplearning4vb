Imports System
Imports System.Text

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

Namespace org.datavec.spark.functions.pairdata


	<Serializable>
	Public Class BytesPairWritable
		Implements org.apache.hadoop.io.Writable

'JAVA TO VB CONVERTER NOTE: The field first was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private first_Conflict() As SByte
'JAVA TO VB CONVERTER NOTE: The field second was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private second_Conflict() As SByte
'JAVA TO VB CONVERTER NOTE: The field uriFirst was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private uriFirst_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field uriSecond was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private uriSecond_Conflict As String

		Public Sub New()
		End Sub

		Public Sub New(ByVal first() As SByte, ByVal second() As SByte, ByVal uriFirst As String, ByVal uriSecond As String)
			Me.first_Conflict = first
			Me.second_Conflict = second
			Me.uriFirst_Conflict = uriFirst
			Me.uriSecond_Conflict = uriSecond
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(java.io.DataOutput dataOutput) throws java.io.IOException
		Public Overrides Sub write(ByVal dataOutput As DataOutput)
			Dim length1 As Integer = (If(first_Conflict IsNot Nothing, first_Conflict.Length, 0))
			Dim length2 As Integer = (If(second_Conflict IsNot Nothing, second_Conflict.Length, 0))
			Dim s1Bytes() As SByte = (If(uriFirst_Conflict IsNot Nothing, uriFirst_Conflict.GetBytes(Encoding.UTF8), Nothing))
			Dim s2Bytes() As SByte = (If(uriSecond_Conflict IsNot Nothing, uriSecond_Conflict.GetBytes(Encoding.UTF8), Nothing))
			Dim s1Len As Integer = (If(s1Bytes IsNot Nothing, s1Bytes.Length, 0))
			Dim s2Len As Integer = (If(s2Bytes IsNot Nothing, s2Bytes.Length, 0))
			dataOutput.writeInt(length1)
			dataOutput.writeInt(length2)
			dataOutput.writeInt(s1Len)
			dataOutput.writeInt(s2Len)
			If first_Conflict IsNot Nothing Then
				dataOutput.write(first_Conflict)
			End If
			If second_Conflict IsNot Nothing Then
				dataOutput.write(second_Conflict)
			End If
			If s1Bytes IsNot Nothing Then
				dataOutput.write(s1Bytes)
			End If
			If s2Bytes IsNot Nothing Then
				dataOutput.write(s2Bytes)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void readFields(java.io.DataInput dataInput) throws java.io.IOException
		Public Overrides Sub readFields(ByVal dataInput As DataInput)
			Dim length1 As Integer = dataInput.readInt()
			Dim length2 As Integer = dataInput.readInt()
			Dim s1Len As Integer = dataInput.readInt()
			Dim s2Len As Integer = dataInput.readInt()
			If length1 > 0 Then
				first_Conflict = New SByte(length1 - 1){}
				dataInput.readFully(first_Conflict)
			End If
			If length2 > 0 Then
				second_Conflict = New SByte(length2 - 1){}
				dataInput.readFully(second_Conflict)
			End If
			If s1Len > 0 Then
				Dim s1Bytes(s1Len - 1) As SByte
				dataInput.readFully(s1Bytes)
				uriFirst_Conflict = StringHelper.NewString(s1Bytes, StandardCharsets.UTF_8)
			End If
			If s2Len > 0 Then
				Dim s2Bytes(s2Len - 1) As SByte
				dataInput.readFully(s2Bytes)
				uriSecond_Conflict = StringHelper.NewString(s2Bytes, StandardCharsets.UTF_8)
			End If
		End Sub

		Public Overridable Property First As SByte()
			Get
				Return first_Conflict
			End Get
			Set(ByVal first() As SByte)
				Me.first_Conflict = first
			End Set
		End Property

		Public Overridable Property Second As SByte()
			Get
				Return second_Conflict
			End Get
			Set(ByVal second() As SByte)
				Me.second_Conflict = second
			End Set
		End Property

		Public Overridable Property UriFirst As String
			Get
				Return uriFirst_Conflict
			End Get
			Set(ByVal uriFirst As String)
				Me.uriFirst_Conflict = uriFirst
			End Set
		End Property

		Public Overridable Property UriSecond As String
			Get
				Return uriSecond_Conflict
			End Get
			Set(ByVal uriSecond As String)
				Me.uriSecond_Conflict = uriSecond
			End Set
		End Property




	End Class

End Namespace