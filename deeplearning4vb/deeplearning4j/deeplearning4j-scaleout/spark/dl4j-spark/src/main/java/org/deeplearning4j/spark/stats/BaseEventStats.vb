Imports System
Imports System.Threading
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider

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

Namespace org.deeplearning4j.spark.stats

	<Serializable>
	Public Class BaseEventStats
		Implements EventStats

'JAVA TO VB CONVERTER NOTE: The field machineId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly machineId_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field jvmId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly jvmId_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field threadId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly threadId_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field startTime was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly startTime_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field durationMs was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly durationMs_Conflict As Long

		Public Sub New(ByVal startTime As Long, ByVal durationMs As Long)
			Me.New(UIDProvider.HardwareUID, UIDProvider.JVMUID, Thread.CurrentThread.getId(), startTime, durationMs)
		End Sub

		Public Sub New(ByVal machineId As String, ByVal jvmId As String, ByVal threadId As Long, ByVal startTime As Long, ByVal durationMs As Long)
			Me.machineId_Conflict = machineId
			Me.jvmId_Conflict = jvmId
			Me.threadId_Conflict = threadId
			Me.startTime_Conflict = startTime
			Me.durationMs_Conflict = durationMs
		End Sub

		Public Overridable ReadOnly Property MachineID As String Implements EventStats.getMachineID
			Get
				Return machineId_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property JvmID As String Implements EventStats.getJvmID
			Get
				Return jvmId_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property ThreadID As Long Implements EventStats.getThreadID
			Get
				Return threadId_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property StartTime As Long Implements EventStats.getStartTime
			Get
				Return startTime_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property DurationMs As Long Implements EventStats.getDurationMs
			Get
				Return durationMs_Conflict
			End Get
		End Property

		Public Overridable Function asString(ByVal delimiter As String) As String Implements EventStats.asString
			Return machineId_Conflict + delimiter & jvmId_Conflict & delimiter & threadId_Conflict & delimiter & startTime_Conflict & delimiter & durationMs_Conflict
		End Function

		Public Overridable Function getStringHeader(ByVal delimiter As String) As String Implements EventStats.getStringHeader
			Return "machineId" & delimiter & "jvmId" & delimiter & "threadId" & delimiter & "startTime" & delimiter & "durationMs"
		End Function
	End Class

End Namespace