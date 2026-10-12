Imports System
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
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

Namespace org.nd4j.linalg.shape

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class ShapeTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ShapeTestsC
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			Nd4j.DataType = Me.initialType
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSixteenZeroOne(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSixteenZeroOne(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			assertEquals(4, baseArr.tensorsAlongDimension(0, 1))
			Dim columnVectorFirst As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 5},
				New Double() {9, 13}
			})
			Dim columnVectorSecond As INDArray = Nd4j.create(New Double()() {
				New Double() {2, 6},
				New Double() {10, 14}
			})
			Dim columnVectorThird As INDArray = Nd4j.create(New Double()() {
				New Double() {3, 7},
				New Double() {11, 15}
			})
			Dim columnVectorFourth As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 8},
				New Double() {12, 16}
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSixteenSecondDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSixteenSecondDim(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 3}), Nd4j.create(New Double() {2, 4}), Nd4j.create(New Double() {5, 7}), Nd4j.create(New Double() {6, 8}), Nd4j.create(New Double() {9, 11}), Nd4j.create(New Double() {10, 12}), Nd4j.create(New Double() {13, 15}), Nd4j.create(New Double() {14, 16})}

			Dim i As Integer = 0
			Do While i < baseArr.tensorsAlongDimension(2)
				Dim arr As INDArray = baseArr.tensorAlongDimension(i, 2)
				assertEquals(assertions(i), arr,"Failed at index " & i)
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThreeTwoTwo(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThreeTwoTwo(ByVal backend As Nd4jBackend)
			Dim threeTwoTwo As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 3}), Nd4j.create(New Double() {2, 4}), Nd4j.create(New Double() {5, 7}), Nd4j.create(New Double() {6, 8}), Nd4j.create(New Double() {9, 11}), Nd4j.create(New Double() {10, 12})}

			assertEquals(assertions.Length, threeTwoTwo.tensorsAlongDimension(1))
			For i As Integer = 0 To assertions.Length - 1
				Dim arr As INDArray = threeTwoTwo.tensorAlongDimension(i, 1)
				assertEquals(assertions(i), arr)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThreeTwoTwoTwo(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThreeTwoTwoTwo(ByVal backend As Nd4jBackend)
			Dim threeTwoTwo As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 2}), Nd4j.create(New Double() {3, 4}), Nd4j.create(New Double() {5, 6}), Nd4j.create(New Double() {7, 8}), Nd4j.create(New Double() {9, 10}), Nd4j.create(New Double() {11, 12})}

			assertEquals(assertions.Length, threeTwoTwo.tensorsAlongDimension(2))
			For i As Integer = 0 To assertions.Length - 1
				assertEquals(assertions(i), threeTwoTwo.tensorAlongDimension(i, 2))
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRow(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {3, 4}
			})
			Dim i As Integer = 0
			Do While i < matrix.rows()
				Dim row As INDArray = matrix.getRow(i)
	'            System.out.println(matrix.getRow(i));
				i += 1
			Loop
			matrix.putRow(1, Nd4j.create(New Double() {1, 2}))
			assertEquals(matrix.getRow(0), matrix.getRow(1))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSixteenFirstDim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSixteenFirstDim(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim assertions() As INDArray = {Nd4j.create(New Double() {1, 5}), Nd4j.create(New Double() {2, 6}), Nd4j.create(New Double() {3, 7}), Nd4j.create(New Double() {4, 8}), Nd4j.create(New Double() {9, 13}), Nd4j.create(New Double() {10, 14}), Nd4j.create(New Double() {11, 15}), Nd4j.create(New Double() {12, 16})}

			Dim i As Integer = 0
			Do While i < baseArr.tensorsAlongDimension(1)
				Dim arr As INDArray = baseArr.tensorAlongDimension(i, 1)
				assertEquals(assertions(i), arr,"Failed at index " & i)
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapePermute(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapePermute(ByVal backend As Nd4jBackend)
			Dim arrNoPermute As INDArray = Nd4j.ones(DataType.DOUBLE,5, 3, 4)
			Dim reshaped2dNoPermute As INDArray = arrNoPermute.reshape(ChrW(5 * 3), 4) 'OK
			assertArrayEquals(reshaped2dNoPermute.shape(), New Long() {5 * 3, 4})

			Dim arr As INDArray = Nd4j.ones(DataType.DOUBLE,5, 4, 3)
			Dim permuted As INDArray = arr.permute(0, 2, 1)
			assertArrayEquals(arrNoPermute.shape(), permuted.shape())
			Dim reshaped2D As INDArray = permuted.reshape(ChrW(5 * 3), 4) 'NullPointerException
			assertArrayEquals(reshaped2D.shape(), New Long() {5 * 3, 4})
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEight(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEight(ByVal backend As Nd4jBackend)
			Dim baseArr As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(2), 2, 2)
			assertEquals(2, baseArr.tensorsAlongDimension(0, 1))
			Dim columnVectorFirst As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {5, 7}
			})
			Dim columnVectorSecond As INDArray = Nd4j.create(New Double()() {
				New Double() {2, 4},
				New Double() {6, 8}
			})
			Dim test1 As INDArray = baseArr.tensorAlongDimension(0, 0, 1)
			assertEquals(columnVectorFirst, test1)
			Dim test2 As INDArray = baseArr.tensorAlongDimension(1, 0, 1)
			assertEquals(columnVectorSecond, test2)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOtherReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOtherReshape(ByVal backend As Nd4jBackend)
			Dim nd As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6}, New Long() {2, 3})

			Dim slice As INDArray = nd.slice(1, 0)

			Dim vector As INDArray = slice
	'        for (int i = 0; i < vector.length(); i++) {
	'            System.out.println(vector.getDouble(i));
	'        }
			assertEquals(Nd4j.create(New Double() {4, 5, 6}), vector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorAlongDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorAlongDimension(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 3, 2)
			Dim assertion As INDArray = Nd4j.create(New Double() {3, 4})
			Dim vectorDimensionTest As INDArray = arr.vectorAlongDimension(1, 2)
			assertEquals(assertion, vectorDimensionTest)
			Dim vectorsAlongDimension1 As val = arr.vectorsAlongDimension(1)
			assertEquals(8, vectorsAlongDimension1)
			Dim zeroOne As INDArray = arr.vectorAlongDimension(0, 1)
			assertEquals(zeroOne, Nd4j.create(New Double() {1, 3, 5}))

			Dim testColumn2Assertion As INDArray = Nd4j.create(New Double() {2, 4, 6})
			Dim testColumn2 As INDArray = arr.vectorAlongDimension(1, 1)

			assertEquals(testColumn2Assertion, testColumn2)


			Dim testColumn3Assertion As INDArray = Nd4j.create(New Double() {7, 9, 11})
			Dim testColumn3 As INDArray = arr.vectorAlongDimension(2, 1)
			assertEquals(testColumn3Assertion, testColumn3)


			Dim v1 As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(New Long() {2, 2})
			Dim testColumnV1 As INDArray = v1.vectorAlongDimension(0, 0)
			Dim testColumnV1Assertion As INDArray = Nd4j.create(New Double() {1, 3})
			assertEquals(testColumnV1Assertion, testColumnV1)

			Dim testRowV1 As INDArray = v1.vectorAlongDimension(1, 0)
			Dim testRowV1Assertion As INDArray = Nd4j.create(New Double() {2, 4})
			assertEquals(testRowV1Assertion, testRowV1)

			Dim n As INDArray = Nd4j.create(Nd4j.linspace(1, 8, 8, DataType.DOUBLE).data(), New Long() {2, 2, 2})
			Dim vectorOne As INDArray = n.vectorAlongDimension(1, 2)
			Dim assertionVectorOne As INDArray = Nd4j.create(New Double() {3, 4})
			assertEquals(assertionVectorOne, vectorOne)


			Dim oneThroughSixteen As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)

			assertEquals(8, oneThroughSixteen.vectorsAlongDimension(1))
			assertEquals(Nd4j.create(New Double() {1, 5}), oneThroughSixteen.vectorAlongDimension(0, 1))
			assertEquals(Nd4j.create(New Double() {2, 6}), oneThroughSixteen.vectorAlongDimension(1, 1))
			assertEquals(Nd4j.create(New Double() {3, 7}), oneThroughSixteen.vectorAlongDimension(2, 1))
			assertEquals(Nd4j.create(New Double() {4, 8}), oneThroughSixteen.vectorAlongDimension(3, 1))
			assertEquals(Nd4j.create(New Double() {9, 13}), oneThroughSixteen.vectorAlongDimension(4, 1))
			assertEquals(Nd4j.create(New Double() {10, 14}), oneThroughSixteen.vectorAlongDimension(5, 1))
			assertEquals(Nd4j.create(New Double() {11, 15}), oneThroughSixteen.vectorAlongDimension(6, 1))
			assertEquals(Nd4j.create(New Double() {12, 16}), oneThroughSixteen.vectorAlongDimension(7, 1))


			Dim fourdTest As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim assertionsArr()() As Double = {
				New Double() {1, 3},
				New Double() {2, 4},
				New Double() {5, 7},
				New Double() {6, 8},
				New Double() {9, 11},
				New Double() {10, 12},
				New Double() {13, 15},
				New Double() {14, 16}
			}

			assertEquals(assertionsArr.Length, fourdTest.vectorsAlongDimension(2))

			For i As Integer = 0 To assertionsArr.Length - 1
				Dim test As INDArray = fourdTest.vectorAlongDimension(i, 2)
				Dim assertionEntry As INDArray = Nd4j.create(assertionsArr(i))
				assertEquals(assertionEntry, test)
			Next i


		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnSum(ByVal backend As Nd4jBackend)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 600, 600, DataType.FLOAT).reshape(ChrW(150), 4)
			Dim columnVar As INDArray = twoByThree.sum(0)
			Dim assertion As INDArray = Nd4j.create(New Single() {44850.0f, 45000.0f, 45150.0f, 45300.0f})
			assertEquals(assertion, columnVar,getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowMean(ByVal backend As Nd4jBackend)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim rowMean As INDArray = twoByThree.mean(1)
			Dim assertion As INDArray = Nd4j.create(New Double() {1.5, 3.5})
			assertEquals(assertion, rowMean,getFailureMessage(backend))


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRowStd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRowStd(ByVal backend As Nd4jBackend)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim rowStd As INDArray = twoByThree.std(1)
			Dim assertion As INDArray = Nd4j.create(New Double() {0.7071067811865476f, 0.7071067811865476f})
			assertEquals(assertion, rowStd,getFailureMessage(backend))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnSumDouble(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnSumDouble(ByVal backend As Nd4jBackend)
			Dim initialType As DataType = Nd4j.dataType()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 600, 600, DataType.DOUBLE).reshape(ChrW(150), 4)
			Dim columnVar As INDArray = twoByThree.sum(0)
			Dim assertion As INDArray = Nd4j.create(New Double() {44850.0f, 45000.0f, 45150.0f, 45300.0f})
			assertEquals(assertion, columnVar,getFailureMessage(backend))
			DataTypeUtil.setDTypeForContext(initialType)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnVariance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnVariance(ByVal backend As Nd4jBackend)
			Dim twoByThree As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim columnVar As INDArray = twoByThree.var(True, 0)
			Dim assertion As INDArray = Nd4j.create(New Double() {2, 2})
			assertEquals(assertion, columnVar)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCumSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCumSum(ByVal backend As Nd4jBackend)
			Dim n As INDArray = Nd4j.create(New Double() {1, 2, 3, 4}, New Long() {1, 4})
			Dim cumSumAnswer As INDArray = Nd4j.create(New Double() {1, 3, 6, 10}, New Long() {1, 4})
			Dim cumSumTest As INDArray = n.cumsum(0)
			assertEquals(cumSumAnswer, cumSumTest,getFailureMessage(backend))

			Dim n2 As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 3, 2)

			Dim axis0assertion As INDArray = Nd4j.create(New Double() {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 8.0, 10.0, 12.0, 14.0, 16.0, 18.0, 21.0, 24.0, 27.0, 30.0, 33.0, 36.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0}, n2.shape())
			Dim axis0Test As INDArray = n2.cumsum(0)
			assertEquals(axis0assertion, axis0Test,getFailureMessage(backend))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumRow(ByVal backend As Nd4jBackend)
			Dim rowVector10 As INDArray = Nd4j.ones(DataType.DOUBLE,1,10)
			Dim sum1 As INDArray = rowVector10.sum(1)
			assertArrayEquals(New Long() {1}, sum1.shape())
			assertTrue(sum1.getDouble(0) = 10)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumColumn(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumColumn(ByVal backend As Nd4jBackend)
			Dim colVector10 As INDArray = Nd4j.ones(10, 1)
			Dim sum0 As INDArray = colVector10.sum(0)
			assertArrayEquals(New Long() {1}, sum0.shape())
			assertTrue(sum0.getDouble(0) = 10)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum2d(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(10, 10)
			Dim sum0 As INDArray = arr.sum(0)
			assertArrayEquals(New Long() {10}, sum0.shape())

			Dim sum1 As INDArray = arr.sum(1)
			assertArrayEquals(New Long() {10}, sum1.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum2dv2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum2dv2(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(10, 10)
			Dim sumBoth As INDArray = arr.sum(0, 1)
			assertArrayEquals(New Long(){}, sumBoth.shape())
			assertTrue(sumBoth.getDouble(0) = 100)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermuteReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermuteReshape(ByVal backend As Nd4jBackend)
			Dim arrTest As INDArray = Nd4j.arange(60).reshape("c"c, 3, 4, 5)
			Dim permute As INDArray = arrTest.permute(2, 1, 0)
			assertArrayEquals(New Long() {5, 4, 3}, permute.shape())
			assertArrayEquals(New Long() {1, 5, 20}, permute.stride())
			Dim reshapedPermute As INDArray = permute.reshape(ChrW(-1), 12)
			assertArrayEquals(New Long() {5, 12}, reshapedPermute.shape())
			assertArrayEquals(New Long() {12, 1}, reshapedPermute.stride())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRavel(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRavel(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4,DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim asseriton As INDArray = Nd4j.linspace(1, 4, 4,DataType.DOUBLE)
			Dim raveled As INDArray = linspace.ravel()
			assertEquals(asseriton, raveled)

			Dim tensorLinSpace As INDArray = Nd4j.linspace(1, 16, 16,DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim linspaced As INDArray = Nd4j.linspace(1, 16, 16,DataType.DOUBLE)
			Dim tensorLinspaceRaveled As INDArray = tensorLinSpace.ravel()
			assertEquals(linspaced, tensorLinspaceRaveled)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutScalar(ByVal backend As Nd4jBackend)
			'Check that the various putScalar methods have the same result...
			Dim shapes As val = New Integer()() {
				New Integer() {3, 4},
				New Integer() {1, 4},
				New Integer() {3, 1},
				New Integer() {3, 4, 5},
				New Integer() {1, 4, 5},
				New Integer() {3, 1, 5},
				New Integer() {3, 4, 1},
				New Integer() {1, 1, 5},
				New Integer() {3, 4, 5, 6},
				New Integer() {1, 4, 5, 6},
				New Integer() {3, 1, 5, 6},
				New Integer() {3, 4, 1, 6},
				New Integer() {3, 4, 5, 1},
				New Integer() {1, 1, 5, 6},
				New Integer() {3, 1, 1, 6},
				New Integer() {3, 1, 1, 1}
			}

			For Each shape As Integer() In shapes
				Dim rank As Integer = shape.Length
				Dim iter As New NdIndexIterator(shape)
				Dim firstC As INDArray = Nd4j.create(shape, "c"c)
				Dim firstF As INDArray = Nd4j.create(shape, "f"c)
				Dim secondC As INDArray = Nd4j.create(shape, "c"c)
				Dim secondF As INDArray = Nd4j.create(shape, "f"c)

				Dim i As Integer = 0
				Do While iter.MoveNext()
					Dim currIdx As val = iter.Current
					firstC.putScalar(currIdx, i)
					firstF.putScalar(currIdx, i)

					Select Case rank
						Case 2
							secondC.putScalar(currIdx(0), currIdx(1), i)
							secondF.putScalar(currIdx(0), currIdx(1), i)
						Case 3
							secondC.putScalar(currIdx(0), currIdx(1), currIdx(2), i)
							secondF.putScalar(currIdx(0), currIdx(1), currIdx(2), i)
						Case 4
							secondC.putScalar(currIdx(0), currIdx(1), currIdx(2), currIdx(3), i)
							secondF.putScalar(currIdx(0), currIdx(1), currIdx(2), currIdx(3), i)
						Case Else
							Throw New Exception()
					End Select
					i += 1
				Loop
				assertEquals(firstC, firstF)
				assertEquals(firstC, secondC)
				assertEquals(firstC, secondF)
			Next shape
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeToTrueScalar_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeToTrueScalar_1(ByVal backend As Nd4jBackend)
			Dim orig As val = Nd4j.create(New Single(){1.0f}, New Integer(){1, 1})
			Dim exp As val = Nd4j.scalar(1.0f)

			assertArrayEquals(New Long(){1, 1}, orig.shape())

			Dim reshaped As val = orig.reshape()

			assertArrayEquals(exp.shapeInfoDataBuffer().asLong(), reshaped.shapeInfoDataBuffer().asLong())
			assertEquals(exp, reshaped)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeToTrueScalar_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeToTrueScalar_2(ByVal backend As Nd4jBackend)
			Dim orig As val = Nd4j.create(New Single(){1.0f}, New Integer(){1})
			Dim exp As val = Nd4j.scalar(1.0f)

			assertArrayEquals(New Long(){1}, orig.shape())

			Dim reshaped As val = orig.reshape()

			assertArrayEquals(exp.shapeInfoDataBuffer().asLong(), reshaped.shapeInfoDataBuffer().asLong())
			assertEquals(exp, reshaped)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeToTrueScalar_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeToTrueScalar_3(ByVal backend As Nd4jBackend)
			Dim orig As val = Nd4j.create(New Single(){1.0f}, New Integer(){1, 1})
			Dim exp As val = Nd4j.createFromArray(New Single(){1.0f})

			assertArrayEquals(New Long(){1, 1}, orig.shape())

			Dim reshaped As val = orig.reshape(1)

			assertArrayEquals(exp.shapeInfoDataBuffer().asLong(), reshaped.shapeInfoDataBuffer().asLong())
			assertEquals(exp, reshaped)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshapeToTrueScalar_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshapeToTrueScalar_4(ByVal backend As Nd4jBackend)
			Dim orig As val = Nd4j.create(New Single(){1.0f}, New Integer(){1, 1})
			Dim exp As val = Nd4j.scalar(1.0f)

			assertArrayEquals(New Long(){1, 1}, orig.shape())

			Dim reshaped As val = orig.reshape(New Integer(){})

			assertArrayEquals(exp.shapeInfoDataBuffer().asLong(), reshaped.shapeInfoDataBuffer().asLong())
			assertEquals(exp, reshaped)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testViewAfterReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testViewAfterReshape(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.rand(3,4)
			Dim x2 As val = x.ravel()
			Dim x3 As val = x.reshape(6,2)

			assertFalse(x.isView())
			assertTrue(x2.isView())
			assertTrue(x3.isView())
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace