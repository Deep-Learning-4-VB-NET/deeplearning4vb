Imports System.Collections.Generic
Imports Onnx = onnx.Onnx
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.api.ops.impl.shape.tensorops


	Public Class TensorArrayRead
		Inherits BaseTensorOp

		Protected Friend importDataType As DataType

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(name, sameDiff, args)
		End Sub
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(Nothing, sameDiff, args)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"TensorArrayRead", "TensorArrayReadV2", "TensorArrayReadV3"}
		End Function


		Public Overrides Function opName() As String
			Return "read_list"
		End Function

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)

			Me.importDataType = TFGraphMapper.convertType(attributesForNode("dtype").getType())
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataType As IList(Of DataType)) As IList(Of DataType)
			'Same output type as the TensorArray - which is defined by input 0
			Dim dt As DataType = Nothing
			If importDataType <> Nothing Then
				dt = importDataType
			Else
				Dim i As Integer = 0
				Do While i < args().Length
					Dim tArr As SDVariable = arg(i)
					Dim op As DifferentialFunction = sameDiff.getVariableOutputOp(tArr.name())
					If TypeOf op Is TensorArray Then
						Dim t3 As TensorArray = DirectCast(op, TensorArray)
						dt = t3.getTensorArrayDataType()
						Exit Do
					End If

					i += 1
				Loop

			End If
			Return Collections.singletonList(dt)
		End Function
	End Class

End Namespace