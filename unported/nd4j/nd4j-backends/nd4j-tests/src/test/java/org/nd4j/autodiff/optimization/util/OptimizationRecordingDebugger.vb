Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports OptimizationDebugger = org.nd4j.autodiff.samediff.optimize.debug.OptimizationDebugger

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
Namespace org.nd4j.autodiff.optimization.util


	Public Class OptimizationRecordingDebugger
		Implements OptimizationDebugger

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.Map<String,org.nd4j.autodiff.samediff.optimize.Optimizer> applied = new java.util.HashMap<>();
		Private applied As IDictionary(Of String, Optimizer) = New Dictionary(Of String, Optimizer)()

		Public Overridable Sub beforeOptimizationCheck(ByVal sd As SameDiff, ByVal op As SameDiffOp, ByVal o As Optimizer) Implements OptimizationDebugger.beforeOptimizationCheck
			'No op
		End Sub

		Public Overridable Sub afterOptimizationsCheck(ByVal sd As SameDiff, ByVal op As SameDiffOp, ByVal o As Optimizer, ByVal wasApplied As Boolean) Implements OptimizationDebugger.afterOptimizationsCheck
			If wasApplied Then
				applied(op.Name) = o
			End If
		End Sub
	End Class
End Namespace