Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports All = org.nd4j.linalg.api.ops.impl.reduce.bool.All
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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

Namespace org.nd4j.linalg.shape

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class EmptyTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EmptyTests
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmpyArray_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmpyArray_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.empty()

			assertNotNull(array)
			assertTrue(array.isEmpty())

			assertFalse(array.isScalar())
			assertFalse(array.isVector())
			assertFalse(array.isRowVector())
			assertFalse(array.isColumnVector())
			assertFalse(array.isCompressed())
			assertFalse(array.isSparse())

			assertFalse(array.isAttached())

			assertEquals(Nd4j.dataType(), array.dataType())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyDtype_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyDtype_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.empty(DataType.INT)

			assertTrue(array.isEmpty())
			assertEquals(DataType.INT, array.dataType())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyDtype_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyDtype_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.empty(DataType.LONG)

			assertTrue(array.isEmpty())
			assertEquals(DataType.LONG, array.dataType())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat_1(ByVal backend As Nd4jBackend)
			Dim row1 As val = Nd4j.create(New Double(){1, 1, 1, 1}, New Long(){1, 4})
			Dim row2 As val = Nd4j.create(New Double(){2, 2, 2, 2}, New Long(){1, 4})
			Dim row3 As val = Nd4j.create(New Double(){3, 3, 3, 3}, New Long(){1, 4})

			Dim exp As val = Nd4j.create(New Double(){1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3}, New Integer(){3, 4})

			Dim op As val = DynamicCustomOp.builder("concat").addInputs(row1, row2, row3).addIntegerArguments(0).build()

			Nd4j.Executioner.exec(op)

			Dim z As val = op.getOutputArgument(0)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyReductions(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyReductions(ByVal backend As Nd4jBackend)

			Dim empty As INDArray = Nd4j.empty(DataType.FLOAT)
			Try
				empty.sumNumber()
			Catch e As Exception
				assertTrue(e.Message.contains("empty"))
			End Try

			Try
				empty.varNumber()
			Catch e As Exception
				assertTrue(e.Message.contains("empty"))
			End Try

			Try
				empty.stdNumber()
			Catch e As Exception
				assertTrue(e.Message.contains("empty"))
			End Try

			Try
				empty.meanNumber()
			Catch e As Exception
				assertTrue(e.Message.contains("empty"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetEmpty(ByVal backend As Nd4jBackend)
			Dim empty As INDArray = Nd4j.empty(DataType.FLOAT)
			Try
				empty.getFloat(0)
			Catch e As Exception
				assertTrue(e.Message.contains("empty"))
			End Try

			Try
				empty.getDouble(0)
			Catch e As Exception
				assertTrue(e.Message.contains("empty"))
			End Try

			Try
				empty.getLong(0)
			Catch e As Exception
				assertTrue(e.Message.contains("empty"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyWithShape_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyWithShape_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.FLOAT, 2, 0, 3)

			assertNotNull(array)
			assertEquals(DataType.FLOAT, array.dataType())
			assertEquals(0, array.length())
			assertTrue(array.isEmpty())
			assertArrayEquals(New Long(){2, 0, 3}, array.shape())
			assertArrayEquals(New Long(){0, 0, 0}, array.stride())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyWithShape_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyWithShape_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.FLOAT, 0)

			assertNotNull(array)
			assertEquals(DataType.FLOAT, array.dataType())
			assertEquals(0, array.length())
			assertTrue(array.isEmpty())
			assertArrayEquals(New Long(){0}, array.shape())
			assertArrayEquals(New Long(){0}, array.stride())
			assertEquals(1, array.rank())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyWithShape_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testEmptyWithShape_3(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim array As val = Nd4j.create(DataType.FLOAT, 2, 0, 3)
			array.tensorAlongDimension(0, 2)
			End Sub)

		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyWithShape_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyWithShape_4(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.FLOAT, 0, 3)

			assertNotNull(array)
			assertEquals(DataType.FLOAT, array.dataType())
			assertEquals(0, array.length())
			assertTrue(array.isEmpty())
			assertArrayEquals(New Long(){0, 3}, array.shape())
			assertArrayEquals(New Long(){0, 0}, array.stride())
			assertEquals(2, array.rank())
			assertEquals(0, array.rows())
			assertEquals(3, array.columns())
			assertEquals(0, array.size(0))
			assertEquals(3, array.size(1))
			assertEquals(0, array.stride(0))
			assertEquals(0, array.stride(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyReduction_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyReduction_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 2, 0, 3)
			Dim e As val = Nd4j.create(DataType.FLOAT, 2, 1, 3).assign(0)

			Dim reduced As val = x.sum(True, 1)

			assertArrayEquals(e.shape(), reduced.shape())
			assertEquals(e, reduced)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyReduction_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyReduction_2(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 2, 0, 3)
			Dim e As val = Nd4j.create(DataType.FLOAT, 2, 3).assign(0)

			Dim reduced As val = x.sum(False, 1)

			assertArrayEquals(e.shape(), reduced.shape())
			assertEquals(e, reduced)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyReduction_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyReduction_3(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 2, 0)
			Dim e As val = Nd4j.create(DataType.FLOAT, 0)

			Dim reduced As val = x.argMax(0)

			assertArrayEquals(e.shape(), reduced.shape())
			assertEquals(e, reduced)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyReduction_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyReduction_4(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim x As val = Nd4j.create(DataType.FLOAT, 2, 0)
			Dim e As val = Nd4j.create(DataType.FLOAT, 0)
			Dim reduced As val = x.argMax(1)
			assertArrayEquals(e.shape(), reduced.shape())
			assertEquals(e, reduced)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyCreateMethods(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyCreateMethods(ByVal backend As Nd4jBackend)
			Dim dt As DataType = DataType.FLOAT
			assertArrayEquals(New Long(){0}, Nd4j.create(0).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.create(0,0).shape())
			assertArrayEquals(New Long(){0, 0, 0}, Nd4j.create(0,0,0).shape())
			assertArrayEquals(New Long(){0}, Nd4j.create(0L).shape())
			assertArrayEquals(New Long(){0}, Nd4j.create(dt, 0L).shape())

			assertArrayEquals(New Long(){0}, Nd4j.zeros(0).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.zeros(0,0).shape())
			assertArrayEquals(New Long(){0, 0, 0}, Nd4j.zeros(0,0,0).shape())
			assertArrayEquals(New Long(){0, 0, 0}, Nd4j.zeros(New Integer(){0, 0, 0}, "f"c).shape())
			assertArrayEquals(New Long(){0}, Nd4j.zeros(0L).shape())
			assertArrayEquals(New Long(){0}, Nd4j.zeros(dt, 0L).shape())

			assertArrayEquals(New Long(){0}, Nd4j.ones(0).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.ones(0,0).shape())
			assertArrayEquals(New Long(){0, 0, 0}, Nd4j.ones(0,0,0).shape())
			assertArrayEquals(New Long(){0}, Nd4j.ones(0L).shape())
			assertArrayEquals(New Long(){0}, Nd4j.ones(dt, 0L).shape())

			assertArrayEquals(New Long(){0}, Nd4j.valueArrayOf(0, 1.0).shape())
			assertArrayEquals(New Long(){0}, Nd4j.valueArrayOf(0,1.0).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.valueArrayOf(0,0,1.0).shape())
			assertArrayEquals(New Long(){1, 0}, Nd4j.valueArrayOf(New Long(){1, 0}, 1.0).shape())
			assertArrayEquals(New Long(){1, 0}, Nd4j.valueArrayOf(New Long(){1, 0}, 1.0f).shape())
			assertArrayEquals(New Long(){1, 0}, Nd4j.valueArrayOf(New Long(){1, 0}, 1L).shape())
			assertArrayEquals(New Long(){1, 0}, Nd4j.valueArrayOf(New Long(){1, 0}, 1).shape())

			assertArrayEquals(New Long(){0}, Nd4j.createUninitialized(0).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.createUninitialized(0,0).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.createUninitialized(dt, 0,0).shape())

			assertArrayEquals(New Long(){0, 0}, Nd4j.zerosLike(Nd4j.ones(0,0)).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.onesLike(Nd4j.ones(0,0)).shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.ones(0,0).like().shape())
			assertArrayEquals(New Long(){0, 0}, Nd4j.ones(0,0).ulike().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEqualShapesEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEqualShapesEmpty(ByVal backend As Nd4jBackend)
			assertTrue(Nd4j.create(0).equalShapes(Nd4j.create(0)))
			assertFalse(Nd4j.create(0).equalShapes(Nd4j.create(1, 0)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyWhere(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyWhere(ByVal backend As Nd4jBackend)
			Dim mask As val = Nd4j.createFromArray(False, False, False, False, False)
			Dim result As val = Nd4j.where(mask, Nothing, Nothing)

			assertTrue(result(0).isEmpty())
			assertNotNull(result(0).shapeInfoDataBuffer().asLong())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllEmptyReduce(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllEmptyReduce(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.createFromArray(True, True, True)
			Dim all As val = New All(x)
			all.setEmptyReduce(True) 'For TF compatibility - empty array for axis (which means no-op - and NOT all array reduction)
			Dim [out] As INDArray = Nd4j.exec(all)
			assertEquals(x, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyNoop(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyNoop(ByVal backend As Nd4jBackend)
			Dim output As val = Nd4j.empty(DataType.LONG)

			Dim op As val = DynamicCustomOp.builder("noop").addOutputs(output).build()

			Nd4j.exec(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyConstructor_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyConstructor_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(New Double(){})
			assertTrue(x.isEmpty())
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace