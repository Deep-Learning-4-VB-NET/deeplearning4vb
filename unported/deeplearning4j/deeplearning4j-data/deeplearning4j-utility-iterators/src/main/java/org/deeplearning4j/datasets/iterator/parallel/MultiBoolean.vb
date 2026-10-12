Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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

Namespace org.deeplearning4j.datasets.iterator.parallel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class MultiBoolean
	Public Class MultiBoolean
		Private ReadOnly numEntries As Integer
		Private holder As Integer = 0
		Private max As Integer = 0
		Private oneTime As Boolean
		Private timeTracker As MultiBoolean

		Public Sub New(ByVal numEntries As Integer)
			Me.New(numEntries, False)
		End Sub

		Public Sub New(ByVal numEntries As Integer, ByVal initialValue As Boolean)
			Me.New(numEntries, initialValue, False)
		End Sub

		Public Sub New(ByVal numEntries As Integer, ByVal initialValue As Boolean, ByVal oneTime As Boolean)
			If numEntries > 32 Then
				Throw New System.NotSupportedException("Up to 32 entries can be tracked at once.")
			End If

			Me.oneTime = oneTime
			Me.numEntries = numEntries
			For i As Integer = 1 To numEntries
				Me.max = Me.max Or 1 << i
			Next i

			If initialValue Then
				Me.holder = Me.max
			End If

			If oneTime Then
				Me.timeTracker = New MultiBoolean(numEntries, False, False)
			End If
		End Sub

		''' <summary>
		''' Sets specified entry to specified state
		''' </summary>
		''' <param name="value"> </param>
		''' <param name="entry"> </param>
		Public Overridable Sub set(ByVal value As Boolean, ByVal entry As Integer)
			If entry > numEntries OrElse entry < 0 Then
				Throw New ND4JIllegalStateException("Entry index given (" & entry & ")in is higher then configured one (" & numEntries & ")")
			End If

			If oneTime AndAlso Me.timeTracker.get(entry) Then
				Return
			End If

			If value Then
				Me.holder = Me.holder Or 1 << (entry + 1)
			Else
				Me.holder = Me.holder And Not (1 << (entry + 1))
			End If

			If oneTime Then
				Me.timeTracker.set(True, entry)
			End If
		End Sub

		''' <summary>
		''' Gets current state for specified entry
		''' </summary>
		''' <param name="entry">
		''' @return </param>
		Public Overridable Function get(ByVal entry As Integer) As Boolean
			If entry > numEntries OrElse entry < 0 Then
				Throw New ND4JIllegalStateException("Entry index given (" & entry & ")in is higher then configured one (" & numEntries & ")")
			End If

			Return (Me.holder And 1 << (entry + 1)) <> 0
		End Function

		''' <summary>
		''' This method returns true if ALL states are true. False otherwise.
		''' 
		''' @return
		''' </summary>
		Public Overridable Function allTrue() As Boolean
			'log.info("Holder: {}; Max: {}", holder, max);
			Return holder = max
		End Function

		''' <summary>
		''' This method returns true if ALL states are false. False otherwise
		''' @return
		''' </summary>
		Public Overridable Function allFalse() As Boolean
			Return holder = 0
		End Function
	End Class

End Namespace