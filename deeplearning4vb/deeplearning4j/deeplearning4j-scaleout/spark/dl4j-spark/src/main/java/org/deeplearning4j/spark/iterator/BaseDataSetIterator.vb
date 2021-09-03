Imports System
Imports System.Collections.Generic
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException

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

Namespace org.deeplearning4j.spark.iterator


	<Serializable>
	Public MustInherit Class BaseDataSetIterator(Of T)
		Implements DataSetIterator

		Protected Friend dataSetStreams As ICollection(Of T)
'JAVA TO VB CONVERTER NOTE: The field preprocessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preprocessor_Conflict As DataSetPreProcessor
		Protected Friend iter As IEnumerator(Of T)
'JAVA TO VB CONVERTER NOTE: The field totalOutcomes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend totalOutcomes_Conflict As Long = -1
'JAVA TO VB CONVERTER NOTE: The field inputColumns was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputColumns_Conflict As Long = -1
'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend batch_Conflict As Integer = -1
		Protected Friend preloadedDataSet As DataSet
		Protected Friend cursor As Integer = 0

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Return [next]()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			If inputColumns_Conflict = -1 Then
				preloadDataSet()
			End If
			Return CInt(inputColumns_Conflict)
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			If totalOutcomes_Conflict = -1 Then
				preloadDataSet()
			End If
			If preloadedDataSet Is Nothing OrElse preloadedDataSet.Labels Is Nothing Then
				Return 0
			End If
			Return CInt(preloadedDataSet.Labels.size(1))
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return dataSetStreams IsNot Nothing
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			If dataSetStreams Is Nothing Then
				Throw New System.InvalidOperationException("Cannot reset iterator constructed with an iterator")
			End If
			iter = dataSetStreams.GetEnumerator()
			cursor = 0
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			If batch_Conflict = -1 Then
				preloadDataSet()
			End If
			Return batch_Conflict
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preprocessor_Conflict = preProcessor
			End Set
			Get
				Return Me.preprocessor_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return preloadedDataSet IsNot Nothing OrElse iter.hasNext()
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

		Private Sub preloadDataSet()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			preloadedDataSet = load(iter.next())

			If preloadedDataSet.Labels.size(1) > Integer.MaxValue OrElse preloadedDataSet.Features.size(1) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			totalOutcomes_Conflict = CInt(preloadedDataSet.Labels.size(1))
			inputColumns_Conflict = CInt(preloadedDataSet.Features.size(1))
		End Sub


		Protected Friend MustOverride Function load(ByVal ds As T) As DataSet
	End Class

End Namespace