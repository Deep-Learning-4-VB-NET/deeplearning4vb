Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports SparseSoftmaxCrossEntropyLossWithLogitsBp = org.nd4j.linalg.api.ops.impl.loss.bp.SparseSoftmaxCrossEntropyLossWithLogitsBp
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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
'ORIGINAL LINE: @NoArgsConstructor public class SparseSoftmaxCrossEntropyLossWithLogits extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class SparseSoftmaxCrossEntropyLossWithLogits
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SparseSoftmaxCrossEntropyLossWithLogits(@NonNull SameDiff sameDiff, @NonNull SDVariable logits, @NonNull SDVariable labels)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal logits As SDVariable, ByVal labels As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){labels, logits}, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SparseSoftmaxCrossEntropyLossWithLogits(@NonNull INDArray logits, @NonNull INDArray labels)
		Public Sub New(ByVal logits As INDArray, ByVal labels As INDArray)
			MyBase.New(New INDArray(){labels, logits}, Nothing)
		End Sub

		Public Overridable Sub addArgs()
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)

			'Switch order: TF uses [logits, labels]; libnd4j expects [labels, logits]
			Dim op As SameDiffOp = initWith.getOps().get(Me.getOwnName())
			Dim list As IList(Of String) = op.getInputsToOp()
			Dim newList As IList(Of String) = New List(Of String) From {list(1), list(0)}
			op.InputsToOp = newList
		End Sub

		Public Overrides Function opName() As String
			Return "sparse_softmax_cross_entropy_loss_with_logits"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "SparseSoftmaxCrossEntropyWithLogits"
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			If dArguments IsNot Nothing AndAlso dArguments.Count > 0 Then
				Return New List(Of DataType) From {dArguments(0)}
			End If
			Return Collections.singletonList(inputDataTypes(1)) 'Same as predictions (logits)
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'args: label, logits
			Dim labelsGrad As SDVariable = sameDiff.zerosLike(arg(0))
			Dim logitsGrad As SDVariable = (New SparseSoftmaxCrossEntropyLossWithLogitsBp(sameDiff, arg(1), arg(0))).outputVariable()
			Return New List(Of SDVariable) From {labelsGrad, logitsGrad}
		End Function
	End Class

End Namespace