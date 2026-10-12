Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports PointIndex = org.nd4j.linalg.indexing.PointIndex
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

Namespace org.nd4j.linalg.api.indexing.resolve

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class NDArrayIndexResolveTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NDArrayIndexResolveTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResolvePoint(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testResolvePoint(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim test() As INDArrayIndex = NDArrayIndex.resolve(arr.shape(), NDArrayIndex.point(1))
			Dim assertion() As INDArrayIndex = {NDArrayIndex.point(1), NDArrayIndex.all()}
			assertArrayEquals(assertion, test)

			Dim allAssertion() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.all()}
			assertArrayEquals(allAssertion, NDArrayIndex.resolve(arr.shape(), NDArrayIndex.all()))

			Dim allAndOne() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.point(1)}
			assertArrayEquals(allAndOne, NDArrayIndex.resolve(arr.shape(), allAndOne))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResolvePointVector()
		Public Overridable Sub testResolvePointVector()
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4)
			Dim getPoint() As INDArrayIndex = {NDArrayIndex.point(1)}
			Dim resolved() As INDArrayIndex = NDArrayIndex.resolve(arr.shape(), getPoint)
			If getPoint.Length = resolved.Length Then
				assertArrayEquals(getPoint, resolved)
			Else
				assertEquals(2, resolved.Length)
				assertTrue(TypeOf resolved(0) Is PointIndex)
				assertEquals(0, resolved(0).offset())
				assertTrue(TypeOf resolved(1) Is PointIndex)
				assertEquals(1, resolved(1).offset())
			End If

		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace