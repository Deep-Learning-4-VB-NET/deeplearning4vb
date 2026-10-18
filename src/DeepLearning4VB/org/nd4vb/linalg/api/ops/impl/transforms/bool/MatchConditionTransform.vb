Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseTransformBoolOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseTransformBoolOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports Condition = DeepLearning4VB.org.nd4vb.linalg.indexing.conditions.Condition
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.bool

    ''' <summary>The legacy transform bool op <c>match_condition_transform</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform</c>).</summary>
    Public Class MatchConditionTransform
        Inherits BaseTransformBoolOp

        Private condition As Condition
        Private compare As Double
        Private eps As Double
        Private mode As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal condition As Condition)
			MyBase.New(sameDiff, [in], False)
			Me.condition = condition
			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = Nd4j.EPS_THRESHOLD
			Me.extraArgs = New Object() {compare, eps, mode}
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal condition As Condition)
            Me.New(x, z, Nd4vbT.EPS_THRESHOLD, condition)
            Me.yField = y
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal condition As Condition)
            Me.New(x, z, Nd4vbT.EPS_THRESHOLD, condition)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal condition As Condition)
            Me.New(x, Nothing, Nd4vbT.EPS_THRESHOLD, condition)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal eps As Double, ByVal condition As Condition)
            MyBase.New(x, Nothing, z)

            Me.compare = condition.Value
            Me.mode = condition.condtionNum()
            Me.eps = eps

            Me.extraArgsField = New Object() {compare, eps, mode}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal eps As Double, ByVal condition As Condition)
            Me.New(x, Nothing, eps, condition)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 5
        End Function

        Public Overrides Function opName() As String
            Return "match_condition_transform"
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
