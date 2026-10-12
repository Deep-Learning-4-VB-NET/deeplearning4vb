Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.spark.models.sequencevectors
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports TokenizerFunction = org.deeplearning4j.spark.models.sequencevectors.functions.TokenizerFunction
Imports SparkElementsLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkElementsLearningAlgorithm
Imports SparkSequenceLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkSequenceLearningAlgorithm
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration

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

Namespace org.deeplearning4j.spark.models.word2vec

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SparkWord2Vec extends org.deeplearning4j.spark.models.sequencevectors.SparkSequenceVectors<org.deeplearning4j.models.word2vec.VocabWord>
	<Serializable>
	Public Class SparkWord2Vec
		Inherits SparkSequenceVectors(Of VocabWord)

		Protected Friend Sub New()
			' FIXME: this is development-time constructor, please remove before release
			configuration = New VectorsConfiguration()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			configuration.setTokenizerFactory(GetType(DefaultTokenizerFactory).FullName)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SparkWord2Vec(@NonNull VoidConfiguration psConfiguration, @NonNull VectorsConfiguration configuration)
		Public Sub New(ByVal psConfiguration As VoidConfiguration, ByVal configuration As VectorsConfiguration)
			Me.configuration = configuration
			Me.paramServerConfiguration = psConfiguration
		End Sub

		Protected Friend Overrides ReadOnly Property ShallowVocabCache As VocabCache(Of ShallowSequenceElement)
			Get
				Return MyBase.getShallowVocabCache()
			End Get
		End Property


		Protected Friend Overrides Sub validateConfiguration()
			MyBase.validateConfiguration()

			If configuration.getTokenizerFactory() Is Nothing Then
				Throw New DL4JInvalidConfigException("TokenizerFactory is undefined. Can't train Word2Vec without it.")
			End If
		End Sub

		''' <summary>
		''' PLEASE NOTE: This method isn't supported for Spark implementation. Consider using fitLists() or fitSequences() instead.
		''' </summary>
		<Obsolete>
		Public Overrides Sub fit()
			Throw New System.NotSupportedException("To use fit() method, please consider using standalone implementation")
		End Sub

		Public Overridable Sub fitSentences(ByVal sentences As JavaRDD(Of String))
			''' <summary>
			''' Basically all we want here is tokenization, to get JavaRDD<Sequence<VocabWord>> out of Strings, and then we just go  for SeqVec
			''' </summary>

			validateConfiguration()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.api.java.JavaSparkContext context = new org.apache.spark.api.java.JavaSparkContext(sentences.context());
			Dim context As New JavaSparkContext(sentences.context())

			broadcastEnvironment(context)

			Dim seqRdd As JavaRDD(Of Sequence(Of VocabWord)) = sentences.map(New TokenizerFunction(configurationBroadcast))

			' now since we have new rdd - just pass it to SeqVec
			MyBase.fitSequences(seqRdd)
		End Sub


		Public Class Builder
			Inherits SparkSequenceVectors.Builder(Of VocabWord)

			''' <summary>
			''' This method should NOT be used in real world applications
			''' </summary>
			<Obsolete>
			Public Sub New()
				MyBase.New()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VoidConfiguration psConfiguration)
			Public Sub New(ByVal psConfiguration As VoidConfiguration)
				MyBase.New(psConfiguration)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VoidConfiguration psConfiguration, @NonNull VectorsConfiguration configuration)
			Public Sub New(ByVal psConfiguration As VoidConfiguration, ByVal configuration As VectorsConfiguration)
				MyBase.New(psConfiguration, configuration)
			End Sub

			''' <summary>
			''' This method defines tokenizer htat will be used for corpus tokenization
			''' </summary>
			''' <param name="tokenizerFactory">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setTokenizerFactory(@NonNull TokenizerFactory tokenizerFactory)
			Public Overridable Function setTokenizerFactory(ByVal tokenizerFactory As TokenizerFactory) As Builder
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				configuration.setTokenizerFactory(tokenizerFactory.GetType().FullName)
				If tokenizerFactory.getTokenPreProcessor() IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					configuration.setTokenPreProcessor(tokenizerFactory.getTokenPreProcessor().GetType().FullName)
				End If

				Return Me
			End Function


			''' <summary>
			''' This method defines the learning algorithm that will be used during training
			''' </summary>
			''' <param name="ela">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setLearningAlgorithm(@NonNull SparkElementsLearningAlgorithm ela)
			Public Overridable Function setLearningAlgorithm(ByVal ela As SparkElementsLearningAlgorithm) As Builder
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.configuration.setElementsLearningAlgorithm(ela.GetType().FullName)
				Return Me
			End Function

			''' <summary>
			''' This method defines the way model will be exported after training is finished
			''' </summary>
			''' <param name="exporter">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setModelExporter(@NonNull SparkModelExporter<org.deeplearning4j.models.word2vec.VocabWord> exporter)
			Public Overridable Overloads Function setModelExporter(ByVal exporter As SparkModelExporter(Of VocabWord)) As Builder
				Me.modelExporter_Conflict = exporter
				Return Me
			End Function

			''' 
			''' 
			''' <param name="numWorkers">
			''' @return </param>
			Public Overrides Function workers(ByVal numWorkers As Integer) As Builder
				MyBase.workers(numWorkers)
				Return Me
			End Function


			Public Overrides Function epochs(ByVal numEpochs As Integer) As Builder
				MyBase.epochs(numEpochs)
				Return Me
			End Function

			Public Overrides Function setStorageLevel(ByVal level As StorageLevel) As Builder
				MyBase.StorageLevel = level
				Return Me
			End Function

			Public Overrides Function minWordFrequency(ByVal num As Integer) As Builder
				MyBase.minWordFrequency(num)
				Return Me
			End Function

			Public Overrides Function setLearningRate(ByVal lr As Double) As Builder
				MyBase.LearningRate = lr
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder setParameterServerConfiguration(@NonNull VoidConfiguration configuration)
			Public Overrides Function setParameterServerConfiguration(ByVal configuration As VoidConfiguration) As Builder
				MyBase.ParameterServerConfiguration = configuration
				Return Me
			End Function

			Public Overrides Function iterations(ByVal num As Integer) As Builder
				MyBase.iterations(num)
				Return Me
			End Function

			Public Overrides Function subsampling(ByVal rate As Double) As Builder
				MyBase.subsampling(rate)
				Return Me
			End Function

			Public Overrides Function negativeSampling(ByVal samples As Long) As Builder
				MyBase.negativeSampling(samples)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder setElementsLearningAlgorithm(@NonNull SparkElementsLearningAlgorithm ela)
			Public Overrides Function setElementsLearningAlgorithm(ByVal ela As SparkElementsLearningAlgorithm) As Builder
				MyBase.ElementsLearningAlgorithm = ela
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder setSequenceLearningAlgorithm(@NonNull SparkSequenceLearningAlgorithm sla)
			Public Overrides Function setSequenceLearningAlgorithm(ByVal sla As SparkSequenceLearningAlgorithm) As Builder
				Throw New System.NotSupportedException("This method isn't supported by Word2Vec")
			End Function

			Public Overrides Function useHierarchicSoftmax(ByVal reallyUse As Boolean) As Builder
				MyBase.useHierarchicSoftmax(reallyUse)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter layerSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function layerSize(ByVal layerSize_Conflict As Integer) As Builder
				MyBase.layerSize(layerSize_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method returns you SparkWord2Vec instance ready for training
			''' 
			''' @return
			''' </summary>
			Public Overrides Function build() As SparkWord2Vec
				Dim sw2v As New SparkWord2Vec(peersConfiguration, configuration)
				sw2v.exporter = Me.modelExporter_Conflict
				sw2v.storageLevel = Me.storageLevel_Conflict
				sw2v.workers = Me.workers_Conflict

				Return sw2v
			End Function
		End Class
	End Class

End Namespace