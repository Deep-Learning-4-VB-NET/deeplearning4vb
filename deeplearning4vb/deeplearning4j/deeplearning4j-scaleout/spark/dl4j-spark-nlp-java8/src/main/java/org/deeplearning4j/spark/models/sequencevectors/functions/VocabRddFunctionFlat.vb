Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports SparkElementsLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkElementsLearningAlgorithm
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


	Public Class VocabRddFunctionFlat(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements FlatMapFunction(Of Sequence(Of T), T)

		Protected Friend vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration)
		Protected Friend paramServerConfigurationBroadcast As Broadcast(Of VoidConfiguration)

		<NonSerialized>
		Protected Friend configuration As VectorsConfiguration
		<NonSerialized>
		Protected Friend ela As SparkElementsLearningAlgorithm
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected transient org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> driver;
		<NonSerialized>
		Protected Friend driver As TrainingDriver(Of TrainingMessage)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocabRddFunctionFlat(@NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> vectorsConfigurationBroadcast, @NonNull Broadcast<org.nd4j.parameterserver.distributed.conf.VoidConfiguration> paramServerConfigurationBroadcast)
		Public Sub New(ByVal vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration), ByVal paramServerConfigurationBroadcast As Broadcast(Of VoidConfiguration))
			Me.vectorsConfigurationBroadcast = vectorsConfigurationBroadcast
			Me.paramServerConfigurationBroadcast = paramServerConfigurationBroadcast
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<T> call(org.deeplearning4j.models.sequencevectors.sequence.Sequence<T> sequence) throws Exception
		Public Overrides Function [call](ByVal sequence As Sequence(Of T)) As IEnumerator(Of T)
			If configuration Is Nothing Then
				configuration = vectorsConfigurationBroadcast.getValue()
			End If

			If ela Is Nothing Then
				Dim className As String = configuration.getElementsLearningAlgorithm()
				ela = DL4JClassLoading.createNewInstance(className)
			End If
			driver = ela.getTrainingDriver()

			' we just silently initialize server
			VoidParameterServer.Instance.init(paramServerConfigurationBroadcast.getValue(), New RoutedTransport(), driver)

			' TODO: call for initializeSeqVec here

			Dim elements As IList(Of T) = New List(Of T)()

			CType(elements, List(Of T)).AddRange(sequence.getElements())

			' FIXME: this is PROBABLY bad, we might want to ensure, there's no duplicates.
			If configuration.isTrainSequenceVectors() Then
				If Not sequence.getSequenceLabels().isEmpty() Then
					CType(elements, List(Of T)).AddRange(sequence.getSequenceLabels())
				End If
			End If

			Return elements.GetEnumerator()
		End Function
	End Class

End Namespace