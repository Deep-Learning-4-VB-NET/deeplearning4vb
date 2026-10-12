Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
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
	Public Class ExistingDataSetIterator
		Implements DataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

		<NonSerialized>
		Private iterable As IEnumerable(Of DataSet)
		<NonSerialized>
		Private iterator As IEnumerator(Of DataSet)
		Private totalExamples As Integer = 0
		Private numFeatures As Integer = 0
		Private numLabels As Integer = 0
		Private labels As IList(Of String)

		''' <summary>
		''' Note that when using this constructor, resetting is not supported </summary>
		''' <param name="iterator"> Iterator to wrap </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExistingDataSetIterator(@NonNull Iterator<org.nd4j.linalg.dataset.DataSet> iterator)
		Public Sub New(ByVal iterator As IEnumerator(Of DataSet))
			Me.iterator = iterator
		End Sub

		''' <summary>
		''' Note that when using this constructor, resetting is not supported </summary>
		''' <param name="iterator"> Iterator to wrap </param>
		''' <param name="labels">   String labels. May be null. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExistingDataSetIterator(@NonNull Iterator<org.nd4j.linalg.dataset.DataSet> iterator, @NonNull List<String> labels)
		Public Sub New(ByVal iterator As IEnumerator(Of DataSet), ByVal labels As IList(Of String))
			Me.New(iterator)
			Me.labels = labels
		End Sub

		''' <summary>
		''' Wraps the specified iterable. Supports resetting </summary>
		''' <param name="iterable"> Iterable to wrap </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExistingDataSetIterator(@NonNull Iterable<org.nd4j.linalg.dataset.DataSet> iterable)
		Public Sub New(ByVal iterable As IEnumerable(Of DataSet))
			Me.iterable = iterable
			Me.iterator = iterable.GetEnumerator()
		End Sub

		''' <summary>
		''' Wraps the specified iterable. Supports resetting </summary>
		''' <param name="iterable"> Iterable to wrap </param>
		''' <param name="labels">   Labels list. May be null </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExistingDataSetIterator(@NonNull Iterable<org.nd4j.linalg.dataset.DataSet> iterable, @NonNull List<String> labels)
		Public Sub New(ByVal iterable As IEnumerable(Of DataSet), ByVal labels As IList(Of String))
			Me.New(iterable)
			Me.labels = labels
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExistingDataSetIterator(@NonNull Iterable<org.nd4j.linalg.dataset.DataSet> iterable, int totalExamples, int numFeatures, int numLabels)
		Public Sub New(ByVal iterable As IEnumerable(Of DataSet), ByVal totalExamples As Integer, ByVal numFeatures As Integer, ByVal numLabels As Integer)
			Me.New(iterable)

			Me.totalExamples = totalExamples
			Me.numFeatures = numFeatures
			Me.numLabels = numLabels
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			' TODO: this might be changed
			Throw New System.NotSupportedException("next(int) isn't supported")
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return numFeatures
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			If labels IsNot Nothing Then
				Return labels.Count
			End If

			Return numLabels
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return iterable IsNot Nothing
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			'No need to asynchronously prefetch here: already in memory
			Return False
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			If iterable IsNot Nothing Then
				Me.iterator = iterable.GetEnumerator()
			Else
				Throw New System.InvalidOperationException("To use reset() method you need to provide Iterable<DataSet>, not Iterator")
			End If
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return 0
		End Function

		Public Overridable WriteOnly Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return labels
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			If iterator IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return iterator.hasNext()
			End If

			Return False
		End Function

		Public Overrides Function [next]() As DataSet
			If preProcessor_Conflict IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = iterator.next()
				If Not ds.PreProcessed Then
					preProcessor_Conflict.preProcess(ds)
					ds.markAsPreProcessed()
				End If
				Return ds
			Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return iterator.next()
			End If
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub
	End Class

End Namespace