Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' The base class of the bool reductions (all, any, has NaN, has infinity): z is always <c>BOOL</c>
    ''' (<c>REDUCE_BOOL</c>). <see cref="emptyValue"/> is the result of reducing an empty array.
    ''' </summary>
    Public MustInherit Class BaseReduceBoolOp
        Inherits BaseReduceOp
        Implements ReduceBoolOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, i_v, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, input, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, i_v)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, input, dimensions)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            MyBase.New(x, Nothing, z, keepDims, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, Nothing, False, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, keepDims, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, z, False, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, y, z, dimensions)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff)
			MyBase.New(sameDiff)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub
#End If

        Protected Friend Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            MyBase.New(x, y, z, keepDims, dimensions)
        End Sub

        Public Overrides Function opType() As Op.Type
            Return Op.Type.REDUCE_BOOL
        End Function

        Public Overrides Function getOpType() As Op.Type
            Return opType()
        End Function

        Public Overrides Function resultType() As DataType
            Return DataType.BOOL
        End Function

        Public Overrides Function resultType(ByVal oc As OpContext) As DataType
            Return DataType.BOOL
        End Function

        Public Overrides Function validateDataTypes(ByVal oc As OpContext) As Boolean
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
            Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
            If y IsNot Nothing Then
                Preconditions.checkArgument(x.dataType() = y.dataType(),"Op.X type must be the same as Op.Y:" & " x.dataType=%s, y.dataType=%s, op=%s", x.dataType(), y.dataType(), Me.GetType().FullName)
            End If

            Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())
            If z IsNot Nothing Then
                Preconditions.checkArgument(z.B, "Op.Z type must be bool: got type %s for op %s", z.dataType(), Me.GetType())
            End If

            Return True
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Return calculateOutputShape(Nothing)
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
            If x Is Nothing Then
                Return New List(Of LongShapeDescriptor)()
            End If

            'Calculate reduction shape. Note that reduction on scalar - returns a scalar
            Dim reducedShape() As Long = If(x.rank() = 0, x.shape(), ShapeT.getReducedShape(x.shape(), dimensionsField, KeepDims))
            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(reducedShape, DataType.BOOL)}
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'All reduce bool: always bool output type. 2nd input is axis arg
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or input datatype for %s, got input %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes.Count = 1 OrElse dataTypes(1).isIntType(), "When executing reductions" & "with 2 inputs, second input (axis) must be an integer datatype for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {DataType.BOOL}
        End Function

        Public MustOverride Function emptyValue() As Boolean

    End Class

End Namespace
