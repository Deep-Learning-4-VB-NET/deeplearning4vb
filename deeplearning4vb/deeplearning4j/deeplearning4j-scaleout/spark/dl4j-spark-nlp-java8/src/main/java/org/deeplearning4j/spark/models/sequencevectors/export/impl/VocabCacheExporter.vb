Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports org.deeplearning4j.models.embeddings.inmemory
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.spark.models.sequencevectors.export.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class VocabCacheExporter implements org.deeplearning4j.spark.models.sequencevectors.export.SparkModelExporter<org.deeplearning4j.models.word2vec.VocabWord>
	Public Class VocabCacheExporter
		Implements SparkModelExporter(Of VocabWord)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache;
		Protected Friend vocabCache As VocabCache(Of VocabWord)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.embeddings.inmemory.InMemoryLookupTable<org.deeplearning4j.models.word2vec.VocabWord> lookupTable;
		Protected Friend lookupTable As InMemoryLookupTable(Of VocabWord)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.word2vec.Word2Vec word2Vec;
		Protected Friend word2Vec As Word2Vec

		Public Sub New()

		End Sub

		Public Overridable Sub export(ByVal rdd As JavaRDD(Of ExportContainer(Of VocabWord))) Implements SparkModelExporter(Of VocabWord).export

			' beware, generally that's VERY bad idea, but will work fine for testing purposes
			Dim list As IList(Of ExportContainer(Of VocabWord)) = rdd.collect()

			If vocabCache Is Nothing Then
				vocabCache = New AbstractCache(Of VocabWord)()
			End If

			Dim syn0 As INDArray = Nothing

			' just roll through list
			For Each element As ExportContainer(Of VocabWord) In list
				Dim word As VocabWord = element.getElement()
				Dim weights As INDArray = element.getArray()

				If syn0 Is Nothing Then
					syn0 = Nd4j.create(list.Count, weights.length())
				End If


				vocabCache.addToken(word)
				vocabCache.addWordToIndex(word.Index, word.Label)


				syn0.getRow(word.Index).assign(weights)
			Next element

			If lookupTable Is Nothing Then
				lookupTable = (New InMemoryLookupTable.Builder(Of VocabWord)()).cache(vocabCache).vectorLength(syn0.columns()).build()
			End If

			lookupTable.setSyn0(syn0)

			' this is bad & dirty, but we don't really need anything else for testing :)
			word2Vec = WordVectorSerializer.fromPair(Pair.makePair(Of InMemoryLookupTable, VocabCache)(lookupTable, vocabCache))
		End Sub
	End Class

End Namespace