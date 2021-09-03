Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports var = lombok.var
Imports Preconditions = org.nd4j.common.base.Preconditions
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
'ORIGINAL LINE: @Slf4j public class IndexedTail
	Public Class IndexedTail
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected java.util.concurrent.ConcurrentHashMap<Long, java.util.concurrent.atomic.AtomicLong> positions = new java.util.concurrent.ConcurrentHashMap<>();
		Protected Friend positions As New ConcurrentDictionary(Of Long, AtomicLong)()

		' here we store individual updates
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected java.util.Map<Long, org.nd4j.linalg.api.ndarray.INDArray> updates = new java.util.concurrent.ConcurrentHashMap<>();
		Protected Friend updates As IDictionary(Of Long, INDArray) = New ConcurrentDictionary(Of Long, INDArray)()

		' simple counter for new updates
		Protected Friend updatesCounter As New AtomicLong(0)

		' index of last deleted element. used for maintenance, and removal of useless updates
		Protected Friend lastDeletedIndex As New AtomicLong(-1)

		' this value is used as max number of possible consumers.
		Protected Friend ReadOnly expectedConsumers As Integer

		' flag useful for debugging only
'JAVA TO VB CONVERTER NOTE: The field dead was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend dead_Conflict As New AtomicBoolean(False)

		Protected Friend lock As New ReentrantReadWriteLock()

		' fields required for collapser
		Protected Friend ReadOnly allowCollapse As Boolean
		Protected Friend ReadOnly shape() As Long
		Protected Friend ReadOnly collapseThreshold As Integer = 32
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.common.primitives.AtomicBoolean collapsedMode = new org.nd4j.common.primitives.AtomicBoolean(false);
		Protected Friend collapsedMode As New AtomicBoolean(False)
		Protected Friend collapsedIndex As New AtomicLong(-1)

		Public Sub New(ByVal expectedConsumers As Integer)
			Me.New(expectedConsumers, False, Nothing)
		End Sub

		Public Sub New(ByVal expectedConsumers As Integer, ByVal allowCollapse As Boolean, ByVal shape() As Long)
			Me.expectedConsumers = expectedConsumers
			Me.allowCollapse = allowCollapse

			If allowCollapse Then
				Preconditions.checkArgument(shape IsNot Nothing, "shape can't be null if collapse is allowed")
			End If

			Me.shape = shape
		End Sub

		''' <summary>
		''' This mehtod adds update, with optional collapse </summary>
		''' <param name="update"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void put(@NonNull INDArray update)
		Public Overridable Sub put(ByVal update As INDArray)
			Try
				lock.writeLock().lock()

				'if we're already in collapsed mode - we just insta-decompress
				If collapsedMode.get() Then
					Dim lastUpdateIndex As val = collapsedIndex.get()
					Dim lastUpdate As val = updates(lastUpdateIndex)

					Preconditions.checkArgument(Not lastUpdate.isCompressed(), "lastUpdate should NOT be compressed during collapse mode")

					smartDecompress(update, lastUpdate)

					' collapser only can work if all consumers are already introduced
				ElseIf allowCollapse AndAlso positions.Count >= expectedConsumers Then
					' getting last added update
					Dim lastUpdateIndex As val = updatesCounter.get()

					' looking for max common non-applied update
					Dim maxIdx As Long = firstNotAppliedIndexEverywhere()
					Dim array As val = Nd4j.create(shape)

					Dim delta As val = lastUpdateIndex - maxIdx
					If delta >= collapseThreshold Then
						log.trace("Max delta to collapse: {}; Range: <{}...{}>", delta, maxIdx, lastUpdateIndex)
						For e As Long = maxIdx To lastUpdateIndex - 1
							Dim u As val = updates(e)
							If u Is Nothing Then
								log.error("Failed on index {}", e)
							End If
							   ' continue;

							smartDecompress(u, array)

							' removing updates array
							updates.Remove(e)
						Next e

						' decode latest update
						smartDecompress(update, array)

						' putting collapsed array back at last index
						updates(lastUpdateIndex) = array
						collapsedIndex.set(lastUpdateIndex)

						' shift counter by 1
						updatesCounter.getAndIncrement()

						' we're saying that right now all updates within some range are collapsed into 1 update
						collapsedMode.set(True)
					Else
						updates(updatesCounter.getAndIncrement()) = update
					End If
				Else
					updates(updatesCounter.getAndIncrement()) = update
				End If
			Finally
				lock.writeLock().unlock()
			End Try
		End Sub

		Public Overridable Function firstNotAppliedIndexEverywhere() As Long
			Dim maxIdx As Long = -1

			' if there's no updates posted yet - just return negative value
			If updatesCounter.get() = 0 Then
				Return maxIdx
			End If

			For Each v As val In positions.Values
				If v.get() > maxIdx Then
					maxIdx = v.get()
				End If
			Next v

			Return maxIdx + 1
		End Function

		Public Overridable Function maxAppliedIndexEverywhere() As Long
			Dim maxIdx As Long = Long.MaxValue
			For Each v As val In positions.Values
				If v.get() < maxIdx Then
					maxIdx = v.get()
				End If
			Next v

			Return maxIdx
		End Function

		Public Overridable Function hasAnything() As Boolean
			Return hasAnything(Thread.CurrentThread.getId())
		End Function

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public Overridable Function hasAnything(ByVal threadId As Long) As Boolean
			Dim threadPosition As var = getLocalPosition(threadId)

			Dim r As val = threadPosition < updatesCounter.get()
			log.trace("hasAnything({}): {}; position: {}; updates: {}", threadId, r, threadPosition, updatesCounter.get())

			Return r
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public boolean drainTo(@NonNull INDArray array)
		Public Overridable Function drainTo(ByVal array As INDArray) As Boolean
			Return drainTo(Thread.CurrentThread.getId(), array)
		End Function

		Protected Friend Overridable ReadOnly Property GlobalPosition As Long
			Get
				Try
					lock.readLock().lock()
    
					Return updatesCounter.get()
				Finally
					lock.readLock().unlock()
				End Try
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property LocalPosition As Long
			Get
				Return getLocalPosition(Thread.CurrentThread.getId())
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property Delta As Long
			Get
				Return getDelta(Thread.CurrentThread.getId())
			End Get
		End Property

		Public Overridable Function getDelta(ByVal threadId As Long) As Long
			Return GlobalPosition - getLocalPosition(threadId)
		End Function

		Protected Friend Overridable Function getLocalPosition(ByVal threadId As Long) As Long
			Dim threadPosition As var = positions(threadId)

			' will be instantiated on first call from any given thread
			If threadPosition Is Nothing Then
				threadPosition = New AtomicLong(-1)
				positions(threadId) = threadPosition
			End If

			Return If(threadPosition.get() < 0, 0, threadPosition.get())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public boolean drainTo(long threadId, @NonNull INDArray array)
		Public Overridable Function drainTo(ByVal threadId As Long, ByVal array As INDArray) As Boolean
			Dim threadPosition As var = positions(threadId)

			' will be instantiated on first call from any given thread
			If threadPosition Is Nothing Then
				threadPosition = New AtomicLong(-1)
				positions(threadId) = threadPosition
			End If

			Dim globalPos As Long = 0
			Dim localPos As Long = 0
			Dim delta As Long = 0
			Dim sessionUpdates As val = New List(Of INDArray)()

			Try
				lock.readLock().lock()

				' since drain fetches all existing updates for a given consumer
				collapsedMode.set(False)

				globalPos = updatesCounter.get()
				localPos = getLocalPosition(threadId)

				' we're finding out, how many arrays we should provide
				delta = getDelta(threadId)

				' within read lock we only move references and tag updates as applied
				Dim e As Long = localPos
				Do While e < localPos + delta
					Dim update As val = updates(e)

					If allowCollapse AndAlso update Is Nothing Then
						e += 1
						Continue Do
					End If

					' FIXME: just continue here, probably it just means that collapser was working in this position
					If update Is Nothing Then
						log.trace("Global: [{}]; Local: [{}]", globalPos, localPos)
						Throw New Exception("Element [" & e & "] is absent")
					End If

					sessionUpdates.add(update)
					e += 1
				Loop

				' and shifting stuff by one
				threadPosition.set(globalPos)
			Finally
				lock.readLock().unlock()
			End Try


			' now we decompress all arrays within delta into provided array
			For Each u As val In sessionUpdates
				smartDecompress(u.unsafeDuplication(True), array)
			Next u



			' TODO: this call should be either synchronized, or called from outside
			maintenance()

			Return delta > 0
		End Function

		''' <summary>
		''' This method does maintenance of updates within
		''' </summary>
		Public Overridable Sub maintenance()
			SyncLock Me
				' first of all we're checking, if all consumers were already registered. if not - just no-op.
				If positions.Count < expectedConsumers Then
					log.trace("Skipping maintanance due to not all expected consumers shown up: [{}] vs [{}]", positions.Count, expectedConsumers)
					Return
				End If
        
				' now we should get minimal id of consumed update
				Dim minIdx As val = maxAppliedIndexEverywhere()
				Dim allPositions As val = New Long(positions.Count - 1){}
				Dim cnt As Integer = 0
				For Each p As val In positions.Values
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: allPositions[cnt++] = p.get();
					allPositions(cnt) = p.get()
						cnt += 1
				Next p
        
				log.trace("Min idx: {}; last deleted index: {}; stored updates: {}; positions: {}", minIdx, lastDeletedIndex.get(), updates.Count, allPositions)
        
				' now we're checking, if there are undeleted updates between
				If minIdx > lastDeletedIndex.get() Then
					' delete everything between them
					For e As Long = lastDeletedIndex.get() To minIdx - 1
						updates.Remove(e)
					Next e
        
					' now, making sure we won't try to delete stuff twice
					lastDeletedIndex.set(minIdx)
					'System.gc();
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' This method returns actual number of updates stored within tail
		''' @return
		''' </summary>
		Public Overridable Function updatesSize() As Integer
			Return updates.Count
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected org.nd4j.linalg.api.ndarray.INDArray smartDecompress(org.nd4j.linalg.api.ndarray.INDArray encoded, @NonNull INDArray target)
		Protected Friend Overridable Function smartDecompress(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray
			Dim result As INDArray = target

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

		Public Overridable ReadOnly Property Dead As Boolean
			Get
				Return dead_Conflict.get()
			End Get
		End Property

		Public Overridable Sub notifyDead()
			dead_Conflict.set(True)
		End Sub

		Public Overridable Sub purge()
			positions.Clear()
			updates.Clear()
			updatesCounter.set(0)
			lastDeletedIndex.set(-1)
			collapsedMode.set(False)
			collapsedIndex.set(-1)
		End Sub
	End Class

End Namespace