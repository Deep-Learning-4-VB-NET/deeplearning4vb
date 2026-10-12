Imports System
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage

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

Namespace org.nd4j.parameterserver.distributed.v2.transport.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DelayedDummyTransport extends DummyTransport
	Public Class DelayedDummyTransport
		Inherits DummyTransport

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DelayedDummyTransport(@NonNull String id, @NonNull Connector connector)
		Public Sub New(ByVal id As String, ByVal connector As Connector)
			MyBase.New(id, connector)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DelayedDummyTransport(@NonNull String id, @NonNull Connector connector, @NonNull String rootId)
		Public Sub New(ByVal id As String, ByVal connector As Connector, ByVal rootId As String)
			MyBase.New(id, connector, rootId)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DelayedDummyTransport(@NonNull String id, @NonNull Connector connector, @NonNull String rootId, @NonNull VoidConfiguration configuration)
		Public Sub New(ByVal id As String, ByVal connector As Connector, ByVal rootId As String, ByVal configuration As VoidConfiguration)
			MyBase.New(id, connector, rootId, configuration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void sendMessage(@NonNull VoidMessage message, @NonNull String id)
		Public Overridable Overloads Sub sendMessage(ByVal message As VoidMessage, ByVal id As String)
			Dim bos As val = New MemoryStream()
			SyncLock Me
				SerializationUtils.serialize(message, bos)
			End SyncLock

			Dim bis As val = New MemoryStream(bos.toByteArray())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.parameterserver.distributed.v2.messages.VoidMessage msg = org.nd4j.common.util.SerializationUtils.deserialize(bis);
			Dim msg As VoidMessage = SerializationUtils.deserialize(bis)

			If msg.OriginatorId Is Nothing Then
				msg.OriginatorId = Me.id()
			End If


			'super.sendMessage(message, id);
			connector.executorService().submit(Sub()
			Try
				' imitate some bad network here, latency of 0.05ms - 0.2ms
				Dim sleepTime As val = RandomUtils.nextInt(50, 200) * 1000
				LockSupport.parkNanos(sleepTime)

				DelayedDummyTransport.this.sendMessage(msg, id)
			Catch e As Exception
				log.error("Got exception: ", e)
			End Try
			End Sub)

			'super.sendMessage(msg, id);
		End Sub
	End Class

End Namespace