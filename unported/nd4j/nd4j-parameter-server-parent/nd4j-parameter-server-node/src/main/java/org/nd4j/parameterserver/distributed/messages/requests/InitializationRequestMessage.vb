Imports System
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.messages.RequestMessage
Imports InitializationAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.InitializationAggregation
Imports DistributedInitializationMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedInitializationMessage

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
'ORIGINAL LINE: @Slf4j @Builder @Deprecated public class InitializationRequestMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.RequestMessage
	<Obsolete, Serializable>
	Public Class InitializationRequestMessage
		Inherits BaseVoidMessage
		Implements RequestMessage

		Protected Friend vectorLength As Integer
		Protected Friend numWords As Integer
		Protected Friend seed As Long
		Protected Friend useHs As Boolean
		Protected Friend useNeg As Boolean
		Protected Friend columnsPerShard As Integer

		Protected Friend Sub New()
			MyBase.New(4)
			taskId = -119L
		End Sub

		Public Sub New(ByVal vectorLength As Integer, ByVal numWords As Integer, ByVal seed As Long, ByVal useHs As Boolean, ByVal useNeg As Boolean, ByVal columnsPerShard As Integer)
			Me.New()
			Me.vectorLength = vectorLength
			Me.numWords = numWords
			Me.seed = seed
			Me.useHs = useHs
			Me.useNeg = useNeg
			Me.columnsPerShard = columnsPerShard
		End Sub


		Public Overrides Sub processMessage()
			Dim [dim] As New DistributedInitializationMessage(vectorLength, numWords, seed, useHs, useNeg, columnsPerShard)

			Dim aggregation As New InitializationAggregation(CShort(Math.Truncate(voidConfiguration.getNumberOfShards())), transport.ShardIndex)
			aggregation.OriginatorId = Me.originatorId

			clipboard.pin(aggregation)

			[dim].OriginatorId = Me.originatorId
			[dim].extractContext(Me)
			[dim].processMessage()

			If voidConfiguration.getNumberOfShards() > 1 Then
				transport.sendMessageToAllShards([dim])
			End If
		End Sub

		Public Overrides ReadOnly Property BlockingMessage As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

End Namespace