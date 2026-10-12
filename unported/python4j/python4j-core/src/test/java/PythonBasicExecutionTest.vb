Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.python4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertThrows

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




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonBasicExecutionTest
Public Class PythonBasicExecutionTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSimpleExecIllegal()
	Public Overridable Sub testSimpleExecIllegal()
		assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim code As String = "print('Hello World')"
			PythonExecutioner.exec(code)
		End Sub)


	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimpleExec()
	Public Overridable Sub testSimpleExec()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim code As String = "print('Hello World')"
			PythonExecutioner.exec(code)
		End Using

	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBadCode() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testBadCode()
		Try
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
				Dim code As String = "printx('Hello world')"
				PythonExecutioner.exec(code)
			End Using

		Catch e As Exception
			assertEquals("NameError: name 'printx' is not defined", e.Message)
			Return
		End Try
		Throw New Exception("Bad code did not throw!")
	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExecWithInputs()
	Public Overridable Sub testExecWithInputs()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
			inputs.Add(New PythonVariable(Of )("x", PythonTypes.STR, "Hello "))
			inputs.Add(New PythonVariable(Of )("y", PythonTypes.STR, "World"))
			Dim code As String = "print(x + y)"
			PythonExecutioner.exec(code, inputs, Nothing)
		End Using

	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExecWithInputsAndOutputs()
	Public Overridable Sub testExecWithInputsAndOutputs()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
			inputs.Add(New PythonVariable(Of )("x", PythonTypes.STR, "Hello "))
			inputs.Add(New PythonVariable(Of )("y", PythonTypes.STR, "World"))
			Dim [out] As PythonVariable = New PythonVariable(Of )("z", PythonTypes.STR)
			Dim code As String = "z = x + y"
			PythonExecutioner.exec(code, inputs, Collections.singletonList([out]))
			assertEquals("Hello World", [out].getValue())
		End Using
	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExecAndReturnAllVariables()
	Public Overridable Sub testExecAndReturnAllVariables()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			PythonContextManager.reset()
			Dim code As String = "a = 5" & vbLf & "b = '10'" & vbLf & "c = 20.0"
			Dim vars As IList(Of PythonVariable) = PythonExecutioner.execAndReturnAllVariables(code)

			assertEquals("a", vars(0).getName())
			assertEquals(PythonTypes.INT, vars(0).getType())
			assertEquals(5L, CLng(vars(0).getValue()))

			assertEquals("b", vars(1).getName())
			assertEquals(PythonTypes.STR, vars(1).getType())
			assertEquals("10", vars(1).getValue().ToString())

			assertEquals("c", vars(2).getName())
			assertEquals(PythonTypes.FLOAT, vars(2).getType())
			assertEquals(20.0, CDbl(vars(2).getValue()), 1e-5)

		End Using
	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExecWithInputsAndReturnAllVariables()
	Public Overridable Sub testExecWithInputsAndReturnAllVariables()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			PythonContextManager.reset()
			Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
			inputs.Add(New PythonVariable(Of )("a", PythonTypes.INT, 5))
			Dim code As String = "b = '10'" & vbLf & "c = 20.0 + a"
			Dim vars As IList(Of PythonVariable) = PythonExecutioner.execAndReturnAllVariables(code, inputs)

			assertEquals("a", vars(0).getName())
			assertEquals(PythonTypes.INT, vars(0).getType())
			assertEquals(5L, CLng(vars(0).getValue()))

			assertEquals("b", vars(1).getName())
			assertEquals(PythonTypes.STR, vars(1).getType())
			assertEquals("10", vars(1).getValue().ToString())

			assertEquals("c", vars(2).getName())
			assertEquals(PythonTypes.FLOAT, vars(2).getType())
			assertEquals(25.0, CDbl(vars(2).getValue()), 1e-5)

		End Using
	End Sub

End Class
