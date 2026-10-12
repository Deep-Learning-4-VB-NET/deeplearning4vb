Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports UiConnectionInfo = org.deeplearning4j.core.ui.UiConnectionInfo
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.deeplearning4j.ui

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Ui Connection Info Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.UI) class UiConnectionInfoTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class UiConnectionInfoTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub setUp()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get First Part 1") void testGetFirstPart1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetFirstPart1()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setPort(8080).build()
			assertEquals(info.FirstPart, "http://localhost:8080")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get First Part 2") void testGetFirstPart2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetFirstPart2()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).enableHttps(True).setPort(8080).build()
			assertEquals(info.FirstPart, "https://localhost:8080")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get First Part 3") void testGetFirstPart3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetFirstPart3()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).build()
			assertEquals(info.FirstPart, "https://192.168.1.1:8082")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 1") void testGetSecondPart1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart1()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).setPath("www-data").build()
			assertEquals(info.SecondPart, "/www-data/")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 2") void testGetSecondPart2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart2()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).setPath("/www-data/tmp/").build()
			assertEquals(info.SecondPart, "/www-data/tmp/")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 3") void testGetSecondPart3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart3()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).setPath("/www-data/tmp").build()
			assertEquals(info.SecondPart, "/www-data/tmp/")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 4") void testGetSecondPart4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart4()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).setPath("/www-data//tmp").build()
			assertEquals(info.SecondPart, "/www-data/tmp/")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 5") void testGetSecondPart5() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart5()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).setPath("/www-data//tmp").build()
			assertEquals(info.getSecondPart("alpha"), "/www-data/tmp/alpha/")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 6") void testGetSecondPart6() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart6()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).setPath("//www-data//tmp").build()
			assertEquals(info.getSecondPart("/alpha/"), "/www-data/tmp/alpha/")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 7") void testGetSecondPart7() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart7()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(True).setPort(8082).setPath("//www-data//tmp").build()
			assertEquals(info.getSecondPart("/alpha//beta/"), "/www-data/tmp/alpha/beta/")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get Second Part 8") void testGetSecondPart8() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetSecondPart8()
			Dim info As UiConnectionInfo = (New UiConnectionInfo.Builder()).setAddress("192.168.1.1").enableHttps(False).setPort(8082).setPath("/www-data//tmp").build()
			assertEquals(info.FullAddress, "http://192.168.1.1:8082/www-data/tmp/")
		End Sub
	End Class

End Namespace