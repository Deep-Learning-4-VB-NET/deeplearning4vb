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
	''' This is simple implementation of DecayingTimer, it doesn't store any actual information for number of events happened.
	''' Just a fact: there were events, or there were no events
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class BinaryTimer
		Implements RateTimer

		Private timer As AtomicLong
		Private timeframeMilliseconds As Long

		Public Sub New(ByVal timeframe As Long, ByVal timeUnit As TimeUnit)
			timer = New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis())

			timeframeMilliseconds = TimeUnit.MILLISECONDS.convert(timeframe, timeUnit)
		End Sub

		''' <summary>
		''' This method notifies timer about event
		''' </summary>
		Public Overridable Sub triggerEvent() Implements RateTimer.triggerEvent
			timer.set(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		''' <summary>
		''' This method returns average frequency of events happened within predefined timeframe
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property FrequencyOfEvents As Double Implements RateTimer.getFrequencyOfEvents
			Get
				If Alive Then
					Return 1
				Else
					Return 0
				End If
			End Get
		End Property

		''' <summary>
		''' This method returns total number of events happened withing predefined timeframe
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property NumberOfEvents As Long Implements RateTimer.getNumberOfEvents
			Get
				If Alive Then
					Return 1
				Else
					Return 0
				End If
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property Alive As Boolean
			Get
				Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
    
				If currentTime - timer.get() > timeframeMilliseconds Then
					Return False
				End If
    
				Return True
			End Get
		End Property
	End Class

End Namespace