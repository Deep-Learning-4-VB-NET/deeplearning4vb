Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.nd4j.linalg.memory

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.WORKSPACES) public class CloseableTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CloseableTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimpleRelease_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleRelease_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})
			assertTrue(array.closeable())

			array.close()

			assertFalse(array.closeable())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCyclicRelease_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCyclicRelease_1(ByVal backend As Nd4jBackend)
			For e As Integer = 0 To 99
				Using array As lombok.val = org.nd4j.linalg.factory.Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})
					array.addi(1.0f)
				End Using
				System.GC.Collect()
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testViewRelease_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testViewRelease_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(5, 5)
			assertTrue(array.closeable())

			Dim view As val = array.get(NDArrayIndex.point(1), NDArrayIndex.all())

			assertTrue(array.closeable())
			assertFalse(view.closeable())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAttachedRelease_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAttachedRelease_1(ByVal backend As Nd4jBackend)
			Dim wsconf As val = WorkspaceConfiguration.builder().build()

			Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsconf, "haha72yjhfdfs")
				Dim array As val = Nd4j.create(5, 5)
				assertFalse(array.closeable())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAccessException_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testAccessException_1(ByVal backend As Nd4jBackend)
		   assertThrows(GetType(System.InvalidOperationException),Sub()
		   Dim array As val = Nd4j.create(5, 5)
		   array.close()
		   array.data().pointer()
		   End Sub)

		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAccessException_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testAccessException_2(ByVal backend As Nd4jBackend)
		  assertThrows(GetType(System.InvalidOperationException),Sub()
		  Dim array As val = Nd4j.create(5, 5)
		  Dim view As val = array.getRow(0)
		  array.close()
		  view.data().pointer()
		  End Sub)
		 End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace