Imports System
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BasicSequenceProvider = org.nd4j.parameterserver.distributed.logic.sequence.BasicSequenceProvider
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.messages.RequestMessage
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports org.nd4j.parameterserver.distributed.training

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

Namespace org.nd4j.parameterserver.distributed.messages.requests

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j @Deprecated public class CbowRequestMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.TrainingMessage, org.nd4j.parameterserver.distributed.messages.RequestMessage
	<Obsolete, Serializable>
	Public Class CbowRequestMessage
		Inherits BaseVoidMessage
		Implements TrainingMessage, RequestMessage

		Protected Friend counter As SByte = 1

		Friend frameId As Long

		Protected Friend w1 As Integer
		Protected Friend syn0rows() As Integer
		Protected Friend syn1rows() As Integer
		Protected Friend alpha As Double
		Protected Friend nextRandom As Long
		Protected Friend negSamples As Integer
		Protected Friend codes() As SByte

		Protected Friend negatives() As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CbowRequestMessage(@NonNull int[] syn0rows, @NonNull int[] syn1rows, int w1, byte[] codes, int negSamples, double alpha, long nextRandom)
		Public Sub New(ByVal syn0rows() As Integer, ByVal syn1rows() As Integer, ByVal w1 As Integer, ByVal codes() As SByte, ByVal negSamples As Integer, ByVal alpha As Double, ByVal nextRandom As Long)
			Me.syn0rows = syn0rows
			Me.syn1rows = syn1rows
			Me.w1 = w1
			Me.alpha = alpha
			Me.nextRandom = nextRandom
			Me.negSamples = negSamples
			Me.codes = codes


			Me.setTaskId(BasicSequenceProvider.Instance.getNextValue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void processMessage()
		Public Overridable Overloads Sub processMessage()
			' we just pick training here
			Dim cbt As TrainingDriver(Of CbowRequestMessage) = CType(trainer, TrainingDriver(Of CbowRequestMessage))
			cbt.startTraining(Me)
		End Sub

		Public Overrides ReadOnly Property JoinSupported As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Sub joinMessage(ByVal message As VoidMessage)
			' TODO: apply proper join handling here
			counter += 1
		End Sub
	End Class

End Namespace