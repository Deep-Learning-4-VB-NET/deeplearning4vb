Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.junit.jupiter.api
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assumptions.assumeFalse

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

Namespace org.nd4j.imports.tfgraphs


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TFGraphTestAllSameDiff
	Public Class TFGraphTestAllSameDiff 'Note: Can't extend BaseNd4jTest here as we need no-arg constructor for parameterized tests

		Private Const EXECUTE_WITH As TFGraphTestAllHelper.ExecuteWith = TFGraphTestAllHelper.ExecuteWith.SAMEDIFF
		Private Const BASE_DIR As String = "tf_graphs/examples"
		Private Const MODEL_FILENAME As String = "frozen_model.pb"

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



'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.stream.Stream<org.junit.jupiter.params.provider.Arguments> data() throws java.io.IOException
		Public Shared Function data() As Stream(Of Arguments)
			Dim localPath As val = Environment.GetEnvironmentVariable(TFGraphTestAllHelper.resourceFolderVar)

			' if this variable isn't set - we're using dl4j-tests-resources
			If localPath Is Nothing Then
				Dim baseDir As New File(System.getProperty("java.io.tmpdir"), System.Guid.randomUUID().ToString())
				Dim params As IList(Of Object()) = TFGraphTestAllHelper.fetchTestParams(BASE_DIR, MODEL_FILENAME, EXECUTE_WITH, baseDir)
				Return params.Select(Function(input) Arguments.of(input))
			Else
				Dim baseDir As New File(localPath)
				Return TFGraphTestAllHelper.fetchTestParams(BASE_DIR, MODEL_FILENAME, EXECUTE_WITH, baseDir).Select(Function(input) Arguments.of(input))
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest(name = "{2}") @MethodSource("data") public void testOutputOnly(Map<String, org.nd4j.linalg.api.ndarray.INDArray> inputs, Map<String, org.nd4j.linalg.api.ndarray.INDArray> predictions, String modelName, java.io.File localTestDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOutputOnly(ByVal inputs As IDictionary(Of String, INDArray), ByVal predictions As IDictionary(Of String, INDArray), ByVal modelName As String, ByVal localTestDir As File)
			Nd4j.create(1)
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



			Dim precisionOverride As Pair(Of Double, Double) = TFGraphTestAllHelper.testPrecisionOverride(modelName)
			Dim maxRE As Double? = (If(precisionOverride Is Nothing, Nothing, precisionOverride.First))
			Dim minAbs As Double? = (If(precisionOverride Is Nothing, Nothing, precisionOverride.Second))

			Dim verboseDebugMode As Boolean = True
			If debugModeRegexes IsNot Nothing Then
				For Each regex As String In debugModeRegexes
					If modelName.matches(regex) Then
						verboseDebugMode = True
						Exit For
					End If
				Next regex
			End If

			Try
				' TFGraphTestAllHelper.checkIntermediate(inputs,modelName,BASE_DIR,MODEL_FILENAME,EXECUTE_WITH,TFGraphTestAllHelper.LOADER,maxRE,minAbs,localTestDir,true);

				TFGraphTestAllHelper.checkOnlyOutput(inputs, predictions, modelName, BASE_DIR, MODEL_FILENAME, EXECUTE_WITH, TFGraphTestAllHelper.LOADER, maxRE, minAbs, verboseDebugMode)
			Catch t As Exception
				log.error("ERROR Executing test: {} - input keys {}", modelName, (If(inputs Is Nothing, Nothing, inputs.Keys)), t)
				Throw t
			End Try
			'TFGraphTestAllHelper.checkIntermediate(inputs, modelName, EXECUTE_WITH);
		End Sub

	End Class

End Namespace