Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.linalg.indexing
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.junit.jupiter.api.Assertions
Imports org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.nd4j.linalg.api.indexing


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class IndexingTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class IndexingTestsC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNegativeBounds(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNegativeBounds(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1,10,10, DataType.DOUBLE).reshape(ChrW(2), 5)
			Dim interval As INDArrayIndex = interval(0,1,-2,arr.size(1))
			Dim get As INDArray = arr.get(all(),interval)
			Dim assertion As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2, 3},
				New Double() {6, 7, 8}
			})
			assertEquals(assertion,get)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewAxis(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim get As INDArray = arr.get(all(), all(), newAxis(), newAxis(), all())
			Dim shapeAssertion() As Long = {3, 2, 1, 1, 2}
			assertArrayEquals(shapeAssertion, get.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void broadcastBug(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub broadcastBug(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.create(New Double() {1.0, 2.0, 3.0, 4.0}, New Integer() {2, 2})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray col = a.get(NDArrayIndex.all(), NDArrayIndex.point(0));
			Dim col As INDArray = a.get(all(), point(0))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray aBad = col.broadcast(2, 2);
			Dim aBad As INDArray = col.broadcast(2, 2)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray aGood = col.dup().broadcast(2, 2);
			Dim aGood As INDArray = col.dup().broadcast(2, 2)
			assertTrue(Transforms.abs(aGood.sub(aBad).div(aGood)).maxNumber().doubleValue() < 0.01)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIntervalsIn3D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIntervalsIn3D(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.arange(8).reshape(ChrW(2), 2, 2).castTo(DataType.DOUBLE)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 5},
				New Double() {6, 7}
			}).reshape(ChrW(1), 2, 2)
			Dim rest As INDArray = arr.get(interval(1, 2), interval(0, 2), interval(0, 2))
			assertEquals(assertion, rest)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSmallInterval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSmallInterval(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.arange(8).reshape(ChrW(2), 2, 2).castTo(DataType.DOUBLE)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 5},
				New Double() {6, 7}
			}).reshape(ChrW(1), 2, 2)
			Dim rest As INDArray = arr.get(interval(1, 2), all(), all())
			assertEquals(assertion, rest)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllWithNewAxisAndInterval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllWithNewAxisAndInterval(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 2, 3)
			Dim assertion2 As INDArray = Nd4j.create(New Double()() {
				New Double() {7, 8, 9}
			}).reshape(ChrW(1), 1, 3)

			Dim get2 As INDArray = arr.get(point(1), newAxis(), interval(0, 1))
			assertEquals(assertion2, get2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllWithNewAxisInMiddle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllWithNewAxisInMiddle(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 2, 3)
			Dim assertion2 As INDArray = Nd4j.create(New Double()() {
				New Double() {7, 8, 9},
				New Double() {10, 11, 12}
			}).reshape(ChrW(1), 2, 3)

			Dim get2 As INDArray = arr.get(point(1), newAxis(), all(), all())
			assertEquals(assertion2, get2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllWithNewAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllWithNewAxis(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 2, 3)
			Dim get As INDArray = arr.get(newAxis(), all(), point(1))
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 5, 6},
				New Double() {10, 11, 12},
				New Double() {16, 17, 18},
				New Double() {22, 23, 24}
			}).reshape(ChrW(1), 4, 3)
			assertEquals(assertion, get)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIndexingWithMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIndexingWithMmul(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)
			Dim b As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).reshape(ChrW(1), -1)
	'        System.out.println(b);
			Dim view As INDArray = a.get(all(), interval(0, 1))
			Dim c As INDArray = view.mmul(b)
			Dim assertion As INDArray = a.get(all(), interval(0, 1)).dup().mmul(b)
			assertEquals(assertion, c)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPointPointInterval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPointPointInterval(ByVal backend As Nd4jBackend)
			Dim wholeArr As INDArray = Nd4j.linspace(1, 36, 36, DataType.DOUBLE).reshape(ChrW(4), 3, 3)
			Dim get As INDArray = wholeArr.get(point(0), interval(1, 3), interval(1, 3))
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {5, 6},
				New Double() {8, 9}
			})

			assertEquals(assertion, get)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIntervalLowerBound(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIntervalLowerBound(ByVal backend As Nd4jBackend)
			Dim wholeArr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 2, 3)
			Dim subarray As INDArray = wholeArr.get(interval(1, 3), point(0), indices(0, 2))
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {7, 9},
				New Double() {13, 15}
			})

			assertEquals(assertion, subarray)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetPointRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetPointRowVector(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 1000, 1000, DataType.DOUBLE).reshape(ChrW(1), -1)

			Dim arr2 As INDArray = arr.get(point(0), interval(0, 100))

			assertEquals(100, arr2.length()) 'Returning: length 0
			assertEquals(Nd4j.linspace(1, 100, 100, DataType.DOUBLE), arr2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpecifiedIndexVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpecifiedIndexVector(ByVal backend As Nd4jBackend)
			Dim rootMatrix As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(4), 4)
			Dim threeD As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim get As INDArray = rootMatrix.get(all(), New SpecifiedIndex(0, 2))
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {5, 7},
				New Double() {9, 11},
				New Double() {13, 15}
			})

			assertEquals(assertion, get)

			Dim assertion2 As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3, 4},
				New Double() {5, 7, 8},
				New Double() {9, 11, 12},
				New Double() {13, 15, 16}
			})
			Dim get2 As INDArray = rootMatrix.get(all(), New SpecifiedIndex(0, 2, 3))

			assertEquals(assertion2, get2)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutRowIndexing(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutRowIndexing(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.ones(1, 10)
			Dim row As INDArray = Nd4j.create(1, 10)

			arr.putRow(0, row) 'OK
			arr.put(New INDArrayIndex() {point(0), all()}, row) 'Exception
			assertEquals(arr, row)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorIndexing2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorIndexing2(ByVal backend As Nd4jBackend)
			Dim wholeVector As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).get(interval(1, 2, 3, True))
			Dim assertion As INDArray = Nd4j.create(New Double() {2, 4})
			assertEquals(assertion, wholeVector)
			Dim wholeVectorTwo As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).get(interval(1, 2, 4, True))
			assertEquals(assertion, wholeVectorTwo)
			Dim wholeVectorThree As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).get(interval(1, 2, 4, False))
			assertEquals(assertion, wholeVectorThree)
			Dim threeFiveAssertion As INDArray = Nd4j.create(New Double() {3, 5})
			Dim threeFive As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).get(interval(2, 2, 4, True))
			assertEquals(threeFiveAssertion, threeFive)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOffsetsC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOffsetsC(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			assertEquals(3, offset(arr, 1, 1))
			assertEquals(3, offset(arr, point(1), point(1)))

			Dim arr2 As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(3), 2)
			assertEquals(3, offset(arr2, 1, 1))
			assertEquals(3, offset(arr2, point(1), point(1)))
			assertEquals(6, offset(arr2, 2, 2))
			assertEquals(6, offset(arr2, point(2), point(2)))



		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIndexFor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIndexFor(ByVal backend As Nd4jBackend)
			Dim shape() As Long = {1, 2}
			Dim indexes() As INDArrayIndex = indexesFor(shape)
			For i As Integer = 0 To indexes.Length - 1
				assertEquals(shape(i), indexes(i).offset())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetScalar(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)
			Dim d As INDArray = arr.get(point(1))
			assertTrue(d.Scalar)
			assertEquals(2.0, d.getDouble(0), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorIndexing(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorIndexing(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 10, 10, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim assertion As INDArray = Nd4j.create(New Double() {2, 3, 4, 5})
			Dim viewTest As INDArray = arr.get(point(0), interval(1, 5))
			assertEquals(assertion, viewTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNegativeIndices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNegativeIndices(ByVal backend As Nd4jBackend)
			Dim test As INDArray = Nd4j.create(10, 10, 10)
			test.putScalar(New Integer() {0, 0, -1}, 1.0)
			assertEquals(1.0, test.getScalar(0, 0, -1).sumNumber())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetIndices2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetIndices2d(ByVal backend As Nd4jBackend)
			Dim twoByTwo As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(3), 2)
			Dim firstRow As INDArray = twoByTwo.getRow(0)
			Dim secondRow As INDArray = twoByTwo.getRow(1)
			Dim firstAndSecondRow As INDArray = twoByTwo.getRows(1, 2)
			Dim firstRowViaIndexing As INDArray = twoByTwo.get(interval(0, 1), all())
			assertEquals(firstRow.reshape(ChrW(1), 2), firstRowViaIndexing)
			Dim secondRowViaIndexing As INDArray = twoByTwo.get(point(1), all())
			assertEquals(secondRow, secondRowViaIndexing)

			Dim firstAndSecondRowTest As INDArray = twoByTwo.get(interval(1, 3), all())
			assertEquals(firstAndSecondRow, firstAndSecondRowTest)

			Dim individualElement As INDArray = twoByTwo.get(interval(1, 2), interval(1, 2))
			assertEquals(Nd4j.create(New Double() {4}, New Integer(){1, 1}), individualElement)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRow(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.linspace(0, 14, 15, DataType.DOUBLE).reshape(ChrW(3), 5)
			Dim toGet() As Integer = {0, 1}
			Dim [out] As INDArray = [in].getRows(toGet)
			assertEquals([in].getRow(0), [out].getRow(0))
			assertEquals([in].getRow(1), [out].getRow(1))

			Dim toGet2() As Integer = {0, 1, 2, 0, 1, 2}
			Dim out2 As INDArray = [in].getRows(toGet2)
			For i As Integer = 0 To toGet2.Length - 1
				assertEquals([in].getRow(toGet2(i)), out2.getRow(i))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRowEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRowEdgeCase(ByVal backend As Nd4jBackend)
			Dim rowVec As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim get As INDArray = rowVec.getRow(0) 'Returning shape [1,1]

			assertArrayEquals(New Long() {1, 5}, get.shape())
			assertEquals(rowVec, get)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetColumnEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetColumnEdgeCase(ByVal backend As Nd4jBackend)
			Dim colVec As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).reshape(ChrW(1), -1).transpose()
			Dim get As INDArray = colVec.getColumn(0) 'Returning shape [1,1]

			assertArrayEquals(New Long() {5, 1}, get.shape())
			assertEquals(colVec, get)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatColumns(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatColumns(ByVal backend As Nd4jBackend)
			Dim input1 As INDArray = Nd4j.zeros(2, 1).castTo(DataType.DOUBLE)
			Dim input2 As INDArray = Nd4j.ones(2, 1).castTo(DataType.DOUBLE)
			Dim concat As INDArray = Nd4j.concat(1, input1, input2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1},
				New Double() {0, 1}
			})
			assertEquals(assertion, concat)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetIndicesVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetIndicesVector(ByVal backend As Nd4jBackend)
			Dim line As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim test As INDArray = Nd4j.create(New Double() {2, 3})
			Dim result As INDArray = line.get(point(0), interval(1, 3))
			assertEquals(test, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArangeMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArangeMul(ByVal backend As Nd4jBackend)
			Dim arange As INDArray = Nd4j.arange(1, 17).reshape(ChrW(4), 4).castTo(DataType.DOUBLE)
			Dim index As INDArrayIndex = interval(0, 2)
			Dim get As INDArray = arange.get(index, index)
			Dim ones As INDArray = Nd4j.ones(DataType.DOUBLE, 2, 2).mul(0.25)
			Dim mul As INDArray = get.mul(ones)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {0.25, 0.5},
				New Double() {1.25, 1.5}
			})
			assertEquals(assertion, mul)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIndexingThorough()
		Public Overridable Sub testIndexingThorough()
			Dim fullShape() As Long = {3, 4, 5, 6, 7}

			'Note: 888,880 total test cases here - randomly run a fraction of the tests to minimize runtime
			' whilst still maintaining good coverage
			Dim r As New Random(12345)
			Dim fractionRun As Double = 0.01

			Dim totalTestCaseCount As Long = 0
			For rank As Integer = 1 To 5
				For Each order As Char In New Char(){"c"c, "f"c}
					Dim n(rank - 1) As Integer
					Dim inShape(rank - 1) As Long
					Dim prod As Long = 1
					For i As Integer = 0 To rank - 1
						n(i) = 10
						inShape(i) = fullShape(i)
						prod *= fullShape(i)
					Next i

					For Each newAxisTestCase As Integer In New Integer(){0, 1, 2, 3} '0 = none, 1=at start, 2=at end, 3=between
						Dim outRank As Integer = rank
						Select Case newAxisTestCase
							Case 1, 2 'At start
								outRank += 1
							Case 3 'Between
								outRank += rank - 1
						End Select

						Dim indexes(outRank - 1) As INDArrayIndex
						Dim iter As New NdIndexIterator(n) 'This is used as a simple counter
						Do While iter.MoveNext()
							Dim [next]() As Long = iter.Current

							If r.nextFloat() > fractionRun Then
								'Randomly skip fraction of tests to minimize runtime
								Continue Do
							End If

							Dim pos As Integer = 0

							If newAxisTestCase = 1 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.newAxis();
								indexes(pos) = newAxis()
									pos += 1
							End If

							For i As Integer = 0 To [next].Length - 1
								Select Case CInt([next](i))
									Case 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.point(0);
										indexes(pos) = point(0)
											pos += 1
									Case 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.point(fullShape[i] - 1);
										indexes(pos) = point(fullShape(i) - 1)
											pos += 1
									Case 2
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.point(fullShape[i] / 2);
										indexes(pos) = point(fullShape(i) \ 2)
											pos += 1
									Case 3
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.interval(0, fullShape[i]);
										indexes(pos) = interval(0, fullShape(i))
											pos += 1
									Case 4
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.interval(0, fullShape[i] - 1, true);
										indexes(pos) = interval(0, fullShape(i) - 1, True)
											pos += 1
									Case 5
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.interval(1, 2, fullShape[i]);
										indexes(pos) = interval(1, 2, fullShape(i))
											pos += 1
									Case 6
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.interval(1, 2, fullShape[i] - 1, true);
										indexes(pos) = interval(1, 2, fullShape(i) - 1, True)
											pos += 1
									Case 7
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.all();
										indexes(pos) = all()
											pos += 1
									Case 8
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.indices(0);
										indexes(pos) = indices(0)
											pos += 1
									Case 9
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.indices(2,1);
										indexes(pos) = indices(2,1)
											pos += 1
									Case Else
										Throw New Exception()
								End Select
								If newAxisTestCase = 3 AndAlso i < [next].Length - 1 Then 'Between
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.newAxis();
									indexes(pos) = newAxis()
										pos += 1
								End If
							Next i

							If newAxisTestCase = 2 Then 'At end
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: indexes[pos++] = NDArrayIndex.newAxis();
								indexes(pos) = newAxis()
									pos += 1
							End If

							Dim arr As INDArray = Nd4j.linspace(DataType.FLOAT, 1, prod, prod).reshape("c"c, inShape).dup(order)
							Dim [sub] As INDArray = arr.get(indexes)

							Dim msg As String = "Test case: rank = " & rank & ", order = " & order & ", inShape = " & Arrays.toString(inShape) & ", indexes = " & Arrays.toString(indexes) & ", newAxisTest=" & newAxisTestCase

							Dim expShape() As Long = getShape(arr, indexes)
							Dim subShape() As Long = [sub].shape()
							assertArrayEquals(expShape, subShape,msg)

							msg = "Test case: rank = " & rank & ", order = " & order & ", inShape = " & Arrays.toString(inShape) & ", outShape = " & Arrays.toString(expShape) & ", indexes = " & Arrays.toString(indexes) & ", newAxisTest=" & newAxisTestCase

							Dim posIter As New NdIndexIterator(expShape)
							Do While posIter.MoveNext()
								Dim outIdxs() As Long = posIter.Current
								Dim act As Double = [sub].getDouble(outIdxs)
								Dim exp As Double = getDouble(indexes, arr, outIdxs)

								assertEquals(exp, act, 1e-6,msg)
							Loop
							totalTestCaseCount += 1
						Loop
					Next newAxisTestCase
				Next order
			Next rank

			assertTrue(totalTestCaseCount > 5000,totalTestCaseCount.ToString())
		End Sub

		Private Shared Function getShape(ByVal [in] As INDArray, ByVal idxs() As INDArrayIndex) As Long()
			Dim countPoint As Integer = 0
			Dim countNewAxis As Integer = 0
			For Each i As INDArrayIndex In idxs
				If TypeOf i Is PointIndex Then
					countPoint += 1
				End If
				If TypeOf i Is NewAxis Then
					countNewAxis += 1
				End If
			Next i

			Preconditions.checkState([in].rank() = idxs.Length - countNewAxis)

			Dim [out](([in].rank() - countPoint + countNewAxis) - 1) As Long
			Dim outAxisCount As Integer = 0
			Dim inAxisCount As Integer = 0
			For i As Integer = 0 To idxs.Length - 1
				If TypeOf idxs(i) Is PointIndex Then
					'Point index doesn't appear in output
					inAxisCount += 1
					Continue For
				End If
				If TypeOf idxs(i) Is NDArrayIndexAll Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[outAxisCount++] = in.size(inAxisCount++);
					[out](outAxisCount) = [in].size(inAxisCount)
						inAxisCount += 1
						outAxisCount += 1
				ElseIf TypeOf idxs(i) Is IntervalIndex Then
					Dim ii As IntervalIndex = DirectCast(idxs(i), IntervalIndex)
					Dim begin As Long = ii.offset() 'Inclusive
					Dim [end] As Long = ii.end() 'Inclusive
					If Not ii.isInclusive() Then
						[end] -= 1
					End If
					Dim stride As Long = ii.stride()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[outAxisCount++] = (end-begin)/stride + 1;
					[out](outAxisCount) = ([end]-begin)\stride + 1
						outAxisCount += 1
					inAxisCount += 1
				ElseIf TypeOf idxs(i) Is NewAxis Then
					'Don't increment inAxisCount as newAxis doesn't correspend to input axis
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[outAxisCount++] = 1;
					[out](outAxisCount) = 1
						outAxisCount += 1
				ElseIf TypeOf idxs(i) Is SpecifiedIndex Then
					Dim si As SpecifiedIndex = DirectCast(idxs(i), SpecifiedIndex)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[outAxisCount++] = si.getIndexes().length;
					[out](outAxisCount) = si.getIndexes().length
						outAxisCount += 1
					inAxisCount += 1
				Else
					Throw New Exception()
				End If
			Next i
			Return [out]
		End Function

		Public Shared Function getDouble(ByVal idxs() As INDArrayIndex, ByVal source As INDArray, ByVal viewIdx() As Long) As Double
			Dim originalIdxs(source.rank() - 1) As Long
			Dim origIdxC As Integer = 0
			Dim viewC As Integer = 0
			For i As Integer = 0 To idxs.Length - 1
				If TypeOf idxs(i) Is PointIndex Then
					Dim p As PointIndex = DirectCast(idxs(i), PointIndex)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: originalIdxs[origIdxC++] = p.offset();
					originalIdxs(origIdxC) = p.offset()
						origIdxC += 1
					'View counter not increased: doesn't appear in output
				ElseIf TypeOf idxs(i) Is NDArrayIndexAll Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: originalIdxs[origIdxC++] = viewIdx[viewC++];
					originalIdxs(origIdxC) = viewIdx(viewC)
						viewC += 1
						origIdxC += 1
				ElseIf TypeOf idxs(i) Is IntervalIndex Then
					Dim ii As IntervalIndex = DirectCast(idxs(i), IntervalIndex)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: originalIdxs[origIdxC++] = ii.offset() + viewIdx[viewC++] * ii.stride();
					originalIdxs(origIdxC) = ii.offset() + viewIdx(viewC) * ii.stride()
						viewC += 1
						origIdxC += 1
				ElseIf TypeOf idxs(i) Is NewAxis Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(viewIdx[viewC++] == 0);
					Preconditions.checkState(viewIdx(viewC) = 0)
						viewC += 1
					Continue For 'Skip new axis, size 1 dimension doesn't appear in source
				ElseIf TypeOf idxs(i) Is SpecifiedIndex Then
					Dim si As SpecifiedIndex = DirectCast(idxs(i), SpecifiedIndex)
					Dim s() As Long = si.getIndexes()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: originalIdxs[origIdxC++] = s[(int)viewIdx[viewC++]];
					originalIdxs(origIdxC) = s(CInt(viewIdx(viewC)))
						viewC += 1
						origIdxC += 1
				Else
					Throw New Exception()
				End If
			Next i

			Dim d As Double = source.getDouble(originalIdxs)
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void debugging()
		Public Overridable Sub debugging()
			Dim inShape() As Long = {3, 4}
			Dim indexes() As INDArrayIndex = {point(0), interval(1, 2, 4)}

			Dim prod As Long = ArrayUtil.prod(inShape)
			Dim order As Char = "c"c
			Dim arr As INDArray = Nd4j.linspace(DataType.FLOAT, 1, prod, prod).reshape("c"c, inShape).dup(order)
			Dim [sub] As INDArray = arr.get(indexes)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace