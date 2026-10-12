Imports System.Collections.Generic
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.api.ops


	Public Class NoOp
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable)
			MyBase.New("noop", sd, New SDVariable(){[in]})
		End Sub

		Public Sub New(ByVal [in] As INDArray)
			addInputArgument([in])
		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(f1(0))
		End Function



		Public Overrides Function opName() As String
			Return "noop"
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub

		Public Overrides Function onnxName() As String
			Return "NoOp"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "NoOp"
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return 1
			End Get
		End Property


		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			If inputArguments_Conflict IsNot Nothing AndAlso inputArguments_Conflict.Count > 0 Then
				Return Collections.singletonList(inputArguments(0).shapeDescriptor())
			End If
			Return Collections.singletonList(Nd4j.empty(DataType.BOOL).shapeDescriptor())
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			If oc.getInputArrays() IsNot Nothing AndAlso oc.getInputArrays().Count > 0 Then
				Return Collections.singletonList(oc.getInputArray(0).shapeDescriptor())
			End If
			Return Collections.singletonList(Nd4j.empty(DataType.BOOL).shapeDescriptor())
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(DataType.BOOL)
		End Function
	End Class

End Namespace