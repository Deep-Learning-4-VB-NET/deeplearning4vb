Imports System
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports BroadcastableMessage = org.nd4j.parameterserver.distributed.v2.messages.BroadcastableMessage
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseVoidMessage
Imports MeshOrganizer = org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer

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
'ORIGINAL LINE: @NoArgsConstructor public class MeshUpdateMessage extends org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseVoidMessage implements org.nd4j.parameterserver.distributed.v2.messages.BroadcastableMessage
	<Serializable>
	Public Class MeshUpdateMessage
		Inherits BaseVoidMessage
		Implements BroadcastableMessage

		Private Const serialVersionUID As Long = 1L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private String relayId;
		Private relayId As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer mesh;
		Private mesh As MeshOrganizer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MeshUpdateMessage(@NonNull MeshOrganizer mesh)
		Public Sub New(ByVal mesh As MeshOrganizer)
			Me.mesh = mesh
		End Sub
	End Class

End Namespace