Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Accumulator = org.apache.spark.Accumulator
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports Huffman = org.deeplearning4j.models.word2vec.Huffman
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports org.deeplearning4j.spark.models.sequencevectors.functions
Imports SparkElementsLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkElementsLearningAlgorithm
Imports SparkSequenceLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkSequenceLearningAlgorithm
Imports org.deeplearning4j.spark.models.sequencevectors.primitives
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports VoidParameterServer = org.nd4j.parameterserver.distributed.VoidParameterServer
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports FaultToleranceStrategy = org.nd4j.parameterserver.distributed.enums.FaultToleranceStrategy
Imports RoutedTransport = org.nd4j.parameterserver.distributed.transport.RoutedTransport
Imports NetworkInformation = org.nd4j.parameterserver.distributed.util.NetworkInformation
Imports NetworkOrganizer = org.nd4j.parameterserver.distributed.util.NetworkOrganizer

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

Namespace org.deeplearning4j.spark.models.sequencevectors


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SparkSequenceVectors<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> extends org.deeplearning4j.models.sequencevectors.SequenceVectors<T>
	<Serializable>
	Public Class SparkSequenceVectors(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Inherits SequenceVectors(Of T)

		Protected Friend elementsFreqAccum As Accumulator(Of Counter(Of Long))
		Protected Friend elementsFreqAccumExtra As Accumulator(Of ExtraCounter(Of Long))
		Protected Friend storageLevel As StorageLevel = StorageLevel.MEMORY_ONLY()


		' FIXME: we probably do not need this at all
		Protected Friend vocabCacheBroadcast As Broadcast(Of VocabCache(Of T))

		Protected Friend shallowVocabCacheBroadcast As Broadcast(Of VocabCache(Of ShallowSequenceElement))
		Protected Friend configurationBroadcast As Broadcast(Of VectorsConfiguration)

		<NonSerialized>
		Protected Friend isEnvironmentReady As Boolean = False
		<NonSerialized>
		Protected Friend shallowVocabCache As VocabCache(Of ShallowSequenceElement)
		Protected Friend isAutoDiscoveryMode As Boolean = True

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.deeplearning4j.spark.models.sequencevectors.export.SparkModelExporter<T> exporter;
		Protected Friend exporter As SparkModelExporter(Of T)

		Protected Friend ela As SparkElementsLearningAlgorithm
		Protected Friend sla As SparkSequenceLearningAlgorithm

		Protected Friend paramServerConfiguration As VoidConfiguration

		Protected Friend Sub New()
			Me.New(New VectorsConfiguration())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected SparkSequenceVectors(@NonNull VectorsConfiguration configuration)
		Protected Friend Sub New(ByVal configuration As VectorsConfiguration)
			Me.configuration = configuration
		End Sub

		Protected Friend Overridable ReadOnly Property ShallowVocabCache As VocabCache(Of ShallowSequenceElement)
			Get
				Return shallowVocabCache
			End Get
		End Property


		''' <summary>
		''' PLEASE NOTE: This method isn't supported for Spark implementation. Consider using fitLists() or fitSequences() instead.
		''' </summary>
		<Obsolete>
		Public Overrides Sub fit()
			Throw New System.NotSupportedException("To use fit() method, please consider using standalone implementation")
		End Sub

		Protected Friend Overridable Sub validateConfiguration()
			If Not configuration.isUseHierarchicSoftmax() AndAlso configuration.getNegative() = 0 Then
				Throw New DL4JInvalidConfigException("Both HierarchicSoftmax and NegativeSampling are disabled. Nothing to learn here.")
			End If

			If configuration.getElementsLearningAlgorithm() Is Nothing AndAlso configuration.getSequenceLearningAlgorithm() Is Nothing Then
				Throw New DL4JInvalidConfigException("No LearningAlgorithm was set. Nothing to learn here.")
			End If

			If exporter Is Nothing Then
				Throw New DL4JInvalidConfigException("SparkModelExporter is undefined. No sense for training, if model won't be exported.")
			End If
		End Sub

		Protected Friend Overridable Sub broadcastEnvironment(ByVal context As JavaSparkContext)
			If Not isEnvironmentReady Then
				configurationBroadcast = context.broadcast(configuration)

				isEnvironmentReady = True
			End If
		End Sub

		''' <summary>
		''' Utility method. fitSequences() used within.
		''' 
		''' PLEASE NOTE: This method can't be used to train for labels, since List<T> can't hold labels. If you need labels - consider manual Sequence creation instead.
		''' </summary>
		''' <param name="corpus"> </param>
		Public Overridable Sub fitLists(ByVal corpus As JavaRDD(Of IList(Of T)))
			' we just convert List to sequences
			Dim rdd As JavaRDD(Of Sequence(Of T)) = corpus.map(New ListSequenceConvertFunction(Of T)())

			' and use fitSequences()
			fitSequences(rdd)
		End Sub

		''' <summary>
		''' Base training entry point
		''' </summary>
		''' <param name="corpus"> </param>
		Public Overridable Sub fitSequences(ByVal corpus As JavaRDD(Of Sequence(Of T)))
			''' <summary>
			''' Basically all we want for base implementation here is 3 things:
			''' a) build vocabulary
			''' b) build huffman tree
			''' c) do training
			''' 
			''' in this case all classes extending SeqVec, like deepwalk or word2vec will be just building their RDD<Sequence<T>>,
			''' and calling this method for training, instead implementing own routines
			''' </summary>

			validateConfiguration()

			If ela Is Nothing Then
				Dim className As String = configuration.getElementsLearningAlgorithm()
				ela = DL4JClassLoading.createNewInstance(className)
			End If

			If workers > 1 Then
				log.info("Repartitioning corpus to {} parts...", workers)
				corpus.repartition(workers)
			End If

			If storageLevel IsNot Nothing Then
				corpus.persist(storageLevel)
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.api.java.JavaSparkContext sc = new org.apache.spark.api.java.JavaSparkContext(corpus.context());
			Dim sc As New JavaSparkContext(corpus.context())

			' this will have any effect only if wasn't called before, in extension classes
			broadcastEnvironment(sc)

			Dim finalCounter As Counter(Of Long)
			Dim numberOfSequences As Long = 0

			''' <summary>
			''' Here we s
			''' </summary>
			If paramServerConfiguration Is Nothing Then
				paramServerConfiguration = VoidConfiguration.builder().numberOfShards(2).unicastPort(40123).multicastPort(40124).build()
				paramServerConfiguration.setFaultToleranceStrategy(FaultToleranceStrategy.NONE)
			End If

			isAutoDiscoveryMode = If(paramServerConfiguration.getShardAddresses() IsNot Nothing AndAlso Not paramServerConfiguration.getShardAddresses().isEmpty(), False, True)

			Dim paramServerConfigurationBroadcast As Broadcast(Of VoidConfiguration) = Nothing

			If isAutoDiscoveryMode Then
				log.info("Trying auto discovery mode...")

				elementsFreqAccumExtra = corpus.context().accumulator(New ExtraCounter(Of Long)(), New ExtraElementsFrequenciesAccumulator())

				Dim elementsCounter As New ExtraCountFunction(Of T)(elementsFreqAccumExtra, configuration.isTrainSequenceVectors())

				Dim countedCorpus As JavaRDD(Of Pair(Of Sequence(Of T), Long)) = corpus.map(elementsCounter)

				' just to trigger map function, since we need huffman tree before proceeding
				numberOfSequences = countedCorpus.count()

				finalCounter = elementsFreqAccumExtra.value()

				Dim spareReference As ExtraCounter(Of Long) = DirectCast(finalCounter, ExtraCounter(Of Long))

				' getting list of available hosts
				Dim availableHosts As ISet(Of NetworkInformation) = spareReference.getNetworkInformation()

				log.info("availableHosts: {}", availableHosts)
				If availableHosts.Count > 1 Then
					' now we have to pick N shards and optionally N backup nodes, and pass them within configuration bean
					Dim organizer As New NetworkOrganizer(availableHosts, paramServerConfiguration.NetworkMask)

					paramServerConfiguration.setShardAddresses(organizer.getSubset(paramServerConfiguration.getNumberOfShards()))

					' backup shards are optional
					If paramServerConfiguration.getFaultToleranceStrategy() <> FaultToleranceStrategy.NONE Then
						paramServerConfiguration.setBackupAddresses(organizer.getSubset(paramServerConfiguration.getNumberOfShards(), paramServerConfiguration.getShardAddresses()))
					End If
				Else
					' for single host (aka driver-only, aka spark-local) just run on loopback interface
					paramServerConfiguration.setShardAddresses(Arrays.asList("127.0.0.1:" & paramServerConfiguration.getPortSupplier().getPort()))
					paramServerConfiguration.setFaultToleranceStrategy(FaultToleranceStrategy.NONE)
				End If



				log.info("Got Shards so far: {}", paramServerConfiguration.getShardAddresses())

				' update ps configuration with real values where required
				paramServerConfiguration.setNumberOfShards(paramServerConfiguration.getShardAddresses().size())
				paramServerConfiguration.setUseHS(configuration.isUseHierarchicSoftmax())
				paramServerConfiguration.setUseNS(configuration.getNegative() > 0)

				paramServerConfigurationBroadcast = sc.broadcast(paramServerConfiguration)

			Else

				' update ps configuration with real values where required
				paramServerConfiguration.setNumberOfShards(paramServerConfiguration.getShardAddresses().size())
				paramServerConfiguration.setUseHS(configuration.isUseHierarchicSoftmax())
				paramServerConfiguration.setUseNS(configuration.getNegative() > 0)

				paramServerConfigurationBroadcast = sc.broadcast(paramServerConfiguration)


				' set up freqs accumulator
				elementsFreqAccum = corpus.context().accumulator(New Counter(Of Long)(), New ElementsFrequenciesAccumulator())
				Dim elementsCounter As New CountFunction(Of T)(configurationBroadcast, paramServerConfigurationBroadcast, elementsFreqAccum, configuration.isTrainSequenceVectors())

				' count all sequence elements and their sum
				Dim countedCorpus As JavaRDD(Of Pair(Of Sequence(Of T), Long)) = corpus.map(elementsCounter)

				' just to trigger map function, since we need huffman tree before proceeding
				numberOfSequences = countedCorpus.count()

				' now we grab counter, which contains frequencies for all SequenceElements in corpus
				finalCounter = elementsFreqAccum.value()
			End If

			Dim numberOfElements As Long = CLng(Math.Truncate(finalCounter.totalCount()))

			Dim numberOfUniqueElements As Long = finalCounter.size()

			log.info("Total number of sequences: {}; Total number of elements entries: {}; Total number of unique elements: {}", numberOfSequences, numberOfElements, numberOfUniqueElements)

	'        
	'         build RDD of reduced SequenceElements, just get rid of labels temporary, stick to some numerical values,
	'         like index or hashcode. So we could reduce driver memory footprint
	'         


			' build huffman tree, and update original RDD with huffman encoding info
			shallowVocabCache = buildShallowVocabCache(finalCounter)
			shallowVocabCacheBroadcast = sc.broadcast(shallowVocabCache)

			' FIXME: probably we need to reconsider this approach
			Dim vocabRDD As JavaRDD(Of T) = corpus.flatMap(New VocabRddFunctionFlat(Of T)(configurationBroadcast, paramServerConfigurationBroadcast)).distinct()
			vocabRDD.count()

			''' <summary>
			''' now we initialize Shards with values. That call should be started from driver which is either Client or Shard in standalone mode.
			''' </summary>
			VoidParameterServer.Instance.init(paramServerConfiguration, New RoutedTransport(), ela.getTrainingDriver())
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			VoidParameterServer.Instance.initializeSeqVec(configuration.getLayersSize(), CInt(numberOfUniqueElements), 119, configuration.getLayersSize() / paramServerConfiguration.getNumberOfShards(), paramServerConfiguration.isUseHS(), paramServerConfiguration.isUseNS())

			' proceed to training
			' also, training function is the place where we invoke ParameterServer
			Dim trainer As New TrainingFunction(Of T)(shallowVocabCacheBroadcast, configurationBroadcast, paramServerConfigurationBroadcast)
			Dim partitionTrainer As New PartitionTrainingFunction(Of T)(shallowVocabCacheBroadcast, configurationBroadcast, paramServerConfigurationBroadcast)

			If configuration IsNot Nothing Then
				Dim e As Integer = 0
				Do While e < configuration.getEpochs()
					corpus.foreachPartition(partitionTrainer)
					e += 1
				Loop
			End If
			'corpus.foreach(trainer);


			' we're transferring vectors to ExportContainer
			Dim exportRdd As JavaRDD(Of ExportContainer(Of T)) = vocabRDD.map(New DistributedFunction(Of T)(paramServerConfigurationBroadcast, configurationBroadcast, shallowVocabCacheBroadcast))

			' at this particular moment training should be pretty much done, and we're good to go for export
			If exporter IsNot Nothing Then
				exporter.export(exportRdd)
			End If

			' unpersist, if we've persisten corpus after all
			If storageLevel IsNot Nothing Then
				corpus.unpersist()
			End If

			log.info("Training finish, starting cleanup...")
			VoidParameterServer.Instance.shutdown()
		End Sub

		''' <summary>
		''' This method builds shadow vocabulary and huffman tree
		''' </summary>
		''' <param name="counter">
		''' @return </param>
		Protected Friend Overridable Function buildShallowVocabCache(ByVal counter As Counter(Of Long)) As VocabCache(Of ShallowSequenceElement)

			' TODO: need simplified cache here, that will operate on Long instead of string labels
			Dim vocabCache As VocabCache(Of ShallowSequenceElement) = New AbstractCache(Of ShallowSequenceElement)()
			For Each id As Long? In counter.keySet()
				Dim shallowElement As New ShallowSequenceElement(counter.getCount(id), id)
				vocabCache.addToken(shallowElement)
			Next id

			' building huffman tree
			Dim huffman As New Huffman(vocabCache.vocabWords())
			huffman.build()
			huffman.applyIndexes(vocabCache)

			Return vocabCache
		End Function

		Protected Friend Overridable ReadOnly Property Counter As Counter(Of Long)
			Get
				If isAutoDiscoveryMode Then
					Return elementsFreqAccumExtra.value()
				Else
					Return elementsFreqAccum.value()
				End If
			End Get
		End Property


		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
			Protected Friend configuration As VectorsConfiguration
'JAVA TO VB CONVERTER NOTE: The field modelExporter was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend modelExporter_Conflict As SparkModelExporter(Of T)
			Protected Friend peersConfiguration As VoidConfiguration
'JAVA TO VB CONVERTER NOTE: The field workers was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend workers_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field storageLevel was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend storageLevel_Conflict As StorageLevel

			''' <summary>
			''' This method should NOT be used in real world environment
			''' </summary>
			<Obsolete>
			Public Sub New()
				Me.New(New VoidConfiguration(), New VectorsConfiguration())
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VoidConfiguration psConfiguration)
			Public Sub New(ByVal psConfiguration As VoidConfiguration)
				Me.New(psConfiguration, New VectorsConfiguration())
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VoidConfiguration psConfiguration, @NonNull VectorsConfiguration w2vConfiguration)
			Public Sub New(ByVal psConfiguration As VoidConfiguration, ByVal w2vConfiguration As VectorsConfiguration)
				Me.configuration = w2vConfiguration
				Me.peersConfiguration = psConfiguration
			End Sub

			''' 
			''' <param name="level">
			''' @return </param>
			Public Overridable Function setStorageLevel(ByVal level As StorageLevel) As Builder(Of T)
				storageLevel_Conflict = level
				Return Me
			End Function

			''' 
			''' <param name="num">
			''' @return </param>
			Public Overridable Function minWordFrequency(ByVal num As Integer) As Builder(Of T)
				configuration.setMinWordFrequency(num)
				Return Me
			End Function

			''' 
			''' <param name="num">
			''' @return </param>
			Public Overridable Function workers(ByVal num As Integer) As Builder(Of T)
				Me.workers_Conflict = num
				Return Me
			End Function

			''' 
			''' <param name="lr">
			''' @return </param>
			Public Overridable Function setLearningRate(ByVal lr As Double) As Builder(Of T)
				configuration.setLearningRate(lr)
				Return Me
			End Function

			''' 
			''' <param name="configuration">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setParameterServerConfiguration(@NonNull VoidConfiguration configuration)
			Public Overridable Function setParameterServerConfiguration(ByVal configuration As VoidConfiguration) As Builder(Of T)
				peersConfiguration = configuration
				Return Me
			End Function

			''' 
			''' <param name="modelExporter">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setModelExporter(@NonNull SparkModelExporter<T> modelExporter)
			Public Overridable Function setModelExporter(ByVal modelExporter As SparkModelExporter(Of T)) As Builder(Of T)
				Me.modelExporter_Conflict = modelExporter
				Return Me
			End Function

			''' 
			''' <param name="num">
			''' @return </param>
			Public Overridable Function epochs(ByVal num As Integer) As Builder(Of T)
				configuration.setEpochs(num)
				Return Me
			End Function

			''' 
			''' <param name="num">
			''' @return </param>
			Public Overridable Function iterations(ByVal num As Integer) As Builder(Of T)
				configuration.setIterations(num)
				Return Me
			End Function

			''' 
			''' <param name="rate">
			''' @return </param>
			Public Overridable Function subsampling(ByVal rate As Double) As Builder(Of T)
				configuration.setSampling(rate)
				Return Me
			End Function

			''' 
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function useHierarchicSoftmax(ByVal reallyUse As Boolean) As Builder(Of T)
				configuration.setUseHierarchicSoftmax(reallyUse)
				Return Me
			End Function

			''' 
			''' <param name="samples">
			''' @return </param>
			Public Overridable Function negativeSampling(ByVal samples As Long) As Builder(Of T)
				configuration.setNegative(CDbl(samples))
				Return Me
			End Function

			''' 
			''' <param name="ela">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setElementsLearningAlgorithm(@NonNull SparkElementsLearningAlgorithm ela)
			Public Overridable Function setElementsLearningAlgorithm(ByVal ela As SparkElementsLearningAlgorithm) As Builder(Of T)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				configuration.setElementsLearningAlgorithm(ela.GetType().FullName)
				Return Me
			End Function

			''' 
			''' <param name="sla">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setSequenceLearningAlgorithm(@NonNull SparkSequenceLearningAlgorithm sla)
			Public Overridable Function setSequenceLearningAlgorithm(ByVal sla As SparkSequenceLearningAlgorithm) As Builder(Of T)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				configuration.setSequenceLearningAlgorithm(sla.GetType().FullName)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter layerSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layerSize(ByVal layerSize_Conflict As Integer) As Builder(Of T)
				If layerSize_Conflict < 1 Then
					Throw New DL4JInvalidConfigException("LayerSize should be positive value")
				End If

				configuration.setLayersSize(layerSize_Conflict)
				Return Me
			End Function


			Public Overridable Function build() As SparkSequenceVectors(Of T)
				If modelExporter_Conflict Is Nothing Then
					Throw New System.InvalidOperationException("ModelExporter is undefined!")
				End If

				Dim seqVec As New SparkSequenceVectors(configuration)
				seqVec.exporter = modelExporter_Conflict
				seqVec.paramServerConfiguration = peersConfiguration
				seqVec.storageLevel = storageLevel_Conflict
				seqVec.workers = workers_Conflict

				Return seqVec
			End Function
		End Class
	End Class

End Namespace