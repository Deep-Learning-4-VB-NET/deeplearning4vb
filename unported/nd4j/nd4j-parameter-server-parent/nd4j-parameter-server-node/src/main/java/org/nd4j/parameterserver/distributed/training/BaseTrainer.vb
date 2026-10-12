Imports System
Imports NonNull = lombok.NonNull
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports FrameCompletionHandler = org.nd4j.parameterserver.distributed.logic.completion.FrameCompletionHandler
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
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

Namespace org.nd4j.parameterserver.distributed.training

	<Obsolete>
	Public MustInherit Class BaseTrainer(Of T As org.nd4j.parameterserver.distributed.messages.TrainingMessage)
		Implements TrainingDriver(Of T)

		Public MustOverride Function targetMessageClass() As String Implements TrainingDriver(Of T).targetMessageClass
		Public MustOverride Sub finishTraining(ByVal originatorId As Long, ByVal taskId As Long)
		Public MustOverride Sub aggregationFinished(ByVal aggregation As org.nd4j.parameterserver.distributed.messages.VoidAggregation) Implements TrainingDriver(Of T).aggregationFinished
		Public MustOverride Sub pickTraining(ByVal message As T) Implements TrainingDriver(Of T).pickTraining
		Public MustOverride Sub startTraining(ByVal message As T) Implements TrainingDriver(Of T).startTraining
		Protected Friend voidConfiguration As VoidConfiguration
		Protected Friend transport As Transport
		Protected Friend clipboard As Clipboard
		Protected Friend storage As Storage

		Protected Friend completionHandler As New FrameCompletionHandler()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Transport transport, @NonNull Storage storage, @NonNull Clipboard clipboard)
		Public Overridable Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport, ByVal storage As Storage, ByVal clipboard As Clipboard)
			Me.clipboard = clipboard
			Me.transport = transport
			Me.voidConfiguration = voidConfiguration
			Me.storage = storage
		End Sub

		Protected Friend Overridable Function replicate(ByVal value As Integer, ByVal size As Integer) As Integer()
			Dim result(size - 1) As Integer
			For e As Integer = 0 To size - 1
				result(e) = value
			Next e

			Return result
		End Function

		Public Overridable Sub addCompletionHook(ByVal originatorId As Long, ByVal frameId As Long, ByVal messageId As Long) Implements TrainingDriver(Of T).addCompletionHook
			completionHandler.addHook(originatorId, frameId, messageId)
		End Sub
	End Class

End Namespace