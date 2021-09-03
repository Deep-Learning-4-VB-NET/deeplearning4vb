Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseScalarBoolOp extends BaseOp implements ScalarOp
	Public MustInherit Class BaseScalarBoolOp
		Inherits BaseOp
		Implements ScalarOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()
		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal num As Number)
			MyBase.New(x, y, z)
			Me.scalarValue = Nd4j.scalar(x.dataType(), num)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal num As Number)
			MyBase.New(x)
			Me.scalarValue = Nd4j.scalar(x.dataType(), num)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal set As Number)
			MyBase.New(x, Nothing, z)
			Me.scalarValue= Nd4j.scalar(x.dataType(), set)
		End Sub




		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number)
			Me.New(sameDiff,i_v,scalar,False,Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal inPlace As Boolean)
			Me.New(sameDiff,i_v,scalar,inPlace,Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			MyBase.New(sameDiff,inPlace,extraArgs)
			Me.scalarValue = Nd4j.scalar(i_v.dataType(), scalar)
			If i_v IsNot Nothing Then
				Me.xVertexId = i_v.name()
				sameDiff.addArgsFor(New String(){xVertexId},Me)
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If

		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal extraArgs() As Object)
			Me.New(sameDiff,i_v,scalar,False,extraArgs)
		End Sub



		Public Overrides Function z() As INDArray Implements org.nd4j.linalg.api.ops.Op.z
			Return z_Conflict
		End Function


		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(Nothing)
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
			If x Is Nothing Then
				Return Collections.emptyList()
			End If

			'Calculate reduction shape. Note that reduction on scalar - returns a scalar
			Return Collections.singletonList(LongShapeDescriptor.fromShape(x.shape(), DataType.BOOL))
		End Function

		Public Overrides Function opType() As Type
			Return Type.SCALAR_BOOL
		End Function

		Public Overridable WriteOnly Property Scalar Implements ScalarOp.setScalar As Number
			Set(ByVal scalar As Number)
				Me.scalarValue = Nd4j.scalar(scalar)
			End Set
		End Property

		Public Overridable WriteOnly Property Scalar Implements ScalarOp.setScalar As INDArray
			Set(ByVal scalar As INDArray)
				Me.scalarValue = scalar
			End Set
		End Property

		Public Overridable Function scalar() As INDArray Implements ScalarOp.scalar
			If scalarValue Is Nothing AndAlso y() IsNot Nothing AndAlso y().Scalar Then
				Return y()
			End If
			Return scalarValue
		End Function


		Public Overridable Property Dimension As Integer() Implements ScalarOp.getDimension
			Get
				Return dimensions
			End Get
			Set(ByVal dimension() As Integer)
				defineDimensions(dimension)
			End Set
		End Property


		Public Overridable Function validateDataTypes(ByVal experimentalMode As Boolean) As Boolean Implements ScalarOp.validateDataTypes
			Preconditions.checkArgument(z().B, "Op.Z must have floating point type, since one of operands is floating point." & " op.z.datatype=" & z().dataType())

			Return True
		End Function

		Public Overridable ReadOnly Property OpType As Type Implements ScalarOp.getOpType
			Get
				Return Type.SCALAR_BOOL
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'All scalar bool ops: output type is always bool
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
			Return Collections.singletonList(DataType.BOOL)
		End Function
	End Class

End Namespace