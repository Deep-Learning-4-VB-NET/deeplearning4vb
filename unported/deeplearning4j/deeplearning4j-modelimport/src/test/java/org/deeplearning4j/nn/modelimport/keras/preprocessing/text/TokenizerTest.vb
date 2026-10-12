Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessing.text


	''' <summary>
	''' Tests for Keras Tokenizer
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class TokenizerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class TokenizerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void tokenizerBasics()
		Public Overridable Sub tokenizerBasics()
			Dim numDocs As Integer = 5
			Dim numWords As Integer = 12

			Dim tokenizer As New KerasTokenizer(numWords)

			Dim texts() As String = { "Black then white are all I see", "In my infancy", "Red and yellow then came to be", "Reaching out to me", "Lets me see." }

			tokenizer.fitOnTexts(texts)
			assertEquals(numDocs, tokenizer.getDocumentCount().intValue())

			Dim matrix As INDArray = tokenizer.textsToMatrix(texts, TokenizerMode.BINARY)
			assertArrayEquals(New Long() {numDocs, numWords}, matrix.shape())

			Dim sequences()() As Integer? = tokenizer.textsToSequences(texts)

			tokenizer.sequencesToTexts(sequences)
			tokenizer.sequencesToMatrix(sequences, TokenizerMode.TFIDF)
			tokenizer.fitOnSequences(sequences)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void tokenizerParity()
		Public Overridable Sub tokenizerParity()
			' See #7448
			Dim tokenize As New KerasTokenizer(1000)
			Dim itemsArray() As String = { "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." }
			tokenize.fitOnTexts(itemsArray)
			Dim index As IDictionary(Of String, Integer) = tokenize.getWordIndex()
			Dim expectedIndex As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			expectedIndex("lorem") = 1
			expectedIndex("ipsum") = 2
			expectedIndex("dolor") = 3
			expectedIndex("sit") = 4
			expectedIndex("amet") = 5
			expectedIndex("consectetur") = 6
			expectedIndex("adipiscing") = 7
			expectedIndex("elit") = 8
			expectedIndex("sed") = 9
			expectedIndex("do") = 10
			expectedIndex("eiusmod") = 11
			expectedIndex("tempor") = 12
			expectedIndex("incididunt") = 13
			expectedIndex("ut") = 14
			expectedIndex("labore") = 15
			expectedIndex("et") = 16
			expectedIndex("dolore") = 17
			expectedIndex("magna") = 18
			expectedIndex("aliqua") = 19
			assertEquals(expectedIndex.Count, index.Count)
			For Each entry As KeyValuePair(Of String, Integer) In expectedIndex.SetOfKeyValuePairs()
				assertEquals(entry.Value, index(entry.Key))
			Next entry

		End Sub
	End Class

End Namespace