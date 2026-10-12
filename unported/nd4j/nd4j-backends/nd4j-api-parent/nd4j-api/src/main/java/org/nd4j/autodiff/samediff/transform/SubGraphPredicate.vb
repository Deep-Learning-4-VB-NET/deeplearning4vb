Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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


	Public Class SubGraphPredicate
		Inherits OpPredicate

		Protected Friend ReadOnly root As OpPredicate
		Protected Friend inputCount As Integer? = Nothing
		Protected Friend outputCount As Integer? = Nothing
		Protected Friend opInputMatchPredicates As IDictionary(Of Integer, OpPredicate) = New Dictionary(Of Integer, OpPredicate)() 'Must match - but these are NOT included in the resultant subgraph
		Protected Friend opInputSubgraphPredicates As IDictionary(Of Integer, OpPredicate) = New Dictionary(Of Integer, OpPredicate)() 'Must match - and thare ARE incrluded in the resultant subgraph

		Protected Friend Sub New(ByVal root As OpPredicate)
			Me.root = root
		End Sub

		''' <summary>
		''' Determine if the subgraph, starting with the root function, matches the predicate
		''' </summary>
		''' <param name="sameDiff"> SameDiff instance the function belongs to </param>
		''' <param name="rootFn">   Function that defines the root of the subgraph </param>
		''' <returns> True if the subgraph mathes the predicate </returns>
		Public Overrides Function matches(ByVal sameDiff As SameDiff, ByVal rootFn As DifferentialFunction) As Boolean

			If Not root.matches(sameDiff, rootFn) Then
				Return False
			End If

			Dim inputs() As SDVariable = rootFn.args()
			Dim inCount As Integer = If(inputs Is Nothing, 0, inputs.Length)
			If inputCount IsNot Nothing Then
				If inCount <> Me.inputCount Then
					Return False
				End If
			End If

			Dim outputs() As SDVariable = rootFn.outputVariables()
			Dim outCount As Integer = If(outputs Is Nothing, 0, outputs.Length)
			If outputCount IsNot Nothing Then
				If outCount <> outputCount Then
					Return False
				End If
			End If

			For Each m As IDictionary(Of Integer, OpPredicate) In java.util.Arrays.asList(opInputMatchPredicates, opInputSubgraphPredicates)
				For Each e As KeyValuePair(Of Integer, OpPredicate) In m.SetOfKeyValuePairs()
					Dim inNum As Integer = e.Key
					If inNum >= inCount Then
						Return False
					End If

					Dim [in] As SDVariable = inputs(inNum)
					Dim df As DifferentialFunction = sameDiff.getVariableOutputOp([in].name())
					If df Is Nothing OrElse Not e.Value.matches(sameDiff, df) Then
						Return False
					End If
				Next e
			Next m

			Return True
		End Function

		''' <summary>
		''' Get the SubGraph that matches the predicate
		''' </summary>
		''' <param name="sd"> SameDiff instance the function belongs to </param>
		''' <param name="rootFn">   Function that defines the root of the subgraph </param>
		''' <returns> The subgraph that matches the predicate </returns>
		Public Overridable Function getSubGraph(ByVal sd As SameDiff, ByVal rootFn As DifferentialFunction) As SubGraph
			Preconditions.checkState(matches(sd, rootFn), "Root function does not match predicate")

			Dim childNodes As IList(Of DifferentialFunction) = New List(Of DifferentialFunction)()
			'Need to work out child nodes
			If opInputSubgraphPredicates.Count > 0 Then
				For Each entry As KeyValuePair(Of Integer, OpPredicate) In opInputSubgraphPredicates.SetOfKeyValuePairs()
					Dim p2 As OpPredicate = entry.Value
					Dim arg As SDVariable = rootFn.arg(entry.Key)
					Dim df As DifferentialFunction = sd.getVariableOutputOp(arg.name())
					If df IsNot Nothing Then
						childNodes.Add(df)

						If TypeOf p2 Is SubGraphPredicate Then
							Dim sg As SubGraph = DirectCast(p2, SubGraphPredicate).getSubGraph(sd, df)
							CType(childNodes, List(Of DifferentialFunction)).AddRange(sg.childNodes)
						End If
					End If
				Next entry
			End If

			Dim sg As SubGraph = SubGraph.builder().sameDiff(sd).rootNode(rootFn).childNodes(childNodes).build()

			Return sg
		End Function


		''' <summary>
		''' Create a SubGraphPredicate with the specified root predicate </summary>
		''' <param name="root"> Predicate for matching the root </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static SubGraphPredicate withRoot(@NonNull OpPredicate root)
		Public Shared Function withRoot(ByVal root As OpPredicate) As SubGraphPredicate
			Return New SubGraphPredicate(root)
		End Function

		''' <summary>
		''' Modify the current subgraph to match only if the function has the specified number of inputs </summary>
		''' <param name="inputCount"> Match only if the function has the specified number of inputs </param>
		Public Overridable Function withInputCount(ByVal inputCount As Integer) As SubGraphPredicate
			Me.inputCount = inputCount
			Return Me
		End Function

		''' <summary>
		''' Modify the current subgraph to match only if the function has the specified number of outputs </summary>
		''' <param name="outputCount"> Match only if the function has the specified number of outputs </param>
		Public Overridable Function withOutputCount(ByVal outputCount As Integer) As SubGraphPredicate
			Me.outputCount = outputCount
			Return Me
		End Function

		''' <summary>
		''' Require the subgraph to match the specified predicate for the specified input.<br>
		''' Note that this does NOT add the specified input to part of the subgraph<br>
		''' i.e., the subgraph matches if the input matches the predicate, but when returning the SubGraph itself, the
		''' function for this input is not added to the SubGraph </summary>
		''' <param name="inputNum">    Input number </param>
		''' <param name="opPredicate"> Predicate that the input must match </param>
		''' <returns> This predicate with the additional requirement added </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SubGraphPredicate withInputMatching(int inputNum, @NonNull OpPredicate opPredicate)
'JAVA TO VB CONVERTER NOTE: The parameter opPredicate was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function withInputMatching(ByVal inputNum As Integer, ByVal opPredicate_Conflict As OpPredicate) As SubGraphPredicate
			opInputMatchPredicates(inputNum) = opPredicate_Conflict
			Return Me
		End Function

		''' <summary>
		''' Require the subgraph to match the specified predicate for the specified input.<br>
		''' Note that this DOES add the specified input to part of the subgraph<br>
		''' i.e., the subgraph matches if the input matches the predicate, and when returning the SubGraph itself, the
		''' function for this input IS added to the SubGraph </summary>
		''' <param name="inputNum">    Input number </param>
		''' <param name="opPredicate"> Predicate that the input must match </param>
		''' <returns> This predicate with the additional requirement added </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SubGraphPredicate withInputSubgraph(int inputNum, @NonNull OpPredicate opPredicate)
'JAVA TO VB CONVERTER NOTE: The parameter opPredicate was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function withInputSubgraph(ByVal inputNum As Integer, ByVal opPredicate_Conflict As OpPredicate) As SubGraphPredicate
			opInputSubgraphPredicates(inputNum) = opPredicate_Conflict
			Return Me
		End Function
	End Class

End Namespace