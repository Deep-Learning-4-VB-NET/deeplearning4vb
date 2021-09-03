Imports System
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports HashUtil = org.nd4j.linalg.util.HashUtil
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports InitializationRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.InitializationRequestMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports RoutedTransport = org.nd4j.parameterserver.distributed.transport.RoutedTransport
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

Namespace org.nd4j.parameterserver.distributed.logic.routing


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class InterleavedRouterTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class InterleavedRouterTest
		Inherits BaseND4JTest

		Friend configuration As VoidConfiguration
		Friend transport As Transport
		Friend originator As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			configuration = VoidConfiguration.builder().shardAddresses(Arrays.asList("1.2.3.4", "2.3.4.5", "3.4.5.6", "4.5.6.7")).numberOfShards(4).build()

			transport = New RoutedTransport()
			transport.setIpAndPort("8.9.10.11", 87312)
			originator = HashUtil.getLongHash(transport.Ip & ":" & transport.Port)
		End Sub



		''' <summary>
		''' Testing default assignment for everything, but training requests
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void assignTarget1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub assignTarget1()
			Dim router As New InterleavedRouter()
			router.init(configuration, transport)

			For i As Integer = 0 To 99
				Dim message As VoidMessage = New InitializationRequestMessage(100, 10, 123, False, False, 10)
				Dim target As Integer = router.assignTarget(message)

				assertTrue(target >= 0 AndAlso target <= 3)
				assertEquals(originator, message.OriginatorId)
			Next i
		End Sub

		''' <summary>
		''' Testing assignment for training message
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void assignTarget2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub assignTarget2()
			Dim router As New InterleavedRouter()
			router.init(configuration, transport)

			Dim w1() As Integer = {512, 345, 486, 212}

			For i As Integer = 0 To w1.Length - 1
				Dim message As New SkipGramRequestMessage(w1(i), 1, New Integer() {1, 2, 3}, New SByte() {0, 0, 1}, CShort(0), 0.02, 119)
				Dim target As Integer = router.assignTarget(message)

				assertEquals(w1(i) Mod configuration.getNumberOfShards(), target)
				assertEquals(originator, message.OriginatorId)
			Next i
		End Sub

		''' <summary>
		''' Testing default assignment for everything, but training requests.
		''' Difference here is pre-defined default index, for everything but TrainingMessages
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void assignTarget3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub assignTarget3()
			Dim router As New InterleavedRouter(2)
			router.init(configuration, transport)


			For i As Integer = 0 To 2
				Dim message As VoidMessage = New InitializationRequestMessage(100, 10, 123, False, False, 10)
				Dim target As Integer = router.assignTarget(message)

				assertEquals(2, target)
				assertEquals(originator, message.OriginatorId)
			Next i
		End Sub

	End Class

End Namespace