Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports OptimizationDebugger = org.nd4j.autodiff.samediff.optimize.debug.OptimizationDebugger
Imports org.nd4j.autodiff.samediff.optimize.optimizations

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

Namespace org.nd4j.autodiff.samediff.optimize


	''' 
	''' <summary>
	''' @author Alex Black
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class GraphOptimizer
	Public Class GraphOptimizer

		Public Shared Function defaultOptimizations() As IList(Of OptimizerSet)
			Return New List(Of OptimizerSet) From {Of OptimizerSet}
		End Function

		Public Shared Function optimize(ByVal graph As SameDiff, ParamArray ByVal requiredOutputs() As String) As SameDiff
			Return optimize(graph, Arrays.asList(requiredOutputs))
		End Function

		Public Shared Function optimize(ByVal graph As SameDiff, ByVal requiredOutputs As IList(Of String)) As SameDiff
			Return optimize(graph, requiredOutputs, defaultOptimizations())
		End Function

		Public Shared Function optimize(ByVal graph As SameDiff, ByVal requiredOutputs As IList(Of String), ByVal optimizations As IList(Of OptimizerSet)) As SameDiff
			Return optimize(graph, requiredOutputs, optimizations, Nothing)
		End Function

		Public Shared Function optimize(ByVal graph As SameDiff, ByVal requiredOutputs As IList(Of String), ByVal optimizations As IList(Of OptimizerSet), ByVal debugger As OptimizationDebugger) As SameDiff
			'TODO Use required outputs - strip unnecessary graph components

			Dim sd As SameDiff = graph.dup()

			Dim cArr As ArrayHolder = sd.getConstantArrays()
			Dim vArr As ArrayHolder = sd.getVariablesArrays()

			Dim h As New OptimizationHelper(graph, New OptimizationConfig()) 'TODO defaults for config

			For i As Integer = 0 To 2 'Run multiple times - one run isn't enough, as some more optimizations may need to be applied to the output of earlier optimizations
				For Each s As OptimizerSet In optimizations
					Dim l As IList(Of Optimizer) = s.getOptimizers()
					For Each o As Optimizer In l
						Dim startingOps As ICollection(Of SameDiffOp) = New List(Of SameDiffOp)(sd.getOps().values()) 'Create list to avoid concurrent modification exception
						For Each op As SameDiffOp In startingOps
							'Because ops might disappear from previous optimization steps, we need to check if the previous op
							' still exists when iterating...
							If Not sd.getOps().containsKey(op.Name) Then
								Continue For
							End If

							If debugger IsNot Nothing Then
								debugger.beforeOptimizationCheck(sd, op, o)
							End If

							Dim applied As Boolean = o.checkAndApply(sd, h, op, cArr, vArr)
							If applied Then
								log.info("Operation was applied: {}", o)
							End If

							If debugger IsNot Nothing Then
								debugger.afterOptimizationsCheck(sd, op, o, applied)
							End If
						Next op
					Next o
				Next s
			Next i

			Dim constBefore As Integer = 0
			Dim constAfter As Integer = 0
			Dim varBefore As Integer = 0
			Dim varAfter As Integer = 0
			Dim arrBefore As Integer = 0
			Dim arrAfter As Integer = 0

			For Each v As SDVariable In graph.variables()
				Select Case v.getVariableType()
					Case VARIABLE
						varBefore += 1
					Case CONSTANT
						constBefore += 1
					Case ARRAY
						arrBefore += 1
					Case PLACEHOLDER
				End Select
			Next v

			For Each v As SDVariable In sd.variables()
				Select Case v.getVariableType()
					Case VARIABLE
						varAfter += 1
					Case CONSTANT
						constAfter += 1
					Case ARRAY
						arrAfter += 1
					Case PLACEHOLDER
				End Select
			Next v


			log.info("Total variables: {} before, {} after", graph.getVariables().size(), sd.getVariables().size())
			log.info("Constant variables: {} before, {} after", constBefore, constAfter)
			log.info("Array type variables: {} before, {} after", arrBefore, arrAfter)
			log.info("Variable type variables: {} before, {} after", varBefore, varAfter)
			log.info("Ops: {} before, {} after", graph.getOps().size(), sd.getOps().size())

			Return sd
		End Function

	End Class

End Namespace