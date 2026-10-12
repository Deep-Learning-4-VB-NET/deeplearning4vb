Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports org.nd4j.parameterserver.distributed.messages
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport

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

Namespace org.nd4j.parameterserver.distributed.logic.routing


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class InterleavedRouter extends BaseRouter
	<Obsolete>
	Public Class InterleavedRouter
		Inherits BaseRouter

		Protected Friend targetIndex As Short = (Short) -1
		Protected Friend counter As New AtomicLong(0)

		Public Sub New()

		End Sub

		Public Sub New(ByVal defaultIndex As Integer)
			Me.New()
			Me.targetIndex = CShort(defaultIndex)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Transport transport)
		Public Overridable Overloads Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport)
			MyBase.init(voidConfiguration, transport)

			' by default messages are being routed to any random shard
			If targetIndex < 0 Then
				targetIndex = CShort(Math.Truncate(RandomUtils.nextInt(0, voidConfiguration.getNumberOfShards())))
			End If
		End Sub

		Public Overrides Function assignTarget(ByVal message As TrainingMessage) As Integer
			Originator = message
			If TypeOf message Is SkipGramRequestMessage Then
				Dim sgrm As SkipGramRequestMessage = DirectCast(message, SkipGramRequestMessage)

				Dim w1 As Integer = sgrm.getW1()
				If w1 >= voidConfiguration.getNumberOfShards() Then
					message.TargetId = CShort(Math.Truncate(w1 Mod voidConfiguration.getNumberOfShards()))
				Else
					message.TargetId = CShort(w1)
				End If
			Else
				message.TargetId = CShort(Math.Truncate(Math.Abs(counter.incrementAndGet() Mod voidConfiguration.getNumberOfShards())))
			End If

			Return message.TargetId
		End Function

		Public Overrides Function assignTarget(ByVal message As VoidMessage) As Integer
			Originator = message
			If TypeOf message Is Frame Then
				message.TargetId = CShort(Math.Truncate(Math.Abs(counter.incrementAndGet() Mod voidConfiguration.getNumberOfShards())))
			Else
				message.TargetId = targetIndex
			End If
			Return message.TargetId
		End Function
	End Class

End Namespace