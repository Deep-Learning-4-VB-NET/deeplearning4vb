Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports org.junit.jupiter.api
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.workspace


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag public class EndlessWorkspaceTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EndlessWorkspaceTests
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void startUp()
		Public Overridable Sub startUp()
			Nd4j.MemoryManager.togglePeriodicGc(False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void shutUp()
		Public Overridable Sub shutUp()
			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Nd4j.DataType = Me.initialType
			Nd4j.MemoryManager.togglePeriodicGc(True)
		End Sub

		''' <summary>
		''' This test checks for allocations within single workspace, without any spills
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessTest1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub endlessTest1(ByVal backend As Nd4jBackend)

			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(100 * 1024L * 1024L).build()

			Nd4j.MemoryManager.togglePeriodicGc(False)

			Dim counter As New AtomicLong(0)
			Do
				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.AndActivateWorkspace
					Dim time1 As Long = System.nanoTime()
					Dim array As INDArray = Nd4j.create(1024 * 1024)
					Dim time2 As Long = System.nanoTime()
					array.addi(1.0f)
					assertEquals(1.0f, array.meanNumber().floatValue(), 0.1f)

					If counter.incrementAndGet() Mod 1000 = 0 Then
						log.info("{} iterations passed... Allocation time: {} ns", counter.get(), time2 - time1)
					End If
				End Using
			Loop
		End Sub

		''' <summary>
		''' This test checks for allocation from workspace AND spills </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessTest2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub endlessTest2(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).build()

			Nd4j.MemoryManager.togglePeriodicGc(False)

			Dim counter As New AtomicLong(0)
			Do
				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.AndActivateWorkspace
					Dim time1 As Long = System.nanoTime()
					Dim array As INDArray = Nd4j.create(2 * 1024 * 1024)
					Dim time2 As Long = System.nanoTime()
					array.addi(1.0f)
					assertEquals(1.0f, array.meanNumber().floatValue(), 0.1f)

					Dim time3 As Long = System.nanoTime()
					Dim array2 As INDArray = Nd4j.create(3 * 1024 * 1024)
					Dim time4 As Long = System.nanoTime()

					If counter.incrementAndGet() Mod 1000 = 0 Then
						log.info("{} iterations passed... Allocation time: {} vs {} (ns)", counter.get(), time2 - time1, time4 - time3)
						System.GC.Collect()
					End If
				End Using
			Loop
		End Sub

		''' <summary>
		''' This endless test checks for nested workspaces and cross-workspace memory use
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessTest3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub endlessTest3(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).build()

			Nd4j.MemoryManager.togglePeriodicGc(False)
			Dim counter As New AtomicLong(0)
			Do
				Using workspace1 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS_1")
					Dim array1 As INDArray = Nd4j.create(2 * 1024 * 1024)
					array1.assign(1.0)

					Using workspace2 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS_2")
						Dim array2 As INDArray = Nd4j.create(2 * 1024 * 1024)
						array2.assign(1.0)

						array1.addi(array2)

						assertEquals(2.0f, array1.meanNumber().floatValue(), 0.01)

						If counter.incrementAndGet() Mod 1000 = 0 Then
							log.info("{} iterations passed...", counter.get())
							System.GC.Collect()
						End If
					End Using
				End Using
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessTest4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub endlessTest4(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(100 * 1024L * 1024L).build()
			Do
				Using workspace1 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS_1")
					For i As Integer = 0 To 999
						Dim array As INDArray = Nd4j.createUninitialized(RandomUtils.nextInt(1, 50), RandomUtils.nextInt(1, 50))

						Dim mean As INDArray = array.max(1)
					Next i

					For i As Integer = 0 To 999
						Dim array As INDArray = Nd4j.createUninitialized(RandomUtils.nextInt(1, 100))

						array.maxNumber().doubleValue()
					Next i
				End Using
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessTest5(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub endlessTest5(ByVal backend As Nd4jBackend)
			Do
				Dim thread As New Thread(Sub()
				Dim wsConf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyLearning(LearningPolicy.NONE).build()

				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsConf, "PEW-PEW")
					Dim array As INDArray = Nd4j.create(10)
				End Using
				End Sub)

				thread.Start()
				thread.Join()

				System.GC.Collect()
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessTest6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub endlessTest6(ByVal backend As Nd4jBackend)
			Nd4j.MemoryManager.togglePeriodicGc(False)
			Dim wsConf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyLearning(LearningPolicy.NONE).build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong cnt = new java.util.concurrent.atomic.AtomicLong(0);
			Dim cnt As New AtomicLong(0)
			Do

				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsConf, "PEW-PEW")
					Dim array As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, 4f, 5f})
				End Using

				If cnt.incrementAndGet() Mod 1000000 = 0 Then
					log.info("TotalBytes: {}", Pointer.totalBytes())
				End If
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessValidation1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub endlessValidation1(ByVal backend As Nd4jBackend)
			Nd4j.MemoryManager.togglePeriodicGc(True)

			Dim counter As New AtomicLong(0)
			Do
				Dim array1 As INDArray = Nd4j.create(2 * 1024 * 1024)
				array1.assign(1.0)

				assertEquals(1.0f, array1.meanNumber().floatValue(), 0.01)

				If counter.incrementAndGet() Mod 1000 = 0 Then
					log.info("{} iterations passed...", counter.get())
					System.GC.Collect()
				End If
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPerf1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPerf1(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(50000L).build()

			Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS_1")

			Dim tmp As INDArray = Nd4j.create(64 * 64 + 1)

			'Nd4j.getMemoryManager().togglePeriodicGc(true);

			Dim results As IList(Of Long) = New List(Of Long)()
			Dim resultsOp As IList(Of Long) = New List(Of Long)()
			For i As Integer = 0 To 999999
				Dim time1 As Long = System.nanoTime()
				Dim time3 As Long = 0
				Dim time4 As Long = 0
				'MemoryWorkspace workspace = Nd4j.getWorkspaceManager().getAndActivateWorkspace("WS_1");
				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS_1")
					Dim array As INDArray = Nd4j.createUninitialized(64 * 64 + 1)
					Dim arrayx As INDArray = Nd4j.createUninitialized(64 * 64 + 1)

					time3 = System.nanoTime()
					arrayx.addi(1.01)
					time4 = System.nanoTime()

				End Using
				'workspace.notifyScopeLeft();
				Dim time2 As Long = System.nanoTime()

				results.Add(time2 - time1)
				resultsOp.Add(time4 - time3)
			Next i
			results.Sort()
			resultsOp.Sort()

			Dim pos As Integer = CInt(Math.Truncate(results.Count * 0.9))

			log.info("Block: {} ns; Op: {} ns;", results(pos), resultsOp(pos))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void endlessTestSerDe1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub endlessTestSerDe1(ByVal backend As Nd4jBackend)
			Dim features As INDArray = Nd4j.create(32, 3, 224, 224)
			Dim labels As INDArray = Nd4j.create(32, 200)
			Dim tmp As File = File.createTempFile("12dadsad", "dsdasds")
			Dim array((33 * 3 * 224 * 224) - 1) As Single
			Dim ds As New DataSet(features, labels)
			ds.save(tmp)

			Dim wsConf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).policyLearning(LearningPolicy.FIRST_LOOP).build()

			Do

				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsConf, "serde")
	'                
	'                            try (FileOutputStream fos = new FileOutputStream(tmp); BufferedOutputStream bos = new BufferedOutputStream(fos)) {
	'                SerializationUtils.serialize(array, fos);
	'                            }
	'                
	'                            try (FileInputStream fis = new FileInputStream(tmp); BufferedInputStream bis = new BufferedInputStream(fis)) {
	'                long time1 = System.currentTimeMillis();
	'                float[] arrayR = (float[]) SerializationUtils.deserialize(bis);
	'                long time2 = System.currentTimeMillis();
	'                
	'                log.info("Load time: {}", time2 - time1);
	'                            }
	'                



					Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
					ds.load(tmp)
					Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()

					log.info("Load time: {}", time2 - time1)
				End Using
			Loop
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace