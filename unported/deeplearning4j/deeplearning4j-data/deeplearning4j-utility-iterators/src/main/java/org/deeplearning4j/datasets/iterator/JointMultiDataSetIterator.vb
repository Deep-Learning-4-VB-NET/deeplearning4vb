Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NoArgsConstructor @AllArgsConstructor public class JointMultiDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
	<Serializable>
	Public Class JointMultiDataSetIterator
		Implements MultiDataSetIterator

'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As MultiDataSetPreProcessor
		Protected Friend iterators As ICollection(Of DataSetIterator)
		Protected Friend outcome As Integer = -1

		''' <param name="iterators"> Underlying iterators to wrap </param>
		Public Sub New(ParamArray ByVal iterators() As DataSetIterator)
			Me.iterators = New List(Of DataSetIterator)()
			Me.iterators.addAll(Arrays.asList(iterators))
			Me.outcome = -1
		End Sub

		''' 
		''' <param name="outcome">   Index to get the label from. If < 0, labels from all iterators will be used to create the
		'''                  final MultiDataSet </param>
		''' <param name="iterators"> Underlying iterators to wrap </param>
		Public Sub New(ByVal outcome As Integer, ParamArray ByVal iterators() As DataSetIterator)
			Me.New(iterators)
			Me.outcome = outcome
		End Sub

		''' <summary>
		''' Fetch the next 'num' examples. Similar to the next method, but returns a specified number of examples
		''' </summary>
		''' <param name="num"> Number of examples to fetch </param>
		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' Set the preprocessor to be applied to each MultiDataSet, before each MultiDataSet is returned.
		''' </summary>
		''' <param name="preProcessor"> MultiDataSetPreProcessor. May be null. </param>
		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		''' <summary>
		''' Is resetting supported by this DataSetIterator? Many DataSetIterators do support resetting,
		''' but some don't
		''' </summary>
		''' <returns> true if reset method is supported; false otherwise </returns>
		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Dim sup As Boolean = True

			For Each i As val In iterators
				If Not i.resetSupported() Then
					sup = False
					Exit For
				End If
			Next i

			Return sup
		End Function

		''' <summary>
		''' Does this MultiDataSetIterator support asynchronous prefetching of multiple MultiDataSet objects?
		''' Most MultiDataSetIterators do, but in some cases it may not make sense to wrap this iterator in an
		''' iterator that does asynchronous prefetching. For example, it would not make sense to use asynchronous
		''' prefetching for the following types of iterators:
		''' (a) Iterators that store their full contents in memory already
		''' (b) Iterators that re-use features/labels arrays (as future next() calls will overwrite past contents)
		''' (c) Iterators that already implement some level of asynchronous prefetching
		''' (d) Iterators that may return different data depending on when the next() method is called
		''' </summary>
		''' <returns> true if asynchronous prefetching from this iterator is OK; false if asynchronous prefetching should not
		''' be used with this iterator </returns>
		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Dim sup As Boolean = True

			For Each i As val In iterators
				If Not i.asyncSupported() Then
					sup = False
					Exit For
				End If
			Next i

			Return sup
		End Function

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			For Each i As val In iterators
				i.reset()
			Next i
		End Sub

		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
		Public Overrides Function hasNext() As Boolean
			Dim has As Boolean = True

			For Each i As val In iterators
				If Not i.hasNext() Then
					has = False
					Exit For
				End If
			Next i

			Return has
		End Function

		''' <summary>
		''' Returns the next element in the iteration.
		''' </summary>
		''' <returns> the next element in the iteration </returns>
		Public Overrides Function [next]() As MultiDataSet
			Dim features As val = New List(Of INDArray)()
			Dim labels As val = New List(Of INDArray)()
			Dim featuresMask As val = New List(Of INDArray)()
			Dim labelsMask As val = New List(Of INDArray)()

			Dim hasFM As Boolean = False
			Dim hasLM As Boolean = False

			Dim cnt As Integer = 0
			For Each i As val In iterators
				Dim ds As val = i.next()

				features.add(ds.getFeatures())
				featuresMask.add(ds.getFeaturesMaskArray())

				If outcome < 0 OrElse cnt = outcome Then
					labels.add(ds.getLabels())
					labelsMask.add(ds.getLabelsMaskArray())
				End If

				If ds.getFeaturesMaskArray() IsNot Nothing Then
					hasFM = True
				End If

				If ds.getLabelsMaskArray() IsNot Nothing Then
					hasLM = True
				End If

				cnt += 1
			Next i

			Dim fm() As INDArray = If(hasFM, featuresMask.toArray(New INDArray(){}), Nothing)
			Dim lm() As INDArray = If(hasLM, labelsMask.toArray(New INDArray(){}), Nothing)

			Dim mds As val = New org.nd4j.linalg.dataset.MultiDataSet(features.toArray(New INDArray(){}), labels.toArray(New INDArray(){}), fm, lm)

			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(mds)
			End If

			Return mds
		End Function

		''' <summary>
		''' PLEASE NOTE: This method is NOT implemented
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
			' noopp
		End Sub
	End Class

End Namespace