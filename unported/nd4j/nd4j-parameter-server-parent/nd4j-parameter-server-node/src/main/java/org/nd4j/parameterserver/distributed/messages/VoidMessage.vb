Imports System
Imports System.IO
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports ClassLoaderObjectInputStream = org.apache.commons.io.input.ClassLoaderObjectInputStream
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports org.nd4j.parameterserver.distributed.training
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

Namespace org.nd4j.parameterserver.distributed.messages


	<Obsolete>
	Public Interface VoidMessage

		Property TargetId As Short


		ReadOnly Property TaskId As Long

		ReadOnly Property MessageType As Integer

		Property OriginatorId As Long


		Function asBytes() As SByte()

		Function asUnsafeBuffer() As UnsafeBuffer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") static <T extends VoidMessage> T fromBytes(byte[] array)
		Shared Function fromBytes(Of T As VoidMessage)(ByVal array() As SByte) As T
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'			ClassLoader classloader = org.nd4j.common.config.ND4JClassLoading.getNd4jClassloader();

'JAVA TO VB CONVERTER WARNING: The following constructor is declared outside of its associated class:
'ORIGINAL LINE: try (java.io.ByteArrayInputStream bis = new java.io.ByteArrayInputStream(array); java.io.ObjectInputStream ois = new org.apache.commons.io.input.ClassLoaderObjectInputStream(classloader, bis))
			Sub New(ByVal bis As MemoryStream)
'JAVA TO VB CONVERTER WARNING: The following constructor is declared outside of its associated class:
'ORIGINAL LINE: return (T) ois.readObject();
				Sub New(ByVal  As )
'JAVA TO VB CONVERTER WARNING: The following constructor is declared outside of its associated class:
'ORIGINAL LINE: catch (Exception objectReadException)
			Sub New(ByVal objectReadException As Exception)
				throw Function RuntimeException(ByVal  As ) As New

		''' <summary>
		''' This method initializes message for further processing
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void attachContext(org.nd4j.parameterserver.distributed.conf.VoidConfiguration voidConfiguration, org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends TrainingMessage> trainer, org.nd4j.parameterserver.distributed.logic.completion.Clipboard clipboard, org.nd4j.parameterserver.distributed.transport.Transport transport, org.nd4j.parameterserver.distributed.logic.Storage storage, org.nd4j.parameterserver.distributed.enums.NodeRole role, short shardIndex);
		Sub attachContext(Of T1 As TrainingMessage)(ByVal voidConfiguration As VoidConfiguration, ByVal trainer As TrainingDriver(Of T1), ByVal clipboard As Clipboard, ByVal transport As Transport, ByVal storage As Storage, ByVal role As NodeRole, ByVal shardIndex As Short)

		Sub extractContext(ByVal message As BaseVoidMessage)

		''' <summary>
		''' This method will be started in context of executor, either Shard, Client or Backup node
		''' </summary>
		Sub processMessage()

		ReadOnly Property JoinSupported As Boolean

		ReadOnly Property BlockingMessage As Boolean

		Sub joinMessage(ByVal message As VoidMessage)

		ReadOnly Property RetransmitCount As Integer

		Sub incrementRetransmitCount()
	End Interface

End Namespace