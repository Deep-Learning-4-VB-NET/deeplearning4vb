Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
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

Namespace org.nd4j.linalg.api.buffer



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class DataBufferTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DataBufferTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoArgCreateBufferFromArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoArgCreateBufferFromArray(ByVal backend As Nd4jBackend)

			'Tests here:
			'1. Create from JVM array
			'2. Create from JVM array with offset -> does this even make sense?
			'3. Create detached buffer

			Dim initialConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).build()
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.createNewWorkspace(initialConfig, "WorkspaceId")

			For Each useWs As Boolean In New Boolean(){False, True}

				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = (If(useWs, workspace.notifyScopeEntered(), Nothing))

					'Float
					Dim f As DataBuffer = Nd4j.createBuffer(New Single(){1, 2, 3})
					checkTypes(DataType.FLOAT, f, 3)
					assertEquals(useWs, f.Attached)
					testDBOps(f)

					f = Nd4j.createBuffer(New Single(){1, 2, 3}, 0)
					checkTypes(DataType.FLOAT, f, 3)
					assertEquals(useWs, f.Attached)
					testDBOps(f)

					f = Nd4j.createBufferDetached(New Single(){1, 2, 3})
					checkTypes(DataType.FLOAT, f, 3)
					assertFalse(f.Attached)
					testDBOps(f)

					'Double
					Dim d As DataBuffer = Nd4j.createBuffer(New Double(){1, 2, 3})
					checkTypes(DataType.DOUBLE, d, 3)
					assertEquals(useWs, d.Attached)
					testDBOps(d)

					d = Nd4j.createBuffer(New Double(){1, 2, 3}, 0)
					checkTypes(DataType.DOUBLE, d, 3)
					assertEquals(useWs, d.Attached)
					testDBOps(d)

					d = Nd4j.createBufferDetached(New Double(){1, 2, 3})
					checkTypes(DataType.DOUBLE, d, 3)
					assertFalse(d.Attached)
					testDBOps(d)

					'Int
					Dim i As DataBuffer = Nd4j.createBuffer(New Integer(){1, 2, 3})
					checkTypes(DataType.INT, i, 3)
					assertEquals(useWs, i.Attached)
					testDBOps(i)

					i = Nd4j.createBuffer(New Integer(){1, 2, 3})
					checkTypes(DataType.INT, i, 3)
					assertEquals(useWs, i.Attached)
					testDBOps(i)

					i = Nd4j.createBufferDetached(New Integer(){1, 2, 3})
					checkTypes(DataType.INT, i, 3)
					assertFalse(i.Attached)
					testDBOps(i)

					'Long
					Dim l As DataBuffer = Nd4j.createBuffer(New Long(){1, 2, 3})
					checkTypes(DataType.LONG, l, 3)
					assertEquals(useWs, l.Attached)
					testDBOps(l)

					l = Nd4j.createBuffer(New Long(){1, 2, 3})
					checkTypes(DataType.LONG, l, 3)
					assertEquals(useWs, l.Attached)
					testDBOps(l)

					l = Nd4j.createBufferDetached(New Long(){1, 2, 3})
					checkTypes(DataType.LONG, l, 3)
					assertFalse(l.Attached)
					testDBOps(l)

					'byte
	'                DataBuffer b = Nd4j.createBuffer(new byte[]{1, 2, 3});
	'                checkTypes(DataType.BYTE, b, 3);
	'                testDBOps(b);
	'
	'                b = Nd4j.createBuffer(new byte[]{1, 2, 3}, 0);
	'                checkTypes(DataType.BYTE, b, 3);
	'                testDBOps(b);
	'
	'                b = Nd4j.createBufferDetached(new byte[]{1,2,3});
	'                checkTypes(DataType.BYTE, b, 3);
	'                testDBOps(b);

					'short
					'TODO
				End Using
			Next useWs
		End Sub

		Protected Friend Shared Sub checkTypes(ByVal dataType As DataType, ByVal db As DataBuffer, ByVal expLength As Long)
			assertEquals(dataType, db.dataType())
			assertEquals(expLength, db.length())
			Select Case dataType.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					assertTrue(TypeOf db.pointer() Is DoublePointer)
					assertTrue(TypeOf db.indexer() Is DoubleIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					assertTrue(TypeOf db.pointer() Is FloatPointer)
					assertTrue(TypeOf db.indexer() Is FloatIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					assertTrue(TypeOf db.pointer() Is ShortPointer)
					assertTrue(TypeOf db.indexer() Is HalfIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					assertTrue(TypeOf db.pointer() Is LongPointer)
					assertTrue(TypeOf db.indexer() Is LongIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					assertTrue(TypeOf db.pointer() Is IntPointer)
					assertTrue(TypeOf db.indexer() Is IntIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					assertTrue(TypeOf db.pointer() Is ShortPointer)
					assertTrue(TypeOf db.indexer() Is ShortIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					assertTrue(TypeOf db.pointer() Is BytePointer)
					assertTrue(TypeOf db.indexer() Is UByteIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					assertTrue(TypeOf db.pointer() Is BytePointer)
					assertTrue(TypeOf db.indexer() Is ByteIndexer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					'Bool type uses byte pointers
					assertTrue(TypeOf db.pointer() Is BooleanPointer)
					assertTrue(TypeOf db.indexer() Is BooleanIndexer)
			End Select
		End Sub

		Protected Friend Shared Sub testDBOps(ByVal db As DataBuffer)
			For i As Integer = 0 To 2
				If db.dataType() <> DataType.BOOL Then
					testGet(db, i, i + 1)
				Else
					testGet(db, i, 1)
				End If
			Next i
			testGetRange(db)
			testAsArray(db)

			If db.dataType() <> DataType.BOOL Then
				testAssign(db)
			End If
		End Sub

		Protected Friend Shared Sub testGet(ByVal from As DataBuffer, ByVal idx As Integer, ByVal exp As Number)
			assertEquals(exp.doubleValue(), from.getDouble(idx), 0.0)
			assertEquals(exp.floatValue(), from.getFloat(idx), 0.0f)
			assertEquals(exp.intValue(), from.getInt(idx))
			assertEquals(exp.longValue(), from.getLong(idx), 0.0f)
		End Sub

		Protected Friend Shared Sub testGetRange(ByVal from As DataBuffer)
			If from.dataType() <> DataType.BOOL Then
				assertArrayEquals(New Double(){1, 2, 3}, from.getDoublesAt(0, 3), 0.0)
				assertArrayEquals(New Double(){1, 3}, from.getDoublesAt(0, 2, 2), 0.0)
				assertArrayEquals(New Double(){2, 3}, from.getDoublesAt(1, 1, 2), 0.0)
				assertArrayEquals(New Single(){1, 2, 3}, from.getFloatsAt(0, 3), 0.0f)
				assertArrayEquals(New Single(){1, 3}, from.getFloatsAt(0, 2, 2), 0.0f)
				assertArrayEquals(New Single(){2, 3}, from.getFloatsAt(1, 1, 3), 0.0f)
				assertArrayEquals(New Integer(){1, 2, 3}, from.getIntsAt(0, 3))
				assertArrayEquals(New Integer(){1, 3}, from.getIntsAt(0, 2, 2))
				assertArrayEquals(New Integer(){2, 3}, from.getIntsAt(1, 1, 3))
			Else
				assertArrayEquals(New Double(){1, 1, 1}, from.getDoublesAt(0, 3), 0.0)
				assertArrayEquals(New Double(){1, 1}, from.getDoublesAt(0, 2, 2), 0.0)
				assertArrayEquals(New Double(){1, 1}, from.getDoublesAt(1, 1, 2), 0.0)
				assertArrayEquals(New Single(){1, 1, 1}, from.getFloatsAt(0, 3), 0.0f)
				assertArrayEquals(New Single(){1, 1}, from.getFloatsAt(0, 2, 2), 0.0f)
				assertArrayEquals(New Single(){1, 1}, from.getFloatsAt(1, 1, 3), 0.0f)
				assertArrayEquals(New Integer(){1, 1, 1}, from.getIntsAt(0, 3))
				assertArrayEquals(New Integer(){1, 1}, from.getIntsAt(0, 2, 2))
				assertArrayEquals(New Integer(){1, 1}, from.getIntsAt(1, 1, 3))
			End If
		End Sub

		Protected Friend Shared Sub testAsArray(ByVal db As DataBuffer)
			If db.dataType() <> DataType.BOOL Then
				assertArrayEquals(New Double(){1, 2, 3}, db.asDouble(), 0.0)
				assertArrayEquals(New Single(){1, 2, 3}, db.asFloat(), 0.0f)
				assertArrayEquals(New Integer(){1, 2, 3}, db.asInt())
				assertArrayEquals(New Long(){1, 2, 3}, db.asLong())
			Else
				assertArrayEquals(New Double(){1, 1, 1}, db.asDouble(), 0.0)
				assertArrayEquals(New Single(){1, 1, 1}, db.asFloat(), 0.0f)
				assertArrayEquals(New Integer(){1, 1, 1}, db.asInt())
				assertArrayEquals(New Long(){1, 1, 1}, db.asLong())
			End If
		End Sub

		Protected Friend Shared Sub testAssign(ByVal db As DataBuffer)
			db.assign(5.0)
			testGet(db, 0, 5.0)
			testGet(db, 2, 5.0)

			If db.dataType().isSigned() Then
				db.assign(-3.0f)
				testGet(db, 0, -3.0)
				testGet(db, 2, -3.0)
			End If

			db.assign(New Long(){0, 1, 2}, New Single(){10, 9, 8}, True)
			testGet(db, 0, 10)
			testGet(db, 1, 9)
			testGet(db, 2, 8)

			db.assign(New Long(){0, 2}, New Single(){7, 6}, False)
			testGet(db, 0, 7)
			testGet(db, 1, 9)
			testGet(db, 2, 6)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateTypedBuffer(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateTypedBuffer(ByVal backend As Nd4jBackend)

			Dim initialConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).build()
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.createNewWorkspace(initialConfig, "WorkspaceId")

			For Each sourceType As String In New String(){"int", "long", "float", "double", "short", "byte", "boolean"}
				For Each dt As DataType In DataType.values()
					If dt = DataType.UTF8 OrElse dt = DataType.COMPRESSED OrElse dt = DataType.UNKNOWN Then
						Continue For
					End If

	'                log.info("Testing source [{}]; target: [{}]", sourceType, dt);

					For Each useWs As Boolean In New Boolean(){False, True}

						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = (If(useWs, workspace.notifyScopeEntered(), Nothing))

							Dim db1 As DataBuffer
							Dim db2 As DataBuffer
							Select Case sourceType
								Case "int"
									db1 = Nd4j.createTypedBuffer(New Integer(){1, 2, 3}, dt)
									db2 = Nd4j.createTypedBufferDetached(New Integer(){1, 2, 3}, dt)
								Case "long"
									db1 = Nd4j.createTypedBuffer(New Long(){1, 2, 3}, dt)
									db2 = Nd4j.createTypedBufferDetached(New Long(){1, 2, 3}, dt)
								Case "float"
									db1 = Nd4j.createTypedBuffer(New Single(){1, 2, 3}, dt)
									db2 = Nd4j.createTypedBufferDetached(New Single(){1, 2, 3}, dt)
								Case "double"
									db1 = Nd4j.createTypedBuffer(New Double(){1, 2, 3}, dt)
									db2 = Nd4j.createTypedBufferDetached(New Double(){1, 2, 3}, dt)
								Case "short"
									db1 = Nd4j.createTypedBuffer(New Short(){1, 2, 3}, dt)
									db2 = Nd4j.createTypedBufferDetached(New Short(){1, 2, 3}, dt)
								Case "byte"
									db1 = Nd4j.createTypedBuffer(New SByte(){1, 2, 3}, dt)
									db2 = Nd4j.createTypedBufferDetached(New SByte(){1, 2, 3}, dt)
								Case "boolean"
									db1 = Nd4j.createTypedBuffer(New Boolean(){True, False, True}, dt)
									db2 = Nd4j.createTypedBufferDetached(New Boolean(){True, False, True}, dt)
								Case Else
									Throw New Exception()
							End Select

							checkTypes(dt, db1, 3)
							checkTypes(dt, db2, 3)

							assertEquals(useWs, db1.Attached)
							assertFalse(db2.Attached)

							If Not sourceType.Equals("boolean") Then
								testDBOps(db1)
								testDBOps(db2)
							End If
						End Using
					Next useWs
				Next dt
			Next sourceType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAsBytes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAsBytes(ByVal backend As Nd4jBackend)
			Dim orig As INDArray = Nd4j.linspace(DataType.INT, 0, 10, 1)

			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.BFLOAT16, DataType.LONG, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.BOOL, DataType.UINT64, DataType.UINT32, DataType.UINT16, DataType.UBYTE}
				Dim arr As INDArray = orig.castTo(dt)

				Dim b() As SByte = arr.data().asBytes() 'NOTE: BIG ENDIAN

				If ByteOrder.nativeOrder().Equals(ByteOrder.LITTLE_ENDIAN) Then
					'Switch from big endian (as defined by asBytes which uses big endian) to little endian
					Dim w As Integer = dt.width()
					If w > 1 Then
						Dim len As Integer = b.Length \ w
						For i As Integer = 0 To len - 1
							Dim j As Integer = 0
							Do While j < w \ 2
								Dim temp As SByte = b((i + 1) * w - j - 1)
								b((i + 1) * w - j - 1) = b(i * w + j)
								b(i * w + j) = temp
								j += 1
							Loop
						Next i
					End If
				End If

				Dim arr2 As INDArray = Nd4j.create(dt, arr.shape())
				Dim bb As ByteBuffer = arr2.data().pointer().asByteBuffer()
				Dim buffer As Buffer = CType(bb, Buffer)
				buffer.position(0)
				bb.put(b)

				Nd4j.AffinityManager.tagLocation(arr2, AffinityManager.Location.HOST)

				assertEquals(arr.ToString(), arr2.ToString())
				assertEquals(arr, arr2)

				'Sanity check on data buffer getters:
				Dim db As DataBuffer = arr.data()
				Dim db2 As DataBuffer = arr2.data()
				For i As Integer = 0 To 9
					assertEquals(db.getDouble(i), db2.getDouble(i), 0)
					assertEquals(db.getFloat(i), db2.getFloat(i), 0)
					assertEquals(db.getInt(i), db2.getInt(i), 0)
					assertEquals(db.getLong(i), db2.getLong(i), 0)
					assertEquals(db.getNumber(i), db2.getNumber(i))
				Next i

				assertArrayEquals(db.getDoublesAt(0, 10), db2.getDoublesAt(0, 10), 0)
				assertArrayEquals(db.getFloatsAt(0, 10), db2.getFloatsAt(0, 10), 0)
				assertArrayEquals(db.getIntsAt(0, 10), db2.getIntsAt(0, 10))
				assertArrayEquals(db.getLongsAt(0, 10), db2.getLongsAt(0, 10))
			Next dt
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEnsureLocation()
		Public Overridable Sub testEnsureLocation()
			'https://github.com/eclipse/deeplearning4j/issues/8783
			Nd4j.create(1)

			Dim bp As New BytePointer(5)


			Dim ptr As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().pointerForAddress(bp.address())
			Dim buff As DataBuffer = Nd4j.createBuffer(ptr, 5, DataType.INT8)


			Dim arr2 As INDArray = Nd4j.create(buff, New Long(){5}, New Long(){1}, 0, "c"c, DataType.INT8)
			Dim before As Long = arr2.data().pointer().address()
			Nd4j.AffinityManager.ensureLocation(arr2, AffinityManager.Location.HOST)
			Dim after As Long = arr2.data().pointer().address()

			assertEquals(before, after)
		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

	End Class

End Namespace