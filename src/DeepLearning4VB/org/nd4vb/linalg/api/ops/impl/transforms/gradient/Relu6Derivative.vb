Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

    ''' <summary>The custom op <c>relu6_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.gradient.Relu6Derivative</c>).</summary>
    Public Class Relu6Derivative
        Inherits DynamicCustomOp

        Private Const DEFAULT_CUTOFF As Double = 0.0

        Private cutoff As Double = DEFAULT_CUTOFF

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal cutoff As Double)
			MyBase.New("relu6_bp", sameDiff, New SDVariable(){i_v1, i_v2})
			Me.cutoff = cutoff
			Me.extraArgs = New Object(){cutoff}
		End Sub
#End If

        Public Sub New()
            Me.extraArgsField = New Object(){cutoff}
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal gradient As INDArray, ByVal output As INDArray)
            Me.New(input, gradient, output, DEFAULT_CUTOFF)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal gradient As INDArray, ByVal output As INDArray, ByVal cutoff As Double)
            MyBase.New(New INDArray(){input, gradient}, wrapOrNull(output))
            Me.cutoff = cutoff
            Me.extraArgsField = New Object(){cutoff}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 0
        End Function

        Public Overrides Function opName() As String
            Return "relu6_bp"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
