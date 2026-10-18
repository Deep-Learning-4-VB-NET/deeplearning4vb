Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
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

    ''' <summary>The legacy transform bool op <c>and</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.And</c>).</summary>
    Public Class [And]
        Inherits BaseTransformBoolOp

        Protected Friend comparable As Double = 0.0

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal ix As SDVariable, ByVal iy As SDVariable)
			MyBase.New(sameDiff, ix, iy)
			Me.extraArgs = New Object() {Me.comparable}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
			Me.extraArgs = New Object() {Me.comparable}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal comparable As Double)
			MyBase.New(sameDiff, i_v, inPlace)
			Me.comparable = comparable
			Me.extraArgs = New Object() {Me.comparable}
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
            Me.New(x, y, 0.0)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal comparable As Object)
            Me.New(x, y, x, comparable)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
            Me.New(x, y, z, 0.0)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal comparable As Object)
            MyBase.New(x, y, z)
            Me.comparable = comparable.doubleValue()
            Me.extraArgsField = New Object() {Me.comparable}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 7
        End Function

        Public Overrides Function getOpType() As Op.Type
            Return Op.Type.PAIRWISE_BOOL
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.PAIRWISE_BOOL
        End Function

        Public Overrides Function opName() As String
            Return "and"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No Tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(larg()), sameDiff.zerosLike(rarg())}
		End Function
#End If

    End Class

End Namespace
