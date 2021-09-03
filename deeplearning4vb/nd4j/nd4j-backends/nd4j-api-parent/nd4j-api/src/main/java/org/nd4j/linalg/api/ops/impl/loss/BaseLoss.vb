Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.ops.impl.loss


	Public MustInherit Class BaseLoss
		Inherits DynamicCustomOp

		Protected Friend lossReduce As LossReduce

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseLoss(@NonNull SameDiff sameDiff, @NonNull LossReduce lossReduce, @NonNull SDVariable predictions, org.nd4j.autodiff.samediff.SDVariable weights, @NonNull SDVariable labels)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){predictions, getWeights(sameDiff, weights, predictions), labels})
			Me.lossReduce = lossReduce
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseLoss(@NonNull LossReduce lossReduce, @NonNull INDArray predictions, org.nd4j.linalg.api.ndarray.INDArray weights, @NonNull INDArray labels)
		Public Sub New(ByVal lossReduce As LossReduce, ByVal predictions As INDArray, ByVal weights As INDArray, ByVal labels As INDArray)
			MyBase.New(New INDArray(){predictions, getWeights(weights, predictions), labels}, Nothing)
			Me.lossReduce = lossReduce
			addArgs()
		End Sub

		Protected Friend Shared Function getWeights(ByVal weights As INDArray, ByVal predictions As INDArray) As INDArray
			Return If(weights IsNot Nothing, weights, Nd4j.scalar(predictions.dataType(), 1.0))
		End Function

		Protected Friend Shared Function getWeights(ByVal sd As SameDiff, ByVal weights As SDVariable, ByVal predictions As SDVariable) As SDVariable
			Return If(weights IsNot Nothing, weights, sd.constant(Nd4j.scalar(predictions.dataType(), 1.0)))
		End Function

		Protected Friend Sub New()
		End Sub

		Protected Friend Overridable Sub addArgs()
			iArguments.Clear()
			tArguments.Clear()
			addIArgument(lossReduce.ordinal()) 'Ops: 0 - "none"; 1 - "weighted_sum";  2 - "weighted_mean";  3 - "weighted_sum_by_nonzero_weights"
		End Sub

		Public Overrides MustOverride Function opName() As String

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count >= 2, "Expected exactly 2 or more input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0)) 'Same as predictions
		End Function
	End Class

End Namespace