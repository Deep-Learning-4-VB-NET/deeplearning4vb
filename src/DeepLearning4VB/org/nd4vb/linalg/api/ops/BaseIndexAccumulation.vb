Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
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
    ''' The base class of the index reductions: z is the index (<c>LONG</c>) of the element that the reduction picks
    ''' along the dimensions (<c>INDEXREDUCE</c>).
    ''' <para>
    ''' Differences from Java: the field <c>keepDims</c> is named <see cref="keepDimsField"/>. Lombok's <c>@Data</c>
    ''' members are written out: <see cref="KeepDims"/> and <see cref="setKeepDims"/>, and <see cref="Equals"/>,
    ''' <see cref="GetHashCode"/> and <see cref="ToString"/> over <c>keepDims</c> only (Lombok's default, which
    ''' replaces <see cref="BaseOp"/>'s versions; <c>ToString</c> gives <c>BaseIndexAccumulation(keepDims=false)</c>).
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseIndexAccumulation
        Inherits BaseOp
        Implements IndexAccumulation

        ''' <summary>True to keep the reduced dimensions as size 1 (Java field <c>keepDims</c>).</summary>
        Protected Friend keepDimsField As Boolean = False

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff,Nothing)
			If i_v IsNot Nothing Then
				Me.dimensions = dimensions
				sameDiff.addArgsFor(New SDVariable(){i_v},Me)

				Me.xVertexId = i_v.name()
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If
			Me.keepDims = keepDims
			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff,Nothing)
			If i_v IsNot Nothing Then
				Me.dimensions = dimensions
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v, Me)
				SameDiffUtils.validateDifferentialFunctionSameDiff(sameDiff, i_v2, Me)
				Me.xVertexId = i_v.name()
				Me.yVertexId = i_v2.name()
				sameDiff.addArgsFor(New SDVariable(){i_v, i_v2},Me)
			Else
				Throw New System.ArgumentException("Input not null variable.")
			End If
			Me.keepDims = keepDims
			defineDimensions(dimensions)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal dimensions() As Integer)
            Me.New(x, CType(Nothing, INDArray), dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
            Me.New(x, CType(Nothing, INDArray), dimensions)
            Me.keepDimsField = keepDims
            defineDimensions(dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal dimensions() As Integer)
            MyBase.New(x, z)
            defineDimensions(dimensions)
        End Sub

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Return calculateOutputShape(Nothing)
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Dim x As INDArray = If(oc IsNot Nothing, oc.getInputArray(0), Me.x())
            If x Is Nothing Then
                Return New List(Of LongShapeDescriptor)()
            End If

            Dim reducedShape() As Long = ShapeT.getReducedShape(x.shape(), dimensionsField, keepDimsField)
            Return New List(Of LongShapeDescriptor) From {LongShapeDescriptor.fromShape(reducedShape, DataType.LONG)}
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.INDEXREDUCE
        End Function

        Public Overridable Function validateDataTypes() As Boolean Implements IndexAccumulation.validateDataTypes

            If z() IsNot Nothing Then
                Preconditions.checkArgument(z().dataType() = DataType.LONG, "IndexReduce operations require LONG output: " & "got result array of type %s for op %s", zField.dataType(), Me.GetType())
            End If

            Return True
        End Function

        ''' <summary>True to keep the reduced dimensions as size 1 (Java <c>isKeepDims()</c>).</summary>
        Public Overridable ReadOnly Property KeepDims As Boolean Implements IndexAccumulation.KeepDims
            Get
                Return keepDimsField
            End Get
        End Property

        Public Overridable Sub setKeepDims(ByVal keepDims As Boolean)
            Me.keepDimsField = keepDims
        End Sub

        Public Overrides Function dimensions() As INDArray Implements IndexAccumulation.dimensions
            Return MyBase.dimensions()
        End Function

        Public Overrides ReadOnly Property FinalResult As Object Implements IndexAccumulation.FinalResult
            Get
                Return MyBase.FinalResult
            End Get
        End Property

        ''' <summary>Lombok <c>@Data</c>: equal when the classes and <c>keepDims</c> are equal.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As BaseIndexAccumulation = TryCast(o, BaseIndexAccumulation)
            If other Is Nothing OrElse Not other.canEqual(Me) Then
                Return False
            End If
            Return keepDimsField = other.keepDimsField
        End Function

        ''' <summary>Lombok's <c>canEqual</c>.</summary>
        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is BaseIndexAccumulation
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 59 + If(keepDimsField, 79, 97)
        End Function

        Public Overrides Function ToString() As String
            Return "BaseIndexAccumulation(keepDims=" & If(keepDimsField, "true", "false") & ")"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'All index accumulation ops: always long output type
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {DataType.LONG}
        End Function

    End Class

End Namespace
