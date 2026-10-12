Imports System

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

Namespace org.deeplearning4j.nn.conf.stepfunctions

	<Serializable>
	Public Class GradientStepFunction
		Inherits StepFunction

		Private Const serialVersionUID As Long = -2078308971477295356L

		Public Overrides Function GetHashCode() As Integer
			Return 0
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then
				Return True
			End If
			If obj Is Nothing Then
				Return False
			End If
			If Me.GetType() <> obj.GetType() Then
				Return False
			End If
			Return True
		End Function

		Public Overrides Function ToString() As String
			Return "GradientStepFunction{" & "}"c
		End Function
	End Class

End Namespace