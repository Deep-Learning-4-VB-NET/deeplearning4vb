Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SameDiffUtils = DeepLearning4VB.org.nd4vb.autodiff.util.SameDiffUtils
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
    ''' The base class of the scalar comparison ops: z = f(x, scalar) with a <c>BOOL</c> z (<c>SCALAR_BOOL</c>).
    ''' Unlike <see cref="BaseScalarOp"/>, the parameterless constructor leaves the scalar unset, and
    ''' <see cref="scalar"/> returns y only when no scalar value is set. Java's <c>Number</c> parameters are
    ''' <c>Object</c>.
    ''' </summary>
    Public MustInherit Class BaseScalarBoolOp
        Inherits BaseOp
        Implements ScalarOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal num As Object)
            MyBase.New(x, y, z)
            Me.scalarValue = Nd4vbT.scalar(x.dataType(), num)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal num As Object)
            MyBase.New(x)
            Me.scalarValue = Nd4vbT.scalar(x.dataType(), num)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal [set] As Object)
            MyBase.New(x, Nothing, z)
            Me.scalarValue = Nd4vbT.scalar(x.dataType(), [set])
        End Sub

#If SAMEDIFF Then
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
#End If

        Public Overrides Function z() As INDArray
            Return zField
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
            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(x.shape(), DataType.BOOL)}
        End Function

        Public Overrides Function dimensions() As INDArray Implements ScalarOp.dimensions
            Return MyBase.dimensions()
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.SCALAR_BOOL
        End Function

        ''' <summary>Sets the scalar as a scalar array of the value's own type.</summary>
        Public Overridable Sub setScalar(ByVal scalar As Object) Implements ScalarOp.setScalar
            Me.scalarValue = Nd4vbT.scalar(scalar)
        End Sub

        Public Overridable Sub setScalar(ByVal scalar As INDArray) Implements ScalarOp.setScalar
            Me.scalarValue = scalar
        End Sub

        ''' <summary>The scalar operand: the stored scalar value, or y when no value is stored and y is a scalar array.</summary>
        Public Overridable Function scalar() As INDArray Implements ScalarOp.scalar
            If scalarValue Is Nothing AndAlso y() IsNot Nothing AndAlso y().Scalar Then
                Return y()
            End If
            Return scalarValue
        End Function

        ''' <summary>The dimensions as given (Java <c>getDimension()</c>); setting them calls <see cref="defineDimensions"/>.</summary>
        Public Overridable Property Dimension As Integer() Implements ScalarOp.Dimension
            Get
                Return dimensionsField
            End Get
            Set(ByVal dimension() As Integer)
                defineDimensions(dimension)
            End Set
        End Property

        Public Overridable Function validateDataTypes(ByVal experimentalMode As Boolean) As Boolean Implements ScalarOp.validateDataTypes
            Preconditions.checkArgument(z().B, "Op.Z must have floating point type, since one of operands is floating point." & " op.z.datatype=" & z().dataType().ToString())

            Return True
        End Function

        Public Overridable Overloads Function getOpType() As Op.Type Implements ScalarOp.getOpType
            Return Op.Type.SCALAR_BOOL
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'All scalar bool ops: output type is always bool
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {DataType.BOOL}
        End Function

    End Class

End Namespace
