Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.junit.jupiter.api
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports LocationPolicy = org.nd4j.linalg.api.memory.enums.LocationPolicy
Imports MirroringPolicy = org.nd4j.linalg.api.memory.enums.MirroringPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.api.buffer.DataType.DOUBLE

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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag @Execution(ExecutionMode.SAME_THREAD) public class BasicWorkspaceTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class BasicWorkspaceTests
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()

		Private Shared ReadOnly basicConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

		Private Shared ReadOnly loopOverTimeConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()


		Private Shared ReadOnly loopFirstConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			Nd4j.DataType = [DOUBLE]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void shutdown()
		Public Overridable Sub shutdown()
			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()

			Nd4j.DataType = initialType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCold(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCold(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(10)

			array.addi(1.0)

			assertEquals(10f, array.sumNumber().floatValue(), 0.01f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMinSize1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMinSize1(ByVal backend As Nd4jBackend)
			Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().minSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Using workspace As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, "WT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array As INDArray = Nd4j.create(100)

				assertEquals(0, workspace.CurrentSize)
			End Using

			Using workspace As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, "WT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array As INDArray = Nd4j.create(100)

				assertEquals(10 * 1024 * 1024, workspace.CurrentSize)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBreakout2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBreakout2(ByVal backend As Nd4jBackend)

			assertEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			Dim scoped As INDArray = outScope2()

			assertEquals(Nothing, scoped)

			assertEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBreakout1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBreakout1(ByVal backend As Nd4jBackend)

			assertEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			Dim scoped As INDArray = outScope1()

			assertEquals(True, scoped.Attached)

			assertEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)
		End Sub

		Private Function outScope2() As INDArray
			Try
				Using wsOne As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "EXT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Throw New Exception()
				End Using
			Catch e As Exception
				Return Nothing
			End Try
		End Function

		Private Function outScope1() As INDArray
			Using wsOne As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "EXT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Return Nd4j.create(10)
			End Using
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeverage3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeverage3(ByVal backend As Nd4jBackend)
			Using wsOne As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "EXT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array As INDArray = Nothing
				Using wsTwo As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "INT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Dim matrix As INDArray = Nd4j.create(32, 1, 40)

					Dim view As INDArray = matrix.tensorAlongDimension(0, 1, 2)
					view.assign(1.0f)
					assertEquals(40.0f, matrix.sumNumber().floatValue(), 0.01f)
					assertEquals(40.0f, view.sumNumber().floatValue(), 0.01f)
					array = view.leverageTo("EXT")
				End Using

				assertEquals(40.0f, array.sumNumber().floatValue(), 0.01f)
			End Using
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeverageTo2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeverageTo2(ByVal backend As Nd4jBackend)
			Dim exp As val = Nd4j.scalar(15.0)
			Using wsOne As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(loopOverTimeConfig, "EXT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array1 As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})
				Dim array3 As INDArray = Nothing

				Using wsTwo As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "INT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Dim array2 As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

					Dim reqMemory As Long = 5 * Nd4j.sizeOfDataType([DOUBLE])

					array3 = array2.leverageTo("EXT")

					assertEquals(0, wsOne.CurrentSize)

					assertEquals(15f, array3.sumNumber().floatValue(), 0.01f)

					array2.assign(0)

					assertEquals(15f, array3.sumNumber().floatValue(), 0.01f)
				End Using

				Using wsTwo As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "INT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Dim array2 As INDArray = Nd4j.create(100)
				End Using

				assertEquals(15f, array3.sumNumber().floatValue(), 0.01f)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeverageTo1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeverageTo1(ByVal backend As Nd4jBackend)
			Using wsOne As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "EXT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array1 As INDArray = Nd4j.create([DOUBLE], 5)

				Using wsTwo As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "INT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Dim array2 As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

					Dim reqMemory As Long = 5 * Nd4j.sizeOfDataType([DOUBLE])
					assertEquals(reqMemory + reqMemory Mod 16, wsOne.PrimaryOffset)

					array2.leverageTo("EXT")

					assertEquals((reqMemory + reqMemory Mod 16) * 2, wsOne.PrimaryOffset)
				End Using
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOutOfScope1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOutOfScope1(ByVal backend As Nd4jBackend)
			Using wsOne As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "EXT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array1 As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

				Dim reqMemory As Long = 5 * Nd4j.sizeOfDataType(array1.dataType())
				assertEquals(reqMemory + reqMemory Mod 16, wsOne.PrimaryOffset)

				Dim array2 As INDArray

				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					array2 = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})
				End Using
				assertFalse(array2.Attached)

				log.info("Current workspace: {}", Nd4j.MemoryManager.CurrentWorkspace)
				assertTrue(wsOne Is Nd4j.MemoryManager.CurrentWorkspace)

				Dim array3 As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

				reqMemory = 5 * Nd4j.sizeOfDataType(array3.dataType())
				assertEquals((reqMemory + reqMemory Mod 16) * 2, wsOne.PrimaryOffset)

				array1.addi(array2)

				assertEquals(30.0f, array1.sumNumber().floatValue(), 0.01f)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeverage1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeverage1(ByVal backend As Nd4jBackend)
			Using wsOne As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "EXT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				assertEquals(0, wsOne.PrimaryOffset)

				Using wsTwo As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "INT"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

					Dim array As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

					assertEquals(0, wsOne.PrimaryOffset)

					Dim reqMemory As Long = 5 * Nd4j.sizeOfDataType(array.dataType())
					assertEquals(reqMemory + reqMemory Mod 16, wsTwo.PrimaryOffset)

					Dim copy As INDArray = array.leverage()

					assertEquals(reqMemory + reqMemory Mod 16, wsTwo.PrimaryOffset)
					assertEquals(reqMemory + reqMemory Mod 16, wsOne.PrimaryOffset)

					assertNotEquals(Nothing, copy)

					assertTrue(copy.Attached)

					assertEquals(15.0f, copy.sumNumber().floatValue(), 0.01f)
				End Using
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoShape1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoShape1(ByVal backend As Nd4jBackend)
			Dim outDepth As Integer = 50
			Dim miniBatch As Integer = 64
			Dim outH As Integer = 8
			Dim outW As Integer = 8

			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim delta As INDArray = Nd4j.create(New Integer() {50, 64, 8, 8}, New Integer() {64, 3200, 8, 1}, "c"c)
				delta = delta.permute(1, 0, 2, 3)

				assertArrayEquals(New Long() {64, 50, 8, 8}, delta.shape())
				assertArrayEquals(New Long() {3200, 64, 8, 1}, delta.stride())

				Dim delta2d As INDArray = Shape.newShapeNoCopy(delta, New Integer() {outDepth, miniBatch * outH * outW}, False)

				assertNotNull(delta2d)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateDetached1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateDetached1(ByVal backend As Nd4jBackend)
			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)

				Dim array1 As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

				Dim array2 As INDArray = Nd4j.createUninitializedDetached([DOUBLE], ChrW(5))

				array2.assign(array1)

				Dim reqMemory As Long = 5 * Nd4j.sizeOfDataType(array1.dataType())
				assertEquals(reqMemory + reqMemory Mod 16, wsI.PrimaryOffset)
				assertEquals(array1, array2)

				Dim array3 As INDArray = Nd4j.createUninitializedDetached(DataType.FLOAT, New Long(){})
				assertTrue(array3.Scalar)
				assertEquals(1, array3.length())
				assertEquals(1, array3.data().length())
			End Using
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDetach1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDetach1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nothing
			Dim copy As INDArray = Nothing
			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				array = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

				' despite we're allocating this array in workspace, it's empty yet, so it's external allocation
				assertTrue(array.InScope)
				assertTrue(array.Attached)

				Dim reqMemory As Long = 5 * Nd4j.sizeOfDataType(array.dataType())
				assertEquals(reqMemory + reqMemory Mod 16, wsI.PrimaryOffset)

				copy = array.detach()

				assertTrue(array.InScope)
				assertTrue(array.Attached)
				assertEquals(reqMemory + reqMemory Mod 16, wsI.PrimaryOffset)

				assertFalse(copy.Attached)
				assertTrue(copy.InScope)
				assertEquals(reqMemory + reqMemory Mod 16, wsI.PrimaryOffset)
			End Using

			assertEquals(15.0f, copy.sumNumber().floatValue(), 0.01f)
			assertFalse(array Is copy)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScope2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScope2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nothing
			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(loopFirstConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				array = Nd4j.create([DOUBLE], 100)

				' despite we're allocating this array in workspace, it's empty yet, so it's external allocation
				assertTrue(array.InScope)
				assertEquals(0, wsI.CurrentSize)
			End Using


			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(loopFirstConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				array = Nd4j.create([DOUBLE], 100)

				assertTrue(array.InScope)
				assertEquals(100 * Nd4j.sizeOfDataType(array.dataType()), wsI.PrimaryOffset)
			End Using

			assertFalse(array.InScope)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScope1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScope1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nothing
			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				array = Nd4j.create([DOUBLE], 100)

				assertTrue(array.InScope)
			End Using

			assertFalse(array.InScope)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsAttached3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsAttached3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create([DOUBLE], 100)
			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim arrayL As INDArray = array.leverageTo("ITER")

				assertFalse(array.Attached)
				assertFalse(arrayL.Attached)

			End Using

			Dim array2 As INDArray = Nd4j.create([DOUBLE], 100)

			assertFalse(array.Attached)
			assertFalse(array2.Attached)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsAttached2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsAttached2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create([DOUBLE], 100)
			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(loopFirstConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim arrayL As INDArray = array.leverageTo("ITER")

				assertFalse(array.Attached)
				assertFalse(arrayL.Attached)
			End Using

			Dim array2 As INDArray = Nd4j.create(100)

			assertFalse(array.Attached)
			assertFalse(array2.Attached)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIsAttached1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIsAttached1(ByVal backend As Nd4jBackend)

			Using wsI As org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(loopFirstConfig, "ITER"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				Dim array As INDArray = Nd4j.create([DOUBLE], 100)

				assertTrue(array.Attached)
			End Using

			Dim array As INDArray = Nd4j.create(100)

			assertFalse(array.Attached)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOverallocation3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOverallocation3(ByVal backend As Nd4jBackend)
			Dim overallocationConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(overallocationConfig), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertEquals(0, workspace.CurrentSize)

			For x As Integer = 10 To 100 Step 10
				Using cW As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
					Dim array As INDArray = Nd4j.create([DOUBLE], x)
				End Using
			Next x

			assertEquals(0, workspace.CurrentSize)

			workspace.initializeWorkspace()


			' should be 800 = 100 elements * 4 bytes per element * 2 as overallocation coefficient
			assertEquals(200 * Nd4j.sizeOfDataType([DOUBLE]), workspace.CurrentSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOverallocation2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOverallocation2(ByVal backend As Nd4jBackend)
			Dim overallocationConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(overallocationConfig), Nd4jWorkspace)

			'Nd4j.getMemoryManager().setCurrentWorkspace(workspace);

			assertEquals(0, workspace.CurrentSize)

			Using cW As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
				Dim array As INDArray = Nd4j.create([DOUBLE], 100)
			End Using

			' should be 800 = 100 elements * 4 bytes per element * 2 as overallocation coefficient
			assertEquals(200 * Nd4j.sizeOfDataType([DOUBLE]), workspace.CurrentSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOverallocation1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOverallocation1(ByVal backend As Nd4jBackend)
			Dim overallocationConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024).maxSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.NONE).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(overallocationConfig), Nd4jWorkspace)

			assertEquals(2048, workspace.CurrentSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToggle1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToggle1(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(loopFirstConfig), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Using cW As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
				Dim array1 As INDArray = Nd4j.create([DOUBLE], 100)

				cW.toggleWorkspaceUse(False)

				Dim arrayDetached As INDArray = Nd4j.create([DOUBLE], 100)

				arrayDetached.assign(1.0f)

				Dim sum As Double = arrayDetached.sumNumber().doubleValue()
				assertEquals(100f, sum, 0.01)

				cW.toggleWorkspaceUse(True)

				Dim array2 As INDArray = Nd4j.create([DOUBLE], 100)
			End Using

			assertEquals(0, workspace.PrimaryOffset)
			assertEquals(200 * Nd4j.sizeOfDataType([DOUBLE]), workspace.CurrentSize)

			log.info("--------------------------")

			Using cW As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
				Dim array1 As INDArray = Nd4j.create([DOUBLE], 100)

				cW.toggleWorkspaceUse(False)

				Dim arrayDetached As INDArray = Nd4j.create([DOUBLE], 100)

				arrayDetached.assign(1.0f)

				Dim sum As Double = arrayDetached.sumNumber().doubleValue()
				assertEquals(100f, sum, 0.01)

				cW.toggleWorkspaceUse(True)

				assertEquals(100 * Nd4j.sizeOfDataType([DOUBLE]), workspace.PrimaryOffset)

				Dim array2 As INDArray = Nd4j.create([DOUBLE], 100)

				assertEquals(200 * Nd4j.sizeOfDataType([DOUBLE]), workspace.PrimaryOffset)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLoop4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLoop4(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(loopFirstConfig), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Using cW As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
				Dim array1 As INDArray = Nd4j.create([DOUBLE], 100)
				Dim array2 As INDArray = Nd4j.create([DOUBLE], 100)
			End Using

			assertEquals(0, workspace.PrimaryOffset)
			assertEquals(200 * Nd4j.sizeOfDataType([DOUBLE]), workspace.CurrentSize)

			Using cW As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
				Dim array1 As INDArray = Nd4j.create([DOUBLE], 100)

				assertEquals(100 * Nd4j.sizeOfDataType([DOUBLE]), workspace.PrimaryOffset)
			End Using

			assertEquals(0, workspace.PrimaryOffset)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLoops3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLoops3(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(loopFirstConfig), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			workspace.notifyScopeEntered()

			Dim arrayCold1 As INDArray = Nd4j.create([DOUBLE], 100)
			Dim arrayCold2 As INDArray = Nd4j.create([DOUBLE], 10)

			assertEquals(0, workspace.PrimaryOffset)
			assertEquals(0, workspace.CurrentSize)

			workspace.notifyScopeLeft()

			assertEquals(0, workspace.PrimaryOffset)

			Dim reqMem As Long = 110 * Nd4j.sizeOfDataType([DOUBLE])

			assertEquals(reqMem + reqMem Mod 8, workspace.CurrentSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLoops2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLoops2(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(loopOverTimeConfig), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			For x As Integer = 1 To 100
				workspace.notifyScopeEntered()

				Dim arrayCold As INDArray = Nd4j.create([DOUBLE], x)

				assertEquals(0, workspace.PrimaryOffset)
				assertEquals(0, workspace.CurrentSize)

				workspace.notifyScopeLeft()
			Next x

			workspace.initializeWorkspace()

			Dim reqMem As Long = 100 * Nd4j.sizeOfDataType([DOUBLE])

			'assertEquals(reqMem + reqMem % 8, workspace.getCurrentSize());
			assertEquals(0, workspace.PrimaryOffset)

			workspace.notifyScopeEntered()

			Dim arrayHot As INDArray = Nd4j.create([DOUBLE], 10)

			reqMem = 10 * Nd4j.sizeOfDataType([DOUBLE])
			assertEquals(reqMem + reqMem Mod 8, workspace.PrimaryOffset)

			workspace.notifyScopeLeft()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLoops1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLoops1(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(loopOverTimeConfig), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			workspace.notifyScopeEntered()

			Dim arrayCold As INDArray = Nd4j.create([DOUBLE], 10)

			assertEquals(0, workspace.PrimaryOffset)
			assertEquals(0, workspace.CurrentSize)

			arrayCold.assign(1.0f)

			assertEquals(10f, arrayCold.sumNumber().floatValue(), 0.01f)

			workspace.notifyScopeLeft()


			workspace.initializeWorkspace()
			Dim reqMemory As Long = 11 * Nd4j.sizeOfDataType(arrayCold.dataType())
			assertEquals(reqMemory + reqMemory Mod 16, workspace.CurrentSize)


			log.info("-----------------------")

			For x As Integer = 0 To 9
				assertEquals(0, workspace.PrimaryOffset)

				workspace.notifyScopeEntered()

				Dim array As INDArray = Nd4j.create([DOUBLE], 10)


				Dim reqMem As Long = 10 * Nd4j.sizeOfDataType(array.dataType())

				assertEquals(reqMem + reqMem Mod 16, workspace.PrimaryOffset)

				array.addi(1.0)

				assertEquals(reqMem + reqMem Mod 16, workspace.PrimaryOffset)

				assertEquals(10, array.sumNumber().doubleValue(), 0.01,"Failed on iteration " & x)

				workspace.notifyScopeLeft()

				assertEquals(0, workspace.PrimaryOffset)
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllocation6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllocation6(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "testAllocation6"), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Dim array As INDArray = Nd4j.rand([DOUBLE], 100, 10)

			' checking if allocation actually happened
			assertEquals(1000 * Nd4j.sizeOfDataType(array.dataType()), workspace.PrimaryOffset)

			Dim dup As INDArray = array.dup()

			assertEquals(2000 * Nd4j.sizeOfDataType(dup.dataType()), workspace.PrimaryOffset)

			'assertEquals(5, dup.sumNumber().doubleValue(), 0.01);

			workspace.close()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllocation5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllocation5(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "testAllocation5"), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Dim array As INDArray = Nd4j.create([DOUBLE], New Long() {1, 5}, "c"c)

			' checking if allocation actually happened
			Dim reqMemory As Long = 5 * Nd4j.sizeOfDataType([DOUBLE])
			assertEquals(reqMemory + reqMemory Mod 16, workspace.PrimaryOffset)

			array.assign(1.0f)

			Dim dup As INDArray = array.dup()

			assertEquals((reqMemory + reqMemory Mod 16) * 2, workspace.PrimaryOffset)

			assertEquals(5, dup.sumNumber().doubleValue(), 0.01)

			workspace.close()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllocation4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllocation4(ByVal backend As Nd4jBackend)
			Dim failConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024 * 1024).maxSize(1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.FAIL).build()


			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.createNewWorkspace(failConfig), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Dim array As INDArray = Nd4j.create([DOUBLE], New Long() {1, 5}, "c"c)

			' checking if allocation actually happened
			Dim reqMem As Long = 5 * Nd4j.sizeOfDataType(array.dataType())
			assertEquals(reqMem + reqMem Mod 16, workspace.PrimaryOffset)

			Try
				Dim array2 As INDArray = Nd4j.create([DOUBLE], 10000000)
				assertTrue(False)
			Catch e As ND4JIllegalStateException
				assertTrue(True)
			End Try

			assertEquals(reqMem + reqMem Mod 16, workspace.PrimaryOffset)

			Dim array2 As INDArray = Nd4j.create([DOUBLE], New Long() {1, 5}, "c"c)

			assertEquals((reqMem + reqMem Mod 16) * 2, workspace.PrimaryOffset)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllocation3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllocation3(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "testAllocation2"), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Dim array As INDArray = Nd4j.create([DOUBLE], New Long() {1, 5}, "c"c)

			' checking if allocation actually happened
			Dim reqMem As Long = 5 * Nd4j.sizeOfDataType(array.dataType())
			assertEquals(reqMem + reqMem Mod 16, workspace.PrimaryOffset)

			array.assign(1.0f)

			assertEquals(5, array.sumNumber().doubleValue(), 0.01)

			workspace.close()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllocation2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllocation2(ByVal backend As Nd4jBackend)
			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "testAllocation2"), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Dim array As INDArray = Nd4j.create([DOUBLE], 5)

			' checking if allocation actually happened
			Dim reqMem As Long = 5 * Nd4j.sizeOfDataType(array.dataType())
			assertEquals(reqMem + reqMem Mod 16, workspace.PrimaryOffset)

			array.assign(1.0f)

			assertEquals(5, array.sumNumber().doubleValue(), 0.01)

			workspace.close()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllocation1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAllocation1(ByVal backend As Nd4jBackend)



			Dim exp As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

			Dim workspace As Nd4jWorkspace = DirectCast(Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "TestAllocation1"), Nd4jWorkspace)

			Nd4j.MemoryManager.CurrentWorkspace = workspace

			assertNotEquals(Nothing, Nd4j.MemoryManager.CurrentWorkspace)

			assertEquals(0, workspace.PrimaryOffset)

			Dim array As INDArray = Nd4j.create(New Double() {1f, 2f, 3f, 4f, 5f})

			' checking if allocation actually happened
			Dim reqMem As Long = 5 * Nd4j.sizeOfDataType(array.dataType())
			assertEquals(reqMem + reqMem Mod 16, workspace.PrimaryOffset)


			assertEquals(exp, array)

			' checking stuff at native side
			Dim sum As Double = array.sumNumber().doubleValue()
			assertEquals(15.0, sum, 0.01)

			' checking INDArray validity
			assertEquals(1.0, array.getFloat(0), 0.01)
			assertEquals(2.0, array.getFloat(1), 0.01)
			assertEquals(3.0, array.getFloat(2), 0.01)
			assertEquals(4.0, array.getFloat(3), 0.01)
			assertEquals(5.0, array.getFloat(4), 0.01)


			' checking INDArray validity
			assertEquals(1.0, array.getDouble(0), 0.01)
			assertEquals(2.0, array.getDouble(1), 0.01)
			assertEquals(3.0, array.getDouble(2), 0.01)
			assertEquals(4.0, array.getDouble(3), 0.01)
			assertEquals(5.0, array.getDouble(4), 0.01)

			' checking workspace memory space

			Dim array2 As INDArray = Nd4j.create(New Double() {5f, 4f, 3f, 2f, 1f})

			sum = array2.sumNumber().doubleValue()
			assertEquals(15.0, sum, 0.01)

			' 44 = 20 + 4 + 20, 4 was allocated as Op.extraArgs for sum
			'assertEquals(44, workspace.getPrimaryOffset());


			array.addi(array2)

			sum = array.sumNumber().doubleValue()
			assertEquals(30.0, sum, 0.01)


			' checking INDArray validity
			assertEquals(6.0, array.getFloat(0), 0.01)
			assertEquals(6.0, array.getFloat(1), 0.01)
			assertEquals(6.0, array.getFloat(2), 0.01)
			assertEquals(6.0, array.getFloat(3), 0.01)
			assertEquals(6.0, array.getFloat(4), 0.01)

			workspace.close()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) public void testMmap1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmap1(ByVal backend As Nd4jBackend)
			' we don't support MMAP on cuda yet
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			If Nd4j.Executioner.GetType().FullName.ToLower().contains("cuda") Then
				Return
			End If

			Dim mmap As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1000000).policyLocation(LocationPolicy.MMAP).policyLearning(LearningPolicy.NONE).build()

			Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.getAndActivateWorkspace(mmap, "M2")

			Dim mArray As INDArray = Nd4j.create([DOUBLE], 100)
			mArray.assign(10f)

			assertEquals(1000f, mArray.sumNumber().floatValue(), 1e-5)

			ws.close()


			ws.notifyScopeEntered()

			Dim mArrayR As INDArray = Nd4j.createUninitialized([DOUBLE], 100)
			assertEquals(1000f, mArrayR.sumNumber().floatValue(), 1e-5)

			ws.close()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) @Disabled("Still failing even with single thread execution") public void testMmap2(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMmap2(ByVal backend As Nd4jBackend)
			' we don't support MMAP on cuda yet
			If Not backend.Environment.CPU Then
				Return
			End If

			Dim tmp As File = File.createTempFile("tmp", "fdsfdf")
			tmp.deleteOnExit()
			Nd4jWorkspace.fillFile(tmp, 100000)

			Dim mmap As WorkspaceConfiguration = WorkspaceConfiguration.builder().policyLocation(LocationPolicy.MMAP).tempFilePath(tmp.getAbsolutePath()).build()

			Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.getAndActivateWorkspace(mmap, "M3")

			Dim mArray As INDArray = Nd4j.create([DOUBLE], 100)
			mArray.assign(10f)

			assertEquals(1000f, mArray.sumNumber().floatValue(), 1e-5)

			ws.notifyScopeLeft()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvalidLeverageMigrateDetach(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvalidLeverageMigrateDetach(ByVal backend As Nd4jBackend)

			Try
				Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(basicConfig, "testInvalidLeverage")

				Dim invalidArray As INDArray = Nothing

				For i As Integer = 0 To 9
					Using ws2 As org.nd4j.linalg.api.memory.MemoryWorkspace = ws.notifyScopeEntered()
						invalidArray = Nd4j.linspace(1, 10, 10, [DOUBLE])
					End Using
				Next i
				assertTrue(invalidArray.Attached)

				Dim ws2 As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(basicConfig, "testInvalidLeverage2")

				'Leverage
				Try
						Using ws3 As MemoryWorkspace = ws2.notifyScopeEntered()
						invalidArray.leverage()
						fail("Exception should be thrown")
						End Using
				Catch e As ND4JWorkspaceException
					'Expected exception
					log.info("Expected exception: {}", e.Message)
				End Try

				Try
						Using ws3 As MemoryWorkspace = ws2.notifyScopeEntered()
						invalidArray.leverageTo("testInvalidLeverage2")
						fail("Exception should be thrown")
						End Using
				Catch e As ND4JWorkspaceException
					'Expected exception
					log.info("Expected exception: {}", e.Message)
				End Try

				Try
						Using ws3 As MemoryWorkspace = ws2.notifyScopeEntered()
						invalidArray.leverageOrDetach("testInvalidLeverage2")
						fail("Exception should be thrown")
						End Using
				Catch e As ND4JWorkspaceException
					'Expected exception
					log.info("Expected exception: {}", e.Message)
				End Try

				Try
					invalidArray.leverageTo("testInvalidLeverage2")
					fail("Exception should be thrown")
				Catch e As ND4JWorkspaceException
					'Expected exception
					log.info("Expected exception: {}", e.Message)
				End Try

				'Detach
				Try
					invalidArray.detach()
					fail("Exception should be thrown")
				Catch e As ND4JWorkspaceException
					log.info("Expected exception: {}", e.Message)
				End Try


				'Migrate
				Try
						Using ws3 As MemoryWorkspace = ws2.notifyScopeEntered()
						invalidArray.migrate()
						fail("Exception should be thrown")
						End Using
				Catch e As ND4JWorkspaceException
					'Expected exception
					log.info("Expected exception: {}", e.Message)
				End Try

				Try
					invalidArray.migrate(True)
					fail("Exception should be thrown")
				Catch e As ND4JWorkspaceException
					'Expected exception
					log.info("Expected exception: {}", e.Message)
				End Try


				'Dup
				Try
					invalidArray.dup()
					fail("Exception should be thrown")
				Catch e As ND4JWorkspaceException
					log.info("Expected exception: {}", e.Message)
				End Try

				'Unsafe dup:
				Try
					invalidArray.unsafeDuplication()
					fail("Exception should be thrown")
				Catch e As ND4JWorkspaceException
					log.info("Expected exception: {}", e.Message)
				End Try

				Try
					invalidArray.unsafeDuplication(True)
					fail("Exception should be thrown")
				Catch e As ND4JWorkspaceException
					log.info("Expected exception: {}", e.Message)
				End Try


			Finally
				Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBadGenerationLeverageMigrateDetach(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBadGenerationLeverageMigrateDetach(ByVal backend As Nd4jBackend)
			Dim gen2 As INDArray = Nothing

			For i As Integer = 0 To 3
				Dim wsOuter As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(basicConfig, "testBadGeneration")

				Using wsOuter2 As org.nd4j.linalg.api.memory.MemoryWorkspace = wsOuter.notifyScopeEntered()
					Dim arr As INDArray = Nd4j.linspace(1, 10, 10, [DOUBLE])
					If i = 2 Then
						gen2 = arr
					End If

					If i = 3 Then
						Dim wsInner As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(basicConfig, "testBadGeneration2")
						Using wsInner2 As org.nd4j.linalg.api.memory.MemoryWorkspace = wsInner.notifyScopeEntered()

							'Leverage
							Try
								gen2.leverage()
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								'Expected exception
								log.info("Expected exception: {}", e.Message)
							End Try

							Try
								gen2.leverageTo("testBadGeneration2")
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								'Expected exception
								log.info("Expected exception: {}", e.Message)
							End Try

							Try
								gen2.leverageOrDetach("testBadGeneration2")
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								'Expected exception
								log.info("Expected exception: {}", e.Message)
							End Try

							Try
								gen2.leverageTo("testBadGeneration2")
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								'Expected exception
								log.info("Expected exception: {}", e.Message)
							End Try

							'Detach
							Try
								gen2.detach()
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								log.info("Expected exception: {}", e.Message)
							End Try


							'Migrate
							Try
								gen2.migrate()
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								'Expected exception
								log.info("Expected exception: {}", e.Message)
							End Try

							Try
								gen2.migrate(True)
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								'Expected exception
								log.info("Expected exception: {}", e.Message)
							End Try


							'Dup
							Try
								gen2.dup()
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								log.info("Expected exception: {}", e.Message)
							End Try

							'Unsafe dup:
							Try
								gen2.unsafeDuplication()
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								log.info("Expected exception: {}", e.Message)
							End Try

							Try
								gen2.unsafeDuplication(True)
								fail("Exception should be thrown")
							Catch e As ND4JWorkspaceException
								log.info("Expected exception: {}", e.Message)
							End Try
						End Using
					End If
				End Using
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDtypeLeverage(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDtypeLeverage(ByVal backend As Nd4jBackend)

			For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				For Each arrayDType As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					Nd4j.setDefaultDataTypes(globalDtype, globalDtype)

					Dim configOuter As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.NONE).build()
					Dim configInner As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.NONE).build()

					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configOuter, "ws")
						Dim arr As INDArray = Nd4j.create(arrayDType, 3, 4)
						Using wsInner As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configOuter, "wsInner")
							Dim leveraged As INDArray = arr.leverageTo("ws")
							assertTrue(leveraged.Attached)
							assertEquals(arrayDType, leveraged.dataType())

							Dim detached As INDArray = leveraged.detach()
							assertFalse(detached.Attached)
							assertEquals(arrayDType, detached.dataType())
						End Using
					End Using
				Next arrayDType
			Next globalDtype
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCircularWorkspaceAsymmetry_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCircularWorkspaceAsymmetry_1(ByVal backend As Nd4jBackend)
			' nothing to test on CPU here
			If Nd4j.Environment.CPU Then
				Return
			End If

			' circular workspace mode
			Dim configuration As val = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.STRICT).policySpill(SpillPolicy.FAIL).policyLearning(LearningPolicy.NONE).build()


			Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "circular_ws")
				Dim array As val = Nd4j.create(DataType.FLOAT, 10, 10)

				' we expect that this array has no data/buffer on HOST side
				assertEquals(AffinityManager.Location.DEVICE, Nd4j.AffinityManager.getActiveLocation(array))

				' since this array doesn't have HOST buffer - it will allocate one now
				array.getDouble(3L)
			End Using

			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace