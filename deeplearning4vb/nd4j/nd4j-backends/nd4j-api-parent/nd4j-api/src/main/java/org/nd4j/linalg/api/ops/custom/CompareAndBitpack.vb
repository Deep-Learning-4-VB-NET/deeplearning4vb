Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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
Namespace org.nd4j.linalg.api.ops.custom


	Public Class CompareAndBitpack
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal threshold As Double)
			inputArguments_Conflict.Add([in])
			inputArguments_Conflict.Add(Nd4j.scalar(threshold))
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal threshold As Double, ByVal [out] As INDArray)
			Me.New([in], threshold)
			outputArguments_Conflict.Add([out])
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal threshold As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){threshold})
		End Sub

		Public Overrides Function opName() As String
			Return "compare_and_bitpack"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "CompareAndBitpack"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got input %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input data types must be the same: got %s", dataTypes)
			Return Collections.singletonList(DataType.UINT8)
		End Function
	End Class
End Namespace