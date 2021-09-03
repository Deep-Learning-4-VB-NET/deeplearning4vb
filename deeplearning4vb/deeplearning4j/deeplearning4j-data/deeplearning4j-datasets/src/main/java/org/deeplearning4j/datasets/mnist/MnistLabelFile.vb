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

Namespace org.deeplearning4j.datasets.mnist




	''' 
	''' <summary>
	''' MNIST database label file.
	''' 
	''' </summary>
	Public Class MnistLabelFile
		Inherits MnistDbFile

		''' <summary>
		''' Creates new MNIST database label file ready for reading.
		''' </summary>
		''' <param name="name">
		'''            the system-dependent filename </param>
		''' <param name="mode">
		'''            the access mode </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="FileNotFoundException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MnistLabelFile(String name, String mode) throws java.io.IOException
		Public Sub New(ByVal name As String, ByVal mode As String)
			MyBase.New(name, mode)
		End Sub

		''' <summary>
		''' Reads the integer at the current position.
		''' </summary>
		''' <returns> integer representing the label </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int readLabel() throws java.io.IOException
		Public Overridable Function readLabel() As Integer
			Return readUnsignedByte()
		End Function

		''' <summary>
		''' Read the specified number of labels from the current position </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public int[] readLabels(int num) throws java.io.IOException
		Public Overridable Function readLabels(ByVal num As Integer) As Integer()
			Dim [out](num - 1) As Integer
			For i As Integer = 0 To num - 1
				[out](i) = readLabel()
			Next i
			Return [out]
		End Function

		Protected Friend Overrides ReadOnly Property MagicNumber As Integer
			Get
				Return 2049
			End Get
		End Property
	End Class

End Namespace