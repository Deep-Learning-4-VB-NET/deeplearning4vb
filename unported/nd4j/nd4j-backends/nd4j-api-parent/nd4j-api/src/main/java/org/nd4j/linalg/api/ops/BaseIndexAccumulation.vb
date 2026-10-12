Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public abstract class BaseIndexAccumulation extends BaseOp implements IndexAccumulation
	Public MustInherit Class BaseIndexAccumulation
		Inherits BaseOp
		Implements IndexAccumulation

		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()
		Public MustOverride ReadOnly Property KeepDims As Boolean Implements IndexAccumulation.isKeepDims
		Protected Friend keepDims As Boolean = False

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff,Nothing)
			If i_v IsNot Nothing Then
				Me.dimensions = dimensions
				sameDiff.addArgsFor(New SDVariable(){i_v},Me)

				Me.xVertexId = i_v.name()
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If
			Me.keepDims = keepDims
			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff,Nothing)
			If i_v IsNot Nothing Then
				Me.dimensions = dimensions
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v2, Me)
				Me.xVertexId = i_v.name()
				Me.yVertexId = i_v2.name()
				sameDiff.addArgsFor(New SDVariable(){i_v, i_v2},Me)
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If
			Me.keepDims = keepDims
			defineDimensions(dimensions)
		End Sub


		Public Sub New()
		End Sub


		Public Sub New(ByVal x As INDArray, ByVal dimensions() As Integer)
			Me.New(x, Nothing, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			Me.New(x, Nothing, dimensions)
			Me.keepDims = keepDims
			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal dimensions() As Integer)
			MyBase.New(x, z)
			defineDimensions(dimensions)
		End Sub


		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(Nothing)
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			If x Is Nothing Then
				Return Collections.emptyList()
			End If

			Dim reducedShape() As Long = Shape.getReducedShape(x.shape(), dimensions, keepDims)
			Return Collections.singletonList(LongShapeDescriptor.fromShape(reducedShape, DataType.LONG))
		End Function

		Public Overrides Function opType() As Type
			Return Type.INDEXREDUCE
		End Function

		Public Overridable Function validateDataTypes() As Boolean Implements IndexAccumulation.validateDataTypes

			If z() IsNot Nothing Then
				Preconditions.checkArgument(z().dataType() = DataType.LONG, "IndexReduce operations require LONG output: " & "got result array of type %s for op %s", z_Conflict.dataType(), Me.GetType())
			End If

			Return True
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'All index accumulation ops: always long output type
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
			Return Collections.singletonList(DataType.LONG)
		End Function
	End Class

End Namespace