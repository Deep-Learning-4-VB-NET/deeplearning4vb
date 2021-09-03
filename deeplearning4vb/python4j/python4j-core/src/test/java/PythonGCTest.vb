Imports Tag = org.junit.jupiter.api.Tag
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Python = org.nd4j.python4j.Python
Imports PythonGC = org.nd4j.python4j.PythonGC
Imports PythonGIL = org.nd4j.python4j.PythonGIL
Imports PythonObject = org.nd4j.python4j.PythonObject
Imports Test = org.junit.jupiter.api.Test
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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag public class PythonGCTest
Public Class PythonGCTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGC() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testGC()
		Using pythonGIL As org.nd4j.python4j.PythonGIL = org.nd4j.python4j.PythonGIL.lock()
			Dim gcModule As PythonObject = Python.importModule("gc")
			Dim getObjects As PythonObject = gcModule.attr("get_objects")
			Dim pyObjCount1 As PythonObject = Python.len(getObjects.call())
			Dim objCount1 As Long = pyObjCount1.toLong()
			Dim pyList As PythonObject = Python.list()
			pyList.attr("append").call("a")
			pyList.attr("append").call(1.0)
			pyList.attr("append").call(True)
			Dim pyObjCount2 As PythonObject = Python.len(getObjects.call())
			Dim objCount2 As Long = pyObjCount2.toLong()
			Dim diff As Long = objCount2 - objCount1
			assertTrue(diff > 2)
			Using gc As org.nd4j.python4j.PythonGC = org.nd4j.python4j.PythonGC.watch()
				Dim pyList2 As PythonObject = Python.list()
				pyList2.attr("append").call("a")
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
