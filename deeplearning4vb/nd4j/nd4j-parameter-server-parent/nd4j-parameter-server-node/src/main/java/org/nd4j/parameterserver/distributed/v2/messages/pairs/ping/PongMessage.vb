Imports System
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports BaseResponseMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseResponseMessage

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

Namespace org.nd4j.parameterserver.distributed.v2.messages.pairs.ping

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class PongMessage extends org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseResponseMessage
	<Serializable>
	Public Class PongMessage
		Inherits BaseResponseMessage

		' TODO: add some telemetry here
	End Class

End Namespace