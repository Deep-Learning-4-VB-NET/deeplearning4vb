Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.graph.models.deepwalk


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class TestGraphHuffman extends org.deeplearning4j.BaseDL4JTest
	Public Class TestGraphHuffman
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testGraphHuffman()
		Public Overridable Sub testGraphHuffman()
			'Simple test case from Weiss - Data Structires and Algorithm Analysis in Java 3ed pg436
			'Huffman code is non-unique, but length of code for each node is same for all Huffman codes

			Dim gh As New GraphHuffman(7)

			Dim vertexDegrees() As Integer = {10, 15, 12, 3, 4, 13, 1}

			gh.buildTree(vertexDegrees)

			For i As Integer = 0 To 6
				Dim s As String = i & vbTab & gh.getCodeLength(i) & vbTab & gh.getCodeString(i) & vbTab & vbTab & gh.getCode(i) & vbTab & vbTab & Arrays.toString(gh.getPathInnerNodes(i))
	'            System.out.println(s);
			Next i

			Dim expectedLengths() As Integer = {3, 2, 2, 5, 4, 2, 5}
			For i As Integer = 0 To vertexDegrees.Length - 1
				assertEquals(expectedLengths(i), gh.getCodeLength(i))
			Next i

			'Check that codes are actually unique:
			Dim codeSet As ISet(Of String) = New HashSet(Of String)()
			For i As Integer = 0 To 6
				Dim code As String = gh.getCodeString(i)
				assertFalse(codeSet.Contains(code))
				codeSet.Add(code)
			Next i

			'Furthermore, Huffman code is a prefix code: i.e., no code word is a prefix of any other code word
			'Check all pairs of codes to ensure this holds
			For i As Integer = 0 To 6
				Dim code As String = gh.getCodeString(i)
				For j As Integer = i + 1 To 6
					Dim codeOther As String = gh.getCodeString(j)

					If code.Length = codeOther.Length Then
						assertNotEquals(code, codeOther)
					ElseIf code.Length < codeOther.Length Then
						assertNotEquals(code, codeOther.Substring(0, code.Length))
					Else
						assertNotEquals(codeOther, code.Substring(0, codeOther.Length))
					End If
				Next j
			Next i
		End Sub
	End Class

End Namespace