Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports InitializationAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.InitializationAggregation
Imports VectorAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.VectorAggregation
Imports VoidAggregation = org.nd4j.parameterserver.distributed.messages.VoidAggregation
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

Namespace org.nd4j.parameterserver.distributed.logic


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class ClipboardTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class ClipboardTest
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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPin1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPin1()
			Dim clipboard As New Clipboard()

			Dim rng As New Random(CInt(12345L))

			For i As Integer = 0 To 99
				Dim aggregation As New VectorAggregation(rng.nextLong(), CShort(100), CShort(i), Nd4j.create(5))

				clipboard.pin(aggregation)
			Next i

			assertEquals(False, clipboard.hasCandidates())
			assertEquals(0, clipboard.NumberOfCompleteStacks)
			assertEquals(100, clipboard.NumberOfPinnedStacks)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPin2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPin2()
			Dim clipboard As New Clipboard()

			Dim rng As New Random(CInt(12345L))

			Dim validId As Long? = 123L

			Dim shardIdx As Short = 0
			For i As Integer = 0 To 299
				Dim aggregation As New VectorAggregation(rng.nextLong(), CShort(100), CShort(1), Nd4j.create(5))

				' imitating valid
				If i Mod 2 = 0 AndAlso shardIdx < 100 Then
					aggregation.setTaskId(validId)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: aggregation.setShardIndex(shardIdx++);
					aggregation.ShardIndex = shardIdx
						shardIdx += 1
				End If

				clipboard.pin(aggregation)
			Next i

			Dim aggregation As VoidAggregation = clipboard.getStackFromClipboard(0L, validId)
			assertNotEquals(Nothing, aggregation)

			assertEquals(0, aggregation.MissingChunks)

			assertEquals(True, clipboard.hasCandidates())
			assertEquals(1, clipboard.NumberOfCompleteStacks)
		End Sub

		''' <summary>
		''' This test checks how clipboard handles singular aggregations </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPin3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPin3()
			Dim clipboard As New Clipboard()

			Dim rng As New Random(CInt(12345L))

			Dim validId As Long? = 123L
			Dim aggregation As New InitializationAggregation(1, 0)
			clipboard.pin(aggregation)

			assertTrue(clipboard.isTracking(0L, aggregation.TaskId))
			assertTrue(clipboard.isReady(0L, aggregation.TaskId))
		End Sub
	End Class

End Namespace