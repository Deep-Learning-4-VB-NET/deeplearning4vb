Imports System.Collections.Generic
Imports org.nd4j.parameterserver.distributed.v2.messages

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

Namespace org.nd4j.parameterserver.distributed.v2.messages.history


	Public Class HashHistoryHolder(Of T)
		Implements MessagesHistoryHolder(Of T)

		Protected Friend ReadOnly set As ISet(Of T)
		''' 
		''' <param name="tailLength"> number of elements to hold in history </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HashHistoryHolder(final int tailLength)
		Public Sub New(ByVal tailLength As Integer)
			set = Collections.newSetFromMap(New LinkedHashMapAnonymousInnerClass(Me, tailLength))
		End Sub

		Private Class LinkedHashMapAnonymousInnerClass
			Inherits LinkedHashMap(Of T, Boolean)

			Private ReadOnly outerInstance As HashHistoryHolder(Of T)

			Private tailLength As Integer

			Public Sub New(ByVal outerInstance As HashHistoryHolder(Of T), ByVal tailLength As Integer)
				MyBase.New(tailLength)
				Me.outerInstance = outerInstance
				Me.tailLength = tailLength
			End Sub

			Protected Friend Overrides Function removeEldestEntry(ByVal eldest As KeyValuePair(Of T, Boolean)) As Boolean
				Return size() > tailLength
			End Function
		End Class

		Public Overridable Function storeIfUnknownMessageId(ByVal id As T) As Boolean Implements MessagesHistoryHolder(Of T).storeIfUnknownMessageId
			SyncLock Me
				If Not isKnownMessageId(id) Then
					set.Add(id)
					Return False
				Else
					Return True
				End If
			End SyncLock
		End Function

		Public Overridable Function isKnownMessageId(ByVal id As T) As Boolean Implements MessagesHistoryHolder(Of T).isKnownMessageId
			SyncLock Me
				Return set.Contains(id)
			End SyncLock
		End Function
	End Class

End Namespace