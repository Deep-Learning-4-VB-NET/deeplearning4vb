Imports System
Imports System.Collections.Generic
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.nd4j.linalg.dataset.api.iterator


	<Serializable>
	Public Class TestDataSetIterator
		Implements DataSetIterator

		Private Const serialVersionUID As Long = -7569201667767185411L
		Private curr As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private batch_Conflict As Integer = 10
		Private list As IList(Of DataSet)
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

		Public Sub New(ByVal dataset As DataSet, ByVal batch As Integer)
			Me.New(dataset.asList(), batch)
		End Sub

		Public Sub New(ByVal coll As IList(Of DataSet), ByVal batch As Integer)
			list = New List(Of DataSet)(coll)
			Me.batch_Conflict = batch
		End Sub

		''' <summary>
		''' This makes an iterator from the given dataset and batchsize
		''' ONLY for use in tests in nd4j
		''' Initializes with a default batch of 5
		''' </summary>
		''' <param name="dataset"> the dataset to make the iterator from </param>
		''' <param name="batch">   the batchsize for the iterator </param>
		Public Sub New(ByVal dataset As DataSet)
			Me.New(dataset, 5)

		End Sub

		Public Overrides Function hasNext() As Boolean
			SyncLock Me
				Return curr < list.Count
			End SyncLock
		End Function

		Public Overrides Function [next]() As DataSet
			SyncLock Me
				Return [next](batch_Conflict)
			End SyncLock
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return CInt(list(0).getFeatures().columns())
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return CInt(list(0).getLabels().columns())
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return False
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			SyncLock Me
				curr = 0
			End SyncLock
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batch_Conflict
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Dim [end] As Integer = curr + num

			Dim r As IList(Of DataSet) = New List(Of DataSet)()
			If [end] >= list.Count Then
				[end] = list.Count
			End If
			Do While curr < [end]
				r.Add(list(curr))
				curr += 1
			Loop

			Dim d As DataSet = DataSet.merge(r)
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(d)
			End If
			Return d
		End Function

	End Class

End Namespace