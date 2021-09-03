Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.autodiff.samediff.transform


	Public Class GraphTransformUtil

		Private Sub New()
		End Sub

		''' <summary>
		''' Find all of the subgraphs that match the specified SubGraphPredicate and then replace them with a different subgraph.<br>
		''' Note that the original SameDiff instance is not modified; a copy is made, which is then modified and returned.
		''' <br>
		''' Note: For each subgraph to be replaced by SubGraphProcessor, its replacement should have the same number of output
		''' SDVariables.
		''' </summary>
		''' <param name="sd">        SameDiff instance to copy and modify </param>
		''' <param name="p">         SubGraphPredicate to define and select the subgraphs that should be modified or replaced </param>
		''' <param name="processor"> SubGraphProcessor is used to define how the subgraphs (selected by the SubGraphPredicate) should
		'''                  be modified/replaced </param>
		''' <returns> A SameDiff instance that has been modified </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff replaceSubgraphsMatching(@NonNull SameDiff sd, @NonNull SubGraphPredicate p, @NonNull SubGraphProcessor processor)
		Public Shared Function replaceSubgraphsMatching(ByVal sd As SameDiff, ByVal p As SubGraphPredicate, ByVal processor As SubGraphProcessor) As SameDiff
			'Make a copy so that if the transform fails part way through, we don't leave user with broken graph
			sd = sd.dup()

			Dim subgraphs As IList(Of SubGraph) = getSubgraphsMatching(sd, p)

			For Each sg As SubGraph In subgraphs
				Dim newOutputs As IList(Of SDVariable) = processor.processSubgraph(sd, sg)
				Dim oldOutputs As IList(Of SDVariable) = sg.outputs()
				Preconditions.checkState(oldOutputs.Count = newOutputs.Count, "Error applying subgraph processor: " & "different number of outputs for subgraph (%s) vs. returned by preprocessor (%s)", oldOutputs.Count, newOutputs.Count)

				'Step 1: replace the old outputs with new outputs
				'So for initial graph (x -> y -> z) and post application of processor we now have (x -> (y, A); y->z),
				' we want to end up with (x -> A -> z)
				Dim allSubGraphFns As IList(Of DifferentialFunction) = sg.allFunctionsInSubgraph()
				For i As Integer = 0 To oldOutputs.Count - 1
					Dim oldOutVarName As String = oldOutputs(i).name()
					Dim newOutVarName As String = newOutputs(i).name()
					Preconditions.checkState(Not oldOutVarName.Equals(newOutVarName), "Reusing old variables not yet implemented")

					'Update inputs for ops: if X->opA, and now Y->opA, then X.inputsForOps contains "opA"; Y.inputsForOps should be updated
					Dim oldInputsForOps As IList(Of String) = sd.getVariables().get(oldOutVarName).getInputsForOp()
					If oldInputsForOps IsNot Nothing Then
						Dim newInputsForOps As IList(Of String) = New List(Of String)()
						For Each s As String In oldInputsForOps
							Dim df As DifferentialFunction = sd.getOpById(s)
							If Not allSubGraphFns.Contains(df) Then
								newInputsForOps.Add(s)
							End If
						Next s
						sd.getVariables().get(newOutVarName).setInputsForOp(newInputsForOps)
					End If


					'Basically: anywhere that oldName exists, newName should be substituted
					For Each v As Variable In sd.getVariables().values()
						' if control dep v -> oldOutput exists, replace it
						If v.getControlDepsForVar() IsNot Nothing Then
							Dim cds As IList(Of String) = v.getControlDepsForVar()
							Dim idx As Integer
							idx = cds.IndexOf(oldOutVarName)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((idx = cds.indexOf(oldOutVarName)) > 0)
							Do While idx > 0
								cds(idx) = newOutVarName
									idx = cds.IndexOf(oldOutVarName)
							Loop
						End If

						If v.getControlDeps() IsNot Nothing Then
							Dim cds As IList(Of String) = v.getControlDeps()
							'Control dependency oldOutput -> v exists, replace it
							Dim idx As Integer
							idx = cds.IndexOf(oldOutVarName)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((idx = cds.indexOf(oldOutVarName)) > 0)
							Do While idx > 0
								cds(idx) = newOutVarName
									idx = cds.IndexOf(oldOutVarName)
							Loop
						End If
					Next v

					For Each op As SameDiffOp In sd.getOps().values()
						Dim inputsToOp As IList(Of String) = op.getInputsToOp()
						If inputsToOp IsNot Nothing Then
							Dim idx As Integer
							idx = inputsToOp.IndexOf(oldOutVarName)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((idx = inputsToOp.indexOf(oldOutVarName)) >= 0)
							Do While idx >= 0
								'Previous Op.inputs = {oldVarName, ...} - now {newVarName, ...}
								inputsToOp(idx) = newOutVarName
									idx = inputsToOp.IndexOf(oldOutVarName)
							Loop
						End If

						'Don't need to modify outputsOfOp - old outputs are only on functions to be removed anyway
						Dim controlDeps As IList(Of String) = op.getControlDeps()
						If controlDeps IsNot Nothing Then
							Dim idx As Integer
							idx = controlDeps.IndexOf(oldOutVarName)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((idx = controlDeps.indexOf(oldOutVarName)) >= 0)
							Do While idx >= 0
								'Previous Op.inputs = {oldVarName, ...} - now {newVarName, ...}
								controlDeps(idx) = newOutVarName
									idx = controlDeps.IndexOf(oldOutVarName)
							Loop
						End If
					Next op
				Next i

				'Step 2: Update input variables: if X -> (subgraph) exists, then X.inputsForOp needs to be updated
				Dim inputs As IList(Of SDVariable) = sg.inputs()
				For Each v As SDVariable In inputs
					Dim var As Variable = sd.getVariables().get(v.name())
					If var.getInputsForOp() IsNot Nothing Then
						Dim newInputsForOp As IList(Of String) = New List(Of String)(var.getInputsForOp())
						For Each opName As String In var.getInputsForOp()
							'Two possibilities here:
							' (1) variable is (was) input to op that has been removed - just remove from list
							' (2) variable is now connected directly as an output: (A->B->C) becomes (A->C)
							' For the latter case, this
							Dim df As DifferentialFunction = sd.getOpById(opName)
							If allSubGraphFns.Contains(df) Then
								newInputsForOp.Remove(opName)
							End If
						Next opName
						var.setInputsForOp(newInputsForOp)
					End If
				Next v


				'Step 3: Remove the old variables and old functions
				Dim ops As IDictionary(Of String, SameDiffOp) = sd.getOps()
				Dim vars As IDictionary(Of String, Variable) = sd.getVariables()

				For Each df As DifferentialFunction In sg.allFunctionsInSubgraph()
					ops.Remove(df.getOwnName())
					Dim outputs() As SDVariable = df.outputVariables()
					If outputs IsNot Nothing Then
						For Each v As SDVariable In outputs
							vars.Remove(v.name())
						Next v
					End If
				Next df
			Next sg

			Return sd
		End Function

		''' <summary>
		''' Get a list of all the subgraphs that match the specified predicate
		''' </summary>
		''' <param name="sd"> SameDiff instance to get the subgraphs for </param>
		''' <param name="p">  Subgraph predicate. This defines the subgraphs that should be selected in the SameDiff instance </param>
		''' <returns> Subgraphs </returns>
		Public Shared Function getSubgraphsMatching(ByVal sd As SameDiff, ByVal p As SubGraphPredicate) As IList(Of SubGraph)
			Dim [out] As IList(Of SubGraph) = New List(Of SubGraph)()
			For Each df As DifferentialFunction In sd.ops()
				If p.matches(sd, df) Then
					Dim sg As SubGraph = p.getSubGraph(sd, df)
					[out].Add(sg)
				End If
			Next df

			Return [out]
		End Function
	End Class

End Namespace