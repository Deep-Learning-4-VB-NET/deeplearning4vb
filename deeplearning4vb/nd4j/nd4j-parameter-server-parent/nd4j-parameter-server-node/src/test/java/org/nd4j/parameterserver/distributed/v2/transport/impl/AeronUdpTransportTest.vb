Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
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

Namespace org.nd4j.parameterserver.distributed.v2.transport.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class AeronUdpTransportTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class AeronUdpTransportTest
		Inherits BaseND4JTest

		Private Const IP As String = "127.0.0.1"
		Private Const ROOT_PORT As Integer = 40781

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 240_000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testBasic_Connection_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasic_Connection_1()
			' we definitely want to shutdown all transports after test, to avoid issues with shmem
			Using transportA As lombok.val = New AeronUdpTransport(IP, ROOT_PORT, IP, ROOT_PORT, org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().build()), transportB As lombok.val = New AeronUdpTransport(IP, 40782, IP, ROOT_PORT, org.nd4j.parameterserver.distributed.conf.VoidConfiguration.builder().build())
				transportA.launchAsMaster()

				Thread.Sleep(50)

				transportB.launch()

				Thread.Sleep(50)

				assertEquals(2, transportA.getMesh().totalNodes())
				assertEquals(transportA.getMesh(), transportB.getMesh())
			End Using
		End Sub
	End Class
End Namespace