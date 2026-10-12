Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Shape = org.nd4j.linalg.api.shape.Shape

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
Namespace org.nd4j.linalg.api.ops.custom


	Public Class DivideNoNan
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal in1 As INDArray, ByVal in2 As INDArray)
			inputArguments_Conflict.Add(in1)
			inputArguments_Conflict.Add(in2)
		End Sub

		Public Sub New(ByVal in1 As INDArray, ByVal in2 As INDArray, ByVal [out] As INDArray)
			Me.New(in1,in2)
			outputArguments_Conflict.Add([out])
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal in1 As SDVariable, ByVal in2 As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){in1, in2})
		End Sub

		Public Overrides Function opName() As String
			Return "divide_no_nan"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "DivNoNan"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got input %s", Me.GetType(), dataTypes)

			Dim z As DataType = Shape.pickPairwiseDataType(dataTypes(0), dataTypes(1))
			Return Collections.singletonList(z)
		End Function
	End Class
End Namespace