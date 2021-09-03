Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports SoftmaxCrossEntropyWithLogitsLossBp = org.nd4j.linalg.api.ops.impl.loss.bp.SoftmaxCrossEntropyWithLogitsLossBp

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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class SoftmaxCrossEntropyWithLogitsLoss extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class SoftmaxCrossEntropyWithLogitsLoss
		Inherits DynamicCustomOp

		Protected Friend classesDim As Integer

	'    public SoftmaxCrossEntropyWithLogitsLoss(SameDiff sameDiff, SDVariable logits, SDVariable weights, SDVariable labels, int classesDim) {
	'        super(null, sameDiff, new SDVariable[]{logits, weights, labels}, false);
	'        this.classesDim = classesDim;
	'        addIArgument(classesDim);
	'    }

		Public Sub New(ByVal sameDiff As SameDiff, ByVal logits As SDVariable, ByVal labels As SDVariable, ByVal classesDim As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){logits, labels}, False)
			Me.classesDim = classesDim
			addIArgument(classesDim)
		End Sub

		Public Overrides Function opName() As String
			Return "softmax_cross_entropy_loss_with_logits"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "SoftmaxCrossEntropyWithLogits"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 2 OrElse inputDataTypes.Count = 3), "Expected 2 or 3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)

			Return Collections.singletonList(inputDataTypes(0)) 'Same as predictions
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args: logits, weigths, label
			Return (New SoftmaxCrossEntropyWithLogitsLossBp(sameDiff, arg(0), arg(1), classesDim)).outputs()
		End Function
	End Class

End Namespace