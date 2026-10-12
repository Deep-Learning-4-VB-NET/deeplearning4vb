Imports Slf4j = lombok.extern.slf4j.Slf4j
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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Slf4j @NativeTag public class TransformsTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TransformsTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEq1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEq1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 1, 2, 1})
			Dim exp As INDArray = Nd4j.create(New Boolean() {False, False, True, False})

			Dim z As INDArray = x.eq(2)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNEq1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNEq1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 1, 2, 1})
			Dim exp As INDArray = Nd4j.create(New Boolean() {True, False, True, False})

			Dim z As INDArray = x.neq(1)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLT1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLT1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 1, 2, 1})
			Dim exp As INDArray = Nd4j.create(New Boolean() {True, True, False, True})

			Dim z As INDArray = x.lt(2)

			assertEquals(exp, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGT1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGT1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 1, 2, 4})
			Dim exp As INDArray = Nd4j.create(New Boolean() {False, False, True, True})

			Dim z As INDArray = x.gt(1)

			assertEquals(exp, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarMinMax1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarMinMax1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {1, 3, 5, 7})
			Dim xCopy As INDArray = x.dup()
			Dim exp1 As INDArray = Nd4j.create(New Double() {1, 3, 5, 7})
			Dim exp2 As INDArray = Nd4j.create(New Double() {1e-5, 1e-5, 1e-5, 1e-5})

			Dim z1 As INDArray = Transforms.max(x, Nd4j.EPS_THRESHOLD, True)
			Dim z2 As INDArray = Transforms.min(x, Nd4j.EPS_THRESHOLD, True)

			assertEquals(exp1, z1)
			assertEquals(exp2, z2)
			' Assert that x was not modified
			assertEquals(x, xCopy)

			Dim exp3 As INDArray = Nd4j.create(New Double() {10, 10, 10, 10})
			Transforms.max(x, 10, False)
			assertEquals(exp3, x)

			Transforms.min(x, Nd4j.EPS_THRESHOLD, False)
			assertEquals(exp2, x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayMinMax(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayMinMax(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {1, 3, 5, 7})
			Dim y As INDArray = Nd4j.create(New Double() {2, 2, 6, 6})
			Dim xCopy As INDArray = x.dup()
			Dim yCopy As INDArray = y.dup()
			Dim expMax As INDArray = Nd4j.create(New Double() {2, 3, 6, 7})
			Dim expMin As INDArray = Nd4j.create(New Double() {1, 2, 5, 6})

			Dim z1 As INDArray = Transforms.max(x, y, True)
			Dim z2 As INDArray = Transforms.min(x, y, True)

			assertEquals(expMax, z1)
			assertEquals(expMin, z2)
			' Assert that x was not modified
			assertEquals(xCopy, x)

			Transforms.max(x, y, False)
			' Assert that x was modified
			assertEquals(expMax, x)
			' Assert that y was not modified
			assertEquals(yCopy, y)

			' Reset the modified x
			x = xCopy.dup()

			Transforms.min(x, y, False)
			' Assert that X was modified
			assertEquals(expMin, x)
			' Assert that y was not modified
			assertEquals(yCopy, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAnd1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAnd1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 0, 1, 0, 0})
			Dim y As INDArray = Nd4j.create(New Double() {0, 0, 1, 1, 0})
			Dim e As INDArray = Nd4j.create(New Boolean() {False, False, True, False, False})

			Dim z As INDArray = Transforms.and(x, y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOr1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOr1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 0, 1, 0, 0})
			Dim y As INDArray = Nd4j.create(New Double() {0, 0, 1, 1, 0})
			Dim e As val = Nd4j.create(New Boolean() {False, False, True, True, False})

			Dim z As INDArray = Transforms.or(x, y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testXor1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testXor1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 0, 1, 0, 0})
			Dim y As INDArray = Nd4j.create(New Double() {0, 0, 1, 1, 0})
			Dim exp As INDArray = Nd4j.create(New Boolean() {False, False, False, True, False})

			Dim z As INDArray = Transforms.xor(x, y)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNot1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNot1(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.create(New Double() {0, 0, 1, 0, 0})
			Dim exp As INDArray = Nd4j.create(New Boolean() {False, False, True, False, False})

			Dim z As INDArray = Transforms.not(x)

			assertEquals(exp, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlice_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSlice_1(ByVal backend As Nd4jBackend)
			Dim arr As val = Nd4j.linspace(1,4, 4, DataType.FLOAT).reshape(ChrW(2), 2, 1)
			Dim exp0 As val = Nd4j.create(New Single(){1, 2}, New Integer() {2, 1})
			Dim exp1 As val = Nd4j.create(New Single(){3, 4}, New Integer() {2, 1})

			Dim slice0 As val = arr.slice(0).dup("c"c)
			assertEquals(exp0, slice0)
			assertEquals(exp0, arr.slice(0))

			Dim slice1 As val = arr.slice(1).dup("c"c)
			assertEquals(exp1, slice1)
			assertEquals(exp1, arr.slice(1))

			Dim tf As val = arr.slice(1)
			Dim slice1_1 As val = tf.slice(0)
			assertTrue(slice1_1.isScalar())
			assertEquals(3.0, slice1_1.getDouble(0), 1e-5)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace