Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports VoidAggregation = org.nd4j.parameterserver.distributed.messages.VoidAggregation

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

Namespace org.nd4j.parameterserver.distributed.logic.completion


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class Clipboard
	<Obsolete>
	Public Class Clipboard
		Protected Friend clipboard As IDictionary(Of RequestDescriptor, VoidAggregation) = New ConcurrentDictionary(Of RequestDescriptor, VoidAggregation)()

		Protected Friend completedQueue As LinkedList(Of VoidAggregation) = New ConcurrentLinkedQueue(Of VoidAggregation)()

		Protected Friend trackingCounter As New AtomicInteger(0)
		Protected Friend completedCounter As New AtomicInteger(0)

		''' <summary>
		''' This method places incoming VoidAggregation into clipboard, for further tracking
		''' </summary>
		''' <param name="aggregation"> </param>
		''' <returns> TRUE, if given VoidAggregation was the last chunk, FALSE otherwise </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public boolean pin(@NonNull VoidAggregation aggregation)
		Public Overridable Function pin(ByVal aggregation As VoidAggregation) As Boolean
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(aggregation.getOriginatorId(), aggregation.getTaskId())
			Dim existing As VoidAggregation = clipboard(descriptor)
			If existing Is Nothing Then
				existing = aggregation
				trackingCounter.incrementAndGet()
				clipboard(descriptor) = aggregation
			End If

			existing.accumulateAggregation(aggregation)

			'if (counter.incrementAndGet() % 10000 == 0)
			'    log.info("Clipboard stats: Totals: {}; Completed: {};", clipboard.size(), completedQueue.size());

			Dim missing As Integer = existing.MissingChunks
			If missing = 0 Then
				'  completedQueue.add(existing);
				completedCounter.incrementAndGet()
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' This method removes given VoidAggregation from clipboard, and returns it
		''' </summary>
		''' <param name="aggregation"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.parameterserver.distributed.messages.VoidAggregation unpin(@NonNull VoidAggregation aggregation)
		Public Overridable Function unpin(ByVal aggregation As VoidAggregation) As VoidAggregation
			Return unpin(aggregation.getOriginatorId(), aggregation.getTaskId())
		End Function

		''' <summary>
		''' This method removes given VoidAggregation from clipboard, and returns it
		''' </summary>
		''' <param name="taskId"> </param>
		Public Overridable Function unpin(ByVal originatorId As Long, ByVal taskId As Long) As VoidAggregation
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(originatorId, taskId)
			Dim aggregation As VoidAggregation
			aggregation = clipboard(descriptor)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if ((aggregation = clipboard.get(descriptor)) != null)
			If aggregation IsNot Nothing Then
				clipboard.Remove(descriptor)
				trackingCounter.decrementAndGet()

				' FIXME: we don't want this here
				'            completedQueue.clear();

				Return aggregation
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' This method checks, if clipboard has ready aggregations available
		''' </summary>
		''' <returns> TRUE, if there's at least 1 candidate ready, FALSE otherwise </returns>
		Public Overridable Function hasCandidates() As Boolean
			Return completedCounter.get() > 0
		End Function

		''' <summary>
		''' This method returns one of available aggregations, if there's at least 1 ready.
		''' 
		''' @return
		''' </summary>
		Public Overridable Function nextCandidate() As VoidAggregation
			Dim result As VoidAggregation = completedQueue.RemoveFirst()

			' removing aggregation from tracking table
			If result IsNot Nothing Then
				completedCounter.decrementAndGet()
				unpin(result.OriginatorId, result.TaskId)
			End If

			Return result
		End Function

		Public Overridable Function isReady(ByVal aggregation As VoidAggregation) As Boolean
			Return isReady(aggregation.OriginatorId, aggregation.TaskId)
		End Function

		Public Overridable Function isReady(ByVal originatorId As Long, ByVal taskId As Long) As Boolean
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(originatorId, taskId)
			Dim aggregation As VoidAggregation = clipboard(descriptor)
			If aggregation Is Nothing Then
				Return False
			End If

			Return aggregation.MissingChunks = 0
		End Function

		Public Overridable Function isTracking(ByVal originatorId As Long, ByVal taskId As Long) As Boolean
			Return clipboard.ContainsKey(RequestDescriptor.createDescriptor(originatorId, taskId))
		End Function

		Public Overridable ReadOnly Property NumberOfPinnedStacks As Integer
			Get
				Return trackingCounter.get()
			End Get
		End Property

		Public Overridable ReadOnly Property NumberOfCompleteStacks As Integer
			Get
				Return completedCounter.get()
			End Get
		End Property

		Public Overridable Function getStackFromClipboard(ByVal originatorId As Long, ByVal taskId As Long) As VoidAggregation
			Return clipboard(RequestDescriptor.createDescriptor(originatorId, taskId))
		End Function
	End Class

End Namespace