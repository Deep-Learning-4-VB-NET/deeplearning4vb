Imports System.Collections.Generic
Imports Promise = io.vertx.core.Promise
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports RemoteUIStatsStorageRouter = org.deeplearning4j.core.storage.impl.RemoteUIStatsStorageRouter
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports VertxUIServer = org.deeplearning4j.ui.VertxUIServer
Imports org.nd4j.common.function

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

Namespace org.deeplearning4j.ui.api


	Public Interface UIServer

		''' <summary>
		''' Get (and, initialize if necessary) the UI server. This synchronous function will wait until the server started.
		''' Singleton pattern - all calls to getInstance() will return the same UI instance.
		''' </summary>
		''' <returns> UI instance for this JVM </returns>
		''' <exception cref="DL4JException"> if UI server failed to start;
		''' if the instance has already started in a different mode (multi/single-session);
		''' if interrupted while waiting for completion </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java static interface methods:
'		static UIServer getInstance() throws org.deeplearning4j.exception.DL4JException
	'	{
	'		if (VertxUIServer.getInstance() != Nothing && !VertxUIServer.getInstance().isStopped())
	'		{
	'			Return VertxUIServer.getInstance();
	'		}
	'		else
	'		{
	'			Return getInstance(False, Nothing);
	'		}
	'	}

		''' <summary>
		''' Get (and, initialize if necessary) the UI server. This synchronous function will wait until the server started.
		''' Singleton pattern - all calls to getInstance() will return the same UI instance.
		''' </summary>
		''' <param name="multiSession">         in multi-session mode, multiple training sessions can be visualized in separate browser tabs.
		'''                             <br/>URL path will include session ID as a parameter, i.e.: /train becomes /train/:sessionId </param>
		''' <param name="statsStorageProvider"> function that returns a StatsStorage containing the given session ID.
		'''                             <br/>Use this to auto-attach StatsStorage if an unknown session ID is passed
		'''                             as URL path parameter in multi-session mode, or leave it {@code null}. </param>
		''' <returns> UI instance for this JVM </returns>
		''' <exception cref="DL4JException"> if UI server failed to start;
		''' if the instance has already started in a different mode (multi/single-session);
		''' if interrupted while waiting for completion </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java static interface methods:
'		static UIServer getInstance(boolean multiSession, org.nd4j.common.@function.@Function(Of String, org.deeplearning4j.core.storage.StatsStorage) statsStorageProvider) throws org.deeplearning4j.exception.DL4JException
	'	{
	'		Return VertxUIServer.getInstance(Nothing, multiSession, statsStorageProvider);
	'	}

		''' <summary>
		''' Stop UIServer instance, if already running
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java static interface methods:
'		static void stopInstance() throws Exception
	'	{
	'		VertxUIServer.stopInstance();
	'	}

		ReadOnly Property Stopped As Boolean

		''' <summary>
		''' Check if the instance initialized with one of the factory methods
		''' (<seealso cref="getInstance()"/> or <seealso cref="getInstance(Boolean, Function)"/>) is in multi-session mode
		''' </summary>
		''' <returns> {@code true} if the instance is in multi-session </returns>
		ReadOnly Property MultiSession As Boolean

		''' <summary>
		''' Get the address of the UI
		''' </summary>
		''' <returns> Address of the UI </returns>
		ReadOnly Property Address As String

		''' <summary>
		''' Get the current port for the UI
		''' </summary>
		ReadOnly Property Port As Integer

		''' <summary>
		''' Attach the given StatsStorage instance to the UI, so the data can be visualized
		''' </summary>
		''' <param name="statsStorage"> StatsStorage instance to attach to the UI </param>
		Sub attach(ByVal statsStorage As StatsStorage)

		''' <summary>
		''' Detach the specified StatsStorage instance from the UI
		''' </summary>
		''' <param name="statsStorage"> StatsStorage instance to detach. If not attached: no op. </param>
		Sub detach(ByVal statsStorage As StatsStorage)

		''' <summary>
		''' Check whether the specified StatsStorage instance is attached to the UI instance
		''' </summary>
		''' <param name="statsStorage"> StatsStorage instance to attach </param>
		''' <returns> True if attached </returns>
		Function isAttached(ByVal statsStorage As StatsStorage) As Boolean

		''' <returns> A list of all StatsStorage instances currently attached </returns>
		ReadOnly Property StatsStorageInstances As IList(Of StatsStorage)

		''' <summary>
		''' Enable the remote listener functionality, storing all data in memory, and attaching the instance to the UI.
		''' Typically used with <seealso cref="RemoteUIStatsStorageRouter"/>, which will send information
		''' remotely to this UI instance
		''' </summary>
		''' <seealso cref= #enableRemoteListener(StatsStorageRouter, boolean) </seealso>
		Sub enableRemoteListener()

		''' <summary>
		''' Enable the remote listener functionality, storing the received results in the specified StatsStorageRouter.
		''' If the StatsStorageRouter is a <seealso cref="StatsStorage"/> instance, it may (optionally) be attached to the UI,
		''' as if <seealso cref="attach(StatsStorage)"/> was called on it.
		''' </summary>
		''' <param name="statsStorage"> StatsStorageRouter to post the received results to </param>
		''' <param name="attach">       Whether to attach the given StatsStorage instance to the UI server </param>
		Sub enableRemoteListener(ByVal statsStorage As StatsStorageRouter, ByVal attach As Boolean)

		''' <summary>
		''' Disable the remote listener functionality (disabled by default)
		''' </summary>
		Sub disableRemoteListener()

		''' <returns> Whether the remote listener functionality is currently enabled </returns>
		ReadOnly Property RemoteListenerEnabled As Boolean

		''' <summary>
		''' Stop/shut down the UI server. This synchronous function should wait until the server is stopped. </summary>
		''' <exception cref="InterruptedException"> if the current thread is interrupted while waiting </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void stop() throws InterruptedException;
		Sub [stop]()

		''' <summary>
		''' Stop/shut down the UI server.
		''' This asynchronous function should immediately return, and notify the callback <seealso cref="Promise"/> on completion:
		''' either call <seealso cref="Promise.complete"/> or <seealso cref="io.vertx.core.Promise.fail"/>. </summary>
		''' <param name="stopCallback"> callback <seealso cref="Promise"/> to notify on completion </param>
		Sub stopAsync(ByVal stopCallback As Promise(Of Void))

		''' <summary>
		''' Get shutdown hook of UI server, that will stop the server when the Runtime is stopped.
		''' You may de-register this shutdown hook with <seealso cref="Runtime.removeShutdownHook(Thread)"/>,
		''' and add your own hook with <seealso cref="Runtime.addShutdownHook(Thread)"/> </summary>
		''' <returns> shutdown hook </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java static interface methods:
'		static Thread getShutdownHook()
	'	{
	'		Return VertxUIServer.getShutdownHook();
	'	};
	End Interface

End Namespace