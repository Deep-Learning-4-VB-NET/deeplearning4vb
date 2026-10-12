Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports org.nd4j.common.primitives
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
import static org.junit.jupiter.api.Assertions.assertArrayEquals

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

Namespace org.nd4j.linalg.api

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class TestNDArrayCreationUtil extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestNDArrayCreationUtil
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapes()
		Public Overridable Sub testShapes()

			Dim shape2d() As Long = {2, 3}
			For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAllTestMatricesWithShape(2, 3, 12345, DataType.DOUBLE)
				assertArrayEquals(shape2d, p.First.shape(),p.Second)
			Next p

			Dim shape3d() As Long = {2, 3, 4}
			For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, shape3d, DataType.DOUBLE)
				assertArrayEquals(shape3d, p.First.shape(),p.Second)
			Next p

			Dim shape4d() As Long = {2, 3, 4, 5}
			For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, ArrayUtil.toInts(shape4d), DataType.DOUBLE)
				assertArrayEquals(shape4d, p.First.shape(),p.Second)
			Next p

			Dim shape5d() As Long = {2, 3, 4, 5, 6}
			For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll5dTestArraysWithShape(12345, ArrayUtil.toInts(shape5d), DataType.DOUBLE)
				assertArrayEquals(shape5d, p.First.shape(),p.Second)
			Next p

			Dim shape6d() As Long = {2, 3, 4, 5, 6, 7}
			For Each p As Pair(Of INDArray, String) In NDArrayCreationUtil.getAll6dTestArraysWithShape(12345, ArrayUtil.toInts(shape6d), DataType.DOUBLE)
				assertArrayEquals(shape6d, p.First.shape(),p.Second)
			Next p
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace