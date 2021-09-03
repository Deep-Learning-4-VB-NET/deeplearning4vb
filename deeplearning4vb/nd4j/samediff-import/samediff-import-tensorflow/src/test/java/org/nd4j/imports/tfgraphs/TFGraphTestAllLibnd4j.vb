Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.junit.jupiter.api
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TFGraphTestAllLibnd4j
	Public Class TFGraphTestAllLibnd4j 'Note: Can't extend BaseNd4jTest here as we need no-arg constructor for parameterized tests

		Private Const EXECUTE_WITH As TFGraphTestAllHelper.ExecuteWith = TFGraphTestAllHelper.ExecuteWith.LIBND4J
		Private Const BASE_DIR As String = "tf_graphs/examples"
		Private Const MODEL_FILENAME As String = "frozen_model.pb"

		Private Shared ReadOnly SKIP_FOR_LIBND4J_EXEC() As String = { "alpha_dropout/.*", "layers_dropout/.*", "g_06", "simpleif.*", "simple_cond.*", "cond/cond_true", "simplewhile_.*", "simple_while", "while1/.*", "while2/a", "tensor_array/.*", "primitive_gru_dynamic", "rnn/basiclstmcell/dynamic.*", "rnn/basicrnncell/dynamic.*", "rnn/bidir_basic/dynamic.*", "rnn/fused_adapt_basic/dynamic.*", "rnn/grucell/dynamic.*", "rnn/lstmcell/dynamic.*", "rnn/srucell/dynamic.*", "rnn/grublockcellv2/.*", "rnn/lstmblockcell/.*", "rnn/lstmblockfusedcell/.*"}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void beforeClass()
		Public Shared Sub beforeClass()
			Nd4j.DataType = DataType.FLOAT
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			Nd4j.DataType = DataType.FLOAT
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.stream.Stream<org.junit.jupiter.params.provider.Arguments> data() throws java.io.IOException
		Public Shared Function data() As Stream(Of Arguments)
			Dim localPath As val = Environment.GetEnvironmentVariable(TFGraphTestAllHelper.resourceFolderVar)

			' if this variable isn't set - we're using dl4j-tests-resources
			If localPath Is Nothing Then
				Dim baseDir As New File(System.getProperty("java.io.tmpdir"), System.Guid.randomUUID().ToString())
				Return TFGraphTestAllHelper.fetchTestParams(BASE_DIR, MODEL_FILENAME, EXECUTE_WITH, baseDir).Select(AddressOf Arguments.of)
			Else
				Dim baseDir As New File(localPath)
				Return TFGraphTestAllHelper.fetchTestParams(BASE_DIR, MODEL_FILENAME, EXECUTE_WITH, baseDir).Select(AddressOf Arguments.of)
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("data") public void testOutputOnly(Map<String, org.nd4j.linalg.api.ndarray.INDArray> inputs, Map<String, org.nd4j.linalg.api.ndarray.INDArray> predictions, String modelName, java.io.File localTestDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOutputOnly(ByVal inputs As IDictionary(Of String, INDArray), ByVal predictions As IDictionary(Of String, INDArray), ByVal modelName As String, ByVal localTestDir As File)
			Nd4j.create(1)
			For Each s As String In TFGraphTestAllSameDiff.IGNORE_REGEXES
				If modelName.matches(s) Then
					log.info(vbLf & vbTab & "IGNORE MODEL ON REGEX: {} - regex {}", modelName, s)
					assumeFalse(True)
				End If
			Next s

			For Each s As String In SKIP_FOR_LIBND4J_EXEC
				If modelName.matches(s) Then
					log.info(vbLf & vbTab & "IGNORE MODEL ON REGEX - SKIP LIBND4J EXEC ONLY: {} - regex {}", modelName, s)
					assumeFalse(True)
				End If
			Next s

			log.info("Starting test: {}", modelName)
			Dim precisionOverride As Pair(Of Double, Double) = TFGraphTestAllHelper.testPrecisionOverride(modelName)
			Dim maxRE As Double? = (If(precisionOverride Is Nothing, Nothing, precisionOverride.First))
			Dim minAbs As Double? = (If(precisionOverride Is Nothing, Nothing, precisionOverride.Second))

			TFGraphTestAllHelper.checkOnlyOutput(inputs, predictions, modelName, BASE_DIR, MODEL_FILENAME, EXECUTE_WITH, TFGraphTestAllHelper.LOADER, maxRE, minAbs, False)
			'TFGraphTestAllHelper.checkIntermediate(inputs, modelName, EXECUTE_WITH);
		End Sub

	End Class

End Namespace