Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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
	Public Class ScrollableDataSetIterator
		Implements DataSetIterator

		Private thisPart As Integer = 0
		Private top As Integer = 0
		Private bottom As Integer = 0
		Protected Friend backedIterator As DataSetIterator
		Protected Friend counter As New AtomicLong(0)

		Protected Friend resetPending As New AtomicBoolean(False)
		Protected Friend firstTrain As DataSet = Nothing
		Protected Friend firstMultiTrain As MultiDataSet = Nothing
		Private ratio As Double
		Private totalExamples As Long
		Private itemsPerPart As Long
		Private current As Long


		Public Sub New(ByVal num As Integer, ByVal backedIterator As DataSetIterator, ByVal counter As AtomicLong, ByVal resetPending As AtomicBoolean, ByVal firstTrain As DataSet, ByVal ratio As Double, ByVal totalExamples As Integer)
			Me.thisPart = num
			Me.backedIterator = backedIterator
			Me.counter = counter
			Me.resetPending = resetPending
			Me.firstTrain = firstTrain
			Me.ratio = ratio
			Me.totalExamples = totalExamples
			Me.itemsPerPart = CLng(Math.Truncate(totalExamples * ratio))
			Me.current = 0
		End Sub

		Public Sub New(ByVal num As Integer, ByVal backedIterator As DataSetIterator, ByVal counter As AtomicLong, ByVal resetPending As AtomicBoolean, ByVal firstTrain As DataSet, ByVal itemsPerPart() As Integer)
			Me.thisPart = num
			Me.bottom = itemsPerPart(0)
			Me.top = bottom + itemsPerPart(1)
			Me.itemsPerPart = top

			Me.backedIterator = backedIterator
			Me.counter = counter
			'this.resetPending = resetPending;
			Me.firstTrain = firstTrain
			'this.totalExamples = totalExamples;
			Me.current = 0
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return backedIterator.getLabels()
			End Get
		End Property

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return backedIterator.inputColumns()
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return backedIterator.totalOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return backedIterator.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return backedIterator.asyncSupported()
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			resetPending.set(True)
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return backedIterator.batch()
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal dataSetPreProcessor As DataSetPreProcessor)
				backedIterator.PreProcessor = dataSetPreProcessor
			End Set
			Get
    
				Return backedIterator.PreProcessor
			End Get
		End Property



		Public Overrides Function hasNext() As Boolean
			If resetPending.get() Then
				If resetSupported() Then
					backedIterator.reset()
					counter.set(0)
					current = 0
					resetPending.set(False)
				Else
					Throw New System.NotSupportedException("Reset isn't supported by underlying iterator")
				End If
			End If

			Dim state As Boolean = False
			If current >= top Then
				Return False
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			state = backedIterator.hasNext()
			If Not state Then
				Return False
			End If
			If state AndAlso counter.get() < itemsPerPart Then
				Return True
			Else
				Return False
			End If

		End Function

		Public Overrides Function [next]() As DataSet
			counter.incrementAndGet()
			If (current = 0) AndAlso (bottom <> 0) Then
				backedIterator.reset()
				Dim cnt As Long = current
				Do While cnt < bottom
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If backedIterator.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						backedIterator.next()
					End If
					cnt += 1
				Loop
				current = cnt+1
			Else
				current += 1
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim p As val = backedIterator.next()
			Return p
		End Function
	End Class

End Namespace