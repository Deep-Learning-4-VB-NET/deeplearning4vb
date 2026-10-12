Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.nd4j.linalg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class AveragingTests extends BaseNd4jTestWithBackends
	Public Class AveragingTests
		Inherits BaseNd4jTestWithBackends

		Private ReadOnly THREADS As Integer = 16
		Private ReadOnly LENGTH As Integer = 51200 * 4




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void shutUp()
		Public Overridable Sub shutUp()
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSingleDeviceAveraging1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSingleDeviceAveraging1(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.valueArrayOf(LENGTH, 1.0)
			Dim array2 As INDArray = Nd4j.valueArrayOf(LENGTH, 2.0)
			Dim array3 As INDArray = Nd4j.valueArrayOf(LENGTH, 3.0)
			Dim array4 As INDArray = Nd4j.valueArrayOf(LENGTH, 4.0)
			Dim array5 As INDArray = Nd4j.valueArrayOf(LENGTH, 5.0)
			Dim array6 As INDArray = Nd4j.valueArrayOf(LENGTH, 6.0)
			Dim array7 As INDArray = Nd4j.valueArrayOf(LENGTH, 7.0)
			Dim array8 As INDArray = Nd4j.valueArrayOf(LENGTH, 8.0)
			Dim array9 As INDArray = Nd4j.valueArrayOf(LENGTH, 9.0)
			Dim array10 As INDArray = Nd4j.valueArrayOf(LENGTH, 10.0)
			Dim array11 As INDArray = Nd4j.valueArrayOf(LENGTH, 11.0)
			Dim array12 As INDArray = Nd4j.valueArrayOf(LENGTH, 12.0)
			Dim array13 As INDArray = Nd4j.valueArrayOf(LENGTH, 13.0)
			Dim array14 As INDArray = Nd4j.valueArrayOf(LENGTH, 14.0)
			Dim array15 As INDArray = Nd4j.valueArrayOf(LENGTH, 15.0)
			Dim array16 As INDArray = Nd4j.valueArrayOf(LENGTH, 16.0)


			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim arrayMean As INDArray = Nd4j.averageAndPropagate(New INDArray() {array1, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11, array12, array13, array14, array15, array16})
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Console.WriteLine("Execution time: " & (time2 - time1))

			assertNotEquals(Nothing, arrayMean)

			assertEquals(8.5f, arrayMean.getFloat(12), 0.1f)
			assertEquals(8.5f, arrayMean.getFloat(150), 0.1f)
			assertEquals(8.5f, arrayMean.getFloat(475), 0.1f)


			assertEquals(8.5f, array1.getFloat(475), 0.1f)
			assertEquals(8.5f, array2.getFloat(475), 0.1f)
			assertEquals(8.5f, array3.getFloat(475), 0.1f)
			assertEquals(8.5f, array5.getFloat(475), 0.1f)
			assertEquals(8.5f, array16.getFloat(475), 0.1f)


			assertEquals(8.5, arrayMean.meanNumber().doubleValue(), 0.01)
			assertEquals(8.5, array1.meanNumber().doubleValue(), 0.01)
			assertEquals(8.5, array2.meanNumber().doubleValue(), 0.01)

			assertEquals(arrayMean, array16)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) public void testSingleDeviceAveraging2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSingleDeviceAveraging2(ByVal backend As Nd4jBackend)
			Dim exp As INDArray = Nd4j.linspace(1, LENGTH, LENGTH)
			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To THREADS - 1
				arrays.Add(exp.dup())
			Next i

			Dim mean As INDArray = Nd4j.averageAndPropagate(arrays)

			assertEquals(exp, mean)

			For i As Integer = 0 To THREADS - 1
				assertEquals(exp, arrays(i))
			Next i
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAccumulation1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAccumulation1(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.create(100).assign(1.0)
			Dim array2 As INDArray = Nd4j.create(100).assign(2.0)
			Dim array3 As INDArray = Nd4j.create(100).assign(3.0)
			Dim exp As INDArray = Nd4j.create(100).assign(6.0)

			Dim accum As INDArray = Nd4j.accumulate(array1, array2, array3)

			assertEquals(exp, accum)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAccumulation2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAccumulation2(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.create(100).assign(1.0)
			Dim array2 As INDArray = Nd4j.create(100).assign(2.0)
			Dim array3 As INDArray = Nd4j.create(100).assign(3.0)
			Dim target As INDArray = Nd4j.create(100)
			Dim exp As INDArray = Nd4j.create(100).assign(6.0)

			Dim accum As INDArray = Nd4j.accumulate(target, New INDArray() {array1, array2, array3})

			assertEquals(exp, accum)
			assertTrue(accum Is target)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAccumulation3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAccumulation3(ByVal backend As Nd4jBackend)
			' we want to ensure that cuda backend is able to launch this op on cpu
			Nd4j.AffinityManager.allowCrossDeviceAccess(False)

			Dim array1 As INDArray = Nd4j.create(100).assign(1.0)
			Dim array2 As INDArray = Nd4j.create(100).assign(2.0)
			Dim array3 As INDArray = Nd4j.create(100).assign(3.0)
			Dim target As INDArray = Nd4j.create(100)
			Dim exp As INDArray = Nd4j.create(100).assign(6.0)

			Dim accum As INDArray = Nd4j.accumulate(target, New INDArray() {array1, array2, array3})

			assertEquals(exp, accum)
			assertTrue(accum Is target)

			Nd4j.AffinityManager.allowCrossDeviceAccess(True)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace