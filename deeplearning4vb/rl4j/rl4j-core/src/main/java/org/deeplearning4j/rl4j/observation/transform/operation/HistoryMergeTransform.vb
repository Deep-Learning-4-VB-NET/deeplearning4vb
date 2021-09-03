Imports org.datavec.api.transform
Imports INDArrayHelper = org.deeplearning4j.rl4j.helper.INDArrayHelper
Imports ResettableOperation = org.deeplearning4j.rl4j.observation.transform.ResettableOperation
Imports CircularFifoStore = org.deeplearning4j.rl4j.observation.transform.operation.historymerge.CircularFifoStore
Imports HistoryMergeAssembler = org.deeplearning4j.rl4j.observation.transform.operation.historymerge.HistoryMergeAssembler
Imports HistoryMergeElementStore = org.deeplearning4j.rl4j.observation.transform.operation.historymerge.HistoryMergeElementStore
Imports HistoryStackAssembler = org.deeplearning4j.rl4j.observation.transform.operation.historymerge.HistoryStackAssembler
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

	Public Class HistoryMergeTransform
		Implements Operation(Of INDArray, INDArray), ResettableOperation

		Private ReadOnly historyMergeElementStore As HistoryMergeElementStore
		Private ReadOnly historyMergeAssembler As HistoryMergeAssembler
		Private ReadOnly shouldStoreCopy As Boolean
		Private ReadOnly isFirstDimensionBatch As Boolean

		Private Sub New(ByVal builder As Builder)
			Me.historyMergeElementStore = builder.historyMergeElementStore
			Me.historyMergeAssembler = builder.historyMergeAssembler
			Me.shouldStoreCopy = builder.shouldStoreCopy_Conflict
			Me.isFirstDimensionBatch = builder.isFirstDimenstionBatch_Conflict
		End Sub

		Public Overridable Function transform(ByVal input As INDArray) As INDArray

			Dim element As INDArray
			If isFirstDimensionBatch Then
				element = input.slice(0, 0)
			Else
				element = input
			End If

			If shouldStoreCopy Then
				element = element.dup()
			End If

			historyMergeElementStore.add(element)
			If Not historyMergeElementStore.Ready Then
				Return Nothing
			End If

			Dim result As INDArray = historyMergeAssembler.assemble(historyMergeElementStore.get())

			Return INDArrayHelper.forceCorrectShape(result)
		End Function

		Public Overridable Sub reset() Implements ResettableOperation.reset
			historyMergeElementStore.reset()
		End Sub

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		Public Class Builder
			Friend historyMergeElementStore As HistoryMergeElementStore
			Friend historyMergeAssembler As HistoryMergeAssembler
'JAVA TO VB CONVERTER NOTE: The field shouldStoreCopy was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend shouldStoreCopy_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field isFirstDimenstionBatch was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend isFirstDimenstionBatch_Conflict As Boolean = False

			''' <summary>
			''' Default is <seealso cref="CircularFifoStore CircularFifoStore"/>
			''' </summary>
			Public Overridable Function elementStore(ByVal store As HistoryMergeElementStore) As Builder
				Me.historyMergeElementStore = store
				Return Me
			End Function

			''' <summary>
			''' Default is <seealso cref="HistoryStackAssembler HistoryStackAssembler"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter assembler was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function assembler(ByVal assembler_Conflict As HistoryMergeAssembler) As Builder
				Me.historyMergeAssembler = assembler_Conflict
				Return Me
			End Function

			''' <summary>
			''' If true, tell the HistoryMergeTransform to store copies of incoming INDArrays.
			''' (To prevent later in-place changes to a stored INDArray from changing what has been stored)
			''' 
			''' Default is false
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter shouldStoreCopy was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function shouldStoreCopy(ByVal shouldStoreCopy_Conflict As Boolean) As Builder
				Me.shouldStoreCopy_Conflict = shouldStoreCopy_Conflict
				Return Me
			End Function

			''' <summary>
			''' If true, tell the HistoryMergeTransform that the first dimension of the input INDArray is the batch count.
			''' When this is the case, the HistoryMergeTransform will slice the input like this [batch, height, width] -> [height, width]
			''' 
			''' Default is false
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter isFirstDimenstionBatch was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function isFirstDimenstionBatch(ByVal isFirstDimenstionBatch_Conflict As Boolean) As Builder
				Me.isFirstDimenstionBatch_Conflict = isFirstDimenstionBatch_Conflict
				Return Me
			End Function

			Public Overridable Function build(ByVal frameStackLength As Integer) As HistoryMergeTransform
				If historyMergeElementStore Is Nothing Then
					historyMergeElementStore = New CircularFifoStore(frameStackLength)
				End If

				If historyMergeAssembler Is Nothing Then
					historyMergeAssembler = New HistoryStackAssembler()
				End If

				Return New HistoryMergeTransform(Me)
			End Function
		End Class
	End Class

End Namespace