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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent

    ''' <summary>The custom op <c>gru</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.GRU</c>).</summary>
    Public Class GRU
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public GRU(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable hI, @NonNull SDVariable Wx, @NonNull SDVariable Wh, @NonNull SDVariable biases)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal hI As SDVariable, ByVal Wx As SDVariable, ByVal Wh As SDVariable, ByVal biases As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){x, hI, Wx, Wh, biases})

		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal hI As INDArray, ByVal Wx As INDArray, ByVal Wh As INDArray, ByVal biases As INDArray)
            MyBase.New(New INDArray(){x, hI, Wx, Wh, biases}, Nothing)

        End Sub

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 5, "Expected 5 inputs to GRU: initial cell output, input-to-hidden weights, hidden-to-hidden weights and biases got %s", inputDataTypes)
            Dim dt As DataType = inputDataTypes(1)
            For i As Integer = 0 To inputDataTypes.Count - 1
                Preconditions.checkState(inputDataTypes(i).isFPType(), "All input types must be a floating point type, got %s", dt)
            Next i
            Preconditions.checkState(dt.isFPType(), "Input type 1 must be a floating point type, got %s", dt)
            Return New List(Of DataType) From {dt}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {(New GRUBp(sameDiff, arg(0), arg(1), arg(2), arg(3), arg(4), grads(0))).outputVariables()}
		End Function
#End If

        Public Overrides Function opName() As String
            Return "gru"
        End Function

    End Class

End Namespace
