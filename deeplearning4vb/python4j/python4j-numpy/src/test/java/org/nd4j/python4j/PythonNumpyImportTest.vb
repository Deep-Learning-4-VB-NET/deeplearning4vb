Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonNumpyImportTest
	Public Class PythonNumpyImportTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void init()
		Public Shared Sub init()
			Call (New NumpyArray()).init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumpyImport()
		Public Overridable Sub testNumpyImport()
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
				Using gc As PythonGC = PythonGC.watch()
					Dim np As PythonObject = Python.importModule("numpy")
					Dim zeros As PythonObject = np.attr("zeros").call(5)
					Dim arr As INDArray = NumpyArray.INSTANCE.toJava(zeros)
					assertEquals(arr, Nd4j.zeros(DataType.DOUBLE, 5))
				End Using
			End Using

		End Sub
	End Class

End Namespace