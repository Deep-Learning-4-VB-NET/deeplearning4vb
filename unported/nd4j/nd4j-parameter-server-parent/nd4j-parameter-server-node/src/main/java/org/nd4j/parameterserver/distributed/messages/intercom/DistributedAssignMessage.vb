Imports System
Imports lombok
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
'ORIGINAL LINE: @Data @Deprecated public class DistributedAssignMessage extends org.nd4j.parameterserver.distributed.messages.BaseVoidMessage implements org.nd4j.parameterserver.distributed.messages.DistributedMessage
	<Obsolete, Serializable>
	Public Class DistributedAssignMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		''' <summary>
		''' The only use of this message is negTable sharing.
		''' </summary>
		Private index As Integer
		Private value As Double
		Private key As Integer?
		Private payload As INDArray

		Protected Friend Sub New()
			MyBase.New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedAssignMessage(@NonNull Integer key, int index, double value)
		Public Sub New(ByVal key As Integer, ByVal index As Integer, ByVal value As Double)
			MyBase.New(6)
			Me.index = index
			Me.value = value
			Me.key = key
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedAssignMessage(@NonNull Integer key, org.nd4j.linalg.api.ndarray.INDArray payload)
		Public Sub New(ByVal key As Integer, ByVal payload As INDArray)
			MyBase.New(6)
			Me.key = key
			Me.payload = payload
		End Sub

		''' <summary>
		''' This method assigns specific value to either specific row, or whole array.
		''' Array is identified by key
		''' </summary>
		Public Overrides Sub processMessage()
			If payload IsNot Nothing Then
				' we're assigning array
				If storage.arrayExists(key) AndAlso storage.getArray(key).length() = payload.length() Then
					storage.getArray(key).assign(payload)
				Else
					storage.setArray(key, payload)
				End If
			Else
				' we're assigning number to row
				If index >= 0 Then
					If storage.getArray(key) Is Nothing Then
						Throw New Exception("Init wasn't called before for key [" & key & "]")
					End If
					storage.getArray(key).getRow(index).assign(value)
				Else
					storage.getArray(key).assign(value)
				End If
			End If
		End Sub
	End Class

End Namespace