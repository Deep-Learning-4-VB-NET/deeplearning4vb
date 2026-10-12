Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ExecutionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode
Imports FrameCompletionHandler = org.nd4j.parameterserver.distributed.logic.completion.FrameCompletionHandler
Imports RequestDescriptor = org.nd4j.parameterserver.distributed.logic.completion.RequestDescriptor
Imports WordVectorStorage = org.nd4j.parameterserver.distributed.logic.storage.WordVectorStorage
Imports VoidAggregation = org.nd4j.parameterserver.distributed.messages.VoidAggregation
Imports DotAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.DotAggregation
Imports FrameCompleteMessage = org.nd4j.parameterserver.distributed.messages.complete.FrameCompleteMessage
Imports DistributedCbowDotMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedCbowDotMessage
Imports CbowRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.CbowRequestMessage
Imports org.nd4j.parameterserver.distributed.training
Imports CbowChain = org.nd4j.parameterserver.distributed.training.chains.CbowChain

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
'ORIGINAL LINE: @Slf4j public class CbowTrainer extends org.nd4j.parameterserver.distributed.training.BaseTrainer<org.nd4j.parameterserver.distributed.messages.requests.CbowRequestMessage>
	Public Class CbowTrainer
		Inherits BaseTrainer(Of CbowRequestMessage)

		Private Const HS_MAX_EXP As Single = 6.0f

		Protected Friend chains As IDictionary(Of RequestDescriptor, CbowChain) = New ConcurrentDictionary(Of RequestDescriptor, CbowChain)()
		Protected Friend cntRounds As New AtomicLong(0)



		Public Overrides Sub startTraining(ByVal message As CbowRequestMessage)
			Dim chain As New CbowChain(message)
			chain.addElement(message)

			chains(RequestDescriptor.createDescriptor(message.OriginatorId, message.TaskId)) = chain

			Dim row_syn1() As Integer = message.getSyn1rows()

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

			If message.getSyn0rows() Is Nothing OrElse message.getSyn0rows().length < 1 Then
				Throw New Exception("Empty syn0rows!")
			End If

			Dim dcdm As New DistributedCbowDotMessage(message.TaskId, message.getSyn0rows(), row_syn1, message.getW1(), message.getCodes(), message.getCodes().length > 0, CShort(Math.Truncate(message.getNegSamples())), CSng(message.getAlpha()))
			dcdm.TargetId = (Short) -1
			dcdm.OriginatorId = message.OriginatorId

			If voidConfiguration.getExecutionMode() = ExecutionMode.AVERAGING Then
				transport.putMessage(dcdm)
			ElseIf voidConfiguration.getExecutionMode() = ExecutionMode.SHARDED Then
				transport.sendMessage(dcdm)
			End If
		End Sub

		Public Overrides Sub pickTraining(ByVal message As CbowRequestMessage)
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(message.OriginatorId, message.TaskId)
			If Not chains.ContainsKey(descriptor) Then
				Dim chain As New CbowChain(message)
				chain.addElement(message)
				chains(descriptor) = chain
			End If
		End Sub

		Public Overrides Sub aggregationFinished(ByVal aggregation As VoidAggregation)
			' we just pick DotAggregation here

			Dim chain As CbowChain = chains(RequestDescriptor.createDescriptor(aggregation.OriginatorId, aggregation.TaskId))
			If chain Is Nothing Then
				Throw New Exception("sI_" & transport.ShardIndex & " Unable to find chain for specified originatorId: [" & aggregation.OriginatorId & "]; taskId: [" & aggregation.TaskId & "]")
			End If

			chain.addElement(DirectCast(aggregation, DotAggregation))

			finishTraining(aggregation.OriginatorId, aggregation.TaskId)
		End Sub

		Public Overrides Sub finishTraining(ByVal originatorId As Long, ByVal taskId As Long)
			Dim chainDesc As RequestDescriptor = RequestDescriptor.createDescriptor(originatorId, taskId)
			Dim chain As CbowChain = chains(chainDesc)

			If chain Is Nothing Then
				Throw New Exception("Unable to find chain for specified taskId: [" & taskId & "]")
			End If

			Dim cbr As CbowRequestMessage = chain.getCbowRequest()
			Dim alpha As Double = cbr.getAlpha()

			'log.info("Executing SkipGram round on shard_{}; taskId: {}", transport.getShardIndex(), taskId);

			' TODO: We DON'T want this code being here
			' TODO: We DO want this algorithm to be native
			Dim expTable As INDArray = storage.getArray(WordVectorStorage.EXP_TABLE)
			Dim dots As INDArray = chain.getDotAggregation().getAccumulatedResult()

			Dim syn0 As INDArray = storage.getArray(WordVectorStorage.SYN_0)
			Dim syn1 As INDArray = storage.getArray(WordVectorStorage.SYN_1)
			Dim syn1Neg As INDArray = storage.getArray(WordVectorStorage.SYN_1_NEGATIVE)

			Dim words As INDArray = Nd4j.pullRows(storage.getArray(WordVectorStorage.SYN_0), 1, cbr.getSyn0rows(), "c"c)
			Dim neue As INDArray = words.mean(0)

			Dim neu1e As INDArray = Nd4j.create(syn0.columns())

			Dim e As Integer = 0

			Dim updated As Boolean = False

			' probably applying HS part
			If cbr.getCodes().length > 0 Then
				Do While e < cbr.getCodes().length
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

					Dim code As Integer = cbr.getCodes()(e)
					Dim f As Double = expTable.getFloat(idx)
					Dim g As Double = (1 - code - f) * alpha

					updated = True
					Nd4j.BlasWrapper.axpy(New Double?(g), syn1.getRow(cbr.getSyn1rows()(e)), neu1e)
					Nd4j.BlasWrapper.axpy(New Double?(g), neue, syn1.getRow(cbr.getSyn1rows()(e)))
					e += 1
				Loop
			End If

			If cbr.getNegSamples() > 0 Then
				Dim cnt As Integer = 0
				Do While e < cbr.getNegSamples() + 1
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
					Nd4j.BlasWrapper.axpy(New Double?(g), syn1Neg.getRow(cbr.getNegatives()(cnt)), neu1e)
					Nd4j.BlasWrapper.axpy(New Double?(g), neue, syn1Neg.getRow(cbr.getNegatives()(cnt)))
					e += 1
					cnt += 1
				Loop
			End If

			If updated Then
				Dim i As Integer = 0
				Do While i < cbr.getSyn0rows().length
					Nd4j.BlasWrapper.axpy(New Double?(1.0), neu1e, syn0.getRow(cbr.getSyn0rows()(i)))
					i += 1
				Loop
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
				'log.info("sI_{} isn't tracking this frame: Originator: {}, frameId: {}, taskId: {}", transport.getShardIndex(), chain.getOriginatorId(), chain.getFrameId(), taskId );
			End If



			If cntRounds.incrementAndGet() Mod 100000 = 0 Then
				log.info("{} training rounds finished...", cntRounds.get())
			End If

			' don't forget to remove chain, it'll become a leak otherwise
			chains.Remove(chainDesc)
		End Sub


		Public Overrides Function targetMessageClass() As String
			Return GetType(CbowRequestMessage).Name
		End Function
	End Class

End Namespace