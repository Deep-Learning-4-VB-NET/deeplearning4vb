Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports HandshakeRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.handshake.HandshakeRequest
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

Namespace org.nd4j.parameterserver.distributed.v2.messages

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class VoidMessageTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class VoidMessageTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHandshakeSerialization_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHandshakeSerialization_1()
			Dim req As val = New HandshakeRequest()
			req.setOriginatorId("1234")

			Dim bytes As val = SerializationUtils.toByteArray(req)

			Dim res As VoidMessage = SerializationUtils.deserialize(bytes)

			assertEquals(req.getOriginatorId(), res.OriginatorId)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHandshakeSerialization_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHandshakeSerialization_2()
			Dim req As val = New HandshakeRequest()
			req.setOriginatorId("1234")

			Dim bytes As val = SerializationUtils.toByteArray(req)

			Dim res As VoidMessage = VoidMessage.fromBytes(bytes)

			assertEquals(req.getOriginatorId(), res.OriginatorId)
		End Sub
	End Class
End Namespace