Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.ops.util

	Public Class PrintVariable
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

		Public Sub New(ByVal array As INDArray, ByVal printSpecial As Boolean)
			inputArguments_Conflict.Add(array)
			bArguments.Add(printSpecial)
		End Sub

		Public Sub New(ByVal array As INDArray)
			Me.New(array, False)
		End Sub

		Public Sub New(ByVal array As INDArray, ByVal message As String, ByVal printSpecial As Boolean)
			Me.New(array, Nd4j.create(message), printSpecial)
		End Sub

		Public Sub New(ByVal array As INDArray, ByVal message As String)
			Me.New(array, Nd4j.create(message), False)
		End Sub

		Public Sub New(ByVal array As INDArray, ByVal message As INDArray, ByVal printSpecial As Boolean)
			Me.New(array, printSpecial)
			Preconditions.checkArgument(message.S, "Message argument should have String data type, but got [" & message.dataType() & "] instead")
			inputArguments_Conflict.Add(message)
		End Sub

		Public Sub New(ByVal array As INDArray, ByVal message As INDArray)
			Me.New(array, message, False)
		End Sub

		Public Overrides Function opName() As String
			Return "print_variable"
		End Function
	End Class

End Namespace