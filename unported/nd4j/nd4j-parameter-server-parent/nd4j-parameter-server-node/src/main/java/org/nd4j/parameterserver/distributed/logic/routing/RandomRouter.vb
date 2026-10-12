Imports System
Imports NonNull = lombok.NonNull
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage
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

	<Obsolete>
	Public Class RandomRouter
		Inherits BaseRouter

		Protected Friend numShards As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Transport transport)
		Public Overridable Overloads Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport)
			MyBase.init(voidConfiguration, transport)

			Dim numShards As Integer = voidConfiguration.getNumberOfShards()
		End Sub

		Public Overrides Function assignTarget(ByVal message As TrainingMessage) As Integer
			Originator = message
			message.TargetId = NextShard
			Return message.TargetId
		End Function

		Public Overrides Function assignTarget(ByVal message As VoidMessage) As Integer
			Originator = message
			message.TargetId = NextShard
			Return message.TargetId
		End Function

		Protected Friend Overridable ReadOnly Property NextShard As Short
			Get
				Return CShort(Math.Truncate(RandomUtils.nextInt(0, numShards)))
			End Get
		End Property
	End Class

End Namespace