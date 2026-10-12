Imports System.Collections.Generic
Imports System.Threading
Imports Consumer = io.reactivex.functions.Consumer
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports MeshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode
Imports ModelParametersMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersMessage
Imports ModelParametersRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersRequest
Imports UpdaterParametersMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersMessage
Imports UpdaterParametersRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersRequest
Imports UpdaterParametersProvider = org.nd4j.parameterserver.distributed.v2.transport.UpdaterParametersProvider
Imports DummyTransport = org.nd4j.parameterserver.distributed.v2.transport.impl.DummyTransport
Imports org.nd4j.parameterserver.distributed.v2.util
Imports AbstractUpdatesHandler = org.nd4j.parameterserver.distributed.v2.util.AbstractUpdatesHandler
Imports MeshOrganizer = org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer
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

Namespace org.nd4j.parameterserver.distributed.v2


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class ModelParameterServerTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class ModelParameterServerTest
		Inherits BaseND4JTest

		Private Const rootId As String = "ROOT_NODE"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000L) public void testBasicInitialization_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasicInitialization_1()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DummyTransport(rootId, connector)

			connector.register(rootTransport)

			Dim rootServer As val = New ModelParameterServer(rootTransport, True)
			rootServer.launch()

			assertEquals(rootId, rootTransport.getUpstreamId())

			rootServer.shutdown()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000L) public void testBasicInitialization_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasicInitialization_2()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DummyTransport(rootId, connector)
			Dim clientTransportA As val = New DummyTransport("123", connector, rootId)
			Dim clientTransportB As val = New DummyTransport("1234", connector, rootId)

			connector.register(rootTransport, clientTransportA, clientTransportB)

			Dim rootServer As val = New ModelParameterServer(rootTransport, True)
			Dim clientServerA As val = New ModelParameterServer(clientTransportA, False)
			Dim clientServerB As val = New ModelParameterServer(clientTransportB, False)
			rootServer.launch()
			clientServerA.launch()
			clientServerB.launch()

			Dim meshR As val = rootTransport.getMesh()
			Dim meshA As val = clientTransportA.getMesh()
			Dim meshB As val = clientTransportB.getMesh()

			assertEquals(3, meshA.totalNodes())
			assertEquals(meshR, meshA)
			assertEquals(meshA, meshB)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUpdatesPropagation_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUpdatesPropagation_1()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DummyTransport(rootId, connector)
			Dim clientTransportA As val = New DummyTransport("412334", connector, rootId)
			Dim clientTransportB As val = New DummyTransport("123441", connector, rootId)

			connector.register(rootTransport, clientTransportA, clientTransportB)

			Dim rootServer As val = New ModelParameterServer(rootTransport, True)
			Dim clientServerA As val = New ModelParameterServer(clientTransportA, False)
			Dim clientServerB As val = New ModelParameterServer(clientTransportB, False)
			rootServer.launch()
			clientServerA.launch()
			clientServerB.launch()

			Dim array As val = Nd4j.ones(10, 10)
			clientServerA.sendUpdate(array)

			Dim updatesR As val = rootServer.getUpdates()
			Dim updatesA As val = clientServerA.getUpdates()
			Dim updatesB As val = clientServerB.getUpdates()

			assertEquals(1, updatesR.size())
			assertEquals(1, updatesB.size())

			' we should NOT get this message back to A
			assertEquals(0, updatesA.size())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReconnectPropagation_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReconnectPropagation_1()
			Dim config As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.MESH).build()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DummyTransport(rootId, connector, rootId, config)

			connector.register(rootTransport)

			Dim rootServer As val = New ModelParameterServer(config, rootTransport, True)
			rootServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass(Me))
			rootServer.launch()

			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DummyTransport)()
			For e As Integer = 0 To 127
				Dim clientTransport As val = New DummyTransport(System.Guid.randomUUID().ToString(), connector, rootId, config)
				Dim clientServer As val = New ModelParameterServer(config, clientTransport, False)

				servers.add(clientServer)
				transports.add(clientTransport)

				connector.register(clientTransport)

				clientServer.launch()
				'log.info("Client [{}] started", e );
			Next e

			' at this point we should have 2048 nodes within
			Dim rootMesh As val = rootTransport.getMesh()
			Dim originalVersion As val = rootMesh.getVersion()
			assertEquals(128, rootMesh.getVersion())

			' all mesh structures should be equal
			For Each t As val In transports
				assertEquals(rootMesh, t.getMesh())
			Next t

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
			connector.register(clientTransport)

			clientServer.launch()

			' at this point we have re-registered node
			assertNotEquals(originalVersion, rootMesh.getVersion())
			Dim newNode As val = rootMesh.getNodeById(badId)
			Dim newUpstream As val = newNode.getUpstreamNode().getId()

			' after reconnect node should have 0 downstreams and new upstream
			assertNotEquals(upstreamId, newUpstream)
			assertEquals(0, newNode.numberOfDownstreams())
		End Sub

		Private Class AbstractUpdatesHandlerAnonymousInnerClass
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As ModelParameterServerTest

			Public Sub New(ByVal outerInstance As ModelParameterServerTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides ReadOnly Property ParametersArray As INDArray
				Get
					Return Nd4j.create(10, 10)
				End Get
			End Property

			Public Overrides Sub onNext(ByVal array As INDArray)

			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testModelAndUpdaterParamsUpdate_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelAndUpdaterParamsUpdate_1()
			Dim config As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.PLAIN).build()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DummyTransport(rootId, connector, rootId, config)
			rootTransport.addRequestConsumer(GetType(ModelParametersRequest), New ConsumerAnonymousInnerClass(Me, rootTransport))

			rootTransport.addRequestConsumer(GetType(UpdaterParametersRequest), New ConsumerAnonymousInnerClass2(Me, rootTransport))

			Dim updatedModel As val = New AtomicBoolean(False)
			Dim updatedUpdater As val = New AtomicBoolean(False)
			Dim gotGradients As val = New AtomicBoolean(False)

			connector.register(rootTransport)

			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DummyTransport)()
			For e As Integer = 0 To 127
				Dim clientTransport As val = New DummyTransport(System.Guid.randomUUID().ToString(), connector, rootId, config)
				Dim clientServer As val = New ModelParameterServer(config, clientTransport, False)

				servers.add(clientServer)
				transports.add(clientTransport)

				connector.register(clientTransport)

				clientServer.launch()
				log.info("Client [{}] started", e)
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

			assertTrue(updatedModel.get())
			assertTrue(updatedUpdater.get())
			assertTrue(gotGradients.get())
		End Sub

		Private Class ConsumerAnonymousInnerClass
			Inherits Consumer(Of ModelParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootTransport As val)
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

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootTransport As val)
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

		Private Class AbstractSubscriberAnonymousInnerClass
			Inherits AbstractSubscriber(Of INDArray)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private updatedUpdater As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal updatedUpdater As val)
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

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private updatedModel As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal updatedModel As val)
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

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private gotGradients As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal gotGradients As val)
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
'ORIGINAL LINE: @Test public void testModelAndUpdaterParamsUpdate_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelAndUpdaterParamsUpdate_2()
			Nd4j.create(1)
			Dim config As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.MESH).build()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DummyTransport(rootId, connector, rootId, config)
			Dim rootServer As val = New ModelParameterServer(config, rootTransport, True)
			Dim rootUpdatesCounter As val = New AtomicInteger(0)
			rootTransport.addRequestConsumer(GetType(ModelParametersRequest), New ConsumerAnonymousInnerClass3(Me, rootTransport))

			rootTransport.addRequestConsumer(GetType(UpdaterParametersRequest), New ConsumerAnonymousInnerClass4(Me, rootTransport))

			rootServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass3(Me, rootUpdatesCounter))
			connector.register(rootTransport)
			rootServer.launch()

			Dim updatedModel As val = New AtomicBoolean(False)
			Dim updatedUpdater As val = New AtomicBoolean(False)
			Dim gotGradients As val = New AtomicBoolean(False)


			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DummyTransport)()
			For e As Integer = 0 To 31
				Dim clientTransport As val = New DummyTransport(e.ToString(), connector, rootId, config)
				Dim clientServer As val = New ModelParameterServer(config, clientTransport, False)

				clientServer.configure(config, clientTransport, New UpdaterParametersProviderAnonymousInnerClass(Me))
				servers.add(clientServer)
				transports.add(clientTransport)

				connector.register(clientTransport)

				clientServer.launch()
				log.info("Client [{}] started", e)
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

			clientServer.addUpdaterParamsSubscriber(New AbstractSubscriberAnonymousInnerClass3(Me, updatedUpdater))

			clientServer.addModelParamsSubscriber(New AbstractSubscriberAnonymousInnerClass4(Me, updatedModel))

			clientServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass4(Me, gotGradients))

			connector.register(clientTransport)

			clientServer.launch()

			connector.blockUntilFinished()

			log.info("New upstream: {}", clientTransport.getMesh().getRootNode().getId())

			' getting any server
			Dim serv As val = servers.get(27)
			serv.sendUpdate(Nd4j.linspace(1, 10, 100).reshape(ChrW(10), 10))

			connector.blockUntilFinished()

			Dim failedCnt As Integer = 0
			For e As Integer = 0 To 31
				' we're skipping node 23 since it was reconnected, and has different MPS instance
				' and node 96, since it sends update
				If e <> 23 AndAlso e <> 27 Then
					If servers.get(e).getUpdates().size() = 0 Then
						failedCnt += 1
					End If
				End If
			Next e

			assertEquals(0, failedCnt,"Some nodes got no updates:")

			assertTrue(updatedModel.get())
			assertTrue(gotGradients.get())
			assertTrue(updatedUpdater.get())
		End Sub

		Private Class ConsumerAnonymousInnerClass3
			Inherits Consumer(Of ModelParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootTransport As val)
				Me.outerInstance = outerInstance
				Me.rootTransport = rootTransport
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersRequest modelParametersRequest) throws Exception
			Public Overrides Sub accept(ByVal modelParametersRequest As ModelParametersRequest)
				Dim msg As val = New ModelParametersMessage(System.Guid.randomUUID().ToString(), Nd4j.create(10))
				msg.setRequestId(modelParametersRequest.RequestId)
				rootTransport.sendMessage(msg, modelParametersRequest.OriginatorId)
			End Sub
		End Class

		Private Class ConsumerAnonymousInnerClass4
			Inherits Consumer(Of UpdaterParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootTransport As val)
				Me.outerInstance = outerInstance
				Me.rootTransport = rootTransport
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersRequest updatersParametersRequest) throws Exception
			Public Overrides Sub accept(ByVal updatersParametersRequest As UpdaterParametersRequest)
				Dim msg As val = New UpdaterParametersMessage(System.Guid.randomUUID().ToString(), Nd4j.create(10))
				msg.setRequestId(updatersParametersRequest.RequestId)
				rootTransport.sendMessage(msg, updatersParametersRequest.OriginatorId)
			End Sub
		End Class

		Private Class AbstractUpdatesHandlerAnonymousInnerClass3
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootUpdatesCounter As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootUpdatesCounter As val)
				Me.outerInstance = outerInstance
				Me.rootUpdatesCounter = rootUpdatesCounter
			End Sub

			Public Overrides ReadOnly Property ParametersArray As INDArray
				Get
					Return Nd4j.create(10, 10)
				End Get
			End Property

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				rootUpdatesCounter.incrementAndGet()
			End Sub
		End Class

		Private Class UpdaterParametersProviderAnonymousInnerClass
			Implements UpdaterParametersProvider

			Private ReadOnly outerInstance As ModelParameterServerTest

			Public Sub New(ByVal outerInstance As ModelParameterServerTest)
				Me.outerInstance = outerInstance
			End Sub

			Public ReadOnly Property UpdaterParameters As INDArray Implements UpdaterParametersProvider.getUpdaterParameters
				Get
					Return Nd4j.create(10, 10)
				End Get
			End Property
		End Class

		Private Class AbstractSubscriberAnonymousInnerClass3
			Inherits AbstractSubscriber(Of INDArray)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private updatedUpdater As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal updatedUpdater As val)
				Me.outerInstance = outerInstance
				Me.updatedUpdater = updatedUpdater
			End Sub

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				updatedUpdater.set(True)
			End Sub
		End Class

		Private Class AbstractSubscriberAnonymousInnerClass4
			Inherits AbstractSubscriber(Of INDArray)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private updatedModel As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal updatedModel As val)
				Me.outerInstance = outerInstance
				Me.updatedModel = updatedModel
			End Sub

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				updatedModel.set(True)
			End Sub
		End Class

		Private Class AbstractUpdatesHandlerAnonymousInnerClass4
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private gotGradients As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal gotGradients As val)
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
'ORIGINAL LINE: @Test public void testLinearPropagation_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLinearPropagation_1()
			Nd4j.create(1)
			Dim config As val = VoidConfiguration.builder().meshBuildMode(MeshBuildMode.MESH).build()
			Dim connector As val = New DummyTransport.Connector()
			Dim rootTransport As val = New DummyTransport(rootId, connector, rootId, config)
			Dim rootServer As val = New ModelParameterServer(config, rootTransport, True)
			Dim rootUpdatesCounter As val = New AtomicInteger(0)
			rootTransport.addRequestConsumer(GetType(ModelParametersRequest), New ConsumerAnonymousInnerClass5(Me, rootTransport))

			rootTransport.addRequestConsumer(GetType(UpdaterParametersRequest), New ConsumerAnonymousInnerClass6(Me, rootTransport))

			rootServer.addUpdatesSubscriber(New AbstractUpdatesHandlerAnonymousInnerClass5(Me, rootUpdatesCounter))
			connector.register(rootTransport)
			rootServer.launch()

			Dim servers As val = New List(Of ModelParameterServer)()
			Dim transports As val = New List(Of DummyTransport)()
			For e As Integer = 0 To 6
				Dim clientTransport As val = New DummyTransport(e.ToString(), connector, rootId, config)
				Dim clientServer As val = New ModelParameterServer(config, clientTransport, False)

				servers.add(clientServer)
				transports.add(clientTransport)

				connector.register(clientTransport)

				clientServer.launch()
				log.info("Client [{}] started", e)
			Next e

			Dim mesh As val = rootTransport.getMesh()
			Dim rootNode As val = mesh.getRootNode()
			Dim nodesForRemap As val = New LinkedTransferQueue(Of MeshOrganizer.Node)()
			Dim lastNode As MeshOrganizer.Node = Nothing
			Dim cnt As Integer = 0
			For Each d As val In rootNode.getDownstreamNodes()
				assertEquals(0, d.numberOfDownstreams())
				assertEquals(0, d.numberOfDownstreams())
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (cnt++ > 0)
				If cnt > 0 Then
						cnt += 1
					rootNode.removeFromDownstreams(d)
					lastNode.addDownstreamNode(d)
					lastNode = d
				Else
						cnt += 1
					lastNode = d
				End If
			Next d
			assertEquals(1, rootNode.numberOfDownstreams())

			' now we want to ensure that all nodes have only 1 downstream, and last node has 0 downstreams
			Dim nodes As val = New List(Of MeshOrganizer.Node)(mesh.flatNodes())
			For Each n As val In nodes
				If Not n.getId().Equals("6") Then
					assertEquals(1, n.numberOfDownstreams())
				Else
					assertEquals(0, n.numberOfDownstreams())
				End If
			Next n

			' update all mesh copies, just to be sure
			For e As Integer = 0 To 6
				Dim t As val = transports.get(e)
				t.setMesh(mesh)
			Next e

			Dim middleTransport As val = transports.get(3)
			log.info("Upstream ID: [{}]", middleTransport.getUpstreamId())


			Dim middleServer As val = servers.get(3)
			Dim update As val = Nd4j.create(10,10)
			middleServer.sendUpdate(update)
			connector.blockUntilFinished()

			' checking how many nodes got update
			Dim failCnt As Integer = 0
			For e As Integer = 0 To 6
				Dim s As val = servers.get(e)
				If e <> 3 Then
					If 1 <> s.getUpdates().size() Then
						log.info("Node [{}] have no updates", e)
						failCnt += 1
				Else
					assertEquals(0, s.getUpdates().size())
				End If
				End If
			Next e
			assertEquals(0, failCnt)

			' now we're checking if root server got update
			assertEquals(1, rootUpdatesCounter.get())
		End Sub

		Private Class ConsumerAnonymousInnerClass5
			Inherits Consumer(Of ModelParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootTransport As val)
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

		Private Class ConsumerAnonymousInnerClass6
			Inherits Consumer(Of UpdaterParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootTransport As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootTransport As val)
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

		Private Class AbstractUpdatesHandlerAnonymousInnerClass5
			Inherits AbstractUpdatesHandler

			Private ReadOnly outerInstance As ModelParameterServerTest

			Private rootUpdatesCounter As val

			Public Sub New(ByVal outerInstance As ModelParameterServerTest, ByVal rootUpdatesCounter As val)
				Me.outerInstance = outerInstance
				Me.rootUpdatesCounter = rootUpdatesCounter
			End Sub

			Public Overrides ReadOnly Property ParametersArray As INDArray
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Sub onNext(ByVal array As INDArray)
				assertNotNull(array)
				rootUpdatesCounter.incrementAndGet()
			End Sub
		End Class
	End Class
End Namespace