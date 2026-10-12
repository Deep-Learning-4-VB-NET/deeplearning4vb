Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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

Namespace org.nd4j.autodiff.samediff.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Builder @Data public class SubGraph
	Public Class SubGraph

		Protected Friend sameDiff As SameDiff
		Protected Friend rootNode As DifferentialFunction
		Protected Friend childNodes As IList(Of DifferentialFunction)


		Public Overridable Function outputs() As IList(Of SDVariable)
			'Outputs: the SDVariables of the root OR child nodes that are not consumed *ONLY* by another op within the subgraph
			Dim allOutputs As IList(Of SDVariable) = New List(Of SDVariable)()
			If rootNode.outputVariables() IsNot Nothing Then
				Collections.addAll(allOutputs, rootNode.outputVariables())
			End If
			If childNodes IsNot Nothing AndAlso childNodes.Count > 0 Then

				Dim seenAsInput As ISet(Of SDVariable) = New HashSet(Of SDVariable)()
				If rootNode.args() IsNot Nothing Then
					Collections.addAll(seenAsInput, rootNode.args())
				End If

				For Each df As DifferentialFunction In childNodes
					If df.args() IsNot Nothing Then
						Collections.addAll(seenAsInput, df.args())
					End If
					If df.outputVariables() IsNot Nothing Then
						Collections.addAll(allOutputs, df.outputVariables())
					End If
				Next df
			End If

			'Now: filter all output variables that are consumed *only* by
			'Example subgraph: x -> y -> z... then Y is not an output
			'But suppose same subgraph, but connection y -> a exists; then Y must be an output, because it's used somewhere else
			Dim filteredOutputs As IList(Of SDVariable) = New List(Of SDVariable)(allOutputs.Count)
			For Each v As SDVariable In allOutputs
				Dim var As Variable = sameDiff.getVariables().get(v.name())
				Dim inputsFor As IList(Of String) = var.getInputsForOp()
				Dim allInSubgraph As Boolean = True
				If inputsFor IsNot Nothing Then
					For Each opOwnName As String In inputsFor
						If Not inSubgraph(sameDiff.getOpById(opOwnName)) Then
							allInSubgraph = False
							Exit For
						End If
					Next opOwnName
				End If
				If Not allInSubgraph Then
					filteredOutputs.Add(v)
				End If
			Next v

			Return filteredOutputs
		End Function

		Public Overridable Function inputs() As IList(Of SDVariable)
			'Inputs: the SDVariables that are inputs to this subgraph are those used by any of the differential functions
			' (root or child nodes) that are NOT outputs of any of the child nodes

			Dim outputsOfSubgraphNodes As ISet(Of SDVariable) = New HashSet(Of SDVariable)()
			For Each df As DifferentialFunction In allFunctionsInSubgraph()
				Dim outputVars() As SDVariable = df.outputVariables()
				If outputVars IsNot Nothing Then
					Collections.addAll(outputsOfSubgraphNodes, outputVars)
				End If
			Next df

'JAVA TO VB CONVERTER NOTE: The local variable inputs was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim inputs_Conflict As IList(Of SDVariable) = New List(Of SDVariable)()
			For Each df As DifferentialFunction In allFunctionsInSubgraph()
				Dim args() As SDVariable = df.args()
				If args IsNot Nothing Then
					For Each arg As SDVariable In args
						If Not outputsOfSubgraphNodes.Contains(arg) Then
							inputs_Conflict.Add(arg)
						End If
					Next arg
				End If
			Next df


			Return inputs_Conflict
		End Function

		Public Overridable Function inSubgraph(ByVal df As DifferentialFunction) As Boolean
			If rootNode Is df Then
				Return True
			End If
			If childNodes IsNot Nothing Then
				For Each d As DifferentialFunction In childNodes
					If d Is df Then
						Return True
					End If
				Next d
			End If
			Return False
		End Function

		Public Overridable Function allFunctionsInSubgraph() As IList(Of DifferentialFunction)
			Dim [out] As IList(Of DifferentialFunction) = New List(Of DifferentialFunction)()
			[out].Add(rootNode)
			If childNodes IsNot Nothing Then
				CType([out], List(Of DifferentialFunction)).AddRange(childNodes)
			End If
			Return [out]
		End Function
	End Class

End Namespace