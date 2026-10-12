Imports System
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseResponseMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseResponseMessage
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

Namespace org.nd4j.parameterserver.distributed.v2.messages.pairs.handshake

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NoArgsConstructor @AllArgsConstructor @Builder public class HandshakeResponse extends org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseResponseMessage
	<Serializable>
	Public Class HandshakeResponse
		Inherits BaseResponseMessage

		Private sequenceId As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer mesh;
		Private mesh As MeshOrganizer

		''' <summary>
		''' This method returns true if our node failed earlier, and should re-acquire model/updater/whatever params
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @Builder.@Default private boolean restart = false;
		Private restart As Boolean = False

		''' <summary>
		''' This method returns true if our node failed too many times, so it'll just enable bypass for the rest of data
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @Builder.@Default private boolean dead = false;
		Private dead As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @Builder.@Default private int iteration = 0;
		Private iteration As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @Builder.@Default private int epoch = 0;
		Private epoch As Integer = 0
	End Class

End Namespace