Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.nativ

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class NativeBlasTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NativeBlasTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			Nd4j.Executioner.enableDebugMode(True)
			Nd4j.Executioner.enableVerboseMode(True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void setDown()
		Public Overridable Sub setDown()
			Nd4j.Executioner.enableDebugMode(False)
			Nd4j.Executioner.enableVerboseMode(False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemm1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemm1(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3)
			Dim B As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.create(DataType.DOUBLE, New Long() {3, 3}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)

			' ?
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemm2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemm2(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3).dup("f"c)
			Dim B As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3).dup("f"c)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.create(DataType.DOUBLE, New Long() {3, 3}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)

			' ?
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemm3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemm3(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3).dup("f"c)
			Dim B As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.create(DataType.DOUBLE, New Long() {3, 3}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)

			' ?
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemm4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemm4(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 4, 3)
			Dim B As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 3, 4)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.create(DataType.DOUBLE, New Long() {4, 4}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)

			' ?
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemm5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemm5(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 4, 3).dup("f"c)
			Dim B As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 3, 4)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.create(DataType.DOUBLE, New Long() {4, 4}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)

			' ?
			assertEquals(exp, res)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemm6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemm6(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 4, 3).dup("f"c)
			Dim B As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 3, 4).dup("f"c)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.createUninitialized(DataType.DOUBLE, New Long() {4, 4}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)

			' ?
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemm7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemm7(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 4, 3)
			Dim B As val = Nd4j.linspace(1, 12, 12, DataType.DOUBLE).reshape("c"c, 3, 4).dup("f"c)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.createUninitialized(DataType.DOUBLE, New Long() {4, 4}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)

			' ?
			assertEquals(exp, res)
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemv1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemv1(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3)
			Dim B As val = Nd4j.linspace(1, 3, 3, DataType.DOUBLE).reshape("c"c, 3, 1)

			Dim res As val = Nd4j.create(DataType.DOUBLE, New Long() {3, 1}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)


			Dim exp As val = A.mmul(B)
	'        log.info("exp: {}", exp);

			' ?
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemv2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemv2(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3).dup("f"c)
			Dim B As val = Nd4j.linspace(1, 3, 3, DataType.DOUBLE).reshape("c"c, 3, 1).dup("f"c)

			Dim res As val = Nd4j.createUninitialized(DataType.DOUBLE, New Long() {3, 1}, "f"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)


			Dim exp As val = A.mmul(B)
	'        log.info("exp mean: {}", exp.meanNumber());

			' ?
			assertEquals(exp, res)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasGemv3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBlasGemv3(ByVal backend As Nd4jBackend)

			' we're skipping blas here
			If Nd4j.Executioner.GetType().Name.ToLower().contains("cuda") Then
				Return
			End If

			Dim A As val = Nd4j.linspace(1, 20, 20, DataType.FLOAT).reshape("c"c, 4, 5)
			Dim B As val = Nd4j.linspace(1, 5, 5, DataType.FLOAT).reshape("c"c, 5, 1)

			Dim exp As val = A.mmul(B)

			Dim res As val = Nd4j.createUninitialized(DataType.FLOAT, New Long() {4, 1}, "c"c)

			Dim matmul As val = DynamicCustomOp.builder("matmul").addInputs(A, B).addOutputs(res).build()

			Nd4j.Executioner.exec(matmul)




			' ?
			assertEquals(exp, res)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace