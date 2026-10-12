Imports System
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.learning
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports SparkSequenceLearningAlgorithm = org.deeplearning4j.spark.models.sequencevectors.learning.SparkSequenceLearningAlgorithm
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

Namespace org.deeplearning4j.spark.models.sequencevectors.learning.sequence

	Public MustInherit Class BaseSparkSequenceLearningAlgorithm
		Implements SparkSequenceLearningAlgorithm

		<NonSerialized>
		Protected Friend vocabCache As VocabCache(Of ShallowSequenceElement)
		<NonSerialized>
		Protected Friend vectorsConfiguration As VectorsConfiguration
		<NonSerialized>
		Protected Friend elementsLearningAlgorithm As ElementsLearningAlgorithm(Of ShallowSequenceElement)

		Public Overridable Sub configure(ByVal vocabCache As VocabCache(Of ShallowSequenceElement), ByVal lookupTable As WeightLookupTable(Of ShallowSequenceElement), ByVal configuration As VectorsConfiguration)
			Me.vocabCache = vocabCache
			Me.vectorsConfiguration = configuration
		End Sub

		Public Overridable Sub pretrain(ByVal iterator As SequenceIterator(Of ShallowSequenceElement))
			' no-op by default
		End Sub

		Public Overridable ReadOnly Property EarlyTerminationHit As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable Function inferSequence(ByVal sequence As Sequence(Of ShallowSequenceElement), ByVal nextRandom As Long, ByVal learningRate As Double, ByVal minLearningRate As Double, ByVal iterations As Integer) As INDArray
			Throw New System.NotSupportedException()
		End Function

		Public Overridable ReadOnly Property ElementsLearningAlgorithm As ElementsLearningAlgorithm(Of ShallowSequenceElement)
			Get
				Return elementsLearningAlgorithm
			End Get
		End Property

		Public Overridable Sub finish()
			' no-op on spark
		End Sub
	End Class

End Namespace