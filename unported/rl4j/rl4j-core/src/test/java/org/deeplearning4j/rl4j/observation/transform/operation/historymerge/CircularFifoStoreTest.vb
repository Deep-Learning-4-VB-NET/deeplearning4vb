Imports Test = org.junit.jupiter.api.Test
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

	Public Class CircularFifoStoreTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void when_fifoSizeIsLessThan1_expect_exception()
		Public Overridable Sub when_fifoSizeIsLessThan1_expect_exception()
		  assertThrows(GetType(System.ArgumentException),Sub()
		  Dim sut As New CircularFifoStore(0)
		  End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_adding2elementsWithSize2_expect_notReadyAfter1stReadyAfter2nd()
		Public Overridable Sub when_adding2elementsWithSize2_expect_notReadyAfter1stReadyAfter2nd()
			' Arrange
			Dim sut As New CircularFifoStore(2)
			Dim firstElement As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })
			Dim secondElement As INDArray = Nd4j.create(New Double() { 10.0, 20.0, 30.0 })

			' Act
			sut.add(firstElement)
			Dim isReadyAfter1st As Boolean = sut.Ready
			sut.add(secondElement)
			Dim isReadyAfter2nd As Boolean = sut.Ready

			' Assert
			assertFalse(isReadyAfter1st)
			assertTrue(isReadyAfter2nd)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_adding2elementsWithSize2_expect_getReturnThese2()
		Public Overridable Sub when_adding2elementsWithSize2_expect_getReturnThese2()
			' Arrange
			Dim sut As New CircularFifoStore(2)
			Dim firstElement As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })
			Dim secondElement As INDArray = Nd4j.create(New Double() { 10.0, 20.0, 30.0 })

			' Act
			sut.add(firstElement)
			sut.add(secondElement)
			Dim results() As INDArray = sut.get()

			' Assert
			assertEquals(2, results.Length)

			assertEquals(1.0, results(0).getDouble(0), 0.00001)
			assertEquals(2.0, results(0).getDouble(1), 0.00001)
			assertEquals(3.0, results(0).getDouble(2), 0.00001)

			assertEquals(10.0, results(1).getDouble(0), 0.00001)
			assertEquals(20.0, results(1).getDouble(1), 0.00001)
			assertEquals(30.0, results(1).getDouble(2), 0.00001)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_adding2elementsThenCallingReset_expect_getReturnEmpty()
		Public Overridable Sub when_adding2elementsThenCallingReset_expect_getReturnEmpty()
			' Arrange
			Dim sut As New CircularFifoStore(2)
			Dim firstElement As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })
			Dim secondElement As INDArray = Nd4j.create(New Double() { 10.0, 20.0, 30.0 })

			' Act
			sut.add(firstElement)
			sut.add(secondElement)
			sut.reset()
			Dim results() As INDArray = sut.get()

			' Assert
			assertEquals(0, results.Length)
		End Sub

	End Class

End Namespace