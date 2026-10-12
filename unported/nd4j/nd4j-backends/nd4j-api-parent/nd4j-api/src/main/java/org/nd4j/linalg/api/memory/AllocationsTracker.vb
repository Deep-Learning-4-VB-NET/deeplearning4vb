Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports var = lombok.var
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind

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

Namespace org.nd4j.linalg.api.memory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class AllocationsTracker
	Public Class AllocationsTracker
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New AllocationsTracker()
		Private devices As IDictionary(Of Integer, DeviceAllocationsTracker) = New ConcurrentDictionary(Of Integer, DeviceAllocationsTracker)()

		Protected Friend Sub New()

		End Sub

		Public Shared ReadOnly Property Instance As AllocationsTracker
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Protected Friend Overridable Function trackerForDevice(ByVal deviceId As Integer?) As DeviceAllocationsTracker
			Dim tracker As var = devices(deviceId)
			If tracker Is Nothing Then
				SyncLock Me
					tracker = devices(deviceId)
					If tracker Is Nothing Then
						tracker = New DeviceAllocationsTracker()
						devices(deviceId) = tracker
					End If
				End SyncLock
			End If

			Return tracker
		End Function

		Public Overridable Sub markAllocated(ByVal kind As AllocationKind, ByVal deviceId As Integer?, ByVal bytes As Long)
			Dim tracker As val = trackerForDevice(deviceId)

			tracker.updateState(kind, bytes)
		End Sub

		Public Overridable Sub markReleased(ByVal kind As AllocationKind, ByVal deviceId As Integer?, ByVal bytes As Long)
			Dim tracker As val = trackerForDevice(deviceId)

			tracker.updateState(kind, -bytes)
		End Sub

		Public Overridable Function bytesOnDevice(ByVal deviceId As Integer?) As Long
			Return bytesOnDevice(AllocationKind.GENERAL, deviceId)
		End Function

		Public Overridable Function bytesOnDevice(ByVal kind As AllocationKind, ByVal deviceId As Integer?) As Long
			Dim tracker As val = trackerForDevice(deviceId)
			Return tracker.getState(kind)
		End Function
	End Class

End Namespace