Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports ArrayCacheMemoryMgr = org.nd4j.autodiff.samediff.internal.memory.ArrayCacheMemoryMgr
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.WORKSPACES) public class MemoryMgrTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class MemoryMgrTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayReuseTooLarge(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testArrayReuseTooLarge(ByVal backend As Nd4jBackend)

			Dim mmgr As New ArrayCacheMemoryMgr()
			Dim f As System.Reflection.FieldInfo = GetType(ArrayCacheMemoryMgr).getDeclaredField("maxCacheBytes")
			f.setAccessible(True)
			f.set(mmgr, 1000)

			assertEquals(1000, mmgr.getMaxCacheBytes())

			Dim arrays(99) As INDArray
			For i As Integer = 0 To arrays.Length - 1
				arrays(i) = Nd4j.create(DataType.FLOAT, 25) '100 bytes each
			Next i

			For i As Integer = 0 To 9
				mmgr.release(arrays(i))
			Next i

			assertEquals(1000, mmgr.getCurrentCacheSize())
			Dim [as] As ArrayCacheMemoryMgr.ArrayStore = mmgr.getArrayStores().get(DataType.FLOAT)
			assertEquals(1000, [as].getBytesSum())
			assertEquals(250, [as].getLengthSum())
			assertEquals(10, [as].getSize())
			assertEquals(10, mmgr.getLruCache().size())
			assertEquals(10, mmgr.getLruCacheValues().size())


			'At this point: array store is full.
			'If we try to release more, the oldest (first released) values should be closed
			For i As Integer = 0 To 9
				Dim toRelease As INDArray = Nd4j.create(DataType.FLOAT, 25)
				mmgr.release(toRelease)
				'oldest N only should be closed by this point...
				For j As Integer = 0 To 9
					If j <= i Then
						'Should have been closed
						assertTrue(arrays(j).wasClosed())
					Else
						'Should still be open
						assertFalse(arrays(j).wasClosed())
					End If
				Next j
			Next i


			assertEquals(1000, mmgr.getCurrentCacheSize())
			assertEquals(1000, [as].getBytesSum())
			assertEquals(250, [as].getLengthSum())
			assertEquals(10, [as].getSize())
			assertEquals(10, mmgr.getLruCache().size())
			assertEquals(10, mmgr.getLruCacheValues().size())

			'now, allocate some values:
			For i As Integer = 1 To 10
				Dim a1 As INDArray = mmgr.allocate(True, DataType.FLOAT, 25)
				assertEquals(1000 - i * 100, mmgr.getCurrentCacheSize())
				assertEquals(1000 - i * 100, [as].getBytesSum())
				assertEquals(250 - i * 25, [as].getLengthSum())
				assertEquals(10 - i, [as].getSize())
				assertEquals(10 - i, mmgr.getLruCache().size())
				assertEquals(10 - i, mmgr.getLruCacheValues().size())
			Next i

			assertEquals(0, mmgr.getCurrentCacheSize())
			assertEquals(0, [as].getBytesSum())
			assertEquals(0, [as].getLengthSum())
			assertEquals(0, [as].getSize())
			assertEquals(0, mmgr.getLruCache().size())
			assertEquals(0, mmgr.getLruCacheValues().size())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testManyArrays(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testManyArrays(ByVal backend As Nd4jBackend)

			Dim mmgr As New ArrayCacheMemoryMgr()
			For i As Integer = 0 To 999
				mmgr.release(Nd4j.scalar(0))
			Next i

			assertEquals(4*1000, mmgr.getCurrentCacheSize())
			assertEquals(1000, mmgr.getLruCache().size())
			assertEquals(1000, mmgr.getLruCacheValues().size())

			For i As Integer = 0 To 999
				mmgr.release(Nd4j.scalar(0))
			Next i

			assertEquals(4*2000, mmgr.getCurrentCacheSize())
			assertEquals(2000, mmgr.getLruCache().size())
			assertEquals(2000, mmgr.getLruCacheValues().size())
		End Sub

	End Class

End Namespace