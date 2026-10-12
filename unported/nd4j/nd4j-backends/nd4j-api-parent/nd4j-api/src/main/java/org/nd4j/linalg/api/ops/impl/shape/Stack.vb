Imports System.Collections.Generic
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
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


	Public Class Stack
		Inherits DynamicCustomOp

		Protected Friend jaxis As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal output As INDArray, ByVal axis As Integer)
			MyBase.New(Nothing, inputs,If(output Is Nothing, Nothing, New INDArray()){output}, Nothing, DirectCast(Nothing, IList(Of Integer)))
			Me.jaxis = axis
			addArgs()
		End Sub

		Public Sub New(ByVal input() As INDArray, ByVal axis As Integer)
			addInputArgument(input)
			Me.jaxis = axis
			addArgs()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal values As SDVariable, ByVal axis As Integer)
			Me.New(sameDiff, New SDVariable(){values}, axis)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal values() As SDVariable, ByVal axis As Integer)
			MyBase.New(Nothing, sameDiff, values, False)
			Me.jaxis = axis
			addArgs()
		End Sub

		Public Overridable Sub addArgs()
			addIArgument(jaxis)
		End Sub

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "stack"
		End Function


		Public Overrides Function ToString() As String
			Return opName()
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"Pack", "Stack"}
		End Function

		Public Overrides Function opName() As String
			Return "stack"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Throw New System.NotSupportedException("No analog found for onnx for " & opName())
		End Sub


		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim axisMapping As val = PropertyMapping.builder().onnxAttrName("axis").tfAttrName("axis").propertyNames(New String(){"jaxis"}).build()

			map("jaxis") = axisMapping

			For Each name As val In tensorflowNames()
				ret(name) = map
			Next name

			Return ret
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.unstack(f1(0), jaxis, args().Length)}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim first As DataType = dataTypes(0)
			For i As Integer = 1 To dataTypes.Count - 1
				Dim dt As DataType = dataTypes(i)
				Preconditions.checkState(first = dt, "All inputs must have same datatype - got %s and %s for inputs 0 and %s respectively", first, dt, i)
			Next i
			'Output type is same as input types
			Return Collections.singletonList(first)
		End Function
	End Class

End Namespace