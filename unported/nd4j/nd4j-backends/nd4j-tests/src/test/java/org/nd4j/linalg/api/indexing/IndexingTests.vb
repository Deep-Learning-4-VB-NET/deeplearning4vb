Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.nd4j.linalg.api.indexing

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class IndexingTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class IndexingTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testINDArrayIndexingEqualToRank(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testINDArrayIndexingEqualToRank(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1,6,6, DataType.DOUBLE).reshape("c"c,3,2).castTo(DataType.DOUBLE)
			Dim indexes As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 1, 2},
				New Double() {0, 1, 0}
			})

			Dim assertion As INDArray = Nd4j.create(New Double(){1, 4, 5})
			Dim getTest As INDArray = x.get(indexes)
			assertEquals(assertion,getTest)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testINDArrayIndexingLessThanRankSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testINDArrayIndexingLessThanRankSimple(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1,6,6, DataType.DOUBLE).reshape("c"c,3,2).castTo(DataType.DOUBLE)
			Dim indexes As INDArray = Nd4j.create(New Double()(){
				New Double() {0}
			})

			Dim assertion As INDArray = Nd4j.create(New Double(){1, 2})
			Dim getTest As INDArray = x.get(indexes)
			assertEquals(assertion, getTest)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testINDArrayIndexingLessThanRankFourDimension(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testINDArrayIndexingLessThanRankFourDimension(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1,16,16, DataType.DOUBLE).reshape("c"c,2,2,2,2).castTo(DataType.DOUBLE)
			Dim indexes As INDArray = Nd4j.create(New Double()(){
				New Double() {0},
				New Double() {1}
			})

			Dim assertion As INDArray = Nd4j.create(New Double(){5, 6, 7, 8}).reshape("c"c,1,2,2)
			Dim getTest As INDArray = x.get(indexes)
			assertEquals(assertion,getTest)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPutSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPutSimple(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(1,16,16, DataType.DOUBLE).reshape("c"c,2,2,2,2)
			Dim indexes As INDArray = Nd4j.create(New Double()(){
				New Double() {0},
				New Double() {1}
			})

			x.put(indexes,Nd4j.create(New Double() {5, 5}))
			Dim vals As INDArray = Nd4j.valueArrayOf(New Long() {2, 2, 2, 2},5, DataType.DOUBLE)
			assertEquals(vals,x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetScalar(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetScalar(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 5, 5, DataType.DOUBLE)
			Dim d As INDArray = arr.get(NDArrayIndex.point(1))
			assertTrue(d.Scalar)
			assertEquals(2.0, d.getDouble(0), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewAxis(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.rand(New Integer() {4, 2, 3})
			Dim view As INDArray = arr.get(NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.point(1))
	'        System.out.println(view);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorIndexing(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorIndexing(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.linspace(0, 10, 11, DataType.DOUBLE).reshape(ChrW(1), 11).castTo(DataType.DOUBLE)
			Dim index() As Integer = {5, 8, 9}
			Dim columnsTest As INDArray = x.getColumns(index)
			assertEquals(Nd4j.create(New Double() {5, 8, 9}, New Integer(){1, 3}), columnsTest)
			Dim index2() As Integer = {2, 2, 4} 'retrieve the same columns twice
			Dim columnsTest2 As INDArray = x.getColumns(index2)
			assertEquals(Nd4j.create(New Double() {2, 2, 4}, New Integer(){1, 3}), columnsTest2)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRowsColumnsMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRowsColumnsMatrix(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 6)
			Dim firstAndSecondColumnsAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 5},
				New Double() {2, 6},
				New Double() {3, 7},
				New Double() {4, 8}
			})

	'        System.out.println(arr);
			Dim firstAndSecondColumns As INDArray = arr.getColumns(0, 1)
			assertEquals(firstAndSecondColumnsAssertion, firstAndSecondColumns)

			Dim firstAndSecondRows As INDArray = Nd4j.create(New Double()() {
				New Double() {1.00, 5.00, 9.00, 13.00, 17.00, 21.00},
				New Double() {1.00, 5.00, 9.00, 13.00, 17.00, 21.00},
				New Double() {2.00, 6.00, 10.00, 14.00, 18.00, 22.00}
			})

			Dim rows As INDArray = arr.getRows(0, 0, 1)
			assertEquals(firstAndSecondRows, rows)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlicing(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSlicing(ByVal backend As Nd4jBackend)
			Dim arange As INDArray = Nd4j.arange(1, 17).reshape(ChrW(4), 4).castTo(DataType.DOUBLE)
			Dim slice1Assert As INDArray = Nd4j.create(New Double() {2, 6, 10, 14})
			Dim slice1Test As INDArray = arange.slice(1)
			assertEquals(slice1Assert, slice1Test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArangeMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArangeMul(ByVal backend As Nd4jBackend)
			Dim arange As INDArray = Nd4j.arange(1, 17).reshape("f"c, 4, 4).castTo(DataType.DOUBLE)
			Dim index As INDArrayIndex = NDArrayIndex.interval(0, 2)
			Dim get As INDArray = arange.get(index, index)
			Dim zeroPointTwoFive As INDArray = Nd4j.ones(DataType.DOUBLE, 2, 2).mul(0.25)
			Dim mul As INDArray = get.mul(zeroPointTwoFive)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {0.25, 1.25},
				New Double() {0.5, 1.5}
			}, "f"c)
			assertEquals(assertion, mul)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetIndicesVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetIndicesVector(ByVal backend As Nd4jBackend)
			Dim line As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(1), -1)
			Dim test As INDArray = Nd4j.create(New Double() {2, 3})
			Dim result As INDArray = line.get(NDArrayIndex.point(0), NDArrayIndex.interval(1, 3))
			assertEquals(test, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetIndicesVectorView(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetIndicesVectorView(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.linspace(1, 25, 25, DataType.DOUBLE).reshape("c"c,5, 5)
			Dim column As INDArray = matrix.getColumn(0).reshape(ChrW(1), 5)
			Dim test As INDArray = Nd4j.create(New Double() {6, 11})
			Dim result As INDArray = Nothing 'column.get(NDArrayIndex.point(0), NDArrayIndex.interval(1, 3));
	'        assertEquals(test, result);
	'
			Dim column3 As INDArray = matrix.getColumn(2).reshape(ChrW(1), 5)
	'        INDArray exp = Nd4j.create(new double[] {8, 13});
	'        result = column3.get(NDArrayIndex.point(0), NDArrayIndex.interval(1, 3));
	'        assertEquals(exp, result);

			Dim exp2 As INDArray = Nd4j.create(New Double() {8, 18})
			result = column3.get(NDArrayIndex.point(0), NDArrayIndex.interval(1, 2, 4))
			assertEquals(exp2, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2dGetPoint(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2dGetPoint(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1,12,12, DataType.DOUBLE).reshape("c"c,3,4)
			For i As Integer = 0 To 2
				Dim exp As INDArray = Nd4j.create(New Double(){i*4+1, i*4+2, i*4+3, i*4+4})
				Dim row As INDArray = arr.getRow(i)
				Dim get As INDArray = arr.get(NDArrayIndex.point(i), NDArrayIndex.all())

				assertEquals(1, row.rank())
				assertEquals(1, get.rank())
				assertEquals(exp, row)
				assertEquals(exp, get)
			Next i

			For i As Integer = 0 To 3
				Dim exp As INDArray = Nd4j.create(New Double(){1+i, 5+i, 9+i})
				Dim col As INDArray = arr.getColumn(i)
				Dim get As INDArray = arr.get(NDArrayIndex.all(), NDArrayIndex.point(i))

				assertEquals(1, col.rank())
				assertEquals(1, get.rank())
				assertEquals(exp, col)
				assertEquals(exp, get)
			Next i
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace