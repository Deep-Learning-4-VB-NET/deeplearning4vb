Imports System
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports WordVectorStorage = org.nd4j.parameterserver.distributed.logic.storage.WordVectorStorage
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.messages.RequestMessage
Imports VectorAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.VectorAggregation
Imports DistributedVectorMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedVectorMessage

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

Namespace org.nd4j.parameterserver.distributed.messages.requests

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j @Deprecated public class VectorRequestMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.RequestMessage
	<Obsolete, Serializable>
	Public Class VectorRequestMessage
		Inherits BaseVoidMessage
		Implements RequestMessage

		Protected Friend key As Integer?
		Protected Friend rowIndex As Integer

		Protected Friend Sub New()
			MyBase.New(7)
		End Sub

		Public Sub New(ByVal rowIndex As Integer)
			Me.New(WordVectorStorage.SYN_0, rowIndex)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VectorRequestMessage(@NonNull Integer key, int rowIndex)
		Public Sub New(ByVal key As Integer, ByVal rowIndex As Integer)
			Me.New()
			Me.rowIndex = rowIndex

			' FIXME: this is temporary, should be changed
			Me.taskId = rowIndex
			Me.key = key
		End Sub

		''' <summary>
		''' This message is possible to get only as Shard
		''' </summary>
		Public Overrides Sub processMessage()
			Dim aggregation As New VectorAggregation(rowIndex, CShort(Math.Truncate(voidConfiguration.getNumberOfShards())), getShardIndex(), storage.getArray(key).getRow(rowIndex).dup())
			aggregation.OriginatorId = Me.OriginatorId

			clipboard.pin(aggregation)

			Dim dvm As New DistributedVectorMessage(key, rowIndex)
			dvm.OriginatorId = Me.originatorId

			If voidConfiguration.getNumberOfShards() > 1 Then
				transport.sendMessageToAllShards(dvm)
			Else
				aggregation.extractContext(Me)
				aggregation.processMessage()
			End If
		End Sub

		Public Overrides ReadOnly Property BlockingMessage As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

End Namespace