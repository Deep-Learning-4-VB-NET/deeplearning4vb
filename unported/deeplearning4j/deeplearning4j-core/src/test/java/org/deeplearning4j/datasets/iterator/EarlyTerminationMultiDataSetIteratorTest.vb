Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Tags = org.junit.jupiter.api.Tags
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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
Namespace org.deeplearning4j.datasets.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Early Termination Multi Data Set Iterator Test") @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) class EarlyTerminationMultiDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class EarlyTerminationMultiDataSetIteratorTest
		Inherits BaseDL4JTest

		Friend minibatchSize As Integer = 5

		Friend numExamples As Integer = 105



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next And Reset") void testNextAndReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextAndReset()
			Dim terminateAfter As Integer = 2
			Dim iter As MultiDataSetIterator = New MultiDataSetIteratorAdapter(New MnistDataSetIterator(minibatchSize, numExamples))
			Dim count As Integer = 0
			Dim seenMDS As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
			Do While count < terminateAfter
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				seenMDS.Add(iter.next())
				count += 1
			Loop
			iter.reset()
			Dim earlyEndIter As New EarlyTerminationMultiDataSetIterator(iter, terminateAfter)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(earlyEndIter.hasNext())
			count = 0
			Do While earlyEndIter.MoveNext()
				Dim path As MultiDataSet = earlyEndIter.Current
				assertEquals(path.Features(0), seenMDS(count).getFeatures()(0))
				assertEquals(path.Labels(0), seenMDS(count).getLabels()(0))
				count += 1
			Loop
			assertEquals(count, terminateAfter)
			' check data is repeated
			earlyEndIter.reset()
			count = 0
			Do While earlyEndIter.MoveNext()
				Dim path As MultiDataSet = earlyEndIter.Current
				assertEquals(path.Features(0), seenMDS(count).getFeatures()(0))
				assertEquals(path.Labels(0), seenMDS(count).getLabels()(0))
				count += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next Num") void testNextNum() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextNum()
			Dim terminateAfter As Integer = 1
			Dim iter As MultiDataSetIterator = New MultiDataSetIteratorAdapter(New MnistDataSetIterator(minibatchSize, numExamples))
			Dim earlyEndIter As New EarlyTerminationMultiDataSetIterator(iter, terminateAfter)
			earlyEndIter.next(10)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertEquals(False, earlyEndIter.hasNext())
			earlyEndIter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertEquals(True, earlyEndIter.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test calls to Next Not Allowed") void testCallstoNextNotAllowed() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCallstoNextNotAllowed()
			assertThrows(GetType(Exception),Sub()
			Dim terminateAfter As Integer = 1
			Dim iter As MultiDataSetIterator = New MultiDataSetIteratorAdapter(New MnistDataSetIterator(minibatchSize, numExamples))
			Dim earlyEndIter As New EarlyTerminationMultiDataSetIterator(iter, terminateAfter)
			earlyEndIter.next(10)
			iter.reset()
			earlyEndIter.next(10)
			End Sub)

		End Sub
	End Class

End Namespace