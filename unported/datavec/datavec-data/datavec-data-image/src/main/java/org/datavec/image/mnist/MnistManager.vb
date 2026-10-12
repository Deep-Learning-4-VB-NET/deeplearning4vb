Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic

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




	Public Class MnistManager
'JAVA TO VB CONVERTER NOTE: The field images was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private images_Conflict As MnistImageFile
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labels_Conflict As MnistLabelFile

		''' <summary>
		''' Writes the given image in the given file using the PPM data format.
		''' </summary>
		''' <param name="image"> </param>
		''' <param name="ppmFileName"> </param>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeImageToPpm(int[][] image, String ppmFileName) throws java.io.IOException
		Public Shared Sub writeImageToPpm(ByVal image()() As Integer, ByVal ppmFileName As String)
			Using ppmOut As New StreamWriter(ppmFileName)
				Dim rows As Integer = image.Length
				Dim cols As Integer = image(0).Length
				ppmOut.Write("P3" & vbLf)
				ppmOut.Write("" & rows & " " & cols & " 255" & vbLf)
				For Each anImage As Integer() In image
					Dim s As New StringBuilder()
					For j As Integer = 0 To cols - 1
						s.Append(anImage(j) & " " & anImage(j) & " " & anImage(j) & "  ")
					Next j
					ppmOut.Write(s.ToString())
				Next anImage
			End Using

		End Sub

		''' <summary>
		''' Constructs an instance managing the two given data files. Supports
		''' <code>NULL</code> value for one of the arguments in case reading only one
		''' of the files (images and labels) is required.
		''' </summary>
		''' <param name="imagesFile">
		'''            Can be <code>NULL</code>. In that case all future operations
		'''            using that file will fail. </param>
		''' <param name="labelsFile">
		'''            Can be <code>NULL</code>. In that case all future operations
		'''            using that file will fail. </param>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistManager(String imagesFile, String labelsFile) throws java.io.IOException
		Public Sub New(ByVal imagesFile As String, ByVal labelsFile As String)
			If imagesFile IsNot Nothing Then
				images_Conflict = New MnistImageFile(imagesFile, "r")
			End If
			If labelsFile IsNot Nothing Then
				labels_Conflict = New MnistLabelFile(labelsFile, "r")
			End If
		End Sub

		''' <summary>
		''' Reads the current image.
		''' </summary>
		''' <returns> matrix </returns>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int[][] readImage() throws java.io.IOException
		Public Overridable Function readImage() As Integer()()
			If images_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Images file not initialized.")
			End If
			Return images_Conflict.readImage()
		End Function

		''' <summary>
		''' Set the position to be read.
		''' </summary>
		''' <param name="index"> </param>
		Public Overridable WriteOnly Property Current As Integer
			Set(ByVal index As Integer)
				images_Conflict.CurrentIndex = index
				labels_Conflict.CurrentIndex = index
			End Set
		End Property

		''' <summary>
		''' Reads the current label.
		''' </summary>
		''' <returns> int </returns>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int readLabel() throws java.io.IOException
		Public Overridable Function readLabel() As Integer
			If labels_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("labels file not initialized.")
			End If
			Return labels_Conflict.readLabel()
		End Function

		''' <summary>
		''' Get the underlying images file as <seealso cref="MnistImageFile"/>.
		''' </summary>
		''' <returns> <seealso cref="MnistImageFile"/>. </returns>
		Public Overridable ReadOnly Property Images As MnistImageFile
			Get
				Return images_Conflict
			End Get
		End Property

		''' <summary>
		''' Get the underlying labels file as <seealso cref="MnistLabelFile"/>.
		''' </summary>
		''' <returns> <seealso cref="MnistLabelFile"/>. </returns>
		Public Overridable ReadOnly Property Labels As MnistLabelFile
			Get
				Return labels_Conflict
			End Get
		End Property
	End Class

End Namespace