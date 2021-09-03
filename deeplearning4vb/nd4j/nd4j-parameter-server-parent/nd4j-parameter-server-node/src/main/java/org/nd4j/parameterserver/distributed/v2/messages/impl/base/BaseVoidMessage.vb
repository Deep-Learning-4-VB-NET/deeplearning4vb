Imports System
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Setter = lombok.Setter
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage

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

Namespace org.nd4j.parameterserver.distributed.v2.messages.impl.base

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public abstract class BaseVoidMessage implements org.nd4j.parameterserver.distributed.v2.messages.VoidMessage
	<Serializable>
	Public MustInherit Class BaseVoidMessage
		Implements VoidMessage

		Public MustOverride Function fromBytes(ByVal bytes() As SByte) As VoidMessage
		Public MustOverride Function asUnsafeBuffer() As org.agrona.concurrent.UnsafeBuffer Implements VoidMessage.asUnsafeBuffer
		Public MustOverride Property OriginatorId Implements VoidMessage.setOriginatorId As String
		Public MustOverride ReadOnly Property MessageId As String Implements VoidMessage.getMessageId
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected String messageId;
		Protected Friend messageId As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected String originatorId;
		Protected Friend originatorId As String
	End Class

End Namespace