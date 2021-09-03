Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils

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

Namespace org.nd4j.parameterserver.distributed.v2.messages


	Public Interface VoidMessage
		''' <summary>
		''' This method returns unique messageId
		''' @return
		''' </summary>
		ReadOnly Property MessageId As String

		''' <summary>
		''' This message allows to set messageId
		''' </summary>
		'void setMessageId();

		''' <summary>
		''' This method returns Id of originator
		''' @return
		''' </summary>
		Property OriginatorId As String


		''' <summary>
		''' This method serializes this VoidMessage into UnsafeBuffer
		''' 
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java default interface methods:
'		default org.agrona.concurrent.UnsafeBuffer asUnsafeBuffer()
	'	{
	'		Return New UnsafeBuffer(SerializationUtils.toByteArray(Me));
	'	}

'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java static interface methods:
'		static VoidMessage fromBytes(byte[] bytes)
	'	{
	'		Return SerializationUtils.readObject(New ByteArrayInputStream(bytes));
	'	}
	End Interface

End Namespace