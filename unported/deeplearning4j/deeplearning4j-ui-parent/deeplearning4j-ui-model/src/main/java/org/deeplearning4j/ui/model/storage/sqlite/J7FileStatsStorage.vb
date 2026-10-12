Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports LongArrayList = it.unimi.dsi.fastutil.longs.LongArrayList
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.core.storage
Imports FileStatsStorage = org.deeplearning4j.ui.model.storage.FileStatsStorage
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.ui.model.storage.sqlite


	Public Class J7FileStatsStorage
		Implements StatsStorage

		Private Const TABLE_NAME_METADATA As String = "StorageMetaData"
		Private Const TABLE_NAME_STATIC_INFO As String = "StaticInfo"
		Private Const TABLE_NAME_UPDATES As String = "Updates"

		Private Shared ReadOnly INSERT_META_SQL As String = "INSERT OR REPLACE INTO " & TABLE_NAME_METADATA & " (SessionID, TypeID, ObjectClass, ObjectBytes) VALUES ( ?, ?, ?, ? );"
		Private Shared ReadOnly INSERT_STATIC_SQL As String = "INSERT OR REPLACE INTO " & TABLE_NAME_STATIC_INFO & " (SessionID, TypeID, WorkerID, ObjectClass, ObjectBytes) VALUES ( ?, ?, ?, ?, ? );"
		Private Shared ReadOnly INSERT_UPDATE_SQL As String = "INSERT OR REPLACE INTO " & TABLE_NAME_UPDATES & " (SessionID, TypeID, WorkerID, Timestamp, ObjectClass, ObjectBytes) VALUES ( ?, ?, ?, ?, ?, ? );"

		Private ReadOnly file As File
		Private ReadOnly connection As Connection
		Private listeners As IList(Of StatsStorageListener) = New List(Of StatsStorageListener)()

		''' <param name="file"> Storage location for the stats </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public J7FileStatsStorage(@NonNull File file)
		Public Sub New(ByVal file As File)
			Me.file = file

			Try
				connection = DriverManager.getConnection("jdbc:sqlite:" & file.getAbsolutePath())
			Catch e As Exception
				Throw New Exception("Error ninializing J7FileStatsStorage instance", e)
			End Try

			Try
				initializeTables()
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void initializeTables() throws SQLException
		Private Sub initializeTables()

			'Need tables for:
			'(a) Metadata  -> session ID and type ID; class; StorageMetaData as a binary BLOB
			'(b) Static info -> session ID, type ID, worker ID, persistable class, persistable bytes
			'(c) Update info -> session ID, type ID, worker ID, timestamp, update class, update bytes

			'First: check if tables exist
			Dim meta As DatabaseMetaData = connection.getMetaData()
			Dim rs As ResultSet = meta.getTables(Nothing, Nothing, "%", Nothing)
			Dim hasStorageMetaDataTable As Boolean = False
			Dim hasStaticInfoTable As Boolean = False
			Dim hasUpdatesTable As Boolean = False
			Do While rs.next()
				'3rd value: table name - http://docs.oracle.com/javase/6/docs/api/java/sql/DatabaseMetaData.html#getTables%28java.lang.String,%20java.lang.String,%20java.lang.String,%20java.lang.String[]%29
				Dim name As String = rs.getString(3)
				If TABLE_NAME_METADATA.Equals(name) Then
					hasStorageMetaDataTable = True
				ElseIf TABLE_NAME_STATIC_INFO.Equals(name) Then
					hasStaticInfoTable = True
				ElseIf TABLE_NAME_UPDATES.Equals(name) Then
					hasUpdatesTable = True
				End If
			Loop


			Dim statement As Statement = connection.createStatement()

			If Not hasStorageMetaDataTable Then
				statement.executeUpdate("CREATE TABLE " & TABLE_NAME_METADATA & " (" & "SessionID TEXT NOT NULL, " & "TypeID TEXT NOT NULL, " & "ObjectClass TEXT NOT NULL, " & "ObjectBytes BLOB NOT NULL, " & "PRIMARY KEY ( SessionID, TypeID )" & ");")
			End If

			If Not hasStaticInfoTable Then
				statement.executeUpdate("CREATE TABLE " & TABLE_NAME_STATIC_INFO & " (" & "SessionID TEXT NOT NULL, " & "TypeID TEXT NOT NULL, " & "WorkerID TEXT NOT NULL, " & "ObjectClass TEXT NOT NULL, " & "ObjectBytes BLOB NOT NULL, " & "PRIMARY KEY ( SessionID, TypeID, WorkerID )" & ");")
			End If

			If Not hasUpdatesTable Then
				statement.executeUpdate("CREATE TABLE " & TABLE_NAME_UPDATES & " (" & "SessionID TEXT NOT NULL, " & "TypeID TEXT NOT NULL, " & "WorkerID TEXT NOT NULL, " & "Timestamp INTEGER NOT NULL, " & "ObjectClass TEXT NOT NULL, " & "ObjectBytes BLOB NOT NULL, " & "PRIMARY KEY ( SessionID, TypeID, WorkerID, Timestamp )" & ");")
			End If

			statement.close()

		End Sub

		Private Shared Function serializeForDB(ByVal [object] As Object) As Pair(Of String, SByte())
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Dim classStr As String = [object].GetType().FullName
			Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using baos As System.IO.MemoryStream_Output = new System.IO.MemoryStream_Output(), oos As ObjectOutputStream = new ObjectOutputStream(baos)
					New MemoryStream(), oos As New ObjectOutputStream(baos)
						Using baos As New MemoryStream(), oos As ObjectOutputStream
					oos.writeObject([object])
					oos.close()
					Dim bytes() As SByte = baos.toByteArray()
					Return New Pair(Of String, SByte())(classStr, bytes)
					End Using
			Catch e As IOException
				Throw New Exception("Error serializing object for storage", e)
			End Try
		End Function

		Private Shared Function deserialize(Of T)(ByVal bytes() As SByte) As T
			Try
					Using ois As New ObjectInputStream(New MemoryStream(bytes))
					Return CType(ois.readObject(), T)
					End Using
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
				Throw New Exception(e)
			End Try
		End Function

		Private Function queryAndGet(Of T)(ByVal sql As String, ByVal columnIndex As Integer) As T
			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(sql)
					If Not rs.next() Then
						Return Nothing
					End If
					Dim bytes() As SByte = rs.getBytes(columnIndex)
					Return deserialize(bytes)
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Private Function selectDistinct(ByVal columnName As String, ByVal queryMeta As Boolean, ByVal queryStatic As Boolean, ByVal queryUpdates As Boolean, ByVal conditionColumn As String, ByVal conditionValue As String) As IList(Of String)
			Dim unique As ISet(Of String) = New HashSet(Of String)()

			Try
					Using statement As Statement = connection.createStatement()
					If queryMeta Then
						queryHelper(statement, querySqlHelper(columnName, TABLE_NAME_METADATA, conditionColumn, conditionValue), unique)
					End If
        
					If queryStatic Then
						queryHelper(statement, querySqlHelper(columnName, TABLE_NAME_STATIC_INFO, conditionColumn, conditionValue), unique)
					End If
        
					If queryUpdates Then
						queryHelper(statement, querySqlHelper(columnName, TABLE_NAME_UPDATES, conditionColumn, conditionValue), unique)
					End If
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try

			Return New List(Of String)(unique)
		End Function

		Private Function querySqlHelper(ByVal columnName As String, ByVal table As String, ByVal conditionColumn As String, ByVal conditionValue As String) As String
			Dim unique As String = "SELECT DISTINCT " & columnName & " FROM " & table
			If conditionColumn IsNot Nothing Then
				unique &= " WHERE " & conditionColumn & " = '" & conditionValue & "'"
			End If
			unique &= ";"
			Return unique
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void queryHelper(Statement statement, String q, @Set<String> unique) throws SQLException
		Private Sub queryHelper(ByVal statement As Statement, ByVal q As String, ByVal unique As ISet(Of String))
			Dim rs As ResultSet = statement.executeQuery(q)
			Do While rs.next()
				Dim str As String = rs.getString(1)
				unique.Add(str)
			Loop
		End Sub

		Protected Friend Overridable Function checkStorageEvents(ByVal p As Persistable) As IList(Of StatsStorageEvent)
			If listeners.Count = 0 Then
				Return Nothing
			End If

			Dim newSID As StatsStorageEvent = Nothing
			Dim newTID As StatsStorageEvent = Nothing
			Dim newWID As StatsStorageEvent = Nothing

			Dim sid As String = p.SessionID
			Dim tid As String = p.TypeID
			Dim wid As String = p.WorkerID

			'Is this a new session ID? type ID? worker ID?

			'This is not the most efficient approach
			Dim isNewSID As Boolean = False
			Dim isNewTID As Boolean = False
			Dim isNewWID As Boolean = False
			If Not listSessionIDs().Contains(sid) Then
				isNewSID = True
				isNewTID = True
				isNewWID = True
			End If

			If Not isNewTID AndAlso Not listTypeIDsForSession(sid).Contains(tid) Then
				isNewTID = True
			End If

			If Not isNewWID AndAlso Not listWorkerIDsForSessionAndType(sid, tid).Contains(wid) Then
				isNewWID = True
			End If

			If isNewSID Then
				newSID = New StatsStorageEvent(Me, StatsStorageListener.EventType.NewSessionID, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
			End If
			If isNewTID Then
				newTID = New StatsStorageEvent(Me, StatsStorageListener.EventType.NewTypeID, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
			End If
			If isNewWID Then
				newWID = New StatsStorageEvent(Me, StatsStorageListener.EventType.NewWorkerID, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
			End If

			If Not isNewSID AndAlso Not isNewTID AndAlso Not isNewWID Then
				Return Nothing
			End If
			Dim sses As IList(Of StatsStorageEvent) = New List(Of StatsStorageEvent)(3)
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

		Public Overridable Sub putStorageMetaData(ByVal storageMetaData As StorageMetaData) Implements StatsStorage.putStorageMetaData
			putStorageMetaData(Collections.singletonList(storageMetaData))
		End Sub

		Public Overridable Sub putStorageMetaData(Of T1 As StorageMetaData)(ByVal collection As ICollection(Of T1))
			Dim sses As IList(Of StatsStorageEvent) = Nothing
			Try
				Dim ps As PreparedStatement = connection.prepareStatement(INSERT_META_SQL)

				For Each storageMetaData As StorageMetaData In collection
					Dim ssesTemp As IList(Of StatsStorageEvent) = checkStorageEvents(storageMetaData)
					If ssesTemp IsNot Nothing Then
						If sses Is Nothing Then
							sses = ssesTemp
						Else
							CType(sses, List(Of StatsStorageEvent)).AddRange(ssesTemp)
						End If
					End If

					If listeners.Count > 0 Then
						Dim sse As New StatsStorageEvent(Me, StatsStorageListener.EventType.PostMetaData, storageMetaData.SessionID, storageMetaData.TypeID, storageMetaData.WorkerID, storageMetaData.TimeStamp)
						If sses Is Nothing Then
							sses = New List(Of StatsStorageEvent)()
						End If
						sses.Add(sse)
					End If


					'Normally we'd batch these... sqlite has an autocommit feature that messes up batching with .addBatch() and .executeUpdate()
					Dim p As Pair(Of String, SByte()) = serializeForDB(storageMetaData)

					ps.setString(1, storageMetaData.SessionID)
					ps.setString(2, storageMetaData.TypeID)
					ps.setString(3, p.First)
					ps.setObject(4, p.Second)
					ps.executeUpdate()
				Next storageMetaData
			Catch e As SQLException
				Throw New Exception(e)
			End Try

			notifyListeners(sses)
		End Sub

		Public Overridable Sub putStaticInfo(ByVal staticInfo As Persistable) Implements StatsStorage.putStaticInfo
			putStaticInfo(Collections.singletonList(staticInfo))
		End Sub

		Public Overridable Sub putStaticInfo(Of T1 As Persistable)(ByVal collection As ICollection(Of T1))
			Dim sses As IList(Of StatsStorageEvent) = Nothing
			Try
				Dim ps As PreparedStatement = connection.prepareStatement(INSERT_STATIC_SQL)

				For Each p As Persistable In collection
					Dim ssesTemp As IList(Of StatsStorageEvent) = checkStorageEvents(p)
					If ssesTemp IsNot Nothing Then
						If sses Is Nothing Then
							sses = ssesTemp
						Else
							CType(sses, List(Of StatsStorageEvent)).AddRange(ssesTemp)
						End If
					End If

					If listeners.Count > 0 Then
						Dim sse As New StatsStorageEvent(Me, StatsStorageListener.EventType.PostStaticInfo, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
						If sses Is Nothing Then
							sses = New List(Of StatsStorageEvent)()
						End If
						sses.Add(sse)
					End If

					'Normally we'd batch these... sqlite has an autocommit feature that messes up batching with .addBatch() and .executeUpdate()
					Dim pair As Pair(Of String, SByte()) = serializeForDB(p)

					ps.setString(1, p.SessionID)
					ps.setString(2, p.TypeID)
					ps.setString(3, p.WorkerID)
					ps.setString(4, pair.First)
					ps.setBytes(5, pair.Second)
					ps.executeUpdate()
				Next p
			Catch e As SQLException
				Throw New Exception(e)
			End Try

			notifyListeners(sses)
		End Sub

		Public Overridable Sub putUpdate(ByVal update As Persistable) Implements StatsStorage.putUpdate
			putUpdate(Collections.singletonList(update))
		End Sub

		Public Overridable Sub putUpdate(Of T1 As Persistable)(ByVal collection As ICollection(Of T1))
			Dim sses As IList(Of StatsStorageEvent) = Nothing

			Try
				Dim ps As PreparedStatement = connection.prepareStatement(INSERT_UPDATE_SQL)

				For Each p As Persistable In collection
					Dim ssesTemp As IList(Of StatsStorageEvent) = checkStorageEvents(p)
					If ssesTemp IsNot Nothing Then
						If sses Is Nothing Then
							sses = ssesTemp
						Else
							CType(sses, List(Of StatsStorageEvent)).AddRange(ssesTemp)
						End If
					End If

					If listeners.Count > 0 Then
						Dim sse As New StatsStorageEvent(Me, StatsStorageListener.EventType.PostUpdate, p.SessionID, p.TypeID, p.WorkerID, p.TimeStamp)
						If sses Is Nothing Then
							sses = New List(Of StatsStorageEvent)()
						End If
						sses.Add(sse)
					End If

					'Normally we'd batch these... sqlite has an autocommit feature that messes up batching with .addBatch() and .executeUpdate()
					Dim pair As Pair(Of String, SByte()) = serializeForDB(p)

					ps.setString(1, p.SessionID)
					ps.setString(2, p.TypeID)
					ps.setString(3, p.WorkerID)
					ps.setLong(4, p.TimeStamp)
					ps.setString(5, pair.First)
					ps.setObject(6, pair.Second)
					ps.executeUpdate()
				Next p
			Catch e As SQLException
				Throw New Exception(e)
			End Try

			notifyListeners(sses)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws IOException
		Public Overridable Sub close() Implements StatsStorage.close
			Try
				connection.close()
			Catch e As Exception
				Throw New IOException(e)
			End Try
		End Sub

		Public Overridable ReadOnly Property Closed As Boolean Implements StatsStorage.isClosed
			Get
				Try
					Return connection.isClosed()
				Catch e As Exception
					Return True
				End Try
			End Get
		End Property

		Public Overridable Function listSessionIDs() As IList(Of String)
			Return selectDistinct("SessionID", True, True, False, Nothing, Nothing)
		End Function

		Public Overridable Function sessionExists(ByVal sessionID As String) As Boolean Implements StatsStorage.sessionExists
			Dim existsMetaSQL As String = "SELECT 1 FROM " & TABLE_NAME_METADATA & " WHERE SessionID = '" & sessionID & "';"
			Dim existsStaticSQL As String = "SELECT 1 FROM " & TABLE_NAME_STATIC_INFO & " WHERE SessionID = '" & sessionID & "';"

			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(existsMetaSQL)
					If rs.next() Then
						Return True
					End If
        
					rs = statement.executeQuery(existsStaticSQL)
					Return rs.next()
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getStaticInfo(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Persistable Implements StatsStorage.getStaticInfo
			Dim selectStaticSQL As String = "SELECT ObjectBytes FROM " & TABLE_NAME_STATIC_INFO & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "' AND WorkerID = '" & workerID & "';"
			Return queryAndGet(selectStaticSQL, 1)
		End Function

		Public Overridable Function getAllStaticInfos(ByVal sessionID As String, ByVal typeID As String) As IList(Of Persistable)
			Dim selectStaticSQL As String = "SELECT * FROM " & TABLE_NAME_STATIC_INFO & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "';"
			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(selectStaticSQL)
					Dim [out] As IList(Of Persistable) = New List(Of Persistable)()
					Do While rs.next()
						Dim bytes() As SByte = rs.getBytes(5)
						[out].Add(DirectCast(deserialize(bytes), Persistable))
					Loop
					Return [out]
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function listTypeIDsForSession(ByVal sessionID As String) As IList(Of String)
			Return selectDistinct("TypeID", True, True, True, "SessionID", sessionID)
		End Function

		Public Overridable Function listWorkerIDsForSession(ByVal sessionID As String) As IList(Of String)
			Return selectDistinct("WorkerID", False, True, True, "SessionID", sessionID)
		End Function

		Public Overridable Function listWorkerIDsForSessionAndType(ByVal sessionID As String, ByVal typeID As String) As IList(Of String)
			Dim uniqueStatic As String = "SELECT DISTINCT WorkerID FROM " & TABLE_NAME_STATIC_INFO & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "';"
			Dim uniqueUpdates As String = "SELECT DISTINCT WorkerID FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "';"

			Dim unique As ISet(Of String) = New HashSet(Of String)()
			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(uniqueStatic)
					Do While rs.next()
						Dim str As String = rs.getString(1)
						unique.Add(str)
					Loop
        
					rs = statement.executeQuery(uniqueUpdates)
					Do While rs.next()
						Dim str As String = rs.getString(1)
						unique.Add(str)
					Loop
        
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try

			Return New List(Of String)(unique)
		End Function

		Public Overridable Function getNumUpdateRecordsFor(ByVal sessionID As String) As Integer Implements StatsStorage.getNumUpdateRecordsFor
			Dim sql As String = "SELECT COUNT(*) FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "';"
			Try
					Using statement As Statement = connection.createStatement()
					Return statement.executeQuery(sql).getInt(1)
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getNumUpdateRecordsFor(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Integer Implements StatsStorage.getNumUpdateRecordsFor
			Dim sql As String = "SELECT COUNT(*) FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "' AND WorkerID = '" & workerID & "';"
			Try
					Using statement As Statement = connection.createStatement()
					Return statement.executeQuery(sql).getInt(1)
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getLatestUpdate(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Persistable Implements StatsStorage.getLatestUpdate
			Dim sql As String = "SELECT ObjectBytes FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "' AND WorkerID = '" & workerID & "' ORDER BY Timestamp DESC LIMIT 1;"
			Return queryAndGet(sql, 1)
		End Function

		Public Overridable Function getUpdate(ByVal sessionID As String, ByVal typeId As String, ByVal workerID As String, ByVal timestamp As Long) As Persistable Implements StatsStorage.getUpdate
			Dim sql As String = "SELECT ObjectBytes FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeId & "' AND WorkerID = '" & workerID & "' AND Timestamp = '" & timestamp & "';"
			Return queryAndGet(sql, 1)
		End Function

		Public Overridable Function getLatestUpdateAllWorkers(ByVal sessionID As String, ByVal typeID As String) As IList(Of Persistable)
			Dim sql As String = "SELECT workerId, MAX(Timestamp) FROM " & TABLE_NAME_UPDATES & " WHERE SessionID ='" & sessionID & "' AND " & "TypeID = '" & typeID & "' GROUP BY workerId"

			Dim m As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(sql)
					Do While rs.next()
						Dim wid As String = rs.getString(1)
						Dim ts As Long = rs.getLong(2)
						m(wid) = ts
					Loop
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try

			Dim [out] As IList(Of Persistable) = New List(Of Persistable)()
			For Each s As String In m.Keys
				[out].Add(getUpdate(sessionID, typeID, s, m(s)))
			Next s
			Return [out]
		End Function

		Public Overridable Function getAllUpdatesAfter(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamp As Long) As IList(Of Persistable)
			Dim sql As String = "SELECT * FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "' AND workerId = '" & workerID & "' AND Timestamp > " & timestamp & ";"
			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(sql)
					Dim [out] As IList(Of Persistable) = New List(Of Persistable)()
					Do While rs.next()
						Dim bytes() As SByte = rs.getBytes(6)
						[out].Add(DirectCast(deserialize(bytes), Persistable))
					Loop
					Return [out]
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getAllUpdatesAfter(ByVal sessionID As String, ByVal typeID As String, ByVal timestamp As Long) As IList(Of Persistable)
			Dim sql As String = "SELECT ObjectBytes FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "'  " & "AND TypeID = '" & typeID & "' AND Timestamp > " & timestamp & ";"
			Return queryUpdates(sql)
		End Function

		Public Overridable Function getAllUpdateTimes(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Long() Implements StatsStorage.getAllUpdateTimes
			Dim sql As String = "SELECT Timestamp FROM " & TABLE_NAME_UPDATES & " WHERE SessionID = '" & sessionID & "'  " & "AND TypeID = '" & typeID & "' AND workerID = '" & workerID & "';"
			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(sql)
					Dim list As New LongArrayList()
					Do While rs.next()
						list.add(rs.getLong(1))
					Loop
					Return list.toLongArray()
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getUpdates(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamps() As Long) As IList(Of Persistable)
			If timestamps Is Nothing OrElse timestamps.Length = 0 Then
				Return java.util.Collections.emptyList()
			End If

			Dim sb As New StringBuilder()
			sb.Append("SELECT ObjectBytes FROM ").Append(TABLE_NAME_UPDATES).Append(" WHERE SessionID = '").Append(sessionID).Append("' AND TypeID = '").Append(typeID).Append("' AND workerID='").Append(workerID).Append("'  AND Timestamp IN (")

			For i As Integer = 0 To timestamps.Length - 1
				If i > 0 Then
					sb.Append(",")
				End If
				sb.Append(timestamps(i))
			Next i
			sb.Append(");")

			Dim sql As String = sb.ToString()
			Return queryUpdates(sql)
		End Function

		Private Function queryUpdates(ByVal sql As String) As IList(Of Persistable)
			Try
					Using statement As Statement = connection.createStatement()
					Dim rs As ResultSet = statement.executeQuery(sql)
					Dim [out] As IList(Of Persistable) = New List(Of Persistable)()
					Do While rs.next()
						Dim bytes() As SByte = rs.getBytes(1)
						[out].Add(DirectCast(deserialize(bytes), Persistable))
					Loop
					Return [out]
					End Using
			Catch e As SQLException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getStorageMetaData(ByVal sessionID As String, ByVal typeID As String) As StorageMetaData Implements StatsStorage.getStorageMetaData
			Dim sql As String = "SELECT ObjectBytes FROM " & TABLE_NAME_METADATA & " WHERE SessionID = '" & sessionID & "' AND TypeID = '" & typeID & "' LIMIT 1;"
			Return queryAndGet(sql, 1)
		End Function

		Public Overridable Sub registerStatsStorageListener(ByVal listener As StatsStorageListener) Implements StatsStorage.registerStatsStorageListener
			listeners.Add(listener)
		End Sub

		Public Overridable Sub deregisterStatsStorageListener(ByVal listener As StatsStorageListener) Implements StatsStorage.deregisterStatsStorageListener
			listeners.Remove(listener)
		End Sub

		Public Overridable Sub removeAllListeners() Implements StatsStorage.removeAllListeners
			listeners.Clear()
		End Sub

		Public Overridable ReadOnly Property Listeners As IList(Of StatsStorageListener)
			Get
				Return New List(Of StatsStorageListener)(listeners)
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "J7FileStatsStorage(file=" & file & ")"
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
	End Class

End Namespace