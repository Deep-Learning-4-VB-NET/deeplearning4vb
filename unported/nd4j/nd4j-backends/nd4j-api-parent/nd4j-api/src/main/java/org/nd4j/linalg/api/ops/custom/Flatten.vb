Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
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

Namespace org.nd4j.linalg.api.ops.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class Flatten extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Flatten
		Inherits DynamicCustomOp

		Private order As Long

		Public Sub New(ByVal order As Char, ParamArray ByVal inputs() As INDArray)
			Me.order = AscW(order)

			For Each [in] As val In inputs
				inputArguments_Conflict.Add([in])
			Next [in]

			iArguments.Add(Convert.ToInt64(CInt(Me.order)))
		End Sub

		Public Sub New(ByVal output As INDArray, ParamArray ByVal inputs() As INDArray)
			Me.New(output.ordering(), inputs)

			outputArguments_Conflict.Add(output)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal order As Char, ParamArray ByVal inputs() As SDVariable)
			MyBase.New(sameDiff, inputs)
			Me.order = AscW(order)
			addIArgument(order)
		End Sub

		Public Overrides Function opName() As String
			Return "flatten"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return New List(Of DataType) From {inputDataTypes(0)}
		End Function
	End Class

End Namespace