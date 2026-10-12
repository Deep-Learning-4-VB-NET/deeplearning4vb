Imports System
Imports System.Linq
Imports Data = lombok.Data
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
'ORIGINAL LINE: @Data @Slf4j @Deprecated public class SkipGramRequestMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.TrainingMessage, org.nd4j.parameterserver.distributed.messages.RequestMessage
	<Obsolete, Serializable>
	Public Class SkipGramRequestMessage
		Inherits BaseVoidMessage
		Implements TrainingMessage, RequestMessage

		' learning rate for this sequence
		Protected Friend alpha As Double

		Friend frameId As Long

		' current word & lastWord
		Protected Friend w1 As Integer
		Protected Friend w2 As Integer

		' following fields are for hierarchic softmax
		' points & codes for current word
		Protected Friend points() As Integer
		Protected Friend codes() As SByte

		Protected Friend negatives() As Integer

		Protected Friend negSamples As Short

		Protected Friend nextRandom As Long

		Protected Friend counter As SByte = 1

		Protected Friend Sub New()
			MyBase.New(0)
		End Sub

		Public Sub New(ByVal w1 As Integer, ByVal w2 As Integer, ByVal points() As Integer, ByVal codes() As SByte, ByVal negSamples As Short, ByVal lr As Double, ByVal nextRandom As Long)
			Me.New()
			Me.w1 = w1
			Me.w2 = w2
			Me.points = points
			Me.codes = codes
			Me.negSamples = negSamples
			Me.alpha = lr
			Me.nextRandom = nextRandom

			' FIXME: THIS IS TEMPORARY SOLUTION - FIX THIS!!!1
			Me.setTaskId(BasicSequenceProvider.Instance.getNextValue())
		End Sub

		''' <summary>
		''' This method does actual training for SkipGram algorithm
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void processMessage()
		Public Overridable Overloads Sub processMessage()
			''' <summary>
			''' This method in reality just delegates training to specific TrainingDriver, based on message opType.
			''' In this case - SkipGram training
			''' </summary>
			'log.info("sI_{} starts SkipGram round...", transport.getShardIndex());

			' FIXME: we might use something better then unchecked opType cast here
			Dim sgt As TrainingDriver(Of SkipGramRequestMessage) = CType(trainer, TrainingDriver(Of SkipGramRequestMessage))
			sgt.startTraining(Me)
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

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If
			If Not MyBase.Equals(o) Then
				Return False
			End If

			Dim message As SkipGramRequestMessage = DirectCast(o, SkipGramRequestMessage)

			If w1 <> message.w1 Then
				Return False
			End If
			If w2 <> message.w2 Then
				Return False
			End If
			If negSamples <> message.negSamples Then
				Return False
			End If
			If Not points.SequenceEqual(message.points) Then
				Return False
			End If
			Return codes.SequenceEqual(message.codes)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = MyBase.GetHashCode()
			result = 31 * result + w1
			result = 31 * result + w2
			result = 31 * result + Arrays.hashCode(points)
			result = 31 * result + Arrays.hashCode(codes)
			result = 31 * result + CInt(negSamples)
			Return result
		End Function
	End Class

End Namespace