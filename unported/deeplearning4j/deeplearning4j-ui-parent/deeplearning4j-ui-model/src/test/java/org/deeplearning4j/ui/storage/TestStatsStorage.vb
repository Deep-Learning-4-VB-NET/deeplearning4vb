Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports StatsStorageListener = org.deeplearning4j.core.storage.StatsStorageListener
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsInitializationReport = org.deeplearning4j.ui.model.stats.api.StatsInitializationReport
Imports StatsReport = org.deeplearning4j.ui.model.stats.api.StatsReport
Imports SbeStatsInitializationReport = org.deeplearning4j.ui.model.stats.impl.SbeStatsInitializationReport
Imports SbeStatsReport = org.deeplearning4j.ui.model.stats.impl.SbeStatsReport
Imports JavaStatsInitializationReport = org.deeplearning4j.ui.model.stats.impl.java.JavaStatsInitializationReport
Imports JavaStatsReport = org.deeplearning4j.ui.model.stats.impl.java.JavaStatsReport
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports MapDBStatsStorage = org.deeplearning4j.ui.model.storage.mapdb.MapDBStatsStorage
Imports J7FileStatsStorage = org.deeplearning4j.ui.model.storage.sqlite.J7FileStatsStorage
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.ui.storage



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestStatsStorage extends org.deeplearning4j.BaseDL4JTest
	Public Class TestStatsStorage
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/21 - Failing on linux-x86_64-cuda-9.2 only - Issue #7657") public void testStatsStorage(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStatsStorage(ByVal testDir As Path)

			For Each useJ7Storage As Boolean In New Boolean() {False, True}
				For i As Integer = 0 To 2

					Dim ss As StatsStorage
					Select Case i
						Case 0
							Dim f As File = createTempFile(testDir,"TestMapDbStatsStore", ".db")
							f.delete() 'Don't want file to exist...
							ss = (New MapDBStatsStorage.Builder()).file(f).build()
						Case 1
							Dim f2 As File = createTempFile(testDir,"TestJ7FileStatsStore", ".db")
							f2.delete() 'Don't want file to exist...
							ss = New J7FileStatsStorage(f2)
						Case 2
							ss = New InMemoryStatsStorage()
						Case Else
							Throw New Exception()
					End Select


					Dim l As New CountingListener()
					ss.registerStatsStorageListener(l)
					assertEquals(1, ss.getListeners().Count)

					assertEquals(0, ss.listSessionIDs().Count)
					assertNull(ss.getLatestUpdate("sessionID", "typeID", "workerID"))
					assertEquals(0, ss.listSessionIDs().Count)


					ss.putStaticInfo(getInitReport(0, 0, 0, useJ7Storage))
					assertEquals(1, l.countNewSession)
					assertEquals(1, l.countNewWorkerId)
					assertEquals(1, l.countStaticInfo)
					assertEquals(0, l.countUpdate)

					assertEquals(Collections.singletonList("sid0"), ss.listSessionIDs())
					assertTrue(ss.sessionExists("sid0"))
					assertFalse(ss.sessionExists("sid1"))
					Dim expected As Persistable = getInitReport(0, 0, 0, useJ7Storage)
					Dim p As Persistable = ss.getStaticInfo("sid0", "tid0", "wid0")
					assertEquals(expected, p)
					Dim allStatic As IList(Of Persistable) = ss.getAllStaticInfos("sid0", "tid0")
					assertEquals(Collections.singletonList(expected), allStatic)
					assertNull(ss.getLatestUpdate("sid0", "tid0", "wid0"))
					assertEquals(Collections.singletonList("tid0"), ss.listTypeIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSessionAndType("sid0", "tid0"))
					assertEquals(0, ss.getAllUpdatesAfter("sid0", "tid0", "wid0", 0).Count)
					assertEquals(0, ss.getNumUpdateRecordsFor("sid0"))
					assertEquals(0, ss.getNumUpdateRecordsFor("sid0", "tid0", "wid0"))


					'Put first update
					ss.putUpdate(getReport(0, 0, 0, 12345, useJ7Storage))
					assertEquals(1, ss.getNumUpdateRecordsFor("sid0"))
					assertEquals(getReport(0, 0, 0, 12345, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid0"))
					assertEquals(Collections.singletonList("tid0"), ss.listTypeIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSessionAndType("sid0", "tid0"))
					assertEquals(Collections.singletonList(getReport(0, 0, 0, 12345, useJ7Storage)), ss.getAllUpdatesAfter("sid0", "tid0", "wid0", 0))
					assertEquals(1, ss.getNumUpdateRecordsFor("sid0"))
					assertEquals(1, ss.getNumUpdateRecordsFor("sid0", "tid0", "wid0"))

					Dim list As IList(Of Persistable) = ss.getLatestUpdateAllWorkers("sid0", "tid0")
					assertEquals(1, list.Count)
					assertEquals(getReport(0, 0, 0, 12345, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid0", 12345))
					assertEquals(1, l.countNewSession)
					assertEquals(1, l.countNewWorkerId)
					assertEquals(1, l.countStaticInfo)
					assertEquals(1, l.countUpdate)

					'Put second update
					ss.putUpdate(getReport(0, 0, 0, 12346, useJ7Storage))
					assertEquals(1, ss.getLatestUpdateAllWorkers("sid0", "tid0").Count)
					assertEquals(Collections.singletonList("tid0"), ss.listTypeIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSessionAndType("sid0", "tid0"))
					assertEquals(getReport(0, 0, 0, 12346, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid0"))
					assertEquals(getReport(0, 0, 0, 12346, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid0", 12346))

					ss.putUpdate(getReport(0, 0, 1, 12345, useJ7Storage))
					assertEquals(2, ss.getLatestUpdateAllWorkers("sid0", "tid0").Count)
					assertEquals(getReport(0, 0, 1, 12345, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid1"))
					assertEquals(getReport(0, 0, 1, 12345, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid1", 12345))

					assertEquals(1, l.countNewSession)
					assertEquals(2, l.countNewWorkerId)
					assertEquals(1, l.countStaticInfo)
					assertEquals(3, l.countUpdate)


					'Put static info and update with different session, type and worker IDs
					ss.putStaticInfo(getInitReport(100, 200, 300, useJ7Storage))
					assertEquals(2, ss.getLatestUpdateAllWorkers("sid0", "tid0").Count)

					ss.putUpdate(getReport(100, 200, 300, 12346, useJ7Storage))
					assertEquals(Collections.singletonList(getReport(100, 200, 300, 12346, useJ7Storage)), ss.getLatestUpdateAllWorkers("sid100", "tid200"))
					assertEquals(Collections.singletonList("tid200"), ss.listTypeIDsForSession("sid100"))
					Dim temp As IList(Of String) = ss.listWorkerIDsForSession("sid100")
					Console.WriteLine("temp: " & temp)
					assertEquals(Collections.singletonList("wid300"), ss.listWorkerIDsForSession("sid100"))
					assertEquals(Collections.singletonList("wid300"), ss.listWorkerIDsForSessionAndType("sid100", "tid200"))
					assertEquals(getReport(100, 200, 300, 12346, useJ7Storage), ss.getLatestUpdate("sid100", "tid200", "wid300"))
					assertEquals(getReport(100, 200, 300, 12346, useJ7Storage), ss.getUpdate("sid100", "tid200", "wid300", 12346))

					assertEquals(2, l.countNewSession)
					assertEquals(3, l.countNewWorkerId)
					assertEquals(2, l.countStaticInfo)
					assertEquals(4, l.countUpdate)




					'Test get updates times:
					Dim expTSWid0() As Long = {12345, 12346}
					Dim actTSWid0() As Long = ss.getAllUpdateTimes("sid0", "tid0", "wid0")
					assertArrayEquals(expTSWid0, actTSWid0)

					Dim expTSWid1() As Long = {12345}
					Dim actTSWid1() As Long = ss.getAllUpdateTimes("sid0", "tid0", "wid1")
					assertArrayEquals(expTSWid1, actTSWid1)



					ss.putUpdate(getReport(100, 200, 300, 12347, useJ7Storage))
					ss.putUpdate(getReport(100, 200, 300, 12348, useJ7Storage))
					ss.putUpdate(getReport(100, 200, 300, 12349, useJ7Storage))

					Dim expTSWid300() As Long = {12346, 12347, 12348, 12349}
					Dim actTSWid300() As Long = ss.getAllUpdateTimes("sid100", "tid200", "wid300")
					assertArrayEquals(expTSWid300, actTSWid300)

					'Test subset query:
					Dim subset As IList(Of Persistable) = ss.getUpdates("sid100", "tid200", "wid300", New Long(){12346, 12349})
					assertEquals(2, subset.Count)
					assertEquals(java.util.Arrays.asList(getReport(100, 200, 300, 12346, useJ7Storage), getReport(100, 200, 300, 12349, useJ7Storage)), subset)
				Next i
			Next useJ7Storage
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/21 - Failing on linux-x86_64-cuda-9.2 only - Issue #7657") public void testFileStatsStore(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFileStatsStore(ByVal testDir As Path)

			For Each useJ7Storage As Boolean In New Boolean() {False, True}
				For i As Integer = 0 To 1
					Dim f As File
					If i = 0 Then
						f = createTempFile(testDir,"TestMapDbStatsStore", ".db")
					Else
						f = createTempFile(testDir,"TestSqliteStatsStore", ".db")
					End If

					f.delete() 'Don't want file to exist...
					Dim ss As StatsStorage
					If i = 0 Then
						ss = (New MapDBStatsStorage.Builder()).file(f).build()
					Else
						ss = New J7FileStatsStorage(f)
					End If


					Dim l As New CountingListener()
					ss.registerStatsStorageListener(l)
					assertEquals(1, ss.getListeners().Count)

					assertEquals(0, ss.listSessionIDs().Count)
					assertNull(ss.getLatestUpdate("sessionID", "typeID", "workerID"))
					assertEquals(0, ss.listSessionIDs().Count)


					ss.putStaticInfo(getInitReport(0, 0, 0, useJ7Storage))
					assertEquals(1, l.countNewSession)
					assertEquals(1, l.countNewWorkerId)
					assertEquals(1, l.countStaticInfo)
					assertEquals(0, l.countUpdate)

					assertEquals(Collections.singletonList("sid0"), ss.listSessionIDs())
					assertTrue(ss.sessionExists("sid0"))
					assertFalse(ss.sessionExists("sid1"))
					Dim expected As Persistable = getInitReport(0, 0, 0, useJ7Storage)
					Dim p As Persistable = ss.getStaticInfo("sid0", "tid0", "wid0")
					assertEquals(expected, p)
					Dim allStatic As IList(Of Persistable) = ss.getAllStaticInfos("sid0", "tid0")
					assertEquals(Collections.singletonList(expected), allStatic)
					assertNull(ss.getLatestUpdate("sid0", "tid0", "wid0"))
					assertEquals(Collections.singletonList("tid0"), ss.listTypeIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSessionAndType("sid0", "tid0"))
					assertEquals(0, ss.getAllUpdatesAfter("sid0", "tid0", "wid0", 0).Count)
					assertEquals(0, ss.getNumUpdateRecordsFor("sid0"))
					assertEquals(0, ss.getNumUpdateRecordsFor("sid0", "tid0", "wid0"))


					'Put first update
					ss.putUpdate(getReport(0, 0, 0, 12345, useJ7Storage))
					assertEquals(1, ss.getNumUpdateRecordsFor("sid0"))
					assertEquals(getReport(0, 0, 0, 12345, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid0"))
					assertEquals(Collections.singletonList("tid0"), ss.listTypeIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSessionAndType("sid0", "tid0"))
					assertEquals(Collections.singletonList(getReport(0, 0, 0, 12345, useJ7Storage)), ss.getAllUpdatesAfter("sid0", "tid0", "wid0", 0))
					assertEquals(1, ss.getNumUpdateRecordsFor("sid0"))
					assertEquals(1, ss.getNumUpdateRecordsFor("sid0", "tid0", "wid0"))

					Dim list As IList(Of Persistable) = ss.getLatestUpdateAllWorkers("sid0", "tid0")
					assertEquals(1, list.Count)
					assertEquals(getReport(0, 0, 0, 12345, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid0", 12345))
					assertEquals(1, l.countNewSession)
					assertEquals(1, l.countNewWorkerId)
					assertEquals(1, l.countStaticInfo)
					assertEquals(1, l.countUpdate)

					'Put second update
					ss.putUpdate(getReport(0, 0, 0, 12346, useJ7Storage))
					assertEquals(1, ss.getLatestUpdateAllWorkers("sid0", "tid0").Count)
					assertEquals(Collections.singletonList("tid0"), ss.listTypeIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSession("sid0"))
					assertEquals(Collections.singletonList("wid0"), ss.listWorkerIDsForSessionAndType("sid0", "tid0"))
					assertEquals(getReport(0, 0, 0, 12346, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid0"))
					assertEquals(getReport(0, 0, 0, 12346, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid0", 12346))

					ss.putUpdate(getReport(0, 0, 1, 12345, useJ7Storage))
					assertEquals(2, ss.getLatestUpdateAllWorkers("sid0", "tid0").Count)
					assertEquals(getReport(0, 0, 1, 12345, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid1"))
					assertEquals(getReport(0, 0, 1, 12345, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid1", 12345))

					assertEquals(1, l.countNewSession)
					assertEquals(2, l.countNewWorkerId)
					assertEquals(1, l.countStaticInfo)
					assertEquals(3, l.countUpdate)


					'Put static info and update with different session, type and worker IDs
					ss.putStaticInfo(getInitReport(100, 200, 300, useJ7Storage))
					assertEquals(2, ss.getLatestUpdateAllWorkers("sid0", "tid0").Count)

					ss.putUpdate(getReport(100, 200, 300, 12346, useJ7Storage))
					assertEquals(Collections.singletonList(getReport(100, 200, 300, 12346, useJ7Storage)), ss.getLatestUpdateAllWorkers("sid100", "tid200"))
					assertEquals(Collections.singletonList("tid200"), ss.listTypeIDsForSession("sid100"))
					Dim temp As IList(Of String) = ss.listWorkerIDsForSession("sid100")
					Console.WriteLine("temp: " & temp)
					assertEquals(Collections.singletonList("wid300"), ss.listWorkerIDsForSession("sid100"))
					assertEquals(Collections.singletonList("wid300"), ss.listWorkerIDsForSessionAndType("sid100", "tid200"))
					assertEquals(getReport(100, 200, 300, 12346, useJ7Storage), ss.getLatestUpdate("sid100", "tid200", "wid300"))
					assertEquals(getReport(100, 200, 300, 12346, useJ7Storage), ss.getUpdate("sid100", "tid200", "wid300", 12346))

					assertEquals(2, l.countNewSession)
					assertEquals(3, l.countNewWorkerId)
					assertEquals(2, l.countStaticInfo)
					assertEquals(4, l.countUpdate)


					'Close and re-open
					ss.close()
					assertTrue(ss.Closed)

					If i = 0 Then
						ss = (New MapDBStatsStorage.Builder()).file(f).build()
					Else
						ss = New J7FileStatsStorage(f)
					End If


					assertEquals(getReport(0, 0, 0, 12345, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid0", 12345))
					assertEquals(getReport(0, 0, 0, 12346, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid0"))
					assertEquals(getReport(0, 0, 0, 12346, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid0", 12346))
					assertEquals(getReport(0, 0, 1, 12345, useJ7Storage), ss.getLatestUpdate("sid0", "tid0", "wid1"))
					assertEquals(getReport(0, 0, 1, 12345, useJ7Storage), ss.getUpdate("sid0", "tid0", "wid1", 12345))
					assertEquals(2, ss.getLatestUpdateAllWorkers("sid0", "tid0").Count)


					assertEquals(1, ss.getLatestUpdateAllWorkers("sid100", "tid200").Count)
					assertEquals(Collections.singletonList("tid200"), ss.listTypeIDsForSession("sid100"))
					assertEquals(Collections.singletonList("wid300"), ss.listWorkerIDsForSession("sid100"))
					assertEquals(Collections.singletonList("wid300"), ss.listWorkerIDsForSessionAndType("sid100", "tid200"))
					assertEquals(getReport(100, 200, 300, 12346, useJ7Storage), ss.getLatestUpdate("sid100", "tid200", "wid300"))
					assertEquals(getReport(100, 200, 300, 12346, useJ7Storage), ss.getUpdate("sid100", "tid200", "wid300", 12346))
				Next i
			Next useJ7Storage
		End Sub

		Private Shared Function getInitReport(ByVal idNumber As Integer, ByVal tid As Integer, ByVal wid As Integer, ByVal useJ7Storage As Boolean) As StatsInitializationReport
			Dim rep As StatsInitializationReport
			If useJ7Storage Then
				rep = New JavaStatsInitializationReport()
			Else
				rep = New SbeStatsInitializationReport()
			End If

			rep.reportModelInfo("classname", "jsonconfig", New String() {"p0", "p1"}, 1, 10)
			rep.reportIDs("sid" & idNumber, "tid" & tid, "wid" & wid, 12345)
			rep.reportHardwareInfo(0, 2, 1000, 2000, New Long() {3000, 4000}, New String() {"dev0", "dev1"}, "hardwareuid")
			Dim envInfo As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			envInfo("envInfo0") = "value0"
			envInfo("envInfo1") = "value1"
			rep.reportSoftwareInfo("arch", "osName", "jvmName", "jvmVersion", "1.8", "backend", "dtype", "hostname", "jvmuid", envInfo)
			Return rep
		End Function

		Private Shared Function getReport(ByVal sid As Integer, ByVal tid As Integer, ByVal wid As Integer, ByVal time As Long, ByVal useJ7Storage As Boolean) As StatsReport
			Dim rep As StatsReport
			If useJ7Storage Then
				rep = New JavaStatsReport()
			Else
				rep = New SbeStatsReport()
			End If

			rep.reportIDs("sid" & sid, "tid" & tid, "wid" & wid, time)
			rep.reportScore(100.0)
			rep.reportPerformance(1000, 1001, 1002, 1003.0, 1004.0)
			Return rep
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data private static class CountingListener implements org.deeplearning4j.core.storage.StatsStorageListener
		Private Class CountingListener
			Implements StatsStorageListener

			Friend countNewSession As Integer
			Friend countNewTypeID As Integer
			Friend countNewWorkerId As Integer
			Friend countStaticInfo As Integer
			Friend countUpdate As Integer
			Friend countMetaData As Integer

			Public Overridable Sub notify(ByVal [event] As StatsStorageEvent)
				Console.WriteLine("Event: " & [event])
				Select Case [event].getEventType()
					Case NewSessionID
						countNewSession += 1
					Case NewTypeID
						countNewTypeID += 1
					Case NewWorkerID
						countNewWorkerId += 1
					Case PostMetaData
						countMetaData += 1
					Case PostStaticInfo
						countStaticInfo += 1
					Case PostUpdate
						countUpdate += 1
				End Select
			End Sub
		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.io.File createTempFile(java.nio.file.Path testDir, String prefix, String suffix) throws java.io.IOException
		Private Function createTempFile(ByVal testDir As Path, ByVal prefix As String, ByVal suffix As String) As File
			Dim newFile As New File(testDir.toFile(),prefix & "-" & System.nanoTime() & suffix)
			newFile.createNewFile()
			newFile.deleteOnExit()
			Return newFile
		End Function

	End Class

End Namespace