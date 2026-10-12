Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.junit.jupiter.api
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports Isolated = org.junit.jupiter.api.parallel.Isolated
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ManhattanDistance = org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.nd4j.linalg.specials


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Isolated @Execution(ExecutionMode.SAME_THREAD) @Tag(TagNames.LARGE_RESOURCES) @Disabled("Too long of a timeout to be used in CI") public class LongTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LongTests
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void beforeEach()
		Public Overridable Sub beforeEach()
			System.GC.Collect()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void afterEach()
		Public Overridable Sub afterEach()
			System.GC.Collect()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testSomething1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSomething1(ByVal backend As Nd4jBackend)
			' we create 2D array, total nr. of elements is 2.4B elements, > MAX_INT
			Dim huge As INDArray = Nd4j.create(DataType.INT8,8000000, 300)

			' we apply element-wise scalar ops, just to make sure stuff still works
			huge.subi(1).divi(2)


			' now we're checking different rows, they should NOT equal
			Dim row0 As INDArray = huge.getRow(100001).assign(1.0)
			Dim row1 As INDArray = huge.getRow(100002).assign(2.0)
			assertNotEquals(row0, row1)


			' same idea, but this code is broken: rowA and rowB will be pointing to the same offset
			Dim rowA As INDArray = huge.getRow(huge.rows() - 3)
			Dim rowB As INDArray = huge.getRow(huge.rows() - 10)

			' safety check, to see if we're really working on the same offset.
			rowA.addi(1.0)

			' and this fails, so rowA and rowB are pointing to the same offset, despite different getRow() arguments were used
			assertNotEquals(rowA, rowB)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testSomething2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSomething2(ByVal backend As Nd4jBackend)
			' we create 2D array, total nr. of elements is 2.4B elements, > MAX_INT
			Dim huge As INDArray = Nd4j.create(DataType.INT8,100, 10)

			' we apply element-wise scalar ops, just to make sure stuff still works
			huge.subi(1).divi(2)


			' now we're checking different rows, they should NOT equal
			Dim row0 As INDArray = huge.getRow(73).assign(1.0)
			Dim row1 As INDArray = huge.getRow(74).assign(2.0)
			assertNotEquals(row0, row1)


			' same idea, but this code is broken: rowA and rowB will be pointing to the same offset
			Dim rowA As INDArray = huge.getRow(huge.rows() - 3)
			Dim rowB As INDArray = huge.getRow(huge.rows() - 10)

			' safety check, to see if we're really working on the same offset.
			rowA.addi(1.0)

			' and this fails, so rowA and rowB are pointing to the same offset, despite different getRow() arguments were used
			assertNotEquals(rowA, rowB)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testLongTadOffsets1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongTadOffsets1(ByVal backend As Nd4jBackend)
			Dim huge As INDArray = Nd4j.create(DataType.INT8,230000000, 10)

			Dim tad As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(huge, 1)

			assertEquals(230000000, tad.Second.length())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testLongTadOp1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongTadOp1(ByVal backend As Nd4jBackend)

			Dim exp As Double = Transforms.manhattanDistance(Nd4j.create(DataType.INT16,1000).assign(1.0), Nd4j.create(DataType.INT16,1000).assign(2.0))

			Dim hugeX As INDArray = Nd4j.create(DataType.INT16,2200000, 1000).assign(1.0)
			Dim hugeY As INDArray = Nd4j.create(DataType.INT16,1, 1000).assign(2.0)

			Dim x As Integer = 0
			Do While x < hugeX.rows()
				assertEquals(1000, hugeX.getRow(x).sumNumber().intValue(),"Failed at row " & x)
				x += 1
			Loop

			Dim result As INDArray = Nd4j.Executioner.exec(New ManhattanDistance(hugeX, hugeY, 1))
			x = 0
			Do While x < hugeX.rows()
				assertEquals(exp, result.getDouble(x), 1e-5)
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testLongTadOp2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongTadOp2(ByVal backend As Nd4jBackend)
			Dim hugeX As INDArray = Nd4j.create(DataType.INT16,2300000, 1000).assign(1.0)
			hugeX.addiRowVector(Nd4j.create(DataType.INT16,1000).assign(2.0))

			Dim x As Integer = 0
			Do While x < hugeX.rows()
				assertEquals(hugeX.getRow(x).sumNumber().intValue(),3000,"Failed at row " & x)
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testLongTadOp2_micro(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongTadOp2_micro(ByVal backend As Nd4jBackend)

			Dim hugeX As INDArray = Nd4j.create(DataType.INT16,230, 1000).assign(1.0)
			hugeX.addiRowVector(Nd4j.create(DataType.INT16,1000).assign(2.0))

			Dim x As Integer = 0
			Do While x < hugeX.rows()
				assertEquals(3000, hugeX.getRow(x).sumNumber().intValue(),"Failed at row " & x)
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testLongTadOp3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongTadOp3(ByVal backend As Nd4jBackend)

			Dim hugeX As INDArray = Nd4j.create(DataType.INT16,2300000, 1000).assign(1.0)
			Dim mean As INDArray = hugeX.mean(1)

			Dim x As Integer = 0
			Do While x < hugeX.rows()
				assertEquals(1.0, mean.getDouble(x), 1e-5,"Failed at row " & x)
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testLongTadOp4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongTadOp4(ByVal backend As Nd4jBackend)

			Dim hugeX As INDArray = Nd4j.create(DataType.INT8,2300000, 1000).assign(1.0)
			Dim mean As INDArray = hugeX.argMax(1)

			Dim x As Integer = 0
			Do While x < hugeX.rows()
				assertEquals(0.0, mean.getDouble(x), 1e-5,"Failed at row " & x)
				x += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) public void testLongTadOp5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLongTadOp5(ByVal backend As Nd4jBackend)

			Dim list As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 2299999
				list.Add(Nd4j.create(DataType.INT8,1000).assign(2.0))
			Next i

			Dim hugeX As INDArray = Nd4j.vstack(list)

			Dim x As Integer = 0
			Do While x < hugeX.rows()
				assertEquals(2.0, hugeX.getRow(x).meanNumber().doubleValue(), 1e-5,"Failed at row " & x)
				x += 1
			Loop
		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace