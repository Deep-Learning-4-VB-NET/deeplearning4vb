Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports StorageType = org.deeplearning4j.core.storage.StorageType
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.core.storage.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class RemoteUIStatsStorageRouter implements org.deeplearning4j.core.storage.StatsStorageRouter, Serializable, Closeable
	<Serializable>
	Public Class RemoteUIStatsStorageRouter
		Implements StatsStorageRouter, System.IDisposable

		Private Const ROUTE_IS_DOWN As String = "Info posted to RemoteUIStatsStorageRouter but router is shut down."
		Private Const MAX_WARNINGS_REACHED As String = "RemoteUIStatsStorageRouter: Reached max shutdown warnings. No further warnings will be produced."
		''' <summary>
		''' Default path for posting data to the UI - i.e., http://localhost:9000/remoteReceive or similar
		''' </summary>
		Public Const DEFAULT_PATH As String = "remoteReceive"
		''' <summary>
		''' Default maximum number of (consecutive) retries on failure
		''' </summary>
		Public Const DEFAULT_MAX_RETRIES As Integer = 10
		''' <summary>
		''' Base delay for retries
		''' </summary>
		Public Const DEFAULT_BASE_RETR_DELAY_MS As Long = 1000
		''' <summary>
		''' Default backoff multiplicative factor for retrying
		''' </summary>
		Public Const DEFAULT_RETRY_BACKOFF_FACTOR As Double = 2.0

		Private Const MAX_SHUTDOWN_WARN_COUNT As Long = 5

		Private ReadOnly USER_AGENT As String = "Mozilla/5.0"


		Private ReadOnly url As URL
		Private ReadOnly maxRetryCount As Integer
		Private ReadOnly retryDelayMS As Long
		Private ReadOnly retryBackoffFactor As Double

		<NonSerialized>
		Private queue As New LinkedBlockingDeque(Of ToPost)()

		<NonSerialized>
		Private postThread As Thread

'JAVA TO VB CONVERTER NOTE: The field shutdown was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private shutdown_Conflict As New AtomicBoolean(False)
		Private shutdownWarnCount As New AtomicLong(0)

		Private Shared ReadOnly objectMapper As New ObjectMapper()

		''' <summary>
		''' Create remote UI with defaults for all values except address
		''' </summary>
		''' <param name="address"> Address of the remote UI: for example, "http://localhost:9000" </param>
		Public Sub New(ByVal address As String)
			Me.New(address, DEFAULT_MAX_RETRIES, DEFAULT_BASE_RETR_DELAY_MS, DEFAULT_RETRY_BACKOFF_FACTOR)
		End Sub

		''' <param name="address">            Address of the remote UI: for example, "http://localhost:9000" </param>
		''' <param name="maxRetryCount">      Maximum number of retries before failing. Set to -1 to always retry </param>
		''' <param name="retryDelayMS">       Base delay before retrying, in milliseconds </param>
		''' <param name="retryBackoffFactor"> Backoff factor for retrying: 2.0 for example gives delays of 1000, 2000, 4000, 8000,
		'''                           etc milliseconds, with a base retry delay of 1000 </param>
		Public Sub New(ByVal address As String, ByVal maxRetryCount As Integer, ByVal retryDelayMS As Long, ByVal retryBackoffFactor As Double)
			Me.New(address, DEFAULT_PATH, maxRetryCount, retryDelayMS, retryBackoffFactor)
		End Sub

		''' <param name="address">            Address of the remote UI: for example, "http://localhost:9000" </param>
		''' <param name="path">               Path/endpoint to post to: for example "remoteReceive" -> added to path to become like
		'''                           "http://localhost:9000/remoteReceive" </param>
		''' <param name="maxRetryCount">      Maximum number of retries before failing. Set to -1 to always retry </param>
		''' <param name="retryDelayMS">       Base delay before retrying, in milliseconds </param>
		''' <param name="retryBackoffFactor"> Backoff factor for retrying: 2.0 for example gives delays of 1000, 2000, 4000, 8000,
		'''                           etc milliseconds, with a base retry delay of 1000 </param>
		Public Sub New(ByVal address As String, ByVal path As String, ByVal maxRetryCount As Integer, ByVal retryDelayMS As Long, ByVal retryBackoffFactor As Double)
			Me.maxRetryCount = maxRetryCount
			Me.retryDelayMS = retryDelayMS
			Me.retryBackoffFactor = retryBackoffFactor

			Dim url As String = address
			If path IsNot Nothing Then
				If url.EndsWith("/", StringComparison.Ordinal) Then
					url = url & path
				Else
					url = url & "/" & path
				End If
			End If

			Try
				Me.url = New URL(url)
			Catch e As MalformedURLException
				Throw New Exception(e)
			End Try
		End Sub

		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			shutdown()
		End Sub

		Public Overridable Sub shutdown()
			Me.shutdown_Conflict.set(True)
		End Sub

		Private Sub checkThread()
			SyncLock Me
				If postThread Is Nothing Then
					postThread = New Thread(New PostRunnable(Me))
					postThread.setDaemon(True)
					postThread.Start()
				End If
				If queue Is Nothing Then
					'May be null if router has been deserialized
					queue = New LinkedBlockingDeque(Of ToPost)()
				End If
			End SyncLock
		End Sub

		Public Overridable Sub putStorageMetaData(ByVal storageMetaData As StorageMetaData)
			putStorageMetaData(Collections.singleton(storageMetaData))
		End Sub

		Public Overridable Sub putStorageMetaData(Of T1 As StorageMetaData)(ByVal storageMetaData As ICollection(Of T1))
			checkThread()
			If shutdown_Conflict.get() Then
				Dim count As Long = shutdownWarnCount.getAndIncrement()
				If count <= MAX_SHUTDOWN_WARN_COUNT Then
					log.warn(ROUTE_IS_DOWN)
				End If
				If count = MAX_SHUTDOWN_WARN_COUNT Then
					log.warn(MAX_WARNINGS_REACHED)
				End If
			Else
				For Each m As StorageMetaData In storageMetaData
					queue.add(New ToPost(m, Nothing, Nothing))
				Next m
			End If
		End Sub

		Public Overridable Sub putStaticInfo(ByVal staticInfo As Persistable)
			putStaticInfo(Collections.singletonList(staticInfo))
		End Sub

		Public Overridable Sub putStaticInfo(Of T1 As Persistable)(ByVal staticInfo As ICollection(Of T1))
			checkThread()
			If shutdown_Conflict.get() Then
				Dim count As Long = shutdownWarnCount.getAndIncrement()
				If count <= MAX_SHUTDOWN_WARN_COUNT Then
					log.warn(ROUTE_IS_DOWN)
				End If
				If count = MAX_SHUTDOWN_WARN_COUNT Then
					log.warn(MAX_WARNINGS_REACHED)
				End If
			Else
				For Each p As Persistable In staticInfo
					queue.add(New ToPost(Nothing, p, Nothing))
				Next p
			End If
		End Sub

		Public Overridable Sub putUpdate(ByVal update As Persistable)
			putUpdate(Collections.singleton(update))
		End Sub

		Public Overridable Sub putUpdate(Of T1 As Persistable)(ByVal updates As ICollection(Of T1))
			checkThread()
			If shutdown_Conflict.get() Then
				Dim count As Long = shutdownWarnCount.getAndIncrement()
				If count <= MAX_SHUTDOWN_WARN_COUNT Then
					log.warn(ROUTE_IS_DOWN)
				End If
				If count = MAX_SHUTDOWN_WARN_COUNT Then
					log.warn(MAX_WARNINGS_REACHED)
				End If
			Else
				For Each p As Persistable In updates
					queue.add(New ToPost(Nothing, Nothing, p))
				Next p
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class ToPost
		Private Class ToPost
			Friend ReadOnly meta As StorageMetaData
			Friend ReadOnly staticInfo As Persistable
			Friend ReadOnly update As Persistable
		End Class

		'Runnable class for doing async posting
		Private Class PostRunnable
			Implements ThreadStart

			Private ReadOnly outerInstance As RemoteUIStatsStorageRouter

			Public Sub New(ByVal outerInstance As RemoteUIStatsStorageRouter)
				Me.outerInstance = outerInstance
			End Sub


			Friend failureCount As Integer = 0
			Friend nextDelayMs As Long = outerInstance.retryDelayMS


			Public Overrides Sub run()
				Try
					runHelper()
				Catch e As Exception
					log.error("Exception encountered in remote UI posting thread. Shutting down.", e)
					outerInstance.shutdown_Conflict.set(True)
				End Try
			End Sub

			Friend Overridable Sub runHelper()

				Do While Not outerInstance.shutdown_Conflict.get()

					Dim list As IList(Of ToPost) = New List(Of ToPost)()
					Dim t As ToPost
					Try
						t = outerInstance.queue.take() 'Blocking operation
					Catch e As InterruptedException
						Thread.CurrentThread.Interrupt()
						Continue Do
					End Try
					list.Add(t)
					outerInstance.queue.drainTo(list) 'Non-blocking

					Dim successCount As Integer = 0
					For Each toPost As ToPost In list
						Dim success As Boolean
						Try
							success = outerInstance.tryPost(toPost)
						Catch e As IOException
							failureCount += 1
							log.warn("Error posting to remote UI at {}, consecutive failure count = {}. Waiting {} ms before retrying", outerInstance.url, failureCount, nextDelayMs, e)
							success = False
						End Try
						If Not success Then
							For i As Integer = list.Count - 1 To successCount + 1 Step -1
								outerInstance.queue.addFirst(list(i)) 'Add remaining back to be processed in original order
							Next i
							waitForRetry()
							Exit For
						Else
							successCount += 1
							failureCount = 0
							nextDelayMs = outerInstance.retryDelayMS
						End If
					Next toPost
				Loop
			End Sub

			Friend Overridable Sub waitForRetry()
				If outerInstance.maxRetryCount >= 0 AndAlso failureCount > outerInstance.maxRetryCount Then
					Throw New Exception("RemoteUIStatsStorageRouter: hit maximum consecutive failures(" & outerInstance.maxRetryCount & "). Shutting down remote router thread")
				Else
					Try
						Thread.Sleep(nextDelayMs)
					Catch e As InterruptedException
						Thread.CurrentThread.Interrupt()
					End Try
					nextDelayMs *= outerInstance.retryBackoffFactor
				End If
			End Sub
		End Class


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.net.HttpURLConnection getConnection() throws IOException
		Private ReadOnly Property Connection As HttpURLConnection
			Get
				Dim connection As HttpURLConnection = CType(url.openConnection(), HttpURLConnection)
				connection.setRequestMethod("POST")
				connection.setRequestProperty("User-Agent", USER_AGENT)
				connection.setRequestProperty("Content-Type", "application/json")
				connection.setDoOutput(True)
				Return connection
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private boolean tryPost(ToPost toPost) throws IOException
		Private Function tryPost(ByVal toPost As ToPost) As Boolean

			Dim connection As HttpURLConnection = Me.Connection

			Dim className As String
			Dim asBytes() As SByte
			Dim type As StorageType
			If toPost.getMeta() IsNot Nothing Then
				Dim smd As StorageMetaData = toPost.getMeta()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				className = smd.GetType().FullName
				asBytes = smd.encode()
				type = StorageType.MetaData
			ElseIf toPost.getStaticInfo() IsNot Nothing Then
				Dim p As Persistable = toPost.getStaticInfo()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				className = p.GetType().FullName
				asBytes = p.encode()
				type = StorageType.StaticInfo
			Else
				Dim p As Persistable = toPost.getUpdate()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				className = p.GetType().FullName
				asBytes = p.encode()
				type = StorageType.Update
			End If

			Dim base64 As String = DatatypeConverter.printBase64Binary(asBytes)

			Dim jsonObj As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
			jsonObj("type") = type.ToString()
			jsonObj("class") = className
			jsonObj("data") = base64

			Dim str As String
			Try
				str = objectMapper.writeValueAsString(jsonObj)
			Catch e As Exception
				Throw New Exception(e) 'Should never get an exception from simple Map<String,String>
			End Try

			Dim dos As New DataOutputStream(connection.getOutputStream())
			dos.writeBytes(str)
			dos.flush()
			dos.close()

			Try
				Dim responseCode As Integer = connection.getResponseCode()

				If responseCode <> 200 Then
					Dim [in] As New StreamReader(connection.getInputStream())
					Dim inputLine As String
					Dim response As New StringBuilder()

					inputLine = [in].ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((inputLine = in.readLine()) != null)
					Do While inputLine IsNot Nothing
						response.Append(inputLine)
							inputLine = [in].ReadLine()
					Loop
					[in].Close()

					log.warn("Error posting to remote UI - received response code {}" & vbTab & "Content: {}", response, response.ToString())

					Return False
				End If
			Catch e As IOException
				Dim msg As String = e.Message
				If msg.Contains("403 for URL") Then
					log.warn("Error posting to remote UI at {} (Response code: 403)." & " Remote listener support is not enabled? use UIServer.getInstance().enableRemoteListener()", url, e)
				Else
					log.warn("Error posting to remote UI at {}", url, e)
				End If

				Return False
			End Try

			Return True
		End Function
	End Class

End Namespace