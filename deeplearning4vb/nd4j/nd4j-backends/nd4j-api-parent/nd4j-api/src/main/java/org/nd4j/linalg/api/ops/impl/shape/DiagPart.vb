Imports System.Collections.Generic
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.impl.shape



	Public Class DiagPart
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, args, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable)
			Me.New(sameDiff, New SDVariable(){[in]}, False)
		End Sub

		Public Sub New(ByVal [in] As INDArray)
			Me.New([in], Nothing)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray)
			MyBase.New(Nothing, [in], [out], Nothing, Nothing)
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim grad As SDVariable = i_v(0)
			Dim ret As SDVariable = sameDiff.math().diag(grad)
			Return Collections.singletonList(ret)
		End Function

		Public Overrides Function opName() As String
			Return "diag_part"
		End Function


		Public Overrides Function tensorflowName() As String
			Return "DiagPart"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of org.nd4j.linalg.api.buffer.DataType)) As IList(Of org.nd4j.linalg.api.buffer.DataType)
			Preconditions.checkState(dataTypes.Count = 1, "Expected list with exactly 1 datatype for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as input type
			Return dataTypes
		End Function

	End Class

End Namespace