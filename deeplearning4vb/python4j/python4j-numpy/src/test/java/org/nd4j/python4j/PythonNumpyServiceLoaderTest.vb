Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NumpyArray = org.nd4j.python4j.NumpyArray
Imports PythonTypes = org.nd4j.python4j.PythonTypes
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
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonNumpyServiceLoaderTest
	Public Class PythonNumpyServiceLoaderTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void init()
		Public Shared Sub init()
			Call (New NumpyArray()).init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testServiceLoader()
		Public Overridable Sub testServiceLoader()
			assertEquals(NumpyArray.INSTANCE, PythonTypes.get(Of INDArray)("numpy.ndarray"))
			assertEquals(NumpyArray.INSTANCE, PythonTypes.getPythonTypeForJavaObject(Nd4j.zeros(1)))
		End Sub
	End Class

End Namespace