Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports PropagationMode = org.nd4j.parameterserver.distributed.v2.enums.PropagationMode
Imports GradientsUpdateMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.GradientsUpdateMessage
Imports HandshakeRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.handshake.HandshakeRequest
Imports HandshakeResponse = org.nd4j.parameterserver.distributed.v2.messages.pairs.handshake.HandshakeResponse
Imports org.nd4j.parameterserver.distributed.v2.transport
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

Namespace org.nd4j.parameterserver.distributed.v2.transport.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class DummyTransportTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class DummyTransportTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicConnection_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasicConnection_1()
			Dim counter As val = New AtomicInteger(0)
			Dim connector As val = New DummyTransport.Connector()
			Dim transportA As val = New DummyTransport("alpha", connector)
			Dim transportB As val = New DummyTransport("beta", connector)

			connector.register(transportA, transportB)
			transportB.addInterceptor(GetType(HandshakeRequest), Sub(message)
			counter.incrementAndGet()
			End Sub)

			transportA.sendMessage(New HandshakeRequest(), "beta")

			' we expect that message was delivered, and connector works
			assertEquals(1, counter.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHandshake_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHandshake_1()
			Dim counter As val = New AtomicInteger(0)
			Dim connector As val = New DummyTransport.Connector()
			Dim transportA As val = New DummyTransport("alpha", connector)
			Dim transportB As val = New DummyTransport("beta", connector)

			connector.register(transportA, transportB)
			transportB.addInterceptor(GetType(HandshakeResponse), Sub(message As HandshakeResponse)
			assertNotNull(message)
			assertNotNull(message.getMesh())
			counter.incrementAndGet()
			End Sub)

			transportB.sendMessage(New HandshakeRequest(), "alpha")

			' we expect that message was delivered, and connector works
			assertEquals(1, counter.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMeshPropagation_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMeshPropagation_1()
			Dim counter As val = New AtomicInteger(0)
			Dim connector As val = New DummyTransport.Connector()
			Dim transportA As val = New DummyTransport("alpha", connector)
			Dim transportB As val = New DummyTransport("beta", connector)
			Dim transportG As val = New DummyTransport("gamma", connector)
			Dim transportD As val = New DummyTransport("delta", connector)

			connector.register(transportA, transportB, transportG, transportD)


			transportB.sendMessage(New HandshakeRequest(), "alpha")
			transportG.sendMessage(New HandshakeRequest(), "alpha")
			transportD.sendMessage(New HandshakeRequest(), "alpha")

			Dim meshA As val = transportA.getMesh()
			Dim meshB As val = transportB.getMesh()
			Dim meshG As val = transportG.getMesh()
			Dim meshD As val = transportD.getMesh()

			' versions should be equal
			assertEquals(meshA.getVersion(), meshB.getVersion())
			assertEquals(meshA.getVersion(), meshG.getVersion())
			assertEquals(meshA.getVersion(), meshD.getVersion())

			' and meshs in general too
			assertEquals(meshA, meshB)
			assertEquals(meshA, meshG)
			assertEquals(meshA, meshD)

			assertTrue(meshA.isKnownNode("alpha"))
			assertTrue(meshA.isKnownNode("beta"))
			assertTrue(meshA.isKnownNode("gamma"))
			assertTrue(meshA.isKnownNode("delta"))

			Dim node As val = meshB.getNodeById("alpha")
			assertTrue(node.isRootNode())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUpdatesPropagation_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUpdatesPropagation_1()
			Dim counter As val = New AtomicInteger(0)
			Dim connector As val = New DummyTransport.Connector()
			Dim transportA As val = New DummyTransport("alpha", connector)
			Dim transportB As val = New DummyTransport("beta", connector)
			Dim transportG As val = New DummyTransport("gamma", connector)
			Dim transportD As val = New DummyTransport("delta", connector)

			connector.register(transportA, transportB, transportG, transportD)

			transportB.sendMessage(New HandshakeRequest(), "alpha")
			transportG.sendMessage(New HandshakeRequest(), "alpha")
			transportD.sendMessage(New HandshakeRequest(), "alpha")


			Dim f As val = New MessageCallableAnonymousInnerClass(Me, counter)

			transportA.addPrecursor(GetType(GradientsUpdateMessage), f)
			transportB.addPrecursor(GetType(GradientsUpdateMessage), f)
			transportG.addPrecursor(GetType(GradientsUpdateMessage), f)
			transportD.addPrecursor(GetType(GradientsUpdateMessage), f)

			Dim array As val = Nd4j.ones(10, 10)

			Dim msg As val = New GradientsUpdateMessage("message", array)
			msg.setOriginatorId("beta")
			transportB.propagateMessage(msg, PropagationMode.BOTH_WAYS)

			' we expect that each of the nodes gets this message
			assertEquals(400, counter.get())
		End Sub

		Private Class MessageCallableAnonymousInnerClass
			Implements MessageCallable(Of GradientsUpdateMessage)

			Private ReadOnly outerInstance As DummyTransportTest

			Private counter As val

			Public Sub New(ByVal outerInstance As DummyTransportTest, ByVal counter As val)
				Me.outerInstance = outerInstance
				Me.counter = counter
			End Sub

			Public Sub apply(ByVal message As GradientsUpdateMessage)
				Dim update As val = message.Payload
				counter.addAndGet(update.sumNumber().intValue())
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReconnectAfterFailure_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReconnectAfterFailure_1()
			Dim counter As val = New AtomicInteger(0)
			Dim connector As val = New DummyTransport.Connector()
			Dim transportA As val = New DummyTransport("alpha", connector)
			Dim transportB As val = New DummyTransport("beta", connector)
			Dim transportG As val = New DummyTransport("gamma", connector)
			Dim transportD As val = New DummyTransport("delta", connector)
			Dim transportE As val = New DummyTransport("epsilon", connector)
			Dim transportZ As val = New DummyTransport("zeta", connector)
			Dim transportT As val = New DummyTransport("theta", connector)

			connector.register(transportA, transportB, transportG, transportD, transportE, transportZ, transportT)

			transportB.sendMessage(New HandshakeRequest(), "alpha")
			transportG.sendMessage(New HandshakeRequest(), "alpha")
			transportD.sendMessage(New HandshakeRequest(), "alpha")
			transportE.sendMessage(New HandshakeRequest(), "alpha")
			transportZ.sendMessage(New HandshakeRequest(), "alpha")
			transportT.sendMessage(New HandshakeRequest(), "alpha")

			Dim originalMeshA As val = transportA.getMesh()
			Dim originalMeshZ As val = transportZ.getMesh()

			assertEquals(originalMeshA, originalMeshZ)

			Dim version As val = originalMeshA.getVersion()
			Dim upstream As val = originalMeshZ.getUpstreamForNode("zeta")


			Dim restarted As val = New AtomicBoolean(False)
			Dim f As val = New MessageCallableAnonymousInnerClass2(Me, restarted)
			transportZ.addPrecursor(GetType(HandshakeResponse), f)

			' this message basically says that Z is restarting
			transportZ.sendMessage(New HandshakeRequest(), "alpha")

			Dim newMesh As val = transportZ.getMesh()
			Dim newUpstream As val = newMesh.getUpstreamForNode("zeta")

			assertNotEquals(version, newMesh.getVersion())
			assertTrue(restarted.get())
		End Sub

		Private Class MessageCallableAnonymousInnerClass2
			Implements MessageCallable(Of HandshakeResponse)

			Private ReadOnly outerInstance As DummyTransportTest

			Private restarted As val

			Public Sub New(ByVal outerInstance As DummyTransportTest, ByVal restarted As val)
				Me.outerInstance = outerInstance
				Me.restarted = restarted
			End Sub

			Public Sub apply(ByVal message As HandshakeResponse)
				assertTrue(message.isRestart())
				restarted.set(True)
			End Sub
		End Class
	End Class
End Namespace