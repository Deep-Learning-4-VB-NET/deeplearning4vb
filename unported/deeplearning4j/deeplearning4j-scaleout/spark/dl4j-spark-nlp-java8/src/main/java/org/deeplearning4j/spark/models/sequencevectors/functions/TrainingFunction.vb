Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports SparkElementsLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkElementsLearningAlgorithm
Imports SparkSequenceLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkSequenceLearningAlgorithm
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports VoidParameterServer = org.nd4j.parameterserver.distributed.VoidParameterServer
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports org.nd4j.parameterserver.distributed.training
Imports RoutedTransport = org.nd4j.parameterserver.distributed.transport.RoutedTransport

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

Namespace org.deeplearning4j.spark.models.sequencevectors.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TrainingFunction<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.apache.spark.api.java.function.VoidFunction<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>>
	Public Class TrainingFunction(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements VoidFunction(Of Sequence(Of T))

		Protected Friend vocabCacheBroadcast As Broadcast(Of VocabCache(Of ShallowSequenceElement))
		Protected Friend configurationBroadcast As Broadcast(Of VectorsConfiguration)
		Protected Friend paramServerConfigurationBroadcast As Broadcast(Of VoidConfiguration)

		<NonSerialized>
		Protected Friend paramServer As VoidParameterServer
		<NonSerialized>
		Protected Friend vectorsConfiguration As VectorsConfiguration

		<NonSerialized>
		Protected Friend elementsLearningAlgorithm As SparkElementsLearningAlgorithm
		<NonSerialized>
		Protected Friend sequenceLearningAlgorithm As SparkSequenceLearningAlgorithm
		<NonSerialized>
		Protected Friend shallowVocabCache As VocabCache(Of ShallowSequenceElement)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected transient org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> driver;
		<NonSerialized>
		Protected Friend driver As TrainingDriver(Of TrainingMessage)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TrainingFunction(@NonNull Broadcast<org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement>> vocabCacheBroadcast, @NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> vectorsConfigurationBroadcast, @NonNull Broadcast<org.nd4j.parameterserver.distributed.conf.VoidConfiguration> paramServerConfigurationBroadcast)
		Public Sub New(ByVal vocabCacheBroadcast As Broadcast(Of VocabCache(Of ShallowSequenceElement)), ByVal vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration), ByVal paramServerConfigurationBroadcast As Broadcast(Of VoidConfiguration))
			Me.vocabCacheBroadcast = vocabCacheBroadcast
			Me.configurationBroadcast = vectorsConfigurationBroadcast
			Me.paramServerConfigurationBroadcast = paramServerConfigurationBroadcast
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void call(org.deeplearning4j.models.sequencevectors.sequence.Sequence<T> sequence) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overrides Sub [call](ByVal sequence As Sequence(Of T))
			''' <summary>
			''' Depending on actual training mode, we'll either go for SkipGram/CBOW/PV-DM/PV-DBOW or whatever
			''' </summary>
			If vectorsConfiguration Is Nothing Then
				vectorsConfiguration = configurationBroadcast.getValue()
			End If

			Dim elementsLearningAlgorithm As String = vectorsConfiguration.getElementsLearningAlgorithm()
			If paramServer Is Nothing Then
				paramServer = VoidParameterServer.Instance

				If Me.elementsLearningAlgorithm Is Nothing Then
					Me.elementsLearningAlgorithm = DL4JClassLoading.createNewInstance(elementsLearningAlgorithm)
				End If

				driver = Me.elementsLearningAlgorithm.getTrainingDriver()

				' FIXME: init line should probably be removed, basically init happens in VocabRddFunction
				paramServer.init(paramServerConfigurationBroadcast.getValue(), New RoutedTransport(), driver)
			End If

			If vectorsConfiguration Is Nothing Then
				vectorsConfiguration = configurationBroadcast.getValue()
			End If

			If shallowVocabCache Is Nothing Then
				shallowVocabCache = vocabCacheBroadcast.getValue()
			End If


			If Me.elementsLearningAlgorithm Is Nothing AndAlso elementsLearningAlgorithm IsNot Nothing Then
				' TODO: do ELA initialization
				Me.elementsLearningAlgorithm = DL4JClassLoading.createNewInstance(elementsLearningAlgorithm)
				Me.elementsLearningAlgorithm.configure(shallowVocabCache, Nothing, vectorsConfiguration)
			End If

			Dim sequenceLearningAlgorithm As String = vectorsConfiguration.getSequenceLearningAlgorithm()
			If Me.sequenceLearningAlgorithm Is Nothing AndAlso sequenceLearningAlgorithm IsNot Nothing Then
				' TODO: do SLA initialization
				Me.sequenceLearningAlgorithm = DL4JClassLoading.createNewInstance(sequenceLearningAlgorithm)
				Me.sequenceLearningAlgorithm.configure(shallowVocabCache, Nothing, vectorsConfiguration)
			End If

			If Me.elementsLearningAlgorithm Is Nothing AndAlso Me.sequenceLearningAlgorithm Is Nothing Then
				Throw New ND4JIllegalStateException("No LearningAlgorithms specified!")
			End If

	'        
	'         at this moment we should have everything ready for actual initialization
	'         the only limitation we have - our sequence is detached from actual vocabulary, so we need to merge it back virtually
	'        
			Dim mergedSequence As New Sequence(Of ShallowSequenceElement)()
			For Each element As T In sequence.getElements()
				' it's possible to get null here, i.e. if frequency for this element is below minWordFrequency threshold
				Dim reduced As ShallowSequenceElement = shallowVocabCache.tokenFor(element.getStorageId())

				If reduced IsNot Nothing Then
					mergedSequence.addElement(reduced)
				End If
			Next element

			' do the same with labels, transfer them, if any
			If Me.sequenceLearningAlgorithm IsNot Nothing AndAlso vectorsConfiguration.isTrainSequenceVectors() Then
				For Each label As T In sequence.getSequenceLabels()
					Dim reduced As ShallowSequenceElement = shallowVocabCache.tokenFor(label.getStorageId())

					If reduced IsNot Nothing Then
						mergedSequence.addSequenceLabel(reduced)
					End If
				Next label
			End If


			' now we have shallow sequence, which we'll use for training
			''' <summary>
			''' All we want here, is uniform way to do training, that's matching both standalone and spark codebase.
			''' So we need some neat method, that takes sequence as input, and returns **something** that's either used for aggregation, or for ParamServer message
			''' </summary>
			' FIXME: temporary hook
			If sequence.size() > 0 Then
				paramServer.execDistributed(Me.elementsLearningAlgorithm.frameSequence(mergedSequence, New AtomicLong(119), 25e-3))
			Else
				log.warn("Skipping empty sequence...")
			End If

		End Sub
	End Class

End Namespace