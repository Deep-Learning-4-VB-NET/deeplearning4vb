Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.shape

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class LongShapeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LongShapeTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLongBuffer_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongBuffer_1(ByVal backend As Nd4jBackend)
			Dim exp As val = New Long(){2, 5, 3, 3, 1, 0, 1, 99}
			Dim buffer As val = Nd4j.DataBufferFactory.createLong(exp)

			Dim java As val = buffer.asLong()

			assertArrayEquals(exp, java)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLongShape_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongShape_1(ByVal backend As Nd4jBackend)
			Dim exp As val = New Long(){2, 5, 3, 3, 1, 16384, 1, 99}

			Dim array As val = Nd4j.createUninitialized(DataType.DOUBLE, 5, 3)
			Dim buffer As val = array.shapeInfoDataBuffer()

			Dim java As val = buffer.asLong()

			assertArrayEquals(exp, java)
			assertEquals(8, buffer.getElementSize())
		End Sub



		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace