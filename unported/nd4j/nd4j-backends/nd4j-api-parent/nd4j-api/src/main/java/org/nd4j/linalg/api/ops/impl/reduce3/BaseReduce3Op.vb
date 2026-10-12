Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseReduceFloatOp = org.nd4j.linalg.api.ops.BaseReduceFloatOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.ops.impl.reduce3


	Public MustInherit Class BaseReduce3Op
		Inherits BaseReduceFloatOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, DirectCast(Nothing, Integer()))
			If dimensions IsNot Nothing Then
				sameDiff.addArgsFor(New String(){dimensions.name()},Me)
			End If

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, DirectCast(Nothing, Integer()))
			If dimensions IsNot Nothing Then
				sameDiff.addArgsFor(New String(){dimensions.name()},Me)
			End If
		End Sub


		Public Sub New()
		End Sub


		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, False, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, Nothing, True, False, dimensions)
			Me.isComplex = allDistances
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Me.New(x, y, z, False, False, DirectCast(Nothing, Integer()))
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(x,y,z,keepDims, False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.isComplex = allDistances
			extraArgs = New Object(){0.0f, 0.0f}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, False, dimensions)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sd,x,y,dimensions)
			Me.keepDims_Conflict = keepDims
			Me.isComplex = isComplex
		End Sub

		Public Overrides Function opType() As Type
			Return Type.REDUCE3
		End Function

		Public Overrides ReadOnly Property OpType As Type
			Get
				Return opType()
			End Get
		End Property

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())

		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function resultType() As DataType
			If x_Conflict.dataType().isFPType() Then
				Return x_Conflict.dataType()
			End If
			Return Nd4j.defaultFloatingPointType()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'Second input is dynamic axis arg
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 2 OrElse dataTypes.Count = 3), "Expected 2 or 3 input datatype for %s, got input %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes.Count = 2 OrElse dataTypes(2).isIntType(), "When executing distance reductions" & "with 3 inputs, third input (axis) must be an integer datatype for %s, got %s", Me.GetType(), dataTypes)
			'Output data type: always float. TODO let's allow configuration...
			If dataTypes(0).isFPType() Then
				Return Collections.singletonList(dataTypes(0))
			End If
			Return Collections.singletonList(Nd4j.defaultFloatingPointType())
		End Function
	End Class

End Namespace