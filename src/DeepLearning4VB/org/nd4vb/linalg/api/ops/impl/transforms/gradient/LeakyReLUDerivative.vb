Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseScalarOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseScalarOp
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

    ''' <summary>The legacy scalar op <c>leakyreluderivative</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUDerivative</c>).</summary>
    Public Class LeakyReLUDerivative
        Inherits BaseScalarOp

        Public Sub New()
        End Sub

        Private alpha As Double = 0.01

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal alpha As Double)
			MyBase.New(sameDiff, i_v, alpha, inPlace)
			Me.alpha = alpha
			Me.extraArgs = New Object() {alpha}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal alpha As Double)
			Me.New(sameDiff, i_v, False, alpha)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object, ByVal alpha As Double)
			MyBase.New(sameDiff, i_v, alpha, extraArgs)
			Me.alpha = alpha
			Me.extraArgs = New Object() {alpha}
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            Me.New(x, z, 0.01)
        End Sub

        Public Sub New(ByVal x As INDArray)
            Me.New(x,x,0.01)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal alpha As Double)
            MyBase.New(x, Nothing, z, alpha)
            Me.alpha = alpha
            Me.extraArgsField = New Object() {alpha}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal alpha As Double)
            MyBase.New(x, alpha)
            Me.alpha = alpha
            Me.extraArgsField = New Object() {alpha}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 36
        End Function

        Public Overrides Function opName() As String
            Return "leakyreluderivative"
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

    End Class

End Namespace
