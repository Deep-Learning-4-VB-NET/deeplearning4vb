Imports System
Imports System.Collections.Generic
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetCache = org.nd4j.linalg.dataset.api.iterator.cache.DataSetCache
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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
	Public Class CachingDataSetIterator
		Implements DataSetIterator

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DataSetCache))

		Private sourceIterator As DataSetIterator
		Private cache As DataSetCache
		Private [namespace] As String
		Private currentIndex As Integer = 0
		Private usingCache As Boolean = False
		Private allowPrefetching As Boolean

		Public Sub New(ByVal sourceIterator As DataSetIterator, ByVal cache As DataSetCache, ByVal [namespace] As String)
			Me.New(sourceIterator, cache, [namespace], False)
		End Sub

		Public Sub New(ByVal sourceIterator As DataSetIterator, ByVal cache As DataSetCache, ByVal [namespace] As String, ByVal allowPrefetching As Boolean)
			Me.sourceIterator = sourceIterator
			Me.cache = cache
			Me.namespace = [namespace]
			Me.currentIndex = 0

			Me.usingCache = cache.isComplete([namespace])
			Me.allowPrefetching = allowPrefetching
		End Sub

		Public Sub New(ByVal sourceIterator As DataSetIterator, ByVal cache As DataSetCache)
			Me.New(sourceIterator, cache, "default")
		End Sub

		Private Function makeKey(ByVal index As Integer) As String
			Return String.Format("data-set-cache-{0}-{1:D6}.bin", [namespace], index)
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return sourceIterator.inputColumns()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return sourceIterator.totalOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return allowPrefetching
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			sourceIterator.reset()
			currentIndex = 0
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return sourceIterator.batch()
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				sourceIterator.PreProcessor = preProcessor
			End Set
			Get
				Return sourceIterator.PreProcessor
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return sourceIterator.getLabels()
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			If usingCache Then
				Return cache.contains(makeKey(currentIndex))
			Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If sourceIterator.hasNext() Then
					Return True
				Else
					usingCache = True
					cache.setComplete([namespace], True)
					Return False
				End If
			End If
		End Function

		Public Overrides Function [next]() As DataSet
			Dim key As String = makeKey(currentIndex)

			Dim ds As DataSet

			If usingCache Then
				ds = cache.get(key)
			Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ds = sourceIterator.next()
				cache.put(key, ds)
			End If

			currentIndex += 1

			Return ds
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace