Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
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

Namespace org.nd4j.parameterserver.distributed.messages.aggregations


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public abstract class BaseAggregation extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.VoidAggregation, java.io.Serializable
	<Obsolete, Serializable>
	Public MustInherit Class BaseAggregation
		Inherits BaseVoidMessage
		Implements VoidAggregation

		Public MustOverride ReadOnly Property Payload As INDArray Implements VoidAggregation.getPayload
		Public MustOverride Sub accumulateAggregation(ByVal aggregation As VoidAggregation)
		Public MustOverride ReadOnly Property AggregationType As Short Implements VoidAggregation.getAggregationType
		Public MustOverride ReadOnly Property ShardIndex As Short Implements VoidAggregation.getShardIndex
		Public Overrides MustOverride ReadOnly Property RetransmitCount As Integer
		Public Overrides MustOverride Sub processMessage()
		Public Overrides MustOverride Function fromBytes(ByVal array() As SByte) As T
		Public Overrides MustOverride Property OriginatorId As Long
		Public Overrides MustOverride ReadOnly Property TaskId As Long
		Public Overrides MustOverride WriteOnly Property TargetId As Short
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected short aggregationType = -1;
		Protected Friend aggregationType As Short = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected short aggregationWidth;
		Protected Friend aggregationWidth As Short
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int numberOfElements;
		Protected Friend numberOfElements As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected short shardIndex;
'JAVA TO VB CONVERTER NOTE: The field shardIndex was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shadows shardIndex_Conflict As Short


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.nd4j.linalg.api.ndarray.INDArray payload;
		Protected Friend payload As INDArray

		' transient part
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient java.util.concurrent.atomic.AtomicInteger chunksCounter;
		<NonSerialized>
		Protected Friend chunksCounter As AtomicInteger
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient Map<Short, org.nd4j.linalg.api.ndarray.INDArray> chunks;
		<NonSerialized>
		Protected Friend chunks As IDictionary(Of Short, INDArray)

		Protected Friend Sub New()
			chunksCounter = New AtomicInteger(1)
			chunks = New ConcurrentDictionary(Of Short, INDArray)()
		End Sub

		Protected Friend Sub New(ByVal taskId As Long, ByVal aggregationWidth As Short, ByVal shardIndex As Short)
			Me.New()
			Me.aggregationWidth = aggregationWidth
			Me.taskId = taskId
			Me.shardIndex_Conflict = shardIndex
		End Sub

		Public Overridable WriteOnly Property ShardIndex As Short
			Set(ByVal shardIndex As Short)
				If shardIndex = Me.shardIndex_Conflict Then
					Return
				End If
    
				chunks.Remove(Me.shardIndex_Conflict)
				chunks(shardIndex) = payload
    
				Me.shardIndex_Conflict = shardIndex
			End Set
		End Property

		Protected Friend Overridable Sub addToChunks(ByVal array As INDArray)
			chunks(Me.shardIndex_Conflict) = array
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void accumulateAggregation(@NonNull VoidAggregation aggregation)
		Public Overridable Sub accumulateAggregation(ByVal aggregation As VoidAggregation) Implements VoidAggregation.accumulateAggregation
			If aggregation.getAggregationType() <> AggregationType Then
				Throw New ND4JIllegalStateException("Trying to aggregate different aggregations!")
			End If

			' no need to do anything in this case
			If Me.ShardIndex = aggregation.getShardIndex() Then
				Return
			End If

			If chunks(aggregation.getShardIndex()) Is Nothing Then
				chunksCounter.incrementAndGet()
			End If

			chunks(aggregation.getShardIndex()) = aggregation.getPayload()
		End Sub

		Public Overridable ReadOnly Property AccumulatedResult As INDArray Implements VoidAggregation.getAccumulatedResult
			Get
    
				If aggregationWidth = 1 Then
					Return chunks(CShort(0))
				Else
					Return Nd4j.hstack(chunks.Values)
				End If
			End Get
		End Property

		Public Overridable ReadOnly Property MissingChunks As Integer Implements VoidAggregation.getMissingChunks
			Get
				Return aggregationWidth - chunksCounter.get()
			End Get
		End Property

		Public Overrides ReadOnly Property MessageType As Integer
			Get
				' joint aggregation messageType for all aggregations
				Return 21
			End Get
		End Property

		Public Overrides Function asBytes() As SByte()
			Return SerializationUtils.toByteArray(Me)
		End Function

		Public Overrides Function asUnsafeBuffer() As UnsafeBuffer
			Return New UnsafeBuffer(asBytes())
		End Function

		Public Overrides ReadOnly Property TargetId As Short
			Get
				Return (Short) -1
			End Get
		End Property
	End Class

End Namespace