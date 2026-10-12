Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.rl4j.observation.transform.operation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class ArrayToINDArrayTransformTest
	Public Class ArrayToINDArrayTransformTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notUsingShape_expect_transformTo1DINDArray()
		Public Overridable Sub when_notUsingShape_expect_transformTo1DINDArray()
			' Arrange
			Dim sut As New ArrayToINDArrayTransform()
			Dim data() As Double = { 1.0, 2.0, 3.0 }

			' Act
			Dim result As INDArray = sut.transform(data)

			' Assert
			assertArrayEquals(New Long() { 3 }, result.shape())
			assertEquals(1.0, result.getDouble(0), 0.00001)
			assertEquals(2.0, result.getDouble(1), 0.00001)
			assertEquals(3.0, result.getDouble(2), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_usingShape_expect_transformTo1DINDArray()
		Public Overridable Sub when_usingShape_expect_transformTo1DINDArray()
			' Arrange
			Dim sut As New ArrayToINDArrayTransform(1, 3)
			Dim data() As Double = { 1.0, 2.0, 3.0 }

			' Act
			Dim result As INDArray = sut.transform(data)

			' Assert
			assertArrayEquals(New Long() { 1, 3 }, result.shape())
			assertEquals(1.0, result.getDouble(0, 0), 0.00001)
			assertEquals(2.0, result.getDouble(0, 1), 0.00001)
			assertEquals(3.0, result.getDouble(0, 2), 0.00001)
		End Sub

	End Class

End Namespace