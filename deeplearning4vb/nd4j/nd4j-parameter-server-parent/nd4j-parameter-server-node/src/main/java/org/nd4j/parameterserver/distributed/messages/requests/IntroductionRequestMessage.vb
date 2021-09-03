Imports System
Imports NonNull = lombok.NonNull
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.messages.RequestMessage

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

Namespace org.nd4j.parameterserver.distributed.messages.requests

	<Obsolete, Serializable>
	Public Class IntroductionRequestMessage
		Inherits BaseVoidMessage
		Implements RequestMessage

		Private ip As String
		Private port As Integer

		Public Sub New()
			MyBase.New(5)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public IntroductionRequestMessage(@NonNull String ip, int port)
		Public Sub New(ByVal ip As String, ByVal port As Integer)
			Me.New()
			Me.ip = ip
			Me.port = port
		End Sub

		Public Overrides Sub processMessage()
			' redistribute this message over network
			transport.addClient(ip, port)

			'        DistributedIntroductionMessage dim = new DistributedIntroductionMessage(ip, port);

			'        dim.extractContext(this);
			'        dim.processMessage();

			'        if (voidConfiguration.getNumberOfShards() > 1)
			'            transport.sendMessageToAllShards(dim);

			'        IntroductionCompleteMessage icm = new IntroductionCompleteMessage(this.taskId);
			'        icm.setOriginatorId(this.originatorId);

			'        transport.sendMessage(icm);
		End Sub

		Public Overrides ReadOnly Property BlockingMessage As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

End Namespace