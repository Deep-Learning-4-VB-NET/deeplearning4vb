Imports System
Imports System.Collections.Generic
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

    ''' <summary>The legacy scalar op <c>sub_scalar</c> (Java <c>org.nd4j.linalg.api.ops.impl.scalar.ScalarSubtraction</c>).</summary>
    Public Class ScalarSubtraction
        Inherits BaseScalarOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal num As Object)
            MyBase.New(x, y, z, num)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal num As Object)
            MyBase.New(x, num)
        End Sub

        Public Sub New(ByVal x As INDArray)
            Me.New(x, 0)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number)
			MyBase.New(sameDiff, i_v, scalar)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, scalar, inPlace)
		End Sub
#End If

        Public Overrides Function opNum() As Integer
            Return 1
        End Function

        Public Overrides Function opName() As String
            Return "sub_scalar"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim g As SDVariable = i_v1(0)

			Return New List(Of SDVariable) From {g}
		End Function
#End If

    End Class

End Namespace
