Imports System
Imports lombok
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastableMessage = org.nd4j.parameterserver.distributed.v2.messages.BroadcastableMessage
Imports BaseINDArrayMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseINDArrayMessage

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

Namespace org.nd4j.parameterserver.distributed.v2.messages.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public final class GradientsUpdateMessage extends org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseINDArrayMessage implements org.nd4j.parameterserver.distributed.v2.messages.BroadcastableMessage
	<Serializable>
	Public NotInheritable Class GradientsUpdateMessage
		Inherits BaseINDArrayMessage
		Implements BroadcastableMessage

		Private Const serialVersionUID As Long = 1L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private String relayId;
		Private relayId As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int iteration = 0;
		Private iteration As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int epoch = 0;
		Private epoch As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public GradientsUpdateMessage(@NonNull String messageId, org.nd4j.linalg.api.ndarray.INDArray payload)
		Public Sub New(ByVal messageId As String, ByVal payload As INDArray)
			MyBase.New(messageId, payload)
		End Sub
	End Class

End Namespace