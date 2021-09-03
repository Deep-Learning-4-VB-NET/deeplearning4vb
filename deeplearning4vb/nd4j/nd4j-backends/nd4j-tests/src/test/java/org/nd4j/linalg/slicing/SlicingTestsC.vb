Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports SpecifiedIndex = org.nd4j.linalg.indexing.SpecifiedIndex
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.slicing

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class SlicingTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SlicingTestsC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceRowVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceRowVector(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.zeros(5)
	'        System.out.println(arr.slice(1));
			arr.slice(1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceAssertion(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceAssertion(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 30, 30).reshape(ChrW(3), 5, 2)
			Dim firstRow As INDArray = arr.slice(0).slice(0)
	'        for (int i = 0; i < firstRow.length(); i++) {
	'            System.out.println(firstRow.getDouble(i));
	'        }
	'        System.out.println(firstRow);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceShape(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape(ChrW(3), 5, 2)

			Dim sliceZero As INDArray = arr.slice(0)
			Dim i As Integer = 0
			Do While i < sliceZero.rows()
				Dim row As INDArray = sliceZero.slice(i)
	'            for (int j = 0; j < row.length(); j++) {
	'                System.out.println(row.getDouble(j));
	'            }
	'            System.out.println(row);
				i += 1
			Loop

			Dim assertion As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, New Integer() {5, 2})
			i = 0
			Do While i < assertion.rows()
				Dim row As INDArray = assertion.slice(i)
	'            for (int j = 0; j < row.length(); j++) {
	'                System.out.println(row.getDouble(j));
	'            }
	'            System.out.println(row);
				i += 1
			Loop
			assertArrayEquals(New Long() {5, 2}, sliceZero.shape())
			assertEquals(assertion, sliceZero)

			Dim assertionTwo As INDArray = Nd4j.create(New Double() {11, 12, 13, 14, 15, 16, 17, 18, 19, 20}, New Integer() {5, 2})
			Dim sliceTest As INDArray = arr.slice(1)
			assertEquals(assertionTwo, sliceTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSwapReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSwapReshape(ByVal backend As Nd4jBackend)
			Dim n2 As INDArray = Nd4j.create(Nd4j.linspace(1, 30, 30, DataType.FLOAT).data(), New Integer() {3, 5, 2})
			Dim swapped As INDArray = n2.swapAxes(n2.shape().Length - 1, 1)
			Dim firstSlice2 As INDArray = swapped.slice(0).slice(0)
			Dim oneThreeFiveSevenNine As INDArray = Nd4j.create(New Single() {1, 3, 5, 7, 9})
			assertEquals(firstSlice2, oneThreeFiveSevenNine)
			Dim raveled As INDArray = oneThreeFiveSevenNine.reshape(ChrW(5), 1)
			Dim raveledOneThreeFiveSevenNine As INDArray = oneThreeFiveSevenNine.reshape(ChrW(5), 1)
			assertEquals(raveled, raveledOneThreeFiveSevenNine)


			Dim firstSlice3 As INDArray = swapped.slice(0).slice(1)
			Dim twoFourSixEightTen As INDArray = Nd4j.create(New Single() {2, 4, 6, 8, 10})
			assertEquals(firstSlice2, oneThreeFiveSevenNine)
			Dim raveled2 As INDArray = twoFourSixEightTen.reshape(ChrW(5), 1)
			Dim raveled3 As INDArray = firstSlice3.reshape(ChrW(5), 1)
			assertEquals(raveled2, raveled3)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRow(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(2), 3)
			Dim get As INDArray = arr.getRow(1)
			Dim get2 As INDArray = arr.get(NDArrayIndex.point(1), NDArrayIndex.all())
			Dim assertion As INDArray = Nd4j.create(New Double() {4, 5, 6})
			assertEquals(assertion, get)
			assertEquals(get, get2)
			get2.assign(Nd4j.linspace(1, 3, 3, DataType.DOUBLE))
			assertEquals(Nd4j.linspace(1, 3, 3, DataType.DOUBLE), get2)

			Dim threeByThree As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)
			Dim offsetTest As INDArray = threeByThree.get(New SpecifiedIndex(1, 2), NDArrayIndex.all())
			Dim threeByThreeAssertion As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 5, 6},
				New Double() {7, 8, 9}
			})

			assertEquals(threeByThreeAssertion, offsetTest)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorIndexing(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorIndexing(ByVal backend As Nd4jBackend)
			Dim zeros As INDArray = Nd4j.create(1, 400000)
			Dim get As INDArray = zeros.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 300000))
			assertArrayEquals(New Long() {300000}, get.shape())
		End Sub



		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace