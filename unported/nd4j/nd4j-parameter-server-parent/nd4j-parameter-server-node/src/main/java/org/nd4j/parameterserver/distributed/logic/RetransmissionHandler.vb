Imports System
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
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

Namespace org.nd4j.parameterserver.distributed.logic

	<Obsolete>
	Public Interface RetransmissionHandler
		Public Enum TransmissionStatus
			MESSAGE_SENT
			NOT_CONNECTED
			BACKPRESSURE
			ADMIN_ACTION
		End Enum

		Sub init(ByVal configuration As VoidConfiguration, ByVal transport As Transport)

		Sub handleMessage(ByVal message As TrainingMessage)

		Sub onBackPressure()

'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java static interface methods:
'		static TransmissionStatus getTransmissionStatus(long resp)
	'	{
	'		if (resp >= 0)
	'		{
	'			Return TransmissionStatus.MESSAGE_SENT;
	'		}
	'		else if (resp == -1)
	'		{
	'			Return TransmissionStatus.NOT_CONNECTED;
	'		}
	'		else if (resp == -2)
	'		{
	'			Return TransmissionStatus.BACKPRESSURE;
	'		}
	'		else if (resp == -3)
	'		{
	'			Return TransmissionStatus.ADMIN_ACTION;
	'		}
	'		else
	'		{
	'			throw New ND4JIllegalStateException("Unknown response from Aeron received: [" + resp + "]");
	'		}
	'	}


	End Interface

End Namespace