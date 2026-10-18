Imports System
Imports System.Collections.Generic
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
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
    ''' The base class of the reduce ops: z = the reduction of x (and y, for reduce3 ops) along the dimensions,
    ''' keeping the reduced dimensions as size 1 when <see cref="KeepDims"/> is set.
    ''' <para>
    ''' What an executioner reads: <see cref="getOpType"/> (reduce float, same, long, bool or reduce3),
    ''' <see cref="getDimensions"/> (the dimensions as given; Nothing or <c>Integer.MaxValue</c> means all
    ''' dimensions) or <see cref="dimensions"/> (normalized, as an array), <see cref="KeepDims"/>,
    ''' <see cref="ComplexAccumulation"/> (reduce3 "all distances" mode) and <see cref="resultType()"/>.
    ''' </para>
    ''' <para>
    ''' Differences from Java: the fields <c>keepDims</c>, <c>isComplex</c> and <c>isEmptyReduce</c> are named
    ''' <see cref="keepDimsField"/>, <see cref="isComplexField"/> and <see cref="isEmptyReduceField"/> (VB names are
    ''' case-insensitive); their Lombok accessors keep the Java names (<see cref="setKeepDims"/>,
    ''' <see cref="isComplex"/>, <see cref="setComplex"/>, <see cref="isEmptyReduce"/>, <see cref="setEmptyReduce"/>).
    ''' The SameDiff field <c>dimensionVariable</c> is inside <c>#If SAMEDIFF</c>.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseReduceOp
        Inherits BaseOp
        Implements ReduceOp

        Public MustOverride Function validateDataTypes(ByVal oc As OpContext) As Boolean Implements ReduceOp.validateDataTypes

        Public MustOverride Function resultType(ByVal oc As OpContext) As DataType Implements ReduceOp.resultType

        ''' <summary>The data type of z.</summary>
        Public MustOverride Function resultType() As DataType Implements ReduceOp.resultType

        ''' <summary>The reduce kind of the op (Java <c>getOpType()</c>).</summary>
        Public MustOverride Overloads Function getOpType() As Op.Type Implements ReduceOp.getOpType

        ''' <summary>True to keep the reduced dimensions as size 1 (Java field <c>keepDims</c>).</summary>
        Protected Friend keepDimsField As Boolean = False
        ''' <summary>Java field <c>isComplex</c>: reduce3 "all distances" mode.</summary>
        Protected Friend isComplexField As Boolean = False
        ''' <summary>Java field <c>isEmptyReduce</c>.</summary>
        Protected Friend isEmptyReduceField As Boolean = False

#If SAMEDIFF Then
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

		Public Sub New(ByVal sameDiff As SameDiff)
			Me.sameDiff = sameDiff
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			Me.New(sameDiff,i_v,dimensions,False)
		End Sub

		Public Overridable Function getDimensionVariable() As SDVariable
			Return dimensionVariable
		End Function

		Public Overridable Sub setDimensionVariable(ByVal dimensionVariable As SDVariable)
			Me.dimensionVariable = dimensionVariable
		End Sub
#End If

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Initialize with the given
        ''' input, pairwise transform, result, and number
        ''' of elements
        ''' </summary>
        ''' <param name="x"> the input </param>
        ''' <param name="y"> the pairwise transform </param>
        ''' <param name="z"> the result </param>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            MyBase.New(x, y, z)
            Me.keepDimsField = keepDims
            Me.dimensionsField = dimensions
            defineDimensions(dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, CType(Nothing, INDArray), dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            Me.New(x, CType(Nothing, INDArray), dimensions)
            Me.keepDimsField = keepDims
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, y, CType(Nothing, INDArray), dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, y, z, False, dimensions)
        End Sub

        ''' <summary>
        ''' The result of reducing nothing: z gets a copy of x (or x reshaped to the reduced shape).
        ''' </summary>
        Public Overridable Function noOp() As INDArray Implements ReduceOp.noOp
            If zField IsNot Nothing AndAlso xField IsNot zField Then
                Return z().assign(xField)
            Else
                'Need to take into account shapes: for example, [1,3].sum(0) -> [3]
                'Or [1,1,1,1].sum(0,2,3) -> [1]
                If keepDimsField Then
                    Return x().dup(x().ordering())
                Else
                    Dim shape() As Long = xField.shape()
                    If dimensionsField Is Nothing OrElse ShapeT.isWholeArray(shape, dimensionsField) Then
                        'Return scalar
                        Return xField.reshape().dup()
                    Else
                        'Strip out size 1 dimensions
                        Dim outShape() As Long = ArrayUtil.removeIndex(shape, dimensionsField)
                        Return xField.dup("c"c).reshape("c"c, outShape)
                    End If
                End If
            End If
        End Function

        ''' <summary>True to keep the reduced dimensions as size 1 (Java <c>isKeepDims()</c>).</summary>
        Public Overridable ReadOnly Property KeepDims As Boolean Implements ReduceOp.KeepDims
            Get
                Return keepDimsField
            End Get
        End Property

        Public Overridable Sub setKeepDims(ByVal keepDims As Boolean)
            Me.keepDimsField = keepDims
        End Sub

        Public MustOverride Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)

#If MODEL_IMPORT Then
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
#End If

        ''' <summary>True in reduce3 "all distances" mode (Java <c>isComplexAccumulation()</c>).</summary>
        Public Overridable ReadOnly Property ComplexAccumulation As Boolean Implements ReduceOp.ComplexAccumulation
            Get
                Return isComplexField
            End Get
        End Property

        Public Overridable Function isComplex() As Boolean
            Return isComplexField
        End Function

        Public Overridable Sub setComplex(ByVal isComplex As Boolean)
            Me.isComplexField = isComplex
        End Sub

        Public Overridable Function isEmptyReduce() As Boolean
            Return isEmptyReduceField
        End Function

        Public Overridable Sub setEmptyReduce(ByVal isEmptyReduce As Boolean)
            Me.isEmptyReduceField = isEmptyReduce
        End Sub

        ''' <summary>Stores the dimensions as given and as an array (<see cref="defineDimensions"/>).</summary>
        Public Overrides Sub setDimensions(ParamArray ByVal dimensions() As Integer) Implements ReduceOp.setDimensions
            Me.dimensionsField = dimensions
            defineDimensions(dimensions)
        End Sub

        Public Overrides Function dimensions() As INDArray Implements ReduceOp.dimensions
            Return MyBase.dimensions()
        End Function

        Public Overrides ReadOnly Property FinalResult As Object Implements ReduceOp.FinalResult
            Get
                Return MyBase.FinalResult
            End Get
        End Property

    End Class

End Namespace
