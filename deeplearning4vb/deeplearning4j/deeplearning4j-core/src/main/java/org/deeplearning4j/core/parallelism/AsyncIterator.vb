Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Getter = lombok.Getter
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

Namespace org.deeplearning4j.core.parallelism


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class AsyncIterator<T extends Object> implements java.util.Iterator<T>
	Public Class AsyncIterator(Of T As Object)
		Implements IEnumerator(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected java.util.concurrent.BlockingQueue<T> buffer;
		Protected Friend buffer As BlockingQueue(Of T)
		Protected Friend thread As ReaderThread(Of T)
		Protected Friend iterator As IEnumerator(Of T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected T terminator = (T) new Object();
		Protected Friend terminator As T = DirectCast(New Object(), T)
		Protected Friend nextElement As T
		Protected Friend shouldWork As New AtomicBoolean(True)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncIterator(@NonNull Iterator<T> iterator, int bufferSize)
		Public Sub New(ByVal iterator As IEnumerator(Of T), ByVal bufferSize As Integer)
			Me.buffer = New LinkedBlockingQueue(Of T)(bufferSize)
			Me.iterator = iterator

			thread = New ReaderThread(Me, Of )(iterator, Me.buffer, terminator)
			thread.Start()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncIterator(@NonNull Iterator<T> iterator)
		Public Sub New(ByVal iterator As IEnumerator(Of T))
			Me.New(iterator, 1024)
		End Sub

		Public Overrides Function hasNext() As Boolean
			Try
				If nextElement IsNot Nothing AndAlso nextElement IsNot terminator Then
					Return True
				End If

				' if on previous run we've got terminator - just return false
				If nextElement Is terminator Then
					Return False
				End If

				nextElement = buffer.take()

				' same on this run
				Return (nextElement IsNot terminator)
			Catch e As Exception
				log.error("Premature end of loop!")
				Return False
			End Try
		End Function

		Public Overrides Function [next]() As T
			Dim temp As T = nextElement
			nextElement = If(temp Is terminator, terminator, Nothing)
			Return temp
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Overridable Sub shutdown()
			If shouldWork.get() Then
				shouldWork.set(False)
				thread.Interrupt()
				Try
					' Shutdown() should be a synchronous operation since the iterator is reset after shutdown() is
					' called in AsyncLabelAwareIterator.reset().
					thread.Join()
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
				End Try
				nextElement = terminator
				buffer.clear()
			End If
		End Sub

		Private Class ReaderThread(Of T)
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As AsyncIterator(Of T)

			Friend buffer As BlockingQueue(Of T)
			Friend iterator As IEnumerator(Of T)
			Friend terminator As T

			Public Sub New(ByVal outerInstance As AsyncIterator(Of T), ByVal iterator As IEnumerator(Of T), ByVal buffer As BlockingQueue(Of T), ByVal terminator As T)
				Me.outerInstance = outerInstance
				Me.buffer = buffer
				Me.iterator = iterator
				Me.terminator = terminator

				setDaemon(True)
				setName("AsyncIterator Reader thread")
			End Sub

			Public Overrides Sub run()
				'log.info("AsyncReader [{}] started", Thread.currentThread().getId());
				Try
					Do While iterator.MoveNext() AndAlso outerInstance.shouldWork.get()
						Dim smth As T = iterator.Current

						If smth IsNot Nothing Then
							buffer.put(smth)
						End If
					Loop
					buffer.put(terminator)
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					' do nothing
					outerInstance.shouldWork.set(False)
				Catch e As Exception
					' TODO: pass that forward
					Throw New Exception(e)
				Finally
					'log.info("AsyncReader [{}] stopped", Thread.currentThread().getId());
				End Try
			End Sub
		End Class
	End Class

End Namespace