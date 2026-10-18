Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports MulBpOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.MulBpOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.pairwise.arithmetic

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp</c>.</summary>
    Public Class MulOp
        Inherits BaseDynamicTransformOp

        Public Const OP_NAME As String = "multiply"

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MulOp(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable y)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){x, y}, False)
		End Sub
#End If

        Public Sub New(ByVal first As INDArray, ByVal second As INDArray)
            Me.New(first, second, Nothing)
        End Sub

        Public Sub New(ByVal first As INDArray, ByVal second As INDArray, ByVal result As INDArray)
            Me.New(New INDArray(){first, second}, wrapOrNull(result))
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(inputs, outputs)
        End Sub

        Public Overrides Function opName() As String
            Return OP_NAME
        End Function

        Public Overrides Function onnxName() As String
            Return "Mul"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Mul"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New MulBpOp(sameDiff, larg(), rarg(), i_v(0))).outputs()
		End Function
#End If

    End Class

End Namespace
