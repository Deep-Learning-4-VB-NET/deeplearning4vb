Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports StatsStorageListener = org.deeplearning4j.core.storage.StatsStorageListener
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports MapDBStatsStorage = org.deeplearning4j.ui.model.storage.mapdb.MapDBStatsStorage

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

Namespace org.deeplearning4j.ui.model.storage


	Public Class InMemoryStatsStorage
		Inherits BaseCollectionStatsStorage

		Private ReadOnly uid As String

		Public Sub New()
			MyBase.New()
			Dim str As String = System.Guid.randomUUID().ToString()
			uid = str.Substring(0, Math.Min(str.Length, 8))

			sessionIDs = Collections.synchronizedSet(New HashSet(Of String)())
			storageMetaData = New ConcurrentDictionary(Of SessionTypeId, StorageMetaData)()
			staticInfo = New ConcurrentDictionary(Of SessionTypeWorkerId, Persistable)()
		End Sub


		Protected Friend Overrides Function getUpdateMap(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal createIfRequired As Boolean) As IDictionary(Of Long, Persistable)
			SyncLock Me
				Dim id As New SessionTypeWorkerId(sessionID, typeID, workerID)
				If updates.ContainsKey(id) Then
					Return updates(id)
				End If
				If Not createIfRequired Then
					Return Nothing
				End If
				Dim updateMap As IDictionary(Of Long, Persistable) = New ConcurrentDictionary(Of Long, Persistable)()
				updates(id) = updateMap
				Return updateMap
			End SyncLock
		End Function

		Public Overrides Sub putStaticInfo(ByVal staticInfo As Persistable)
			Dim sses As IList(Of StatsStorageEvent) = checkStorageEvents(staticInfo)
			If Not sessionIDs.Contains(staticInfo.SessionID) Then
				sessionIDs.Add(staticInfo.SessionID)
			End If
			Dim id As New SessionTypeWorkerId(staticInfo.SessionID, staticInfo.TypeID, staticInfo.WorkerID)

			Me.staticInfo(id) = staticInfo
			Dim sse As StatsStorageEvent = Nothing
			If listeners.Count > 0 Then
				sse = New StatsStorageEvent(Me, StatsStorageListener.EventType.PostStaticInfo, staticInfo.SessionID, staticInfo.TypeID, staticInfo.WorkerID, staticInfo.TimeStamp)
			End If
			For Each l As StatsStorageListener In listeners
				l.notify(sse)
			Next l

			notifyListeners(sses)
		End Sub

		Public Overrides Sub putUpdate(ByVal update As Persistable)
			Dim sses As IList(Of StatsStorageEvent) = checkStorageEvents(update)
			Dim updateMap As IDictionary(Of Long, Persistable) = getUpdateMap(update.SessionID, update.TypeID, update.WorkerID, True)
			updateMap(update.TimeStamp) = update

			Dim sse As StatsStorageEvent = Nothing
			If listeners.Count > 0 Then
				sse = New StatsStorageEvent(Me, StatsStorageListener.EventType.PostUpdate, update.SessionID, update.TypeID, update.WorkerID, update.TimeStamp)
			End If
			For Each l As StatsStorageListener In listeners
				l.notify(sse)
			Next l

			notifyListeners(sses)
		End Sub

		Public Overrides Sub putStorageMetaData(ByVal storageMetaData As StorageMetaData)
			Dim sses As IList(Of StatsStorageEvent) = checkStorageEvents(storageMetaData)
			Dim id As New SessionTypeId(storageMetaData.SessionID, storageMetaData.TypeID)
			Me.storageMetaData(id) = storageMetaData

			Dim sse As StatsStorageEvent = Nothing
			If listeners.Count > 0 Then
				sse = New StatsStorageEvent(Me, StatsStorageListener.EventType.PostMetaData, storageMetaData.SessionID, storageMetaData.TypeID, storageMetaData.WorkerID, storageMetaData.TimeStamp)
			End If
			For Each l As StatsStorageListener In listeners
				l.notify(sse)
			Next l

			notifyListeners(sses)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws java.io.IOException
		Public Overrides Sub close()
			'No op
		End Sub

		Public Overrides ReadOnly Property Closed As Boolean
			Get
				Return False
			End Get
		End Property


		Public Overrides Function ToString() As String
			Return "InMemoryStatsStorage(uid=" & uid & ")"
		End Function
	End Class

End Namespace