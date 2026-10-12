Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports DistributedMessage = org.nd4j.parameterserver.distributed.messages.DistributedMessage

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

Namespace org.nd4j.parameterserver.distributed.messages.intercom

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @Deprecated public class DistributedSolidMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.DistributedMessage
	<Obsolete, Serializable>
	Public Class DistributedSolidMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		''' <summary>
		''' The only use of this message is negTable sharing.
		''' </summary>

		Private key As Integer?
		Private payload As INDArray
		Private overwrite As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedSolidMessage(@NonNull Integer key, @NonNull INDArray array, boolean overwrite)
		Public Sub New(ByVal key As Integer, ByVal array As INDArray, ByVal overwrite As Boolean)
			MyBase.New(5)
			Me.payload = array
			Me.key = key
			Me.overwrite = overwrite
		End Sub

		''' <summary>
		''' This method will be started in context of executor, either Shard, Client or Backup node
		''' </summary>
		Public Overrides Sub processMessage()
			If overwrite Then
				storage.setArray(key, payload)
			ElseIf Not storage.arrayExists(key) Then
				storage.setArray(key, payload)
			End If
		End Sub
	End Class

End Namespace