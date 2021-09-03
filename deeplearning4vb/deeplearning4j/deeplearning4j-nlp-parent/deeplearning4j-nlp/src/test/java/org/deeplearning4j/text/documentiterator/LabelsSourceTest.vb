Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.text.documentiterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class LabelsSourceTest extends org.deeplearning4j.BaseDL4JTest
	Public Class LabelsSourceTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNextLabel1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNextLabel1()
			Dim generator As New LabelsSource("SENTENCE_")

			assertEquals("SENTENCE_0", generator.nextLabel())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNextLabel2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNextLabel2()
			Dim generator As New LabelsSource("SENTENCE_%d_HAHA")

			assertEquals("SENTENCE_0_HAHA", generator.nextLabel())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNextLabel3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNextLabel3()
			Dim list As IList(Of String) = New List(Of String) From {"LABEL0", "LABEL1", "LABEL2"}
			Dim generator As New LabelsSource(list)

			assertEquals("LABEL0", generator.nextLabel())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabelsCount1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLabelsCount1()
			Dim list As IList(Of String) = New List(Of String) From {"LABEL0", "LABEL1", "LABEL2"}
			Dim generator As New LabelsSource(list)

			assertEquals("LABEL0", generator.nextLabel())
			assertEquals("LABEL1", generator.nextLabel())
			assertEquals("LABEL2", generator.nextLabel())

			assertEquals(3, generator.NumberOfLabelsUsed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabelsCount2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLabelsCount2()
			Dim generator As New LabelsSource("SENTENCE_")

			assertEquals("SENTENCE_0", generator.nextLabel())
			assertEquals("SENTENCE_1", generator.nextLabel())
			assertEquals("SENTENCE_2", generator.nextLabel())
			assertEquals("SENTENCE_3", generator.nextLabel())
			assertEquals("SENTENCE_4", generator.nextLabel())

			assertEquals(5, generator.NumberOfLabelsUsed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabelsCount3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLabelsCount3()
			Dim generator As New LabelsSource("SENTENCE_")

			assertEquals("SENTENCE_0", generator.nextLabel())
			assertEquals("SENTENCE_1", generator.nextLabel())
			assertEquals("SENTENCE_2", generator.nextLabel())
			assertEquals("SENTENCE_3", generator.nextLabel())
			assertEquals("SENTENCE_4", generator.nextLabel())

			assertEquals(5, generator.NumberOfLabelsUsed)

			generator.reset()

			assertEquals(5, generator.NumberOfLabelsUsed)
		End Sub
	End Class

End Namespace