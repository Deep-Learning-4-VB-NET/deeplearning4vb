Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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


	Public Class ShapeN
		Inherits DynamicCustomOp

		Protected Friend dataType As DataType

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, inputs, inPlace)
		End Sub

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
		End Function


		Public Overrides Function opName() As String
			Return "shapes_of"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "ShapeN"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim [out] As IList(Of SDVariable) = New List(Of SDVariable)()
			For Each [in] As SDVariable In args()
				[out].Add(sameDiff.zerosLike([in]))
			Next [in]
			Return [out]
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return args().Length
			End Get
		End Property

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			dataType = TFGraphMapper.convertType(nodeDef.getAttrOrThrow("out_type").getType())
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'Output type is always long (i.e., shape of array) - for each input
			'TODO TF allows customizing int or long
			Dim n As Integer = NumOutputs
			Dim outputTypes As IList(Of DataType) = New List(Of DataType)(n)
			For i As Integer = 0 To n - 1
				outputTypes.Add(If(dataType = Nothing, DataType.LONG, dataType))
			Next i
			Return outputTypes
		End Function
	End Class

End Namespace