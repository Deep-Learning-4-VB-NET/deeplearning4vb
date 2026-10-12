Imports System.Collections.Generic
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataTypeAdapter = org.nd4j.imports.descriptors.properties.adapters.DataTypeAdapter
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom



	''' <summary>
	''' Fill an array of given "shape" with the provided "value", e.g.
	''' shape [2, 2] and value 42 returns [[42, 42], [42, 42]].
	''' 
	''' @author Max Pumperla
	''' </summary>
	Public Class Fill
		Inherits DynamicCustomOp

		Private value As Double
		Private dtype As DataType

		Public Sub New()
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal dtype As DataType, ByVal value As Double)
			MyBase.New(Nothing,sameDiff, New SDVariable() {shape}, False)
			Me.value = value
			Me.dtype = dtype
			addArgs()
		End Sub

		Public Sub New(ByVal shape As INDArray, ByVal dtype As DataType, ByVal value As Double)
			MyBase.New(New INDArray(){shape, Nd4j.scalar(dtype, value)}, Nothing)
			Me.value = value
			Me.dtype = dtype
		End Sub

		Public Sub New(ByVal shape As INDArray, ByVal result As INDArray, ByVal value As Double)
			MyBase.New(Nothing, shape, result, Collections.singletonList(value), Nothing)
			Me.value = value
		End Sub

		Public Sub New(ByVal shape As INDArray, ByVal value As INDArray, ByVal result As INDArray)
			MyBase.New(Nothing, New INDArray(){shape, value}, New INDArray(){result})
		End Sub


		Protected Friend Overridable Sub addArgs()
			addTArgument(value)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim dt As org.tensorflow.framework.DataType = attributesForNode("T").getType()
			Me.dtype = DataTypeAdapter.dtypeConv(dt)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Sub assertValidForExecution()
			Dim descriptor As val = Me.Descriptor
			If descriptor.getNumInputs() > 0 AndAlso numInputArguments() > 2 OrElse numInputArguments() < 1 Then
				Throw New ND4JIllegalStateException("Op failure for " & opName() & " Number of inputs is invalid for execution. Specified " & numInputArguments() & " but should be " & descriptor.getNumInputs())
			End If

			If descriptor.getNumOutputs() > 0 AndAlso numOutputArguments() <> descriptor.getNumOutputs() Then
				Throw New ND4JIllegalStateException("Op failure for " & opName() & " Number of outputs is invalid for execution. Specified " & numOutputArguments() & " but should be " & descriptor.getNumInputs())
			End If

			'< 0 means dynamic size
			If descriptor.getNumIArgs() >= 0 AndAlso numIArguments() <> descriptor.getNumIArgs() Then
				Throw New ND4JIllegalStateException("Op failure for " & opName() & " Number of integer arguments is invalid for execution. Specified " & numIArguments() & " but should be " & descriptor.getNumIArgs())
			End If

			If descriptor.getNumTArgs() >= 0 AndAlso numTArguments() < 1 Then
				Throw New ND4JIllegalStateException("Op failure for " & opName() & " Number of inputs is invalid for execution. Specified " & numTArguments() & " but should be " & descriptor.getNumTArgs())
			End If

		End Sub

		Public Overrides Function opName() As String
			Return "fill"
		End Function

		Public Overrides Function onnxName() As String
			Return "ConstantFill"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Fill"
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			If dArguments.Count > 0 Then
				Return Collections.singletonList(dArguments(0))
			End If
			'1 or 2 possible: 2 for TF import (fill with specified value
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkNotNull(dtype, "Output datatype was null (not set)")
			Return Collections.singletonList(dtype)
		End Function
	End Class

End Namespace