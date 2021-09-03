Imports Tag = org.junit.jupiter.api.Tag
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Python = org.nd4j.python4j.Python
Imports PythonContextManager = org.nd4j.python4j.PythonContextManager
Imports PythonExecutioner = org.nd4j.python4j.PythonExecutioner
Imports Test = org.junit.jupiter.api.Test
Imports PythonGIL = org.nd4j.python4j.PythonGIL
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
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.PYTHON) public class PythonContextManagerTest
Public Class PythonContextManagerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInt() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testInt()
		Using pythonGIL As org.nd4j.python4j.PythonGIL = org.nd4j.python4j.PythonGIL.lock()
			Python.Context = "context1"
			Python.exec("a = 1")
			Python.Context = "context2"
			Python.exec("a = 2")
			Python.Context = "context3"
			Python.exec("a = 3")


			Python.Context = "context1"
			assertEquals(1, PythonExecutioner.getVariable("a").toInt())

			Python.Context = "context2"
			assertEquals(2, PythonExecutioner.getVariable("a").toInt())

			Python.Context = "context3"
			assertEquals(3, PythonExecutioner.getVariable("a").toInt())

			PythonContextManager.deleteNonMainContexts()

		End Using

	End Sub

End Class
