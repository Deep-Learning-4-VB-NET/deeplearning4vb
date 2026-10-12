Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.nd4j.linalg.api.ops.impl.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public class TensorMmulBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class TensorMmulBp
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal samediff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal gradAtOutput As SDVariable, ByVal axes()() As Integer)
			Me.New(samediff, x, y, gradAtOutput, axes(0), axes(1))
		End Sub

		Public Sub New(ByVal samediff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal gradAtOutput As SDVariable, ByVal axesX() As Integer, ByVal axesY() As Integer)
			MyBase.New(Nothing, samediff, New SDVariable(){x, y, gradAtOutput})
			Dim axes()() As Integer = {axesX, axesY}
			addIArgument(axesX.Length)
			addIArgument(axesX)
			addIArgument(axesY.Length)
			addIArgument(axesY)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal axes()() As Integer)
			Me.New(x, y, gradAtOutput, axes(0), axes(1))
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal axesX() As Integer, ByVal axesY() As Integer)
			MyBase.New(Nothing,New INDArray(){x, y, gradAtOutput},Nothing)
			Dim axes()() As Integer = {axesX, axesY}
			addIArgument(axesX.Length)
			addIArgument(axesX)
			addIArgument(axesY.Length)
			addIArgument(axesY)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal dldx As INDArray, ByVal dldy As INDArray, ByVal axes()() As Integer)
			Me.New(x, y, gradAtOutput, dldx, dldy, axes(0), axes(1))
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal gradAtOutput As INDArray, ByVal dldx As INDArray, ByVal dldy As INDArray, ByVal axesX() As Integer, ByVal axesY() As Integer)
				MyBase.New(Nothing, New INDArray(){x, y, gradAtOutput}, New INDArray(){dldx, dldy})
				Dim axes()() As Integer = {axesX, axesY}
				addIArgument(axesX.Length)
				addIArgument(axesX)
				addIArgument(axesY.Length)
				addIArgument(axesY)
		End Sub

		Public Overrides Function opName() As String
			Return "tensormmul_bp"
		End Function

		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation of " & Me.GetType().FullName & " not supported")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 inputs to tensormmul_bp op, got %s", dataTypes)
			Preconditions.checkState(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType() AndAlso dataTypes(0).isFPType(), "Inputs to tensormmul_bp op must both be a floating" & "point type: got %s", dataTypes)
			Return dataTypes.subList(0, 2)
		End Function

	End Class

End Namespace