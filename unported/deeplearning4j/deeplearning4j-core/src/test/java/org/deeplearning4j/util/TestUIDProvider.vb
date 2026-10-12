Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
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

Namespace org.deeplearning4j.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class TestUIDProvider extends org.deeplearning4j.BaseDL4JTest
	Public Class TestUIDProvider
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIDProvider()
		Public Overridable Sub testUIDProvider()
			Dim jvmUID As String = UIDProvider.JVMUID
			Dim hardwareUID As String = UIDProvider.HardwareUID

			assertNotNull(jvmUID)
			assertNotNull(hardwareUID)

			assertTrue(jvmUID.Length > 0)
			assertTrue(hardwareUID.Length > 0)

			assertEquals(jvmUID, UIDProvider.JVMUID)
			assertEquals(hardwareUID, UIDProvider.HardwareUID)

			Console.WriteLine("JVM uid:      " & jvmUID)
			Console.WriteLine("Hardware uid: " & hardwareUID)
		End Sub

	End Class

End Namespace