Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports AveragingTransactionsHolder = org.nd4j.linalg.api.ops.performance.primitives.AveragingTransactionsHolder
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Slf4j @NativeTag public class PerformanceTrackerTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class PerformanceTrackerTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			PerformanceTracker.Instance.clear()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.BANDWIDTH
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			PerformanceTracker.Instance.clear()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAveragedHolder_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAveragedHolder_1(ByVal backend As Nd4jBackend)
			Dim holder As New AveragingTransactionsHolder()

			holder.addValue(MemcpyDirection.HOST_TO_HOST,50L)
			holder.addValue(MemcpyDirection.HOST_TO_HOST,150L)

			assertEquals(100L, holder.getAverageValue(MemcpyDirection.HOST_TO_HOST).Value)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAveragedHolder_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAveragedHolder_2(ByVal backend As Nd4jBackend)
			Dim holder As New AveragingTransactionsHolder()

			holder.addValue(MemcpyDirection.HOST_TO_HOST, 50L)
			holder.addValue(MemcpyDirection.HOST_TO_HOST,150L)
			holder.addValue(MemcpyDirection.HOST_TO_HOST,100L)

			assertEquals(100L, holder.getAverageValue(MemcpyDirection.HOST_TO_HOST).Value)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPerformanceTracker_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPerformanceTracker_1(ByVal backend As Nd4jBackend)
			Dim perf As PerformanceTracker = PerformanceTracker.Instance

			' 100 nanoseconds spent for 5000 bytes. result should be around 50000 bytes per microsecond
			Dim res As Long = perf.addMemoryTransaction(0, 100, 5000)
			assertEquals(50000, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPerformanceTracker_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPerformanceTracker_2(ByVal backend As Nd4jBackend)
			Dim perf As PerformanceTracker = PerformanceTracker.Instance

			' 10 nanoseconds spent for 5000 bytes. result should be around 500000 bytes per microsecond
			Dim res As Long = perf.addMemoryTransaction(0, 10, 5000, MemcpyDirection.HOST_TO_HOST)
			assertEquals(500000, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPerformanceTracker_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPerformanceTracker_3(ByVal backend As Nd4jBackend)
			Dim perf As val = PerformanceTracker.Instance

			' 10000 nanoseconds spent for 5000 bytes. result should be around 500 bytes per microsecond
			Dim res As val = perf.addMemoryTransaction(0, 10000, 5000)
			assertEquals(500, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testTrackerCpu_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTrackerCpu_1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			If Not Nd4j.Executioner.GetType().FullName.ToLower().contains("native") Then
				Return
			End If

			Dim fa(99999999) As Single
			Dim array As INDArray = Nd4j.create(fa, New Integer(){10000, 10000})

			Dim map As val = PerformanceTracker.Instance.getCurrentBandwidth()

			' getting H2H bandwidth
			Dim bw As val = map.get(0).get(MemcpyDirection.HOST_TO_HOST)
			log.info("H2H bandwidth: {}", map)

			assertTrue(bw > 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled("useless these days") public void testTrackerGpu_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTrackerGpu_1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			If Not Nd4j.Executioner.GetType().FullName.ToLower().contains("cuda") Then
				Return
			End If

			Dim fa As val = New Single(99999999){}
			Dim array As val = Nd4j.create(fa, New Integer(){10000, 10000})

			Dim map As val = PerformanceTracker.Instance.getCurrentBandwidth()

			' getting H2D bandwidth for device 0
			Dim bw As val = map.get(0).get(MemcpyDirection.HOST_TO_DEVICE)
			log.info("H2D bandwidth: {}", map)

			assertTrue(bw > 0)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace