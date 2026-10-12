Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports GradientsUpdateMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.GradientsUpdateMessage
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

Namespace org.nd4j.parameterserver.distributed.v2.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class MessageSplitterTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class MessageSplitterTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMessageSplit_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMessageSplit_1()
			Dim array As val = Nd4j.linspace(1, 100000, 100000).reshape(ChrW(-1), 1000)
			Dim splitter As val = New MessageSplitter()

			Dim message As val = New GradientsUpdateMessage("123", array)

			Dim messages As val = splitter.split(message, 16384)

			assertNotNull(messages)
			assertFalse(messages.isEmpty())

			log.info("Number of messages: {}", messages.size())

			For Each m As val In messages
				assertEquals("123", m.getOriginalId())
			Next m

			Dim dec As [Optional](Of GradientsUpdateMessage) = Nothing
			For Each m As val In messages
				dec = splitter.merge(m)
			Next m

			assertNotNull(dec)
			assertTrue(dec.Present)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSmallMessageSplit_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSmallMessageSplit_1()
			Dim array As val = Nd4j.linspace(1, 15, 15).reshape(ChrW(-1), 5)
			Dim splitter As val = New MessageSplitter()

			Dim message As val = New GradientsUpdateMessage("123", array)

			Dim messages As val = splitter.split(message, 16384)

			assertNotNull(messages)
			assertEquals(1, messages.size())

			For Each m As val In messages
				assertEquals("123", m.getOriginalId())
			Next m

			Dim dec As [Optional](Of GradientsUpdateMessage) = splitter.merge((New List(Of GradientsUpdateMessage)(messages))(0))

			assertNotNull(dec)
			assertTrue(dec.Present)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConcurrentAppend_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConcurrentAppend_1()
			Dim splitter As val = New MessageSplitter()
			Dim array As val = Nd4j.linspace(1, 100000, 100000).reshape(ChrW(-1), 1000)
			For e As Integer = 0 To 99
				Dim message As val = New GradientsUpdateMessage(System.Guid.randomUUID().ToString(), array)
				Dim chunks As val = splitter.split(message, 16384)
				Dim ref As val = New Atomic(Of GradientsUpdateMessage)()

				chunks.ForEach(Sub(c)
				Dim o As val = splitter.merge(c)
				If o.isPresent() Then
					ref.set(CType(o.get(), GradientsUpdateMessage))
				End If
				End Sub)

				assertNotNull(ref.get())
				assertEquals(array, ref.get().getPayload())
				assertEquals(0, splitter.memoryUse.intValue())
				assertEquals(False, splitter.isTrackedMessage(message.getMessageId()))
				assertEquals(0, splitter.trackers.size())
			Next e
		End Sub
	End Class
End Namespace