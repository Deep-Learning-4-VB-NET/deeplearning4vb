Imports System
Imports System.Threading
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.messages.RequestMessage
Imports DistributedShutdownMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedShutdownMessage

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

	<Obsolete, Serializable>
	Public Class ShutdownRequestMessage
		Inherits BaseVoidMessage
		Implements RequestMessage

		Public Sub New()
			MyBase.New(8)
		End Sub

		Public Overrides Sub processMessage()
			Dim dsm As New DistributedShutdownMessage()
			transport.sendMessage(dsm)

			Try
				Thread.Sleep(1000)
			Catch e As Exception
			End Try

			transport.shutdown()
			storage.shutdown()
		End Sub
	End Class

End Namespace