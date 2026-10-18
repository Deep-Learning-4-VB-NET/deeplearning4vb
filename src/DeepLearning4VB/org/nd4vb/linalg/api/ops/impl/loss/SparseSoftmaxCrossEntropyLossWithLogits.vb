Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SparseSoftmaxCrossEntropyLossWithLogitsBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.loss.bp.SparseSoftmaxCrossEntropyLossWithLogitsBp
#End If
#If MODEL_IMPORT Then
Imports SameDiffOp = DeepLearning4VB.org.nd4vb.autodiff.samediff.internal.SameDiffOp
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.loss

    ''' <summary>The custom op <c>sparse_softmax_cross_entropy_loss_with_logits</c> (Java <c>org.nd4j.linalg.api.ops.impl.loss.SparseSoftmaxCrossEntropyLossWithLogits</c>).</summary>
    Public Class SparseSoftmaxCrossEntropyLossWithLogits
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SparseSoftmaxCrossEntropyLossWithLogits(@NonNull SameDiff sameDiff, @NonNull SDVariable logits, @NonNull SDVariable labels)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal logits As SDVariable, ByVal labels As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){labels, logits}, False)
		End Sub
#End If

        Public Sub New(ByVal logits As INDArray, ByVal labels As INDArray)
            MyBase.New(New INDArray(){labels, logits}, Nothing)
        End Sub

        Public Overridable Sub addArgs()
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)

			'Switch order: TF uses [logits, labels]; libnd4j expects [labels, logits]
			Dim op As SameDiffOp = initWith.getOps().get(Me.getOwnName())
			Dim list As IList(Of String) = op.getInputsToOp()
			Dim newList As IList(Of String) = New List(Of String) From {list(1), list(0)}
			op.InputsToOp = newList
		End Sub
#End If

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
            Return JavaCollections.singletonList(inputDataTypes(1)) 'Same as predictions (logits)
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'args: label, logits
			Dim labelsGrad As SDVariable = sameDiff.zerosLike(arg(0))
			Dim logitsGrad As SDVariable = (New SparseSoftmaxCrossEntropyLossWithLogitsBp(sameDiff, arg(1), arg(0))).outputVariable()
			Return New List(Of SDVariable) From {labelsGrad, logitsGrad}
		End Function
#End If

    End Class

End Namespace
