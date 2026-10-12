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


	Public Class SimilarityListener(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements VectorsListener(Of T)

		Protected Friend Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(SimilarityListener))
		Private ReadOnly targetEvent As ListenerEvent
		Private ReadOnly frequency As Integer
		Private ReadOnly element1 As String
		Private ReadOnly element2 As String
		Private ReadOnly counter As New AtomicLong(0)

		Public Sub New(ByVal targetEvent As ListenerEvent, ByVal frequency As Integer, ByVal label1 As String, ByVal label2 As String)
			Me.targetEvent = targetEvent
			Me.frequency = frequency
			Me.element1 = label1
			Me.element2 = label2
		End Sub

		Public Overridable Function validateEvent(ByVal [event] As ListenerEvent, ByVal argument As Long) As Boolean Implements VectorsListener(Of T).validateEvent
			Return [event] = targetEvent
		End Function

		Public Overridable Sub processEvent(ByVal [event] As ListenerEvent, ByVal sequenceVectors As SequenceVectors(Of T), ByVal argument As Long) Implements VectorsListener(Of T).processEvent
			If [event] <> targetEvent Then
				Return
			End If

			Dim cnt As Long = counter.getAndIncrement()

			If cnt Mod frequency <> 0 Then
				Return
			End If

			Dim similarity As Double = sequenceVectors.similarity(element1, element2)

			logger.info("Invocation: {}, similarity: {}", cnt, similarity)
		End Sub
	End Class

End Namespace