Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports Broadcast = DeepLearning4VB.org.nd4vb.linalg.factory.Broadcast
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SameDiffUtils = DeepLearning4VB.org.nd4vb.autodiff.util.SameDiffUtils
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
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
    ''' The base class of the broadcast ops: z = f(x, y) where y is broadcast along <see cref="Dimension"/> of x
    ''' (<c>BROADCAST</c>).
    ''' <para>
    ''' Differences from Java: the field <c>dimension</c> is named <see cref="dimensionField"/>. Outside
    ''' <c>#If SAMEDIFF</c>, the <see cref="Dimension"/> getter computes missing dimensions from the shapes of x and y
    ''' (as <see cref="BaseBroadcastBoolOp"/> does; Java read the SameDiff arguments) and throws
    ''' <see cref="NotSupportedException"/> when x or y is missing.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseBroadcastOp
        Inherits BaseOp
        Implements BroadcastOp

        ''' <summary>The broadcast dimensions (Java field <c>dimension</c>).</summary>
        Protected Friend dimensionField() As Integer

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal dimension() As Integer)
			Me.New(sameDiff, i_v1, i_v2, False, dimension)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal inPlace As Boolean, ByVal dimension() As Integer)
			MyBase.New(sameDiff, inPlace, New Object(){i_v2})
			If i_v1 IsNot Nothing AndAlso i_v2 IsNot Nothing Then
				Me.sameDiff = sameDiff
				Me.inPlace = inPlace
				Me.dimension_Conflict = dimension
				sameDiff.addArgsFor(New SDVariable(){i_v1, i_v2},Me)
			Else
				Throw New System.ArgumentException("Input not null variables.")
			End If
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff)
			Me.sameDiff = sameDiff
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal dimension() As Integer, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, extraArgs)
			Me.dimension_Conflict = dimension
			If i_v1 IsNot Nothing AndAlso i_v2 IsNot Nothing Then
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v1, Me)
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v2, Me)
				Me.sameDiff = sameDiff
				sameDiff.addArgsFor(New SDVariable(){i_v1, i_v2},Me)
			Else
				Throw New System.ArgumentException("Input not null variables.")
			End If
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimension() As Integer, ByVal inPlace As Boolean)
			Me.New(sameDiff, i_v, i_v.Shape, inPlace, dimension, Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal shape() As Long, ByVal inPlace As Boolean, ByVal dimension() As Integer, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, inPlace, extraArgs)
			Me.dimension_Conflict = dimension
			If i_v IsNot Nothing Then
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
				sameDiff.addArgsFor(New SDVariable(){i_v},Me)
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimension() As Integer, ByVal extraArgs() As Object)
			Me.New(sameDiff, i_v, i_v.Shape, False, dimension, extraArgs)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimension() As Integer)
            MyBase.New(x, y, z)
            Broadcast.validateBroadcastDims(x, y, z, dimension)

            Me.dimensionField = dimension
            defineDimensions(dimension)
        End Sub

        Public Overrides Function dimensions() As INDArray Implements BroadcastOp.dimensions
            Return MyBase.dimensions()
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.BROADCAST
        End Function

        ''' <summary>
        ''' Calculate the output shape for this op
        ''' </summary>
        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            If xField Is Nothing OrElse yField Is Nothing Then
                Return New List(Of LongShapeDescriptor)()
            End If

            Dim shapeX() As Long = xField.shape()
            Dim shapeY() As Long = yField.shape()

            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(ShapeT.broadcastOutputShape(shapeX, shapeY), ShapeT.pickPairwiseDataType(xField.dataType(), yField.dataType()))}
        End Function

        ''' <summary>The broadcast dimensions (Java <c>getDimension()</c>/<c>setDimension()</c>).</summary>
        Public Overridable Property Dimension As Integer() Implements BroadcastOp.Dimension
            Get
                If dimensionField Is Nothing Then
#If SAMEDIFF Then
					dimension_Conflict = Shape.getBroadcastDimensions(larg().Shape, rarg().Shape)
#Else
                    If xField Is Nothing OrElse yField Is Nothing Then
                        Throw New NotSupportedException("Broadcast dimensions without x and y arrays need SameDiff (phase 2 milestone M5)")
                    End If
                    dimensionField = ShapeT.getBroadcastDimensions(xField.shape(), yField.shape())
#End If
                End If
                Return dimensionField
            End Get
            Set(ByVal dimension() As Integer)
                Me.dimensionField = dimension
            End Set
        End Property

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
		End Sub
#End If

        Public Overridable Function validateDataTypes(ByVal experimentalMode As Boolean) As Boolean Implements BroadcastOp.validateDataTypes

            Dim op As Integer = opNum()

            If y() IsNot Nothing AndAlso z() IsNot Nothing Then
                Preconditions.checkArgument(y().dataType() = z().dataType() OrElse x().dataType() = z().dataType(), "Op.Z type must be either Op.X or Op.Y: x.dataType=%s, y.dataType=%s, z.dataType=%s, op=%s", xField.dataType(), yField.dataType(), zField.dataType(), CObj(Me).GetType().FullName)
            End If

            If Not experimentalMode Then
                Preconditions.checkArgument(xField.dataType() = yField.dataType() OrElse yField.dataType() = DataType.BOOL, "Op.X must have same data type as Op.Y: X.datatype=%s, Y.datatype=%s", CObj(xField.dataType()), CObj(yField.dataType()))
            End If

            If y() IsNot Nothing Then
                If op <> 1 AndAlso (y().R OrElse x().R) Then
                    Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point: x.dataType=%s, y.dataType=%s, z.dataType=%s, op=%s", xField.dataType(), yField.dataType(), zField.dataType(), CObj(Me).GetType().FullName)
                End If
            ElseIf x().R Then
                Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point: x.dataType=%s, z.dataType=%s, op=%s", xField.dataType(), zField.dataType(), CObj(Me).GetType().FullName)
            End If

            Return True
        End Function

        Public Overridable Overloads Function getOpType() As Op.Type Implements BroadcastOp.getOpType
            Return Op.Type.BROADCAST
        End Function

    End Class

End Namespace
