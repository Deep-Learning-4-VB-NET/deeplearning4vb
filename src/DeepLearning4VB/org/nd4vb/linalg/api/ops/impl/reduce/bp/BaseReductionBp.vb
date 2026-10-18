Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
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

Namespace org.nd4vb.linalg.api.ops.impl.reduce.bp

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.reduce.bp.BaseReductionBp</c>.</summary>
    Public MustInherit Class BaseReductionBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend keepDimsField As Boolean

#If SAMEDIFF Then
		''' 
		''' <param name="origInput">    Pre-reduced input </param>
		''' <param name="gradAtOutput"> Gradient at the output </param>
		''' <param name="keepDims">     If true: reduction dimensions were kept </param>
		''' <param name="dimensions">   Dimensions to reduce. May be null </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal origInput As SDVariable, ByVal gradAtOutput As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){origInput, gradAtOutput}, False)
			Me.keepDims = keepDims
			Me.dimensions = dimensions
			addArgs()
		End Sub

		''' 
		''' <param name="origInput1">   Pre-reduced input 1 </param>
		''' <param name="origInput2">   Pre-reduced input 2 </param>
		''' <param name="gradAtOutput"> Gradient at the output </param>
		''' <param name="keepDims">     If true: reduction dimensions were kept </param>
		''' <param name="dimensions">   Dimensions to reduce. May be null </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal origInput1 As SDVariable, ByVal origInput2 As SDVariable, ByVal gradAtOutput As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){origInput1, origInput2, gradAtOutput}, False)
			Me.keepDims = keepDims
			Me.dimensions = dimensions
			addArgs()
		End Sub
#End If

        ''' 
        ''' <param name="origInput">    Pre-reduced input </param>
        ''' <param name="gradAtOutput"> Gradient at the output </param>
        ''' <param name="output">       Output array - i.e., gradient at the input to the reduction function </param>
        ''' <param name="keepDims">     If true: reduction dimensions were kept </param>
        ''' <param name="dimensions">   Dimensions to reduce. May be null </param>
        Public Sub New(ByVal origInput As INDArray, ByVal gradAtOutput As INDArray, ByVal output As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(Nothing, New INDArray(){origInput, gradAtOutput}, (If(output Is Nothing, Nothing, New INDArray() {output})))
            Me.keepDimsField = keepDims
            Me.dimensionsField = dimensions
            addArgs()
        End Sub

        ''' 
        ''' <param name="origInput1">   Pre-reduced input1 </param>
        ''' <param name="origInput2">   Pre-reduced input2 </param>
        ''' <param name="gradAtOutput"> Gradient at the output </param>
        ''' <param name="output">       Output array - i.e., gradient at the input to the reduction function </param>
        ''' <param name="keepDims">     If true: reduction dimensions were kept </param>
        ''' <param name="dimensions">   Dimensions to reduce. May be null </param>
        Public Sub New(ByVal origInput1 As INDArray, ByVal origInput2 As INDArray, ByVal gradAtOutput As INDArray, ByVal output As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(Nothing, New INDArray(){origInput1, origInput2, gradAtOutput}, (If(output Is Nothing, Nothing, New INDArray() {output})))
            Me.keepDimsField = keepDims
            Me.dimensionsField = dimensions
            addArgs()
        End Sub

        Public Sub New(ByVal origInput1 As INDArray, ByVal origInput2 As INDArray, ByVal gradAtOutput As INDArray, ByVal output1 As INDArray, ByVal output2 As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(Nothing, New INDArray(){origInput1, origInput2, gradAtOutput}, New INDArray(){output1, output2})
            Me.keepDimsField = keepDims
            Me.dimensionsField = dimensions
            addArgs()
        End Sub

        Protected Friend Overridable Sub addArgs()
            addTArgument(If(keepDimsField, 1, 0))
            If dimensionsField IsNot Nothing AndAlso dimensionsField.Length > 0 Then
                If dimensionsField.Length <> 1 OrElse dimensionsField(0) <> Integer.MaxValue Then
                    'Integer.MAX_VALUE means "full array" but here no dimension args == full array
                    addIArgument(dimensionsField)
                End If
            End If
        End Sub

        Public Overrides MustOverride Function opName() As String

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Reduction backprop ops: expect 2 inputs... the original input, and the gradient at the outputs
            'For example, for y=mean(x), inputs to ReduceMeanBp are x and dL/dy; output is dL/dx
            'Now, we expect gradient dL/dx datatype to be same as x - which resticts us to real-valued x input
            'i.e., 'gradient' of integer or boolean isn't defined
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatype for %s, got input %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0).isFPType(), "First input must be a floating point type, got %s", dataTypes(0))
            Preconditions.checkState(dataTypes(1).isFPType(), "Second input (gradient at reduction output) must be a floating point type, got %s", dataTypes(1))
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
