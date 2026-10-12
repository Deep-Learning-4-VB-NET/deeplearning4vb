Imports System
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.messages.RequestMessage
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports org.nd4j.parameterserver.distributed.training
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

Namespace org.deeplearning4j.spark.parameterserver.networking.v1.messages

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SilentUpdatesMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.TrainingMessage, org.nd4j.parameterserver.distributed.messages.RequestMessage
	<Serializable>
	Public Class SilentUpdatesMessage
		Inherits BaseVoidMessage
		Implements TrainingMessage, RequestMessage

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected long updateId;
		Protected Friend updateId As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.api.ndarray.INDArray updates;
		Protected Friend updates As INDArray
'JAVA TO VB CONVERTER NOTE: The field frameId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend frameId_Conflict As Long

		Protected Friend Sub New()
			' just for ser/de
		End Sub

		Public Sub New(ByVal encodedUpdates As INDArray, ByVal updateId As Long)
			Me.updates = encodedUpdates
			Me.updateId = updateId
		End Sub


		Public Overridable Overloads Sub attachContext(Of T1 As TrainingMessage)(ByVal voidConfiguration As VoidConfiguration, ByVal trainer As TrainingDriver(Of T1), ByVal clipboard As Clipboard, ByVal transport As Transport, ByVal storage As Storage, ByVal role As NodeRole, ByVal shardIndex As Short)
			Me.voidConfiguration = voidConfiguration
			Me.trainer = trainer
			Me.transport = transport
		End Sub

		Public Overrides Sub processMessage()
			' basically no-op?
			Dim tr As TrainingDriver(Of SilentUpdatesMessage) = CType(trainer, TrainingDriver(Of SilentUpdatesMessage))
			tr.startTraining(Me)
		End Sub

		Public Overridable ReadOnly Property Counter As SByte Implements TrainingMessage.getCounter
			Get
				Return 0
			End Get
		End Property

		Public Overridable Property FrameId As Long Implements TrainingMessage.getFrameId
			Get
				Return frameId_Conflict
			End Get
			Set(ByVal frameId As Long)
				Me.frameId_Conflict = frameId
			End Set
		End Property


		Public Overrides ReadOnly Property JoinSupported As Boolean
			Get
				Return False
			End Get
		End Property
	End Class

End Namespace