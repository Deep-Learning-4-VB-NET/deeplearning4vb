Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Chain = org.nd4j.parameterserver.distributed.messages.Chain
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports DotAggregation = org.nd4j.parameterserver.distributed.messages.aggregations.DotAggregation
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage

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
'ORIGINAL LINE: @Data @Slf4j public class SkipGramChain implements org.nd4j.parameterserver.distributed.messages.Chain
	Public Class SkipGramChain
		Implements Chain

		Protected Friend originatorId As Long
'JAVA TO VB CONVERTER NOTE: The field taskId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend taskId_Conflict As Long
		Protected Friend frameId As Long

		Protected Friend requestMessage As SkipGramRequestMessage
		Protected Friend dotAggregation As DotAggregation

		Public Sub New(ByVal originatorId As Long, ByVal taskId As Long, ByVal frameId As Long)
			Me.taskId_Conflict = taskId
			Me.frameId = frameId
			Me.originatorId = originatorId
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SkipGramChain(@NonNull SkipGramRequestMessage message)
		Public Sub New(ByVal message As SkipGramRequestMessage)
			Me.New(message.getTaskId(), message)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SkipGramChain(long taskId, @NonNull SkipGramRequestMessage message)
		Public Sub New(ByVal taskId As Long, ByVal message As SkipGramRequestMessage)
			Me.New(message.getOriginatorId(), taskId, message.getFrameId())
			addElement(message)
		End Sub

		Public Overridable ReadOnly Property TaskId As Long Implements Chain.getTaskId
			Get
				Return taskId_Conflict
			End Get
		End Property

		Public Overridable Sub addElement(ByVal message As VoidMessage)
			If TypeOf message Is SkipGramRequestMessage Then
				requestMessage = DirectCast(message, SkipGramRequestMessage)

			ElseIf TypeOf message Is DotAggregation Then
				dotAggregation = DirectCast(message, DotAggregation)

			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Throw New ND4JIllegalStateException("Unknown message received: [" & message.GetType().FullName & "]")
			End If
		End Sub
	End Class

End Namespace