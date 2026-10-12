Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports JCommander = com.beust.jcommander.JCommander
Imports Parameter = com.beust.jcommander.Parameter
Imports ParameterException = com.beust.jcommander.ParameterException
Imports Parameters = com.beust.jcommander.Parameters
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports ReflectionUtils = org.nd4j.common.io.ReflectionUtils
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports Unirest = com.mashape.unirest.http.Unirest
Imports Aeron = io.aeron.Aeron
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports ThreadingMode = io.aeron.driver.ThreadingMode
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
Imports CloseHelper = org.agrona.CloseHelper
Imports BusySpinIdleStrategy = org.agrona.concurrent.BusySpinIdleStrategy
Imports JSONObject = org.json.JSONObject
Imports AeronNDArraySubscriber = org.nd4j.aeron.ipc.AeronNDArraySubscriber
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports NDArrayCallback = org.nd4j.aeron.ipc.NDArrayCallback
Imports NDArrayHolder = org.nd4j.aeron.ipc.NDArrayHolder
Imports AeronNDArrayResponder = org.nd4j.aeron.ipc.response.AeronNDArrayResponder
Imports InMemoryNDArrayHolder = org.nd4j.aeron.ndarrayholder.InMemoryNDArrayHolder
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports MasterConnectionInfo = org.nd4j.parameterserver.model.MasterConnectionInfo
Imports ServerState = org.nd4j.parameterserver.model.ServerState
Imports SlaveConnectionInfo = org.nd4j.parameterserver.model.SlaveConnectionInfo
Imports SubscriberState = org.nd4j.parameterserver.model.SubscriberState
Imports ParameterServerUpdater = org.nd4j.parameterserver.updater.ParameterServerUpdater
Imports SoftSyncParameterUpdater = org.nd4j.parameterserver.updater.SoftSyncParameterUpdater
Imports SynchronousParameterUpdater = org.nd4j.parameterserver.updater.SynchronousParameterUpdater
Imports InMemoryUpdateStorage = org.nd4j.parameterserver.updater.storage.InMemoryUpdateStorage
Imports CheckSocket = org.nd4j.parameterserver.util.CheckSocket
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.parameterserver


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @Parameters(separators = ",") public class ParameterServerSubscriber implements AutoCloseable
	Public Class ParameterServerSubscriber
		Implements AutoCloseable

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(ParameterServerSubscriber))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-p", "--port"}, description = "The port to listen on for the daemon", arity = 1) private int port = 40123;
		Private port As Integer = 40123
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-id", "--streamId"}, description = "The stream id to listen on", arity = 1) private int streamId = 10;
		Private streamId As Integer = 10
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-h", "--host"}, description = "Host for the server to bind to", arity = 1) private String host = "localhost";
		Private host As String = "localhost"
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-d", "--deleteDirectoryOnStart"}, description = "Delete aeron directory on startup.", arity = 1) private boolean deleteDirectoryOnStart = true;
		Private deleteDirectoryOnStart As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-m", "--master"}, description = "Whether this subscriber is a master node or not.", arity = 1) private boolean master = false;
		Private master As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-pm", "--publishmaster"}, description = "Publish master url: host:port - this is for peer nodes needing to publish to another peer.", arity = 1) private String publishMasterUrl = "localhost:40123";
		Private publishMasterUrl As String = "localhost:40123"
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-md", "--mediadriverdirectory"}, description = "The media driver directory opName. This is for when the media driver is started as a separate process.", arity = 1) private String mediaDriverDirectoryName;
		Private mediaDriverDirectoryName As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-sp", "--statusserverport"}, description = "The status server port, defaults to 9000.", arity = 1) private int statusServerPort = 9000;
		Private statusServerPort As Integer = 9000
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-sh", "--statusserverhost"}, description = "The status host, defaults to localhost.", arity = 1) private String statusServerHost = "localhost";
		Private statusServerHost As String = "localhost"
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-up", "--update"}, description = "The update opType for this parameter server. Defaults to sync. You can specify custom and use a jvm argument -Dorg.nd4j.parameterserver.updatetype=your.fully.qualified.class if you want to use a custom class. This must be able to be instantiated from an empty constructor though.", arity = 1) private String updateTypeString = UpdateType.SYNC.toString().toLowerCase();
		Private updateTypeString As String = UpdateType.SYNC.ToString().ToLower()

		Private updateType As UpdateType = UpdateType.SYNC

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-s", "--shape"}, description = "The shape of the ndarray", arity = 1) private java.util.List<Integer> shape;
		Private shape As IList(Of Integer)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-hbi", "--heartbeatinterval"}, description = "Heartbeat interval in ms", arity = 1) private int heartbeatMs = 1000;
		Private heartbeatMs As Integer = 1000
		Private objectMapper As New ObjectMapper()
		Private scheduledExecutorService As ScheduledExecutorService
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"-u", "--updatesPerEpoch"}, description = "The number of updates per epoch", arity = 1, required = true) private int updatesPerEpoch;
		Private updatesPerEpoch As Integer


		''' <summary>
		''' Specify a custom class as a jvm arg.
		''' Note that this class must be a fully qualified classname
		''' </summary>
		Public Const CUSTOM_UPDATE_TYPE As String = "org.nd4j.parameterserver.updatetype"

		''' <summary>
		''' Update types are for
		''' instantiating various kinds of update types
		''' </summary>
		Public Enum UpdateType
			HOGWILD
			SYNC
			TIME_DELAYED
			SOFTSYNC
			CUSTOM
		End Enum



		Private mediaDriver As MediaDriver
		Private responder As AeronNDArrayResponder
		Private subscriber As AeronNDArraySubscriber
		Private callback As NDArrayCallback
		'alias for the callback where relevant
		Private parameterServerListener As ParameterServerListener
		Private aeron As Aeron
		Private heartbeat As ScheduledExecutorService

		''' <summary>
		''' Allow passing in a
		''' media driver that already exists
		''' </summary>
		''' <param name="mediaDriver"> </param>
		Public Sub New(ByVal mediaDriver As MediaDriver)
			Preconditions.checkNotNull(mediaDriver)
			Me.mediaDriver = mediaDriver
		End Sub



		''' <summary>
		''' Return the current <seealso cref="SubscriberState"/>
		''' of this subscriber
		''' </summary>
		''' <returns> the current state of this subscriber </returns>
		Public Overridable Function asState() As SubscriberState
			Return SubscriberState.builder().parameterUpdaterStatus(If(parameterServerListener Is Nothing, Collections.emptyMap(), parameterServerListener.getUpdater().status())).isMaster(isMaster()).connectionInfo(If(isMaster(), masterConnectionInfo().ToString(), slaveConnectionInfo().ToString())).isAsync(parameterServerListener.getUpdater().isAsync()).isReady(parameterServerListener.getUpdater().isReady()).totalUpdates(getResponder().getNdArrayHolder().totalUpdates()).streamId(streamId).serverState(If(subscriberLaunched(), ServerState.STARTED.ToString().ToLower(), ServerState.STOPPED.ToString().ToLower())).build()
		End Function

		''' <summary>
		''' When this is a slave node
		''' it returns the connection url for this node
		''' and the associated master connection urls in the form of:
		''' host:port:streamId
		''' </summary>
		''' <returns> the slave connection info </returns>
		Public Overridable Function slaveConnectionInfo() As SlaveConnectionInfo
			If isMaster() Then
				Throw New System.InvalidOperationException("Unable to determine slave connection info. This is a master node")
			End If
			Return SlaveConnectionInfo.builder().connectionUrl(subscriber.connectionUrl()).masterUrl(publishMasterUrl).build()

		End Function


		''' <summary>
		''' When this is a master node,
		''' it returns the connection url for this node,
		''' it's slaves (if any exist) and the responder
		''' connection url in the form of:
		''' host:port:streamId
		''' </summary>
		''' <returns> the master connection info </returns>
		Public Overridable Function masterConnectionInfo() As MasterConnectionInfo
			If Not isMaster() Then
				Throw New System.InvalidOperationException("Unable to determine master connection info. This is a slave node")
			End If
			Return MasterConnectionInfo.builder().connectionUrl(subscriber.connectionUrl()).responderUrl(responder.connectionUrl()).slaveUrls(New List(Of )()).build()
		End Function

		''' <param name="args"> </param>
		Public Overridable Sub run(ByVal args() As String)
			Dim jcmdr As New JCommander(Me)

			Try
				jcmdr.parse(args)
			Catch e As ParameterException
				log.error("",e)
				'User provides invalid input -> print the usage info
				jcmdr.usage()
				Try
					Thread.Sleep(500)
				Catch e2 As Exception
				End Try
				Environment.Exit(1)
			End Try


			'ensure that the update opType is configured from the command line args
			updateType = System.Enum.Parse(GetType(UpdateType), updateTypeString.ToUpper())



			If publishMasterUrl Is Nothing AndAlso Not master Then
				Throw New System.InvalidOperationException("Please specify a master url or set master to true")
			End If

			'allows passing in a media driver for things like unit tests
			'also ensure we don't use a media driver when a directory is specified
			'for a remote one
			If mediaDriver Is Nothing AndAlso mediaDriverDirectoryName Is Nothing Then
				'length of array * sizeof(float)
				Dim ipcLength As Integer = ArrayUtil.prod(Ints.toArray(shape)) * 4
				'must be a power of 2
				ipcLength *= 2
				'padding for NDArrayMessage
				ipcLength += 64
				'Length in bytes for the SO_RCVBUF, 0 means use OS default. This needs to be larger than Receiver Window.
				System.setProperty("aeron.socket.so_rcvbuf", ipcLength.ToString())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.driver.MediaDriver.Context mediaDriverCtx = new io.aeron.driver.MediaDriver.Context().threadingMode(io.aeron.driver.ThreadingMode.DEDICATED).dirDeleteOnStart(deleteDirectoryOnStart).termBufferSparseFile(false).ipcTermBufferLength(ipcLength).publicationTermBufferLength(ipcLength).conductorIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).receiverIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).senderIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy());
				Dim mediaDriverCtx As MediaDriver.Context = (New MediaDriver.Context()).threadingMode(ThreadingMode.DEDICATED).dirDeleteOnStart(deleteDirectoryOnStart).termBufferSparseFile(False).ipcTermBufferLength(ipcLength).publicationTermBufferLength(ipcLength).conductorIdleStrategy(New BusySpinIdleStrategy()).receiverIdleStrategy(New BusySpinIdleStrategy()).senderIdleStrategy(New BusySpinIdleStrategy())
				AeronUtil.setDaemonizedThreadFactories(mediaDriverCtx)

				mediaDriver = MediaDriver.launchEmbedded(mediaDriverCtx)
				'set the variable since we are using a media driver directly
				mediaDriverDirectoryName = mediaDriver.aeronDirectoryName()
				log.info("Using media driver directory " & mediaDriver.aeronDirectoryName())
			End If

			If aeron Is Nothing Then
				Me.aeron = Aeron.connect(Context)
			End If


			If master Then
				If Me.callback Is Nothing Then
					Dim updater As ParameterServerUpdater = Nothing
					'instantiate with shape instead of just length
					Select Case updateType
						Case org.nd4j.parameterserver.ParameterServerSubscriber.UpdateType.HOGWILD
						Case org.nd4j.parameterserver.ParameterServerSubscriber.UpdateType.SYNC
							updater = New SynchronousParameterUpdater(New InMemoryUpdateStorage(), New InMemoryNDArrayHolder(Ints.toArray(shape)), updatesPerEpoch)
						Case org.nd4j.parameterserver.ParameterServerSubscriber.UpdateType.SOFTSYNC
							updater = New SoftSyncParameterUpdater()
						Case org.nd4j.parameterserver.ParameterServerSubscriber.UpdateType.TIME_DELAYED
						Case org.nd4j.parameterserver.ParameterServerSubscriber.UpdateType.CUSTOM
							Dim parameterServerUpdateType As String = System.getProperty(CUSTOM_UPDATE_TYPE)
							Dim updaterClass As Type(Of ParameterServerUpdater) = ND4JClassLoading.loadClassByName(parameterServerUpdateType)
							updater = ReflectionUtils.newInstance(updaterClass)
						Case Else
							Throw New System.InvalidOperationException("Illegal opType of updater")
					End Select

					callback = New ParameterServerListener(Ints.toArray(shape), updater)
					parameterServerListener = DirectCast(callback, ParameterServerListener)

				End If
				'start an extra daemon for responding to get queries
				Dim cast As ParameterServerListener = DirectCast(callback, ParameterServerListener)
				responder = AeronNDArrayResponder.startSubscriber(aeron, host, port + 1, cast.getUpdater().ndArrayHolder(), streamId + 1)
				log.info("Started responder on master node " & responder.connectionUrl())
			Else
				Dim publishMasterUrlArr() As String = publishMasterUrl.Split(":", True)
				If publishMasterUrlArr Is Nothing OrElse publishMasterUrlArr.Length < 2 Then
					Throw New System.InvalidOperationException("Please specify publish master url as host:port")
				End If

				callback = New PublishingListener(String.Format("aeron:udp?endpoint={0}:{1}", publishMasterUrlArr(0), publishMasterUrlArr(1)), Integer.Parse(publishMasterUrlArr(2)), Context)
			End If

			log.info("Starting subscriber on " & host & ":" & port & " and stream " & streamId)
			Dim running As New AtomicBoolean(True)

			'start a node
			subscriber = AeronNDArraySubscriber.startSubscriber(aeron, host, port, callback, streamId, running)

			Do While Not subscriber.launched()
				LockSupport.parkNanos(100000)
			Loop

			'send heartbeat to a status server. There will usually be 1 status server per master.
			'Only schedule this if a remote server is available.
			If CheckSocket.remotePortTaken(statusServerHost, statusServerPort, 10000) Then
				scheduledExecutorService = Executors.newScheduledThreadPool(1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger failCount = new java.util.concurrent.atomic.AtomicInteger(0);
				Dim failCount As New AtomicInteger(0)
				scheduledExecutorService.scheduleAtFixedRate(Sub()
				Try
					If failCount.get() >= 3 Then
						Return
					End If
					Dim subscriberState As SubscriberState = asState()
					Dim jsonObject As New JSONObject(objectMapper.writeValueAsString(subscriberState))
					Dim url As String = String.Format("http://{0}:{1:D}/updatestatus/{2:D}", statusServerHost, statusServerPort, streamId)
					Dim entity As val = Unirest.post(url).header("Content-Type", "application/json").body(jsonObject).asString()
				Catch e As Exception
					failCount.incrementAndGet()
					If failCount.get() >= 3 Then
						log.warn("Failed to send update, shutting down likely?", e)
					End If
				End Try
				End Sub, 0, heartbeatMs, TimeUnit.MILLISECONDS)

			Else
				log.info("No status server found. Will not send heartbeats. Specified host was " & statusServerHost & " and port was " & statusServerPort)
			End If


			Runtime.getRuntime().addShutdownHook(New Thread(Sub()
			close()
			End Sub))

			'set the server for the status of the master and slave nodes
		End Sub


		Public Overrides Sub close()
			If subscriber IsNot Nothing Then
				CloseHelper.quietClose(subscriber)
			End If
			If responder IsNot Nothing Then
				CloseHelper.quietClose(responder)
			End If
			If scheduledExecutorService IsNot Nothing Then
				scheduledExecutorService.shutdown()
			End If
		End Sub



		'get a context
		Public Overridable ReadOnly Property Context As Aeron.Context
			Get
				Dim ctx As Aeron.Context = (New Aeron.Context()).driverTimeoutMs(Long.MaxValue).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriverDirectoryName).keepAliveIntervalNs(1000000).errorHandler(Function(e) log.error(e.ToString(), e))
				AeronUtil.setDaemonizedThreadFactories(ctx)
				Return ctx
			End Get
		End Property

		''' <summary>
		''' Get the master ndarray from the
		''' internal <seealso cref="NDArrayHolder"/>
		''' </summary>
		''' <returns> the master ndarray </returns>
		Public Overridable ReadOnly Property MasterArray As INDArray
			Get
				Return parameterServerListener.getUpdater().ndArrayHolder().get()
			End Get
		End Property


		''' <summary>
		''' Returns true if the subscriber is launched
		''' 
		''' @return
		''' </summary>
		Public Overridable Function subscriberLaunched() As Boolean
			Return subscriber.launched()
		End Function

		Public Shared Sub Main(ByVal args() As String)
			Call (New ParameterServerSubscriber()).run(args)
		End Sub
	End Class

End Namespace