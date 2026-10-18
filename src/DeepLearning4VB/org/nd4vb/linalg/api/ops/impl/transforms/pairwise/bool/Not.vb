Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseTransformBoolOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseTransformBoolOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.pairwise.bool

    ''' <summary>The legacy transform bool op <c>not</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Not</c>).</summary>
    Public Class [Not]
        Inherits BaseTransformBoolOp

        Protected Friend comparable As Double = 0.0

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, i_v, False)
			Me.extraArgs = New Object() {Me.comparable}
		End Sub
#End If

        Public Sub New()
            Me.extraArgsField = New Object() {Me.comparable}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal comparable As Object)
            MyBase.New(x, y, z)
            Me.comparable = comparable.doubleValue()
            Me.extraArgsField = New Object() {Me.comparable}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 10
        End Function

        Public Overrides Function opName() As String
            Return "not"
        End Function

        Public Overrides Function onnxName() As String
            Return "Not"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

    End Class

End Namespace
