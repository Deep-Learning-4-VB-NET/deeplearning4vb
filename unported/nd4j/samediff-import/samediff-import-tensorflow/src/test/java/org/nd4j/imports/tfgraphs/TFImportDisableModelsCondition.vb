Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ConditionEvaluationResult = org.junit.jupiter.api.extension.ConditionEvaluationResult
Imports ExecutionCondition = org.junit.jupiter.api.extension.ExecutionCondition
Imports ExtensionContext = org.junit.jupiter.api.extension.ExtensionContext
import static org.junit.jupiter.api.Assumptions.assumeFalse

'
' *
' *  *  ******************************************************************************
' *  *  *
' *  *  *
' *  *  * This program and the accompanying materials are made available under the
' *  *  * terms of the Apache License, Version 2.0 which is available at
' *  *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *  *
' *  *  *  See the NOTICE file distributed with this work for additional
' *  *  *  information regarding copyright ownership.
' *  *  * Unless required by applicable law or agreed to in writing, software
' *  *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  *  * License for the specific language governing permissions and limitations
' *  *  * under the License.
' *  *  *
' *  *  * SPDX-License-Identifier: Apache-2.0
' *  *  *****************************************************************************
' *
' *
' 

Namespace org.nd4j.imports.tfgraphs


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TFImportDisableModelsCondition implements org.junit.jupiter.api.extension.ExecutionCondition
	Public Class TFImportDisableModelsCondition
		Implements ExecutionCondition

		''' <summary>
		''' NOTE: If this is empty or the tests names are wrong,
		''' all tests will trigger an assumeFalse(..) that indicates
		''' the status of the test failing. No tests will run.
		''' </summary>
		Public Shared ReadOnly EXECUTE_ONLY_MODELS As IList(Of String) = New List(Of String) From {}

		Public Shared ReadOnly IGNORE_REGEXES() As String = {"scatter_nd_sub/locking/rank1shape_1indices", "reductions/scatter_update_vector", "reductions/scatter_update_scalar", "emptyArrayTests/scatter_update/rank1_emptyIndices_emptyUpdates", "bincount/rank2_weights", "slogdet/.*", "fused_batch_norm/float16_nhwc", "emptyArrayTests/scatter_update/rank2_emptyIndices_emptyUpdates", "layers_dropout/.*", "truncatemod/.*", "confusion/.*", "conv_4", "g_09", "g_11", "multinomial/.*", "conv3d_transpose.*", "ragged/reduce_mean/.*", "random_gamma/.*", "Conv3DBackpropInputV2/.*", "non_max_suppression_v4/.*", "non_max_suppression_v5/.*", "random_uniform_int/.*", "random_uniform/.*", "random_poisson_v2/.*"}

	'     As per TFGraphTestList.printArraysDebugging - this field defines a set of regexes for test cases that should have
	'       all arrays printed during execution.
	'       If a test name matches any regex here, an ExecPrintListener will be added to the listeners, and all output
	'       arrays will be printed during execution
	'     
		Private ReadOnly debugModeRegexes As IList(Of String) = New List(Of String) From {"fused_batch_norm/float16_nhwc"}



		Private Function getStore(ByVal context As ExtensionContext) As ExtensionContext.Store
			Return context.getStore(ExtensionContext.Namespace.create(Me.GetType(), context.getRequiredTestMethod()))
		End Function

		Public Overrides Function evaluateExecutionCondition(ByVal extensionContext As ExtensionContext) As ConditionEvaluationResult
			Dim modelName As String = getStore(extensionContext).ToString()
			If EXECUTE_ONLY_MODELS.Count = 0 Then
				For Each s As String In IGNORE_REGEXES
					If modelName.matches(s) Then
						log.info(vbLf & vbTab & "IGNORE MODEL ON REGEX: {} - regex {}", modelName, s)
						assumeFalse(True)
					End If
				Next s
			ElseIf Not EXECUTE_ONLY_MODELS.Contains(modelName) Then
				log.info("Not executing " & modelName)
				assumeFalse(True)
				'OpValidationSuite.ignoreFailing();
			End If


			Return ConditionEvaluationResult.disabled("Method found")
		End Function
	End Class

End Namespace