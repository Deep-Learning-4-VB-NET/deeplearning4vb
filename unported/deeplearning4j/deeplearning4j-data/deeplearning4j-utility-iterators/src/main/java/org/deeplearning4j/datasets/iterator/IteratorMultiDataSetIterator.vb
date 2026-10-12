Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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
	Public Class IteratorMultiDataSetIterator
		Implements MultiDataSetIterator

		Private ReadOnly iterator As IEnumerator(Of MultiDataSet)
		Private ReadOnly batchSize As Integer
		Private ReadOnly queued As LinkedList(Of MultiDataSet) 'Used when splitting larger examples than we want to return in a batch
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As MultiDataSetPreProcessor

		Public Sub New(ByVal iterator As IEnumerator(Of MultiDataSet), ByVal batchSize As Integer)
			Me.iterator = iterator
			Me.batchSize = batchSize
			Me.queued = New LinkedList(Of MultiDataSet)()
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return queued.Count > 0 OrElse iterator.hasNext()
		End Function

		Public Overrides Function [next]() As MultiDataSet
			Return [next](batchSize)
		End Function

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException()
			End If

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
			Dim countSoFar As Integer = 0
			Do While (queued.Count > 0 OrElse iterator.MoveNext()) AndAlso countSoFar < batchSize
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim next_Conflict As MultiDataSet
				If queued.Count > 0 Then
					next_Conflict = queued.RemoveFirst()
				Else
					next_Conflict = iterator.Current
				End If

				Dim nExamples As Long = next_Conflict.getFeatures(0).size(0)
				If countSoFar + nExamples <= batchSize Then
					'Add the entire MultiDataSet as-is
					list.Add(next_Conflict)
				Else
					'Split the MultiDataSet

					Dim nFeatures As Integer = next_Conflict.numFeatureArrays()
					Dim nLabels As Integer = next_Conflict.numLabelsArrays()

					Dim fToKeep(nFeatures - 1) As INDArray
					Dim lToKeep(nLabels - 1) As INDArray
					Dim fToCache(nFeatures - 1) As INDArray
					Dim lToCache(nLabels - 1) As INDArray
					Dim fMaskToKeep() As INDArray = (If(next_Conflict.FeaturesMaskArrays IsNot Nothing, New INDArray(nFeatures - 1){}, Nothing))
					Dim lMaskToKeep() As INDArray = (If(next_Conflict.LabelsMaskArrays IsNot Nothing, New INDArray(nLabels - 1){}, Nothing))
					Dim fMaskToCache() As INDArray = (If(next_Conflict.FeaturesMaskArrays IsNot Nothing, New INDArray(nFeatures - 1){}, Nothing))
					Dim lMaskToCache() As INDArray = (If(next_Conflict.LabelsMaskArrays IsNot Nothing, New INDArray(nLabels - 1){}, Nothing))

					For i As Integer = 0 To nFeatures - 1
						Dim fi As INDArray = next_Conflict.getFeatures(i)
						fToKeep(i) = getRange(fi, 0, batchSize - countSoFar)
						fToCache(i) = getRange(fi, batchSize - countSoFar, nExamples)

						If fMaskToKeep IsNot Nothing Then
							Dim fmi As INDArray = next_Conflict.getFeaturesMaskArray(i)
							fMaskToKeep(i) = getRange(fmi, 0, batchSize - countSoFar)
							fMaskToCache(i) = getRange(fmi, batchSize - countSoFar, nExamples)
						End If
					Next i

					For i As Integer = 0 To nLabels - 1
						Dim li As INDArray = next_Conflict.getLabels(i)
						lToKeep(i) = getRange(li, 0, batchSize - countSoFar)
						lToCache(i) = getRange(li, batchSize - countSoFar, nExamples)

						If lMaskToKeep IsNot Nothing Then
							Dim lmi As INDArray = next_Conflict.getLabelsMaskArray(i)
							lMaskToKeep(i) = getRange(lmi, 0, batchSize - countSoFar)
							lMaskToCache(i) = getRange(lmi, batchSize - countSoFar, nExamples)
						End If
					Next i

					Dim toKeep As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(fToKeep, lToKeep, fMaskToKeep, lMaskToKeep)
					Dim toCache As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(fToCache, lToCache, fMaskToCache, lMaskToCache)
					list.Add(toKeep)
					queued.AddLast(toCache)
				End If

				countSoFar += nExamples
			Loop

			Dim [out] As MultiDataSet
			If list.Count = 1 Then
				[out] = list(0)
			Else
				[out] = org.nd4j.linalg.dataset.MultiDataSet.merge(list)
			End If

			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess([out])
			End If
			Return [out]
		End Function

		Private Shared Function getRange(ByVal arr As INDArray, ByVal exampleFrom As Long, ByVal exampleToExclusive As Long) As INDArray
			If arr Is Nothing Then
				Return Nothing
			End If

			Dim rank As Integer = arr.rank()
			Select Case rank
				Case 2
					Return arr.get(NDArrayIndex.interval(exampleFrom, exampleToExclusive), NDArrayIndex.all())
				Case 3
					Return arr.get(NDArrayIndex.interval(exampleFrom, exampleToExclusive), NDArrayIndex.all(), NDArrayIndex.all())
				Case 4
					Return arr.get(NDArrayIndex.interval(exampleFrom, exampleToExclusive), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
				Case Else
					Throw New Exception("Invalid rank: " & rank)
			End Select
		End Function

		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return False
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			'No need to asynchronously prefetch here: already in memory
			Return False
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			Throw New System.NotSupportedException("Reset not supported")
		End Sub

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub
	End Class

End Namespace