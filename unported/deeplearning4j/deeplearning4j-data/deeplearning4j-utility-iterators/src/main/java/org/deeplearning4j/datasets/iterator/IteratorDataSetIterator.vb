Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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
	Public Class IteratorDataSetIterator
		Implements DataSetIterator

		Private ReadOnly iterator As IEnumerator(Of DataSet)
		Private ReadOnly batchSize As Integer
		Private ReadOnly queued As LinkedList(Of DataSet) 'Used when splitting larger examples than we want to return in a batch
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

'JAVA TO VB CONVERTER NOTE: The field inputColumns was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputColumns_Conflict As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field totalOutcomes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private totalOutcomes_Conflict As Integer = -1

		Private cursor As Integer = 0

		Public Sub New(ByVal iterator As IEnumerator(Of DataSet), ByVal batchSize As Integer)
			Me.iterator = iterator
			Me.batchSize = batchSize
			Me.queued = New LinkedList(Of DataSet)()
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return queued.Count > 0 OrElse iterator.hasNext()
		End Function

		Public Overrides Function [next]() As DataSet
			Return [next](batchSize)
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException()
			End If

			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			Dim countSoFar As Integer = 0
			Do While (queued.Count > 0 OrElse iterator.MoveNext()) AndAlso countSoFar < batchSize
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim next_Conflict As DataSet
				If queued.Count > 0 Then
					next_Conflict = queued.RemoveFirst()
				Else
					next_Conflict = iterator.Current
				End If
				Dim nExamples As Integer = next_Conflict.numExamples()
				If countSoFar + nExamples <= batchSize Then
					'Add the entire DataSet as-is
					list.Add(next_Conflict)
				Else
					'Otherwise, split it
					Dim toKeep As DataSet = DirectCast(next_Conflict.getRange(0, batchSize - countSoFar), DataSet)
					Dim toCache As DataSet = DirectCast(next_Conflict.getRange(batchSize - countSoFar, nExamples), DataSet)
					list.Add(toKeep)
					queued.AddLast(toCache)
				End If

				countSoFar += nExamples
			Loop

			If inputColumns_Conflict = -1 Then
				'Set columns etc for later use
				Dim temp As DataSet = list(0)

				inputColumns_Conflict = CInt(temp.Features.size(1))
				totalOutcomes_Conflict = If(temp.Labels Is Nothing, 0, CInt(temp.Labels.size(1))) 'May be null for layerwise pretraining
			End If

			Dim [out] As DataSet
			If list.Count = 1 Then
				[out] = list(0)
			Else
				[out] = DataSet.merge(list)
			End If

			If preProcessor_Conflict IsNot Nothing Then
				If Not [out].PreProcessed Then
					preProcessor_Conflict.preProcess([out])
					[out].markAsPreProcessed()
				End If
			End If
			cursor += [out].numExamples()
			Return [out]
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			If inputColumns_Conflict <> -1 Then
				Return inputColumns_Conflict
			End If
			prefetchBatchSetInputOutputValues()
			Return inputColumns_Conflict
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			If totalOutcomes_Conflict <> -1 Then
				Return totalOutcomes_Conflict
			End If
			prefetchBatchSetInputOutputValues()
			Return totalOutcomes_Conflict
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return False
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			Throw New System.NotSupportedException("Reset not supported")
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batchSize
		End Function

		Public Overridable WriteOnly Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Private Sub prefetchBatchSetInputOutputValues()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() Then
				Return
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iterator.next()
			inputColumns_Conflict = CInt([next].Features.size(1))
			totalOutcomes_Conflict = CInt([next].Labels.size(1))
			queued.AddLast([next])
		End Sub
	End Class

End Namespace