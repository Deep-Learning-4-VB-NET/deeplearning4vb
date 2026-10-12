Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports SRUWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.SRUWeights
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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class SRU extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class SRU
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.SRUWeights weights;
		Private weights As SRUWeights

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.autodiff.samediff.SDVariable mask;
		Private mask As SDVariable

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SRU(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable initialC, org.nd4j.autodiff.samediff.SDVariable mask, @NonNull SRUWeights weights)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal initialC As SDVariable, ByVal mask As SDVariable, ByVal weights As SRUWeights)
			MyBase.New(Nothing, sameDiff, wrapFilterNull(x, weights.getWeights(), weights.getBias(), initialC, mask))
			Me.mask = mask
			Me.weights = weights
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal initialC As INDArray, ByVal mask As INDArray, ByVal sruWeights As SRUWeights)
			MyBase.New(wrapFilterNull(x, sruWeights.getIWeights(), sruWeights.getIBias(), initialC, mask), Nothing)
			Me.mask = DirectCast(mask, SDVariable)
			Me.weights = sruWeights
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal initialC As INDArray, ByVal sruWeights As SRUWeights)
			MyBase.New(wrapFilterNull(x, sruWeights.getIWeights(), sruWeights.getIBias(), initialC), Nothing)
			Me.weights = sruWeights
		End Sub

		Public Overrides Function opName() As String
			Return "sru"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op name for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op name for " & opName())
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub
	End Class

End Namespace