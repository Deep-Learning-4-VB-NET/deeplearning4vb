Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.nd4j.parameterserver.distributed.v2.messages.history

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class HashHistoryHolderTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class HashHistoryHolderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicStuff_1()
		Public Overridable Sub testBasicStuff_1()
			Dim history As val = New HashHistoryHolder(Of String)(1024)

			Dim first As val = System.Guid.randomUUID().ToString()

			' we assume that message is unknown
			assertFalse(history.storeIfUnknownMessageId(first))

			' we assume that message is unknown
			assertTrue(history.storeIfUnknownMessageId(first))

			For e As Integer = 0 To 999
				assertFalse(history.storeIfUnknownMessageId(e.ToString()))
			Next e

			' we still know this entity
			assertTrue(history.storeIfUnknownMessageId(first))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicStuff_2()
		Public Overridable Sub testBasicStuff_2()
			Dim history As val = New HashHistoryHolder(Of String)(2048)

			Dim iterations As val = 1000000
			Dim timeStart As val = System.nanoTime()
			For e As Integer = 0 To iterations - 1
				assertFalse(history.storeIfUnknownMessageId(e.ToString()))
			Next e
			Dim timeStop As val= System.nanoTime()

			log.info("Average time per iteration: [{} us]", (timeStop - timeStart) / iterations)
		End Sub
	End Class
End Namespace