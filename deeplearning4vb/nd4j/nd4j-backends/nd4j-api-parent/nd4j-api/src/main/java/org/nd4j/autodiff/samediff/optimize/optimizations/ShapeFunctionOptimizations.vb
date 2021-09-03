Imports System.Collections.Generic
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports OptimizationHelper = org.nd4j.autodiff.samediff.optimize.OptimizationHelper
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports Permute = org.nd4j.linalg.api.ops.impl.shape.Permute

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


	Public Class ShapeFunctionOptimizations
		Inherits BaseOptimizerSet

		''' <summary>
		''' Fuse [permute1 -> permute2 -> ... -> permuteN] into a single permute op,
		''' as long as the intermediate permute outputs aren't needed for another op
		''' </summary>
		Public Class FuseChainedPermutes
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				If Not (TypeOf op.Op Is Permute) Then
					Return False
				End If

				Dim inputs As IList(Of String) = op.getInputsToOp()
				Dim input As String = inputs(0)

				Dim toFuse As IList(Of String) = New List(Of String)()
				toFuse.Add(op.Name)
				Dim currInput As String = input
				Do While currInput IsNot Nothing
					Dim v As Variable = sd.getVariables().get(currInput)
					'In order to fuse permute operations, we require:
					' (a) the intermediate variable is ONLY needed by the next permute
					' (b) the permute dimensions are constant,

					If v.getInputsForOp().size() > 1 Then
						Exit Do
					End If
				Loop

				If toFuse.Count > 1 Then
					'Fuse the permute ops

	'                return true;
					Return False
				End If


				Return False
			End Function
		End Class

		''' <summary>
		''' Fuse [reshape1 -> reshape2 -> ... -> reshapeN] into a single reshape op,
		''' as long as the intermediate reshape ops aren't needed for another op
		''' </summary>
		Public Class FuseChainedReshapes
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				Return False
			End Function
		End Class

		''' <summary>
		''' Fuse [concat(concat(concat(x,y,dim=D), z, dim=D), a, dim=D)] into a single concat op, concat(x,y,z,a, dim=D)
		''' As long as the intermediate outputs aren't needed elsewhere
		''' </summary>
		Public Class FuseChainedConcatOps
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				Return False
			End Function
		End Class

	End Class

End Namespace