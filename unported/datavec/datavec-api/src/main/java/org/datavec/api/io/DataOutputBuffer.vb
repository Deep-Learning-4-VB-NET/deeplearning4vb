Imports System
Imports System.IO

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

Namespace org.datavec.api.io


	Public Class DataOutputBuffer
		Inherits DataOutputStream

		Private Class Buffer
			Inherits MemoryStream

			Public Overridable ReadOnly Property Data As SByte()
				Get
					Return buf
				End Get
			End Property

			Public Overridable ReadOnly Property Length As Integer
				Get
					Return count
				End Get
			End Property

			Public Sub New()
				MyBase.New()
			End Sub

			Public Sub New(ByVal size As Integer)
				MyBase.New(size)
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void write(DataInput in, int len) throws IOException
			Public Overridable Sub write(ByVal [in] As DataInput, ByVal len As Integer)
				Dim newcount As Integer = count + len
				If newcount > buf.length Then
					Dim newbuf(Math.Max(buf.length << 1, newcount) - 1) As SByte
					Array.Copy(buf, 0, newbuf, 0, count)
					buf = newbuf
				End If
				[in].readFully(buf, count, len)
				count = newcount
			End Sub
		End Class

		Private buffer As Buffer

		''' <summary>
		''' Constructs a new empty buffer. </summary>
		Public Sub New()
			Me.New(New Buffer())
		End Sub

		Public Sub New(ByVal size As Integer)
			Me.New(New Buffer(size))
		End Sub

		Private Sub New(ByVal buffer As Buffer)
			MyBase.New(buffer)
			Me.buffer = buffer
		End Sub

		''' <summary>
		''' Returns the current contents of the buffer.
		'''  Data is only valid to <seealso cref="getLength()"/>.
		''' </summary>
		Public Overridable ReadOnly Property Data As SByte()
			Get
				Return buffer.Data
			End Get
		End Property

		''' <summary>
		''' Returns the length of the valid data currently in the buffer. </summary>
		Public Overridable ReadOnly Property Length As Integer
			Get
				Return buffer.Length
			End Get
		End Property

		''' <summary>
		''' Resets the buffer to empty. </summary>
		Public Overridable Function reset() As DataOutputBuffer
			Me.written = 0
			buffer.reset()
			Return Me
		End Function

		''' <summary>
		''' Writes bytes from a DataInput directly into the buffer. </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void write(DataInput in, int length) throws IOException
		Public Overridable Sub write(ByVal [in] As DataInput, ByVal length As Integer)
			buffer.write([in], length)
		End Sub

		''' <summary>
		''' Write to a file stream </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void writeTo(OutputStream out) throws IOException
		Public Overridable Sub writeTo(ByVal [out] As Stream)
			buffer.writeTo([out])
		End Sub
	End Class

End Namespace