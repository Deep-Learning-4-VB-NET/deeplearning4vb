Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
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

Namespace org.datavec.api.split


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class NumberedFileInputSplitTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class NumberedFileInputSplitTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumberedFileInputSplitBasic()
		Public Overridable Sub testNumberedFileInputSplitBasic()
			Dim baseString As String = "/path/to/files/prefix%d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumberedFileInputSplitVaryIndeces()
		Public Overridable Sub testNumberedFileInputSplitVaryIndeces()
			Dim baseString As String = "/path/to/files/prefix-%d.suffix"
			Dim minIdx As Integer = 3
			Dim maxIdx As Integer = 27
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumberedFileInputSplitBasicNoPrefix()
		Public Overridable Sub testNumberedFileInputSplitBasicNoPrefix()
			Dim baseString As String = "/path/to/files/%d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumberedFileInputSplitWithLeadingZeroes()
		Public Overridable Sub testNumberedFileInputSplitWithLeadingZeroes()
			Dim baseString As String = "/path/to/files/prefix-%07d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumberedFileInputSplitWithLeadingZeroesNoSuffix()
		Public Overridable Sub testNumberedFileInputSplitWithLeadingZeroesNoSuffix()
			Dim baseString As String = "/path/to/files/prefix-%d"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNumberedFileInputSplitWithLeadingSpaces()
		Public Overridable Sub testNumberedFileInputSplitWithLeadingSpaces()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim baseString As String = "/path/to/files/prefix-%5d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNumberedFileInputSplitWithNoLeadingZeroInPadding()
		Public Overridable Sub testNumberedFileInputSplitWithNoLeadingZeroInPadding()
			assertThrows(GetType(System.ArgumentException), Sub()
			Dim baseString As String = "/path/to/files/prefix%5d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNumberedFileInputSplitWithLeadingPlusInPadding()
		Public Overridable Sub testNumberedFileInputSplitWithLeadingPlusInPadding()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim baseString As String = "/path/to/files/prefix%+5d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNumberedFileInputSplitWithLeadingMinusInPadding()
		Public Overridable Sub testNumberedFileInputSplitWithLeadingMinusInPadding()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim baseString As String = "/path/to/files/prefix%-5d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNumberedFileInputSplitWithTwoDigitsInPadding()
		Public Overridable Sub testNumberedFileInputSplitWithTwoDigitsInPadding()
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim baseString As String = "/path/to/files/prefix%011d.suffix"
		   Dim minIdx As Integer = 0
		   Dim maxIdx As Integer = 10
		   runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
		   End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNumberedFileInputSplitWithInnerZerosInPadding()
		Public Overridable Sub testNumberedFileInputSplitWithInnerZerosInPadding()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim baseString As String = "/path/to/files/prefix%101d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNumberedFileInputSplitWithRepeatInnerZerosInPadding()
		Public Overridable Sub testNumberedFileInputSplitWithRepeatInnerZerosInPadding()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim baseString As String = "/path/to/files/prefix%0505d.suffix"
			Dim minIdx As Integer = 0
			Dim maxIdx As Integer = 10
			runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
			End Sub)

		End Sub


		Private Shared Sub runNumberedFileInputSplitTest(ByVal baseString As String, ByVal minIdx As Integer, ByVal maxIdx As Integer)
			Dim split As New NumberedFileInputSplit(baseString, minIdx, maxIdx)
			Dim locs() As URI = split.locations()
			assertEquals(locs.Length, (maxIdx - minIdx) + 1)
			Dim j As Integer = 0
			For i As Integer = minIdx To maxIdx
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String path = locs[j++].getPath();
				Dim path As String = locs(j).getPath()
					j += 1
				Dim exp As String = String.format(baseString, i)
				Dim msg As String = exp & " vs " & path
				assertTrue(path.EndsWith(exp, StringComparison.Ordinal),msg) 'Note: on Windows, Java can prepend drive to path - "/C:/"
			Next i
		End Sub
	End Class

End Namespace