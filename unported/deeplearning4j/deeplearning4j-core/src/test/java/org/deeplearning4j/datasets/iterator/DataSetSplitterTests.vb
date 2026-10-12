Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DataSetGenerator = org.deeplearning4j.datasets.iterator.tools.DataSetGenerator
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class DataSetSplitterTests extends org.deeplearning4j.BaseDL4JTest
	Public Class DataSetSplitterTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSplitter_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSplitter_1()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			Dim splitter As val = New DataSetIteratorSplitter(back, 1000, 0.7)

			Dim train As val = splitter.getTrainIterator()
			Dim test As val = splitter.getTestIterator()
			Dim numEpochs As val = 10

			Dim gcntTrain As Integer = 0
			Dim gcntTest As Integer = 0
			Dim [global] As Integer = 0
			' emulating epochs here
			For e As Integer = 0 To numEpochs - 1
				Dim cnt As Integer = 0
				Do While train.hasNext()
					Dim data As val = train.next().getFeatures()

'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertEquals((float) cnt++, data.getFloat(0), 1e-5,"Train failed on iteration " + cnt + "; epoch: " + e);
					assertEquals(CSng(cnt)++, data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
					gcntTrain += 1
					[global] += 1
				Loop

				train.reset()


				Do While test.hasNext()
					Dim data As val = test.next().getFeatures()
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertEquals((float) cnt++, data.getFloat(0), 1e-5,"Train failed on iteration " + cnt + "; epoch: " + e);
					assertEquals(CSng(cnt)++, data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
					gcntTest += 1
					[global] += 1
				Loop

				test.reset()
			Next e

			assertEquals(1000 * numEpochs, [global])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSplitter_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSplitter_2()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			Dim splitter As val = New DataSetIteratorSplitter(back, 1000, 0.7)

			Dim train As val = splitter.getTrainIterator()
			Dim test As val = splitter.getTestIterator()
			Dim numEpochs As val = 10

			Dim gcntTrain As Integer = 0
			Dim gcntTest As Integer = 0
			Dim [global] As Integer = 0
			' emulating epochs here
			For e As Integer = 0 To numEpochs - 1
				Dim cnt As Integer = 0
				Do While train.hasNext()
					Dim data As val = train.next().getFeatures()

'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertEquals((float) cnt++, data.getFloat(0), 1e-5,"Train failed on iteration " + cnt + "; epoch: " + e);
					assertEquals(CSng(cnt)++, data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
					gcntTrain += 1
					[global] += 1
				Loop

				train.reset()

				If e Mod 2 = 0 Then
					Do While test.hasNext()
						Dim data As val = test.next().getFeatures()
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertEquals((float) cnt++, data.getFloat(0), 1e-5,"Train failed on iteration " + cnt + "; epoch: " + e);
						assertEquals(CSng(cnt)++, data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
						gcntTest += 1
						[global] += 1
					Loop
				End If
			Next e

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			assertEquals(700 * numEpochs + (300 * numEpochs / 2), [global])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testSplitter_3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSplitter_3()
		   assertThrows(GetType(ND4JIllegalStateException), Sub()
		   Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})
		   Dim splitter As val = New DataSetIteratorSplitter(back, 1000, 0.7)
		   Dim train As val = splitter.getTrainIterator()
		   Dim test As val = splitter.getTestIterator()
		   Dim numEpochs As val = 10
		   Dim gcntTrain As Integer = 0
		   Dim gcntTest As Integer = 0
		   Dim [global] As Integer = 0
		   For e As Integer = 0 To numEpochs - 1
			   Dim cnt As Integer = 0
			   Do While train.hasNext()
				   Dim data As val = train.next().getFeatures()
				   assertEquals(CSng(cnt)++, data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
				   gcntTrain += 1
				   [global] += 1
			   Loop
			   train.reset()
			   Do While test.hasNext()
				   Dim data As val = test.next().getFeatures()
				   assertEquals(CSng(cnt)++, data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
				   gcntTest += 1
				   [global] += 1
			   Loop
			   train.hasNext()
			   back.shift()
		   Next e
		   assertEquals(1000 * numEpochs, [global])
		   End Sub)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSplitter_4()
		Public Overridable Sub testSplitter_4()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			Dim splitter As val = New DataSetIteratorSplitter(back, 1000, New Double(){0.5, 0.3, 0.2})
			Dim iteratorList As IList(Of DataSetIterator) = splitter.getIterators()
			Dim numEpochs As val = 10
			Dim [global] As Integer = 0
			' emulating epochs here
			For e As Integer = 0 To numEpochs - 1
				Dim iterNo As Integer = 0
				Dim perEpoch As Integer = 0
				For Each partIterator As val In iteratorList
					Dim cnt As Integer = 0
					partIterator.reset()
					Do While partIterator.hasNext()
						Dim data As val = partIterator.next().getFeatures()
						assertEquals(CSng(perEpoch), data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
						'gcntTrain++;
						[global] += 1
						cnt += 1
						perEpoch += 1
					Loop
					iterNo += 1
				Next partIterator
			Next e

			assertEquals(1000* numEpochs, [global])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSplitter_5()
		Public Overridable Sub testSplitter_5()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			Dim splitter As val = New DataSetIteratorSplitter(back, New Integer(){900, 100})

			Dim iteratorList As IList(Of DataSetIterator) = splitter.getIterators()
			Dim numEpochs As val = 10

			Dim [global] As Integer = 0
			' emulating epochs here
			For e As Integer = 0 To numEpochs - 1
				Dim iterNo As Integer = 0
				Dim perEpoch As Integer = 0
				For Each partIterator As val In iteratorList
					partIterator.reset()
					Do While partIterator.hasNext()
						Dim cnt As Integer = 0
						Dim data As val = partIterator.next().getFeatures()

						assertEquals(CSng(perEpoch), data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
						'gcntTrain++;
						[global] += 1
						cnt += 1
						perEpoch += 1
					Loop
					iterNo += 1
				Next partIterator
			Next e

			assertEquals(1000 * numEpochs, [global])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSplitter_6()
		Public Overridable Sub testSplitter_6()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			' we're going to mimic train+test+validation split
			Dim splitter As val = New DataSetIteratorSplitter(back, New Integer(){800, 100, 100})

			assertEquals(3, splitter.getIterators().size())

			Dim trainIter As val = splitter.getIterators().get(0)
			Dim testIter As val = splitter.getIterators().get(1)
			Dim validationIter As val = splitter.getIterators().get(2)

			' we're going to have multiple epochs
			Dim numEpochs As Integer = 10
			For e As Integer = 0 To numEpochs - 1
				Dim globalIter As Integer = 0
				trainIter.reset()
				testIter.reset()
				validationIter.reset()

				Dim trained As Boolean = False
				Do While trainIter.hasNext()
					trained = True
					Dim ds As val = trainIter.next()
					assertNotNull(ds)

					assertEquals(globalIter, ds.getFeatures().getDouble(0), 1e-5f,"Failed at iteration [" & globalIter & "]")
					globalIter += 1
				Loop
				assertTrue(trained,"Failed at epoch [" & e & "]")
				assertEquals(800, globalIter)


				' test set is used every epoch
				Dim tested As Boolean = False
				'testIter.reset();
				Do While testIter.hasNext()
					tested = True
					Dim ds As val = testIter.next()
					assertNotNull(ds)

					assertEquals(globalIter, ds.getFeatures().getDouble(0), 1e-5f,"Failed at iteration [" & globalIter & "]")
					globalIter += 1
				Loop
				assertTrue(tested,"Failed at epoch [" & e & "]")
				assertEquals(900, globalIter)

				' validation set is used every 5 epochs
				If e Mod 5 = 0 Then
					Dim validated As Boolean = False
					'validationIter.reset();
					Do While validationIter.hasNext()
						validated = True
						Dim ds As val = validationIter.next()
						assertNotNull(ds)

						assertEquals(globalIter, ds.getFeatures().getDouble(0), 1e-5f,"Failed at iteration [" & globalIter & "]")
						globalIter += 1
					Loop
					assertTrue(validated,"Failed at epoch [" & e & "]")
				End If

				' all 3 iterators have exactly 1000 elements combined
				If e Mod 5 = 0 Then
					assertEquals(1000, globalIter)
				Else
					assertEquals(900, globalIter)
				End If
				trainIter.reset()
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUnorderedSplitter_1()
		Public Overridable Sub testUnorderedSplitter_1()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			Dim splitter As val = New DataSetIteratorSplitter(back, New Integer(){500, 500})

			Dim iteratorList As IList(Of DataSetIterator) = splitter.getIterators()
			Dim numEpochs As val = 10

			Dim [global] As Integer = 0
			' emulating epochs here
			For e As Integer = 0 To numEpochs - 1

				' Get data from second part, then rewind for the first one.
				Dim cnt As Integer = 0
				Dim partNumber As Integer = 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While iteratorList(partNumber).hasNext()
					Dim farCnt As Integer = (1000 \ 2) * (partNumber) + cnt
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim data As val = iteratorList(partNumber).next().getFeatures()

					assertEquals(CSng(farCnt), data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
					cnt += 1
					[global] += 1
				Loop
				iteratorList(partNumber).reset()
				partNumber = 0
				cnt = 0
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While iteratorList(0).hasNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim data As val = iteratorList(0).next().getFeatures()

'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertEquals((float) cnt++, data.getFloat(0), 1e-5,"Train failed on iteration " + cnt + "; epoch: " + e);
					assertEquals(CSng(cnt)++, data.getFloat(0), 1e-5,"Train failed on iteration " & cnt & "; epoch: " & e)
					[global] += 1
				Loop
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUnorderedSplitter_2()
		Public Overridable Sub testUnorderedSplitter_2()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			Dim splitter As val = New DataSetIteratorSplitter(back, New Integer(){2})

			Dim iteratorList As IList(Of DataSetIterator) = splitter.getIterators()

			For partNumber As Integer = 0 To iteratorList.Count - 1
				Dim cnt As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While iteratorList(partNumber).hasNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim data As val = iteratorList(partNumber).next().getFeatures()

					assertEquals(CSng(500*partNumber + cnt), data.getFloat(0), 1e-5,"Train failed on iteration " & cnt)
					cnt += 1
				Loop
			Next partNumber
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUnorderedSplitter_3()
		Public Overridable Sub testUnorderedSplitter_3()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			Dim splitter As val = New DataSetIteratorSplitter(back, New Integer(){10})

			Dim iteratorList As IList(Of DataSetIterator) = splitter.getIterators()
			Dim random As New Random()
			Dim indexes(iteratorList.Count - 1) As Integer
			For i As Integer = 0 To indexes.Length - 1
				indexes(i) = random.Next(iteratorList.Count)
			Next i

			For Each partNumber As Integer In indexes
				Dim cnt As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While iteratorList(partNumber).hasNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim data As val = iteratorList(partNumber).next().getFeatures()

					assertEquals(CSng(500*partNumber + cnt), data.getFloat(0), 1e-5,"Train failed on iteration " & cnt)
					cnt += 1
				Loop
			Next partNumber
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUnorderedSplitter_4()
		Public Overridable Sub testUnorderedSplitter_4()
			Dim back As val = New DataSetGenerator(1000, New Integer(){32, 100}, New Integer(){32, 5})

			' we're going to mimic train+test+validation split
			Dim splitter As val = New DataSetIteratorSplitter(back, New Integer(){80, 10, 5})

			assertEquals(3, splitter.getIterators().size())

			Dim trainIter As val = splitter.getIterators().get(0) ' 0..79
			Dim testIter As val = splitter.getIterators().get(1) ' 80 ..89
			Dim validationIter As val = splitter.getIterators().get(2) ' 90..94

			' we're skipping train/test and go for validation first. we're that crazy, right.
			Dim valCnt As Integer = 0
			Do While validationIter.hasNext()
				Dim ds As val = validationIter.next()
				assertNotNull(ds)

				assertEquals(CSng(valCnt) + 90, ds.getFeatures().getFloat(0), 1e-5,"Validation failed on iteration " & valCnt)
				valCnt += 1
			Loop
			assertEquals(5, valCnt)
		End Sub
	End Class

End Namespace