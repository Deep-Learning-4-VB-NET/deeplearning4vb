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

    ''' <summary>The legacy scalar op <c>log_x</c> (Java <c>org.nd4j.linalg.api.ops.impl.scalar.LogX</c>).</summary>
    Public Class LogX
        Inherits BaseScalarOp

        Private base As Double

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal base As Double)
			MyBase.New(sameDiff, i_v, base)
			Me.base = base
			Me.extraArgs = New Object() {base}
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal base As Double)
            MyBase.New(x, Nothing, z, base)
            Me.base = base
            Me.extraArgsField = New Object() {base}
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal base As Double)
            MyBase.New(x, base)
            Me.base = base
            Me.extraArgsField = New Object() {base}
        End Sub

        Public Overrides Function opNum() As Integer
            Return 38
        End Function

        Public Overrides Function opName() As String
            Return "log_x"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'dlog_b(x)/dx = 1/(x*log_e(b))

			Dim logb As Double = Math.Log(base)
			Return Collections.singletonList(f1(0).div(arg().mul(logb)))
		End Function
#End If

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No TensorFlow op found for " & Me.GetType().Name)
        End Function

    End Class

End Namespace
