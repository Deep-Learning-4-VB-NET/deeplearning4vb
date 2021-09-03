Imports System
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
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

Namespace org.nd4j.linalg.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class TestArrayUtils extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestArrayUtils
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlattenDoubleArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlattenDoubleArray(ByVal backend As Nd4jBackend)
			assertArrayEquals(New Double(){}, ArrayUtil.flattenDoubleArray(New Double(){}), 0.0)
			Dim r As New Random(12345L)

			Dim d1(9) As Double
			For i As Integer = 0 To d1.Length - 1
				d1(i) = r.NextDouble()
			Next i
			assertArrayEquals(d1, ArrayUtil.flattenDoubleArray(d1), 0.0)

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim d2[][] As Double = new Double[5][10]
			Dim d2()() As Double = RectangularArrays.RectangularDoubleArray(5, 10)
			For i As Integer = 0 To 4
				For j As Integer = 0 To 9
					d2(i)(j) = r.NextDouble()
				Next j
			Next i
			assertArrayEquals(ArrayUtil.flatten(d2), ArrayUtil.flattenDoubleArray(d2), 0.0)

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim d3[][][] As Double = new Double[5][10][15]
			Dim d3()()() As Double = RectangularArrays.RectangularDoubleArray(5, 10, 15)
			Dim exp3((5 * 10 * 15) - 1) As Double
			Dim c As Integer = 0
			For i As Integer = 0 To 4
				For j As Integer = 0 To 9
					For k As Integer = 0 To 14
						Dim d As Double = r.NextDouble()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: exp3[c++] = d;
						exp3(c) = d
							c += 1
						d3(i)(j)(k) = d
					Next k
				Next j
			Next i
			assertArrayEquals(exp3, ArrayUtil.flattenDoubleArray(d3), 0.0)


'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim d4[][][][] As Double = new Double[3][5][7][9]
			Dim d4()()()() As Double = RectangularArrays.RectangularDoubleArray(3, 5, 7, 9)
			Dim exp4((3 * 5 * 7 * 9) - 1) As Double
			c = 0
			For i As Integer = 0 To 2
				For j As Integer = 0 To 4
					For k As Integer = 0 To 6
						For l As Integer = 0 To 8
							Dim d As Double = r.NextDouble()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: exp4[c++] = d;
							exp4(c) = d
								c += 1
							d4(i)(j)(k)(l) = d
						Next l
					Next k
				Next j
			Next i
			assertArrayEquals(exp4, ArrayUtil.flattenDoubleArray(d4), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlattenFloatArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlattenFloatArray(ByVal backend As Nd4jBackend)
			assertArrayEquals(New Single(){}, ArrayUtil.flattenFloatArray(New Single(){}), 0.0f)
			Dim r As New Random(12345L)

			Dim f1(9) As Single
			For i As Integer = 0 To f1.Length - 1
				f1(i) = r.nextFloat()
			Next i
			assertArrayEquals(f1, ArrayUtil.flattenFloatArray(f1), 0.0f)

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim f2[][] As Single = new Single[5][10]
			Dim f2()() As Single = RectangularArrays.RectangularSingleArray(5, 10)
			For i As Integer = 0 To 4
				For j As Integer = 0 To 9
					f2(i)(j) = r.nextFloat()
				Next j
			Next i
			assertArrayEquals(ArrayUtil.flatten(f2), ArrayUtil.flattenFloatArray(f2), 0.0f)

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim f3[][][] As Single = new Single[5][10][15]
			Dim f3()()() As Single = RectangularArrays.RectangularSingleArray(5, 10, 15)
			Dim exp3((5 * 10 * 15) - 1) As Single
			Dim c As Integer = 0
			For i As Integer = 0 To 4
				For j As Integer = 0 To 9
					For k As Integer = 0 To 14
						Dim d As Single = r.nextFloat()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: exp3[c++] = d;
						exp3(c) = d
							c += 1
						f3(i)(j)(k) = d
					Next k
				Next j
			Next i
			assertArrayEquals(exp3, ArrayUtil.flattenFloatArray(f3), 0.0f)


'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim f4[][][][] As Single = new Single[3][5][7][9]
			Dim f4()()()() As Single = RectangularArrays.RectangularSingleArray(3, 5, 7, 9)
			Dim exp4((3 * 5 * 7 * 9) - 1) As Single
			c = 0
			For i As Integer = 0 To 2
				For j As Integer = 0 To 4
					For k As Integer = 0 To 6
						For l As Integer = 0 To 8
							Dim d As Single = r.nextFloat()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: exp4[c++] = d;
							exp4(c) = d
								c += 1
							f4(i)(j)(k)(l) = d
						Next l
					Next k
				Next j
			Next i
			assertArrayEquals(exp4, ArrayUtil.flattenFloatArray(f4), 0.0f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayShape(ByVal backend As Nd4jBackend)
			assertArrayEquals(ArrayUtil.arrayShape(New Integer(){}), New Integer() {0})
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: assertArrayEquals(ArrayUtil.arrayShape(new Integer[5][7][9]), new Integer[] {5, 7, 9})
			assertArrayEquals(ArrayUtil.arrayShape(RectangularArrays.RectangularIntegerArray(5, 7, 9)), New Integer() {5, 7, 9})
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: assertArrayEquals(ArrayUtil.arrayShape(new Object[2][3][4][5][6]), new Integer[] {2, 3, 4, 5, 6})
			assertArrayEquals(ArrayUtil.arrayShape(RectangularArrays.RectangularObjectArray(2, 3, 4, 5, 6)), New Integer() {2, 3, 4, 5, 6})
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: assertArrayEquals(ArrayUtil.arrayShape(new Double[9][7][5][3]), new Integer[] {9, 7, 5, 3})
			assertArrayEquals(ArrayUtil.arrayShape(RectangularArrays.RectangularDoubleArray(9, 7, 5, 3)), New Integer() {9, 7, 5, 3})
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: assertArrayEquals(ArrayUtil.arrayShape(new Double[1][1][1][0]), new Integer[] {1, 1, 1, 0})
			assertArrayEquals(ArrayUtil.arrayShape(RectangularArrays.RectangularDoubleArray(1, 1, 1, 0)), New Integer() {1, 1, 1, 0})
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: assertArrayEquals(ArrayUtil.arrayShape(new Char[3][2][1]), new Integer[] {3, 2, 1})
			assertArrayEquals(ArrayUtil.arrayShape(RectangularArrays.RectangularCharArray(3, 2, 1)), New Integer() {3, 2, 1})
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: assertArrayEquals(ArrayUtil.arrayShape(new String[3][2][1]), new Integer[] {3, 2, 1})
			assertArrayEquals(ArrayUtil.arrayShape(RectangularArrays.RectangularStringArray(3, 2, 1)), New Integer() {3, 2, 1})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMinOfMaxMethods(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgMinOfMaxMethods(ByVal backend As Nd4jBackend)
			Dim first() As Integer = {1, 5, 2, 4}
			Dim second() As Integer = {4, 6, 3, 2}

			assertEquals(2, ArrayUtil.argMinOfMax(first, second))

			Dim third() As Integer = {7, 3, 8, 10}
			assertEquals(1, ArrayUtil.argMinOfMax(first, second, third))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssertNotRagged(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssertNotRagged(ByVal backend As Nd4jBackend)

			'Rank 1 - should be fine
			ArrayUtil.assertNotRagged(New Object(){})
			ArrayUtil.assertNotRagged(New Object(9){})

			'Rank 2
			ArrayUtil.assertNotRagged({ New Object(3){}, New Object(3){}, New Object(3){} })
			ArrayUtil.assertNotRagged({ New Object(0){}, New Object(0){} })
			ArrayUtil.assertNotRagged({ New Double(3){}, New Double(3){}, New Double(3){} })
			Dim ragged() As Object = { New Object(3){}, New Object(3){}, New Object(3){} }
			ragged(2) = New Object(9){}
			shouldBeRagged(ragged)
			Dim ragged2()() As Double = { New Double(2){}, New Double(2){} }
			ragged2(0) = New Double(1){}
			shouldBeRagged(ragged2)

			'Rank 3
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: ArrayUtil.assertNotRagged(new Object[1][0][2])
			ArrayUtil.assertNotRagged(RectangularArrays.RectangularObjectArray(1, 0, 2))
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: ArrayUtil.assertNotRagged(new Object[2][3][4])
			ArrayUtil.assertNotRagged(RectangularArrays.RectangularObjectArray(2, 3, 4))
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: ArrayUtil.assertNotRagged(new Double[2][3][4])
			ArrayUtil.assertNotRagged(RectangularArrays.RectangularDoubleArray(2, 3, 4))
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ragged3[][][] As Object = new Object[2][3][4]
			Dim ragged3()()() As Object = RectangularArrays.RectangularObjectArray(2, 3, 4)
			ragged3(1)(2) = New Object(6){}
			shouldBeRagged(ragged3)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ragged4[][][] As Double = new Double[2][3][4]
			Dim ragged4()()() As Double = RectangularArrays.RectangularDoubleArray(2, 3, 4)
			ragged4(0)(1) = New Double(0){}
			shouldBeRagged(ragged4)

			'Rank 4:
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: ArrayUtil.assertNotRagged(new Object[2][3][4][5])
			ArrayUtil.assertNotRagged(RectangularArrays.RectangularObjectArray(2, 3, 4, 5))
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: ArrayUtil.assertNotRagged(new Double[2][3][4][5])
			ArrayUtil.assertNotRagged(RectangularArrays.RectangularDoubleArray(2, 3, 4, 5))
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim ragged5[][][][] As Object = new Object[2][3][4][5]
			Dim ragged5()()()() As Object = RectangularArrays.RectangularObjectArray(2, 3, 4, 5)
			ragged5(1)(2)(1) = { New Object(4){}, New Object(4){}, New Object(4){} }
			shouldBeRagged(ragged5)
		End Sub

		Private Shared Sub shouldBeRagged(ByVal arr() As Object)
			Try
				ArrayUtil.assertNotRagged(arr)
				fail("Expected exception")
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("Ragged array detected"),msg)
			End Try
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace