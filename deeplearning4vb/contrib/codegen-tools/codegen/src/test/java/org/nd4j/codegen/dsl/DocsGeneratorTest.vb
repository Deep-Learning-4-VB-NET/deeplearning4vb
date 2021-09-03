Imports Microsoft.VisualBasic
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports Test = org.junit.jupiter.api.Test
Imports DocsGenerator = org.nd4j.codegen.impl.java.DocsGenerator
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 
Namespace org.nd4j.codegen.dsl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Docs Generator Test") class DocsGeneratorTest
	Friend Class DocsGeneratorTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test J Dto MD Adapter") void testJDtoMDAdapter()
		Friend Overridable Sub testJDtoMDAdapter()
			Dim original As String = "{@code %INPUT_TYPE% eye = eye(3,2)" & vbLf & "                eye:" & vbLf & "                [ 1, 0]" & vbLf & "                [ 0, 1]" & vbLf & "                [ 0, 0]}"
			Dim expected As String = "{ INDArray eye = eye(3,2)" & vbLf & "                eye:" & vbLf & "                [ 1, 0]" & vbLf & "                [ 0, 1]" & vbLf & "                [ 0, 0]}"
			Dim adapter As New DocsGenerator.JavaDocToMDAdapter(original)
			Dim [out] As String = adapter.filter("@code", StringUtils.EMPTY).filter("%INPUT_TYPE%", "INDArray").ToString()
			assertEquals([out], expected)
		End Sub
	End Class

End Namespace