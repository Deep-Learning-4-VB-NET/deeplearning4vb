Imports System
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports FlatArray = org.nd4j.graph.FlatArray
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports MirroringPolicy = org.nd4j.linalg.api.memory.enums.MirroringPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports IsInf = org.nd4j.linalg.api.ops.impl.reduce.bool.IsInf
Imports IsNaN = org.nd4j.linalg.api.ops.impl.reduce.bool.IsNaN
Imports CountNonZero = org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero
Imports CosineSimilarity = org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity
Imports EqualTo = org.nd4j.linalg.api.ops.impl.transforms.custom.EqualTo
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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

Namespace org.nd4j.linalg.mixed

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class MixedDataTypesTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class MixedDataTypesTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.LONG, 3, 3)

			assertNotNull(array)
			assertEquals(9, array.length())
			assertEquals(DataType.LONG, array.dataType())
			assertEquals(DataType.LONG, ArrayOptionsHelper.dataType(array.shapeInfoJava()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.SHORT, 3, 3)

			assertNotNull(array)
			assertEquals(9, array.length())
			assertEquals(DataType.SHORT, array.dataType())
			assertEquals(DataType.SHORT, ArrayOptionsHelper.dataType(array.shapeInfoJava()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_3(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create(DataType.HALF, 3, 3)

			assertNotNull(array)
			assertEquals(9, array.length())
			assertEquals(DataType.HALF, array.dataType())
			assertEquals(DataType.HALF, ArrayOptionsHelper.dataType(array.shapeInfoJava()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_4(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(DataType.DOUBLE, 1.0)
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.DOUBLE, scalar.dataType())
			assertEquals(1.0, scalar.getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_5(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(Convert.ToInt32(1))
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.INT, scalar.dataType())
			assertEquals(1.0, scalar.getInt(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_5_0(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_5_0(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(Convert.ToInt64(1))
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.LONG, scalar.dataType())
			assertEquals(1.0, scalar.getInt(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_5_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_5_1(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(Convert.ToDouble(1))
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.DOUBLE, scalar.dataType())
			assertEquals(1.0, scalar.getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_5_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_5_2(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(Convert.ToSingle(1))
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.FLOAT, scalar.dataType())
			assertEquals(1.0, scalar.getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_5_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_5_3(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(Convert.ToInt16(CShort(1)))
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.SHORT, scalar.dataType())
			assertEquals(1.0, scalar.getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_5_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_5_4(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(Convert.ToSByte(CSByte(1)))
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.BYTE, scalar.dataType())
			assertEquals(1.0, scalar.getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_6(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(1)
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.INT, scalar.dataType())
			assertEquals(1.0, scalar.getInt(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicCreation_7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicCreation_7(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(1L)
			assertNotNull(scalar)
			assertEquals(0, scalar.rank())
			assertEquals(1, scalar.length())
			assertEquals(DataType.LONG, scalar.dataType())
			assertEquals(1, scalar.getInt(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_1(ByVal backend As Nd4jBackend)
			Dim exp As val = New Integer(){1, 1, 1, 1, 1, 1, 1, 1, 1}
			Dim array As val = Nd4j.create(DataType.INT, 3, 3)
			assertEquals(DataType.INT, array.dataType())
			array.assign(1)

			Dim vector As val = array.data().asInt()
			assertArrayEquals(exp, vector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_2(ByVal backend As Nd4jBackend)
			Dim exp As val = New Integer(){1, 1, 1, 1, 1, 1, 1, 1, 1}
			Dim arrayX As val = Nd4j.create(DataType.INT, 3, 3)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 1, 1, 1, 1, 1, 1, 1, 1}, New Long(){3, 3}, DataType.INT)

			arrayX.addi(arrayY)

			Dim vector As val = arrayX.data().asInt()
			assertArrayEquals(exp, vector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_3(ByVal backend As Nd4jBackend)
			If Not NativeOpsHolder.Instance.getDeviceNativeOps().isExperimentalEnabled() Then
				Return
			End If

			Dim exp As val = New Integer(){1, 1, 1, 1, 1, 1, 1, 1, 1}
			Dim arrayX As val = Nd4j.create(DataType.INT, 3, 3)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 1, 1, 1, 1, 1, 1, 1, 1}, New Long(){3, 3}, DataType.LONG)

			Dim vectorY As val = arrayY.data().asInt()
			assertArrayEquals(exp, vectorY)

			arrayX.addi(arrayY)

			Dim vectorX As val = arrayX.data().asInt()
			assertArrayEquals(exp, vectorX)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_4(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){7, 8, 7, 9, 1, 1, 1, 1, 1}, New Long(){3, 3}, DataType.LONG)

			Dim result As val = arrayX.maxNumber()
			Dim l As val = result.longValue()

			assertEquals(9L, l)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_5(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)

			Dim result As val = arrayX.meanNumber().floatValue()

			assertEquals(2.5f, result, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_6(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){1, 0, 0, 4}, New Long(){4}, DataType.INT)

			Dim z As val = Nd4j.Executioner.exec(New CountNonZero(arrayX))

			assertEquals(DataType.LONG, z.dataType())
			Dim result As val = z.getInt(0)

			assertEquals(2, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_7(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Single(){1, 0, Float.NaN, 4}, New Long(){4}, DataType.FLOAT)

			Dim z As val = Nd4j.Executioner.exec(New IsInf(arrayX))

			assertEquals(DataType.BOOL, z.dataType())
			Dim result As val = z.getInt(0)

			Dim z2 As val = Nd4j.Executioner.exec(New IsNaN(arrayX))
			assertEquals(DataType.BOOL, z2.dataType())
			Dim result2 As val = z2.getInt(0)

			assertEquals(1, result2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_8(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_8(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 0, 0, 4}, New Long(){4}, DataType.INT)
			Dim exp As val = New Long(){1, 0, 0, 1}

			Dim result As val = Nd4j.Executioner.exec(New EqualTo(arrayX, arrayY, arrayX.ulike().castTo(DataType.BOOL)))(0)
			assertEquals(DataType.BOOL, result.dataType())
			Dim arr As val = result.data().asLong()

			assertArrayEquals(exp, arr)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicOps_9(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicOps_9(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim exp As val = New Long(){1, 0, 0, 1}

			Dim op As val = New CosineSimilarity(arrayX, arrayY)
			Dim result As val = Nd4j.Executioner.exec(op)
			Dim arr As val = result.getDouble(0)

			assertEquals(1.0, arr, 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewAssign_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewAssign_1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(DataType.FLOAT, 5)
			Dim arrayY As val = Nd4j.create(New Double(){1, 2, 3, 4, 5})
			Dim exp As val = Nd4j.create(New Single(){1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			arrayX.assign(arrayY)

			assertEquals(exp, arrayX)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNewAssign_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNewAssign_2(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(DataType.INT, 5)
			Dim arrayY As val = Nd4j.create(New Double(){1, 2, 3, 4, 5})
			Dim exp As val = Nd4j.create(New Integer(){1, 2, 3, 4, 5}, New Long(){5}, DataType.INT)

			arrayX.assign(arrayY)

			assertEquals(exp, arrayX)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMethods_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMethods_1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim exp As val = Nd4j.create(New Integer(){2, 4, 6, 8}, New Long(){4}, DataType.INT)

			Dim arrayZ As val = arrayX.add(arrayY)
			assertEquals(DataType.INT, arrayZ.dataType())
			assertEquals(exp, arrayZ)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMethods_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMethods_2(ByVal backend As Nd4jBackend)
			If Not NativeOpsHolder.Instance.getDeviceNativeOps().isExperimentalEnabled() Then
				Return
			End If

			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim arrayY As val = Nd4j.create(New Double(){1, 2, 3, 4}, New Long(){4}, DataType.DOUBLE)
			Dim exp As val = Nd4j.create(New Double(){2, 4, 6, 8}, New Long(){4}, DataType.DOUBLE)

			Dim arrayZ As val = arrayX.add(arrayY)

			assertEquals(DataType.DOUBLE, arrayZ.dataType())
			assertEquals(exp, arrayZ)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMethods_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMethods_3(ByVal backend As Nd4jBackend)
			If Not NativeOpsHolder.Instance.getDeviceNativeOps().isExperimentalEnabled() Then
				Return
			End If

			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim arrayY As val = Nd4j.create(New Double(){0.5, 0.5, 0.5, 0.5}, New Long(){4}, DataType.DOUBLE)
			Dim exp As val = Nd4j.create(New Double(){1.5, 2.5, 3.5, 4.5}, New Long(){4}, DataType.DOUBLE)

			Dim arrayZ As val = arrayX.add(arrayY)

			assertEquals(DataType.DOUBLE, arrayZ.dataType())
			assertEquals(exp, arrayZ)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTypesValidation_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTypesValidation_1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.LONG)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim exp As val = New Long(){1, 0, 0, 1}
			Dim op As val = New CosineSimilarity(arrayX, arrayY)
			Dim result As val = Nd4j.Executioner.exec(op)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTypesValidation_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTypesValidation_2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception),Sub()
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 0, 0, 4}, New Long(){4}, DataType.LONG)
			Dim exp As val = New Long(){1, 0, 0, 1}
			Dim result As val = Nd4j.Executioner.exec(New EqualTo(arrayX, arrayY, arrayX.ulike().castTo(DataType.BOOL)))(0)
			Dim arr As val = result.data().asLong()
			assertArrayEquals(exp, arr)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTypesValidation_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTypesValidation_3(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception),Sub()
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim result As val = Nd4j.Executioner.exec(DirectCast(New SoftMax(arrayX, arrayX, -1), CustomOp))
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTypesValidation_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTypesValidation_4(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)
			Dim arrayY As val = Nd4j.create(New Integer(){1, 0, 0, 4}, New Long(){4}, DataType.DOUBLE)
			Dim arrayE As val = Nd4j.create(New Integer(){2, 2, 3, 8}, New Long(){4}, DataType.INT)

			arrayX.addi(arrayY)
			assertEquals(arrayE, arrayX)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlatSerde_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlatSerde_1(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Integer(){1, 2, 3, 4}, New Long(){4}, DataType.INT)

			Dim builder As val = New FlatBufferBuilder(512)
			Dim flat As val = arrayX.toFlatArray(builder)
			builder.finish(flat)
			Dim db As val = builder.dataBuffer()

			Dim flatb As val = FlatArray.getRootAsFlatArray(db)

			Dim restored As val = Nd4j.createFromFlatArray(flatb)

			assertEquals(arrayX, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlatSerde_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlatSerde_2(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Long(){1, 2, 3, 4}, New Long(){4}, DataType.LONG)

			Dim builder As val = New FlatBufferBuilder(512)
			Dim flat As val = arrayX.toFlatArray(builder)
			builder.finish(flat)
			Dim db As val = builder.dataBuffer()

			Dim flatb As val = FlatArray.getRootAsFlatArray(db)

			Dim restored As val = Nd4j.createFromFlatArray(flatb)

			assertEquals(arrayX, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlatSerde_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlatSerde_3(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(New Boolean(){True, False, True, True}, New Long(){4}, DataType.BOOL)

			Dim builder As val = New FlatBufferBuilder(512)
			Dim flat As val = arrayX.toFlatArray(builder)
			builder.finish(flat)
			Dim db As val = builder.dataBuffer()

			Dim flatb As val = FlatArray.getRootAsFlatArray(db)

			Dim restored As val = Nd4j.createFromFlatArray(flatb)

			assertEquals(arrayX, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBoolFloatCast2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBoolFloatCast2(ByVal backend As Nd4jBackend)
			Dim first As val = Nd4j.zeros(DataType.FLOAT, 3, 5000)
			Dim asBool As INDArray = first.castTo(DataType.BOOL)
			Dim [not] As INDArray = Transforms.not(asBool)
			Dim asFloat As INDArray = [not].castTo(DataType.FLOAT)

	'        System.out.println(not);
	'        System.out.println(asFloat);
			Dim exp As INDArray = Nd4j.ones(DataType.FLOAT, 3, 5000)
			assertEquals(DataType.FLOAT, exp.dataType())
			assertEquals(exp.dataType(), asFloat.dataType())

			Dim arrX As val = asFloat.data().asFloat()
			Dim arrE As val = exp.data().asFloat()
			assertArrayEquals(arrE, arrX, 1e-5f)

			assertEquals(exp, asFloat)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReduce3Large(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReduce3Large(ByVal backend As Nd4jBackend)
			Dim arrayX As val = Nd4j.create(DataType.FLOAT, 10, 5000)
			Dim arrayY As val = Nd4j.create(DataType.FLOAT, 10, 5000)

			assertTrue(arrayX.equalsWithEps(arrayY, -1e-5f))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssignScalarSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssignScalarSimple(ByVal backend As Nd4jBackend)
			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Dim arr As INDArray = Nd4j.scalar(dt, 10.0)
				arr.assign(2.0)
	'            System.out.println(dt + " - value: " + arr + " - " + arr.getDouble(0));
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimple(ByVal backend As Nd4jBackend)
			Nd4j.create(1)
			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT, DataType.LONG}
	'            System.out.println("----- " + dt + " -----");
				Dim arr As INDArray = Nd4j.ones(dt,1, 5)
	'            System.out.println("Ones: " + arr);
				arr.assign(1.0)
	'            System.out.println("assign(1.0): " + arr);
	'            System.out.println("DIV: " + arr.div(8));
	'            System.out.println("MUL: " + arr.mul(8));
	'            System.out.println("SUB: " + arr.sub(8));
	'            System.out.println("ADD: " + arr.add(8));
	'            System.out.println("RDIV: " + arr.rdiv(8));
	'            System.out.println("RSUB: " + arr.rsub(8));
				arr.div(8)
				arr.mul(8)
				arr.sub(8)
				arr.add(8)
				arr.rdiv(8)
				arr.rsub(8)
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspaceBool(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWorkspaceBool(ByVal backend As Nd4jBackend)
			Dim conf As val = WorkspaceConfiguration.builder().minSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Dim ws As val = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(conf, "WS")

			For i As Integer = 0 To 9
				Using workspace As lombok.val = CType(ws.notifyScopeEntered(), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
					Dim bool As val = Nd4j.create(DataType.BOOL, 1, 10)
					Dim dbl As val = Nd4j.create(DataType.DOUBLE, 1, 10)

					Dim boolAttached As val = bool.isAttached()
					Dim doubleAttached As val = dbl.isAttached()

	'                System.out.println(i + "\tboolAttached=" + boolAttached + ", doubleAttached=" + doubleAttached );
					'System.out.println("bool: " + bool);        //java.lang.IllegalStateException: Indexer must never be null
					'System.out.println("double: " + dbl);
				End Using
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testArrayCreationFromPointer(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayCreationFromPointer(ByVal backend As Nd4jBackend)
			Dim source As val = Nd4j.create(New Double(){1, 2, 3, 4, 5})

			Dim pAddress As val = source.data().addressPointer()
			Dim shape As val = source.shape()
			Dim stride As val = source.stride()
			Dim order As val = source.ordering()

			Dim buffer As val = Nd4j.createBuffer(pAddress, source.length(), source.dataType())
			Dim restored As val = Nd4j.create(buffer, shape, stride, 0, order, source.dataType())
			assertEquals(source, restored)

			assertArrayEquals(source.toDoubleVector(), restored.toDoubleVector(), 1e-5)

			assertEquals(source.getDouble(0), restored.getDouble(0), 1e-5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBfloat16_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBfloat16_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.BFLOAT16, 5)
			Dim y As val = Nd4j.createFromArray(New Integer(){2, 2, 2, 2, 2}).castTo(DataType.BFLOAT16)

			x.addi(y)
			assertEquals(x, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUint16_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUint16_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.UINT16, 5)
			Dim y As val = Nd4j.createFromArray(New Integer(){2, 2, 2, 2, 2}).castTo(DataType.UINT16)

			x.addi(y)
			assertEquals(x, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUint32_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUint32_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.UINT32, 5)
			Dim y As val = Nd4j.createFromArray(New Integer(){2, 2, 2, 2, 2}).castTo(DataType.UINT32)

			x.addi(y)
			assertEquals(x, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUint64_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUint64_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.UINT64, 5)
			Dim y As val = Nd4j.createFromArray(New Integer(){2, 2, 2, 2, 2}).castTo(DataType.UINT64)

			x.addi(y)
			assertEquals(x, y)
		End Sub
	End Class

End Namespace