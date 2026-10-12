Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Accumulator = org.apache.spark.Accumulator
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports SparkElementsLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkElementsLearningAlgorithm
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
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
'ORIGINAL LINE: @Slf4j public class CountFunction<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.apache.spark.api.java.function.@Function<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>, org.nd4j.common.primitives.Pair<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>, Long>>
	Public Class CountFunction(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements [Function](Of Sequence(Of T), Pair(Of Sequence(Of T), Long))

		Protected Friend accumulator As Accumulator(Of Counter(Of Long))
		Protected Friend fetchLabels As Boolean
		Protected Friend voidConfigurationBroadcast As Broadcast(Of VoidConfiguration)
		Protected Friend vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration)

		<NonSerialized>
		Protected Friend ela As SparkElementsLearningAlgorithm
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected transient org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> driver;
		<NonSerialized>
		Protected Friend driver As TrainingDriver(Of TrainingMessage)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CountFunction(@NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> vectorsConfigurationBroadcast, @NonNull Broadcast<org.nd4j.parameterserver.distributed.conf.VoidConfiguration> voidConfigurationBroadcast, @NonNull Accumulator<org.nd4j.common.primitives.Counter<Long>> accumulator, boolean fetchLabels)
		Public Sub New(ByVal vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration), ByVal voidConfigurationBroadcast As Broadcast(Of VoidConfiguration), ByVal accumulator As Accumulator(Of Counter(Of Long)), ByVal fetchLabels As Boolean)
			Me.accumulator = accumulator
			Me.fetchLabels = fetchLabels
			Me.voidConfigurationBroadcast = voidConfigurationBroadcast
			Me.vectorsConfigurationBroadcast = vectorsConfigurationBroadcast
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.common.primitives.Pair<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>, Long> call(org.deeplearning4j.models.sequencevectors.sequence.Sequence<T> sequence) throws Exception
		Public Overrides Function [call](ByVal sequence As Sequence(Of T)) As Pair(Of Sequence(Of T), Long)
			' since we can't be 100% sure that sequence size is ok itself, or it's not overflow through int limits, we'll recalculate it.
			' anyway we're going to loop through it for elements frequencies
			Dim localCounter As New Counter(Of Long)()
			Dim seqLen As Long = 0

			If ela Is Nothing Then
				Dim elementsLearningAlgorithm As String = vectorsConfigurationBroadcast.getValue().getElementsLearningAlgorithm()
				ela = DL4JClassLoading.createNewInstance(elementsLearningAlgorithm)
			End If
			driver = ela.getTrainingDriver()

			'System.out.println("Initializing VoidParameterServer in CountFunction");
			VoidParameterServer.Instance.init(voidConfigurationBroadcast.getValue(), New RoutedTransport(), driver)

			For Each element As T In sequence.getElements()
				If element Is Nothing Then
					Continue For
				End If

				' FIXME: hashcode is bad idea here. we need Long id
				localCounter.incrementCount(element.getStorageId(), 1.0f)
				seqLen += 1
			Next element

			' FIXME: we're missing label information here due to shallow vocab mechanics
			If sequence.getSequenceLabels() IsNot Nothing Then
				For Each label As T In sequence.getSequenceLabels()
					localCounter.incrementCount(label.getStorageId(), 1.0f)
				Next label
			End If

			accumulator.add(localCounter)

			Return Pair.makePair(sequence, seqLen)
		End Function
	End Class

End Namespace