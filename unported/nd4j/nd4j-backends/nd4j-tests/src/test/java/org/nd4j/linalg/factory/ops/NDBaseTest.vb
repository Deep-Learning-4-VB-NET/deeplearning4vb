Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
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

Namespace org.nd4j.linalg.factory.ops

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.SAMEDIFF) @NativeTag public class NDBaseTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NDBaseTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		' TODO: Comment from the review. We'll remove the new NDBase() at some point.

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAll(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAll(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.BOOL, 3, 3)
			Dim y As INDArray = base.all(x, 1)
			Dim y_exp As INDArray = Nd4j.createFromArray(False, False, False)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAny(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAny(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.BOOL)
			Dim y As INDArray = base.any(x, 1)
			Dim y_exp As INDArray = Nd4j.createFromArray(True, True, True)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgmax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgmax(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()

			Dim x As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {0.75, 0.5, 0.25},
				New Double() {0.5, 0.75, 0.25},
				New Double() {0.5, 0.25, 0.75}
			})
			Dim y As INDArray = base.argmax(x, 0) 'with default keepdims
			Dim y_exp As INDArray = Nd4j.createFromArray(0L, 1L, 2L)
			assertEquals(y_exp, y)

			y = base.argmax(x, False, 0) 'with explicit keepdims false
			assertEquals(y_exp, y)

			y = base.argmax(x, True, 0) 'with keepdims true
			y_exp = Nd4j.createFromArray(New Long()(){
				New Long() {0L, 1L, 2L}
			}) 'expect different shape.
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgmin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArgmin(ByVal backend As Nd4jBackend)
			'Copy Paste from argmax, replaced with argmin.
			Dim base As New NDBase()

			Dim x As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {0.75, 0.5, 0.25},
				New Double() {0.5, 0.75, 0.25},
				New Double() {0.5, 0.25, 0.75}
			})
			Dim y As INDArray = base.argmin(x, 0) 'with default keepdims
			Dim y_exp As INDArray = Nd4j.createFromArray(1L, 2L, 0L)
			assertEquals(y_exp, y)

			y = base.argmin(x, False, 0) 'with explicit keepdims false
			assertEquals(y_exp, y)

			y = base.argmin(x, True, 0) 'with keepdims true
			y_exp = Nd4j.createFromArray(New Long()(){
				New Long() {1L, 2L, 0L}
			}) 'expect different shape.
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = Nd4j.ones(DataType.DOUBLE, 3, 3)

			Dim z As INDArray = base.concat(0, x, y)
			assertArrayEquals(New Long(){6, 3}, z.shape())

			z = base.concat(1, x, y)
			assertArrayEquals(New Long(){3, 6}, z.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCumprod(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCumprod(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.cumprod(x, False, False, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 2.0, 3.0},
				New Double() {4.0, 10.0, 18.0},
				New Double() {28.0, 80.0, 162.0}
			})
			assertEquals(y_exp, y)

			y = base.cumprod(x, False, False, 1)
			y_exp = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 2.0, 6.0},
				New Double() {4.0, 20.0, 120.0},
				New Double() {7.0, 56.0, 504.0}
			})
			assertEquals(y_exp, y)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCumsum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCumsum(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.cumsum(x, False, False, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 2.0, 3.0},
				New Double() {5.0, 7.0, 9.0},
				New Double() {12.0, 15.0, 18.0}
			})
			assertEquals(y_exp, y)

			y = base.cumsum(x, False, False, 1)
			y_exp = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 3.0, 6.0},
				New Double() {4.0, 9.0, 15.0},
				New Double() {7.0, 15.0, 24.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDot(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDot(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 3)
			Dim y As INDArray = base.dot(x, x, 0)
			Dim y_exp As INDArray = Nd4j.scalar(14.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDynamicpartition(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDynamicpartition(ByVal backend As Nd4jBackend)
			'Try to execute the sample in the code dcumentation:
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 5)
			Dim numPartitions As Integer = 2
			Dim partitions() As Integer = {1, 0, 0, 1, 0}
			'INDArray y = base.dynamicPartition(x, partitions, numPartitions); TODO: Fix
			'TODO: crashes here. Op needs fixing.

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDynamicStitch(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDynamicStitch(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			'INDArray y = base.dynamicStitch(new INDArray[]{x, x}, 0); TODO: Fix
			'TODO: crashes here. Op needs fixing.  Bad constructor, as previously flagged. Both input and indices need to be INDArrays
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarEq(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarEq(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.eq(x, 0.0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEq(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEq(ByVal backend As Nd4jBackend)
			'element wise  eq.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.eq(x, x)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExpandDims(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExpandDims(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1,2).reshape(1,2)
			Dim y As INDArray = base.expandDims(x, 0)
			Dim y_exp As INDArray = x.reshape(ChrW(1), 1, 2)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFill(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFill(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(2, 2)
			Dim y As INDArray = base.fill(x, DataType.DOUBLE, 1.1)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1.1, 1.1},
				New Double() {1.1, 1.1}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGather(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGather(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim ind() As Integer = {0}
			Dim y As INDArray = base.gather(x, ind, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(0.0, 0.0, 0.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarGt(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarGt(ByVal backend As Nd4jBackend)
			'Scalar gt.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.gt(x, -0.1)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGt(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGt(ByVal backend As Nd4jBackend)
			'element wise  gt.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim x1 As INDArray = Nd4j.ones(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.gt(x1, x)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarGte(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarGte(ByVal backend As Nd4jBackend)
			'Scalar gte.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.gte(x, -0.1)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGte(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGte(ByVal backend As Nd4jBackend)
			'element wise  gte.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim x1 As INDArray = Nd4j.ones(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.gte(x1, x)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIdentity(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIdentity(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.identity(x)
			assertEquals(x, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvertPermutation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvertPermutation(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(2,0,1)
			Dim y As INDArray = base.invertPermutation(x)
			Dim exp As INDArray = Nd4j.createFromArray(1,2,0)
			assertEquals(exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testisNumericTensor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testisNumericTensor(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.isNumericTensor(x)
			assertEquals(Nd4j.scalar(True), y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLinspace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLinspace(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim y As INDArray = base.linspace(DataType.DOUBLE, 0.0, 9.0, 19)
			'TODO: test crashes.
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarLt(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarLt(ByVal backend As Nd4jBackend)
			'Scalar lt.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.lt(x, 0.1)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLt(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLt(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x1 As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.lt(x1, x)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarLte(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarLte(ByVal backend As Nd4jBackend)
			'Scalar gt.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.lte(x, 0.1)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLte(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLte(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x1 As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.lte(x1, x)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchCondition(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchCondition(ByVal backend As Nd4jBackend)
			' same test as TestMatchTransformOp,
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1.0, 1.0, 1.0, 0.0, 1.0, 1.0)
			Dim y As INDArray = base.matchCondition(x, Conditions.epsEquals(0.0))
			Dim y_exp As INDArray = Nd4j.createFromArray(False, False, False, True, False, False)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatchConditionCount(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatchConditionCount(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1.0, 1.0, 1.0, 0.0, 1.0, 1.0)
			Dim y As INDArray = base.matchConditionCount(x, Conditions.epsEquals(0.0))
			assertEquals(Nd4j.scalar(1L), y)

			x = Nd4j.eye(3)
			y = base.matchConditionCount(x, Conditions.epsEquals(1.0), True, 1)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Long?()(){
				New System.Nullable() {1L},
				New System.Nullable() {1L},
				New System.Nullable() {1L}
			})
			assertEquals(y_exp, y)

			y = base.matchConditionCount(x, Conditions.epsEquals(1.0), True, 0)
			y_exp = Nd4j.createFromArray(New Long?()(){
				New System.Nullable() {1L, 1L, 1L}
			})
			assertEquals(y_exp, y)

			y = base.matchConditionCount(x, Conditions.epsEquals(1.0), False, 1)
			y_exp = Nd4j.createFromArray(1L, 1L, 1L)
			assertEquals(y_exp, y)

			y = base.matchConditionCount(x, Conditions.epsEquals(1.0), False, 0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMax(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.FLOAT)
			Dim y As INDArray = base.max(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(1.0f, 1.0f, 1.0f)
			assertEquals(y_exp, y)

			y = base.max(x, True, 0)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {1.0f, 1.0f, 1.0f}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMean(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.FLOAT)
			Dim y As INDArray = base.mean(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(0.333333f, 0.333333f, 0.333333f)
			assertEquals(y_exp, y)

			y = base.mean(x, True, 0)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {0.333333f, 0.333333f, 0.333333f}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMin(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.FLOAT)
			Dim y As INDArray = base.min(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(0.0f, 0.0f, 0.0f)
			assertEquals(y_exp, y)

			y = base.min(x, True, 0)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {0.0f, 0.0f, 0.0f}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmulTranspose(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(DataType.FLOAT, 4, 3)
			Dim y As INDArray = Nd4j.rand(DataType.FLOAT, 5, 4)
			Dim exp As INDArray = x.transpose().mmul(y.transpose())
			Dim z As INDArray = (New NDBase()).mmul(x, y, True, True, False)
			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmul(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim x1 As INDArray = Nd4j.eye(3).castTo(DataType.DOUBLE)
			Dim y As INDArray = base.mmul(x, x1)
			assertEquals(y, x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarNeq(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarNeq(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.neq(x, 1.0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNeq(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNeq(ByVal backend As Nd4jBackend)
			'element wise  eq.
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(DataType.DOUBLE, 3, 3)
			Dim x1 As INDArray = Nd4j.ones(DataType.DOUBLE, 3, 3)
			Dim y As INDArray = base.neq(x, x1)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, True, True},
				New Boolean() {True, True, True},
				New Boolean() {True, True, True}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm1(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.FLOAT)
			Dim y As INDArray = base.norm1(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(1.0f, 1.0f, 1.0f)
			assertEquals(y_exp, y)

			y = base.norm1(x, True, 0)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {1.0f, 1.0f, 1.0f}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNorm2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNorm2(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.FLOAT)
			Dim y As INDArray = base.norm2(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(1.0f, 1.0f, 1.0f)
			assertEquals(y_exp, y)

			y = base.norm2(x, True, 0)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {1.0f, 1.0f, 1.0f}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNormMax(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.FLOAT)
			Dim y As INDArray = base.normmax(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(1.0f, 1.0f, 1.0f)
			assertEquals(y_exp, y)

			y = base.normmax(x, True, 0)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {1.0f, 1.0f, 1.0f}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOneHot(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOneHot(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(0.0, 1.0, 2.0)
			Dim y As INDArray = base.oneHot(x, 1, 0, 1.0, 0.0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Single()(){
				New Single() {1.0f, 0.0f, 0.0f}
			})
			assertEquals(y_exp, y)

			y = base.oneHot(x, 1)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {1.0f},
				New Single() { 0.0f},
				New Single() {0.0f}
			})
			assertEquals(y_exp, y)

			y = base.oneHot(x, 1, 0, 1.0, 0.0, DataType.DOUBLE)
			y_exp = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 0.0, 0.0}
			})
			assertEquals(y_exp, y) 'TODO: Looks like we're getting back the wrong datatype.       https://github.com/eclipse/deeplearning4j/issues/8607
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOnesLike(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOnesLike(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(3, 3)
			Dim y As INDArray = base.onesLike(x)
			Dim y_exp As INDArray = Nd4j.createFromArray(1, 1)
			assertEquals(y_exp, y)

			y = base.onesLike(x, DataType.INT64)
			y_exp = Nd4j.createFromArray(1L, 1L)
			assertEquals(y_exp, y) 'TODO: Getting back a double array, not a long.    https://github.com/eclipse/deeplearning4j/issues/8605
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPermute(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPermute(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(1, 6, 6).reshape(ChrW(2), 3)
			Dim y As INDArray = base.permute(x, 1,0)
			assertArrayEquals(New Long(){3, 2}, y.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testProd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testProd(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3).castTo(DataType.FLOAT)
			Dim y As INDArray = base.prod(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(0.0f, 0.0f, 0.0f)
			assertEquals(y_exp, y)

			y = base.prod(x, True, 0)
			y_exp = Nd4j.createFromArray(New Single()(){
				New Single() {0.0f, 0.0f, 0.0f}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRange(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim y As INDArray = base.range(0.0, 3.0, 1.0, DataType.DOUBLE)
			Dim y_exp As INDArray = Nd4j.createFromArray(0.0, 1.0, 2.0)
			assertEquals(y_exp, y) 'TODO: Asked for DOUBLE, got back a FLOAT Array.   https://github.com/eclipse/deeplearning4j/issues/8606
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRank(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRank(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.eye(3)
			Dim y As INDArray = base.rank(x)
			Dim y_exp As INDArray = Nd4j.scalar(2)
			Console.WriteLine(y)
			assertEquals(y_exp, y)
		End Sub

	'    
	'      @ParameterizedTest
	'    @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs")
	'    public void testRepeat(Nd4jBackend backend) {
	'        fail("AB 2020/01/09 - Not sure what this op is supposed to do...");
	'        NDBase base = new NDBase();
	'        INDArray x = Nd4j.eye(3);
	'        INDArray y = base.repeat(x, 0);
	'        //TODO: fix, crashes the JVM.
	'    }
	'     


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReplaceWhere(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReplaceWhere(ByVal backend As Nd4jBackend)
			' test from BooleanIndexingTest.
			Dim base As New NDBase()
			Dim array1 As INDArray = Nd4j.createFromArray(1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0)
			Dim array2 As INDArray = Nd4j.createFromArray(7.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0)

			Dim y As INDArray = base.replaceWhere(array1, array2, Conditions.greaterThan(4))
			Dim y_exp As INDArray = Nd4j.createFromArray(1.0, 2.0, 3.0, 4.0, 3.0, 2.0, 1.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReshape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReshape(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim shape As INDArray = Nd4j.createFromArray(New Long() {3, 3})
			Dim y As INDArray = base.reshape(x, shape)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 2.0, 3.0},
				New Double() { 4.0, 5.0, 6.0},
				New Double() { 7.0, 8.0, 9.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 6).reshape(ChrW(2), 3)
			Dim y As INDArray = base.reverse(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() { 4.0, 5.0, 6.0},
				New Double() {1.0, 2.0, 3.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverseSequence(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverseSequence(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim seq_kengths As INDArray = Nd4j.createFromArray(2,3,1)
			Dim y As INDArray = base.reverseSequence(x, seq_kengths)

			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() { 2.0, 1.0, 3.0},
				New Double() {6.0, 5.0, 4.0},
				New Double() {7.0, 8.0, 9.0}
			})
			assertEquals(y_exp, y)

			y = base.reverseSequence(x, seq_kengths, 0, 1)
			y_exp = Nd4j.createFromArray(New Double()(){
				New Double() { 4.0, 8.0, 3.0},
				New Double() {1.0, 5.0, 6.0},
				New Double() {7.0, 2.0, 9.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarFloorMod(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarFloorMod(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.scalarFloorMod(x, 2.0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() { 1.0, 0.0, 1.0},
				New Double() {0.0, 1.0, 0.0},
				New Double() { 1.0, 0.0, 1.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarMax(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.scalarMax(x, 5.0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() { 5.0, 5.0, 5.0},
				New Double() {5.0, 5.0, 6.0},
				New Double() { 7.0, 8.0, 9.0}
			})
			assertEquals(y_exp, y)
			'System.out.println(y);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarMin(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.scalarMin(x, 5.0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() { 1.0, 2.0, 3.0},
				New Double() {4.0, 5.0, 5.0},
				New Double() { 5.0, 5.0, 5.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarSet(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1.0, 2.0, 0.0, 4.0, 5.0)
			Dim y As INDArray = base.scalarSet(x, 1.0)
			Dim y_exp As INDArray = Nd4j.ones(DataType.DOUBLE, 5)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterAdd(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()

			'from testScatterOpGradients.
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 20, 10)
			Dim indices As INDArray = Nd4j.createFromArray(3, 4, 5, 10, 18)
			Dim updates As INDArray = Nd4j.ones(DataType.DOUBLE, 5, 10)
			Dim y As INDArray = base.scatterAdd(x,indices, updates)

			y = y.getColumn(0)
			Dim y_exp As INDArray = Nd4j.createFromArray(1.0, 1.0, 1.0, 2.0, 2.0, 2.0, 1.0, 1.0, 1.0, 1.0, 2.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 2.0, 1.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterDiv(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterDiv(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()

			'from testScatterOpGradients.
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 20, 10).add(1.0)
			Dim indices As INDArray = Nd4j.createFromArray(3, 4, 5, 10, 18)
			Dim updates As INDArray = Nd4j.ones(DataType.DOUBLE, 5, 10).add(1.0)
			Dim y As INDArray = base.scatterDiv(x,indices, updates)

			y = y.getColumn(0)
			Dim y_exp As INDArray = Nd4j.createFromArray(2.0, 2.0, 2.0, 1.0, 1.0, 1.0, 2.0, 2.0, 2.0, 2.0, 1.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 1.0, 2.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterMax(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()

			'from testScatterOpGradients.
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 20, 10).add(1.0)
			Dim indices As INDArray = Nd4j.createFromArray(3, 4, 5, 10, 18)
			Dim updates As INDArray = Nd4j.ones(DataType.DOUBLE, 5, 10).add(1.0)
			Dim y As INDArray = base.scatterMax(x,indices, updates)

			y = y.getColumn(0)
			Dim y_exp As INDArray = Nd4j.createFromArray(2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterMin(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()

			'from testScatterOpGradients.
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 20, 10).add(1.0)
			Dim indices As INDArray = Nd4j.createFromArray(3, 4, 5, 10, 18)
			Dim updates As INDArray = Nd4j.ones(DataType.DOUBLE, 5, 10).add(1.0)
			Dim y As INDArray = base.scatterMin(x,indices, updates)

			y = y.getColumn(0)
			Dim y_exp As INDArray = Nd4j.createFromArray(2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterMul(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()

			'from testScatterOpGradients.
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 20, 10).add(1.0)
			Dim indices As INDArray = Nd4j.createFromArray(3, 4, 5, 10, 18)
			Dim updates As INDArray = Nd4j.ones(DataType.DOUBLE, 5, 10).add(1.0)
			Dim y As INDArray = base.scatterMul(x,indices, updates)

			y = y.getColumn(0)
			Dim y_exp As INDArray = Nd4j.createFromArray(2.0, 2.0, 2.0, 4.0, 4.0, 4.0, 2.0, 2.0, 2.0, 2.0, 4.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 4.0, 2.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScatterSub(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScatterSub(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()

			'from testScatterOpGradients.
			Dim x As INDArray = Nd4j.ones(DataType.DOUBLE, 20, 10).add(1.0)
			Dim indices As INDArray = Nd4j.createFromArray(3, 4, 5, 10, 18)
			Dim updates As INDArray = Nd4j.ones(DataType.DOUBLE, 5, 10).add(1.0)
			Dim y As INDArray = base.scatterSub(x,indices, updates)

			y = y.getColumn(0)
			Dim y_exp As INDArray = Nd4j.createFromArray(2.0, 2.0, 2.0, 0.0, 0.0, 0.0, 2.0, 2.0, 2.0, 2.0, 0.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 0.0, 2.0)
			assertEquals(y_exp, y)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentMax(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(3, 6, 1, 4, 9,2, 2)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(0,0,1,1,1,2,2)
			Dim y As INDArray = base.segmentMax(x, segmentIDs)
			Dim y_exp As INDArray = Nd4j.createFromArray(6, 9, 2)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentMean(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(3.0, 6.0, 1.0, 4.0, 9.0,2.0, 2.0)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(0,0,1,1,1,2,2)
			Dim y As INDArray = base.segmentMean(x, segmentIDs)
			Dim y_exp As INDArray = Nd4j.createFromArray(4.5, 4.6667, 2.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentMin(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(3.0, 6.0, 1.0, 4.0, 9.0,2.0, 2.0)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(0,0,1,1,1,2,2)
			Dim y As INDArray = base.segmentMin(x, segmentIDs)
			Dim y_exp As INDArray = Nd4j.createFromArray(3.0, 1.0, 2.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentProd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentProd(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(3.0, 6.0, 1.0, 4.0, 9.0,2.0, 2.0)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(0,0,1,1,1,2,2)
			Dim y As INDArray = base.segmentProd(x, segmentIDs)
			Dim y_exp As INDArray = Nd4j.createFromArray(18.0, 36.0, 4.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSegmentSum(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(3.0, 6.0, 1.0, 4.0, 9.0,2.0, 2.0)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(0,0,1,1,1,2,2)
			Dim y As INDArray = base.segmentSum(x, segmentIDs)
			Dim y_exp As INDArray = Nd4j.createFromArray(9.0, 14.0, 4.0)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSequenceMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSequenceMask(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim length As INDArray = Nd4j.createFromArray(1, 3, 2)
			Dim maxlength As Integer = 5
			Dim dt As DataType = DataType.BOOL
			Dim y As INDArray = base.sequenceMask(length, maxlength, dt)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Boolean()(){
				New Boolean() {True, False, False, False, False},
				New Boolean() {True, True, True, False, False},
				New Boolean() {True, True, False, False, False}
			})
			assertEquals(y_exp, y)

			y = base.sequenceMask(length, maxlength, DataType.FLOAT)
			y_exp = y_exp.castTo(DataType.FLOAT)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShape(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(3,3)
			Dim y As INDArray = base.shape(x)
			Dim y_exp As INDArray = Nd4j.createFromArray(3L, 3L)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSize(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSize(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(3,3)
			Dim y As INDArray = base.size(x)
			assertEquals(Nd4j.scalar(9L), y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSizeAt(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSizeAt(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(10,20, 30)
			Dim y As INDArray = base.sizeAt(x, 1)
			assertEquals(Nd4j.scalar(20L), y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSlice(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 6).reshape(ChrW(2), 3)
			Dim y As INDArray = base.slice(x, New Integer(){0, 1}, 2,1)
			Dim y_exp As INDArray = Nd4j.create(New Double()(){
				New Double() {2.0},
				New Double() {5.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSquaredNorm(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSquaredNorm(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.squaredNorm(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(66.0, 93.0, 126.0)
			assertEquals(y_exp, y)

			y = base.squaredNorm(x, True, 0)
			y_exp = Nd4j.createFromArray(New Double()(){
				New Double() {66.0, 93.0, 126.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSqueeze(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSqueeze(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 10).reshape(ChrW(2), 1, 5)
			Dim y As INDArray = base.squeeze(x,1)
			Dim exp As INDArray = x.reshape(ChrW(2), 5)
			assertEquals(exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStack(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStack(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 3)
			Dim y As INDArray = base.stack(1, x)
			' TODO: Op definition looks wrong. Compare stack in Nd4j.
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStandardDeviation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStandardDeviation(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 4)
			Dim y As INDArray = base.standardDeviation(x, False, 0)
			assertEquals(Nd4j.scalar(1.118034), y)

			x = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			y = base.standardDeviation(x, False, True, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {2.4494898, 2.4494898, 2.4494898}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStridedSlice(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStridedSlice(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.stridedSlice(x, New Long(){0, 1}, New Long() {3, 3}, 2,1)

			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {2.0, 3.0},
				New Double() {8.0, 9.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSum(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.sum(x, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(12.0, 15.0, 18.0)
			assertEquals(y_exp, y)

			y = base.sum(x, True, 0)
			assertEquals(y_exp.reshape(ChrW(1), 3), y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorMul(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTensorMul(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim dimX() As Integer = {1}
			Dim dimY() As Integer = {0}
			Dim transposeX As Boolean = False
			Dim transposeY As Boolean = False
			Dim transposeResult As Boolean = False

			Dim res As INDArray = base.tensorMmul(x, y, dimX, dimY, transposeX, transposeY, transposeResult)
			' org.nd4j.linalg.exception.ND4JIllegalStateException: Op name tensordot - no output arrays were provided and calculateOutputShape failed to execute

			Dim exp As INDArray = x.mmul(y)
			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTile(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTile(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 4).reshape(ChrW(2), 2)
			Dim repeat As INDArray = Nd4j.createFromArray(2, 3)
			Dim y As INDArray = base.tile(x, repeat) ' the sample from the code docs.

			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 2.0, 1.0, 2.0, 1.0, 2.0},
				New Double() {3.0, 4.0, 3.0, 4.0, 3.0, 4.0},
				New Double() {1.0, 2.0, 1.0, 2.0, 1.0, 2.0},
				New Double() {3.0, 4.0, 3.0, 4.0, 3.0, 4.0}
			})
			assertEquals(y_exp, y)

			y = base.tile(x, 2, 3) ' the sample from the code docs.
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTranspose(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTranspose(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			Dim y As INDArray = base.transpose(x)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {1.0, 4.0, 7.0},
				New Double() {2.0, 5.0, 8.0},
				New Double() {3.0, 6.0, 9.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnsegmentMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnsegmentMax(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1,3,2,6,4,9,8)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(1,0,2,0,1,1,2)
			Dim y As INDArray = base.unsortedSegmentMax(x, segmentIDs, 3)
			Dim y_exp As INDArray = Nd4j.createFromArray(6,9,8)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnsegmentMean(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnsegmentMean(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1,3,2,6,4,9,8).castTo(DataType.FLOAT)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(1,0,2,0,1,1,2)
			Dim y As INDArray = base.unsortedSegmentMean(x, segmentIDs, 3)
			Dim y_exp As INDArray = Nd4j.createFromArray(4.5f,4.6667f, 5.0f)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnsegmentedMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnsegmentedMin(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1,3,2,6,4,9,8)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(1,0,2,0,1,1,2)
			Dim y As INDArray = base.unsortedSegmentMin(x, segmentIDs, 3)
			Dim y_exp As INDArray = Nd4j.createFromArray(3,1,2)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnsegmentProd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnsegmentProd(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1,3,2,6,4,9,8)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(1,0,2,0,1,1,2)
			Dim y As INDArray = base.unsortedSegmentProd(x, segmentIDs, 3)
			Dim y_exp As INDArray = Nd4j.createFromArray(18,36,16)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnsortedSegmentSqrtN(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnsortedSegmentSqrtN(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1.0,3.0,2.0,6.0,4.0,9.0,8.0)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(1,0,2,0,1,1,2)
			Dim y As INDArray = base.unsortedSegmentSqrtN(x, segmentIDs, 3)
			Dim y_exp As INDArray = Nd4j.createFromArray(6.3640, 8.0829, 7.0711)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnsortedSegmentSum(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnsortedSegmentSum(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.createFromArray(1,3,2,6,4,9,8)
			Dim segmentIDs As INDArray = Nd4j.createFromArray(1,0,2,0,1,1,2)
			Dim y As INDArray = base.unsortedSegmentSum(x, segmentIDs, 3)
			Dim y_exp As INDArray = Nd4j.createFromArray(9,14,10)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariance(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 4)
			Dim y As INDArray = base.variance(x, False, 0)
			assertEquals(Nd4j.scalar(1.250), y)

			x = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 9).reshape(ChrW(3), 3)
			y = base.variance(x, False, True, 0)
			Dim y_exp As INDArray = Nd4j.createFromArray(New Double()(){
				New Double() {6.0, 6.0, 6.0}
			})
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testZerosLike(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testZerosLike(ByVal backend As Nd4jBackend)
			Dim base As New NDBase()
			Dim x As INDArray = Nd4j.zeros(3,3)
			Dim y As INDArray = base.zerosLike(x)
			assertEquals(x, y)
			assertNotSame(x, y)
		End Sub
	End Class

End Namespace