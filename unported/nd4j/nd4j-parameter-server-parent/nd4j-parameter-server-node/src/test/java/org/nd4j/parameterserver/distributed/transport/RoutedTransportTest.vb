Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports ClientRouter = org.nd4j.parameterserver.distributed.logic.ClientRouter
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports IntroductionRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.IntroductionRequestMessage
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

Namespace org.nd4j.parameterserver.distributed.transport


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class RoutedTransportTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class RoutedTransportTest
		Inherits BaseND4JTest

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
		''' This test
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void testMessaging1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMessaging1()

			Dim list As IList(Of String) = New List(Of String)()
			For t As Integer = 0 To 4
				list.Add("127.0.0.1:3838" & t)
			Next t

			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().shardAddresses(list).build()
			voidConfiguration.setUnicastControllerPort(43120) ' this port will be used only by client

			' first of all we start shards
			Dim transports(list.Count - 1) As RoutedTransport
			For t As Integer = 0 To transports.Length - 1

				Dim clipboard As New Clipboard()

				transports(t) = New RoutedTransport()
				transports(t).setIpAndPort("127.0.0.1", Convert.ToInt32("3838" & t))
				transports(t).init(voidConfiguration, clipboard, NodeRole.SHARD, "127.0.0.1", voidConfiguration.getUnicastControllerPort(), CShort(t))
			Next t

			For t As Integer = 0 To transports.Length - 1
				transports(t).launch(Transport.ThreadingModel.DEDICATED_THREADS)
			Next t

			' now we start client, for this test we'll have only one client
			Dim clipboard As New Clipboard()
			Dim clientTransport As New RoutedTransport()
			clientTransport.setIpAndPort("127.0.0.1", voidConfiguration.getUnicastControllerPort())

			' setRouter call should be called before init, and we need
			Dim router As ClientRouter = New InterleavedRouter(0)
			clientTransport.setRouter(router)
			router.init(voidConfiguration, clientTransport)

			clientTransport.init(voidConfiguration, clipboard, NodeRole.CLIENT, "127.0.0.1", voidConfiguration.getUnicastControllerPort(), (Short) -1)
			clientTransport.launch(Transport.ThreadingModel.DEDICATED_THREADS)

			' we send message somewhere
			Dim message As VoidMessage = New IntroductionRequestMessage("127.0.0.1", voidConfiguration.getUnicastControllerPort())
			clientTransport.sendMessage(message)

			Thread.Sleep(500)
			message = transports(0).messages.poll(1, TimeUnit.SECONDS)
			assertNotEquals(Nothing, message)

			For t As Integer = 1 To transports.Length - 1
				message = transports(t).messages.poll(1, TimeUnit.SECONDS)
				assertEquals(Nothing, message)
			Next t


			''' <summary>
			''' This is very important part, shutting down all transports
			''' </summary>
			For Each transport As RoutedTransport In transports
				transport.shutdown()
			Next transport

			clientTransport.shutdown()
		End Sub
	End Class

End Namespace