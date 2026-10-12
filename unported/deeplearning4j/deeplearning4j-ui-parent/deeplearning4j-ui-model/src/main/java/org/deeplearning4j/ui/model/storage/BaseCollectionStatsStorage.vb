Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports org.deeplearning4j.core.storage

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


	Public MustInherit Class BaseCollectionStatsStorage
		Implements StatsStorage

		Public MustOverride ReadOnly Property Closed As Boolean Implements StatsStorage.isClosed
		Public MustOverride Sub close() Implements StatsStorage.close

		Protected Friend sessionIDs As ISet(Of String)
		Protected Friend storageMetaData As IDictionary(Of SessionTypeId, StorageMetaData)
		Protected Friend staticInfo As IDictionary(Of SessionTypeWorkerId, Persistable)

		Protected Friend updates As IDictionary(Of SessionTypeWorkerId, IDictionary(Of Long, Persistable)) = New ConcurrentDictionary(Of SessionTypeWorkerId, IDictionary(Of Long, Persistable))()

		Protected Friend listeners As IList(Of StatsStorageListener) = New List(Of StatsStorageListener)()

		Protected Friend Sub New()

		End Sub

		Protected Friend MustOverride Function getUpdateMap(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal createIfRequired As Boolean) As IDictionary(Of Long, Persistable)

		'Return any relevant storage events
		'We want to return these so they can be logged later. Can't be logged immediately, as this may case a race
		'condition with whatever is receiving the events: i.e., might get the event before the contents are actually
		'available in the DB
		Protected Friend Overridable Function checkStorageEvents(ByVal p As Persistable) As IList(Of StatsStorageEvent)
			If listeners.Count = 0 Then
				Return Nothing
			End If

			Dim count As Integer = 0
			Dim newSID As StatsStorageEvent = Nothing
			Dim newTID As StatsStorageEvent = Nothing
			Dim newWID As StatsStorageEvent = Nothing

			'Is this a new session ID?
			If Not sessionIDs.Contains(p.SessionID) Then
				newSID = New StatsStorageEvent(Me, StatsStorageListener.EventType.NewSessionID, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
				count += 1
			End If

			'Check for new type and worker IDs
			'TODO probably more efficient way to do this
			Dim foundTypeId As Boolean = False
			Dim foundWorkerId As Boolean = False
			Dim typeId As String = p.TypeID
			Dim wid As String = p.WorkerID
			For Each ts As SessionTypeId In storageMetaData.Keys
				If typeId.Equals(ts.getTypeID()) Then
					foundTypeId = True
					Exit For
				End If
			Next ts
			For Each stw As SessionTypeWorkerId In staticInfo.Keys
				If Not foundTypeId AndAlso typeId.Equals(stw.getTypeID()) Then
					foundTypeId = True
				End If
				If Not foundWorkerId AndAlso wid.Equals(stw.getWorkerID()) Then
					foundWorkerId = True
				End If
				If foundTypeId AndAlso foundWorkerId Then
					Exit For
				End If
			Next stw
			If Not foundTypeId OrElse Not foundWorkerId Then
				For Each stw As SessionTypeWorkerId In updates.Keys
					If Not foundTypeId AndAlso typeId.Equals(stw.getTypeID()) Then
						foundTypeId = True
					End If
					If Not foundWorkerId AndAlso wid.Equals(stw.getWorkerID()) Then
						foundWorkerId = True
					End If
					If foundTypeId AndAlso foundWorkerId Then
						Exit For
					End If
				Next stw
			End If
			If Not foundTypeId Then
				'New type ID
				newTID = New StatsStorageEvent(Me, StatsStorageListener.EventType.NewTypeID, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
				count += 1
			End If
			If Not foundWorkerId Then
				'New worker ID
				newWID = New StatsStorageEvent(Me, StatsStorageListener.EventType.NewWorkerID, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
				count += 1
			End If
			If count = 0 Then
				Return Nothing
			End If
			Dim sses As IList(Of StatsStorageEvent) = New List(Of StatsStorageEvent)(count)
			If newSID IsNot Nothing Then
				sses.Add(newSID)
			End If
			If newTID IsNot Nothing Then
				sses.Add(newTID)
			End If
			If newWID IsNot Nothing Then
				sses.Add(newWID)
			End If
			Return sses
		End Function

		Protected Friend Overridable Sub notifyListeners(ByVal sses As IList(Of StatsStorageEvent))
			If sses Is Nothing OrElse sses.Count = 0 OrElse listeners.Count = 0 Then
				Return
			End If
			For Each l As StatsStorageListener In listeners
				For Each e As StatsStorageEvent In sses
					l.notify(e)
				Next e
			Next l
		End Sub

		Public Overridable Function listSessionIDs() As IList(Of String)
			Return New List(Of String)(sessionIDs)
		End Function

		Public Overridable Function sessionExists(ByVal sessionID As String) As Boolean Implements StatsStorage.sessionExists
			Return sessionIDs.Contains(sessionID)
		End Function

		Public Overridable Function getStaticInfo(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Persistable Implements StatsStorage.getStaticInfo
			Dim id As New SessionTypeWorkerId(sessionID, typeID, workerID)
			Return staticInfo(id)
		End Function

		Public Overridable Function getAllStaticInfos(ByVal sessionID As String, ByVal typeID As String) As IList(Of Persistable)
			Dim [out] As IList(Of Persistable) = New List(Of Persistable)()
			For Each key As SessionTypeWorkerId In staticInfo.Keys
				If sessionID.Equals(key.getSessionID()) AndAlso typeID.Equals(key.getTypeID()) Then
					[out].Add(staticInfo(key))
				End If
			Next key
			Return [out]
		End Function

		Public Overridable Function listTypeIDsForSession(ByVal sessionID As String) As IList(Of String)
			Dim typeIDs As ISet(Of String) = New HashSet(Of String)()
			For Each st As SessionTypeId In storageMetaData.Keys
				If Not sessionID.Equals(st.getSessionID()) Then
					Continue For
				End If
				typeIDs.Add(st.getTypeID())
			Next st

			For Each stw As SessionTypeWorkerId In staticInfo.Keys
				If Not sessionID.Equals(stw.getSessionID()) Then
					Continue For
				End If
				typeIDs.Add(stw.getTypeID())
			Next stw
			For Each stw As SessionTypeWorkerId In updates.Keys
				If Not sessionID.Equals(stw.getSessionID()) Then
					Continue For
				End If
				typeIDs.Add(stw.getTypeID())
			Next stw

			Return New List(Of String)(typeIDs)
		End Function

		Public Overridable Function listWorkerIDsForSession(ByVal sessionID As String) As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each ids As SessionTypeWorkerId In staticInfo.Keys
				If sessionID.Equals(ids.getSessionID()) Then
					[out].Add(ids.getWorkerID())
				End If
			Next ids
			Return [out]
		End Function

		Public Overridable Function listWorkerIDsForSessionAndType(ByVal sessionID As String, ByVal typeID As String) As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each ids As SessionTypeWorkerId In staticInfo.Keys
				If sessionID.Equals(ids.getSessionID()) AndAlso typeID.Equals(ids.getTypeID()) Then
					[out].Add(ids.getWorkerID())
				End If
			Next ids
			Return [out]
		End Function

		Public Overridable Function getNumUpdateRecordsFor(ByVal sessionID As String) As Integer Implements StatsStorage.getNumUpdateRecordsFor
			Dim count As Integer = 0
			For Each id As SessionTypeWorkerId In updates.Keys
				If sessionID.Equals(id.getSessionID()) Then
					Dim map As IDictionary(Of Long, Persistable) = updates(id)
					If map IsNot Nothing Then
						count += map.Count
					End If
				End If
			Next id
			Return count
		End Function

		Public Overridable Function getNumUpdateRecordsFor(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Integer Implements StatsStorage.getNumUpdateRecordsFor
			Dim id As New SessionTypeWorkerId(sessionID, typeID, workerID)
			Dim map As IDictionary(Of Long, Persistable) = updates(id)
			If map IsNot Nothing Then
				Return map.Count
			End If
			Return 0
		End Function

		Public Overridable Function getLatestUpdate(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Persistable Implements StatsStorage.getLatestUpdate
			Dim id As New SessionTypeWorkerId(sessionID, typeID, workerID)
			Dim map As IDictionary(Of Long, Persistable) = updates(id)
			If map Is Nothing OrElse map.Count = 0 Then
				Return Nothing
			End If
			Dim maxTime As Long = Long.MinValue
			For Each l As Long? In map.Keys
				maxTime = Math.Max(maxTime, l)
			Next l
			Return map(maxTime)
		End Function

		Public Overridable Function getUpdate(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamp As Long) As Persistable Implements StatsStorage.getUpdate
			Dim id As New SessionTypeWorkerId(sessionID, typeID, workerID)
			Dim map As IDictionary(Of Long, Persistable) = updates(id)
			If map Is Nothing Then
				Return Nothing
			End If

			Return map(timestamp)
		End Function

		Public Overridable Function getLatestUpdateAllWorkers(ByVal sessionID As String, ByVal typeID As String) As IList(Of Persistable)
			Dim list As IList(Of Persistable) = New List(Of Persistable)()

			For Each id As SessionTypeWorkerId In updates.Keys
				If sessionID.Equals(id.getSessionID()) AndAlso typeID.Equals(id.getTypeID()) Then
					Dim p As Persistable = getLatestUpdate(sessionID, typeID, id.workerID)
					If p IsNot Nothing Then
						list.Add(p)
					End If
				End If
			Next id

			Return list
		End Function

		Public Overridable Function getAllUpdatesAfter(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamp As Long) As IList(Of Persistable)
			Dim list As IList(Of Persistable) = New List(Of Persistable)()

			Dim map As IDictionary(Of Long, Persistable) = getUpdateMap(sessionID, typeID, workerID, False)
			If map Is Nothing Then
				Return list
			End If

			For Each time As Long? In map.Keys
				If time > timestamp Then
					list.Add(map(time))
				End If
			Next time

			list.Sort(New ComparatorAnonymousInnerClass(Me))

			Return list
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Persistable)

			Private ReadOnly outerInstance As BaseCollectionStatsStorage

			Public Sub New(ByVal outerInstance As BaseCollectionStatsStorage)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Persistable, ByVal o2 As Persistable) As Integer Implements IComparer(Of Persistable).Compare
				Return Long.compare(o1.TimeStamp, o2.TimeStamp)
			End Function
		End Class

		Public Overridable Function getAllUpdatesAfter(ByVal sessionID As String, ByVal typeID As String, ByVal timestamp As Long) As IList(Of Persistable)
			Dim list As IList(Of Persistable) = New List(Of Persistable)()

			For Each stw As SessionTypeWorkerId In staticInfo.Keys
				If stw.getSessionID().Equals(sessionID) AndAlso stw.getTypeID().Equals(typeID) Then
					Dim u As IDictionary(Of Long, Persistable) = updates(stw)
					If u Is Nothing Then
						Continue For
					End If
					For Each l As Long In u.Keys
						If l > timestamp Then
							list.Add(u(l))
						End If
					Next l
				End If
			Next stw

			'Sort by time stamp
			list.Sort(New ComparatorAnonymousInnerClass2(Me))

			Return list
		End Function

		Private Class ComparatorAnonymousInnerClass2
			Implements IComparer(Of Persistable)

			Private ReadOnly outerInstance As BaseCollectionStatsStorage

			Public Sub New(ByVal outerInstance As BaseCollectionStatsStorage)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As Persistable, ByVal o2 As Persistable) As Integer Implements IComparer(Of Persistable).Compare
				Return Long.compare(o1.TimeStamp, o2.TimeStamp)
			End Function
		End Class

		Public Overridable Function getStorageMetaData(ByVal sessionID As String, ByVal typeID As String) As StorageMetaData Implements StatsStorage.getStorageMetaData
			Return Me.storageMetaData(New SessionTypeId(sessionID, typeID))
		End Function

		Public Overridable Function getAllUpdateTimes(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Long() Implements StatsStorage.getAllUpdateTimes
			Dim stw As New SessionTypeWorkerId(sessionID, typeID, workerID)
			Dim m As IDictionary(Of Long, Persistable) = updates(stw)
			If m Is Nothing Then
				Return New Long(){}
			End If

			Dim ret(m.Count - 1) As Long
			Dim i As Integer=0
			For Each l As Long? In m.Keys
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[i++] = l;
				ret(i) = l
					i += 1
				If i >= ret.Length Then
					Exit For 'Map "m" can in principle be modified concurrently while iterating here - resulting in an array index exception
				End If
			Next l
			Array.Sort(ret)
			Return ret
		End Function

		Public Overridable Function getUpdates(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamps() As Long) As IList(Of Persistable)
			Dim stw As New SessionTypeWorkerId(sessionID, typeID, workerID)
			Dim m As IDictionary(Of Long, Persistable) = updates(stw)
			If m Is Nothing Then
				Return java.util.Collections.emptyList()
			End If

			Dim ret As IList(Of Persistable) = New List(Of Persistable)(timestamps.Length)
			For Each l As Long In timestamps
				Dim p As Persistable = m(l)
				If p IsNot Nothing Then
					ret.Add(p)
				End If
			Next l
			Return ret
		End Function

		' ----- Store new info -----

		Public MustOverride Overrides Sub putStaticInfo(ByVal staticInfo As Persistable) Implements StatsStorage.putStaticInfo

		Public Overridable Sub putStaticInfo(Of T1 As Persistable)(ByVal staticInfo As ICollection(Of T1))
			For Each p As Persistable In staticInfo
				putStaticInfo(p)
			Next p
		End Sub

		Public MustOverride Overrides Sub putUpdate(ByVal update As Persistable) Implements StatsStorage.putUpdate

		Public Overridable Sub putUpdate(Of T1 As Persistable)(ByVal updates As ICollection(Of T1))
			For Each p As Persistable In updates
				putUpdate(p)
			Next p
		End Sub

		Public MustOverride Overrides Sub putStorageMetaData(ByVal storageMetaData As StorageMetaData) Implements StatsStorage.putStorageMetaData

		Public Overridable Sub putStorageMetaData(Of T1 As StorageMetaData)(ByVal storageMetaData As ICollection(Of T1))
			For Each m As StorageMetaData In storageMetaData
				putStorageMetaData(m)
			Next m
		End Sub


		' ----- Listeners -----

		Public Overridable Sub registerStatsStorageListener(ByVal listener As StatsStorageListener) Implements StatsStorage.registerStatsStorageListener
			If Not Me.listeners.Contains(listener) Then
				Me.listeners.Add(listener)
			End If
		End Sub

		Public Overridable Sub deregisterStatsStorageListener(ByVal listener As StatsStorageListener) Implements StatsStorage.deregisterStatsStorageListener
			Me.listeners.Remove(listener)
		End Sub

		Public Overridable Sub removeAllListeners() Implements StatsStorage.removeAllListeners
			Me.listeners.Clear()
		End Sub

		Public Overridable ReadOnly Property Listeners As IList(Of StatsStorageListener)
			Get
				Return New List(Of StatsStorageListener)(listeners)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class SessionTypeWorkerId implements java.io.Serializable, Comparable<SessionTypeWorkerId>
		<Serializable>
		Public Class SessionTypeWorkerId
			Implements IComparable(Of SessionTypeWorkerId)

			Friend ReadOnly sessionID As String
			Friend ReadOnly typeID As String
			Friend ReadOnly workerID As String

			Public Sub New(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String)
				Me.sessionID = sessionID
				Me.typeID = typeID
				Me.workerID = workerID
			End Sub

			Public Overridable Function CompareTo(ByVal o As SessionTypeWorkerId) As Integer Implements IComparable(Of SessionTypeWorkerId).CompareTo
				Dim c As Integer = String.CompareOrdinal(sessionID, o.sessionID)
				If c <> 0 Then
					Return c
				End If
				c = String.CompareOrdinal(typeID, o.typeID)
				If c <> 0 Then
					Return c
				End If
				Return String.CompareOrdinal(workerID, workerID)
			End Function

			Public Overrides Function ToString() As String
				Return "(" & sessionID & "," & typeID & "," & workerID & ")"
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class SessionTypeId implements java.io.Serializable, Comparable<SessionTypeId>
		<Serializable>
		Public Class SessionTypeId
			Implements IComparable(Of SessionTypeId)

			Friend ReadOnly sessionID As String
			Friend ReadOnly typeID As String

			Public Overridable Function CompareTo(ByVal o As SessionTypeId) As Integer Implements IComparable(Of SessionTypeId).CompareTo
				Dim c As Integer = String.CompareOrdinal(sessionID, o.sessionID)
				If c <> 0 Then
					Return c
				End If
				Return String.CompareOrdinal(typeID, o.typeID)
			End Function

			Public Overrides Function ToString() As String
				Return "(" & sessionID & "," & typeID & ")"
			End Function
		End Class
	End Class

End Namespace