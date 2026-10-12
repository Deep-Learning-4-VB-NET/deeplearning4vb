Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertTrue

'
' *
' *  *  ******************************************************************************
' *  *  *
' *  *  *
' *  *  * This program and the accompanying materials are made available under the
' *  *  * terms of the Apache License, Version 2.0 which is available at
' *  *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *  *
' *  *  *  See the NOTICE file distributed with this work for additional
' *  *  *  information regarding copyright ownership.
' *  *  * Unless required by applicable law or agreed to in writing, software
' *  *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  *  * License for the specific language governing permissions and limitations
' *  *  * under the License.
' *  *  *
' *  *  * SPDX-License-Identifier: Apache-2.0
' *  *  *****************************************************************************
' *
' *
' 

Namespace org.nd4j.python4j
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
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonNumpyGCTest
	Public Class PythonNumpyGCTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void init()
		Public Shared Sub init()
			Call (New NumpyArray()).init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGC()
		Public Overridable Sub testGC()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
				Dim gcModule As PythonObject = Python.importModule("gc")
				Dim getObjects As PythonObject = gcModule.attr("get_objects")
				Dim pyObjCount1 As PythonObject = Python.len(getObjects.call())
				Dim objCount1 As Long = pyObjCount1.toLong()
				Dim pyList As PythonObject = Python.list()
				pyList.attr("append").call(New PythonObject(Nd4j.linspace(1, 10, 10)))
				pyList.attr("append").call(1.0)
				pyList.attr("append").call(True)
				Dim pyObjCount2 As PythonObject = Python.len(getObjects.call())
				Dim objCount2 As Long = pyObjCount2.toLong()
				Dim diff As Long = objCount2 - objCount1
				assertTrue(diff > 2)
				Using gc As PythonGC = PythonGC.watch()
					Dim pyList2 As PythonObject = Python.list()
					pyList2.attr("append").call(New PythonObject(Nd4j.linspace(1, 10, 10)))
					pyList2.attr("append").call(1.0)
					pyList2.attr("append").call(True)
				End Using
				Dim pyObjCount3 As PythonObject = Python.len(getObjects.call())
				Dim objCount3 As Long = pyObjCount3.toLong()
				diff = objCount3 - objCount2
				assertTrue(diff <= 2) ' 2 objects created during function call
			End Using

		End Sub
	End Class

End Namespace