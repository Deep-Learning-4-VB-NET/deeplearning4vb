Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
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
'ORIGINAL LINE: @Slf4j public abstract class BaseScalarOp extends BaseOp implements ScalarOp
	Public MustInherit Class BaseScalarOp
		Inherits BaseOp
		Implements ScalarOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()

		Public Sub New()
			Me.scalarValue = Nd4j.scalar(0.0f)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal num As Number)
			MyBase.New(x, y, z)
			If x.Compressed Then
				Nd4j.Compressor.decompressi(x)
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Me.scalarValue = Nd4j.scalar(x.dataType(), num)
			End Using
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal num As Number)
			MyBase.New(x)
			If x.Compressed Then
				Nd4j.Compressor.decompressi(x)
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Me.scalarValue = Nd4j.scalar(x.dataType(), num)
			End Using

		End Sub
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal set As Number)
			MyBase.New(x, Nothing, z)
			If x.Compressed Then
				Nd4j.Compressor.decompressi(x)
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Me.scalarValue = Nd4j.scalar(x.dataType(), set)
			End Using
		End Sub




		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number)
			Me.New(sameDiff,i_v,scalar,False,Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal inPlace As Boolean)
			Me.New(sameDiff,i_v,scalar,inPlace,Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseScalarOp(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable i_v, Number scalar, boolean inPlace, Object[] extraArgs)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			MyBase.New(sameDiff,inPlace,extraArgs)
			Me.scalarValue = Nd4j.scalar(i_v.dataType(), scalar)
			Me.xVertexId = i_v.name()
			sameDiff.addArgsFor(New String(){xVertexId},Me)
			SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
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

			Dim ret As val = New List(Of LongShapeDescriptor)(1)

			Dim s() As Long
			If x IsNot Nothing Then
				s = x.shape()
			Else
				s = arg().Shape
			End If

			Dim aT As val = arg().dataType()
			Dim sT As val = scalarValue.dataType()

			ret.add(LongShapeDescriptor.fromShape(s, Shape.pickPairwiseDataType(aT, sT)))
			Return ret
		End Function

		Public Overrides Function opType() As Type
			Return Type.SCALAR
		End Function

		Public Overridable WriteOnly Property Scalar Implements ScalarOp.setScalar As Number
			Set(ByVal scalar As Number)
				Me.scalarValue = Nd4j.scalar(x_Conflict.dataType(), scalar)
			End Set
		End Property

		Public Overridable WriteOnly Property Scalar Implements ScalarOp.setScalar As INDArray
			Set(ByVal scalar As INDArray)
				Me.scalarValue = scalar
			End Set
		End Property

		Public Overridable Function scalar() As INDArray Implements ScalarOp.scalar
			If y() IsNot Nothing AndAlso y().Scalar Then
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
			If y() IsNot Nothing Then
				If y().R OrElse x().R Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point:" & " x.dataType=%s, y.dataType=%s, z.dataType=%s, op=%s", x_Conflict.dataType(), y_Conflict.dataType(), z_Conflict.dataType(), Me.GetType().FullName)
				End If

				If Not experimentalMode Then
					Preconditions.checkArgument(x_Conflict.dataType() = y_Conflict.dataType() OrElse y_Conflict.dataType() = DataType.BOOL, "Op.X must have same data type as Op.Y")
				End If
			ElseIf x().R Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point:" & " x.dataType=%s, z.dataType=%s, op=%s", x_Conflict.dataType(), z_Conflict.dataType(), Me.GetType().FullName)
			End If


			Return True
		End Function

		Public Overridable ReadOnly Property OpType As Type Implements ScalarOp.getOpType
			Get
				Return Type.SCALAR
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'All scalar ops: output type is same as input type
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count >= 1, "Expected 1 or more input datatype %s, got input %s", Me.GetType(), dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace