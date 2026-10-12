Imports System.Collections.Generic
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
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.indexing.NDArrayIndex.all

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


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class ShapeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ShapeTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowColVectorVsScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowColVectorVsScalar(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(2)
			assertTrue(arr.RowVector)
			Dim colVector As INDArray = arr.reshape(ChrW(2), 1)
			assertTrue(colVector.ColumnVector)
			assertFalse(arr.Scalar)
			assertFalse(colVector.Scalar)

			Dim arr3 As INDArray = Nd4j.scalar(1.0)
			assertFalse(arr3.ColumnVector)
			assertFalse(arr3.RowVector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSixteenZeroOne(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSixteenZeroOne(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			assertEquals(4, baseArr.tensorsAlongDimension(0, 1))
			Dim columnVectorFirst As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {2, 4}
			})
			Dim columnVectorSecond As INDArray = Nd4j.create(New Double()() {
				New Double() {9, 11},
				New Double() {10, 12}
			})
			Dim columnVectorThird As INDArray = Nd4j.create(New Double()() {
				New Double() {5, 7},
				New Double() {6, 8}
			})
			Dim columnVectorFourth As INDArray = Nd4j.create(New Double()() {
				New Double() {13, 15},
				New Double() {14, 16}
			})
			Dim assertions() As INDArray = {columnVectorFirst, columnVectorSecond, columnVectorThird, columnVectorFourth}

			Dim i As Integer = 0
			Do While i < baseArr.tensorsAlongDimension(0, 1)
				Dim test As INDArray = baseArr.tensorAlongDimension(i, 0, 1)
				assertEquals(assertions(i), test,"Wrong at index " & i)
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorAlongDimension1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorAlongDimension1(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(1, 5, 5)
			assertEquals(arr.vectorsAlongDimension(0), 5)
			assertEquals(arr.vectorsAlongDimension(1), 5)
			Dim i As Integer = 0
			Do While i < arr.vectorsAlongDimension(0)
				If i < arr.vectorsAlongDimension(0) - 1 AndAlso i > 0 Then
					assertEquals(25, arr.vectorAlongDimension(i, 0).length())
				End If
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSixteenSecondDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSixteenSecondDim(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 5}), Nd4j.create(New Double() {9, 13}), Nd4j.create(New Double() {3, 7}), Nd4j.create(New Double() {11, 15}), Nd4j.create(New Double() {2, 6}), Nd4j.create(New Double() {10, 14}), Nd4j.create(New Double() {4, 8}), Nd4j.create(New Double() {12, 16})}

			Dim i As Integer = 0
			Do While i < baseArr.tensorsAlongDimension(2)
				Dim arr As INDArray = baseArr.tensorAlongDimension(i, 2)
				assertEquals(assertions(i), arr,"Failed at index " & i)
				i += 1
			Loop
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorAlongDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorAlongDimension(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.FLOAT).reshape(ChrW(4), 3, 2)
			Dim assertion As INDArray = Nd4j.create(New Single() {5, 17}, New Long() {2})
			Dim vectorDimensionTest As INDArray = arr.vectorAlongDimension(1, 2)
			assertEquals(assertion, vectorDimensionTest)
			Dim zeroOne As INDArray = arr.vectorAlongDimension(0, 1)
			assertEquals(Nd4j.create(New Single() {1, 5, 9}), zeroOne)

			Dim testColumn2Assertion As INDArray = Nd4j.create(New Single() {13, 17, 21})
			Dim testColumn2 As INDArray = arr.vectorAlongDimension(1, 1)

			assertEquals(testColumn2Assertion, testColumn2)


			Dim testColumn3Assertion As INDArray = Nd4j.create(New Single() {2, 6, 10})
			Dim testColumn3 As INDArray = arr.vectorAlongDimension(2, 1)
			assertEquals(testColumn3Assertion, testColumn3)


			Dim v1 As INDArray = Nd4j.linspace(1, 4, 4, DataType.FLOAT).reshape(New Long() {2, 2})
			Dim testColumnV1 As INDArray = v1.vectorAlongDimension(0, 0)
			Dim testColumnV1Assertion As INDArray = Nd4j.create(New Single() {1, 2})
			assertEquals(testColumnV1Assertion, testColumnV1)

			Dim testRowV1 As INDArray = v1.vectorAlongDimension(1, 0)
			Dim testRowV1Assertion As INDArray = Nd4j.create(New Single() {3, 4})
			assertEquals(testRowV1Assertion, testRowV1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThreeTwoTwo(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThreeTwoTwo(ByVal backend As Nd4jBackend)
			Dim threeTwoTwo As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 4}), Nd4j.create(New Double() {7, 10}), Nd4j.create(New Double() {2, 5}), Nd4j.create(New Double() {8, 11}), Nd4j.create(New Double() {3, 6}), Nd4j.create(New Double() {9, 12})}

			assertEquals(assertions.Length, threeTwoTwo.tensorsAlongDimension(1))
			For i As Integer = 0 To assertions.Length - 1
				Dim test As INDArray = threeTwoTwo.tensorAlongDimension(i, 1)
				assertEquals(assertions(i), test)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoCopy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoCopy(ByVal backend As Nd4jBackend)
			Dim threeTwoTwo As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE)
			Dim arr As INDArray = Shape.newShapeNoCopy(threeTwoTwo, New Long() {3, 2, 2}, True)
			assertArrayEquals(arr.shape(), New Long() {3, 2, 2})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThreeTwoTwoTwo(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThreeTwoTwoTwo(ByVal backend As Nd4jBackend)
			Dim threeTwoTwo As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 7}), Nd4j.create(New Double() {4, 10}), Nd4j.create(New Double() {2, 8}), Nd4j.create(New Double() {5, 11}), Nd4j.create(New Double() {3, 9}), Nd4j.create(New Double() {6, 12})}

			assertEquals(assertions.Length, threeTwoTwo.tensorsAlongDimension(2))
			For i As Integer = 0 To assertions.Length - 1
				Dim test As INDArray = threeTwoTwo.tensorAlongDimension(i, 2)
				assertEquals(assertions(i), test)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewAxis(ByVal backend As Nd4jBackend)
			Dim tensor As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 7},
				New Double() {4, 10}
			}).reshape(ChrW(1), 2, 2)
			Dim tensorGet As INDArray = tensor.get(NDArrayIndex.point(0), NDArrayIndex.newAxis(), all(), all())
			assertEquals(assertion, tensorGet)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSixteenFirstDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSixteenFirstDim(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 3}), Nd4j.create(New Double() {9, 11}), Nd4j.create(New Double() {5, 7}), Nd4j.create(New Double() {13, 15}), Nd4j.create(New Double() {2, 4}), Nd4j.create(New Double() {10, 12}), Nd4j.create(New Double() {6, 8}), Nd4j.create(New Double() {14, 16})}

			Dim i As Integer = 0
			Do While i < baseArr.tensorsAlongDimension(1)
				Dim arr As INDArray = baseArr.tensorAlongDimension(i, 1)
				assertEquals(assertions(i), arr,"Failed at index " & i)
				i += 1
			Loop

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimShuffle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimShuffle(ByVal backend As Nd4jBackend)
			Dim scalarTest As INDArray = Nd4j.scalar(0.0).reshape(ChrW(1), -1)
			Dim broadcast As INDArray = scalarTest.dimShuffle(New Object() {"x"c}, New Long() {0, 1}, New Boolean() {True, True})
			assertTrue(broadcast.rank() = 3)
			Dim rowVector As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(1), -1)
			assertEquals(rowVector, rowVector.dimShuffle(New Object() {0, 1}, New Integer() {0, 1}, New Boolean() {False, False}))
			'add extra dimension to row vector in middle
			Dim rearrangedRowVector As INDArray = rowVector.dimShuffle(New Object() {0, "x"c, 1}, New Integer() {0, 1}, New Boolean() {True, True})
			assertArrayEquals(New Long() {1, 1, 4}, rearrangedRowVector.shape())

			Dim dimshuffed As INDArray = rowVector.dimShuffle(New Object() {"x"c, 0, "x"c, "x"c}, New Long() {0, 1}, New Boolean() {True, True})
			assertArrayEquals(New Long() {1, 1, 1, 1, 4}, dimshuffed.shape())
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEight(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEight(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(2), 2, 2)
			assertEquals(2, baseArr.tensorsAlongDimension(0, 1))
			Dim columnVectorFirst As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {2, 4}
			})
			Dim columnVectorSecond As INDArray = Nd4j.create(New Double()() {
				New Double() {5, 7},
				New Double() {6, 8}
			})
			assertEquals(columnVectorFirst, baseArr.tensorAlongDimension(0, 0, 1))
			assertEquals(columnVectorSecond, baseArr.tensorAlongDimension(1, 0, 1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastShapes()
		Public Overridable Sub testBroadcastShapes()
			'Test cases: in1Shape, in2Shape, shapeOf(op(in1,in2))
			Dim testCases As IList(Of Triple(Of Long(), Long(), Long())) = New List(Of Triple(Of Long(), Long(), Long()))()
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1}, New Long(){1, 4}, New Long(){3, 4}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1}, New Long(){3, 4}, New Long(){3, 4}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 4}, New Long(){1, 4}, New Long(){3, 4}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 4, 1}, New Long(){1, 1, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 4, 1}, New Long(){3, 1, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 5}, New Long(){1, 4, 1}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 5}, New Long(){1, 4, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 5}, New Long(){3, 4, 5}, New Long(){3, 4, 5}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){3, 1, 1, 1}, New Long(){1, 4, 5, 6}, New Long(){3, 4, 5, 6}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){1, 1, 1, 6}, New Long(){3, 4, 5, 6}, New Long(){3, 4, 5, 6}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){1, 4, 5, 1}, New Long(){3, 1, 1, 6}, New Long(){3, 4, 5, 6}))
			testCases.Add(New Triple(Of Long(), Long(), Long())(New Long(){1, 6}, New Long(){3, 4, 5, 1}, New Long(){3, 4, 5, 6}))

			For Each t As Triple(Of Long(), Long(), Long()) In testCases
				Dim x As val = t.getFirst()
				Dim y As val = t.getSecond()
				Dim exp As val = t.getThird()

				Dim act As val = Shape.broadcastOutputShape(x,y)
				assertArrayEquals(exp,act)
			Next t
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace