Imports System.Collections.Generic
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports OptimizationHelper = org.nd4j.autodiff.samediff.optimize.OptimizationHelper
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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


	''' <summary>
	''' This set of optimizations looks for functions that are applied to constants, and "pre executes" them, so they don't have
	''' to be calculated (returning the same value) on each run.
	''' 
	''' @author Alex Black
	''' </summary>
	Public Class ConstantFunctionOptimizations
		Inherits BaseOptimizerSet

		Public Const CONSTANT_FN_FOLDING_MAX_SIZE As String = "optimizer.constants.function.max.output.size"
		Public Const CONSTANT_FN_FOLDING_MAX_SIZE_DEFAULT As Long = 4 * 1024 * 1024 '4MB

		Public Class FoldConstantFunctions
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				'TODO This function needs to check for non-deterministic ops - i.e., random ops - and not apply the optimization to these

				Dim [in] As IList(Of String) = op.getInputsToOp()
				If [in] Is Nothing OrElse [in].Count = 0 Then
					Return False
				End If
				For Each s As String In [in]
					If Not sd.getVariable(s).Constant Then
						Return False
					End If
				Next s

				Dim maxSizeToApply As Long = Long.Parse(helper.getProperties().getProperty(CONSTANT_FN_FOLDING_MAX_SIZE, CONSTANT_FN_FOLDING_MAX_SIZE_DEFAULT.ToString()))
				'Apply the optimization:
				Dim df As DifferentialFunction = op.Op
				df.clearArrays()
				For i As Integer = 0 To [in].Count - 1
					Dim s As String = [in](i)
					Dim arr As INDArray = sd.getVariable(s).Arr
					If TypeOf df Is CustomOp Then
						DirectCast(df, CustomOp).addInputArgument(arr)
					Else
						If i = 0 Then
							DirectCast(df, Op).X = arr
						Else
							DirectCast(df, Op).Y = arr
						End If
					End If
				Next i

				Dim outputs() As INDArray
				If TypeOf df Is CustomOp Then
					Dim o As CustomOp = DirectCast(df, CustomOp)
					Nd4j.exec(o)
					outputs = New INDArray(o.numOutputArguments() - 1){}
					For j As Integer = 0 To outputs.Length - 1
						outputs(j) = o.getOutputArgument(j)
					Next j
				Else
					Dim o As Op = DirectCast(df, Op)
					Nd4j.exec(o)
					outputs = New INDArray(){o.z()}
				End If
				Dim sizeCount As Long = 0
				For Each i As INDArray In outputs
					If Not i.dataType().isNumerical() Then
						Continue For
					End If
					sizeCount += i.length() * i.dataType().width()
				Next i

				If sizeCount > maxSizeToApply Then
					Return False
				End If

				'Convert outputs to constants
				Dim outputNames As IList(Of String) = op.getOutputsOfOp()
				For i As Integer = 0 To outputNames.Count - 1
					Dim n As String = outputNames(i)
					sd.getVariable(n).setVariableType(VariableType.CONSTANT)
					constantArrays.setArray(n, outputs(i))
					sd.getVariables().get(n).setOutputOfOp(Nothing)
				Next i

				'Remove the op
				OptimizationUtils.removeOp(sd, df.getOwnName())

				Return True
			End Function
		End Class
	End Class

End Namespace