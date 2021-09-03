Imports System
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports MeaningfulMessage = org.nd4j.parameterserver.distributed.messages.MeaningfulMessage

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

Namespace org.nd4j.parameterserver.distributed.messages.complete

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j @Deprecated public abstract class BaseCompleteMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.MeaningfulMessage
	<Obsolete, Serializable>
	Public MustInherit Class BaseCompleteMessage
		Inherits BaseVoidMessage
		Implements MeaningfulMessage

		Public MustOverride ReadOnly Property Payload As INDArray Implements MeaningfulMessage.getPayload
		Public Overrides MustOverride ReadOnly Property RetransmitCount As Integer
		Public Overrides MustOverride Function fromBytes(ByVal array() As SByte) As T
		Public Overrides MustOverride Property OriginatorId As Long
		Public Overrides MustOverride ReadOnly Property TaskId As Long
		Public Overrides MustOverride Property TargetId As Short

		Protected Friend payload As INDArray

		Public Sub New()
			MyBase.New(10)
		End Sub

		Public Sub New(ByVal messageType As Integer)
			MyBase.New(messageType)
		End Sub


		Public Overrides Sub processMessage()
			' no-op
		End Sub
	End Class

End Namespace