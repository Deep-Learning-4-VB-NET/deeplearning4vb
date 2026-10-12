Imports Slf4j = lombok.extern.slf4j.Slf4j
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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig
Imports StackAggregator = org.nd4j.linalg.profiler.data.StackAggregator
Imports StackDescriptor = org.nd4j.linalg.profiler.data.primitives.StackDescriptor
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
'ORIGINAL LINE: @Slf4j @NativeTag public class StackAggregatorTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class StackAggregatorTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().stackTrace(True).build()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL
			OpProfiler.Instance.reset()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.DISABLED
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicBranching1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicBranching1(ByVal backend As Nd4jBackend)
			Dim aggregator As New StackAggregator()

			aggregator.incrementCount()

			aggregator.incrementCount()

			assertEquals(2, aggregator.TotalEventsNumber)
			assertEquals(2, aggregator.UniqueBranchesNumber)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicBranching2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicBranching2(ByVal backend As Nd4jBackend)
			Dim aggregator As New StackAggregator()

			For i As Integer = 0 To 9
				aggregator.incrementCount()
			Next i

			assertEquals(10, aggregator.TotalEventsNumber)

			' simnce method is called in loop, there should be only 1 unique code branch
			assertEquals(1, aggregator.UniqueBranchesNumber)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTrailingFrames1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTrailingFrames1(ByVal backend As Nd4jBackend)
			Dim aggregator As New StackAggregator()
			aggregator.incrementCount()


			Dim descriptor As StackDescriptor = aggregator.LastDescriptor

			log.info("Trace: {}", descriptor.ToString())

			' we just want to make sure that OpProfiler methods are NOT included in trace
			assertTrue(descriptor.getStackTrace()(descriptor.size() - 1).getClassName().contains("StackAggregatorTests"))
		End Sub

	'    @ParameterizedTest
	'    @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs")
	'    public void testTrailingFrames2(Nd4jBackend backend) {
	'        INDArray x = Nd4j.create(new int[] {10, 10}, 'f');
	'        INDArray y = Nd4j.create(new int[] {10, 10}, 'c');
	'
	'        x.assign(y);
	'
	'
	'        x.assign(y);
	'
	'        Nd4j.getExecutioner().commit();
	'
	'        StackAggregator aggregator = OpProfiler.getInstance().getMixedOrderAggregator();
	'
	'        StackDescriptor descriptor = aggregator.getLastDescriptor();
	'
	'        log.info("Trace: {}", descriptor.toString());
	'
	'        assertEquals(2, aggregator.getTotalEventsNumber());
	'        assertEquals(2, aggregator.getUniqueBranchesNumber());
	'
	'        aggregator.renderTree();
	'    }

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testScalarAggregator(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarAggregator(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(10)

			x.putScalar(0, 1.0)

			Dim x_0 As Double = x.getDouble(0)

			assertEquals(1.0, x_0, 1e-5)

			Dim aggregator As StackAggregator = OpProfiler.Instance.ScalarAggregator

			assertEquals(2, aggregator.TotalEventsNumber)
			assertEquals(2, aggregator.UniqueBranchesNumber)

			aggregator.renderTree(False)
		End Sub
	End Class

End Namespace