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

Namespace org.nd4j.linalg.shape.concat.padding

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class PaddingTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class PaddingTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAppend(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAppend(ByVal backend As Nd4jBackend)
			Dim appendTo As INDArray = Nd4j.ones(DataType.DOUBLE,3, 3)
			Dim ret As INDArray = Nd4j.append(appendTo, 3, 1, -1)
			assertArrayEquals(New Long() {3, 6}, ret.shape())

			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim otherAppend As INDArray = Nd4j.append(linspace, 3, 1.0, -1)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 3, 1, 1, 1},
				New Double() {2, 4, 1, 1, 1}
			})

			assertEquals(assertion, otherAppend)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPrepend(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPrepend(ByVal backend As Nd4jBackend)
			Dim appendTo As INDArray = Nd4j.ones(DataType.DOUBLE, 3, 3)
			Dim ret As INDArray = Nd4j.append(appendTo, 3, 1, -1)
			assertArrayEquals(New Long() {3, 6}, ret.shape())

			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 1, 1, 3},
				New Double() {1, 1, 1, 2, 4}
			})

			Dim prepend As INDArray = Nd4j.prepend(linspace, 3, 1.0, -1)
			assertEquals(assertion, prepend)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPad(ByVal backend As Nd4jBackend)

			Dim start As INDArray = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape(ChrW(3), 3)
			Dim ret As INDArray = Nd4j.pad(start, 5, 5)
			Dim data()() As Double = {
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 1, 4, 7, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 2, 5, 8, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 3, 6, 9, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0},
				New Double() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0}
			}
			Dim assertion As INDArray = Nd4j.create(data)
			assertEquals(assertion, ret)


		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace