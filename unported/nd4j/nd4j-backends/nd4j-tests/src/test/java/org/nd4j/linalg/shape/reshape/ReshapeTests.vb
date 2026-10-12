Imports Slf4j = lombok.extern.slf4j.Slf4j
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
Imports org.junit.jupiter.api.Assertions
Imports org.junit.jupiter.api.Assumptions

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

Namespace org.nd4j.linalg.shape.reshape


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class ReshapeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ReshapeTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThreeTwoTwoTwo(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThreeTwoTwoTwo(ByVal backend As Nd4jBackend)
			Dim threeTwoTwo As INDArray = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape(ChrW(3), 2, 2)
			Dim sliceZero As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 7},
				New Double() {4, 10}
			})
			Dim sliceOne As INDArray = Nd4j.create(New Double()() {
				New Double() {2, 8},
				New Double() {5, 11}
			})
			Dim sliceTwo As INDArray = Nd4j.create(New Double()() {
				New Double() {3, 9},
				New Double() {6, 12}
			})
			Dim assertions() As INDArray = {sliceZero, sliceOne, sliceTwo}

			Dim i As Integer = 0
			Do While i < threeTwoTwo.slices()
				Dim sliceI As INDArray = threeTwoTwo.slice(i)
				assertEquals(assertions(i), sliceI)
				i += 1
			Loop

			Dim linspaced As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim assertionsTwo() As INDArray = {Nd4j.create(New Double() {1, 3}), Nd4j.create(New Double() {2, 4})}

			For i As Integer = 0 To assertionsTwo.Length - 1
				assertEquals(linspaced.slice(i), assertionsTwo(i))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testColumnVectorReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testColumnVectorReshape(ByVal backend As Nd4jBackend)
			Dim delta As Double = 1e-1
			Dim arr As INDArray = Nd4j.create(1, 3)
			Dim reshaped As INDArray = arr.reshape("f"c, 3, 1)
			assertArrayEquals(New Long() {3, 1}, reshaped.shape())
			assertEquals(0.0, reshaped.getDouble(1), delta)
			assertEquals(0.0, reshaped.getDouble(2), delta)
			log.info("Reshaped: {}", reshaped.shapeInfoDataBuffer().asInt())
			assertNotNull(reshaped.ToString())
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace