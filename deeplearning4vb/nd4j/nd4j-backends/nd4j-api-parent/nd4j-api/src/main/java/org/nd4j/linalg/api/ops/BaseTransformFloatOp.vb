Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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


	Public MustInherit Class BaseTransformFloatOp
		Inherits BaseTransformOp
		Implements TransformFloatOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()

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

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub

		Public Overrides ReadOnly Property OpType As Type Implements org.nd4j.linalg.api.ops.TransformOp.getOpType
			Get
				Return Type.TRANSFORM_FLOAT
			End Get
		End Property

		Public Overrides Function opType() As Type
			Return Type.TRANSFORM_FLOAT
		End Function

		Public Overrides Function resultType() As DataType Implements org.nd4j.linalg.api.ops.TransformOp.resultType
			If Me.x() IsNot Nothing AndAlso Me.x().R Then
				Return Me.x().dataType()
			End If

			Return Nd4j.defaultFloatingPointType()
		End Function

		Public Overrides Function resultType(ByVal oc As OpContext) As DataType Implements org.nd4j.linalg.api.ops.TransformOp.resultType
			If oc.getInputArray(0) IsNot Nothing AndAlso oc.getInputArray(0).R Then
				Return oc.getInputArray(0).dataType()
			End If

			Return Nd4j.defaultFloatingPointType()
		End Function

		Public Overrides Function validateDataTypes(ByVal oc As OpContext, ByVal experimentalMode As Boolean) As Boolean Implements org.nd4j.linalg.api.ops.TransformOp.validateDataTypes
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
			Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())

			If y IsNot Nothing AndAlso Not experimentalMode Then
				Preconditions.checkArgument(x.dataType() = y.dataType(), "Op.X must have same data type as Op.Y")
			End If

			If z IsNot Nothing Then
				Preconditions.checkArgument(z.R,"Op.Z must be one of floating types: z.datatype=%s for op %s", z.dataType(), Me.GetType())
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
			Return Collections.singletonList(LongShapeDescriptor.fromShape(x.shape(),If(x.R, x.dataType(), Nd4j.defaultFloatingPointType())))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
			If dataTypes(0).isFPType() Then
				Return Collections.singletonList(dataTypes(0))
			End If
			'TODO is this what we want for all cases?
			Return Collections.singletonList(DataType.FLOAT)
		End Function
	End Class

End Namespace