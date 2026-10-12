Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
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
Imports BasicSequenceProvider = org.nd4j.parameterserver.distributed.logic.sequence.BasicSequenceProvider
Imports org.nd4j.parameterserver.distributed.messages
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


	Public Class PartitionTrainingFunction(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements VoidFunction(Of IEnumerator(Of Sequence(Of T)))

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
'ORIGINAL LINE: public PartitionTrainingFunction(@NonNull Broadcast<org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement>> vocabCacheBroadcast, @NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> vectorsConfigurationBroadcast, @NonNull Broadcast<org.nd4j.parameterserver.distributed.conf.VoidConfiguration> paramServerConfigurationBroadcast)
		Public Sub New(ByVal vocabCacheBroadcast As Broadcast(Of VocabCache(Of ShallowSequenceElement)), ByVal vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration), ByVal paramServerConfigurationBroadcast As Broadcast(Of VoidConfiguration))
			Me.vocabCacheBroadcast = vocabCacheBroadcast
			Me.configurationBroadcast = vectorsConfigurationBroadcast
			Me.paramServerConfigurationBroadcast = paramServerConfigurationBroadcast
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public void call(java.util.Iterator<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>> sequenceIterator) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overrides Sub [call](ByVal sequenceIterator As IEnumerator(Of Sequence(Of T)))
			''' <summary>
			''' first we initialize
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

			If shallowVocabCache Is Nothing Then
				shallowVocabCache = vocabCacheBroadcast.getValue()
			End If

			If Me.elementsLearningAlgorithm Is Nothing AndAlso elementsLearningAlgorithm IsNot Nothing Then
				' TODO: do ELA initialization
				Me.elementsLearningAlgorithm = DL4JClassLoading.createNewInstance(elementsLearningAlgorithm)
			End If

			If Me.elementsLearningAlgorithm IsNot Nothing Then
				Me.elementsLearningAlgorithm.configure(shallowVocabCache, Nothing, vectorsConfiguration)
			End If

			Dim sequenceLearningAlgorithm As String = vectorsConfiguration.getSequenceLearningAlgorithm()
			If Me.sequenceLearningAlgorithm Is Nothing AndAlso sequenceLearningAlgorithm IsNot Nothing Then
				' TODO: do SLA initialization
				Me.sequenceLearningAlgorithm = DL4JClassLoading.createNewInstance(sequenceLearningAlgorithm)
				Me.sequenceLearningAlgorithm.configure(shallowVocabCache, Nothing, vectorsConfiguration)
			End If
			If Me.sequenceLearningAlgorithm IsNot Nothing Then
				Me.sequenceLearningAlgorithm.configure(shallowVocabCache, Nothing, vectorsConfiguration)
			End If

			If Me.elementsLearningAlgorithm Is Nothing AndAlso Me.sequenceLearningAlgorithm Is Nothing Then
				Throw New ND4JIllegalStateException("No LearningAlgorithms specified!")
			End If


			Dim sequences As IList(Of Sequence(Of ShallowSequenceElement)) = New List(Of Sequence(Of ShallowSequenceElement))()

			' now we roll throw Sequences and prepare/convert/"learn" them
			Do While sequenceIterator.MoveNext()
				Dim sequence As Sequence(Of T) = sequenceIterator.Current

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

				sequences.Add(mergedSequence)
				If sequences.Count >= 8 Then
					trainAllAtOnce(sequences)
					sequences.Clear()
				End If
			Loop

			If sequences.Count > 0 Then
				' finishing training round, to make sure we don't have trails
				trainAllAtOnce(sequences)
				sequences.Clear()
			End If
		End Sub


		Protected Friend Overridable Sub trainAllAtOnce(ByVal sequences As IList(Of Sequence(Of ShallowSequenceElement)))
			Dim bigFrame As New Frame(BasicSequenceProvider.Instance.getNextValue())

			For Each sequence As Sequence(Of ShallowSequenceElement) In sequences
				Dim frame As Frame = elementsLearningAlgorithm.frameSequence(sequence, New AtomicLong(119L), 25e-3f)
				bigFrame.stackMessages(frame.getMessages())
			Next sequence

			If bigFrame.size() > 0 Then
				paramServer.execDistributed(bigFrame)
			End If
		End Sub
	End Class

End Namespace