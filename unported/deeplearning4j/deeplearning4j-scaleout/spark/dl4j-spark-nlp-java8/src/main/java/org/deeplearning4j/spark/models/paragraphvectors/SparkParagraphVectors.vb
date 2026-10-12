Imports System
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports DocumentSequenceConvertFunction = org.deeplearning4j.spark.models.paragraphvectors.functions.DocumentSequenceConvertFunction
Imports KeySequenceConvertFunction = org.deeplearning4j.spark.models.paragraphvectors.functions.KeySequenceConvertFunction
Imports org.deeplearning4j.spark.models.sequencevectors
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument

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

Namespace org.deeplearning4j.spark.models.paragraphvectors

	<Serializable>
	Public Class SparkParagraphVectors
		Inherits SparkSequenceVectors(Of VocabWord)

		Protected Friend Sub New()
			'
		End Sub

		Protected Friend Overrides ReadOnly Property ShallowVocabCache As VocabCache(Of ShallowSequenceElement)
			Get
				Return MyBase.getShallowVocabCache()
			End Get
		End Property

		Protected Friend Overrides Sub validateConfiguration()
			MyBase.validateConfiguration()

			If configuration.getTokenizerFactory() Is Nothing Then
				Throw New DL4JInvalidConfigException("TokenizerFactory is undefined. Can't train ParagraphVectors without it.")
			End If
		End Sub

		''' <summary>
		''' This method builds ParagraphVectors model, expecting JavaPairRDD with key as label, and value as document-in-a-string.
		''' </summary>
		''' <param name="documentsRdd"> </param>
		Public Overridable Sub fitMultipleFiles(ByVal documentsRdd As JavaPairRDD(Of String, String))
	'        
	'            All we want here, is to transform JavaPairRDD into JavaRDD<Sequence<VocabWord>>
	'         
			validateConfiguration()

			broadcastEnvironment(New JavaSparkContext(documentsRdd.context()))

			Dim sequenceRdd As JavaRDD(Of Sequence(Of VocabWord)) = documentsRdd.map(New KeySequenceConvertFunction(configurationBroadcast))

			MyBase.fitSequences(sequenceRdd)
		End Sub

		''' <summary>
		''' This method builds ParagraphVectors model, expecting JavaRDD<LabelledDocument>.
		''' It can be either non-tokenized documents, or tokenized.
		''' </summary>
		''' <param name="documentsRdd"> </param>
		Public Overridable Sub fitLabelledDocuments(ByVal documentsRdd As JavaRDD(Of LabelledDocument))

			validateConfiguration()

			broadcastEnvironment(New JavaSparkContext(documentsRdd.context()))

			Dim sequenceRDD As JavaRDD(Of Sequence(Of VocabWord)) = documentsRdd.map(New DocumentSequenceConvertFunction(configurationBroadcast))

			MyBase.fitSequences(sequenceRDD)
		End Sub

	End Class

End Namespace