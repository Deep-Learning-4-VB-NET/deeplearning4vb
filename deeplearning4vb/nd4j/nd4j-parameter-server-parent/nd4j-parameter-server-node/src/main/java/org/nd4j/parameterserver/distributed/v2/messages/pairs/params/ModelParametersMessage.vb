Imports System
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ResponseMessage = org.nd4j.parameterserver.distributed.v2.messages.ResponseMessage
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

Namespace org.nd4j.parameterserver.distributed.v2.messages.pairs.params

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public final class ModelParametersMessage extends org.nd4j.parameterserver.distributed.v2.messages.impl.base.BaseINDArrayMessage implements org.nd4j.parameterserver.distributed.v2.messages.ResponseMessage
	<Serializable>
	Public NotInheritable Class ModelParametersMessage
		Inherits BaseINDArrayMessage
		Implements ResponseMessage

		Private Const serialVersionUID As Long = 1L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int iterationNumber = 0;
		Private iterationNumber As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int epochNumber = 0;
		Private epochNumber As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ModelParametersMessage(@NonNull String messageId, org.nd4j.linalg.api.ndarray.INDArray payload)
		Public Sub New(ByVal messageId As String, ByVal payload As INDArray)
			MyBase.New(messageId, payload)
		End Sub
	End Class

End Namespace