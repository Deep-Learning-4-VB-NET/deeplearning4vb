Imports System
Imports Consumer = io.reactivex.functions.Consumer
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports org.deeplearning4j.optimize.solvers.accumulation
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports IndexedTail = org.deeplearning4j.optimize.solvers.accumulation.IndexedTail
Imports SmartFancyBlockingQueue = org.deeplearning4j.optimize.solvers.accumulation.SmartFancyBlockingQueue
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ThresholdCompression = org.nd4j.linalg.compression.ThresholdCompression
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports UpdatesHandler = org.nd4j.parameterserver.distributed.v2.transport.UpdatesHandler
Imports Subscriber = org.reactivestreams.Subscriber
Imports Subscription = org.reactivestreams.Subscription

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

Namespace org.deeplearning4j.spark.parameterserver.networking.v2


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Builder @Slf4j public class UpdatesConsumer implements org.nd4j.parameterserver.distributed.v2.transport.UpdatesHandler
	Public Class UpdatesConsumer
		Implements UpdatesHandler

		Protected Friend numWorkers As Integer

		<NonSerialized>
		Protected Friend params As INDArray
		<NonSerialized>
		Protected Friend updates As INDArray
		<NonSerialized>
		Protected Friend stepFunction As StepFunction

		<NonSerialized>
		Protected Friend accumulator As GradientsAccumulator

		<NonSerialized>
		Protected Friend ReadOnly updatesCount As New AtomicLong(0)
		<NonSerialized>
		Protected Friend ReadOnly hasSomething As New AtomicBoolean(False)
'JAVA TO VB CONVERTER NOTE: The field bypassMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend ReadOnly bypassMode_Conflict As New AtomicBoolean(False)
		<NonSerialized>
		Protected Friend ReadOnly denseCounter As New AtomicLong(0)
		<NonSerialized>
		Protected Friend ReadOnly sparseCounter As New AtomicLong(0)

		' make this stuff configurable
		<NonSerialized>
		Protected Friend updatesBuffer As IndexedTail

		Public Overrides Sub onSubscribe(ByVal subscription As Subscription)
			' no-op
		End Sub

		''' <summary>
		''' This </summary>
		''' <param name="reallBypass"> </param>
		Public Overridable Sub bypassMode(ByVal reallBypass As Boolean)
			bypassMode_Conflict.set(reallBypass)
		End Sub

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property BypassMod As Boolean
			Get
				Return bypassMode_Conflict.get()
			End Get
		End Property

		Public Overridable ReadOnly Property UpdatesQueue As IndexedTail
			Get
				If updatesBuffer Is Nothing AndAlso accumulator IsNot Nothing Then
					SyncLock Me
						If updatesBuffer Is Nothing Then
							updatesBuffer = New IndexedTail(numWorkers, True, params.shape())
						End If
					End SyncLock
				End If
    
				Return updatesBuffer
			End Get
		End Property

		Public Overrides Sub onNext(ByVal array As INDArray)
			If updatesBuffer Is Nothing AndAlso accumulator IsNot Nothing Then
				SyncLock Me
					If updatesBuffer Is Nothing Then
						updatesBuffer = New IndexedTail(numWorkers, True, params.shape())
					End If
				End SyncLock
			End If

			If Not bypassMode_Conflict.get() Then
				If accumulator IsNot Nothing Then
					' this means consumer runs on worker node

					Try
						' we're just storing update into buffer, and it'll be consumed by GradientsAccumulator on next cycle
						'log.info("Putting update to the queue, current size: [{}]", updatesBuffer.size());
						updatesBuffer.put(array)
					Catch e As Exception
						log.error("",e)
						Throw New Exception(e)
					End Try
				ElseIf params IsNot Nothing AndAlso stepFunction IsNot Nothing Then
					SyncLock Me
						' threshold decoder is inplace & fast
						Dim encoding As Integer = array.data().getInt(3)
						If encoding = ThresholdCompression.FLEXIBLE_ENCODING Then
							Nd4j.Executioner.thresholdDecode(array, updates)
							sparseCounter.incrementAndGet()
						ElseIf encoding = ThresholdCompression.BITMAP_ENCODING Then
							Nd4j.Executioner.bitmapDecode(array, updates)
							denseCounter.incrementAndGet()
						Else
							Throw New DL4JInvalidConfigException("Unknown compression header received: " & encoding)
						End If


						' this simple flag shows that we have something not applied, will be used at finishTraining() method
						hasSomething.set(True)

						' we apply updates every X iterations, and we don't really need X to be small here
						If updatesCount.incrementAndGet() Mod 32 = 0 Then
							flush()
						End If
					End SyncLock
				Else
					Throw New ND4JIllegalStateException("Accumulator & StepFunction is null at the same time")
				End If
			End If
		End Sub

		Public Overridable Sub flush()
			SyncLock Me
				If params IsNot Nothing AndAlso updates IsNot Nothing AndAlso hasSomething.get() Then
					stepFunction.step(params, updates)
					Nd4j.Executioner.commit()

					log.debug("Applying updates. Current ratio: [{}]; Sparse: [{}]; Dense: [{}];", CDbl(sparseCounter.get()) / denseCounter.get(), sparseCounter.get(), denseCounter.get())

					' once accumulated updates are applied - reset storage, and wait for other messsages
					Nd4j.MemoryManager.memset(updates)
					hasSomething.set(False)
				End If
			End SyncLock
		End Sub

		Public Overrides Sub onError(ByVal throwable As Exception)
			Throw New Exception(throwable)
		End Sub

		Public Overrides Sub onComplete()
			' no-op
		End Sub

		Public Overridable ReadOnly Property ParametersArray As INDArray Implements UpdatesHandler.getParametersArray
			Get
				SyncLock Me
					Return params.dup(params.ordering())
				End SyncLock
			End Get
		End Property
	End Class

End Namespace