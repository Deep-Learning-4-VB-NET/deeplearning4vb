Imports System.Collections.Generic
Imports Aeron = io.aeron.Aeron
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CloseHelper = org.agrona.CloseHelper
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports NDArrayCallback = org.nd4j.aeron.ipc.NDArrayCallback
Imports ParameterServerListener = org.nd4j.parameterserver.ParameterServerListener
Imports ParameterServerSubscriber = org.nd4j.parameterserver.ParameterServerSubscriber

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

Namespace org.nd4j.parameterserver.node


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NoArgsConstructor @Data public class ParameterServerNode implements AutoCloseable
	Public Class ParameterServerNode
		Implements AutoCloseable

		Private subscriber() As ParameterServerSubscriber
		Private mediaDriver As MediaDriver
		Private aeron As Aeron
		Private statusPort As Integer
		Private numWorkers As Integer



		''' 
		''' <param name="mediaDriver"> the media driver to sue for communication </param>
		''' <param name="statusPort"> the port for the server status </param>
		Public Sub New(ByVal mediaDriver As MediaDriver, ByVal statusPort As Integer)
			Me.New(mediaDriver, statusPort, Runtime.getRuntime().availableProcessors())

		End Sub

		''' 
		''' <param name="mediaDriver"> the media driver to sue for communication </param>
		''' <param name="statusPort"> the port for the server status </param>
		Public Sub New(ByVal mediaDriver As MediaDriver, ByVal statusPort As Integer, ByVal numWorkers As Integer)
			Me.mediaDriver = mediaDriver
			Me.statusPort = statusPort
			Me.numWorkers = numWorkers
			subscriber = New ParameterServerSubscriber(numWorkers - 1){}

		End Sub


		''' <summary>
		''' Pass in the media driver used for communication
		''' and a defualt status port of 9000 </summary>
		''' <param name="mediaDriver"> </param>
		Public Sub New(ByVal mediaDriver As MediaDriver)
			Me.New(mediaDriver, 9000)
		End Sub

		''' <summary>
		''' Run this node with the given args
		''' These args are the same ones
		''' that a <seealso cref="ParameterServerSubscriber"/> takes </summary>
		''' <param name="args"> the arguments for the <seealso cref="ParameterServerSubscriber"/> </param>
		Public Overridable Sub runMain(ByVal args() As String)
			If mediaDriver Is Nothing Then
				mediaDriver = MediaDriver.launchEmbedded()
			End If
			log.info("Started media driver with aeron directory " & mediaDriver.aeronDirectoryName())
			'cache a reference to the first listener.
			'The reason we do this is to share an updater and listener across *all* subscribers
			'This will create a shared pool of subscribers all updating the same "server".
			'This will simulate a shared pool but allow an accumulative effect of anything
			'like averaging we try.
			Dim parameterServerListener As NDArrayCallback = Nothing
			Dim cast As ParameterServerListener = Nothing
			For i As Integer = 0 To numWorkers - 1
				subscriber(i) = New ParameterServerSubscriber(mediaDriver)
				'ensure reuse of aeron wherever possible
				If aeron Is Nothing Then
					aeron = Aeron.connect(getContext(mediaDriver))
				End If
				subscriber(i).setAeron(aeron)
				Dim multiArgs As IList(Of String) = New List(Of String) From {args}
				If multiArgs.Contains("-id") Then
					Dim streamIdIdx As Integer = multiArgs.IndexOf("-id") + 1
					Dim streamId As Integer = Integer.Parse(multiArgs(streamIdIdx)) + i
					multiArgs(streamIdIdx) = streamId.ToString()
				ElseIf multiArgs.Contains("--streamId") Then
					Dim streamIdIdx As Integer = multiArgs.IndexOf("--streamId") + 1
					Dim streamId As Integer = Integer.Parse(multiArgs(streamIdIdx)) + i
					multiArgs(streamIdIdx) = streamId.ToString()
				End If


				If i = 0 Then
					subscriber(i).run(CType(multiArgs, List(Of String)).ToArray())
					parameterServerListener = subscriber(i).getCallback()
					cast = subscriber(i).getParameterServerListener()
				Else
					'note that we set both the callback AND the listener here
					subscriber(i).setCallback(parameterServerListener)
					subscriber(i).setParameterServerListener(cast)
					'now run the callback initialized with this callback instead
					'in the run method it will use this reference instead of creating it
					'itself
					subscriber(i).run(CType(multiArgs, List(Of String)).ToArray())
				End If


			Next i

		End Sub



		''' <summary>
		''' Returns true if all susbcribers in the
		''' subscriber pool have been launched
		''' @return
		''' </summary>
		Public Overridable Function subscriberLaunched() As Boolean
			Dim launched As Boolean = True
			For i As Integer = 0 To numWorkers - 1
				launched = launched AndAlso subscriber(i).subscriberLaunched()
			Next i

			Return launched
		End Function


		''' <summary>
		''' Stop the server </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()
			If subscriber IsNot Nothing Then
				For i As Integer = 0 To subscriber.Length - 1
					If subscriber(i) IsNot Nothing Then
						subscriber(i).close()
					End If
				Next i
			End If

			If mediaDriver IsNot Nothing Then
				CloseHelper.quietClose(mediaDriver)
			End If
			If aeron IsNot Nothing Then
				CloseHelper.quietClose(aeron)
			End If

		End Sub



		Private Shared Function getContext(ByVal mediaDriver As MediaDriver) As Aeron.Context
			Return (New Aeron.Context()).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriver.aeronDirectoryName()).keepAliveIntervalNs(100000).errorHandler(Function(e) log.error(e.ToString(), e))
		End Function


		Public Shared Sub Main(ByVal args() As String)
			Call (New ParameterServerNode()).runMain(args)
		End Sub

	End Class

End Namespace