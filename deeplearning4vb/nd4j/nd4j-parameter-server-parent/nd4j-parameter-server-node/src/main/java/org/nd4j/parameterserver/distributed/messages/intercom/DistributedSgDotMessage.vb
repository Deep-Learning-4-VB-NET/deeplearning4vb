Imports System
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ExecutionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode
Imports WordVectorStorage = org.nd4j.parameterserver.distributed.logic.storage.WordVectorStorage
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports DistributedMessage = org.nd4j.parameterserver.distributed.messages.DistributedMessage
Imports DotAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.DotAggregation
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports SkipGramTrainer = org.nd4j.parameterserver.distributed.training.impl.SkipGramTrainer

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

Namespace org.nd4j.parameterserver.distributed.messages.intercom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j @Deprecated public class DistributedSgDotMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.DistributedMessage
	<Obsolete, Serializable>
	Public Class DistributedSgDotMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		Protected Friend rowsA() As Integer
		Protected Friend rowsB() As Integer

		' payload for trainer pickup
		Protected Friend w1, w2 As Integer
		Protected Friend useHS As Boolean
		Protected Friend negSamples As Short
		Protected Friend alpha As Single
		Protected Friend codes() As SByte

		Public Sub New()
			messageType_Conflict = 22
		End Sub

		<Obsolete>
		Public Sub New(ByVal taskId As Long, ByVal rowA As Integer, ByVal rowB As Integer)
			Me.New(taskId, New Integer() {rowA}, New Integer() {rowB}, 0, 0, New SByte() {}, False, CShort(0), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedSgDotMessage(long taskId, @NonNull int[] rowsA, @NonNull int[] rowsB, int w1, int w2, @NonNull byte[] codes, boolean useHS, short negSamples, float alpha)
		Public Sub New(ByVal taskId As Long, ByVal rowsA() As Integer, ByVal rowsB() As Integer, ByVal w1 As Integer, ByVal w2 As Integer, ByVal codes() As SByte, ByVal useHS As Boolean, ByVal negSamples As Short, ByVal alpha As Single)
			Me.New()
			Me.rowsA = rowsA
			Me.rowsB = rowsB
			Me.taskId = taskId

			Me.w1 = w1
			Me.w2 = w2
			Me.useHS = useHS
			Me.negSamples = negSamples
			Me.alpha = alpha
			Me.codes = codes
		End Sub

		''' <summary>
		''' This method calculates dot of gives rows
		''' </summary>
		Public Overrides Sub processMessage()
			' this only picks up new training round
			'log.info("sI_{} Processing DistributedSgDotMessage taskId: {}", transport.getShardIndex(), getTaskId());

			Dim sgrm As New SkipGramRequestMessage(w1, w2, rowsB, codes, negSamples, alpha, 119)
			If negSamples > 0 Then
				' unfortunately we have to get copy of negSamples here
				Dim negatives() As Integer = Arrays.CopyOfRange(rowsB, codes.Length, rowsB.Length)
				sgrm.setNegatives(negatives)
			End If
			sgrm.setTaskId(Me.taskId)
			sgrm.OriginatorId = Me.OriginatorId



			' FIXME: get rid of THAT
			Dim sgt As SkipGramTrainer = CType(trainer, SkipGramTrainer)
			sgt.pickTraining(sgrm)

			'TODO: make this thing a single op, even specialOp is ok
			' we calculate dot for all involved rows

			Dim resultLength As Integer = codes.Length + (If(negSamples > 0, (negSamples + 1), 0))

			Dim result As INDArray = Nd4j.createUninitialized(resultLength, 1)
			Dim e As Integer = 0
			Do While e < codes.Length
				Dim dot As Double = Nd4j.BlasWrapper.dot(storage.getArray(WordVectorStorage.SYN_0).getRow(w2), storage.getArray(WordVectorStorage.SYN_1).getRow(rowsB(e)))
				result.putScalar(e, dot)
				e += 1
			Loop

			' negSampling round
			Do While e < resultLength
				Dim dot As Double = Nd4j.BlasWrapper.dot(storage.getArray(WordVectorStorage.SYN_0).getRow(w2), storage.getArray(WordVectorStorage.SYN_1_NEGATIVE).getRow(rowsB(e)))
				result.putScalar(e, dot)
				e += 1
			Loop

			If voidConfiguration.getExecutionMode() = ExecutionMode.AVERAGING Then
				' just local bypass
				Dim dot As New DotAggregation(taskId, CShort(1), shardIndex, result)
				dot.TargetId = (Short) -1
				dot.OriginatorId = OriginatorId
				transport.putMessage(dot)
			ElseIf voidConfiguration.getExecutionMode() = ExecutionMode.SHARDED Then
				' send this message to everyone
				Dim dot As New DotAggregation(taskId, CShort(Math.Truncate(voidConfiguration.getNumberOfShards())), shardIndex, result)
				dot.TargetId = (Short) -1
				dot.OriginatorId = OriginatorId
				transport.sendMessage(dot)
			End If
		End Sub
	End Class

End Namespace