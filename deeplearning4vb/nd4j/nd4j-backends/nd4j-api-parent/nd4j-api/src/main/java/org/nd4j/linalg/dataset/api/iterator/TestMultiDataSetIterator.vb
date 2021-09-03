Imports System
Imports System.Collections.Generic
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor

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


	''' <summary>
	''' @author Ede Meijer
	''' </summary>
	<Serializable>
	Public Class TestMultiDataSetIterator
		Implements MultiDataSetIterator

		Private curr As Integer = 0
		Private batch As Integer = 10
		Private list As IList(Of MultiDataSet)
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As MultiDataSetPreProcessor

		''' <summary>
		''' Makes an iterator from the given datasets. DataSets are expected to are batches of exactly 1 example.
		''' ONLY for use in tests in nd4j
		''' </summary>
		Public Sub New(ByVal batch As Integer, ParamArray ByVal dataset() As MultiDataSet)
			list = New List(Of MultiDataSet) From {dataset}
			Me.batch = batch
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			Dim [end] As Integer = curr + num

			Dim r As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
			If [end] >= list.Count Then
				[end] = list.Count
			End If
			Do While curr < [end]
				r.Add(list(curr))
				curr += 1
			Loop

			Dim d As MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet.merge(r)
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(d)
			End If
			Return d
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return Me.preProcessor_Conflict
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return False
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			curr = 0
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return curr < list.Count
		End Function

		Public Overrides Function [next]() As MultiDataSet
			Return [next](batch)
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace