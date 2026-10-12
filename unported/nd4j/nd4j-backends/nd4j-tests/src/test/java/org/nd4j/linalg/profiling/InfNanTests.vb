Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports OpExecutionerUtil = org.nd4j.linalg.api.ops.executioner.OpExecutionerUtil
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig
import static org.junit.jupiter.api.Assertions.assertThrows

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

Namespace org.nd4j.linalg.profiling

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Execution(ExecutionMode.SAME_THREAD) public class InfNanTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class InfNanTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
		   Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForINF(True).checkForNAN(True).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void cleanUp()
		Public Overridable Sub cleanUp()
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInf1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInf1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(True).checkForINF(True).build()
			Dim x As INDArray = Nd4j.create(100)
			x.putScalar(2, Single.NegativeInfinity)
			OpExecutionerUtil.checkForAny(x)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInf2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInf2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(True).checkForINF(True).build()
			Dim x As INDArray = Nd4j.create(100)
			x.putScalar(2, Single.NegativeInfinity)
			OpExecutionerUtil.checkForAny(x)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInf3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInf3(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(100)

			OpExecutionerUtil.checkForAny(x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInf4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInf4(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().build()

			Dim x As INDArray = Nd4j.create(100)

			OpExecutionerUtil.checkForAny(x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaN1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaN1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(True).build()
			Dim x As INDArray = Nd4j.create(100)
			x.putScalar(2, Float.NaN)
			OpExecutionerUtil.checkForAny(x)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaN2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaN2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForINF(True).checkForNAN(True).build()
			Dim x As INDArray = Nd4j.create(100)
			x.putScalar(2, Float.NaN)
			OpExecutionerUtil.checkForAny(x)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaN3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaN3(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForINF(True).checkForNAN(True).build()
			Dim x As INDArray = Nd4j.create(100)

			OpExecutionerUtil.checkForAny(x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaN4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaN4(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().build()
			Dim x As INDArray = Nd4j.create(100)

			OpExecutionerUtil.checkForAny(x)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace