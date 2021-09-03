Imports System.Collections
Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.python4j
Imports Test = org.junit.jupiter.api.Test
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
'ORIGINAL LINE: @javax.annotation.concurrent.NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonCollectionsTest
Public Class PythonCollectionsTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPythonDictFromMap() throws PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testPythonDictFromMap()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
	  Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
		  Dim map As System.Collections.IDictionary = New Hashtable()
		  map("a") = 1
		  map(1) = "a"
		  map("list1") = java.util.Arrays.asList(1, 2.0, 3, 4f)
		  Dim innerMap As System.Collections.IDictionary = New Hashtable()
		  innerMap("b") = 2
		  innerMap(2) = "b"
		  map("innermap") = innerMap
		  map("list2") = java.util.Arrays.asList(4, "5", innerMap, False, True)
		  Dim dict As PythonObject = PythonTypes.convert(map)
		  Dim map2 As System.Collections.IDictionary = PythonTypes.DICT.toJava(dict)
		  assertEquals(map.ToString(), map2.ToString())
	  End Using

	End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPythonListFromList() throws PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testPythonListFromList()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
			Dim list As IList(Of Object) = New List(Of Object)()
			list.Add(1)
			list.Add("2")
			list.Add(java.util.Arrays.asList("a", 1.0, 2f, 10, True, False))
			Dim map As System.Collections.IDictionary = New Hashtable()
			map("a") = 1
			map(1) = "a"
			map("list1") = java.util.Arrays.asList(1, 2.0, 3, 4f)
			list.Add(map)
			Dim dict As PythonObject = PythonTypes.convert(list)
			Dim list2 As System.Collections.IList = PythonTypes.LIST.toJava(dict)
			assertEquals(list.ToString(), list2.ToString())
		End Using

	End Sub
End Class
