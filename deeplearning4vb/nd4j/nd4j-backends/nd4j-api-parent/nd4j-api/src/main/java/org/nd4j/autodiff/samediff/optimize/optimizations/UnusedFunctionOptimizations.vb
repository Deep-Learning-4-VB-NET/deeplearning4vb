Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports OptimizationHelper = org.nd4j.autodiff.samediff.optimize.OptimizationHelper
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports org.nd4j.common.function
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class UnusedFunctionOptimizations extends BaseOptimizerSet
	Public Class UnusedFunctionOptimizations
		Inherits BaseOptimizerSet

		Public Class RemoveUnusedConstants
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				'TODO check this once _per graph_ not per op
				Dim variables As IList(Of Variable) = New List(Of Variable)(sd.getVariables().values())
				Dim anyRemoved As Boolean = False
				For Each v As Variable In variables
					If v.getVariable().getVariableType() = VariableType.CONSTANT Then
						Dim inputFor As IList(Of String) = v.getInputsForOp()
						If inputFor Is Nothing OrElse inputFor.Count = 0 Then
							'This constant isn't used...

							'TODO let's put these on disk instead of keeping them in memory...
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray arr = v.getVariable().getArr();
							Dim arr As INDArray = v.getVariable().getArr()
							helper.arrayRecoveryFunction(v.getName(), New SupplierAnonymousInnerClass(Me))

							sd.getVariables().remove(v.getName())
							log.info("Removed unused constant: {}", v.getName())
							anyRemoved = True
						End If
					End If
				Next v
				Return anyRemoved
			End Function

			Private Class SupplierAnonymousInnerClass
				Implements Supplier(Of INDArray)

				Private ReadOnly outerInstance As RemoveUnusedConstants

				Public Sub New(ByVal outerInstance As RemoveUnusedConstants)
					Me.outerInstance = outerInstance
				End Sub

				Public Function get() As INDArray
					Return arr
				End Function
			End Class
		End Class

	End Class

End Namespace