Imports System
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
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

Namespace org.nd4j.linalg.dataset.adapter

	<Serializable>
	Public Class MultiDataSetIteratorAdapter
		Implements MultiDataSetIterator

		Private iter As org.nd4j.linalg.dataset.api.iterator.DataSetIterator
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As MultiDataSetPreProcessor

		Public Sub New(ByVal iter As org.nd4j.linalg.dataset.api.iterator.DataSetIterator)
			Me.iter = iter
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			Dim mds As MultiDataSet = iter.next(i).toMultiDataSet()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(mds)
			End If
			Return mds
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal multiDataSetPreProcessor As MultiDataSetPreProcessor)
				Me.preProcessor_Conflict = multiDataSetPreProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return iter.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return iter.asyncSupported()
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			iter.reset()
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.hasNext()
		End Function

		Public Overrides Function [next]() As MultiDataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = iter.next().toMultiDataSet()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(mds)
			End If
			Return mds
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

	End Class

End Namespace