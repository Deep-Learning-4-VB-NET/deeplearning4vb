Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports SoftmaxCrossEntropyLossBp = org.nd4j.linalg.api.ops.impl.loss.bp.SoftmaxCrossEntropyLossBp
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
'ORIGINAL LINE: @NoArgsConstructor public class SoftmaxCrossEntropyLoss extends BaseLoss
	Public Class SoftmaxCrossEntropyLoss
		Inherits BaseLoss

		Public Const DEFAULT_LABEL_SMOOTHING As Double = 0.0

		Private labelSmoothing As Double = 0.0

		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal logits As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double)
			Me.New(sameDiff, lossReduce, logits, weights, labels, labelSmoothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal logits As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable, ByVal labelSmoothing As Double)
			MyBase.New(sameDiff, lossReduce, logits, weights, labels)
			Me.labelSmoothing = labelSmoothing

			addArgs()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal logits As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable)
			Me.New(sameDiff, lossReduce, logits, weights, labels, 0.0)
		End Sub

		Public Sub New(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal weights As INDArray, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double)
			MyBase.New(lossReduce, predictions, weights, labels)
			Me.labelSmoothing = labelSmoothing
			addArgs()
		End Sub

		Public Overrides Sub addArgs()
			MyBase.addArgs()
			addTArgument(labelSmoothing)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Function opName() As String
			Return "softmax_cross_entropy_loss"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "SoftmaxCrossEntropy"
		End Function


		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args are: predictions, weights, label
			Return (New SoftmaxCrossEntropyLossBp(sameDiff, lossReduce, arg(0), arg(1), arg(2), labelSmoothing)).outputs()
		End Function
	End Class

End Namespace