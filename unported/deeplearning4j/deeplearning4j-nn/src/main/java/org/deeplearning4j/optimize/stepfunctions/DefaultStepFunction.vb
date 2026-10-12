Imports System
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.optimize.stepfunctions

	<Serializable>
	Public Class DefaultStepFunction
		Implements StepFunction

		Private Const serialVersionUID As Long = -4707790524365648985L

		''' <summary>
		'''Does x = x + stepSize * line </summary>
		''' <param name="step"> step size. </param>
		Public Overridable Sub [step](ByVal parameters As INDArray, ByVal searchDirection As INDArray, ByVal [step] As Double) Implements StepFunction.step
			Nd4j.BlasWrapper.level1().axpy(searchDirection.length(), [step], searchDirection, parameters)
		End Sub

		Public Overridable Sub [step](ByVal x As INDArray, ByVal line As INDArray) Implements StepFunction.step
			[step](x, line, 1.0)
		End Sub

		Public Overridable Sub [step]() Implements StepFunction.step
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace