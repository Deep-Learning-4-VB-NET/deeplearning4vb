﻿Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformStrictOp = org.nd4j.linalg.api.ops.BaseTransformStrictOp
Imports SigmoidDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.SigmoidDerivative

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class LogSigmoid extends org.nd4j.linalg.api.ops.BaseTransformStrictOp
	Public Class LogSigmoid
		Inherits BaseTransformStrictOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			Me.New(sameDiff, i_v, False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(x, z)
		End Sub

		Public Sub New(ByVal ndArray As INDArray)
			MyBase.New(ndArray)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 49
		End Function

		Public Overrides Function opName() As String
			Return "logsigmoid"
		End Function

		Public Overrides Function onnxName() As String
			Return "LogSigmoid"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "LogSigmoid"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim v As SDVariable = (New SigmoidDerivative(sameDiff, arg(), i_v(0))).outputVariable().div(sameDiff.nn_Conflict.sigmoid(arg()))
			Return Collections.singletonList(v)
		End Function


	End Class

End Namespace