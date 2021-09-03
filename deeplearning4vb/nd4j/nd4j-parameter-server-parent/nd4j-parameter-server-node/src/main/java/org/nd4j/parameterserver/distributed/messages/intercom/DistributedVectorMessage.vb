Imports System
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports DistributedMessage = org.nd4j.parameterserver.distributed.messages.DistributedMessage
Imports VectorAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.VectorAggregation

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
'ORIGINAL LINE: @Data @Slf4j @Deprecated public class DistributedVectorMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.DistributedMessage
	<Obsolete, Serializable>
	Public Class DistributedVectorMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		Protected Friend rowIndex As Integer
		Protected Friend key As Integer

		Public Sub New()
			messageType_Conflict = 20
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedVectorMessage(@NonNull Integer key, int rowIndex)
		Public Sub New(ByVal key As Integer, ByVal rowIndex As Integer)
			Me.New()
			Me.rowIndex = rowIndex
			Me.key = key
		End Sub

		''' <summary>
		''' This method will be started in context of executor, either Shard, Client or Backup node
		''' </summary>
		Public Overrides Sub processMessage()
			Dim aggregation As New VectorAggregation(rowIndex, CShort(Math.Truncate(voidConfiguration.getNumberOfShards())), shardIndex, storage.getArray(key).getRow(rowIndex).dup())
			aggregation.OriginatorId = Me.OriginatorId
			transport.sendMessage(aggregation)
		End Sub
	End Class

End Namespace