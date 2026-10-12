Imports System
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.sequencevectors
Imports ListenerEvent = org.deeplearning4j.models.sequencevectors.enums.ListenerEvent
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
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

Namespace org.deeplearning4j.models.sequencevectors.listeners


	<Obsolete>
	Public Class ScoreListener(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements VectorsListener(Of T)

		Protected Friend Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(ScoreListener))
		Private ReadOnly targetEvent As ListenerEvent
		Private ReadOnly callsCount As New AtomicLong(0)
		Private ReadOnly frequency As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ScoreListener(@NonNull ListenerEvent targetEvent, int frequency)
		Public Sub New(ByVal targetEvent As ListenerEvent, ByVal frequency As Integer)
			Me.targetEvent = targetEvent
			Me.frequency = frequency
		End Sub

		Public Overridable Function validateEvent(ByVal [event] As ListenerEvent, ByVal argument As Long) As Boolean Implements VectorsListener(Of T).validateEvent
			If [event] = targetEvent Then
				Return True
			End If

			Return False
		End Function

		Public Overridable Sub processEvent(ByVal [event] As ListenerEvent, ByVal sequenceVectors As SequenceVectors(Of T), ByVal argument As Long) Implements VectorsListener(Of T).processEvent
			If [event] <> targetEvent Then
				Return
			End If

			callsCount.incrementAndGet()

			If callsCount.get() Mod frequency = 0 Then
				logger.info("Average score for last batch: {}", sequenceVectors.ElementsScore)
			End If
		End Sub
	End Class

End Namespace