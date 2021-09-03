Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ReduceOp = org.nd4j.linalg.api.ops.ReduceOp
Imports ScalarOp = org.nd4j.linalg.api.ops.ScalarOp
Imports OpDescriptor = org.nd4j.linalg.api.ops.grid.OpDescriptor

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

Namespace org.nd4j.linalg.api.ops.impl.meta


	Public Class ReduceMetaOp
		Inherits BaseMetaOp

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal opA As ScalarOp, ByVal opB As ReduceOp, ParamArray ByVal dimensions() As Integer)
			Me.New(New OpDescriptor(opA), New OpDescriptor(opB, dimensions))
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			MyBase.New(x, y)
		End Sub

		Public Sub New(ByVal opA As ScalarOp, ByVal opB As ReduceOp)
			MyBase.New(opA, opB)
		End Sub

		Public Sub New(ByVal opA As OpDescriptor, ByVal opB As OpDescriptor)
			MyBase.New(opA, opB)
		End Sub


		Public Overrides Function opNum() As Integer
			Return 4
		End Function

		Public Overrides Function opName() As String
			Return "meta_reduce"
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Nothing
		End Function
	End Class

End Namespace