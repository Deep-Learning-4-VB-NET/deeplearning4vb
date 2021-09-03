Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports MnistFetcher = org.deeplearning4j.datasets.base.MnistFetcher
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
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
Namespace org.deeplearning4j.datasets


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Mnist Fetcher Test") @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_ETL) class MnistFetcherTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class MnistFetcherTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public static java.nio.file.Path tempPath;
		Public Shared tempPath As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll static void setup() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Shared Sub setup()
			DL4JResources.setBaseDirectory(tempPath.toFile())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterAll static void after() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Shared Sub after()
			DL4JResources.resetBaseDirectoryLocation()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Mnist") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.FILE_IO) void testMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMnist()
			Dim iter As New MnistDataSetIterator(32, 60000, False, True, False, -1)
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				Dim arr As INDArray = ds.Features.sum(1)
				Dim countMatch As Integer = Nd4j.Executioner.execAndReturn(New MatchCondition(arr, Conditions.equals(0))).z().getInt(0)
				assertEquals(0, countMatch)
				count += 1
			Loop
			assertEquals(60000 \ 32, count)
			count = 0
			iter = New MnistDataSetIterator(32, False, 12345)
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				Dim arr As INDArray = ds.Features.sum(1)
				Dim countMatch As Integer = Nd4j.Executioner.execAndReturn(New MatchCondition(arr, Conditions.equals(0))).z().getInt(0)
				assertEquals(0, countMatch)
				count += 1
			Loop
			assertEquals(CInt(Math.Truncate(Math.Ceiling(10000 / 32.0))), count)
			iter.close()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Mnist Data Fetcher") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.FILE_IO) @Disabled("Temp directory not being set properly on CI") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) void testMnistDataFetcher() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMnistDataFetcher()
			Dim mnistFetcher As New MnistFetcher()
			Dim mnistDir As File = mnistFetcher.downloadAndUntar()
			assertTrue(mnistDir.isDirectory())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.FILE_IO) @Disabled("Temp directory not being set properly on CI") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testMnistSubset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMnistSubset()
			Const numExamples As Integer = 100
			Dim iter1 As New MnistDataSetIterator(10, numExamples, False, True, True, 123)
			Dim examples1 As Integer = 0
			Dim itCount1 As Integer = 0
			Do While iter1.MoveNext()
				itCount1 += 1
				examples1 += iter1.Current.numExamples()
			Loop
			assertEquals(10, itCount1)
			assertEquals(100, examples1)
			iter1.close()
			Dim iter2 As New MnistDataSetIterator(10, numExamples, False, True, True, 123)
			iter2.close()
			Dim examples2 As Integer = 0
			Dim itCount2 As Integer = 0
			For i As Integer = 0 To 9
				itCount2 += 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				examples2 += iter2.next().numExamples()
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(iter2.hasNext())
			assertEquals(10, itCount2)
			assertEquals(100, examples2)
			Dim iter3 As New MnistDataSetIterator(19, numExamples, False, True, True, 123)
			iter3.close()
			Dim examples3 As Integer = 0
			Dim itCount3 As Integer = 0
			Do While iter3.MoveNext()
				itCount3 += 1
				examples3 += iter3.Current.numExamples()
			Loop
			assertEquals(100, examples3)
			assertEquals(CInt(Math.Truncate(Math.Ceiling(100 / 19.0))), itCount3)
			Dim iter4 As New MnistDataSetIterator(32, True, 12345)
			Dim count4 As Integer = 0
			Do While iter4.MoveNext()
				count4 += iter4.Current.numExamples()
			Loop
			assertEquals(60000, count4)
			iter4.close()
			iter1.close()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Subset Repeatability") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.FILE_IO) @Disabled("Temp directory not being set properly on CI") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) void testSubsetRepeatability() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSubsetRepeatability()
			Dim it As New MnistDataSetIterator(1, 1, False, False, True, 0)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim d1 As DataSet = it.next()
			For i As Integer = 0 To 9
				it.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim d2 As DataSet = it.next()
				assertEquals(d1.get(0).getFeatures(), d2.get(0).getFeatures())
			Next i
			it.close()
			' Check larger number:
			it = New MnistDataSetIterator(8, 32, False, False, True, 12345)
			Dim featureLabelSet As ISet(Of String) = New HashSet(Of String)()
			Do While it.MoveNext()
				Dim ds As DataSet = it.Current
				Dim f As INDArray = ds.Features
				Dim l As INDArray = ds.Labels
				Dim i As Integer = 0
				Do While i < f.size(0)
					featureLabelSet.Add(f.getRow(i).ToString() & vbTab & l.getRow(i).ToString())
					i += 1
				Loop
			Loop
			assertEquals(32, featureLabelSet.Count)
			it.close()
			For i As Integer = 0 To 2
				it.reset()
				Dim flSet2 As ISet(Of String) = New HashSet(Of String)()
				Do While it.MoveNext()
					Dim ds As DataSet = it.Current
					Dim f As INDArray = ds.Features
					Dim l As INDArray = ds.Labels
					Dim j As Integer = 0
					Do While j < f.size(0)
						flSet2.Add(f.getRow(j).ToString() & vbTab & l.getRow(j).ToString())
						j += 1
					Loop
				Loop
				assertEquals(featureLabelSet, flSet2)
			Next i

		End Sub
	End Class

End Namespace