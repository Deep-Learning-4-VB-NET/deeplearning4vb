Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports GRUWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.GRUWeights

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent



	Public Class GRUCell
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.GRUWeights weights;
		Private weights As GRUWeights

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal hLast As SDVariable, ByVal weights As GRUWeights)
			MyBase.New(Nothing, sameDiff, weights.argsWithInputs(x, hLast))
			Me.weights = weights
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal hLast As INDArray, ByVal gruWeights As GRUWeights)
			MyBase.New(Nothing, Nothing, gruWeights.argsWithInputs(x, hLast))
			Me.weights = gruWeights
		End Sub


		Public Overrides Function opName() As String
			Return "gruCell"
		End Function


		Public Overrides Function onnxName() As String
			Return "GRU"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "GRUBlockCell"
		End Function

		Public Overrides Function onnxNames() As String()
			Return MyBase.onnxNames()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 6, "Expected exactly 6 inputs to GRUCell, got %s", inputDataTypes)
			'4 outputs, all of same type as input
			Dim dt As DataType = inputDataTypes(0)
			Preconditions.checkState(dt.isFPType(), "Input type 0 must be a floating point type, got %s", dt)
			Return New List(Of DataType) From {dt, dt, dt, dt}
		End Function

		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not yet implemented")
		End Function
	End Class

End Namespace