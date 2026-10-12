Imports System
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports WhereNumpy = org.nd4j.linalg.api.ops.impl.controlflow.WhereNumpy
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports CompareAndReplace = org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndReplace
Imports CompareAndSet = org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports GreaterThan = org.nd4j.linalg.indexing.conditions.GreaterThan
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

Namespace org.nd4j.linalg.indexing


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class BooleanIndexingTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class BooleanIndexingTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			assertTrue(BooleanIndexing.and(array, Conditions.greaterThan(0.5f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			assertTrue(BooleanIndexing.and(array, Conditions.lessThan(6.0f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			assertFalse(BooleanIndexing.and(array, Conditions.lessThan(5.0f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd4(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			assertFalse(BooleanIndexing.and(array, Conditions.greaterThan(4.0f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd5(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1e-5f, 1e-5f, 1e-5f, 1e-5f, 1e-5f})

			assertTrue(BooleanIndexing.and(array, Conditions.greaterThanOrEqual(1e-5f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd6(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1e-5f, 1e-5f, 1e-5f, 1e-5f, 1e-5f})

			assertFalse(BooleanIndexing.and(array, Conditions.lessThan(1e-5f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd7(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1e-5f, 1e-5f, 1e-5f, 1e-5f, 1e-5f})

			assertTrue(BooleanIndexing.and(array, Conditions.equals(1e-5f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOr1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOr1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			assertTrue(BooleanIndexing.or(array, Conditions.greaterThan(3.0f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOr2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOr2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			assertTrue(BooleanIndexing.or(array, Conditions.lessThan(3.0f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOr3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOr3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f})

			assertFalse(BooleanIndexing.or(array, Conditions.greaterThan(6.0f)))
		End Sub

	'    
	'        2D array checks
	'     

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2dAnd1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2dAnd1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(10, 10)

			assertTrue(BooleanIndexing.and(array, Conditions.equals(0f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2dAnd2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2dAnd2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(DataType.DOUBLE,10, 10)
			array.slice(4).putScalar(2, 1e-4)
	'        System.out.println(array);
			Dim [and] As Boolean = BooleanIndexing.and(array, Conditions.epsEquals(0f))
			assertFalse([and])


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2dAnd3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2dAnd3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(10, 10)

			array.slice(4).putScalar(2, 1e-5f)

			assertFalse(BooleanIndexing.and(array, Conditions.greaterThan(0f)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2dAnd4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2dAnd4(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.zeros(10, 10)

			array.slice(4).putScalar(2, 1e-5f)

			assertTrue(BooleanIndexing.or(array, Conditions.greaterThan(1e-6f)))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConditionalAssign1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConditionalAssign1(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7})
			Dim array2 As INDArray = Nd4j.create(New Double() {7, 6, 5, 4, 3, 2, 1})
			Dim comp As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 3, 2, 1})

			BooleanIndexing.replaceWhere(array1, array2, Conditions.greaterThan(4))

			assertEquals(comp, array1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaSTransform1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaSTransform1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5})

			Nd4j.Executioner.exec(New CompareAndSet(array, 3, Conditions.equals(0)))

			assertEquals(comp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaSTransform2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaSTransform2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {3, 2, 3, 4, 5})

			Nd4j.Executioner.exec(New CompareAndSet(array, 3.0, Conditions.lessThan(2)))

			assertEquals(comp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaSPairwiseTransform1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaSPairwiseTransform1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5})

			Nd4j.Executioner.exec(New CompareAndSet(array, comp, Conditions.lessThan(5)))

			assertEquals(comp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaRPairwiseTransform1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaRPairwiseTransform1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5})

			Dim z As INDArray = Nd4j.exec(New CompareAndReplace(array, comp, Conditions.lessThan(1)))

			assertEquals(comp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaSPairwiseTransform2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaSPairwiseTransform2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim y As INDArray = Nd4j.create(New Double() {2, 4, 3, 0, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {2, 4, 3, 4, 5})

			Nd4j.Executioner.exec(New CompareAndSet(x, y, Conditions.epsNotEquals(0.0)))

			assertEquals(comp, x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaRPairwiseTransform2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaRPairwiseTransform2(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim y As INDArray = Nd4j.create(New Double() {2, 4, 3, 4, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {2, 4, 0, 4, 5})

			Dim z As INDArray = Nd4j.exec(New CompareAndReplace(x, y, Conditions.epsNotEquals(0.0)))

			assertEquals(comp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaSPairwiseTransform3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaSPairwiseTransform3(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim y As INDArray = Nd4j.create(New Double() {2, 4, 3, 4, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {2, 4, 3, 4, 5})

			Dim z As INDArray = Nd4j.exec(New CompareAndReplace(x, y, Conditions.lessThan(4)))

			assertEquals(comp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCaRPairwiseTransform3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCaRPairwiseTransform3(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {1, 2, 0, 4, 5})
			Dim y As INDArray = Nd4j.create(New Double() {2, 4, 3, 4, 5})
			Dim comp As INDArray = Nd4j.create(New Double() {2, 2, 3, 4, 5})

			Dim z As INDArray = Nd4j.exec(New CompareAndReplace(x, y, Conditions.lessThan(2)))

			assertEquals(comp, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchConditionAllDimensions1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchConditionAllDimensions1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9})

			Dim val As Integer = CInt(Math.Truncate(Nd4j.Executioner.exec(New MatchCondition(array, Conditions.lessThan(5))).getDouble(0)))

			assertEquals(5, val)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchConditionAllDimensions2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchConditionAllDimensions2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, Double.NaN, 5, 6, 7, 8, 9})

			Dim val As Integer = CInt(Math.Truncate(Nd4j.Executioner.exec(New MatchCondition(array, Conditions.Nan)).getDouble(0)))

			assertEquals(1, val)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchConditionAllDimensions3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchConditionAllDimensions3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {0, 1, 2, 3, Double.NegativeInfinity, 5, 6, 7, 8, 9})

			Dim val As Integer = CInt(Math.Truncate(Nd4j.Executioner.exec(New MatchCondition(array, Conditions.Infinite)).getDouble(0)))

			assertEquals(1, val)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchConditionAlongDimension1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchConditionAlongDimension1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.ones(3, 10)
			array.getRow(2).assign(0.0)

			Dim result() As Boolean = BooleanIndexing.and(array, Conditions.equals(0.0), 1)
			Dim comp() As Boolean = {False, False, True}

	'        System.out.println("Result: " + Arrays.toString(result));
			assertArrayEquals(comp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchConditionAlongDimension2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchConditionAlongDimension2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.ones(3, 10)
			array.getRow(2).assign(0.0).putScalar(0, 1.0)

	'        System.out.println("Array: " + array);

			Dim result() As Boolean = BooleanIndexing.or(array, Conditions.lessThan(0.9), 1)
			Dim comp() As Boolean = {False, False, True}

	'        System.out.println("Result: " + Arrays.toString(result));
			assertArrayEquals(comp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchConditionAlongDimension3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchConditionAlongDimension3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.ones(3, 10)
			array.getRow(2).assign(0.0).putScalar(0, 1.0)

			Dim result() As Boolean = BooleanIndexing.and(array, Conditions.lessThan(0.0), 1)
			Dim comp() As Boolean = {False, False, False}

	'        System.out.println("Result: " + Arrays.toString(result));
			assertArrayEquals(comp, result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConditionalUpdate(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConditionalUpdate(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(-2, 2, 5, DataType.DOUBLE)
			Dim ones As INDArray = Nd4j.ones(DataType.DOUBLE, 5)
			Dim exp As INDArray = Nd4j.create(New Double() {1, 1, 0, 1, 1})


			Nd4j.Executioner.exec(New CompareAndSet(ones, arr, ones, Conditions.equals(0.0)))

			assertEquals(exp, ones)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFirstIndex1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFirstIndex1(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9, 0})
			Dim result As INDArray = BooleanIndexing.firstIndex(arr, Conditions.greaterThanOrEqual(3))

			assertEquals(2, result.getDouble(0), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFirstIndex2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFirstIndex2(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9, 0})
			Dim result As INDArray = BooleanIndexing.firstIndex(arr, Conditions.lessThan(3))

			assertEquals(0, result.getDouble(0), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLastIndex1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLastIndex1(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9, 0})
			Dim result As INDArray = BooleanIndexing.lastIndex(arr, Conditions.greaterThanOrEqual(3))

			assertEquals(8, result.getDouble(0), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFirstIndex2D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFirstIndex2D(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {1, 2, 3, 0, 1, 3, 7, 8, 9}).reshape("c"c, 3, 3)
			Dim result As INDArray = BooleanIndexing.firstIndex(arr, Conditions.greaterThanOrEqual(2), 1)
			Dim exp As INDArray = Nd4j.create(New Long() {1, 2, 0}, New Long(){3}, DataType.LONG)

			assertEquals(exp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLastIndex2D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLastIndex2D(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double() {1, 2, 3, 0, 1, 3, 7, 8, 0}).reshape("c"c, 3, 3)
			Dim result As INDArray = BooleanIndexing.lastIndex(arr, Conditions.greaterThanOrEqual(2), 1)
			Dim exp As INDArray = Nd4j.create(New Long() {2, 2, 1}, New Long(){3}, DataType.LONG)

			assertEquals(exp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEpsEquals1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEpsEquals1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Double() {-1, -1, -1e-8, 1e-8, 1, 1})
			Dim condition As New MatchCondition(array, Conditions.epsEquals(0.0))
			Dim numZeroes As Integer = Nd4j.Executioner.exec(condition).getInt(0)

			assertEquals(2, numZeroes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testChooseNonZero(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testChooseNonZero(ByVal backend As Nd4jBackend)
			Dim testArr As INDArray = Nd4j.create(New Double() { 0.00, 0.51, 0.68, 0.69, 0.86, 0.91, 0.96, 0.97, 0.97, 1.03, 1.13, 1.16, 1.16, 1.17, 1.19, 1.25, 1.25, 1.26, 1.27, 1.28, 1.29, 1.29, 1.29, 1.30, 1.31, 1.32, 1.33, 1.33, 1.35, 1.35, 1.36, 1.37, 1.38, 1.40, 1.41, 1.42, 1.43, 1.44, 1.44, 1.45, 1.45, 1.47, 1.47, 1.51, 1.51, 1.51, 1.52, 1.53, 1.56, 1.57, 1.58, 1.59, 1.61, 1.62, 1.63, 1.63, 1.64, 1.64, 1.66, 1.66, 1.67, 1.67, 1.70, 1.70, 1.70, 1.72, 1.72, 1.72, 1.72, 1.73, 1.74, 1.74, 1.76, 1.76, 1.77, 1.77, 1.80, 1.80, 1.81, 1.82, 1.83, 1.83, 1.84, 1.84, 1.84, 1.85, 1.85, 1.85, 1.86, 1.86, 1.87, 1.88, 1.89, 1.89, 1.89, 1.89, 1.89, 1.91, 1.91, 1.91, 1.92, 1.94, 1.95, 1.97, 1.98, 1.98, 1.98, 1.98, 1.98, 1.99, 2.00, 2.00, 2.01, 2.01, 2.02, 2.03, 2.03, 2.03, 2.04, 2.04, 2.05, 2.06, 2.07, 2.08, 2.08, 2.08, 2.08, 2.09, 2.09, 2.10, 2.10, 2.11, 2.11, 2.11, 2.12, 2.12, 2.13, 2.13, 2.14, 2.14, 2.14, 2.14, 2.15, 2.15, 2.16, 2.16, 2.16, 2.16, 2.16, 2.17 })

			Dim filtered As INDArray = BooleanIndexing.chooseFrom(New INDArray(){testArr},Arrays.asList(0.0), Collections.emptyList(),New GreaterThan())
			assertFalse(filtered.getDouble(0) = 0)
			assertEquals(testArr.length() - 1,filtered.length())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testChooseBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testChooseBasic(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ANY_PANIC
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(True)
			Dim arr As INDArray = Nd4j.linspace(1,4,4, Nd4j.dataType()).reshape(ChrW(2), 2)
			Dim filtered As INDArray = BooleanIndexing.chooseFrom(New INDArray(){arr}, Arrays.asList(2.0), Collections.emptyList(),New GreaterThan())
			assertEquals(2, filtered.length())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testChooseGreaterThanZero(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testChooseGreaterThanZero(ByVal backend As Nd4jBackend)
			Dim zero As INDArray = Nd4j.linspace(0,4,4, Nd4j.dataType())
			Dim filtered As INDArray = BooleanIndexing.chooseFrom(New INDArray(){zero},Arrays.asList(0.0), Collections.emptyList(),New GreaterThan())
			assertEquals(3, filtered.length())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testChooseNone(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testChooseNone(ByVal backend As Nd4jBackend)
			Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ANY_PANIC
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(True)
			Dim arr As INDArray = Nd4j.linspace(1,4,4, Nd4j.dataType()).reshape(ChrW(2), 2)
			Dim filtered As INDArray = BooleanIndexing.chooseFrom(New INDArray(){arr},Arrays.asList(5.0), Collections.emptyList(),New GreaterThan())
			assertNull(filtered)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhere(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWhere(ByVal backend As Nd4jBackend)
			Dim data As INDArray = Nd4j.create(4)
			Dim mask As INDArray = Nd4j.create(DataType.BOOL, 4)
			Dim put As INDArray = Nd4j.create(4)
			Dim resultData As INDArray = Nd4j.create(4)
			Dim assertion As INDArray = Nd4j.create(4)
			For i As Integer = 0 To 3
				data.putScalar(i,i)
				If i > 1 Then
					assertion.putScalar(i, 5.0)
					mask.putScalar(i, 1)
				Else
					assertion.putScalar(i, i)
					mask.putScalar(i, 0)
				End If

				put.putScalar(i, 5.0)
				resultData.putScalar(i, 0.0)
			Next i


			Nd4j.Executioner.exec(New WhereNumpy(New INDArray(){mask, data, put},New INDArray(){resultData}))
			assertEquals(assertion,resultData)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEpsStuff_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEpsStuff_1(ByVal backend As Nd4jBackend)
			Dim dtype As val = Nd4j.dataType()
			Dim array As val = Nd4j.create(New Single(){0.001f, 5e-6f, 5e-6f, 5e-6f, 5e-6f})
			Dim exp As val = Nd4j.create(New Single(){0.001f, 1.0f, 1.0f, 1.0f, 1.0f})
			BooleanIndexing.replaceWhere(array, 1.0f, Conditions.epsEquals(0))

			assertEquals(exp, array)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace