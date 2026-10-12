Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports ParallelMultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.ParallelMultiDataSetIterator

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

Namespace org.deeplearning4j.spark.parameterserver.iterators


	<Serializable>
	Public Class VirtualMultiDataSetIterator
		Implements ParallelMultiDataSetIterator

		Protected Friend ReadOnly iterators As IList(Of IEnumerator(Of MultiDataSet))
		Protected Friend ReadOnly position As AtomicInteger

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VirtualMultiDataSetIterator(@NonNull List<java.util.Iterator<org.nd4j.linalg.dataset.api.MultiDataSet>> iterators)
		Public Sub New(ByVal iterators As IList(Of IEnumerator(Of MultiDataSet)))
			Me.iterators = iterators
			Me.position = New AtomicInteger(0)
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return [next]()
		End Function

		Public Overridable Property PreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
    
			End Set
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean
			Return False
		End Function

		Public Overridable Function asyncSupported() As Boolean
			Return True
		End Function

		Public Overridable Sub reset()
			Throw New System.NotSupportedException()
		End Sub

		Public Overrides Function hasNext() As Boolean
			' just checking if that's not the last iterator, or if that's the last one - check if it has something
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ret As Boolean = position.get() < iterators.Count - 1 OrElse (position.get() < iterators.Count AndAlso iterators(position.get()).hasNext())
			Return ret
		End Function

		Public Overrides Function [next]() As MultiDataSet
			' TODO: this solution isn't ideal, it assumes non-empty iterators all the time. Would be nice to do something here
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterators(position.get()).hasNext() Then
				position.getAndIncrement()
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iterators(position.get()).next()
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Overridable Sub attachThread(ByVal producer As Integer) Implements ParallelMultiDataSetIterator.attachThread

		End Sub

		Public Overridable Function hasNextFor() As Boolean Implements ParallelMultiDataSetIterator.hasNextFor
			Return False
		End Function

		Public Overridable Function hasNextFor(ByVal consumer As Integer) As Boolean Implements ParallelMultiDataSetIterator.hasNextFor
			Return False
		End Function

		Public Overridable Function nextFor(ByVal consumer As Integer) As MultiDataSet Implements ParallelMultiDataSetIterator.nextFor
			Return Nothing
		End Function

		Public Overridable Function nextFor() As MultiDataSet Implements ParallelMultiDataSetIterator.nextFor
			Return Nothing
		End Function
	End Class

End Namespace