Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor

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


	Public MustInherit Class BaseTransformSameOp
		Inherits BaseTransformOp
		Implements TransformSameOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, i_v1, i_v2)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v1, i_v2, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff)
			MyBase.New(sameDiff)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v1, i_v2, extraArgs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal shape() As Long, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v, shape, inPlace, extraArgs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v, extraArgs)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(x, z)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			MyBase.New(x,y,z)
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub


		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub

		Public Overrides ReadOnly Property OpType As Type Implements org.nd4j.linalg.api.ops.TransformOp.getOpType
			Get
				Return Type.TRANSFORM_SAME
			End Get
		End Property

		Public Overrides Function opType() As Type
			Return Type.TRANSFORM_SAME
		End Function

		Public Overrides Function resultType() As DataType Implements org.nd4j.linalg.api.ops.TransformOp.resultType
			Return Me.x().dataType()
		End Function

		Public Overrides Function resultType(ByVal oc As OpContext) As DataType Implements org.nd4j.linalg.api.ops.TransformOp.resultType
			Return oc.getInputArray(0).dataType()
		End Function

		Public Overrides Function validateDataTypes(ByVal oc As OpContext, ByVal experimentalMode As Boolean) As Boolean Implements org.nd4j.linalg.api.ops.TransformOp.validateDataTypes
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
			Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())
			If y IsNot Nothing Then
				Preconditions.checkArgument(x.dataType() = y.dataType(), "Op.X type must be the same as Op.Y type: x.datatype=%s, y.datatype=%s for op %s", x.dataType(), y.dataType(), Me.GetType())
			End If


			If z IsNot Nothing Then
				Preconditions.checkArgument(z.dataType() = x.dataType(), "Op.Z must be the same as Op.X type: x.datatype=%s, z.datatype=%s for op %s", x.dataType(), z.dataType(), Me.GetType())
			End If

			Return True
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(Nothing)
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			If x Is Nothing Then
				Return Collections.emptyList()
			End If

			Return Collections.singletonList(LongShapeDescriptor.fromShape(x.shape(), x.dataType()))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'All same transform ops: always same output type as input type
			Preconditions.checkState(dataTypes IsNot Nothing, "Expected exactly 1 or more input datatype for %s, got input %s", Me.GetType(), dataTypes)

			Dim check As DataType = Nothing
			For Each dataType As DataType In dataTypes
				If check <> Nothing Then
					Preconditions.checkState(dataType = check,"Data types must all be the same!")
				Else
					check = dataType
				End If
			Next dataType
			Return New List(Of DataType) From {dataTypes(0)}
		End Function
	End Class

End Namespace