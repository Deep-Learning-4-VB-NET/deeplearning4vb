Imports NonNull = lombok.NonNull

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

Namespace org.nd4j.linalg.util


	Public Class HashUtil
		Private Shared ReadOnly byteTable() As Long = createLookupTable()
		Private Const HSTART As Long = &HBB40E64DA205B064L
		Private Const HMULT As Long = 7664345821815920749L

		Private Shared Function createLookupTable() As Long()
			Dim byteTable(255) As Long
			Dim h As Long = &H544B2FBACAAF1684L
			For i As Integer = 0 To 255
				For j As Integer = 0 To 30
					h = (CLng(CULng(h) >> 7)) Xor h
					h = (h << 11) Xor h
					h = (CLng(CULng(h) >> 10)) Xor h
				Next j
				byteTable(i) = h
			Next i
			Return byteTable
		End Function

		''' <summary>
		''' This method returns long hash for a given bytes array
		''' </summary>
		''' <param name="data">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static long getLongHash(@NonNull byte[] data)
		Public Shared Function getLongHash(ByVal data() As SByte) As Long
			Dim h As Long = HSTART
'JAVA TO VB CONVERTER NOTE: The variable hmult was renamed since Visual Basic does not handle local variables named the same as class members well:
			Const hmult_Conflict As Long = HMULT
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] ht = byteTable;
			Dim ht() As Long = byteTable
			Dim len As Integer = data.Length
			Dim i As Integer = 0
			Do While i < len
				h = (h * hmult_Conflict) Xor ht(data(i) And &Hff)
				i += 1
			Loop
			Return h
		End Function

		''' <summary>
		''' This method returns long hash for a given string
		''' </summary>
		''' <param name="string">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static long getLongHash(@NonNull String string)
		Public Shared Function getLongHash(ByVal [string] As String) As Long
			Dim h As Long = HSTART
'JAVA TO VB CONVERTER NOTE: The variable hmult was renamed since Visual Basic does not handle local variables named the same as class members well:
			Const hmult_Conflict As Long = HMULT
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] ht = byteTable;
			Dim ht() As Long = byteTable
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int len = string.length();
			Dim len As Integer = [string].Length
			For i As Integer = 0 To len - 1
				Dim ch As Char = [string].Chars(i)
				h = (h * hmult_Conflict) Xor ht(AscW(ch) And &Hff)
				h = (h * hmult_Conflict) Xor ht((CInt(CUInt(ch) >> 8)) And &Hff)
			Next i
			Return h
		End Function
	End Class

End Namespace