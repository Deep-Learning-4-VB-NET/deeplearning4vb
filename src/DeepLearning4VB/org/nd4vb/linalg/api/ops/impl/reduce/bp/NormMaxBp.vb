Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
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

Namespace org.nd4vb.linalg.api.ops.impl.reduce.bp

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.reduce.bp.NormMaxBp</c>.</summary>
    Public Class NormMaxBp
        Inherits BaseReductionBp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal origInput As SDVariable, ByVal gradAtOutput As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, origInput, gradAtOutput, keepDims, dimensions)
		End Sub
#End If

        Public Sub New(ByVal origInput As INDArray, ByVal gradAtOutput As INDArray, ByVal output As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(origInput, gradAtOutput, output, keepDims, dimensions)
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "reduce_norm_max_bp"
        End Function

    End Class

End Namespace
