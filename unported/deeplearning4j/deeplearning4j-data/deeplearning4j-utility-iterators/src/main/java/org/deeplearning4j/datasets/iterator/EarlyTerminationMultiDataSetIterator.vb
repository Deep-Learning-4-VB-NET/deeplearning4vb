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

Namespace org.deeplearning4j.datasets.iterator

	<Serializable>
	Public Class EarlyTerminationMultiDataSetIterator
		Implements MultiDataSetIterator

		Private underlyingIterator As MultiDataSetIterator
		Private terminationPoint As Integer
		Private minibatchCount As Integer = 0

		''' <summary>
		''' Constructor takes the iterator to wrap and the number of minibatches after which the call to hasNext()
		''' will return false </summary>
		''' <param name="underlyingIterator">, iterator to wrap </param>
		''' <param name="terminationPoint">, minibatches after which hasNext() will return false </param>
		Public Sub New(ByVal underlyingIterator As MultiDataSetIterator, ByVal terminationPoint As Integer)
			If terminationPoint <= 0 Then
				Throw New System.ArgumentException("Termination point (the number of calls to .next() or .next(num)) has to be > 0")
			End If
			Me.underlyingIterator = underlyingIterator
			Me.terminationPoint = terminationPoint
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			If minibatchCount < terminationPoint Then
				minibatchCount += 1
				Return underlyingIterator.next(num)
			Else
				Throw New Exception("Calls to next have exceeded termination point.")
			End If
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				underlyingIterator.PreProcessor = preProcessor
			End Set
			Get
				Return underlyingIterator.PreProcessor
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return underlyingIterator.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return underlyingIterator.asyncSupported()
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			minibatchCount = 0
			underlyingIterator.reset()
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return underlyingIterator.hasNext() AndAlso minibatchCount < terminationPoint
		End Function

		Public Overrides Function [next]() As MultiDataSet
			If minibatchCount < terminationPoint Then
				minibatchCount += 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return underlyingIterator.next()
			Else
				Throw New Exception("Calls to next have exceeded the allotted number of minibatches.")
			End If
		End Function

		Public Overrides Sub remove()
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
			underlyingIterator.remove()
		End Sub
	End Class

End Namespace