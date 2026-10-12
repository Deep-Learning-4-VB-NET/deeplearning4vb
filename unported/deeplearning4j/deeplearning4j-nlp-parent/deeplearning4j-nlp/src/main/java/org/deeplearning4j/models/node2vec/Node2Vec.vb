Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.learning
Imports org.deeplearning4j.models.embeddings.learning
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.embeddings.reader
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports org.deeplearning4j.models.sequencevectors
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.iterators
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.sequencevectors.transformers.impl
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

Namespace org.deeplearning4j.models.node2vec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class Node2Vec<V extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement, E extends Number> extends org.deeplearning4j.models.sequencevectors.SequenceVectors<V>
	<Obsolete, Serializable>
	Public Class Node2Vec(Of V As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement, E As Number)
		Inherits SequenceVectors(Of V)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray inferVector(@NonNull Collection<org.deeplearning4j.models.sequencevectors.graph.primitives.Vertex<V>> vertices)
		Public Overridable Function inferVector(ByVal vertices As ICollection(Of Vertex(Of V))) As INDArray
			Return Nothing
		End Function

		Public Class Builder(Of V As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement, E As Number)
			Inherits SequenceVectors.Builder(Of V)

			Friend walker As GraphWalker(Of V)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull GraphWalker<V> walker, @NonNull VectorsConfiguration configuration)
			Public Sub New(ByVal walker As GraphWalker(Of V), ByVal configuration As VectorsConfiguration)
				Me.walker = walker
				Me.configuration = configuration

				' FIXME: this will cause transformer initialization
				Dim transformer As GraphTransformer(Of V) = (New GraphTransformer.Builder(Of V)(walker.getSourceGraph())).setGraphWalker(walker).shuffleOnReset(True).build()

				Me.iterator = (New AbstractSequenceIterator.Builder(Of V)(transformer)).build()
			End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override protected Builder<V, E> useExistingWordVectors(@NonNull WordVectors vec)
			Protected Friend Overrides Function useExistingWordVectors(ByVal vec As WordVectors) As Builder(Of V, E)
				MyBase.useExistingWordVectors(vec)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> iterate(@NonNull SequenceIterator<V> iterator)
			Public Overrides Function iterate(ByVal iterator As SequenceIterator(Of V)) As Builder(Of V, E)
				MyBase.iterate(iterator)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> sequenceLearningAlgorithm(@NonNull String algoName)
			Public Overrides Function sequenceLearningAlgorithm(ByVal algoName As String) As Builder(Of V, E)
				MyBase.sequenceLearningAlgorithm(algoName)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> sequenceLearningAlgorithm(@NonNull SequenceLearningAlgorithm<V> algorithm)
			Public Overrides Function sequenceLearningAlgorithm(ByVal algorithm As SequenceLearningAlgorithm(Of V)) As Builder(Of V, E)
				MyBase.sequenceLearningAlgorithm(algorithm)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> elementsLearningAlgorithm(@NonNull String algoName)
			Public Overrides Function elementsLearningAlgorithm(ByVal algoName As String) As Builder(Of V, E)
				MyBase.elementsLearningAlgorithm(algoName)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> elementsLearningAlgorithm(@NonNull ElementsLearningAlgorithm<V> algorithm)
			Public Overrides Function elementsLearningAlgorithm(ByVal algorithm As ElementsLearningAlgorithm(Of V)) As Builder(Of V, E)
				MyBase.elementsLearningAlgorithm(algorithm)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter iterations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function iterations(ByVal iterations_Conflict As Integer) As Builder(Of V, E)
				MyBase.iterations(iterations_Conflict)
				Return Me
			End Function

			Public Overrides Function epochs(ByVal numEpochs As Integer) As Builder(Of V, E)
				MyBase.epochs(numEpochs)
				Return Me
			End Function

			Public Overrides Function workers(ByVal numWorkers As Integer) As Builder(Of V, E)
				MyBase.workers(numWorkers)
				Return Me
			End Function

			Public Overrides Function useHierarchicSoftmax(ByVal reallyUse As Boolean) As Builder(Of V, E)
				MyBase.useHierarchicSoftmax(reallyUse)
				Return Me
			End Function

			Public Overrides Function useAdaGrad(ByVal reallyUse As Boolean) As Builder(Of V, E)
				MyBase.useAdaGrad(reallyUse)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter layerSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function layerSize(ByVal layerSize_Conflict As Integer) As Builder(Of V, E)
				MyBase.layerSize(layerSize_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter learningRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function learningRate(ByVal learningRate_Conflict As Double) As Builder(Of V, E)
				MyBase.learningRate(learningRate_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter minWordFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function minWordFrequency(ByVal minWordFrequency_Conflict As Integer) As Builder(Of V, E)
				MyBase.minWordFrequency(minWordFrequency_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter minLearningRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function minLearningRate(ByVal minLearningRate_Conflict As Double) As Builder(Of V, E)
				MyBase.minLearningRate(minLearningRate_Conflict)
				Return Me
			End Function

			Public Overrides Function resetModel(ByVal reallyReset As Boolean) As Builder(Of V, E)
				MyBase.resetModel(reallyReset)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> vocabCache(@NonNull VocabCache<V> vocabCache)
'JAVA TO VB CONVERTER NOTE: The parameter vocabCache was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function vocabCache(ByVal vocabCache_Conflict As VocabCache(Of V)) As Builder(Of V, E)
				MyBase.vocabCache(vocabCache_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> lookupTable(@NonNull WeightLookupTable<V> lookupTable)
'JAVA TO VB CONVERTER NOTE: The parameter lookupTable was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function lookupTable(ByVal lookupTable_Conflict As WeightLookupTable(Of V)) As Builder(Of V, E)
				MyBase.lookupTable(lookupTable_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter sampling was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function sampling(ByVal sampling_Conflict As Double) As Builder(Of V, E)
				MyBase.sampling(sampling_Conflict)
				Return Me
			End Function

			Public Overrides Function negativeSample(ByVal negative As Double) As Builder(Of V, E)
				MyBase.negativeSample(negative)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> stopWords(@NonNull List<String> stopList)
			Public Overrides Function stopWords(ByVal stopList As IList(Of String)) As Builder(Of V, E)
				MyBase.stopWords(stopList)
				Return Me
			End Function

			Public Overrides Function trainElementsRepresentation(ByVal trainElements As Boolean) As Builder(Of V, E)
				MyBase.trainElementsRepresentation(trainElements)
				Return Me
			End Function

			Public Overrides Function trainSequencesRepresentation(ByVal trainSequences As Boolean) As Builder(Of V, E)
				MyBase.trainSequencesRepresentation(trainSequences)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> stopWords(@NonNull Collection<V> stopList)
			Public Overrides Function stopWords(ByVal stopList As ICollection(Of V)) As Builder(Of V, E)
				MyBase.stopWords(stopList)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter windowSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function windowSize(ByVal windowSize_Conflict As Integer) As Builder(Of V, E)
				MyBase.windowSize(windowSize_Conflict)
				Return Me
			End Function

			Public Overrides Function seed(ByVal randomSeed As Long) As Builder(Of V, E)
				MyBase.seed(randomSeed)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> modelUtils(@NonNull ModelUtils<V> modelUtils)
'JAVA TO VB CONVERTER NOTE: The parameter modelUtils was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function modelUtils(ByVal modelUtils_Conflict As ModelUtils(Of V)) As Builder(Of V, E)
				MyBase.modelUtils(modelUtils_Conflict)
				Return Me
			End Function

			Public Overrides Function useUnknown(ByVal reallyUse As Boolean) As Builder(Of V, E)
				MyBase.useUnknown(reallyUse)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> unknownElement(@NonNull V element)
			Public Overrides Function unknownElement(ByVal element As V) As Builder(Of V, E)
				MyBase.unknownElement(element)
				Return Me
			End Function

			Public Overrides Function useVariableWindow(ParamArray ByVal windows() As Integer) As Builder(Of V, E)
				MyBase.useVariableWindow(windows)
				Return Me
			End Function

			Public Overrides Function usePreciseWeightInit(ByVal reallyUse As Boolean) As Builder(Of V, E)
				MyBase.usePreciseWeightInit(reallyUse)
				Return Me
			End Function

			Protected Friend Overrides Sub presetTables()
				MyBase.presetTables()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder<V, E> setVectorsListeners(@NonNull Collection<org.deeplearning4j.models.sequencevectors.interfaces.VectorsListener<V>> vectorsListeners)
			Public Overrides Function setVectorsListeners(ByVal vectorsListeners As ICollection(Of VectorsListener(Of V))) As Builder(Of V, E)
				MyBase.setVectorsListeners(vectorsListeners)
				Return Me
			End Function

			Public Overrides Function enableScavenger(ByVal reallyEnable As Boolean) As Builder(Of V, E)
				MyBase.enableScavenger(reallyEnable)
				Return Me
			End Function

			Public Overrides Function build() As Node2Vec(Of V, E)
				Dim node2vec As New Node2Vec(Of V, E)()
				node2vec.iterator = Me.iterator

				Return node2vec
			End Function
		End Class
	End Class

End Namespace