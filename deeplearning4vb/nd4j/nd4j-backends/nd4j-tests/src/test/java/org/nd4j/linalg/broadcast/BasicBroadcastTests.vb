Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LessThan = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThan
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports RealDivOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RealDivOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertThrows
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

Namespace org.nd4j.linalg.broadcast

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class BasicBroadcastTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class BasicBroadcastTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 5)
			Dim y As val = Nd4j.createFromArray(New Single(){1.0f, 1.0f, 1.0f, 1.0f, 1.0f})
			Dim e As val = Nd4j.create(DataType.FLOAT, 3, 5).assign(1.0f)

			' inplace setup
			Dim op As val = New AddOp(New INDArray(){x, y}, New INDArray(){x})

			Nd4j.exec(op)

			assertEquals(e, x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_2(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2)
			Dim y As val = Nd4j.createFromArray(New Single(){1.0f, 1.0f, 1.0f, 1.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.FLOAT, 3, 2, 2).assign(1.0f)

			'Nd4j.exec(new PrintVariable(x, "X array"));
			'Nd4j.exec(new PrintVariable(y, "Y array"));

			Dim z As val = x.add(y)

			assertEquals(e, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_3(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(1)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.FLOAT, 3, 2, 2).assign(2.0f)

			Dim z As val = x.mul(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_4(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.FLOAT, 3, 2, 2).assign(2.0f)

			Dim z As val = x.div(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_5(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.FLOAT, 3, 2, 2).assign(2.0f)

			Dim z As val = x.sub(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_6(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.FLOAT, 3, 2, 2).assign(-2.0f)

			Dim z As val = x.rsub(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_7(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.BOOL, 3, 2, 2).assign(False)

			Dim z As val = x.lt(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastFailureTest_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastFailureTest_1(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim z As val = x.subi(y)
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastFailureTest_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastFailureTest_2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim z As val = x.divi(y)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastFailureTest_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastFailureTest_3(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException), Sub()
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim z As val = x.muli(y)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastFailureTest_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastFailureTest_4(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim z As val = x.addi(y)
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastFailureTest_5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastFailureTest_5(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim z As val = x.rsubi(y)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastFailureTest_6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastFailureTest_6(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim z As val = x.rdivi(y)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_8(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_8(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(4.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.BOOL, 3, 2, 2).assign(True)

			Dim z As val = x.gt(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_9(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_9(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(2.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.BOOL, 3, 2, 2).assign(True)

			Dim z As val = x.eq(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicBroadcastTest_10(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub basicBroadcastTest_10(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 3, 1, 2).assign(1.0f)
			Dim y As val = Nd4j.createFromArray(New Single(){2.0f, 2.0f, 2.0f, 2.0f}).reshape(ChrW(2), 2)
			Dim e As val = Nd4j.create(DataType.BOOL, 3, 2, 2).assign(False)

			Dim z As val = x.eq(y)

			assertEquals(e, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void emptyBroadcastTest_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub emptyBroadcastTest_1(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 1, 2)
			Dim y As val = Nd4j.create(DataType.FLOAT, 0, 2)

			Dim z As val = x.add(y)
			assertEquals(y, z)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void emptyBroadcastTest_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub emptyBroadcastTest_2(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 1, 2)
			Dim y As val = Nd4j.create(DataType.FLOAT, 0, 2)

			Dim z As val = x.addi(y)
			assertEquals(y, z)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void emptyBroadcastTest_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub emptyBroadcastTest_3(ByVal backend As Nd4jBackend)
			Dim x As val = Nd4j.create(DataType.FLOAT, 1, 0, 1)
			Dim y As val = Nd4j.create(DataType.FLOAT, 1, 0, 2)

			Dim op As val = New RealDivOp(New INDArray(){x, y}, New INDArray(){})
			Dim z As val = Nd4j.exec(op)(0)

			assertEquals(y, z)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testValidInvalidBroadcast(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testValidInvalidBroadcast(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(3,1)
			Dim y As INDArray = Nd4j.create(3, 4)

			x.add(y)
			y.addi(x)
			Try
				x.addi(y)
			Catch e As Exception
				Dim s As String = e.Message
				assertTrue(s.Contains("broadcast") AndAlso s.Contains("shape"),s)
			End Try

			x.sub(y)
			y.subi(x)
			Try
				x.subi(y)
			Catch e As Exception
				Dim s As String = e.Message
				assertTrue(s.Contains("broadcast") AndAlso s.Contains("shape"),s)
			End Try

			x.mul(y)
			y.muli(x)
			Try
				x.muli(y)
			Catch e As Exception
				Dim s As String = e.Message
				assertTrue(s.Contains("broadcast") AndAlso s.Contains("shape"),s)
			End Try

			x.div(y)
			y.divi(x)
			Try
				x.divi(y)
			Catch e As Exception
				Dim s As String = e.Message
				assertTrue(s.Contains("broadcast") AndAlso s.Contains("shape"),s)
			End Try

			x.rsub(y)
			y.rsubi(x)
			Try
				x.rsubi(y)
			Catch e As Exception
				Dim s As String = e.Message
				assertTrue(s.Contains("broadcast") AndAlso s.Contains("shape"),s)
			End Try

			x.rdiv(y)
			y.rdivi(x)
			Try
				x.rdivi(y)
			Catch e As Exception
				Dim s As String = e.Message
				assertTrue(s.Contains("broadcast") AndAlso s.Contains("shape"),s)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLt(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLt(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.scalar(0)
			Dim y As INDArray = Nd4j.createFromArray(2,1,2)

			Dim result As INDArray = Nd4j.create(DataType.BOOL, 3)
			Dim lt As INDArray = Nd4j.exec(New LessThan(x,y,result))(0)

			Dim exp As INDArray = Nd4j.createFromArray(True, True, True)
			assertEquals(exp, lt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdd(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.scalar(0)
			Dim y As INDArray = Nd4j.createFromArray(2,1,2)

			Dim result As INDArray = Nd4j.create(DataType.INT, 3)
			Dim sum As INDArray = Nd4j.exec(New AddOp(x,y,result))(0)

			Dim exp As INDArray = Nd4j.createFromArray(2, 1, 2)
			assertEquals(exp, sum)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBroadcatableBool_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBroadcatableBool_1(ByVal backend As Nd4jBackend)
			Dim op As val = DynamicCustomOp.builder("greater_equal").addInputs(Nd4j.create(DataType.FLOAT, 3), Nd4j.create(DataType.FLOAT, 3)).build()

			Dim l As val = op.calculateOutputShape()
			assertEquals(1, l.size())
			assertEquals(DataType.BOOL, l.get(0).dataType())
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace