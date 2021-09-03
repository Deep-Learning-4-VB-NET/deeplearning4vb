Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports Indices = org.nd4j.linalg.indexing.Indices
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.nd4j.linalg.api.indexing.shape

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class IndexShapeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class IndexShapeTests
		Inherits BaseNd4jTestWithBackends

		Private shape() As Integer = {1, 1, 2, 1, 3, 4, 5, 1}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSinglePoint(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSinglePoint(ByVal backend As Nd4jBackend)
	'        
	'        Assumes all indexes are filled out.
	'        Test simple general point case
	'         
			Dim assertion() As Integer = {2, 1, 4, 5, 1}
			Dim indexes() As INDArrayIndex = {NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.all()}

			Dim testShape() As Integer = Indices.shape(shape, indexes)
			assertArrayEquals(assertion, testShape)

			Dim secondAssertion() As Integer = {1, 2, 1, 5, 1}
			Dim otherCase() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.point(0) }

			assertArrayEquals(secondAssertion, Indices.shape(shape, otherCase))


			Dim thridAssertion() As Integer = {1, 2, 1, 4, 5, 1}
			Dim thirdCase() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(0)}
			assertArrayEquals(thridAssertion, Indices.shape(shape, thirdCase))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInterval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInterval(ByVal backend As Nd4jBackend)
			Dim basicAssertion() As Integer = {1, 1, 1, 1, 3, 1, 2, 1}
			Dim basicTest() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 1), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(1, 2), NDArrayIndex.interval(2, 4), NDArrayIndex.all()}
			assertArrayEquals(basicAssertion, Indices.shape(shape, basicTest))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewAxis(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewAxis(ByVal backend As Nd4jBackend)
			'normal prepend
			Dim prependAssertion() As Integer = {1, 1, 1, 1, 2, 1, 3, 4, 5, 1}
			Dim prependTest() As INDArrayIndex = {NDArrayIndex.newAxis(), NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}

			assertArrayEquals(prependAssertion, Indices.shape(shape, prependTest))

			'test setting for particular indexes.
			'when an all is encountered before a new axis,
			'it is assumed that new axis must occur at the destination
			'where the new axis was specified
			Dim addToMiddle() As Integer = {1, 1, 2, 1, 1, 1, 3, 4, 5, 1}
			Dim setInMiddleTest() As INDArrayIndex = {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.newAxis(), NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}
			assertArrayEquals(addToMiddle, Indices.shape(shape, setInMiddleTest))

			'test prepending AND adding to middle
			Dim prependAndAddToMiddleAssertion() As Integer = {1, 1, 1, 1, 2, 1, 1, 1, 3, 4, 5, 1}

			Dim prependAndMiddle() As INDArrayIndex = {NDArrayIndex.newAxis(), NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.newAxis(), NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}
			assertArrayEquals(prependAndAddToMiddleAssertion, Indices.shape(shape, prependAndMiddle))

		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace