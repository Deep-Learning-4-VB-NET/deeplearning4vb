Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
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
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
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

Namespace org.nd4j.linalg.shape.concat



	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class ConcatTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ConcatTestsC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatVertically(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatVertically(ByVal backend As Nd4jBackend)
			Dim rowVector As INDArray = Nd4j.ones(1, 5)
			Dim other As INDArray = Nd4j.ones(1, 5)
			Dim concat As INDArray = Nd4j.vstack(other, rowVector)
			assertEquals(rowVector.rows() * 2, concat.rows())
			assertEquals(rowVector.columns(), concat.columns())

			Dim arr2 As INDArray = Nd4j.create(5, 5)
			Dim slice1 As INDArray = arr2.slice(0)
			Dim slice2 As INDArray = arr2.slice(1)
			Dim arr3 As INDArray = Nd4j.create(2, 5)
			Dim vstack As INDArray = Nd4j.vstack(slice1, slice2)
			assertEquals(arr3, vstack)

			Dim col1 As INDArray = arr2.getColumn(0).reshape(ChrW(5), 1)
			Dim col2 As INDArray = arr2.getColumn(1).reshape(ChrW(5), 1)
			Dim vstacked As INDArray = Nd4j.vstack(col1, col2)
			assertEquals(Nd4j.create(10, 1), vstacked)
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatScalars1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatScalars1(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.scalar(1)
			Dim second As INDArray = Nd4j.scalar(2)
			Dim third As INDArray = Nd4j.scalar(3)

			Dim result As INDArray = Nd4j.concat(0, first, second, third)

			assertEquals(1f, result.getFloat(0), 0.01f)
			assertEquals(2f, result.getFloat(1), 0.01f)
			assertEquals(3f, result.getFloat(2), 0.01f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatVectors1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatVectors1(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.ones(1, 10)
			Dim second As INDArray = Nd4j.ones(1, 10)
			Dim third As INDArray = Nd4j.ones(1, 10)

			Dim result As INDArray = Nd4j.concat(0, first, second, third)

			assertEquals(3, result.rows())
			assertEquals(10, result.columns())

	'        System.out.println(result);

			For x As Integer = 0 To 29
				assertEquals(1f, result.getFloat(x), 0.001f)
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatMatrices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatMatrices(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim b As INDArray = a.dup()


			Dim concat1 As INDArray = Nd4j.concat(1, a, b)
			Dim oneAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 1, 2},
				New Double() {3, 4, 3, 4}
			})

	'        System.out.println("Assertion: " + Arrays.toString(oneAssertion.data().asFloat()));
	'        System.out.println("Result: " + Arrays.toString(concat1.data().asFloat()));

			assertEquals(oneAssertion, concat1)

			Dim concat As INDArray = Nd4j.concat(0, a, b)
			Dim zeroAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {3, 4},
				New Double() {1, 2},
				New Double() {3, 4}
			})
			assertEquals(zeroAssertion, concat)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign(ByVal backend As Nd4jBackend)
			Dim vector As INDArray = Nd4j.linspace(1, 5, 5, Nd4j.dataType())
			vector.assign(1)
			assertEquals(Nd4j.ones(5), vector)
			Dim twos As INDArray = Nd4j.ones(2, 2)
			Dim rand As INDArray = Nd4j.rand(2, 2)
			twos.assign(rand)
			assertEquals(rand, twos)

			Dim tensor As INDArray = Nd4j.rand(New Long(){3L, 3L, 3L})
			Dim ones As INDArray = Nd4j.ones(3, 3, 3)
			assertTrue(tensor.shape().SequenceEqual(ones.shape()))
			ones.assign(tensor)
			assertEquals(tensor, ones)
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

			Dim concat1 As INDArray = Nd4j.hstack(rowVector, matrix)
			Dim concat0 As INDArray = Nd4j.vstack(rowVector, matrix)
			assertEquals(assertion1, concat1)
			assertEquals(assertion0, concat0)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat3d(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.linspace(1, 24, 24, Nd4j.dataType()).reshape("c"c, 2, 3, 4)
			Dim second As INDArray = Nd4j.linspace(24, 36, 12, Nd4j.dataType()).reshape("c"c, 1, 3, 4)
			Dim third As INDArray = Nd4j.linspace(36, 48, 12, Nd4j.dataType()).reshape("c"c, 1, 3, 4)

			'ConcatV2, dim 0
			Dim exp As INDArray = Nd4j.create(2 + 1 + 1, 3, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.interval(0, 2), NDArrayIndex.all(), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(3), NDArrayIndex.all(), NDArrayIndex.all()}, third)

			Dim concat0 As INDArray = Nd4j.concat(0, first, second, third)

			assertEquals(exp, concat0)

			'ConcatV2, dim 1
			second = Nd4j.linspace(24, 32, 8, Nd4j.dataType()).reshape("c"c, 2, 1, 4)
			Dim i As Integer = 0
			Do While i < second.tensorsAlongDimension(1)
				Dim secondTad As INDArray = second.tensorAlongDimension(i, 1)
	'            System.out.println(second.tensorAlongDimension(i, 1));
				i += 1
			Loop

			third = Nd4j.linspace(32, 48, 16).reshape("c"c, 2, 2, 4)
			exp = Nd4j.create(2, 3 + 1 + 2, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(0, 3), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.point(3), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(4, 6), NDArrayIndex.all()}, third)

			Dim concat1 As INDArray = Nd4j.concat(1, first, second, third)

			assertEquals(exp, concat1)

			'ConcatV2, dim 2
			second = Nd4j.linspace(24, 36, 12).reshape("c"c, 2, 3, 2)
			third = Nd4j.linspace(36, 42, 6).reshape("c"c, 2, 3, 1)
			exp = Nd4j.create(2, 3, 4 + 2 + 1)

			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, 6)}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(6)}, third)

			Dim concat2 As INDArray = Nd4j.concat(2, first, second, third)

			assertEquals(exp, concat2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatVector(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.concat(0, Nd4j.ones(1,1000000), Nd4j.create(1, 1))
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat3dv2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat3dv2(ByVal backend As Nd4jBackend)

			Dim first As INDArray = Nd4j.linspace(1, 24, 24).reshape("c"c, 2, 3, 4)
			Dim second As INDArray = Nd4j.linspace(24, 35, 12).reshape("c"c, 1, 3, 4)
			Dim third As INDArray = Nd4j.linspace(36, 47, 12).reshape("c"c, 1, 3, 4)

			'ConcatV2, dim 0
			Dim exp As INDArray = Nd4j.create(2 + 1 + 1, 3, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.interval(0, 2), NDArrayIndex.all(), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.point(3), NDArrayIndex.all(), NDArrayIndex.all()}, third)

			Dim firsts As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){2, 3, 4}, DataType.DOUBLE)
			Dim seconds As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){1, 3, 4}, DataType.DOUBLE)
			Dim thirds As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){1, 3, 4}, DataType.DOUBLE)
			For Each f As Pair(Of INDArray, String) In firsts
				For Each s As Pair(Of INDArray, String) In seconds
					For Each t As Pair(Of INDArray, String) In thirds
						Dim f2 As INDArray = f.First.assign(first)
						Dim s2 As INDArray = s.First.assign(second)
						Dim t2 As INDArray = t.First.assign(third)

						Dim concat0 As INDArray = Nd4j.concat(0, f2, s2, t2)
						If Not exp.Equals(concat0) Then
							concat0 = Nd4j.concat(0, f2, s2, t2)
						End If
						assertEquals(exp, concat0)
					Next t
				Next s
			Next f

			'ConcatV2, dim 1
			second = Nd4j.linspace(24, 31, 8).reshape("c"c, 2, 1, 4)
			third = Nd4j.linspace(32, 47, 16).reshape("c"c, 2, 2, 4)
			exp = Nd4j.create(2, 3 + 1 + 2, 4)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(0, 3), NDArrayIndex.all()}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.point(3), NDArrayIndex.all()}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(4, 6), NDArrayIndex.all()}, third)

			firsts = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){2, 3, 4}, DataType.DOUBLE)
			seconds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){2, 1, 4}, DataType.DOUBLE)
			thirds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){2, 2, 4}, DataType.DOUBLE)
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
			second = Nd4j.linspace(24, 35, 12).reshape("c"c, 2, 3, 2)
			third = Nd4j.linspace(36, 41, 6).reshape("c"c, 2, 3, 1)
			exp = Nd4j.create(2, 3, 4 + 2 + 1)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)}, first)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(4, 6)}, second)
			exp.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(6)}, third)

			firsts = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){2, 3, 4}, DataType.DOUBLE)
			seconds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){2, 3, 2}, DataType.DOUBLE)
			thirds = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){2, 3, 1}, DataType.DOUBLE)
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLargeConcat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLargeConcat(ByVal backend As Nd4jBackend)
			Dim list As val = New List(Of INDArray)()

			For e As Integer = 0 To 19999
				list.add(Nd4j.create(DataType.INT, 1, 300).assign(e))
			Next e

			Dim timeStart As val = System.nanoTime()
			Dim result As val = Nd4j.concat(0, list.toArray(New INDArray(list.size() - 1){}))
			Dim timeEnd As val = System.nanoTime()

			log.info("Time: {} us", (timeEnd - timeStart) / 1000)

			For e As Integer = 0 To 19999
				assertEquals(CSng(e), result.getRow(e).meanNumber().floatValue(), 1e-5f)
			Next e
		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace