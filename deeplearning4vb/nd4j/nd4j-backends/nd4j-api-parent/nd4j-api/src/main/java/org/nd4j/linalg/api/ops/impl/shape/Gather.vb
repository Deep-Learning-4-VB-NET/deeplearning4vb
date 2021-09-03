Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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


	''' <summary>
	''' Gather op
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class Gather extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Gather
		Inherits DynamicCustomOp

		Protected Friend indices() As Integer
		Protected Friend jaxis As Integer = 0

		Public Sub New(ByVal sameDiff As SameDiff, ByVal df As SDVariable, ByVal indices As SDVariable, ByVal axis As Integer)
			Me.New(sameDiff, df, indices, axis, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal df As SDVariable, ByVal indices() As Integer, ByVal axis As Integer)
			Me.New(sameDiff, df, indices, axis, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal indices() As Integer, ByVal axis As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, inPlace)

			addIArgument(axis)
			addIArgument(indices)
			Me.jaxis = axis
			Me.indices = indices
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal indices As SDVariable, ByVal axis As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, indices}, inPlace)
			addIArgument(axis)
			Me.jaxis = axis
		End Sub

		Public Sub New(ByVal df As INDArray, ByVal indexes() As Integer, ByVal axis As Integer)
			addInputArgument(df)
			addIArgument(axis)
			addIArgument(indexes)
			Me.jaxis = axis
			Me.indices = indices
		End Sub

		Public Sub New(ByVal df As INDArray, ByVal indexes As INDArray, ByVal axis As Integer)
			addInputArgument(df, indexes)
			addIArgument(axis)
			Me.jaxis = axis
			Me.indices = indices
		End Sub

		Public Overrides Function onnxName() As String
			Return "Gather"
		End Function


		Public Overrides Function tensorflowNames() As String()
			Return New String(){"Gather", "GatherV2"}
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			Dim broadcast As val = PropertyMapping.builder().onnxAttrName("indices").tfInputPosition(1).propertyNames(New String(){"indices"}).build()

			map("indices") = broadcast

			ret(tensorflowNames()(0)) = map
			ret(onnxName()) = map

			Dim map2 As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			Dim broadcast2 As val = PropertyMapping.builder().tfInputPosition(1).propertyNames(New String(){"indices"}).build()
			map2("indices") = broadcast2

			Dim axis2 As val = PropertyMapping.builder().tfInputPosition(2).propertyNames(New String(){"axis"}).build()
			map2("axis") = axis2

			ret("GatherV2") = map2


			Return ret
		End Function

		Public Overrides Function opName() As String
			Return "gather"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'2 args: input and indices. Plus integer dimension arg
			'Gather backprop is just scatter add

			Dim indicesGrad As SDVariable = sameDiff.zerosLike(arg(1))
			Dim inputGrad As SDVariable = sameDiff.zerosLike(arg(0))

			Dim inputs() As SDVariable = args()
			Dim axis As SDVariable
			Dim rank As SDVariable = inputs(0).rank()
			If inputs.Length = 2 Then
				axis = sameDiff.constant(jaxis)
				If jaxis < 0 Then
					axis = axis.add(rank)
				End If
			Else
				axis = inputs(2)
			End If

			'Use scatter add plus permute
			Dim dimsExAxis As SDVariable = sameDiff.range(Nothing, sameDiff.constant(0), rank, sameDiff.constant(1), DataType.INT)
			Dim axisRank1 As SDVariable = axis.reshape(1)
			dimsExAxis = sameDiff.math().listDiff(dimsExAxis, axisRank1)(0) 'Don't need indices
			Dim permuteDims As SDVariable = sameDiff.concat(0, axisRank1, dimsExAxis)
			Dim invertDims As SDVariable = sameDiff.invertPermutation(permuteDims)


			'Permute gradients so original axis is at position 0... then scatter add, and reverse
			Dim gradAtOut As SDVariable = i_v(0)
			Dim permuteGrad As SDVariable = gradAtOut.permute(permuteDims)
			Dim inputGradPermute As SDVariable = inputGrad.permute(permuteDims)
			inputGrad = sameDiff.scatterAdd(inputGradPermute, arg(1), permuteGrad)

			'Now, invert the permutation so axis is back where it was
			inputGrad = inputGrad.permute(invertDims)

			Return New List(Of SDVariable) From {inputGrad, indicesGrad}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'Output type is same as (first) input type
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace