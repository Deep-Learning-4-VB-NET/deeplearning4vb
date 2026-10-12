Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseScalarOp = org.nd4j.linalg.api.ops.BaseScalarOp

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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class LeakyReLUDerivative extends org.nd4j.linalg.api.ops.BaseScalarOp
	Public Class LeakyReLUDerivative
		Inherits BaseScalarOp

		Private alpha As Double = 0.01

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal alpha As Double)
			MyBase.New(sameDiff, i_v, alpha, inPlace)
			Me.alpha = alpha
			Me.extraArgs = New Object() {alpha}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal alpha As Double)
			Me.New(sameDiff, i_v, False, alpha)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object, ByVal alpha As Double)
			MyBase.New(sameDiff, i_v, alpha, extraArgs)
			Me.alpha = alpha
			Me.extraArgs = New Object() {alpha}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			Me.New(x, z, 0.01)
		End Sub

		Public Sub New(ByVal x As INDArray)
			Me.New(x,x,0.01)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal alpha As Double)
			MyBase.New(x, Nothing, z, alpha)
			Me.alpha = alpha
			Me.extraArgs = New Object() {alpha}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal alpha As Double)
			MyBase.New(x, alpha)
			Me.alpha = alpha
			Me.extraArgs = New Object() {alpha}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 36
		End Function

		Public Overrides Function opName() As String
			Return "leakyreluderivative"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function
	End Class

End Namespace