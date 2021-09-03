Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports HashUtil = org.nd4j.linalg.util.HashUtil
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports ClientRouter = org.nd4j.parameterserver.distributed.logic.ClientRouter
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport

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

Namespace org.nd4j.parameterserver.distributed.logic.routing

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public abstract class BaseRouter implements org.nd4j.parameterserver.distributed.logic.ClientRouter
	<Obsolete>
	Public MustInherit Class BaseRouter
		Implements ClientRouter

		Public MustOverride Function assignTarget(ByVal message As VoidMessage) As Integer Implements ClientRouter.assignTarget
		Public MustOverride Function assignTarget(ByVal message As org.nd4j.parameterserver.distributed.messages.TrainingMessage) As Integer Implements ClientRouter.assignTarget
		Protected Friend voidConfiguration As VoidConfiguration
		Protected Friend transport As Transport

		<NonSerialized>
		Protected Friend originatorIdx As Long = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Transport transport)
		Public Overridable Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport)
			Me.voidConfiguration = voidConfiguration
			Me.transport = transport
		End Sub

		Public Overridable WriteOnly Property Originator Implements ClientRouter.setOriginator As VoidMessage
			Set(ByVal message As VoidMessage)
				If originatorIdx = 0 Then
					originatorIdx = HashUtil.getLongHash(transport.Ip & ":" & transport.Port)
				End If
    
				message.OriginatorId = originatorIdx
			End Set
		End Property


	End Class

End Namespace