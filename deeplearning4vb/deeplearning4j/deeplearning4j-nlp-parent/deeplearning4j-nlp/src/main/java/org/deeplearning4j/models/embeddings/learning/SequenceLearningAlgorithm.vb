Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.models.embeddings.learning


	Public Interface SequenceLearningAlgorithm(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)

		ReadOnly Property CodeName As String

		Sub configure(ByVal vocabCache As VocabCache(Of T), ByVal lookupTable As WeightLookupTable(Of T), ByVal configuration As VectorsConfiguration)

		Sub pretrain(ByVal iterator As SequenceIterator(Of T))

		''' <summary>
		''' This method does training over the sequence of elements passed into it
		''' </summary>
		''' <param name="sequence"> </param>
		''' <param name="nextRandom"> </param>
		''' <param name="learningRate"> </param>
		''' <returns> average score for this sequence </returns>
		Function learnSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal learningRate As Double, ByVal batchSequences As BatchSequences(Of T)) As Double

		ReadOnly Property EarlyTerminationHit As Boolean

		''' <summary>
		''' This method does training on previously unseen paragraph, and returns inferred vector
		''' </summary>
		''' <param name="sequence"> </param>
		''' <param name="nextRandom"> </param>
		''' <param name="learningRate">
		''' @return </param>
		Function inferSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As Long, ByVal learningRate As Double, ByVal minLearningRate As Double, ByVal iterations As Integer) As INDArray

		ReadOnly Property ElementsLearningAlgorithm As ElementsLearningAlgorithm(Of T)

		Sub finish()
	End Interface

End Namespace