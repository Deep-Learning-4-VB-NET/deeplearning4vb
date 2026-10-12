Imports System
Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.autodiff.samediff



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.SAMEDIFF) @NativeTag public class NameScopeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NameScopeTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVariableNameScopesBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVariableNameScopesBasic(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim v As SDVariable = sd.var("x")
			Using ns As NameScope = sd.withNameScope("nameScope")
				Dim v2 As SDVariable = sd.var("x2")
				assertEquals("nameScope/x2", v2.name())
				assertTrue(sd.getVariables().containsKey("nameScope/x2"))
				assertEquals("nameScope", sd.currentNameScope())

				Dim v3 As SDVariable = sd.var("x")
				assertEquals("nameScope/x", v3.name())
				assertTrue(sd.getVariables().containsKey("nameScope/x"))

				Using ns2 As NameScope = sd.withNameScope("scope2")
					assertEquals("nameScope/scope2", sd.currentNameScope())
					Dim v4 As SDVariable = sd.var("x")
					assertEquals("nameScope/scope2/x", v4.name())
					assertTrue(sd.getVariables().containsKey("nameScope/scope2/x"))
				End Using

				assertEquals("nameScope", sd.currentNameScope())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpFieldsAndNames(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOpFieldsAndNames(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.var("x", DataType.FLOAT, 1)
			Dim y As SDVariable
			Dim z As SDVariable

			Dim add As SDVariable
			Dim addWithName As SDVariable
			Dim merge As SDVariable
			Dim mergeWithName As SDVariable
			Using ns As NameScope = sd.withNameScope("s1")
				y = sd.var("y", DataType.FLOAT, 1)
				add = x.add(y)
				addWithName = x.add("addxy", y)
				Using ns2 As NameScope = sd.withNameScope("s2")
					z = sd.var("z", DataType.FLOAT, 1)
					merge = sd.math().mergeMax(New SDVariable(){y, z})
					mergeWithName = sd.math_Conflict.mergeMax("mmax", New SDVariable(){y, z})
				End Using
			End Using
			Dim a As SDVariable = sd.var("a", DataType.FLOAT, 1)

			assertEquals("x", x.name())
			assertEquals("s1/y", y.name())
			assertEquals("s1/s2/z", z.name())
			assertEquals("a", a.name())

			assertTrue(add.name().StartsWith("s1/", StringComparison.Ordinal),add.name())
			assertEquals("s1/addxy", addWithName.name())

			assertTrue(merge.name().StartsWith("s1/s2/", StringComparison.Ordinal),merge.name())
			assertEquals("s1/s2/mmax", mergeWithName.name())

			Dim allowedVarNames As ISet(Of String) = New HashSet(Of String)(Arrays.asList("x", "s1/y", "s1/s2/z", "a", add.name(), addWithName.name(), merge.name(), mergeWithName.name()))
			Dim allowedOpNames As ISet(Of String) = New HashSet(Of String)()

			'Check op names:
			Dim ops As IDictionary(Of String, SameDiffOp) = sd.getOps()
			Console.WriteLine(ops.Keys)

			For Each s As String In ops.Keys
				assertTrue(s.StartsWith("s1", StringComparison.Ordinal) OrElse s.StartsWith("s1/s2", StringComparison.Ordinal),s)
				allowedOpNames.Add(s)
			Next s

			'Check fields - Variable, SDOp, etc
			For Each v As Variable In sd.getVariables().values()
				assertTrue(allowedVarNames.Contains(v.getVariable().name()),v.getVariable().name())
				assertEquals(v.getName(), v.getVariable().name())
				If v.getInputsForOp() IsNot Nothing Then
					For Each s As String In v.getInputsForOp()
						assertTrue(allowedOpNames.Contains(s),s)
					Next s
				End If

				If v.getOutputOfOp() IsNot Nothing Then
					assertTrue(allowedOpNames.Contains(v.getOutputOfOp()))
				End If
			Next v

			assertTrue(allowedOpNames.ContainsAll(sd.getOps().keySet()))

			For Each op As SameDiffOp In sd.getOps().values()
				assertTrue(allowedOpNames.Contains(op.Name))
				assertEquals(op.Name, op.Op.getOwnName())
				If op.getInputsToOp() IsNot Nothing Then
					assertTrue(allowedVarNames.ContainsAll(op.getInputsToOp()))
				End If

				If op.getOutputsOfOp() IsNot Nothing Then
					assertTrue(allowedVarNames.ContainsAll(op.getOutputsOfOp()))
				End If
			Next op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoNesting(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoNesting(ByVal backend As Nd4jBackend)
			Dim SD As SameDiff = SameDiff.create()

			Dim a As SDVariable = SD.constant(4)

			Dim scope As NameScope = SD.withNameScope("test")

			Dim [out] As SDVariable = SD.argmax(a)

			[out].add(45)

			scope.Dispose()

			assertTrue(SD.variableMap().ContainsKey("test/argmax"),"Var with name test/argmax exists")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoTesting2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoTesting2(ByVal backend As Nd4jBackend)
			Dim SD As SameDiff = SameDiff.create()

			Dim a As SDVariable = SD.constant(4)
			Dim b As SDVariable = SD.constant(5).lt(4)

			Dim scope As NameScope = SD.withNameScope("test")

			Dim [out] As SDVariable = SD.switchOp(a, b)(0)

			[out].add(45)

			scope.Dispose()

			assertTrue(SD.variableMap().ContainsKey("test/switch:1"),"Var with name test/switch:1 exists")
		End Sub
	End Class

End Namespace