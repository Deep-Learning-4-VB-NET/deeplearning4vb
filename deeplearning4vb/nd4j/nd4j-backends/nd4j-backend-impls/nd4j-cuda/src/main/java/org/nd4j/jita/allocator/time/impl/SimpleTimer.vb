Imports System
Imports RateTimer = org.nd4j.jita.allocator.time.RateTimer

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

Namespace org.nd4j.jita.allocator.time.impl


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class SimpleTimer
		Implements RateTimer

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile long timeframe;
		Protected Friend timeframe As Long
		Protected Friend ReadOnly latestEvent As New AtomicLong(0)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile long[] buckets;
		Protected Friend buckets() As Long
		Protected Friend ReadOnly lock As New ReentrantReadWriteLock()

		Public Sub New(ByVal timeframe As Long, ByVal timeUnit As TimeUnit)
			Me.timeframe = TimeUnit.MILLISECONDS.convert(timeframe, timeUnit)

			Dim bucketsize As Integer = CInt(Math.Truncate(TimeUnit.SECONDS.convert(timeframe, timeUnit)))
			Me.buckets = New Long(bucketsize - 1){}
		End Sub

		''' <summary>
		''' This method notifies timer about event
		''' </summary>
		Public Overridable Sub triggerEvent() Implements RateTimer.triggerEvent
			' delete all outdated events
			Try
				lock.writeLock().lock()
				Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
				If latestEvent.get() = 0 Then
					Me.latestEvent.set(currentTime)
				End If

				actualizeCounts(currentTime)
				Dim currentBin As Integer = CInt(Math.Truncate(TimeUnit.SECONDS.convert(currentTime, TimeUnit.MILLISECONDS))) Mod buckets.Length

				buckets(currentBin) += 1

				' nullify next bin
				If currentBin = buckets.Length - 1 Then
					buckets(0) = 0
				Else
					buckets(currentBin + 1) = 0
				End If

				' set new time
				Me.latestEvent.set(currentTime)
			Finally
				lock.writeLock().unlock()
			End Try
		End Sub

		Protected Friend Overridable Sub actualizeCounts(ByVal currentTime As Long)
			Dim currentBin As Integer = CInt(Math.Truncate(TimeUnit.SECONDS.convert(currentTime, TimeUnit.MILLISECONDS))) Mod buckets.Length

			Dim lastTime As Long = latestEvent.get()
			Dim expiredBinsNum As Integer = CInt(Math.Truncate(TimeUnit.SECONDS.convert(currentTime - lastTime, TimeUnit.MILLISECONDS)))

			If expiredBinsNum > 0 AndAlso expiredBinsNum < buckets.Length Then
				For x As Integer = 1 To expiredBinsNum
					Dim position As Integer = currentBin + x
					If position >= buckets.Length Then
						position -= buckets.Length
					End If
					buckets(position) = 0
				Next x
			ElseIf expiredBinsNum >= buckets.Length Then
				' nullify everything, counter is really outdated
				For x As Integer = 0 To buckets.Length - 1
					buckets(x) = 0
				Next x
			Else
				' do nothing here probably

			End If
		End Sub

		''' <summary>
		''' This method returns average frequency of events happened within predefined timeframe
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property FrequencyOfEvents As Double Implements RateTimer.getFrequencyOfEvents
			Get
				Return NumberOfEvents / CDbl(TimeUnit.SECONDS.convert(timeframe, TimeUnit.MILLISECONDS))
			End Get
		End Property

		Protected Friend Overridable Function sumCounts() As Long
			Dim result As Long = 0
			For x As Integer = 0 To buckets.Length - 1
				result += buckets(x)
			Next x

			Return result
		End Function

		''' <summary>
		''' This method returns total number of events happened withing predefined timeframe
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property NumberOfEvents As Long Implements RateTimer.getNumberOfEvents
			Get
				Try
					lock.readLock().lock()
					Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
					actualizeCounts(currentTime)
					Return sumCounts()
				Finally
					lock.readLock().unlock()
				End Try
			End Get
		End Property
	End Class

End Namespace