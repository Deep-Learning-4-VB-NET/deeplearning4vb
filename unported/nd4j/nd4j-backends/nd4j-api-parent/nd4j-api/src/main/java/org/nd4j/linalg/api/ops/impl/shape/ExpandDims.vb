Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JIllegalArgumentException = org.nd4j.linalg.exception.ND4JIllegalArgumentException
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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


	Public Class ExpandDims
		Inherits DynamicCustomOp

		Private jaxis As Integer


		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args As SDVariable, ByVal axis As Integer)
			Me.New(sameDiff, New SDVariable(){args}, axis)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal axis As Integer)
			MyBase.New(Nothing, sameDiff, args)
			If axis = Integer.MaxValue Then
				Throw New ND4JIllegalArgumentException("Cannot perform ExpandDims with axis == Integer.MAX_VALUE")
			End If
			Me.jaxis = axis
			addIArgument(Me.jaxis)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(Nothing, sameDiff, args)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(Nothing, inputs, outputs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, args, inPlace)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal axis As Integer)
			MyBase.New(New INDArray(){x}, Nothing)
			Me.jaxis = axis
			addIArgument(axis)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim targetNode As val = TFGraphMapper.getNodeWithNameFromGraph(graph, nodeDef.getInput(1))
			Dim dimArr As val = TFGraphMapper.getNDArrayFromTensor(targetNode)

			If dimArr IsNot Nothing Then
				Dim axis As Integer = dimArr.data().asInt()(0)
				Me.jaxis = axis
				addIArgument(Me.jaxis)
			Else
				Me.jaxis = Integer.MaxValue
				addIArgument(Me.jaxis)
			End If
		End Sub

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("axis") = axis
			Return ret
		End Function

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim axisMapping As val = PropertyMapping.builder().tfInputPosition(1).propertyNames(New String(){"axis"}).build()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			map("axis") = axisMapping

			ret(tensorflowName()) = map
			Return ret
		End Function

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

			If descriptor.getNumTArgs() >= 0 AndAlso numTArguments() <> descriptor.getNumTArgs() Then
				Throw New ND4JIllegalStateException("Op failure for " & opName() & " Number of inputs is invalid for execution. Specified " & numTArguments() & " but should be " & descriptor.getNumTArgs())
			End If

		End Sub

		Public Overrides Function opName() As String
			Return "expand_dims"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())

		End Function

		Public Overrides Function tensorflowName() As String
			Return "ExpandDims"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'Simply need a reshape to remove the dimension...
			Dim ret As SDVariable = sameDiff.squeeze(i_v(0), jaxis)
			Return New List(Of SDVariable) From {ret}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'Axis may be defined either as integer or as an array
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected list with 1 or 2 datatype for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as input type
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace