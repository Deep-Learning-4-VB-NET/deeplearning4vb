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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.gradient

    ''' <summary>The custom op <c>softmax_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftmaxBp</c>).</summary>
    Public Class SoftmaxBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal grad As SDVariable, ByVal dimension As Integer?)
			MyBase.New(Nothing, sd, New SDVariable(){input, grad})
			If dimension IsNot Nothing Then
				addIArgument(dimension)
			End If
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal grad As INDArray, ByVal dimension As Integer?)
            Me.New(input, grad, Nothing, dimension)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal grad As INDArray, ByVal output As INDArray, ByVal dimension As Integer?)
            MyBase.New(New INDArray(){input, grad}, wrapOrNull(output))
            If dimension IsNot Nothing Then
                addIArgument(dimension.Value)
            End If
        End Sub

        Public Overrides Function opName() As String
            Return "softmax_bp"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentiating op softmax_bp not supported")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 inputs datatype for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0).isFPType(), "Input 0 must be a floating point type, got %s", dataTypes(0))
            Preconditions.checkState(dataTypes(1).isFPType(), "Input 1 must be a floating point type, got %s", dataTypes(1))
            Preconditions.checkState(dataTypes(0) = dataTypes(1), "Both input must be same type: got %s", dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
