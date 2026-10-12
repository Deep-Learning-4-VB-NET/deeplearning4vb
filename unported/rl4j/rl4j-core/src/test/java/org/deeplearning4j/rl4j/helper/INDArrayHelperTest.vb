Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.rl4j.helper

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class INDArrayHelperTest
	Public Class INDArrayHelperTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_inputHasIncorrectShape_expect_outputWithCorrectShape()
		Public Overridable Sub when_inputHasIncorrectShape_expect_outputWithCorrectShape()
			' Arrange
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0})

			' Act
			Dim output As INDArray = INDArrayHelper.forceCorrectShape(input)

			' Assert
			assertEquals(2, output.shape().Length)
			assertEquals(1, output.shape()(0))
			assertEquals(3, output.shape()(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_inputHasCorrectShape_expect_outputWithSameShape()
		Public Overridable Sub when_inputHasCorrectShape_expect_outputWithSameShape()
			' Arrange
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0}).reshape(ChrW(1), 3)

			' Act
			Dim output As INDArray = INDArrayHelper.forceCorrectShape(input)

			' Assert
			assertEquals(2, output.shape().Length)
			assertEquals(1, output.shape()(0))
			assertEquals(3, output.shape()(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_inputHasOneDimension_expect_outputWithTwoDimensions()
		Public Overridable Sub when_inputHasOneDimension_expect_outputWithTwoDimensions()
			' Arrange
			Dim input As INDArray = Nd4j.create(New Double() { 1.0 })

			' Act
			Dim output As INDArray = INDArrayHelper.forceCorrectShape(input)

			' Assert
			assertEquals(2, output.shape().Length)
			assertEquals(1, output.shape()(0))
			assertEquals(1, output.shape()(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCreateBatchForShape_expect_INDArrayWithCorrectShapeAndOriginalShapeUnchanged()
		Public Overridable Sub when_callingCreateBatchForShape_expect_INDArrayWithCorrectShapeAndOriginalShapeUnchanged()
			' Arrange
			Dim shape() As Long = { 1, 3, 4}

			' Act
			Dim output As INDArray = INDArrayHelper.createBatchForShape(2, shape)

			' Assert
			' Output shape
			assertArrayEquals(New Long() { 2, 3, 4 }, output.shape())

			' Input should remain unchanged
			assertArrayEquals(New Long() { 1, 3, 4 }, shape)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCreateRnnBatchForShape_expect_INDArrayWithCorrectShapeAndOriginalShapeUnchanged()
		Public Overridable Sub when_callingCreateRnnBatchForShape_expect_INDArrayWithCorrectShapeAndOriginalShapeUnchanged()
			' Arrange
			Dim shape() As Long = { 1, 3, 1 }

			' Act
			Dim output As INDArray = INDArrayHelper.createRnnBatchForShape(5, shape)

			' Assert
			' Output shape
			assertArrayEquals(New Long() { 1, 3, 5 }, output.shape())

			' Input should remain unchanged
			assertArrayEquals(New Long() { 1, 3, 1 }, shape)
		End Sub

	End Class

End Namespace