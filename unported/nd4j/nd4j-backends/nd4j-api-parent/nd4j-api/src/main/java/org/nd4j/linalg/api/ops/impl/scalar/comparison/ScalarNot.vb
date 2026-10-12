Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseScalarBoolOp = org.nd4j.linalg.api.ops.BaseScalarBoolOp

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

Namespace org.nd4j.linalg.api.ops.impl.scalar.comparison


	''' <summary>
	''' Return a binary (0 or 1)
	''' when greater than a number
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class ScalarNot
		Inherits BaseScalarBoolOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal num As Number)
			MyBase.New(x, Nothing, z, num)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal num As Number)
			MyBase.New(x, num)
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number)
			MyBase.New(sameDiff, i_v, scalar)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, scalar, inPlace)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 10
		End Function

		Public Overrides Function opName() As String
			Return "not_scalar"
		End Function

		Public Overrides Function onnxName() As String
			Return "NotScalar"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'Not continuously differentiable, but 0 gradient in most places

			Return New List(Of SDVariable) From {sameDiff.zerosLike(arg())}
		End Function
	End Class

End Namespace