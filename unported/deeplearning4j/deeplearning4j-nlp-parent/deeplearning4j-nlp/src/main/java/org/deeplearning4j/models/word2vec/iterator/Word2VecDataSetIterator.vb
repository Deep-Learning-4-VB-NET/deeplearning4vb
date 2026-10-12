Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports InputHomogenization = org.deeplearning4j.text.inputsanitation.InputHomogenization
Imports Window = org.deeplearning4j.text.movingwindow.Window
Imports WindowConverter = org.deeplearning4j.text.movingwindow.WindowConverter
Imports Windows = org.deeplearning4j.text.movingwindow.Windows
Imports SentencePreProcessor = org.deeplearning4j.text.sentenceiterator.SentencePreProcessor
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil

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

Namespace org.deeplearning4j.models.word2vec.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Word2VecDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class Word2VecDataSetIterator
		Implements DataSetIterator

		Private vec As Word2Vec
		Private iter As LabelAwareSentenceIterator
		Private cachedWindow As IList(Of Window)
		Private labels As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private batch_Conflict As Integer = 10
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

		''' <summary>
		''' Allows for customization of all of the params of the iterator </summary>
		''' <param name="vec"> the word2vec model to use </param>
		''' <param name="iter"> the sentence iterator to use </param>
		''' <param name="labels"> the possible labels </param>
		''' <param name="batch"> the batch size </param>
		''' <param name="homogenization"> whether to homogenize the sentences or not </param>
		''' <param name="addLabels"> whether to add labels for windows </param>
		Public Sub New(ByVal vec As Word2Vec, ByVal iter As LabelAwareSentenceIterator, ByVal labels As IList(Of String), ByVal batch As Integer, ByVal homogenization As Boolean, ByVal addLabels As Boolean)
			Me.vec = vec
			Me.iter = iter
			Me.labels = labels
			Me.batch_Conflict = batch
			cachedWindow = New CopyOnWriteArrayList(Of Window)()

			If addLabels AndAlso homogenization Then
				iter.PreProcessor = New SentencePreProcessorAnonymousInnerClass(Me)}

			ElseIf addLabels Then
				iter.PreProcessor = New SentencePreProcessorAnonymousInnerClass2(Me)

			ElseIf homogenization Then
				iter.PreProcessor = New SentencePreProcessorAnonymousInnerClass3(Me)

	End Class

		''' <summary>
		''' Initializes this iterator with homogenization and adding labels
		''' and a batch size of 10 </summary>
		''' <param name="vec"> the vector model to use </param>
		''' <param name="iter"> the sentence iterator to use </param>
		''' <param name="labels"> the possible labels </param>
		public Word2VecDataSetIterator(Word2Vec vec, LabelAwareSentenceIterator iter, IList(Of String) labels)
			Me.New(vec, iter, labels, 10)
			End If

		''' <summary>
		''' Initializes this iterator with homogenization and adding labels </summary>
		''' <param name="vec"> the vector model to use </param>
		''' <param name="iter"> the sentence iterator to use </param>
		''' <param name="labels"> the possible labels </param>
		''' <param name="batch"> the batch size </param>
		public Word2VecDataSetIterator(Word2Vec vec, LabelAwareSentenceIterator iter, IList(Of String) labels, Integer batch)
		If True Then
			Me.New(vec, iter, labels, batch, True, True)


		End If

		''' <summary>
		''' Like the standard next method but allows a
		''' customizable number of examples returned
		''' </summary>
		''' <param name="num"> the number of examples </param>
		''' <returns> the next data applyTransformToDestination </returns>
		public DataSet [next](Integer num)
		If True Then
			If num <= cachedWindow.Count Then
				Return fromCached(num)
			'no more sentences, return the left over
			ElseIf num >= cachedWindow.Count AndAlso Not iter.hasNext() Then
				Return fromCached(cachedWindow.Count)

			'need the next sentence
			Else
				Do While cachedWindow.Count < num AndAlso iter.hasNext()
					Dim sentence As String = iter.nextSentence()
					If sentence.Length = 0 Then
						Continue Do
					End If
					Dim windows As IList(Of Window) = Windows.windows(sentence, vec.getTokenizerFactory(), vec.getWindow(), vec)
					If windows.Count = 0 AndAlso sentence.Length > 0 Then
						Throw New System.InvalidOperationException("Empty window on sentence")
					End If
					For Each w As Window In windows
						w.Label = iter.currentLabel()
					Next w
					CType(cachedWindow, List(Of Window)).AddRange(windows)
				Loop

				Return fromCached(num)
			End If

		End If

		private DataSet fromCached(Integer num)
		If True Then
			If cachedWindow.Count = 0 Then
				Do While cachedWindow.Count < num AndAlso iter.hasNext()
					Dim sentence As String = iter.nextSentence()
					If sentence.Length = 0 Then
						Continue Do
					End If
					Dim windows As IList(Of Window) = Windows.windows(sentence, vec.getTokenizerFactory(), vec.getWindow(), vec)
					For Each w As Window In windows
						w.Label = iter.currentLabel()
					Next w
					CType(cachedWindow, List(Of Window)).AddRange(windows)
				Loop
			End If


			Dim windows As IList(Of Window) = New List(Of Window)(num)

			For i As Integer = 0 To num - 1
				If cachedWindow.Count = 0 Then
					Exit For
				End If
				windows.Add(cachedWindow.RemoveAt(0))
			Next i

			If windows.Count = 0 Then
				Return Nothing
			End If



			Dim inputs As INDArray = Nd4j.create(num, inputColumns())
			Dim i As Integer = 0
			Do While i < inputs.rows()
				inputs.putRow(i, WindowConverter.asExampleMatrix(windows(i), vec))
				i += 1
			Loop

			Dim labelOutput As INDArray = Nd4j.create(num, labels.Count)
			i = 0
			Do While i < labelOutput.rows()
				Dim label As String = windows(i).getLabel()
				labelOutput.putRow(i, FeatureUtil.toOutcomeVector(labels.IndexOf(label), labels.Count))
				i += 1
			Loop

			Dim ret As New DataSet(inputs, labelOutput)
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(ret)
			End If

			Return ret
		End If

		public Integer inputColumns()
		If True Then
			Return vec.lookupTable().layerSize() * vec.getWindow()
		End If

		public Integer totalOutcomes()
		If True Then
			Return labels.Count
		End If

		public Boolean resetSupported()
		If True Then
			Return True
		End If

		public Boolean asyncSupported()
		If True Then
			Return False
		End If

		public void reset()
		If True Then
			iter.reset()
			cachedWindow.Clear()
		End If

		public Integer Me.batch()
		If True Then
			Return batch
		End If

		public void setPreProcessor(DataSetPreProcessor preProcessor_Conflict)
		If True Then
			Me.preProcessor_Conflict = preProcessor_Conflict
		End If

		public IList(Of String) getLabels()
		If True Then
			Return Nothing
		End If


		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		public Boolean hasNext()
		If True Then
			Return iter.hasNext() OrElse cachedWindow.Count > 0
		End If

		''' <summary>
		''' Returns the next element in the iteration.
		''' </summary>
		''' <returns> the next element in the iteration </returns>
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		public DataSet [next]()
		If True Then
			Return [next](batch)
		End If

		''' <summary>
		''' Removes from the underlying collection the last element returned
		''' by this iterator (optional operation).  This method can be called
		''' only once per call to <seealso cref="next"/>.  The behavior of an iterator
		''' is unspecified if the underlying collection is modified while the
		''' iteration is in progress in any way other than by calling this
		''' method.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the {@code remove}
		'''                                       operation is not supported by this iterator </exception>
		''' <exception cref="IllegalStateException">         if the {@code next} method has not
		'''                                       yet been called, or the {@code remove} method has already
		'''                                       been called after the last call to the {@code next}
		'''                                       method </exception>
		public void remove()
		If True Then
			Throw New System.NotSupportedException()
		End If
			End If

End Namespace