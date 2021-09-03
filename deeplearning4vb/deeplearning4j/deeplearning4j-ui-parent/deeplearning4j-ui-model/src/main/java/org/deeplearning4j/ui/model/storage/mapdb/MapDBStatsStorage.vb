Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports org.deeplearning4j.core.storage
Imports FileStatsStorage = org.deeplearning4j.ui.model.storage.FileStatsStorage
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports BaseCollectionStatsStorage = org.deeplearning4j.ui.model.storage.BaseCollectionStatsStorage
Imports org.mapdb

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

Namespace org.deeplearning4j.ui.model.storage.mapdb


	Public Class MapDBStatsStorage
		Inherits BaseCollectionStatsStorage

		Private Const COMPOSITE_KEY_HEADER As String = "&&&"
		Private Const COMPOSITE_KEY_SEPARATOR As String = "@@@"

		Private isClosed As Boolean = False
		Private db As DB
		Private updateMapLock As Lock = New ReentrantLock(True)

		Private classToInteger As IDictionary(Of String, Integer) 'For storage
		Private integerToClass As IDictionary(Of Integer, String) 'For storage
		Private classCounter As Atomic.Integer

		Public Sub New()
			Me.New(New Builder())
		End Sub

		Public Sub New(ByVal f As File)
			Me.New((New Builder()).file(f))
		End Sub

		Private Sub New(ByVal builder As Builder)
			Dim f As File = builder.getFile()

			If f Is Nothing Then
				'In-Memory Stats Storage
				db = DBMaker.memoryDB().make()
			Else
				db = DBMaker.fileDB(f).closeOnJvmShutdown().transactionEnable().make()
			End If

			'Initialize/open the required maps/lists
			sessionIDs = db.hashSet("sessionIDs", Serializer.STRING).createOrOpen()
			storageMetaData = db.hashMap("storageMetaData").keySerializer(New SessionTypeIdSerializer()).valueSerializer(New PersistableSerializer(Me, Of StorageMetaData)()).createOrOpen()
			staticInfo = db.hashMap("staticInfo").keySerializer(New SessionTypeWorkerIdSerializer()).valueSerializer(New PersistableSerializer(Me, Of )()).createOrOpen()

			classToInteger = db.hashMap("classToInteger").keySerializer(Serializer.STRING).valueSerializer(Serializer.INTEGER).createOrOpen()

			integerToClass = db.hashMap("integerToClass").keySerializer(Serializer.INTEGER).valueSerializer(Serializer.STRING).createOrOpen()

			classCounter = db.atomicInteger("classCounter").createOrOpen()

			'Load up any saved update maps to the update map...
			For Each s As String In db.getAllNames()
				If s.StartsWith(COMPOSITE_KEY_HEADER, StringComparison.Ordinal) Then
					Dim m As IDictionary(Of Long, Persistable) = db.hashMap(s).keySerializer(Serializer.LONG).valueSerializer(New PersistableSerializer(Me, Of Long, Persistable)()).open()
					Dim arr() As String = s.Split(COMPOSITE_KEY_SEPARATOR, True)
					arr(0) = arr(0).Substring(COMPOSITE_KEY_HEADER.Length) 'Remove header...
					Dim id As New SessionTypeWorkerId(arr(0), arr(1), arr(2))
					updates(id) = m
				End If
			Next s
		End Sub

		Protected Friend Overrides Function getUpdateMap(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal createIfRequired As Boolean) As IDictionary(Of Long, Persistable)
			Dim id As New SessionTypeWorkerId(sessionID, typeID, workerID)
			If updates.ContainsKey(id) Then
				Return updates(id)
			End If
			If Not createIfRequired Then
				Return Nothing
			End If
			Dim compositeKey As String = COMPOSITE_KEY_HEADER & sessionID & COMPOSITE_KEY_SEPARATOR & typeID & COMPOSITE_KEY_SEPARATOR & workerID

			Dim updateMap As IDictionary(Of Long, Persistable)
			updateMapLock.lock()
			Try
				'Try again, in case another thread created it before lock was acquired in this thread
				If updates.ContainsKey(id) Then
					Return updates(id)
				End If
				updateMap = db.hashMap(compositeKey).keySerializer(Serializer.LONG).valueSerializer(New PersistableSerializer(Me, Of )()).createOrOpen()
				updates(id) = updateMap
			Finally
				updateMapLock.unlock()
			End Try

			Return updateMap
		End Function



		Public Overrides Sub close()
			db.commit() 'For write ahead log: need to ensure that we persist all data to disk...
			db.close()
			isClosed = True
		End Sub

		Public Overrides ReadOnly Property Closed As Boolean
			Get
				Return isClosed
			End Get
		End Property

		' ----- Store new info -----

		Public Overrides Sub putStaticInfo(ByVal staticInfo As Persistable)
			Dim sses As IList(Of StatsStorageEvent) = checkStorageEvents(staticInfo)
			If Not sessionIDs.Contains(staticInfo.SessionID) Then
				sessionIDs.Add(staticInfo.SessionID)
			End If
			Dim id As New SessionTypeWorkerId(staticInfo.SessionID, staticInfo.TypeID, staticInfo.WorkerID)

			Me.staticInfo(id) = staticInfo
			db.commit() 'For write ahead log: need to ensure that we persist all data to disk...
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
			db.commit() 'For write ahead log: need to ensure that we persist all data to disk...

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
			db.commit() 'For write ahead log: need to ensure that we persist all data to disk...

			Dim sse As StatsStorageEvent = Nothing
			If listeners.Count > 0 Then
				sse = New StatsStorageEvent(Me, StatsStorageListener.EventType.PostMetaData, storageMetaData.SessionID, storageMetaData.TypeID, storageMetaData.WorkerID, storageMetaData.TimeStamp)
			End If
			For Each l As StatsStorageListener In listeners
				l.notify(sse)
			Next l

			notifyListeners(sses)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class Builder
		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field file was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend file_Conflict As File
'JAVA TO VB CONVERTER NOTE: The field useWriteAheadLog was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend useWriteAheadLog_Conflict As Boolean = True

			Public Sub New()
				Me.New(Nothing)
			End Sub

			Public Sub New(ByVal file As File)
				Me.file_Conflict = file
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter file was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function file(ByVal file_Conflict As File) As Builder
				Me.file_Conflict = file_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter useWriteAheadLog was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function useWriteAheadLog(ByVal useWriteAheadLog_Conflict As Boolean) As Builder
				Me.useWriteAheadLog_Conflict = useWriteAheadLog_Conflict
				Return Me
			End Function

			Public Overridable Function build() As MapDBStatsStorage
				Return New MapDBStatsStorage(Me)
			End Function

		End Class


		Private Function getIntForClass(ByVal c As Type) As Integer
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Dim str As String = c.FullName
			If classToInteger.ContainsKey(str) Then
				Return classToInteger(str)
			End If
			Dim idx As Integer = classCounter.getAndIncrement()
			classToInteger(str) = idx
			integerToClass(idx) = str
			db.commit()
			Return idx
		End Function

		Private Function getClassForInt(ByVal [integer] As Integer) As String
			Dim c As String = integerToClass([integer])
			If c Is Nothing Then
				Throw New Exception("Unknown class index: " & [integer]) 'Should never happen
			End If
			Return c
		End Function

		'Simple serializer, based on MapDB's SerializerJava
		Private Class SessionTypeWorkerIdSerializer
			Implements Serializer(Of SessionTypeWorkerId)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void serialize(@NonNull DataOutput2 out, @NonNull SessionTypeWorkerId value) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
			Public Overrides Sub serialize(ByVal [out] As DataOutput2, ByVal value As SessionTypeWorkerId)
				Dim out2 As New ObjectOutputStream([out])
				out2.writeObject(value)
				out2.flush()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public SessionTypeWorkerId deserialize(@NonNull DataInput2 in, int available) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
			Public Overrides Function deserialize(ByVal [in] As DataInput2, ByVal available As Integer) As SessionTypeWorkerId
				Try
					Dim in2 As New ObjectInputStream(New DataInput2.DataInputToStream([in]))
					Return CType(in2.readObject(), SessionTypeWorkerId)
				Catch e As ClassNotFoundException
					Throw New IOException(e)
				End Try
			End Function

			Public Overrides Function compare(ByVal w1 As SessionTypeWorkerId, ByVal w2 As SessionTypeWorkerId) As Integer
				Return w1.CompareTo(w2)
			End Function
		End Class

		'Simple serializer, based on MapDB's SerializerJava
		Private Class SessionTypeIdSerializer
			Implements Serializer(Of SessionTypeId)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void serialize(@NonNull DataOutput2 out, @NonNull SessionTypeId value) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
			Public Overrides Sub serialize(ByVal [out] As DataOutput2, ByVal value As SessionTypeId)
				Dim out2 As New ObjectOutputStream([out])
				out2.writeObject(value)
				out2.flush()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public SessionTypeId deserialize(@NonNull DataInput2 in, int available) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
			Public Overrides Function deserialize(ByVal [in] As DataInput2, ByVal available As Integer) As SessionTypeId
				Try
					Dim in2 As New ObjectInputStream(New DataInput2.DataInputToStream([in]))
					Return CType(in2.readObject(), SessionTypeId)
				Catch e As ClassNotFoundException
					Throw New IOException(e)
				End Try
			End Function

			Public Overrides Function compare(ByVal w1 As SessionTypeId, ByVal w2 As SessionTypeId) As Integer
				Return w1.CompareTo(w2)
			End Function
		End Class

		Private Class PersistableSerializer(Of T As Persistable)
			Implements Serializer(Of T)

			Private ReadOnly outerInstance As MapDBStatsStorage

			Public Sub New(ByVal outerInstance As MapDBStatsStorage)
				Me.outerInstance = outerInstance
			End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void serialize(@NonNull DataOutput2 out, @NonNull Persistable value) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
			Public Overrides Sub serialize(ByVal [out] As DataOutput2, ByVal value As Persistable)
				'Persistable values can't be decoded in isolation, i.e., without knowing the type
				'So, we'll first write an integer representing the class name, so we can decode it later...
				Dim classIdx As Integer = outerInstance.getIntForClass(value.GetType())
				[out].writeInt(classIdx)
				value.encode([out])
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public T deserialize(@NonNull DataInput2 input, int available) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
			Public Overrides Function deserialize(ByVal input As DataInput2, ByVal available As Integer) As T
				Dim classIdx As Integer = input.readInt()
				Dim className As String = outerInstance.getClassForInt(classIdx)

				Dim persistable As Persistable = DL4JClassLoading.createNewInstance(className)

				Dim remainingLength As Integer = available - 4 ' -4 for int class index
				Dim temp(remainingLength - 1) As SByte
				input.readFully(temp)
				persistable.decode(temp)
				Return DirectCast(persistable, T)
			End Function

			Public Overrides Function compare(ByVal p1 As Persistable, ByVal p2 As Persistable) As Integer
				Dim c As Integer = String.CompareOrdinal(p1.SessionID, p2.SessionID)
				If c <> 0 Then
					Return c
				End If
				c = String.CompareOrdinal(p1.TypeID, p2.TypeID)
				If c <> 0 Then
					Return c
				End If
				Return String.CompareOrdinal(p1.WorkerID, p2.WorkerID)
			End Function
		End Class

	End Class

End Namespace