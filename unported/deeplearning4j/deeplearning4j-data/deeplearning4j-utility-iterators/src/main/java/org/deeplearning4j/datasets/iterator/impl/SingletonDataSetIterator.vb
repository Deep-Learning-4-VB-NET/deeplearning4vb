Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
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

Namespace org.deeplearning4j.datasets.iterator.impl


	<Serializable>
	Public Class SingletonDataSetIterator
		Implements DataSetIterator

		Private ReadOnly dataSet As DataSet
'JAVA TO VB CONVERTER NOTE: The field hasNext was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasNext_Conflict As Boolean = True
		Private preprocessed As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
		Private preProcessor As DataSetPreProcessor

		''' <param name="multiDataSet"> The underlying dataset to return </param>
		Public Sub New(ByVal multiDataSet As DataSet)
			Me.dataSet = multiDataSet
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return [next]()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return 0
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return 0
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return False
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			hasNext_Conflict = True
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return 0
		End Function

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			Return hasNext_Conflict
		End Function

		Public Overrides Function [next]() As DataSet
			If Not hasNext_Conflict Then
				Throw New NoSuchElementException("No elements remaining")
			End If
			hasNext_Conflict = False
			If preProcessor IsNot Nothing AndAlso Not preprocessed Then
				preProcessor.preProcess(dataSet)
				preprocessed = True
			End If
			Return dataSet
		End Function

		Public Overrides Sub remove()
			'No op
		End Sub
	End Class

End Namespace