Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Chain = org.nd4j.parameterserver.distributed.messages.Chain
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports DotAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.DotAggregation
Imports CbowRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.CbowRequestMessage

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

Namespace org.nd4j.parameterserver.distributed.training.chains

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j public class CbowChain implements org.nd4j.parameterserver.distributed.messages.Chain
	Public Class CbowChain
		Implements Chain

		Protected Friend originatorId As Long
		Protected Friend taskId As Long
		Protected Friend frameId As Long

		Protected Friend cbowRequest As CbowRequestMessage
		Protected Friend dotAggregation As DotAggregation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CbowChain(@NonNull CbowRequestMessage message)
		Public Sub New(ByVal message As CbowRequestMessage)
			Me.New(message.getTaskId(), message)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CbowChain(long taskId, @NonNull CbowRequestMessage message)
		Public Sub New(ByVal taskId As Long, ByVal message As CbowRequestMessage)
			Me.taskId = taskId
			Me.originatorId = message.getOriginatorId()
			Me.frameId = message.getFrameId()
		End Sub

		Public Overridable Sub addElement(ByVal message As VoidMessage)
			If TypeOf message Is CbowRequestMessage Then

				cbowRequest = DirectCast(message, CbowRequestMessage)
			ElseIf TypeOf message Is DotAggregation Then

				dotAggregation = DirectCast(message, DotAggregation)
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Throw New ND4JIllegalStateException("Unknown message passed: " & message.GetType().FullName)
			End If
		End Sub
	End Class

End Namespace