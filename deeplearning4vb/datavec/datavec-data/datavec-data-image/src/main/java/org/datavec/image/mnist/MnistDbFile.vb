Imports System

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

Namespace org.datavec.image.mnist



	Public MustInherit Class MnistDbFile
		Inherits RandomAccessFile

'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private count_Conflict As Integer


		''' <summary>
		''' Creates new instance and reads the header information.
		''' </summary>
		''' <param name="name">
		'''            the system-dependent filename </param>
		''' <param name="mode">
		'''            the access mode </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <exception cref="java.io.FileNotFoundException"> </exception>
		''' <seealso cref= java.io.RandomAccessFile </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistDbFile(String name, String mode) throws java.io.IOException
		Public Sub New(ByVal name As String, ByVal mode As String)
			MyBase.New(name, mode)
			If MagicNumber <> readInt() Then
				Throw New Exception("This MNIST DB file " & name & " should start with the number " & MagicNumber & ".")
			End If
			count_Conflict = readInt()
		End Sub

		''' <summary>
		''' MNIST DB files start with unique integer number.
		''' </summary>
		''' <returns> integer number that should be found in the beginning of the file. </returns>
		Protected Friend MustOverride ReadOnly Property MagicNumber As Integer

		''' <summary>
		''' The current entry index.
		''' </summary>
		''' <returns> long </returns>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public long getCurrentIndex() throws java.io.IOException
		Public Overridable Property CurrentIndex As Long
			Get
	'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return (getFilePointer() - HeaderSize) / EntryLength + 1
			End Get
			Set(ByVal curr As Long)
				Try
					If curr < 0 OrElse curr > count_Conflict Then
						Throw New Exception(curr & " is not in the range 0 to " & count_Conflict)
					End If
					seek(HeaderSize + (curr - 1) * EntryLength)
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End Set
		End Property


		Public Overridable ReadOnly Property HeaderSize As Integer
			Get
				Return 8 ' two integers
			End Get
		End Property

		''' <summary>
		''' Number of bytes for each entry.
		''' Defaults to 1.
		''' </summary>
		''' <returns> int </returns>
		Public Overridable ReadOnly Property EntryLength As Integer
			Get
				Return 1
			End Get
		End Property

		''' <summary>
		''' Move to the next entry.
		''' </summary>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void next() throws java.io.IOException
		Public Overridable Sub [next]()
			If CurrentIndex < count_Conflict Then
				skipBytes(EntryLength)
			End If
		End Sub

		''' <summary>
		''' Move to the previous entry.
		''' </summary>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void prev() throws java.io.IOException
		Public Overridable Sub prev()
			If CurrentIndex > 0 Then
				seek(getFilePointer() - EntryLength)
			End If
		End Sub

		Public Overridable ReadOnly Property Count As Integer
			Get
				Return count_Conflict
			End Get
		End Property
	End Class

End Namespace