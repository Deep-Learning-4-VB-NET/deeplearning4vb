Imports Test = org.junit.jupiter.api.Test
Imports BTools = org.nd4j.common.tools.BTools
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

Namespace org.nd4j.common.tools

	Public Class BToolsTest
		'

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetMtLvESS() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetMtLvESS()
			'
			assertEquals("?", BTools.getMtLvESS(-5))
			assertEquals("", BTools.getMtLvESS(0))
			assertEquals("...", BTools.getMtLvESS(3))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetMtLvISS() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetMtLvISS()
			'
			assertEquals(" ", BTools.MtLvISS)
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetSpaces() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetSpaces()
			'
			assertEquals("?", BTools.getSpaces(-3))
			assertEquals("", BTools.getSpaces(0))
			assertEquals("    ", BTools.getSpaces(4))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetSBln() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetSBln()
			'
			assertEquals("?", BTools.getSBln())
			assertEquals("?", BTools.getSBln(Nothing))
			assertEquals("T", BTools.getSBln(True))
			assertEquals("F", BTools.getSBln(False))
			assertEquals("TFFT", BTools.getSBln(True, False, False, True))
			assertEquals("FTFFT", BTools.getSBln(False, True, False, False, True))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetSDbl() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetSDbl()
			'
			assertEquals("NaN", BTools.getSDbl(Double.NaN, 0))
			assertEquals("-6", BTools.getSDbl(-5.5R, 0))
			assertEquals("-5.50", BTools.getSDbl(-5.5R, 2))
			assertEquals("-5.30", BTools.getSDbl(-5.3R, 2))
			assertEquals("-5", BTools.getSDbl(-5.3R, 0))
			assertEquals("0.00", BTools.getSDbl(0R, 2))
			assertEquals("0", BTools.getSDbl(0R, 0))
			assertEquals("0.30", BTools.getSDbl(0.3R, 2))
			assertEquals("4.50", BTools.getSDbl(4.5R, 2))
			assertEquals("4", BTools.getSDbl(4.5R, 0))
			assertEquals("6", BTools.getSDbl(5.5R, 0))
			assertEquals("12 345 678", BTools.getSDbl(12345678R, 0))
			'
			assertEquals("-456", BTools.getSDbl(-456R, 0, False))
			assertEquals("-456", BTools.getSDbl(-456R, 0, True))
			assertEquals("+456", BTools.getSDbl(456R, 0, True))
			assertEquals("456", BTools.getSDbl(456R, 0, False))
			assertEquals(" 0", BTools.getSDbl(0R, 0, True))
			assertEquals("0", BTools.getSDbl(0R, 0, False))
			'
			assertEquals("  4.50", BTools.getSDbl(4.5R, 2, False, 6))
			assertEquals(" +4.50", BTools.getSDbl(4.5R, 2, True, 6))
			assertEquals("   +456", BTools.getSDbl(456R, 0, True, 7))
			assertEquals("    456", BTools.getSDbl(456R, 0, False, 7))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetSInt() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetSInt()
			'
			assertEquals("23", BTools.getSInt(23, 1))
			assertEquals("23", BTools.getSInt(23, 2))
			assertEquals(" 23", BTools.getSInt(23, 3))
			'
			assertEquals("0000056", BTools.getSInt(56, 7, "0"c))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetSIntA() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetSIntA()
			'
			assertEquals("?", BTools.getSIntA(Nothing))
			assertEquals("?", BTools.getSIntA())
			assertEquals("0", BTools.getSIntA(0))
			assertEquals("5, 6, 7", BTools.getSIntA(5, 6, 7))
			Dim intA() As Integer = { 2, 3, 4, 5, 6 }
			assertEquals("2, 3, 4, 5, 6", BTools.getSIntA(intA))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetIndexCharsCount() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetIndexCharsCount()
			'
			assertEquals(1, BTools.getIndexCharsCount(-5))
			assertEquals(1, BTools.getIndexCharsCount(5))
			assertEquals(3, BTools.getIndexCharsCount(345))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetSLcDtTm() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetSLcDtTm()
			'
			assertEquals(15, BTools.SLcDtTm.Length)
			assertEquals("LDTm: ", BTools.SLcDtTm.Substring(0, 6))
			'
		End Sub


	End Class
End Namespace