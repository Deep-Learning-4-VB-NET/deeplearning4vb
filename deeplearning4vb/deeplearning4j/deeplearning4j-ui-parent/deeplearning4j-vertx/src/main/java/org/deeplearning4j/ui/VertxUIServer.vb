Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports JCommander = com.beust.jcommander.JCommander
Imports Parameter = com.beust.jcommander.Parameter
Imports AbstractVerticle = io.vertx.core.AbstractVerticle
Imports Future = io.vertx.core.Future
Imports Promise = io.vertx.core.Promise
Imports Vertx = io.vertx.core.Vertx
Imports HttpServer = io.vertx.core.http.HttpServer
Imports MimeMapping = io.vertx.core.http.impl.MimeMapping
Imports Router = io.vertx.ext.web.Router
Imports RoutingContext = io.vertx.ext.web.RoutingContext
Imports BodyHandler = io.vertx.ext.web.handler.BodyHandler
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties
Imports DL4JFileUtils = org.deeplearning4j.common.util.DL4JFileUtils
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageEvent = org.deeplearning4j.core.storage.StatsStorageEvent
Imports StatsStorageListener = org.deeplearning4j.core.storage.StatsStorageListener
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports Route = org.deeplearning4j.ui.api.Route
Imports UIModule = org.deeplearning4j.ui.api.UIModule
Imports UIServer = org.deeplearning4j.ui.api.UIServer
Imports I18NProvider = org.deeplearning4j.ui.i18n.I18NProvider
Imports FileStatsStorage = org.deeplearning4j.ui.model.storage.FileStatsStorage
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports QueueStatsStorageListener = org.deeplearning4j.ui.model.storage.impl.QueueStatsStorageListener
Imports SameDiffModule = org.deeplearning4j.ui.module.SameDiffModule
Imports ConvolutionalListenerModule = org.deeplearning4j.ui.module.convolutional.ConvolutionalListenerModule
Imports DefaultModule = org.deeplearning4j.ui.module.defaultModule.DefaultModule
Imports RemoteReceiverModule = org.deeplearning4j.ui.module.remote.RemoteReceiverModule
Imports TrainModule = org.deeplearning4j.ui.module.train.TrainModule
Imports TsneModule = org.deeplearning4j.ui.module.tsne.TsneModule
Imports org.nd4j.common.function
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

Namespace org.deeplearning4j.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class VertxUIServer extends io.vertx.core.AbstractVerticle implements org.deeplearning4j.ui.api.UIServer
	Public Class VertxUIServer
		Inherits AbstractVerticle
		Implements UIServer

		Public Const DEFAULT_UI_PORT As Integer = 9000
		Public Const ASSETS_ROOT_DIRECTORY As String = "deeplearning4jUiAssets/"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static VertxUIServer instance;
		Private Shared instance As VertxUIServer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static java.util.concurrent.atomic.AtomicBoolean multiSession = new java.util.concurrent.atomic.AtomicBoolean(false);
'JAVA TO VB CONVERTER NOTE: The field multiSession was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared multiSession_Conflict As New AtomicBoolean(False)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private static org.nd4j.common.function.@Function<String, org.deeplearning4j.core.storage.StatsStorage> statsStorageProvider;
		Private Shared statsStorageProvider As [Function](Of String, StatsStorage)

		Private Shared instancePort As Integer?
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static Thread shutdownHook;
		Private Shared shutdownHook As Thread

		''' <summary>
		''' Get (and, initialize if necessary) the UI server. This synchronous function will wait until the server started. </summary>
		''' <param name="port"> TCP socket port for <seealso cref="HttpServer"/> to listen </param>
		''' <param name="multiSession">         in multi-session mode, multiple training sessions can be visualized in separate browser tabs.
		'''                             <br/>URL path will include session ID as a parameter, i.e.: /train becomes /train/:sessionId </param>
		''' <param name="statsStorageProvider"> function that returns a StatsStorage containing the given session ID.
		'''                             <br/>Use this to auto-attach StatsStorage if an unknown session ID is passed
		'''                             as URL path parameter in multi-session mode, or leave it {@code null}. </param>
		''' <returns> UI instance for this JVM </returns>
		''' <exception cref="DL4JException"> if UI server failed to start;
		''' if the instance has already started in a different mode (multi/single-session);
		''' if interrupted while waiting for completion </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static VertxUIServer getInstance(System.Nullable<Integer> port, boolean multiSession, org.nd4j.common.function.@Function<String, org.deeplearning4j.core.storage.StatsStorage> statsStorageProvider) throws org.deeplearning4j.exception.DL4JException
		Public Shared Function getInstance(ByVal port As Integer?, ByVal multiSession As Boolean, ByVal statsStorageProvider As [Function](Of String, StatsStorage)) As VertxUIServer
			Return getInstance(port, multiSession, statsStorageProvider, Nothing)
		End Function

		''' 
		''' <summary>
		''' Get (and, initialize if necessary) the UI server. This function will wait until the server started
		''' (synchronous way), or pass the given callback to handle success or failure (asynchronous way). </summary>
		''' <param name="port"> TCP socket port for <seealso cref="HttpServer"/> to listen </param>
		''' <param name="multiSession">         in multi-session mode, multiple training sessions can be visualized in separate browser tabs.
		'''                             <br/>URL path will include session ID as a parameter, i.e.: /train becomes /train/:sessionId </param>
		''' <param name="statsStorageProvider"> function that returns a StatsStorage containing the given session ID.
		'''                             <br/>Use this to auto-attach StatsStorage if an unknown session ID is passed
		'''                             as URL path parameter in multi-session mode, or leave it {@code null}. </param>
		''' <param name="startCallback"> asynchronous deployment handler callback that will be notify of success or failure.
		'''                      If {@code null} given, then this method will wait until deployment is complete.
		'''                      If the deployment is successful the result will contain a String representing the
		'''                      unique deployment ID of the deployment. </param>
		''' <returns> UI server instance </returns>
		''' <exception cref="DL4JException"> if UI server failed to start;
		''' if the instance has already started in a different mode (multi/single-session);
		''' if interrupted while waiting for completion </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static VertxUIServer getInstance(System.Nullable<Integer> port, boolean multiSession, org.nd4j.common.function.@Function<String, org.deeplearning4j.core.storage.StatsStorage> statsStorageProvider, io.vertx.core.Promise<String> startCallback) throws org.deeplearning4j.exception.DL4JException
		Public Shared Function getInstance(ByVal port As Integer?, ByVal multiSession As Boolean, ByVal statsStorageProvider As [Function](Of String, StatsStorage), ByVal startCallback As Promise(Of String)) As VertxUIServer
			If instance Is Nothing OrElse instance.Stopped Then
				VertxUIServer.multiSession_Conflict.set(multiSession)
				VertxUIServer.setStatsStorageProvider(statsStorageProvider)
				instancePort = port

				If startCallback IsNot Nothing Then
					'Launch UI server verticle and pass asynchronous callback that will be notified of completion
					deploy(startCallback)
				Else
					'Launch UI server verticle and wait for it to start
					deploy()
				End If
			ElseIf Not instance.Stopped Then
				If multiSession AndAlso Not instance.MultiSession Then
					Throw New DL4JException("Cannot return multi-session instance." & " UIServer has already started in single-session mode at " & instance.Address & " You may stop the UI server instance, and start a new one.")
				ElseIf Not multiSession AndAlso instance.MultiSession Then
					Throw New DL4JException("Cannot return single-session instance." & " UIServer has already started in multi-session mode at " & instance.Address & " You may stop the UI server instance, and start a new one.")
				End If
			End If

			Return instance
		End Function

		''' <summary>
		''' Deploy (start) <seealso cref="VertxUIServer"/>, waiting until starting is complete. </summary>
		''' <exception cref="DL4JException"> if UI server failed to start;
		''' if interrupted while waiting for completion </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void deploy() throws org.deeplearning4j.exception.DL4JException
		Private Shared Sub deploy()
			Dim l As New System.Threading.CountdownEvent(1)
			Dim promise As Promise(Of String) = Promise.promise()
			promise.future().compose(Function(success) Future.future(Function(prom) l.Signal()), Function(failure) Future.future(Function(prom) l.Signal()))
			deploy(promise)
			' synchronous function
			Try
				l.await()
			Catch e As InterruptedException
				Throw New DL4JException(e)
			End Try

			Dim future As Future(Of String) = promise.future()
			If future.failed() Then
				Throw New DL4JException("Deeplearning4j UI server failed to start.", future.cause())
			End If
		End Sub

		''' <summary>
		''' Deploy (start) <seealso cref="VertxUIServer"/>,
		''' and pass callback to handle successful or failed completion of deployment. </summary>
		''' <param name="startCallback"> promise that will handle success or failure of deployment.
		''' If the deployment is successful the result will contain a String representing the unique deployment ID of the
		''' deployment. </param>
		Private Shared Sub deploy(ByVal startCallback As Promise(Of String))
			log.debug("Deeplearning4j UI server is starting.")
			Dim promise As Promise(Of String) = Promise.promise()
			promise.future().compose(Function(success) Future.future(Function(prom) startCallback.complete(success)), Function(failure) Future.future(Function(prom) startCallback.fail(New Exception(failure))))

			Dim vertx As Vertx = Vertx.vertx()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			vertx.deployVerticle(GetType(VertxUIServer).FullName, promise)

			VertxUIServer.shutdownHook = New Thread(Sub()
			If VertxUIServer.instance IsNot Nothing AndAlso Not VertxUIServer.instance.Stopped Then
				log.info("Deeplearning4j UI server is auto-stopping in shutdown hook.")
				Try
					instance.stop()
				Catch e As InterruptedException
					log.error("Interrupted stopping of Deeplearning4j UI server in shutdown hook.", e)
				End Try
			End If
			End Sub)
			Runtime.getRuntime().addShutdownHook(shutdownHook)
		End Sub


		Private uiModules As IList(Of UIModule) = New CopyOnWriteArrayList(Of UIModule)()
		Private remoteReceiverModule As RemoteReceiverModule
		''' <summary>
		''' Loader that attaches {@code StatsStorage} provided by {@code #statsStorageProvider} for the given session ID
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.common.function.@Function<String, Boolean> statsStorageLoader;
		Private statsStorageLoader As [Function](Of String, Boolean)

		'typeIDModuleMap: Records which modules are registered for which type IDs
		Private typeIDModuleMap As IDictionary(Of String, IList(Of UIModule)) = New ConcurrentDictionary(Of String, IList(Of UIModule))()

		Private server As HttpServer
		Private shutdown As New AtomicBoolean(False)
		Private uiProcessingDelay As Long = 500 '500ms. TODO make configurable


		Private ReadOnly eventQueue As BlockingQueue(Of StatsStorageEvent) = New LinkedBlockingQueue(Of StatsStorageEvent)()
		Private listeners As IList(Of Pair(Of StatsStorage, StatsStorageListener)) = New CopyOnWriteArrayList(Of Pair(Of StatsStorage, StatsStorageListener))()
		Private statsStorageInstances As IList(Of StatsStorage) = New CopyOnWriteArrayList(Of StatsStorage)()

		Private uiEventRoutingThread As Thread

		Public Sub New()
			instance = Me
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void stopInstance() throws Exception
		Public Shared Sub stopInstance() Implements UIServer.stopInstance
			If instance Is Nothing OrElse instance.Stopped Then
				Return
			End If
			instance.stop()
			VertxUIServer.reset()
		End Sub

		Private Shared Sub reset()
			VertxUIServer.instance = Nothing
			VertxUIServer.statsStorageProvider = Nothing
			VertxUIServer.instancePort = Nothing
			VertxUIServer.multiSession_Conflict.set(False)
		End Sub

		''' <summary>
		''' Auto-attach StatsStorage if an unknown session ID is passed as URL path parameter in multi-session mode </summary>
		''' <param name="statsStorageProvider"> function that returns a StatsStorage containing the given session ID </param>
		Public Overridable Sub autoAttachStatsStorageBySessionId(ByVal statsStorageProvider As [Function](Of String, StatsStorage))
			If statsStorageProvider IsNot Nothing Then
				Me.statsStorageLoader = Function(sessionId)
				log.info("Loading StatsStorage via StatsStorageProvider for session ID (" & sessionId & ").")
				Dim statsStorage As StatsStorage = statsStorageProvider.apply(sessionId)
				If statsStorage IsNot Nothing Then
					If statsStorage.sessionExists(sessionId) Then
						attach(statsStorage)
						Return True
					End If
					log.info("Failed to load StatsStorage via StatsStorageProvider for session ID. " & "Session ID (" & sessionId & ") does not exist in StatsStorage.")
					Return False
				Else
					log.info("Failed to load StatsStorage via StatsStorageProvider for session ID (" & sessionId & "). " & "StatsStorageProvider returned null.")
					Return False
				End If
				End Function
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void start(io.vertx.core.Promise<Void> startCallback) throws Exception
		Public Overrides Sub start(ByVal startCallback As Promise(Of Void))
			'Create REST endpoints
			Dim uploadDir As New File(System.getProperty("java.io.tmpdir"), "DL4JUI_" & DateTimeHelper.CurrentUnixTimeMillis())
			uploadDir.mkdirs()
			Dim r As Router = Router.router(vertx)
			r.route().handler(BodyHandler.create().setUploadsDirectory(uploadDir.getAbsolutePath()))
			r.get("/assets/*").handler(Sub(rc)
			Dim path As String = rc.request().path()
			path = path.Substring(8)
			Dim mime As String
			Dim newPath As String
			If path.Contains("webjars") Then
				newPath = "META-INF/resources/" & path.Substring(path.IndexOf("webjars", StringComparison.Ordinal))
			Else
				newPath = ASSETS_ROOT_DIRECTORY & (If(path.StartsWith("/", StringComparison.Ordinal), path.Substring(1), path))
			End If
			mime = MimeMapping.getMimeTypeForFilename(FilenameUtils.getName(newPath))
			rc.response().putHeader("content-type", mime).sendFile(newPath)
			End Sub)


			If MultiSession Then
				r.get("/setlang/:sessionId/:to").handler(Sub(rc)
				Dim sid As String = rc.request().getParam("sessionID")
				Dim [to] As String = rc.request().getParam("to")
				I18NProvider.getInstance(sid).DefaultLanguage = [to]
				rc.response().end()
				End Sub)
			Else
				r.get("/setlang/:to").handler(Sub(rc)
				Dim [to] As String = rc.request().getParam("to")
				I18NProvider.Instance.DefaultLanguage = [to]
				rc.response().end()
				End Sub)
			End If

			If VertxUIServer.statsStorageProvider IsNot Nothing Then
				autoAttachStatsStorageBySessionId(VertxUIServer.statsStorageProvider)
			End If

			uiModules.Add(New DefaultModule(MultiSession)) 'For: navigation page "/"
			uiModules.Add(New TrainModule())
			uiModules.Add(New ConvolutionalListenerModule())
			uiModules.Add(New TsneModule())
			uiModules.Add(New SameDiffModule())
			remoteReceiverModule = New RemoteReceiverModule()
			uiModules.Add(remoteReceiverModule)

			'Check service loader mechanism (Arbiter UI, etc) for modules
			modulesViaServiceLoader(uiModules)

			For Each m As UIModule In uiModules
				Dim routes As IList(Of Route) = m.getRoutes()
				For Each route As Route In routes
					Select Case route.getHttpMethod()
						Case [GET]
							r.get(route.getRoute()).handler(Function(rc) route.getConsumer().accept(extractArgsFromRoute(route.getRoute(), rc), rc))
						Case PUT
							r.put(route.getRoute()).handler(Function(rc) route.getConsumer().accept(extractArgsFromRoute(route.getRoute(), rc), rc))
						Case POST
							r.post(route.getRoute()).handler(Function(rc) route.getConsumer().accept(extractArgsFromRoute(route.getRoute(), rc), rc))
						Case Else
							Throw New System.InvalidOperationException("Unknown or not supported HTTP method: " & route.getHttpMethod())
					End Select
				Next route

				'Determine which type IDs this module wants to receive:
				Dim typeIDs As IList(Of String) = m.getCallbackTypeIDs()
				For Each typeID As String In typeIDs
					Dim list As IList(Of UIModule) = typeIDModuleMap(typeID)
					If list Is Nothing Then
						list = Collections.synchronizedList(New List(Of )())
						typeIDModuleMap(typeID) = list
					End If
					list.Add(m)
				Next typeID
			Next m

			'Check port property
			Dim port As Integer = If(instancePort Is Nothing, DEFAULT_UI_PORT, instancePort)
			Dim portProp As String = System.getProperty(DL4JSystemProperties.UI_SERVER_PORT_PROPERTY)
			If portProp IsNot Nothing AndAlso portProp.Length > 0 Then
				Try
					port = Integer.Parse(portProp)
				Catch e As System.FormatException
					log.warn("Error parsing port property {}={}", DL4JSystemProperties.UI_SERVER_PORT_PROPERTY, portProp)
				End Try
			End If

		If port < 0 OrElse port > &HFFFF Then
				Throw New System.InvalidOperationException("Valid port range is 0 <= port <= 65535. The given port was " & port)
		End If

			uiEventRoutingThread = New Thread(New StatsEventRouterRunnable(Me))
			uiEventRoutingThread.setDaemon(True)
			uiEventRoutingThread.Start()

			server = vertx.createHttpServer().requestHandler(r).listen(port, Sub(result)
			If result.succeeded() Then
				Dim address As String = UIServer.getInstance().getAddress()
				log.info("Deeplearning4j UI server started at: {}", address)
				startCallback.complete()
			Else
				startCallback.fail(New Exception("Deeplearning4j UI server failed to listen on port " & server.actualPort(), result.cause()))
			End If
			End Sub)
		End Sub

		Private Function extractArgsFromRoute(ByVal path As String, ByVal rc As RoutingContext) As IList(Of String)
			If Not path.Contains(":") Then
				Return Collections.emptyList()
			End If
			Dim split() As String = path.Split("/", True)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each s As String In split
				If s.StartsWith(":", StringComparison.Ordinal) Then
					Dim s2 As String = s.Substring(1)
					[out].Add(rc.request().getParam(s2))
				End If
			Next s
			Return [out]
		End Function

		Private Sub modulesViaServiceLoader(ByVal uiModules As IList(Of UIModule))
			Dim sl As ServiceLoader(Of UIModule) = DL4JClassLoading.loadService(GetType(UIModule))
			Dim iter As IEnumerator(Of UIModule) = sl.GetEnumerator()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iter.hasNext() Then
				Return
			End If

			Do While iter.MoveNext()
				Dim [module] As UIModule = iter.Current
				Dim moduleClass As Type = [module].GetType()
				Dim foundExisting As Boolean = False
				For Each mExisting As UIModule In uiModules
					If mExisting.GetType() = moduleClass Then
						foundExisting = True
						Exit For
					End If
				Next mExisting

				If Not foundExisting Then
					log.debug("Loaded UI module via service loader: {}", [module].GetType())
					uiModules.Add([module])
				End If
			Loop
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void stop() throws InterruptedException
		Public Overridable Sub [stop]() Implements UIServer.stop
			Dim l As New System.Threading.CountdownEvent(1)
			Dim promise As Promise(Of Void) = Promise.promise()
			promise.future().compose(Function(successEvent) Future.future(Function(prom) l.Signal()), Function(failureEvent) Future.future(Function(prom) l.Signal()))
			stopAsync(promise)
			' synchronous function should wait until the server is stopped
			l.await()
		End Sub

		Public Overridable Sub stopAsync(ByVal stopCallback As Promise(Of Void)) Implements UIServer.stopAsync
			''' <summary>
			''' Stop Vertx instance and release any resources held by it.
			''' Pass promise to <seealso cref="stop(Promise)"/>.
			''' </summary>
			vertx.close(Function(ar) stopCallback.handle(ar))
		End Sub

		Public Overrides Sub [stop](ByVal stopCallback As Promise(Of Void))
			shutdown.set(True)
			stopCallback.complete()
			log.info("Deeplearning4j UI server stopped.")
		End Sub

		Public Overridable ReadOnly Property Stopped As Boolean Implements UIServer.isStopped
			Get
				Return shutdown.get()
			End Get
		End Property

		Public Overridable ReadOnly Property MultiSession As Boolean Implements UIServer.isMultiSession
			Get
				Return multiSession_Conflict.get()
			End Get
		End Property

		Public Overridable ReadOnly Property Address As String Implements UIServer.getAddress
			Get
				Return "http://localhost:" & server.actualPort()
			End Get
		End Property

		Public Overridable ReadOnly Property Port As Integer Implements UIServer.getPort
			Get
				Return server.actualPort()
			End Get
		End Property

		Public Overridable Sub attach(ByVal statsStorage As StatsStorage) Implements UIServer.attach
			If statsStorage Is Nothing Then
				Throw New System.ArgumentException("StatsStorage cannot be null")
			End If
			If statsStorageInstances.Contains(statsStorage) Then
				Return
			End If
			Dim listener As StatsStorageListener = New QueueStatsStorageListener(eventQueue)
			listeners.Add(New Pair(Of StatsStorage, StatsStorageListener)(statsStorage, listener))
			statsStorage.registerStatsStorageListener(listener)
			statsStorageInstances.Add(statsStorage)

			For Each uiModule As UIModule In uiModules
				uiModule.onAttach(statsStorage)
			Next uiModule

			log.info("StatsStorage instance attached to UI: {}", statsStorage)
		End Sub

		Public Overridable Sub detach(ByVal statsStorage As StatsStorage) Implements UIServer.detach
			If statsStorage Is Nothing Then
				Throw New System.ArgumentException("StatsStorage cannot be null")
			End If
			If Not statsStorageInstances.Contains(statsStorage) Then
				Return 'No op
			End If
			Dim found As Boolean = False
			For Each p As Pair(Of StatsStorage, StatsStorageListener) In listeners
				If p.First Is statsStorage Then 'Same object, not equality
					statsStorage.deregisterStatsStorageListener(p.Second)
					listeners.Remove(p)
					found = True
				End If
			Next p
			statsStorageInstances.Remove(statsStorage)
			For Each uiModule As UIModule In uiModules
				uiModule.onDetach(statsStorage)
			Next uiModule
			For Each sessionId As String In statsStorage.listSessionIDs()
				I18NProvider.removeInstance(sessionId)
			Next sessionId
			If found Then
				log.info("StatsStorage instance detached from UI: {}", statsStorage)
			End If
		End Sub

		Public Overridable Function isAttached(ByVal statsStorage As StatsStorage) As Boolean Implements UIServer.isAttached
			Return statsStorageInstances.Contains(statsStorage)
		End Function

		Public Overridable ReadOnly Property StatsStorageInstances As IList(Of StatsStorage) Implements UIServer.getStatsStorageInstances
			Get
				Return New List(Of StatsStorage)(statsStorageInstances)
			End Get
		End Property

		Public Overridable Sub enableRemoteListener() Implements UIServer.enableRemoteListener
			If remoteReceiverModule Is Nothing Then
				remoteReceiverModule = New RemoteReceiverModule()
			End If
			If remoteReceiverModule.Enabled Then
				Return
			End If
			enableRemoteListener(New InMemoryStatsStorage(), True)
		End Sub

		Public Overridable Sub enableRemoteListener(ByVal statsStorage As StatsStorageRouter, ByVal attach As Boolean) Implements UIServer.enableRemoteListener
			remoteReceiverModule.Enabled = True
			remoteReceiverModule.StatsStorage = statsStorage
			If attach AndAlso TypeOf statsStorage Is StatsStorage Then
				Me.attach(DirectCast(statsStorage, StatsStorage))
			End If
		End Sub

		Public Overridable Sub disableRemoteListener() Implements UIServer.disableRemoteListener
			remoteReceiverModule.Enabled = False
		End Sub

		Public Overridable ReadOnly Property RemoteListenerEnabled As Boolean Implements UIServer.isRemoteListenerEnabled
			Get
				Return remoteReceiverModule.Enabled
			End Get
		End Property


		Private Class StatsEventRouterRunnable
			Implements ThreadStart

			Private ReadOnly outerInstance As VertxUIServer

			Public Sub New(ByVal outerInstance As VertxUIServer)
				Me.outerInstance = outerInstance
			End Sub


			Public Overrides Sub run()
				Try
					runHelper()
				Catch e As Exception
					log.error("Unexpected exception from Event routing runnable", e)
				End Try
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void runHelper() throws Exception
			Friend Overridable Sub runHelper()
				log.trace("VertxUIServer.StatsEventRouterRunnable started")
				'Idea: collect all event stats, and route them to the appropriate modules
				Do While Not outerInstance.shutdown.get()

					Dim events As IList(Of StatsStorageEvent) = New List(Of StatsStorageEvent)()
					Dim sse As StatsStorageEvent = outerInstance.eventQueue.take() 'Blocking operation
					events.Add(sse)
					outerInstance.eventQueue.drainTo(events) 'Non-blocking

					For Each m As UIModule In outerInstance.uiModules

						Dim callbackTypes As IList(Of String) = m.getCallbackTypeIDs()
						Dim [out] As IList(Of StatsStorageEvent) = New List(Of StatsStorageEvent)()
						For Each e As StatsStorageEvent In events
							If callbackTypes.Contains(e.getTypeID()) AndAlso outerInstance.statsStorageInstances.Contains(e.getStatsStorage()) Then
								[out].Add(e)
							End If
						Next e

						m.reportStorageEvents([out])
					Next m

					events.Clear()

					Try
						Thread.Sleep(outerInstance.uiProcessingDelay)
					Catch e As InterruptedException
						Thread.CurrentThread.Interrupt()
						If Not outerInstance.shutdown.get() Then
							Throw New Exception("Unexpected interrupted exception", e)
						End If
					End Try
				Loop
			End Sub
		End Class

		'==================================================================================================================
		' CLI Launcher

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data private static class CLIParams
		Private Class CLIParams
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-r", "--enableRemote"}, description = "Whether to enable remote or not", arity = 1) private boolean cliEnableRemote;
			Friend cliEnableRemote As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-p", "--uiPort"}, description = "Custom HTTP port for UI", arity = 1) private int cliPort = DEFAULT_UI_PORT;
			Friend cliPort As Integer = DEFAULT_UI_PORT

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-f", "--customStatsFile"}, description = "Path to create custom stats file (remote only)", arity = 1) private String cliCustomStatsFile;
			Friend cliCustomStatsFile As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-m", "--multiSession"}, description = "Whether to enable multiple separate browser sessions or not", arity = 1) private boolean cliMultiSession;
			Friend cliMultiSession As Boolean
		End Class

		Public Overridable Sub main(ByVal args() As String)
			Dim d As New CLIParams()
			Call (New JCommander(d)).parse(args)
			instancePort = d.getCliPort()
			UIServer.getInstance(d.isCliMultiSession(), Nothing)
			If d.isCliEnableRemote() Then
				Try
					Dim tempStatsFile As File = DL4JFileUtils.createTempFile("dl4j", "UIstats")
					tempStatsFile.delete()
					tempStatsFile.deleteOnExit()
					enableRemoteListener(New FileStatsStorage(tempStatsFile), True)
				Catch e As Exception
					log.error("Failed to create temporary file for stats storage",e)
					Environment.Exit(1)
				End Try
			End If
		End Sub
	End Class

End Namespace