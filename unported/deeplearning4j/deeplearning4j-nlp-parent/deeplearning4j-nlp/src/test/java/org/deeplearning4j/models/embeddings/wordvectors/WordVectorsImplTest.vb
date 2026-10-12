Imports Tag = org.junit.jupiter.api.Tag
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.models.embeddings
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports Mockito = org.mockito.Mockito
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.mockito.Mockito.when

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

Namespace org.deeplearning4j.models.embeddings.wordvectors

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class WordVectorsImplTest extends org.deeplearning4j.BaseDL4JTest
	Public Class WordVectorsImplTest
		Inherits BaseDL4JTest

		Private vocabCache As VocabCache
		Private weightLookupTable As WeightLookupTable
		Private wordVectors As WordVectorsImpl(Of SequenceElement)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void init() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub init()
			vocabCache = Mockito.mock(GetType(VocabCache))
			weightLookupTable = Mockito.mock(GetType(WeightLookupTable))
			wordVectors = New WordVectorsImpl(Of SequenceElement)()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void getWordVectors_HaveTwoWordsNotInVocabAndOneIn_ExpectAllNonWordsRemoved()
		Public Overridable Sub getWordVectors_HaveTwoWordsNotInVocabAndOneIn_ExpectAllNonWordsRemoved()
			Dim wordVector As INDArray = Nd4j.create(1, 1)
			wordVector.putScalar(0, 5)
			[when](vocabCache.indexOf("word")).thenReturn(0)
			[when](vocabCache.containsWord("word")).thenReturn(True)
			[when](weightLookupTable.getWeights()).thenReturn(wordVector)
			wordVectors.Vocab = vocabCache
			wordVectors.LookupTable = weightLookupTable

			Dim indArray As INDArray = wordVectors.getWordVectors(Lists.newArrayList("word", "here", "is"))

			assertEquals(wordVector, indArray)
		End Sub
	End Class

End Namespace