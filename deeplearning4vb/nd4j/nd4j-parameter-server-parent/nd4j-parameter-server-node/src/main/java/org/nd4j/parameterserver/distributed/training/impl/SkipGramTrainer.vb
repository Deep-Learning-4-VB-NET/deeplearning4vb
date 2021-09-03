Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ExecutionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode
Imports FrameCompletionHandler = org.nd4j.parameterserver.distributed.logic.completion.FrameCompletionHandler
Imports RequestDescriptor = org.nd4j.parameterserver.distributed.logic.completion.RequestDescriptor
Imports WordVectorStorage = org.nd4j.parameterserver.distributed.logic.storage.WordVectorStorage
Imports DotAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.DotAggregation
Imports VoidAggregation = org.nd4j.parameterserver.distributed.messages.VoidAggregation
Imports FrameCompleteMessage = org.nd4j.parameterserver.distributed.messages.complete.FrameCompleteMessage
Imports DistributedSgDotMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedSgDotMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports org.nd4j.parameterserver.distributed.training
Imports SkipGramChain = org.nd4j.parameterserver.distributed.training.chains.SkipGramChain

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

Namespace org.nd4j.parameterserver.distributed.training.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SkipGramTrainer extends org.nd4j.parameterserver.distributed.training.BaseTrainer<org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage>
	Public Class SkipGramTrainer
		Inherits BaseTrainer(Of SkipGramRequestMessage)

		Private Const HS_MAX_EXP As Single = 6.0f

		Protected Friend chains As IDictionary(Of RequestDescriptor, SkipGramChain) = New ConcurrentDictionary(Of RequestDescriptor, SkipGramChain)()
		Protected Friend cntRounds As New AtomicLong(0)

		Public Overrides Sub startTraining(ByVal message As SkipGramRequestMessage)
			''' <summary>
			''' All we do right HERE - is dot calculation start
			''' </summary>

			''' <summary>
			''' If we're on HS, we know pairs in advance: it's our points.
			''' </summary>
			'        log.info("sI_{} adding SkipGramChain originator: {}; frame: {}; task: {}", transport.getShardIndex(), message.getOriginatorId(), message.getFrameId(), message.getTaskId());
			Dim chain As New SkipGramChain(message.OriginatorId, message.TaskId, message.FrameId)
			chain.addElement(message)

			'        log.info("Starting chain [{}]", chain.getTaskId());


			chains(RequestDescriptor.createDescriptor(message.OriginatorId, message.TaskId)) = chain

			' we assume this is HS round
			'if (message.getPoints() != null && message.getPoints().length > 0) {

			Dim row_syn0(-1) As Integer 'replicate(message.getW2(), message.getPoints().length);

			Dim row_syn1() As Integer = message.getPoints()

			If message.getNegSamples() > 0 Then
				Dim rows As Integer = CInt(storage.getArray(WordVectorStorage.SYN_0).rows())
				Dim tempArray(message.getNegSamples()) As Integer
				tempArray(0) = message.getW1()

				Dim e As Integer = 1
				Do While e < message.getNegSamples() + 1
					Do
						Dim rnd As Integer = RandomUtils.nextInt(0, rows)
						If rnd <> message.getW1() Then
							tempArray(e) = rnd
							Exit Do
						End If
					Loop
					e += 1
				Loop

				row_syn1 = ArrayUtils.addAll(row_syn1, tempArray)

				message.setNegatives(tempArray)
			End If

			If message.getPoints().length <> message.getCodes().length Then
				Throw New Exception("Mismatiching points/codes lengths here!")
			End If

			' FIXME: taskId should be real here, since it'll be used for task chain tracking
			' as result, we'll have aggregated dot as single ordered column, which might be used for gradient calculation
			Dim ddm As New DistributedSgDotMessage(message.TaskId, row_syn0, row_syn1, message.getW1(), message.getW2(), message.getCodes(), message.getCodes() IsNot Nothing AndAlso message.getCodes().length > 0, message.getNegSamples(), CSng(message.getAlpha()))

			ddm.TargetId = (Short) -1
			ddm.OriginatorId = message.OriginatorId


			If voidConfiguration.getExecutionMode() = ExecutionMode.AVERAGING Then
				transport.putMessage(ddm)
			ElseIf voidConfiguration.getExecutionMode() = ExecutionMode.SHARDED Then
				transport.sendMessage(ddm)
			End If

			'  } //else log.info("sI_{} Skipping step: {}", transport.getShardIndex(), chain.getTaskId());

		End Sub

		''' <summary>
		''' This method will be called from non-initialized Shard context </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void pickTraining(@NonNull SkipGramRequestMessage message)
		Public Overrides Sub pickTraining(ByVal message As SkipGramRequestMessage)
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(message.getOriginatorId(), message.getTaskId())
			If Not chains.ContainsKey(descriptor) Then
				Dim chain As New SkipGramChain(message)
				'            log.info("sI_{} Picking chain: originator: {}; taskId: {}", transport.getShardIndex(), message.getOriginatorId(), message.getTaskId());
				chains(descriptor) = chain
			End If
		End Sub

		Public Overrides Function targetMessageClass() As String
			Return GetType(SkipGramRequestMessage).Name
		End Function

		''' <summary>
		''' This method is invoked after particular aggregation finished </summary>
		''' <param name="aggregation"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void aggregationFinished(@NonNull VoidAggregation aggregation)
		Public Overrides Sub aggregationFinished(ByVal aggregation As VoidAggregation)
			' the only possible aggregation here is DotAggregation, actually
			' so we just calculate gradients here

			Dim chain As SkipGramChain = chains(RequestDescriptor.createDescriptor(aggregation.getOriginatorId(), aggregation.getTaskId()))

			If chain Is Nothing Then
				Throw New Exception("sI_" & transport.ShardIndex & " Unable to find chain for specified originatorId: [" & aggregation.getOriginatorId() & "]; taskId: [" & aggregation.getTaskId() & "]")
			End If

			chain.addElement(CType(aggregation, DotAggregation))

			finishTraining(aggregation.getOriginatorId(), aggregation.getTaskId())
		End Sub

		Public Overrides Sub finishTraining(ByVal originatorId As Long, ByVal taskId As Long)
			Dim chainDesc As RequestDescriptor = RequestDescriptor.createDescriptor(originatorId, taskId)
			Dim chain As SkipGramChain = chains(chainDesc)

			If chain Is Nothing Then
				Throw New Exception("Unable to find chain for specified taskId: [" & taskId & "]")
			End If

			Dim sgrm As SkipGramRequestMessage = chain.getRequestMessage()
			Dim alpha As Double = sgrm.getAlpha()

			'log.info("Executing SkipGram round on shard_{}; taskId: {}", transport.getShardIndex(), taskId);

			' TODO: We DON'T want this code being here
			' TODO: We DO want this algorithm to be native
			Dim expTable As INDArray = storage.getArray(WordVectorStorage.EXP_TABLE)
			Dim dots As INDArray = chain.getDotAggregation().getAccumulatedResult()

			Dim syn0 As INDArray = storage.getArray(WordVectorStorage.SYN_0)
			Dim syn1 As INDArray = storage.getArray(WordVectorStorage.SYN_1)
			Dim syn1Neg As INDArray = storage.getArray(WordVectorStorage.SYN_1_NEGATIVE)

			Dim neu1e As INDArray = Nd4j.create(syn0.columns())

			Dim e As Integer = 0

			Dim updated As Boolean = False

			' apply optional SkipGram HS gradients
			If sgrm.getCodes().length > 0 Then
				Do While e < sgrm.getCodes().length
					Dim dot As Single = dots.getFloat(e)

					If dot < -HS_MAX_EXP OrElse dot >= HS_MAX_EXP Then
						e += 1
						Continue Do
					End If

					Dim idx As Integer = CInt(Math.Truncate((dot + HS_MAX_EXP) * (CSng(expTable.length()) / HS_MAX_EXP / 2.0)))

					If idx >= expTable.length() OrElse idx < 0 Then
						e += 1
						Continue Do
					End If

					Dim code As Integer = chain.getRequestMessage().getCodes()(e)
					Dim f As Double = expTable.getFloat(idx)
					Dim g As Double = (1 - code - f) * alpha

					updated = True
					Nd4j.BlasWrapper.axpy(New Double?(g), syn1.getRow(sgrm.getPoints()(e)), neu1e)
					Nd4j.BlasWrapper.axpy(New Double?(g), syn0.getRow(sgrm.getW2()), syn1.getRow(sgrm.getPoints()(e)))
					e += 1
				Loop
			End If

			' apply optional NegSample gradients
			If sgrm.getNegSamples() > 0 Then
				' here we assume that we already
				Dim cnt As Integer = 0
				Do While e < sgrm.getNegSamples() + 1
					Dim dot As Single = dots.getFloat(e)

					Dim code As Single = If(cnt = 0, 1.0f, 0.0f)
					Dim g As Double = 0.0f

					If dot > HS_MAX_EXP Then
						g = (code - 1) * alpha
					ElseIf dot < -HS_MAX_EXP Then
						g = (code - 0) * alpha
					Else
						Dim idx As Integer = CInt(Math.Truncate((dot + HS_MAX_EXP) * (expTable.length() / HS_MAX_EXP / 2.0)))
						If idx >= expTable.length() OrElse idx < 0 Then
							e += 1
					cnt += 1
							Continue Do
						End If

						g = (code - expTable.getDouble(idx)) * alpha
					End If

					updated = True
					Nd4j.BlasWrapper.axpy(New Double?(g), syn1Neg.getRow(sgrm.getNegatives()(cnt)), neu1e)
					Nd4j.BlasWrapper.axpy(New Double?(g), syn0.getRow(sgrm.getW2()), syn1Neg.getRow(sgrm.getNegatives()(cnt)))
					e += 1
					cnt += 1
				Loop
			End If

			If updated Then
				Nd4j.BlasWrapper.axpy(New Double?(1.0), neu1e, syn0.getRow(sgrm.getW2()))
			End If

			' we send back confirmation message only from Shard which received this message
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(chain.OriginatorId, chain.getFrameId())

			If completionHandler.isTrackingFrame(descriptor) Then
				completionHandler.notifyFrame(chain.OriginatorId, chain.getFrameId(), chain.TaskId)

				If completionHandler.isCompleted(descriptor) Then
					Dim frameDescriptor As FrameCompletionHandler.FrameDescriptor = completionHandler.getCompletedFrameInfo(descriptor)


					' TODO: there is possible race condition here
					If frameDescriptor IsNot Nothing Then
						Dim fcm As New FrameCompleteMessage(chain.getFrameId())
						fcm.OriginatorId = frameDescriptor.getFrameOriginatorId()
						transport.sendMessage(fcm)
					Else
						log.warn("Frame double spending detected")
					End If
				End If
			Else
				log.info("sI_{} isn't tracking this frame: Originator: {}, frameId: {}, taskId: {}", transport.ShardIndex, chain.OriginatorId, chain.getFrameId(), taskId)
			End If

			If cntRounds.incrementAndGet() Mod 100000 = 0 Then
				log.info("{} training rounds finished...", cntRounds.get())
			End If

			' don't forget to remove chain, it'll become a leak otherwise
			chains.Remove(chainDesc)
		End Sub

	End Class

End Namespace