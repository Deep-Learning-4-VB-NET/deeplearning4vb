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

Namespace org.deeplearning4j.rl4j.observation.transform.operation.historymerge

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class HistoryStackAssemblerTest
	Public Class HistoryStackAssemblerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_assembling2INDArrays_expect_stackedAsResult()
		Public Overridable Sub when_assembling2INDArrays_expect_stackedAsResult()
			' Arrange
			Dim input() As INDArray = { Nd4j.create(New Double() { 1.0, 2.0, 3.0 }), Nd4j.create(New Double() { 10.0, 20.0, 30.0 })}
			Dim sut As New HistoryStackAssembler()

			' Act
			Dim result As INDArray = sut.assemble(input)

			' Assert
			assertEquals(2, result.shape().Length)
			assertEquals(2, result.shape()(0))
			assertEquals(3, result.shape()(1))

			assertEquals(1.0, result.getDouble(0, 0), 0.00001)
			assertEquals(2.0, result.getDouble(0, 1), 0.00001)
			assertEquals(3.0, result.getDouble(0, 2), 0.00001)

			assertEquals(10.0, result.getDouble(1, 0), 0.00001)
			assertEquals(20.0, result.getDouble(1, 1), 0.00001)
			assertEquals(30.0, result.getDouble(1, 2), 0.00001)

		End Sub
	End Class

End Namespace