Imports System
Imports System.IO
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports StopWatch = org.apache.commons.lang3.time.StopWatch
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.nd4j.aeron.ipc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Disabled("Tests are too flaky") @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class AeronNDArraySerdeTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class AeronNDArraySerdeTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToAndFrom()
		Public Overridable Sub testToAndFrom()
			Dim arr As INDArray = Nd4j.scalar(1.0)
			Dim buffer As UnsafeBuffer = AeronNDArraySerde.toBuffer(arr)
			Dim back As INDArray = AeronNDArraySerde.toArray(buffer)
			assertEquals(arr, back)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToAndFromCompressed()
		Public Overridable Sub testToAndFromCompressed()
			Dim arr As INDArray = Nd4j.scalar(1.0)
			Dim compress As INDArray = Nd4j.Compressor.compress(arr, "GZIP")
			assertTrue(compress.Compressed)
			Dim buffer As UnsafeBuffer = AeronNDArraySerde.toBuffer(compress)
			Dim back As INDArray = AeronNDArraySerde.toArray(buffer)
			Dim decompressed As INDArray = Nd4j.Compressor.decompress(compress)
			assertEquals(arr, decompressed)
			assertEquals(arr, back)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testToAndFromCompressedLarge()
		Public Overridable Sub testToAndFromCompressedLarge()
			skipUnlessIntegrationTests()

			Dim arr As INDArray = Nd4j.zeros(CInt(Math.Truncate(1e))7)
			Dim compress As INDArray = Nd4j.Compressor.compress(arr, "GZIP")
			assertTrue(compress.Compressed)
			Dim buffer As UnsafeBuffer = AeronNDArraySerde.toBuffer(compress)
			Dim back As INDArray = AeronNDArraySerde.toArray(buffer)
			Dim decompressed As INDArray = Nd4j.Compressor.decompress(compress)
			assertEquals(arr, decompressed)
			assertEquals(arr, back)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void timeOldVsNew() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub timeOldVsNew()
			Dim numTrials As Integer = 1000
			Dim oldTotal As Long = 0
			Dim newTotal As Long = 0
			Dim arr As INDArray = Nd4j.create(100000)
			Nd4j.Compressor.compressi(arr, "GZIP")
			For i As Integer = 0 To numTrials - 1
				Dim oldStopWatch As New StopWatch()
				Dim bos As New BufferedOutputStream(New MemoryStream(CInt(arr.length())))
				Dim dos As New DataOutputStream(bos)
				oldStopWatch.start()
				Nd4j.write(arr, dos)
				oldStopWatch.stop()
				' System.out.println("Old " + oldStopWatch.getNanoTime());
				oldTotal += oldStopWatch.getNanoTime()
				Dim newStopWatch As New StopWatch()
				newStopWatch.start()
				AeronNDArraySerde.toBuffer(arr)
				newStopWatch.stop()
				'  System.out.println("New " + newStopWatch.getNanoTime());
				newTotal += newStopWatch.getNanoTime()

			Next i

			oldTotal \= numTrials
			newTotal \= numTrials
			Console.WriteLine("Old avg " & oldTotal & " New avg " & newTotal)

		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property
	End Class

End Namespace