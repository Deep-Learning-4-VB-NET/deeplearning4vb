Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports Isolated = org.junit.jupiter.api.parallel.Isolated
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Concat = org.nd4j.linalg.api.ops.impl.shape.Concat
Imports Log = org.nd4j.linalg.api.ops.impl.transforms.strict.Log
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @Slf4j @NativeTag @Isolated @Execution(ExecutionMode.SAME_THREAD) public class OperationProfilerTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class OperationProfilerTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.OPERATIONS
			OpProfiler.Instance.reset()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.DISABLED
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCounter1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCounter1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.createUninitialized(100)

			array.assign(10f)
			array.divi(2f)

			assertEquals(2, OpProfiler.Instance.InvocationsCount)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStack1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStack1(ByVal backend As Nd4jBackend)

			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL

			Dim array As INDArray = Nd4j.createUninitialized(100)

			array.assign(10f)
			array.assign(20f)
			array.assign(30f)

			assertEquals(3, OpProfiler.Instance.InvocationsCount)

			OpProfiler.Instance.printOutDashboard()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadCombos1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadCombos1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(100)
			Dim y As INDArray = Nd4j.create(100)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processOperands(x, y)

			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.NONE))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadCombos2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadCombos2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(100).reshape("f"c, 10, 10)
			Dim y As INDArray = Nd4j.create(100).reshape("c"c, 10, 10)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processOperands(x, y)

			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.MIXED_ORDER))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadCombos3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadCombos3(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(27).reshape("c"c, 3, 3, 3).tensorAlongDimension(0, 1, 2)
			Dim y As INDArray = Nd4j.create(100).reshape("f"c, 10, 10)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processOperands(x, y)

	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.MIXED_ORDER))
			'assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.NON_EWS_ACCESS));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadCombos4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadCombos4(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(27).reshape("c"c, 3, 3, 3).tensorAlongDimension(0, 1, 2)
			Dim y As INDArray = Nd4j.create(100).reshape("f"c, 10, 10)
			Dim z As INDArray = Nd4j.create(100).reshape("f"c, 10, 10)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processOperands(x, y, z)

	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.MIXED_ORDER))
			'assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.NON_EWS_ACCESS));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadCombos5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadCombos5(ByVal backend As Nd4jBackend)
			Dim w As INDArray = Nd4j.create(100).reshape("c"c, 10, 10)
			Dim x As INDArray = Nd4j.create(100).reshape("c"c, 10, 10)
			Dim y As INDArray = Nd4j.create(100).reshape("f"c, 10, 10)
			Dim z As INDArray = Nd4j.create(100).reshape("c"c, 10, 10)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processOperands(w, x, y, z)

	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.MIXED_ORDER))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testBadCombos6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadCombos6(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(27).reshape("f"c, 3, 3, 3).slice(1)
			Dim y As INDArray = Nd4j.create(100).reshape("f"c, 10, 10)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processOperands(x, y)

	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.STRIDED_ACCESS))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadTad1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadTad1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(2, 4, 5, 6)

			Dim pair As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(x, 0, 2)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processTADOperands(pair.First)

	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.TAD_NON_EWS_ACCESS))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadTad2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadTad2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(2, 4, 5, 6)

			Dim pair As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(x, 2, 3)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processTADOperands(pair.First)

	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.TAD_NON_EWS_ACCESS))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadTad3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadTad3(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Integer() {2, 4, 5, 6, 7}, "f"c)

			Dim pair As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(x, 0, 2, 4)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processTADOperands(pair.First)

	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.TAD_NON_EWS_ACCESS))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadTad4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadTad4(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(DataType.DOUBLE,2, 4, 5, 6)

			Dim pair As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(x, 3)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processTADOperands(pair.First)

	'        log.info("TAD: {}", Arrays.toString(pair.getFirst().asInt()));
	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.NONE))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadTad5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadTad5(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Integer() {2, 4, 5, 6, 7}, "f"c)

			Dim pair As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(x, 4)

			Dim causes() As OpProfiler.PenaltyCause = OpProfiler.Instance.processTADOperands(pair.First)

	'        log.info("TAD: {}", Arrays.toString(pair.getFirst().asInt()));
	'        log.info("Causes: {}", Arrays.toString(causes));
			assertEquals(1, causes.Length)
			assertTrue(ArrayUtils.contains(causes, OpProfiler.PenaltyCause.TAD_STRIDED_ACCESS))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCxFxF1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCxFxF1(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.create(10, 10).reshape("f"c, 10, 10)
			Dim b As INDArray = Nd4j.create(10, 10).reshape("c"c, 10, 10)
			Dim c As INDArray = Nd4j.create(10, 10).reshape("f"c, 10, 10)

			Dim ret As String = OpProfiler.Instance.processOrders(a, b, c)
			assertEquals("F x C x F", ret)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCxFxF2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCxFxF2(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.create(10, 10).reshape("c"c, 10, 10)
			Dim b As INDArray = Nd4j.create(10, 10).reshape("c"c, 10, 10)
			Dim c As INDArray = Nd4j.create(10, 10).reshape("f"c, 10, 10)

			Dim ret As String = OpProfiler.Instance.processOrders(a, b, c)
			assertEquals("C x C x F", ret)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCxFxF3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCxFxF3(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.create(10, 10).reshape("c"c, 10, 10)
			Dim b As INDArray = Nd4j.create(10, 10).reshape("c"c, 10, 10)
			Dim c As INDArray = Nd4j.create(10, 10).reshape("c"c, 10, 10)

			Dim ret As String = OpProfiler.Instance.processOrders(a, b, c)
			assertEquals("C x C x C", ret)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasFF(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasFF(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL

			Dim a As INDArray = Nd4j.create(10, 10).reshape("f"c, 10, 10)
			Dim b As INDArray = Nd4j.create(10, 10).reshape("f"c, 10, 10)

			a.mmul(b)

			OpProfiler.Instance.printOutDashboard()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaNPanic1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaNPanic1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.NAN_PANIC
			Dim a As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, Float.NaN})
			a.muli(3f)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaNPanic2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaNPanic2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.INF_PANIC
			Dim a As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, Single.PositiveInfinity})
			a.muli(3f)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNaNPanic3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNaNPanic3(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ANY_PANIC
			Dim a As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, Single.NegativeInfinity})
			a.muli(3f)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScopePanic1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScopePanic1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
			Dim array As INDArray
			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS119")
				array = Nd4j.create(10)
				assertTrue(array.Attached)
			End Using
			array.add(1.0)
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScopePanic2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScopePanic2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC
			Dim array As INDArray
			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS120")
				array = Nd4j.create(10)
				assertTrue(array.Attached)
				assertEquals(1, workspace.GenerationId)
			End Using
			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS120")
				assertEquals(2, workspace.GenerationId)
				array.add(1.0)
				assertTrue(array.Attached)
			End Using
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScopePanic3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScopePanic3(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC


			Dim array As INDArray

			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS121")
				array = Nd4j.create(10)
				assertTrue(array.Attached)

				assertEquals(1, workspace.GenerationId)


				Using workspaceInner As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS122")
					array.add(1.0)
				End Using
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScopePanicPerf(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScopePanicPerf(ByVal backend As Nd4jBackend)
			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS121")
				Dim x As INDArray = Nd4j.create(1000, 1000).assign(1.0)
				Dim y As INDArray = Nd4j.create(1000, 1000).assign(1.0)

				Dim iterations As Integer = 100

				For e As Integer = 0 To iterations - 1
					x.addi(y)
				Next e

				Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.SCOPE_PANIC

				Dim nanosC As val = System.nanoTime()
				For e As Integer = 0 To iterations - 1
					x.addi(y)
				Next e
				Dim nanosD As val = System.nanoTime()

				Dim avgB As val = (nanosD - nanosC) / iterations


				Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.DISABLED

				Dim nanosA As val = System.nanoTime()
				For e As Integer = 0 To iterations - 1
					x.addi(y)
				Next e
				Dim nanosB As val = System.nanoTime()

				Dim avgA As val = (nanosB - nanosA) / iterations


	'            log.info("A: {}; B: {}", avgA, avgB);
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExtendedStatistics(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExtendedStatistics(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().nativeStatistics(True).build()

			Dim array As INDArray = Nd4j.ones(10)
			Dim stats As val = OpProfiler.Instance.getStatistics()

			assertEquals(10, stats.getCountPositive())
			assertEquals(0, stats.getCountNegative())
			assertEquals(0, stats.getCountZero())
			assertEquals(0, stats.getCountInf())
			assertEquals(0, stats.getCountNaN())
			assertEquals(1.0f, stats.getMeanValue(), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNanPanic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNanPanic(ByVal backend As Nd4jBackend)
			Try
				Dim op As DynamicCustomOp = DynamicCustomOp.builder("add").addInputs(Nd4j.valueArrayOf(10, Double.NaN).castTo(DataType.DOUBLE), Nd4j.scalar(0.0)).addOutputs(Nd4j.create(DataType.DOUBLE, 10)).build()

				Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(True).build()
				Try
					Nd4j.exec(op) 'Should trigger NaN panic
					fail()
				Catch e As Exception
					'throw new RuntimeException(e);
					log.info("Message: {}", e.Message)
					assertTrue(e.Message.contains("NaN"),e.Message)
				End Try

				Dim [in] As INDArray = op.getInputArgument(0)

				Try
					Transforms.sigmoid([in])
					fail()
				Catch e As Exception
					assertTrue(e.Message.contains("NaN"))
				End Try
			Finally
				Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(False).build()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInfPanic()
		Public Overridable Sub testInfPanic()
			Try
				Dim op As DynamicCustomOp = DynamicCustomOp.builder("add").addInputs(Nd4j.valueArrayOf(10, Double.PositiveInfinity).castTo(DataType.DOUBLE), Nd4j.scalar(0.0)).addOutputs(Nd4j.create(DataType.DOUBLE, 10)).build()

				Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForINF(True).build()
				Try
					Nd4j.exec(op) 'Should trigger NaN panic
					fail()
				Catch e As Exception
					log.error("",e)
					assertTrue(e.Message.contains("Inf"),e.Message)
				End Try

				Dim [in] As INDArray = op.getInputArgument(0)

				Try
					Transforms.max([in], 1.0, False)
					fail()
				Catch e As Exception
					assertTrue(e.Message.contains("Inf"))
				End Try
			Finally
				Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForINF(False).build()
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpProfilerOpContextLegacy()
		Public Overridable Sub testOpProfilerOpContextLegacy()

			For Each nan As Boolean In New Boolean(){True, False}

				Dim [in] As INDArray = Nd4j.valueArrayOf(10,If(nan, -1, 0)).castTo(DataType.FLOAT)

				Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(nan).checkForINF(Not nan).build()

				Dim oc As OpContext = Nd4j.Executioner.buildContext()
				oc.setInputArray(0, [in])
				oc.setOutputArray(0, [in].ulike())
				Try
					Nd4j.exec(New Log(), oc)
					Console.WriteLine(oc.getOutputArray(0))
					fail("Expected op profiler exception")
				Catch t As Exception
					'OK
					assertTrue(t.getMessage().contains(If(nan, "NaN", "Inf")),t.getMessage())
				End Try
			Next nan
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpProfilerOpContextCustomOp()
		Public Overridable Sub testOpProfilerOpContextCustomOp()

			For Each nan As Boolean In New Boolean(){True, False}

				Dim [in] As INDArray = Nd4j.create(DataType.DOUBLE, 10).assign(If(nan, Double.NaN, Double.PositiveInfinity))
				Dim in2 As INDArray = [in].dup()


				Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(nan).checkForINF(Not nan).build()

				Dim oc As OpContext = Nd4j.Executioner.buildContext()
				oc.IArguments = 0
				oc.setInputArray(0, [in])
				oc.setInputArray(1, in2)
				oc.setOutputArray(0, Nd4j.create(DataType.DOUBLE, 20))
				Try
					Nd4j.exec(New Concat(), oc)
					Console.WriteLine(oc.getOutputArray(0))
					fail("Expected op profiler exception")
				Catch t As Exception
					'OK
					assertTrue(t.getMessage().contains(If(nan, "NaN", "Inf")),t.getMessage())
				End Try
			Next nan
		End Sub
	End Class

End Namespace