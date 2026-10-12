Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.blas



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class BlasTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class BlasTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void simpleTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub simpleTest(ByVal backend As Nd4jBackend)
			Dim m1 As INDArray = Nd4j.create(New Double()(){
				New Double() {1.0},
				New Double() {2.0},
				New Double() {3.0},
				New Double() {4.0}
			})

			m1 = m1.reshape(ChrW(2), 2)

			Dim m2 As INDArray = Nd4j.create(New Double()(){
				New Double() {1.0, 2.0, 3.0, 4.0}
			})
			m2 = m2.reshape(ChrW(2), 2)
			m2.Order = "f"c

			'mmul gives the correct result
			Dim correctResult As INDArray
			correctResult = m1.mmul(m2)
			Console.WriteLine("================")
			Console.WriteLine(m1)
			Console.WriteLine(m2)
			Console.WriteLine(correctResult)
			Console.WriteLine("================")
			Dim newResult As INDArray = Nd4j.create(DataType.DOUBLE, correctResult.shape(), "c"c)
			m1.mmul(m2, newResult)
			assertEquals(correctResult, newResult)

			'But not so mmuli (which is somewhat mixed)
			Dim target As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			target = m1.mmuli(m2, m1)
			assertEquals(True, target.Equals(correctResult))
			assertEquals(True, m1.Equals(correctResult))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemmInvalid1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemmInvalid1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(3, 4);
			Dim a As INDArray = Nd4j.rand(3, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 5);
			Dim b As INDArray = Nd4j.rand(4, 5)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray target = org.nd4j.linalg.factory.Nd4j.zeros(new int[]{2, 3, 5}, "f"c);
			Dim target As INDArray = Nd4j.zeros(New Integer(){2, 3, 5}, "f"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray view = target.tensorAlongDimension(0, 1, 2);
			Dim view As INDArray = target.tensorAlongDimension(0, 1, 2)

			Try
				Nd4j.gemm(a, b, view, False, False, 1.0, 0.0)
				fail("Expected exception")
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("view"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemmInvalid3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemmInvalid3(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(4, 3);
			Dim a As INDArray = Nd4j.rand(4, 3)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 5);
			Dim b As INDArray = Nd4j.rand(4, 5)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray target = org.nd4j.linalg.factory.Nd4j.zeros(new int[]{2, 3, 5}, "f"c);
			Dim target As INDArray = Nd4j.zeros(New Integer(){2, 3, 5}, "f"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray view = target.tensorAlongDimension(0, 1, 2);
			Dim view As INDArray = target.tensorAlongDimension(0, 1, 2)

			Try
				Nd4j.gemm(a, b, view, True, False, 1.0, 0.0)
				fail("Expected exception")
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("view"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(4, 3);
			Dim a As INDArray = Nd4j.rand(4, 3)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 5);
			Dim b As INDArray = Nd4j.rand(4, 5)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray result = a.transpose().mmul(b);
			Dim result As INDArray = a.transpose().mmul(b)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray result2 = org.nd4j.linalg.factory.Nd4j.gemm(a, b, true, false);
			Dim result2 As INDArray = Nd4j.gemm(a, b, True, False)

			assertEquals(result, result2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm2(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(4, 3);
			Dim a As INDArray = Nd4j.rand(4, 3)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 5);
			Dim b As INDArray = Nd4j.rand(4, 5)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray target = org.nd4j.linalg.factory.Nd4j.zeros(new int[]{2, 3, 5}, "f"c);
			Dim target As INDArray = Nd4j.zeros(New Integer(){2, 3, 5}, "f"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray view = target.tensorAlongDimension(0, 1, 2);
			Dim view As INDArray = target.tensorAlongDimension(0, 1, 2)

			a.transpose().mmuli(b, view)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray result = a.transpose().mmul(b);
			Dim result As INDArray = a.transpose().mmul(b)

			assertEquals(result, view)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm3(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(4, 3);
			Dim a As INDArray = Nd4j.rand(4, 3)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 5);
			Dim b As INDArray = Nd4j.rand(4, 5)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray target = org.nd4j.linalg.factory.Nd4j.zeros(new int[]{2, 3, 5}, "c"c);
			Dim target As INDArray = Nd4j.zeros(New Integer(){2, 3, 5}, "c"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray view = target.tensorAlongDimension(0, 1, 2);
			Dim view As INDArray = target.tensorAlongDimension(0, 1, 2)

			a.transpose().mmuli(b, view)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray result = a.transpose().mmul(b);
			Dim result As INDArray = a.transpose().mmul(b)

			assertEquals(result, view)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmuli1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmuli1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations = org.nd4j.linalg.factory.Nd4j.createUninitialized(new long[]{1, 3, 1}, "f"c);
			Dim activations As INDArray = Nd4j.createUninitialized(New Long(){1, 3, 1}, "f"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray z = activations.tensorAlongDimension(0, 1, 2);
			Dim z As INDArray = activations.tensorAlongDimension(0, 1, 2)

			Nd4j.Random.setSeed(12345)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(3, 4);
			Dim a As INDArray = Nd4j.rand(3, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 1);
			Dim b As INDArray = Nd4j.rand(4, 1)

			Dim ab As INDArray = a.mmul(b)
			a.mmul(b, z)
			assertEquals(ab, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmuli2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmuli2(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations = org.nd4j.linalg.factory.Nd4j.createUninitialized(new long[]{2, 3, 1}, "f"c);
			Dim activations As INDArray = Nd4j.createUninitialized(New Long(){2, 3, 1}, "f"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray z = activations.tensorAlongDimension(0, 1, 2);
			Dim z As INDArray = activations.tensorAlongDimension(0, 1, 2)

			Nd4j.Random.setSeed(12345)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(3, 4);
			Dim a As INDArray = Nd4j.rand(3, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 1);
			Dim b As INDArray = Nd4j.rand(4, 1)

			Dim ab As INDArray = a.mmul(b)
			a.mmul(b, z)
			assertEquals(ab, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmuli3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmuli3(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations = org.nd4j.linalg.factory.Nd4j.createUninitialized(new long[]{1, 3, 2}, "f"c);
			Dim activations As INDArray = Nd4j.createUninitialized(New Long(){1, 3, 2}, "f"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray z = activations.tensorAlongDimension(0, 1, 2);
			Dim z As INDArray = activations.tensorAlongDimension(0, 1, 2)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(3, 4);
			Dim a As INDArray = Nd4j.rand(3, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(4, 2);
			Dim b As INDArray = Nd4j.rand(4, 2)

			Dim ab As INDArray = a.mmul(b)
			a.mmul(b, z)
			assertEquals(ab, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test_Fp16_Mmuli_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test_Fp16_Mmuli_1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray activations = org.nd4j.linalg.factory.Nd4j.createUninitialized(org.nd4j.linalg.api.buffer.DataType.HALF, new long[]{1, 3, 2}, "f"c);
			Dim activations As INDArray = Nd4j.createUninitialized(DataType.HALF, New Long(){1, 3, 2}, "f"c)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray z = activations.tensorAlongDimension(0, 1, 2);
			Dim z As INDArray = activations.tensorAlongDimension(0, 1, 2)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray a = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.HALF, 3, 4);
			Dim a As INDArray = Nd4j.rand(DataType.HALF, 3, 4)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray b = org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType.HALF,4, 2);
			Dim b As INDArray = Nd4j.rand(DataType.HALF,4, 2)

			Dim ab As INDArray = a.mmul(b)
			a.mmul(b, z)
			assertEquals(ab, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test_Fp16_Mmuli_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test_Fp16_Mmuli_2(ByVal backend As Nd4jBackend)
			Dim a As val = Nd4j.create(DataType.HALF, 32, 768)
			Dim b As val = Nd4j.create(DataType.HALF, 768)

			Dim c As val = a.mmul(b)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHalfPrecision(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHalfPrecision(ByVal backend As Nd4jBackend)
			Dim a As val = Nd4j.create(DataType.HALF, 64, 768)
			Dim b As val = Nd4j.create(DataType.HALF, 768, 1024)
			Dim c As val = Nd4j.create(DataType.HALF, New Long(){64, 1024}, "f"c)

			Dim durations As val = New List(Of Long)()
			Dim iterations As val = 100
			For e As Integer = 0 To iterations - 1
				Dim timeStart As val = DateTimeHelper.CurrentUnixTimeMillis()
				a.mmuli(b, c)
				Dim timeEnd As val = DateTimeHelper.CurrentUnixTimeMillis()
				durations.add(timeEnd - timeStart)
			Next e

			Collections.sort(durations)

			log.info("Median time: {} ms", durations.get(durations.size() \ 2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmuli4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmuli4(ByVal backend As Nd4jBackend)
			Try
				Nd4j.rand(1, 3).mmuli(Nd4j.rand(3, 1), Nd4j.createUninitialized(New Integer(){10, 10, 1}))
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("shape"))
			End Try
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace