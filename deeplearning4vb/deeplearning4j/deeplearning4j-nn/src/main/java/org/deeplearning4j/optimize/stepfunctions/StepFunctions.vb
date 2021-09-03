Imports System
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction

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

	Public Class StepFunctions

		Private Shared ReadOnly DEFAULT_STEP_FUNCTION_INSTANCE As New DefaultStepFunction()
		Private Shared ReadOnly GRADIENT_STEP_FUNCTION_INSTANCE As New GradientStepFunction()
		Private Shared ReadOnly NEGATIVE_DEFAULT_STEP_FUNCTION_INSTANCE As New NegativeDefaultStepFunction()
		Private Shared ReadOnly NEGATIVE_GRADIENT_STEP_FUNCTION_INSTANCE As New NegativeGradientStepFunction()

		Private Sub New()
		End Sub

		Public Shared Function createStepFunction(ByVal stepFunction As org.deeplearning4j.nn.conf.stepfunctions.StepFunction) As StepFunction
			If stepFunction Is Nothing Then
				Return Nothing
			End If
			If TypeOf stepFunction Is org.deeplearning4j.nn.conf.stepfunctions.DefaultStepFunction Then
				Return DEFAULT_STEP_FUNCTION_INSTANCE
			End If
			If TypeOf stepFunction Is org.deeplearning4j.nn.conf.stepfunctions.GradientStepFunction Then
				Return GRADIENT_STEP_FUNCTION_INSTANCE
			End If
			If TypeOf stepFunction Is org.deeplearning4j.nn.conf.stepfunctions.NegativeDefaultStepFunction Then
				Return NEGATIVE_DEFAULT_STEP_FUNCTION_INSTANCE
			End If
			If TypeOf stepFunction Is org.deeplearning4j.nn.conf.stepfunctions.NegativeGradientStepFunction Then
				Return NEGATIVE_GRADIENT_STEP_FUNCTION_INSTANCE
			End If

			Throw New Exception("unknown step function: " & stepFunction)
		End Function
	End Class

End Namespace