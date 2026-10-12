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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayMath = org.nd4j.linalg.util.NDArrayMath
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

Namespace org.nd4j.linalg.shape


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class NDArrayMathTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NDArrayMathTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorPerSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorPerSlice(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(2, 2, 2, 2)
			assertEquals(4, NDArrayMath.vectorsPerSlice(arr))

			Dim matrix As INDArray = Nd4j.create(2, 2)
			assertEquals(2, NDArrayMath.vectorsPerSlice(matrix))

			Dim arrSliceZero As INDArray = arr.slice(0)
			assertEquals(4, NDArrayMath.vectorsPerSlice(arrSliceZero))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatricesPerSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatricesPerSlice(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(2, 2, 2, 2)
			assertEquals(2, NDArrayMath.matricesPerSlice(arr))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLengthPerSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLengthPerSlice(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(2, 2, 2, 2)
			Dim lengthPerSlice As val = NDArrayMath.lengthPerSlice(arr)
			assertEquals(8, lengthPerSlice)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void toffsetForSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub toffsetForSlice(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(3, 2, 2)
			Dim slice As Integer = 1
			assertEquals(4, NDArrayMath.offsetForSlice(arr, slice))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMapOntoVector(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMapOntoVector(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(3, 2, 2)
			assertEquals(NDArrayMath.mapIndexOntoVector(2, arr), 4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNumVectors(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNumVectors(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(3, 2, 2)
			assertEquals(4, NDArrayMath.vectorsPerSlice(arr))
			Dim matrix As INDArray = Nd4j.create(2, 2)
			assertEquals(2, NDArrayMath.vectorsPerSlice(matrix))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOffsetForSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOffsetForSlice(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim dimensions() As Integer = {0, 1}
			Dim permuted As INDArray = arr.permute(2, 3, 0, 1)
			Dim test() As Integer = {0, 0, 1, 1}
			Dim i As Integer = 0
			Do While i < permuted.tensorsAlongDimension(dimensions)
				assertEquals(test(i), NDArrayMath.sliceOffsetForTensor(i, permuted, New Integer() {2, 2}))
				i += 1
			Loop

			Dim arrTensorsPerSlice As val = NDArrayMath.tensorsPerSlice(arr, New Integer() {2, 2})
			assertEquals(2, arrTensorsPerSlice)

			Dim arr2 As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim assertions() As Integer = {0, 1, 2}
			For i As Integer = 0 To assertions.Length - 1
				assertEquals(assertions(i), NDArrayMath.sliceOffsetForTensor(i, arr2, New Integer() {2, 2}))
			Next i



			Dim tensorsPerSlice As val = NDArrayMath.tensorsPerSlice(arr2, New Integer() {2, 2})
			assertEquals(1, tensorsPerSlice)


			Dim otherTest As INDArray = Nd4j.linspace(1, 144, 144, DataType.DOUBLE).reshape(ChrW(6), 3, 2, 2, 2)
	'        System.out.println(otherTest);
			Dim baseArr As INDArray = Nd4j.linspace(1, 8, 8, DataType.DOUBLE).reshape(ChrW(2), 2, 2)
			i = 0
			Do While i < baseArr.tensorsAlongDimension(0, 1)
	'            System.out.println(NDArrayMath.sliceOffsetForTensor(i, baseArr, new int[] {2, 2}));
				NDArrayMath.sliceOffsetForTensor(i, baseArr, New Integer() {2, 2})
				i += 1
			Loop


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOddDimensions(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOddDimensions(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(3, 2, 2)
			Dim numMatrices As val = NDArrayMath.matricesPerSlice(arr)
			assertEquals(1, numMatrices)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTotalVectors(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTotalVectors(ByVal backend As Nd4jBackend)
			Dim arr2 As INDArray = Nd4j.create(2, 2, 2, 2)
			assertEquals(8, NDArrayMath.numVectors(arr2))
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace