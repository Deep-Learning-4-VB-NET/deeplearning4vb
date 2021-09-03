Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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
'ORIGINAL LINE: @NoArgsConstructor @Slf4j public abstract class BaseBroadcastOp extends BaseOp implements BroadcastOp
	Public MustInherit Class BaseBroadcastOp
		Inherits BaseOp
		Implements BroadcastOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()

'JAVA TO VB CONVERTER NOTE: The field dimension was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend dimension_Conflict() As Integer


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

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, y, z)
			Broadcast.validateBroadcastDims(x,y,z, dimension)

			Me.dimension_Conflict = dimension

			defineDimensions(dimension)
		End Sub

		Public Overrides Function opType() As Type
			Return Type.BROADCAST
		End Function

		''' <summary>
		''' Calculate the output shape for this op
		''' 
		''' @return
		''' </summary>
		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			If x_Conflict Is Nothing OrElse y_Conflict Is Nothing Then
				Return Collections.emptyList()
			End If

			Dim shapeX() As Long = x_Conflict.shape()
			Dim shapeY() As Long = y_Conflict.shape()

			Return Collections.singletonList(LongShapeDescriptor.fromShape(Shape.broadcastOutputShape(shapeX, shapeY), Shape.pickPairwiseDataType(x_Conflict.dataType(), y_Conflict.dataType())))
		End Function


		Public Overridable Property Dimension As Integer() Implements BroadcastOp.getDimension
			Get
				If dimension_Conflict Is Nothing Then
					dimension_Conflict = Shape.getBroadcastDimensions(larg().Shape, rarg().Shape)
				End If
				Return dimension_Conflict
			End Get
			Set(ByVal dimension() As Integer)
				Me.dimension_Conflict = dimension
			End Set
		End Property




		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
		End Sub



		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub

		Public Overridable Function validateDataTypes(ByVal experimentalMode As Boolean) As Boolean Implements BroadcastOp.validateDataTypes

			Dim op As val = opNum()

			If y() IsNot Nothing AndAlso z() IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Preconditions.checkArgument(y().dataType() = z().dataType() OrElse x().dataType() = z().dataType(), "Op.Z type must be either Op.X or Op.Y: x.dataType=%s, y.dataType=%s, z.dataType=%s, op=%s", x_Conflict.dataType(), y_Conflict.dataType(), z_Conflict.dataType(), Me.GetType().FullName)
			End If

				If Not experimentalMode Then
					Preconditions.checkArgument(x_Conflict.dataType() = y_Conflict.dataType() OrElse y_Conflict.dataType() = DataType.BOOL, "Op.X must have same data type as Op.Y: X.datatype=%s, Y.datatype=%s", x_Conflict.dataType(), y_Conflict.dataType())
				End If

			If y() IsNot Nothing Then
				If op <> 1 AndAlso (y().R OrElse x().R) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point: x.dataType=%s, y.dataType=%s, z.dataType=%s, op=%s", x_Conflict.dataType(), y_Conflict.dataType(), z_Conflict.dataType(), Me.GetType().FullName)
				End If
			ElseIf x().R Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Preconditions.checkArgument(z().R, "Op.Z must have floating point type, since one of operands is floating point: x.dataType=%s, z.dataType=%s, op=%s", x_Conflict.dataType(), z_Conflict.dataType(), Me.GetType().FullName)
			End If

			Return True
		End Function

		Public Overridable ReadOnly Property OpType As Type Implements BroadcastOp.getOpType
			Get
				Return Type.BROADCAST
			End Get
		End Property
	End Class

End Namespace