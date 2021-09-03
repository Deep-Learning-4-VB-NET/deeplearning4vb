Imports System
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.embeddings
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports SparkElementsLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkElementsLearningAlgorithm

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


	Public MustInherit Class BaseSparkLearningAlgorithm
		Implements SparkElementsLearningAlgorithm

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public abstract org.nd4j.parameterserver.distributed.messages.Frame<JavaToDotNetGenericWildcard As org.nd4j.parameterserver.distributed.messages.TrainingMessage> frameSequence(org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement> sequence, java.util.concurrent.atomic.AtomicLong nextRandom, Double learningRate);
		Public MustOverride Function frameSequence(ByVal sequence As Sequence(Of ShallowSequenceElement), ByVal nextRandom As AtomicLong, ByVal learningRate As Double) As org.nd4j.parameterserver.distributed.messages.Frame(Of org.nd4j.parameterserver.distributed.messages.TrainingMessage)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public abstract org.nd4j.parameterserver.distributed.training.TrainingDriver<JavaToDotNetGenericWildcard As org.nd4j.parameterserver.distributed.messages.TrainingMessage> getTrainingDriver();
		Public MustOverride ReadOnly Property TrainingDriver As org.nd4j.parameterserver.distributed.training.TrainingDriver(Of org.nd4j.parameterserver.distributed.messages.TrainingMessage) Implements SparkElementsLearningAlgorithm.getTrainingDriver
		<NonSerialized>
		Protected Friend vocabCache As VocabCache(Of ShallowSequenceElement)
		<NonSerialized>
		Protected Friend vectorsConfiguration As VectorsConfiguration
		<NonSerialized>
		Protected Friend nextRandom As AtomicLong

		Protected Friend Sub New()

		End Sub

		Public Overridable Function learnSequence(ByVal sequence As Sequence(Of ShallowSequenceElement), ByVal nextRandom As AtomicLong, ByVal learningRate As Double) As Double
			' no-op
			Return 0
		End Function

		Public Overridable Sub configure(ByVal vocabCache As VocabCache(Of ShallowSequenceElement), ByVal lookupTable As WeightLookupTable(Of ShallowSequenceElement), ByVal configuration As VectorsConfiguration)
			Me.vocabCache = vocabCache
			Me.vectorsConfiguration = configuration
		End Sub

		Public Overridable Sub pretrain(ByVal iterator As SequenceIterator(Of ShallowSequenceElement))
			' no-op
		End Sub

		Public Overridable ReadOnly Property EarlyTerminationHit As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub finish()
			' no-op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement> applySubsampling(@NonNull Sequence<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement> sequence, @NonNull AtomicLong nextRandom, long totalElementsCount, double prob)
		Public Shared Function applySubsampling(ByVal sequence As Sequence(Of ShallowSequenceElement), ByVal nextRandom As AtomicLong, ByVal totalElementsCount As Long, ByVal prob As Double) As Sequence(Of ShallowSequenceElement)
			Dim result As New Sequence(Of ShallowSequenceElement)()

			' subsampling implementation, if subsampling threshold met, just continue to next element
			If prob > 0 Then
				result.setSequenceId(sequence.getSequenceId())
				If sequence.getSequenceLabels() IsNot Nothing Then
					result.SequenceLabels = sequence.getSequenceLabels()
				End If
				If sequence.getSequenceLabel() IsNot Nothing Then
					result.SequenceLabel = sequence.getSequenceLabel()
				End If

				For Each element As ShallowSequenceElement In sequence.getElements()
					Dim numWords As Double = CDbl(totalElementsCount)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim ran As Double = (Math.Sqrt(element.getElementFrequency() / (prob * numWords)) + 1) * (prob * numWords) / element.getElementFrequency()

					nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))

					If ran < (nextRandom.get() And &HFFFF) / CDbl(65536) Then
						Continue For
					End If
					result.addElement(element)
				Next element
				Return result
			Else
				Return sequence
			End If
		End Function
	End Class

End Namespace