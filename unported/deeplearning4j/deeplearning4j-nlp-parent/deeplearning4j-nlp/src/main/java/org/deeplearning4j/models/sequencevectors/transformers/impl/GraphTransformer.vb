Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports Huffman = org.deeplearning4j.models.word2vec.Huffman
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.text.labels
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.models.sequencevectors.transformers.impl


	Public Class GraphTransformer(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements IEnumerable(Of Sequence(Of T))

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<T, ?> sourceGraph;
		Protected Friend sourceGraph As IGraph(Of T, Object)
		Protected Friend walker As GraphWalker(Of T)
		Protected Friend labelsProvider As LabelsProvider(Of T)
		Protected Friend counter As New AtomicInteger(0)
		Protected Friend shuffle As Boolean = True
		Protected Friend vocabCache As VocabCache(Of T)

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(GraphTransformer))

		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' This method handles required initialization for GraphTransformer
		''' </summary>
		Protected Friend Overridable Sub initialize()
			log.info("Building Huffman tree for source graph...")
			Dim nVertices As Integer = sourceGraph.numVertices()
			'int[] degrees = new int[nVertices];
			'for( int i=0; i<nVertices; i++ )
			' degrees[i] = sourceGraph.getVertexDegree(i);
	'        
	'        for (int y = 0; y < nVertices; y+= 20) {
	'            int[] copy = Arrays.copyOfRange(degrees, y, y+20);
	'            System.out.println("D: " + Arrays.toString(copy));
	'        }
	'        
			'        GraphHuffman huffman = new GraphHuffman(nVertices);
			'        huffman.buildTree(degrees);

			log.info("Transferring Huffman tree info to nodes...")
			For i As Integer = 0 To nVertices - 1
				Dim element As T = sourceGraph.getVertex(i).getValue()
				element.setElementFrequency(sourceGraph.getConnectedVertices(i).Count)

				If vocabCache IsNot Nothing Then
					vocabCache.addToken(element)
				End If
			Next i

			If vocabCache IsNot Nothing Then
				Dim huffman As New Huffman(vocabCache.vocabWords())
				huffman.build()
				huffman.applyIndexes(vocabCache)
			End If
		End Sub


		Public Overridable Function GetEnumerator() As IEnumerator(Of Sequence(Of T)) Implements IEnumerator(Of Sequence(Of T)).GetEnumerator
			Me.counter.set(0)
			Me.walker.reset(shuffle)
			Return New IteratorAnonymousInnerClass(Me)
		End Function

		Private Class IteratorAnonymousInnerClass
			Implements IEnumerator(Of Sequence(Of T))

			Private ReadOnly outerInstance As GraphTransformer(Of T)

			Public Sub New(ByVal outerInstance As GraphTransformer(Of T))
				Me.outerInstance = outerInstance
				outerInstance.walker = outerInstance.walker
			End Sub

			Private outerInstance.walker As GraphWalker(Of T)

			Public Sub remove()
				Throw New System.NotSupportedException("This is not supported on read-only iterator")
			End Sub

			Public Function hasNext() As Boolean
				Return outerInstance.walker.hasNext()
			End Function

			Public Function [next]() As Sequence(Of T)
				Dim sequence As Sequence(Of T) = outerInstance.walker.next()
				sequence.setSequenceId(outerInstance.counter.getAndIncrement())

				' we might already have labels defined from walker
				If outerInstance.walker.LabelEnabled AndAlso sequence.getSequenceLabels() Is Nothing Then
					If outerInstance.labelsProvider IsNot Nothing Then
						' TODO: sequence labels to be implemented for graph walks
						sequence.SequenceLabel = outerInstance.labelsProvider.getLabel(sequence.getSequenceId())
					End If
				End If

				Return sequence
			End Function
		End Class

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<T, ?> sourceGraph;
			Protected Friend sourceGraph As IGraph(Of T, Object)
'JAVA TO VB CONVERTER NOTE: The field labelsProvider was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend labelsProvider_Conflict As LabelsProvider(Of T)
			Protected Friend walker As GraphWalker(Of T)
			Protected Friend shuffle As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field vocabCache was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend vocabCache_Conflict As VocabCache(Of T)

			Public Sub New()
				'
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull GraphWalker<T> walker)
			Public Sub New(ByVal walker As GraphWalker(Of T))
				Me.walker = walker
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull IGraph<T, ?> sourceGraph)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
			Public Sub New(ByVal sourceGraph As IGraph(Of T1))
				Me.sourceGraph = sourceGraph
			End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setLabelsProvider(@NonNull LabelsProvider<T> provider)
			Public Overridable Function setLabelsProvider(ByVal provider As LabelsProvider(Of T)) As Builder(Of T)
				Me.labelsProvider_Conflict = provider
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setGraphWalker(@NonNull GraphWalker<T> walker)
			Public Overridable Function setGraphWalker(ByVal walker As GraphWalker(Of T)) As Builder(Of T)
				Me.walker = walker
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setVocabCache(@NonNull VocabCache<T> vocabCache)
			Public Overridable Function setVocabCache(ByVal vocabCache As VocabCache(Of T)) As Builder(Of T)
				Me.vocabCache_Conflict = vocabCache
				Return Me
			End Function

			Public Overridable Function shuffleOnReset(ByVal reallyShuffle As Boolean) As Builder(Of T)
				Me.shuffle = reallyShuffle
				Return Me
			End Function

			Public Overridable Function build() As GraphTransformer(Of T)
				If Me.walker Is Nothing Then
					Throw New System.InvalidOperationException("Please provide GraphWalker instance.")
				End If

				Dim transformer As New GraphTransformer(Of T)()
				If Me.sourceGraph Is Nothing Then
					Me.sourceGraph = walker.getSourceGraph()
				End If

				transformer.sourceGraph = Me.sourceGraph
				transformer.labelsProvider = Me.labelsProvider_Conflict
				transformer.shuffle = Me.shuffle
				transformer.vocabCache = Me.vocabCache_Conflict
				transformer.walker = Me.walker

				' FIXME: get rid of this
				transformer.initialize()

				Return transformer
			End Function
		End Class
	End Class

End Namespace