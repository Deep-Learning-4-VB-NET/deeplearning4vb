Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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

Namespace org.nd4j.linalg.api.ops.impl.shape


	''' <summary>
	''' GatherND op
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class GatherNd extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class GatherNd
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal indices As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, indices})
		End Sub

		Public Sub New(ByVal df As INDArray, ByVal indices As INDArray)
			MyBase.New(New INDArray(){df, indices}, Nothing)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

		Public Overrides Function opName() As String
			Return "gather_nd"
		End Function

		Public Overrides Function onnxName() As String
			Return "GatherND"
		End Function


		Public Overrides Function tensorflowNames() As String()
			Return New String(){"GatherNd"}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'Output type is same as (first) input type
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace