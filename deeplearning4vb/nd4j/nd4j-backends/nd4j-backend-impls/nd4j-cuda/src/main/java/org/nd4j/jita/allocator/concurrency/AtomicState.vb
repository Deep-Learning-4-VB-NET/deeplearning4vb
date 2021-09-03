Imports System
Imports System.Threading
Imports AccessState = org.nd4j.jita.allocator.enums.AccessState

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

Namespace org.nd4j.jita.allocator.concurrency


	''' 
	''' <summary>
	''' Thread-safe atomic Tick/Tack/Toe implementation.
	''' 
	''' TODO: add more explanations here
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class AtomicState

'JAVA TO VB CONVERTER NOTE: The field currentState was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly currentState_Conflict As AtomicInteger

'JAVA TO VB CONVERTER NOTE: The field tickRequests was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly tickRequests_Conflict As New AtomicLong(0)
'JAVA TO VB CONVERTER NOTE: The field tackRequests was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly tackRequests_Conflict As New AtomicLong(0)
		Protected Friend ReadOnly toeRequests As New AtomicLong(0)

		Protected Friend ReadOnly waitingTicks As New AtomicLong(0)

		Protected Friend ReadOnly isToeWaiting As New AtomicBoolean(False)
		Protected Friend ReadOnly isToeScheduled As New AtomicBoolean(False)

		Protected Friend ReadOnly toeThread As New AtomicLong(0)

		Public Sub New()
			Me.New(AccessState.TACK)
		End Sub

		Public Sub New(ByVal initialStatus As AccessState)
			currentState_Conflict = New AtomicInteger(initialStatus.ordinal())
		End Sub

		''' <summary>
		''' This method requests to change state to Tick.
		''' 
		''' PLEASE NOTE: this method is blocking, if memory is in Toe state
		''' </summary>
		Public Overridable Sub requestTick()
			requestTick(10, TimeUnit.SECONDS)
		End Sub

		''' <summary>
		''' This method requests to change state to Tick.
		''' 
		''' PLEASE NOTE: this method is blocking, if memory is in Toe state.
		''' PLEASE NOTE: if Tick can't be acquired within specified timeframe, exception will be thrown
		''' </summary>
		''' <param name="time"> </param>
		''' <param name="timeUnit"> </param>
		Public Overridable Sub requestTick(ByVal time As Long, ByVal timeUnit As TimeUnit)
			Dim timeframeMs As Long = TimeUnit.MILLISECONDS.convert(time, timeUnit)
			Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim isWaiting As Boolean = False

			' if we have Toe request queued - we' have to wait till it finishes.
			Try
				Do While isToeScheduled.get() OrElse isToeWaiting.get() OrElse CurrentState = AccessState.TOE
					If Not isWaiting Then
						isWaiting = True
						waitingTicks.incrementAndGet()
					End If
					Thread.Sleep(50)
				Loop

				currentState_Conflict.set(AccessState.TICK.ordinal())
				waitingTicks.decrementAndGet()
				tickRequests_Conflict.incrementAndGet()
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method requests to change state to Tack
		''' 
		''' 
		''' </summary>
		Public Overridable Sub requestTack()
			currentState_Conflict.set(AccessState.TACK.ordinal())
			tackRequests_Conflict.incrementAndGet()
		End Sub

		''' 
		''' <summary>
		''' This method requests to change state to Toe
		''' 
		''' 
		''' PLEASE NOTE: this method is blocking, untill all Tick requests are brought down to Tack state;
		''' 
		''' </summary>
		Public Overridable Sub requestToe()
			isToeWaiting.set(True)
			Try

				Do While CurrentState <> AccessState.TACK
					' now we make TOE reentrant
					If CurrentState = AccessState.TOE AndAlso toeThread.get() = Thread.CurrentThread.getId() Then
						Exit Do
					End If
					Thread.Sleep(20)
				Loop

				toeRequests.incrementAndGet()
				currentState_Conflict.set(AccessState.TOE.ordinal())

				toeThread.set(Thread.CurrentThread.getId())

				isToeWaiting.set(False)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method requests to change state to Toe
		''' 
		''' PLEASE NOTE: this method is non-blocking, if Toe request is impossible atm, it will return false.
		''' </summary>
		''' <returns> TRUE, if Toe state entered, FALSE otherwise </returns>
		Public Overridable Function tryRequestToe() As Boolean
			scheduleToe()
			If isToeWaiting.get() OrElse CurrentState = AccessState.TOE Then
				'System.out.println("discarding TOE");
				discardScheduledToe()
				Return False
			Else
				'System.out.println("requesting TOE");
				discardScheduledToe()
				requestToe()
				Return True
			End If
		End Function

		''' <summary>
		''' This method requests release Toe status back to Tack.
		''' 
		''' PLEASE NOTE: only the thread originally entered Toe state is able to release it.
		''' </summary>
		Public Overridable Sub releaseToe()
			If CurrentState = AccessState.TOE Then
				If 1 > 0 Then
					'if (toeThread.get() == Thread.currentThread().getId()) {
					If toeRequests.decrementAndGet() = 0 Then
						tickRequests_Conflict.set(0)
						tackRequests_Conflict.set(0)

						currentState_Conflict.set(AccessState.TACK.ordinal())
					End If
				Else
					Throw New System.InvalidOperationException("releaseToe() is called from different thread.")
				End If
			Else
				Throw New System.InvalidOperationException("Object is NOT in Toe state!")
			End If
		End Sub

		''' <summary>
		''' This method returns the current memory state
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property CurrentState As AccessState
			Get
				If System.Enum.GetValues(GetType(AccessState))(currentState_Conflict.get()) = AccessState.TOE Then
					Return AccessState.TOE
				Else
					If tickRequests_Conflict.get() <= tackRequests_Conflict.get() Then
    
						' TODO: looks like this piece of code should be locked :/
						tickRequests_Conflict.set(0)
						tackRequests_Conflict.set(0)
    
						Return AccessState.TACK
					Else
						Return AccessState.TICK
					End If
				End If
			End Get
		End Property

		''' <summary>
		''' This methods
		''' </summary>
		''' <returns> number of WAITING tick requests, if they are really WAITING. If state isn't Toe, return value will always be 0. </returns>
		Public Overridable ReadOnly Property WaitingTickRequests As Long
			Get
				Return waitingTicks.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of current Tick sessions
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property TickRequests As Long
			Get
				Return tickRequests_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of current Tack sessions
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property TackRequests As Long
			Get
				Return tackRequests_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' This method checks, if Toe state can be entered.
		''' </summary>
		''' <returns> True if Toe is available, false otherwise </returns>
		Public Overridable ReadOnly Property ToeAvailable As Boolean
			Get
				Return CurrentState = AccessState.TACK
			End Get
		End Property

		''' <summary>
		''' This method schedules Toe state entry, but doesn't enters it.
		''' </summary>
		Public Overridable Sub scheduleToe()
			isToeScheduled.set(True)
		End Sub

		''' <summary>
		''' This method discards scheduled Toe state entry, but doesn't exits currently entered Toe state, if that's the case.
		''' </summary>
		Public Overridable Sub discardScheduledToe()
			If isToeScheduled.get() Then
				isToeScheduled.set(False)
			End If
		End Sub
	End Class

End Namespace