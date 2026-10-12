Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports BasicSequenceProvider = org.nd4j.parameterserver.distributed.logic.sequence.BasicSequenceProvider
Imports org.nd4j.parameterserver.distributed.messages
Imports CbowRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.CbowRequestMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports ClientRouter = org.nd4j.parameterserver.distributed.logic.ClientRouter
Imports CbowTrainer = org.nd4j.parameterserver.distributed.training.impl.CbowTrainer
Imports SkipGramTrainer = org.nd4j.parameterserver.distributed.training.impl.SkipGramTrainer
Imports MulticastTransport = org.nd4j.parameterserver.distributed.transport.MulticastTransport
Imports RoutedTransport = org.nd4j.parameterserver.distributed.transport.RoutedTransport
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport
Imports InterleavedRouter = org.nd4j.parameterserver.distributed.logic.routing.InterleavedRouter
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
'ORIGINAL LINE: @Slf4j @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class VoidParameterServerStressTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class VoidParameterServerStressTest
		Inherits BaseND4JTest

		Private Const NUM_WORDS As Integer = 100000

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tearDown()

		End Sub

		''' <summary>
		''' This test measures performance of blocking messages processing, VectorRequestMessage in this case
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testPerformanceStandalone1()
		Public Overridable Sub testPerformanceStandalone1()
			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().networkMask("192.168.0.0/16").numberOfShards(1).build()

			voidConfiguration.setShardAddresses("192.168.1.35")

			Dim parameterServer As New VoidParameterServer()

			parameterServer.init(voidConfiguration)
			parameterServer.initializeSeqVec(100, NUM_WORDS, 123, 10, True, False)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Long> times = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim times As IList(Of Long) = New CopyOnWriteArrayList(Of Long)()

			Dim threads(7) As Thread
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int e = t;
				Dim e As Integer = t
				threads(t) = New Thread(Sub()
				Dim results As IList(Of Long) = New List(Of Long)()
				Dim chunk As Integer = NUM_WORDS \ threads.Length
				Dim start As Integer = e * chunk
				Dim [end] As Integer = (e + 1) * chunk
				For i As Integer = 0 To 999999
					Dim time1 As Long = System.nanoTime()
					Dim array As INDArray = parameterServer.getVector(RandomUtils.nextInt(start, [end]))
					Dim time2 As Long = System.nanoTime()
					results.Add(time2 - time1)
					If (i + 1) Mod 1000 = 0 Then
						log.info("Thread {} cnt {}", e, i + 1)
					End If
				Next i
				CType(times, List(Of Long)).AddRange(results)
				End Sub)
				threads(t).setDaemon(True)
				threads(t).Start()
			Next t


			For t As Integer = 0 To threads.Length - 1
				Try
					threads(t).Join()
				Catch e As Exception
				End Try
			Next t

			Dim newTimes As IList(Of Long) = New List(Of Long)(times)

			newTimes.Sort()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			log.info("p50: {} us", newTimes(newTimes.Count \ 2) / 1000)

			parameterServer.shutdown()
		End Sub

		''' <summary>
		''' This test measures performance of non-blocking messages processing, SkipGramRequestMessage in this case
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testPerformanceStandalone2()
		Public Overridable Sub testPerformanceStandalone2()
			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().networkMask("192.168.0.0/16").numberOfShards(1).build()

			voidConfiguration.setShardAddresses("192.168.1.35")

			Dim parameterServer As New VoidParameterServer()

			parameterServer.init(voidConfiguration)
			parameterServer.initializeSeqVec(100, NUM_WORDS, 123, 10, True, False)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Long> times = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim times As IList(Of Long) = New CopyOnWriteArrayList(Of Long)()

			Dim threads(7) As Thread
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int e = t;
				Dim e As Integer = t
				threads(t) = New Thread(Sub()
				Dim results As IList(Of Long) = New List(Of Long)()
				Dim chunk As Integer = NUM_WORDS \ threads.Length
				Dim start As Integer = e * chunk
				Dim [end] As Integer = (e + 1) * chunk
				For i As Integer = 0 To 99999
					Dim sgrm As SkipGramRequestMessage = VoidParameterServerStressTest.SGRM
					Dim time1 As Long = System.nanoTime()
					parameterServer.execDistributed(sgrm)
					Dim time2 As Long = System.nanoTime()
					results.Add(time2 - time1)
					If (i + 1) Mod 1000 = 0 Then
						log.info("Thread {} cnt {}", e, i + 1)
					End If
				Next i
				CType(times, List(Of Long)).AddRange(results)
				End Sub)
				threads(t).setDaemon(True)
				threads(t).Start()
			Next t


			For t As Integer = 0 To threads.Length - 1
				Try
					threads(t).Join()
				Catch e As Exception
				End Try
			Next t

			Dim newTimes As IList(Of Long) = New List(Of Long)(times)

			newTimes.Sort()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			log.info("p50: {} us", newTimes(newTimes.Count \ 2) / 1000)

			parameterServer.shutdown()
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testPerformanceMulticast1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPerformanceMulticast1()
			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().networkMask("192.168.0.0/16").numberOfShards(1).build()

			Dim addresses As IList(Of String) = New List(Of String)()
			For s As Integer = 0 To 4
				addresses.Add("192.168.1.35:3789" & s)
			Next s

			voidConfiguration.setShardAddresses(addresses)
			voidConfiguration.setForcedRole(NodeRole.CLIENT)

			Dim voidConfigurations(4) As VoidConfiguration
			Dim shards(4) As VoidParameterServer
			For s As Integer = 0 To shards.Length - 1
				voidConfigurations(s) = VoidConfiguration.builder().networkMask("192.168.0.0/16").build()
				voidConfigurations(s).setUnicastControllerPort(Convert.ToInt32("3789" & s))

				voidConfigurations(s).setShardAddresses(addresses)

				Dim transport As New MulticastTransport()
				transport.setIpAndPort("192.168.1.35", Convert.ToInt32("3789" & s))
				shards(s) = New VoidParameterServer(False)
				shards(s).ShardIndex = CShort(s)
				shards(s).init(voidConfigurations(s), transport, New SkipGramTrainer())

				assertEquals(NodeRole.SHARD, shards(s).getNodeRole())
			Next s

			' this is going to be our Client shard
			Dim parameterServer As New VoidParameterServer()
			parameterServer.init(voidConfiguration)
			assertEquals(NodeRole.CLIENT, VoidParameterServer.Instance.getNodeRole())

			log.info("Instantiation finished...")

			parameterServer.initializeSeqVec(100, NUM_WORDS, 123, 20, True, False)


			log.info("Initialization finished...")

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Long> times = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim times As IList(Of Long) = New CopyOnWriteArrayList(Of Long)()

			Dim threads(7) As Thread
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int e = t;
				Dim e As Integer = t
				threads(t) = New Thread(Sub()
				Dim results As IList(Of Long) = New List(Of Long)()
				Dim chunk As Integer = NUM_WORDS \ threads.Length
				Dim start As Integer = e * chunk
				Dim [end] As Integer = (e + 1) * chunk
				For i As Integer = 0 To 99999
					Dim time1 As Long = System.nanoTime()
					Dim array As INDArray = parameterServer.getVector(RandomUtils.nextInt(start, [end]))
					Dim time2 As Long = System.nanoTime()
					results.Add(time2 - time1)
					If (i + 1) Mod 1000 = 0 Then
						log.info("Thread {} cnt {}", e, i + 1)
					End If
				Next i
				CType(times, List(Of Long)).AddRange(results)
				End Sub)
				threads(t).setDaemon(True)
				threads(t).Start()
			Next t


			For t As Integer = 0 To threads.Length - 1
				Try
					threads(t).Join()
				Catch e As Exception
				End Try
			Next t

			Dim newTimes As IList(Of Long) = New List(Of Long)(times)

			newTimes.Sort()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			log.info("p50: {} us", newTimes(newTimes.Count \ 2) / 1000)

			parameterServer.shutdown()

			For Each server As VoidParameterServer In shards
				server.shutdown()
			Next server
		End Sub

		''' <summary>
		''' This is one of the MOST IMPORTANT tests
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testPerformanceUnicast1()
		Public Overridable Sub testPerformanceUnicast1()
			Dim list As IList(Of String) = New List(Of String)()
			For t As Integer = 0 To 0
				list.Add("127.0.0.1:3838" & t)
			Next t

			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().numberOfShards(list.Count).shardAddresses(list).build()
			voidConfiguration.setUnicastControllerPort(49823)

			Dim shards(list.Count - 1) As VoidParameterServer
			For t As Integer = 0 To shards.Length - 1
				shards(t) = New VoidParameterServer(NodeRole.SHARD)

				Dim transport As Transport = New RoutedTransport()
				transport.setIpAndPort("127.0.0.1", Convert.ToInt32("3838" & t))

				shards(t).ShardIndex = CShort(t)
				shards(t).init(voidConfiguration, transport, New SkipGramTrainer())


				assertEquals(NodeRole.SHARD, shards(t).getNodeRole())
			Next t

			Dim clientNode As New VoidParameterServer(NodeRole.CLIENT)
			Dim transport As New RoutedTransport()
			Dim router As ClientRouter = New InterleavedRouter(0)

			transport.setRouter(router)
			transport.setIpAndPort("127.0.0.1", voidConfiguration.getUnicastControllerPort())

			router.init(voidConfiguration, transport)

			clientNode.init(voidConfiguration, transport, New SkipGramTrainer())
			assertEquals(NodeRole.CLIENT, clientNode.getNodeRole())

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Long> times = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim times As IList(Of Long) = New CopyOnWriteArrayList(Of Long)()

			' at this point, everything should be started, time for tests
			clientNode.initializeSeqVec(100, NUM_WORDS, 123, 25, True, False)

			log.info("Initialization finished, going to tests...")

			Dim threads(3) As Thread
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int e = t;
				Dim e As Integer = t
				threads(t) = New Thread(Sub()
				Dim results As IList(Of Long) = New List(Of Long)()
				Dim chunk As Integer = NUM_WORDS \ threads.Length
				Dim start As Integer = e * chunk
				Dim [end] As Integer = (e + 1) * chunk
				For i As Integer = 0 To 199
					Dim time1 As Long = System.nanoTime()
					Dim array As INDArray = clientNode.getVector(RandomUtils.nextInt(start, [end]))
					Dim time2 As Long = System.nanoTime()
					results.Add(time2 - time1)
					If (i + 1) Mod 100 = 0 Then
						log.info("Thread {} cnt {}", e, i + 1)
					End If
				Next i
				CType(times, List(Of Long)).AddRange(results)
				End Sub)

				threads(t).setDaemon(True)
				threads(t).Start()
			Next t

			For t As Integer = 0 To threads.Length - 1
				Try
					threads(t).Join()
				Catch e As Exception
				End Try
			Next t

			Dim newTimes As IList(Of Long) = New List(Of Long)(times)

			newTimes.Sort()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			log.info("p50: {} us", newTimes(newTimes.Count \ 2) / 1000)

			' shutdown everything
			For Each shard As VoidParameterServer In shards
				shard.Transport.shutdown()
			Next shard

			clientNode.Transport.shutdown()
		End Sub


		''' <summary>
		''' This is second super-important test for unicast transport.
		''' Here we send non-blocking messages
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testPerformanceUnicast2()
		Public Overridable Sub testPerformanceUnicast2()
			Dim list As IList(Of String) = New List(Of String)()
			For t As Integer = 0 To 4
				list.Add("127.0.0.1:3838" & t)
			Next t

			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().numberOfShards(list.Count).shardAddresses(list).build()
			voidConfiguration.setUnicastControllerPort(49823)

			Dim shards(list.Count - 1) As VoidParameterServer
			For t As Integer = 0 To shards.Length - 1
				shards(t) = New VoidParameterServer(NodeRole.SHARD)

				Dim transport As Transport = New RoutedTransport()
				transport.setIpAndPort("127.0.0.1", Convert.ToInt32("3838" & t))

				shards(t).ShardIndex = CShort(t)
				shards(t).init(voidConfiguration, transport, New SkipGramTrainer())


				assertEquals(NodeRole.SHARD, shards(t).getNodeRole())
			Next t

			Dim clientNode As New VoidParameterServer()
			Dim transport As New RoutedTransport()
			Dim router As ClientRouter = New InterleavedRouter(0)

			transport.setRouter(router)
			transport.setIpAndPort("127.0.0.1", voidConfiguration.getUnicastControllerPort())

			router.init(voidConfiguration, transport)

			clientNode.init(voidConfiguration, transport, New SkipGramTrainer())
			assertEquals(NodeRole.CLIENT, clientNode.getNodeRole())

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Long> times = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim times As IList(Of Long) = New CopyOnWriteArrayList(Of Long)()

			' at this point, everything should be started, time for tests
			clientNode.initializeSeqVec(100, NUM_WORDS, 123, 25, True, False)

			log.info("Initialization finished, going to tests...")

			Dim threads(3) As Thread
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int e = t;
				Dim e As Integer = t
				threads(t) = New Thread(Sub()
				Dim results As IList(Of Long) = New List(Of Long)()
				Dim chunk As Integer = NUM_WORDS \ threads.Length
				Dim start As Integer = e * chunk
				Dim [end] As Integer = (e + 1) * chunk
				For i As Integer = 0 To 199
					Dim frame As New Frame(Of SkipGramRequestMessage)(BasicSequenceProvider.Instance.getNextValue())
					For f As Integer = 0 To 127
						frame.stackMessage(SGRM)
					Next f
					Dim time1 As Long = System.nanoTime()
					clientNode.execDistributed(frame)
					Dim time2 As Long = System.nanoTime()
					results.Add(time2 - time1)
					If (i + 1) Mod 100 = 0 Then
						log.info("Thread {} cnt {}", e, i + 1)
					End If
				Next i
				CType(times, List(Of Long)).AddRange(results)
				End Sub)

				threads(t).setDaemon(True)
				threads(t).Start()
			Next t

			For t As Integer = 0 To threads.Length - 1
				Try
					threads(t).Join()
				Catch e As Exception
				End Try
			Next t

			Dim newTimes As IList(Of Long) = New List(Of Long)(times)

			newTimes.Sort()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			log.info("p50: {} us", newTimes(newTimes.Count \ 2) / 1000)

			' shutdown everything
			For Each shard As VoidParameterServer In shards
				shard.Transport.shutdown()
			Next shard

			clientNode.Transport.shutdown()
		End Sub

		''' <summary>
		''' This test checks for single Shard scenario, when Shard is also a Client
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testPerformanceUnicast3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPerformanceUnicast3()
			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().numberOfShards(1).shardAddresses(Arrays.asList("127.0.0.1:49823")).build()
			voidConfiguration.setUnicastControllerPort(49823)

			Dim transport As Transport = New RoutedTransport()
			transport.setIpAndPort("127.0.0.1", Convert.ToInt32("49823"))

			Dim parameterServer As New VoidParameterServer(NodeRole.SHARD)
			parameterServer.ShardIndex = CShort(0)
			parameterServer.init(voidConfiguration, transport, New CbowTrainer())

			parameterServer.initializeSeqVec(100, NUM_WORDS, 123L, 100, True, False)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Long> times = new java.util.ArrayList<>();
			Dim times As IList(Of Long) = New List(Of Long)()

			log.info("Starting loop...")
			For i As Integer = 0 To 199
				Dim frame As New Frame(Of CbowRequestMessage)(BasicSequenceProvider.Instance.getNextValue())
				For f As Integer = 0 To 127
					frame.stackMessage(CRM)
				Next f
				Dim time1 As Long = System.nanoTime()
				parameterServer.execDistributed(frame)
				Dim time2 As Long = System.nanoTime()

				times.Add(time2 - time1)

				If i Mod 50 = 0 Then
					log.info("{} frames passed...", i)
				End If
			Next i


			times.Sort()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			log.info("p50: {} us", times(times.Count \ 2) / 1000)

			parameterServer.shutdown()
		End Sub

		''' <summary>
		''' This test checks multiple Clients hammering single Shard
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testPerformanceUnicast4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPerformanceUnicast4()
			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().numberOfShards(1).shardAddresses(Arrays.asList("127.0.0.1:49823")).build()
			voidConfiguration.setUnicastControllerPort(49823)

			Dim transport As Transport = New RoutedTransport()
			transport.setIpAndPort("127.0.0.1", Convert.ToInt32("49823"))

			Dim parameterServer As New VoidParameterServer(NodeRole.SHARD)
			parameterServer.ShardIndex = CShort(0)
			parameterServer.init(voidConfiguration, transport, New SkipGramTrainer())

			parameterServer.initializeSeqVec(100, NUM_WORDS, 123L, 100, True, False)


			Dim clients(0) As VoidParameterServer
			For c As Integer = 0 To clients.Length - 1
				clients(c) = New VoidParameterServer(NodeRole.CLIENT)

				Dim clientTransport As Transport = New RoutedTransport()
				clientTransport.setIpAndPort("127.0.0.1", Convert.ToInt32("4872" & c))

				clients(c).init(voidConfiguration, clientTransport, New SkipGramTrainer())

				assertEquals(NodeRole.CLIENT, clients(c).getNodeRole())
			Next c

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Long> times = new java.util.concurrent.CopyOnWriteArrayList<>();
			Dim times As IList(Of Long) = New CopyOnWriteArrayList(Of Long)()
			log.info("Starting loop...")
			Dim threads(clients.Length - 1) As Thread
			For t As Integer = 0 To threads.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int c = t;
				Dim c As Integer = t
				threads(t) = New Thread(Sub()
				Dim results As IList(Of Long) = New List(Of Long)()
				Dim sequence As New AtomicLong(0)
				For i As Integer = 0 To 499
					Dim frame As New Frame(Of SkipGramRequestMessage)(sequence.incrementAndGet())
					For f As Integer = 0 To 127
						frame.stackMessage(SGRM)
					Next f
					Dim time1 As Long = System.nanoTime()
					clients(c).execDistributed(frame)
					Dim time2 As Long = System.nanoTime()
					results.Add(time2 - time1)
					If (i + 1) Mod 50 = 0 Then
						log.info("Thread_{} finished {} frames...", c, i)
					End If
				Next i
				CType(times, List(Of Long)).AddRange(results)
				End Sub)

				threads(t).setDaemon(True)
				threads(t).Start()
			Next t



			For Each thread As Thread In threads
				thread.Join()
			Next thread

			Dim newTimes As IList(Of Long) = New List(Of Long)(times)

			newTimes.Sort()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			log.info("p50: {} us", newTimes(newTimes.Count \ 2) / 1000)

			For Each client As VoidParameterServer In clients
				client.shutdown()
			Next client

			parameterServer.shutdown()
		End Sub

		''' <summary>
		''' This method just produces random SGRM requests, fot testing purposes.
		''' No real sense could be found here.
		''' 
		''' @return
		''' </summary>
		Protected Friend Shared ReadOnly Property SGRM As SkipGramRequestMessage
			Get
				Dim w1 As Integer = RandomUtils.nextInt(0, NUM_WORDS)
				Dim w2 As Integer = RandomUtils.nextInt(0, NUM_WORDS)
    
				Dim codes(RandomUtils.nextInt(15, 45) - 1) As SByte
				Dim points(codes.Length - 1) As Integer
				For e As Integer = 0 To codes.Length - 1
					codes(e) = CSByte(If(e Mod 2 = 0, 0, 1))
					points(e) = RandomUtils.nextInt(0, NUM_WORDS)
				Next e
    
				Return New SkipGramRequestMessage(w1, w2, points, codes, CShort(0), 0.025, 213412L)
			End Get
		End Property


		Protected Friend Shared ReadOnly Property CRM As CbowRequestMessage
			Get
				Dim w1 As Integer = RandomUtils.nextInt(0, NUM_WORDS)
    
				Dim syn0(4) As Integer
    
				For e As Integer = 0 To syn0.Length - 1
					syn0(e) = RandomUtils.nextInt(0, NUM_WORDS)
				Next e
    
				Dim codes(RandomUtils.nextInt(15, 45) - 1) As SByte
				Dim points(codes.Length - 1) As Integer
				For e As Integer = 0 To codes.Length - 1
					codes(e) = CSByte(If(e Mod 2 = 0, 0, 1))
					points(e) = RandomUtils.nextInt(0, NUM_WORDS)
				Next e
    
				Return New CbowRequestMessage(syn0, points, w1, codes, 0, 0.025, 119)
			End Get
		End Property
	End Class

End Namespace