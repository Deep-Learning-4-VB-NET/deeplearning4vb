Imports System
Imports Frame = org.bytedeco.javacv.Frame
Imports FrameConverter = org.bytedeco.javacv.FrameConverter
Imports Writable = org.datavec.api.writable.Writable
Imports WritableFactory = org.datavec.api.writable.WritableFactory
Imports WritableType = org.datavec.api.writable.WritableType

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

Namespace org.datavec.image.data


	<Serializable>
	Public Class ImageWritable
		Implements Writable

		Shared Sub New()
			WritableFactory.Instance.registerWritableType(WritableType.Image.typeIdx(), GetType(ImageWritable))
		End Sub

'JAVA TO VB CONVERTER NOTE: The field frame was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend frame_Conflict As Frame

		Public Sub New()
			'No-arg cosntructor for reflection-based creation of ImageWritable objects
		End Sub

		Public Sub New(ByVal frame As Frame)
			Me.frame_Conflict = frame
		End Sub

		Public Overridable Property Frame As Frame
			Get
				Return frame_Conflict
			End Get
			Set(ByVal frame As Frame)
				Me.frame_Conflict = frame
			End Set
		End Property


		Public Overridable ReadOnly Property Width As Integer
			Get
				Return frame_Conflict.imageWidth
			End Get
		End Property

		Public Overridable ReadOnly Property Height As Integer
			Get
				Return frame_Conflict.imageHeight
			End Get
		End Property

		Public Overridable ReadOnly Property Depth As Integer
			Get
				Return frame_Conflict.imageDepth
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
			Throw New System.NotSupportedException("Not supported yet.")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
			Throw New System.NotSupportedException("Not supported yet.")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
			[out].writeShort(WritableType.Image.typeIdx())
		End Sub

		Public Overridable Function toDouble() As Double Implements Writable.toDouble
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toFloat() As Single Implements Writable.toFloat
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toInt() As Integer Implements Writable.toInt
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function toLong() As Long Implements Writable.toLong
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function [getType]() As WritableType
			Return WritableType.Image
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is ImageWritable Then
				Dim f2 As Frame = DirectCast(obj, ImageWritable).Frame

				Dim b1() As Buffer = Me.frame_Conflict.image
				Dim b2() As Buffer = f2.image

				If b1.Length <> b2.Length Then
					Return False
				End If

				For i As Integer = 0 To b1.Length - 1
					If Not b1(i).Equals(b2(i)) Then
						Return False
					End If
				Next i

				Return True
			Else
				Return False
			End If
		End Function
	End Class

End Namespace