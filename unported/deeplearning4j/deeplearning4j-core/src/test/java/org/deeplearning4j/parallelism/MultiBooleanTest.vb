Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiBoolean = org.deeplearning4j.datasets.iterator.parallel.MultiBoolean
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertTrue
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
Namespace org.deeplearning4j.parallelism

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Multi Boolean Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class MultiBooleanTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class MultiBooleanTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Boolean 1") void testBoolean1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBoolean1()
			Dim bool As New MultiBoolean(5)
			assertTrue(bool.allFalse())
			assertFalse(bool.allTrue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Boolean 2") void testBoolean2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBoolean2()
			Dim bool As New MultiBoolean(5)
			bool.set(True, 2)
			assertFalse(bool.allFalse())
			assertFalse(bool.allTrue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Boolean 3") void testBoolean3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBoolean3()
			Dim bool As New MultiBoolean(5)
			bool.set(True, 0)
			bool.set(True, 1)
			bool.set(True, 2)
			bool.set(True, 3)
			assertFalse(bool.allTrue())
			bool.set(True, 4)
			assertFalse(bool.allFalse())
			assertTrue(bool.allTrue())
			bool.set(False, 2)
			assertFalse(bool.allTrue())
			bool.set(True, 2)
			assertTrue(bool.allTrue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Boolean 4") void testBoolean4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBoolean4()
			Dim bool As New MultiBoolean(5, True)
			assertTrue(bool.get(1))
			bool.set(False, 1)
			assertFalse(bool.get(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Boolean 5") void testBoolean5() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBoolean5()
			Dim bool As New MultiBoolean(5, True, True)
			For i As Integer = 0 To 4
				bool.set(False, i)
			Next i
			For i As Integer = 0 To 4
				bool.set(True, i)
			Next i
			assertTrue(bool.allFalse())
		End Sub
	End Class

End Namespace