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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class SlicingTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SlicingTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlices()
		Public Overridable Sub testSlices()
			Dim arr As INDArray = Nd4j.create(Nd4j.linspace(1, 24, 24, DataType.DOUBLE).data(), New Integer() {4, 3, 2})
			Dim i As Integer = 0
			Do While i < arr.slices()
				Dim slice As INDArray = arr.slice(i).slice(1)
				Dim slices As val = slice.slices()
				assertEquals(2, slices)
				i += 1
			Loop

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlice()
		Public Overridable Sub testSlice()
			Dim arr As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape(ChrW(4), 3, 2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 13},
				New Double() {5, 17},
				New Double() {9, 21}
			})

			Dim firstSlice As INDArray = arr.slice(0)
			Dim slice1Assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {2, 14},
				New Double() {6, 18},
				New Double() {10, 22}
			})

			Dim secondSlice As INDArray = arr.slice(1)
			assertEquals(assertion, firstSlice)
			assertEquals(slice1Assertion, secondSlice)

		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace