Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
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
    ''' The base class of the floating-point transform ops: z is floating point, the type of x when x is floating
    ''' point and the default floating-point type otherwise (<c>TRANSFORM_FLOAT</c>).
    ''' </summary>
    Public MustInherit Class BaseTransformFloatOp
        Inherits BaseTransformOp
        Implements TransformFloatOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal shape() As Long, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v, shape, inPlace, extraArgs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v, extraArgs)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(x, z)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal x As INDArray)
            MyBase.New(x)
        End Sub

        Public Overrides Function getOpType() As Op.Type
            Return Op.Type.TRANSFORM_FLOAT
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.TRANSFORM_FLOAT
        End Function

        ''' <summary>The data type of x when it is floating point, otherwise the default floating-point type.</summary>
        Public Overrides Function resultType() As DataType
            If Me.x() IsNot Nothing AndAlso Me.x().R Then
                Return Me.x().dataType()
            End If

            Return Nd4vbT.defaultFloatingPointType()
        End Function

        Public Overrides Function resultType(ByVal oc As OpContext) As DataType
            If oc.getInputArray(0) IsNot Nothing AndAlso oc.getInputArray(0).R Then
                Return oc.getInputArray(0).dataType()
            End If

            Return Nd4vbT.defaultFloatingPointType()
        End Function

        Public Overrides Function validateDataTypes(ByVal oc As OpContext, ByVal experimentalMode As Boolean) As Boolean
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
            Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
            Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())

            If y IsNot Nothing AndAlso Not experimentalMode Then
                Preconditions.checkArgument(x.dataType() = y.dataType(), "Op.X must have same data type as Op.Y")
            End If

            If z IsNot Nothing Then
                Preconditions.checkArgument(z.R, "Op.Z must be one of floating types: z.datatype=%s for op %s", z.dataType(), Me.GetType())
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
            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(x.shape(), If(x.R, x.dataType(), Nd4vbT.defaultFloatingPointType()))}
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
            If dataTypes(0).isFPType() Then
                Return New List(Of DataType) From {dataTypes(0)}
            End If
            'TODO is this what we want for all cases?
            Return New List(Of DataType) From {DataType.FLOAT}
        End Function

    End Class

End Namespace
