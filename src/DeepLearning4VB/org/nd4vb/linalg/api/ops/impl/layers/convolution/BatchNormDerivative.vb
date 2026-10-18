Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.BatchNormDerivative</c>.</summary>
    Public Class BatchNormDerivative
        Inherits BatchNorm

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "derivativeBuilder") public BatchNormDerivative(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ndarray.INDArray[] inputArrays, org.nd4j.linalg.api.ndarray.INDArray[] outputArrays, boolean inPlace, boolean applyGamma, boolean applyBeta, double epsilon, int[] axis)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal inputArrays() As INDArray, ByVal outputArrays() As INDArray, ByVal inPlace As Boolean, ByVal applyGamma As Boolean, ByVal applyBeta As Boolean, ByVal epsilon As Double, ByVal axis() As Integer)
			MyBase.New(sameDiff, inputFunctions, inputArrays, outputArrays, inPlace, applyGamma, applyBeta, epsilon, axis)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "batchnorm_bp"
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op name found for " & opName())
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op name found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Unable to take derivative of derivative.")
		End Function
#End If

    End Class

End Namespace
