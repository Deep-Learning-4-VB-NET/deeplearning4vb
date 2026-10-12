Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports VoidAggregation = org.nd4j.parameterserver.distributed.messages.VoidAggregation
Imports VectorCompleteMessage = org.nd4j.parameterserver.distributed.messages.complete.VectorCompleteMessage

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
'ORIGINAL LINE: @Slf4j @Deprecated public class VectorAggregation extends BaseAggregation
	<Obsolete, Serializable>
	Public Class VectorAggregation
		Inherits BaseAggregation

		Protected Friend Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal taskId As Long, ByVal aggregationWidth As Short, ByVal shardIndex As Short, ByVal array As INDArray)
			MyBase.New(taskId, aggregationWidth, shardIndex)
			Me.payload = If(array.View, array.dup(array.ordering()), array)

			addToChunks(payload)
		End Sub

		''' <summary>
		''' Vector aggregations are saved only by Shards started aggregation process. All other Shards are ignoring this meesage
		''' </summary>
		Public Overrides Sub processMessage()
			If clipboard.isTracking(Me.originatorId, Me.TaskId) Then
				clipboard.pin(Me)

				If clipboard.isReady(Me.originatorId, taskId) Then
					Dim aggregation As VoidAggregation = clipboard.unpin(Me.originatorId, taskId)

					' FIXME: probably there's better solution, then "screw-and-forget" one
					If aggregation Is Nothing Then
						Return
					End If

					Dim msg As New VectorCompleteMessage(taskId, aggregation.AccumulatedResult)
					msg.OriginatorId = aggregation.OriginatorId
					transport.sendMessage(msg)
				End If
			End If
		End Sub
	End Class

End Namespace