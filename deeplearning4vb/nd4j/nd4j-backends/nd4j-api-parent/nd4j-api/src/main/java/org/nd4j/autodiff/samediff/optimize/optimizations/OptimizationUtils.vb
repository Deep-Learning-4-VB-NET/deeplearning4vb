Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable

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

Namespace org.nd4j.autodiff.samediff.optimize.optimizations


	Public Class OptimizationUtils

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void replaceOpInputsWith(org.nd4j.autodiff.samediff.SameDiff sd, @NonNull String replaceInput, @NonNull String newInput)
		Public Shared Sub replaceOpInputsWith(ByVal sd As SameDiff, ByVal replaceInput As String, ByVal newInput As String)
			If replaceInput.Equals(newInput) Then
				Return
			End If

			'Update op input structure: Replace all instances replaceInput->X with newInput->X
			Dim ops As ICollection(Of SameDiffOp) = sd.getOps().values()
			For Each o As SameDiffOp In ops
				Dim l As IList(Of String) = o.getInputsToOp()
				Do While l IsNot Nothing AndAlso l.Contains(replaceInput)
					Dim idx As Integer = l.IndexOf(replaceInput)
					l(idx) = newInput
				Loop
			Next o

			'Update variable structure
			Dim v As Variable = sd.getVariables().get(replaceInput)
			Dim v2 As Variable = sd.getVariables().get(newInput)
			'NOTE: this only works if we carefully control the order in which replaceOpInputsWith is called!
			v2.setInputsForOp(v.getInputsForOp())
			v.setInputsForOp(New List(Of String)())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void removeOp(@NonNull SameDiff sd, @NonNull String opToRemove)
		Public Shared Sub removeOp(ByVal sd As SameDiff, ByVal opToRemove As String)
			Dim op As SameDiffOp = sd.getOps().remove(opToRemove)
			For Each s As String In op.getInputsToOp()
				Dim v As Variable = sd.getVariables().get(s)
				v.getInputsForOp().remove(op.Name)
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void removeVariable(@NonNull SameDiff sd, @NonNull String varToRemove)
		Public Shared Sub removeVariable(ByVal sd As SameDiff, ByVal varToRemove As String)
			sd.getVariables().remove(varToRemove)
		End Sub

	End Class

End Namespace