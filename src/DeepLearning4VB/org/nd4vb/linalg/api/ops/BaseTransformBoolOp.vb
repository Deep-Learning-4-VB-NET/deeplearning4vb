Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
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
    ''' The base class of the bool transform ops: z is always <c>BOOL</c> (<c>TRANSFORM_BOOL</c>). With a y array the
    ''' executioner runs the op as a pairwise bool op.
    ''' <para>
    ''' As in Java, the class implements <see cref="TransformSameOp"/>. <see cref="calculateOutputDataTypes"/> counts
    ''' the SameDiff arguments, so outside <c>#If SAMEDIFF</c> it throws <see cref="NotSupportedException"/>.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseTransformBoolOp
        Inherits BaseTransformOp
        Implements TransformSameOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, i_v1, i_v2)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v1, i_v2, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff)
			MyBase.New(sameDiff)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v1, i_v2, extraArgs)
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

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
            MyBase.New(x, y, z)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal x As INDArray)
            MyBase.New(x)
        End Sub

        Public Overrides Function getOpType() As Op.Type
            Return Op.Type.TRANSFORM_BOOL
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.TRANSFORM_BOOL
        End Function

        ''' <summary>Always <c>BOOL</c>.</summary>
        Public Overrides Function resultType() As DataType
            Return DataType.BOOL
        End Function

        Public Overrides Function resultType(ByVal oc As OpContext) As DataType
            Return DataType.BOOL
        End Function

        Public Overrides Function validateDataTypes(ByVal oc As OpContext, ByVal experimentalMode As Boolean) As Boolean
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
            Dim y As INDArray = If(oc IsNot Nothing, oc.getInputArray(1), Me.y())
            Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), Me.z())

            If Me.y() IsNot Nothing Then
                Preconditions.checkArgument(x.dataType() = y.dataType(), "Op.X must be the same type as Op.Y: " & "x.datatype=%s, y.datatype=%s", CObj(x.dataType()), CObj(y.dataType()))
            End If

            If z IsNot Nothing Then
                Preconditions.checkArgument(z.B, "Op.Z type must be bool: z.datatype=%s for op %s", z.dataType(), Me.GetType())
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
            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(x.shape(), DataType.BOOL)}
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'All bool tranform ops: always bool output type
			Dim args() As SDVariable = Me.args()
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = args.Length, "Expected exactly %s input datatype(s) for %s, got input %s", args.Length, Me.GetType(), dataTypes)
			Return Collections.singletonList(DataType.BOOL)
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
