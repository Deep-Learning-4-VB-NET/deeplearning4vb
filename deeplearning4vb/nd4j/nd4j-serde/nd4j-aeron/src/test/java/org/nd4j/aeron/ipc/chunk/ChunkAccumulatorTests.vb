Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.aeron.ipc.chunk


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Disabled("Tests are too flaky") @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class ChunkAccumulatorTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class ChunkAccumulatorTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAccumulator()
		Public Overridable Sub testAccumulator()
			Dim chunkAccumulator As ChunkAccumulator = New InMemoryChunkAccumulator()
			Dim message As NDArrayMessage = NDArrayMessage.wholeArrayUpdate(Nd4j.ones(1000))
			Dim chunkSize As Integer = 128
			Dim chunks() As NDArrayMessageChunk = NDArrayMessage.chunks(message, chunkSize)
			For i As Integer = 0 To chunks.Length - 1
				chunkAccumulator.accumulateChunk(chunks(i))
			Next i

			Dim message1 As NDArrayMessage = chunkAccumulator.reassemble(chunks(0).getId())
			assertEquals(message, message1)
		End Sub

	End Class

End Namespace