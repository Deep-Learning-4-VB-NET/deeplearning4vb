Imports System.Collections.Generic
Imports System.Threading
Imports Consumer = io.reactivex.functions.Consumer
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports MeshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode
Imports GradientsUpdateMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.GradientsUpdateMessage
Imports ModelParametersMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersMessage
Imports ModelParametersRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersRequest
Imports UpdaterParametersMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersMessage
Imports UpdaterParametersRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersRequest
Imports org.nd4j.parameterserver.distributed.v2.transport
Imports DelayedDummyTransport = org.nd4j.parameterserver.distributed.v2.transport.impl.DelayedDummyTransport
Imports DummyTransport = org.nd4j.parameterserver.distributed.v2.transport.impl.DummyTransport
Imports org.nd4j.parameterserver.distributed.v2.util
Imports AbstractUpdatesHandler = org.nd4j.parameterserver.distributed.v2.util.AbstractUpdatesHandler
Imports MessageSplitter = org.nd4j.parameterserver.distributed.v2.util.MessageSplitter
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.parameterserver.distributed.v2


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class DelayedModelParameterServerTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class DelayedModelParameterServerTest
		Inherits BaseND4JTest

		Private Const rootId As String = "ROOT_NODE"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			MessageSplitter.Instance.reset()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void setDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setDown()
			MessageSplitter.Instance.reset()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000L) public void testBasicInitialization_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasicInitialization_1()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DelayedDummyTransport(rootId, connector)

			connector.register(rootTransport)

			Dim rootServer As val = New ModelParameterServer(rootTransport, True)
			rootServer.launch()

			assertEquals(rootId, rootTransport.getUpstreamId())

			rootServer.shutdown()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(40000L) public void testBasicInitialization_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasicInitialization_2()
			For e As Integer = 0 To 99
				Dim connector As val = New DummyTransport.Connector()
				Dim rootTransport As val = New DelayedDummyTransport(rootId, connector)
				Dim clientTransportA As val = New DelayedDummyTransport("123", connector, rootId)
				Dim clientTransportB As val = New DelayedDummyTransport("1234", connector, rootId)

				connector.register(rootTransport, clientTransportA, clientTransportB)

				Dim rootServer As val = New ModelParameterServer(rootTransport, True)
				Dim clientServerA As val = New ModelParameterServer(clientTransportA, False)
				Dim clientServerB As val = New ModelParameterServer(clientTransportB, False)
				rootServer.launch()
				clientServerA.launch()
				clientServerB.launch()

				' since clientB starts AFTER clientA, we have to wait till MeshUpdate message is propagated, since ithis message is NOT blocking
				Thread.Sleep(25)

				Dim meshR As val = rootTransport.getMesh()
				Dim meshA As val = clientTransportA.getMesh()
				Dim meshB As val = clientTransportB.getMesh()

				assertEquals(3, meshR.totalNodes(),"Root node failed")
				assertEquals(3, meshB.totalNodes(),"B node failed")
				assertEquals(3, meshA.totalNodes(),"A node failed")
				assertEquals(meshR, meshA)
				assertEquals(meshA, meshB)

				log.info("Iteration [{}] finished", e)
			Next e
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(180000L) public void testUpdatesPropagation_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUpdatesPropagation_1()
			Dim conf As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.PLAIN).build()
			Dim array As val = Nd4j.ones(10, 10)

			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DelayedDummyTransport(rootId, connector, rootId, conf)
			Dim clientTransportA As val = New DelayedDummyTransport("412334", connector, rootId, conf)
			Dim clientTransportB As val = New DelayedDummyTransport("123441", connector, rootId, conf)

			connector.register(rootTransport, clientTransportA, clientTransportB)

			Dim rootServer As val = New ModelParameterServer(rootTransport, True)
			Dim clientServerA As val = New ModelParameterServer(clientTransportA, False)
			Dim clientServerB As val = New ModelParameterServer(clientTransportB, False)
			rootServer.launch()
			clientServerA.launch()
			clientServerB.launch()

			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DelayedDummyTransport)()
			For e As Integer = 0 To 127
				Dim clientTransport As val = New DelayedDummyTransport(e.ToString(), connector, rootId, conf)
				Dim clientServer As val = New ModelParameterServer(clientTransport, False)

				connector.register(clientTransport)
				servers.add(clientServer)
				transports.add(clientTransport)

				clientServer.launch()

				'log.info("Server [{}] started...", e);
			Next e
			connector.blockUntilFinished()

			' 259 == 256 + A+B+R
			assertEquals(servers.size() + 3, rootTransport.getMesh().totalNodes())

			clientServerA.sendUpdate(array)

			connector.blockUntilFinished()

			Dim updatesR As val = rootServer.getUpdates()
			Dim updatesA As val = clientServerA.getUpdates()
			Dim updatesB As val = clientServerB.getUpdates()

			assertEquals(1, updatesR.size())
			assertEquals(1, updatesB.size())

			' we should NOT get this message back to A
			assertEquals(0, updatesA.size())

			For e As Integer = 0 To servers.size() - 1
				Dim s As val = servers.get(e)
				assertEquals(1, s.getUpdates().size(),"Failed at node [" & e & "]")
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(180000L) public void testModelAndUpdaterParamsUpdate_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelAndUpdaterParamsUpdate_1()
			Dim config As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.PLAIN).build()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DelayedDummyTransport(rootId, connector, rootId, config)
			rootTransport.addRequestConsumer(GetType(ModelParametersRequest), New ConsumerAnonymousInnerClass(Me, rootTransport))

			rootTransport.addRequestConsumer(GetType(UpdaterParametersRequest), New ConsumerAnonymousInnerClass2(Me, rootTransport))


			Dim updatedModel As val = New AtomicBoolean(False)
			Dim updatedUpdater As val = New AtomicBoolean(False)
			Dim gotGradients As val = New AtomicBoolean(False)

			connector.register(rootTransport)

			Dim counters As val = New AtomicInteger(127){}
			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DummyTransport)()
			For e As Integer = 0 To 127
				Dim clientTransport As val = New DelayedDummyTransport(System.Guid.randomUUID().ToString(), connector, rootId, config)
				Dim clientServer As val = New ModelParameterServer(config, clientTransport, False)

				counters(e) = New AtomicInteger(0)

				Dim f As val = e

				clientServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass(Me, counters, f))

				servers.add(clientServer)
				transports.add(clientTransport)

				connector.register(clientTransport)

				clientServer.launch()
				'log.info("Client [{}] started", e );
			Next e

			Thread.Sleep(100)
			Dim rootMesh As val = rootTransport.getMesh()

			' now we're picking one server that'll play bad role
			Dim badServer As val = servers.get(23)
			Dim badTransport As val = transports.get(23)
			Dim badId As val = badTransport.id()
			Dim badNode As val = rootMesh.getNodeById(badId)

			Dim upstreamId As val = badNode.getUpstreamNode().getId()
			log.info("Upstream: [{}]; Number of downstreams: [{}]", upstreamId, badNode.numberOfDownstreams())

			connector.dropConnection(badId)
			Dim clientTransport As val = New DummyTransport(badId, connector, rootId)
			Dim clientServer As val = New ModelParameterServer(clientTransport, False)

			clientServer.addUpdaterParamsSubscriber(New AbstractSubscriberAnonymousInnerClass(Me, updatedUpdater))

			clientServer.addModelParamsSubscriber(New AbstractSubscriberAnonymousInnerClass2(Me, updatedModel))

			clientServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass2(Me, gotGradients))

			connector.register(clientTransport)

			clientServer.launch()

			connector.blockUntilFinished()

			' getting any server
			Dim serv As val = servers.get(96)
			serv.sendUpdate(Nd4j.linspace(1, 10, 100).reshape(ChrW(10), 10))

			connector.blockUntilFinished()

			For e As Integer = 0 To 127
				' we're skipping node 23 since it was reconnected, and has different MPS instance
				' and node 96, since it sends update
				If e <> 23 AndAlso e <> 96 Then
					assertEquals(1, counters(e).get(),"Failed at node: [" & e & "]")
				End If
			Next e

			assertTrue(updatedModel.get())
			assertTrue(updatedUpdater.get())
			assertTrue(gotGradients.get())
		End Sub

		Private Class ConsumerAnonymousInnerClass
			Inherits Consumer(Of ModelParametersRequest)

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal rootTransport As val)
				Me.outerInstance = outerInstance
				Me.rootTransport = rootTransport
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersRequest modelParametersRequest) throws Exception
			Public Overrides Sub accept(ByVal modelParametersRequest As ModelParametersRequest)
				Dim msg As val = New ModelParametersMessage("123", Nd4j.create(10))
				msg.setRequestId(modelParametersRequest.RequestId)
				rootTransport.sendMessage(msg, modelParametersRequest.OriginatorId)
			End Sub
		End Class

		Private Class ConsumerAnonymousInnerClass2
			Inherits Consumer(Of UpdaterParametersRequest)

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal rootTransport As val)
				Me.outerInstance = outerInstance
				Me.rootTransport = rootTransport
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersRequest updatersParametersRequest) throws Exception
			Public Overrides Sub accept(ByVal updatersParametersRequest As UpdaterParametersRequest)
				Dim msg As val = New UpdaterParametersMessage("123", Nd4j.create(10))
				msg.setRequestId(updatersParametersRequest.RequestId)
				rootTransport.sendMessage(msg, updatersParametersRequest.OriginatorId)
			End Sub
		End Class

		Private Class AbstractUpdatesHandlerAnonymousInnerClass
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private counters As val
			Private f As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal counters As val, ByVal f As val)
				Me.outerInstance = outerInstance
				Me.counters = counters
				Me.f = f
			End Sub

			Public Overrides ReadOnly Property ParametersArray As INDArray
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				counters(f).incrementAndGet()
			End Sub
		End Class

		Private Class AbstractSubscriberAnonymousInnerClass
			Inherits AbstractSubscriber(Of INDArray)

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private updatedUpdater As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal updatedUpdater As val)
				Me.outerInstance = outerInstance
				Me.updatedUpdater = updatedUpdater
			End Sub

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				updatedUpdater.set(True)
			End Sub
		End Class

		Private Class AbstractSubscriberAnonymousInnerClass2
			Inherits AbstractSubscriber(Of INDArray)

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private updatedModel As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal updatedModel As val)
				Me.outerInstance = outerInstance
				Me.updatedModel = updatedModel
			End Sub

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				updatedModel.set(True)
			End Sub
		End Class

		Private Class AbstractUpdatesHandlerAnonymousInnerClass2
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private gotGradients As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal gotGradients As val)
				Me.outerInstance = outerInstance
				Me.gotGradients = gotGradients
			End Sub

			Public Overrides ReadOnly Property ParametersArray As INDArray
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				assertEquals(Nd4j.linspace(1, 10, 100).reshape(ChrW(10), 10), array)
				gotGradients.set(True)
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(180000L) public void testMeshConsistency_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMeshConsistency_1()
			Nd4j.create(1)
			Const numMessages As Integer = 500
			Dim rootCount As val = New AtomicInteger(0)
			Dim rootSum As val = New AtomicInteger(0)
			Dim counter As val = New AtomicInteger(0)
			Dim sum As val = New AtomicInteger(0)
			Dim config As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.PLAIN).build()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DelayedDummyTransport(rootId, connector, rootId, config)

			rootTransport.addPrecursor(GetType(GradientsUpdateMessage), New MessageCallableAnonymousInnerClass(Me, rootCount, rootSum))
			connector.register(rootTransport)

			Dim counters As val = New AtomicInteger(15){}
			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DummyTransport)()
			For e As Integer = 0 To 15
				Dim clientTransport As val = New DelayedDummyTransport(System.Guid.randomUUID().ToString(), connector, rootId, config)
				Dim clientServer As val = New ModelParameterServer(config, clientTransport, False)

				Dim f As val = e
				counters(f) = New AtomicInteger(0)
				clientServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass3(Me, counters, f))

				servers.add(clientServer)
				transports.add(clientTransport)

				connector.register(clientTransport)

				clientServer.launch()
				'log.info("Client [{}] started", e );
			Next e


			Dim deductions As val = New Integer(servers.size() - 1){}
			For e As Integer = 0 To numMessages - 1
				Dim f As val = RandomUtils.nextInt(0, servers.size())
				Dim server As val = servers.get(f)

				' later we'll reduce this number from expected number of updates
				deductions(f) += 1

				server.sendUpdate(Nd4j.create(5).assign(e))
				sum.addAndGet(e)
			Next e

			connector.blockUntilFinished()

			' checking if master node got all updates we've sent
			assertEquals(numMessages, rootCount.get())
			assertEquals(sum.get(), rootSum.get())

			' now we're checking all nodes, they should get numMessages - messages that were sent through them
			For e As Integer = 0 To servers.size() - 1
				Dim server As val = servers.get(e)
				assertEquals(numMessages - deductions(e), counters(e).get(),"Failed at node: [" & e & "]")
			Next e
		End Sub

		Private Class MessageCallableAnonymousInnerClass
			Implements MessageCallable(Of GradientsUpdateMessage)

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private rootCount As val
			Private rootSum As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal rootCount As val, ByVal rootSum As val)
				Me.outerInstance = outerInstance
				Me.rootCount = rootCount
				Me.rootSum = rootSum
			End Sub

			Public Sub apply(ByVal message As GradientsUpdateMessage)
				Dim array As val = message.Payload
				rootSum.addAndGet(array.meanNumber().intValue())
				rootCount.incrementAndGet()
			End Sub
		End Class

		Private Class AbstractUpdatesHandlerAnonymousInnerClass3
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private counters As val
			Private f As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal counters As val, ByVal f As val)
				Me.outerInstance = outerInstance
				Me.counters = counters
				Me.f = f
			End Sub

			Public Overrides ReadOnly Property ParametersArray As INDArray
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				counters(f).incrementAndGet()
			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(180000L) public void testMeshConsistency_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMeshConsistency_2()
			Nd4j.create(1)
			Const numMessages As Integer = 100
			Dim rootCount As val = New AtomicInteger(0)
			Dim rootSum As val = New AtomicInteger(0)
			Dim counter As val = New AtomicInteger(0)
			Dim sum As val = New AtomicInteger(0)
			Dim config As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.MESH).build()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DelayedDummyTransport(rootId, connector, rootId, config)

			rootTransport.addPrecursor(GetType(GradientsUpdateMessage), New MessageCallableAnonymousInnerClass2(Me, rootCount, rootSum))
			connector.register(rootTransport)

			Dim counters As val = New AtomicInteger(15){}
			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DummyTransport)()
			For e As Integer = 0 To 15
				Dim clientTransport As val = New DelayedDummyTransport(System.Guid.randomUUID().ToString(), connector, rootId, config)
				Dim clientServer As val = New ModelParameterServer(config, clientTransport, False)

				Dim f As val = e
				counters(f) = New AtomicInteger(0)
				clientServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass4(Me, counters, f))

				servers.add(clientServer)
				transports.add(clientTransport)

				connector.register(clientTransport)

				clientServer.launch()
				'log.info("Client [{}] started", e );
			Next e

			Thread.Sleep(500)


			Dim deductions As val = New Integer(servers.size() - 1){}
			For e As Integer = 0 To numMessages - 1
				Dim f As val = RandomUtils.nextInt(0, servers.size())
				Dim server As val = servers.get(f)

				' later we'll reduce this number from expected number of updates
				deductions(f) += 1

				server.sendUpdate(Nd4j.create(5).assign(e))
				sum.addAndGet(e)
			Next e

			connector.blockUntilFinished()

			'Thread.sleep(1000);
			'Thread.sleep(3000000000000L);

			' checking if master node got all updates we've sent
			assertEquals(numMessages, rootCount.get())
			assertEquals(sum.get(), rootSum.get())

			' now we're checking all nodes, they should get numMessages - messages that were sent through them
			For e As Integer = 0 To servers.size() - 1
				Dim server As val = servers.get(e)
				assertEquals(numMessages - deductions(e), counters(e).get(),"Failed at node: [" & e & "]")
			Next e
		End Sub

		Private Class MessageCallableAnonymousInnerClass2
			Implements MessageCallable(Of GradientsUpdateMessage)

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private rootCount As val
			Private rootSum As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal rootCount As val, ByVal rootSum As val)
				Me.outerInstance = outerInstance
				Me.rootCount = rootCount
				Me.rootSum = rootSum
			End Sub

			Public Sub apply(ByVal message As GradientsUpdateMessage)
				Dim array As val = message.Payload
				rootSum.addAndGet(array.meanNumber().intValue())
				rootCount.incrementAndGet()
			End Sub
		End Class

		Private Class AbstractUpdatesHandlerAnonymousInnerClass4
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As DelayedModelParameterServerTest

			Private counters As val
			Private f As val

			Public Sub New(ByVal outerInstance As DelayedModelParameterServerTest, ByVal counters As val, ByVal f As val)
				Me.outerInstance = outerInstance
				Me.counters = counters
				Me.f = f
			End Sub

			Public Overrides ReadOnly Property ParametersArray As INDArray
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				counters(f).incrementAndGet()
			End Sub
		End Class
	End Class

End Namespace