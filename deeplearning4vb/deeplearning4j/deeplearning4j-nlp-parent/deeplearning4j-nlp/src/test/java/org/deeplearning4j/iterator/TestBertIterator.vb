Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports Getter = lombok.Getter
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BertMaskedLMMasker = org.deeplearning4j.iterator.bert.BertMaskedLMMasker
Imports CollectionLabeledPairSentenceProvider = org.deeplearning4j.iterator.provider.CollectionLabeledPairSentenceProvider
Imports CollectionLabeledSentenceProvider = org.deeplearning4j.iterator.provider.CollectionLabeledSentenceProvider
Imports BertWordPieceTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.BertWordPieceTokenizerFactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports Resources = org.nd4j.common.resources.Resources
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

Namespace org.deeplearning4j.iterator



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class TestBertIterator extends org.deeplearning4j.BaseDL4JTest
	Public Class TestBertIterator
		Inherits BaseDL4JTest

		Private Shared pathToVocab As File = Resources.asFile("other/vocab.txt")
		Private Shared c As Charset = StandardCharsets.UTF_8
		Private Shared shortSentence As String = "I saw a girl with a telescope."
		Private Shared longSentence As String = "Donaudampfschifffahrts Kapitänsmützeninnenfuttersaum"
		Private Shared sentenceA As String = "Goodnight noises everywhere"
		Private Shared sentenceB As String = "Goodnight moon"


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testBertSequenceClassification() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertSequenceClassification()
			If Platform.isWindows() Then
				Return
			End If
			Dim minibatchSize As Integer = 2
			Dim testHelper As New TestSentenceHelper()
			Dim b As BertIterator = BertIterator.builder().tokenizer(testHelper.getTokenizer()).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, 16).minibatchSize(minibatchSize).sentenceProvider(testHelper.getSentenceProvider()).featureArrays(BertIterator.FeatureArrays.INDICES_MASK).vocabMap(testHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).build()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = b.next()
			assertEquals(1, mds.Features.Length)
			Console.WriteLine(mds.getFeatures(0))
			Console.WriteLine(mds.getFeaturesMaskArray(0))

			Dim expF As INDArray = Nd4j.create(DataType.INT, 1, 16)
			Dim expM As INDArray = Nd4j.create(DataType.INT, 1, 16)
			Dim m As IDictionary(Of String, Integer) = testHelper.getTokenizer().getVocab()
			For i As Integer = 0 To minibatchSize - 1
				Dim expFTemp As INDArray = Nd4j.create(DataType.INT, 1, 16)
				Dim expMTemp As INDArray = Nd4j.create(DataType.INT, 1, 16)
				Dim tokens As IList(Of String) = testHelper.getTokenizedSentences().get(i)
				Console.WriteLine(tokens)
				For j As Integer = 0 To tokens.Count - 1
					Dim token As String = tokens(j)
					If Not m.ContainsKey(token) Then
						Throw New System.InvalidOperationException("Unknown token: """ & token & """")
					End If
					Dim idx As Integer = m(token)
					expFTemp.putScalar(0, j, idx)
					expMTemp.putScalar(0, j, 1)
				Next j
				If i = 0 Then
					expF = expFTemp.dup()
					expM = expMTemp.dup()
				Else
					expF = Nd4j.vstack(expF, expFTemp)
					expM = Nd4j.vstack(expM, expMTemp)
				End If
			Next i
			assertEquals(expF, mds.getFeatures(0))
			assertEquals(expM, mds.getFeaturesMaskArray(0))
			assertEquals(expF, b.featurizeSentences(testHelper.getSentences()).First(0))
			assertEquals(expM, b.featurizeSentences(testHelper.getSentences()).Second(0))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(b.hasNext())
			b.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(b.hasNext())

			'Same thing, but with segment ID also
			b = BertIterator.builder().tokenizer(testHelper.getTokenizer()).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, 16).minibatchSize(minibatchSize).sentenceProvider(testHelper.getSentenceProvider()).featureArrays(BertIterator.FeatureArrays.INDICES_MASK_SEGMENTID).vocabMap(testHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).build()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			mds = b.next()
			assertEquals(2, mds.Features.Length)
			'Segment ID should be all 0s for single segment task
			Dim segmentId As INDArray = expM.like()
			assertEquals(segmentId, mds.getFeatures(1))
			assertEquals(segmentId, b.featurizeSentences(testHelper.getSentences()).First(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000) public void testBertUnsupervised() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertUnsupervised()
			Dim minibatchSize As Integer = 2
			Dim testHelper As New TestSentenceHelper()
			'Task 1: Unsupervised
			Dim b As BertIterator = BertIterator.builder().tokenizer(testHelper.getTokenizer()).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, 16).minibatchSize(minibatchSize).sentenceProvider(testHelper.getSentenceProvider()).featureArrays(BertIterator.FeatureArrays.INDICES_MASK).vocabMap(testHelper.getTokenizer().getVocab()).task(BertIterator.Task.UNSUPERVISED).masker(New BertMaskedLMMasker(New Random(12345), 0.2, 0.5, 0.5)).unsupervisedLabelFormat(BertIterator.UnsupervisedLabelFormat.RANK2_IDX).maskToken("[MASK]").build()

			Console.WriteLine("Mask token index: " & testHelper.getTokenizer().getVocab().get("[MASK]"))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = b.next()
			Console.WriteLine(mds.getFeatures(0))
			Console.WriteLine(mds.getFeaturesMaskArray(0))
			Console.WriteLine(mds.getLabels(0))
			Console.WriteLine(mds.getLabelsMaskArray(0))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(b.hasNext())
			b.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(b.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000) public void testLengthHandling() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLengthHandling()
			Dim minibatchSize As Integer = 2
			Dim testHelper As New TestSentenceHelper()
			Dim expF As INDArray = Nd4j.create(DataType.INT, 1, 16)
			Dim expM As INDArray = Nd4j.create(DataType.INT, 1, 16)
			Dim m As IDictionary(Of String, Integer) = testHelper.getTokenizer().getVocab()
			For i As Integer = 0 To minibatchSize - 1
				Dim tokens As IList(Of String) = testHelper.getTokenizedSentences().get(i)
				Dim expFTemp As INDArray = Nd4j.create(DataType.INT, 1, 16)
				Dim expMTemp As INDArray = Nd4j.create(DataType.INT, 1, 16)
				Console.WriteLine(tokens)
				For j As Integer = 0 To tokens.Count - 1
					Dim token As String = tokens(j)
					If Not m.ContainsKey(token) Then
						Throw New System.InvalidOperationException("Unknown token: """ & token & """")
					End If
					Dim idx As Integer = m(token)
					expFTemp.putScalar(0, j, idx)
					expMTemp.putScalar(0, j, 1)
				Next j
				If i = 0 Then
					expF = expFTemp.dup()
					expM = expMTemp.dup()
				Else
					expF = Nd4j.vstack(expF, expFTemp)
					expM = Nd4j.vstack(expM, expMTemp)
				End If
			Next i

			'--------------------------------------------------------------

			'Fixed length: clip or pad - already tested in other tests

			'Any length: as long as we need to fit longest sequence

			Dim b As BertIterator = BertIterator.builder().tokenizer(testHelper.getTokenizer()).lengthHandling(BertIterator.LengthHandling.ANY_LENGTH, -1).minibatchSize(minibatchSize).sentenceProvider(testHelper.getSentenceProvider()).featureArrays(BertIterator.FeatureArrays.INDICES_MASK).vocabMap(testHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).build()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = b.next()
			Dim expShape() As Long = {2, 14}
			assertArrayEquals(expShape, mds.getFeatures(0).shape())
			assertArrayEquals(expShape, mds.getFeaturesMaskArray(0).shape())
			assertEquals(expF.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 14)), mds.getFeatures(0))
			assertEquals(expM.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 14)), mds.getFeaturesMaskArray(0))
			assertEquals(mds.getFeatures(0), b.featurizeSentences(testHelper.getSentences()).First(0))
			assertEquals(mds.getFeaturesMaskArray(0), b.featurizeSentences(testHelper.getSentences()).Second(0))

			'Clip only: clip to maximum, but don't pad if less
			b = BertIterator.builder().tokenizer(testHelper.getTokenizer()).lengthHandling(BertIterator.LengthHandling.CLIP_ONLY, 20).minibatchSize(minibatchSize).sentenceProvider(testHelper.getSentenceProvider()).featureArrays(BertIterator.FeatureArrays.INDICES_MASK).vocabMap(testHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).build()
			expShape = New Long(){2, 14}
			assertArrayEquals(expShape, mds.getFeatures(0).shape())
			assertArrayEquals(expShape, mds.getFeaturesMaskArray(0).shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000) public void testMinibatchPadding() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMinibatchPadding()
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Dim minibatchSize As Integer = 3
			Dim testHelper As New TestSentenceHelper(minibatchSize)
			Dim zeros As INDArray = Nd4j.create(DataType.INT, 1, 16)
			Dim expF As INDArray = Nd4j.create(DataType.INT, 1, 16)
			Dim expM As INDArray = Nd4j.create(DataType.INT, 1, 16)
			Dim m As IDictionary(Of String, Integer) = testHelper.getTokenizer().getVocab()
			For i As Integer = 0 To minibatchSize - 1
				Dim tokens As IList(Of String) = testHelper.getTokenizedSentences().get(i)
				Dim expFTemp As INDArray = Nd4j.create(DataType.INT, 1, 16)
				Dim expMTemp As INDArray = Nd4j.create(DataType.INT, 1, 16)
				Console.WriteLine(tokens)
				For j As Integer = 0 To tokens.Count - 1
					Dim token As String = tokens(j)
					If Not m.ContainsKey(token) Then
						Throw New System.InvalidOperationException("Unknown token: """ & token & """")
					End If
					Dim idx As Integer = m(token)
					expFTemp.putScalar(0, j, idx)
					expMTemp.putScalar(0, j, 1)
				Next j
				If i = 0 Then
					expF = expFTemp.dup()
					expM = expMTemp.dup()
				Else
					expF = Nd4j.vstack(expF.dup(), expFTemp)
					expM = Nd4j.vstack(expM.dup(), expMTemp)
				End If
			Next i

			expF = Nd4j.vstack(expF, zeros)
			expM = Nd4j.vstack(expM, zeros)
			Dim expL As INDArray = Nd4j.createFromArray(New Single()(){
				New Single() {0, 1},
				New Single() {1, 0},
				New Single() {0, 1},
				New Single() {0, 0}
			})
			Dim expLM As INDArray = Nd4j.create(DataType.FLOAT, 4, 1)
			expLM.putScalar(0, 0, 1)
			expLM.putScalar(1, 0, 1)
			expLM.putScalar(2, 0, 1)

			'--------------------------------------------------------------

			Dim b As BertIterator = BertIterator.builder().tokenizer(testHelper.getTokenizer()).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, 16).minibatchSize(minibatchSize + 1).padMinibatches(True).sentenceProvider(testHelper.getSentenceProvider()).featureArrays(BertIterator.FeatureArrays.INDICES_MASK_SEGMENTID).vocabMap(testHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).build()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = b.next()
			Dim expShape() As Long = {4, 16}
			assertArrayEquals(expShape, mds.getFeatures(0).shape())
			assertArrayEquals(expShape, mds.getFeatures(1).shape())
			assertArrayEquals(expShape, mds.getFeaturesMaskArray(0).shape())

			Dim lShape() As Long = {4, 2}
			Dim lmShape() As Long = {4, 1}
			assertArrayEquals(lShape, mds.getLabels(0).shape())
			assertArrayEquals(lmShape, mds.getLabelsMaskArray(0).shape())

			assertEquals(expF, mds.getFeatures(0))
			assertEquals(expM, mds.getFeaturesMaskArray(0))
			assertEquals(expL, mds.getLabels(0))
			assertEquals(expLM, mds.getLabelsMaskArray(0))

			assertEquals(expF, b.featurizeSentences(testHelper.getSentences()).First(0))
			assertEquals(expM, b.featurizeSentences(testHelper.getSentences()).Second(0))
		End Sub

	'    
	'        Checks that a mds from a pair sentence is equal to hstack'd mds from the left side and right side of the pair
	'        Checks different lengths for max length to check popping and padding
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSentencePairsSingle() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSentencePairsSingle()
			If Platform.isWindows() Then
				Return
			End If
			Dim prependAppend As Boolean
			Dim numOfSentences As Integer

			Dim testHelper As New TestSentenceHelper()
			Dim shortL As Integer = testHelper.getShortestL()
			Dim longL As Integer = testHelper.getLongestL()

			Dim multiDataSetTriple As Triple(Of MultiDataSet, MultiDataSet, MultiDataSet)
			Dim fromPair, leftSide, rightSide As MultiDataSet

			' check for pair max length exactly equal to sum of lengths - pop neither no padding
			' should be the same as hstack with segment ids 1 for second sentence
			prependAppend = True
			numOfSentences = 1
			multiDataSetTriple = generateMultiDataSets(New Triple(Of Integer, Integer, Integer)(shortL + longL, shortL, longL), prependAppend, numOfSentences)
			fromPair = multiDataSetTriple.getFirst()
			leftSide = multiDataSetTriple.getSecond()
			rightSide = multiDataSetTriple.getThird()
			assertEquals(fromPair.getFeatures(0), Nd4j.hstack(leftSide.getFeatures(0), rightSide.getFeatures(0)))
			rightSide.getFeatures(1).addi(1) 'add 1 for right side segment ids
			assertEquals(fromPair.getFeatures(1), Nd4j.hstack(leftSide.getFeatures(1), rightSide.getFeatures(1)))
			assertEquals(fromPair.getFeaturesMaskArray(0), Nd4j.hstack(leftSide.getFeaturesMaskArray(0), rightSide.getFeaturesMaskArray(0)))

			'check for pair max length greater than sum of lengths - pop neither with padding
			' features should be the same as hstack of shorter and longer padded with prepend/append
			' segment id should 1 only in the longer for part of the length of the sentence
			prependAppend = True
			numOfSentences = 1
			multiDataSetTriple = generateMultiDataSets(New Triple(Of Integer, Integer, Integer)(shortL + longL + 5, shortL, longL + 5), prependAppend, numOfSentences)
			fromPair = multiDataSetTriple.getFirst()
			leftSide = multiDataSetTriple.getSecond()
			rightSide = multiDataSetTriple.getThird()
			assertEquals(fromPair.getFeatures(0), Nd4j.hstack(leftSide.getFeatures(0), rightSide.getFeatures(0)))
			rightSide.getFeatures(1).get(NDArrayIndex.all(), NDArrayIndex.interval(0, longL + 1)).addi(1) 'segmentId stays 0 for the padded part
			assertEquals(fromPair.getFeatures(1), Nd4j.hstack(leftSide.getFeatures(1), rightSide.getFeatures(1)))
			assertEquals(fromPair.getFeaturesMaskArray(0), Nd4j.hstack(leftSide.getFeaturesMaskArray(0), rightSide.getFeaturesMaskArray(0)))

			'check for pair max length less than shorter sentence - pop both
			'should be the same as hstack with segment ids 1 for second sentence if no prepend/append
			Dim maxL As Integer = 5 'checking odd
			numOfSentences = 3
			prependAppend = False
			multiDataSetTriple = generateMultiDataSets(New Triple(Of Integer, Integer, Integer)(maxL, maxL \ 2, maxL - maxL \ 2), prependAppend, numOfSentences)
			fromPair = multiDataSetTriple.getFirst()
			leftSide = multiDataSetTriple.getSecond()
			rightSide = multiDataSetTriple.getThird()
			assertEquals(fromPair.getFeatures(0), Nd4j.hstack(leftSide.getFeatures(0), rightSide.getFeatures(0)))
			rightSide.getFeatures(1).addi(1)
			assertEquals(fromPair.getFeatures(1), Nd4j.hstack(leftSide.getFeatures(1), rightSide.getFeatures(1)))
			assertEquals(fromPair.getFeaturesMaskArray(0), Nd4j.hstack(leftSide.getFeaturesMaskArray(0), rightSide.getFeaturesMaskArray(0)))
		End Sub

	'    
	'        Same idea as previous test - construct mds from bert iterator with sep sentences and check against one with pairs
	'        Checks various max lengths
	'        Has sentences of varying lengths
	'    
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSentencePairsUnequalLengths() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSentencePairsUnequalLengths()
			If Platform.isWindows() Then
				Return
			End If
			Dim minibatchSize As Integer = 4
			Dim numOfSentencesinIter As Integer = 3

			Dim testPairHelper As New TestSentencePairsHelper(numOfSentencesinIter)
			Dim shortL As Integer = testPairHelper.getShortL()
			Dim longL As Integer = testPairHelper.getLongL()
			Dim sent1L As Integer = testPairHelper.getSentenceALen()
			Dim sent2L As Integer = testPairHelper.getSentenceBLen()

			Console.WriteLine("Sentence Pairs, Left")
			Console.WriteLine(testPairHelper.getSentencesLeft())
			Console.WriteLine("Sentence Pairs, Right")
			Console.WriteLine(testPairHelper.getSentencesRight())

			'anything outside this range more will need to check padding,truncation
			Dim maxL As Integer = longL + shortL
			Do While maxL > 2 * shortL + 1

				Console.WriteLine("Running for max length = " & maxL)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim leftMDS As MultiDataSet = BertIterator.builder().tokenizer(testPairHelper.getTokenizer()).minibatchSize(minibatchSize).featureArrays(BertIterator.FeatureArrays.INDICES_MASK_SEGMENTID).vocabMap(testPairHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, longL * 10).sentenceProvider((New TestSentenceHelper(numOfSentencesinIter)).getSentenceProvider()).padMinibatches(True).build().next()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim rightMDS As MultiDataSet = BertIterator.builder().tokenizer(testPairHelper.getTokenizer()).minibatchSize(minibatchSize).featureArrays(BertIterator.FeatureArrays.INDICES_MASK_SEGMENTID).vocabMap(testPairHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, longL * 10).sentenceProvider((New TestSentenceHelper(True, numOfSentencesinIter)).getSentenceProvider()).padMinibatches(True).build().next()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim pairMDS As MultiDataSet = BertIterator.builder().tokenizer(testPairHelper.getTokenizer()).minibatchSize(minibatchSize).featureArrays(BertIterator.FeatureArrays.INDICES_MASK_SEGMENTID).vocabMap(testPairHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, maxL).sentencePairProvider(testPairHelper.getPairSentenceProvider()).padMinibatches(True).build().next()

				'CHECK FEATURES
				Dim combinedFeat As INDArray = Nd4j.create(DataType.INT, minibatchSize, maxL)
				'left side
				Dim leftFeatures As INDArray = leftMDS.getFeatures(0)
				Dim topLSentFeat As INDArray = leftFeatures.getRow(0).get(NDArrayIndex.interval(0, shortL))
				Dim midLSentFeat As INDArray = leftFeatures.getRow(1).get(NDArrayIndex.interval(0, maxL - shortL))
				Dim bottomLSentFeat As INDArray = leftFeatures.getRow(2).get(NDArrayIndex.interval(0, sent1L))
				'right side
				Dim rightFeatures As INDArray = rightMDS.getFeatures(0)
				Dim topRSentFeat As INDArray = rightFeatures.getRow(0).get(NDArrayIndex.interval(0, maxL - shortL))
				Dim midRSentFeat As INDArray = rightFeatures.getRow(1).get(NDArrayIndex.interval(0, shortL))
				Dim bottomRSentFeat As INDArray = rightFeatures.getRow(2).get(NDArrayIndex.interval(0, sent2L))
				'expected pair
				combinedFeat.getRow(0).addi(Nd4j.hstack(topLSentFeat, topRSentFeat))
				combinedFeat.getRow(1).addi(Nd4j.hstack(midLSentFeat, midRSentFeat))
				combinedFeat.getRow(2).get(NDArrayIndex.interval(0, sent1L + sent2L)).addi(Nd4j.hstack(bottomLSentFeat, bottomRSentFeat))

				assertEquals(maxL, pairMDS.getFeatures(0).shape()(1))
				assertArrayEquals(combinedFeat.shape(), pairMDS.getFeatures(0).shape())
				assertEquals(combinedFeat, pairMDS.getFeatures(0))

				'CHECK SEGMENT ID
				Dim combinedFetSeg As INDArray = Nd4j.create(DataType.INT, minibatchSize, maxL)
				combinedFetSeg.get(NDArrayIndex.point(0), NDArrayIndex.interval(shortL, maxL)).addi(1)
				combinedFetSeg.get(NDArrayIndex.point(1), NDArrayIndex.interval(maxL - shortL, maxL)).addi(1)
				combinedFetSeg.get(NDArrayIndex.point(2), NDArrayIndex.interval(sent1L, sent1L + sent2L)).addi(1)
				assertArrayEquals(combinedFetSeg.shape(), pairMDS.getFeatures(1).shape())
				assertEquals(maxL, combinedFetSeg.shape()(1))
				assertEquals(combinedFetSeg, pairMDS.getFeatures(1))

				testPairHelper.getPairSentenceProvider().reset()
				maxL -= 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSentencePairFeaturizer() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSentencePairFeaturizer()
			If Platform.isWindows() Then
				Return
			End If
			Dim minibatchSize As Integer = 2
			Dim testPairHelper As New TestSentencePairsHelper(minibatchSize)
			Dim b As BertIterator = BertIterator.builder().tokenizer(testPairHelper.getTokenizer()).minibatchSize(minibatchSize).padMinibatches(True).featureArrays(BertIterator.FeatureArrays.INDICES_MASK_SEGMENTID).vocabMap(testPairHelper.getTokenizer().getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION).lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, 128).sentencePairProvider(testPairHelper.getPairSentenceProvider()).prependToken("[CLS]").appendToken("[SEP]").build()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = b.next()
			Dim featuresArr() As INDArray = mds.Features
			Dim featuresMaskArr() As INDArray = mds.FeaturesMaskArrays

			Dim p As Pair(Of INDArray(), INDArray()) = b.featurizeSentencePairs(testPairHelper.getSentencePairs())
			assertEquals(p.First.Length, 2)
			assertEquals(featuresArr(0), p.First(0))
			assertEquals(featuresArr(1), p.First(1))
			assertEquals(featuresMaskArr(0), p.Second(0))
		End Sub

		''' <summary>
		''' Returns three multidatasets (one from pair of sentences and the other two from single sentence lists) from bert iterator
		''' with given max lengths and whether to prepend/append
		''' Idea is the sentence pair dataset can be constructed from the single sentence datasets
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.nd4j.common.primitives.Triple<org.nd4j.linalg.dataset.api.MultiDataSet, org.nd4j.linalg.dataset.api.MultiDataSet, org.nd4j.linalg.dataset.api.MultiDataSet> generateMultiDataSets(org.nd4j.common.primitives.Triple<Integer, Integer, Integer> maxLengths, boolean prependAppend, int numSentences) throws java.io.IOException
		Private Function generateMultiDataSets(ByVal maxLengths As Triple(Of Integer, Integer, Integer), ByVal prependAppend As Boolean, ByVal numSentences As Integer) As Triple(Of MultiDataSet, MultiDataSet, MultiDataSet)
			Dim t As New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)
			Dim maxforPair As Integer = maxLengths.getFirst()
			Dim maxPartOne As Integer = maxLengths.getSecond()
			Dim maxPartTwo As Integer = maxLengths.getThird()
			Dim commonBuilder As BertIterator.Builder
			commonBuilder = BertIterator.builder().tokenizer(t).minibatchSize(4).featureArrays(BertIterator.FeatureArrays.INDICES_MASK_SEGMENTID).vocabMap(t.getVocab()).task(BertIterator.Task.SEQ_CLASSIFICATION)
			Dim pairIter As BertIterator = commonBuilder.lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH,If(prependAppend, maxforPair + 3, maxforPair)).sentencePairProvider((New TestSentencePairsHelper(numSentences)).getPairSentenceProvider()).prependToken(If(prependAppend, "[CLS]", Nothing)).appendToken(If(prependAppend, "[SEP]", Nothing)).build()
			Dim leftIter As BertIterator = commonBuilder.lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH,If(prependAppend, maxPartOne + 2, maxPartOne)).sentenceProvider((New TestSentenceHelper(numSentences)).getSentenceProvider()).prependToken(If(prependAppend, "[CLS]", Nothing)).appendToken(If(prependAppend, "[SEP]", Nothing)).build()
			Dim rightIter As BertIterator = commonBuilder.lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH,If(prependAppend, maxPartTwo + 1, maxPartTwo)).sentenceProvider((New TestSentenceHelper(True, numSentences)).getSentenceProvider()).prependToken(Nothing).appendToken(If(prependAppend, "[SEP]", Nothing)).build()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return New Triple(Of MultiDataSet, MultiDataSet, MultiDataSet)(pairIter.next(), leftIter.next(), rightIter.next())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static class TestSentencePairsHelper
		Private Class TestSentencePairsHelper

			Friend sentencesLeft As IList(Of String)
			Friend sentencesRight As IList(Of String)
			Friend sentencePairs As IList(Of Pair(Of String, String))
			Friend tokenizedSentencesLeft As IList(Of IList(Of String))
			Friend tokenizedSentencesRight As IList(Of IList(Of String))
			Friend labels As IList(Of String)
			Friend shortL As Integer
			Friend longL As Integer
			Friend sentenceALen As Integer
			Friend sentenceBLen As Integer
			Friend tokenizer As BertWordPieceTokenizerFactory
			Friend pairSentenceProvider As CollectionLabeledPairSentenceProvider

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private TestSentencePairsHelper() throws java.io.IOException
			Friend Sub New()
				Me.New(3)
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private TestSentencePairsHelper(int minibatchSize) throws java.io.IOException
			Friend Sub New(ByVal minibatchSize As Integer)
				sentencesLeft = New List(Of String)()
				sentencesRight = New List(Of String)()
				sentencePairs = New List(Of Pair(Of String, String))()
				labels = New List(Of String)()
				tokenizedSentencesLeft = New List(Of IList(Of String))()
				tokenizedSentencesRight = New List(Of IList(Of String))()
				tokenizer = New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)
				sentencesLeft.Add(shortSentence)
				sentencesRight.Add(longSentence)
				sentencePairs.Add(New Pair(Of String, String)(shortSentence, longSentence))
				labels.Add("positive")
				If minibatchSize > 1 Then
					sentencesLeft.Add(longSentence)
					sentencesRight.Add(shortSentence)
					sentencePairs.Add(New Pair(Of String, String)(longSentence, shortSentence))
					labels.Add("negative")
					If minibatchSize > 2 Then
						sentencesLeft.Add(sentenceA)
						sentencesRight.Add(sentenceB)
						sentencePairs.Add(New Pair(Of String, String)(sentenceA, sentenceB))
						labels.Add("positive")
					End If
				End If
				For i As Integer = 0 To minibatchSize - 1
					Dim tokensL As IList(Of String) = tokenizer.create(sentencesLeft(i)).getTokens()
					Dim tokensR As IList(Of String) = tokenizer.create(sentencesRight(i)).getTokens()
					If i = 0 Then
						shortL = tokensL.Count
						longL = tokensR.Count
					End If
					If i = 2 Then
						sentenceALen = tokensL.Count
						sentenceBLen = tokensR.Count
					End If
					tokenizedSentencesLeft.Add(tokensL)
					tokenizedSentencesRight.Add(tokensR)
				Next i
				pairSentenceProvider = New CollectionLabeledPairSentenceProvider(sentencesLeft, sentencesRight, labels, Nothing)
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static class TestSentenceHelper
		Private Class TestSentenceHelper

			Friend sentences As IList(Of String)
			Friend tokenizedSentences As IList(Of IList(Of String))
			Friend labels As IList(Of String)
			Friend shortestL As Integer = 0
			Friend longestL As Integer = 0
			Friend tokenizer As BertWordPieceTokenizerFactory
			Friend sentenceProvider As CollectionLabeledSentenceProvider

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private TestSentenceHelper() throws java.io.IOException
			Friend Sub New()
				Me.New(False, 2)
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private TestSentenceHelper(int minibatchSize) throws java.io.IOException
			Friend Sub New(ByVal minibatchSize As Integer)
				Me.New(False, minibatchSize)
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private TestSentenceHelper(boolean alternateOrder) throws java.io.IOException
			Friend Sub New(ByVal alternateOrder As Boolean)
				Me.New(False, 3)
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private TestSentenceHelper(boolean alternateOrder, int minibatchSize) throws java.io.IOException
			Friend Sub New(ByVal alternateOrder As Boolean, ByVal minibatchSize As Integer)
				sentences = New List(Of String)()
				labels = New List(Of String)()
				tokenizedSentences = New List(Of IList(Of String))()
				tokenizer = New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)
				If Not alternateOrder Then
					sentences.Add(shortSentence)
					labels.Add("positive")
					If minibatchSize > 1 Then
						sentences.Add(longSentence)
						labels.Add("negative")
						If minibatchSize > 2 Then
							sentences.Add(sentenceA)
							labels.Add("positive")
						End If
					End If
				Else
					sentences.Add(longSentence)
					labels.Add("negative")
					If minibatchSize > 1 Then
						sentences.Add(shortSentence)
						labels.Add("positive")
						If minibatchSize > 2 Then
							sentences.Add(sentenceB)
							labels.Add("positive")
						End If
					End If
				End If
				For i As Integer = 0 To sentences.Count - 1
					Dim tokenizedSentence As IList(Of String) = tokenizer.create(sentences(i)).getTokens()
					If i = 0 Then
						shortestL = tokenizedSentence.Count
					End If
					If tokenizedSentence.Count > longestL Then
						longestL = tokenizedSentence.Count
					End If
					If tokenizedSentence.Count < shortestL Then
						shortestL = tokenizedSentence.Count
					End If
					tokenizedSentences.Add(tokenizedSentence)
				Next i
				sentenceProvider = New CollectionLabeledSentenceProvider(sentences, labels, Nothing)
			End Sub
		End Class

	End Class

End Namespace