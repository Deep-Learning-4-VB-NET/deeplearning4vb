Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName

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
'ORIGINAL LINE: @DisplayName("Early Termination Data Set Iterator Test") @NativeTag @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) class EarlyTerminationDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class EarlyTerminationDataSetIteratorTest
		Inherits BaseDL4JTest

		Friend minibatchSize As Integer = 10

		Friend numExamples As Integer = 105



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next And Reset") void testNextAndReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextAndReset()
			Dim terminateAfter As Integer = 2
			Dim iter As DataSetIterator = New MnistDataSetIterator(minibatchSize, numExamples)
			Dim earlyEndIter As New EarlyTerminationDataSetIterator(iter, terminateAfter)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(earlyEndIter.hasNext())
			Dim batchesSeen As Integer = 0
			Dim seenData As IList(Of DataSet) = New List(Of DataSet)()
			Do While earlyEndIter.MoveNext()
				Dim path As DataSet = earlyEndIter.Current
				assertFalse(path Is Nothing)
				seenData.Add(path)
				batchesSeen += 1
			Loop
			assertEquals(batchesSeen, terminateAfter)
			' check data is repeated after reset
			earlyEndIter.reset()
			batchesSeen = 0
			Do While earlyEndIter.MoveNext()
				Dim path As DataSet = earlyEndIter.Current
				assertEquals(seenData(batchesSeen).getFeatures(), path.Features)
				assertEquals(seenData(batchesSeen).getLabels(), path.Labels)
				batchesSeen += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next Num") void testNextNum() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextNum()
			Dim terminateAfter As Integer = 1
			Dim iter As DataSetIterator = New MnistDataSetIterator(minibatchSize, numExamples)
			Dim earlyEndIter As New EarlyTerminationDataSetIterator(iter, terminateAfter)
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
			Dim iter As DataSetIterator = New MnistDataSetIterator(minibatchSize, numExamples)
			Dim earlyEndIter As New EarlyTerminationDataSetIterator(iter, terminateAfter)
			earlyEndIter.next(10)
			iter.reset()
			earlyEndIter.next(10)
			End Sub)

		End Sub
	End Class

End Namespace