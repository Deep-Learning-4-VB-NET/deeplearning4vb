Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OptTestConfig = org.nd4j.autodiff.optimization.util.OptTestConfig
Imports OptimizationTestUtil = org.nd4j.autodiff.optimization.util.OptimizationTestUtil
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports GraphOptimizer = org.nd4j.autodiff.samediff.optimize.GraphOptimizer
Imports ConstantFunctionOptimizations = org.nd4j.autodiff.samediff.optimize.optimizations.ConstantFunctionOptimizations
Imports IdentityFunctionOptimizations = org.nd4j.autodiff.samediff.optimize.optimizations.IdentityFunctionOptimizations
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.junit.Assert

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
Namespace org.nd4j.autodiff.optimization


	Public Class TestOptimization
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path tempDir;
		Friend tempDir As Path

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 1_000_000_000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConstantOpFolding(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend)
		Public Overridable Sub testConstantOpFolding(ByVal nd4jBackend As Nd4jBackend)
			'We expect 2 things in this test:
			'(a) the output of  add(constant, constant) is pre-calculated and itself becomes a constant
			'(b) the


			Dim sd As SameDiff = SameDiff.create()
			Dim c As SDVariable = sd.constant("c", Nd4j.scalar(1.0))
			Dim c2 As SDVariable = c.add("add", 1)
			Dim v As SDVariable = sd.var("variable", Nd4j.scalar(1.0))
			Dim [out] As SDVariable = v.sub("out", c2)

			Dim copy As SameDiff = sd.dup()

			Dim optimized As SameDiff = GraphOptimizer.optimize(sd, "out")
			assertEquals(3, optimized.getVariables().size()) '"add", "variable", "out" -> "c" should be removed
			assertEquals(VariableType.CONSTANT, optimized.getVariable("add").getVariableType())
			assertEquals(1, optimized.getOps().size())
			assertEquals("subtract", optimized.getOps().values().GetEnumerator().next().getName())

			assertFalse(optimized.hasVariable("c"))

			assertEquals(sd.outputSingle(Collections.emptyMap(), "out"), optimized.outputSingle(Collections.emptyMap(), "out"))

			'Check the

			'Check that the original can be saved and loaded, and still gives the same results

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConstantOpFolding2(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend)
		Public Overridable Sub testConstantOpFolding2(ByVal nd4jBackend As Nd4jBackend)
			'We expect 2 things in this test:
			'(a) the output of  add(constant, constant) is pre-calculated and itself becomes a constant
			'(b) the


			Dim sd As SameDiff = SameDiff.create()
			Dim c As SDVariable = sd.constant("c", Nd4j.scalar(1.0))
			Dim c2 As SDVariable = c.add("add", 1)
			Dim v As SDVariable = sd.var("variable", Nd4j.scalar(1.0))
			Dim [out] As SDVariable = v.sub("out", c2)

			Dim subDir As File = tempDir.resolve("op-folding").toFile()
			assertTrue(subDir.mkdirs())
			Dim conf As OptTestConfig = OptTestConfig.builder().original(sd).tempFolder(subDir).outputs(Collections.singletonList("out")).mustApply(sd.getVariables().get("add").getOutputOfOp(), GetType(ConstantFunctionOptimizations.FoldConstantFunctions)).build()

			Dim optimized As SameDiff = OptimizationTestUtil.testOptimization(conf)
			assertEquals(3, optimized.getVariables().size()) '"add", "variable", "out" -> "c" should be removed
			assertEquals(VariableType.CONSTANT, optimized.getVariable("add").getVariableType())
			assertEquals(1, optimized.getOps().size())
			assertEquals("subtract", optimized.getOps().values().GetEnumerator().next().getName())

			assertFalse(optimized.hasVariable("c"))

			assertEquals(sd.outputSingle(Collections.emptyMap(), "out"), optimized.outputSingle(Collections.emptyMap(), "out"))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIdentityRemoval(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend)
		Public Overridable Sub testIdentityRemoval(ByVal nd4jBackend As Nd4jBackend)

			'Ensure that optimizer is actually used when calling output methods:
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 3))
			Dim i1 As SDVariable = sd.identity([in])
			Dim i2 As SDVariable = sd.identity(w)
			Dim i3 As SDVariable = sd.identity(b)
			Dim [out] As SDVariable = sd.nn_Conflict.softmax("out", sd.identity(i1.mmul(i2).add(i3)))


			Dim subDir As File = tempDir.resolve("new-dir-identity-removal").toFile()
			assertTrue(subDir.mkdirs())

			Dim conf As OptTestConfig = OptTestConfig.builder().original(sd).tempFolder(subDir).outputs(Collections.singletonList("out")).placeholder("in", Nd4j.rand(DataType.FLOAT, 5, 4)).mustApply(sd.getVariables().get(i1.name()).getOutputOfOp(), GetType(IdentityFunctionOptimizations.RemoveIdentityOps)).mustApply(sd.getVariables().get(i2.name()).getOutputOfOp(), GetType(IdentityFunctionOptimizations.RemoveIdentityOps)).mustApply(sd.getVariables().get(i3.name()).getOutputOfOp(), GetType(IdentityFunctionOptimizations.RemoveIdentityOps)).build()

			Dim optimized As SameDiff = OptimizationTestUtil.testOptimization(conf)
			assertEquals(3, optimized.getOps().size())
			assertFalse(optimized.hasVariable(i1.name()))
			assertFalse(optimized.hasVariable(i2.name()))
			assertFalse(optimized.hasVariable(i3.name()))
			assertTrue(optimized.hasVariable("out"))
		End Sub
	End Class
End Namespace