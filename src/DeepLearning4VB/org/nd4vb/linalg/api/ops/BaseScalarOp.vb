Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
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
    ''' The base class of the scalar ops: z = f(x, scalar) (<c>SCALAR</c>). The scalar is <see cref="scalar"/>: y when
    ''' y is a scalar array, otherwise the scalar value given to the constructor or <c>setScalar</c>, stored as a
    ''' scalar array of the data type of x.
    ''' <para>
    ''' Differences from Java: Java's <c>Number</c> parameters are <c>Object</c> (any boxed number). Outside
    ''' <c>#If SAMEDIFF</c>, <see cref="calculateOutputShape(OpContext)"/> takes the input data type from x (Java read
    ''' it from the SameDiff argument, which is the variable of x) and needs x.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseScalarOp
        Inherits BaseOp
        Implements ScalarOp

        ''' <summary>Creates the op with the scalar 0 (a <c>FLOAT</c> scalar array).</summary>
        Public Sub New()
            Me.scalarValue = Nd4vbT.scalar(0.0F)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal num As Object)
            MyBase.New(x, y, z)
            If x.Compressed Then
                Nd4vbT.Compressor.decompressi(x)
            End If

            Using ws As MemoryWorkspace = Nd4vbT.MemoryManager.scopeOutOfWorkspaces()
                Me.scalarValue = Nd4vbT.scalar(x.dataType(), num)
            End Using
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal num As Object)
            MyBase.New(x)
            If x.Compressed Then
                Nd4vbT.Compressor.decompressi(x)
            End If

            Using ws As MemoryWorkspace = Nd4vbT.MemoryManager.scopeOutOfWorkspaces()
                Me.scalarValue = Nd4vbT.scalar(x.dataType(), num)
            End Using
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal [set] As Object)
            MyBase.New(x, Nothing, z)
            If x.Compressed Then
                Nd4vbT.Compressor.decompressi(x)
            End If

            Using ws As MemoryWorkspace = Nd4vbT.MemoryManager.scopeOutOfWorkspaces()
                Me.scalarValue = Nd4vbT.scalar(x.dataType(), [set])
            End Using
        End Sub

#If SAMEDIFF Then
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
#End If

        Public Overrides Function z() As INDArray
            Return zField
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Return calculateOutputShape(Nothing)
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
#If SAMEDIFF Then
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
#Else
            If x Is Nothing Then
                Throw New NotSupportedException("calculateOutputShape without an x array needs SameDiff (phase 2 milestone M5)")
            End If
            Dim aT As DataType = x.dataType()
            Dim sT As DataType = scalarValue.dataType()
            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(x.shape(), ShapeT.pickPairwiseDataType(aT, sT))}
#End If
        End Function

        Public Overrides Function dimensions() As INDArray Implements ScalarOp.dimensions
            Return MyBase.dimensions()
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.SCALAR
        End Function

        ''' <summary>Sets the scalar as a scalar array of the data type of x.</summary>
        Public Overridable Sub setScalar(ByVal scalar As Object) Implements ScalarOp.setScalar
            Me.scalarValue = Nd4vbT.scalar(xField.dataType(), scalar)
        End Sub

        Public Overridable Sub setScalar(ByVal scalar As INDArray) Implements ScalarOp.setScalar
            Me.scalarValue = scalar
        End Sub

        ''' <summary>The scalar operand: y when it is a scalar array, otherwise the stored scalar value.</summary>
        Public Overridable Function scalar() As INDArray Implements ScalarOp.scalar
            If y() IsNot Nothing AndAlso y().Scalar Then
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
            If y() IsNot Nothing Then
                If y().R OrElse x().R Then
                    Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point:" & " x.dataType=%s, y.dataType=%s, z.dataType=%s, op=%s", xField.dataType(), yField.dataType(), zField.dataType(), CObj(Me).GetType().FullName)
                End If

                If Not experimentalMode Then
                    Preconditions.checkArgument(xField.dataType() = yField.dataType() OrElse yField.dataType() = DataType.BOOL, "Op.X must have same data type as Op.Y")
                End If
            ElseIf x().R Then
                Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point:" & " x.dataType=%s, z.dataType=%s, op=%s", xField.dataType(), zField.dataType(), CObj(Me).GetType().FullName)
            End If

            Return True
        End Function

        Public Overridable Overloads Function getOpType() As Op.Type Implements ScalarOp.getOpType
            Return Op.Type.SCALAR
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'All scalar ops: output type is same as input type
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count >= 1, "Expected 1 or more input datatype %s, got input %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
