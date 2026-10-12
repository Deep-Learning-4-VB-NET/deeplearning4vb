Imports DirectBuffer = org.agrona.DirectBuffer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.nd4j.aeron.ipc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Disabled("Tests are too flaky") @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class NDArrayMessageTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class NDArrayMessageTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNDArrayMessageToAndFrom()
		Public Overridable Sub testNDArrayMessageToAndFrom()
			Dim message As NDArrayMessage = NDArrayMessage.wholeArrayUpdate(Nd4j.scalar(1.0))
			Dim bufferConvert As DirectBuffer = NDArrayMessage.toBuffer(message)
			Dim buffer As Buffer = CType(bufferConvert.byteBuffer(), Buffer)
			buffer.rewind()
			Dim newMessage As NDArrayMessage = NDArrayMessage.fromBuffer(bufferConvert, 0)
			assertEquals(message, newMessage)

			Dim compressed As INDArray = Nd4j.Compressor.compress(Nd4j.scalar(1.0), "GZIP")
			Dim messageCompressed As NDArrayMessage = NDArrayMessage.wholeArrayUpdate(compressed)
			Dim bufferConvertCompressed As DirectBuffer = NDArrayMessage.toBuffer(messageCompressed)
			Dim newMessageTest As NDArrayMessage = NDArrayMessage.fromBuffer(bufferConvertCompressed, 0)
			assertEquals(messageCompressed, newMessageTest)


		End Sub


	End Class

End Namespace