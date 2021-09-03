Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.linalg.api.ops.impl.loss.bp



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class SoftmaxCrossEntropyLossBp extends BaseLossBp
	Public Class SoftmaxCrossEntropyLossBp
		Inherits BaseLossBp

		Private labelSmoothing As Double = 0.0

		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal logits As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable, ByVal labelSmoothing As Double)
			MyBase.New(sameDiff, lossReduce, logits, weights, labels)
			Me.labelSmoothing = labelSmoothing
			addArgs()
		End Sub


		Public Overrides Sub addArgs()
			MyBase.addArgs()
			addTArgument(labelSmoothing)
		End Sub
		Public Overrides Function opName() As String
			Return "softmax_cross_entropy_loss_grad"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 2 OrElse inputDataTypes.Count = 3), "Expected 2 or 3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)

			Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(1), inputDataTypes(2)} 'Same as predictions
		End Function
	End Class

End Namespace