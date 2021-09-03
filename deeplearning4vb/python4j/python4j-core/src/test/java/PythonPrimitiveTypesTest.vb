Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Tag = org.junit.jupiter.api.Tag
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.python4j
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonPrimitiveTypesTest
Public Class PythonPrimitiveTypesTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInt() throws PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testInt()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim j As Long = 3
			Dim p As PythonObject = PythonTypes.INT.toPython(j)
			Dim j2 As Long = PythonTypes.INT.toJava(p)

			assertEquals(j, j2)

			Dim p2 As PythonObject = PythonTypes.convert(j)
			Dim j3 As Long = PythonTypes.INT.toJava(p2)

			assertEquals(j, j3)
		End Using

	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStr() throws PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testStr()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim s As String = "abcd"
			Dim p As PythonObject = PythonTypes.STR.toPython(s)
			Dim s2 As String = PythonTypes.STR.toJava(p)

			assertEquals(s, s2)

			Dim p2 As PythonObject = PythonTypes.convert(s)
			Dim s3 As String = PythonTypes.STR.toJava(p2)

			assertEquals(s, s3)
		End Using

	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFloat() throws PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testFloat()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim f As Double = 7
			Dim p As PythonObject = PythonTypes.FLOAT.toPython(f)
			Dim f2 As Double = PythonTypes.FLOAT.toJava(p)

			assertEquals(f, f2, 1e-5)

			Dim p2 As PythonObject = PythonTypes.convert(f)
			Dim f3 As Double = PythonTypes.FLOAT.toJava(p2)

			assertEquals(f, f3, 1e-5)
		End Using

	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBool() throws PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testBool()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim b As Boolean = True
			Dim p As PythonObject = PythonTypes.BOOL.toPython(b)
			Dim b2 As Boolean = PythonTypes.BOOL.toJava(p)

			assertEquals(b, b2)

			Dim p2 As PythonObject = PythonTypes.convert(b)
			Dim b3 As Boolean = PythonTypes.BOOL.toJava(p2)

			assertEquals(b, b3)
		End Using

	End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBytes()
	Public Overridable Sub testBytes()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim bytes(255) As SByte
			For i As Integer = 0 To 255
				bytes(i) = CSByte(i)
			Next i
			Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
			inputs.Add(New PythonVariable(Of )("b1", PythonTypes.BYTES, bytes))
			Dim outputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
			outputs.Add(New PythonVariable(Of )("b2", PythonTypes.BYTES))
			Dim code As String = "b2=b1"
			PythonExecutioner.exec(code, inputs, outputs)
			assertArrayEquals(bytes, CType(outputs(0).getValue(), SByte()))
		End Using

	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBytes2()
	Public Overridable Sub testBytes2()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim bytes() As SByte = {97, 98, 99}
			Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
			inputs.Add(New PythonVariable(Of )("b1", PythonTypes.BYTES, bytes))
			Dim outputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
			outputs.Add(New PythonVariable(Of )("s1", PythonTypes.STR))
			outputs.Add(New PythonVariable(Of )("b2", PythonTypes.BYTES))
			Dim code As String = "s1 = ''.join(chr(c) for c in b1)" & vbLf & "b2=b'def'"
			PythonExecutioner.exec(code, inputs, outputs)
			assertEquals("abc", outputs(0).getValue())
			assertArrayEquals(New SByte(){100, 101, 102}, CType(outputs(1).getValue(), SByte()))

		End Using
	End Sub

End Class
