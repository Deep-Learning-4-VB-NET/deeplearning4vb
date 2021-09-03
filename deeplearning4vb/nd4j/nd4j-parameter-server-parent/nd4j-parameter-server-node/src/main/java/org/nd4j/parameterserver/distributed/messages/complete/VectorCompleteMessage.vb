Imports System
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.parameterserver.distributed.messages.complete

	<Obsolete, Serializable>
	Public Class VectorCompleteMessage
		Inherits BaseCompleteMessage

		Protected Friend Sub New()
			MyBase.New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VectorCompleteMessage(long taskId, @NonNull INDArray vector)
		Public Sub New(ByVal taskId As Long, ByVal vector As INDArray)
			Me.New()
			Me.taskId = taskId
			Me.payload = If(vector.isView(), vector.dup(vector.ordering()), vector)
		End Sub
	End Class

End Namespace