Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports BasicSequenceProvider = org.nd4j.parameterserver.distributed.logic.sequence.BasicSequenceProvider
Imports org.nd4j.parameterserver.distributed.messages
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports org.nd4j.parameterserver.distributed.training
Imports SkipGramTrainer = org.nd4j.parameterserver.distributed.training.impl.SkipGramTrainer

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

Namespace org.deeplearning4j.spark.models.sequencevectors.learning.elements


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SparkSkipGram extends BaseSparkLearningAlgorithm
	Public Class SparkSkipGram
		Inherits BaseSparkLearningAlgorithm

		Public Overridable ReadOnly Property CodeName As String
			Get
				Return "Spark-SkipGram"
			End Get
		End Property

		Public Overridable Overloads Function learnSequence(ByVal sequence As Sequence(Of ShallowSequenceElement), ByVal nextRandom As AtomicLong, ByVal learningRate As Double, ByVal batchSequences As BatchSequences(Of ShallowSequenceElement)) As Double
			Throw New System.NotSupportedException()
		End Function

		<NonSerialized>
		Protected Friend counter As AtomicLong
		<NonSerialized>
		Protected Friend frame As ThreadLocal(Of Frame(Of SkipGramRequestMessage))

		Protected Friend driver As TrainingDriver(Of SkipGramRequestMessage) = New SkipGramTrainer()

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: @Override public org.nd4j.parameterserver.distributed.messages.Frame<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> frameSequence(org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement> sequence, java.util.concurrent.atomic.AtomicLong nextRandom, double learningRate)
		Public Overridable Overloads Function frameSequence(ByVal sequence As Sequence(Of ShallowSequenceElement), ByVal nextRandom As AtomicLong, ByVal learningRate As Double) As Frame(Of TrainingMessage)

			' FIXME: totalElementsCount should have real value
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

			For i As Integer = 0 To sequence.size() - 1
				nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))

				Dim word As ShallowSequenceElement = sequence.getElementByIndex(i)
				If word Is Nothing Then
					Continue For
				End If

				Dim b As Integer = CInt(Math.Truncate(nextRandom.get() Mod currentWindow))
				Dim [end] As Integer = currentWindow * 2 + 1 - b
				For a As Integer = b To [end] - 1
					If a <> currentWindow Then
						Dim c As Integer = i - currentWindow + a
						If c >= 0 AndAlso c < sequence.size() Then
							Dim lastWord As ShallowSequenceElement = sequence.getElementByIndex(c)
							iterateSample(word, lastWord, nextRandom, learningRate)
							nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
						End If
					End If
				Next a
			Next i

			' at this moment we should have something in ThreadLocal Frame, so we'll send it to VoidParameterServer for processing

			Dim currentFrame As Frame(Of SkipGramRequestMessage) = frame.get()
			frame.set(New Frame(Of SkipGramRequestMessage)(BasicSequenceProvider.Instance.getNextValue()))

			Return currentFrame
		End Function



		Protected Friend Overridable Sub iterateSample(ByVal word As ShallowSequenceElement, ByVal lastWord As ShallowSequenceElement, ByVal nextRandom As AtomicLong, ByVal lr As Double)
			If word Is Nothing OrElse lastWord Is Nothing OrElse lastWord.Index < 0 OrElse word.Index = lastWord.Index Then
				Return
			End If
			''' <summary>
			''' all we want here, is actually very simple:
			''' we just build simple SkipGram frame, and send it over network
			''' </summary>

			Dim idxSyn1(-1) As Integer
			Dim codes(-1) As SByte
			If vectorsConfiguration.isUseHierarchicSoftmax() Then
				idxSyn1 = New Integer(word.getCodeLength() - 1){}
				codes = New SByte(word.getCodeLength() - 1){}
				Dim i As Integer = 0
				Do While i < word.getCodeLength()
					Dim code As SByte = word.getCodes()(i)
					Dim point As Integer = word.getPoints()(i)
					If point >= vocabCache.numWords() OrElse point < 0 Then
						i += 1
						Continue Do
					End If

					codes(i) = code
					idxSyn1(i) = point
					i += 1
				Loop
			End If

			Dim neg As Short = CShort(Math.Truncate(vectorsConfiguration.getNegative()))
			Dim sgrm As New SkipGramRequestMessage(word.Index, lastWord.Index, idxSyn1, codes, neg, lr, nextRandom.get())

			' we just stackfor now
			frame.get().stackMessage(sgrm)
		End Sub

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: @Override public org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends org.nd4j.parameterserver.distributed.messages.TrainingMessage> getTrainingDriver()
		Public Overrides ReadOnly Property TrainingDriver As TrainingDriver(Of TrainingMessage)
			Get
				Return driver
			End Get
		End Property
	End Class

End Namespace