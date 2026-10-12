Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction

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

Namespace org.nd4j.autodiff.samediff.internal


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class SameDiffOp
	Public Class SameDiffOp
'JAVA TO VB CONVERTER NOTE: The field name was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend name_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field op was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend op_Conflict As DifferentialFunction 'Actual op (note: should be mutable: i.e., cloneable, no arrays set)
'JAVA TO VB CONVERTER NOTE: The field inputsToOp was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputsToOp_Conflict As IList(Of String) 'Name of SDVariables as input
'JAVA TO VB CONVERTER NOTE: The field outputsOfOp was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend outputsOfOp_Conflict As IList(Of String) 'Name of SDVariables as output
'JAVA TO VB CONVERTER NOTE: The field controlDeps was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend controlDeps_Conflict As IList(Of String) 'Name of SDVariables as control dependencies (not data inputs, but need to be available before exec)
'JAVA TO VB CONVERTER NOTE: The field varControlDeps was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend varControlDeps_Conflict As IList(Of String) 'Variables (constants, placeholders, etc) that are control dependencies for this op
'JAVA TO VB CONVERTER NOTE: The field controlDepFor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend controlDepFor_Conflict As IList(Of String) 'Name of the variables that this op is a control dependency for

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public SameDiffOp(String name, org.nd4j.autodiff.functions.DifferentialFunction op, java.util.List<String> inputsToOp, java.util.List<String> outputsOfOp, java.util.List<String> controlDeps, java.util.List<String> varControlDeps, java.util.List<String> controlDepFor)
		Public Sub New(ByVal name As String, ByVal op As DifferentialFunction, ByVal inputsToOp As IList(Of String), ByVal outputsOfOp As IList(Of String), ByVal controlDeps As IList(Of String), ByVal varControlDeps As IList(Of String), ByVal controlDepFor As IList(Of String))
			Me.name_Conflict = name
			Me.op_Conflict = op
			Me.inputsToOp_Conflict = inputsToOp
			Me.outputsOfOp_Conflict = outputsOfOp
			Me.controlDeps_Conflict = controlDeps
			Me.varControlDeps_Conflict = varControlDeps
			Me.controlDepFor_Conflict = controlDepFor
		End Sub

		Public Overridable Property Name As String
			Get
				Return name_Conflict
			End Get
			Set(ByVal name As String)
				Me.name_Conflict = name
			End Set
		End Property


		Public Overridable Property Op As DifferentialFunction
			Get
				Return op_Conflict
			End Get
			Set(ByVal op As DifferentialFunction)
				Me.op_Conflict = op
			End Set
		End Property


		Public Overridable Property InputsToOp As IList(Of String)
			Get
				Return inputsToOp_Conflict
			End Get
			Set(ByVal inputsToOp As IList(Of String))
				Me.inputsToOp_Conflict = inputsToOp
			End Set
		End Property


		Public Overridable Property OutputsOfOp As IList(Of String)
			Get
				Return outputsOfOp_Conflict
			End Get
			Set(ByVal outputsOfOp As IList(Of String))
				Me.outputsOfOp_Conflict = outputsOfOp
			End Set
		End Property


		Public Overridable Property ControlDeps As IList(Of String)
			Get
				Return controlDeps_Conflict
			End Get
			Set(ByVal controlDeps As IList(Of String))
				Me.controlDeps_Conflict = controlDeps
			End Set
		End Property


		Public Overridable Property VarControlDeps As IList(Of String)
			Get
				Return varControlDeps_Conflict
			End Get
			Set(ByVal varControlDeps As IList(Of String))
				Me.varControlDeps_Conflict = varControlDeps
			End Set
		End Property


		Public Overridable Property ControlDepFor As IList(Of String)
			Get
				Return controlDepFor_Conflict
			End Get
			Set(ByVal controlDepFor As IList(Of String))
				Me.controlDepFor_Conflict = controlDepFor
			End Set
		End Property

	End Class

End Namespace