Imports System
Imports System.Collections.Generic
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

Namespace org.nd4vb.linalg.api.ops.custom

    ''' <summary>The custom op <c>triu</c> (Java <c>org.nd4j.linalg.api.ops.custom.Triu</c>).</summary>
    Public Class Triu
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private diag As Integer = 0

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal diag As Integer)
			MyBase.New(sameDiff, New SDVariable(){[in]})
			addIArgument(diag)
			Me.diag=diag
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){[in]})
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal diag As Integer)
            MyBase.New(New INDArray(){input}, Nothing)
            addIArgument(diag)
            Me.diag=diag

        End Sub

        Public Overrides Function opName() As String
            Return "triu"
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(arg(0).dataType())
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)

			Return (New TriuBp(sameDiff, arg(0), f1(0), diag)).outputs()
		End Function
#End If

    End Class

End Namespace
