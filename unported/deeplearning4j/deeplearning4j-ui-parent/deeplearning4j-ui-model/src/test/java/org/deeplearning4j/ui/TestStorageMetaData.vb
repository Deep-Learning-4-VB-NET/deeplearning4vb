Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports SbeStorageMetaData = org.deeplearning4j.ui.model.storage.impl.SbeStorageMetaData
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

Namespace org.deeplearning4j.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestStorageMetaData extends org.deeplearning4j.BaseDL4JTest
	Public Class TestStorageMetaData
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStorageMetaData()
		Public Overridable Sub testStorageMetaData()

			Dim extraMeta As Serializable = "ExtraMetaData"
			Dim timeStamp As Long = 123456
			Dim m As StorageMetaData = New SbeStorageMetaData(timeStamp, "sessionID", "typeID", "workerID", "org.some.class.InitType", "org.some.class.UpdateType", extraMeta)

			Dim bytes() As SByte = m.encode()
			Dim m2 As StorageMetaData = New SbeStorageMetaData()
			m2.decode(bytes)

			assertEquals(m, m2)
			assertArrayEquals(bytes, m2.encode())

			'Sanity check: null values
			m = New SbeStorageMetaData(0, Nothing, Nothing, Nothing, Nothing, DirectCast(Nothing, String))
			bytes = m.encode()
			m2 = New SbeStorageMetaData()
			m2.decode(bytes)
			'In practice, we don't want these things to ever be null anyway...
			assertNullOrZeroLength(m2.SessionID)
			assertNullOrZeroLength(m2.TypeID)
			assertNullOrZeroLength(m2.WorkerID)
			assertNullOrZeroLength(m2.InitTypeClass)
			assertNullOrZeroLength(m2.UpdateTypeClass)
			assertArrayEquals(bytes, m2.encode())
		End Sub

		Private Shared Sub assertNullOrZeroLength(ByVal str As String)
			assertTrue(str Is Nothing OrElse str.Length = 0)
		End Sub

	End Class

End Namespace