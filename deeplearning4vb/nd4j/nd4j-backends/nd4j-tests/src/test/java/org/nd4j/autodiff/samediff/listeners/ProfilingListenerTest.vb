Imports System
Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports ProfilingListener = org.nd4j.autodiff.listeners.profiler.ProfilingListener
Imports ProfileAnalyzer = org.nd4j.autodiff.listeners.profiler.comparison.ProfileAnalyzer
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse

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

Namespace org.nd4j.autodiff.samediff.listeners


	Public Class ProfilingListenerTest
		Inherits BaseNd4jTestWithBackends


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testProfilingListenerSimple(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testProfilingListenerSimple(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 3)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, 1, 2)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 3, 2))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 2))
			Dim sm As SDVariable = sd.nn_Conflict.softmax("predictions", [in].mmul("matmul", w).add("addbias", b))
			Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, sm)

			Dim i As INDArray = Nd4j.rand(DataType.FLOAT, 1, 3)
			Dim l As INDArray = Nd4j.rand(DataType.FLOAT, 1, 2)

			Dim testDir As Path = Paths.get((New File(System.getProperty("java.io.tmpdir"))).toURI())
			Dim dir As File = testDir.resolve("new-dir-" & System.Guid.randomUUID().ToString()).toFile()
			dir.mkdirs()
			Dim f As New File(dir, "test.json")
			f.deleteOnExit()
			Dim listener As ProfilingListener = ProfilingListener.builder(f).recordAll().warmup(5).build()

			sd.setListeners(listener)
			Dim ph As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			ph("in") = i

			For x As Integer = 0 To 9
				sd.outputSingle(ph, "predictions")
			Next x

			Dim content As String = FileUtils.readFileToString(f, StandardCharsets.UTF_8)
	'        System.out.println(content);
			assertFalse(content.Length = 0)
			'Should be 2 begins and 2 ends for each entry
			'5 warmup iterations, 5 profile iterations, x2 for both the op name and the op "instance" name
			Dim opNames() As String = {"matmul", "add", "softmax"}
			For Each s As String In opNames
				assertEquals(10, StringUtils.countMatches(content, s),s)
			Next s

			Console.WriteLine("///////////////////////////////////////////")
			'ProfileAnalyzer.summarizeProfile(f, ProfileAnalyzer.ProfileFormat.SAMEDIFF);

		End Sub
	End Class

End Namespace