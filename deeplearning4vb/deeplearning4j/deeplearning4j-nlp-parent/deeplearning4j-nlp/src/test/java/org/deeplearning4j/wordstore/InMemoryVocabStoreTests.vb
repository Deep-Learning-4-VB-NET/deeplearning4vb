Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports InMemoryLookupCache = org.deeplearning4j.models.word2vec.wordstore.inmemory.InMemoryLookupCache
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.wordstore

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class InMemoryVocabStoreTests extends org.deeplearning4j.BaseDL4JTest
	Public Class InMemoryVocabStoreTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStorePut()
		Public Overridable Sub testStorePut()
			Dim cache As VocabCache(Of VocabWord) = New InMemoryLookupCache()
			assertFalse(cache.containsWord("hello"))
			cache.addWordToIndex(0, "hello")
			assertTrue(cache.containsWord("hello"))
			assertEquals(1, cache.numWords())
			assertEquals("hello", cache.wordAtIndex(0))
		End Sub
	End Class

End Namespace