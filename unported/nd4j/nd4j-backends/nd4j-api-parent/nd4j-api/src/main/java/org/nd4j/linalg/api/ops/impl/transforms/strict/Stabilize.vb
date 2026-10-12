Imports System.Collections.Generic
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformStrictOp = org.nd4j.linalg.api.ops.BaseTransformStrictOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.strict


	Public Class Stabilize
		Inherits BaseTransformStrictOp

		Friend realMin As Double = 1.1755e-38f
		Friend cutOff As Double = FastMath.log(realMin)
		Friend k As Double

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal realMin As Double, ByVal cutOff As Double, ByVal k As Double)
			MyBase.New(sameDiff, i_v, inPlace)
			Me.realMin = realMin
			Me.cutOff = cutOff
			Me.k = k
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal k As Double)
			MyBase.New(x, z)
			Me.k = k
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal k As Double)
			MyBase.New(x)
			Me.k = k
		End Sub

		Public Overrides Function opNum() As Integer
			Return 19
		End Function

		Public Overrides Function opName() As String
			Return "stabilize"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Nothing
		End Function
	End Class

End Namespace