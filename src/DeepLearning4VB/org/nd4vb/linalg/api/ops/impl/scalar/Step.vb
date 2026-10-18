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

Namespace org.nd4vb.linalg.api.ops.impl.scalar

    ''' <summary>The legacy scalar op <c>step</c> (Java <c>org.nd4j.linalg.api.ops.impl.scalar.Step</c>).</summary>
    Public Class [Step]
        Inherits BaseScalarOp

        Private ReadOnly cutoff As Double

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal cutoff As Double)
			MyBase.New(sameDiff, i_v, cutoff, inPlace)
			Me.cutoff = cutoff
			Me.extraArgs = New Object() {cutoff}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal cutoff As Double)
			Me.New(sameDiff, i_v, False, cutoff)
		End Sub
#End If

        Public Sub New()
            cutoff = 0.0
            Me.extraArgsField = New Object() {cutoff}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(x, z, 0.0)
            cutoff = 0.0
            Me.extraArgsField = New Object() {cutoff}
        End Sub

        Public Sub New(ByVal x As INDArray)
            MyBase.New(x, 0.0)
            cutoff = 0.0
            Me.extraArgsField = New Object() {cutoff}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal cutoff As Double)
            MyBase.New(x, z, cutoff)
            Me.cutoff = cutoff
            Me.extraArgsField = New Object() {cutoff}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal cutoff As Double)
            MyBase.New(x, cutoff)
            Me.cutoff = cutoff
            Me.extraArgsField = New Object() {cutoff}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 41
        End Function

        Public Overrides Function opName() As String
            Return "step"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

    End Class

End Namespace
