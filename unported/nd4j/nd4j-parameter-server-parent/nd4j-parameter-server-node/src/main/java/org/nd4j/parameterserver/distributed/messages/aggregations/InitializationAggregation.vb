Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports InitializationCompleteMessage = org.nd4j.parameterserver.distributed.messages.complete.InitializationCompleteMessage

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
'ORIGINAL LINE: @Slf4j @Deprecated public class InitializationAggregation extends BaseAggregation
	<Obsolete, Serializable>
	Public Class InitializationAggregation
		Inherits BaseAggregation

		Protected Friend Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal aggregationWidth As Integer, ByVal shardIndex As Integer)
			Me.New(CShort(aggregationWidth), CShort(shardIndex))
		End Sub

		Public Sub New(ByVal aggregationWidth As Short, ByVal shardIndex As Short)
			MyBase.New(-119L, aggregationWidth, shardIndex)
			Me.payload = Nd4j.scalar(1.0)
		End Sub

		Public Overrides Sub processMessage()
			'log.info("sI_{} received init aggregation", transport.getShardIndex());
			If clipboard.isTracking(Me.originatorId, taskId) Then
				clipboard.pin(Me)

				If clipboard.isReady(Me.originatorId, taskId) Then
					Dim aggregation As InitializationAggregation = DirectCast(clipboard.unpin(Me.originatorId, taskId), InitializationAggregation)

					Dim icm As New InitializationCompleteMessage(taskId)
					icm.OriginatorId = aggregation.OriginatorId
					transport.sendMessage(icm)
				End If
			End If
		End Sub
	End Class

End Namespace