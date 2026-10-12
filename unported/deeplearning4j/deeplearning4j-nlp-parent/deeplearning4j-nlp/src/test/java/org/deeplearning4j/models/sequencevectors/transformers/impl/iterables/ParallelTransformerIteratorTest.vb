Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports BasicLabelAwareIterator = org.deeplearning4j.text.documentiterator.BasicLabelAwareIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports MutipleEpochsSentenceIterator = org.deeplearning4j.text.sentenceiterator.MutipleEpochsSentenceIterator
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports org.junit.jupiter.api
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.deeplearning4j.models.sequencevectors.transformers.impl.iterables


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class ParallelTransformerIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class ParallelTransformerIteratorTest
		Inherits BaseDL4JTest

		Private factory As TokenizerFactory = New DefaultTokenizerFactory()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void hasNext() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub hasNext()
			Dim iterator As SentenceIterator = New BasicLineIterator(Resources.asFile("big/raw_sentences.txt"))

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iterator).allowMultithreading(True).tokenizerFactory(factory).build()

			Dim iter As IEnumerator(Of Sequence(Of VocabWord)) = transformer.GetEnumerator()
			Dim cnt As Integer = 0
			Dim sequence As Sequence(Of VocabWord) = Nothing
			Do While iter.MoveNext()
				sequence = iter.Current
				assertNotEquals(Nothing, sequence,"Failed on [" & cnt & "] iteration")
				assertNotEquals(0, sequence.size(),"Failed on [" & cnt & "] iteration")
				cnt += 1
			Loop

			'   log.info("Last element: {}", sequence.asLabels());

			assertEquals(97162, cnt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void testSpeedComparison1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSpeedComparison1()
			Dim iterator As SentenceIterator = New MutipleEpochsSentenceIterator(New BasicLineIterator(Resources.asFile("big/raw_sentences.txt")), 25)

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iterator).allowMultithreading(False).tokenizerFactory(factory).build()

			Dim iter As IEnumerator(Of Sequence(Of VocabWord)) = transformer.GetEnumerator()
			Dim cnt As Integer = 0
			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Do While iter.MoveNext()
				Dim sequence As Sequence(Of VocabWord) = iter.Current
				assertNotEquals(Nothing, sequence,"Failed on [" & cnt & "] iteration")
				assertNotEquals(0, sequence.size(),"Failed on [" & cnt & "] iteration")
				cnt += 1
			Loop
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Single-threaded time: {} ms", time2 - time1)
			iterator.reset()

			transformer = (New SentenceTransformer.Builder()).iterator(iterator).allowMultithreading(True).tokenizerFactory(factory).build()

			iter = transformer.GetEnumerator()

			time1 = DateTimeHelper.CurrentUnixTimeMillis()
			Do While iter.MoveNext()
				Dim sequence As Sequence(Of VocabWord) = iter.Current
				assertNotEquals(Nothing, sequence,"Failed on [" & cnt & "] iteration")
				assertNotEquals(0, sequence.size(),"Failed on [" & cnt & "] iteration")
				cnt += 1
			Loop
			time2 = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Multi-threaded time: {} ms", time2 - time1)


			Dim baseIterator As SentenceIterator = iterator
			baseIterator.reset()


			Dim lai As LabelAwareIterator = (New BasicLabelAwareIterator.Builder(New MutipleEpochsSentenceIterator(New BasicLineIterator(Resources.asFile("big/raw_sentences.txt")), 25))).build()

			transformer = (New SentenceTransformer.Builder()).iterator(lai).allowMultithreading(False).tokenizerFactory(factory).build()

			iter = transformer.GetEnumerator()

			time1 = DateTimeHelper.CurrentUnixTimeMillis()
			Do While iter.MoveNext()
				Dim sequence As Sequence(Of VocabWord) = iter.Current
				assertNotEquals(Nothing, sequence, "Failed on [" & cnt & "] iteration")
				assertNotEquals(0, sequence.size(),"Failed on [" & cnt & "] iteration")
				cnt += 1
			Loop
			time2 = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Prefetched Single-threaded time: {} ms", time2 - time1)
			lai.reset()


			transformer = (New SentenceTransformer.Builder()).iterator(lai).allowMultithreading(True).tokenizerFactory(factory).build()

			iter = transformer.GetEnumerator()

			time1 = DateTimeHelper.CurrentUnixTimeMillis()
			Do While iter.MoveNext()
				Dim sequence As Sequence(Of VocabWord) = iter.Current
				assertNotEquals(Nothing, sequence, "Failed on [" & cnt & "] iteration")
				assertNotEquals(0, sequence.size(),"Failed on [" & cnt & "] iteration")
				cnt += 1
			Loop
			time2 = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Prefetched Multi-threaded time: {} ms", time2 - time1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompletes_WhenIteratorHasOneElement() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCompletes_WhenIteratorHasOneElement()

			Dim testString As String = ""
			Dim stringsArray(99) As String
			For i As Integer = 0 To 99
				testString &= Convert.ToString(i) & " "
				stringsArray(i) = Convert.ToString(i)
			Next i
			Dim inputStream As Stream = IOUtils.toInputStream(testString, "UTF-8")
			Dim iterator As SentenceIterator = New BasicLineIterator(inputStream)

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iterator).allowMultithreading(True).tokenizerFactory(factory).build()

			Dim iter As IEnumerator(Of Sequence(Of VocabWord)) = transformer.GetEnumerator()

			Dim sequence As Sequence(Of VocabWord) = Nothing
			Dim cnt As Integer = 0
			Do While iter.MoveNext()
				sequence = iter.Current
				Dim words As IList(Of VocabWord) = sequence.getElements()
				For Each word As VocabWord In words
					assertEquals(stringsArray(cnt), word.Word)
					cnt += 1
				Next word
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void orderIsStableForParallelTokenization() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub orderIsStableForParallelTokenization()

			Dim stringsArray(999) As String
			Dim testStrings As String = ""
			For i As Integer = 0 To 999
				stringsArray(i) = Convert.ToString(i)
				testStrings &= Convert.ToString(i) & vbLf
			Next i
			Dim inputStream As Stream = IOUtils.toInputStream(testStrings, "UTF-8")
			Dim iterator As SentenceIterator = New BasicLineIterator(inputStream)

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iterator).allowMultithreading(True).tokenizerFactory(factory).build()

			Dim iter As IEnumerator(Of Sequence(Of VocabWord)) = transformer.GetEnumerator()

			Dim sequence As Sequence(Of VocabWord) = Nothing
			Dim cnt As Integer = 0
			Do While iter.MoveNext()
				sequence = iter.Current
				Dim words As IList(Of VocabWord) = sequence.getElements()
				For Each word As VocabWord In words
					assertEquals(stringsArray(cnt), word.Word)
					cnt += 1
				Next word
			Loop

		End Sub

	End Class

End Namespace