Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
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
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports MirroringPolicy = org.nd4j.linalg.api.memory.enums.MirroringPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag @Execution(ExecutionMode.SAME_THREAD) public class WorkspaceProviderTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class WorkspaceProviderTests
		Inherits BaseNd4jTestWithBackends

		Private Shared ReadOnly basicConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(81920).overallocationLimit(0.1).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.NONE).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.OVERALLOCATE).build()

		Private Shared ReadOnly bigConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(20 * 1024 * 1024L).overallocationLimit(0.1).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.NONE).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.OVERALLOCATE).build()

		Private Shared ReadOnly loopConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).build()


		Private Shared ReadOnly delayedConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).cyclesBeforeInitialization(3).policyAllocation(AllocationPolicy.STRICT).build()

		Private Shared ReadOnly reallocateConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).build()

		Private Shared ReadOnly reallocateDelayedConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.REALLOCATE).cyclesBeforeInitialization(3).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).build()


		Private Shared ReadOnly reallocateUnspecifiedConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.0).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.OVERALLOCATE).policyReset(ResetPolicy.BLOCK_LEFT).build()



		Private Shared ReadOnly firstConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).build()


		Private Shared ReadOnly circularConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().minSize(10 * 1024L * 1024L).overallocationLimit(1.0).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).build()


		Private Shared ReadOnly adsiConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().overallocationLimit(3.0).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.OVERALLOCATE).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).build()

		Friend initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void shutUp()
		Public Overridable Sub shutUp()
			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
		End Sub

		''' <summary>
		''' This simple test checks for over-time learning with coefficient applied
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnboundedLoop2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnboundedLoop2(ByVal backend As Nd4jBackend)
			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.OVERALLOCATE).overallocationLimit(4.0).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(5).build()

			Dim ws1 As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, "ITER"), Nd4jWorkspace)

			Dim requiredMemory As Long = 100 * Nd4j.sizeOfDataType()
			Dim shiftedSize As Long = (CLng(Math.Truncate(requiredMemory * 1.3))) + (8 - ((CLng(Math.Truncate(requiredMemory * 1.3))) Mod 8))

			For x As Integer = 0 To 99
				Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, "ITER").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Dim array As INDArray = Nd4j.create(DataType.DOUBLE,100)
				End Using

				' only checking after workspace is initialized
				If x > 4 Then
					assertEquals(shiftedSize, ws1.InitialBlockSize)
					assertEquals(5 * shiftedSize, ws1.CurrentSize)
				ElseIf x < 4 Then
					' we're making sure we're not initialize early
					assertEquals(0, ws1.CurrentSize,"Failed on iteration " & x)
				End If
			Next x

			' maximum allocation amount is 100 elements during learning, and additional coefficient is 4.0. result is workspace of 500 elements
			assertEquals(5 * shiftedSize, ws1.CurrentSize)

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnboundedLoop1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnboundedLoop1(ByVal backend As Nd4jBackend)
			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(100 * 100 * Nd4j.sizeOfDataType()).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.STRICT).build()

			For x As Integer = 0 To 99
				Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, "ITER").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

					Dim array As INDArray = Nd4j.create(DataType.DOUBLE,100)
				End Using

				Dim ws1 As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, "ITER"), Nd4jWorkspace)

				assertEquals((x + 1) * 100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
			Next x

			Dim ws1 As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, "ITER"), Nd4jWorkspace)
			assertEquals(100 * 100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)

			' just to trigger reset
			ws1.notifyScopeEntered()

			' confirming reset
			'        assertEquals(0, ws1.getPrimaryOffset());

			ws1.notifyScopeLeft()

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultithreading1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultithreading1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.linalg.api.memory.MemoryWorkspace> workspaces = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim workspaces As IList(Of MemoryWorkspace) = New CopyOnWriteArrayList(Of MemoryWorkspace)()
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration

			Dim threads(19) As Thread
			For x As Integer = 0 To threads.Length - 1
				threads(x) = New Thread(Sub()
				Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.WorkspaceForCurrentThread
				workspaces.Add(workspace)
				End Sub)

				threads(x).Start()
			Next x

			For x As Integer = 0 To threads.Length - 1
				threads(x).Join()
			Next x

			For x As Integer = 0 To threads.Length - 1
				For y As Integer = 0 To threads.Length - 1
					If x = y Then
						Continue For
					End If

					assertFalse(workspaces(x) Is workspaces(y))
				Next y
			Next x

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspacesOverlap2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspacesOverlap2(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration

			assertFalse(Nd4j.WorkspaceManager.checkIfWorkspaceExists("WS1"))
			assertFalse(Nd4j.WorkspaceManager.checkIfWorkspaceExists("WS2"))

			Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array As INDArray = Nd4j.create(New Double() {6f, 3f, 1f, 9f, 21f})
				Dim array3 As INDArray = Nothing

				Dim reqMem As Long = 5 * Nd4j.sizeOfDataType(DataType.DOUBLE)
				assertEquals(reqMem + reqMem Mod 16, ws1.PrimaryOffset)
				Using ws2 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

					Dim array2 As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

					reqMem = 5 * Nd4j.sizeOfDataType(DataType.DOUBLE)
					assertEquals(reqMem + reqMem Mod 16, ws1.PrimaryOffset)
					assertEquals(reqMem + reqMem Mod 16, ws2.PrimaryOffset)

					Using ws3 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeBorrowed(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
						assertTrue(ws1 Is ws3)
						assertTrue(ws1 Is Nd4j.MemoryManager.CurrentWorkspace)

						array3 = array2.unsafeDuplication()
						assertTrue(ws1 Is array3.data().ParentWorkspace)
						assertEquals(reqMem + reqMem Mod 16, ws2.PrimaryOffset)
						assertEquals((reqMem + reqMem Mod 16) * 2, ws1.PrimaryOffset)
					End Using

					log.info("Current workspace: {}", Nd4j.MemoryManager.CurrentWorkspace)
					assertTrue(ws2 Is Nd4j.MemoryManager.CurrentWorkspace)

					assertEquals(reqMem + reqMem Mod 16, ws2.PrimaryOffset)
					assertEquals((reqMem + reqMem Mod 16) * 2, ws1.PrimaryOffset)

					assertEquals(15f, array3.sumNumber().floatValue(), 0.01f)
				End Using
			End Using

			log.info("------")

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testNestedWorkspacesOverlap1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspacesOverlap1(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration
			Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, 4f, 5f})

				Dim reqMem As Long = 5 * array.dataType().width()
				Dim add As Long = ((Nd4jWorkspace.alignmentBase \ 2) - reqMem Mod (Nd4jWorkspace.alignmentBase \ 2))
				assertEquals(reqMem + add, ws1.PrimaryOffset)
				Using ws2 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

					Dim array2 As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, 4f, 5f})

					reqMem = 5 * array2.dataType().width()
					assertEquals(reqMem + ((Nd4jWorkspace.alignmentBase \ 2) - reqMem Mod (Nd4jWorkspace.alignmentBase \ 2)), ws1.PrimaryOffset)
					assertEquals(reqMem + ((Nd4jWorkspace.alignmentBase \ 2) - reqMem Mod (Nd4jWorkspace.alignmentBase \ 2)), ws2.PrimaryOffset)

					Using ws3 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeBorrowed(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
						assertTrue(ws1 Is ws3)

						Dim array3 As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, 4f, 5f})

						assertEquals(reqMem + ((Nd4jWorkspace.alignmentBase \ 2) - reqMem Mod (Nd4jWorkspace.alignmentBase \ 2)), ws2.PrimaryOffset)
						assertEquals((reqMem + ((Nd4jWorkspace.alignmentBase \ 2) - reqMem Mod (Nd4jWorkspace.alignmentBase \ 2))) * 2, ws1.PrimaryOffset)
					End Using
				End Using
			End Using

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspacesSerde3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWorkspacesSerde3()
			Dim array As INDArray = Nd4j.create(DataType.DOUBLE,10).assign(1.0)
			Dim restored As INDArray = Nothing

			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			Nd4j.write(array, dos)

			Using workspace As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "WS_1"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					workspace.enableDebug(True)

					Dim bis As New MemoryStream(bos.toByteArray())
					Dim dis As New DataInputStream(bis)
					restored = Nd4j.read(dis)

					assertEquals(0, workspace.PrimaryOffset)

					assertEquals(array.length(), restored.length())
					assertEquals(1.0f, restored.meanNumber().floatValue(), 1.0f)

					' we want to ensure it's the same cached shapeInfo used here
					assertEquals(array.shapeInfoDataBuffer().addressPointer().address(), restored.shapeInfoDataBuffer().addressPointer().address())
				End Using
			End Using
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspacesSerde2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWorkspacesSerde2()
			Dim array As INDArray = Nd4j.create(10).assign(1.0)
			Dim restored As INDArray = Nothing

			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			Nd4j.write(array, dos)

			Using workspace As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "WS_1"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				workspace.enableDebug(True)

				Dim bis As New MemoryStream(bos.toByteArray())
				Dim dis As New DataInputStream(bis)
				restored = Nd4j.read(dis)

				Dim requiredMemory As Long = 10 * Nd4j.sizeOfDataType()
				assertEquals(requiredMemory + requiredMemory Mod 8, workspace.PrimaryOffset)

				assertEquals(array.length(), restored.length())
				assertEquals(1.0f, restored.meanNumber().floatValue(), 1.0f)

				' we want to ensure it's the same cached shapeInfo used here
				assertEquals(array.shapeInfoDataBuffer().addressPointer().address(), restored.shapeInfoDataBuffer().addressPointer().address())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspacesSerde1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWorkspacesSerde1()
			Dim shape() As Integer = {17, 57, 79}
			Dim array As INDArray = Nd4j.create(shape).assign(1.0)
			Dim restored As INDArray = Nothing

			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			Nd4j.write(array, dos)

			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(bigConfiguration, "WS_1")
				Dim bis As New MemoryStream(bos.toByteArray())
				Dim dis As New DataInputStream(bis)
				restored = Nd4j.read(dis)

				assertEquals(array.length(), restored.length())
				assertEquals(1.0f, restored.meanNumber().floatValue(), 1.0f)

				' we want to ensure it's the same cached shapeInfo used here
				assertEquals(array.shapeInfoDataBuffer().addressPointer().address(), restored.shapeInfoDataBuffer().addressPointer().address())
			End Using
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCircularBufferReset1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCircularBufferReset1(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(circularConfiguration, "WSR_1"), Nd4jWorkspace)

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WSR_1")
				Nd4j.create(10000)
				assertEquals(0, workspace.CurrentSize)
				assertEquals(1, workspace.NumberOfExternalAllocations)
			End Using

			assertEquals(10 * 1024L * 1024L, workspace.CurrentSize)
			assertEquals(0, workspace.PrimaryOffset)
			assertEquals(1, workspace.NumberOfExternalAllocations)

			For i As Integer = 0 To (11 * 1024 * 1024) - 1 Step 10000 * Nd4j.sizeOfDataType()
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WSR_1")
					Nd4j.create(10000)
				End Using

	'            
	'            if (i < 10480000)
	'                assertEquals("I: " + i,1, workspace.getNumberOfExternalAllocations());
	'            else
	'                assertEquals(0, workspace.getNumberOfExternalAllocations());
	'                
			Next i

			assertEquals(0, workspace.NumberOfExternalAllocations)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableInput1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableInput1(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(adsiConfiguration, "ADSI"), Nd4jWorkspace)

			Dim array1 As INDArray = Nothing
			Dim array2 As INDArray = Nothing

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(adsiConfiguration, "ADSI")
				' we allocate first element smaller then subsequent;
				array1 = Nd4j.create(DataType.DOUBLE, 8, 128, 100)
			End Using

			Dim requiredMemory As Long = 8 * 128 * 100 * Nd4j.sizeOfDataType(DataType.DOUBLE)
			Dim shiftedSize As Long = (CLng(Math.Truncate(requiredMemory * 1.3))) + (8 - ((CLng(Math.Truncate(requiredMemory * 1.3))) Mod 8))
			assertEquals(shiftedSize, workspace.InitialBlockSize)
			assertEquals(shiftedSize * 4, workspace.CurrentSize)
			assertEquals(0, workspace.PrimaryOffset)
			assertEquals(0, workspace.DeviceOffset)

			assertEquals(1, workspace.CyclesCount)
			assertEquals(0, workspace.StepNumber)


			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(adsiConfiguration, "ADSI")
				' allocating same shape
				array1 = Nd4j.create(8, 128, 100)
			End Using

			assertEquals(workspace.InitialBlockSize, workspace.PrimaryOffset)
			assertEquals(workspace.InitialBlockSize, workspace.DeviceOffset)

			assertEquals(2, workspace.CyclesCount)
			assertEquals(0, workspace.StepNumber)


			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(adsiConfiguration, "ADSI")
				' allocating bigger shape
				array1 = Nd4j.create(DataType.DOUBLE, 8, 128, 200)
			End Using

			' offsets should be intact, allocation happened as pinned
			assertEquals(workspace.InitialBlockSize, workspace.PrimaryOffset)
			assertEquals(workspace.InitialBlockSize, workspace.DeviceOffset)

			assertEquals(1, workspace.NumberOfPinnedAllocations)

			assertEquals(3, workspace.CyclesCount)
			assertEquals(0, workspace.StepNumber)


			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(adsiConfiguration, "ADSI")
				' allocating same shape
				array1 = Nd4j.create(DataType.DOUBLE, 8, 128, 100)
			End Using

			assertEquals(2, workspace.NumberOfPinnedAllocations)
			assertEquals(0, workspace.StepNumber)
			assertEquals(4, workspace.CyclesCount)

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(adsiConfiguration, "ADSI")
				' allocating same shape
				array1 = Nd4j.create(DataType.DOUBLE, 8, 128, 100)
			End Using

			assertEquals(3, workspace.NumberOfPinnedAllocations)
			assertEquals(1, workspace.StepNumber)
			assertEquals(5, workspace.CyclesCount)

			For i As Integer = 0 To 11
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(adsiConfiguration, "ADSI")
					' allocating same shape
					array1 = Nd4j.create(DataType.DOUBLE, 8, 128, 100)
				End Using
			Next i

			' Now we know that workspace was reallocated and offset was shifted to the end of workspace
			assertEquals(4, workspace.StepNumber)

			requiredMemory = 8 * 128 * 200 * Nd4j.sizeOfDataType(DataType.DOUBLE)
			shiftedSize = (CLng(Math.Truncate(requiredMemory * 1.3))) + (8 - ((CLng(Math.Truncate(requiredMemory * 1.3))) Mod 8))

			'assertEquals(shiftedSize * 4, workspace.getCurrentSize());
			assertEquals(workspace.CurrentSize, workspace.PrimaryOffset)
			assertEquals(workspace.CurrentSize, workspace.DeviceOffset)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocate3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocate3(ByVal backend As Nd4jBackend)
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(reallocateUnspecifiedConfiguration, "WS_1")

			For i As Integer = 1 To 10
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(reallocateUnspecifiedConfiguration, "WS_1")
					Dim array As INDArray = Nd4j.create(100 * i)
				End Using

				If i = 3 Then
					workspace.initializeWorkspace()
					assertEquals(100 * i * Nd4j.sizeOfDataType(), workspace.CurrentSize,"Failed on iteration " & i)
				End If
			Next i

			log.info("-----------------------------")

			For i As Integer = 10 To 1 Step -1
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(reallocateUnspecifiedConfiguration, "WS_1")
					Dim array As INDArray = Nd4j.create(100 * i)
				End Using
			Next i

			workspace.initializeWorkspace()
			assertEquals(100 * 10 * Nd4j.sizeOfDataType(), workspace.CurrentSize,"Failed on final")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocate2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocate2(ByVal backend As Nd4jBackend)
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(reallocateDelayedConfiguration, "WS_1")

			For i As Integer = 1 To 10
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(reallocateDelayedConfiguration, "WS_1")
					Dim array As INDArray = Nd4j.create(100 * i)
				End Using

				If i >= 3 Then
					assertEquals(100 * i * Nd4j.sizeOfDataType(), workspace.CurrentSize,"Failed on iteration " & i)
				Else
					assertEquals(0, workspace.CurrentSize)
				End If
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCircularLearning1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCircularLearning1(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray
			Dim array2 As INDArray
			For i As Integer = 0 To 1
				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(circularConfiguration, "WSX")
					array1 = Nd4j.create(10).assign(1)
				End Using

				Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(circularConfiguration, "WSX"), Nd4jWorkspace)
				assertEquals(10 * 1024 * 1024L, workspace.CurrentSize)
				log.info("Current step number: {}", workspace.getStepNumber())
				If i = 0 Then
					assertEquals(0, workspace.PrimaryOffset)
				ElseIf i = 1 Then
					assertEquals(workspace.getInitialBlockSize(), workspace.PrimaryOffset)
				End If
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocate1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocate1(ByVal backend As Nd4jBackend)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(reallocateConfiguration, "WS_1")
				Dim array As INDArray = Nd4j.create(DataType.DOUBLE,100)
			End Using



			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(reallocateConfiguration, "WS_1"), Nd4jWorkspace)
			workspace.initializeWorkspace()

			assertEquals(100 * Nd4j.sizeOfDataType(), workspace.CurrentSize)

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(reallocateConfiguration, "WS_1")
				Dim array As INDArray = Nd4j.create(DataType.DOUBLE,1000)
			End Using

			assertEquals(1000 * Nd4j.sizeOfDataType(), workspace.MaxCycleAllocations)

			workspace.initializeWorkspace()

			assertEquals(1000 * Nd4j.sizeOfDataType(), workspace.CurrentSize)

			' now we're working on reallocated array, that should be able to hold >100 elements
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(reallocateConfiguration, "WS_1")
				Dim array As INDArray = Nd4j.create(500).assign(1.0)

				assertEquals(1.0, array.meanNumber().doubleValue(), 0.01)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("raver119: This test doesn't make any sense to me these days. We're borrowing from the same workspace. Why?") public void testNestedWorkspaces11(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces11(ByVal backend As Nd4jBackend)
			For x As Integer = 1 To 9
				Using ws1 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "WS_1")
					Dim array1 As INDArray = Nd4j.create(DataType.DOUBLE,100 * x)

					For i As Integer = 1 To 9
						Using ws2 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "WS_1")
							Dim array2 As INDArray = Nd4j.create(DataType.DOUBLE,100 * x)
							For e As Integer = 1 To 9
								Using ws3 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(basicConfiguration, "WS_1").notifyScopeBorrowed()
									Dim array3 As INDArray = Nd4j.create(DataType.DOUBLE,100 * x)
								End Using
							Next e
						End Using
					Next i
				End Using
			Next x
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces10(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces10(ByVal backend As Nd4jBackend)
			For x As Integer = 1 To 9
				Using ws1 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "WS_1")
					Dim array1 As INDArray = Nd4j.create(100 * x)
					Using ws2 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "WS_1")
						Dim array2 As INDArray = Nd4j.create(100 * x)
						Using ws3 As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(basicConfiguration, "WS_1").notifyScopeBorrowed()
							Dim array3 As INDArray = Nd4j.create(100 * x)
						End Using

					End Using
				End Using
			Next x
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces9(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces9(ByVal backend As Nd4jBackend)
			For x As Integer = 1 To 9
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(delayedConfiguration, "WS_1")
					Dim array As INDArray = Nd4j.create(100 * x)
				End Using
			Next x

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(delayedConfiguration, "WS_1"), Nd4jWorkspace)
			workspace.initializeWorkspace()

			assertEquals(300 * Nd4j.sizeOfDataType(), workspace.CurrentSize)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces8(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces8(ByVal backend As Nd4jBackend)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(loopConfiguration, "WS_1")
				Dim array As INDArray = Nd4j.create(100)
			End Using



			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(loopConfiguration, "WS_1"), Nd4jWorkspace)
			workspace.initializeWorkspace()

			assertEquals(100 * Nd4j.sizeOfDataType(), workspace.CurrentSize)

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(loopConfiguration, "WS_1")
				Dim array As INDArray = Nd4j.create(1000)
			End Using

			Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(loopConfiguration, "WS_1").initializeWorkspace()

			assertEquals(100 * Nd4j.sizeOfDataType(), workspace.CurrentSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces7(ByVal backend As Nd4jBackend)
			Using wsExternal As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "External"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array1 As INDArray = Nd4j.create(10)
				Dim array2 As INDArray = Nothing
				Dim array3 As INDArray = Nothing
				Dim array4 As INDArray = Nothing
				Dim array5 As INDArray = Nothing


				Using wsFeedForward As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfiguration, "FeedForward"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					array2 = Nd4j.create(10)
					assertEquals(True, array2.Attached)

					Using borrowed As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("External").notifyScopeBorrowed(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
						array3 = Nd4j.create(10)

						assertTrue(wsExternal Is array3.data().ParentWorkspace)

						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							array4 = Nd4j.create(10)
						End Using

						array5 = Nd4j.create(10)
						log.info("Workspace5: {}", array5.data().ParentWorkspace)
						assertTrue(Nothing Is array4.data().ParentWorkspace)
						assertFalse(array4.Attached)
						assertTrue(wsExternal Is array5.data().ParentWorkspace)
					End Using

					assertEquals(True, array3.Attached)
					assertEquals(False, array4.Attached)
					assertEquals(True, array5.Attached)
				End Using
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces6(ByVal backend As Nd4jBackend)

			Using wsExternal As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(firstConfiguration, "External"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array1 As INDArray = Nd4j.create(10)
				Dim array2 As INDArray = Nothing
				Dim array3 As INDArray = Nothing
				Dim array4 As INDArray = Nothing


				Using wsFeedForward As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(firstConfiguration, "FeedForward"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					array2 = Nd4j.create(10)
					assertEquals(True, array2.Attached)

					Using borrowed As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("External").notifyScopeBorrowed(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
						array3 = Nd4j.create(10)

						assertTrue(wsExternal Is array3.data().ParentWorkspace)
					End Using

					assertEquals(True, array3.Attached)

					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						array4 = Nd4j.create(10)
					End Using

					assertEquals(False, array4.Attached)
				End Using


				assertEquals(0, wsExternal.CurrentSize)
				log.info("------")
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces5(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration
			Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				Dim array1 As INDArray = Nd4j.create(100)
				Using ws2 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

					Dim array2 As INDArray = Nd4j.create(100)
				End Using

				Dim reqMem As Long = 200 * Nd4j.sizeOfDataType()
				assertEquals(reqMem + reqMem Mod 8, ws1.PrimaryOffset)

				Dim array3 As INDArray = Nd4j.create(100)

				reqMem = 300 * Nd4j.sizeOfDataType()
				assertEquals(reqMem + reqMem Mod 8, ws1.PrimaryOffset)
			End Using

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces4(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration

			Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				Dim array1 As INDArray = Nd4j.create(100)

				Using ws2 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Dim array2 As INDArray = Nd4j.create(100)

					Using ws3 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS3").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
						Dim array3 As INDArray = Nd4j.create(100)

						assertEquals(100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
						assertEquals(100 * Nd4j.sizeOfDataType(), ws2.PrimaryOffset)
						assertEquals(100 * Nd4j.sizeOfDataType(), ws3.PrimaryOffset)
					End Using

					Dim array2b As INDArray = Nd4j.create(100)

					assertEquals(200 * Nd4j.sizeOfDataType(), ws2.PrimaryOffset)
				End Using

				Dim array1b As INDArray = Nd4j.create(100)

				assertEquals(200 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
			End Using

			Dim ws1 As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1"), Nd4jWorkspace)
			Dim ws2 As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2"), Nd4jWorkspace)
			Dim ws3 As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS3"), Nd4jWorkspace)


			assertEquals(0 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
			assertEquals(0 * Nd4j.sizeOfDataType(), ws2.PrimaryOffset)
			assertEquals(0 * Nd4j.sizeOfDataType(), ws3.PrimaryOffset)

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces3(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration


			' We open top-level workspace
			Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				Dim array1 As INDArray = Nd4j.create(100)

				assertEquals(100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)

				' we open first nested workspace
				Using ws2 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					assertEquals(0 * Nd4j.sizeOfDataType(), ws2.PrimaryOffset)

					Dim array2 As INDArray = Nd4j.create(100)

					assertEquals(100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
					assertEquals(100 * Nd4j.sizeOfDataType(), ws2.PrimaryOffset)
				End Using

				' and second nexted workspace
				Using ws3 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS3").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					assertEquals(0 * Nd4j.sizeOfDataType(), ws3.PrimaryOffset)

					Dim array2 As INDArray = Nd4j.create(100)

					assertEquals(100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
					assertEquals(100 * Nd4j.sizeOfDataType(), ws3.PrimaryOffset)
				End Using

				' this allocation should happen within top-level workspace
				Dim array1b As INDArray = Nd4j.create(100)

				assertEquals(200 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
			End Using

			assertEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces2(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration

			Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				Dim array1 As INDArray = Nd4j.create(100)

				assertEquals(100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)

				For x As Integer = 1 To 100
					Using ws2 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(loopConfiguration, "WS2").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
						Dim array2 As INDArray = Nd4j.create(x)
					End Using

					Dim ws2 As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2"), Nd4jWorkspace)
					Dim reqMemory As Long = x * Nd4j.sizeOfDataType()
					assertEquals(reqMemory + reqMemory Mod 16, ws2.LastCycleAllocations)
				Next x

				Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2").initializeWorkspace()

				assertEquals(100 * Nd4j.sizeOfDataType(), Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2").CurrentSize)
			End Using

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNestedWorkspaces1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNestedWorkspaces1(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DefaultWorkspaceConfiguration = basicConfiguration


			Using ws1 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				Dim array1 As INDArray = Nd4j.create(100)

				assertEquals(100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)

				Using ws2 As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getWorkspaceForCurrentThread("WS2").notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					assertEquals(0 * Nd4j.sizeOfDataType(), ws2.PrimaryOffset)

					Dim array2 As INDArray = Nd4j.create(100)

					assertEquals(100 * Nd4j.sizeOfDataType(), ws1.PrimaryOffset)
					assertEquals(100 * Nd4j.sizeOfDataType(), ws2.PrimaryOffset)
				End Using
			End Using

			assertNull(Nd4j.MemoryManager.CurrentWorkspace)
			log.info("---------------")
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewWorkspace1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewWorkspace1(ByVal backend As Nd4jBackend)
			Dim workspace1 As MemoryWorkspace = Nd4j.WorkspaceManager.WorkspaceForCurrentThread

			assertNotEquals(Nothing, workspace1)

			Dim workspace2 As MemoryWorkspace = Nd4j.WorkspaceManager.WorkspaceForCurrentThread

			assertEquals(workspace1, workspace2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspaceGc_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWorkspaceGc_1()

			For e As Integer = 0 To 9
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				Dim wsConf As val = WorkspaceConfiguration.builder().initialSize(1000000).build()
				Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsConf, "SomeRandomName999" & f)
					Dim array As val = Nd4j.create(2, 2)
				End Using
				End Sub)
				t.start()
				t.join()

				System.GC.Collect()
				Thread.Sleep(50)
			Next e

			System.GC.Collect()
			Thread.Sleep(1000)
			System.GC.Collect()

			log.info("Done")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMemcpy1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMemcpy1(ByVal backend As Nd4jBackend)
			Dim warmUp As INDArray = Nd4j.create(100000)
			For x As Integer = 0 To 4999
				warmUp.addi(0.1)
			Next x

			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().policyMirroring(MirroringPolicy.HOST_ONLY).initialSize(1024L * 1024L * 1024L).policyLearning(LearningPolicy.NONE).build()

			Dim array As INDArray = Nd4j.createUninitialized(150000000)

			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.createNewWorkspace(configuration, "HOST")
			workspace.notifyScopeEntered()


			Dim memcpy As INDArray = array.unsafeDuplication(False)


			workspace.notifyScopeLeft()

		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace