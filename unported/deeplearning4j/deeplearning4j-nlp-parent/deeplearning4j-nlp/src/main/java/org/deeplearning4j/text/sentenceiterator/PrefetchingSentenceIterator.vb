Imports System
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils
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

Namespace org.deeplearning4j.text.sentenceiterator


	<Obsolete>
	Public Class PrefetchingSentenceIterator
		Implements SentenceIterator

		Private sourceIterator As SentenceIterator
		Private fetchSize As Integer
		Private reader As AsyncIteratorReader
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As SentencePreProcessor

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(PrefetchingSentenceIterator))

		Private Sub New()

		End Sub

		''' <summary>
		''' Here we start async readers
		''' </summary>
		Private Sub init()
			reader = New AsyncIteratorReader(Me, sourceIterator, fetchSize, Me.preProcessor_Conflict)
			reader.Start()
		End Sub

		Public Overridable Function nextSentence() As String Implements SentenceIterator.nextSentence
			Return reader.nextLine()
		End Function

		Public Overridable Function hasNext() As Boolean Implements SentenceIterator.hasNext
			Return If(reader IsNot Nothing, reader.hasMoreLines(), False)
		End Function

		Public Overridable Sub reset() Implements SentenceIterator.reset
			If reader IsNot Nothing Then
				reader.reset()
			End If
		End Sub

		Public Overridable Sub finish() Implements SentenceIterator.finish
			If reader IsNot Nothing Then
				reader.terminate()
			End If
		End Sub

		Public Overridable Property PreProcessor As SentencePreProcessor Implements SentenceIterator.getPreProcessor
			Get
				Return preProcessor_Conflict
			End Get
			Set(ByVal preProcessor As SentencePreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override protected void finalize() throws Throwable
		Protected Overrides Sub Finalize()
			If reader IsNot Nothing Then
				reader.terminate()
			End If
			MyBase.Finalize()
		End Sub

		Public Class Builder
			Friend iterator As SentenceIterator
'JAVA TO VB CONVERTER NOTE: The field fetchSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend fetchSize_Conflict As Integer = 10000
			Friend preProcessor As SentencePreProcessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull SentenceIterator iterator)
			Public Sub New(ByVal iterator As SentenceIterator)
				Me.iterator = iterator
			End Sub

			Public Overridable Function setFetchSize(ByVal fetchSize As Integer) As Builder
				Me.fetchSize_Conflict = fetchSize
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setSentencePreProcessor(@NonNull SentencePreProcessor preProcessor)
			Public Overridable Function setSentencePreProcessor(ByVal preProcessor As SentencePreProcessor) As Builder
				Me.preProcessor = preProcessor
				Return Me
			End Function

			Public Overridable Function build() As PrefetchingSentenceIterator
				Dim pre As New PrefetchingSentenceIterator()
				pre.sourceIterator = Me.iterator
				pre.fetchSize = Me.fetchSize_Conflict
				pre.preProcessor = Me.preProcessor

				pre.init()
				Return pre
			End Function
		End Class

		Private Class AsyncIteratorReader
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As PrefetchingSentenceIterator

			Friend iterator As SentenceIterator
			Friend fetchSize As Integer
			Friend shouldTerminate As New AtomicBoolean(False)
			Friend lock As New ReentrantReadWriteLock()
			Friend preProcessor As SentencePreProcessor
			Friend isRunning As New AtomicBoolean(True)
			Friend buffer As ArrayBlockingQueue(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncIteratorReader(@NonNull SentenceIterator iterator, int fetchSize, SentencePreProcessor preProcessor)
			Public Sub New(ByVal outerInstance As PrefetchingSentenceIterator, ByVal iterator As SentenceIterator, ByVal fetchSize As Integer, ByVal preProcessor As SentencePreProcessor)
				Me.outerInstance = outerInstance
				Me.iterator = iterator
				Me.fetchSize = fetchSize
				Me.preProcessor = preProcessor

				buffer = New ArrayBlockingQueue(Of String)(fetchSize * 3)
				Me.setName("AsyncIteratorReader thread")
			End Sub

			Public Overrides Sub run()
				Do While Not shouldTerminate.get()
					If iterator.hasNext() Then
						isRunning.set(True)
					Else
						ThreadUtils.uncheckedSleep(50)
					End If
					Do While Not shouldTerminate.get() AndAlso iterator.hasNext()

						Dim cnt As Integer = 0
						If buffer.size() < fetchSize Then
							Do While Not shouldTerminate.get() AndAlso cnt < fetchSize AndAlso iterator.hasNext()
								Try
									lock.writeLock().lock()
									Dim line As String = iterator.nextSentence()
									If line IsNot Nothing Then
										buffer.add(If(Me.preProcessor Is Nothing, line, Me.preProcessor.preProcess(line)))
									End If
								Finally
									lock.writeLock().unlock()
								End Try
								cnt += 1
							Loop
							'                            log.info("Lines added: [" + cnt + "], buffer size: [" + buffer.size() + "]");
						Else
							ThreadUtils.uncheckedSleep(10)
						End If
					Loop
					isRunning.set(False)
				Loop
			End Sub

			Public Overridable Function nextLine() As String
				If Not buffer.isEmpty() Then
					Return buffer.poll()
				End If

				Try
					Return buffer.poll(2L, TimeUnit.SECONDS)
				Catch e As Exception
					Return Nothing
				End Try
			End Function

			Public Overridable Function hasMoreLines() As Boolean
				If Not buffer.isEmpty() Then
					Return True
				End If

				Try
					Me.lock.readLock().lock()
					Return iterator.hasNext() OrElse Not buffer.isEmpty()
				Finally
					Me.lock.readLock().unlock()
				End Try
			End Function

			Public Overridable Sub reset()
				Try
					lock.writeLock().lock()
					buffer.clear()
					iterator.reset()
				Catch e As Exception
					Throw New Exception(e)
				Finally
					lock.writeLock().unlock()
				End Try
			End Sub

			Public Overridable Sub terminate()
				shouldTerminate.set(True)
			End Sub
		End Class
	End Class

End Namespace