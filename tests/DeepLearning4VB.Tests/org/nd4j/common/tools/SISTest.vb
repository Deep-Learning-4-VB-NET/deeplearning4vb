Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports SIS = org.nd4j.common.tools.SIS
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



	Public Class SISTest
		'
		'
		Private sis As SIS
		'

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAll(@TempDir Path tmpFld) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAll(ByVal tmpFld As Path)
			'
			sis = New SIS()
			'
			Dim mtLv As Integer = 0
			'
			sis.initValues(mtLv, "TEST", System.out, System.err, tmpFld.getRoot().toAbsolutePath().ToString(), "Test", "ABC", True, True)
			'
			Dim fFName As String = sis.getfullFileName()
			sis.info(fFName)
			sis.info("aaabbbcccdddeefff")
			'
			assertEquals(33, fFName.Length)
			assertEquals("Z", fFName.Substring(0, 1))
						Dim tempVar = fFName.length() - 13
			assertEquals("_Test_ABC.txt", fFName.Substring(tempVar, fFName.Length - (tempVar)))
		'	assertEquals( "", fFName );
		'	assertEquals( "", tmpFld.getRoot().getAbsolutePath() );
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			'
			Dim mtLv As Integer = 0
			If sis IsNot Nothing Then
				sis.onStop(mtLv)
			End If
			'
		'	tmpFld.delete();
			'
		End Sub



	End Class
End Namespace