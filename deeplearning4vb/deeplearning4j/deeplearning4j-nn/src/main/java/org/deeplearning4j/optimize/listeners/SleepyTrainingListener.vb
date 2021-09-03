Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.optimize.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data @Builder @Slf4j public class SleepyTrainingListener extends org.deeplearning4j.optimize.api.BaseTrainingListener implements java.io.Serializable
	<Serializable>
	Public Class SleepyTrainingListener
		Inherits BaseTrainingListener

		Public Enum SleepMode
			''' <summary>
			''' In this mode parkNanos() call will be used, to make process really idle
			''' </summary>
			PARK

			''' <summary>
			''' In this mode Thread.sleep() call will be used, to make sleep traceable via profiler
			''' </summary>
			SLEEP

			''' <summary>
			''' Busy-lock will be used, to guarantee 100% thread use
			''' </summary>
			BUSY
		End Enum


		Public Enum TimeMode
			''' <summary>
			''' In this mode, listener will be trying to match specified time for a given invocation method.
			''' I.e. if iteration sleep is set to 500, and real iteration was 35 ms, thread will be sleeping for 465ms, to match target time of 500ms
			''' 
			''' </summary>
			ADDITIVE

			''' <summary>
			''' In this mode, listener will just call
			''' </summary>
			SIMPLE
		End Enum

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) protected final transient ThreadLocal<java.util.concurrent.atomic.AtomicLong> lastEE = new ThreadLocal<>();
		<NonSerialized>
		Protected Friend ReadOnly lastEE As New ThreadLocal(Of AtomicLong)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) protected final transient ThreadLocal<java.util.concurrent.atomic.AtomicLong> lastES = new ThreadLocal<>();
		<NonSerialized>
		Protected Friend ReadOnly lastES As New ThreadLocal(Of AtomicLong)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) protected final transient ThreadLocal<java.util.concurrent.atomic.AtomicLong> lastFF = new ThreadLocal<>();
		<NonSerialized>
		Protected Friend ReadOnly lastFF As New ThreadLocal(Of AtomicLong)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) protected final transient ThreadLocal<java.util.concurrent.atomic.AtomicLong> lastBP = new ThreadLocal<>();
		<NonSerialized>
		Protected Friend ReadOnly lastBP As New ThreadLocal(Of AtomicLong)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) protected final transient ThreadLocal<java.util.concurrent.atomic.AtomicLong> lastIteration = new ThreadLocal<>();
		<NonSerialized>
		Protected Friend ReadOnly lastIteration As New ThreadLocal(Of AtomicLong)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long timerEE = 0L;
		Protected Friend timerEE As Long = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long timerES = 0L;
		Protected Friend timerES As Long = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long timerFF = 0L;
		Protected Friend timerFF As Long = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long timerBP = 0L;
		Protected Friend timerBP As Long = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long timerIteration = 0L;
		Protected Friend timerIteration As Long = 0L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected SleepMode sleepMode = SleepMode.PARK;
		Protected Friend sleepMode As SleepMode = SleepMode.PARK

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected TimeMode timeMode = TimeMode.SIMPLE;
		Protected Friend timeMode As TimeMode = TimeMode.SIMPLE

		Protected Friend Overridable Sub sleep(ByVal sleepTimeMs As Long)
			If sleepTimeMs < 1 Then
				Return
			End If

			Select Case sleepMode
				Case org.deeplearning4j.optimize.listeners.SleepyTrainingListener.SleepMode.PARK
					ThreadUtils.uncheckedSleep(sleepTimeMs)
				Case org.deeplearning4j.optimize.listeners.SleepyTrainingListener.SleepMode.BUSY
					Dim target As Long = DateTimeHelper.CurrentUnixTimeMillis() + sleepTimeMs
					Do While DateTimeHelper.CurrentUnixTimeMillis() < target
						Thread.yield()
					Loop
				Case org.deeplearning4j.optimize.listeners.SleepyTrainingListener.SleepMode.SLEEP
					Try
						Thread.Sleep(sleepTimeMs)
					Catch e As InterruptedException
						Thread.CurrentThread.Interrupt()
						Throw New Exception(e)
					End Try
				Case Else
					Throw New System.InvalidOperationException("Unknown SleepMode value passed in: " & sleepMode)
			End Select
		End Sub

		Protected Friend Overridable Sub sleep(ByVal lastTime As AtomicLong, ByVal sleepTime As Long)
			If sleepTime = 0 Then
				Return
			End If

			' if that's SIMPLE mode - just sleep specific time, and go
			If timeMode = TimeMode.SIMPLE Then
				sleep(sleepTime)
				Return
			End If

			' we're skipping first iteration here, just sleeping fixed amount of time
			If lastTime Is Nothing Then
				sleep(sleepTime)
				Return
			End If


			' getting delta between real cycle time and desired one.
			Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim delta As Long = sleepTime - (currentTime - lastTime.get())

			sleep(delta)
		End Sub

		Public Overrides Sub onEpochStart(ByVal model As Model)
			sleep(lastES.get(), timerES)

			If lastES.get() Is Nothing Then
				lastES.set(New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis()))
			Else
				lastES.get().set(DateTimeHelper.CurrentUnixTimeMillis())
			End If
		End Sub

		Public Overrides Sub onEpochEnd(ByVal model As Model)
			sleep(lastEE.get(), timerEE)

			If lastEE.get() Is Nothing Then
				lastEE.set(New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis()))
			Else
				lastEE.get().set(DateTimeHelper.CurrentUnixTimeMillis())
			End If
		End Sub

		Public Overrides Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray))
			sleep(lastFF.get(), timerFF)

			If lastFF.get() Is Nothing Then
				lastFF.set(New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis()))
			Else
				lastFF.get().set(DateTimeHelper.CurrentUnixTimeMillis())
			End If
		End Sub

		Public Overrides Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray))
			sleep(lastFF.get(), timerFF)

			If lastFF.get() Is Nothing Then
				lastFF.set(New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis()))
			Else
				lastFF.get().set(DateTimeHelper.CurrentUnixTimeMillis())
			End If
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			sleep(lastIteration.get(), timerIteration)

			If lastIteration.get() Is Nothing Then
				lastIteration.set(New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis()))
			Else
				lastIteration.get().set(DateTimeHelper.CurrentUnixTimeMillis())
			End If
		End Sub

		Public Overrides Sub onBackwardPass(ByVal model As Model)
			sleep(lastBP.get(), timerBP)

			If lastBP.get() Is Nothing Then
				lastBP.set(New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis()))
			Else
				lastBP.get().set(DateTimeHelper.CurrentUnixTimeMillis())
			End If
		End Sub

		Public Overrides Sub onGradientCalculation(ByVal model As Model)
			'
		End Sub
	End Class

End Namespace