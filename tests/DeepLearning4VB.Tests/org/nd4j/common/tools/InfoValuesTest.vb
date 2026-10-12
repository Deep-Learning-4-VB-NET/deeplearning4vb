Imports Test = org.junit.jupiter.api.Test
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

	Public Class InfoValuesTest
		'
		Private t1_titleA() As String = { "T0", "T1", "T2", "T3", "T4", "T5" }
		'
		Private t2_titleA() As String = { "", "T1", "T2" }
		'

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testconstructor() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testconstructor()
			'
			Dim iv As InfoValues
			'
			iv = New InfoValues(t1_titleA)
			assertEquals("T0", iv.titleA(0))
			assertEquals("T1", iv.titleA(1))
			assertEquals("T2", iv.titleA(2))
			assertEquals("T3", iv.titleA(3))
			assertEquals("T4", iv.titleA(4))
			assertEquals("T5", iv.titleA(5))
			'
			iv = New InfoValues(t2_titleA)
			assertEquals("", iv.titleA(0))
			assertEquals("T1", iv.titleA(1))
			assertEquals("T2", iv.titleA(2))
			assertEquals("", iv.titleA(3))
			assertEquals("", iv.titleA(4))
			assertEquals("", iv.titleA(5))
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testgetValues() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testgetValues()
			'
			Dim iv As InfoValues
			'
			iv = New InfoValues("Test")
			iv.vsL.Add(" AB ")
			iv.vsL.Add(" CD ")
			'
			assertEquals(" AB | CD |", iv.Values)
			'
		End Sub


	End Class

End Namespace