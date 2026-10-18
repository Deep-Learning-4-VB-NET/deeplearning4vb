Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SoftmaxBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.gradient.SoftmaxBp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax</c>.</summary>
    Public Class SoftMax
        Inherits BaseDynamicTransformOp

        Public Sub New()
            MyBase.New()
        End Sub

        Private dimension As Integer = 1

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(sameDiff, args, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal dimension As Integer)
			Me.New(sameDiff, New SDVariable(){x}, dimension)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, args, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal dimension As Integer)
			MyBase.New(sameDiff, args, False)
			Me.dimension = dimension
			addIArgument(dimension)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal dimension As Integer, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, args, inPlace)
			Me.dimension = dimension
			addIArgument(dimension)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal dimension As Integer)
            Me.New(input, Nothing, dimension)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal result As INDArray, ByVal dimension As Integer)
            MyBase.New(New INDArray(){input}, wrapOrNull(result))
            Me.dimension = dimension
            addIArgument(dimension)
        End Sub

        Public Sub New(ByVal input As INDArray)
            Me.New(input, input)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal result As INDArray)
            Me.New(input, result, -1)
        End Sub

        Public Overrides Function opName() As String
            Return "softmax"
        End Function

        Public Overrides Function onnxName() As String
            Return "Softmax"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Softmax"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New SoftmaxBp(sameDiff, arg(), i_v(0), Me.dimension)).outputs()
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0).isFPType(), "Input must be a floating point type, got %s", dataTypes(0))
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
