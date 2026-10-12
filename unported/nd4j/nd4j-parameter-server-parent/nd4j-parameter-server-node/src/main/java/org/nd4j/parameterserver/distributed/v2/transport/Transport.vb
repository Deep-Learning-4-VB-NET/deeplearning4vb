Imports System
Imports Consumer = io.reactivex.functions.Consumer
Imports PropagationMode = org.nd4j.parameterserver.distributed.v2.enums.PropagationMode
Imports ResponseMessage = org.nd4j.parameterserver.distributed.v2.messages.ResponseMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.v2.messages.RequestMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage
Imports INDArrayMessage = org.nd4j.parameterserver.distributed.v2.messages.INDArrayMessage
Imports MeshOrganizer = org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer
Imports Publisher = org.reactivestreams.Publisher

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

Namespace org.nd4j.parameterserver.distributed.v2.transport


	Public Interface Transport

		''' <summary>
		''' This method returns id of the current transport
		''' @return
		''' </summary>
		Function id() As String

		''' <summary>
		''' This methos returns Id of the upstream node
		''' @return
		''' </summary>
		ReadOnly Property UpstreamId As String

		''' <summary>
		''' This method returns random
		''' </summary>
		''' <param name="id"> </param>
		''' <param name="exclude">
		''' @return </param>
		Function getRandomDownstreamFrom(ByVal id As String, ByVal exclude As String) As String

		''' <summary>
		''' This method returns consumer that accepts messages for delivery
		''' @return
		''' </summary>
		Function outgoingConsumer() As Consumer(Of VoidMessage)

		''' <summary>
		''' This method returns flow of messages for parameter server
		''' @return
		''' </summary>
		Function incomingPublisher() As Publisher(Of INDArrayMessage)

		''' <summary>
		''' This method starts  this Transport instance
		''' </summary>
		Sub launch()

		''' <summary>
		''' This method will start this Transport instance
		''' </summary>
		Sub launchAsMaster()

		''' <summary>
		''' This method shuts down this Transport instance
		''' </summary>
		Sub shutdown()

		''' <summary>
		''' This method will send message to the network, using tree structure </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void propagateMessage(org.nd4j.parameterserver.distributed.v2.messages.VoidMessage message, org.nd4j.parameterserver.distributed.v2.enums.PropagationMode mode) throws java.io.IOException;
		Sub propagateMessage(ByVal message As VoidMessage, ByVal mode As PropagationMode)

		''' <summary>
		''' This method will send message to the node specified by Id
		''' </summary>
		''' <param name="message"> </param>
		''' <param name="id"> </param>
		Sub sendMessage(ByVal message As VoidMessage, ByVal id As String)

		''' <summary>
		''' This method will send message to specified node, and will return its response
		''' </summary>
		''' <param name="message"> </param>
		''' <param name="id"> </param>
		''' @param <T>
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: <T extends org.nd4j.parameterserver.distributed.v2.messages.ResponseMessage> T sendMessageBlocking(org.nd4j.parameterserver.distributed.v2.messages.RequestMessage message, String id) throws InterruptedException;
		 Function sendMessageBlocking(Of T As ResponseMessage)(ByVal message As RequestMessage, ByVal id As String) As T

		''' <summary>
		''' This method will send message to specified node, and will return its response
		''' </summary>
		''' <param name="message"> </param>
		''' <param name="id"> </param>
		''' @param <T>
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: <T extends org.nd4j.parameterserver.distributed.v2.messages.ResponseMessage> T sendMessageBlocking(org.nd4j.parameterserver.distributed.v2.messages.RequestMessage message, String id, long waitTime, java.util.concurrent.TimeUnit timeUnit) throws InterruptedException;
		 Function sendMessageBlocking(Of T As ResponseMessage)(ByVal message As RequestMessage, ByVal id As String, ByVal waitTime As Long, ByVal timeUnit As TimeUnit) As T

		''' <summary>
		''' This method will be invoked for all incoming messages
		''' PLEASE NOTE: this method is mostly suited for tests
		''' </summary>
		''' <param name="message"> </param>
		Sub processMessage(ByVal message As VoidMessage)


		''' <summary>
		''' This method allows to set callback instance, which will be called upon restart event </summary>
		''' <param name="callback"> </param>
		WriteOnly Property RestartCallback As RestartCallback

		''' <summary>
		''' This methd allows to set callback instance for various </summary>
		''' <param name="cls"> </param>
		''' <param name="callback"> </param>
		''' @param <T1> RequestMessage class </param>
		''' @param <T2> ResponseMessage class </param>
		 Sub addRequestConsumer(Of T As RequestMessage)(ByVal cls As Type(Of T), ByVal consumer As Consumer(Of T))

		''' <summary>
		''' This method will be called if mesh update was received
		''' 
		''' PLEASE NOTE: This method will be called ONLY if new mesh differs from current one </summary>
		''' <param name="mesh"> </param>
		Sub onMeshUpdate(ByVal mesh As MeshOrganizer)

		''' <summary>
		''' This method will be called upon remap request </summary>
		''' <param name="id"> </param>
		Sub onRemap(ByVal id As String)

		''' <summary>
		''' This method returns total number of nodes known to this Transport
		''' @return
		''' </summary>
		Function totalNumberOfNodes() As Integer


		''' <summary>
		''' This method returns ID of the root node
		''' @return
		''' </summary>
		ReadOnly Property RootId As String

		''' <summary>
		'''  This method checks if all connections required for work are established </summary>
		''' <returns> true </returns>
		ReadOnly Property Connected As Boolean

		''' <summary>
		''' This method checks if this node was properly introduced to driver
		''' @return
		''' </summary>
		ReadOnly Property Introduced As Boolean

		''' <summary>
		''' This method checks connection to the given node ID, and if it's not connected - establishes connection </summary>
		''' <param name="id"> </param>
		Sub ensureConnection(ByVal id As String)
	End Interface

End Namespace