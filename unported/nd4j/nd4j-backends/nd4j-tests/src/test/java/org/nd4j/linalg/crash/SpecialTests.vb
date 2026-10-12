Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports var = lombok.var
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.junit.jupiter.api.Assertions
Imports org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.nd4j.linalg.crash


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class SpecialTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SpecialTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimensionalThings1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimensionalThings1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(New Integer() {20, 30, 50})
			Dim y As INDArray = Nd4j.rand(x.shape())

			Dim result As INDArray = transform(x, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDimensionalThings2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDimensionalThings2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(New Integer() {20, 30, 50})
			Dim y As INDArray = Nd4j.rand(x.shape())


			For i As Integer = 0 To 0
				Dim number As Integer = 5
				Dim start As Integer = RandomUtils.nextInt(0, CInt(x.shape()(2)) - number)

				transform(getView(x, start, 5), getView(y, start, 5))
			Next i
		End Sub

		Protected Friend Shared Function getView(ByVal x As INDArray, ByVal from As Integer, ByVal number As Integer) As INDArray
			Return x.get(all(), all(), interval(from, from + number))
		End Function

		Protected Friend Shared Function transform(ByVal a As INDArray, ByVal b As INDArray) As INDArray
			Dim nShape() As Integer = {1, 2}
			Dim a_reduced As INDArray = a.sum(nShape)
			Dim b_reduced As INDArray = b.sum(nShape)

			'log.info("reduced shape: {}", Arrays.toString(a_reduced.shapeInfoDataBuffer().asInt()));

			Return Transforms.abs(a_reduced.sub(b_reduced)).div(a_reduced)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarShuffle1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarShuffle1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim listData As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To 2
				Dim features As INDArray = Nd4j.ones(25, 25)
				Dim label As INDArray = Nd4j.create(New Single() {1}, New Integer() {1})
				Dim dataset As New DataSet(features, label)
				listData.Add(dataset)
			Next i
			Dim data As DataSet = DataSet.merge(listData)
			data.shuffle()
			End Sub)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarShuffle2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarShuffle2(ByVal backend As Nd4jBackend)
			Dim listData As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To 2
				Dim features As INDArray = Nd4j.ones(14, 25)
				Dim label As INDArray = Nd4j.create(14, 50)
				Dim dataset As New DataSet(features, label)
				listData.Add(dataset)
			Next i
			Dim data As DataSet = DataSet.merge(listData)
			data.shuffle()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVstack2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVstack2(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(10000, 100)

			Dim views As IList(Of INDArray) = New List(Of INDArray)()
			views.Add(matrix.getRow(1))
			views.Add(matrix.getRow(4))
			views.Add(matrix.getRow(7))

			Dim result As INDArray = Nd4j.vstack(views)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVstack1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVstack1(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.create(10000, 100)

			Dim views As IList(Of INDArray) = New List(Of INDArray)()
			Dim i As Integer = 0
			Do While i < matrix.rows() \ 2
				views.Add(matrix.getRow(RandomUtils.nextInt(0, matrix.rows())))
				'views.add(Nd4j.create(1, 10));
				i += 1
			Loop

	'        log.info("Starting...");

			'while (true) {
			For i As Integer = 0 To 0
				Dim result As INDArray = Nd4j.vstack(views)

				System.GC.Collect()
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatMulti() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConcatMulti()
			Dim shapeA As val = New Integer() {50, 20}
			Dim shapeB As val = New Integer() {50, 497}

			'Nd4j.create(1);

			Dim executor As val = CType(Executors.newFixedThreadPool(2), ThreadPoolExecutor)

			For e As Integer = 0 To 0
				executor.submit(Sub()
				Dim arrayA As val = Nd4j.createUninitialized(shapeA)
				End Sub)
			Next e

			Thread.Sleep(1000)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatMulti2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcatMulti2(ByVal backend As Nd4jBackend)
			Nd4j.create(1)
			Dim executor As val = CType(Executors.newFixedThreadPool(2), ThreadPoolExecutor)
			executor.submit(Sub()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMigrationMultiGpu_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMigrationMultiGpu_1()
			If Nd4j.AffinityManager.NumberOfDevices < 2 Then
				Return
			End If

			Dim list As val = New CopyOnWriteArrayList(Of INDArray)()
			Dim threads As val = New List(Of Thread)()
			Dim devices As val = Nd4j.AffinityManager.NumberOfDevices
			For e As Integer = 0 To devices - 1
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
				log.info("Current device: {}", deviceId)
				For i As Integer = 0 To 9
					Dim ar As val = Nd4j.create(100, 100).assign(1.0f)
					assertEquals(deviceId, Nd4j.AffinityManager.getDeviceForArray(ar))
					list.add(ar)
					Nd4j.Executioner.commit()
				Next i
				End Sub)

				t.start()
				t.join()
				threads.add(t)

	'            log.info("------------------------");
			Next e

			For Each t As val In threads
				t.join()
			Next t

			For Each a As val In list
				Dim device As val = Nd4j.AffinityManager.getDeviceForArray(a)
				Try
					assertEquals(1.0f, a.meanNumber().floatValue(), 1e-5)
				Catch e As Exception
					log.error("Failed for array from device [{}]", device)
					Throw e
				End Try
			Next a
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMigrationMultiGpu_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMigrationMultiGpu_2()
			If Nd4j.AffinityManager.NumberOfDevices < 2 Then
				Return
			End If

			Dim wsConf As val = WorkspaceConfiguration.builder().policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.STRICT).initialSize(50 * 1024L * 1024L).build()

			For x As Integer = 0 To 9

				Dim list As val = New CopyOnWriteArrayList(Of INDArray)()
				Dim threads As val = New List(Of Thread)()
				Dim e As Integer = 0
				Do While e < Nd4j.AffinityManager.NumberOfDevices
					Dim f As val = e
					Dim t As val = New Thread(Sub()
					For i As Integer = 0 To 99
						Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsConf, "id")
							list.add(Nd4j.create(3, 3).assign(1.0f))
							Nd4j.Executioner.commit()
						End Using
					Next i
					End Sub)

					t.start()
					threads.add(t)
					e += 1
				Loop

				For Each t As val In threads
					t.join()
				Next t

				For Each a As val In list
					assertTrue(a.isAttached())
					assertEquals(1.0f, a.meanNumber().floatValue(), 1e-5)
				Next a

				System.GC.Collect()
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastLt()
		Public Overridable Sub testBroadcastLt()
			For i As Integer = 0 To 9

				Dim x As INDArray = Nd4j.create(DataType.DOUBLE, 1, 3, 2, 4, 4)
				Dim y As INDArray = Nd4j.create(DataType.DOUBLE, 1, 2, 4, 4)
				Dim z As INDArray = Nd4j.create(DataType.BOOL, 1, 3, 2, 4, 4)
				Broadcast.lt(x, y, z, 0, 2, 3, 4)

			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastLt2()
		Public Overridable Sub testBroadcastLt2()
			For i As Integer = 0 To 9
				Dim orig As INDArray = Nd4j.create(DataType.DOUBLE, 1, 7, 4, 4)
				Dim y As INDArray = orig.get(all(), interval(0,2), all(), all())

				Dim x As INDArray = Nd4j.create(DataType.DOUBLE, 1, 3, 2, 4, 4)
				Dim z As INDArray = Nd4j.create(DataType.BOOL, 1, 3, 2, 4, 4)
				Broadcast.lt(x, y, z, 0, 2, 3, 4)

			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void reproduceWorkspaceCrash()
		Public Overridable Sub reproduceWorkspaceCrash()
			Dim conf As val = WorkspaceConfiguration.builder().build()

			Dim ws As val = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(conf, "WS")

			Dim arr As INDArray = Nd4j.create(New Double(){1, 0, 0, 0, 1, 0, 0, 0, 0, 0}, New Long(){1, 10})

			'assertNotEquals(Nd4j.defaultFloatingPointType(), arr.dataType());
			Nd4j.setDefaultDataTypes(DataType.DOUBLE, DataType.DOUBLE)

			For i As Integer = 0 To 99
				Using ws2 As lombok.val = ws.notifyScopeEntered()
	'                System.out.println("Iteration: " + i);
					Dim ok As INDArray = arr.eq(0.0)
					ok.dup()

					assertEquals(arr.dataType(), Nd4j.defaultFloatingPointType())
					assertEquals(DataType.DOUBLE, Nd4j.defaultFloatingPointType())
					Dim crash As INDArray = arr.eq(0.0).castTo(Nd4j.defaultFloatingPointType())
					crash.dup() 'Crashes here on i=1 iteration
				End Using
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void reproduceWorkspaceCrash_2()
		Public Overridable Sub reproduceWorkspaceCrash_2()
			Dim dtypes As val = New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.LONG, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.UBYTE, DataType.BOOL}
			For Each dX As val In dtypes
				For Each dZ As val In dtypes
					Dim array As val = Nd4j.create(dX, 2, 5).assign(1)

	'                log.info("Trying to cast {} to {}", dX, dZ);
					Dim casted As val = array.castTo(dZ)

					Dim exp As val = Nd4j.create(dZ, 2, 5).assign(1)
					assertEquals(exp, casted)
				Next dZ
			Next dX
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void reproduceWorkspaceCrash_3()
		Public Overridable Sub reproduceWorkspaceCrash_3()
			Dim conf As val = WorkspaceConfiguration.builder().build()

			Dim ws As val = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(conf, "WS")
			Dim dtypes As val = New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.LONG, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.UBYTE, DataType.BOOL}
			For Each dX As val In dtypes
				For Each dZ As val In dtypes
					Using ws2 As lombok.val = ws.notifyScopeEntered()
						Dim array As val = Nd4j.create(dX, 2, 5).assign(1)
	'                    log.info("Trying to cast {} to {}", dX, dZ);
						Dim casted As val = array.castTo(dZ)
						Dim exp As val = Nd4j.create(dZ, 2, 5).assign(1)
						assertEquals(exp, casted)

						Nd4j.Executioner.commit()
					End Using
				Next dZ
			Next dX
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCastLong_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCastLong_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.LONG, 100, 100).assign(1)
			Dim second As val = Nd4j.create(DataType.LONG, 100, 100).assign(1)
	'        log.info("----------------");
			Dim castedA As val = array.castTo(DataType.BYTE).assign(3)
			Dim castedB As val = array.castTo(DataType.BYTE).assign(3)
			Nd4j.Executioner.commit()
			assertEquals(castedA, castedB)

			assertEquals(array, second)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCastHalf_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCastHalf_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.HALF, 2, 5).assign(1)
			assertEquals(10.0f, array.sumNumber().floatValue(), 1e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCastHalf_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCastHalf_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.HALF, 2, 5).assign(1)
			assertEquals(10.0f, array.sumNumber().floatValue(), 1e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCastHalf_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCastHalf_3(ByVal backend As Nd4jBackend)
			Dim arrayY As val = Nd4j.create(DataType.FLOAT, 2, 5).assign(2)
			Dim arrayX As val = Nd4j.create(DataType.HALF, 2, 5).assign(arrayY)
			assertEquals(20.0f, arrayX.sumNumber().floatValue(), 1e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce_Small_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce_Small_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.SHORT, 100, 30).assign(1)
			assertEquals(3000, array.sumNumber().intValue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce_Small_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce_Small_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.BYTE, 100, 100).assign(0)
			assertEquals(0, array.sumNumber().intValue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce3_Small_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce3_Small_1(ByVal backend As Nd4jBackend)
			Dim arrayA As val = Nd4j.create(DataType.SHORT, 100, 100).assign(1)
			Dim arrayB As val = Nd4j.create(DataType.SHORT, 100, 100).assign(1)
			assertEquals(arrayA, arrayB)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce3_Small_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce3_Small_2(ByVal backend As Nd4jBackend)
			Dim arrayA As val = Nd4j.create(DataType.BYTE, 100, 100).assign(1)
			Dim arrayB As val = Nd4j.create(DataType.BYTE, 100, 100).assign(1)
			assertEquals(arrayA, arrayB)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void reproduceWorkspaceCrash_4()
		Public Overridable Sub reproduceWorkspaceCrash_4()
			Dim conf As val = WorkspaceConfiguration.builder().build()

			Dim ws As val = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(conf, "WS")
			Dim dtypes As val = New DataType(){DataType.LONG, DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.UBYTE, DataType.BOOL}
			For Each dX As val In dtypes
				For Each dZ As val In dtypes
					Using ws2 As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace("WS")
						Dim array As val = Nd4j.create(dX, 100, 100).assign(1)

	'                    log.info("Trying to cast {} to {}", dX, dZ);
						Dim casted As val = array.castTo(dZ)

						Dim exp As val = Nd4j.create(dZ, 100, 100).assign(1)
						assertEquals(exp, casted)
					End Using
				Next dZ
			Next dX
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void reproduceWorkspaceCrash_5()
		Public Overridable Sub reproduceWorkspaceCrash_5()
			Dim conf As val = WorkspaceConfiguration.builder().build()

			Dim ws As val = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(conf, "WS")

			Dim arr As INDArray = Nd4j.create(New Double(){1, 0, 0, 0, 1, 0, 0, 0, 0, 0}, New Long(){1, 10})

			Nd4j.setDefaultDataTypes(DataType.DOUBLE, DataType.DOUBLE)
			assertEquals(DataType.DOUBLE, arr.dataType())

			For i As Integer = 0 To 99
				Using ws2 As lombok.val = ws.notifyScopeEntered()
					Dim crash As INDArray = arr.castTo(DataType.BOOL).castTo(DataType.DOUBLE)
					crash.dup()
				End Using
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcatAgain()
		Public Overridable Sub testConcatAgain()
			Dim toConcat(2) As INDArray
			For i As Integer = 0 To toConcat.Length - 1
				toConcat(i) = Nd4j.valueArrayOf(New Long(){10, 1}, i).castTo(DataType.FLOAT)
			Next i

			Dim [out] As INDArray = Nd4j.concat(1, toConcat)
	'        System.out.println(out);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat2()
		Public Overridable Sub testConcat2()
			'Nd4j.getExecutioner().enableDebugMode(true);
			'Nd4j.getExecutioner().enableVerboseMode(true);
			Dim n As Integer = 784 'OK for 10, 100, 500
			'Fails for 784, 783, 750, 720, 701, 700

			Dim arrs(n - 1) As INDArray
			For i As Integer = 0 To n - 1
				Dim a As INDArray = Nd4j.create(DataType.DOUBLE, 10,1).assign(i) 'Also fails for FLOAT
				arrs(i) = a
			Next i

			Nd4j.Executioner.commit()
			Dim [out] As INDArray = Nothing
			For e As Integer = 0 To 4
				If e Mod 10 = 0 Then
	'                log.info("Iteration: [{}]", e);

					[out] = Nd4j.concat(1, arrs)
				End If
			Next e
			Nd4j.Executioner.commit()
	'        System.out.println(out);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testYoloStyle()
		Public Overridable Sub testYoloStyle()
			Dim WS_ALL_LAYERS_ACT_CONFIG As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.05).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()



			For i As Integer = 0 To 9
				Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(WS_ALL_LAYERS_ACT_CONFIG, "ws")
	'                System.out.println("STARTING: " + i);

					Dim objectPresentMask As INDArray = Nd4j.create(DataType.BOOL, 1,4,4)

					Dim shape() As Long = {1, 3, 2, 4, 4}
					Dim noIntMask1 As INDArray = Nd4j.createUninitialized(DataType.BOOL, shape, "c"c)
					Dim noIntMask2 As INDArray = Nd4j.createUninitialized(DataType.BOOL, shape, "c"c)

					noIntMask1 = Transforms.or(noIntMask1.get(all(), all(), point(0), all(), all()), noIntMask1.get(all(), all(), point(1), all(), all())) 'Shape: [mb, b, H, W]. Values 1 if no intersection
					noIntMask2 = Transforms.or(noIntMask2.get(all(), all(), point(0), all(), all()), noIntMask2.get(all(), all(), point(1), all(), all()))
					Dim noIntMask As INDArray = Transforms.or(noIntMask1, noIntMask2)

					Nd4j.Executioner.commit()

					Dim intMask As INDArray = Transforms.not(noIntMask) 'Values 0 if no intersection
					Nd4j.Executioner.commit()

					Broadcast.mul(intMask, objectPresentMask, intMask, 0, 2, 3)
					Nd4j.Executioner.commit()
	'                System.out.println("DONE: " + i);
				End Using
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpaceToBatch(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpaceToBatch(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(7331)

			Dim miniBatch As Integer = 4
			Dim inputShape() As Integer = {1, 2, 2, 1}

			Dim M As Integer = 2

			Dim input As INDArray = Nd4j.randn(inputShape).castTo(DataType.DOUBLE)
			Dim blocks As INDArray = Nd4j.createFromArray(2, 2)
			Dim padding As INDArray = Nd4j.createFromArray(0, 0, 0, 0).reshape(2,2)

			Dim expOut As INDArray = Nd4j.create(DataType.DOUBLE, miniBatch, 1, 1, 1)
			Dim op As val = DynamicCustomOp.builder("space_to_batch_nd").addInputs(input, blocks, padding).addOutputs(expOut).build()
			Nd4j.Executioner.execAndReturn(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBatchToSpace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBatchToSpace(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(1337)

			Dim miniBatch As Integer = 4
			Dim inputShape() As Integer = {miniBatch, 1, 1, 1}

			Dim M As Integer = 2

			Dim input As INDArray = Nd4j.randn(inputShape).castTo(DataType.DOUBLE)
			Dim blocks As INDArray = Nd4j.createFromArray(2, 2)
			Dim crops As INDArray = Nd4j.createFromArray(0, 0, 0, 0).reshape(2,2)

			Dim expOut As INDArray = Nd4j.create(DataType.DOUBLE, 1, 2, 2, 1)
			Dim op As DynamicCustomOp = DynamicCustomOp.builder("batch_to_space_nd").addInputs(input, blocks, crops).addOutputs(expOut).build()
			Nd4j.Executioner.execAndReturn(op)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testYoloS()
		Public Overridable Sub testYoloS()
			'Nd4j.getExecutioner().enableDebugMode(true);
			'Nd4j.getExecutioner().enableVerboseMode(true);
			'Nd4j.setDefaultDataTypes(DataType.DOUBLE, DataType.DOUBLE);

			Dim WS_ALL_LAYERS_ACT_CONFIG As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).overallocationLimit(0.05).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()


			Dim labels As INDArray = Nd4j.create(DataType.DOUBLE, 1,7,5,7)

			For i As Integer = 0 To 9
				Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(WS_ALL_LAYERS_ACT_CONFIG, "ws")
	'                System.out.println("STARTING: " + i);

					Dim nhw As val = New Long(){1, 5, 7}

					Dim size1 As val = labels.size(1)
					Dim classLabels As INDArray = labels.get(all(), interval(4,size1), all(), all()) 'Shape: [minibatch, nClasses, H, W]
					Dim maskObjectPresent As INDArray = classLabels.sum(Nd4j.createUninitialized(DataType.DOUBLE, nhw, "c"c), 1).castTo(DataType.BOOL) 'Shape: [minibatch, H, W]

					Dim labelTLXY As INDArray = labels.get(all(), interval(0,2), all(), all())
					Dim labelBRXY As INDArray = labels.get(all(), interval(2,4), all(), all())

					Nd4j.Executioner.commit()

					Dim labelCenterXY As INDArray = labelTLXY.add(labelBRXY)
					Dim m As val = labelCenterXY.muli(0.5) 'In terms of grid units
					Dim labelsCenterXYInGridBox As INDArray = labelCenterXY.dup(labelCenterXY.ordering()) '[mb, 2, H, W]
					Nd4j.Executioner.commit()
	'                System.out.println("DONE: " + i);
				End Using
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchCondition()
		Public Overridable Sub testMatchCondition()
			Dim x As INDArray = Nd4j.valueArrayOf(New Long(){10, 10}, 2.0, DataType.DOUBLE)
			Dim op As val = New MatchCondition(x, Conditions.equals(2))
			Dim z As INDArray = Nd4j.Executioner.exec(op)
			Dim count As Integer = z.getInt(0)
			assertEquals(100, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcastMul_bool(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcastMul_bool(ByVal backend As Nd4jBackend)
			Dim mask As val = Nd4j.create(DataType.BOOL, 1, 3, 4, 4)
			Dim [object] As val = Nd4j.create(DataType.BOOL, 1, 4, 4)

			Broadcast.mul(mask, [object], mask, 0, 2, 3)
			Nd4j.Executioner.commit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshape()
		Public Overridable Sub testReshape()
			Dim c As INDArray = Nd4j.linspace(1,6,6, DataType.DOUBLE).reshape("c"c, 2,3)
			Dim f As INDArray = c.dup("f"c)
			Dim fr As val = f.reshape("f"c, 3, 2).dup("f"c)

	'        log.info("FO: {}", f.data().asFloat());
	'        log.info("FR: {}", fr.data().asFloat());

			Dim outC As INDArray = Nd4j.create(DataType.DOUBLE, 3,2)
			Dim outF As INDArray = Nd4j.create(DataType.DOUBLE, 3,2)

			Dim op As var = DynamicCustomOp.builder("reshape").addInputs(c).addOutputs(outC).addIntegerArguments(3,2).build()

			Nd4j.Executioner.exec(op)

			op = DynamicCustomOp.builder("reshape").addInputs(f).addOutputs(outF).addIntegerArguments(-99, 3,2).build()

			Nd4j.Executioner.exec(op)

			assertEquals(outC, outF)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace