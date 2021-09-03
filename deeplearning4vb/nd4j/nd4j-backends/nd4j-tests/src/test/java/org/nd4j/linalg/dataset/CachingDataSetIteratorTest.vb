Imports System
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports CachingDataSetIterator = org.nd4j.linalg.dataset.api.iterator.CachingDataSetIterator
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports SamplingDataSetIterator = org.nd4j.linalg.dataset.api.iterator.SamplingDataSetIterator
Imports DataSetCache = org.nd4j.linalg.dataset.api.iterator.cache.DataSetCache
Imports InFileDataSetCache = org.nd4j.linalg.dataset.api.iterator.cache.InFileDataSetCache
Imports InMemoryDataSetCache = org.nd4j.linalg.dataset.api.iterator.cache.InMemoryDataSetCache
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.dataset



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class CachingDataSetIteratorTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CachingDataSetIteratorTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInMemory(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInMemory(ByVal backend As Nd4jBackend)
			Dim cache As DataSetCache = New InMemoryDataSetCache()

			runDataSetTest(cache)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInFile() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInFile()
			Dim cacheDir As Path = Files.createTempDirectory("nd4j-data-set-cache-test")
			Dim cache As DataSetCache = New InFileDataSetCache(cacheDir)

			runDataSetTest(cache)

			FileUtils.deleteDirectory(cacheDir.toFile())
		End Sub

		Private Sub runDataSetTest(ByVal cache As DataSetCache)
			Dim rows As Integer = 500
			Dim inputColumns As Integer = 100
			Dim outputColumns As Integer = 2
'JAVA TO VB CONVERTER NOTE: The variable dataSet was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim dataSet_Conflict As New DataSet(Nd4j.ones(rows, inputColumns), Nd4j.zeros(rows, outputColumns))

			Dim batchSize As Integer = 10
			Dim totalNumberOfSamples As Integer = 50
			Dim expectedNumberOfDataSets As Integer = totalNumberOfSamples \ batchSize
			Dim it As DataSetIterator = New SamplingDataSetIterator(dataSet_Conflict, batchSize, totalNumberOfSamples)

			Dim [namespace] As String = "test-namespace"

			Dim cachedIt As New CachingDataSetIterator(it, cache, [namespace])
			Dim preProcessor As New PreProcessor(Me)
			cachedIt.PreProcessor = preProcessor

			assertDataSetCacheGetsCompleted(cache, [namespace], cachedIt)

			assertPreProcessingGetsCached(expectedNumberOfDataSets, it, cachedIt, preProcessor)

			assertCachingDataSetIteratorHasAllTheData(rows, inputColumns, outputColumns, dataSet_Conflict, it, cachedIt)
		End Sub

		Private Sub assertDataSetCacheGetsCompleted(ByVal cache As DataSetCache, ByVal [namespace] As String, ByVal cachedIt As DataSetIterator)
			Do While cachedIt.MoveNext()
				assertFalse(cache.isComplete([namespace]))
				cachedIt.Current
			Loop

			assertTrue(cache.isComplete([namespace]))
		End Sub

		Private Sub assertPreProcessingGetsCached(ByVal expectedNumberOfDataSets As Integer, ByVal it As DataSetIterator, ByVal cachedIt As CachingDataSetIterator, ByVal preProcessor As PreProcessor)

			assertSame(preProcessor, cachedIt.PreProcessor)
			assertSame(preProcessor, it.PreProcessor)

			cachedIt.reset()
			it.reset()

			Do While cachedIt.MoveNext()
				cachedIt.Current
			Loop

			assertEquals(expectedNumberOfDataSets, preProcessor.CallCount)

			cachedIt.reset()
			it.reset()

			Do While cachedIt.MoveNext()
				cachedIt.Current
			Loop

			assertEquals(expectedNumberOfDataSets, preProcessor.CallCount)
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter dataSet was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Sub assertCachingDataSetIteratorHasAllTheData(ByVal rows As Integer, ByVal inputColumns As Integer, ByVal outputColumns As Integer, ByVal dataSet_Conflict As DataSet, ByVal it As DataSetIterator, ByVal cachedIt As CachingDataSetIterator)
			cachedIt.reset()
			it.reset()

			dataSet_Conflict.Features = Nd4j.zeros(rows, inputColumns)
			dataSet_Conflict.Labels = Nd4j.ones(rows, outputColumns)

			Do While it.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(cachedIt.hasNext())

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim cachedDs As DataSet = cachedIt.next()
				assertEquals(1000.0, cachedDs.Features.sumNumber())
				assertEquals(0.0, cachedDs.Labels.sumNumber())

				Dim ds As DataSet = it.Current
				assertEquals(0.0, ds.Features.sumNumber())
				assertEquals(20.0, ds.Labels.sumNumber())
			Loop

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(cachedIt.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(it.hasNext())
		End Sub

		<Serializable>
		Private Class PreProcessor
			Implements DataSetPreProcessor

			Private ReadOnly outerInstance As CachingDataSetIteratorTest

			Public Sub New(ByVal outerInstance As CachingDataSetIteratorTest)
				Me.outerInstance = outerInstance
			End Sub


'JAVA TO VB CONVERTER NOTE: The field callCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend callCount_Conflict As Integer

			Public Overridable Sub preProcess(ByVal toPreProcess As org.nd4j.linalg.dataset.api.DataSet)
				callCount_Conflict += 1
			End Sub

			Public Overridable ReadOnly Property CallCount As Integer
				Get
					Return callCount_Conflict
				End Get
			End Property
		End Class
	End Class

End Namespace