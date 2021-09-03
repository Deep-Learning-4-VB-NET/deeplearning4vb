﻿Imports System.Collections.Generic
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

Namespace org.nd4j.linalg.api.ops.impl.scalar


	Public Class ReplaceNans
		Inherits BaseScalarOp

		Private set As Double

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal set As Double)
			MyBase.New(sameDiff, i_v, set, inPlace)
			Me.set = set
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal extraArgs() As Object, ByVal set As Double)
			MyBase.New(sameDiff, i_v, set, extraArgs)
			Me.set = set
		End Sub

		Public Sub New()

		End Sub

		Public Sub New(ByVal x As INDArray, ByVal set As Double)
			MyBase.New(x, Nothing, x, set)
			Me.set = set
			Me.extraArgs = New Object() {set}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal set As Double)
			MyBase.New(x, z, set)
			Me.set = set
			Me.extraArgs = New Object() {set}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 37
		End Function

		Public Overrides Function opName() As String
			Return "replace_nans"
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