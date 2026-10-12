Imports Test = org.junit.jupiter.api.Test
Imports InfoLine = org.nd4j.common.tools.InfoLine
Imports InfoValues = org.nd4j.common.tools.InfoValues
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

	Public Class InfoLineTest
		'

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAll() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAll()
			'
			Dim iv0 As New InfoValues(" A", " B")
			Dim iv1 As New InfoValues(" C", " D")
			Dim iv2 As New InfoValues(" E", " F", " G", " H")
			'
			iv0.vsL.Add(" ab ")
			iv1.vsL.Add(" cd ")
			iv2.vsL.Add(" ef ")
			'
			Dim il As New InfoLine()
			'
			il.ivL.Add(iv0)
			il.ivL.Add(iv1)
			il.ivL.Add(iv2)
			'
			Dim mtLv As Integer = 2
			'
			assertEquals(".. | A  | C  | E  |", il.getTitleLine(mtLv, 0))
			assertEquals(".. | B  | D  | F  |", il.getTitleLine(mtLv, 1))
			assertEquals(".. |    |    | G  |", il.getTitleLine(mtLv, 2))
			assertEquals(".. |    |    | H  |", il.getTitleLine(mtLv, 3))
			assertEquals(".. | ab | cd | ef |", il.getValuesLine(mtLv))
			'
		End Sub



	End Class
End Namespace