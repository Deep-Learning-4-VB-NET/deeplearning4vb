Imports System.IO
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

Namespace org.nd4j.common.util


	Public Class InputStreamUtil
		''' <summary>
		''' Count number of lines in a file
		''' </summary>
		''' <param name="is">
		''' @return </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int countLines(java.io.InputStream is) throws java.io.IOException
		Public Shared Function countLines(ByVal [is] As Stream) As Integer
			Try
				Dim c(1023) As SByte
				Dim count As Integer = 0
				Dim readChars As Integer = 0
				Dim empty As Boolean = True
				readChars = [is].Read(c, 0, c.Length)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((readChars = is.read(c)) != -1)
				Do While readChars <> -1
					empty = False
					For i As Integer = 0 To readChars - 1
						If c(i) = ControlChars.Lf Then
							count += 1
						End If
					Next i
						readChars = [is].Read(c, 0, c.Length)
				Loop
				Return If(count = 0 AndAlso Not empty, 1, count)
			Finally
				[is].Close()
			End Try


		End Function

		''' <summary>
		''' Count number of lines in a file
		''' </summary>
		''' <param name="filename">
		''' @return </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int countLines(String filename) throws java.io.IOException
		Public Shared Function countLines(ByVal filename As String) As Integer
			Dim fis As New FileStream(filename, FileMode.Open, FileAccess.Read)
			Try
				Return countLines(New BufferedInputStream(fis))
			Finally
				fis.Close()
			End Try
		End Function
	End Class

End Namespace