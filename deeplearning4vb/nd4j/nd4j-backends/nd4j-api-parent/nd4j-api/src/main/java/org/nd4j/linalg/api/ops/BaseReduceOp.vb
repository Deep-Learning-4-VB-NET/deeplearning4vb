Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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
'ORIGINAL LINE: @Slf4j public abstract class BaseReduceOp extends BaseOp implements ReduceOp
	Public MustInherit Class BaseReduceOp
		Inherits BaseOp
		Implements ReduceOp

		Public MustOverride Function validateDataTypes(ByVal oc As OpContext) As Boolean Implements ReduceOp.validateDataTypes
		Public MustOverride Function resultType(ByVal oc As OpContext) As org.nd4j.linalg.api.buffer.DataType Implements ReduceOp.resultType
		Public MustOverride Function resultType() As org.nd4j.linalg.api.buffer.DataType Implements ReduceOp.resultType
		Public MustOverride ReadOnly Property OpType As Type Implements ReduceOp.getOpType
		Public Overrides MustOverride WriteOnly Property ExtraArgs Implements org.nd4j.linalg.api.ops.Op.setExtraArgs As Object()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected boolean keepDims = false;
'JAVA TO VB CONVERTER NOTE: The field keepDims was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend keepDims_Conflict As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected boolean isComplex = false;
		Protected Friend isComplex As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected boolean isEmptyReduce = false;
		Protected Friend isEmptyReduce As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected org.nd4j.autodiff.samediff.SDVariable dimensionVariable;
		Protected Friend dimensionVariable As SDVariable


		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, Nothing)
			If i_v IsNot Nothing Then
				If dimensions Is Nothing OrElse dimensions.Length < 1 Then
					dimensions = New Integer() {Integer.MaxValue}
				End If

				Me.dimensions = dimensions
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
				Me.keepDims_Conflict = keepDims
				Me.xVertexId = i_v.name()
				sameDiff.addArgsFor(New String(){xVertexId},Me)
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If

			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff,Nothing)
			If i_v IsNot Nothing Then
				If dimensions Is Nothing OrElse dimensions.Length < 1 Then
					dimensions = New Integer() {Integer.MaxValue}
				End If

				Me.dimensions = dimensions

				Me.xVertexId = i_v.name()
				Me.yVertexId = i_v2.name()
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v2, Me)
				Me.keepDims_Conflict = keepDims
				sameDiff.addArgsFor(New String(){xVertexId, yVertexId},Me)

			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If

			defineDimensions(dimensions)
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			Me.New(sameDiff, i_v, False)
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			Me.New(sameDiff,i_v,dimensions,False)

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			Me.New(sameDiff,i_v,i_v2,dimensions,False)
		End Sub









		'Special constructors for allowing dimensions to be an SDVariable

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, Nothing)
			If i_v IsNot Nothing Then
				If dimensions Is Nothing OrElse dimensions.Length < 1 Then
					dimensions = New Integer() {Integer.MaxValue}
				End If

				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
				Me.keepDims_Conflict = keepDims
				Me.xVertexId = i_v.name()
				sameDiff.addArgsFor(New String(){xVertexId},Me)
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If

			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff,Nothing)

			Me.dimensionVariable = dimensions


			Me.xVertexId = i_v.name()
			Me.yVertexId = dimensions.name()
			SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
			SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, dimensions, Me)
			Me.keepDims_Conflict = keepDims
			sameDiff.addArgsFor(New String(){xVertexId, yVertexId},Me)

		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable)
			Me.New(sameDiff,i_v,i_v2,False)
		End Sub






		Public Sub New()
		End Sub


		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(x, y, z)
			Me.keepDims_Conflict = keepDims
			Me.dimensions = dimensions
			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, Nothing, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(x, Nothing, dimensions)
			Me.keepDims_Conflict = keepDims
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, Nothing, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, z, False, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff)
			Me.sameDiff = sameDiff
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			Me.New(sameDiff,i_v,dimensions,False)
		End Sub

		Public Overridable Function noOp() As INDArray Implements ReduceOp.noOp
			If z_Conflict IsNot Nothing AndAlso x_Conflict IsNot z_Conflict Then
				Return z().assign(x_Conflict)
			Else
				'Need to take into account shapes: for example, [1,3].sum(0) -> [3]
				'Or [1,1,1,1].sum(0,2,3) -> [1]
				If keepDims_Conflict Then
					Return x().dup(x().ordering())
				Else
					Dim shape() As Long = x_Conflict.shape()
					If dimensions Is Nothing OrElse Shape.isWholeArray(shape, dimensions) Then
						'Return scalar
						Return x_Conflict.reshape().dup()
					Else
						'Strip out size 1 dimensions
						Dim outShape() As Long = ArrayUtil.removeIndex(shape, dimensions)
						Return x_Conflict.dup("c"c).reshape("c"c, outShape)
					End If
				End If
			End If
		End Function

		Public Overridable ReadOnly Property KeepDims As Boolean Implements ReduceOp.isKeepDims
			Get
				Return keepDims_Conflict
			End Get
		End Property


		Public Overrides MustOverride Function calculateOutputShape() As IList(Of LongShapeDescriptor)


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If Not attributesForNode.ContainsKey("axis") AndAlso Not hasReductionIndices(nodeDef) Then
				Me.dimensions = New Integer() { Integer.MaxValue }
			End If 'Otherwise: dimensions are dynamically set during execution in InferenceSession

			If attributesForNode.ContainsKey("keep_dims") Then
				Dim keepDims As val = attributesForNode("keep_dims").getB()
				Me.keepDims_Conflict = keepDims
			End If
			defineDimensions(Me.dimensions)
		End Sub

		Protected Friend Overridable Function hasReductionIndices(ByVal nodeDef As NodeDef) As Boolean
			Dim i As Integer = 0
			Do While i < nodeDef.getInputCount()
				If nodeDef.getInput(i).contains("reduction_indices") Then
					Return True
				End If
				i += 1
			Loop

			Return False
		End Function


		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub

		Public Overridable ReadOnly Property ComplexAccumulation As Boolean Implements ReduceOp.isComplexAccumulation
			Get
				Return isComplex
			End Get
		End Property

		Public Overridable WriteOnly Property Dimensions Implements ReduceOp.setDimensions As Integer()
			Set(ByVal dimensions() As Integer)
				Me.dimensions = dimensions
				defineDimensions(dimensions)
			End Set
		End Property
	End Class

End Namespace