Imports HistoryMergeAssembler = org.deeplearning4j.rl4j.observation.transform.operation.historymerge.HistoryMergeAssembler
Imports HistoryMergeElementStore = org.deeplearning4j.rl4j.observation.transform.operation.historymerge.HistoryMergeElementStore
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

Namespace org.deeplearning4j.rl4j.observation.transform.operation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class HistoryMergeTransformTest
	Public Class HistoryMergeTransformTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_firstDimensionIsNotBatch_expect_observationAddedAsIs()
		Public Overridable Sub when_firstDimensionIsNotBatch_expect_observationAddedAsIs()
			' Arrange
			Dim store As New MockStore(False)
			Dim sut As HistoryMergeTransform = HistoryMergeTransform.builder().isFirstDimenstionBatch(False).elementStore(store).build(4)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })

			' Act
			sut.transform(input)

			' Assert
			assertEquals(1, store.addedObservation.shape().Length)
			assertEquals(3, store.addedObservation.shape()(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_firstDimensionIsBatch_expect_observationAddedAsSliced()
		Public Overridable Sub when_firstDimensionIsBatch_expect_observationAddedAsSliced()
			' Arrange
			Dim store As New MockStore(False)
			Dim sut As HistoryMergeTransform = HistoryMergeTransform.builder().isFirstDimenstionBatch(True).elementStore(store).build(4)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 }).reshape(ChrW(1), 3)

			' Act
			sut.transform(input)

			' Assert
			assertEquals(1, store.addedObservation.shape().Length)
			assertEquals(3, store.addedObservation.shape()(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notReady_expect_resultIsNull()
		Public Overridable Sub when_notReady_expect_resultIsNull()
			' Arrange
			Dim store As New MockStore(False)
			Dim sut As HistoryMergeTransform = HistoryMergeTransform.builder().isFirstDimenstionBatch(True).elementStore(store).build(4)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })

			' Act
			Dim result As INDArray = sut.transform(input)

			' Assert
			assertNull(result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notShouldStoreCopy_expect_sameIsStored()
		Public Overridable Sub when_notShouldStoreCopy_expect_sameIsStored()
			' Arrange
			Dim store As New MockStore(False)
			Dim sut As HistoryMergeTransform = HistoryMergeTransform.builder().shouldStoreCopy(False).elementStore(store).build(4)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })

			' Act
			Dim result As INDArray = sut.transform(input)

			' Assert
			assertSame(input, store.addedObservation)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_shouldStoreCopy_expect_copyIsStored()
		Public Overridable Sub when_shouldStoreCopy_expect_copyIsStored()
			' Arrange
			Dim store As New MockStore(True)
			Dim sut As HistoryMergeTransform = HistoryMergeTransform.builder().shouldStoreCopy(True).elementStore(store).build(4)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })

			' Act
			Dim result As INDArray = sut.transform(input)

			' Assert
			assertNotSame(input, store.addedObservation)
			assertEquals(1, store.addedObservation.shape().Length)
			assertEquals(3, store.addedObservation.shape()(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_transformCalled_expect_storeContentAssembledAndOutputHasCorrectShape()
		Public Overridable Sub when_transformCalled_expect_storeContentAssembledAndOutputHasCorrectShape()
			' Arrange
			Dim store As New MockStore(True)
			Dim assemble As New MockAssemble()
			Dim sut As HistoryMergeTransform = HistoryMergeTransform.builder().elementStore(store).assembler(assemble).build(4)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 })

			' Act
			Dim result As INDArray = sut.transform(input)

			' Assert
			assertEquals(1, assemble.assembleElements.Length)
			assertSame(store.addedObservation, assemble.assembleElements(0))

			assertEquals(2, result.shape().Length)
			assertEquals(1, result.shape()(0))
			assertEquals(3, result.shape()(1))
		End Sub

		Public Class MockStore
			Implements HistoryMergeElementStore

			Friend ReadOnly isReady As Boolean
			Friend addedObservation As INDArray

			Public Sub New(ByVal isReady As Boolean)

				Me.isReady = isReady
			End Sub

			Public Overridable Sub add(ByVal observation As INDArray) Implements HistoryMergeElementStore.add
				addedObservation = observation
			End Sub

			Public Overridable Function get() As INDArray() Implements HistoryMergeElementStore.get
				Return New INDArray() { addedObservation }
			End Function

			Public Overridable ReadOnly Property Ready As Boolean Implements HistoryMergeElementStore.isReady
				Get
					Return isReady
				End Get
			End Property

			Public Overridable Sub reset() Implements HistoryMergeElementStore.reset

			End Sub
		End Class

		Public Class MockAssemble
			Implements HistoryMergeAssembler

			Friend assembleElements() As INDArray

			Public Overridable Function assemble(ByVal elements() As INDArray) As INDArray Implements HistoryMergeAssembler.assemble
				assembleElements = elements
				Return elements(0)
			End Function
		End Class
	End Class

End Namespace