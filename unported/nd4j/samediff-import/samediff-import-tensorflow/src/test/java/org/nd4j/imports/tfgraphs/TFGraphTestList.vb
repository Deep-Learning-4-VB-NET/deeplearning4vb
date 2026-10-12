Imports System.Collections.Generic
Imports org.junit.jupiter.api
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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
'ORIGINAL LINE: @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TFGraphTestList
	Public Class TFGraphTestList


		'Only enable this for debugging, and leave it disabled for normal testing and CI - it prints all arrays for every execution step
		'Implemented internally using ExecPrintListener
		Public Const printArraysDebugging As Boolean = False

		Public Shared modelNames() As String = { "resize_nearest_neighbor/int32" }

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub

		'change this to SAMEDIFF for samediff
		Public Shared executeWith As TFGraphTestAllHelper.ExecuteWith = TFGraphTestAllHelper.ExecuteWith.SAMEDIFF
	'    public static TFGraphTestAllHelper.ExecuteWith executeWith = TFGraphTestAllHelper.ExecuteWith.LIBND4J;
		' public static TFGraphTestAllHelper.ExecuteWith executeWith = TFGraphTestAllHelper.ExecuteWith.JUST_PRINT;

		Public Const MODEL_DIR As String = "tf_graphs/examples"
		Public Const MODEL_FILENAME As String = "frozen_model.pb"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void beforeClass()
		Public Shared Sub beforeClass()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub

		Private modelName As String


		Public Shared Function data() As Stream(Of Arguments)
			Dim modelNamesParams As IList(Of Object()) = New List(Of Object())()
			For i As Integer = 0 To modelNames.Length - 1
				Dim currentParams() As Object = New String(){modelNames(i)}
				modelNamesParams.Add(currentParams)
			Next i
			Return modelNamesParams.Select(AddressOf Arguments.of)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("#data") public void testOutputOnly(@TempDir Path testDir,String modelName) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOutputOnly(ByVal testDir As Path, ByVal modelName As String)
			'Nd4jCpu.Environment.getInstance().setUseMKLDNN(false);
			Dim dir As File = testDir.toFile()
			Dim inputs As IDictionary(Of String, INDArray) = TFGraphTestAllHelper.inputVars(modelName, MODEL_DIR, dir)
			Dim predictions As IDictionary(Of String, INDArray) = TFGraphTestAllHelper.outputVars(modelName, MODEL_DIR, dir)
			Dim precisionOverride As Pair(Of Double, Double) = TFGraphTestAllHelper.testPrecisionOverride(modelName)
			Dim maxRE As Double? = (If(precisionOverride Is Nothing, Nothing, precisionOverride.First))
			Dim minAbs As Double? = (If(precisionOverride Is Nothing, Nothing, precisionOverride.Second))

			TFGraphTestAllHelper.checkOnlyOutput(inputs, predictions, modelName, MODEL_DIR, MODEL_FILENAME, executeWith, TFGraphTestAllHelper.LOADER, maxRE, minAbs, printArraysDebugging)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("#data") public void testAlsoIntermediate(@TempDir Path testDir,String modelName) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAlsoIntermediate(ByVal testDir As Path, ByVal modelName As String)
			'Nd4jCpu.Environment.getInstance().setUseMKLDNN(false);
			Dim dir As File = testDir.toFile()
			Dim inputs As IDictionary(Of String, INDArray) = TFGraphTestAllHelper.inputVars(modelName, MODEL_DIR, dir)
			TFGraphTestAllHelper.checkIntermediate(inputs, modelName, MODEL_DIR, MODEL_FILENAME, executeWith, dir, printArraysDebugging)
		End Sub
	End Class

End Namespace