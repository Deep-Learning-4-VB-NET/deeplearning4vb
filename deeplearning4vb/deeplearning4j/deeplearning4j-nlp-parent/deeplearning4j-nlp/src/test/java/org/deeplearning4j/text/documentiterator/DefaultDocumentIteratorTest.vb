Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class DefaultDocumentIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class DefaultDocumentIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDocumentIterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDocumentIterator()
			Dim reuters5250 As New ClassPathResource("/reuters/5250")
			Dim f As File = reuters5250.File

			Dim iter As DocumentIterator = New FileDocumentIterator(f.getAbsolutePath())

			Dim doc As Stream = iter.nextDocument()

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			Dim [next] As Tokenizer = t.create(doc)
			Dim list() As String = "PEARSON CONCENTRATES ON FOUR SECTORS".Split(" ", True)
			'/PEARSON CONCENTRATES ON FOUR SECTORS
			Dim count As Integer = 0
			Do While [next].hasMoreTokens() AndAlso count < list.Length
				Dim token As String = [next].nextToken()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(list[count++], token);
				assertEquals(list(count), token)
					count += 1
			Loop


			doc.Close()
		End Sub
	End Class

End Namespace