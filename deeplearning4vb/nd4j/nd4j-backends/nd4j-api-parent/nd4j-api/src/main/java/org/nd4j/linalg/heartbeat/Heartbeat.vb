Imports System.Threading
Imports Environment = org.nd4j.linalg.heartbeat.reports.Environment
Imports [Event] = org.nd4j.linalg.heartbeat.reports.Event
Imports Task = org.nd4j.linalg.heartbeat.reports.Task

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

Namespace org.nd4j.linalg.heartbeat



	Public Class Heartbeat
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New Heartbeat()
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile long serialVersionID;
		Private serialVersionID As Long
		Private enabled As New AtomicBoolean(True)


		Protected Friend Sub New()

		End Sub

		Public Shared ReadOnly Property Instance As Heartbeat
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overridable Sub disableHeartbeat()
			Me.enabled.set(False)
		End Sub

		Public Overridable Sub reportEvent(ByVal [event] As [Event], ByVal environment As Environment, ByVal task As Task)
			SyncLock Me
        
			End SyncLock
		End Sub

		Public Overridable Sub derivedId(ByVal id As Long)
			SyncLock Me
        
			End SyncLock
		End Sub

		Private ReadOnly Property DerivedId As Long
			Get
				SyncLock Me
					Return serialVersionID
				End SyncLock
			End Get
		End Property

		Private Class RepoThread
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As Heartbeat

			''' <summary>
			''' Thread for quiet background reporting.
			''' </summary>
			Friend ReadOnly environment As Environment
			Friend ReadOnly task As Task
			Friend ReadOnly [event] As [Event]


			Public Sub New(ByVal outerInstance As Heartbeat, ByVal [event] As [Event], ByVal environment As Environment, ByVal task As Task)
				Me.outerInstance = outerInstance
				Me.environment = environment
				Me.task = task
				Me.event = [event]
			End Sub

			Public Overrides Sub run()

			End Sub
		End Class

	End Class

End Namespace