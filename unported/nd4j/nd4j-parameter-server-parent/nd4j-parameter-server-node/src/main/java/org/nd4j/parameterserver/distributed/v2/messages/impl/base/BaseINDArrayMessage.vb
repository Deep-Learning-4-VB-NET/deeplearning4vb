Imports System
Imports lombok
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayMessage = org.nd4j.parameterserver.distributed.v2.messages.INDArrayMessage

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
'ORIGINAL LINE: @NoArgsConstructor @AllArgsConstructor public abstract class BaseINDArrayMessage implements org.nd4j.parameterserver.distributed.v2.messages.INDArrayMessage
	<Serializable>
	Public MustInherit Class BaseINDArrayMessage
		Implements INDArrayMessage

		Public MustOverride Function fromBytes(ByVal bytes() As SByte) As VoidMessage
		Public MustOverride Function asUnsafeBuffer() As org.agrona.concurrent.UnsafeBuffer
		Public MustOverride Property OriginatorId As String
		Public MustOverride ReadOnly Property MessageId As String
		Public MustOverride ReadOnly Property Payload As INDArray Implements INDArrayMessage.getPayload
		Private Const serialVersionUID As Long = 1L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected String messageId;
		Protected Friend messageId As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected String originatorId;
		Protected Friend originatorId As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected String requestId;
		Protected Friend requestId As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.api.ndarray.INDArray payload;
		Protected Friend payload As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseINDArrayMessage(@NonNull String messageId, org.nd4j.linalg.api.ndarray.INDArray payload)
		Protected Friend Sub New(ByVal messageId As String, ByVal payload As INDArray)
			Me.messageId = messageId
			Me.payload = payload
		End Sub
	End Class

End Namespace