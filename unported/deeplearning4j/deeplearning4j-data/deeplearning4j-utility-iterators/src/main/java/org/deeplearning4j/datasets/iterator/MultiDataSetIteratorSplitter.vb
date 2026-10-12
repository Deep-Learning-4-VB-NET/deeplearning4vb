Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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
'ORIGINAL LINE: @Slf4j public class MultiDataSetIteratorSplitter
	Public Class MultiDataSetIteratorSplitter
		Protected Friend backedIterator As MultiDataSetIterator
		Protected Friend ReadOnly totalExamples As Long
		Protected Friend ReadOnly ratio As Double
		Protected Friend ReadOnly numTrain As Long
		Protected Friend ReadOnly numTest As Long
		Protected Friend ReadOnly ratios() As Double
		Protected Friend ReadOnly numArbitrarySets As Long
		Protected Friend ReadOnly splits() As Integer

		Protected Friend counter As New AtomicLong(0)

		Protected Friend resetPending As New AtomicBoolean(False)
		Protected Friend firstTrain As org.nd4j.linalg.dataset.MultiDataSet = Nothing

		''' 
		''' <param name="baseIterator"> </param>
		''' <param name="totalBatches"> - total number of batches in underlying iterator. this value will be used to determine number of test/train batches </param>
		''' <param name="ratio"> - this value will be used as splitter. should be between in range of 0.0 > X < 1.0. I.e. if value 0.7 is provided, then 70% of total examples will be used for training, and 30% of total examples will be used for testing </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MultiDataSetIteratorSplitter(@NonNull MultiDataSetIterator baseIterator, long totalBatches, double ratio)
		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal totalBatches As Long, ByVal ratio As Double)
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
			Me.numTrain = CLng(Math.Truncate(totalExamples * ratio))
			Me.numTest = totalExamples - numTrain
			Me.ratios = Nothing
			Me.numArbitrarySets = 0
			Me.splits = Nothing

			log.warn("IteratorSplitter is used: please ensure you don't use randomization/shuffle in underlying iterator!")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MultiDataSetIteratorSplitter(@NonNull MultiDataSetIterator baseIterator, long totalBatches, double[] ratios)
		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal totalBatches As Long, ByVal ratios() As Double)
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
			Me.numTrain = CLng(Math.Truncate(totalExamples * ratio))
			Me.numTest = totalExamples - numTrain
			Me.ratios = Nothing
			Me.numArbitrarySets = ratios.Length

			Me.splits = New Integer(Me.ratios.Length - 1){}
			For i As Integer = 0 To Me.splits.Length - 1
				Me.splits(i) = CInt(Math.Truncate(totalExamples * ratios(i)))
			Next i

			log.warn("IteratorSplitter is used: please ensure you don't use randomization/shuffle in underlying iterator!")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MultiDataSetIteratorSplitter(@NonNull MultiDataSetIterator baseIterator, int[] splits)
		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal splits() As Integer)

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
			Me.numTrain = CLng(Math.Truncate(totalExamples * ratio))
			Me.numTest = totalExamples - numTrain
			Me.ratios = Nothing
			Me.numArbitrarySets = splits.Length
			Me.splits = splits

			log.warn("IteratorSplitter is used: please ensure you don't use randomization/shuffle in underlying iterator!")
		End Sub

		Public Overridable ReadOnly Property Iterators As IList(Of MultiDataSetIterator)
			Get
				Dim retVal As IList(Of MultiDataSetIterator) = New List(Of MultiDataSetIterator)()
				Dim partN As Integer = 0
				Dim bottom As Integer = 0
				For Each split As Integer In splits
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: ScrollableMultiDataSetIterator partIterator = new ScrollableMultiDataSetIterator(partN++, backedIterator, counter, firstTrain, new int[]{bottom,split});
					Dim partIterator As New ScrollableMultiDataSetIterator(partN, backedIterator, counter, firstTrain, New Integer(){bottom, split})
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
		Public Overridable ReadOnly Property TrainIterator As MultiDataSetIterator
			Get
				Return New MultiDataSetIteratorAnonymousInnerClass(Me)
			End Get
		End Property

		Private Class MultiDataSetIteratorAnonymousInnerClass
			Implements MultiDataSetIterator

			Private ReadOnly outerInstance As MultiDataSetIteratorSplitter

			Public Sub New(ByVal outerInstance As MultiDataSetIteratorSplitter)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
				Throw New System.NotSupportedException("To be implemented yet")
			End Function

			Public Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
				Set(ByVal preProcessor As MultiDataSetPreProcessor)
					outerInstance.backedIterator.PreProcessor = preProcessor
				End Set
				Get
					Return outerInstance.backedIterator.PreProcessor
				End Get
			End Property


			Public Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
				Return outerInstance.backedIterator.resetSupported()
			End Function

			Public Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
				Return outerInstance.backedIterator.asyncSupported()
			End Function

			Public Sub reset() Implements MultiDataSetIterator.reset
				outerInstance.resetPending.set(True)
			End Sub

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

			Public Function [next]() As MultiDataSet
				outerInstance.counter.incrementAndGet()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim p As val = outerInstance.backedIterator.next()

				If outerInstance.counter.get() = 1 AndAlso outerInstance.firstTrain Is Nothing Then
					' first epoch ever, we'll save first dataset and will use it to check for equality later
					outerInstance.firstTrain = CType(p.copy(), org.nd4j.linalg.dataset.MultiDataSet)
					outerInstance.firstTrain.detach()
				ElseIf outerInstance.counter.get() = 1 Then
					' epoch > 1, comparing first dataset to previously stored dataset. they should be equal
					Dim cnt As Integer = 0
					For Each c As val In p.getFeatures()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (!c.equalsWithEps(firstTrain.getFeatures()[cnt++], 1e-5))
						If Not c.equalsWithEps(outerInstance.firstTrain.Features(cnt), 1e-5) Then
								cnt += 1
							Throw New ND4JIllegalStateException("First examples do not match. Randomization was used?")
							Else
								cnt += 1
							End If
					Next c
				End If

				Return p
			End Function

			Public Sub remove()
				Throw New System.NotSupportedException()
			End Sub
		End Class

		''' <summary>
		''' This method returns test iterator instance
		''' 
		''' @return
		''' </summary>
		<Obsolete>
		Public Overridable ReadOnly Property TestIterator As MultiDataSetIterator
			Get
				Return New MultiDataSetIteratorAnonymousInnerClass2(Me)
			End Get
		End Property

		Private Class MultiDataSetIteratorAnonymousInnerClass2
			Implements MultiDataSetIterator

			Private ReadOnly outerInstance As MultiDataSetIteratorSplitter

			Public Sub New(ByVal outerInstance As MultiDataSetIteratorSplitter)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
				Throw New System.NotSupportedException("To be implemented yet")
			End Function

			Public Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
				Set(ByVal preProcessor As MultiDataSetPreProcessor)
					outerInstance.backedIterator.PreProcessor = preProcessor
				End Set
				Get
					Return outerInstance.backedIterator.PreProcessor
				End Get
			End Property


			Public Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
				Return outerInstance.backedIterator.resetSupported()
			End Function

			Public Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
				Return outerInstance.backedIterator.asyncSupported()
			End Function


			Public Sub reset() Implements MultiDataSetIterator.reset
				outerInstance.resetPending.set(True)
			End Sub

			Public Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim state As val = outerInstance.backedIterator.hasNext()
				If state AndAlso outerInstance.counter.get() < outerInstance.numTrain + outerInstance.numTest Then
					Return True
				Else
					Return False
				End If
			End Function

			Public Function [next]() As MultiDataSet
				outerInstance.counter.incrementAndGet()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return outerInstance.backedIterator.next()
			End Function

			Public Sub remove()
				Throw New System.NotSupportedException()
			End Sub
		End Class
	End Class

End Namespace