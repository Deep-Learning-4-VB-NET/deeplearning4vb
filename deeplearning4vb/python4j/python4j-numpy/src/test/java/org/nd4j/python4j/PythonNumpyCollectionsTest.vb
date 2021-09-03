Imports System.Collections
Imports System.Collections.Generic
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports PythonException = org.nd4j.python4j.PythonException
Imports PythonGIL = org.nd4j.python4j.PythonGIL
Imports PythonObject = org.nd4j.python4j.PythonObject
Imports PythonTypes = org.nd4j.python4j.PythonTypes
Imports Test = org.junit.jupiter.api.Test
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonNumpyCollectionsTest
	Public Class PythonNumpyCollectionsTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void init()
		Public Shared Sub init()
			Call (New NumpyArray()).init()
		End Sub


		Public Shared Function params() As Stream(Of Arguments)
			Return java.util.Arrays.asList(New DataType(){ DataType.BOOL, DataType.FLOAT16, DataType.FLOAT, DataType.DOUBLE, DataType.INT8, DataType.INT16, DataType.INT32, DataType.INT64, DataType.UINT8, DataType.UINT16, DataType.UINT32, DataType.UINT64 }).Select(AddressOf Arguments.of)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.nd4j.python4j.PythonNumpyCollectionsTest#params") @ParameterizedTest public void testPythonDictFromMap(org.nd4j.linalg.api.buffer.DataType dataType) throws org.nd4j.python4j.PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPythonDictFromMap(ByVal dataType As DataType)
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As org.nd4j.python4j.PythonGIL = org.nd4j.python4j.PythonGIL.lock()
				Dim map As System.Collections.IDictionary = New Hashtable()
				map("a") = 1
				map(1) = "a"
				map("arr") = Nd4j.ones(dataType, 2, 3)
				map("list1") = java.util.Arrays.asList(1, 2.0, 3, 4f, Nd4j.zeros(dataType,3,2))
				Dim innerMap As System.Collections.IDictionary = New Hashtable()
				innerMap("b") = 2
				innerMap(2) = "b"
				innerMap(5) = Nd4j.ones(dataType, 5)
				map("innermap") = innerMap
				map("list2") = java.util.Arrays.asList(4, "5", innerMap, False, True)
				Dim dict As PythonObject = PythonTypes.convert(map)
				Dim map2 As System.Collections.IDictionary = PythonTypes.DICT.toJava(dict)
				assertEquals(map.ToString(), map2.ToString())
			End Using

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.nd4j.python4j.PythonNumpyCollectionsTest#params") @ParameterizedTest public void testPythonListFromList(org.nd4j.linalg.api.buffer.DataType dataType) throws org.nd4j.python4j.PythonException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPythonListFromList(ByVal dataType As DataType)
'JAVA TO VB CONVERTER NOTE: The variable pythonGIL was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Using pythonGIL_Conflict As org.nd4j.python4j.PythonGIL = org.nd4j.python4j.PythonGIL.lock()
				Dim list As IList(Of Object) = New List(Of Object)()
				list.Add(1)
				list.Add("2")
				list.Add(Nd4j.ones(dataType, 2, 3))
				list.Add(java.util.Arrays.asList("a", Nd4j.ones(dataType, 1, 2),1.0, 2f, 10, True, False, Nd4j.zeros(dataType, 3, 2)))
				Dim map As System.Collections.IDictionary = New Hashtable()
				map("a") = 1
				map(1) = "a"
				map(5) = Nd4j.ones(dataType,4, 5)
				map("list1") = java.util.Arrays.asList(1, 2.0, 3, 4f, Nd4j.zeros(dataType, 3, 1))
				list.Add(map)
				Dim dict As PythonObject = PythonTypes.convert(list)
				Dim list2 As System.Collections.IList = PythonTypes.LIST.toJava(dict)
				assertEquals(list.ToString(), list2.ToString())
			End Using

		End Sub
	End Class

End Namespace