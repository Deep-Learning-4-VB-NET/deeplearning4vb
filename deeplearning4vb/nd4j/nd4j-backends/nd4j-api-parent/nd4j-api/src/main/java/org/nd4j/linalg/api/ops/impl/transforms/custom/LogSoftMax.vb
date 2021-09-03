Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LogSoftMaxDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.LogSoftMaxDerivative
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


	Public Class LogSoftMax
		Inherits DynamicCustomOp

		Private dimension As Integer? = Nothing

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, i_v)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(Nothing, x, z, Nothing, Nothing)
		End Sub

		Public Sub New(ByVal x As INDArray)
			Me.New(x, x)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal dimension As Integer)
			Me.New(x, Nothing)
			Me.dimension = dimension
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimension As Integer)
			Me.New(sameDiff, i_v)
			Me.dimension = dimension
			addIArgument(dimension)
		End Sub


		Public Overrides Function opName() As String
			Return "log_softmax"
		End Function
		Public Overrides Function tensorflowName() As String
			Return "LogSoftmax"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			If dimension Is Nothing Then
				Return (New LogSoftMaxDerivative(sameDiff, arg(), i_v(0))).outputs()
			Else
				Return (New LogSoftMaxDerivative(sameDiff, arg(), i_v(0), dimension)).outputs()
			End If
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inTypes IsNot Nothing AndAlso inTypes.Count = 1, "Expected 1 input datatype for %s, got %s", Me.GetType(), inTypes)
			If inTypes(0).isFPType() Then
				Return Collections.singletonList(inTypes(0))
			End If
			Return Collections.singletonList(Nd4j.defaultFloatingPointType())
		End Function
	End Class

End Namespace