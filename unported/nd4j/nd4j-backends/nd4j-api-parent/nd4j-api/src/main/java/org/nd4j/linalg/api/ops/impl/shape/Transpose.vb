Imports System.Collections.Generic
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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


	Public Class Transpose
		Inherits DynamicCustomOp

		Protected Friend permuteDims() As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){i_v})
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal permuteDims() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in]})
			Me.permuteDims = permuteDims
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal permuteDims As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in], permuteDims})
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal result As INDArray)
			MyBase.New(Nothing, New INDArray(){input},If(result Is Nothing, Nothing, New INDArray()){result}, Nothing, DirectCast(Nothing, IList(Of Integer)))
		End Sub

		Public Sub New(ByVal input As INDArray)
			Me.New(input, Nothing)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New LinkedHashMap(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New LinkedHashMap(Of String, PropertyMapping)()

			Dim mapping As val = PropertyMapping.builder().onnxAttrName("perm").propertyNames(New String(){"permuteDims"}).tfInputPosition(1).build()


			map("permuteDims") = mapping
			ret(tensorflowName()) = map
			ret(onnxName()) = map
			Return ret
		End Function


		Public Overrides Function opName() As String
			Return "transpose"
		End Function

		Public Overrides Function onnxName() As String
			Return "Transpose"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Transpose"
		End Function


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			'permute dimensions are not specified as second input
			If nodeDef.getInputCount() < 2 Then
				Return
			End If
			Dim permuteDimsNode As NodeDef = Nothing
			Dim i As Integer = 0
			Do While i < graph.getNodeCount()
				If graph.getNode(i).getName().Equals(nodeDef.getInput(1)) Then
					permuteDimsNode = graph.getNode(i)
				End If

				i += 1
			Loop

			Dim permuteArrayOp As INDArray = TFGraphMapper.getNDArrayFromTensor(permuteDimsNode)
			If permuteArrayOp IsNot Nothing Then
				Me.permuteDims = permuteArrayOp.data().asInt()
			End If

			'handle once properly mapped
			If arg().Shape Is Nothing OrElse arg().getVariableType() = VariableType.PLACEHOLDER OrElse arg().Arr Is Nothing Then
				Return
			End If

			Dim arr As INDArray = sameDiff.getArrForVarName(arg().name())

			If permuteArrayOp IsNot Nothing Then
				addInputArgument(arr, permuteArrayOp)
			Else
				addInputArgument(arr)
			End If

			If arr IsNot Nothing AndAlso permuteDims Is Nothing Then
				Me.permuteDims = ArrayUtil.reverseCopy(ArrayUtil.range(0, arr.rank()))
			End If

			If permuteDims IsNot Nothing AndAlso permuteDims.Length < arg().Shape.Length Then
				Throw New ND4JIllegalStateException("Illegal permute found. Not all dimensions specified")
			End If
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			If Not attributesForNode.ContainsKey("perm") Then

			Else
				Me.permuteDims = Ints.toArray(attributesForNode("perm").getIntsList())
			End If
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable
			If permuteDims Is Nothing Then
				ret = sameDiff.transpose(i_v(0))
			Else
				Dim reverse() As Integer = ArrayUtil.invertPermutation(permuteDims)
				ret = sameDiff.permute(i_v(0), reverse)
			End If
			Return Collections.singletonList(ret)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of org.nd4j.linalg.api.buffer.DataType)) As IList(Of org.nd4j.linalg.api.buffer.DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected list with 1 or 2 datatype for %s, got %s", Me.GetType(), dataTypes)
			If dArguments IsNot Nothing AndAlso dArguments.Count > 0 Then
				Return Collections.singletonList(dArguments(0))
			End If
			'Output type is same as input type. Second input is permute dimensions as array
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace