Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.datasets.iterator


	<Serializable>
	Public MustInherit Class AbstractDataSetIterator(Of T)
		Implements DataSetIterator

'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor
		<NonSerialized>
		Private iterable As IEnumerable(Of Pair(Of T, T))
		<NonSerialized>
		Private iterator As IEnumerator(Of Pair(Of T, T))

		Private ReadOnly batchSize As Integer

		' FIXME: capacity 4 is triage here, proper investigation requires
		Private ReadOnly queue As New LinkedBlockingQueue(Of DataSet)(4)
		Private labels As IList(Of String)
		Private numFeatures As Integer = -1
		Private numLabels As Integer = -1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected AbstractDataSetIterator(@NonNull Iterable<org.nd4j.common.primitives.Pair<T, T>> iterable, int batchSize)
		Protected Friend Sub New(ByVal iterable As IEnumerable(Of Pair(Of T, T)), ByVal batchSize As Integer)
			If batchSize < 1 Then
				Throw New System.InvalidOperationException("batchSize can't be < 1")
			End If

			Me.iterable = iterable
			Me.iterator = Me.iterable.GetEnumerator()
			Me.batchSize = batchSize

			fillQueue()
		End Sub


		''' <summary>
		''' Like the standard next method but allows a
		''' customizable number of examples returned
		''' </summary>
		''' <param name="num"> the number of examples </param>
		''' <returns> the next data applyTransformToDestination </returns>
		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.InvalidOperationException("next(int) isn't supported for this DataSetIterator")
		End Function

		''' <summary>
		''' Input columns for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return numFeatures
		End Function

		''' <summary>
		''' The number of labels for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return numLabels
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return iterable IsNot Nothing
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Public Overridable Sub reset() Implements DataSetIterator.reset
			queue.clear()
			If iterable IsNot Nothing Then
				iterator = iterable.GetEnumerator()
			End If
		End Sub

		''' <summary>
		''' Batch size
		''' 
		''' @return
		''' </summary>
		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batchSize
		End Function

		''' <summary>
		''' Set a pre processor
		''' </summary>
		''' <param name="preProcessor"> a pre processor to set </param>
		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property

		''' <summary>
		''' Get dataset iterator record reader labels
		''' </summary>
		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return labels
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
		Public Overrides Function hasNext() As Boolean
			fillQueue()
			Return Not queue.isEmpty()
		End Function

		Protected Friend Overridable Sub fillQueue()
			If queue.isEmpty() Then
				Dim ndLabels As IList(Of INDArray) = Nothing
				Dim ndFeatures As IList(Of INDArray) = Nothing
				Dim fLabels()() As Single = Nothing
				Dim fFeatures()() As Single = Nothing
				Dim dLabels()() As Double = Nothing
				Dim dFeatures()() As Double = Nothing

				Dim sampleCount As Integer = 0

				For cnt As Integer = 0 To batchSize - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If iterator.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim pair As Pair(Of T, T) = iterator.next()
						If numFeatures < 1 Then
							If TypeOf pair.First Is INDArray Then
								numFeatures = CInt(DirectCast(pair.First, INDArray).length())
								numLabels = CInt(DirectCast(pair.Second, INDArray).length())
							ElseIf TypeOf pair.First Is Single() Then
								numFeatures = CType(pair.First, Single()).Length
								numLabels = CType(pair.Second, Single()).Length
							ElseIf TypeOf pair.First Is Double() Then
								numFeatures = CType(pair.First, Double()).Length
								numLabels = CType(pair.Second, Double()).Length
							End If
						End If

						If TypeOf pair.First Is INDArray Then
							If ndLabels Is Nothing Then
								ndLabels = New List(Of INDArray)()
								ndFeatures = New List(Of INDArray)()
							End If
							ndFeatures.Add((DirectCast(pair.First, INDArray)))
							ndLabels.Add((DirectCast(pair.Second, INDArray)))
						ElseIf TypeOf pair.First Is Single() Then
							If fLabels Is Nothing Then
								fLabels = New Single(batchSize - 1)(){}
								fFeatures = New Single(batchSize - 1)(){}
							End If
							fFeatures(sampleCount) = CType(pair.First, Single())
							fLabels(sampleCount) = CType(pair.Second, Single())
						ElseIf TypeOf pair.First Is Double() Then
							If dLabels Is Nothing Then
								dLabels = New Double(batchSize - 1)(){}
								dFeatures = New Double(batchSize - 1)(){}
							End If
							dFeatures(sampleCount) = CType(pair.First, Double())
							dLabels(sampleCount) = CType(pair.Second, Double())
						End If

						sampleCount += 1
					Else
						Exit For
					End If
				Next cnt

				If sampleCount = batchSize Then
					Dim labels As INDArray = Nothing
					Dim features As INDArray = Nothing
					If ndLabels IsNot Nothing Then
						labels = Nd4j.vstack(ndLabels)
						features = Nd4j.vstack(ndFeatures)
					ElseIf fLabels IsNot Nothing Then
						labels = Nd4j.create(fLabels)
						features = Nd4j.create(fFeatures)
					ElseIf dLabels IsNot Nothing Then
						labels = Nd4j.create(dLabels)
						features = Nd4j.create(dFeatures)
					End If

					Dim dataSet As New DataSet(features, labels)
					Try
						queue.add(dataSet)
					Catch e As Exception
						' live with it
					End Try
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the next element in the iteration.
		''' </summary>
		''' <returns> the next element in the iteration </returns>
		''' <exception cref="NoSuchElementException"> if the iteration has no more elements </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet next() throws java.util.NoSuchElementException
		Public Overrides Function [next]() As DataSet
			If queue.isEmpty() Then
				Throw New NoSuchElementException()
			End If

			Dim dataSet As DataSet = queue.poll()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(dataSet)
			End If

			Return dataSet
		End Function

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
		'''                                       method
		''' @implSpec The default implementation throws an instance of
		''' <seealso cref="System.NotSupportedException"/> and performs no other action. </exception>
		Public Overrides Sub remove()
			' no-op
		End Sub

	End Class

End Namespace