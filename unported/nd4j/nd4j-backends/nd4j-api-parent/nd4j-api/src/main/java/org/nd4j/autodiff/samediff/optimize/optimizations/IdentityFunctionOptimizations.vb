Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports OptimizationHelper = org.nd4j.autodiff.samediff.optimize.OptimizationHelper
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports Identity = org.nd4j.linalg.api.ops.impl.transforms.same.Identity

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


	Public Class IdentityFunctionOptimizations
		Inherits BaseOptimizerSet

		''' <summary>
		''' Remove permute(0,1,2,...,rank-1) as this is a no-op
		''' </summary>
		Public Class RemoveIdentityPermute
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				Return False
			End Function
		End Class

		''' <summary>
		''' Remove identity(x)
		''' </summary>
		Public Class RemoveIdentityOps
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				If TypeOf op.Op Is Identity Then
					Dim inName As String = op.getInputsToOp()(0)
					Dim outputName As String = op.getOutputsOfOp()(0)
					OptimizationUtils.removeOp(sd, op.Name)
					OptimizationUtils.replaceOpInputsWith(sd, outputName, inName)
					OptimizationUtils.removeVariable(sd, outputName)
					Return True
				End If

				Return False
			End Function
		End Class
	End Class

End Namespace