Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports IActivation = org.nd4j.linalg.activations.IActivation
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

Namespace org.nd4j.linalg.lossfunctions.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) public class LossMAE extends LossL1
	<Serializable>
	Public Class LossMAE
		Inherits LossL1

		Public Sub New()

		End Sub

		''' <summary>
		''' Mean Absolute Error loss function where each the output is (optionally) weighted/scaled by a flags scalar value.
		''' Note that the weights array must be a row vector, of length equal to the labels/output dimension 1 size.
		''' A weight vector of 1s should give identical results to no weight vector.
		''' </summary>
		''' <param name="weights"> Weights array (row vector). May be null. </param>
		Public Sub New(ByVal weights As INDArray)
			MyBase.New(weights)
		End Sub

		Public Overrides Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double

			Dim score As Double = MyBase.computeScore(labels, preOutput, activationFn, mask, average)
			score /= (labels.size(1))
			Return score
		End Function

		Public Overrides Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			Dim scoreArr As INDArray = MyBase.computeScoreArray(labels, preOutput, activationFn, mask)
			scoreArr.divi(scoreArr.size(1))
			Return scoreArr
		End Function

		Public Overrides Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			Dim gradients As INDArray = MyBase.computeGradient(labels, preOutput, activationFn, mask)
			gradients.divi(labels.size(1))
			Return gradients
		End Function

		''' <summary>
		''' The opName of this function
		''' 
		''' @return
		''' </summary>
		Public Overrides Function name() As String
			Return ToString()
		End Function


		Public Overrides Function ToString() As String
			If weights Is Nothing Then
				Return "LossMAE()"
			End If
			Return "LossMAE(weights=" & weights & ")"
		End Function
	End Class

End Namespace