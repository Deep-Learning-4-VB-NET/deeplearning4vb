Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Shape = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
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
    ''' The base class of the floating-point reductions (mean, norms, entropy, ...): z is floating point, the type of
    ''' x when x is floating point and the default floating-point type otherwise (<c>REDUCE_FLOAT</c>). Outside
    ''' <c>#If SAMEDIFF</c>, <see cref="calculateOutputShape(OpContext)"/> takes the input data type from x (Java read it
    ''' from the SameDiff argument, which is the variable of x).
    ''' </summary>
    Public MustInherit Class BaseReduceFloatOp
        Inherits BaseReduceOp
        Implements ReduceFloatOp

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, y, z, keepDims, dimensions)
        End Sub

#If SAMEDIFF Then
		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, input, dimensions, keepDims)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, input, dimensions)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, input, dimensions, keepDims)
		End Sub

		Protected Friend Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, input, dimensions)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(input, Nothing, output, dimensions)
            Me.keepDimsField = keepDims
            Me.dimensionsField = dimensions
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, y, z, dimensions)
        End Sub
        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, Nothing, z, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, keepDims, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
            MyBase.New(x, dimensions)
        End Sub

        Protected Friend Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function opType() As Op.Type
            Return Op.Type.REDUCE_FLOAT
        End Function

        Public Overrides Function getOpType() As Op.Type
            Return opType()
        End Function

        Public Overrides Function resultType() As DataType
            Return resultType(Nothing)
        End Function

        Public Overrides Function resultType(ByVal oc As OpContext) As DataType
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
            If x IsNot Nothing AndAlso x.R Then
                Return x.dataType()
            End If

            Return Nd4vbT.defaultFloatingPointType()
        End Function

        Public Overrides Function validateDataTypes(ByVal oc As OpContext) As Boolean
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
            Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
            If y IsNot Nothing Then
                Preconditions.checkArgument(x.dataType() = y.dataType(), "Op.X [%s] type must be the same as Op.Y [%s] for op %s: x.shape=%ndShape, y.shape=%ndShape", x.dataType(), y.dataType(), Me.GetType().FullName, x, y)
            End If

            Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())
            If z IsNot Nothing Then
                Preconditions.checkArgument(z.R,"Op.Z (result array) must be one of floating types: z datatype = %s", CObj(z.dataType()))
            End If

            Return True
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Return calculateOutputShape(Nothing)
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())

			If x Is Nothing Then
				Return Collections.emptyList()
			End If

			'Calculate reduction shape. Note that reduction on scalar - returns a scalar
			Dim reducedShape() As Long = If(x.rank() = 0, x.shape(), Shape.getReducedShape(x.shape(),dimensions, KeepDims))
			Dim retType As DataType = arg().dataType()
			If Not retType.isFPType() Then
				retType = Nd4j.defaultFloatingPointType()
			End If
			Return Collections.singletonList(LongShapeDescriptor.fromShape(reducedShape, retType))
		End Function
#Else
        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())

            If x Is Nothing Then
                Return New List(Of LongShapeDescriptor)()
            End If

            'Calculate reduction shape. Note that reduction on scalar - returns a scalar
            Dim reducedShape() As Long = If(x.rank() = 0, x.shape(), ShapeT.getReducedShape(x.shape(), dimensionsField, KeepDims))
            Dim retType As DataType = x.dataType()
            If Not retType.isFPType() Then
                retType = Nd4vbT.defaultFloatingPointType()
            End If
            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(reducedShape, retType)}
        End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Second input is dynamic axis arg
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or 2 input datatype for %s, got input %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes.Count = 1 OrElse dataTypes(1).isIntType(), "When executing reductions" & "with 2 inputs, second input (axis) must be an integer datatype for %s, got %s", Me.GetType(), dataTypes)
            If dataTypes(0).isFPType() Then
                Return New List(Of DataType) From {dataTypes(0)}
            End If
            Return New List(Of DataType) From {Nd4vbT.defaultFloatingPointType()}
        End Function

    End Class

End Namespace
