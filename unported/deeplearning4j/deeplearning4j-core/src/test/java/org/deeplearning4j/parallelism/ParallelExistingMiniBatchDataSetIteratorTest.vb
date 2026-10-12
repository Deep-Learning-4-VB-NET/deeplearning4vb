Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DataSetDeserializer = org.deeplearning4j.datasets.iterator.callbacks.DataSetDeserializer
Imports FileSplitParallelDataSetIterator = org.deeplearning4j.datasets.iterator.parallel.FileSplitParallelDataSetIterator
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
Imports DisplayName = org.junit.jupiter.api.DisplayName
import static org.junit.jupiter.api.Assertions.assertTimeout
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.parallelism

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Parallel Existing Mini Batch Data Set Iterator Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class ParallelExistingMiniBatchDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ParallelExistingMiniBatchDataSetIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path tempDir;
		Public tempDir As Path

		Private Shared rootFolder As File

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub setUp()
			If rootFolder Is Nothing Then
				rootFolder = tempDir.toFile()
				For i As Integer = 0 To 25
					Call (New ClassPathResource("/datasets/mnist/mnist-train-" & i & ".bin")).getTempFileFromArchive(rootFolder)
				Next i
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test New Simple Loop 1") void testNewSimpleLoop1()
		Friend Overridable Sub testNewSimpleLoop1()
			assertTimeout(ofMillis(30000), Sub()
			Dim fspdsi As New FileSplitParallelDataSetIterator(rootFolder, "mnist-train-%d.bin", New DataSetDeserializer())
			Dim pairs As IList(Of Pair(Of Long, Long)) = New List(Of Pair(Of Long, Long))()
			Dim time1 As Long = System.nanoTime()
			Dim cnt As Integer = 0
			Do While fspdsi.MoveNext()
				Dim ds As DataSet = fspdsi.Current
				Dim time2 As Long = System.nanoTime()
				pairs.Add(New Pair(Of Long, Long)(time2 - time1, 0L))
				assertNotNull(ds)
				Thread.Sleep(10)
				cnt += 1
				time1 = System.nanoTime()
			Loop
			assertEquals(26, cnt)
			For Each times As Pair(Of Long, Long) In pairs
				log.info("Parallel: {} ns; Simple: {} ns", times.First, times.Second)
			Next times
			End Sub)
		End Sub
	'    
	'    @Test
	'    public void testSimpleLoop1() throws Exception {
	'        ParallelExistingMiniBatchDataSetIterator iterator = new ParallelExistingMiniBatchDataSetIterator(rootFolder,"mnist-train-%d.bin", 4);
	'        ExistingMiniBatchDataSetIterator test = new ExistingMiniBatchDataSetIterator(rootFolder,"mnist-train-%d.bin");
	'    
	'    
	'        List<Pair<Long, Long>> pairs = new ArrayList<>();
	'    
	'        int cnt = 0;
	'        long time1 = System.nanoTime();
	'        while (iterator.hasNext()) {
	'            DataSet ds = iterator.next();
	'            long time2 = System.nanoTime();
	'            assertNotNull(ds);
	'            assertEquals(64, ds.numExamples());
	'            pairs.add(new Pair<Long, Long>(time2 - time1, 0L));
	'            cnt++;
	'            time1 = System.nanoTime();
	'        }
	'        assertEquals(26, cnt);
	'    
	'        cnt = 0;
	'        time1 = System.nanoTime();
	'        while (test.hasNext()) {
	'            DataSet ds = test.next();
	'            long time2 = System.nanoTime();
	'            assertNotNull(ds);
	'            assertEquals(64, ds.numExamples());
	'            pairs.get(cnt).setSecond(time2 - time1);
	'            cnt++;
	'            time1 = System.nanoTime();
	'        }
	'    
	'        assertEquals(26, cnt);
	'    
	'        for (Pair<Long, Long> times: pairs) {
	'            log.info("Parallel: {} ns; Simple: {} ns", times.getFirst(), times.getSecond());
	'        }
	'    }
	'    
	'    @Test
	'    public void testReset1() throws Exception {
	'        ParallelExistingMiniBatchDataSetIterator iterator = new ParallelExistingMiniBatchDataSetIterator(rootFolder,"mnist-train-%d.bin", 8);
	'    
	'        int cnt = 0;
	'        long time1 = System.nanoTime();
	'        while (iterator.hasNext()) {
	'            DataSet ds = iterator.next();
	'            long time2 = System.nanoTime();
	'            assertNotNull(ds);
	'            assertEquals(64, ds.numExamples());
	'            cnt++;
	'    
	'            if (cnt == 10)
	'                iterator.reset();
	'    
	'            time1 = System.nanoTime();
	'        }
	'        assertEquals(36, cnt);
	'    }
	'    
	'    @Test
	'    public void testWithAdsi1() throws Exception {
	'        ParallelExistingMiniBatchDataSetIterator iterator = new ParallelExistingMiniBatchDataSetIterator(rootFolder,"mnist-train-%d.bin", 8);
	'        AsyncDataSetIterator adsi = new AsyncDataSetIterator(iterator, 8, true);
	'    
	'        int cnt = 0;
	'        long time1 = System.nanoTime();
	'        while (adsi.hasNext()) {
	'            DataSet ds = adsi.next();
	'            long time2 = System.nanoTime();
	'            assertNotNull(ds);
	'            assertEquals(64, ds.numExamples());
	'            cnt++;
	'    
	'            if (cnt == 10)
	'                adsi.reset();
	'    
	'            time1 = System.nanoTime();
	'        }
	'        assertEquals(36, cnt);
	'    }
	'    
	End Class

End Namespace