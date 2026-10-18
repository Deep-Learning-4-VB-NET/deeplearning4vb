Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseTransformStrictOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseTransformStrictOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.strict

    ''' <summary>The legacy transform strict op <c>tan</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.strict.Tan</c>).</summary>
    Public Class Tan
        Inherits BaseTransformStrictOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			Me.New(sameDiff, i_v, False)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(x, z)
        End Sub

        Public Sub New(ByVal x As INDArray)
            MyBase.New(x)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 41
        End Function

        Public Overrides Function opName() As String
            Return "tan"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Tan"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'd(tan(x))/dx = (sec(x))^2 = 1 / (cos(x))^2
			Dim cosx As SDVariable = sameDiff.math_Conflict.cos(arg())
			Dim cosSqx As SDVariable = sameDiff.math().square(cosx)
			Return Collections.singletonList(i_v(0).div(cosSqx))
		End Function
#End If

    End Class

End Namespace
