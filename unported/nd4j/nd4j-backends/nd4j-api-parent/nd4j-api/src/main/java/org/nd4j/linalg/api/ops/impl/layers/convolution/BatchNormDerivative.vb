Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.api.ops.impl.layers.convolution



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BatchNormDerivative extends BatchNorm
	Public Class BatchNormDerivative
		Inherits BatchNorm

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "derivativeBuilder") public BatchNormDerivative(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ndarray.INDArray[] inputArrays, org.nd4j.linalg.api.ndarray.INDArray[] outputArrays, boolean inPlace, boolean applyGamma, boolean applyBeta, double epsilon, int[] axis)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal inputArrays() As INDArray, ByVal outputArrays() As INDArray, ByVal inPlace As Boolean, ByVal applyGamma As Boolean, ByVal applyBeta As Boolean, ByVal epsilon As Double, ByVal axis() As Integer)
			MyBase.New(sameDiff, inputFunctions, inputArrays, outputArrays, inPlace, applyGamma, applyBeta, epsilon, axis)
		End Sub

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

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Unable to take derivative of derivative.")
		End Function

	End Class

End Namespace