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




	Public Class MnistImageFile
		Inherits MnistDbFile

'JAVA TO VB CONVERTER NOTE: The field rows was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private rows_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field cols was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private cols_Conflict As Integer

		''' <summary>
		''' Creates new MNIST database image file ready for reading.
		''' </summary>
		''' <param name="name">
		'''            the system-dependent filename </param>
		''' <param name="mode">
		'''            the access mode </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <exception cref="java.io.FileNotFoundException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistImageFile(String name, String mode) throws java.io.IOException
		Public Sub New(ByVal name As String, ByVal mode As String)
			MyBase.New(name, mode)

			' read header information
			rows_Conflict = readInt()
			cols_Conflict = readInt()
		End Sub

		''' <summary>
		''' Reads the image at the current position.
		''' </summary>
		''' <returns> matrix representing the image </returns>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int[][] readImage() throws java.io.IOException
		Public Overridable Function readImage() As Integer()()
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim dat[][] As Integer = new Integer[Rows][Cols]
			Dim dat()() As Integer = RectangularArrays.RectangularIntegerArray(Rows, Cols)
			Dim i As Integer = 0
			Do While i < Cols
				Dim j As Integer = 0
				Do While j < Rows
					dat(i)(j) = readUnsignedByte()
					j += 1
				Loop
				i += 1
			Loop
			Return dat
		End Function

		''' <summary>
		''' Move the cursor to the next image.
		''' </summary>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void nextImage() throws java.io.IOException
		Public Overridable Sub nextImage()
			MyBase.next()
		End Sub

		''' <summary>
		''' Move the cursor to the previous image.
		''' </summary>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void prevImage() throws java.io.IOException
		Public Overridable Sub prevImage()
			MyBase.prev()
		End Sub

		Protected Friend Overrides ReadOnly Property MagicNumber As Integer
			Get
				Return 2051
			End Get
		End Property

		''' <summary>
		''' Number of rows per image.
		''' </summary>
		''' <returns> int </returns>
		Public Overridable ReadOnly Property Rows As Integer
			Get
				Return rows_Conflict
			End Get
		End Property

		''' <summary>
		''' Number of columns per image.
		''' </summary>
		''' <returns> int </returns>
		Public Overridable ReadOnly Property Cols As Integer
			Get
				Return cols_Conflict
			End Get
		End Property

		Public Overrides ReadOnly Property EntryLength As Integer
			Get
				Return cols_Conflict * rows_Conflict
			End Get
		End Property

		Public Overrides ReadOnly Property HeaderSize As Integer
			Get
				Return MyBase.HeaderSize + 8 ' to more integers - rows and columns
			End Get
		End Property
	End Class

End Namespace