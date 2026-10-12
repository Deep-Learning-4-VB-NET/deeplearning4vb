Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Configuration = freemarker.template.Configuration
Imports Template = freemarker.template.Template
Imports TemplateExceptionHandler = freemarker.template.TemplateExceptionHandler
Imports Version = freemarker.template.Version
Imports HttpResponseStatus = io.netty.handler.codec.http.HttpResponseStatus
Imports RoutingContext = io.vertx.ext.web.RoutingContext
Imports LongArrayList = it.unimi.dsi.fastutil.longs.LongArrayList
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports StatsStorageListener = org.deeplearning4j.core.storage.StatsStorageListener
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports org.deeplearning4j.nn.conf.layers
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports VertxUIServer = org.deeplearning4j.ui.VertxUIServer
Imports HttpMethod = org.deeplearning4j.ui.api.HttpMethod
Imports I18N = org.deeplearning4j.ui.api.I18N
Imports Route = org.deeplearning4j.ui.api.Route
Imports UIModule = org.deeplearning4j.ui.api.UIModule
Imports DefaultI18N = org.deeplearning4j.ui.i18n.DefaultI18N
Imports I18NProvider = org.deeplearning4j.ui.i18n.I18NProvider
Imports I18NResource = org.deeplearning4j.ui.i18n.I18NResource
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports Histogram = org.deeplearning4j.ui.model.stats.api.Histogram
Imports StatsInitializationReport = org.deeplearning4j.ui.model.stats.api.StatsInitializationReport
Imports StatsReport = org.deeplearning4j.ui.model.stats.api.StatsReport
Imports StatsType = org.deeplearning4j.ui.model.stats.api.StatsType
Imports org.nd4j.common.function
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports Resources = org.nd4j.common.resources.Resources
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException

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

Namespace org.deeplearning4j.ui.module.train


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TrainModule implements org.deeplearning4j.ui.api.UIModule
	Public Class TrainModule
		Implements UIModule

		Public Const NAN_REPLACEMENT_VALUE As Double = 0.0 'UI front-end chokes on NaN in JSON
		Public Const DEFAULT_MAX_CHART_POINTS As Integer = 512
		Private Shared ReadOnly df2 As New DecimalFormat("#.00")
		Private Shared dateFormat As DateFormat = New SimpleDateFormat("yyyy-MM-dd HH:mm:ss")

		Private Enum ModelType
			MLN
			CG
			Layer
		End Enum

		Private ReadOnly maxChartPoints As Integer 'Technically, the way it's set up: won't exceed 2*maxChartPoints
		Private knownSessionIDs As IDictionary(Of String, StatsStorage) = Collections.synchronizedMap(New Dictionary(Of String, StatsStorage)())
		Private currentSessionID As String
		Private currentWorkerIdx As Integer
		Private workerIdxCount As IDictionary(Of String, AtomicInteger) = New ConcurrentDictionary(Of String, AtomicInteger)() 'Key: session ID
		Private workerIdxToName As IDictionary(Of String, IDictionary(Of Integer, String)) = New ConcurrentDictionary(Of String, IDictionary(Of Integer, String))() 'Key: session ID
		Private lastUpdateForSession As IDictionary(Of String, Long) = New ConcurrentDictionary(Of String, Long)()


		Private ReadOnly configuration As Configuration

		''' <summary>
		''' TrainModule
		''' </summary>
		Public Sub New()
			Dim maxChartPointsProp As String = System.getProperty(DL4JSystemProperties.CHART_MAX_POINTS_PROPERTY)
			Dim value As Integer = DEFAULT_MAX_CHART_POINTS
			If maxChartPointsProp IsNot Nothing Then
				Try
					value = Integer.Parse(maxChartPointsProp)
				Catch e As System.FormatException
					log.warn("Invalid system property: {} = {}", DL4JSystemProperties.CHART_MAX_POINTS_PROPERTY, maxChartPointsProp)
				End Try
			End If
			If value >= 10 Then
				maxChartPoints = value
			Else
				maxChartPoints = DEFAULT_MAX_CHART_POINTS
			End If

			configuration = New Configuration(New Version(2, 3, 23))
			configuration.setDefaultEncoding("UTF-8")
			configuration.setLocale(Locale.US)
			configuration.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER)

			configuration.setClassForTemplateLoading(GetType(TrainModule), "")
			Try
				Dim dir As File = Resources.asFile("templates/TrainingOverview.html.ftl").getParentFile()
				configuration.setDirectoryForTemplateLoading(dir)
			Catch t As Exception
				Throw New Exception(t)
			End Try
		End Sub

		Public Overridable ReadOnly Property CallbackTypeIDs As IList(Of String)
			Get
				Return Collections.singletonList(StatsListener.TYPE_ID)
			End Get
		End Property

		Public Overridable ReadOnly Property Routes As IList(Of Route)
			Get
				Dim r As IList(Of Route) = New List(Of Route)()
				r.Add(New Route("/train/multisession", HttpMethod.GET, Function(path, rc) rc.response().end(If(VertxUIServer.getInstance().isMultiSession(), "true", "false"))))
				If VertxUIServer.getInstance().isMultiSession() Then
					r.Add(New Route("/train", HttpMethod.GET, Sub(path, rc) Me.listSessions(rc)))
					r.Add(New Route("/train/:sessionId", HttpMethod.GET, Sub(path, rc)
					rc.response().putHeader("location", path.get(0) & "/overview").setStatusCode(HttpResponseStatus.FOUND.code()).end()
					End Sub))
					r.Add(New Route("/train/:sessionId/overview", HttpMethod.GET, Sub(path, rc)
					If knownSessionIDs.ContainsKey(path.get(0)) Then
						renderFtl("TrainingOverview.html.ftl", rc)
					Else
						sessionNotFound(path.get(0), rc.request().path(), rc)
					End If
					End Sub))
					r.Add(New Route("/train/:sessionId/overview/data", HttpMethod.GET, Sub(path, rc)
					If knownSessionIDs.ContainsKey(path.get(0)) Then
						getOverviewDataForSession(path.get(0), rc)
					Else
						sessionNotFound(path.get(0), rc.request().path(), rc)
					End If
					End Sub))
					r.Add(New Route("/train/:sessionId/model", HttpMethod.GET, Sub(path, rc)
					If knownSessionIDs.ContainsKey(path.get(0)) Then
						renderFtl("TrainingModel.html.ftl", rc)
					Else
						sessionNotFound(path.get(0), rc.request().path(), rc)
					End If
					End Sub))
					r.Add(New Route("/train/:sessionId/model/graph", HttpMethod.GET, Sub(path, rc) Me.getModelGraphForSession(path.get(0), rc)))
					r.Add(New Route("/train/:sessionId/model/data/:layerId", HttpMethod.GET, Sub(path, rc) Me.getModelDataForSession(path.get(0), path.get(1), rc)))
					r.Add(New Route("/train/:sessionId/system", HttpMethod.GET, Sub(path, rc)
					If knownSessionIDs.ContainsKey(path.get(0)) Then
						Me.renderFtl("TrainingSystem.html.ftl", rc)
					Else
						sessionNotFound(path.get(0), rc.request().path(), rc)
					End If
					End Sub))
					r.Add(New Route("/train/:sessionId/info", HttpMethod.GET, Sub(path, rc) Me.sessionInfoForSession(path.get(0), rc)))
					r.Add(New Route("/train/:sessionId/system/data", HttpMethod.GET, Sub(path, rc) Me.getSystemDataForSession(path.get(0), rc)))
				Else
					r.Add(New Route("/train", HttpMethod.GET, Function(path, rc) rc.reroute("/train/overview")))
					r.Add(New Route("/train/sessions/current", HttpMethod.GET, Function(path, rc) rc.response().end(If(currentSessionID Is Nothing, "", currentSessionID))))
					r.Add(New Route("/train/sessions/set/:to", HttpMethod.GET, Sub(path, rc) Me.setSession(path.get(0), rc)))
					r.Add(New Route("/train/overview", HttpMethod.GET, Sub(path, rc) Me.renderFtl("TrainingOverview.html.ftl", rc)))
					r.Add(New Route("/train/overview/data", HttpMethod.GET, Sub(path, rc) Me.getOverviewData(rc)))
					r.Add(New Route("/train/model", HttpMethod.GET, Sub(path, rc) Me.renderFtl("TrainingModel.html.ftl", rc)))
					r.Add(New Route("/train/model/graph", HttpMethod.GET, Sub(path, rc) Me.getModelGraph(rc)))
					r.Add(New Route("/train/model/data/:layerId", HttpMethod.GET, Sub(path, rc) Me.getModelData(path.get(0), rc)))
					r.Add(New Route("/train/system", HttpMethod.GET, Sub(path, rc) Me.renderFtl("TrainingSystem.html.ftl", rc)))
					r.Add(New Route("/train/sessions/info", HttpMethod.GET, Sub(path, rc) Me.sessionInfo(rc)))
					r.Add(New Route("/train/system/data", HttpMethod.GET, Sub(path, rc) Me.getSystemData(rc)))
				End If
    
				' common for single- and multi-session mode
				r.Add(New Route("/train/sessions/lastUpdate/:sessionId", HttpMethod.GET, Sub(path, rc) Me.getLastUpdateForSession(path.get(0), rc)))
				r.Add(New Route("/train/workers/setByIdx/:to", HttpMethod.GET, Sub(path, rc) Me.setWorkerByIdx(path.get(0), rc)))
    
				Return r
			End Get
		End Property

		''' <summary>
		''' Render a single Freemarker .ftl file from the /templates/ directory </summary>
		''' <param name="file"> File to render </param>
		''' <param name="rc">   Routing context </param>
		Private Sub renderFtl(ByVal file As String, ByVal rc As RoutingContext)
			Dim sessionId As String = rc.request().getParam("sessionID")
			Dim langCode As String = DefaultI18N.getInstance(sessionId).DefaultLanguage
			Dim input As IDictionary(Of String, String) = DefaultI18N.Instance.getMessages(langCode)
			Dim html As String
			Try
				Dim content As String = FileUtils.readFileToString(Resources.asFile("templates/" & file), StandardCharsets.UTF_8)
				Dim template As New Template(FilenameUtils.getName(file), New StringReader(content), configuration)
				Dim stringWriter As New StringWriter()
				template.process(input, stringWriter)
				html = stringWriter.ToString()
			Catch t As Exception
				log.error("", t)
				Throw New Exception(t)
			End Try

			rc.response().end(html)
		End Sub

		''' <summary>
		''' List training sessions. Returns a HTML list of training sessions
		''' </summary>
		Private Sub listSessions(ByVal rc As RoutingContext)
			SyncLock Me
				Dim sb As New StringBuilder("<!DOCTYPE html>" & vbLf & "<html lang=""en"">" & vbLf & "<head>" & vbLf & "        <meta charset=""utf-8"">" & vbLf & "        <title>Training sessions - DL4J Training UI</title>" & vbLf & "    </head>" & vbLf & vbLf & "    <body>" & vbLf & "        <h1>DL4J Training UI</h1>" & vbLf & "        <p>UI server is in multi-session mode." & " To visualize a training session, please select one from the following list.</p>" & vbLf & "        <h2>List of attached training sessions</h2>" & vbLf)
				If knownSessionIDs.Count > 0 Then
					sb.Append("        <ul>")
					For Each sessionId As String In knownSessionIDs.Keys
						sb.Append("            <li><a href=""/train/").Append(sessionId).Append(""">").Append(sessionId).Append("</a></li>" & vbLf)
					Next sessionId
					sb.Append("        </ul>")
				Else
					sb.Append("No training session attached.")
				End If
        
				sb.Append("    </body>" & vbLf & "</html>" & vbLf)
        
				rc.response().putHeader("content-type", "text/html; charset=utf-8").end(sb.ToString())
			End SyncLock
		End Sub

		''' <summary>
		''' Load StatsStorage via provider, or return "not found"
		''' </summary>
		''' <param name="sessionId">  session ID to look fo with provider </param>
		''' <param name="targetPath"> one of overview / model / system, or null </param>
		''' <param name="rc"> routing context </param>
		Private Sub sessionNotFound(ByVal sessionId As String, ByVal targetPath As String, ByVal rc As RoutingContext)
			Dim loader As [Function](Of String, Boolean) = VertxUIServer.getInstance().getStatsStorageLoader()
			If loader IsNot Nothing AndAlso loader.apply(sessionId) Then
				If targetPath IsNot Nothing Then
					rc.reroute(targetPath)
				Else
					rc.response().end()
				End If
			Else
				rc.response().setStatusCode(HttpResponseStatus.NOT_FOUND.code()).end("Unknown session ID: " & sessionId)
			End If
		End Sub

		Public Overridable Sub reportStorageEvents(ByVal events As ICollection(Of StatsStorageEvent))
			SyncLock Me
				For Each sse As StatsStorageEvent In events
					If StatsListener.TYPE_ID.Equals(sse.getTypeID()) Then
						If sse.getEventType() = StatsStorageListener.EventType.PostStaticInfo AndAlso StatsListener.TYPE_ID.Equals(sse.getTypeID()) AndAlso Not knownSessionIDs.ContainsKey(sse.getSessionID()) Then
							knownSessionIDs(sse.getSessionID()) = sse.getStatsStorage()
							If VertxUIServer.getInstance().isMultiSession() Then
								log.info("Adding training session {}/train/{} of StatsStorage instance {}", VertxUIServer.getInstance().getAddress(), sse.getSessionID(), sse.getStatsStorage())
							End If
						End If
        
						Dim lastUpdate As Long? = lastUpdateForSession(sse.getSessionID())
						If lastUpdate Is Nothing Then
							lastUpdateForSession(sse.getSessionID()) = sse.getTimestamp()
						ElseIf sse.getTimestamp() > lastUpdate Then
							lastUpdateForSession(sse.getSessionID()) = sse.getTimestamp() 'Should be thread safe - read only elsewhere
						End If
					End If
				Next sse
        
				If currentSessionID Is Nothing Then
					DefaultSession
				End If
			End SyncLock
		End Sub

		Public Overridable Sub onAttach(ByVal statsStorage As StatsStorage) Implements UIModule.onAttach
			SyncLock Me
				For Each sessionID As String In statsStorage.listSessionIDs()
					For Each typeID As String In statsStorage.listTypeIDsForSession(sessionID)
						If Not StatsListener.TYPE_ID.Equals(typeID) Then
							Continue For
						End If
						knownSessionIDs(sessionID) = statsStorage
						If VertxUIServer.getInstance().isMultiSession() Then
							log.info("Adding training session {}/train/{} of StatsStorage instance {}", VertxUIServer.getInstance().getAddress(), sessionID, statsStorage)
						End If
        
						Dim latestUpdates As IList(Of Persistable) = statsStorage.getLatestUpdateAllWorkers(sessionID, typeID)
						For Each update As Persistable In latestUpdates
							Dim updateTime As Long = update.TimeStamp
							If lastUpdateForSession.ContainsKey(sessionID) AndAlso lastUpdateForSession(sessionID) < updateTime Then
								lastUpdateForSession(sessionID) = updateTime
							End If
						Next update
					Next typeID
				Next sessionID
        
				If currentSessionID Is Nothing Then
					DefaultSession
				End If
			End SyncLock
		End Sub

		Public Overridable Sub onDetach(ByVal statsStorage As StatsStorage) Implements UIModule.onDetach
			SyncLock Me
				Dim toRemove As ISet(Of String) = New HashSet(Of String)()
				For Each s As String In knownSessionIDs.Keys
					If knownSessionIDs(s) Is statsStorage Then
						toRemove.Add(s)
						workerIdxCount.Remove(s)
						workerIdxToName.Remove(s)
						currentSessionID = Nothing
					End If
				Next s
				For Each s As String In toRemove
					knownSessionIDs.Remove(s)
					If VertxUIServer.getInstance().isMultiSession() Then
						log.info("Removing training session {}/train/{} of StatsStorage instance {}.", VertxUIServer.getInstance().getAddress(), s, statsStorage)
					End If
					lastUpdateForSession.Remove(s)
				Next s
				DefaultSession
			End SyncLock
		End Sub

		Private Sub getDefaultSession()
			SyncLock Me
				If currentSessionID IsNot Nothing Then
					Return
				End If
        
				Dim mostRecentTime As Long = Long.MinValue
				Dim sessionID As String = Nothing
				For Each entry As KeyValuePair(Of String, StatsStorage) In knownSessionIDs.SetOfKeyValuePairs()
					Dim staticInfos As IList(Of Persistable) = entry.Value.getAllStaticInfos(entry.Key, StatsListener.TYPE_ID)
					If staticInfos Is Nothing OrElse staticInfos.Count = 0 Then
						Continue For
					End If
					Dim p As Persistable = staticInfos(0)
					Dim thisTime As Long = p.TimeStamp
					If thisTime > mostRecentTime Then
						mostRecentTime = thisTime
						sessionID = entry.Key
					End If
				Next entry
        
				If sessionID IsNot Nothing Then
					currentSessionID = sessionID
				End If
			End SyncLock
		End Sub

		Private Function getWorkerIdForIndex(ByVal sessionId As String, ByVal workerIdx As Integer) As String
			SyncLock Me
				If sessionId Is Nothing Then
					Return Nothing
				End If
        
				Dim idxToId As IDictionary(Of Integer, String) = workerIdxToName.computeIfAbsent(sessionId, Function(k) Collections.synchronizedMap(New Dictionary(Of Integer, String)()))
        
				If idxToId.ContainsKey(workerIdx) Then
					Return idxToId(workerIdx)
				End If
        
				'Need to record new worker...
				'Get counter
				Dim counter As AtomicInteger = workerIdxCount(sessionId)
				If counter Is Nothing Then
					counter = New AtomicInteger(0)
					workerIdxCount(sessionId) = counter
				End If
        
				'Get all worker IDs
				Dim ss As StatsStorage = knownSessionIDs(sessionId)
				If ss Is Nothing Then
					Return Nothing
				End If
				Dim allWorkerIds As IList(Of String) = New List(Of String)(ss.listWorkerIDsForSessionAndType(sessionId, StatsListener.TYPE_ID))
				allWorkerIds.Sort()
        
				'Ensure all workers have been assigned an index
				For Each s As String In allWorkerIds
					If idxToId.ContainsValue(s) Then
						Continue For
					End If
					'Unknown worker ID:
					idxToId(counter.getAndIncrement()) = s
				Next s
        
				'May still return null if index is wrong/too high...
				Return idxToId(workerIdx)
			End SyncLock
		End Function

		''' <summary>
		''' Display, for each session: session ID, start time, number of workers, last update
		''' Returns info for each session as JSON
		''' </summary>
		Private Sub sessionInfo(ByVal rc As RoutingContext)
			SyncLock Me
        
				Dim dataEachSession As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				For Each entry As KeyValuePair(Of String, StatsStorage) In knownSessionIDs.SetOfKeyValuePairs()
					Dim sid As String = entry.Key
					Dim ss As StatsStorage = entry.Value
					Dim dataThisSession As IDictionary(Of String, Object) = sessionData(sid, ss)
					dataEachSession(sid) = dataThisSession
				Next entry
				rc.response().putHeader("content-type", "application/json").end(asJson(dataEachSession))
			End SyncLock
		End Sub

		''' <summary>
		''' Extract session data from <seealso cref="StatsStorage"/>
		''' </summary>
		''' <param name="sid"> session ID </param>
		''' <param name="ss">  {@code StatsStorage} instance </param>
		''' <returns> session data map </returns>
		Private Shared Function sessionData(ByVal sid As String, ByVal ss As StatsStorage) As IDictionary(Of String, Object)
			Dim dataThisSession As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim workerIDs As IList(Of String) = ss.listWorkerIDsForSessionAndType(sid, StatsListener.TYPE_ID)
			Dim workerCount As Integer = (If(workerIDs Is Nothing, 0, workerIDs.Count))
			Dim staticInfo As IList(Of Persistable) = ss.getAllStaticInfos(sid, StatsListener.TYPE_ID)
			Dim initTime As Long = Long.MaxValue
			If staticInfo IsNot Nothing Then
				For Each p As Persistable In staticInfo
					initTime = Math.Min(p.TimeStamp, initTime)
				Next p
			End If

			Dim lastUpdateTime As Long = Long.MinValue
			Dim lastUpdatesAllWorkers As IList(Of Persistable) = ss.getLatestUpdateAllWorkers(sid, StatsListener.TYPE_ID)
			For Each p As Persistable In lastUpdatesAllWorkers
				lastUpdateTime = Math.Max(lastUpdateTime, p.TimeStamp)
			Next p

			dataThisSession("numWorkers") = workerCount
			dataThisSession("initTime") = If(initTime = Long.MaxValue, "", initTime)
			dataThisSession("lastUpdate") = If(lastUpdateTime = Long.MinValue, "", lastUpdateTime)

			' add hashmap of workers
			If workerCount > 0 Then
				dataThisSession("workers") = workerIDs
			End If

			'Model info: type, # layers, # params...
			If staticInfo IsNot Nothing AndAlso staticInfo.Count > 0 Then
				Dim sr As StatsInitializationReport = DirectCast(staticInfo(0), StatsInitializationReport)
				Dim modelClassName As String = sr.ModelClassName
				If modelClassName.EndsWith("MultiLayerNetwork", StringComparison.Ordinal) Then
					modelClassName = "MultiLayerNetwork"
				ElseIf modelClassName.EndsWith("ComputationGraph", StringComparison.Ordinal) Then
					modelClassName = "ComputationGraph"
				End If
				Dim numLayers As Integer = sr.ModelNumLayers
				Dim numParams As Long = sr.ModelNumParams

				dataThisSession("modelType") = modelClassName
				dataThisSession("numLayers") = numLayers
				dataThisSession("numParams") = numParams
			Else
				dataThisSession("modelType") = ""
				dataThisSession("numLayers") = ""
				dataThisSession("numParams") = ""
			End If
			Return dataThisSession
		End Function

		''' <summary>
		''' Display, for given session: session ID, start time, number of workers, last update.
		''' Returns info for session as JSON
		''' </summary>
		''' <param name="sessionId"> session ID </param>
		Private Sub sessionInfoForSession(ByVal sessionId As String, ByVal rc As RoutingContext)
			SyncLock Me
        
				Dim dataEachSession As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				Dim ss As StatsStorage = knownSessionIDs(sessionId)
				If ss IsNot Nothing Then
					Dim dataThisSession As IDictionary(Of String, Object) = sessionData(sessionId, ss)
					dataEachSession(sessionId) = dataThisSession
				End If
				rc.response().putHeader("content-type", "application/json").end(asJson(dataEachSession))
			End SyncLock
		End Sub

		Private Sub setSession(ByVal newSessionID As String, ByVal rc As RoutingContext)
			SyncLock Me
				If knownSessionIDs.ContainsKey(newSessionID) Then
					currentSessionID = newSessionID
					currentWorkerIdx = 0
					rc.response().end()
				Else
					rc.response().setStatusCode(HttpResponseStatus.BAD_REQUEST.code()).end()
				End If
			End SyncLock
		End Sub

		Private Sub getLastUpdateForSession(ByVal sessionID As String, ByVal rc As RoutingContext)
			Dim lastUpdate As Long? = lastUpdateForSession(sessionID)
			If lastUpdate IsNot Nothing Then
				rc.response().end(lastUpdate.ToString())
				Return
			End If
			rc.response().end("-1")
		End Sub

		Private Sub setWorkerByIdx(ByVal newWorkerIdx As String, ByVal rc As RoutingContext)
			Try
				currentWorkerIdx = Integer.Parse(newWorkerIdx)
			Catch e As System.FormatException
				log.debug("Invalid call to setWorkerByIdx", e)
			End Try
			rc.response().end()
		End Sub

		Private Shared Function fixNaN(ByVal d As Double) As Double
			Return If(Double.isFinite(d), d, NAN_REPLACEMENT_VALUE)
		End Function

		Private Shared Sub cleanLegacyIterationCounts(ByVal iterationCounts As IList(Of Integer))
			If iterationCounts.Count > 0 Then
				Dim allEqual As Boolean = True
				Dim maxStepSize As Integer = 1
				Dim first As Integer = iterationCounts(0)
				Dim length As Integer = iterationCounts.Count
				Dim prevIterCount As Integer = first
				For i As Integer = 1 To length - 1
					Dim currIterCount As Integer = iterationCounts(i)
					If allEqual AndAlso first <> currIterCount Then
						allEqual = False
					End If
					maxStepSize = Math.Max(maxStepSize, prevIterCount - currIterCount)
					prevIterCount = currIterCount
				Next i


				If allEqual Then
					maxStepSize = 1
				End If

				For i As Integer = 0 To length - 1
					iterationCounts(i) = first + i * maxStepSize
				Next i
			End If
		End Sub

		''' <summary>
		''' Get last update time for given session ID, checking for null values
		''' </summary>
		''' <param name="sessionId"> session ID </param>
		''' <returns> last update time for session if found, or {@code null} </returns>
		Private Function getLastUpdateTime(ByVal sessionId As String) As Long?
			If lastUpdateForSession IsNot Nothing AndAlso sessionId IsNot Nothing AndAlso lastUpdateForSession.ContainsKey(sessionId) Then
				Return lastUpdateForSession(sessionId)
			Else
				Return -1L
			End If
		End Function

		''' <summary>
		''' Get global <seealso cref="I18N"/> instance if <seealso cref="VertxUIServer.isMultiSession()"/> is {@code true}, or instance for session
		''' </summary>
		''' <param name="sessionId"> session ID </param>
		''' <returns> <seealso cref="I18N"/> instance </returns>
		Private Function getI18N(ByVal sessionId As String) As I18N
			Return If(VertxUIServer.getInstance().isMultiSession(), I18NProvider.getInstance(sessionId), I18NProvider.Instance)
		End Function


		Private Sub getOverviewData(ByVal rc As RoutingContext)
			getOverviewDataForSession(currentSessionID, rc)
		End Sub

		Private Sub getOverviewDataForSession(ByVal sessionId As String, ByVal rc As RoutingContext)
			SyncLock Me
				Dim lastUpdateTime As Long? = getLastUpdateTime(sessionId)
				Dim i18N As I18N = getI18N(sessionId)
        
				'First pass (optimize later): query all data...
				Dim ss As StatsStorage = (If(sessionId Is Nothing, Nothing, knownSessionIDs(sessionId)))
				Dim wid As String = getWorkerIdForIndex(sessionId, currentWorkerIdx)
				Dim noData As Boolean = (sessionId Is Nothing) OrElse (ss Is Nothing) OrElse (wid Is Nothing)
        
				Dim scoresIterCount As IList(Of Integer) = New List(Of Integer)()
				Dim scores As IList(Of Double) = New List(Of Double)()
        
				Dim result As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				result("updateTimestamp") = lastUpdateTime
				result("scores") = scores
				result("scoresIter") = scoresIterCount
        
				'Get scores info
				Dim allTimes() As Long = (If(noData, Nothing, ss.getAllUpdateTimes(sessionId, StatsListener.TYPE_ID, wid)))
				Dim updates As IList(Of Persistable) = Nothing
				If allTimes IsNot Nothing AndAlso allTimes.Length > maxChartPoints Then
					Dim subsamplingFrequency As Integer = allTimes.Length \ maxChartPoints
					Dim timesToQuery As New LongArrayList(maxChartPoints + 2)
					Dim i As Integer = 0
					Do While i < allTimes.Length
						timesToQuery.add(allTimes(i))
						i += subsamplingFrequency
					Loop
					If (i - subsamplingFrequency) <> allTimes.Length - 1 Then
						'Also add final point
						timesToQuery.add(allTimes(allTimes.Length - 1))
					End If
					updates = ss.getUpdates(sessionId, StatsListener.TYPE_ID, wid, timesToQuery.toLongArray())
				ElseIf allTimes IsNot Nothing Then
					'Don't subsample
					updates = ss.getAllUpdatesAfter(sessionId, StatsListener.TYPE_ID, wid, 0)
				End If
				If updates Is Nothing OrElse updates.Count = 0 Then
					noData = True
				End If
        
				'Collect update ratios for weights
				'Collect standard deviations: activations, gradients, updates
				Dim updateRatios As IDictionary(Of String, IList(Of Double)) = New Dictionary(Of String, IList(Of Double))() 'Mean magnitude (updates) / mean magnitude (parameters)
				result("updateRatios") = updateRatios
        
				Dim stdevActivations As IDictionary(Of String, IList(Of Double)) = New Dictionary(Of String, IList(Of Double))()
				Dim stdevGradients As IDictionary(Of String, IList(Of Double)) = New Dictionary(Of String, IList(Of Double))()
				Dim stdevUpdates As IDictionary(Of String, IList(Of Double)) = New Dictionary(Of String, IList(Of Double))()
				result("stdevActivations") = stdevActivations
				result("stdevGradients") = stdevGradients
				result("stdevUpdates") = stdevUpdates
        
				If Not noData Then
					Dim u As Persistable = updates(0)
					If TypeOf u Is StatsReport Then
						Dim sp As StatsReport = DirectCast(u, StatsReport)
						Dim map As IDictionary(Of String, Double) = sp.getMeanMagnitudes(StatsType.Parameters)
						If map IsNot Nothing Then
							For Each s As String In map.Keys
								If Not s.ToLower().EndsWith("w", StringComparison.Ordinal) Then
									Continue For 'TODO: more robust "weights only" approach...
								End If
								updateRatios(s) = New List(Of Double)()
							Next s
						End If
        
						Dim stdGrad As IDictionary(Of String, Double) = sp.getStdev(StatsType.Gradients)
						If stdGrad IsNot Nothing Then
							For Each s As String In stdGrad.Keys
								If Not s.ToLower().EndsWith("w", StringComparison.Ordinal) Then
									Continue For 'TODO: more robust "weights only" approach...
								End If
								stdevGradients(s) = New List(Of Double)()
							Next s
						End If
        
						Dim stdUpdate As IDictionary(Of String, Double) = sp.getStdev(StatsType.Updates)
						If stdUpdate IsNot Nothing Then
							For Each s As String In stdUpdate.Keys
								If Not s.ToLower().EndsWith("w", StringComparison.Ordinal) Then
									Continue For 'TODO: more robust "weights only" approach...
								End If
								stdevUpdates(s) = New List(Of Double)()
							Next s
						End If
        
        
						Dim stdAct As IDictionary(Of String, Double) = sp.getStdev(StatsType.Activations)
						If stdAct IsNot Nothing Then
							For Each s As String In stdAct.Keys
								stdevActivations(s) = New List(Of Double)()
							Next s
						End If
					End If
				End If
        
				Dim last As StatsReport = Nothing
				Dim lastIterCount As Integer = -1
				'Legacy issue - Spark training - iteration counts are used to be reset... which means: could go 0,1,2,0,1,2, etc...
				'Or, it could equally go 4,8,4,8,... or 5,5,5,5 - depending on the collection and averaging frequencies
				'Now, it should use the proper iteration counts
				Dim needToHandleLegacyIterCounts As Boolean = False
				If Not noData Then
					Dim lastScore As Double
        
					Dim totalUpdates As Integer = updates.Count
					Dim subsamplingFrequency As Integer = 1
					If totalUpdates > maxChartPoints Then
						subsamplingFrequency = totalUpdates \ maxChartPoints
					End If
        
					Dim pCount As Integer = -1
					Dim lastUpdateIdx As Integer = updates.Count - 1
					For Each u As Persistable In updates
						pCount += 1
						If Not (TypeOf u Is StatsReport) Then
							Continue For
						End If
        
						last = DirectCast(u, StatsReport)
						Dim iterCount As Integer = last.IterationCount
        
						If iterCount <= lastIterCount Then
							needToHandleLegacyIterCounts = True
						End If
						lastIterCount = iterCount
        
						If pCount > 0 AndAlso subsamplingFrequency > 1 AndAlso pCount Mod subsamplingFrequency <> 0 Then
							'Skip this - subsample the data
							If pCount <> lastUpdateIdx Then
								Continue For 'Always keep the most recent value
							End If
						End If
        
						scoresIterCount.Add(iterCount)
						lastScore = last.Score
						If Double.isFinite(lastScore) Then
							scores.Add(lastScore)
						Else
							scores.Add(NAN_REPLACEMENT_VALUE)
						End If
        
        
						'Update ratios: mean magnitudes(updates) / mean magnitudes (parameters)
						Dim updateMM As IDictionary(Of String, Double) = last.getMeanMagnitudes(StatsType.Updates)
						Dim paramMM As IDictionary(Of String, Double) = last.getMeanMagnitudes(StatsType.Parameters)
						If updateMM IsNot Nothing AndAlso paramMM IsNot Nothing AndAlso updateMM.Count > 0 AndAlso paramMM.Count > 0 Then
							For Each s As String In updateRatios.Keys
								Dim ratioHistory As IList(Of Double) = updateRatios(s)
								Dim currUpdate As Double = updateMM.GetOrDefault(s, 0.0)
								Dim currParam As Double = paramMM.GetOrDefault(s, 0.0)
								Dim ratio As Double = currUpdate / currParam
								If Double.isFinite(ratio) Then
									ratioHistory.Add(ratio)
								Else
									ratioHistory.Add(NAN_REPLACEMENT_VALUE)
								End If
							Next s
						End If
        
						'Standard deviations: gradients, updates, activations
						Dim stdGrad As IDictionary(Of String, Double) = last.getStdev(StatsType.Gradients)
						Dim stdUpd As IDictionary(Of String, Double) = last.getStdev(StatsType.Updates)
						Dim stdAct As IDictionary(Of String, Double) = last.getStdev(StatsType.Activations)
        
						If stdGrad IsNot Nothing Then
							For Each s As String In stdevGradients.Keys
								Dim d As Double = stdGrad.GetOrDefault(s, 0.0)
								stdevGradients(s).Add(fixNaN(d))
							Next s
						End If
						If stdUpd IsNot Nothing Then
							For Each s As String In stdevUpdates.Keys
								Dim d As Double = stdUpd.GetOrDefault(s, 0.0)
								stdevUpdates(s).Add(fixNaN(d))
							Next s
						End If
						If stdAct IsNot Nothing Then
							For Each s As String In stdevActivations.Keys
								Dim d As Double = stdAct.GetOrDefault(s, 0.0)
								stdevActivations(s).Add(fixNaN(d))
							Next s
						End If
					Next u
				End If
        
				If needToHandleLegacyIterCounts Then
					cleanLegacyIterationCounts(scoresIterCount)
				End If
        
        
				'----- Performance Info -----
				Dim perfInfo()() As String = {
					New String() {i18N.getMessage("train.overview.perftable.startTime"), ""},
					New String() {i18N.getMessage("train.overview.perftable.totalRuntime"), ""},
					New String() {i18N.getMessage("train.overview.perftable.lastUpdate"), ""},
					New String() {i18N.getMessage("train.overview.perftable.totalParamUpdates"), ""},
					New String() {i18N.getMessage("train.overview.perftable.updatesPerSec"), ""},
					New String() {i18N.getMessage("train.overview.perftable.examplesPerSec"), ""}
				}
        
				If last IsNot Nothing Then
					perfInfo(2)(1) = dateFormat.format(New DateTime(last.TimeStamp)).ToString()
					perfInfo(3)(1) = last.TotalMinibatches.ToString()
					perfInfo(4)(1) = df2.format(last.MinibatchesPerSecond).ToString()
					perfInfo(5)(1) = df2.format(last.ExamplesPerSecond).ToString()
				End If
        
				result("perf") = perfInfo
        
        
				' ----- Model Info -----
				Dim modelInfo()() As String = {
					New String() {i18N.getMessage("train.overview.modeltable.modeltype"), ""},
					New String() {i18N.getMessage("train.overview.modeltable.nLayers"), ""},
					New String() {i18N.getMessage("train.overview.modeltable.nParams"), ""}
				}
				If Not noData Then
					Dim p As Persistable = ss.getStaticInfo(sessionId, StatsListener.TYPE_ID, wid)
					If p IsNot Nothing Then
						Dim initReport As StatsInitializationReport = DirectCast(p, StatsInitializationReport)
						Dim nLayers As Integer = initReport.ModelNumLayers
						Dim numParams As Long = initReport.ModelNumParams
						Dim className As String = initReport.ModelClassName
        
						Dim modelType As String
						If className.EndsWith("MultiLayerNetwork", StringComparison.Ordinal) Then
							modelType = "MultiLayerNetwork"
						ElseIf className.EndsWith("ComputationGraph", StringComparison.Ordinal) Then
							modelType = "ComputationGraph"
						Else
							modelType = className
							If modelType.LastIndexOf("."c) > 0 Then
								modelType = modelType.Substring(modelType.LastIndexOf("."c) + 1)
							End If
						End If
        
						modelInfo(0)(1) = modelType
						modelInfo(1)(1) = nLayers.ToString()
						modelInfo(2)(1) = numParams.ToString()
					End If
				End If
        
				result("model") = modelInfo
        
				Dim json As String = asJson(result)
        
				rc.response().putHeader("content-type", "application/json").end(json)
			End SyncLock
		End Sub

		Private Sub getModelGraph(ByVal rc As RoutingContext)
			getModelGraphForSession(currentSessionID, rc)
		End Sub

		Private Sub getModelGraphForSession(ByVal sessionId As String, ByVal rc As RoutingContext)

			Dim noData As Boolean = (sessionId Is Nothing OrElse Not knownSessionIDs.ContainsKey(sessionId))
			Dim ss As StatsStorage = (If(noData, Nothing, knownSessionIDs(sessionId)))
			Dim allStatic As IList(Of Persistable) = (If(noData, Collections.EMPTY_LIST, ss.getAllStaticInfos(sessionId, StatsListener.TYPE_ID)))

			If allStatic.Count = 0 Then
				rc.response().end()
				Return
			End If

			Dim gi As TrainModuleUtils.GraphInfo = getGraphInfo(getConfig(sessionId))
			If gi Is Nothing Then
				rc.response().end()
				Return
			End If

			Dim json As String = asJson(gi)

			rc.response().putHeader("content-type", "application/json").end(json)
		End Sub

		Private Function getGraphInfo(ByVal conf As Triple(Of MultiLayerConfiguration, ComputationGraphConfiguration, NeuralNetConfiguration)) As TrainModuleUtils.GraphInfo
			If conf Is Nothing Then
				Return Nothing
			End If

			If conf.getFirst() IsNot Nothing Then
				Return TrainModuleUtils.buildGraphInfo(conf.getFirst())
			ElseIf conf.getSecond() IsNot Nothing Then
				Return TrainModuleUtils.buildGraphInfo(conf.getSecond())
			ElseIf conf.getThird() IsNot Nothing Then
				Return TrainModuleUtils.buildGraphInfo(conf.getThird())
			Else
				Return Nothing
			End If
		End Function

		Private Function getConfig(ByVal sessionId As String) As Triple(Of MultiLayerConfiguration, ComputationGraphConfiguration, NeuralNetConfiguration)
			Dim noData As Boolean = (sessionId Is Nothing OrElse Not knownSessionIDs.ContainsKey(sessionId))
			Dim ss As StatsStorage = (If(noData, Nothing, knownSessionIDs(sessionId)))
			Dim allStatic As IList(Of Persistable) = (If(noData, Collections.EMPTY_LIST, ss.getAllStaticInfos(sessionId, StatsListener.TYPE_ID)))
			If allStatic.Count = 0 Then
				Return Nothing
			End If

			Dim p As StatsInitializationReport = DirectCast(allStatic(0), StatsInitializationReport)
			Dim modelClass As String = p.ModelClassName
			Dim config As String = p.ModelConfigJson

			If modelClass.EndsWith("MultiLayerNetwork", StringComparison.Ordinal) Then
				Dim conf As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(config)
				Return New Triple(Of MultiLayerConfiguration, ComputationGraphConfiguration, NeuralNetConfiguration)(conf, Nothing, Nothing)
			ElseIf modelClass.EndsWith("ComputationGraph", StringComparison.Ordinal) Then
				Dim conf As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(config)
				Return New Triple(Of MultiLayerConfiguration, ComputationGraphConfiguration, NeuralNetConfiguration)(Nothing, conf, Nothing)
			Else
				Try
					Dim layer As NeuralNetConfiguration = NeuralNetConfiguration.mapper().readValue(config, GetType(NeuralNetConfiguration))
					Return New Triple(Of MultiLayerConfiguration, ComputationGraphConfiguration, NeuralNetConfiguration)(Nothing, Nothing, layer)
				Catch e As Exception
					log.error("",e)
				End Try
			End If
			Return Nothing
		End Function


		Private Sub getModelData(ByVal layerId As String, ByVal rc As RoutingContext)
			getModelDataForSession(currentSessionID, layerId, rc)
		End Sub

		Private Sub getModelDataForSession(ByVal sessionId As String, ByVal layerId As String, ByVal rc As RoutingContext)
			Dim lastUpdateTime As Long? = getLastUpdateTime(sessionId)

			Dim layerIdx As Integer = Integer.Parse(layerId) 'TODO validation
			Dim i18N As I18N = getI18N(sessionId)

			'Model info for layer

			'First pass (optimize later): query all data...
			Dim ss As StatsStorage = (If(sessionId Is Nothing, Nothing, knownSessionIDs(sessionId)))
			Dim wid As String = getWorkerIdForIndex(sessionId, currentWorkerIdx)
			Dim noData As Boolean = (sessionId Is Nothing) OrElse (ss Is Nothing) OrElse (wid Is Nothing)

			Dim result As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			result("updateTimestamp") = lastUpdateTime

			Dim conf As Triple(Of MultiLayerConfiguration, ComputationGraphConfiguration, NeuralNetConfiguration) = getConfig(sessionId)
			If conf Is Nothing Then
				rc.response().putHeader("content-type", "application/json").end(asJson(result))
				Return
			End If

			Dim gi As TrainModuleUtils.GraphInfo = getGraphInfo(conf)
			If gi Is Nothing Then
				rc.response().putHeader("content-type", "application/json").end(asJson(result))
				Return
			End If


			' Get static layer info
			Dim layerInfoTable()() As String = getLayerInfoTable(sessionId, layerIdx, gi, i18N, noData, ss, wid)

			result("layerInfo") = layerInfoTable

			'First: get all data, and subsample it if necessary, to avoid returning too many points...
			Dim allTimes() As Long = (If(noData, Nothing, ss.getAllUpdateTimes(sessionId, StatsListener.TYPE_ID, wid)))

			Dim updates As IList(Of Persistable) = Nothing
			Dim iterationCounts As IList(Of Integer) = Nothing
			Dim needToHandleLegacyIterCounts As Boolean = False
			If allTimes IsNot Nothing AndAlso allTimes.Length > maxChartPoints Then
				Dim subsamplingFrequency As Integer = allTimes.Length \ maxChartPoints
				Dim timesToQuery As New LongArrayList(maxChartPoints + 2)
				Dim i As Integer = 0
				Do While i < allTimes.Length
					timesToQuery.add(allTimes(i))
					i += subsamplingFrequency
				Loop
				If (i - subsamplingFrequency) <> allTimes.Length - 1 Then
					'Also add final point
					timesToQuery.add(allTimes(allTimes.Length - 1))
				End If
				updates = ss.getUpdates(sessionId, StatsListener.TYPE_ID, wid, timesToQuery.toLongArray())
			ElseIf allTimes IsNot Nothing Then
				'Don't subsample
				updates = ss.getAllUpdatesAfter(sessionId, StatsListener.TYPE_ID, wid, 0)
			End If

			iterationCounts = New List(Of Integer)(updates.Count)
			Dim lastIterCount As Integer = -1
			For Each p As Persistable In updates
				If Not (TypeOf p Is StatsReport) Then
					Continue For
				End If
				Dim sr As StatsReport = DirectCast(p, StatsReport)
				Dim iterCount As Integer = sr.IterationCount

				If iterCount <= lastIterCount Then
					needToHandleLegacyIterCounts = True
				End If
				iterationCounts.Add(iterCount)
			Next p

			'Legacy issue - Spark training - iteration counts are used to be reset... which means: could go 0,1,2,0,1,2, etc...
			'Or, it could equally go 4,8,4,8,... or 5,5,5,5 - depending on the collection and averaging frequencies
			'Now, it should use the proper iteration counts
			If needToHandleLegacyIterCounts Then
				cleanLegacyIterationCounts(iterationCounts)
			End If

			'Get mean magnitudes line chart
			Dim mt As ModelType
			If conf.getFirst() IsNot Nothing Then
				mt = ModelType.MLN
			ElseIf conf.getSecond() IsNot Nothing Then
				mt = ModelType.CG
			Else
				mt = ModelType.Layer
			End If
			Dim mm As MeanMagnitudes = getLayerMeanMagnitudes(layerIdx, gi, updates, iterationCounts, mt)
			Dim mmRatioMap As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			mmRatioMap("layerParamNames") = mm.getRatios().keySet()
			mmRatioMap("iterCounts") = mm.getIterations()
			mmRatioMap("ratios") = mm.getRatios()
			mmRatioMap("paramMM") = mm.getParamMM()
			mmRatioMap("updateMM") = mm.getUpdateMM()
			result("meanMag") = mmRatioMap

			'Get activations line chart for layer
			Dim activationsData As Triple(Of Integer(), Single(), Single()) = getLayerActivations(layerIdx, gi, updates, iterationCounts)
			Dim activationMap As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			activationMap("iterCount") = activationsData.getFirst()
			activationMap("mean") = activationsData.getSecond()
			activationMap("stdev") = activationsData.getThird()
			result("activations") = activationMap

			'Get learning rate vs. time chart for layer
			Dim lrs As IDictionary(Of String, Object) = getLayerLearningRates(layerIdx, gi, updates, iterationCounts, mt)
			result("learningRates") = lrs

			'Parameters histogram data
			Dim lastUpdate As Persistable = (If(updates IsNot Nothing AndAlso updates.Count > 0, updates(updates.Count - 1), Nothing))
			Dim paramHistograms As IDictionary(Of String, Object) = getHistograms(layerIdx, gi, StatsType.Parameters, lastUpdate)
			result("paramHist") = paramHistograms

			'Updates histogram data
			Dim updateHistograms As IDictionary(Of String, Object) = getHistograms(layerIdx, gi, StatsType.Updates, lastUpdate)
			result("updateHist") = updateHistograms

			rc.response().putHeader("content-type", "application/json").end(asJson(result))
		End Sub

		Private Sub getSystemData(ByVal rc As RoutingContext)
			getSystemDataForSession(currentSessionID, rc)
		End Sub

		Private Sub getSystemDataForSession(ByVal sessionId As String, ByVal rc As RoutingContext)
			Dim lastUpdate As Long? = getLastUpdateTime(sessionId)

			Dim i18n As I18N = getI18N(sessionId)

			'First: get the MOST RECENT update...
			'Then get all updates from most recent - 5 minutes -> TODO make this configurable...

			Dim ss As StatsStorage = (If(sessionId Is Nothing, Nothing, knownSessionIDs(sessionId)))
			Dim noData As Boolean = (ss Is Nothing)

			Dim allStatic As IList(Of Persistable) = (If(noData, Collections.EMPTY_LIST, ss.getAllStaticInfos(sessionId, StatsListener.TYPE_ID)))
			Dim latestUpdates As IList(Of Persistable) = (If(noData, Collections.EMPTY_LIST, ss.getLatestUpdateAllWorkers(sessionId, StatsListener.TYPE_ID)))


			Dim lastUpdateTime As Long = -1
			If latestUpdates Is Nothing OrElse latestUpdates.Count = 0 Then
				noData = True
			Else
				For Each p As Persistable In latestUpdates
					lastUpdateTime = Math.Max(lastUpdateTime, p.TimeStamp)
				Next p
			End If

			Dim fromTime As Long = lastUpdateTime - 5 * 60 * 1000 'TODO Make configurable
			Dim lastNMinutes As IList(Of Persistable) = (If(noData, Nothing, ss.getAllUpdatesAfter(sessionId, StatsListener.TYPE_ID, fromTime)))

			Dim mem As IDictionary(Of String, Object) = getMemory(allStatic, lastNMinutes, i18n)
			Dim hwSwInfo As Pair(Of IDictionary(Of String, Object), IDictionary(Of String, Object)) = getHardwareSoftwareInfo(allStatic, i18n)

			Dim ret As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			ret("updateTimestamp") = lastUpdate
			ret("memory") = mem
			ret("hardware") = hwSwInfo.First
			ret("software") = hwSwInfo.Second

			rc.response().putHeader("content-type", "application/json").end(asJson(ret))
		End Sub

		Private Shared Function getLayerType(ByVal layer As Layer) As String
			Dim layerType As String = "n/a"
			If layer IsNot Nothing Then
				Try
					layerType = layer.GetType().Name.replaceAll("Layer$", "")
				Catch e As Exception
				End Try
			End If
			Return layerType
		End Function

		Private Shared Function getLayerInfoTable(ByVal sessionId As String, ByVal layerIdx As Integer, ByVal gi As TrainModuleUtils.GraphInfo, ByVal i18N As I18N, ByVal noData As Boolean, ByVal ss As StatsStorage, ByVal wid As String) As String()()
			Dim layerInfoRows As IList(Of String()) = New List(Of String())()
			layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerName"), gi.getVertexNames().get(layerIdx)})
			layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerType"), ""})

			If Not noData Then
				Dim p As Persistable = ss.getStaticInfo(sessionId, StatsListener.TYPE_ID, wid)
				If p IsNot Nothing Then
					Dim initReport As StatsInitializationReport = DirectCast(p, StatsInitializationReport)
					Dim configJson As String = initReport.ModelConfigJson
					Dim modelClass As String = initReport.ModelClassName

					'TODO error handling...
					Dim layerType As String = ""
					Dim layer As Layer = Nothing
					Dim nnc As NeuralNetConfiguration = Nothing
					If modelClass.EndsWith("MultiLayerNetwork", StringComparison.Ordinal) Then
						Dim conf As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(configJson)
						Dim confIdx As Integer = layerIdx - 1 '-1 because of input
						If confIdx >= 0 Then
							nnc = conf.getConf(confIdx)
							layer = nnc.getLayer()
						Else
							'Input layer
							layerType = "Input"
						End If
					ElseIf modelClass.EndsWith("ComputationGraph", StringComparison.Ordinal) Then
						Dim conf As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(configJson)

						Dim vertexName As String = gi.getVertexNames().get(layerIdx)

						Dim vertices As IDictionary(Of String, GraphVertex) = conf.getVertices()
						If vertices.ContainsKey(vertexName) AndAlso TypeOf vertices(vertexName) Is LayerVertex Then
							Dim lv As LayerVertex = DirectCast(vertices(vertexName), LayerVertex)
							nnc = lv.getLayerConf()
							layer = nnc.getLayer()
						ElseIf conf.getNetworkInputs().contains(vertexName) Then
							layerType = "Input"
						Else
							Dim gv As GraphVertex = conf.getVertices().get(vertexName)
							If gv IsNot Nothing Then
								layerType = gv.GetType().Name
							End If
						End If
					ElseIf modelClass.EndsWith("VariationalAutoencoder", StringComparison.Ordinal) Then
						layerType = gi.getVertexTypes().get(layerIdx)
						Dim map As IDictionary(Of String, String) = gi.getVertexInfo().get(layerIdx)
						For Each entry As KeyValuePair(Of String, String) In map.SetOfKeyValuePairs()
							layerInfoRows.Add(New String(){entry.Key, entry.Value})
						Next entry
					End If

					If layer IsNot Nothing Then
						layerType = getLayerType(layer)
					End If

					If layer IsNot Nothing Then
						Dim activationFn As String = Nothing
						If TypeOf layer Is FeedForwardLayer Then
							Dim ffl As FeedForwardLayer = DirectCast(layer, FeedForwardLayer)
							layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerNIn"), ffl.getNIn().ToString()})
							layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerSize"), ffl.getNOut().ToString()})
						End If
						If TypeOf layer Is BaseLayer Then
							Dim bl As BaseLayer = DirectCast(layer, BaseLayer)
							activationFn = bl.getActivationFn().ToString()
							Dim nParams As Long = layer.initializer().numParams(nnc)
							layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerNParams"), nParams.ToString()})
							If nParams > 0 Then
								Try
									Dim str As String = JsonMappers.Mapper.writeValueAsString(bl.getWeightInitFn())
									layerInfoRows.Add(New String(){ i18N.getMessage("train.model.layerinfotable.layerWeightInit"), str})
								Catch e As JsonProcessingException
									Throw New Exception(e)
								End Try

								Dim u As IUpdater = bl.getIUpdater()
								Dim us As String = (If(u Is Nothing, "", u.GetType().Name))
								layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerUpdater"), us})

								'TODO: Maybe L1/L2, dropout, updater-specific values etc
							End If
						End If

						If TypeOf layer Is ConvolutionLayer OrElse TypeOf layer Is SubsamplingLayer Then
							Dim kernel() As Integer
							Dim stride() As Integer
							Dim padding() As Integer
							If TypeOf layer Is ConvolutionLayer Then
								Dim cl As ConvolutionLayer = DirectCast(layer, ConvolutionLayer)
								kernel = cl.getKernelSize()
								stride = cl.getStride()
								padding = cl.getPadding()
							Else
								Dim ssl As SubsamplingLayer = DirectCast(layer, SubsamplingLayer)
								kernel = ssl.getKernelSize()
								stride = ssl.getStride()
								padding = ssl.getPadding()
								activationFn = Nothing
								layerInfoRows.Add(New String(){ i18N.getMessage("train.model.layerinfotable.layerSubsamplingPoolingType"), ssl.getPoolingType().ToString()})
							End If
							layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerCnnKernel"), java.util.Arrays.toString(kernel)})
							layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerCnnStride"), java.util.Arrays.toString(stride)})
							layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerCnnPadding"), java.util.Arrays.toString(padding)})
						End If

						If activationFn IsNot Nothing Then
							layerInfoRows.Add(New String(){i18N.getMessage("train.model.layerinfotable.layerActivationFn"), activationFn})
						End If
					End If
					layerInfoRows(1)(1) = layerType
				End If
			End If

			Return CType(layerInfoRows, List(Of String())).ToArray()
		End Function

		'TODO float precision for smaller transfers?
		'First: iteration. Second: ratios, by parameter
		Private Shared Function getLayerMeanMagnitudes(ByVal layerIdx As Integer, ByVal gi As TrainModuleUtils.GraphInfo, ByVal updates As IList(Of Persistable), ByVal iterationCounts As IList(Of Integer), ByVal modelType As ModelType) As MeanMagnitudes
			If gi Is Nothing Then
				Return New MeanMagnitudes(java.util.Collections.emptyList(), java.util.Collections.emptyMap(), java.util.Collections.emptyMap(), java.util.Collections.emptyMap())
			End If

			Dim layerName As String = gi.getVertexNames().get(layerIdx)
			If modelType <> ModelType.CG Then
				'Get the original name, for the index...
				layerName = gi.getOriginalVertexName().get(layerIdx)
			End If
			Dim layerType As String = gi.getVertexTypes().get(layerIdx)
			If "input".Equals(layerType, StringComparison.OrdinalIgnoreCase) Then 'TODO better checking - other vertices, etc
				Return New MeanMagnitudes(java.util.Collections.emptyList(), java.util.Collections.emptyMap(), java.util.Collections.emptyMap(), java.util.Collections.emptyMap())
			End If

			Dim iterCounts As IList(Of Integer) = New List(Of Integer)()
			Dim ratioValues As IDictionary(Of String, IList(Of Double)) = New Dictionary(Of String, IList(Of Double))()
			Dim outParamMM As IDictionary(Of String, IList(Of Double)) = New Dictionary(Of String, IList(Of Double))()
			Dim outUpdateMM As IDictionary(Of String, IList(Of Double)) = New Dictionary(Of String, IList(Of Double))()

			If updates IsNot Nothing Then
				Dim pCount As Integer = -1
				For Each u As Persistable In updates
					pCount += 1
					If Not (TypeOf u Is StatsReport) Then
						Continue For
					End If
					Dim sp As StatsReport = DirectCast(u, StatsReport)
					If iterationCounts IsNot Nothing Then
						iterCounts.Add(iterationCounts(pCount))
					Else
						Dim iterCount As Integer = sp.IterationCount
						iterCounts.Add(iterCount)
					End If


					'Info we want, for each parameter in this layer: mean magnitudes for parameters, updates AND the ratio of these
					Dim paramMM As IDictionary(Of String, Double) = sp.getMeanMagnitudes(StatsType.Parameters)
					Dim updateMM As IDictionary(Of String, Double) = sp.getMeanMagnitudes(StatsType.Updates)
					For Each s As String In paramMM.Keys
						Dim prefix As String
						If modelType = ModelType.Layer Then
							prefix = layerName
						Else
							prefix = layerName & "_"
						End If

						If s.StartsWith(prefix, StringComparison.Ordinal) Then
							'Relevant parameter for this layer...
							Dim layerParam As String = s.Substring(prefix.Length)
							Dim pmm As Double = paramMM.GetOrDefault(s, 0.0)
							Dim umm As Double = updateMM.GetOrDefault(s, 0.0)
							If Not Double.isFinite(pmm) Then
								pmm = NAN_REPLACEMENT_VALUE
							End If
							If Not Double.isFinite(umm) Then
								umm = NAN_REPLACEMENT_VALUE
							End If
							Dim ratio As Double
							If umm = 0.0 AndAlso pmm = 0.0 Then
								ratio = 0.0 'To avoid NaN from 0/0
							Else
								ratio = umm / pmm
							End If
							Dim list As IList(Of Double) = ratioValues(layerParam)
							If list Is Nothing Then
								list = New List(Of Double)()
								ratioValues(layerParam) = list
							End If
							list.Add(ratio)

							Dim pmmList As IList(Of Double) = outParamMM(layerParam)
							If pmmList Is Nothing Then
								pmmList = New List(Of Double)()
								outParamMM(layerParam) = pmmList
							End If
							pmmList.Add(pmm)

							Dim ummList As IList(Of Double) = outUpdateMM(layerParam)
							If ummList Is Nothing Then
								ummList = New List(Of Double)()
								outUpdateMM(layerParam) = ummList
							End If
							ummList.Add(umm)
						End If
					Next s
				Next u
			End If

			Return New MeanMagnitudes(iterCounts, ratioValues, outParamMM, outUpdateMM)
		End Function

		Private Shared EMPTY_TRIPLE As New Triple(Of Integer(), Single(), Single())(New Integer(){}, New Single(){}, New Single(){})

		Private Shared Function getLayerActivations(ByVal index As Integer, ByVal gi As TrainModuleUtils.GraphInfo, ByVal updates As IList(Of Persistable), ByVal iterationCounts As IList(Of Integer)) As Triple(Of Integer(), Single(), Single())
			If gi Is Nothing Then
				Return EMPTY_TRIPLE
			End If

			Dim type As String = gi.getVertexTypes().get(index) 'Index may be for an input, for example
			If "input".Equals(type, StringComparison.OrdinalIgnoreCase) Then
				Return EMPTY_TRIPLE
			End If
			Dim origNames As IList(Of String) = gi.getOriginalVertexName()
			If index < 0 OrElse index >= origNames.Count Then
				Return EMPTY_TRIPLE
			End If
			Dim layerName As String = origNames(index)

			Dim size As Integer = (If(updates Is Nothing, 0, updates.Count))
			Dim iterCounts(size - 1) As Integer
			Dim mean(size - 1) As Single
			Dim stdev(size - 1) As Single
			Dim used As Integer = 0
			If updates IsNot Nothing Then
				Dim uCount As Integer = -1
				For Each u As Persistable In updates
					uCount += 1
					If Not (TypeOf u Is StatsReport) Then
						Continue For
					End If
					Dim sp As StatsReport = DirectCast(u, StatsReport)
					If iterationCounts Is Nothing Then
						iterCounts(used) = sp.IterationCount
					Else
						iterCounts(used) = iterationCounts(uCount)
					End If

					Dim means As IDictionary(Of String, Double) = sp.getMean(StatsType.Activations)
					Dim stdevs As IDictionary(Of String, Double) = sp.getStdev(StatsType.Activations)

					'TODO PROPER VALIDATION ETC, ERROR HANDLING
					If means IsNot Nothing AndAlso means.ContainsKey(layerName) Then
						mean(used) = means(layerName).floatValue()
						stdev(used) = stdevs(layerName).floatValue()
						If Not Float.isFinite(mean(used)) Then
							mean(used) = CSng(NAN_REPLACEMENT_VALUE)
						End If
						If Not Float.isFinite(stdev(used)) Then
							stdev(used) = CSng(NAN_REPLACEMENT_VALUE)
						End If
						used += 1
					End If
				Next u
			End If

			If used <> iterCounts.Length Then
				iterCounts = Arrays.CopyOf(iterCounts, used)
				mean = Arrays.CopyOf(mean, used)
				stdev = Arrays.CopyOf(stdev, used)
			End If

			Return New Triple(Of Integer(), Single(), Single())(iterCounts, mean, stdev)
		End Function

		Private Shared ReadOnly EMPTY_LR_MAP As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()

		Shared Sub New()
			EMPTY_LR_MAP("iterCounts") = New Integer(){}
			EMPTY_LR_MAP("paramNames") = Collections.EMPTY_LIST
			EMPTY_LR_MAP("lrs") = Collections.EMPTY_MAP
		End Sub

		Private Shared Function getLayerLearningRates(ByVal layerIdx As Integer, ByVal gi As TrainModuleUtils.GraphInfo, ByVal updates As IList(Of Persistable), ByVal iterationCounts As IList(Of Integer), ByVal modelType As ModelType) As IDictionary(Of String, Object)
			If gi Is Nothing Then
				Return java.util.Collections.emptyMap()
			End If

			Dim origNames As IList(Of String) = gi.getOriginalVertexName()

			Dim type As String = gi.getVertexTypes().get(layerIdx) 'Index may be for an input, for example
			If "input".Equals(type, StringComparison.OrdinalIgnoreCase) Then
				Return EMPTY_LR_MAP
			End If

			If layerIdx < 0 OrElse layerIdx >= origNames.Count Then
				Return EMPTY_LR_MAP
			End If

			Dim layerName As String = gi.getOriginalVertexName().get(layerIdx)

			Dim size As Integer = (If(updates Is Nothing, 0, updates.Count))
			Dim iterCounts(size - 1) As Integer
			Dim byName As IDictionary(Of String, Single()) = New Dictionary(Of String, Single())()
			Dim used As Integer = 0
			If updates IsNot Nothing Then
				Dim uCount As Integer = -1
				For Each u As Persistable In updates
					uCount += 1
					If Not (TypeOf u Is StatsReport) Then
						Continue For
					End If
					Dim sp As StatsReport = DirectCast(u, StatsReport)
					If iterationCounts Is Nothing Then
						iterCounts(used) = sp.IterationCount
					Else
						iterCounts(used) = iterationCounts(uCount)
					End If

					'TODO PROPER VALIDATION ETC, ERROR HANDLING
					Dim lrs As IDictionary(Of String, Double) = sp.getLearningRates()

					Dim prefix As String
					If modelType = ModelType.Layer Then
						prefix = layerName
					Else
						prefix = layerName & "_"
					End If

					For Each p As String In lrs.Keys

						If p.StartsWith(prefix, StringComparison.Ordinal) Then
							Dim layerParamName As String = p.Substring(Math.Min(p.Length, prefix.Length))
							If Not byName.ContainsKey(layerParamName) Then
								byName(layerParamName) = New Single(size - 1){}
							End If
							Dim lrThisParam() As Single = byName(layerParamName)
							lrThisParam(used) = lrs(p).floatValue()
						End If
					Next p
					used += 1
				Next u
			End If

			Dim paramNames As IList(Of String) = New List(Of String)(byName.Keys)
			paramNames.Sort() 'Sorted for consistency

			Dim ret As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			ret("iterCounts") = iterCounts
			ret("paramNames") = paramNames
			ret("lrs") = byName

			Return ret
		End Function


		Private Shared Function getHistograms(ByVal layerIdx As Integer, ByVal gi As TrainModuleUtils.GraphInfo, ByVal statsType As StatsType, ByVal p As Persistable) As IDictionary(Of String, Object)
			If p Is Nothing Then
				Return Nothing
			End If
			If Not (TypeOf p Is StatsReport) Then
				Return Nothing
			End If
			Dim sr As StatsReport = DirectCast(p, StatsReport)

			Dim layerName As String = gi.getOriginalVertexName().get(layerIdx)

			Dim map As IDictionary(Of String, Histogram) = sr.getHistograms(statsType)

			Dim paramNames As IList(Of String) = New List(Of String)()

			Dim ret As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			If layerName IsNot Nothing Then
				For Each s As String In map.Keys
					If s.StartsWith(layerName, StringComparison.Ordinal) Then
						Dim paramName As String
						If s.Chars(layerName.Length) = "_"c Then
							'MLN or CG parameter naming convention
							paramName = s.Substring(layerName.Length + 1)
						Else
							'Pretrain layer (VAE, AE) naming convention
							paramName = s.Substring(layerName.Length)
						End If


						paramNames.Add(paramName)
						Dim h As Histogram = map(s)
						Dim thisHist As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
						Dim min As Double = h.getMin()
						Dim max As Double = h.getMax()
						If Double.IsNaN(min) Then
							'If either is NaN, both will be
							min = NAN_REPLACEMENT_VALUE
							max = NAN_REPLACEMENT_VALUE
						End If
						thisHist("min") = min
						thisHist("max") = max
						thisHist("bins") = h.getNBins()
						thisHist("counts") = h.getBinCounts()
						ret(paramName) = thisHist
					End If
				Next s
			End If
			ret("paramNames") = paramNames

			Return ret
		End Function

		Private Shared Function getMemory(ByVal staticInfoAllWorkers As IList(Of Persistable), ByVal updatesLastNMinutes As IList(Of Persistable), ByVal i18n As I18N) As IDictionary(Of String, Object)

			Dim ret As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()

			'First: map workers to JVMs
			Dim jvmIDs As ISet(Of String) = New HashSet(Of String)()
			Dim workersToJvms As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			Dim workerNumDevices As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim deviceNames As IDictionary(Of String, String()) = New Dictionary(Of String, String())()
			For Each p As Persistable In staticInfoAllWorkers
				'TODO validation/checks
				Dim init As StatsInitializationReport = DirectCast(p, StatsInitializationReport)
				Dim jvmuid As String = init.SwJvmUID
				workersToJvms(p.WorkerID) = jvmuid
				jvmIDs.Add(jvmuid)

				Dim nDevices As Integer = init.HwNumDevices
				workerNumDevices(p.WorkerID) = nDevices

				If nDevices > 0 Then
					Dim deviceNamesArr() As String = init.HwDeviceDescription
					deviceNames(p.WorkerID) = deviceNamesArr
				End If
			Next p

			Dim jvmList As IList(Of String) = New List(Of String)(jvmIDs)
			jvmList.Sort()

			'For each unique JVM, collect memory info
			'Do this by selecting the first worker
			Dim count As Integer = 0
			For Each jvm As String In jvmList
				Dim workersForJvm As IList(Of String) = New List(Of String)()
				For Each s As String In workersToJvms.Keys
					If workersToJvms(s).Equals(jvm) Then
						workersForJvm.Add(s)
					End If
				Next s
				workersForJvm.Sort()
				Dim wid As String = workersForJvm(0)

				Dim numDevices As Integer = workerNumDevices(wid)

				Dim jvmData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()

				Dim timestamps As IList(Of Long) = New List(Of Long)()
				Dim fracJvm As IList(Of Single) = New List(Of Single)()
				Dim fracOffHeap As IList(Of Single) = New List(Of Single)()
				Dim lastBytes((2 + numDevices) - 1) As Long
				Dim lastMaxBytes((2 + numDevices) - 1) As Long

				Dim fracDeviceMem As IList(Of IList(Of Single)) = Nothing
				If numDevices > 0 Then
					fracDeviceMem = New List(Of IList(Of Single))(numDevices)
					For i As Integer = 0 To numDevices - 1
						fracDeviceMem.Add(New List(Of Single)())
					Next i
				End If

				If updatesLastNMinutes IsNot Nothing Then
					For Each p As Persistable In updatesLastNMinutes
						'TODO single pass
						If Not p.WorkerID.Equals(wid) Then
							Continue For
						End If
						If Not (TypeOf p Is StatsReport) Then
							Continue For
						End If

						Dim sp As StatsReport = DirectCast(p, StatsReport)

						timestamps.Add(sp.TimeStamp)

						Dim jvmCurrentBytes As Long = sp.JvmCurrentBytes
						Dim jvmMaxBytes As Long = sp.JvmMaxBytes
						Dim ohCurrentBytes As Long = sp.OffHeapCurrentBytes
						Dim ohMaxBytes As Long = sp.OffHeapMaxBytes

						Dim jvmFrac As Double = jvmCurrentBytes / (CDbl(jvmMaxBytes))
						Dim offheapFrac As Double = ohCurrentBytes / (CDbl(ohMaxBytes))
						If Double.IsNaN(jvmFrac) Then
							jvmFrac = 0.0
						End If
						If Double.IsNaN(offheapFrac) Then
							offheapFrac = 0.0
						End If
						fracJvm.Add(CSng(jvmFrac))
						fracOffHeap.Add(CSng(offheapFrac))

						lastBytes(0) = jvmCurrentBytes
						lastBytes(1) = ohCurrentBytes

						lastMaxBytes(0) = jvmMaxBytes
						lastMaxBytes(1) = ohMaxBytes

						If numDevices > 0 Then
							Dim devBytes() As Long = sp.DeviceCurrentBytes
							Dim devMaxBytes() As Long = sp.DeviceMaxBytes
							For i As Integer = 0 To numDevices - 1
								Dim frac As Double = devBytes(i) / (CDbl(devMaxBytes(i)))
								If Double.IsNaN(frac) Then
									frac = 0.0
								End If
								fracDeviceMem(i).Add(CSng(frac))
								lastBytes(2 + i) = devBytes(i)
								lastMaxBytes(2 + i) = devMaxBytes(i)
							Next i
						End If
					Next p
				End If


				Dim fracUtilized As IList(Of IList(Of Single)) = New List(Of IList(Of Single))()
				fracUtilized.Add(fracJvm)
				fracUtilized.Add(fracOffHeap)

				Dim seriesNames((2 + numDevices) - 1) As String
				seriesNames(0) = i18n.getMessage("train.system.hwTable.jvmCurrent")
				seriesNames(1) = i18n.getMessage("train.system.hwTable.offHeapCurrent")
				Dim isDevice((2 + numDevices) - 1) As Boolean
				Dim devNames() As String = deviceNames(wid)
				For i As Integer = 0 To numDevices - 1
					seriesNames(2 + i) = If(devNames IsNot Nothing AndAlso devNames.Length > i, devNames(i), "")
					fracUtilized.Add(fracDeviceMem(i))
					isDevice(2 + i) = True
				Next i

				jvmData("times") = timestamps
				jvmData("isDevice") = isDevice
				jvmData("seriesNames") = seriesNames
				jvmData("values") = fracUtilized
				jvmData("currentBytes") = lastBytes
				jvmData("maxBytes") = lastMaxBytes
				ret(count.ToString()) = jvmData

				count += 1
			Next jvm

			Return ret
		End Function

		Private Shared Function getHardwareSoftwareInfo(ByVal staticInfoAllWorkers As IList(Of Persistable), ByVal i18n As I18N) As Pair(Of IDictionary(Of String, Object), IDictionary(Of String, Object))
			Dim retHw As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim retSw As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()

			'First: map workers to JVMs
			Dim jvmIDs As ISet(Of String) = New HashSet(Of String)()
			Dim staticByJvm As IDictionary(Of String, StatsInitializationReport) = New Dictionary(Of String, StatsInitializationReport)()
			For Each p As Persistable In staticInfoAllWorkers
				'TODO validation/checks
				Dim init As StatsInitializationReport = DirectCast(p, StatsInitializationReport)
				Dim jvmuid As String = init.SwJvmUID
				jvmIDs.Add(jvmuid)
				staticByJvm(jvmuid) = init
			Next p

			Dim jvmList As IList(Of String) = New List(Of String)(jvmIDs)
			jvmList.Sort()

			'For each unique JVM, collect hardware info
			Dim count As Integer = 0
			For Each jvm As String In jvmList
				Dim sr As StatsInitializationReport = staticByJvm(jvm)

				'---- Hardware Info ----
				Dim hwInfo As IList(Of String()) = New List(Of String())()
				Dim numDevices As Integer = sr.HwNumDevices
				Dim deviceDescription() As String = sr.HwDeviceDescription
				Dim devTotalMem() As Long = sr.HwDeviceTotalMemory

				hwInfo.Add(New String(){i18n.getMessage("train.system.hwTable.jvmMax"), sr.HwJvmMaxMemory.ToString()})
				hwInfo.Add(New String(){i18n.getMessage("train.system.hwTable.offHeapMax"), sr.HwOffHeapMaxMemory.ToString()})
				hwInfo.Add(New String(){i18n.getMessage("train.system.hwTable.jvmProcs"), sr.HwJvmAvailableProcessors.ToString()})
				hwInfo.Add(New String(){i18n.getMessage("train.system.hwTable.computeDevices"), numDevices.ToString()})
				For i As Integer = 0 To numDevices - 1
					Dim label As String = i18n.getMessage("train.system.hwTable.deviceName") & " (" & i & ")"
					Dim name As String = (If(deviceDescription Is Nothing OrElse i >= deviceDescription.Length, i.ToString(), deviceDescription(i)))
					hwInfo.Add(New String(){label, name})

					Dim memLabel As String = i18n.getMessage("train.system.hwTable.deviceMemory") & " (" & i & ")"
					Dim memBytes As String = (If(devTotalMem Is Nothing OrElse i >= devTotalMem.Length, "-", devTotalMem(i).ToString()))
					hwInfo.Add(New String(){memLabel, memBytes})
				Next i

				retHw(count.ToString()) = hwInfo

				'---- Software Info -----

				Dim nd4jBackend As String = sr.SwNd4jBackendClass
				If nd4jBackend IsNot Nothing AndAlso nd4jBackend.Contains(".") Then
					Dim idx As Integer = nd4jBackend.LastIndexOf("."c)
					nd4jBackend = nd4jBackend.Substring(idx + 1)
					Dim temp As String
					Select Case nd4jBackend
						Case "CpuNDArrayFactory"
							temp = "CPU"
						Case "JCublasNDArrayFactory"
							temp = "CUDA"
						Case Else
							temp = nd4jBackend
					End Select
					nd4jBackend = temp
				End If

				Dim datatype As String = sr.SwNd4jDataTypeName
				If datatype Is Nothing Then
					datatype = ""
				Else
					datatype = datatype.ToLower()
				End If

				Dim swInfo As IList(Of String()) = New List(Of String())()
				swInfo.Add(New String(){i18n.getMessage("train.system.swTable.os"), sr.SwOsName})
				swInfo.Add(New String(){i18n.getMessage("train.system.swTable.hostname"), sr.SwHostName})
				swInfo.Add(New String(){i18n.getMessage("train.system.swTable.osArch"), sr.SwArch})
				swInfo.Add(New String(){i18n.getMessage("train.system.swTable.jvmName"), sr.SwJvmName})
				swInfo.Add(New String(){i18n.getMessage("train.system.swTable.jvmVersion"), sr.SwJvmVersion})
				swInfo.Add(New String(){i18n.getMessage("train.system.swTable.nd4jBackend"), nd4jBackend})
				swInfo.Add(New String(){i18n.getMessage("train.system.swTable.nd4jDataType"), datatype})

				retSw(count.ToString()) = swInfo

				count += 1
			Next jvm

			Return New Pair(Of IDictionary(Of String, Object), IDictionary(Of String, Object))(retHw, retSw)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class MeanMagnitudes
		Private Class MeanMagnitudes
			Friend iterations As IList(Of Integer)
			Friend ratios As IDictionary(Of String, IList(Of Double))
			Friend paramMM As IDictionary(Of String, IList(Of Double))
			Friend updateMM As IDictionary(Of String, IList(Of Double))
		End Class


		Private Shared Function asJson(ByVal o As Object) As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(o)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function


		Public Overridable ReadOnly Property InternationalizationResources As IList(Of I18NResource)
			Get
				Dim files As IList(Of I18NResource) = New List(Of I18NResource)()
				Dim langs() As String = {"de", "en", "ja", "ko", "ru", "zh"}
				addAll(files, "train", langs)
				addAll(files, "train.model", langs)
				addAll(files, "train.overview", langs)
				addAll(files, "train.system", langs)
				Return files
			End Get
		End Property

		Private Shared Sub addAll(ByVal [to] As IList(Of I18NResource), ByVal prefix As String, ParamArray ByVal suffixes() As String)
			For Each s As String In suffixes
				[to].Add(New I18NResource("dl4j_i18n/" & prefix & "." & s))
			Next s
		End Sub
	End Class

End Namespace