Imports System
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports RetransmissionHandler = org.nd4j.parameterserver.distributed.logic.RetransmissionHandler
Imports TrainingMessage = org.nd4j.parameterserver.distributed.messages.TrainingMessage
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

Namespace org.nd4j.parameterserver.distributed.logic.retransmission

	<Obsolete>
	Public Class DefaultRetransmissionHandler
		Implements RetransmissionHandler

		Private configuration As VoidConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration configuration, org.nd4j.parameterserver.distributed.transport.Transport transport)
		Public Overridable Sub init(ByVal configuration As VoidConfiguration, ByVal transport As Transport)
			Me.configuration = configuration
		End Sub

		Public Overridable Sub onBackPressure() Implements RetransmissionHandler.onBackPressure
			Try
				Thread.Sleep(2000)
			Catch e As Exception
			End Try
		End Sub

		Public Overridable Sub handleMessage(ByVal message As TrainingMessage) Implements RetransmissionHandler.handleMessage

		End Sub
	End Class

End Namespace