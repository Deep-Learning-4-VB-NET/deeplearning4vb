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


	Public Class DataInputBuffer
		Inherits DataInputStream

		Private Class Buffer
			Inherits MemoryStream

			Public Sub New()
				MyBase.New(New SByte() {})
			End Sub

			Public Overridable Sub reset(ByVal input() As SByte, ByVal start As Integer, ByVal length As Integer)
				Me.buf = input
				Me.count = start + length
				Me.mark = start
				Me.pos = start
			End Sub

			Public Overridable ReadOnly Property Data As SByte()
				Get
					Return buf
				End Get
			End Property

			Public Overridable ReadOnly Property Position As Integer
				Get
					Return pos
				End Get
			End Property

			Public Overridable ReadOnly Property Length As Integer
				Get
					Return count
				End Get
			End Property
		End Class

		Private buffer As Buffer

		''' <summary>
		''' Constructs a new empty buffer. </summary>
		Public Sub New()
			Me.New(New Buffer())
		End Sub

		Private Sub New(ByVal buffer As Buffer)
			MyBase.New(buffer)
			Me.buffer = buffer
		End Sub

		''' <summary>
		''' Resets the data that the buffer reads. </summary>
		Public Overridable Sub reset(ByVal input() As SByte, ByVal length As Integer)
			buffer.reset(input, 0, length)
		End Sub

		''' <summary>
		''' Resets the data that the buffer reads. </summary>
		Public Overridable Sub reset(ByVal input() As SByte, ByVal start As Integer, ByVal length As Integer)
			buffer.reset(input, start, length)
		End Sub

		Public Overridable ReadOnly Property Data As SByte()
			Get
				Return buffer.Data
			End Get
		End Property

		''' <summary>
		''' Returns the current position in the input. </summary>
		Public Overridable ReadOnly Property Position As Integer
			Get
				Return buffer.Position
			End Get
		End Property

		''' <summary>
		''' Returns the length of the input. </summary>
		Public Overridable ReadOnly Property Length As Integer
			Get
				Return buffer.Length
			End Get
		End Property

	End Class

End Namespace