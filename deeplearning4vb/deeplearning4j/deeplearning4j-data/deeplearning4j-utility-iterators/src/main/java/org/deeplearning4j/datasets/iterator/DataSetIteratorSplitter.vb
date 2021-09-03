Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DataSetIteratorSplitter
	Public Class DataSetIteratorSplitter
		Protected Friend backedIterator As DataSetIterator
		Protected Friend ReadOnly totalExamples As Long
		Protected Friend ReadOnly ratio As Double
		Protected Friend ReadOnly ratios() As Double
		Protected Friend ReadOnly numTrain As Long
		Protected Friend ReadOnly numTest As Long
		Protected Friend ReadOnly numArbitrarySets As Long
		Protected Friend ReadOnly splits() As Integer


		Protected Friend counter As New AtomicLong(0)

		Protected Friend resetPending As New AtomicBoolean(False)
		Protected Friend firstTrain As DataSet = Nothing

		Protected Friend partNumber As Integer = 0

		''' <summary>
		''' The only constructor
		''' </summary>
		''' <param name="baseIterator"> - iterator to be wrapped and split </param>
		''' <param name="totalBatches"> - total batches in baseIterator </param>
		''' <param name="ratio"> - train/test split ratio </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DataSetIteratorSplitter(@NonNull DataSetIterator baseIterator, long totalBatches, double ratio)
		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal totalBatches As Long, ByVal ratio As Double)
			If Not (ratio > 0.0 AndAlso ratio < 1.0) Then
				Throw New ND4JIllegalStateException("Ratio value should be in range of 0.0 > X < 1.0")
			End If

			If totalBatches < 0 Then
				Throw New ND4JIllegalStateException("totalExamples number should be positive value")
			End If

			If Not baseIterator.resetSupported() Then
				Throw New ND4JIllegalStateException("Underlying iterator doesn't support reset, so it can't be used for runtime-split")
			End If


			Me.backedIterator = baseIterator
			Me.totalExamples = totalBatches
			Me.ratio = ratio
			Me.ratios = Nothing
			Me.numTrain = CLng(Math.Truncate(totalExamples * ratio))
			Me.numTest = totalExamples - numTrain
			Me.numArbitrarySets = 2
			Me.splits = Nothing

			log.warn("IteratorSplitter is used: please ensure you don't use randomization/shuffle in underlying iterator!")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DataSetIteratorSplitter(@NonNull DataSetIterator baseIterator, long totalBatches, double[] ratios)
		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal totalBatches As Long, ByVal ratios() As Double)
			For Each ratio As Double In ratios
				If Not (ratio > 0.0 AndAlso ratio < 1.0) Then
					Throw New ND4JIllegalStateException("Ratio value should be in range of 0.0 > X < 1.0")
				End If
			Next ratio

			If totalBatches < 0 Then
				Throw New ND4JIllegalStateException("totalExamples number should be positive value")
			End If

			If Not baseIterator.resetSupported() Then
				Throw New ND4JIllegalStateException("Underlying iterator doesn't support reset, so it can't be used for runtime-split")
			End If


			Me.backedIterator = baseIterator
			Me.totalExamples = totalBatches
			Me.ratio = 0.0
			Me.ratios = ratios
			Me.numTrain = 0 '(long) (totalExamples * ratio);
			Me.numTest = 0 'totalExamples - numTrain;
			Me.numArbitrarySets = ratios.Length

			Me.splits = New Integer(Me.ratios.Length - 1){}
			For i As Integer = 0 To Me.splits.Length - 1
				Me.splits(i) = CInt(Math.Truncate(totalExamples * ratios(i)))
			Next i

			log.warn("IteratorSplitter is used: please ensure you don't use randomization/shuffle in underlying iterator!")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DataSetIteratorSplitter(@NonNull DataSetIterator baseIterator, int[] splits)
		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal splits() As Integer)

	'        if (!(simpleRatio > 0.0 && simpleRatio < 1.0))
	'           throw new ND4JIllegalStateException("Ratio value should be in range of 0.0 > X < 1.0");

			Dim totalBatches As Integer = 0
			For Each v As val In splits
				totalBatches += v
			Next v

			If totalBatches < 0 Then
				Throw New ND4JIllegalStateException("totalExamples number should be positive value")
			End If

			If Not baseIterator.resetSupported() Then
				Throw New ND4JIllegalStateException("Underlying iterator doesn't support reset, so it can't be used for runtime-split")
			End If


			Me.backedIterator = baseIterator
			Me.totalExamples = totalBatches
			Me.ratio = 0.0
			Me.ratios = Nothing

			Me.numTrain = 0 '(long) (totalExamples * ratio);
			Me.numTest = 0 'totalExamples - numTrain;
			Me.splits = splits
			Me.numArbitrarySets = splits.Length

			log.warn("IteratorSplitter is used: please ensure you don't use randomization/shuffle in underlying iterator!")
		End Sub

		Public Overridable ReadOnly Property Iterators As IList(Of DataSetIterator)
			Get
				Dim retVal As IList(Of DataSetIterator) = New List(Of DataSetIterator)()
				Dim partN As Integer = 0
				Dim bottom As Integer = 0
				For Each split As Integer In splits
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: ScrollableDataSetIterator partIterator = new ScrollableDataSetIterator(partN++, backedIterator, counter, resetPending, firstTrain, new int[]{bottom,split});
						Dim partIterator As New ScrollableDataSetIterator(partN, backedIterator, counter, resetPending, firstTrain, New Integer(){bottom, split})
							partN += 1
						bottom += split
						retVal.Add(partIterator)
				Next split
				Return retVal
			End Get
		End Property


		''' <summary>
		''' This method returns train iterator instance
		''' 
		''' @return
		''' </summary>
		<Obsolete>
		Public Overridable ReadOnly Property TrainIterator As DataSetIterator
			Get
				Return New DataSetIteratorAnonymousInnerClass(Me)
			End Get
		End Property

		Private Class DataSetIteratorAnonymousInnerClass
			Implements DataSetIterator

			Private ReadOnly outerInstance As DataSetIteratorSplitter

			Public Sub New(ByVal outerInstance As DataSetIteratorSplitter)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next](ByVal i As Integer) As DataSet Implements DataSetIterator.next
				Throw New System.NotSupportedException()
			End Function

			Public ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
				Get
					Return outerInstance.backedIterator.getLabels()
				End Get
			End Property

			Public Function inputColumns() As Integer Implements DataSetIterator.inputColumns
				Return outerInstance.backedIterator.inputColumns()
			End Function

			Public Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
				Return outerInstance.backedIterator.totalOutcomes()
			End Function

			Public Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
				Return outerInstance.backedIterator.resetSupported()
			End Function

			Public Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
				Return outerInstance.backedIterator.asyncSupported()
			End Function

			Public Sub reset() Implements DataSetIterator.reset
				outerInstance.resetPending.set(True)
			End Sub

			Public Function batch() As Integer Implements DataSetIterator.batch
				Return outerInstance.backedIterator.batch()
			End Function

			Public Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
				Set(ByVal dataSetPreProcessor As DataSetPreProcessor)
					outerInstance.backedIterator.PreProcessor = dataSetPreProcessor
				End Set
				Get
					Return outerInstance.backedIterator.PreProcessor
				End Get
			End Property



			Public Function hasNext() As Boolean
				If outerInstance.resetPending.get() Then
					If resetSupported() Then
						outerInstance.backedIterator.reset()
						outerInstance.counter.set(0)
						outerInstance.resetPending.set(False)
					Else
						Throw New System.NotSupportedException("Reset isn't supported by underlying iterator")
					End If
				End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim state As val = outerInstance.backedIterator.hasNext()
				If state AndAlso outerInstance.counter.get() < outerInstance.numTrain Then
					Return True
				Else
					Return False
				End If
			End Function

			Public Function [next]() As DataSet
				outerInstance.counter.incrementAndGet()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim p As val = outerInstance.backedIterator.next()

				If outerInstance.counter.get() = 1 AndAlso outerInstance.firstTrain Is Nothing Then
					' first epoch ever, we'll save first dataset and will use it to check for equality later
					outerInstance.firstTrain = p.copy()
					outerInstance.firstTrain.detach()
				ElseIf outerInstance.counter.get() = 1 Then
					' epoch > 1, comparing first dataset to previously stored dataset. they should be equal
					Dim cnt As Integer = 0
					If Not p.getFeatures().equalsWithEps(outerInstance.firstTrain.Features, 1e-5) Then
						Throw New ND4JIllegalStateException("First examples do not match. Randomization was used?")
					End If
				End If

				Return p
			End Function
		End Class

		''' <summary>
		''' This method returns test iterator instance
		''' 
		''' @return
		''' </summary>
		<Obsolete>
		Public Overridable ReadOnly Property TestIterator As DataSetIterator
			Get
				Return New DataSetIteratorAnonymousInnerClass2(Me)
			End Get
		End Property

		Private Class DataSetIteratorAnonymousInnerClass2
			Implements DataSetIterator

			Private ReadOnly outerInstance As DataSetIteratorSplitter

			Public Sub New(ByVal outerInstance As DataSetIteratorSplitter)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next](ByVal i As Integer) As DataSet Implements DataSetIterator.next
				Throw New System.NotSupportedException()
			End Function

			Public ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
				Get
					Return outerInstance.backedIterator.getLabels()
				End Get
			End Property

			Public Function inputColumns() As Integer Implements DataSetIterator.inputColumns
				Return outerInstance.backedIterator.inputColumns()
			End Function

			Public Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
				Return outerInstance.backedIterator.totalOutcomes()
			End Function

			Public Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
				Return outerInstance.backedIterator.resetSupported()
			End Function

			Public Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
				Return outerInstance.backedIterator.asyncSupported()
			End Function

			Public Sub reset() Implements DataSetIterator.reset
				outerInstance.resetPending.set(True)
			End Sub

			Public Function batch() As Integer Implements DataSetIterator.batch
				Return outerInstance.backedIterator.batch()
			End Function

			Public Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
				Set(ByVal dataSetPreProcessor As DataSetPreProcessor)
					outerInstance.backedIterator.PreProcessor = dataSetPreProcessor
				End Set
				Get
					Return outerInstance.backedIterator.PreProcessor
				End Get
			End Property



			Public Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim state As val = outerInstance.backedIterator.hasNext()
				If state AndAlso outerInstance.counter.get() < outerInstance.numTrain + outerInstance.numTest Then
					Return True
				Else
					Return False
				End If
			End Function

			Public Function [next]() As DataSet
				outerInstance.counter.incrementAndGet()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return outerInstance.backedIterator.next()
			End Function
		End Class
	End Class

End Namespace