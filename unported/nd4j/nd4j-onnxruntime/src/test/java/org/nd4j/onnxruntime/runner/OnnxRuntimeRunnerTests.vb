Imports System.Collections.Generic
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.nd4j.onnxruntime.runner


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class OnnxRuntimeRunnerTests
	Public Class OnnxRuntimeRunnerTests
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testAdd() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAdd()
			Dim classPathResource As New ClassPathResource("add.onnx")
			Dim f As File = classPathResource.File
			Dim x As INDArray = Nd4j.scalar(1.0f).reshape(ChrW(1), 1)
			Dim y As INDArray = Nd4j.scalar(1.0f).reshape(ChrW(1), 1)
			Dim onnxRuntimeRunner As OnnxRuntimeRunner = OnnxRuntimeRunner.builder().modelUri(f.getAbsolutePath()).build()
			Dim inputs As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			inputs("x") = x
			inputs("y") = y
			Dim exec As IDictionary(Of String, INDArray) = onnxRuntimeRunner.exec(inputs)
			Dim z As INDArray = exec("z")
			assertEquals(2.0,z.sumNumber().doubleValue(),1e-1)
		End Sub

	End Class

End Namespace