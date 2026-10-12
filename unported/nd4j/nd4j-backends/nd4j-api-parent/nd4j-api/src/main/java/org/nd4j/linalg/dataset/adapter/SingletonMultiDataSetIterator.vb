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
	Public Class SingletonMultiDataSetIterator
		Implements MultiDataSetIterator

		Private ReadOnly multiDataSet As MultiDataSet
'JAVA TO VB CONVERTER NOTE: The field hasNext was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasNext_Conflict As Boolean = True
		Private preprocessed As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As MultiDataSetPreProcessor

		''' <param name="multiDataSet"> The underlying MultiDataSet to return </param>
		Public Sub New(ByVal multiDataSet As MultiDataSet)
			Me.multiDataSet = multiDataSet
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return [next]()
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return False
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			hasNext_Conflict = True
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return hasNext_Conflict
		End Function

		Public Overrides Function [next]() As MultiDataSet
			If Not hasNext_Conflict Then
				Throw New NoSuchElementException("No elements remaining")
			End If
			hasNext_Conflict = False
			If preProcessor_Conflict IsNot Nothing AndAlso Not preprocessed Then
				preProcessor_Conflict.preProcess(multiDataSet)
				preprocessed = True
			End If
			Return multiDataSet
		End Function

		Public Overrides Sub remove()
			'No op
		End Sub
	End Class

End Namespace