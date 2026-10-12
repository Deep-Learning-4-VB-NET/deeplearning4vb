Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports CollectionLabeledSentenceProvider = org.deeplearning4j.iterator.provider.CollectionLabeledSentenceProvider
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports Test = org.junit.jupiter.api.Test
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class TestCnnSentenceDataSetIterator extends org.deeplearning4j.BaseDL4JTest
	Public Class TestCnnSentenceDataSetIterator
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSentenceIterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSentenceIterator()
			Dim w2v As WordVectors = WordVectorSerializer.readWord2VecModel((New ClassPathResource("word2vec/googleload/sample_vec.bin")).File)

			Dim vectorSize As Integer = w2v.lookupTable().layerSize()

			'        Collection<String> words = w2v.lookupTable().getVocabCache().words();
			'        for(String s : words){
			'            System.out.println(s);
			'        }

			Dim sentences As IList(Of String) = New List(Of String)()
			'First word: all present
			sentences.Add("these balance Database model")
			sentences.Add("into same THISWORDDOESNTEXIST are")
			Dim maxLength As Integer = 4
			Dim s1 As IList(Of String) = New List(Of String) From {"these", "balance", "Database", "model"}
			Dim s2 As IList(Of String) = New List(Of String) From {"into", "same", "are"}

			Dim labelsForSentences As IList(Of String) = New List(Of String) From {"Positive", "Negative"}

			Dim expLabels As INDArray = Nd4j.create(New Single()() {
				New Single() {0, 1},
				New Single() {1, 0}
			})

			Dim alongHeightVals() As Boolean = {True, False}

			For Each norm As Boolean In New Boolean(){True, False}
				For Each alongHeight As Boolean In alongHeightVals

					Dim expectedFeatures As INDArray
					If alongHeight Then
						expectedFeatures = Nd4j.create(2, 1, maxLength, vectorSize)
					Else
						expectedFeatures = Nd4j.create(2, 1, vectorSize, maxLength)
					End If

					Dim fmShape() As Integer
					If alongHeight Then
						fmShape = New Integer(){2, 1, 4, 1}
					Else
						fmShape = New Integer(){2, 1, 1, 4}
					End If
					Dim expectedFeatureMask As INDArray = Nd4j.create(New Single()(){
						New Single() {1, 1, 1, 1},
						New Single() {1, 1, 1, 0}
					}).reshape("c"c, fmShape)


					For i As Integer = 0 To 3
						Dim v As INDArray = If(norm, w2v.getWordVectorMatrixNormalized(s1(i)), w2v.getWordVectorMatrix(s1(i)))
						If alongHeight Then
							expectedFeatures.get(NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.point(i), NDArrayIndex.all()).assign(v)
						Else
							expectedFeatures.get(NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(v)
						End If
					Next i

					For i As Integer = 0 To 2
						Dim v As INDArray = If(norm, w2v.getWordVectorMatrixNormalized(s2(i)), w2v.getWordVectorMatrix(s2(i)))
						If alongHeight Then
							expectedFeatures.get(NDArrayIndex.point(1), NDArrayIndex.point(0), NDArrayIndex.point(i), NDArrayIndex.all()).assign(v)
						Else
							expectedFeatures.get(NDArrayIndex.point(1), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(v)
						End If
					Next i


					Dim p As LabeledSentenceProvider = New CollectionLabeledSentenceProvider(sentences, labelsForSentences, Nothing)
					Dim dsi As CnnSentenceDataSetIterator = (New CnnSentenceDataSetIterator.Builder(CnnSentenceDataSetIterator.Format.CNN2D)).sentenceProvider(p).useNormalizedWordVectors(norm).wordVectors(w2v).maxSentenceLength(256).minibatchSize(32).sentencesAlongHeight(alongHeight).build()

					'            System.out.println("alongHeight = " + alongHeight);
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = dsi.next()
					assertArrayEquals(expectedFeatures.shape(), ds.Features.shape())
					assertEquals(expectedFeatures, ds.Features)
					assertEquals(expLabels, ds.Labels)
					assertEquals(expectedFeatureMask, ds.FeaturesMaskArray)
					assertNull(ds.LabelsMaskArray)

					Dim s1F As INDArray = dsi.loadSingleSentence(sentences(0))
					Dim s2F As INDArray = dsi.loadSingleSentence(sentences(1))
					Dim sub1 As INDArray = ds.Features.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
					Dim sub2 As INDArray
					If alongHeight Then

						sub2 = ds.Features.get(NDArrayIndex.interval(1, 1, True), NDArrayIndex.all(), NDArrayIndex.interval(0, 3), NDArrayIndex.all())
					Else
						sub2 = ds.Features.get(NDArrayIndex.interval(1, 1, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 3))
					End If

					assertArrayEquals(sub1.shape(), s1F.shape())
					assertArrayEquals(sub2.shape(), s2F.shape())
					assertEquals(sub1, s1F)
					assertEquals(sub2, s2F)
				Next alongHeight
			Next norm
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSentenceIteratorCNN1D_RNN() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSentenceIteratorCNN1D_RNN()
			Dim w2v As WordVectors = WordVectorSerializer.readWord2VecModel((New ClassPathResource("word2vec/googleload/sample_vec.bin")).File)

			Dim vectorSize As Integer = w2v.lookupTable().layerSize()

			Dim sentences As IList(Of String) = New List(Of String)()
			'First word: all present
			sentences.Add("these balance Database model")
			sentences.Add("into same THISWORDDOESNTEXIST are")
			Dim maxLength As Integer = 4
			Dim s1 As IList(Of String) = New List(Of String) From {"these", "balance", "Database", "model"}
			Dim s2 As IList(Of String) = New List(Of String) From {"into", "same", "are"}

			Dim labelsForSentences As IList(Of String) = New List(Of String) From {"Positive", "Negative"}

			Dim expLabels As INDArray = Nd4j.create(New Single()() {
				New Single() {0, 1},
				New Single() {1, 0}
			})

			For Each norm As Boolean In New Boolean(){True, False}
				For Each f As CnnSentenceDataSetIterator.Format In New CnnSentenceDataSetIterator.Format(){CnnSentenceDataSetIterator.Format.CNN1D, CnnSentenceDataSetIterator.Format.RNN}

					Dim expectedFeatures As INDArray = Nd4j.create(2, vectorSize, maxLength)
					Dim fmShape() As Integer = {2, 4}
					Dim expectedFeatureMask As INDArray = Nd4j.create(New Single()(){
						New Single() {1, 1, 1, 1},
						New Single() {1, 1, 1, 0}
					}).reshape("c"c, fmShape)


					For i As Integer = 0 To 3
						Dim v As INDArray = If(norm, w2v.getWordVectorMatrixNormalized(s1(i)), w2v.getWordVectorMatrix(s1(i)))
						expectedFeatures.get(NDArrayIndex.point(0), NDArrayIndex.all(),NDArrayIndex.point(i)).assign(v)
					Next i

					For i As Integer = 0 To 2
						Dim v As INDArray = If(norm, w2v.getWordVectorMatrixNormalized(s2(i)), w2v.getWordVectorMatrix(s2(i)))
						expectedFeatures.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(v)
					Next i

					Dim p As LabeledSentenceProvider = New CollectionLabeledSentenceProvider(sentences, labelsForSentences, Nothing)
					Dim dsi As CnnSentenceDataSetIterator = (New CnnSentenceDataSetIterator.Builder(f)).sentenceProvider(p).useNormalizedWordVectors(norm).wordVectors(w2v).maxSentenceLength(256).minibatchSize(32).build()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = dsi.next()
					assertArrayEquals(expectedFeatures.shape(), ds.Features.shape())
					assertEquals(expectedFeatures, ds.Features)
					assertEquals(expLabels, ds.Labels)
					assertEquals(expectedFeatureMask, ds.FeaturesMaskArray)
					assertNull(ds.LabelsMaskArray)

					Dim s1F As INDArray = dsi.loadSingleSentence(sentences(0))
					Dim s2F As INDArray = dsi.loadSingleSentence(sentences(1))
					Dim sub1 As INDArray = ds.Features.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.all(), NDArrayIndex.all())
					Dim sub2 As INDArray = ds.Features.get(NDArrayIndex.interval(1, 1, True), NDArrayIndex.all(), NDArrayIndex.interval(0, 3))

					assertArrayEquals(sub1.shape(), s1F.shape())
					assertArrayEquals(sub2.shape(), s2F.shape())
					assertEquals(sub1, s1F)
					assertEquals(sub2, s2F)
				Next f
			Next norm
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnSentenceDataSetIteratorNoTokensEdgeCase() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnnSentenceDataSetIteratorNoTokensEdgeCase()

			Dim w2v As WordVectors = WordVectorSerializer.readWord2VecModel((New ClassPathResource("word2vec/googleload/sample_vec.bin")).File)

			Dim vectorSize As Integer = w2v.lookupTable().layerSize()

			Dim sentences As IList(Of String) = New List(Of String)()
			'First 2 sentences - no valid words
			sentences.Add("NOVALID WORDSHERE")
			sentences.Add("!!!")
			sentences.Add("these balance Database model")
			sentences.Add("into same THISWORDDOESNTEXIST are")
			Dim maxLength As Integer = 4
			Dim s1 As IList(Of String) = New List(Of String) From {"these", "balance", "Database", "model"}
			Dim s2 As IList(Of String) = New List(Of String) From {"into", "same", "are"}

			Dim labelsForSentences As IList(Of String) = New List(Of String) From {"Positive", "Negative", "Positive", "Negative"}

			Dim expLabels As INDArray = Nd4j.create(New Single()() {
				New Single() {0, 1},
				New Single() {1, 0}
			})


			Dim p As LabeledSentenceProvider = New CollectionLabeledSentenceProvider(sentences, labelsForSentences, Nothing)
			Dim dsi As CnnSentenceDataSetIterator = (New CnnSentenceDataSetIterator.Builder(CnnSentenceDataSetIterator.Format.CNN2D)).sentenceProvider(p).wordVectors(w2v).useNormalizedWordVectors(True).maxSentenceLength(256).minibatchSize(32).sentencesAlongHeight(False).build()

			'            System.out.println("alongHeight = " + alongHeight);
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = dsi.next()

			Dim expectedFeatures As INDArray = Nd4j.create(DataType.FLOAT, 2, 1, vectorSize, maxLength)

			Dim expectedFeatureMask As INDArray = Nd4j.create(New Single()() {
				New Single() {1, 1, 1, 1},
				New Single() {1, 1, 1, 0}
			}).reshape("c"c, 2, 1, 1, 4)

			For i As Integer = 0 To 3
				expectedFeatures.get(NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(w2v.getWordVectorMatrixNormalized(s1(i)))
			Next i

			For i As Integer = 0 To 2
				expectedFeatures.get(NDArrayIndex.point(1), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(w2v.getWordVectorMatrixNormalized(s2(i)))
			Next i

			assertArrayEquals(expectedFeatures.shape(), ds.Features.shape())
			assertEquals(expectedFeatures, ds.Features)
			assertEquals(expLabels, ds.Labels)
			assertEquals(expectedFeatureMask, ds.FeaturesMaskArray)
			assertNull(ds.LabelsMaskArray)


			'Sanity check on single sentence loading:
			Dim allKnownWords As INDArray = dsi.loadSingleSentence("these balance")
			Dim withUnknown As INDArray = dsi.loadSingleSentence("these NOVALID")
			assertNotNull(allKnownWords)
			assertNotNull(withUnknown)

			Try
				dsi.loadSingleSentence("NOVALID AlsoNotInVocab")
				fail("Expected exception")
			Catch t As Exception
				Dim m As String = t.getMessage()
				assertTrue(m.Contains("RemoveWord") AndAlso m.Contains("vocabulary"), m)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnSentenceDataSetIteratorNoValidTokensNextEdgeCase() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnnSentenceDataSetIteratorNoValidTokensNextEdgeCase()
			'Case: 2 minibatches, of size 2
			'First minibatch: OK
			'Second minibatch: would be empty
			'Therefore: after first minibatch is returned, .hasNext() should return false

			Dim w2v As WordVectors = WordVectorSerializer.readWord2VecModel((New ClassPathResource("word2vec/googleload/sample_vec.bin")).File)

			Dim vectorSize As Integer = w2v.lookupTable().layerSize()

			Dim sentences As IList(Of String) = New List(Of String)()
			sentences.Add("these balance Database model")
			sentences.Add("into same THISWORDDOESNTEXIST are")
			'Last 2 sentences - no valid words
			sentences.Add("NOVALID WORDSHERE")
			sentences.Add("!!!")
			Dim maxLength As Integer = 4
			Dim s1 As IList(Of String) = New List(Of String) From {"these", "balance", "Database", "model"}
			Dim s2 As IList(Of String) = New List(Of String) From {"into", "same", "are"}

			Dim labelsForSentences As IList(Of String) = New List(Of String) From {"Positive", "Negative", "Positive", "Negative"}

			Dim expLabels As INDArray = Nd4j.create(New Single()() {
				New Single() {0, 1},
				New Single() {1, 0}
			})


			Dim p As LabeledSentenceProvider = New CollectionLabeledSentenceProvider(sentences, labelsForSentences, Nothing)
			Dim dsi As CnnSentenceDataSetIterator = (New CnnSentenceDataSetIterator.Builder(CnnSentenceDataSetIterator.Format.CNN2D)).sentenceProvider(p).wordVectors(w2v).useNormalizedWordVectors(True).maxSentenceLength(256).minibatchSize(2).sentencesAlongHeight(False).build()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(dsi.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = dsi.next()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(dsi.hasNext())


			Dim expectedFeatures As INDArray = Nd4j.create(2, 1, vectorSize, maxLength)

			Dim expectedFeatureMask As INDArray = Nd4j.create(New Single()() {
				New Single() {1, 1, 1, 1},
				New Single() {1, 1, 1, 0}
			}).reshape("c"c, 2, 1, 1, 4)

			For i As Integer = 0 To 3
				expectedFeatures.get(NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(w2v.getWordVectorMatrixNormalized(s1(i)))
			Next i

			For i As Integer = 0 To 2
				expectedFeatures.get(NDArrayIndex.point(1), NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(w2v.getWordVectorMatrixNormalized(s2(i)))
			Next i

			assertArrayEquals(expectedFeatures.shape(), ds.Features.shape())
			assertEquals(expectedFeatures, ds.Features)
			assertEquals(expLabels, ds.Labels)
			assertEquals(expectedFeatureMask, ds.FeaturesMaskArray)
			assertNull(ds.LabelsMaskArray)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnSentenceDataSetIteratorUseUnknownVector() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnnSentenceDataSetIteratorUseUnknownVector()

			Dim w2v As WordVectors = WordVectorSerializer.readWord2VecModel((New ClassPathResource("word2vec/googleload/sample_vec.bin")).File)

			Dim sentences As IList(Of String) = New List(Of String)()
			sentences.Add("these balance Database model")
			sentences.Add("into same THISWORDDOESNTEXIST are")
			'Last 2 sentences - no valid words
			sentences.Add("NOVALID WORDSHERE")
			sentences.Add("!!!")

			Dim labelsForSentences As IList(Of String) = New List(Of String) From {"Positive", "Negative", "Positive", "Negative"}


			Dim p As LabeledSentenceProvider = New CollectionLabeledSentenceProvider(sentences, labelsForSentences, Nothing)
			Dim dsi As CnnSentenceDataSetIterator = (New CnnSentenceDataSetIterator.Builder(CnnSentenceDataSetIterator.Format.CNN1D)).unknownWordHandling(CnnSentenceDataSetIterator.UnknownWordHandling.UseUnknownVector).sentenceProvider(p).wordVectors(w2v).useNormalizedWordVectors(True).maxSentenceLength(256).minibatchSize(4).sentencesAlongHeight(False).build()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(dsi.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = dsi.next()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(dsi.hasNext())

			Dim f As INDArray = ds.Features
			assertEquals(4, f.size(0))

			Dim unknown As INDArray = w2v.getWordVectorMatrix(w2v.UNK)
			If unknown Is Nothing Then
				unknown = Nd4j.create(DataType.FLOAT, f.size(1))
			End If

			assertEquals(unknown, f.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.point(0)))
			assertEquals(unknown, f.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.point(1)))
			assertEquals(unknown.like(), f.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.point(3)))

			assertEquals(unknown, f.get(NDArrayIndex.point(3), NDArrayIndex.all(), NDArrayIndex.point(0)))
			assertEquals(unknown.like(), f.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.point(1)))

			'Sanity check on single sentence loading:
			Dim allKnownWords As INDArray = dsi.loadSingleSentence("these balance")
			Dim withUnknown As INDArray = dsi.loadSingleSentence("these NOVALID")
			Dim allUnknown As INDArray = dsi.loadSingleSentence("NOVALID AlsoNotInVocab")
			assertNotNull(allKnownWords)
			assertNotNull(withUnknown)
			assertNotNull(allUnknown)
		End Sub
	End Class

End Namespace