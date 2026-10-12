Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.gradient



	Public Class TanhDerivative
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){i_v1, i_v2})
		End Sub

		''' 
		''' <param name="x"> Input </param>
		''' <param name="y"> Gradient at output (dL/dOut) </param>
		''' <param name="z"> Output array, gradient at input (dL/dIn - to be calculated) </param>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			MyBase.New(Nothing, New INDArray(){x, y}, New INDArray(){z})
		End Sub

		Public Sub New()
		End Sub

		''' <param name="x"> Input </param>
		''' <param name="y"> Gradient at output (dL/dOut) </param>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			Me.New(x, y, Nothing)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 0
		End Function

		''' <summary>
		''' The opName of this operation
		''' </summary>
		''' <returns> the opName of this operation </returns>
		Public Overrides Function opName() As String
			Return "tanh_bp"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable = sameDiff.math_Conflict.div(sameDiff.onesLike(outputVariables()(0)), sameDiff.math_Conflict.pow(sameDiff.math_Conflict.cosh(arg()), 2))
			Return Collections.singletonList(ret)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace