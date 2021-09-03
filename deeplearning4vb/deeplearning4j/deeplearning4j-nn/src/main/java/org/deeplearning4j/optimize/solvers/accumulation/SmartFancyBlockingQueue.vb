Imports System
Imports ReaderPreferenceReadWriteLock = EDU.oswego.cs.dl.util.concurrent.ReaderPreferenceReadWriteLock
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ThresholdCompression = org.nd4j.linalg.compression.ThresholdCompression
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean

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
'ORIGINAL LINE: @Slf4j public class SmartFancyBlockingQueue extends FancyBlockingQueue<org.nd4j.linalg.api.ndarray.INDArray>
	Public Class SmartFancyBlockingQueue
		Inherits FancyBlockingQueue(Of INDArray)

		Protected Friend ReadOnly smartLock As New ReaderPreferenceReadWriteLock()
		Protected Friend decompressionThreshold As Integer = 32
		Protected Friend collapsedMode As New AtomicBoolean(False)


		Protected Friend ReadOnly paramsShape() As Long
		Protected Friend ReadOnly paramsOrder As Char

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SmartFancyBlockingQueue(int decompressionThreshold, @NonNull INDArray paramsMatrix)
		Public Sub New(ByVal decompressionThreshold As Integer, ByVal paramsMatrix As INDArray)
			Me.New(decompressionThreshold, New LinkedBlockingQueue(Of INDArray)(1024), paramsMatrix)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SmartFancyBlockingQueue(int decompressionThreshold, java.util.concurrent.BlockingQueue<org.nd4j.linalg.api.ndarray.INDArray> queue, @NonNull INDArray paramsMatrix)
		Public Sub New(ByVal decompressionThreshold As Integer, ByVal queue As BlockingQueue(Of INDArray), ByVal paramsMatrix As INDArray)
			MyBase.New(queue)
			Me.decompressionThreshold = decompressionThreshold

			Me.paramsShape = paramsMatrix.shape()
			Me.paramsOrder = paramsMatrix.ordering()
		End Sub

		Protected Friend Overridable Function smartDecompress(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray
			Dim result As INDArray = If(target Is Nothing, Nd4j.create(paramsShape, paramsOrder), target)

			If encoded.Compressed OrElse encoded.data().dataType() = DataType.INT Then
				Dim encoding As Integer = encoded.data().getInt(3)
				If encoding = ThresholdCompression.FLEXIBLE_ENCODING Then
					Nd4j.Executioner.thresholdDecode(encoded, result)
				ElseIf encoding = ThresholdCompression.BITMAP_ENCODING Then
					Nd4j.Executioner.bitmapDecode(encoded, result)
				Else
					Throw New ND4JIllegalStateException("Unknown encoding mode: [" & encoding & "]")
				End If
			Else
				result.addi(encoded)
			End If

			Return result
		End Function
	'
	'    @Override
	'    public void registerConsumers(int consumers) {
	'        try {
	'            smartLock.writeLock().acquire();
	'
	'            super.registerConsumers(consumers);
	'        } catch (InterruptedException e) {
	'            smartLock.writeLock().release();
	'        }
	'    }
	'
		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Try
					' we use this lock to make
					smartLock.readLock().acquire()
    
					If currentConsumers.get() > 0 Then
						synchronize(currentConsumers.get())
					End If
    
					Return MyBase.Empty
				Catch e As InterruptedException
					Throw New Exception(e)
				Finally
					smartLock.readLock().release()
				End Try
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void put(org.nd4j.linalg.api.ndarray.INDArray array) throws InterruptedException
		Public Overrides Sub put(ByVal array As INDArray)
			Try
				smartLock.writeLock().acquire()

				If backingQueue.size() > decompressionThreshold OrElse collapsedMode.get() Then
					log.trace("Collapsing updates...")

					' if we're already in collapsed mode - we'll just poll back our single collapsed array and update it
					Dim params As INDArray = smartDecompress(array,If(collapsedMode.get() AndAlso backingQueue.size() = 1, backingQueue.poll(), Nothing))
					Do While Not backingQueue.isEmpty()
						Dim arr As val = backingQueue.poll()
						smartDecompress(arr, params)
					Loop

					numElementsDrained.set(0)
					numElementsReady.set(1)
					collapsedMode.set(True)

					' now just put single array back
					MyBase.put(params)
				Else
					MyBase.put(array)
				End If
			Finally
				smartLock.writeLock().release()
			End Try
		End Sub

		Public Overrides Function poll() As INDArray
			Try
				' we use this lock to make
				smartLock.readLock().acquire()

				' from now on this SFBQ instance won't add up to single compressed array
				collapsedMode.set(False)

				Return MyBase.poll()
			Catch e As InterruptedException
			  Throw New Exception(e)
			Finally
				smartLock.readLock().release()
			End Try
		End Function
	End Class

End Namespace