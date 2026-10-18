Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SumBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.bp.SumBp
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

Namespace org.nd4vb.linalg.api.ops.impl.reduce.custom

    ''' <summary>The custom op <c>reduce_logsumexp</c> (Java <c>org.nd4j.linalg.api.ops.impl.reduce.custom.LogSumExp</c>).</summary>
    Public Class LogSumExp
        Inherits DynamicCustomOp

        Protected Friend keepDimsField As Boolean

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v)
			If dimensions IsNot Nothing Then
				addIArgument(dimensions)
				Me.dimensions = dimensions
			End If
			addTArgument(If(keepDims, 1.0, 0.0))
			Me.keepDims = keepDims
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			Me.New(sameDiff, i_v, False, dimensions)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
            Me.New(x, False, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal keepDim As Boolean, ParamArray ByVal dimensions() As Integer)
            Me.New(x, CType(Nothing, INDArray), keepDim, dimensions)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal keepDim As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(Nothing, x,z, JavaCollections.singletonList(If(keepDim, 1.0, 0.0)), dimensions)
        End Sub

        Public Overrides Function opName() As String
            Return "reduce_logsumexp"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'z = log(sum_i exp(x_i)) = log(s)
			'dL/dx = dL/dz * dz/ds * ds/dx
			'dz/ds = 1/s
			Dim exp As SDVariable = sameDiff.math_Conflict.exp(arg())
			Dim sumExp As SDVariable = exp.sum(dimensions)
			Dim gradProd As SDVariable = f1(0).div(sumExp)
			Dim dSumExpdx As SDVariable = (New SumBp(sameDiff, arg(), gradProd, keepDims, dimensions)).outputVariable().mul(exp)
			Return Collections.singletonList(dSumExpdx)
		End Function
#End If

        Public Overrides Function onnxName() As String
            Return "ReduceLogSumExp"
        End Function

    End Class

End Namespace
