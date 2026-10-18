Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseTransformSameOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseTransformSameOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.same

    ''' <summary>The legacy transform same op <c>Reciprocal</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.same.Reciprocal</c>).</summary>
    Public Class Reciprocal
        Inherits BaseTransformSameOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable)
			MyBase.New(sameDiff, [in], False)
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(x, z)
        End Sub

        Public Sub New(ByVal x As INDArray)
            MyBase.New(x)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 11
        End Function

        Public Overrides Function opName() As String
            Return "Reciprocal"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No  onnx opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Reciprocal"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"Reciprocal", "Inv"}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			' -1/(x^2)
			Dim g As SDVariable = sameDiff.math_Conflict.pow(arg(), 2).rdiv(-1).mul(i_v1(0))
			Return Collections.singletonList(g)
		End Function
#End If

    End Class

End Namespace
