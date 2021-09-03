Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AtomicDouble = org.nd4j.shade.guava.util.concurrent.AtomicDouble
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports ThresholdAlgorithmReducer = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithmReducer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.deeplearning4j.optimize.solvers.accumulation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EncodingHandler implements MessageHandler
	<Serializable>
	Public Class EncodingHandler
		Implements MessageHandler

		Public Const THRESHOLD_LOG_FREQ_MS As Long = 10000 'Every 10 sec max by default
		<NonSerialized>
		Protected Friend accumulator As GradientsAccumulator
		Protected Friend initialThresholdAlgorithm As ThresholdAlgorithm
		Protected Friend initialResidualPostProcessor As ResidualPostProcessor

		Protected Friend boundary As Integer?
		Protected Friend encodingDebugMode As Boolean
		Protected Friend atomicBoundary As New AtomicInteger(-1)

		Protected Friend thresholdAlgorithm As New ThreadLocal(Of ThresholdAlgorithm)()
		Protected Friend allThreadThresholdAlgorithms As IDictionary(Of Long, ThresholdAlgorithm) = New ConcurrentDictionary(Of Long, ThresholdAlgorithm)() 'All instances - we need to average them at the end once training is complete
		Protected Friend residualPostProcessor As New ThreadLocal(Of ResidualPostProcessor)()
		Protected Friend iterations As New ThreadLocal(Of AtomicLong)()
		Protected Friend lastStep As New ThreadLocal(Of AtomicLong)()
		Protected Friend lastThreshold As New ThreadLocal(Of AtomicDouble)()
		Protected Friend lastSparsityRatio As New ThreadLocal(Of AtomicDouble)()
		Protected Friend currentThreshold As New ThreadLocal(Of AtomicDouble)()
		Protected Friend bitmapMode As New ThreadLocal(Of AtomicBoolean)()
		Protected Friend lastIterWasDense As New ThreadLocal(Of AtomicBoolean)() 'Same as bitmapMode but lagging by 1 iter

		Protected Friend ReadOnly lastThresholdLogTime As New AtomicLong()

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public EncodingHandler(final org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm thresholdAlgorithm, final org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor residualPostProcessor, System.Nullable<Integer> boundary, boolean encodingDebugMode)
		Public Sub New(ByVal thresholdAlgorithm As ThresholdAlgorithm, ByVal residualPostProcessor As ResidualPostProcessor, ByVal boundary As Integer?, ByVal encodingDebugMode As Boolean)
			Me.initialThresholdAlgorithm = thresholdAlgorithm
			Me.initialResidualPostProcessor = residualPostProcessor
			Me.boundary = If(boundary Is Nothing, Integer.MaxValue, boundary)
			Me.encodingDebugMode = encodingDebugMode
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void initialize(@NonNull GradientsAccumulator accumulator)
		Public Overridable Sub initialize(ByVal accumulator As GradientsAccumulator) Implements MessageHandler.initialize
			Me.accumulator = accumulator
		End Sub

		Public Overridable Function encodeUpdates(ByVal iteration As Integer, ByVal epoch As Integer, ByVal updates As INDArray) As INDArray
			If thresholdAlgorithm.get() Is Nothing Then
				SyncLock Me
					'Synchronized in case threshold algorithm has INDArrays and we're running on GPU - don't want race condition for shifting devices
					thresholdAlgorithm.set(initialThresholdAlgorithm.clone())
					allThreadThresholdAlgorithms(Thread.CurrentThread.getId()) = thresholdAlgorithm.get()
					If initialResidualPostProcessor IsNot Nothing Then
						'May be null for no post processing
						residualPostProcessor.set(initialResidualPostProcessor.clone())
					End If
				End SyncLock
			End If

			Dim lastThr As Double? = Nothing
			Dim lastWasDense As Boolean? = Nothing
			Dim lastSparsity As Double? = Nothing
			If lastThreshold.get() IsNot Nothing Then
				'Keep null on first iteration in an epoch, or get for later iterations
				lastThr = lastThreshold.get().get()
				lastWasDense = lastIterWasDense.get().get()
				lastSparsity = If(lastWasDense.Value OrElse lastSparsityRatio.get() Is Nothing, Nothing, lastSparsityRatio.get().get())
			End If



			'Determine current threshold to use:
			Dim currThreshold As Double = thresholdAlgorithm.get().calculateThreshold(iteration, epoch, lastThr, lastWasDense, lastSparsity, updates)
			If bitmapMode.get() Is Nothing Then 'Initialize values for this thread on first iteration (per epoch)
				bitmapMode.set(New AtomicBoolean(True))
				currentThreshold.set(New AtomicDouble(currThreshold))
				iterations.set(New AtomicLong(0))
				lastStep.set(New AtomicLong(0))

				lastThreshold.set(New AtomicDouble(currThreshold))
				lastIterWasDense.set(New AtomicBoolean())
			End If

			currentThreshold.get().set(currThreshold)
			lastThreshold.get().set(currThreshold)

			'Debug output if enabled:
			residualDebugOutputIfRequired(updates)

			iterations.get().incrementAndGet()

			If boundary IsNot Nothing AndAlso atomicBoundary.get() < 0 Then
				atomicBoundary.compareAndSet(-1, CInt(updates.length() \ 16))
			End If

			Dim encoded As INDArray

			If Not bitmapMode.get().get() Then
				'Sparse updates
				encoded = Nd4j.Executioner.thresholdEncode(updates, currentThreshold.get().get(),If(boundary Is Nothing, Nothing, atomicBoundary.get()))

				' updates were TOO sparse, nothing to share here
				If encoded Is Nothing Then
					bitmapMode.get().set(False)
					If lastSparsityRatio.get() Is Nothing Then
						lastSparsityRatio.set(New AtomicDouble(0.0))
					Else
						lastSparsityRatio.get().set(0.0)
					End If
					lastIterWasDense.get().set(False)
					logThresholdIfReq(False, iteration, epoch)
					Return Nothing
				End If


				Dim encLen As Double = encoded.length()

				' if updates are too dense - we fallback to bitmap encoding
				If encLen >= (updates.length() \ 16) Then
					log.debug("Switching back to bitmapEncoding: iteration {}, epoch {}, threshold {}, encoded length {}", iteration, epoch, currThreshold, encLen)
					bitmapMode.get().set(True)

					encoded = Nd4j.Executioner.bitmapEncode(updates, currentThreshold.get().get())

					applyPostProcessor(iteration, epoch, currThreshold, updates)
					lastSparsityRatio.set(Nothing)
					lastIterWasDense.get().set(True)
					logThresholdIfReq(True, iteration, epoch)
					Return encoded
				Else
					'Record sparsity for use in calculation
					Dim sparsityRatio As Double = encLen / CDbl(updates.length())
					If lastSparsityRatio.get() Is Nothing Then
						lastSparsityRatio.set(New AtomicDouble(sparsityRatio))
					Else
						lastSparsityRatio.get().set(sparsityRatio)
					End If
					lastIterWasDense.get().set(False)
				End If
			Else
				'Dense bitmap updates
				encoded = Nd4j.create(DataType.INT32, updates.length() \ 16 + 5)

				Dim values As Long = Nd4j.Executioner.bitmapEncode(updates, encoded, currentThreshold.get().get())

				If values < (updates.length() \ 16 + 5) \ 2 Then
					Dim current As Boolean = bitmapMode.get().get()
					bitmapMode.get().set(False)
					If Not current Then
						log.debug("Switched to threshold encoding: iteration {}, epoch {}, threshold {}, number of values {}", iteration, epoch, currThreshold, values)
					End If
				End If

				lastSparsityRatio.set(Nothing)
				lastIterWasDense.get().set(True)
			End If

			'if (encoded != null)
			'log.info("Encoded length: {}, Original/encoded ratio: {}", encoded.data().length(), String.format("%.3f", encoded.data().length() * 100.0 / updates.lengthLong()));
			'log.info("Thread: {}; Encoded length: {}", Thread.currentThread().getId(), Arrays.toString(encoded.data().asInt()));

			applyPostProcessor(iteration, epoch, currThreshold, updates)
			logThresholdIfReq(lastIterWasDense.get().get(), iteration, epoch)
			Return encoded
		End Function

		Public Overridable Sub applyPostProcessor(ByVal iteration As Integer, ByVal epoch As Integer, ByVal lastThreshold As Double?, ByVal residuals As INDArray)
			If initialResidualPostProcessor Is Nothing Then
				Return 'No op
			End If

			residualPostProcessor.get().processResidual(iteration, epoch, lastThreshold, residuals)
		End Sub

		<Obsolete>
		Public Overridable Function decodeUpdates(ByVal message As INDArray) As INDArray
			' special op should be called here for decoding

			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' This method does loops encoded data back to updates queue </summary>
		''' <param name="message"> </param>
		Protected Friend Overridable Sub sendMessage(ByVal message As INDArray, ByVal iterationNumber As Integer, ByVal epochNumber As Integer)
			'INDArray update = decodeUpdates(message);
			accumulator.receiveUpdate(message)
		End Sub

		Public Overridable Function broadcastUpdates(ByVal updates As INDArray, ByVal iterationNumber As Integer, ByVal epochNumber As Integer) As Boolean Implements MessageHandler.broadcastUpdates
	'        
	'            we want to do 2 things here:
	'            1) encode updates
	'            2) send them somewhere
	'         
			Dim message As INDArray = encodeUpdates(iterationNumber, epochNumber, updates)
			If message IsNot Nothing Then
				sendMessage(message, iterationNumber, epochNumber)
				Return True
			Else
				Return False
			End If
		End Function

		Protected Friend Overridable Sub logThresholdIfReq(ByVal denseUpdates As Boolean, ByVal iter As Integer, ByVal epoch As Integer)
			Dim now As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim lastLog As Long = lastThresholdLogTime.get()
			If lastLog + THRESHOLD_LOG_FREQ_MS <= now Then
				If lastThresholdLogTime.compareAndSet(lastLog, now) Then 'Avoid RC for logging between multiple threads
					Dim lastThresholdStr As String = format(lastThreshold.get().get())
					If denseUpdates Then
						log.info("Threshold at iter {}, epoch {} [thread {}]: {}, DENSE updates", iter, epoch, Thread.CurrentThread.getId(), lastThresholdStr)
					Else
						Dim d As AtomicDouble = lastSparsityRatio.get()
						Dim lastSparsityStr As String
						If d Is Nothing Then
							lastSparsityStr = "-"
						Else
							lastSparsityStr = format(d.get())
						End If
						log.info("Threshold at iter {}, epoch {}: {}, SPARSE updates, last threshold: {}, last sparsity ratio: {}", iter, epoch, Thread.CurrentThread.getId(), lastThresholdStr, lastSparsityStr)
					End If
				End If
			End If
		End Sub

		Protected Friend Overridable Sub residualDebugOutputIfRequired(ByVal residual As INDArray)
			If Not encodingDebugMode Then
				Return
			End If

			Dim currThreshold As Double = currentThreshold.get().get()
			Dim currThresholdStr As String = format(currThreshold)


			Dim absResidual As INDArray = Transforms.abs(residual, True)

			Dim dAmean As Double = absResidual.meanNumber().doubleValue()
			Dim dAMax As Double = absResidual.maxNumber().doubleValue()
			Dim dPc50 As Double = absResidual.percentileNumber(50).doubleValue()
			Dim dPc95 As Double = absResidual.percentileNumber(95).doubleValue()
			Dim dPc99 As Double = absResidual.percentileNumber(99).doubleValue()
			Dim dPc999 As Double = absResidual.percentileNumber(99.9).doubleValue()
			Dim dPc9999 As Double = absResidual.percentileNumber(99.99).doubleValue()

			Dim amean As String = format(dAmean).Replace("E"c, "e"c)
			Dim aMax As String = format(dAMax).Replace("E"c, "e"c)
			Dim pc50 As String = format(dPc50).Replace("E"c, "e"c)
			Dim pc95 As String = format(dPc95).Replace("E"c, "e"c)
			Dim pc99 As String = format(dPc99).Replace("E"c, "e"c)
			Dim pc999 As String = format(dPc999).Replace("E"c, "e"c)
			Dim pc9999 As String = format(dPc9999).Replace("E"c, "e"c)

			Dim ameanThr As String = format(dAmean / currThreshold).Replace("E"c, "e"c)
			Dim aMaxThr As String = format(dAMax / currThreshold).Replace("E"c, "e"c)
			Dim pc50Thr As String = format(dPc50 / currThreshold).Replace("E"c, "e"c)
			Dim pc95Thr As String = format(dPc95 / currThreshold).Replace("E"c, "e"c)
			Dim pc99Thr As String = format(dPc99 / currThreshold).Replace("E"c, "e"c)
			Dim pc999Thr As String = format(dPc999 / currThreshold).Replace("E"c, "e"c)
			Dim pc9999Thr As String = format(dPc9999 / currThreshold).Replace("E"c, "e"c)

			Dim length As Long = absResidual.length()
			Dim countAbsGTEThreshold As Long = absResidual.gte(currThreshold).sumNumber().longValue()
			Dim sparsity As Double = countAbsGTEThreshold / CDbl(length)
			Dim sparsityStr As String = format(sparsity)

			log.info("Encoding debug info, residual vector: length: {}, threshold: {}, count > thr: {}, sparsity: {}, amean: {} ({}x); amax: {} ({}x); 50%: {} ({}x); 95%: {} ({}x}; 99%: {} ({}x);  99.9%: {} ({}x); 99.99%: {} ({}x)", length, currThresholdStr, countAbsGTEThreshold, sparsityStr, amean, ameanThr, aMax, aMaxThr, pc50, pc50Thr, pc95, pc95Thr, pc99, pc99Thr, pc999, pc999Thr, pc9999, pc9999Thr)
		End Sub

		Protected Friend Shared formatter As New ThreadLocal(Of DecimalFormat)()
		Protected Friend Shared formatter2 As New ThreadLocal(Of DecimalFormat)()

		Protected Friend Shared Function format(ByVal d As Double) As String
			If d = 0 Then
				Return "0.0"
			End If
			If (d <= -0.1 AndAlso d > -100) OrElse (d >= 0.1 AndAlso d < 100) Then
				If formatter2.get() Is Nothing Then
					formatter2.set(New DecimalFormat("0.###"))
				End If
				Return formatter2.get().format(d)
			End If

			If formatter.get() Is Nothing Then
				formatter.set(New DecimalFormat("0.###E0"))
			End If
			Dim df As DecimalFormat = formatter.get()
			Return df.format(d).replace("E"c,"e"c)
		End Function

		''' <summary>
		''' This should ONLY be called once all training threads have completed
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AverageThresholdAlgorithm As ThresholdAlgorithm
			Get
				Dim c As IDictionary(Of Long, ThresholdAlgorithm).ValueCollection = Me.allThreadThresholdAlgorithms.Values
				If c.isEmpty() Then
					Return Nothing
				End If
				If c.size() = 1 Then
					Return c.GetEnumerator().next()
				End If
				Dim iter As IEnumerator(Of ThresholdAlgorithm) = c.GetEnumerator()
				Dim r As ThresholdAlgorithmReducer = Nothing
				Do While iter.MoveNext()
					Dim ta As ThresholdAlgorithm = iter.Current
					If r Is Nothing Then
						r = ta.newReducer()
					End If
					r.add(ta)
				Loop
				Dim ta As ThresholdAlgorithm = r.FinalResult
    
				'Remove the old instances in preparation for use in next epoch, if required
				thresholdAlgorithm = New ThreadLocal(Of ThresholdAlgorithm)()
				allThreadThresholdAlgorithms.Clear()
    
				Return ta
			End Get
		End Property
	End Class

End Namespace