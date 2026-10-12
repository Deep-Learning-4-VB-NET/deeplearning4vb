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
Imports CbowRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.CbowRequestMessage
Imports CbowTrainer = org.nd4j.parameterserver.distributed.training.impl.CbowTrainer

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
'ORIGINAL LINE: @Data @Slf4j @Deprecated public class DistributedCbowDotMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.DistributedMessage
	<Obsolete, Serializable>
	Public Class DistributedCbowDotMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		Protected Friend rowsA() As Integer
		Protected Friend rowsB() As Integer

		' payload for trainer pickup
		Protected Friend w1 As Integer
		Protected Friend useHS As Boolean
		Protected Friend negSamples As Short
		Protected Friend alpha As Single
		Protected Friend codes() As SByte

		Public Sub New()
			messageType_Conflict = 22
		End Sub

		<Obsolete>
		Public Sub New(ByVal taskId As Long, ByVal rowA As Integer, ByVal rowB As Integer)
			Me.New(taskId, New Integer() {rowA}, New Integer() {rowB}, rowA, New SByte() {}, False, CShort(0), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedCbowDotMessage(long taskId, @NonNull int[] rowsA, @NonNull int[] rowsB, int w1, @NonNull byte[] codes, boolean useHS, short negSamples, float alpha)
		Public Sub New(ByVal taskId As Long, ByVal rowsA() As Integer, ByVal rowsB() As Integer, ByVal w1 As Integer, ByVal codes() As SByte, ByVal useHS As Boolean, ByVal negSamples As Short, ByVal alpha As Single)
			Me.New()
			Me.rowsA = rowsA
			Me.rowsB = rowsB
			Me.taskId = taskId

			Me.w1 = w1
			Me.useHS = useHS
			Me.negSamples = negSamples
			Me.alpha = alpha
			Me.codes = codes


			'if (this.rowsA.length != this.rowsB.length)
			'    throw new ND4JIllegalStateException("Length of X should match length of Y");
		End Sub

		''' <summary>
		''' This method calculates dot of gives rows, with averaging applied to rowsA, as required by CBoW
		''' </summary>
		Public Overrides Sub processMessage()
			' this only picks up new training round
			'log.info("sI_{} Starting CBOW dot...", transport.getShardIndex());

			Dim cbrm As New CbowRequestMessage(rowsA, rowsB, w1, codes, negSamples, alpha, 119)
			If negSamples > 0 Then
				' unfortunately we have to get copy of negSamples here
				Dim negatives() As Integer = Arrays.CopyOfRange(rowsB, codes.Length, rowsB.Length)
				cbrm.setNegatives(negatives)
			End If
			cbrm.FrameId = -119L
			cbrm.setTaskId(Me.taskId)
			cbrm.OriginatorId = Me.OriginatorId


			' FIXME: get rid of THAT
			Dim cbt As CbowTrainer = CType(trainer, CbowTrainer)
			cbt.pickTraining(cbrm)


			' we calculate dot for all involved rows, and first of all we get mean word
			Dim words As INDArray = Nd4j.pullRows(storage.getArray(WordVectorStorage.SYN_0), 1, rowsA, "c"c)
			Dim mean As INDArray = words.mean(0)

			Dim resultLength As Integer = codes.Length + (If(negSamples > 0, (negSamples + 1), 0))

			Dim result As INDArray = Nd4j.createUninitialized(resultLength, 1)
			Dim e As Integer = 0
			Do While e < codes.Length
				Dim dot As Double = Nd4j.BlasWrapper.dot(mean, storage.getArray(WordVectorStorage.SYN_1).getRow(rowsB(e)))
				result.putScalar(e, dot)
				e += 1
			Loop

			' negSampling round
			Do While e < resultLength
				Dim dot As Double = Nd4j.BlasWrapper.dot(mean, storage.getArray(WordVectorStorage.SYN_1_NEGATIVE).getRow(rowsB(e)))
				result.putScalar(e, dot)
				e += 1
			Loop

			If voidConfiguration.getExecutionMode() = ExecutionMode.AVERAGING Then
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