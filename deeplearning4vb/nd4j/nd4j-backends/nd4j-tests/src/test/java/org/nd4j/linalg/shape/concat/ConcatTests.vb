Imports System.Collections.Generic
Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.linalg.shape.concat


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class ConcatTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ConcatTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(2), 2, 2)
			Dim B As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim concat As INDArray = Nd4j.concat(0, A, B)
			assertTrue(New Long() {5, 2, 2}.SequenceEqual(concat.shape()))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatHorizontally(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatHorizontally(ByVal backend As Nd4jBackend)
			Dim rowVector As INDArray = Nd4j.ones(1, 5)
			Dim other As INDArray = Nd4j.ones(1, 5)
			Dim concat As INDArray = Nd4j.hstack(other, rowVector)
			assertEquals(rowVector.rows(), concat.rows())
			assertEquals(rowVector.columns() * 2, concat.columns())

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVStackColumn(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVStackColumn(ByVal backend As Nd4jBackend)
			Dim linspaced As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE).reshape(ChrW(3), 1)
			Dim stacked As INDArray = linspaced.dup()
			Dim assertion As INDArray = Nd4j.create(New Double() {1, 2, 3, 1, 2, 3}, New Integer() {6, 1})
			Dim test As INDArray = Nd4j.vstack(linspaced, stacked)
			assertEquals(assertion, test)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatScalars(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatScalars(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.arange(0, 1).reshape(ChrW(1), 1)
			Dim second As INDArray = Nd4j.arange(0, 1).reshape(ChrW(1), 1)
			Dim firstRet As INDArray = Nd4j.concat(0, first, second)
			assertTrue(firstRet.ColumnVector)
			Dim secondRet As INDArray = Nd4j.concat(1, first, second)
			assertTrue(secondRet.RowVector)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatMatrices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatMatrices(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim b As INDArray = a.dup()


			Dim concat1 As INDArray = Nd4j.concat(1, a, b)
			Dim oneAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3, 1, 3},
				New Double() {2, 4, 2, 4}
			})
			assertEquals(oneAssertion, concat1)

			Dim concat As INDArray = Nd4j.concat(0, a, b)
			Dim zeroAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {2, 4},
				New Double() {1, 3},
				New Double() {2, 4}
			})
			assertEquals(zeroAssertion, concat)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatRowVectors(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatRowVectors(ByVal backend As Nd4jBackend)
			Dim rowVector As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6}, New Integer() {1, 6})
			Dim matrix As INDArray = Nd4j.create(New Double() {7, 8, 9, 10, 11, 12}, New Integer() {1, 6})

			Dim assertion1 As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}, New Integer() {1, 12})
			Dim assertion0 As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 3, 4, 5, 6},
				New Double() {7, 8, 9, 10, 11, 12}
			})

			'      INDArray concat1 = Nd4j.hstack(rowVector, matrix);
			Dim concat0 As INDArray = Nd4j.vstack(rowVector, matrix)
			'        assertEquals(assertion1, concat1);
			assertEquals(assertion0, concat0)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat3d(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape("c"c, 2, 3, 4)
			Dim second As INDArray = Nd4j.linspace(24, 36, 12, DataType.DOUBLE).reshape("c"c, 1, 3, 4)
			Dim third As INDArray = Nd4j.linspace(36, 48, 12, DataType.DOUBLE).reshape("c"c, 1, 3, 4)

			'ConcatV2, dim 0
			Dim exp As INDArray = Nd4j.create(DataType.DOUBLE, 2 + 1 + 1, 3, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.interval(0, 2), NDArrayIndex.all(), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(3), NDArrayIndex.all(), NDArrayIndex.all()}, third)

			Dim concat0 As INDArray = Nd4j.concat(0, first, second, third)

			assertEquals(exp, concat0)

	'        System.out.println("1------------------------");

			'ConcatV2, dim 1
			second = Nd4j.linspace(24, 32, 8, DataType.DOUBLE).reshape("c"c, 2, 1, 4)
			third = Nd4j.linspace(32, 48, 16, DataType.DOUBLE).reshape("c"c, 2, 2, 4)
			exp = Nd4j.create(DataType.DOUBLE, 2, 3 + 1 + 2, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(0, 3), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.point(3), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(4, 6), NDArrayIndex.all()}, third)

	'        System.out.println("2------------------------");

			Dim concat1 As INDArray = Nd4j.concat(1, first, second, third)

			assertEquals(exp, concat1)

			'ConcatV2, dim 2
			second = Nd4j.linspace(24, 36, 12, DataType.DOUBLE).reshape("c"c, 2, 3, 2)
			third = Nd4j.linspace(36, 42, 6, DataType.DOUBLE).reshape("c"c, 2, 3, 1)
			exp = Nd4j.create(DataType.DOUBLE, 2, 3, 4 + 2 + 1)

			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, 6)}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(6)}, third)

			Dim concat2 As INDArray = Nd4j.concat(2, first, second, third)

			assertEquals(exp, concat2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat3dv2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat3dv2(ByVal backend As Nd4jBackend)

			Dim first As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape("c"c, 2, 3, 4)
			Dim second As INDArray = Nd4j.linspace(24, 35, 12, DataType.DOUBLE).reshape("c"c, 1, 3, 4)
			Dim third As INDArray = Nd4j.linspace(36, 47, 12, DataType.DOUBLE).reshape("c"c, 1, 3, 4)

			'ConcatV2, dim 0
			Dim exp As INDArray = Nd4j.create(2 + 1 + 1, 3, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.interval(0, 2), NDArrayIndex.all(), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(3), NDArrayIndex.all(), NDArrayIndex.all()}, third)

			Dim firsts As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){2, 3, 4}, DataType.DOUBLE)
			Dim seconds As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){1, 3, 4}, DataType.DOUBLE)
			Dim thirds As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){1, 3, 4}, DataType.DOUBLE)
			For Each f As Pair(Of INDArray, String) In firsts
				For Each s As Pair(Of INDArray, String) In seconds
					For Each t As Pair(Of INDArray, String) In thirds
						Dim f2 As INDArray = f.First.assign(first)
						Dim s2 As INDArray = s.First.assign(second)
						Dim t2 As INDArray = t.First.assign(third)

	'                    System.out.println("-------------------------------------------");
						Dim concat0 As INDArray = Nd4j.concat(0, f2, s2, t2)

						assertEquals(exp, concat0)
					Next t
				Next s
			Next f

			'ConcatV2, dim 1
			second = Nd4j.linspace(24, 31, 8, DataType.DOUBLE).reshape("c"c, 2, 1, 4)
			third = Nd4j.linspace(32, 47, 16, DataType.DOUBLE).reshape("c"c, 2, 2, 4)
			exp = Nd4j.create(2, 3 + 1 + 2, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(0, 3), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.point(3), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(4, 6), NDArrayIndex.all()}, third)

			firsts = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){2, 3, 4}, DataType.DOUBLE)
			seconds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){2, 1, 4}, DataType.DOUBLE)
			thirds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){2, 2, 4}, DataType.DOUBLE)
			For Each f As Pair(Of INDArray, String) In firsts
				For Each s As Pair(Of INDArray, String) In seconds
					For Each t As Pair(Of INDArray, String) In thirds
						Dim f2 As INDArray = f.First.assign(first)
						Dim s2 As INDArray = s.First.assign(second)
						Dim t2 As INDArray = t.First.assign(third)

						Dim concat1 As INDArray = Nd4j.concat(1, f2, s2, t2)

						assertEquals(exp, concat1)
					Next t
				Next s
			Next f

			'ConcatV2, dim 2
			second = Nd4j.linspace(24, 35, 12, DataType.DOUBLE).reshape("c"c, 2, 3, 2)
			third = Nd4j.linspace(36, 41, 6, DataType.DOUBLE).reshape("c"c, 2, 3, 1)
			exp = Nd4j.create(2, 3, 4 + 2 + 1)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, 6)}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(6)}, third)

			firsts = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){2, 3, 4}, DataType.DOUBLE)
			seconds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){2, 3, 2}, DataType.DOUBLE)
			thirds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){2, 3, 1}, DataType.DOUBLE)
			For Each f As Pair(Of INDArray, String) In firsts
				For Each s As Pair(Of INDArray, String) In seconds
					For Each t As Pair(Of INDArray, String) In thirds
						Dim f2 As INDArray = f.First.assign(first)
						Dim s2 As INDArray = s.First.assign(second)
						Dim t2 As INDArray = t.First.assign(third)

						Dim concat2 As INDArray = Nd4j.concat(2, f2, s2, t2)

						assertEquals(exp, concat2)
					Next t
				Next s
			Next f
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void concatf()
		Public Overridable Sub concatf()
			Dim orderBefore As Char = Nd4j.order()
			Try
				Nd4j.factory().Order = "f"c 'Required to reproduce problem
				Dim x As INDArray = Nd4j.create(New Double(){1, 2, 3, 4, 5, 6}, New Integer(){1, 6}, "c"c) 'These can be C or F - no difference
				Dim y As INDArray = Nd4j.create(New Double(){7, 8, 9, 10, 11, 12}, New Integer(){1, 6}, "c"c)

				Dim [out] As INDArray = Nd4j.concat(0, x, y)

				Dim exp As INDArray = Nd4j.createFromArray(New Double()(){
					New Double() {1, 2, 3, 4, 5, 6},
					New Double() {7, 8, 9, 10, 11, 12}
				})

				assertEquals(exp, [out])
			Finally
				Nd4j.factory().Order = orderBefore
			End Try
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace