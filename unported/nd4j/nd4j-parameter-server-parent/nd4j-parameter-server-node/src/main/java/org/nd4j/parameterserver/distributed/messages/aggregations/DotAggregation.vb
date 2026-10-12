Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
'ORIGINAL LINE: @Slf4j @Deprecated public class DotAggregation extends BaseAggregation
	<Obsolete, Serializable>
	Public Class DotAggregation
		Inherits BaseAggregation

		Protected Friend Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal taskId As Long, ByVal aggregationWidth As Short, ByVal shardIndex As Short, ByVal scalar As INDArray)
			MyBase.New(taskId, aggregationWidth, shardIndex)

			Me.payload = scalar
			addToChunks(payload)
		End Sub

		Public Overrides ReadOnly Property AccumulatedResult As INDArray
			Get
				Dim stack As INDArray = MyBase.AccumulatedResult
    
				If aggregationWidth = 1 Then
					Return stack
				End If
    
				If stack.RowVector Then
					Return Nd4j.scalar(stack.sumNumber().doubleValue())
				Else
					Return stack.sum(1)
				End If
			End Get
		End Property

		''' <summary>
		''' This method will be started in context of executor, either Shard, Client or Backup node
		''' </summary>
		Public Overrides Sub processMessage()
			' since our computations are symmetric - we aggregate dot everywhere
			If chunks Is Nothing Then
				chunks = New SortedDictionary(Of Short, INDArray)()
				chunksCounter = New AtomicInteger(1)
				addToChunks(payload)
			End If

			clipboard.pin(Me)

			'log.info("sI_{} dot aggregation received", transport.getShardIndex());

			If clipboard.isReady(Me.OriginatorId, Me.TaskId) Then
				trainer.aggregationFinished(clipboard.unpin(Me.OriginatorId, Me.taskId))
			End If
		End Sub
	End Class

End Namespace