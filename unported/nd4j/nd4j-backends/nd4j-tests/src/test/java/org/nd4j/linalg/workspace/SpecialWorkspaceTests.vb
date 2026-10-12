Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports LocationPolicy = org.nd4j.linalg.api.memory.enums.LocationPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.workspace


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag public class SpecialWorkspaceTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SpecialWorkspaceTests
		Inherits BaseNd4jTestWithBackends

		Private initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void shutUp()
		Public Overridable Sub shutUp()
			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) public void testVariableTimeSeries1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableTimeSeries1(ByVal backend As Nd4jBackend)
			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(3.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).build()

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "WS1")
				Nd4j.create(DataType.DOUBLE,500)
				Nd4j.create(DataType.DOUBLE,500)
			End Using

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1"), Nd4jWorkspace)

			assertEquals(0, workspace.StepNumber)

			Dim requiredMemory As Long = 1000 * DataType.DOUBLE.width()
			Dim shiftedSize As Long = (CLng(Math.Truncate(requiredMemory * 1.3))) + (8 - ((CLng(Math.Truncate(requiredMemory * 1.3))) Mod 8))
			assertEquals(requiredMemory, workspace.SpilledSize)
			assertEquals(shiftedSize, workspace.InitialBlockSize)
			assertEquals(workspace.InitialBlockSize * 4, workspace.CurrentSize)

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS1")
				Nd4j.create(DataType.DOUBLE,2000)
			End Using

			assertEquals(0, workspace.StepNumber)

			assertEquals(1000 * DataType.DOUBLE.width(), workspace.SpilledSize)
			assertEquals(2000 * DataType.DOUBLE.width(), workspace.PinnedSize)

			assertEquals(0, workspace.DeviceOffset)

			' FIXME: fix this!
			'assertEquals(0, workspace.getHostOffset());

			assertEquals(0, workspace.ThisCycleAllocations)
			log.info("------------------")

			assertEquals(1, workspace.NumberOfPinnedAllocations)

			For e As Integer = 0 To 3
				For i As Integer = 0 To 3
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "WS1")
						Nd4j.create(DataType.DOUBLE,500)
						Nd4j.create(DataType.DOUBLE,500)
					End Using

					assertEquals((i + 1) * workspace.InitialBlockSize, workspace.DeviceOffset,"Failed on iteration " & i)
				Next i

				If e >= 2 Then
					assertEquals(0, workspace.NumberOfPinnedAllocations,"Failed on iteration " & e)
				Else
					assertEquals(1, workspace.NumberOfPinnedAllocations,"Failed on iteration " & e)
				End If
			Next e

			assertEquals(0, workspace.SpilledSize)
			assertEquals(0, workspace.PinnedSize)
			assertEquals(0, workspace.NumberOfPinnedAllocations)
			assertEquals(0, workspace.NumberOfExternalAllocations)

			log.info("Workspace state after first block: ---------------------------------------------------------")
			Nd4j.WorkspaceManager.printAllocationStatisticsForCurrentThread()

			log.info("--------------------------------------------------------------------------------------------")

			' we just do huge loop now, with pinned stuff in it
			For i As Integer = 0 To 99
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "WS1")
					Nd4j.create(DataType.DOUBLE,500)
					Nd4j.create(DataType.DOUBLE,500)
					Nd4j.create(DataType.DOUBLE,500)

					assertEquals(1500 * Nd4j.sizeOfDataType(), workspace.ThisCycleAllocations)
				End Using
			Next i

			assertEquals(0, workspace.SpilledSize)
			assertNotEquals(0, workspace.PinnedSize)
			assertNotEquals(0, workspace.NumberOfPinnedAllocations)
			assertEquals(0, workspace.NumberOfExternalAllocations)

			' and we do another clean loo, without pinned stuff in it, to ensure all pinned allocates are gone
			For i As Integer = 0 To 99
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "WS1")
					Nd4j.create(DataType.DOUBLE,500)
					Nd4j.create(DataType.DOUBLE,500)
				End Using
			Next i

			assertEquals(0, workspace.SpilledSize)
			assertEquals(0, workspace.PinnedSize)
			assertEquals(0, workspace.NumberOfPinnedAllocations)
			assertEquals(0, workspace.NumberOfExternalAllocations)

			log.info("Workspace state after second block: ---------------------------------------------------------")
			Nd4j.WorkspaceManager.printAllocationStatisticsForCurrentThread()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableTimeSeries2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableTimeSeries2(ByVal backend As Nd4jBackend)
			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(3.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).build()

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, "WS1"), Nd4jWorkspace)
	'        workspace.enableDebug(true);

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "WS1")
				Nd4j.create(DataType.DOUBLE,500)
				Nd4j.create(DataType.DOUBLE,500)
			End Using

			assertEquals(0, workspace.StepNumber)
			Dim requiredMemory As Long = 1000 * DataType.DOUBLE.width()
			Dim shiftedSize As Long = (CLng(Math.Truncate(requiredMemory * 1.3))) + (8 - ((CLng(Math.Truncate(requiredMemory * 1.3))) Mod 8))
			assertEquals(requiredMemory, workspace.SpilledSize)
			assertEquals(shiftedSize, workspace.InitialBlockSize)
			assertEquals(workspace.InitialBlockSize * 4, workspace.CurrentSize)

			For i As Integer = 0 To 99
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "WS1")
					Nd4j.create(DataType.DOUBLE,500)
					Nd4j.create(DataType.DOUBLE,500)
					Nd4j.create(DataType.DOUBLE,500)
				End Using
			Next i


			assertEquals(workspace.InitialBlockSize * 4, workspace.CurrentSize)

			assertEquals(0, workspace.NumberOfPinnedAllocations)
			assertEquals(0, workspace.NumberOfExternalAllocations)

			assertEquals(0, workspace.SpilledSize)
			assertEquals(0, workspace.PinnedSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testViewDetach_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testViewDetach_1(ByVal backend As Nd4jBackend)
			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10000000).overallocationLimit(3.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.BLOCK_LEFT).build()

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, "WS109"), Nd4jWorkspace)

			Dim row As INDArray = Nd4j.linspace(1, 10, 10).castTo(DataType.DOUBLE)
			Dim exp As INDArray = Nd4j.create(DataType.DOUBLE,10).assign(2.0)
			Dim result As INDArray = Nothing
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "WS109")
				Dim matrix As INDArray = Nd4j.create(DataType.DOUBLE,10, 10)
				Dim e As Integer = 0
				Do While e < matrix.rows()
					matrix.getRow(e).assign(row)
					e += 1
				Loop


				Dim column As INDArray = matrix.getColumn(1)
				assertTrue(column.View)
				assertTrue(column.Attached)
				result = column.detach()
			End Using

			assertFalse(result.View)
			assertFalse(result.Attached)
			assertEquals(exp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAlignment_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAlignment_1(ByVal backend As Nd4jBackend)
			Dim initialConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).build()
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getAndActivateWorkspace(initialConfig, "WS132143452343")

			For j As Integer = 0 To 99

				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()

					For x As Integer = 0 To 9
						'System.out.println("Start iteration (" + j + "," + x + ")");
						Dim arr As INDArray = Nd4j.linspace(1,10,10, DataType.DOUBLE).reshape(ChrW(1), 10)
						Dim sum As INDArray = arr.sum(True, 1)
						Nd4j.create(DataType.BOOL, x+1) 'NOTE: no crash if set to FLOAT/HALF, No crash if removed entirely; same crash for BOOL/UBYTE
						'System.out.println("End iteration (" + j + "," + x + ")");
					Next x
				End Using
			Next j
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoOpExecution_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoOpExecution_1(ByVal backend As Nd4jBackend)
			Dim configuration As val = WorkspaceConfiguration.builder().initialSize(10000000).overallocationLimit(3.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.BLOCK_LEFT).build()

			Dim iterations As Integer = 10000

			Dim array0 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array1 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array2 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array3 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array4 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array5 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array6 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array7 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array8 As val = Nd4j.create(New Long(){ 100, 100})
			Dim array9 As val = Nd4j.create(New Long(){ 100, 100})

			Dim timeStart As val = System.nanoTime()
			For e As Integer = 0 To iterations - 1

				Dim op As val = DynamicCustomOp.builder("noop").addInputs(array0, array1, array2, array3, array4, array5, array6, array7, array8, array9).addOutputs(array0, array1, array2, array3, array4, array5, array6, array7, array8, array9).addIntegerArguments(5, 10).addFloatingPointArguments(3.0, 10.0).addBooleanArguments(True, False).callInplace(True).build()

				Nd4j.Executioner.exec(op)
			Next e
			Dim timeEnd As val = System.nanoTime()
			log.info("{} ns", ((timeEnd - timeStart) / CDbl(iterations)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspaceOrder_1()
		Public Overridable Sub testWorkspaceOrder_1()
			Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1_000_000).overallocationLimit(0.05).policyLearning(LearningPolicy.NONE).build()

			Dim exp As val = Arrays.asList("outer", Nothing, "outer", "inner", "outer", Nothing)
			Dim res As val = New List(Of String)()

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, "outer")
				Using ws2 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, "inner")
					Using ws3 As org.nd4j.linalg.api.memory.MemoryWorkspace = ws.notifyScopeBorrowed()
						Console.WriteLine("X: " & Nd4j.MemoryManager.CurrentWorkspace) 'outer
						res.add(If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nothing, Nd4j.MemoryManager.CurrentWorkspace.Id))
						Using ws4 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
							Console.WriteLine("A: " & Nd4j.MemoryManager.CurrentWorkspace) 'None (null)
							res.add(If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nothing, Nd4j.MemoryManager.CurrentWorkspace.Id))
						End Using
						Console.WriteLine("B: " & Nd4j.MemoryManager.CurrentWorkspace) 'outer
						res.add(If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nothing, Nd4j.MemoryManager.CurrentWorkspace.Id))
					End Using
					Console.WriteLine("C: " & Nd4j.MemoryManager.CurrentWorkspace) 'inner
					res.add(If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nothing, Nd4j.MemoryManager.CurrentWorkspace.Id))
				End Using
				Console.WriteLine("D: " & Nd4j.MemoryManager.CurrentWorkspace) 'outer
				res.add(If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nothing, Nd4j.MemoryManager.CurrentWorkspace.Id))
			End Using
			Console.WriteLine("E: " & Nd4j.MemoryManager.CurrentWorkspace) 'None (null)
			res.add(If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nothing, Nd4j.MemoryManager.CurrentWorkspace.Id))

			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmapedWorkspaceLimits_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMmapedWorkspaceLimits_1()
			If Not Nd4j.Environment.CPU Then
				Return
			End If

			Dim tmpFile As val = Files.createTempFile("some", "file")
			Dim mmap As val = WorkspaceConfiguration.builder().initialSize(200 * 1024L * 1024L).tempFilePath(tmpFile.toAbsolutePath().ToString()).policyLocation(LocationPolicy.MMAP).policyLearning(LearningPolicy.NONE).build()

			Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(mmap, "M2")
				Dim twoHundredMbsOfFloats As Integer = 52_428_800 ' 200mbs % 4
				Dim addMoreFloats As val = True
				If addMoreFloats Then
					twoHundredMbsOfFloats += 1_000
				End If

				Dim x As val = Nd4j.rand(DataType.FLOAT, twoHundredMbsOfFloats)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmapedWorkspace_Path_Limits_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMmapedWorkspace_Path_Limits_1()
			If Not Nd4j.Environment.CPU Then
				Return
			End If

			' getting very long file name
			Dim builder As val = New StringBuilder("long_file_name_")
			For e As Integer = 0 To 99
				builder.append("9")
			Next e


			Dim tmpFile As val = Files.createTempFile("some", builder.ToString())
			Dim mmap As val = WorkspaceConfiguration.builder().initialSize(200 * 1024L * 1024L).tempFilePath(tmpFile.toAbsolutePath().ToString()).policyLocation(LocationPolicy.MMAP).policyLearning(LearningPolicy.NONE).build()

			Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(mmap, "M2")
				Dim x As val = Nd4j.rand(DataType.FLOAT, 1024)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeleteMappedFile_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeleteMappedFile_1()
			If Not Nd4j.Environment.CPU Then
				Return
			End If

			Dim tmpFile As val = Files.createTempFile("some", "file")
			Dim mmap As val = WorkspaceConfiguration.builder().initialSize(200 * 1024L * 1024L).tempFilePath(tmpFile.toAbsolutePath().ToString()).policyLocation(LocationPolicy.MMAP).policyLearning(LearningPolicy.NONE).build()

			Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(mmap, "M2")
				Dim x As val = Nd4j.rand(DataType.FLOAT, 1024)
			End Using

			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()

			Files.delete(tmpFile)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeleteMappedFile_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeleteMappedFile_2()
			assertThrows(GetType(System.ArgumentException),Sub()
			If Not Nd4j.Environment.CPU Then
				Throw New System.ArgumentException("Don't try to run on CUDA")
			End If
			Dim tmpFile As val = Files.createTempFile("some", "file")
			Dim mmap As val = WorkspaceConfiguration.builder().initialSize(200 * 1024L * 1024L).tempFilePath(tmpFile.toAbsolutePath().ToString()).policyLocation(LocationPolicy.MMAP).build()
			Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(mmap, "M2")
				Dim x As val = Nd4j.rand(DataType.FLOAT, 1024)
			End Using
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Files.delete(tmpFile)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMigrateToWorkspace()
		Public Overridable Sub testMigrateToWorkspace()
			Dim src As val = Nd4j.createFromArray(1L,2L)
			Dim wsConf As val = (New WorkspaceConfiguration()).builder().build()
			Nd4j.WorkspaceManager.createNewWorkspace(wsConf,"testWS")
			Dim ws As val = Nd4j.WorkspaceManager.getAndActivateWorkspace("testWS")

			Dim migrated As val = src.migrate()
			assertEquals(src.dataType(), migrated.dataType())
			assertEquals(1L, migrated.getLong(0))

			ws.close()
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace