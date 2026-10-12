Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.messages.RequestMessage
Imports DistributedAssignMessage = org.nd4j.parameterserver.distributed.messages.intercom.DistributedAssignMessage

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class AssignRequestMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.RequestMessage
	<Obsolete, Serializable>
	Public Class AssignRequestMessage
		Inherits BaseVoidMessage
		Implements RequestMessage

		Protected Friend key As Integer?

		Protected Friend rowIdx As Integer

		' assign part
		Protected Friend payload As INDArray
		Protected Friend value As Number


		Protected Friend Sub New()
			MyBase.New(8)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AssignRequestMessage(@NonNull Integer key, @NonNull INDArray array)
		Public Sub New(ByVal key As Integer, ByVal array As INDArray)
			Me.New()
			Me.key = key
			Me.payload = If(array.isView(), array.dup(array.ordering()), array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AssignRequestMessage(@NonNull Integer key, @NonNull Number value, int rowIdx)
		Public Sub New(ByVal key As Integer, ByVal value As Number, ByVal rowIdx As Integer)
			Me.New()
			Me.key = key
			Me.value = value
			Me.rowIdx = rowIdx
		End Sub

		Public Overrides Sub processMessage()
			If payload Is Nothing Then
				Dim dam As New DistributedAssignMessage(key, rowIdx, value.doubleValue())
				dam.extractContext(Me)
				dam.processMessage()
				transport.sendMessageToAllShards(dam)
			Else
				Dim dam As New DistributedAssignMessage(key, payload)
				dam.extractContext(Me)
				dam.processMessage()
				transport.sendMessageToAllShards(dam)
			End If
		End Sub
	End Class

End Namespace