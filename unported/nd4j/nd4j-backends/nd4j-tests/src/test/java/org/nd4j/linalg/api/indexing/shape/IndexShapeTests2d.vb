Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class IndexShapeTests2d extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class IndexShapeTests2d
		Inherits BaseNd4jTestWithBackends


		Private shape() As Long = {3, 2}


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2dCases(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2dCases(ByVal backend As Nd4jBackend)
			assertArrayEquals(New Long() {1, 2}, Indices.shape(shape, NDArrayIndex.point(1)))
			assertArrayEquals(New Long() {3, 1}, Indices.shape(shape, NDArrayIndex.all(), NDArrayIndex.point(1)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewAxis2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewAxis2d(ByVal backend As Nd4jBackend)
			assertArrayEquals(New Long() {1, 3, 2}, Indices.shape(shape, NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.all()))
			assertArrayEquals(New Long() {3, 1, 2}, Indices.shape(shape, NDArrayIndex.all(), NDArrayIndex.newAxis(), NDArrayIndex.all()))

		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace