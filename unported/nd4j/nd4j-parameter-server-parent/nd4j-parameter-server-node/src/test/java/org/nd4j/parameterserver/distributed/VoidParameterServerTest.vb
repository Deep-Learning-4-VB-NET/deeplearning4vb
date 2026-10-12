Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports WordVectorStorage = org.nd4j.parameterserver.distributed.logic.storage.WordVectorStorage
Imports org.nd4j.parameterserver.distributed.messages
Imports DotAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.DotAggregation
Imports AssignRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.AssignRequestMessage
Imports InitializationRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.InitializationRequestMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports VectorRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.VectorRequestMessage
Imports DistributedAssignMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedAssignMessage
Imports DistributedSgDotMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedSgDotMessage
Imports DistributedInitializationMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedInitializationMessage
Imports DistributedSolidMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedSolidMessage
Imports SkipGramTrainer = org.nd4j.parameterserver.distributed.training.impl.SkipGramTrainer
Imports MulticastTransport = org.nd4j.parameterserver.distributed.transport.MulticastTransport
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport
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

Namespace org.nd4j.parameterserver.distributed


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class VoidParameterServerTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class VoidParameterServerTest
		Inherits BaseND4JTest

		Private Shared localIPs As IList(Of String)
		Private Shared badIPs As IList(Of String)
		Private Shared ReadOnly transport As Transport = New MulticastTransport()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			If localIPs Is Nothing Then
				localIPs = New List(Of String)(VoidParameterServer.getLocalAddresses())

				badIPs = New List(Of String) From {"127.0.0.1"}
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tearDown()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000L) public void testNodeRole1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNodeRole1()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration conf = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).multicastNetwork("224.0.1.1").shardAddresses(localIPs).ttl(4).build();
			Dim conf As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).multicastNetwork("224.0.1.1").shardAddresses(localIPs).ttl(4).build()
			conf.setUnicastControllerPort(34567)

			Dim node As New VoidParameterServer()
			node.init(conf, transport, New SkipGramTrainer())

			assertEquals(NodeRole.SHARD, node.getNodeRole())
			node.shutdown()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000L) public void testNodeRole2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNodeRole2()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration conf = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).shardAddresses(badIPs).backupAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim conf As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).shardAddresses(badIPs).backupAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			conf.setUnicastControllerPort(34567)

			Dim node As New VoidParameterServer()
			node.init(conf, transport, New SkipGramTrainer())

			assertEquals(NodeRole.BACKUP, node.getNodeRole())
			node.shutdown()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000L) public void testNodeRole3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNodeRole3()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration conf = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).shardAddresses(badIPs).backupAddresses(badIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim conf As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).shardAddresses(badIPs).backupAddresses(badIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			conf.setUnicastControllerPort(34567)

			Dim node As New VoidParameterServer()
			node.init(conf, transport, New SkipGramTrainer())

			assertEquals(NodeRole.CLIENT, node.getNodeRole())
			node.shutdown()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testNodeInitialization1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNodeInitialization1()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger failCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim failCnt As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger passCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim passCnt As New AtomicInteger(0)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration conf = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim conf As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(10).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			conf.setUnicastControllerPort(34567)

			Dim threads(9) As Thread
			For t As Integer = 0 To threads.Length - 1
				threads(t) = New Thread(Sub()
				Dim node As New VoidParameterServer()
				node.init(conf, transport, New SkipGramTrainer())

				If node.getNodeRole() <> NodeRole.SHARD Then
					failCnt.incrementAndGet()
				End If

				passCnt.incrementAndGet()

				node.shutdown()
				End Sub)

				threads(t).Start()
			Next t


			For t As Integer = 0 To threads.Length - 1
				threads(t).Join()
			Next t

			assertEquals(0, failCnt.get())
			assertEquals(threads.Length, passCnt.get())
		End Sub

		''' <summary>
		''' This is very important test, it covers basic messages handling over network.
		''' Here we have 1 client, 1 connected Shard + 2 shards available over multicast UDP
		''' 
		''' PLEASE NOTE: This test uses manual stepping through messages
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testNodeInitialization2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNodeInitialization2()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger failCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim failCnt As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger passCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim passCnt As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger startCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim startCnt As New AtomicInteger(0)

			Dim exp As INDArray = Nd4j.create(New Double() {0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00})


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration clientConf = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).shardAddresses(localIPs).multicastNetwork("224.0.1.1").streamId(119).forcedRole(org.nd4j.parameterserver.distributed.enums.NodeRole.CLIENT).ttl(4).build();
			Dim clientConf As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).shardAddresses(localIPs).multicastNetwork("224.0.1.1").streamId(119).forcedRole(NodeRole.CLIENT).ttl(4).build()
			clientConf.setUnicastControllerPort(34567)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration shardConf1 = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim shardConf1 As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			shardConf1.setUnicastControllerPort(34568)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration shardConf2 = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim shardConf2 As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			shardConf2.setUnicastControllerPort(34569) ' we'll never get anything on this port

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration shardConf3 = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim shardConf3 As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			shardConf3.setUnicastControllerPort(34570) ' we'll never get anything on this port



			Dim clientNode As New VoidParameterServer(True)
			clientNode.ShardIndex = CShort(0)
			clientNode.init(clientConf)
			clientNode.Transport.launch(Transport.ThreadingModel.DEDICATED_THREADS)


			assertEquals(NodeRole.CLIENT, clientNode.getNodeRole())


			Dim threads(2) As Thread
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration[] voidConfigurations = new org.nd4j.parameterserver.distributed.conf.VoidConfiguration[] {shardConf1, shardConf2, shardConf3};
			Dim voidConfigurations() As VoidConfiguration = {shardConf1, shardConf2, shardConf3}

			Dim shards(threads.Length - 1) As VoidParameterServer
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int x = t;
				Dim x As Integer = t
				threads(t) = New Thread(Sub()
				shards(x) = New VoidParameterServer(True)
				shards(x).ShardIndex = CShort(x)
				shards(x).init(voidConfigurations(x))
				shards(x).Transport.launch(Transport.ThreadingModel.DEDICATED_THREADS)
				assertEquals(NodeRole.SHARD, shards(x).getNodeRole())
				startCnt.incrementAndGet()
				passCnt.incrementAndGet()
				End Sub)

				threads(t).setDaemon(True)
				threads(t).Start()
			Next t

			' we block until all threads are really started before sending commands
			Do While startCnt.get() < threads.Length
				Thread.Sleep(500)
			Loop

			' give additional time to start handlers
			Thread.Sleep(1000)

			' now we'll send commands from Client, and we'll check how these messages will be handled
			Dim message As DistributedInitializationMessage = DistributedInitializationMessage.builder().numWords(100).columnsPerShard(10).seed(123).useHs(False).useNeg(True).vectorLength(100).build()

			log.info("MessageType: {}", message.MessageType)

			clientNode.Transport.sendMessage(message)

			' at this point each and every shard should already have this message

			' now we check message queue within Shards
			For t As Integer = 0 To threads.Length - 1
				Dim incMessage As VoidMessage = shards(t).Transport.takeMessage()
				assertNotEquals(Nothing, incMessage,"Failed for shard " & t)
				assertEquals(message.MessageType, incMessage.MessageType,"Failed for shard " & t)

				' we should put message back to corresponding
				shards(t).Transport.putMessage(incMessage)
			Next t

	'        
	'            at this moment we're 100% sure that:
	'                1) Client was able to send message to one of shards
	'                2) Selected Shard successfully received message from Client
	'                3) Shard retransmits message to all shards
	'        
	'            Now, we're passing this message to VoidParameterServer manually, and check for execution result
	'        

			For t As Integer = 0 To threads.Length - 1
				Dim incMessage As VoidMessage = shards(t).Transport.takeMessage()
				assertNotEquals(Nothing, incMessage,"Failed for shard " & t)
				shards(t).handleMessage(message)

				''' <summary>
				''' Now we're checking how data storage was initialized
				''' </summary>

				assertEquals(Nothing, shards(t).NegTable)
				assertEquals(Nothing, shards(t).Syn1)


				assertNotEquals(Nothing, shards(t).ExpTable)
				assertNotEquals(Nothing, shards(t).Syn0)
				assertNotEquals(Nothing, shards(t).Syn1Neg)
			Next t


			' now we'll check passing for negTable, but please note - we're not sending it right now
			Dim negTable As INDArray = Nd4j.create(100000).assign(12.0f)
			Dim negMessage As New DistributedSolidMessage(WordVectorStorage.NEGATIVE_TABLE, negTable, False)

			For t As Integer = 0 To threads.Length - 1
				shards(t).handleMessage(negMessage)

				assertNotEquals(Nothing, shards(t).NegTable)
				assertEquals(negTable, shards(t).NegTable)
			Next t


			' now we assign each row to something
			For t As Integer = 0 To threads.Length - 1
				shards(t).handleMessage(New DistributedAssignMessage(WordVectorStorage.SYN_0, 1, CDbl(t)))

				assertEquals(Nd4j.create(message.getColumnsPerShard()).assign(CDbl(t)), shards(t).Syn0.getRow(1))
			Next t


			' and now we'll request for aggregated vector for row 1
			clientNode.getVector(1)
			Dim vecm As VoidMessage = shards(0).Transport.takeMessage()

			assertEquals(7, vecm.MessageType)

			Dim vrm As VectorRequestMessage = DirectCast(vecm, VectorRequestMessage)

			assertEquals(1, vrm.getRowIndex())

			shards(0).handleMessage(vecm)

			Thread.Sleep(100)

			' at this moment all 3 shards should already have distributed message
			For t As Integer = 0 To threads.Length - 1
				Dim dm As VoidMessage = shards(t).Transport.takeMessage()

				assertEquals(20, dm.MessageType)

				shards(t).handleMessage(dm)
			Next t

			' at this moment we should have messages propagated across all shards
			Thread.Sleep(100)

			For t As Integer = threads.Length - 1 To 0 Step -1
				Dim msg As VoidMessage
				msg = shards(t).Transport.takeMessage()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((msg = shards[t].getTransport().takeMessage()) != null)
				Do While msg IsNot Nothing
					shards(t).handleMessage(msg)
						msg = shards(t).Transport.takeMessage()
				Loop
			Next t

			' and at this moment, Shard_0 should contain aggregated vector for us
			assertEquals(True, shards(0).clipboard.isTracking(0L, 1L))
			assertEquals(True, shards(0).clipboard.isReady(0L, 1L))

			Dim jointVector As INDArray = shards(0).clipboard.nextCandidate().AccumulatedResult

			log.info("Joint vector: {}", jointVector)

			assertEquals(exp, jointVector)


			''' <summary>
			''' now we're going to test real SkipGram round
			''' </summary>
			' first, we're setting data to something predefined
			For t As Integer = 0 To threads.Length - 1
				shards(t).handleMessage(New DistributedAssignMessage(WordVectorStorage.SYN_0, 0, 0.0))
				shards(t).handleMessage(New DistributedAssignMessage(WordVectorStorage.SYN_0, 1, 1.0))
				shards(t).handleMessage(New DistributedAssignMessage(WordVectorStorage.SYN_0, 2, 2.0))

				shards(t).handleMessage(New DistributedAssignMessage(WordVectorStorage.SYN_1_NEGATIVE, 0, 0.0))
				shards(t).handleMessage(New DistributedAssignMessage(WordVectorStorage.SYN_1_NEGATIVE, 1, 1.0))
				shards(t).handleMessage(New DistributedAssignMessage(WordVectorStorage.SYN_1_NEGATIVE, 2, 2.0))
			Next t

			Dim ddot As New DistributedSgDotMessage(2L, New Integer() {0, 1, 2}, New Integer() {0, 1, 2}, 0, 1, New SByte() {0, 1}, True, CShort(0), 0.01f)
			For t As Integer = 0 To threads.Length - 1
				shards(t).handleMessage(ddot)
			Next t

			Thread.Sleep(100)

			For t As Integer = threads.Length - 1 To 0 Step -1
				Dim msg As VoidMessage
				msg = shards(t).Transport.takeMessage()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((msg = shards[t].getTransport().takeMessage()) != null)
				Do While msg IsNot Nothing
					shards(t).handleMessage(msg)
						msg = shards(t).Transport.takeMessage()
				Loop
			Next t


			' at this moment ot should be caclulated everywhere
			exp = Nd4j.create(New Double() {0.0, 30.0, 120.0})
			For t As Integer = 0 To threads.Length - 1
				assertEquals(True, shards(t).clipboard.isReady(0L, 2L))
				Dim dot As DotAggregation = DirectCast(shards(t).clipboard.unpin(0L, 2L), DotAggregation)
				Dim aggregated As INDArray = dot.AccumulatedResult
				assertEquals(exp, aggregated)
			Next t


			For t As Integer = 0 To threads.Length - 1
				threads(t).Join()
			Next t

			For t As Integer = 0 To threads.Length - 1
				shards(t).shutdown()
			Next t

			assertEquals(threads.Length, passCnt.get())


			For Each server As VoidParameterServer In shards
				server.shutdown()
			Next server

			clientNode.shutdown()
		End Sub

		''' 
		''' <summary>
		''' PLEASE NOTE: This test uses automatic feeding through messages
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Timeout(60000L) public void testNodeInitialization3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNodeInitialization3()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger failCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim failCnt As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger passCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim passCnt As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger startCnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim startCnt As New AtomicInteger(0)

			Nd4j.create(1)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration clientConf = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).shardAddresses(localIPs).multicastNetwork("224.0.1.1").streamId(119).forcedRole(org.nd4j.parameterserver.distributed.enums.NodeRole.CLIENT).ttl(4).build();
			Dim clientConf As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).shardAddresses(localIPs).multicastNetwork("224.0.1.1").streamId(119).forcedRole(NodeRole.CLIENT).ttl(4).build()
			clientConf.setUnicastControllerPort(34567)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration shardConf1 = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim shardConf1 As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			shardConf1.setUnicastControllerPort(34567)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration shardConf2 = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim shardConf2 As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			shardConf2.setUnicastControllerPort(34569) ' we'll never get anything on this port

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration shardConf3 = org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build();
			Dim shardConf3 As VoidConfiguration = VoidConfiguration.builder().multicastPort(45678).numberOfShards(3).streamId(119).shardAddresses(localIPs).multicastNetwork("224.0.1.1").ttl(4).build()
			shardConf3.setUnicastControllerPort(34570) ' we'll never get anything on this port



			Dim clientNode As New VoidParameterServer()
			clientNode.ShardIndex = CShort(0)
			clientNode.init(clientConf)
			clientNode.Transport.launch(Transport.ThreadingModel.DEDICATED_THREADS)


			assertEquals(NodeRole.CLIENT, clientNode.getNodeRole())


			Dim threads(2) As Thread
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.conf.VoidConfiguration[] voidConfigurations = new org.nd4j.parameterserver.distributed.conf.VoidConfiguration[] {shardConf1, shardConf2, shardConf3};
			Dim voidConfigurations() As VoidConfiguration = {shardConf1, shardConf2, shardConf3}
			Dim shards(threads.Length - 1) As VoidParameterServer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicBoolean runner = new java.util.concurrent.atomic.AtomicBoolean(true);
			Dim runner As New AtomicBoolean(True)
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int x = t;
				Dim x As Integer = t
				threads(t) = New Thread(Sub()
				shards(x) = New VoidParameterServer()
				shards(x).ShardIndex = CShort(x)
				shards(x).init(voidConfigurations(x))
				shards(x).Transport.launch(Transport.ThreadingModel.DEDICATED_THREADS)
				assertEquals(NodeRole.SHARD, shards(x).getNodeRole())
				startCnt.incrementAndGet()
				Try
					Do While runner.get()
						Thread.Sleep(100)
					Loop
				Catch e As Exception
				End Try
				End Sub)

				threads(t).setDaemon(True)
				threads(t).Start()
			Next t

			' waiting till all shards are initialized
			Do While startCnt.get() < threads.Length
				Thread.Sleep(20)
			Loop


			Dim irm As InitializationRequestMessage = InitializationRequestMessage.builder().numWords(100).columnsPerShard(50).seed(123).useHs(True).useNeg(False).vectorLength(150).build()

			' after this point we'll assume all Shards are initialized
			' mostly because Init message is blocking
			clientNode.Transport.sendMessage(irm)

			log.info("------------------")

			Dim arm As New AssignRequestMessage(WordVectorStorage.SYN_0, 192f, 11)
			clientNode.Transport.sendMessage(arm)

			Thread.Sleep(1000)

			' This is blocking method
			Dim vec As INDArray = clientNode.getVector(WordVectorStorage.SYN_0, 11)

			assertEquals(Nd4j.create(150).assign(192f), vec)

			' now we go for gradients-like test
			' first of all we set exptable to something predictable
			Dim expSyn0 As INDArray = Nd4j.create(150).assign(0.01f)
			Dim expSyn1_1 As INDArray = Nd4j.create(150).assign(0.020005)
			Dim expSyn1_2 As INDArray = Nd4j.create(150).assign(0.019995f)

			Dim expTable As INDArray = Nd4j.create(10000).assign(0.5f)
			Dim expReqMsg As New AssignRequestMessage(WordVectorStorage.EXP_TABLE, expTable)
			clientNode.Transport.sendMessage(expReqMsg)

			arm = New AssignRequestMessage(WordVectorStorage.SYN_0, 0.01, -1)
			clientNode.Transport.sendMessage(arm)

			arm = New AssignRequestMessage(WordVectorStorage.SYN_1, 0.02, -1)
			clientNode.Transport.sendMessage(arm)

			Thread.Sleep(500)
			' no we'll send single SkipGram request that involves calculation for 0 -> {1,2}, and will check result against pre-calculated values

			Dim sgrm As New SkipGramRequestMessage(0, 1, New Integer() {1, 2}, New SByte() {0, 1}, CShort(0), 0.001, 119L)
			clientNode.Transport.sendMessage(sgrm)

			' TODO: we might want to introduce optional CompletedMessage here
			' now we just wait till everything is finished
			Thread.Sleep(1000)


			' This is blocking method
			Dim row_syn0 As INDArray = clientNode.getVector(WordVectorStorage.SYN_0, 0)
			Dim row_syn1_1 As INDArray = clientNode.getVector(WordVectorStorage.SYN_1, 1)
			Dim row_syn1_2 As INDArray = clientNode.getVector(WordVectorStorage.SYN_1, 2)

			assertEquals(expSyn0, row_syn0)
			assertArrayEquals(expSyn1_1.data().asFloat(), row_syn1_1.data().asFloat(), 1e-6f)
			assertArrayEquals(expSyn1_2.data().asFloat(), row_syn1_2.data().asFloat(), 1e-6f)

			runner.set(False)
			For t As Integer = 0 To threads.Length - 1
				threads(t).Join()
			Next t


			For Each server As VoidParameterServer In shards
				server.shutdown()
			Next server

			clientNode.shutdown()
		End Sub
	End Class

End Namespace