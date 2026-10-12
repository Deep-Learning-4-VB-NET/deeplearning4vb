Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseScalarOp = org.nd4j.linalg.api.ops.BaseScalarOp
Imports ThresholdReluBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.ThresholdReluBp

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

Namespace org.nd4j.linalg.api.ops.impl.scalar


	Public Class RectifiedLinear
		Inherits BaseScalarOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal cutoff As Double)
			MyBase.New(sameDiff, i_v, cutoff, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal cutoff As Double)
			Me.New(sameDiff, i_v, False, cutoff)
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal cutoff As Double)
			MyBase.New(x, Nothing, z, cutoff)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal cutoff As Double)
			MyBase.New(x, cutoff)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			Me.New(x, z, 0.0f)
		End Sub

		Public Sub New(ByVal x As INDArray)
			Me.New(x, 0.0f)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 39
		End Function

		Public Overrides Function opName() As String
			Return "relu"
		End Function

		Public Overrides Function onnxName() As String
			Return "Relu"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Relu"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New ThresholdReluBp(sameDiff, arg(), i_v(0), scalarValue.getDouble(0))).outputs()
		End Function
	End Class

End Namespace