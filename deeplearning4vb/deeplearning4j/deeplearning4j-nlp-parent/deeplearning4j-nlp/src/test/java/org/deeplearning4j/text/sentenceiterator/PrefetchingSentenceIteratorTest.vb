Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.text.sentenceiterator



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("Deprecated module") public class PrefetchingSentenceIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class PrefetchingSentenceIteratorTest
		Inherits BaseDL4JTest



		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(PrefetchingSentenceIteratorTest))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHasMoreLinesFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasMoreLinesFile()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iterator As New BasicLineIterator(file)

			Dim fetcher As PrefetchingSentenceIterator = (New PrefetchingSentenceIterator.Builder(iterator)).setFetchSize(1000).build()

			log.info("Phase 1 starting")

			Dim cnt As Integer = 0
			Do While fetcher.hasNext()
				Dim line As String = fetcher.nextSentence()
				'            log.info(line);
				cnt += 1
			Loop


			assertEquals(97162, cnt)

			log.info("Phase 2 starting")
			fetcher.reset()

			cnt = 0
			Do While fetcher.hasNext()
				Dim line As String = fetcher.nextSentence()
				cnt += 1
			Loop

			assertEquals(97162, cnt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLoadedIterator1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLoadedIterator1()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iterator As New BasicLineIterator(file)

			Dim fetcher As PrefetchingSentenceIterator = (New PrefetchingSentenceIterator.Builder(iterator)).setFetchSize(1000).build()

			log.info("Phase 1 starting")

			Dim cnt As Integer = 0
			Do While fetcher.hasNext()
				Dim line As String = fetcher.nextSentence()
				' we'll imitate some workload in current thread by using ThreadSleep.
				' there's no need to keep it enabled forever, just uncomment next line if you're going to test this iterator.
				' otherwise this test will

				'    Thread.sleep(0, 10);

				cnt += 1
				If cnt Mod 10000 = 0 Then
					log.info("Line processed: " & cnt)
				End If
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPerformance1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPerformance1()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")

			Dim iterator As New BasicLineIterator(file)

			Dim fetcher As PrefetchingSentenceIterator = (New PrefetchingSentenceIterator.Builder(New BasicLineIterator(file))).setFetchSize(500000).build()

			Dim time01 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim cnt0 As Integer = 0
			Do While iterator.hasNext()
				iterator.nextSentence()
				cnt0 += 1
			Loop
			Dim time02 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			Dim time11 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim cnt1 As Integer = 0
			Do While fetcher.hasNext()
				fetcher.nextSentence()
				cnt1 += 1
			Loop
			Dim time12 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Basic iterator: " & (time02 - time01))

			log.info("Prefetched iterator: " & (time12 - time11))

			Dim difference As Long = (time12 - time11) - (time02 - time01)
			log.info("Difference: " & difference)

			' on small corpus time difference can fluctuate a lot
			' but it's still can be used as effectiveness measurement
			assertTrue(difference < 150)
		End Sub
	End Class

End Namespace