Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports DistributedMessage = org.nd4j.parameterserver.distributed.messages.DistributedMessage

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

Namespace org.deeplearning4j.spark.parameterserver.networking.v1.messages

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SilentIntroductoryMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.DistributedMessage
	<Serializable>
	Public Class SilentIntroductoryMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		Protected Friend localIp As String
		Protected Friend port As Integer

		Protected Friend Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SilentIntroductoryMessage(@NonNull String localIP, int port)
		Public Sub New(ByVal localIP As String, ByVal port As Integer)
			Me.localIp = localIP
			Me.port = port
		End Sub

		Public Overrides Sub processMessage()
	'        
	'            basically we just want to send our IP, and get our new shardIndex in return. haha. bad idea obviously, but still...
	'        
	'            or, we can skip direct addressing here, use passive addressing instead, like in client mode?
	'         

			log.info("Adding client {}:{}", localIp, port)
			'transport.addShard(localIp, port);
			transport.addClient(localIp, port)
		End Sub

		Public Overrides ReadOnly Property BlockingMessage As Boolean
			Get
				' this is blocking message, we want to get reply back before going further
				Return True
			End Get
		End Property
	End Class

End Namespace