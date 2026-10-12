Imports System
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

Namespace org.nd4j.linalg.dataset.api.preprocessor

	<Serializable>
	Public Class LabelLastTimeStepPreProcessor
		Implements DataSetPreProcessor

		Public Overridable Sub preProcess(ByVal toPreProcess As DataSet)

			Dim label3d As INDArray = toPreProcess.Labels
			Preconditions.checkState(label3d.rank() = 3, "LabelLastTimeStepPreProcessor expects rank 3 labels, got rank %s labels with shape %ndShape", label3d.rank(), label3d)

			Dim lMask As INDArray = toPreProcess.LabelsMaskArray
			'If no mask: assume that examples for each minibatch are all same length
			Dim labels2d As INDArray
			If lMask Is Nothing Then
				labels2d = label3d.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(label3d.size(2)-1)).dup()
			Else
				'Use the label mask to work out the last time step...
				Dim lastIndex As INDArray = BooleanIndexing.lastIndex(lMask, Conditions.greaterThan(0), 1)
				Dim idxs() As Long = lastIndex.data().asLong()

				'Now, extract out:
				labels2d = Nd4j.create(DataType.FLOAT, label3d.size(0), label3d.size(1))

				'Now, get and assign the corresponding subsets of 3d activations:
				For i As Integer = 0 To idxs.Length - 1
					Dim lastStepIdx As Long = idxs(i)
					Preconditions.checkState(lastStepIdx >= 0, "Invalid last time step index: example %s in minibatch is entirely masked out" & " (label mask is all 0s, meaning no label data is present for this example)", i)
					'TODO can optimize using reshape + pullRows
					labels2d.putRow(i, label3d.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(lastStepIdx)))
				Next i
			End If

			toPreProcess.Labels = labels2d
			toPreProcess.LabelsMaskArray = Nothing 'Remove label mask if present
		End Sub
	End Class

End Namespace