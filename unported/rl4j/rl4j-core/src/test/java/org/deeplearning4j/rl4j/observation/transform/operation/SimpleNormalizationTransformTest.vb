Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertThrows

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class SimpleNormalizationTransformTest
	Public Class SimpleNormalizationTransformTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_maxIsLessThanMin_expect_exception()
		Public Overridable Sub when_maxIsLessThanMin_expect_exception()
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As New SimpleNormalizationTransform(10.0, 1.0)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_transformIsCalled_expect_inputNormalized()
		Public Overridable Sub when_transformIsCalled_expect_inputNormalized()
			' Arrange
			Dim sut As New SimpleNormalizationTransform(1.0, 11.0)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 11.0 })

			' Act
			Dim result As INDArray = sut.transform(input)

			' Assert
			assertEquals(0.0, result.getDouble(0), 0.00001)
			assertEquals(1.0, result.getDouble(1), 0.00001)
		End Sub

	End Class

End Namespace