Imports System
Imports System.Collections.Generic
Imports VisibleForTesting = org.nd4j.shade.guava.annotations.VisibleForTesting
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports Getter = lombok.Getter
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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

Namespace org.deeplearning4j.datasets.iterator



	<Obsolete, Serializable>
	Public Class MultipleEpochsIterator
		Implements DataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @VisibleForTesting protected int epochs = 0;
		Protected Friend epochs As Integer = 0
		Protected Friend numEpochs As Integer
'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend batch_Conflict As Integer = 0
		Protected Friend lastBatch As Integer = batch_Conflict
		Protected Friend iter As DataSetIterator
		Protected Friend ds As DataSet
		Protected Friend batchedDS As IList(Of DataSet) = Lists.newArrayList()
		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(MultipleEpochsIterator))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As DataSetPreProcessor
		Protected Friend newEpoch As Boolean = False
		Protected Friend iterationsCounter As New AtomicLong(0)
		Protected Friend totalIterations As Long = Long.MaxValue

		<Obsolete>
		Public Sub New(ByVal numEpochs As Integer, ByVal iter As DataSetIterator)
			Me.numEpochs = numEpochs
			Me.iter = iter
		End Sub

		<Obsolete>
		Public Sub New(ByVal numEpochs As Integer, ByVal iter As DataSetIterator, ByVal queueSize As Integer)
			Me.numEpochs = numEpochs
			Me.iter = iter
		End Sub

		<Obsolete>
		Public Sub New(ByVal iter As DataSetIterator, ByVal queueSize As Integer, ByVal totalIterations As Long)
			Me.numEpochs = Integer.MaxValue
			Me.iter = iter
			Me.totalIterations = totalIterations
		End Sub

		<Obsolete>
		Public Sub New(ByVal iter As DataSetIterator, ByVal totalIterations As Long)
			Me.numEpochs = Integer.MaxValue
			Me.iter = iter
			Me.totalIterations = totalIterations
		End Sub

		<Obsolete>
		Public Sub New(ByVal numEpochs As Integer, ByVal ds As DataSet)
			Me.numEpochs = numEpochs
			Me.ds = ds
		End Sub


		''' <summary>
		''' Like the standard next method but allows a
		''' customizable number of examples returned
		''' </summary>
		''' <param name="num"> the number of examples </param>
		''' <returns> the next data applyTransformToDestination </returns>
		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As DataSet
			batch_Conflict += 1
			iterationsCounter.incrementAndGet()
			If iter Is Nothing Then
				' return full DataSet
				If num = -1 Then
					next_Conflict = ds
					If epochs < numEpochs Then
						trackEpochs()
					End If
				' return DataSet broken into batches
				Else
					If batchedDS.Count = 0 AndAlso num > 0 Then
						batchedDS = ds.batchBy(num)
					End If
					next_Conflict = batchedDS(batch_Conflict)
					If batch_Conflict + 1 = batchedDS.Count Then
						trackEpochs()
						If epochs < numEpochs Then
							batch_Conflict = -1
						End If
					End If
				End If
			Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				next_Conflict = (If(num = -1, iter.next(), iter.next(num)))
				If next_Conflict Is Nothing Then
					Throw New System.InvalidOperationException("Iterator returned null DataSet")
				End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not iter.hasNext() Then
					trackEpochs()
					' track number of epochs and won't reset if it's over
					If epochs < numEpochs Then
						iter.reset()
						lastBatch = batch_Conflict
						batch_Conflict = 0
					End If
				End If
			End If
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(next_Conflict)
			End If
			Return next_Conflict
		End Function

		Public Overridable Sub trackEpochs()
			epochs += 1
			newEpoch = True
		End Sub

		Public Overrides Function [next]() As DataSet
			Return [next](-1)
		End Function

		''' <summary>
		''' Input columns for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return iter.inputColumns()
		End Function

		''' <summary>
		''' The number of labels for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return iter.totalOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return iter.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return iter.asyncSupported()
		End Function

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Public Overridable Sub reset() Implements DataSetIterator.reset
			If Not iter.resetSupported() Then
				Throw New System.InvalidOperationException("Cannot reset MultipleEpochsIterator with base iter that does not support reset")
			End If
			epochs = 0
			lastBatch = batch_Conflict
			batch_Conflict = 0
			iterationsCounter.set(0)
			iter.reset()
		End Sub

		''' <summary>
		''' Batch size
		''' 
		''' @return
		''' </summary>
		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return iter.batch()
		End Function

		Public Overridable WriteOnly Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return iter.getLabels()
			End Get
		End Property


		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
		Public Overrides Function hasNext() As Boolean
			If iterationsCounter.get() >= totalIterations Then
				Return False
			End If

			If newEpoch Then
				log.info("Epoch " & epochs & ", number of batches completed " & lastBatch)
				newEpoch = False
			End If
			If iter Is Nothing Then
				Return (epochs < numEpochs) AndAlso ((batchedDS.Count > 0 AndAlso batchedDS.Count > batch_Conflict) OrElse batchedDS.Count = 0)
			Else
				' either there are still epochs to complete or its the first epoch
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return (epochs < numEpochs) OrElse (iter.hasNext() AndAlso (epochs = 0 OrElse epochs = numEpochs))
			End If
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
		'''                                       method </exception>
		Public Overrides Sub remove()
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
			iter.remove()
		End Sub
	End Class

End Namespace