Imports System.Collections.Generic
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports BooleanAdapter = org.nd4j.imports.descriptors.properties.adapters.BooleanAdapter
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports CumSumBp = org.nd4j.linalg.api.ops.impl.reduce.bp.CumSumBp
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


	Public Class CumSum
		Inherits DynamicCustomOp

		Protected Friend exclusive As Boolean = False
		Protected Friend reverse As Boolean = False
		Protected Friend jaxis(-1) As Integer

		Public Sub New()
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ParamArray ByVal axis() As Integer)
			Me.New(sameDiff, x, False, False, axis)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal exclusive As Boolean, ByVal reverse As Boolean, ParamArray ByVal axis() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){x})
			Me.sameDiff = sameDiff
			Me.exclusive = exclusive
			Me.reverse = reverse
			Me.jaxis = axis
			addArgs()
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal result As INDArray, ByVal exclusive As Boolean, ByVal reverse As Boolean, ParamArray ByVal axis() As Integer)
			MyBase.New(Nothing, New INDArray(){[in]}, wrapOrNull(result), Nothing, DirectCast(Nothing, IList(Of Integer)))
			Me.exclusive = exclusive
			Me.reverse = reverse
			Me.jaxis = axis
			addArgs()
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal exclusive As Boolean, ByVal reverse As Boolean, ParamArray ByVal axis() As Integer)
			Me.New([in], Nothing, exclusive, reverse, axis)
		End Sub

		Public Overrides Function opName() As String
			Return "cumsum"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Cumsum"
		End Function

		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Dim ret As IDictionary(Of String, IDictionary(Of String, AttributeAdapter)) = New Dictionary(Of String, IDictionary(Of String, AttributeAdapter))()
			Dim tfMappings As IDictionary(Of String, AttributeAdapter) = New LinkedHashMap(Of String, AttributeAdapter)()

			tfMappings("exclusive") = New BooleanAdapter()
			tfMappings("reverse") = New BooleanAdapter()


			ret(tensorflowName()) = tfMappings

			Return ret
		End Function

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim exclusiveMapper As val = PropertyMapping.builder().tfAttrName("exclusive").propertyNames(New String(){"exclusive"}).build()

			Dim reverseMapper As val = PropertyMapping.builder().tfAttrName("reverse").propertyNames(New String(){"reverse"}).build()


			map("exclusive") = exclusiveMapper
			map("reverse") = reverseMapper

			ret(tensorflowName()) = map

			Return ret
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Protected Friend Overridable Sub addArgs()
			addIArgument(If(exclusive, 1, 0),If(reverse, 1, 0))
			For Each a As val In jaxis
				addIArgument(jaxis)
			Next a
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New CumSumBp(sameDiff, arg(0), grad(0), exclusive, reverse, jaxis)).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or 2 input datatype for %s, got %s", Me.GetType(), dataTypes) '2nd optional input - axis
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace