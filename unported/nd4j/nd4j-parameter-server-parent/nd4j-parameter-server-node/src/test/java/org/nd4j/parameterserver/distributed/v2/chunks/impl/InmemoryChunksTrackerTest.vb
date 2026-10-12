Imports System.Collections.Generic
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidChunk = org.nd4j.parameterserver.distributed.v2.chunks.VoidChunk
Imports GradientsUpdateMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.GradientsUpdateMessage
Imports MessageSplitter = org.nd4j.parameterserver.distributed.v2.util.MessageSplitter
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

Namespace org.nd4j.parameterserver.distributed.v2.chunks.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class InmemoryChunksTrackerTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class InmemoryChunksTrackerTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testTracker_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTracker_1()
			Dim array As val = Nd4j.linspace(1, 100000, 10000).reshape(ChrW(-1), 1000)
			Dim splitter As val = MessageSplitter.Instance

			Dim message As val = New GradientsUpdateMessage("123", array)
			Dim messages As val = New List(Of VoidChunk)(splitter.split(message, 16384))

			Dim tracker As val = New InmemoryChunksTracker(Of GradientsUpdateMessage)(messages.get(0))

			assertFalse(tracker.isComplete())

			For Each m As val In messages
				tracker.append(m)
			Next m

			assertTrue(tracker.isComplete())

			Dim des As val = tracker.getMessage()
			assertNotNull(des)

			Dim restored As val = des.getPayload()
			assertNotNull(restored)

			assertEquals(array, restored)
		End Sub
	End Class
End Namespace