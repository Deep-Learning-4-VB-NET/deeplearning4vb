Imports System
Imports System.Collections.Generic
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports BaseSparkLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.elements.BaseSparkLearningAlgorithm
Imports SparkCBOW = org.deeplearning4j.spark.models.sequencevectors.learning.elements.SparkCBOW
Imports BasicSequenceProvider = org.nd4j.parameterserver.distributed.logic.sequence.BasicSequenceProvider
Imports org.nd4j.parameterserver.distributed.messages
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports CbowRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.CbowRequestMessage
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


	Public Class SparkDM
		Inherits SparkCBOW

		Public Overrides ReadOnly Property CodeName As String
			Get
				Return "Spark-DM"
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
						frame = New ThreadLocal(Of Frame(Of CbowRequestMessage))()
					End If
				End SyncLock
			End If

			If frame.get() Is Nothing Then
				frame.set(New Frame(Of CbowRequestMessage)(BasicSequenceProvider.Instance.getNextValue()))
			End If


			Dim i As Integer = 0
			Do While i < sequence.getElements().size()
				nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
				Dim b As Integer = CInt(Math.Truncate(nextRandom.get())) Mod currentWindow

				Dim [end] As Integer = currentWindow * 2 + 1 - b

				Dim currentWord As ShallowSequenceElement = sequence.getElementByIndex(i)

				Dim intsList As IList(Of Integer) = New List(Of Integer)()
				For a As Integer = b To [end] - 1
					If a <> currentWindow Then
						Dim c As Integer = i - currentWindow + a
						If c >= 0 AndAlso c < sequence.size() Then
							Dim lastWord As ShallowSequenceElement = sequence.getElementByIndex(c)

							intsList.Add(lastWord.Index)
						End If
					End If
				Next a

				' basically it's the same as CBOW, we just add labels here
				If sequence.getSequenceLabels() IsNot Nothing Then
					For Each label As ShallowSequenceElement In sequence.getSequenceLabels()
						intsList.Add(label.Index)
					Next label
				Else ' FIXME: we probably should throw this exception earlier?
					Throw New DL4JInvalidInputException("Sequence passed via RDD has no labels within, nothing to learn here")
				End If


				' just converting values to int
				Dim windowWords(intsList.Count - 1) As Integer
				For x As Integer = 0 To windowWords.Length - 1
					windowWords(x) = intsList(x)
				Next x

				If windowWords.Length < 1 Then
					i += 1
					Continue Do
				End If

				iterateSample(currentWord, windowWords, nextRandom, learningRate, False, 0, True, Nothing)
				i += 1
			Loop

			Dim currentFrame As Frame(Of CbowRequestMessage) = frame.get()
			frame.set(New Frame(Of CbowRequestMessage)(BasicSequenceProvider.Instance.getNextValue()))

			Return currentFrame
		End Function

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: @Override public org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> getTrainingDriver()
		Public Overrides ReadOnly Property TrainingDriver As TrainingDriver(Of TrainingMessage)
			Get
				Return Nothing
			End Get
		End Property
	End Class

End Namespace