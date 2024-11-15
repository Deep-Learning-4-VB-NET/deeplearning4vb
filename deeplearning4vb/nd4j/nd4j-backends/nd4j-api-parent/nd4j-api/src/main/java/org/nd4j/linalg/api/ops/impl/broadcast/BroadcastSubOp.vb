﻿Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseBroadcastOp = org.nd4j.linalg.api.ops.BaseBroadcastOp

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

Namespace org.nd4j.linalg.api.ops.impl.broadcast


	Public Class BroadcastSubOp
		Inherits BaseBroadcastOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, y, z, dimension)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal dimension() As Integer)
			MyBase.New(sameDiff, i_v1, i_v2, dimension)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal inPlace As Boolean, ByVal dimension() As Integer)
			MyBase.New(sameDiff, i_v1, i_v2, inPlace, dimension)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff)
			MyBase.New(sameDiff)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal dimension() As Integer, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v1, i_v2, dimension, extraArgs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimension() As Integer, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, dimension, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal shape() As Long, ByVal inPlace As Boolean, ByVal dimension() As Integer, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v, shape, inPlace, dimension, extraArgs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimension() As Integer, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, i_v, dimension, extraArgs)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 1
		End Function

		Public Overrides Function opName() As String
			Return "broadcastsub"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Nothing
		End Function
	End Class

End Namespace