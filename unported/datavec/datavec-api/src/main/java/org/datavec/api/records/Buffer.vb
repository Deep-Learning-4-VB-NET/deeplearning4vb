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

Namespace org.datavec.api.records


	Public Class Buffer
		Implements Comparable, ICloneable

		''' <summary>
		''' Number of valid bytes in this.bytes. </summary>
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private count_Conflict As Integer
		''' <summary>
		''' Backing store for Buffer. </summary>
		Private bytes() As SByte = Nothing

		''' <summary>
		''' Create a zero-count sequence.
		''' </summary>
		Public Sub New()
			Me.count_Conflict = 0
		End Sub

		''' <summary>
		''' Create a Buffer using the byte array as the initial value.
		''' </summary>
		''' <param name="bytes"> This array becomes the backing storage for the object. </param>
		Public Sub New(ByVal bytes() As SByte)
			Me.bytes = bytes
			Me.count_Conflict = If(bytes Is Nothing, 0, bytes.Length)
		End Sub

		''' <summary>
		''' Create a Buffer using the byte range as the initial value.
		''' </summary>
		''' <param name="bytes"> Copy of this array becomes the backing storage for the object. </param>
		''' <param name="offset"> offset into byte array </param>
		''' <param name="length"> length of data </param>
		Public Sub New(ByVal bytes() As SByte, ByVal offset As Integer, ByVal length As Integer)
			copy(bytes, offset, length)
		End Sub


		''' <summary>
		''' Use the specified bytes array as underlying sequence.
		''' </summary>
		''' <param name="bytes"> byte sequence </param>
		Public Overridable Sub set(ByVal bytes() As SByte)
			Me.count_Conflict = If(bytes Is Nothing, 0, bytes.Length)
			Me.bytes = bytes
		End Sub

		''' <summary>
		''' Copy the specified byte array to the Buffer. Replaces the current buffer.
		''' </summary>
		''' <param name="bytes"> byte array to be assigned </param>
		''' <param name="offset"> offset into byte array </param>
		''' <param name="length"> length of data </param>
		Public Sub copy(ByVal bytes() As SByte, ByVal offset As Integer, ByVal length As Integer)
			If Me.bytes Is Nothing OrElse Me.bytes.Length < length Then
				Me.bytes = New SByte(length - 1){}
			End If
			Array.Copy(bytes, offset, Me.bytes, 0, length)
			Me.count_Conflict = length
		End Sub

		''' <summary>
		''' Get the data from the Buffer.
		''' </summary>
		''' <returns> The data is only valid between 0 and getCount() - 1. </returns>
		Public Overridable Function get() As SByte()
			If bytes Is Nothing Then
				bytes = New SByte(){}
			End If
			Return bytes
		End Function

		''' <summary>
		''' Get the current count of the buffer.
		''' </summary>
		Public Overridable ReadOnly Property Count As Integer
			Get
				Return count_Conflict
			End Get
		End Property

		''' <summary>
		''' Get the capacity, which is the maximum count that could handled without
		''' resizing the backing storage.
		''' </summary>
		''' <returns> The number of bytes </returns>
		Public Overridable Property Capacity As Integer
			Get
				Return Me.get().Length
			End Get
			Set(ByVal newCapacity As Integer)
				If newCapacity < 0 Then
					Throw New System.ArgumentException("Invalid capacity argument " & newCapacity)
				End If
				If newCapacity = 0 Then
					Me.bytes = Nothing
					Me.count_Conflict = 0
					Return
				End If
				If newCapacity <> Capacity Then
					Dim data(newCapacity - 1) As SByte
					If newCapacity < count_Conflict Then
						count_Conflict = newCapacity
					End If
					If count_Conflict <> 0 Then
						Array.Copy(Me.get(), 0, data, 0, count_Conflict)
					End If
					bytes = data
				End If
			End Set
		End Property


		''' <summary>
		''' Reset the buffer to 0 size
		''' </summary>
		Public Overridable Sub reset()
			Capacity = 0
		End Sub

		''' <summary>
		''' Change the capacity of the backing store to be the same as the current
		''' count of buffer.
		''' </summary>
		Public Overridable Sub truncate()
			Capacity = count_Conflict
		End Sub

		''' <summary>
		''' Append specified bytes to the buffer.
		''' </summary>
		''' <param name="bytes"> byte array to be appended </param>
		''' <param name="offset"> offset into byte array </param>
		''' <param name="length"> length of data
		'''  </param>
		Public Overridable Sub append(ByVal bytes() As SByte, ByVal offset As Integer, ByVal length As Integer)
			Capacity = count_Conflict + length
			Array.Copy(bytes, offset, Me.get(), count_Conflict, length)
			count_Conflict = count_Conflict + length
		End Sub

		''' <summary>
		''' Append specified bytes to the buffer
		''' </summary>
		''' <param name="bytes"> byte array to be appended </param>
		Public Overridable Sub append(ByVal bytes() As SByte)
			append(bytes, 0, bytes.Length)
		End Sub

		' inherit javadoc
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = 1
			Dim b() As SByte = Me.get()
			For i As Integer = 0 To count_Conflict - 1
				hash = (31 * hash) + CInt(b(i))
			Next i
			Return hash
		End Function

		''' <summary>
		''' Define the sort order of the Buffer.
		''' </summary>
		''' <param name="other"> The other buffer </param>
		''' <returns> Positive if this is bigger than other, 0 if they are equal, and
		'''         negative if this is smaller than other. </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int compareTo(Object other)
		If True Then
			Dim right As Buffer = (DirectCast(other, Buffer))
			Dim lb() As SByte = Me.get()
			Dim rb() As SByte = right.get()
			Dim i As Integer = 0
			Do While i < count_Conflict AndAlso i < right.count_Conflict
				Dim a As Integer = (lb(i) And &Hff)
				Dim b As Integer = (rb(i) And &Hff)
				If a <> b Then
					Return a - b
				End If
				i += 1
			Loop
			Return count_Conflict - right.count_Conflict
		End If

		' inherit javadoc
		public Boolean Equals(Object other)
		If True Then
			If TypeOf other Is Buffer AndAlso Me IsNot other Then
				Return CompareTo(other) = 0
			End If
			Return (Me Is other)
		End If

		' inheric javadoc
		public String ToString()
		If True Then
			Dim sb As New StringBuilder(2 * count_Conflict)
			For idx As Integer = 0 To count_Conflict - 1
				sb.Append(Character.forDigit((bytes(idx) And &HF0) >> 4, 16))
				sb.Append(Character.forDigit(bytes(idx) And &HF, 16))
			Next idx
			Return sb.ToString()
		End If

		''' <summary>
		''' Convert the byte buffer to a string an specific character encoding
		''' </summary>
		''' <param name="charsetName"> Valid Java Character Set Name </param>
		public String toString(String charsetName) throws UnsupportedEncodingException
		If True Then
			Return New String(Me.get(), 0, Me.Count, charsetName)
		End If

		' inherit javadoc
		public Object clone() throws CloneNotSupportedException
		If True Then
			Dim result As Buffer = DirectCast(MyBase.clone(), Buffer)
			result.copy(Me.get(), 0, Me.Count)
			Return result
		End If
	End Class

End Namespace