Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class VirtualIterator<E> extends java.util.Observable implements java.util.Iterator<E>
	Public Class VirtualIterator(Of E)
		Inherits java.util.Observable
		Implements IEnumerator(Of E)

		' TODO: use AsyncIterator here?
		Protected Friend iterator As IEnumerator(Of E)
		Protected Friend state As New AtomicBoolean(True)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VirtualIterator(@NonNull Iterator<E> iterator)
		Public Sub New(ByVal iterator As IEnumerator(Of E))
			Me.iterator = iterator
		End Sub


		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim u As Boolean = iterator.hasNext()
			state.compareAndSet(True, u)
			If Not state.get() Then
				Me.setChanged()
				notifyObservers()
			End If
			Return u
		End Function

		Public Overrides Function [next]() As E
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iterator.next()
		End Function

		Public Overrides Sub remove()
			' no-op, we don't need this call implemented
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: @Override public void forEachRemaining(java.util.function.Consumer<? super E> action)
		Public Overrides Sub forEachRemaining(Of T1)(ByVal action As System.Action(Of T1))
			iterator.forEachRemaining(action)
			state.compareAndSet(True, False)
		End Sub

		''' <summary>
		''' This method blocks until underlying Iterator is depleted
		''' </summary>
		Public Overridable Sub blockUntilDepleted()
			Do While state.get()
				LockSupport.parkNanos(1000L)
			Loop
		End Sub
	End Class

End Namespace