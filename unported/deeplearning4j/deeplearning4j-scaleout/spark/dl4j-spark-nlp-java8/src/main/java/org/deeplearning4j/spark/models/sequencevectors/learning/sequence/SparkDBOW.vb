Imports System
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports BaseSparkLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.elements.BaseSparkLearningAlgorithm
Imports SparkSkipGram = org.deeplearning4j.spark.models.sequencevectors.learning.elements.SparkSkipGram
Imports BasicSequenceProvider = org.nd4j.parameterserver.distributed.logic.sequence.BasicSequenceProvider
Imports org.nd4j.parameterserver.distributed.messages
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports org.nd4j.parameterserver.distributed.training

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

Namespace org.deeplearning4j.spark.models.sequencevectors.learning.sequence


	Public Class SparkDBOW
		Inherits SparkSkipGram

		Public Overrides ReadOnly Property CodeName As String
			Get
				Return "Spark-DBOW"
			End Get
		End Property


'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: @Override public org.nd4j.parameterserver.distributed.messages.Frame<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> frameSequence(org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement> sequence, java.util.concurrent.atomic.AtomicLong nextRandom, double learningRate)
		Public Overridable Overloads Function frameSequence(ByVal sequence As Sequence(Of ShallowSequenceElement), ByVal nextRandom As AtomicLong, ByVal learningRate As Double) As Frame(Of TrainingMessage)
			If vectorsConfiguration.getSampling() > 0 Then
				sequence = BaseSparkLearningAlgorithm.applySubsampling(sequence, nextRandom, 10L, vectorsConfiguration.getSampling())
			End If

			Dim currentWindow As Integer = vectorsConfiguration.getWindow()

			If vectorsConfiguration.getVariableWindows() IsNot Nothing AndAlso vectorsConfiguration.getVariableWindows().length <> 0 Then
				currentWindow = vectorsConfiguration.getVariableWindows()(RandomUtils.nextInt(0, vectorsConfiguration.getVariableWindows().length))
			End If
			If frame Is Nothing Then
				SyncLock Me
					If frame Is Nothing Then
						frame = New ThreadLocal(Of Frame(Of SkipGramRequestMessage))()
					End If
				End SyncLock
			End If

			If frame.get() Is Nothing Then
				frame.set(New Frame(Of SkipGramRequestMessage)(BasicSequenceProvider.Instance.getNextValue()))
			End If

			For Each lastWord As ShallowSequenceElement In sequence.getSequenceLabels()
				For Each word As ShallowSequenceElement In sequence.getElements()
					iterateSample(word, lastWord, nextRandom, learningRate)
					nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
				Next word
			Next lastWord

			' at this moment we should have something in ThreadLocal Frame, so we'll send it to VoidParameterServer for processing

			Dim currentFrame As Frame(Of SkipGramRequestMessage) = frame.get()
			frame.set(New Frame(Of SkipGramRequestMessage)(BasicSequenceProvider.Instance.getNextValue()))

			Return currentFrame
		End Function

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: @Override public org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> getTrainingDriver()
		Public Overrides ReadOnly Property TrainingDriver As TrainingDriver(Of TrainingMessage)
			Get
				Return driver
			End Get
		End Property
	End Class

End Namespace