Imports System
Imports System.Threading
Imports Unirest = com.mashape.unirest.http.Unirest
Imports Aeron = io.aeron.Aeron
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.nd4j.aeron.ipc
Imports HostPortPublisher = org.nd4j.aeron.ipc.response.HostPortPublisher
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MasterStatus = org.nd4j.parameterserver.model.MasterStatus
Imports ServerTypeJson = org.nd4j.parameterserver.model.ServerTypeJson
Imports SubscriberState = org.nd4j.parameterserver.model.SubscriberState
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

Namespace org.nd4j.parameterserver.client


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Builder @Slf4j public class ParameterServerClient implements NDArrayCallback
	Public Class ParameterServerClient
		Implements NDArrayCallback

		'the url to send ndarrays to
		Private ndarraySendUrl As String
		'the url to retrieve ndarrays from
		Private ndarrayRetrieveUrl As String
		Private subscriber As AeronNDArraySubscriber
		'host to listen on for the subscriber
		Private subscriberHost As String
		'port to listen on for the subscriber
		Private subscriberPort As Integer
		'the stream to listen on for the subscriber
		Private subscriberStream As Integer = 11
		'the "current" ndarray
		Private arr As AtomicReference(Of INDArray)
		Private none As INDArray = Nd4j.scalar(1.0)
		Private running As AtomicBoolean
		Private masterStatusHost As String
		Private masterStatusPort As Integer
		Private objectMapper As New ObjectMapper()
		Private aeron As Aeron
		Private compressArray As Boolean = True

		''' <summary>
		''' Tracks number of
		''' arrays send to responder.
		''' @return
		''' </summary>
		Public Overridable Function arraysSentToResponder() As Integer
			If objectMapper Is Nothing Then
				objectMapper = New ObjectMapper()
			End If

			Try
				Dim type As String = objectMapper.readValue(Unirest.get(String.Format("http://{0}:{1:D}/opType", masterStatusHost, masterStatusPort)).asJson().getBody().ToString(), GetType(ServerTypeJson)).getType()
				If Not type.Equals("master") Then
					Throw New System.InvalidOperationException("Wrong opType " & type)
				End If
				Unirest.get(String.Format("http://{0}:{1:D}/started", masterStatusHost, masterStatusPort)).asJson().getBody()
				Return objectMapper.readValue(Unirest.get(String.Format("http://{0}:{1:D}/started", masterStatusHost, masterStatusPort)).asJson().getBody().ToString(), GetType(MasterStatus)).getResponderN()
			Catch e As Exception
				log.error("",e)
			End Try
			Return 0
		End Function

		''' <summary>
		''' Block the clint till ready
		''' for next phase.
		''' 
		''' </summary>
		Public Overridable Sub blockTillReady()
			Do While Not ReadyForNext
				Try
					Thread.Sleep(1000)
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
				End Try
			Loop
		End Sub


		''' <summary>
		''' Returns true if the client is
		''' ready for a next array or not </summary>
		''' <returns> true if the client is
		''' ready for the next array or not,false otherwise </returns>
		Public Overridable ReadOnly Property ReadyForNext As Boolean
			Get
				If objectMapper Is Nothing Then
					objectMapper = New ObjectMapper()
				End If
    
				Try
					Dim masterStream As Integer = Integer.Parse(ndarraySendUrl.Split(":", True)(2))
					Dim subscriberState As SubscriberState = objectMapper.readValue(Unirest.get(String.Format("http://{0}:{1:D}/state/{2:D}", masterStatusHost, masterStatusPort, masterStream)).asJson().getBody().ToString(), GetType(SubscriberState))
					Return subscriberState.isReady()
				Catch e As Exception
					log.error("",e)
				End Try
				Return False
			End Get
		End Property


		''' <summary>
		''' Sends a post request to the
		''' status server to determine if the master node is started.
		''' @return
		''' </summary>
		Public Overridable Function masterStarted() As Boolean
			If objectMapper Is Nothing Then
				objectMapper = New ObjectMapper()
			End If

			Try
				Dim type As String = objectMapper.readValue(Unirest.get(String.Format("http://{0}:{1:D}/opType", masterStatusHost, masterStatusPort)).asJson().getBody().ToString(), GetType(ServerTypeJson)).getType()
				If Not type.Equals("master") Then
					Throw New System.InvalidOperationException("Wrong opType " & type)
				End If
				Unirest.get(String.Format("http://{0}:{1:D}/started", masterStatusHost, masterStatusPort)).asJson().getBody()
				Return objectMapper.readValue(Unirest.get(String.Format("http://{0}:{1:D}/started", masterStatusHost, masterStatusPort)).asJson().getBody().ToString(), GetType(MasterStatus)).started()
			Catch e As Exception
				log.error("",e)
			End Try
			Return False
		End Function



		''' <summary>
		''' Push an ndarray message to the specified
		''' ndarray send url in the form of:
		''' host;port:stream
		''' where stream is the stream for connecting
		''' to a listening aeron server </summary>
		''' <param name="message"> the array to send </param>
		Public Overridable Sub pushNDArrayMessage(ByVal message As NDArrayMessage)
			'start a subscriber that can send us ndarrays
			If subscriber Is Nothing Then
				running = New AtomicBoolean(True)
				subscriber = AeronNDArraySubscriber.startSubscriber(aeron, subscriberHost, subscriberPort, Me, subscriberStream, running)
				log.debug("Started parameter server client on " & subscriber.connectionUrl())
			End If

			Dim split() As String = ndarraySendUrl.Split(":", True)
			Dim port As Integer = Integer.Parse(split(1))
			Dim streamToPublish As Integer = Integer.Parse(split(2))
			Dim channel As String = AeronUtil.aeronChannel(split(0), port)
			log.debug("Parameter server client publishing to " & ndarraySendUrl)
			Try
					Using publisher As AeronNDArrayPublisher = AeronNDArrayPublisher.builder().streamId(streamToPublish).compress(isCompressArray()).aeron(aeron).channel(channel).build()
					publisher.publish(message)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Sub

		''' <summary>
		''' Push an ndarray to the specified
		''' ndarray send url in the form of:
		''' host;port:stream
		''' where stream is the stream for connecting
		''' to a listening aeron server </summary>
		''' <param name="arr"> the array to send </param>
		Public Overridable Sub pushNDArray(ByVal arr As INDArray)
			pushNDArrayMessage(NDArrayMessage.wholeArrayUpdate(arr))
		End Sub


		''' <summary>
		''' Get the connection url for the subscriber
		''' in the format:
		''' host:port:stream </summary>
		''' <returns> the connection url for the subscriber
		''' for this client </returns>
		Public Overridable Function connectionUrl() As String
			Return AeronConnectionInformation.of(subscriberHost, subscriberPort, subscriberStream).ToString()
		End Function



		''' <summary>
		'''  Get an ndarray from the
		'''  designated ndarray retrieve url.
		'''  This will "pull" the current ndarray
		'''  from the master </summary>
		''' <returns> the current ndarray from the master. </returns>
		Public Overridable ReadOnly Property Array As INDArray
			Get
				'start a subscriber that can send us ndarrays
				If subscriber Is Nothing Then
					running = New AtomicBoolean(True)
					subscriber = AeronNDArraySubscriber.startSubscriber(aeron, subscriberHost, subscriberPort, Me, subscriberStream, running)
					log.debug("Started parameter server client on " & subscriber.connectionUrl())
				End If
    
				If arr Is Nothing Then
					arr = New AtomicReference(Of INDArray)(none)
				End If
    
				log.debug("Parameter server client retrieving url from " & ndarrayRetrieveUrl)
				'note here that this is the "master url"
				Dim split() As String = ndarrayRetrieveUrl.Split(":", True)
				'The response daemon is always the master daemon's port + 1
				'A "master daemon" is one that holds both the
				'parameter averaging daemon AND the response daemon for being able to send
				'the "current state ndarray"
				Dim port As Integer = Integer.Parse(split(1))
				Dim streamToPublish As Integer = Integer.Parse(split(2))
				'the channel here is the master node host with the port + 1
				'pointing at the response node where we can request ndarrays to be sent to
				'the listening daemon
				Dim channel As String = AeronUtil.aeronChannel(split(0), port)
				'publish the address of our subscriber
				'note here that we send the ndarray send url, because the
				'master also hosts
				Try
						Using hostPortPublisher As HostPortPublisher = HostPortPublisher.builder().channel(channel).aeron(aeron).streamId(streamToPublish).uriToSend(AeronConnectionInformation.of(subscriberHost, subscriberPort, subscriberStream).ToString()).build()
						hostPortPublisher.send()
            
            
						log.debug("Sent subscriber information " & AeronConnectionInformation.of(subscriberHost, subscriberPort, subscriberStream).ToString())
            
						'wait for array to be available
						Do While arr.get() Is none
							Thread.Sleep(1000)
							log.info("Waiting on array to be updated.")
						Loop
            
						End Using
				Catch e As Exception
					log.error("Error with publishing", e)
				End Try
    
    
				Dim arr2 As INDArray = arr.get()
				arr.set(none)
				Return arr2
			End Get
		End Property

		''' <summary>
		''' A listener for ndarray message
		''' </summary>
		''' <param name="message"> the message for the callback </param>
		Public Overridable Sub onNDArrayMessage(ByVal message As NDArrayMessage) Implements NDArrayCallback.onNDArrayMessage
			Dim arr As INDArray = message.getArr()
			'of note for ndarrays
			Dim dimensions() As Integer = message.getDimensions()
			Dim whole As Boolean = dimensions.Length = 1 AndAlso dimensions(0) = -1

			If Not whole Then
				onNDArrayPartial(arr, message.getIndex(), dimensions)
			Else
				onNDArray(arr)
			End If
		End Sub

		''' <summary>
		''' Used for partial updates using tensor along
		''' dimension </summary>
		'''  <param name="arr">        the array to count as an update </param>
		''' <param name="idx">        the index for the tensor along dimension </param>
		''' <param name="dimensions"> the dimensions to act on for the tensor along dimension </param>
		Public Overridable Sub onNDArrayPartial(ByVal arr As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer) Implements NDArrayCallback.onNDArrayPartial
			Dim get As INDArray = Me.arr.get()
			get.tensorAlongDimension(CInt(idx), dimensions).assign(arr)

		End Sub

		''' <summary>
		''' Setup an ndarray
		''' </summary>
		''' <param name="arr"> </param>
		Public Overridable Sub onNDArray(ByVal arr As INDArray) Implements NDArrayCallback.onNDArray
			log.info("Received array")
			Me.arr.set(arr)
		End Sub
	End Class

End Namespace